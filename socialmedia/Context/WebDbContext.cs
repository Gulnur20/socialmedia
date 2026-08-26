
using Microsoft.EntityFrameworkCore;
using socialmedia.Models;

    namespace socialmedia.Context
{
    public class WebDbContext:DbContext
    {
        public WebDbContext(DbContextOptions<DbContext> options) : base(options)
        {
        }
        public DbSet<MainPost> MainPosts { get; set; }
        public DbSet<PostMedia> PostMedias { get; set; }
        public DbSet<PostsLike> PostsLikes { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<CommentsLike> CommentsLikes { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<Follows> Follows { get; set; }
        public DbSet<FollowRequest> FollowRequests { get; set; }
    }


}
