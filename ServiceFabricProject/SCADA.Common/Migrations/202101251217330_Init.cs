namespace SCADA.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DomDbModels",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Mrid = c.String(),
                        ManipulationConut = c.Int(nullable: false),
                        TimeStamp = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HistoryDbModels",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ClassType = c.Int(nullable: false),
                        Index = c.Int(nullable: false),
                        Mrid = c.String(),
                        RegisterType = c.Int(nullable: false),
                        TimeStamp = c.String(),
                        MeasurementType = c.String(),
                        Value = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PointDbModels",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ClassType = c.Int(nullable: false),
                        Direction = c.Short(nullable: false),
                        Index = c.Int(nullable: false),
                        Mrid = c.String(),
                        ObjectMrid = c.String(),
                        RegisterType = c.Int(nullable: false),
                        TimeStamp = c.String(),
                        MeasurementType = c.Int(nullable: false),
                        Alarm = c.Int(nullable: false),
                        MinValue = c.Single(nullable: false),
                        MaxValue = c.Single(nullable: false),
                        NormalValue = c.Single(nullable: false),
                        Value = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PointDbModels");
            DropTable("dbo.HistoryDbModels");
            DropTable("dbo.DomDbModels");
        }
    }
}
