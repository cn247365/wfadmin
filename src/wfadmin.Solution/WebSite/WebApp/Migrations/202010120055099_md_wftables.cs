namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class md_wftables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WfTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Table_Catalog = c.String(nullable: false, maxLength: 32),
                        Table_Schema = c.String(nullable: false, maxLength: 32),
                        Table_Name = c.String(nullable: false, maxLength: 128),
                        Table_Type = c.String(nullable: false, maxLength: 32),
                        WorkflowName = c.String(maxLength: 128),
                        Description = c.String(maxLength: 128),
                        AllowedAdd = c.Boolean(nullable: false),
                        AllowedEdit = c.Boolean(nullable: false),
                        AllowedDelete = c.Boolean(nullable: false),
                        AllowedQuery = c.Boolean(nullable: false),
                        OperationManual = c.String(maxLength: 256),
                        Url = c.String(maxLength: 128),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 20),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(maxLength: 20),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WfTables");
        }
    }
}
