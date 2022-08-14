using RegulatorApplication.Library.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace RegulatorApplication.Library.Interfaces
{
    public class Regulator : IRegulator, IDisposable
    {
        private decimal _load;
        private decimal _estimatedLoad;
        //Added lock for accurate concurrent work
        private object _forLock = new object();
        //Timer will stop and verify if commited requests are out of date 
        private Timer _timer;
        private int _verificationTime;
        //All needed for regulator settings
        private IRegulatorSettings _regulatorSettings;
        //Use dictionary for fast searching of elements and it contains Count property,
        //so no need to use enumeration to search and get count of elements 
        private IDictionary<Request, DateTime> _uncommitedRequests;
        private IDictionary<Request, DateTime> _commitedRequests;
        //Logs of regulator's work
        private ICollection<string> _logMessages;

        private decimal Load
        {
            get => _load;
            set
            {
                if (_load == value) 
                    return;
                _load = value;
                LoadChanged();
            }
        }
        public EventHandler<decimal> OnLoadChanged { get; set; }

        public Regulator(IRegulatorSettings regulatorSettings)
        {
            _regulatorSettings = regulatorSettings;
            _uncommitedRequests = new ConcurrentDictionary<Request, DateTime>();
            _commitedRequests = new ConcurrentDictionary<Request, DateTime>();
            _logMessages = new List<string>();
            //setting interval of requests' verification equal to 1/10 of length of regulator's interval
            _verificationTime = _regulatorSettings.Interval / 10;
            _timer = new Timer(_verificationTime);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        public void Commit()
        {
            lock (_forLock)
            {
                var isLoadUpdated = false;
                var requests = _uncommitedRequests.Keys.Select(x => x).ToList();
                foreach (var request in requests)
                {
                    if(_commitedRequests.Count < _regulatorSettings.Budget)
                    {
                        if (!_commitedRequests.ContainsKey(request))
                        {
                            isLoadUpdated = true;
                            _commitedRequests.Add(request, DateTime.Now);
                            _uncommitedRequests.Remove(request);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if(isLoadUpdated)
                {
                    Load = (decimal)_commitedRequests.Count / _regulatorSettings.Budget;
                    _estimatedLoad = (decimal)(_uncommitedRequests.Count + _commitedRequests.Count) / _regulatorSettings.Budget;
                    SetLogMessage($"Commit - GetLoad:{_load}; GetEstimatedLoad:{_estimatedLoad}");
                }
            }
        }

        public decimal GetEstimatedLoad()
        {
            return _estimatedLoad;
        }

        public decimal GetLoad()
        {
            return Load;
        }

        public void Ignore(Request request)
        {
            lock (_forLock)
            {
                if (_uncommitedRequests.ContainsKey(request))
                {
                    _uncommitedRequests.Remove(request);
                    _estimatedLoad = (decimal)(_uncommitedRequests.Count + _commitedRequests.Count) / _regulatorSettings.Budget;
                }
                else if(_commitedRequests.ContainsKey(request))
                {
                    _commitedRequests.Remove(request);
                    Load = (decimal)_commitedRequests.Count / _regulatorSettings.Budget;
                }
            }

            SetLogMessage($"Ignore - GetLoad:{Load}; GetEstimatedLoad:{_estimatedLoad}");
        }

        public void Propose(Request request)
        {
            lock (_forLock)
            {
                if (!_uncommitedRequests.ContainsKey(request))
                {
                    //saving current date to dictionary to verify it later
                    _uncommitedRequests.Add(request, DateTime.Now);
                    _estimatedLoad = (decimal)(_uncommitedRequests.Count + _commitedRequests.Count) / _regulatorSettings.Budget;
                }
            }

            SetLogMessage($"Propose - GetLoad:{Load}; GetEstimatedLoad:{_estimatedLoad}");
        }

        public void Rollback()
        {
            lock (_forLock)
            {
                _uncommitedRequests = new Dictionary<Request, DateTime>();
                _estimatedLoad = Load;
            }
            SetLogMessage("Rollback");
        }

        public List<string> GetLogMessages()
        {
            lock (_logMessages)
            {
                return _logMessages.Select(x => x).ToList();
            }
        }

        public void Dispose()
        {
            _uncommitedRequests = new Dictionary<Request, DateTime>();
            _commitedRequests = new Dictionary<Request, DateTime>();
            _timer.Elapsed -= OnTimerElapsed;
            _timer.Stop();
            foreach (Delegate d in OnLoadChanged.GetInvocationList())
            {
                OnLoadChanged -= (EventHandler<decimal>)d;
            }
        }

        private void SetLogMessage(string text)
        {
            lock (_logMessages)
            {
                _logMessages.Add($"{DateTime.Now:yyyy.MM.dd HH:mm:ss.fff} {text}");
            }
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            lock (_forLock)
            {
                if(_commitedRequests.Count == 0)
                {
                    return;
                }

                var requests = _commitedRequests.Keys.Select(x => x).ToList();
                var isLoadUpdated = false;
                foreach (var request in requests)
                {
                    //if request is out of date, than remove it
                    var timeDifference = (DateTime.Now - _commitedRequests[request]).TotalMilliseconds;
                    if (timeDifference > _regulatorSettings.Interval)
                    {
                        isLoadUpdated = true;
                        _commitedRequests.Remove(request);
                    }
                }

                if (isLoadUpdated)
                {
                    Load = (decimal)_commitedRequests.Count / _regulatorSettings.Budget;
                    _estimatedLoad = (decimal)(_uncommitedRequests.Count + _commitedRequests.Count) / _regulatorSettings.Budget;
                    SetLogMessage($"Time elapsed - GetLoad:{_load}; GetEstimatedLoad:{_estimatedLoad}");
                }
            }
        }

        private void LoadChanged()
        {
            OnLoadChanged?.Invoke(this, _load);
        }
    }
}