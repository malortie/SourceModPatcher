namespace Pipelines.Tests
{
    [TestClass]
    public class TestPipelineStage_StepsStatuses_StatsResults
    {
        static NullContext NullContext = new NullContext();

        [TestMethod]
        public void StepsStatuses_StatsResults_OneStepComplete()
        {
            var stage = new NullStage
            {
                StepsDatas = [
                    new NullStepDataComplete(),
                ]
            };

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(1, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.Complete);

            Assert.AreEqual(1, stage.StatsResults.NumStepsTotal);
            Assert.AreEqual(1, stage.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, stage.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, stage.StatsResults.NumStepsFailed);
            Assert.AreEqual(0, stage.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StepsStatuses_StatsResults_OneStepPartiallyComplete()
        {
            var stage = new NullStage
            {
                StepsDatas = [
                    new NullStepDataPartiallyComplete(),
                ]
            };

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(1, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.PartiallyComplete);

            Assert.AreEqual(1, stage.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, stage.StatsResults.NumStepsCompleted);
            Assert.AreEqual(1, stage.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, stage.StatsResults.NumStepsFailed);
            Assert.AreEqual(0, stage.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StepsStatuses_StatsResults_OneStepFailed()
        {
            var stage = new NullStage
            {
                StepsDatas = [
                    new NullStepDataFailed(),
                ]
            };

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(1, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.Failed);

            Assert.AreEqual(1, stage.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, stage.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, stage.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(1, stage.StatsResults.NumStepsFailed);
            Assert.AreEqual(0, stage.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StepsStatuses_StatsResults_OneStepCancelled()
        {
            var stage = new NullStage
            {
                StepsDatas = [
                    new NullStepDataCancelled(),
                ]
            };

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(1, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.Cancelled);

            Assert.AreEqual(1, stage.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, stage.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, stage.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, stage.StatsResults.NumStepsFailed);
            Assert.AreEqual(1, stage.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StepsStatuses_StatsResults_OneOfEachStatus()
        {
            var stage = new NullStage
            {
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataPartiallyComplete(),
                    new NullStepDataFailed(),
                    new NullStepDataCancelled(),
                ]
            };

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(4, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.Complete);
            Assert.AreEqual(stepsStatuses[1], PipelineStepStatus.PartiallyComplete);
            Assert.AreEqual(stepsStatuses[2], PipelineStepStatus.Failed);
            Assert.AreEqual(stepsStatuses[3], PipelineStepStatus.Cancelled);

            Assert.AreEqual(4, stage.StatsResults.NumStepsTotal);
            Assert.AreEqual(1, stage.StatsResults.NumStepsCompleted);
            Assert.AreEqual(1, stage.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(1, stage.StatsResults.NumStepsFailed);
            Assert.AreEqual(1, stage.StatsResults.NumStepsCancelled);
        }
    }
}