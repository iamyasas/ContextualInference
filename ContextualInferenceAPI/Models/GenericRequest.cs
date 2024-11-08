using System;

namespace ContextualInferenceAPI.Models;

public class GenericRequest
{
    public required string ResponseJsonSchema { get; set; }

    public required Dictionary<string, string> Context { get; set; }

}
