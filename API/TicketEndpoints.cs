using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using TicketProcessor.Models;
using TicketProcessor.Services;

namespace TicketProcessor.API
{
    public static class TicketEndpoints
    {
        public static void MapTicketEndpoints(this IEndpointRouteBuilder app)
        {
            // 1. Obtener todos los tickets
            app.MapGet("/api/tickets", async (IConfiguration config) =>
            {
                using var db = new SqlConnection(config.GetConnectionString("BdConexion"));
                var sql = "SELECT Id, ClienteEmail, TextoTicket, CategoriaIA, Estado FROM TicketsSoporte ORDER BY Id DESC";
                var tickets = await db.QueryAsync<Ticket>(sql);
                return Results.Ok(tickets);
            });

            // 2. Clasificar un ticket nuevo al vuelo con IA
            app.MapPost("/api/tickets/clasificar", async (TicketRequest request, ITicketClassifierService aiService) =>
            {
                if (string.IsNullOrWhiteSpace(request.TextoTicket))
                    return Results.BadRequest(new { error = "El texto del ticket es obligatorio." });

                string categoria = await aiService.ClasificarTicketAsync(request.TextoTicket);
                return Results.Ok(new { ticketOriginal = request.TextoTicket, clasificacion = categoria });
            });


            app.MapPost("/api/tickets", async (TicketCreacion req, IConfiguration config) =>
            {
                using var db = new SqlConnection(config.GetConnectionString("BdConexion"));
                var sql = @"
                    INSERT INTO TicketsSoporte (ClienteEmail, TextoTicket, Estado, FechaCreacion) 
                    VALUES (@ClienteEmail, @TextoTicket, 'Pendiente', GETDATE())";
                
                await db.ExecuteAsync(sql, req);
                return Results.Ok(new { mensaje = "Ticket encolado para Hangfire" });
            });
        }


        
    }

    public record TicketRequest(string TextoTicket);

    public record TicketCreacion(string ClienteEmail, string TextoTicket);
    

}


