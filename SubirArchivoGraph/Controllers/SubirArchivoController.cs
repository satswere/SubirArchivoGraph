using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubirArchivoGraph.Models;
using System.Net;
using System.Net.Http.Headers;

using System.Net;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;


namespace SubirArchivoGraph.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubirArchivoController : ControllerBase
    {

        private readonly AuthenticationConfigModel _config = AuthenticationConfigModel.ReadFromJsonFile("appsettings.json");

        [HttpPost]
        [Route("subir-archivo")]
        public async Task<IActionResult> SubirArchivo(SubirArchivoRequest data)
        {
            try
            {
                var currentToken = await Make(_config);

                string url = $"https://graph.microsoft.com/v1.0/drives/{_config.DriveId}/items/{_config.ItemID}:/{data.Nombre}.{data.TipoDearchivo}:/content";
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Put, url);
                request.Headers.Add("Authorization", currentToken);

                // Convert the base64 string to a byte array
                var base64Content = data.Archivo;
                var byteArray = Convert.FromBase64String(base64Content);

                var content = new ByteArrayContent(byteArray); // Use ByteArrayContent to send the file content as a byte array
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream"); // Set the correct content type
                request.Content = content;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                // Deserialize the response into a JSON object
                var responseObject = System.Text.Json.JsonSerializer.Deserialize<object>(responseContent);


                // Return JSON response
                return new JsonResult(new
                {
                    message = HttpStatusCode.OK.ToString(),
                    code = HttpStatusCode.OK,
                    response = responseObject
                });
            }
            catch (Exception ex)
            {
                var jOb = new
                {
                    message = HttpStatusCode.BadRequest.ToString(),
                    code = HttpStatusCode.BadRequest,
                    response = ex.Message.ToString()
                };

                return BadRequest(jOb);
            }

        }

        public static async Task<string> Make(AuthenticationConfigModel config)
        {
            try
            {

                var clientApplication = ConfidentialClientApplicationBuilder
                    .Create(config.ClientId)
                    .WithClientSecret(config.ClientSecret)
                    .WithAuthority(AzureCloudInstance.AzurePublic, config.Tenant)
                    .Build();

                var scopes = new[] { "https://graph.microsoft.com/.default" };

                var result = await clientApplication.AcquireTokenForClient(scopes)
                    .ExecuteAsync();

                string AccessToken = result.AccessToken;


                return AccessToken;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
