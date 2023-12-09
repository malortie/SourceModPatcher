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
        IConfiguration _configuration;

        public ContextFactory(IFileSystem fileSystem, IConfiguration configuration)
        {
            _fileSystem = fileSystem;
            _configuration = configuration;
        }

        public Context CreateContext() 
        {
            return new Context(_fileSystem, _configuration);
        }

        public StepContext CreateStepContext(Context context)
        {
            return new StepContext(context);
        }
    }
}