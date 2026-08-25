using Hangfire;
using TicketProcessor.Models;
using TicketProcessor.Services; 

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de BD
string bdConexion = builder.Configuration.GetConnectionString("BdConexion");

// 2. Servicios de terceros (Hangfire)
builder.Services.AddHangfire(config => config
    .UseSqlServerStorage(bdConexion)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings());
builder.Services.AddHangfireServer();

// 3. NUESTROS Servicios (Inyección de Dependencias)
builder.Services.AddTransient<ITicketClassifierService, DummyClassifierService>();
builder.Services.AddTransient<ITicketProcessorService, TicketProcessorService>();

var app = builder.Build();

app.UseHangfireDashboard();
RecurringJob.AddOrUpdate<ITicketProcessorService>("ProcesarTickets", x => x.ProcesarTicketsPendientesAsync(), Cron.Minutely());

app.Run();