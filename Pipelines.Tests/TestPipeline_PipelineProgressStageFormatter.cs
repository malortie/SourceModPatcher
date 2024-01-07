namespace Pipelines.Tests
{
    public class ProgressStageFormatterMock : IPipelineProgressStageFormatter
    {
        public int FormatTotal { get; private set; } = 0;

        public string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStageData stageData)
        {
            ++FormatTotal;
            return string.Empty;
        }
    }

    [TestClass]
    public class TestPipeline_PipelineProgressStageFormatter
    {
        static NullContext NullContext = new NullContext();
        static IWriter NullWriter = new NullWriter();

        [TestMethod]
        public void StageExecuteFormatter_Called_3_Times()
        {
            var stageExecuteFormatter = new ProgressStageFormatterMock();
            var stageCompletedFormatter = new ProgressStageFormatterMock();
            var stagePartiallyCompletedFormatter = new ProgressStageFormatterMock();
            var stageFailedFormatter = new ProgressStageFormatterMock();
            var stageCancelledFormatter = new ProgressStageFormatterMock();

            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = []
                },
                new NullStage
                {
                    StepsDatas = []
                },
                new NullStage
                {
                    StepsDatas = []
                }
            }, new PipelineProgressWriter(NullWriter) { 
                StageExecuteFormatter = stageExecuteFormatter,
                StageCompletedFormatter = stageCompletedFormatter,
                StagePartiallyCompletedFormatter = stagePartiallyCompletedFormatter,
                StageFailedFormatter = stageFailedFormatter,
                StageCancelledFormatter = stageCancelledFormatter,
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, stageExecuteFormatter.FormatTotal);
        }

        [TestMethod]
        public void StageCompletedFormatter_Called_3_Times()
        {
            var stageExecuteFormatter = new ProgressStageFormatterMock();
            var stageCompletedFormatter = new ProgressStageFormatterMock();
            var stagePartiallyCompletedFormatter = new ProgressStageFormatterMock();
            var stageFailedFormatter = new ProgressStageFormatterMock();
            var stageCancelledFormatter = new ProgressStageFormatterMock();

            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = []
                },
                new NullStage
                {
                    StepsDatas = []
                },
                new NullStage
                {
                    StepsDatas = []
                }
            }, new PipelineProgressWriter(NullWriter) {
                StageExecuteFormatter = stageExecuteFormatter,
                StageCompletedFormatter = stageCompletedFormatter,
                StagePartiallyCompletedFormatter = stagePartiallyCompletedFormatter,
                StageFailedFormatter = stageFailedFormatter,
                StageCancelledFormatter = stageCancelledFormatter,
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, stageCompletedFormatter.FormatTotal);
        }

        [TestMethod]
        public void StagePartiallyCompletedFormatter_Called_3_Times()
        {
            var stageExecuteFormatter = new ProgressStageFormatterMock();
            var stageCompletedFormatter = new ProgressStageFormatterMock();
            var stagePartiallyCompletedFormatter = new ProgressStageFormatterMock();
            var stageFailedFormatter = new ProgressStageFormatterMock();
            var stageCancelledFormatter = new ProgressStageFormatterMock();

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
            }, new PipelineProgressWriter(NullWriter) {
                StageExecuteFormatter = stageExecuteFormatter,
                StageCompletedFormatter = stageCompletedFormatter,
                StagePartiallyCompletedFormatter = stagePartiallyCompletedFormatter,
                StageFailedFormatter = stageFailedFormatter,
                StageCancelledFormatter = stageCancelledFormatter,
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, stagePartiallyCompletedFormatter.FormatTotal);
        }

        [TestMethod]
        public void StageFailedFormatter_Called_3_Times()
        {
            var stageExecuteFormatter = new ProgressStageFormatterMock();
            var stageCompletedFormatter = new ProgressStageFormatterMock();
            var stagePartiallyCompletedFormatter = new ProgressStageFormatterMock();
            var stageFailedFormatter = new ProgressStageFormatterMock();
            var stageCancelledFormatter = new ProgressStageFormatterMock();

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
            }, new PipelineProgressWriter(NullWriter) {
                StageExecuteFormatter = stageExecuteFormatter,
                StageCompletedFormatter = stageCompletedFormatter,
                StagePartiallyCompletedFormatter = stagePartiallyCompletedFormatter,
                StageFailedFormatter = stageFailedFormatter,
                StageCancelledFormatter = stageCancelledFormatter,
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, stageFailedFormatter.FormatTotal);
        }

        [TestMethod]
        public void StageCancelledFormatter_Called_3_Times()
        {
            var stageExecuteFormatter = new ProgressStageFormatterMock();
            var stageCompletedFormatter = new ProgressStageFormatterMock();
            var stagePartiallyCompletedFormatter = new ProgressStageFormatterMock();
            var stageFailedFormatter = new ProgressStageFormatterMock();
            var stageCancelledFormatter = new ProgressStageFormatterMock();

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
            }, new PipelineProgressWriter(NullWriter) {
                StageExecuteFormatter = stageExecuteFormatter,
                StageCompletedFormatter = stageCompletedFormatter,
                StagePartiallyCompletedFormatter = stagePartiallyCompletedFormatter,
                StageFailedFormatter = stageFailedFormatter,
                StageCancelledFormatter = stageCancelledFormatter,
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, stageCancelledFormatter.FormatTotal);
        }

        [TestMethod]
        public void StageFormatter_OneOfEachType()
        {
            var stageExecuteFormatter = new ProgressStageFormatterMock();
            var stageCompletedFormatter = new ProgressStageFormatterMock();
            var stagePartiallyCompletedFormatter = new ProgressStageFormatterMock();
            var stageFailedFormatter = new ProgressStageFormatterMock();
            var stageCancelledFormatter = new ProgressStageFormatterMock();

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
            }, new PipelineProgressWriter(NullWriter)
            {
                StageExecuteFormatter = stageExecuteFormatter,
                StageCompletedFormatter = stageCompletedFormatter,
                StagePartiallyCompletedFormatter = stagePartiallyCompletedFormatter,
                StageFailedFormatter = stageFailedFormatter,
                StageCancelledFormatter = stageCancelledFormatter,
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, stageCompletedFormatter.FormatTotal);
            Assert.AreEqual(1, stagePartiallyCompletedFormatter.FormatTotal);
            Assert.AreEqual(1, stageFailedFormatter.FormatTotal);
            Assert.AreEqual(1, stageCancelledFormatter.FormatTotal);
        }
    }
}