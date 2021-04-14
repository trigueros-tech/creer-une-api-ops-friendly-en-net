using FluentMigrator;

namespace Migrations.Sprint001
{
    [Migration(202104020116)]
    public class AddStateToTasksTable : AutoReversingMigration
    {
        public override void Up()
        {
            // 0 correspond à "à faire"
            Alter.Table("Tasks")
                .AddColumn("State")
                    .AsInt32()
                    .NotNullable()
                    .WithDefaultValue(0);
        }
    }
}