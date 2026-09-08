using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TicketProcessor.Models;
using TicketProcessor.Repositories;

namespace TicketProcessor.Services
{
    public interface ITicketProcessorService
    {
        Task ProcesarTicketsPendientesAsync();
        Task ProcesarTicketsFallidosAsync();
    }

    public class TicketProcessorService : ITicketProcessorService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketClassifierService _clasificador;
        private readonly ILogger<TicketProcessorService> _logger;

        public TicketProcessorService(
            ITicketRepository ticketRepository,
            ITicketClassifierService clasificador,
            ILogger<TicketProcessorService> logger)
        {
            _ticketRepository = ticketRepository;
            _clasificador = clasificador;
            _logger = logger;
        }

        public async Task ProcesarTicketsPendientesAsync()
        {
            var ticketsPendientes = await _ticketRepository.ObtenerPendientesAsync();
            await ProcesarTicketsAsync(
                ticketsPendientes,
                "FALLIDO",
                ticketId => _ticketRepository.MarcarComoFallidoAsync(ticketId));
        }

        public async Task ProcesarTicketsFallidosAsync()
        {
            var ticketsFallidos = await _ticketRepository.ObtenerFallidosAsync();
            await ProcesarTicketsAsync(
                ticketsFallidos,
                "PENDIENTE_REVISAR",
                ticketId => _ticketRepository.MarcarComoPendienteRevisionAsync(ticketId));
        }

        private async Task ProcesarTicketsAsync(
            IEnumerable<Ticket> tickets,
            string estadoError,
            Func<int, Task> marcarErrorAsync)
        {
            foreach (var ticket in tickets)
            {
                try
                {
                    var categoria = await _clasificador.ClasificarTicketAsync(ticket.TextoTicket);
                    await _ticketRepository.MarcarComoProcesadoAsync(ticket.Id, categoria);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "El ticket {TicketId} no pudo procesarse y pasa a {Estado}.",
                        ticket.Id,
                        estadoError);

                    // Si no se puede guardar el fallo, la excepción debe llegar a Hangfire.
                    await marcarErrorAsync(ticket.Id);
                }
            }
        }
    }
}
