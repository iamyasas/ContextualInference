using Newtonsoft.Json.Linq;

namespace ContextualInferenceAPI.Models;

public class GenericRequest
{
    public required JObject ResponseSchema { get; set; }

    public required JObject Context { get; set; }

    public string? Instructions { get; set; }

}
