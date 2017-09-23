using Microsoft.EntityFrameworkCore.ChangeTracking;
using OpenGameList.Data;
using OpenGameList.Data.Comments;
using OpenGameList.Data.Items;
using OpenGameList.Data.Users;
using System;
using System.Linq;

public static class DbSeeder
{
    #region Private Members
    private static ApplicationDbContext Context;
    #endregion Private Members

    #region Public Methods
    public static void Seed(ApplicationDbContext context)
    {
        Context = context;

        // Create the Db if it doesn't exist
        Context.Database.EnsureCreated();
        // Create default Users
        if (Context.Users.Count() == 0) CreateUsers();
        // Create default Items (if there are none) and Comments
        if (Context.Items.Count() == 0) CreateItems();
    }
    #endregion Public Methods

    #region Seed Methods
    public static void CreateUsers()
    {
        // local variables
        DateTime createdDate = new DateTime(2017, 09, 01, 09, 30, 00);

        DateTime lastModifiedDate = DateTime.Now;

        // Create the "Admin" ApplicationUser account (if it doesn't exist already)
        var user_Admin = new ApplicationUser()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "Admin",
            Email = "admin@opengamelist.com",
            CreatedDate = createdDate,
            LastModifiedDate = lastModifiedDate
        };

        // Insert "Admin" into the Database
        Context.Users.Add(user_Admin);

#if DEBUG
        // Create some sample registered user accounts (if they don't exist already)
        var user_Ryan = new ApplicationUser()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "Ryan",
            Email = "ryan@opengamelist.com",
            CreatedDate = createdDate,
            LastModifiedDate = lastModifiedDate
        };

        var user_Solice = new ApplicationUser()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "Solice",
            Email = "solice@opengamelist.com",
            CreatedDate = createdDate,
            LastModifiedDate = lastModifiedDate
        };

        var user_Vodan = new ApplicationUser()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "Vodan",
            Email = "vodan@opengamelist.com",
            CreatedDate = createdDate,
            LastModifiedDate = lastModifiedDate
        };

        // Insert sample registered users into the Database
        Context.Users.AddRange(user_Ryan, user_Solice, user_Vodan);
#endif

        Context.SaveChanges();
    }

    public static void CreateItems()
    {
        // local variables
        DateTime createdDate = new DateTime(2017, 09, 01, 09, 30, 00);
        DateTime lastModifiedDate = DateTime.Now;

        var authorId = Context.Users.Where(u => u.UserName == "Admin").FirstOrDefault().Id;

#if DEBUG
        var num = 1000; // create 1000 sample items

        for (int id = 1; id <= num; id++)
        {
            Context.Items.Add(GetSampleItem(id, authorId, num - id, new DateTime(2016, 12, 31).AddDays(-num)));
        }
#endif

        EntityEntry<Item> e1 = Context.Items.Add(new Item()
        {
            UserId = authorId,
            Title = "Magarena",
            Description = "Single-player fantasy card game similar to Magic: The Gathering.",
            Text = @"Loosely based on Magic: The Gathering, the game lets you play against a computer opponent or another human being.
            The game features a well-developed AI, an intuitive and clear interface and an enticing level of gameplay.",
            Notes = "This is a sample record created by the Code-First Configuration class",
            ViewCount = 2343,
            CreatedDate = createdDate,
            LastModifiedDate = lastModifiedDate
        });

        EntityEntry<Item> e2 = Context.Items.Add(new Item()
        {
            UserId = authorId,
            Title = "Minetest",
            Description = "Open-Source alternative to Minecraft.",
            Text = @"The Minetest gameplay is very similar to Minecraft's: you are playing in a 3D open world, where you can create and/or remove various types of blocks.
            Minetest feature both single-player and multiplayer game modes.
            It also has support for custom mods, additional texture packs and other custom/personalization options.
            Minetest has been released in 2015 under GNU Lesser General Public License.",
            Notes = "This is a sample record created by the Code-First Configuration class",
            ViewCount = 4180,
            CreatedDate = createdDate,
            LastModifiedDate = lastModifiedDate
        });

        EntityEntry<Item> e3 = Context.Items.Add(new Item()
        {
            UserId = authorId,
            Title = "Relic Hunters Zero",
            Description = "A free game about shooting evil space ducks with tiny, cute guns.",
            Text = @"Relic Hunters Zero is fast, tactical and also very smooth to play.
            It also enables the users to look at the source code, so they can can get creative and keep this game alive, fun and free for years to come.
            The game is also available on Steam.",
            Notes = "This is a sample record created by the Code-First Configuration class",
            ViewCount = 5203,
            CreatedDate = createdDate,
            LastModifiedDate = lastModifiedDate
        });

        EntityEntry<Item> e4 = Context.Items.Add(new Item()
        {
            UserId = authorId,
            Title = "SuperTux",
            Description = "A classic 2D jump and run, side-scrolling game similar to the Super Mario series.",
            Text = @"The game is currently under Milestone 3. The Milestone 2, which is currently out, features the following:
            - A nearly completely rewritten game engine based on OpenGL, OpenAL, SDL2, ...
            - Support for translations
            - In-game manager for downloadable add-ons and translations
            - Bonus Island III, a for now unfinished Forest Island and the development levels in Incubator Island
            - A final boss in Icy Island
            - New and improved soundtracks and sound effects
            ... and much more!
            The game has been released under the GNU GPL license.",
            Notes = "This is a sample record created by the Code-First Configuration class",
            ViewCount = 9602,
            CreatedDate = createdDate,
            LastModifiedDate = lastModifiedDate
        });

        EntityEntry<Item> e5 = Context.Items.Add(new Item()
        {
            UserId = authorId,
            Title = "Scrabble3D",
            Description = "A 3D-based revamp to the classic Scrabble game.",
            Text = @"Scrabble3D extends the gameplay of the classic game Scrabble by adding a new whole third dimension.
            Other than playing left to right or top to bottom, you'll be able to place your tiles above or beyond other tiles.
            Since the game features more fields, it also uses a larger letter set.
            You can either play against the computer, players from your LAN or from the Internet.
            The game also features a set of game servers where you can challenge players from all over the world and get ranked into an official, ELO-based rating/ladder system.",
            Notes = "This is a sample record created by the Code-First Configuration class",
            ViewCount = 6754,
            CreatedDate = createdDate,
            LastModifiedDate = lastModifiedDate
        });

        // Create default Comments (if there are none)
        if (Context.Comments.Count() == 0)
        {
            int numComments = 10; // comments per item
            for (int i = 1; i <= numComments; i++)
                Context.Comments.Add(GetSampleComment(i, e1.Entity.Id, authorId,
                createdDate.AddDays(i)));
            for (int i = 1; i <= numComments; i++)
                Context.Comments.Add(GetSampleComment(i, e2.Entity.Id, authorId,
                createdDate.AddDays(i)));
            for (int i = 1; i <= numComments; i++)
                Context.Comments.Add(GetSampleComment(i, e3.Entity.Id, authorId,
                createdDate.AddDays(i)));
            for (int i = 1; i <= numComments; i++)
                Context.Comments.Add(GetSampleComment(i, e4.Entity.Id, authorId,
                createdDate.AddDays(i)));
            for (int i = 1; i <= numComments; i++)
                Context.Comments.Add(GetSampleComment(i, e5.Entity.Id, authorId,
                createdDate.AddDays(i)));
        }

        Context.SaveChanges();
    }
    #endregion Seed Methods

    #region Utility Methods
    /// <summary>
    /// Generate a sample item to populate the DB.
    /// </summary>
    /// <param name="id">the item ID</param>
    /// <param name="authorId">the author ID</param>
    /// <param name="viewCount">the item viewCount</param>
    /// <param name="createdDate">the item CreatedDate</param>
    /// <returns></returns>
    private static Item GetSampleItem(int id, string authorId, int viewCount, DateTime createdDate)
    {
        return new Item()
        {
            UserId = authorId,
            Title = $"Item {id} Title",
            Description = $"This is a sample description for item {id}: Lorem ipsum dolor sit amet.",
            Notes = "This is a sample record created by the Code-First Configuration class",
            ViewCount = viewCount,
            CreatedDate = createdDate,
            LastModifiedDate = createdDate
        };
    }

    /// <summary>
    /// Generate a sample array of comments (for testing purposes only).
    /// </summary>
    /// <param name="n">the comment ID</param>
    /// <param name="itemId">the item ID</param>
    /// <param name="authorId">the author ID</param>
    /// <param name="createdDate">the comment CreatedDate</param>
    /// <returns></returns>
    private static Comment GetSampleComment(int n, int itemId, string authorId, DateTime createdDate)
    {
        return new Comment()
        {
            ItemId = itemId,
            UserId = authorId,
            ParentId = null,
            Text = $"Sample comment #{n} for the item #{itemId}",
            CreatedDate = createdDate,
            LastModifiedDate = createdDate
        };
    }
    #endregion Utility Methods
}
