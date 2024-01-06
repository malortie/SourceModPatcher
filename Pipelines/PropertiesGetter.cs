using System.Reflection;

namespace Pipelines
{
    public interface IPropertiesGetter
    {
        IEnumerable<PropertyInfo> GetProperties(object obj);
    }
}