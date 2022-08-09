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
                "5 - Current load: 0.3",
                "5 - Current load: 0.3",
                "7 - Current load: 0.2",
            };

            var regulator = new Regulator(_budget, _interval);
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

            Assert.AreEqual(expectedMessages, regulator.LogMessages);
        }
    }
}
