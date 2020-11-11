namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _add_YADSCCostCenters : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.YADSCCostCenters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CostCenter = c.String(),
                        CostCenterName = c.String(),
                        CCMGlobalID = c.String(),
                        CCMShortName = c.String(),
                        CCMDisplayName = c.String(),
                        BOMGlobalID = c.String(),
                        BOMShortName = c.String(),
                        BOMDisplayName = c.String(),
                        OverallResopnsible = c.String(),
                        BusLineFunHeadDisplayName = c.String(),
                        BusLineFunHeadGlobalID = c.String(),
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
            DropTable("dbo.YADSCCostCenters");
        }
    }
}
