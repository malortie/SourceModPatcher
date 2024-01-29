namespace Pipelines.Tests
{
    [TestClass]
    public class TestPipeline_PipelineProgressWriter_Writer
    {
        static readonly NullContext NullContext = new();

        [TestMethod]
        public void WriteStageExecute_Writer_Info()
        {
            var writer = new WriterMock();
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = []
                }
            }, new PipelineProgressWriter(writer));

            pipeline.Execute(NullContext);

            Assert.AreEqual(2, writer.InfoTotal); // 1 * WriteStageExecute + 1 * WriteStageCompleted
        }

        [TestMethod]
        public void WriteStageCompleted_Writer_Info()
        {
            var writer = new WriterMock();
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataComplete()
                    ]
                }
            }, new PipelineProgressWriter(writer));

            pipeline.Execute(NullContext);

            Assert.AreEqual(2, writer.InfoTotal); // 1 * WriteStageExecute + 1 * WriteStageCompleted
        }

        [TestMethod]
        public void WriteStagePartiallyCompleted_Writer_Warning()
        {
            var writer = new WriterMock();
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataPartiallyComplete()
                    ]
                }
            }, new PipelineProgressWriter(writer));

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, writer.WarningTotal);
        }

        [TestMethod]
        public void WriteStageFailed_Writer_Failure()
        {
            var writer = new WriterMock();
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataFailed()
                    ]
                }
            }, new PipelineProgressWriter(writer));

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, writer.FailureTotal);
        }

        [TestMethod]
        public void WriteStageCancelled_Writer_Cancellation()
        {
            var writer = new WriterMock();
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataCancelled()
                    ]
                }
            }, new PipelineProgressWriter(writer));

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, writer.CancellationTotal);
        }

        [TestMethod]
        public void WriteStage_Writer_OneOfEachType()
        {
            var writer = new WriterMock();
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataComplete()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataPartiallyComplete()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataFailed()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataCancelled()
                    ]
                }
            }, new PipelineProgressWriter(writer));

            pipeline.Execute(NullContext);

            Assert.AreEqual(5, writer.InfoTotal); // 4 * WriteStageExecute + 1 * WriteStageCompleted
            Assert.AreEqual(1, writer.WarningTotal);
            Assert.AreEqual(1, writer.FailureTotal);
            Assert.AreEqual(1, writer.CancellationTotal);
        }
    }
}