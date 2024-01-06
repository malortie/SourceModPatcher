using Pipelines;
using System.Collections.ObjectModel;
using System.IO.Abstractions;

namespace test_installsourcecontent_modpatcher
{
    public interface IFileTokenReplacer
    {
        ReadOnlyDictionary<string, string> Variables { get; set; }
        bool ReplaceInFile(IFileSystem fileSystem, IWriter writer, string filePath);
    }

    public class FileTokenReplacer : IFileTokenReplacer
    {
        ITokenReplacer _tokenReplacer;

        public FileTokenReplacer(ITokenReplacer tokenReplacer)
        {
            _tokenReplacer = tokenReplacer;
        }

        public ReadOnlyDictionary<string, string> Variables { 
            get { return _tokenReplacer.Variables; }
            set { _tokenReplacer.Variables = value; }
        }

        public bool ReplaceInFile(IFileSystem fileSystem, IWriter writer, string filePath)
        {
            try
            {
                // Replace variables tokens in file.
                var text = fileSystem.File.ReadAllText(filePath);
                text = _tokenReplacer.Replace(text);
                fileSystem.File.WriteAllText(filePath, text);

                // Check that the content of the file has been modified as intended.
                if (!text.Equals(fileSystem.File.ReadAllText(filePath)))
                    return false;

                // File updated successfully.
                return true;
            }
            catch (Exception e)
            {
                writer.Error(e.Message);
                return false;
            }
        }
    }
}
