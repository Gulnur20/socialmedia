using socialmedia.DTOs.Comment.Request;
using socialmedia.DTOs.Comment.Response;
using socialmedia.DTOs.Users.Response;
using socialmedia.Models;
using socialmedia.Repositories;
using socialmedia.Services.Interfaces;
    
    namespace socialmedia.Services
{
    public class CommentService:ICommentService
    {
        private readonly CommentRepository _commentRepository;

        public CommentService(CommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<CommentDto> AddCommentAsync(long postId, CreateCommentDto dto, long userId)
        {
            var comment = new Comment
            {
                PostID = postId,
                UserID = userId,
                CommentText = dto.CommentText,
                CommentDate = DateTime.UtcNow,
                IsUpdate = false,
                UpdateTime = DateTime.UtcNow,
                LikeCount = 0,
                ParentCommentID = dto.ParentCommentID
            };

            await _commentRepository.AddCommentAsync(comment);

            var createdComment = await _commentRepository.GetByIdAsync(comment.CommentID);
            return MapToDto(createdComment!, userId, new List<Comment>());
        }

        public async Task<List<CommentDto>> GetCommentsByPostIdAsync(long postId, long currentUserId)
        {

            var topLevelComments = await _commentRepository.GetTopLevelCommentsByPostIdAsync(postId);
            var allReplies = await _commentRepository.GetAllRepliesByPostIdAsync(postId);

            return topLevelComments
                .Select(c => MapToDto(c, currentUserId, allReplies))
                .ToList();
        }

        public async Task<CommentDto> UpdateCommentAsync(long commentId, UpdateCommentDto dto, long userId)
        {
            var ownerId = await _commentRepository.GetCommentOwnerIdAsync(commentId);
            if (ownerId == null)
                throw new KeyNotFoundException("Yorum bulunamadı.");
            if (ownerId != userId)
                throw new UnauthorizedAccessException("Bu yorumu güncelleme yetkiniz yok.");

            await _commentRepository.UpdateCommentAsync(commentId, dto.CommentText);

            var updatedComment = await _commentRepository.GetByIdAsync(commentId);
            return MapToDto(updatedComment!, userId, new List<Comment>());
        }

        public async Task DeleteCommentAsync(long commentId, long userId)
        {
            var ownerId = await _commentRepository.GetCommentOwnerIdAsync(commentId);
            if (ownerId == null)
                throw new KeyNotFoundException("Yorum bulunamadı.");
            if (ownerId != userId)
                throw new UnauthorizedAccessException("Bu yorumu silme yetkiniz yok.");

            await _commentRepository.DeleteCommentAsync(commentId);
        }

        public async Task<bool> ToggleCommentLikeAsync(long commentId, long userId)
        {
            return await _commentRepository.ToggleCommentLikeAsync(commentId, userId);
        }

        private CommentDto MapToDto(Comment comment, long currentUserId, List<Comment> allReplies)
        {
            var replies = allReplies
                .Where(r => r.ParentCommentID == comment.CommentID)
                .Select(r => MapToDto(r, currentUserId, new List<Comment>())) 
                .ToList();

            return new CommentDto
            {
                CommentID = comment.CommentID,
                CommentText = comment.CommentText,
                CommentDate = comment.CommentDate,
                IsUpdate = comment.IsUpdate,
                UpdateTime = comment.UpdateTime,
                Author = new UserSummaryDto
                {
                    UserID = comment.User!.UserID,
                    Username = comment.User.Username,
                    PPUrl = null
                },
                LikeCount = comment.LikeCount,
                IsLikedByCurrentUser = comment.CommentLikes?.Any(l => l.UserID == currentUserId) ?? false,
                ParentCommentID = comment.ParentCommentID,
                Replies = replies
            };
        }
    }
}
