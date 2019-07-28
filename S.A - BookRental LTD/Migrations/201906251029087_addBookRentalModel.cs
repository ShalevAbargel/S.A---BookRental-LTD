namespace S.A___BookRental_LTD.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBookRentalModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookRents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        BookId = c.Int(nullable: false),
                        StartDate = c.DateTime(),
                        ActualEndDate = c.DateTime(),
                        SchedualEndDate = c.DateTime(),
                        AditionalCharge = c.Double(),
                        RentalPrice = c.Double(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BookRents");
        }
    }
}
