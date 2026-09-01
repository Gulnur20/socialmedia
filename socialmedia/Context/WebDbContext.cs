
using Microsoft.EntityFrameworkCore;
using socialmedia.Models;

namespace socialmedia.Context
{
    public class WebDbContext : DbContext
    {
        public WebDbContext(DbContextOptions<WebDbContext> options) : base(options)
        {
        }
        public DbSet<MainPost> MainPosts { get; set; }
        public DbSet<PostMedia> PostMedias { get; set; }
        public DbSet<PostLike> PostsLikes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentLike> CommentsLikes { get; set; }

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            modelBuilder.Entity<MainPost>().ToTable("MainPost");
            modelBuilder.Entity<PostMedia>().ToTable("PostMedia");
            modelBuilder.Entity<PostLike>().ToTable("PostsLike");
            modelBuilder.Entity<Comment>().ToTable("Comments");
            modelBuilder.Entity<CommentLike>().ToTable("CommentsLike");
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<UserProfile>().ToTable("UserProfile");
            modelBuilder.Entity<UserSettings>().ToTable("UserSettings");
            modelBuilder.Entity<Follow>().ToTable("Follow");
            modelBuilder.Entity<FollowRequest>().ToTable("FollowRequest");


            modelBuilder.Entity<MainPost>().HasKey(p => p.PostID);
            modelBuilder.Entity<PostMedia>().HasKey(m => m.MediaID);
            modelBuilder.Entity<PostLike>().HasKey(l => l.PostLikeID);
            modelBuilder.Entity<Comment>().HasKey(c => c.CommentID);
            modelBuilder.Entity<CommentLike>().HasKey(cl => cl.CommentLikeID);
            modelBuilder.Entity<Users>().HasKey(u => u.UserID);
            modelBuilder.Entity<UserProfile>().HasKey(p => p.UserID);
            modelBuilder.Entity<UserSettings>().HasKey(s => s.UserID);
            modelBuilder.Entity<Follow>().HasKey(f => f.FollowID);
            modelBuilder.Entity<FollowRequest>().HasKey(fr => fr.RequestID);


            modelBuilder.Entity<PostMedia>()
                .HasOne(m => m.MainPost)
                .WithMany(p => p.PostMedia)
                .HasForeignKey(m => m.PostID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PostLike>()
                .HasOne(l => l.MainPost)
                .WithMany(p => p.PostLikes)
                .HasForeignKey(l => l.PostID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.MainPost)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CommentLike>()
                .HasOne(cl => cl.Comment)
                .WithMany(c => c.CommentLikes)
                .HasForeignKey(cl => cl.CommentID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                .HasMany(c => c.Replies)               
                .WithOne()                           
                .HasForeignKey(c => c.ParentCommentID)  
                .OnDelete(DeleteBehavior.NoAction);

        }




    }
}
