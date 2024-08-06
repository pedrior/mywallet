using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;
using MyWallet.Shared.Features;
using Serilog;
using MyWallet;
using MyWallet.Shared.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints();

// Temporary
using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<MigrationWorker>()
    .PerformMigration();

app.Run();

namespace MyWallet
{
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart", Justification = "Referenced by WebApplicationFactory")]
    public partial class Program;

    internal static class Extensions
    {
        public static void UseEndpoints(this WebApplication app)
        {
            var versionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

            var versionedGroup = app.MapGroup("api/v{version:apiVersion}")
                .WithApiVersionSet(versionSet);

            foreach (var endpoint in app.Services.GetRequiredService<IEnumerable<IEndpoint>>())
            {
                endpoint.Build(versionedGroup);
            }
        }
    }
}