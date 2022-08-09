using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegulatorApplication.Library.Models
{
    public class Request
    {
        //event occured, when request is commited
        public event EventHandler<int> Commited;
        //event occured, when request is processed
        public event EventHandler Processed;

        public Request()
        {
            Commited += OnRequestCommited;
        }
        public void Commit(int interval)
        {
            Commited?.Invoke(this, interval);
        }

        private async void OnRequestCommited(object? sender, int e)
        {
            await ProcessAsync(e);
        }

        private async Task ProcessAsync(int interval)
        {
            //processing simulation
            await Task.Delay(interval);
            Processed?.Invoke(this, new EventArgs());
        }
    }
}
