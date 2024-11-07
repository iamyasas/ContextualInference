using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace ContextualInferenceAPI.Utils;

public class JsonSchemeGenerator
{
    public static string GenerateJsonSchema(Type type)
    {
        var schemaGenerator = new JSchemaGenerator
        {
            DefaultRequired = Required.Always,
        };

        JSchema schema = schemaGenerator.Generate(type);
        schema.AllowAdditionalProperties = false;

        return schema.ToString();
    }
}
