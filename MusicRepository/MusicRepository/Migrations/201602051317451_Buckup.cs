namespace MusicRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Buckup : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Autors", "AlbumCount");
            DropColumn("dbo.Autors", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Autors", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Autors", "AlbumCount", c => c.Int());
        }
    }
}
