using Hangfire;
using TicketProcessor.API;
using TicketProcessor.Repositories;
using TicketProcessor.Services;

var builder = WebApplication.CreateBuilder(args);

string bdConexion = builder.Configuration.GetConnectionString("BdConexion") 
    ?? throw new InvalidOperationException("Falta la cadena de conexión");

// 1. Servicios de terceros (Hangfire)
builder.Services.AddHangfire(config => config
    .UseSqlServerStorage(bdConexion)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings());
builder.Services.AddHangfireServer();

// 2. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// 3. Inyección de Dependencias
builder.Services.AddTransient<ITicketClassifierService, GeminiClassifierService>();
builder.Services.AddTransient<ITicketRepository, DapperTicketRepository>();
builder.Services.AddTransient<ITicketProcessorService, TicketProcessorService>();

var app = builder.Build();

// 4. Configuración del Pipeline HTTP
app.UseCors("PermitirFrontend");
app.UseHangfireDashboard();

// 5. Trabajos en segundo plano
RecurringJob.AddOrUpdate<ITicketProcessorService>(
    "ProcesarTickets", 
    x => x.ProcesarTicketsPendientesAsync(), 
    Cron.Minutely());

// 6. Registro de Endpoints
app.MapTicketEndpoints();

app.Run();
