using socialmedia.DTOs.Common;
using socialmedia.DTOs.Post.Request;
using socialmedia.DTOs.Post.Response;

namespace socialmedia.Services.Interfaces
{
    public interface IPostService
    {
        Task<PostDto> CreatePostAsync(CreatePostDto dto, long userId);
        Task<PostDto> GetPostByIdAsync(long postId, long currentUserId);
        Task<PostDto> UpdatePostAsync(long postId, UpdatePostDto dto, long userId);
        Task DeletePostAsync(long postId, long userId);
        Task<bool> TogglePostLikeAsync(long postId, long userId);
        Task<PagedResultDto<PostDto>> GetTimelineAsync(long userId, int page, int pageSize);
    }
}
