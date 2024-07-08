namespace SourceModPatcher.Tests
{
    [TestClass]
    public class TestInstallStepMapper
    {
        [TestMethod]
        public void Map_JSONCopyFilesInstallStep_To_CopyFilesInstallStepData()
        {
            var stepMapper = new InstallStepMapper<JSONInstallStep>();
            var mappedStepData = (CopyFilesInstallStepData)stepMapper.Map(new JSONCopyFilesInstallStep
            {
                Name = "step3",
                Description = "simple step",
                DependsOn = ["step1", "step2"],
                Files = [
                    new()
                    {
                        Source = "source1",
                        Destination = "destination1",
                    },
                    new()
                    {
                        Source = "source2",
                        Destination = "destination2",
                    }
                ]
            });

            Assert.IsNotNull(mappedStepData);
            Assert.AreEqual("step3", mappedStepData.Name);
            Assert.AreEqual("simple step", mappedStepData.Description);
            CollectionAssert.AreEquivalent(new List<string> { "step1", "step2" }, mappedStepData.DependsOn);

            Assert.IsNotNull(mappedStepData.Files);
            Assert.AreEqual(2, mappedStepData.Files.Count);
            var file = mappedStepData.Files[0];
            Assert.AreEqual("source1", file.Source);
            Assert.AreEqual("destination1", file.Destination);
            file = mappedStepData.Files[1];
            Assert.AreEqual("source2", file.Source);
            Assert.AreEqual("destination2", file.Destination);
        }

        [TestMethod]
        public void Map_JSONReplaceTokensInstallStep_To_ReplaceTokensInstallStepData()
        {
            var stepMapper = new InstallStepMapper<JSONInstallStep>();
            var mappedStepData = (ReplaceTokensInstallStepData)stepMapper.Map(new JSONReplaceTokensInstallStep
            {
                Name = "step3",
                Description = "simple step",
                DependsOn = ["step1", "step2"],
                Files = [
                    "file1.txt",
                    "file2.txt"
                ]
            });

            Assert.IsNotNull(mappedStepData);
            Assert.AreEqual("step3", mappedStepData.Name);
            Assert.AreEqual("simple step", mappedStepData.Description);
            CollectionAssert.AreEquivalent(new List<string> { "step1", "step2" }, mappedStepData.DependsOn);
            CollectionAssert.AreEquivalent(new List<string> { "file1.txt", "file2.txt" }, mappedStepData.Files);
        }

        [TestMethod]
        public void Map_JSONValidateVariablesDependenciesInstallStep_To_ValidateVariablesDependenciesInstallStepData()
        {
            var stepMapper = new InstallStepMapper<JSONInstallStep>();
            var mappedStepData = (ValidateVariablesDependenciesInstallStepData)stepMapper.Map(new JSONValidateVariablesDependenciesInstallStep
            {
                Name = "step3",
                Description = "simple step",
                DependsOn = ["step1", "step2"]
            });

            Assert.IsNotNull(mappedStepData);
            Assert.AreEqual("step3", mappedStepData.Name);
            Assert.AreEqual("simple step", mappedStepData.Description);
            CollectionAssert.AreEquivalent(new List<string> { "step1", "step2" }, mappedStepData.DependsOn);
        }
    }
}