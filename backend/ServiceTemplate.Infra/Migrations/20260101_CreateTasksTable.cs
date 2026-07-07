using FluentMigrator;

namespace ServiceTemplate.Infra.Migrations;

[Migration(20260101)]
public class CreateTasksTable : Migration
{
    public override void Up()
    {
        Create.Table("Tasks")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Title").AsString(200).NotNullable()
            .WithColumn("CreatedAt").AsDateTimeOffset().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Tasks");
    }
}
