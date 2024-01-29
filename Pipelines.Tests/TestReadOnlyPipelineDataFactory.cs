namespace Pipelines.Tests
{
    public class DefaultPipelineStepData : AbstractNullStepData { }

    [TestClass]
    public class TestReadOnlyPipelineDataFactory
    {
        [TestMethod]
        public void CreateStepData()
        {
            var roStepData = new ReadOnlyPipelineDataFactory().CreateStepData(new DefaultPipelineStepData
            {
                Name = "step3",
                Description = "step 3",
                DependsOn = ["step1", "step2"],
            });

            Assert.IsNotNull(roStepData);
            Assert.AreEqual("step3", roStepData.Name);
            Assert.AreEqual("step 3", roStepData.Description);
            CollectionAssert.AreEqual(new List<string> { "step1", "step2" }, roStepData.DependsOn.ToList());
        }

        [TestMethod]
        public void CreateStepData_InstanceIsImmutable()
        {
            var stepData = new DefaultPipelineStepData
            {
                Name = "step3",
                Description = "step 3",
                DependsOn = ["step1", "step2"],
            };

            var roStepData = new ReadOnlyPipelineDataFactory().CreateStepData(stepData);
            stepData.Name = "step5";
            stepData.Description = "step 5";
            stepData.DependsOn[0] = "step3";
            stepData.DependsOn[1] = "step4";

            Assert.AreNotEqual(stepData.Name, roStepData.Name);
            Assert.AreNotEqual(stepData.Description, roStepData.Description);
            CollectionAssert.AreNotEqual(stepData.DependsOn, roStepData.DependsOn.ToList());
        }

        [TestMethod]
        public void CreateStageData()
        {
            var roStageData = new ReadOnlyPipelineDataFactory().CreateStageData(new NullStage
            {
                Name = "stage_1",
                Description = "stage 1",
                StepsDatas = [
                    new DefaultPipelineStepData
                    {
                        Name = "step3",
                        Description = "step 3",
                        DependsOn = ["step1", "step2"]
                    },
                    new DefaultPipelineStepData
                    {
                        Name = "step5",
                        Description = "step 5",
                        DependsOn = ["step3", "step4"]
                    }
                ],
                StatsResults = new PipelineStepStatsResults
                {
                    NumStepsCancelled = 1,
                    NumStepsCompleted = 2,
                    NumStepsFailed = 3,
                    NumStepsPartiallyCompleted = 4,
                    NumStepsTotal = 5
                }
            });

            Assert.IsNotNull(roStageData);
            Assert.AreEqual("stage_1", roStageData.Name);
            Assert.AreEqual("stage 1", roStageData.Description);
            Assert.AreEqual(2, roStageData.StepsDatas.Length);

            var stepData = roStageData.StepsDatas[0];
            Assert.AreEqual("step3", stepData.Name);
            Assert.AreEqual("step 3", stepData.Description);
            CollectionAssert.AreEqual(new List<string> { "step1", "step2" }, stepData.DependsOn.ToList());

            stepData = roStageData.StepsDatas[1];
            Assert.AreEqual("step5", stepData.Name);
            Assert.AreEqual("step 5", stepData.Description);
            CollectionAssert.AreEqual(new List<string> { "step3", "step4" }, stepData.DependsOn.ToList());

            Assert.AreEqual(1, roStageData.StatsResults.NumStepsCancelled);
            Assert.AreEqual(2, roStageData.StatsResults.NumStepsCompleted);
            Assert.AreEqual(3, roStageData.StatsResults.NumStepsFailed);
            Assert.AreEqual(4, roStageData.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(5, roStageData.StatsResults.NumStepsTotal);
        }

        [TestMethod]
        public void CreateStageData_InstanceIsImmutable()
        {
            var stage = new NullStage
            {
                Name = "stage_1",
                Description = "stage 1",
                StepsDatas = [
                    new DefaultPipelineStepData
                    {
                        Name = "step3",
                        Description = "step 3",
                        DependsOn = ["step1", "step2"]
                    },
                    new DefaultPipelineStepData
                    {
                        Name = "step5",
                        Description = "step 5",
                        DependsOn = ["step3", "step4"]
                    }
                ],
                StatsResults = new PipelineStepStatsResults
                {
                    NumStepsCancelled = 1,
                    NumStepsCompleted = 2,
                    NumStepsFailed = 3,
                    NumStepsPartiallyCompleted = 4,
                    NumStepsTotal = 5
                }
            };

            var roStageData = new ReadOnlyPipelineDataFactory().CreateStageData(stage);

            stage.Name = "stage_2";
            stage.Description = "stage 2";

            var stepData = stage.StepsDatas[0];
            stepData.Name = "step10";
            stepData.Description = "step 10";
            stepData.DependsOn[0] = "step5";
            stepData.DependsOn[1] = "step6";

            stepData = stage.StepsDatas[1];
            stepData.Name = "step11";
            stepData.Description = "step 11";
            stepData.DependsOn[0] = "step7";
            stepData.DependsOn[1] = "step8";

            stage.StatsResults.NumStepsCancelled = 0;
            stage.StatsResults.NumStepsCompleted = 0;
            stage.StatsResults.NumStepsFailed = 0;
            stage.StatsResults.NumStepsPartiallyCompleted = 0;
            stage.StatsResults.NumStepsTotal = 0;

            Assert.AreNotEqual(stage.Name, roStageData.Name);
            Assert.AreNotEqual(stage.Description, roStageData.Description);

            // Test that steps datas should be different.
            stepData = stage.StepsDatas[0];
            var roStepData = roStageData.StepsDatas[0];

            Assert.AreNotEqual(stepData.Name, roStepData.Name);
            Assert.AreNotEqual(stepData.Description, roStepData.Description);
            CollectionAssert.AreNotEqual(stepData.DependsOn, roStepData.DependsOn.ToList());

            stepData = stage.StepsDatas[1];
            roStepData = roStageData.StepsDatas[1];

            Assert.AreNotEqual(stepData.Name, roStepData.Name);
            Assert.AreNotEqual(stepData.Description, roStepData.Description);
            CollectionAssert.AreNotEqual(stepData.DependsOn, roStepData.DependsOn.ToList());

            // Test that stats should be different.
            Assert.AreNotEqual(stage.StatsResults.NumStepsCancelled, roStageData.StatsResults.NumStepsCancelled);
            Assert.AreNotEqual(stage.StatsResults.NumStepsCompleted, roStageData.StatsResults.NumStepsCompleted);
            Assert.AreNotEqual(stage.StatsResults.NumStepsFailed, roStageData.StatsResults.NumStepsFailed);
            Assert.AreNotEqual(stage.StatsResults.NumStepsPartiallyCompleted, roStageData.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreNotEqual(stage.StatsResults.NumStepsTotal, roStageData.StatsResults.NumStepsTotal);
        }
    }
}