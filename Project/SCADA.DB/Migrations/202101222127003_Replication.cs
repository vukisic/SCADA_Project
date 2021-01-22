namespace SCADA.DB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Replication : DbMigration
    {
        public override void Up()
        {
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
        }
    }
}
