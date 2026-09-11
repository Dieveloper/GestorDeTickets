using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Google.GenAI;
using Google.GenAI.Types;

namespace TicketProcessor.Services
{
    public class GeminiClassifierService : ITicketClassifierService
    {
        private readonly string _apiKey;
        private readonly ILogger<GeminiClassifierService> _logger;
        private readonly ClientOptions? _clientOptions;
        private const string ModelName = "gemini-3.6-flash"; 

        public GeminiClassifierService(IConfiguration config, ILogger<GeminiClassifierService> logger)
            : this(config, logger, null)
        {
        }

        internal GeminiClassifierService(
            IConfiguration config,
            ILogger<GeminiClassifierService> logger,
            ClientOptions? clientOptions)
        {
            _apiKey = config["GeminiApiKey"]?.Trim() ?? throw new ArgumentNullException("Falta la API Key");
            _logger = logger;
            _clientOptions = clientOptions;
        }

        public async Task<string> ClasificarTicketAsync(string textoTicket)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(textoTicket);

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
                await using var client = new Client(apiKey: _apiKey, clientOptions: _clientOptions);
                var response = await client.Models.GenerateContentAsync(
                    model: ModelName, 
                    contents: prompt
                );

                var resultadoIa = response.Text?.Replace(".", "").Replace("\"", "").Trim().ToUpperInvariant();

                if (resultadoIa == "OFICINA" || resultadoIa == "SOPORTE")
                {
                    _logger.LogInformation(
                        "Ticket clasificado por Gemini con el modelo {Model}: {Category}.",
                        ModelName, resultadoIa);
                    return resultadoIa;
                }

                throw new InvalidOperationException("Gemini no devolvió una categoría de clasificación válida.");
            }
            catch (Exception ex)
            {
                // Los mensajes y cuerpos de error del proveedor pueden contener datos sensibles.
                _logger.LogError(
                    "Error al clasificar el ticket con Gemini. Modelo: {Model}. Tipo: {ExceptionType}.",
                    ModelName, ex.GetType().Name);
                throw;
            }
        }
    }
}
