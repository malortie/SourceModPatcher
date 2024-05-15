namespace Pipelines.Tests
{
    [TestClass]
    public class TestPipelineStage_DependsOn
    {
        static readonly NullContext NullContext = new();

        [TestMethod]
        public void DependsOn_DependentsExecuted_WhenDependencyReturnsComplete()
        {
            var stage = new NullStage
            {
                StepsDatas = [
                    new NullStepDataComplete
                    {
                        Name = "step1"
                    },
                    new NullStepDataComplete
                    {
                        Name = "step2",
                        DependsOn = ["step1"]
                    },
                    new NullStepDataPartiallyComplete
                    {
                        Name = "step3",
                        DependsOn = ["step1"]
                    },
                    new NullStepDataFailed
                    {
                        Name = "step4",
                        DependsOn = ["step1"]
                    },
                    new NullStepDataCancelled
                    {
                        Name = "step5",
                        DependsOn = ["step1"]
                    },
                ]
            };

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(5, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.Complete);
            Assert.AreEqual(stepsStatuses[1], PipelineStepStatus.Complete);
            Assert.AreEqual(stepsStatuses[2], PipelineStepStatus.PartiallyComplete);
            Assert.AreEqual(stepsStatuses[3], PipelineStepStatus.Failed);
            Assert.AreEqual(stepsStatuses[4], PipelineStepStatus.Cancelled);
        }

        [TestMethod]
        public void DependsOn_DependentsReturnCancelled_WhenDependencyReturnsPartiallyComplete()
        {
            var stage = new NullStage
            {
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
                    new NullStepDataPartiallyComplete
                    {
                        Name = "step3",
                        DependsOn = ["step1"]
                    },
                    new NullStepDataFailed
                    {
                        Name = "step4",
                        DependsOn = ["step1"]
                    },
                    new NullStepDataCancelled
                    {
                        Name = "step5",
                        DependsOn = ["step1"]
                    },
                ]
            };

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(5, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.PartiallyComplete);
            Assert.AreEqual(stepsStatuses[1], PipelineStepStatus.Cancelled);
            Assert.AreEqual(stepsStatuses[2], PipelineStepStatus.Cancelled);
            Assert.AreEqual(stepsStatuses[3], PipelineStepStatus.Cancelled);
            Assert.AreEqual(stepsStatuses[4], PipelineStepStatus.Cancelled);
        }

        [TestMethod]
        public void DependsOn_DependentsReturnCancelled_WhenDependencyReturnsFailed()
        {
            var stage = new NullStage
            {
                StepsDatas = [
                    new NullStepDataFailed
                    {
                        Name = "step1"
                    },
                    new NullStepDataComplete
                    {
                        Name = "step2",
                        DependsOn = ["step1"]
                    },
                    new NullStepDataPartiallyComplete
                    {
                        Name = "step3",
                        DependsOn = ["step1"]
                    },
                    new NullStepDataFailed
                    {
                        Name = "step4",
                        DependsOn = ["step1"]
                    },
                    new NullStepDataCancelled
                    {
                        Name = "step5",
                        DependsOn = ["step1"]
                    },
                ]
            };

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(5, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.Failed);
            Assert.AreEqual(stepsStatuses[1], PipelineStepStatus.Cancelled);
            Assert.AreEqual(stepsStatuses[2], PipelineStepStatus.Cancelled);
            Assert.AreEqual(stepsStatuses[3], PipelineStepStatus.Cancelled);
            Assert.AreEqual(stepsStatuses[4], PipelineStepStatus.Cancelled);
        }

        [TestMethod]
        public void DependsOn_DependentsReturnCancelled_WhenDependencyReturnsCancelled()
        {
            var stage = new NullStage
            {
                StepsDatas = [
                    new NullStepDataCancelled
                    {
                        Name = "step1"
                    },
                    new NullStepDataComplete
                    {
                        Name = "step2",
                        DependsOn = ["step1"]
                    },
                    new NullStepDataPartiallyComplete
                    {
                        Name = "step3",
                        DependsOn = ["step1"]
                    },
                    new NullStepDataFailed
                    {
                        Name = "step4",
                        DependsOn = ["step1"]
                    },
                    new NullStepDataCancelled
                    {
                        Name = "step5",
                        DependsOn = ["step1"]
                    },
                ]
            };

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(5, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.Cancelled);
            Assert.AreEqual(stepsStatuses[1], PipelineStepStatus.Cancelled);
            Assert.AreEqual(stepsStatuses[2], PipelineStepStatus.Cancelled);
            Assert.AreEqual(stepsStatuses[3], PipelineStepStatus.Cancelled);
            Assert.AreEqual(stepsStatuses[4], PipelineStepStatus.Cancelled);
        }

        [TestMethod]
        public void DependsOn_DependentExecuted_WhenAllDependenciesAreComplete()
        {
            var stage = new NullStage
            {
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

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(3, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.Complete);
            Assert.AreEqual(stepsStatuses[1], PipelineStepStatus.Complete);
            Assert.AreEqual(stepsStatuses[2], PipelineStepStatus.Complete);
        }

        [TestMethod]
        public void DependsOn_DependentReturnsCancelled_WhenAtLeastOneDependencyIsPartiallyComplete()
        {
            var stage = new NullStage
            {
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

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(3, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.Complete);
            Assert.AreEqual(stepsStatuses[1], PipelineStepStatus.PartiallyComplete);
            Assert.AreEqual(stepsStatuses[2], PipelineStepStatus.Cancelled);
        }

        [TestMethod]
        public void DependsOn_DependentReturnsCancelled_WhenAtLeastOneDependencyIsFailed()
        {
            var stage = new NullStage
            {
                StepsDatas = [
                    new NullStepDataComplete
                    {
                        Name = "step1"
                    },
                    new NullStepDataFailed
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

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(3, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.Complete);
            Assert.AreEqual(stepsStatuses[1], PipelineStepStatus.Failed);
            Assert.AreEqual(stepsStatuses[2], PipelineStepStatus.Cancelled);
        }

        [TestMethod]
        public void DependsOn_DependentReturnsCancelled_WhenAtLeastOneDependencyIsCancelled()
        {
            var stage = new NullStage
            {
                StepsDatas = [
                    new NullStepDataComplete
                    {
                        Name = "step1"
                    },
                    new NullStepDataCancelled
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

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(3, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.Complete);
            Assert.AreEqual(stepsStatuses[1], PipelineStepStatus.Cancelled);
            Assert.AreEqual(stepsStatuses[2], PipelineStepStatus.Cancelled);
        }

        [TestMethod]
        public void DependsOn_DependentReturnsCancelled_WhenDependencyWasNotYetExecuted()
        {
            var stage = new NullStage
            {
                StepsDatas = [
                    new NullStepDataComplete
                    {
                        Name = "step2",
                        DependsOn = ["step1"]
                    },
                    new NullStepDataComplete
                    {
                        Name = "step1"
                    },
                ]
            };

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(2, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.Cancelled);
            Assert.AreEqual(stepsStatuses[1], PipelineStepStatus.Complete);
        }

        [TestMethod]
        public void DependsOn_DependentReturnsCancelled_WhenDependenciesWereNotYetExecuted()
        {
            var stage = new NullStage
            {
                StepsDatas = [
                    new NullStepDataComplete
                    {
                        Name = "step3",
                        DependsOn = ["step1", "step2"]
                    },
                    new NullStepDataComplete
                    {
                        Name = "step1"
                    },
                    new NullStepDataComplete
                    {
                        Name = "step2"
                    },
                ]
            };

            var stepsStatuses = stage.DoStage(NullContext);
            Assert.AreEqual(3, stepsStatuses.Length);
            Assert.AreEqual(stepsStatuses[0], PipelineStepStatus.Cancelled);
            Assert.AreEqual(stepsStatuses[1], PipelineStepStatus.Complete);
            Assert.AreEqual(stepsStatuses[2], PipelineStepStatus.Complete);
        }
    }
}