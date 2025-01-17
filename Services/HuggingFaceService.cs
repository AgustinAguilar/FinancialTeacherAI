using System.Text;
using FinancialTeacherAI.Services.DTO;
using FinancialTeacherAI.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;

namespace FinancialTeacherAI.Services
{
    public class HuggingFaceService : IHuggingFaceService
    {
        private readonly string _huggingFaceApiUrl;
        private readonly string _huggingFaceApiKey;
        public HuggingFaceService(IConfiguration configuration)
        {
            _huggingFaceApiUrl = configuration["HuggingFace:ApiUrl"] ?? throw new ArgumentNullException("HuggingFace:ApiUrl");
            _huggingFaceApiKey = configuration["HuggingFace:ApiKey"] ?? throw new ArgumentNullException("HuggingFace:ApiKey");
        }

        public async Task<string> GenerateEmbeddingsForChunk(Input inputPayload)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_huggingFaceApiKey}");
            string json = JsonConvert.SerializeObject(new { inputs = inputPayload });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(_huggingFaceApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Embedding generado:");
                Console.WriteLine(result);
                return result;
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error al procesar el chunk:");
                Console.WriteLine(error);
                throw new Exception(error);
            }
        }
    }
}
