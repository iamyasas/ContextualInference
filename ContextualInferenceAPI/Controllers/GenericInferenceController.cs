using System.Text.Json;
using ContextualInferenceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace ContextualInferenceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericInferenceController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public GenericInferenceController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        public IActionResult InferenceJsonSchemaBasedOnContext([FromBody] GenericRequest request)
        {
            string? apiKey = configuration["Constants:OPENAI_API_KEY"];
            string? model = configuration["Constants:OPENAI_MODEL"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(model))
            {
                return Problem("An unexpected error occured.", statusCode: StatusCodes.Status500InternalServerError);
            }

            var prompt = string.Join(", ", request.Context.Select(kv => $"{kv.Key}: {kv.Value}"));

            ChatClient client = new ChatClient(model, apiKey);

            List<ChatMessage> messages = [
                new UserChatMessage(prompt)
            ];

            ChatCompletionOptions options = new ChatCompletionOptions()
            {
                Temperature = 0.1f,
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "GenericJsonSchema",
                    jsonSchema: BinaryData.FromString(request.ResponseJsonSchema.RootElement.ToString()),
                    jsonSchemaIsStrict: true
                )
            };

            ChatCompletion completion = client.CompleteChat(messages, options);

            string response = completion.Content[0].Text;

            JsonDocument jsonDocument = JsonDocument.Parse(response);

            return Ok(jsonDocument.RootElement);
        }
    }
}
