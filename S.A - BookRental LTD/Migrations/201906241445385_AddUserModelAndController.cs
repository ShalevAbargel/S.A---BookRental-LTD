namespace S.A___BookRental_LTD.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserModelAndController : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserViewModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(nullable: false),
                        Password = c.String(),
                        ConfirmPassword = c.String(),
                        MembershipTypeId = c.Int(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        BirthDate = c.DateTime(nullable: false),
                        Disable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.MembershipTypes", "UserViewModel_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.MembershipTypes", "UserViewModel_Id");
            AddForeignKey("dbo.MembershipTypes", "UserViewModel_Id", "dbo.UserViewModels", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MembershipTypes", "UserViewModel_Id", "dbo.UserViewModels");
            DropIndex("dbo.MembershipTypes", new[] { "UserViewModel_Id" });
            DropColumn("dbo.MembershipTypes", "UserViewModel_Id");
            DropTable("dbo.UserViewModels");
        }
    }
}
