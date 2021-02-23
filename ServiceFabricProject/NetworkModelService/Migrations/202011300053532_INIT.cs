namespace FTN.Services.NetworkModelService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class INIT : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeltaDBModels",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Data = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DeltaDBModels");
        }
    }
}
