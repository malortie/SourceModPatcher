using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;

namespace SourceModPatcher.Tests
{
    public class ConfigurationMock : IConfiguration
    {
        public int GetVariablesTotal { get; private set; } = 0;
        public int GetVariablesFileNameTotal { get; private set; } = 0;
        public int GetInstallVariablesTotal { get; private set; } = 0;
        public int GetSourceModNameTotal { get; private set; } = 0;
        public int GetSourceModFolderTotal { get; private set; } = 0;
        public int GetSourceModDirTotal { get; private set; } = 0;
        public int GetSourceModDataDirTotal { get; private set; } = 0;
        public int GetRequiredContentDependenciesTotal { get; private set; } = 0;
        public int GetOptionalContentDependenciesTotal { get; private set; } = 0;
        public int GetContentNameTotal { get; private set; } = 0;
        public int GetContentOutputVariablesTotal { get; private set; } = 0;

        public ReadOnlyDictionary<string, string> GetVariables()
        {
            ++GetVariablesTotal;
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        }

        public string GetVariablesFileName()
        {
            ++GetVariablesFileNameTotal;
            return string.Empty;
        }

        public ReadOnlyDictionary<string, string> GetInstallVariables()
        {
            ++GetInstallVariablesTotal;
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        }

        public string GetSourceModName(string key)
        {
            ++GetSourceModNameTotal;
            return string.Empty;
        }

        public string GetSourceModFolder(string key)
        {
            ++GetSourceModFolderTotal;
            return string.Empty;
        }

        public string GetSourceModDir(string key)
        {
            ++GetSourceModDirTotal;
            return string.Empty;
        }

        public string GetSourceModDataDir(string key)
        {
            ++GetSourceModDataDirTotal;
            return string.Empty;
        }

        public List<List<string>> GetRequiredContentDependencies(string sourcemodID)
        {
            ++GetRequiredContentDependenciesTotal;
            return [];
        }

        public List<List<string>> GetOptionalContentDependencies(string sourcemodID)
        {
            ++GetOptionalContentDependenciesTotal;
            return [];
        }

        public string GetContentName(string contentID)
        {
            ++GetContentNameTotal;
            return string.Empty;
        }

        public List<string> GetContentOutputVariables(string contentID)
        {
            ++GetContentOutputVariablesTotal;
            return [];
        }
    }

    [TestClass]
    public class TestContext
    {
        [TestMethod]
        public void Call_Configuration_GetVariables()
        {
            var configuration = new ConfigurationMock();
            var context = new Context(new MockFileSystem(), configuration);
            context.GetSourceContentVariables();
            Assert.AreEqual(1, configuration.GetVariablesTotal);
        }

        [TestMethod]
        public void Call_Configuration_GetVariablesFileName()
        {
            var configuration = new ConfigurationMock();
            var context = new Context(new MockFileSystem(), configuration);
            context.GetVariablesFileName();
            Assert.AreEqual(1, configuration.GetVariablesFileNameTotal);
        }

        [TestMethod]
        public void Call_Configuration_GetSourceModName()
        {
            var configuration = new ConfigurationMock();
            var context = new Context(new MockFileSystem(), configuration);
            context.GetSourceModName();
            Assert.AreEqual(1, configuration.GetSourceModNameTotal);
        }

        [TestMethod]
        public void Call_Configuration_GetSourceModFolder()
        {
            var configuration = new ConfigurationMock();
            var context = new Context(new MockFileSystem(), configuration);
            context.GetSourceModFolder();
            Assert.AreEqual(1, configuration.GetSourceModFolderTotal);
        }

        [TestMethod]
        public void Call_Configuration_GetSourceModDir()
        {
            var configuration = new ConfigurationMock();
            var context = new Context(new MockFileSystem(), configuration);
            context.GetSourceModDir();
            Assert.AreEqual(1, configuration.GetSourceModDirTotal);
        }

        [TestMethod]
        public void Call_Configuration_GetSourceModDataDir()
        {
            var configuration = new ConfigurationMock();
            var context = new Context(new MockFileSystem(), configuration);
            context.GetSourceModDataDir();
            Assert.AreEqual(1, configuration.GetSourceModDataDirTotal);
        }
    }
}