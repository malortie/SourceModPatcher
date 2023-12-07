namespace test_installsourcecontent
{
    public interface IWriter
    {
        void Write(string value);
        void WriteLine();
        void WriteLine(string value);
    }

    public class NullWriter : IWriter
    {
        public void Write(string value)
        {
        }
        public void WriteLine()
        {
        }
        public void WriteLine(string value)
        {
        }
    }

    public class ConsoleWriter : IWriter
    {
        public void Write(string value)
        { 
            Console.Write(value);
        }
        public void WriteLine()
        {
            Console.WriteLine();
        }
        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}