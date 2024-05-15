namespace Pipelines.Tests
{
    public class PipelineStageProgressWriterMock : IPipelineStageProgressWriter
    {
        public int WriteStepDependenciesNotCompletedTotal { get; private set; } = 0;
        public int WriteStepExecuteTotal { get; private set; } = 0;
        public int WriteStepCompletedTotal { get; private set; } = 0;
        public int WriteStepPartiallyCompletedTotal { get; private set; } = 0;
        public int WriteStepFailedTotal { get; private set; } = 0;
        public int WriteStepCancelledTotal { get; private set; } = 0;

        public void WriteStepDependenciesNotCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            ++WriteStepDependenciesNotCompletedTotal;
        }
        public void WriteStepExecute(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            ++WriteStepExecuteTotal;
        }
        public void WriteStepCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            ++WriteStepCompletedTotal;
        }
        public void WriteStepPartiallyCompleted(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            ++WriteStepPartiallyCompletedTotal;
        }
        public void WriteStepFailed(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            ++WriteStepFailedTotal;
        }
        public void WriteStepCancelled(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            ++WriteStepCancelledTotal;
        }
    }

    [TestClass]
    public class TestPipelineStage_PipelineStageProgressWriter
    {
        static readonly NullContext NullContext = new();

        [TestMethod]
        public void WriteStepDependenciesNotCompleted_NotCalled_WhenAllDependenciesWereCompleted()
        {
            var progressWriter = new PipelineStageProgressWriterMock();

            var stage = new NullStage
            {
                ProgressWriter = progressWriter,
                StepsDatas = [
                    new NullStepDataComplete
                    {
                        Name = "step1"
                    },
                    new NullStepDataComplete
                    {
                        Name = "step2"
                    },
                    new NullStepDataComplete
                    {
                        Name = "step3",
                        DependsOn = ["step1", "step2"]
                    },
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(0, progressWriter.WriteStepDependenciesNotCompletedTotal);
        }

        [TestMethod]
        public void WriteStepDependenciesNotCompleted_CalledOnDependents_WhenAtLeastOneDependencyWasNotCompleted()
        {
            var progressWriter = new PipelineStageProgressWriterMock();

            var stage = new NullStage
            {
                ProgressWriter = progressWriter,
                StepsDatas = [
                    new NullStepDataComplete
                    {
                        Name = "step1"
                    },
                    new NullStepDataPartiallyComplete
                    {
                        Name = "step2"
                    },
                    new NullStepDataComplete
                    {
                        Name = "step3",
                        DependsOn = ["step1", "step2"]
                    },
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, progressWriter.WriteStepDependenciesNotCompletedTotal);
        }

        [TestMethod]
        public void WriteStepDependenciesNotCompleted_CalledOnDependents_WhenAllDependenciesWereNotCompleted()
        {
            var progressWriter = new PipelineStageProgressWriterMock();

            var stage = new NullStage
            {
                ProgressWriter = progressWriter,
                StepsDatas = [
                    new NullStepDataPartiallyComplete
                    {
                        Name = "step1"
                    },
                    new NullStepDataPartiallyComplete
                    {
                        Name = "step2"
                    },
                    new NullStepDataComplete
                    {
                        Name = "step3",
                        DependsOn = ["step1", "step2"]
                    },
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, progressWriter.WriteStepDependenciesNotCompletedTotal);
        }

        [TestMethod]
        public void WriteStepExecute_Called_WhenAllDependenciesWereCompleted()
        {
            var progressWriter = new PipelineStageProgressWriterMock();

            var stage = new NullStage
            {
                ProgressWriter = progressWriter,
                StepsDatas = [
                    new NullStepDataComplete
                    {
                        Name = "step1"
                    },
                    new NullStepDataComplete
                    {
                        Name = "step2"
                    },
                    new NullStepDataComplete
                    {
                        Name = "step3",
                        DependsOn = ["step1", "step2"]
                    },
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, progressWriter.WriteStepExecuteTotal);
        }

        [TestMethod]
        public void WriteStepExecute_NotCalledOnDependents_WhenAtLeastOneDependencyWasNotCompleted()
        {
            var progressWriter = new PipelineStageProgressWriterMock();

            var stage = new NullStage
            {
                ProgressWriter = progressWriter,
                StepsDatas = [
                    new NullStepDataComplete
                    {
                        Name = "step1"
                    },
                    new NullStepDataPartiallyComplete
                    {
                        Name = "step2"
                    },
                    new NullStepDataComplete
                    {
                        Name = "step3",
                        DependsOn = ["step1", "step2"]
                    },
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(2, progressWriter.WriteStepExecuteTotal);
        }

        [TestMethod]
        public void WriteStepExecute_NotCalledOnDependents_WhenAllDependenciesWereNotCompleted()
        {
            var progressWriter = new PipelineStageProgressWriterMock();

            var stage = new NullStage
            {
                ProgressWriter = progressWriter,
                StepsDatas = [
                    new NullStepDataPartiallyComplete
                    {
                        Name = "step1"
                    },
                    new NullStepDataPartiallyComplete
                    {
                        Name = "step2"
                    },
                    new NullStepDataComplete
                    {
                        Name = "step3",
                        DependsOn = ["step1", "step2"]
                    },
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(2, progressWriter.WriteStepExecuteTotal);
        }

        [TestMethod]
        public void WriteStepCompleted_Called_3_Times()
        {
            var progressWriter = new PipelineStageProgressWriterMock();

            var stage = new NullStage
            {
                ProgressWriter = progressWriter,
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, progressWriter.WriteStepCompletedTotal);
        }

        [TestMethod]
        public void WriteStepPartiallyCompleted_Called_3_Times()
        {
            var progressWriter = new PipelineStageProgressWriterMock();

            var stage = new NullStage
            {
                ProgressWriter = progressWriter,
                StepsDatas = [
                    new NullStepDataPartiallyComplete(),
                    new NullStepDataPartiallyComplete(),
                    new NullStepDataPartiallyComplete(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, progressWriter.WriteStepPartiallyCompletedTotal);
        }

        [TestMethod]
        public void WriteStepFailed_Called_3_Times()
        {
            var progressWriter = new PipelineStageProgressWriterMock();

            var stage = new NullStage
            {
                ProgressWriter = progressWriter,
                StepsDatas = [
                    new NullStepDataFailed(),
                    new NullStepDataFailed(),
                    new NullStepDataFailed(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, progressWriter.WriteStepFailedTotal);
        }

        [TestMethod]
        public void WriteStepCancelled_Called_3_Times()
        {
            var progressWriter = new PipelineStageProgressWriterMock();

            var stage = new NullStage
            {
                ProgressWriter = progressWriter,
                StepsDatas = [
                    new NullStepDataCancelled(),
                    new NullStepDataCancelled(),
                    new NullStepDataCancelled(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, progressWriter.WriteStepCancelledTotal);
        }

        [TestMethod]
        public void WriteStep_OneOfEachType()
        {
            var progressWriter = new PipelineStageProgressWriterMock();

            var stage = new NullStage
            {
                ProgressWriter = progressWriter,
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataPartiallyComplete(),
                    new NullStepDataFailed(),
                    new NullStepDataCancelled(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, progressWriter.WriteStepCompletedTotal);
            Assert.AreEqual(1, progressWriter.WriteStepPartiallyCompletedTotal);
            Assert.AreEqual(1, progressWriter.WriteStepFailedTotal);
            Assert.AreEqual(1, progressWriter.WriteStepCancelledTotal);
        }
    }
}