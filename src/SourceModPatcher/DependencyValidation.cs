namespace SourceModPatcher
{
    public class DependencyValidationContentEntry
    {
        public string ContentID { get; set; } = string.Empty;
        public List<string> MissingVariables { get; set; } = [];
    }

    public class DependencyValidationResult
    {
        public List<DependencyValidationContentEntry> EquivalentContent { get; set; } = [];
        public bool FulFilled { get; set; } = false;
    }

    public interface IDependencyValidation
    {
        DependencyValidationResult Validate(Context context, List<string> dependency);
    }

    public class DependencyValidation : IDependencyValidation
    {
        public DependencyValidationResult Validate(Context context, List<string> dependency)
        {
            var result = new DependencyValidationResult() { FulFilled = false };
            var sourceContentVariablesNames = context.GetSourceContentVariables().Keys.ToList();

            foreach (var contentID in dependency)
            {
                var outputVariables = context.GetContentOutputVariables(contentID);
                var missingVariables = outputVariables.Except(sourceContentVariablesNames).ToList();

                // If all variables are present in variables.json, mark this dependency as fulfilled.
                if (!missingVariables.Any())
                {
                    result.FulFilled = true;
                }

                result.EquivalentContent.Add(new()
                {
                    ContentID = contentID,
                    MissingVariables = missingVariables
                });
            }

            return result;
        }
    }
}
