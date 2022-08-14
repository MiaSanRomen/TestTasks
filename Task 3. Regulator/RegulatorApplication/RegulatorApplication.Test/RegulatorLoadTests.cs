namespace RegulatorApplication.Test
{
    public class RegulatorLoadTests
    {
        private readonly int _budget = 10;
        private readonly int _interval = 1000;

        [Test]
        public void T01_RegulatorGetLoadBeforeProposes()
        {
            decimal expectedValue = 0;

            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));

            Assert.AreEqual(expectedValue, regulator.GetLoad());
        }

        [Test]
        public void T02_RegulatorGetLoadAfterProposes()
        {
            decimal expectedValue = 0;

            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));
            regulator.Propose(new Request());

            Assert.AreEqual(expectedValue, regulator.GetLoad());
        }

        [Test]
        public void T03_RegulatorGetLoadAfterProposesAndCommit()
        {
            decimal expectedValue = 0.2M;

            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));
            regulator.Propose(new Request());
            regulator.Propose(new Request());    
            regulator.Commit();

            Assert.AreEqual(expectedValue, regulator.GetLoad());
        }

        [Test]
        public void T04_RegulatorGetLoadAfterCommitAndIgnore()
        {
            decimal expectedValue = 0.2M;

            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));
            regulator.Propose(new Request());
            regulator.Propose(new Request());
            Request request = new Request();
            regulator.Propose(request);
            regulator.Commit();
            regulator.Ignore(request);

            Assert.AreEqual(expectedValue, regulator.GetLoad());
        }

        [Test]
        public void T05_RegulatorGetLoadAfterCommitAndRollback()
        {
            decimal expectedValue = 0.2M;

            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));
            regulator.Propose(new Request());
            regulator.Propose(new Request());
            regulator.Commit();
            regulator.Rollback();

            Assert.AreEqual(expectedValue, regulator.GetEstimatedLoad());
        }

        [Test]
        public async Task T06_RegulatorGetLoadAfterCommitAndWaiting()
        {
            decimal expectedValue = 0;
            int timeToWait = 1200;

            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));
            regulator.Propose(new Request());
            regulator.Commit();
            await Task.Delay(timeToWait);

            Assert.AreEqual(expectedValue, regulator.GetLoad());
        }

        [Test]
        public async Task T07_RegulatorGetLoadAfterFewCommitsAndWaiting()
        {
            decimal expectedValue = 0.1M;
            int timeToWait = 600;

            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));
            regulator.Propose(new Request());
            regulator.Commit();
            await Task.Delay(timeToWait);
            regulator.Propose(new Request());
            regulator.Commit();
            await Task.Delay(timeToWait);

            Assert.AreEqual(expectedValue, regulator.GetLoad());
        }

        [Test]
        public async Task T08_RegulatorVerifyLoadMaxValue()
        {
            decimal expectedValue = 1M;
            int budget = 3;

            var regulator = new Regulator(new RegulatorSettings(budget, _interval));
            regulator.Propose(new Request());
            regulator.Propose(new Request());
            regulator.Commit();
            regulator.Propose(new Request());
            regulator.Propose(new Request());
            regulator.Commit();

            Assert.AreEqual(expectedValue, regulator.GetLoad());
        }

        [Test]
        public async Task T09_RegulatorGetLoadAfterProposesConcurrently()
        {
            decimal expectedValue = 0M;
            int timeToWait = 1200;

            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));

            await Task.Factory.StartNew(() => regulator.Propose(new Request()));
            await Task.Factory.StartNew(() => regulator.Propose(new Request()));
            await Task.Factory.StartNew(() => regulator.Propose(new Request()));
            regulator.Commit();
            await Task.Delay(timeToWait);

            Assert.AreEqual(expectedValue, regulator.GetLoad());
        }

        [Test]
        public async Task T10_RegulatorGetLoadAfterIgnoresConcurrently()
        {
            decimal expectedValue = 0.1M;
            int timeToWait = 1200;

            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));
            var requestToTestFirst = new Request();
            var requestToTestSecond = new Request();
            regulator.Propose(requestToTestFirst);
            regulator.Propose(requestToTestSecond);
            regulator.Propose(new Request());
            regulator.Commit();
            await Task.Factory.StartNew(() => regulator.Ignore(requestToTestFirst));
            await Task.Factory.StartNew(() => regulator.Ignore(requestToTestSecond));

            Assert.AreEqual(expectedValue, regulator.GetLoad());
        }
    }
}