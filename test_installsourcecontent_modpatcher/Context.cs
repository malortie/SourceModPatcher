using System.Collections.ObjectModel;
using System.IO.Abstractions;
using test_installsourcecontent;

namespace test_installsourcecontent_modpatcher
{
    public class Context
    {
        IFileSystem _fileSystem;
        IConfiguration _configuration;

        public Context(IFileSystem fileSystem, IConfiguration configuration)
        {
            _fileSystem = fileSystem;
            _configuration = configuration;
        }

        public IFileSystem FileSystem { get { return _fileSystem; } }

        public string SourceModKey { get; set; } = string.Empty;

        public bool HasSourceContentVariable(string variableName)
        {
            return _configuration.HasVariable(variableName);
        }

        public ReadOnlyDictionary<string, string> GetSourceContentVariables()
        {
            return _configuration.GetVariables();
        }

        public ReadOnlyDictionary<string, string> GetInstallVariables()
        {
            return _configuration.GetInstallVariables();
        }

        public string GetSourceModName()
        {
            return _configuration.GetSourceModName(SourceModKey);
        }

        public string GetSourceModFolder()
        {
            return _configuration.GetSourceModFolder(SourceModKey);
        }

        public string GetSourceModDir()
        {
            return _configuration.GetSourceModDir(SourceModKey);
        }

        public string GetSourceModDataDir()
        {
            return _configuration.GetSourceModDataDir(SourceModKey);
        }
    }
}