namespace MicroAssignment.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<MicroAssignment.Models.MicroContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MicroAssignment.Models.MicroContext context)
        {
            WebSecurity.InitializeDatabaseConnection(
              "MicroContext",
              "UserProfile",
              "UserId",
              "UserName", autoCreateTables: true);

            if (!Roles.RoleExists("SuperAdmin"))
                Roles.CreateRole("SuperAdmin");

            if (!WebSecurity.UserExists("Demo"))
                WebSecurity.CreateUserAndAccount("Demo", "Admin@123");
            if (!Roles.GetRolesForUser("Demo").Contains("SuperAdmin"))
                Roles.AddUsersToRoles(new[] { "Demo" }, new[] { "SuperAdmin" });
        }
    }
}
