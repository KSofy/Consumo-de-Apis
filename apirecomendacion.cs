using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiEjercicio
{
    public class apirecomendacion
    {
        private readonly string apiKey = ""; //clavedeapikey
        private readonly HttpClient httpClient;

        public apirecomendacion()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public async Task<string> ObtenerRecomendacion(string climaDescripcion)
        {
            var prompt = $"Soy un asistente inteligente. El clima actual es: {climaDescripcion}. ¿Qué me recomendarías hacer o usar hoy?";
            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[] {
                new { role = "system", content = "Eres un asistente que da recomendaciones basadas en el clima." },
                new { role = "user", content = prompt }
            }
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json);
                return result.choices[0].message.content.ToString();
            }

            return "No se pudo obtener una recomendación.";
        }
    }
}





