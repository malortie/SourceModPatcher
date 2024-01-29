namespace Pipelines.Tests
{
    public class PipelineProgressWriterMock : IPipelineProgressWriter
    {
        public int WriteStageExecuteTotal { get; private set; } = 0;
        public int WriteStageCompletedTotal { get; private set; } = 0;
        public int WriteStagePartiallyCompletedTotal { get; private set; } = 0;
        public int WriteStageFailedTotal { get; private set; } = 0;
        public int WriteStageCancelledTotal { get; private set; } = 0;

        public void WriteStageExecute(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            ++WriteStageExecuteTotal;
        }
        public void WriteStageCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            ++WriteStageCompletedTotal;
        }
        public void WriteStagePartiallyCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            ++WriteStagePartiallyCompletedTotal;
        }
        public void WriteStageFailed(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            ++WriteStageFailedTotal;
        }
        public void WriteStageCancelled(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            ++WriteStageCancelledTotal;
        }
    }

    [TestClass]
    public class TestPipeline_PipelineProgressWriter
    {
        static readonly NullContext NullContext = new();

        [TestMethod]
        public void WriteStageExecute_Called_3_Times()
        {
            var progressWriter = new PipelineProgressWriterMock();

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
                        new NullStepDataComplete()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataComplete()
                    ]
                }
            }, progressWriter);

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, progressWriter.WriteStageExecuteTotal);
        }

        [TestMethod]
        public void WriteStageCompleted_Called_3_Times()
        {
            var progressWriter = new PipelineProgressWriterMock();

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
                        new NullStepDataComplete()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataComplete()
                    ]
                }
            }, progressWriter);

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, progressWriter.WriteStageCompletedTotal);
        }

        [TestMethod]
        public void WriteStagePartiallyCompleted_Called_3_Times()
        {
            var progressWriter = new PipelineProgressWriterMock();

            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataPartiallyComplete()
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
                        new NullStepDataPartiallyComplete()
                    ]
                }
            }, progressWriter);

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, progressWriter.WriteStagePartiallyCompletedTotal);
        }

        [TestMethod]
        public void WriteStageFailed_Called_3_Times()
        {
            var progressWriter = new PipelineProgressWriterMock();

            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataFailed()
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
                        new NullStepDataFailed()
                    ]
                }
            }, progressWriter);

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, progressWriter.WriteStageFailedTotal);
        }

        [TestMethod]
        public void WriteStageCancelled_Called_3_Times()
        {
            var progressWriter = new PipelineProgressWriterMock();

            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataCancelled()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataCancelled()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataCancelled()
                    ]
                }
            }, progressWriter);

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, progressWriter.WriteStageCancelledTotal);
        }

        [TestMethod]
        public void WriteStage_OneOfEachType()
        {
            var progressWriter = new PipelineProgressWriterMock();

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
            }, progressWriter);

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, progressWriter.WriteStageCompletedTotal);
            Assert.AreEqual(1, progressWriter.WriteStagePartiallyCompletedTotal);
            Assert.AreEqual(1, progressWriter.WriteStageFailedTotal);
            Assert.AreEqual(1, progressWriter.WriteStageCancelledTotal);
        }
    }
}