using TicketProcessor.Models;

namespace TicketProcessor.Repositories;

public interface ITicketRepository
{
    Task<IEnumerable<Ticket>> ObtenerTodosAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Ticket>> ObtenerPendientesAsync(CancellationToken cancellationToken = default);

    Task CrearAsync(
        string clienteEmail,
        string textoTicket,
        CancellationToken cancellationToken = default);

    Task MarcarComoProcesadoAsync(
        int ticketId,
        string categoriaIA,
        CancellationToken cancellationToken = default);
}
