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
    [Description("Run database migrations down", Name = "migrate-down")]
    public class MigrateDownCommand : OaktonAsyncCommand<MigrateDownInput>
    {
        public MigrateDownCommand()
        {
            Usage("Migrate down to a given version").Arguments(x => x.Version);
        }

        public override Task<bool> Execute(MigrateDownInput input)
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
            UpdateDatabase(services, input.Version);
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
        private static void UpdateDatabase(IServiceProvider serviceProvider, long version)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateDown(version);
        }
    }
}