namespace Garcia.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDb1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "Name", c => c.String(maxLength: 50));
            AlterColumn("dbo.Employees", "Surname", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Employees", "Surname", c => c.String());
            AlterColumn("dbo.Employees", "Name", c => c.String());
        }
    }
}
