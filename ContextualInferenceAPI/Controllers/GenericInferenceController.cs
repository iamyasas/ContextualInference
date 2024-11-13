using System.Text.Json;
using ContextualInferenceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
            var prompt = JObject.FromObject(request);

            if (string.IsNullOrWhiteSpace(request.Instructions))
            {
                prompt["Instructions"] = "Complete all fields in the 'ResponseSchema' accurately based on the provided descriptions and 'Context'. " +
                                         "Each field's description specifies the intended type of information and purpose. " +
                                         "Ensure the generated values align with these descriptions.";
            }

            Console.WriteLine(prompt.ToString());

            ChatClient client = new ChatClient(model, apiKey);

            List<ChatMessage> messages = [
                new UserChatMessage(prompt.ToString())
            ];

            var options = new ChatCompletionOptions()
            {
                Temperature = 0.1f,
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "DynamicJsonSchema",
                    jsonSchema: BinaryData.FromString(request.ResponseSchema.ToString()),
                    jsonSchemaIsStrict: true
                )
            };

            ChatCompletion completion = client.CompleteChat(messages, options);

            var completedDataJson = JObject.Parse(completion.Content[0].Text);

            Console.WriteLine(JObject.FromObject(completion.Usage).ToString());

            return Ok(completedDataJson);
        }
    }
}
