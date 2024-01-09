using test_installsourcecontent;

namespace test_installsourcecontent_tests
{
    [TestClass]
    public class TestInstallStepMapper
    {
        [TestMethod]
        public void Map_JSONExtractVPKInstallStep_To_ExtractVPKInstallStepData()
        {
            var stepMapper = new InstallStepMapper<JSONInstallStep>();
            var mappedStepData = (ExtractVPKInstallStepData)stepMapper.Map(new JSONExtractVPKInstallStep
            {
                Name = "step3",
                Description = "simple step",
                DependsOn = ["step1", "step2"],
                FilesToExclude = ["file1", "file2"],
                FilesToExtract = ["file3", "file4"],
                OutDir = "out",
                Vpks = [
                    new()
                    {
                        VPKFile = "test.vpk",
                        FilesToExclude = ["file5", "file6"],
                        FilesToExtract = ["file7", "file8"]
                    },
                    new()
                    {
                        VPKFile = "test2.vpk",
                        FilesToExclude = ["file9", "file10"],
                        FilesToExtract = ["file11", "file12"]
                    }
                ],
            });

            Assert.IsNotNull(mappedStepData);
            Assert.AreEqual("step3", mappedStepData.Name);
            Assert.AreEqual("simple step", mappedStepData.Description);
            CollectionAssert.AreEquivalent(new List<string> { "step1", "step2" }, mappedStepData.DependsOn);
            CollectionAssert.AreEquivalent(new List<string> { "file1", "file2" }, mappedStepData.FilesToExclude);
            CollectionAssert.AreEquivalent(new List<string> { "file3", "file4" }, mappedStepData.FilesToExtract);
            Assert.AreEqual("out", mappedStepData.OutDir);

            Assert.IsNotNull(mappedStepData.Vpks);
            Assert.AreEqual(2, mappedStepData.Vpks.Count);
            var vpk = mappedStepData.Vpks[0];
            Assert.AreEqual("test.vpk", vpk.VPKFile);
            CollectionAssert.AreEquivalent(new List<string> { "file5", "file6" }, vpk.FilesToExclude);
            CollectionAssert.AreEquivalent(new List<string> { "file7", "file8" }, vpk.FilesToExtract);
            vpk = mappedStepData.Vpks[1];
            Assert.AreEqual("test2.vpk", vpk.VPKFile);
            CollectionAssert.AreEquivalent(new List<string> { "file9", "file10" }, vpk.FilesToExclude);
            CollectionAssert.AreEquivalent(new List<string> { "file11", "file12" }, vpk.FilesToExtract);
        }

        [TestMethod]
        public void Map_JSONSaveVariableInstallStep_To_SaveVariableInstallStepData()
        {
            var stepMapper = new InstallStepMapper<JSONInstallStep>();
            var mappedStepData = (SaveVariableInstallStepData)stepMapper.Map(new JSONSaveVariableInstallStep
            {
                Name = "step3",
                Description = "simple step",
                DependsOn = ["step1", "step2"],
                VariableName = "variable1",
                VariableValue = "value1",
            });

            Assert.IsNotNull(mappedStepData);
            Assert.AreEqual("step3", mappedStepData.Name);
            Assert.AreEqual("simple step", mappedStepData.Description);
            CollectionAssert.AreEquivalent(new List<string> { "step1", "step2" }, mappedStepData.DependsOn);
            Assert.AreEqual("variable1", mappedStepData.VariableName);
            Assert.AreEqual("value1", mappedStepData.VariableValue);
        }
    }
}