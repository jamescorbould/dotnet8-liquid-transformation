using DotLiquid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text.Json;
using AuthorizationLevel = Microsoft.Azure.Functions.Worker.AuthorizationLevel;

namespace Liquid.Transformation
{
    public class Function
    {
        private readonly ILogger<Function> _logger;

        public Function(ILogger<Function> logger)
        {
            _logger = logger;
        }

        [Function("Function")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string renderedTemplate = string.Empty;
            Dictionary<string, object> dictionary;

            // Read the JSON data from the request body.
            using (StreamReader reader = new StreamReader(req.Body))
            {
                string jsonString = await reader.ReadToEndAsync();

                // The template would be stored in blob storage.
                var template = Template.Parse("{ \"kimName\": \"{{ name }}\" }");

                _logger.LogInformation($"jsonString = {jsonString}");

                dynamic expandoObj = JsonConvert.DeserializeObject<ExpandoObject>(jsonString, new ExpandoObjectConverter());
                IDictionary<string, object> expandoDict = new Dictionary<string, object>(expandoObj);

                // Create a Hash object to store the data.
                var data = Hash.FromDictionary(expandoDict);

                // Render the Liquid template with the injected JSON data.
                renderedTemplate = template.Render(data);
            }

            return new OkObjectResult(renderedTemplate);
        }
    }
}