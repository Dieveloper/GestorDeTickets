using System;
using System.Threading.Tasks;
using TicketProcessor.Repositories;

namespace TicketProcessor.Services
{
    public interface ITicketProcessorService
    {
        Task ProcesarTicketsPendientesAsync();
    }

    public class TicketProcessorService : ITicketProcessorService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketClassifierService _clasificador;

        public TicketProcessorService(
            ITicketRepository ticketRepository,
            ITicketClassifierService clasificador)
        {
            _ticketRepository = ticketRepository;
            _clasificador = clasificador;
        }

        public async Task ProcesarTicketsPendientesAsync()
        {
            var ticketsPendientes = await _ticketRepository.ObtenerPendientesAsync();
            
            foreach (var ticket in ticketsPendientes)
            {
                Console.WriteLine($"Procesando ticket ID {ticket.Id}...");

                
                string textoDetectado = await _clasificador.ClasificarTicketAsync(ticket.TextoTicket);
                await _ticketRepository.MarcarComoProcesadoAsync(ticket.Id, textoDetectado);
                
            }
        }
    }
}
