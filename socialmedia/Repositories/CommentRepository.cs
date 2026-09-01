using socialmedia.Context;
using socialmedia.Models;
using Microsoft.EntityFrameworkCore;
namespace socialmedia.Repositories
{
    public class CommentRepository
    {
        private readonly WebDbContext _context;

        public CommentRepository(WebDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddCommentAsync(Comment comment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Comments.AddAsync(comment);

                var post = await _context.MainPosts
                    .FirstOrDefaultAsync(p => p.PostID == comment.PostID);
                if (post != null)
                {
                    post.CommentCount += 1;
                }

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
        public async Task<List<Comment>> GetTopLevelCommentsByPostIdAsync(long postId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.PostID == postId && c.ParentCommentID == null)
                .OrderByDescending(c => c.CommentDate)
                .ToListAsync();
        }
        public async Task<List<Comment>> GetRepliesByParentIdAsync(long parentCommentId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.ParentCommentID == parentCommentId)
                .OrderBy(c => c.CommentDate)
                .ToListAsync();
        }
        public async Task<List<Comment>> GetAllRepliesByPostIdAsync(long postId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.PostID == postId && c.ParentCommentID != null)
                .OrderBy(c => c.CommentDate)
                .ToListAsync();
        }
        public async Task<Comment?> GetByIdAsync(long commentId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CommentID == commentId);
        }

        public async Task<long?> GetCommentOwnerIdAsync(long commentId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentID == commentId);
            return comment?.UserID;
        }

        public async Task<bool> UpdateCommentAsync(long commentId, string newText)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.CommentID == commentId);
            if (comment == null) return false;

            comment.CommentText = newText;
            comment.IsUpdate = true;
            comment.UpdateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCommentAsync(long commentId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.CommentID == commentId);
            if (comment == null) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var commentLikes = _context.CommentsLikes.Where(cl => cl.CommentID == commentId);
                _context.CommentsLikes.RemoveRange(commentLikes);

                
                var replies = _context.Comments.Where(c => c.ParentCommentID == commentId);
                _context.Comments.RemoveRange(replies);

                var post = await _context.MainPosts
                    .FirstOrDefaultAsync(p => p.PostID == comment.PostID);
                if (post != null)
                {
                    post.CommentCount = Math.Max(0, post.CommentCount - 1);
                }

                _context.Comments.Remove(comment);
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
        public async Task<bool> ToggleCommentLikeAsync(long commentId, long userId)
        {
            var existingLike = await _context.CommentsLikes
                .FirstOrDefaultAsync(cl => cl.CommentID == commentId && cl.UserID == userId);
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.CommentID == commentId);
            if (comment == null)
                throw new KeyNotFoundException("Yorum bulunamadı.");

            bool isNowLiked;

            if (existingLike != null)
            {
                _context.CommentsLikes.Remove(existingLike);
                comment.LikeCount = Math.Max(0, comment.LikeCount - 1);
                isNowLiked = false;
            }
            else
            {
                var newLike = new CommentLike
                {
                    CommentID = commentId,
                    UserID = userId,
                    CommentLikedDate = DateTime.UtcNow
                };
                await _context.CommentsLikes.AddAsync(newLike);
                comment.LikeCount += 1;
                isNowLiked = true;
            }

            await _context.SaveChangesAsync();
            return isNowLiked;
        }

    }
}

