using System.IO.Abstractions;

namespace test_installsourcecontent
{
    public interface IContextFactory
    {
        Context CreateContext();
        StepContext CreateStepContext(Context context);
    }

    public class ContextFactory : IContextFactory
    {
        IFileSystem _fileSystem;
        IWriter _writer;
        IConfiguration _configuration;

        public ContextFactory(IFileSystem fileSystem, IWriter writer, IConfiguration configuration)
        {
            _fileSystem = fileSystem;
            _writer = writer;
            _configuration = configuration;
        }

        public Context CreateContext() 
        {
            return new Context(_fileSystem, _writer, _configuration);
        }

        public StepContext CreateStepContext(Context context)
        {
            return new StepContext(context);
        }
    }
}