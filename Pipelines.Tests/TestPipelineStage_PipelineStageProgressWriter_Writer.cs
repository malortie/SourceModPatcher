namespace Pipelines.Tests
{
    public class WriterMock : IWriter
    {
        public int SuccessTotal { get; private set; } = 0;
        public int InfoTotal { get; private set; } = 0;
        public int WarningTotal { get; private set; } = 0;
        public int ErrorTotal { get; private set; } = 0;
        public int FailureTotal { get; private set; } = 0;
        public int CancellationTotal { get; private set; } = 0;

        public void Success(string message)
        {
            ++SuccessTotal;
        }
        public void Info(string message)
        {
            ++InfoTotal;
        }
        public void Warning(string message)
        {
            ++WarningTotal;
        }
        public void Error(string message)
        {
            ++ErrorTotal;
        }
        public void Failure(string message)
        {
            ++FailureTotal;
        }
        public void Cancellation(string message)
        {
            ++CancellationTotal;
        }
    }

    [TestClass]
    public class TestPipelineStage_PipelineStageProgressWriter_Writer
    {
        static NullContext NullContext = new NullContext();

        [TestMethod]
        public void WriteStepExecute_Writer_Info()
        {
            var writer = new WriterMock();
            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(writer),
                StepsDatas = [
                    new NullStepDataComplete()
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(2, writer.InfoTotal); // 1 * WriteStepExecute + 1 * WriteStepCompleted
        }

        [TestMethod]
        public void WriteStepDependenciesNotCompleted_Writer_Warning()
        {
            var writer = new WriterMock();
            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(writer),
                StepsDatas = [
                    new NullStepDataPartiallyComplete
                    {
                        Name = "step1"
                    },
                    new NullStepDataComplete
                    {
                        Name = "step2",
                        DependsOn = ["step1"]
                    },
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(2, writer.WarningTotal); // 1 * WriteStepPartiallyCompleted + 1 * WriteStepDependenciesNotCompleted
        }

        [TestMethod]
        public void WriteStepCompleted_Writer_Info()
        {
            var writer = new WriterMock();
            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(writer),
                StepsDatas = [
                    new NullStepDataComplete()
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(2, writer.InfoTotal); // 1 * WriteStepExecute + 1 * WriteStepCompleted
        }

        [TestMethod]
        public void WriteStepPartiallyCompleted_Writer_Warning()
        {
            var writer = new WriterMock();
            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(writer),
                StepsDatas = [
                    new NullStepDataPartiallyComplete()
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, writer.WarningTotal);
        }

        [TestMethod]
        public void WriteStepFailed_Writer_Failure()
        {
            var writer = new WriterMock();
            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(writer),
                StepsDatas = [
                    new NullStepDataFailed()
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, writer.FailureTotal);
        }

        [TestMethod]
        public void WriteStepCancelled_Writer_Cancellation()
        {
            var writer = new WriterMock();
            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(writer),
                StepsDatas = [
                    new NullStepDataCancelled()
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, writer.CancellationTotal);
        }

        [TestMethod]
        public void WriteStep_Writer_OneOfEachType()
        {
            var writer = new WriterMock();
            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(writer),
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataPartiallyComplete(),
                    new NullStepDataFailed(),
                    new NullStepDataCancelled(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(5, writer.InfoTotal); // 4 * WriteStepExecute + 1 * WriteStepCompleted
            Assert.AreEqual(1, writer.WarningTotal);
            Assert.AreEqual(1, writer.FailureTotal);
            Assert.AreEqual(1, writer.CancellationTotal);
        }
    }
}