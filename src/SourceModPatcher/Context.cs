using SourceContentInstaller;
using System.Collections.ObjectModel;
using System.IO.Abstractions;

namespace SourceModPatcher
{
    public class Context(IFileSystem fileSystem, IConfiguration configuration)
    {
        readonly IFileSystem _fileSystem = fileSystem;
        readonly IConfiguration _configuration = configuration;

        public IFileSystem FileSystem { get { return _fileSystem; } }

        public string SourceModID { get; set; } = string.Empty;

        public virtual ReadOnlyDictionary<string, string> GetSourceContentVariables()
        {
            return _configuration.GetVariables();
        }

        public string GetVariablesFileName()
        {
            return _configuration.GetVariablesFileName();
        }

        public string GetSourceModName()
        {
            return _configuration.GetSourceModName(SourceModID);
        }

        public string GetSourceModFolder()
        {
            return _configuration.GetSourceModFolder(SourceModID);
        }

        public string GetSourceModDir()
        {
            return _configuration.GetSourceModDir(SourceModID);
        }

        public string GetSourceModDataDir()
        {
            return _configuration.GetSourceModDataDir(SourceModID);
        }

        public List<List<string>> GetRequiredContentDependencies()
        {
            return _configuration.GetRequiredContentDependencies(SourceModID);
        }

        public List<List<string>> GetOptionalContentDependencies()
        {
            return _configuration.GetOptionalContentDependencies(SourceModID);
        }

        public string GetContentName(string contentID)
        {
            return _configuration.GetContentName(contentID);
        }

        public List<string> GetContentOutputVariables(string contentID)
        {
            return _configuration.GetContentOutputVariables(contentID);
        }
    }
}