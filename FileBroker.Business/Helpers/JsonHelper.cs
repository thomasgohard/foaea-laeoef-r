using NJsonSchema;
using NJsonSchema.Validation;

namespace FileBroker.Business.Helpers
{
    public class JsonHelper
    {
        public static List<string> Validate<T>(string source)
        {
            var result = new List<string>();

            var schema = JsonSchema.FromType<T>();
            try
            {
                var errors = schema.Validate(source);

                foreach (ValidationError error in errors)
                {
                    string errorMessage = $"Schema error for {typeof(T).Name} at line {error.LinePosition}: {error.Property} [{error.Kind}] {error.Path}";
                    string errorDescription = error.ToString();
                    int pos = errorDescription.IndexOf("NoAdditionalPropertiesAllowed:");
                    if (pos != -1)
                    {
                        errorDescription = errorDescription[pos..];
                        int lineBreakPos = errorDescription.IndexOf("\n");
                        if (lineBreakPos != -1)
                        {
                            errorDescription = errorDescription[..lineBreakPos];
                            int lastDot = errorDescription.LastIndexOf(".");
                            if (lastDot != -1)
                            {
                                lastDot++;
                                string invalidProperty = errorDescription[lastDot..];
                                errorMessage = $"Schema warning for {typeof(T).Name} at line {error.LinePosition}: unknown tag [{invalidProperty}]";
                            }
                        }
                    }

                    result.Add(errorMessage);
                }
            }
            catch (Exception e)
            {
                result.Add(e.Message);
            }

            return result;
        }
    }
}
