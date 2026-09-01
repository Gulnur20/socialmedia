
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
        public async Task<long> CreatePostAsync(MainPost post, List<PostMedia> mediaList)
        {
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
        public async Task<MainPost?> GetPostByIdAsync(long postId)
        {
            return await _context.MainPosts
                .Include(p => p.PostMedia)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PostID == postId);
        }
        public async Task<int> GetPostCountByUserIdAsync(long userId)
        {
            return await _context.MainPosts.CountAsync(p => p.UserID == userId);
        }
        public async Task<(List<MainPost> Posts, int TotalCount)> GetTimelineAsync(List<long> followingIds, int page, int pageSize)
        {
            var query = _context.MainPosts
                .Where(p => followingIds.Contains(p.UserID))
                .OrderByDescending(p => p.PostCreated);

            int totalCount = await query.CountAsync();

            var posts = await query
                .Include(p => p.PostMedia)
                .Include(p => p.User)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (posts, totalCount);
        }

      
        public async Task<long?> GetPostOwnerIdAsync(long postId)
        {
            var post = await _context.MainPosts.FirstOrDefaultAsync(p => p.PostID == postId);
            return post?.UserID;
        }
        public async Task<bool> UpdatePostAsync(long postId, string? caption)
        {
            var post = await _context.MainPosts.FirstOrDefaultAsync(p => p.PostID == postId);
            if (post == null) return false;

            post.Caption = caption;
            post.IsEdited = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TogglePostLikeAsync(long postId, long userId)
        {
            var existingLike = await _context.PostsLikes
                .FirstOrDefaultAsync(l => l.PostID == postId && l.UserID == userId);

            var post = await _context.MainPosts.FirstOrDefaultAsync(p => p.PostID == postId);
            if (post == null)
                throw new KeyNotFoundException("Post bulunamadı.");

            bool isNowLiked;

            if (existingLike != null)
            {
                _context.PostsLikes.Remove(existingLike);
                post.LikeCount = Math.Max(0, post.LikeCount - 1);
                isNowLiked = false;
            }
            else
            {
                var newLike = new PostLike
                {
                    PostID = postId,
                    UserID = userId,
                    PostLikedDate = DateTime.UtcNow
                };
                await _context.PostsLikes.AddAsync(newLike);
                post.LikeCount += 1;
                isNowLiked = true;
            }

            await _context.SaveChangesAsync();
            return isNowLiked;
        }

        public async Task<bool> DeletePostAsync(long postId)
        {
            var post = await _context.MainPosts.FirstOrDefaultAsync(p => p.PostID == postId);
            if (post == null) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var commentIds = await _context.Comments
                    .Where(c => c.PostID == postId)
                    .Select(c => c.CommentID)
                    .ToListAsync();

                var commentLikes = _context.CommentsLikes
                    .Where(cl => commentIds.Contains(cl.CommentID));
                _context.CommentsLikes.RemoveRange(commentLikes);

                var comments = _context.Comments.Where(c => c.PostID == postId);
                _context.Comments.RemoveRange(comments);

                var postLikes = _context.PostsLikes.Where(l => l.PostID == postId);
                _context.PostsLikes.RemoveRange(postLikes);

                var media = _context.PostMedias.Where(m => m.PostID == postId);
                _context.PostMedias.RemoveRange(media);

                _context.MainPosts.Remove(post);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
