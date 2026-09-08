using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TicketProcessor.Models;

namespace TicketProcessor.Repositories;

public class DapperTicketRepository : ITicketRepository
{
    private readonly string _conexion;

    public DapperTicketRepository(IConfiguration config)
    {
        _conexion = config.GetConnectionString("BdConexion")
            ?? throw new InvalidOperationException("Falta la cadena de conexión");
    }

    public async Task<IEnumerable<Ticket>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Id, ClienteEmail, TextoTicket, CategoriaIA, Estado FROM TicketsSoporte ORDER BY Id DESC";

        await using var db = new SqlConnection(_conexion);
        await db.OpenAsync(cancellationToken);

        return await db.QueryAsync<Ticket>(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Ticket>> ObtenerPendientesAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM TicketsSoporte WHERE Estado = 'Pendiente';";

        await using var db = new SqlConnection(_conexion);
        await db.OpenAsync(cancellationToken);

        return await db.QueryAsync<Ticket>(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Ticket>> ObtenerFallidosAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM TicketsSoporte WHERE Estado = 'FALLIDO';";

        await using var db = new SqlConnection(_conexion);
        await db.OpenAsync(cancellationToken);

        return await db.QueryAsync<Ticket>(new CommandDefinition(sql, cancellationToken: cancellationToken));

    }

    public async Task CrearAsync(
        string clienteEmail,
        string textoTicket,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO TicketsSoporte (ClienteEmail, TextoTicket, Estado, FechaCreacion)
            VALUES (@ClienteEmail, @TextoTicket, 'Pendiente', GETDATE())
            """;

        await using var db = new SqlConnection(_conexion);
        await db.OpenAsync(cancellationToken);

        await db.ExecuteAsync(new CommandDefinition(
            sql,
            new { ClienteEmail = clienteEmail, TextoTicket = textoTicket },
            cancellationToken: cancellationToken));
    }

    public async Task MarcarComoProcesadoAsync(
        int ticketId,
        string categoriaIA,
        CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE TicketsSoporte SET ESTADO = 'PROCESADO', CategoriaIA = @CategoriaIA, FechaProcesamiento = GETDATE() WHERE ID = @ID";

        await using var db = new SqlConnection(_conexion);
        await db.OpenAsync(cancellationToken);

        await db.ExecuteAsync(new CommandDefinition(
            sql,
            new { CategoriaIA = categoriaIA, ID = ticketId },
            cancellationToken: cancellationToken));
    }

    public async Task MarcarComoFallidoAsync(
        int ticketId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE TicketsSoporte
            SET Estado = 'FALLIDO', CategoriaIA = NULL, FechaProcesamiento = NULL
            WHERE Id = @ID AND Estado IN ('Pendiente', 'FALLIDO')
            """;
        await using var db = new SqlConnection(_conexion);
        await db.OpenAsync(cancellationToken);

        await db.ExecuteAsync(new CommandDefinition(
            sql,
            new {ID = ticketId },
            cancellationToken: cancellationToken));
    }

    public async Task MarcarComoPendienteRevisionAsync(
        int ticketId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE TicketsSoporte
            SET Estado = 'PENDIENTE_REVISAR', CategoriaIA = NULL, FechaProcesamiento = NULL
            WHERE Id = @ID AND Estado = 'FALLIDO'
            """;

        await using var db = new SqlConnection(_conexion);
        await db.OpenAsync(cancellationToken);

        await db.ExecuteAsync(new CommandDefinition(
            sql,
            new { ID = ticketId },
            cancellationToken: cancellationToken));
    }

}
