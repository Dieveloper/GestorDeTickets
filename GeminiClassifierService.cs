using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Google.GenAI;

namespace TicketProcessor.Services
{
    public class GeminiClassifierService : ITicketClassifierService
    {
        private readonly string _apiKey;
        private const string ModelName = "gemini-3.5-flash"; 

        public GeminiClassifierService(IConfiguration config)
        {
            _apiKey = config["GeminiApiKey"]?.Trim() ?? throw new ArgumentNullException("Falta la API Key");
        }

        public async Task<string> ClasificarTicketAsync(string textoTicket)
        {
            if (string.IsNullOrWhiteSpace(textoTicket)) return "SOPORTE";

            var client = new Client(apiKey: _apiKey);

            var prompt = $@"Eres una API de clasificación estricta. Tu única función es devolver una sola palabra en mayúsculas, sin espacios, sin puntos y sin texto adicional.

            Reglas de clasificación:
            - SOPORTE: Problemas técnicos, contraseñas, errores HTTP, lentitud de web, hardware o software.
            - OFICINA: Facturación, dinero, recibos, cobros, nóminas, datos de pedidos, administración o envíos.

            Ejemplos históricos:
            Texto: El sistema no me deja hacer login, da error 500.
            Clasificación: SOPORTE
            Texto: Hola, la factura de este mes me ha llegado duplicada.
            Clasificación: OFICINA

            Texto actual: {textoTicket}
            Clasificación:";

            try 
            {
                var response = await client.Models.GenerateContentAsync(
                    model: ModelName, 
                    contents: prompt
                );

                var resultadoIa = response.Text?.Replace(".", "").Replace("\"", "").Trim().ToUpper();

                if (resultadoIa == "OFICINA" || resultadoIa == "SOPORTE")
                {
                    Console.WriteLine($"[DEBUG] Clasificado correctamente como: {resultadoIa}");
                    return resultadoIa;
                }

                return "SOPORTE"; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de IA: {ex.Message}");
                return "SOPORTE";
            }
        }
    }
}