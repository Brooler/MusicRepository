namespace MusicRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Albums",
                c => new
                    {
                        AlbumId = c.Int(nullable: false, identity: true),
                        AutorId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Rate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.AlbumId);
            
            CreateTable(
                "dbo.Autors",
                c => new
                    {
                        AutorId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AutorId);
            
            CreateTable(
                "dbo.Tracks",
                c => new
                    {
                        TrackId = c.Int(nullable: false, identity: true),
                        AlbumId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Rate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.TrackId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Tracks");
            DropTable("dbo.Autors");
            DropTable("dbo.Albums");
        }
    }
}
