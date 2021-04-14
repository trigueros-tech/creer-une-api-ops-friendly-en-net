using FluentMigrator;

namespace Migrations.Sprint001
{
    [Migration(202104020116)]
    public class AddStateToTasksTable : Migration
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

        public override void Down()
        {
            Delete.Column("State").FromTable("Tasks");
        }
    }
}