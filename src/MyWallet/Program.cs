using System.Diagnostics.CodeAnalysis;
using MyWallet.Startup;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints();

// Temporary
app.MigrateDatabase();

app.Run();

namespace MyWallet
{
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart", Justification = "Referenced by WebApplicationFactory")]
    public partial class Program;
}