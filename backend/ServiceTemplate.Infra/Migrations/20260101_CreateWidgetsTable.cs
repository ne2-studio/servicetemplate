using FluentMigrator;

namespace ServiceTemplate.Infra.Migrations;

[Migration(20260101)]
public class CreateWidgetsTable : Migration
{
    public override void Up()
    {
        Create.Table("Widgets")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Name").AsString(200).NotNullable().Unique()
            .WithColumn("CreatedAt").AsDateTimeOffset().NotNullable()
            .WithColumn("UpdatedAt").AsDateTimeOffset().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Widgets");
    }
}
