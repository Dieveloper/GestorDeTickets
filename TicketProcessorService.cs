using Microsoft.Data.SqlClient;
using Dapper;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TicketProcessor.Models; 

namespace TicketProcessor.Services
{
    public interface ITicketProcessorService
    {
        Task ProcesarTicketsPendientesAsync();
    }

    public class TicketProcessorService : ITicketProcessorService
    {
        private readonly string _conexion;
        private readonly ITicketClassifierService _clasificador;

        // INYECCIÓN: Pedimos la configuración Y nuestro oráculo
        public TicketProcessorService(IConfiguration config, ITicketClassifierService clasificador)
        {
            _conexion = config.GetConnectionString("BdConexion");
            _clasificador = clasificador;
        }

        public async Task ProcesarTicketsPendientesAsync()
        {
            using var db = new SqlConnection(_conexion);
            var consultaSql = "SELECT * FROM TicketsSoporte WHERE Estado = 'Pendiente';";
            var ticketsPendientes = await db.QueryAsync<Ticket>(consultaSql); 
            
            foreach (var ticket in ticketsPendientes)
            {
                Console.WriteLine($"Procesando ticket ID {ticket.Id}...");

                
                string textoDetectado = await _clasificador.ClasificarTicketAsync(ticket.TextoTicket);
                var sqlUpdate = "UPDATE TicketsSoporte SET ESTADO = 'PROCESADO', CategoriaIA = @CategoriaIA, FechaProcesamiento = GETDATE() WHERE ID = @ID";
                await db.ExecuteAsync(sqlUpdate, new { 
                CategoriaIA = textoDetectado, 
                ID = ticket.Id 
                });
                
            }
        }
    }
}