namespace FTN.Services.NetworkModelService.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FTN.Services.NetworkModelService.DeltaDB.DeltaDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FTN.Services.NetworkModelService.DeltaDB.DeltaDBContext context)
        {
            context.Database.CreateIfNotExists();
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
