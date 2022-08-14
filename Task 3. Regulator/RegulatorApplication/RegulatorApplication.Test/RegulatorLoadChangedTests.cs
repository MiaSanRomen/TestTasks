using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegulatorApplication.Test
{
    public class RegulatorLoadChangedTests
    {
        private readonly int _budget = 10;
        private readonly int _interval = 1000;
        ICollection<string> actualMessages = new List<string>();

        [Test]
        public async Task T01_RegulatorLoadChangedEventVerifyProperBehavior()
        {
            int timeToWait = 600;
            List<string> expectedMessages = new List<string>
            {
                "1 - Current load: 0.2",
                "2 - Current load: 0.3",
                "3 - Current load: 0.2",
                "4 - Current load: 0.4",
                "5 - Current load: 0.2",
            };
            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));
            regulator.OnLoadChanged += UpdateActualMessages;

            regulator.Propose(new Request());
            regulator.Propose(new Request());
            regulator.Commit();
            Request request = new Request();
            regulator.Propose(request);
            regulator.Commit();
            regulator.Ignore(request);
            await Task.Delay(timeToWait);
            regulator.Propose(new Request());
            regulator.Propose(new Request());
            regulator.Commit();
            await Task.Delay(timeToWait);

            Assert.AreEqual(expectedMessages, actualMessages);
        }

        private void UpdateActualMessages(object? sender, decimal load)
        {
            var index = actualMessages.Count() + 1;
            string logMessage = $"{index} - Current load: {load}";
            actualMessages.Add(logMessage);
        }
}
}
