
using Microsoft.EntityFrameworkCore;
using socialmedia.Context;
using socialmedia.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace socialmedia.Repositories
{
    public class PostRepository
    {
        private readonly WebDbContext _context;
        public PostRepository(WebDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreatePostAsync(MainPost post, List<PostMedia> mediaList)
        {
            if (mediaList == null || mediaList.Count == 0)
            {
                throw new ArgumentException("Post oluşturulurken en az bir adet medya yüklemek zorunludur!");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                { 
                    await _context.MainPosts.AddAsync(post);
                    await _context.SaveChangesAsync();
                    foreach (var media in mediaList)
                    { 
                        media.PostID = post.PostID;
                        await _context.PostMedias.AddAsync(media);
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return post.PostID;
                }
                catch (Exception)
                { 
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        public async Task<MainPost> GetPostByIdAsync(int postId)
        {
            return await _context.MainPosts
                                    .FirstOrDefaultAsync(p => p.PostID == postId);
        }
        public async Task<bool> TogglePostLikeAsync(int postId, int userId)
        {
            var existingLike = await _context.PostsLikes
                                             .FirstOrDefaultAsync(l => l.PostID == postId && l.UserID == userId);

            var post = await _context.MainPosts.FirstOrDefaultAsync(p => p.PostID == postId);
            if (post == null) return false;

            if (existingLike != null)
            {
                _context.PostsLikes.Remove(existingLike);
                post.LikeCount = Math.Max(0, post.LikeCount - 1);
            }
            else
            {
                var newLike = new PostsLike
                {
                    PostID = postId,
                    UserID = userId,
                    PostLikedDate = DateTime.Now
                };

                await _context.PostsLikes.AddAsync(newLike);
                post.LikeCount += 1;
            }
            await _context.SaveChangesAsync();
            return true;
        }      
        public async Task<bool> AddCommentAsync(Comments comment)
        {
            await _context.Comments.AddAsync(comment);

            
            var post = await _context.MainPosts.FirstOrDefaultAsync(p => p.PostID == comment.PostID);
            if (post != null)
            {
                post.CommentCount += 1;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleCommentLikeAsync(int commentId, int userId)
        {
            var existingLike = await _context.CommentsLikes
                                             .FirstOrDefaultAsync(cl => cl.CommentID == commentId && cl.UserID == userId);

            var comment = await _context.Comments
                                              .FirstOrDefaultAsync(c => c.CommentID == commentId);
            if (comment == null) return false;

            if (existingLike != null)
            {
                _context.CommentsLikes.Remove(existingLike);
                comment.LikeCount = Math.Max(0, comment.LikeCount - 1);
            }
            else
            {
                var newLike = new CommentsLike
                {
                    CommentID = commentId,
                    UserID = userId,
                    CommentLikedDate = DateTime.Now
                };

                await _context.CommentsLikes.AddAsync(newLike);
                comment.LikeCount += 1;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePostAsync(int postId)
        {
            var post = await _context.MainPosts.FirstOrDefaultAsync(p => p.PostID == postId);
            if (post == null) return false;

            
            _context.MainPosts.Remove(post);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
