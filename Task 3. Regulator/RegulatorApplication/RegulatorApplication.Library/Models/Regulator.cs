using RegulatorApplication.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegulatorApplication.Library.Interfaces
{
    public class Regulator : IRegulator
    {
        private decimal _load;
        private decimal _estimatedLoad;
        private int _budget;
        private int _interval;
        private ICollection<Request> _currentRequests;
        private ICollection<Request> _commitedRequests;

        //Property for OnLoadChanged event invoking
        private decimal Load
        {
            get
            {
                return _load;
            }
            set
            {
                _load = value;
                OnLoadChanged?.Invoke(this, GetLoad());
            }
        }
        //List of messages to validate OnLoadChanged event properly work
        public ICollection<string> LogMessages { get; private set; }
        public EventHandler<decimal> OnLoadChanged { get; set; }

        public Regulator(int budget, int interval)
        {
            _budget = budget;
            _interval = interval;
            _currentRequests = new List<Request>();
            _commitedRequests = new List<Request>();
            LogMessages = new List<string>();
            OnLoadChanged = (object? sender, decimal load) =>
            {
                var index = LogMessages.Count() + 1;
                string logMessage = $"{index} - Current load: {load}";
                LogMessages.Add(logMessage);
            };
        }

        public void Commit()
        {
            foreach(var request in _currentRequests)
            {
                if(!_commitedRequests.Contains(request) && _commitedRequests.Count() < _budget)
                {
                    _commitedRequests.Add(request);
                    request.Processed += OnRequestProcessed;
                    request.Commit(_interval);
                }
            }

            UpdateLoad();
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
            if (request != null && _currentRequests.Contains(request))
            {
                _currentRequests.Remove(request);
                _commitedRequests.Remove(request);
                UpdateEstimatedLoad();
                UpdateLoad();
            }
        }

        public void Propose(Request request)
        {
            _currentRequests.Add(request);
            UpdateEstimatedLoad();
        }

        public void Rollback()
        {
            var requestsToRemove = new List<Request>(_currentRequests);
            foreach (var request in requestsToRemove)
            {
                if (!_commitedRequests.Contains(request))
                {
                    _currentRequests.Remove(request);
                }
            }

            UpdateEstimatedLoad();
        }

        private void OnRequestProcessed(object? sender, EventArgs e)
        {
            var request = (Request)sender;
            _commitedRequests.Remove(request);
            _currentRequests.Remove(request);
            UpdateEstimatedLoad();
            UpdateLoad();
        }

        private void UpdateEstimatedLoad()
        {
            _estimatedLoad = Convert.ToDecimal(_currentRequests.Count()) / Convert.ToDecimal(_budget);
        }

        private void UpdateLoad()
        {
            Load = Convert.ToDecimal(_commitedRequests.Count()) / Convert.ToDecimal(_budget);
        }
    }
}
