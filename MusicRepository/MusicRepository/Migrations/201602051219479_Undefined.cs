namespace MusicRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Undefined : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Autors", "AlbumCount", c => c.Int());
            AddColumn("dbo.Autors", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Autors", "Discriminator");
            DropColumn("dbo.Autors", "AlbumCount");
        }
    }
}
