using System;

namespace TicketProcessor.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public required string ClienteEmail { get; set; }
        public required string TextoTicket { get; set; }
        public string? CategoriaIA { get; set; }
        public required string Estado { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaRespuesta { get; set; }
    }
}