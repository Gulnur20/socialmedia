using socialmedia.DTOs.Comment.Request;
using socialmedia.DTOs.Comment.Response;

namespace socialmedia.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentDto> AddCommentAsync(long postId, CreateCommentDto dto, long userId);
        Task<List<CommentDto>> GetCommentsByPostIdAsync(long postId, long currentUserId);
        Task<CommentDto> UpdateCommentAsync(long commentId, UpdateCommentDto dto, long userId);
        Task DeleteCommentAsync(long commentId, long userId);
        Task<bool> ToggleCommentLikeAsync(long commentId, long userId);
    }
}
