using FluentMigrator;

namespace Migrations.Sprint001
{
    [Migration(202104020113)]
    public class CreateTasksTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("Tasks")
                .WithColumn("Id")
                    .AsInt64()
                    .NotNullable()
                    .PrimaryKey()
                    .Identity()
                .WithColumn("Description")
                    .AsString(255)
                    .NotNullable();
        }
    }
}