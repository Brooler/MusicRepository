namespace MusicRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAutorImageData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Albums", "ImageData", c => c.Binary());
            AddColumn("dbo.Albums", "ImageMimeType", c => c.String());
            AddColumn("dbo.Autors", "ImageData", c => c.Binary());
            AddColumn("dbo.Autors", "ImageMimeType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Autors", "ImageMimeType");
            DropColumn("dbo.Autors", "ImageData");
            DropColumn("dbo.Albums", "ImageMimeType");
            DropColumn("dbo.Albums", "ImageData");
        }
    }
}
