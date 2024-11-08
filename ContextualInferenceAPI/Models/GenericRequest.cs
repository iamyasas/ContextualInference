using System.Text.Json;

namespace ContextualInferenceAPI.Models;

public class GenericRequest
{
    public required JsonDocument ResponseJsonSchema { get; set; }

    public required Dictionary<string, string> Context { get; set; }

}
