using FluentMigrator;

namespace Migrations.Sprint001
{
    [Migration(202104020119)]
    public class AddPriorityToTasksTable : Migration
    {
        public override void Up()
        {
            Alter.Table("Tasks")
                .AddColumn("Priority")
                    .AsInt32()
                    .NotNullable()
                    .WithDefaultValue(0);
        }

        public override void Down()
        {
            Delete.Column("Priority").FromTable("Tasks");
        }
    }
}