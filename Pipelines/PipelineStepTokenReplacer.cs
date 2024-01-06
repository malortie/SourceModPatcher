using System.Collections;
using System.Reflection;

namespace Pipelines
{
    public class PipelineStepReplaceTokenPropertiesGetter : IPropertiesGetter
    {
        public IEnumerable<PropertyInfo> GetProperties(object obj)
        {
            return obj.GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(PipelineStepReplaceTokenAttribute)));
        }
    }

    public interface IPipelineStepTokenReplacer
    {
        IPropertiesGetter PropertiesGetter { get; set; }
 
        void Replace(object obj, ITokenReplacer tokenReplacer);
    }

    public class PipelineStepTokenReplacer : IPipelineStepTokenReplacer
    {
        public IPropertiesGetter PropertiesGetter { get; set; } = new PipelineStepReplaceTokenPropertiesGetter();

        public void Replace(object obj, ITokenReplacer tokenReplacer)
        {
            var propsWithReplacetoken = PropertiesGetter.GetProperties(obj);

            foreach (PropertyInfo prop in propsWithReplacetoken)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (type == typeof(string))
                {
                    string? propertyValue = prop.GetValue(obj) as string;
                    if (null != propertyValue)
                        prop.SetValue(obj, tokenReplacer.Replace(propertyValue) ?? propertyValue);
                }
                else
                {
                    var propertyValue = prop.GetValue(obj);
                    if (null != propertyValue)
                    {
                        var enumerable = propertyValue as IEnumerable;
                        if (null != enumerable)
                        {
                            if (enumerable is IEnumerable<string>)
                            {
                                // Replace tokens in string collections.
                                var newStrings = new List<string>();
                                foreach (string str in enumerable)
                                    newStrings.Add(tokenReplacer.Replace(str) ?? str);
                                prop.SetValue(obj, newStrings);
                            }
                            else
                            {
                                foreach (var item in enumerable)
                                    Replace(item, tokenReplacer);
                            }
                        }
                        else
                            Replace(propertyValue, tokenReplacer);
                    }
                }
            }
        }
    }
}