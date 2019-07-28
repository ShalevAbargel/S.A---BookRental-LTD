namespace S.A___BookRental_LTD.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTYPEOFMEMBERSHIPTOUSERVIEWMODEL : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserViewModels", "TypeOfMembership", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserViewModels", "TypeOfMembership");
        }
    }
}
