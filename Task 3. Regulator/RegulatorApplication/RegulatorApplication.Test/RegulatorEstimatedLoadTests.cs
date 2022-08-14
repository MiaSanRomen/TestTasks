namespace RegulatorApplication.Test
{
    public class RegulatorEstimatedLoadTests
    {
        private readonly int _budget = 10;
        private readonly int _interval = 1000;

        [Test]
        public void T01_RegulatorGetEstimatedLoadBeforeProposes()
        {
            decimal expectedValue = 0;

            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));

            Assert.AreEqual(expectedValue, regulator.GetEstimatedLoad());
        }

        [Test]
        public void T02_RegulatorGetEstimatedLoadAfterProposes()
        {
            decimal expectedValue = 0.1M;
            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));

            regulator.Propose(new Request());

            Assert.AreEqual(expectedValue, regulator.GetEstimatedLoad());
        }

        [Test]
        public void T03_RegulatorGetEstimatedLoadAfterProposesAndIgnore()
        {
            decimal expectedValue = 0.2M;
            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));
            Request request = new Request();

            regulator.Propose(new Request());
            regulator.Propose(new Request());
            regulator.Propose(request);
            regulator.Ignore(request);

            Assert.AreEqual(expectedValue, regulator.GetEstimatedLoad());
        }

        [Test]
        public void T04_RegulatorGetEstimatedLoadAfterProposesAndCommit()
        {
            decimal expectedValue = 0.2M;
            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));

            regulator.Propose(new Request());
            regulator.Propose(new Request());
            regulator.Commit();

            Assert.AreEqual(expectedValue, regulator.GetEstimatedLoad());
        }

        [Test]
        public void T05_RegulatorGetEstimatedLoadAfterProposesAndRollback()
        {
            decimal expectedValue = 0;
            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));

            regulator.Propose(new Request());
            regulator.Propose(new Request());
            regulator.Rollback();

            Assert.AreEqual(expectedValue, regulator.GetEstimatedLoad());
        }

        [Test]
        public void T06_RegulatorGetEstimatedLoadAfterCommitAndRollback()
        {
            decimal expectedValue = 0.2M;
            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));

            regulator.Propose(new Request());
            regulator.Propose(new Request());
            regulator.Commit();
            regulator.Propose(new Request());
            regulator.Rollback();

            Assert.AreEqual(expectedValue, regulator.GetEstimatedLoad());
        }

        [Test]
        public async Task T07_RegulatorGetEstimatedLoadAfterCommitAndWaiting()
        {
            decimal expectedValue = 0;
            int timeToWait = 1200;
            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));

            regulator.Propose(new Request());
            regulator.Commit();
            await Task.Delay(timeToWait);

            Assert.AreEqual(expectedValue, regulator.GetEstimatedLoad());
        }

        [Test]
        public async Task T08_RegulatorGetEstimatedLoadAfterFewCommitsAndWaiting()
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
            
            Assert.AreEqual(expectedValue, regulator.GetEstimatedLoad());
        }

        [Test]
        public async Task T09_RegulatorVerifyEstimatedLoadValueMoreThenOne()
        {
            decimal expectedValue = 2M;
            int budget = 1;

            var regulator = new Regulator(new RegulatorSettings(budget, _interval));
            regulator.Propose(new Request());
            regulator.Propose(new Request());

            Assert.AreEqual(expectedValue, regulator.GetEstimatedLoad());
        }

        [Test]
        public async Task T10_RegulatorGetEstimatedLoadAfterProposesConcurrently()
        {
            decimal expectedValue = 0.3M;

            var regulator = new Regulator(new RegulatorSettings(_budget, _interval));

            await Task.Factory.StartNew(() => regulator.Propose(new Request()));
            await Task.Factory.StartNew(() => regulator.Propose(new Request()));
            await Task.Factory.StartNew(() => regulator.Propose(new Request()));

            Assert.AreEqual(expectedValue, regulator.GetEstimatedLoad());
        }
    }
}