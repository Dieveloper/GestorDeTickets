using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TicketProcessor.Repositories;
using TicketProcessor.Services;

namespace TicketProcessor.API
{
    public static class TicketEndpoints
    {
        public static void MapTicketEndpoints(this IEndpointRouteBuilder app)
        {
            // 1. Obtener todos los tickets
            app.MapGet("/api/tickets", async (ITicketRepository ticketRepository, CancellationToken cancellationToken) =>
            {
                var tickets = await ticketRepository.ObtenerTodosAsync(cancellationToken);
                return Results.Ok(tickets);
            });

            // 2. Clasificar un ticket nuevo mediante IA
            app.MapPost("/api/tickets/clasificar", async (TicketRequest request, ITicketClassifierService aiService) =>
            {
                if (string.IsNullOrWhiteSpace(request.TextoTicket))
                    return Results.BadRequest(new { error = "El texto del ticket es obligatorio." });

                string categoria = await aiService.ClasificarTicketAsync(request.TextoTicket);
                return Results.Ok(new { ticketOriginal = request.TextoTicket, clasificacion = categoria });
            });


            app.MapPost("/api/tickets", async (TicketCreacion req, ITicketRepository ticketRepository, CancellationToken cancellationToken) =>
            {
                await ticketRepository.CrearAsync(req.ClienteEmail, req.TextoTicket, cancellationToken);
                return Results.Ok(new { mensaje = "Ticket encolado para Hangfire" });
            });
        }


        
    }

    public record TicketRequest(string TextoTicket);

    public record TicketCreacion(string ClienteEmail, string TextoTicket);
    

}
