namespace Pipelines.Tests
{
    public class PipelineStageProgressStepFormatterMock : IPipelineStageProgressStepFormatter
    {
        public int FormatTotal { get; private set; } = 0;

        public string Format(IPipelineProgressContext pipelineContext, ReadOnlyPipelineStepData stepData)
        {
            ++FormatTotal;
            return string.Empty;
        }
    }

    [TestClass]
    public class TestPipelineStage_PipelineStageProgressStepFormatter
    {
        static NullContext NullContext = new NullContext();
        static IWriter NullWriter = new NullWriter();

        [TestMethod]
        public void StepExecuteFormatter_Called_WhenAllDependenciesWereCompleted()
        {
            var stepExecuteFormatter = new PipelineStageProgressStepFormatterMock();
            var stepDependenciesNotCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepPartiallyCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepFailedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCancelledFormatter = new PipelineStageProgressStepFormatterMock();

            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(NullWriter)
                {
                    StepExecuteFormatter = stepExecuteFormatter,
                    StepDependenciesNotCompletedFormatter = stepDependenciesNotCompletedFormatter,
                    StepCompletedFormatter = stepCompletedFormatter,
                    StepPartiallyCompletedFormatter = stepPartiallyCompletedFormatter,
                    StepFailedFormatter = stepFailedFormatter,
                    StepCancelledFormatter = stepCancelledFormatter,
                },
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
            Assert.AreEqual(3, stepExecuteFormatter.FormatTotal);
        }

        [TestMethod]
        public void StepExecuteFormatter_NotCalledOnDependents_WhenAtLeastOneDependencyWasNotCompleted()
        {
            var stepExecuteFormatter = new PipelineStageProgressStepFormatterMock();
            var stepDependenciesNotCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepPartiallyCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepFailedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCancelledFormatter = new PipelineStageProgressStepFormatterMock();

            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(NullWriter)
                {
                    StepExecuteFormatter = stepExecuteFormatter,
                    StepDependenciesNotCompletedFormatter = stepDependenciesNotCompletedFormatter,
                    StepCompletedFormatter = stepCompletedFormatter,
                    StepPartiallyCompletedFormatter = stepPartiallyCompletedFormatter,
                    StepFailedFormatter = stepFailedFormatter,
                    StepCancelledFormatter = stepCancelledFormatter,
                },
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
            Assert.AreEqual(2, stepExecuteFormatter.FormatTotal);
        }

        [TestMethod]
        public void StepExecuteFormatter_NotCalledOnDependents_WhenAllDependenciesWereNotCompleted()
        {
            var stepExecuteFormatter = new PipelineStageProgressStepFormatterMock();
            var stepDependenciesNotCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepPartiallyCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepFailedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCancelledFormatter = new PipelineStageProgressStepFormatterMock();

            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(NullWriter)
                {
                    StepExecuteFormatter = stepExecuteFormatter,
                    StepDependenciesNotCompletedFormatter = stepDependenciesNotCompletedFormatter,
                    StepCompletedFormatter = stepCompletedFormatter,
                    StepPartiallyCompletedFormatter = stepPartiallyCompletedFormatter,
                    StepFailedFormatter = stepFailedFormatter,
                    StepCancelledFormatter = stepCancelledFormatter,
                },
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
            Assert.AreEqual(2, stepExecuteFormatter.FormatTotal);
        }

        [TestMethod]
        public void StepDependenciesNotCompletedFormatter_NotCalled_WhenAllDependenciesWereCompleted()
        {
            var stepExecuteFormatter = new PipelineStageProgressStepFormatterMock();
            var stepDependenciesNotCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepPartiallyCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepFailedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCancelledFormatter = new PipelineStageProgressStepFormatterMock();

            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(NullWriter)
                {
                    StepExecuteFormatter = stepExecuteFormatter,
                    StepDependenciesNotCompletedFormatter = stepDependenciesNotCompletedFormatter,
                    StepCompletedFormatter = stepCompletedFormatter,
                    StepPartiallyCompletedFormatter = stepPartiallyCompletedFormatter,
                    StepFailedFormatter = stepFailedFormatter,
                    StepCancelledFormatter = stepCancelledFormatter,
                },
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
            Assert.AreEqual(0, stepDependenciesNotCompletedFormatter.FormatTotal);
        }

        [TestMethod]
        public void StepDependenciesNotCompletedFormatter_CalledOnDependents_WhenAtLeastOneDependencyWasNotCompleted()
        {
            var stepExecuteFormatter = new PipelineStageProgressStepFormatterMock();
            var stepDependenciesNotCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepPartiallyCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepFailedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCancelledFormatter = new PipelineStageProgressStepFormatterMock();

            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(NullWriter)
                {
                    StepExecuteFormatter = stepExecuteFormatter,
                    StepDependenciesNotCompletedFormatter = stepDependenciesNotCompletedFormatter,
                    StepCompletedFormatter = stepCompletedFormatter,
                    StepPartiallyCompletedFormatter = stepPartiallyCompletedFormatter,
                    StepFailedFormatter = stepFailedFormatter,
                    StepCancelledFormatter = stepCancelledFormatter,
                },
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
            Assert.AreEqual(1, stepDependenciesNotCompletedFormatter.FormatTotal);
        }

        [TestMethod]
        public void StepDependenciesNotCompletedFormatter_CalledOnDependents_WhenAllDependenciesWereNotCompleted()
        {
            var stepExecuteFormatter = new PipelineStageProgressStepFormatterMock();
            var stepDependenciesNotCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepPartiallyCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepFailedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCancelledFormatter = new PipelineStageProgressStepFormatterMock();

            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(NullWriter)
                {
                    StepExecuteFormatter = stepExecuteFormatter,
                    StepDependenciesNotCompletedFormatter = stepDependenciesNotCompletedFormatter,
                    StepCompletedFormatter = stepCompletedFormatter,
                    StepPartiallyCompletedFormatter = stepPartiallyCompletedFormatter,
                    StepFailedFormatter = stepFailedFormatter,
                    StepCancelledFormatter = stepCancelledFormatter,
                },
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
            Assert.AreEqual(1, stepDependenciesNotCompletedFormatter.FormatTotal);
        }

        [TestMethod]
        public void StepCompletedFormatter_Called_3_Times()
        {
            var stepExecuteFormatter = new PipelineStageProgressStepFormatterMock();
            var stepDependenciesNotCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepPartiallyCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepFailedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCancelledFormatter = new PipelineStageProgressStepFormatterMock();

            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(NullWriter)
                {
                    StepExecuteFormatter = stepExecuteFormatter,
                    StepDependenciesNotCompletedFormatter = stepDependenciesNotCompletedFormatter,
                    StepCompletedFormatter = stepCompletedFormatter,
                    StepPartiallyCompletedFormatter = stepPartiallyCompletedFormatter,
                    StepFailedFormatter = stepFailedFormatter,
                    StepCancelledFormatter = stepCancelledFormatter,
                },
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, stepCompletedFormatter.FormatTotal);
        }

        [TestMethod]
        public void StepPartiallyCompletedFormatter_Called_3_Times()
        {
            var stepExecuteFormatter = new PipelineStageProgressStepFormatterMock();
            var stepDependenciesNotCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepPartiallyCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepFailedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCancelledFormatter = new PipelineStageProgressStepFormatterMock();

            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(NullWriter)
                {
                    StepExecuteFormatter = stepExecuteFormatter,
                    StepDependenciesNotCompletedFormatter = stepDependenciesNotCompletedFormatter,
                    StepCompletedFormatter = stepCompletedFormatter,
                    StepPartiallyCompletedFormatter = stepPartiallyCompletedFormatter,
                    StepFailedFormatter = stepFailedFormatter,
                    StepCancelledFormatter = stepCancelledFormatter,
                },
                StepsDatas = [
                    new NullStepDataPartiallyComplete(),
                    new NullStepDataPartiallyComplete(),
                    new NullStepDataPartiallyComplete(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, stepPartiallyCompletedFormatter.FormatTotal);
        }

        [TestMethod]
        public void StepFailedFormatter_Called_3_Times()
        {
            var stepExecuteFormatter = new PipelineStageProgressStepFormatterMock();
            var stepDependenciesNotCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepPartiallyCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepFailedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCancelledFormatter = new PipelineStageProgressStepFormatterMock();

            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(NullWriter)
                {
                    StepExecuteFormatter = stepExecuteFormatter,
                    StepDependenciesNotCompletedFormatter = stepDependenciesNotCompletedFormatter,
                    StepCompletedFormatter = stepCompletedFormatter,
                    StepPartiallyCompletedFormatter = stepPartiallyCompletedFormatter,
                    StepFailedFormatter = stepFailedFormatter,
                    StepCancelledFormatter = stepCancelledFormatter,
                },
                StepsDatas = [
                    new NullStepDataFailed(),
                    new NullStepDataFailed(),
                    new NullStepDataFailed(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, stepFailedFormatter.FormatTotal);
        }

        [TestMethod]
        public void StepCancelledFormatter_Called_3_Times()
        {
            var stepExecuteFormatter = new PipelineStageProgressStepFormatterMock();
            var stepDependenciesNotCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepPartiallyCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepFailedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCancelledFormatter = new PipelineStageProgressStepFormatterMock();

            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(NullWriter)
                {
                    StepExecuteFormatter = stepExecuteFormatter,
                    StepDependenciesNotCompletedFormatter = stepDependenciesNotCompletedFormatter,
                    StepCompletedFormatter = stepCompletedFormatter,
                    StepPartiallyCompletedFormatter = stepPartiallyCompletedFormatter,
                    StepFailedFormatter = stepFailedFormatter,
                    StepCancelledFormatter = stepCancelledFormatter,
                },
                StepsDatas = [
                    new NullStepDataCancelled(),
                    new NullStepDataCancelled(),
                    new NullStepDataCancelled(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, stepCancelledFormatter.FormatTotal);
        }

        [TestMethod]
        public void StepFormatter_OneOfEachType()
        {
            var stepExecuteFormatter = new PipelineStageProgressStepFormatterMock();
            var stepDependenciesNotCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepPartiallyCompletedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepFailedFormatter = new PipelineStageProgressStepFormatterMock();
            var stepCancelledFormatter = new PipelineStageProgressStepFormatterMock();

            var stage = new NullStage
            {
                ProgressWriter = new PipelineStageProgressWriter(NullWriter)
                {
                    StepExecuteFormatter = stepExecuteFormatter,
                    StepDependenciesNotCompletedFormatter = stepDependenciesNotCompletedFormatter,
                    StepCompletedFormatter = stepCompletedFormatter,
                    StepPartiallyCompletedFormatter = stepPartiallyCompletedFormatter,
                    StepFailedFormatter = stepFailedFormatter,
                    StepCancelledFormatter = stepCancelledFormatter,
                },
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataPartiallyComplete(),
                    new NullStepDataFailed(),
                    new NullStepDataCancelled(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, stepCompletedFormatter.FormatTotal);
            Assert.AreEqual(1, stepPartiallyCompletedFormatter.FormatTotal);
            Assert.AreEqual(1, stepFailedFormatter.FormatTotal);
            Assert.AreEqual(1, stepCancelledFormatter.FormatTotal);
        }
    }
}