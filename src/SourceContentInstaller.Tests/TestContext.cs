using System.IO.Abstractions.TestingHelpers;

namespace SourceContentInstaller.Tests
{
    public class ConfigurationMock : IConfiguration
    {
        public int GetSteamAppNameTotal { get; private set; } = 0;
        public int GetSteamAppInstallDirTotal { get; private set; } = 0;
        public int GetSteamAppsInstallDirVariablesTotal { get; private set; } = 0;
        public int SaveVariableTotal { get; private set; } = 0;
        public int GetContentNameTotal { get; private set; } = 0;
        public int GetContentInstallDirTotal { get; private set; } = 0;
        public int GetVariablesFileNameTotal { get; private set; } = 0;

        public string GetSteamAppName(int AppID)
        {
            ++GetSteamAppNameTotal;
            return string.Empty;
        }

        public string GetSteamAppInstallDir(int AppID)
        {
            ++GetSteamAppInstallDirTotal;
            return string.Empty;
        }

        public virtual Dictionary<string, string> GetSteamAppsInstallDirVariables()
        {
            ++GetSteamAppsInstallDirVariablesTotal;
            return new();
        }

        public void SaveVariable(string name, string value)
        {
            ++SaveVariableTotal;
        }
        public string GetContentName(string ContentID)
        {
            ++GetContentNameTotal;
            return string.Empty;
        }

        public string GetContentInstallDir(string ContentID)
        {
            ++GetContentInstallDirTotal;
            return string.Empty;
        }

        public string GetVariablesFileName()
        {
            ++GetVariablesFileNameTotal;
            return string.Empty;
        }
    }

    [TestClass]
    public class TestContext
    {
        [TestMethod]
        public void Call_Configuration_GetSteamAppName()
        {
            var configuration = new ConfigurationMock();
            var context = new Context(new MockFileSystem(), configuration);
            context.GetSteamAppName(0);
            Assert.AreEqual(1, configuration.GetSteamAppNameTotal);
        }

        [TestMethod]
        public void Call_Configuration_GetSteamAppInstallDir()
        {
            var configuration = new ConfigurationMock();
            var context = new Context(new MockFileSystem(), configuration);
            context.GetSteamAppInstallDir(0);
            Assert.AreEqual(1, configuration.GetSteamAppInstallDirTotal);
        }

        [TestMethod]
        public void Call_Configuration_SaveVariable()
        {
            var configuration = new ConfigurationMock();
            var context = new Context(new MockFileSystem(), configuration);
            context.SaveVariable("hl2_content_path", "C:/Half-Life 2");
            Assert.AreEqual(1, configuration.SaveVariableTotal);
        }

        [TestMethod]
        public void Call_Configuration_GetContentName()
        {
            var configuration = new ConfigurationMock();
            var context = new Context(new MockFileSystem(), configuration);
            context.GetContentName();
            Assert.AreEqual(1, configuration.GetContentNameTotal);
        }

        [TestMethod]
        public void Call_Configuration_GetContentInstallDir()
        {
            var configuration = new ConfigurationMock();
            var context = new Context(new MockFileSystem(), configuration);
            context.GetContentInstallDir();
            Assert.AreEqual(1, configuration.GetContentInstallDirTotal);
        }

        [TestMethod]
        public void Call_Configuration_GetVariablesFileName()
        {
            var configuration = new ConfigurationMock();
            var context = new Context(new MockFileSystem(), configuration);
            context.GetVariablesFileName();
            Assert.AreEqual(1, configuration.GetVariablesFileNameTotal);
        }
    }
}