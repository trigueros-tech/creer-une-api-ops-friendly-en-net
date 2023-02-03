using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Migrations.Sprint001;
using Oakton;

namespace Api.Infrastructure.Ops
{
    [Description("Run database migrations up", Name = "migrate-up")]
    public class MigrateUpCommand : OaktonAsyncCommand<MigrateUpInput>
    {
        public MigrateUpCommand()
        {
            Usage("Migrate up to latest version")
                .ValidFlags(x => x.VersionFlag, x => x.TagsFlag);
        }

        public override Task<bool> Execute(MigrateUpInput input)
        {
            string connectionString;
            using (var host = input.BuildHost())
            {
                /*
                 * Ici, on bénéficie de la chaine de récupération classique ASP.NET :
                 * présence dans fichier appsettingsjson, fichier appsettings.production.json
                 * ou variable d'environnement
                 */
                var configuration = host.Services.GetRequiredService<IConfiguration>();
                connectionString = configuration.GetConnectionString("Default");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                ConsoleWriter.Write(ConsoleColor.Red, "No connectionString");
                return Task.FromResult(false);
            }

            var services = CreateServices(connectionString, input.TagsFlag);
            
            UpdateDatabase(services, input.VersionFlag);
            return Task.FromResult(true);
        }

        private static IServiceProvider CreateServices(string connectionString, IEnumerable<string> tags)
        {
            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add SQLite support to FluentMigrator
                    .AddSqlServer()
                    // Set the connection string
                    .WithGlobalConnectionString(connectionString)
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(CreateTasksTable).Assembly).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .Configure<RunnerOptions>(o =>
                {
                    var enumerable = tags as string[] ?? tags?.ToArray() ?? Array.Empty<string>();
                    if (enumerable.Any())
                    {   
                        o.Tags = Enumerable.Concat(o.Tags ?? Array.Empty<string>(), enumerable).ToArray();
                    }
                })
                // Build the service provider
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Update the database
        /// </summary>
        private static void UpdateDatabase(IServiceProvider serviceProvider, long? version)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            if (version != null)
            {
                // Execute the migrations
                runner.MigrateUp(version.Value);
            }
            else
            {
                runner.MigrateUp();
            }
        }
    }
}