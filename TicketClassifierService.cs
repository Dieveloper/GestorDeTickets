using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TicketProcessor.Services
{
    public interface ITicketClassifierService
    {
        Task<string> ClasificarTicketAsync(string textoTicket);
    }

    public class DummyClassifierService : ITicketClassifierService
    {
        public Task<string> ClasificarTicketAsync(string textoTicket)
        {
            if (string.IsNullOrWhiteSpace(textoTicket)) 
                return Task.FromResult("SOPORTE");

            List<string> palabrasOficina = new List<string>() { "dinero", "plata", "factura" };
            string[] palabras = Regex.Split(textoTicket.Trim().ToLower(), @"\s+");

            foreach (string palabra in palabras)
            {
                if (palabrasOficina.Contains(palabra))
                {
                    return Task.FromResult("OFICINA");
                }
            }
            
            return Task.FromResult("SOPORTE");
        }
    }
}