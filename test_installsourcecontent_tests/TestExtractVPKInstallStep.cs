using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using test_installsourcecontent;

namespace test_installsourcecontent_tests
{
    public class NullConfiguration : IConfiguration
    {
        public NullConfiguration()
        {
        }

        public string GetSteamAppName(int AppID)
        {
            return string.Empty;
        }

        public string GetSteamAppInstallDir(int AppID)
        {
            return string.Empty;
        }

        public void SaveVariable(string name, string value)
        {
        }

        public string GetContentInstallDir(int AppID)
        {
            return string.Empty;
        }

        public string GetVariablesFileName()
        {
            return string.Empty;
        }
    }


    public class NullContext : Context
    {
        public NullContext(IFileSystem fileSystem, IConfiguration configuration) : base(fileSystem, configuration)
        {
        }
    }

    [TestClass]
    public class TestExtractVPKInstallStep
    {
        static IWriter NullWriter = new NullWriter();

        [TestMethod]
        public void EmptyVPKsReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{});

            var extractor = new VPKExtractor();
            var step = new ExtractVPKInstallStep(extractor);
            var stepData =  new ExtractVPKInstallStepData();

            CollectionAssert.AreEquivalent(new List<string>(), stepData.Vpks);

            var result = step.DoStep(new Context(fileSystem, new NullConfiguration()), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
        }

        [TestMethod]
        public void EmptyOutDirReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });

            var extractor = new VPKExtractor();
            var step = new ExtractVPKInstallStep(extractor);
            var stepData = new ExtractVPKInstallStepData() { 
                Vpks = [
                    new ExtractVPKInstallStepDataVPK { VPKFile = "test" }
                ]
            };

            CollectionAssert.AreNotEquivalent(new List<string>(), stepData.Vpks);
            Assert.AreNotEqual(string.Empty, stepData.Vpks.First().VPKFile);
            Assert.AreEqual(string.Empty, stepData.OutDir);

            var result = step.DoStep(new Context(fileSystem, new NullConfiguration()), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
        }
    }
}