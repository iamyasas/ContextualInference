using System.Text.Json;
using ContextualInferenceAPI.Models;
using ContextualInferenceAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace ContextualInferenceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationInferenceController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public LocationInferenceController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        public IActionResult FillMissingFieldsKV([FromForm] Dictionary<string, string> context)
        {
            string? apiKey = configuration["Constants:OPENAI_API_KEY"];
            string? model = configuration["Constants:OPENAI_MODEL"];

            var prompt = "";

            foreach (var kv in context)
            {
                prompt += kv.Key + ": " + kv.Value + ", ";
            }

            ChatClient client = new ChatClient(model, apiKey);

            List<ChatMessage> messages = [
                new UserChatMessage(prompt)
            ];

            ChatCompletionOptions options = new ChatCompletionOptions()
            {
                Temperature = 0.1f,
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: typeof(LocationInfo).Name,
                    jsonSchema: BinaryData.FromString(JsonSchemeGenerator.GenerateJsonSchema(typeof(LocationInfo))),
                    jsonSchemaIsStrict: true
                )
            };

            ChatCompletion completion = client.CompleteChat(messages, options);

            string response = completion.Content[0].Text;

            LocationInfo? locationInfo = JsonSerializer.Deserialize<LocationInfo>(response);

            return Ok(locationInfo);
        }
    }
}
