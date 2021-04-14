using FluentMigrator;
using FluentMigrator.SqlServer;

namespace Migrations.Sprint001
{
    [Migration(202104020122)]
    [Tags("dev")]
    public class SeedDevelopmentDataToTasksTable : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Tasks")
                .WithIdentityInsert()
                .Row(new
                {
                    Id = 1,
                    Description = "Créer un article sur les migrations de BDD",
                    Priority = 5,
                    State = 1 // In progress
                });
        }

        public override void Down()
        {
            Delete.FromTable("Tasks")
                .Row(new
                {
                    Id = 1
                });
        }
    }
}