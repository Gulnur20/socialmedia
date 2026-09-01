using socialmedia.DTOs.Common;
using socialmedia.DTOs.Post.Request;
using socialmedia.DTOs.Post.Response;
using socialmedia.DTOs.Users.Response;
using socialmedia.Models;
using socialmedia.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using socialmedia.Services.Interfaces;

namespace socialmedia.Services
{
    public class PostService:IPostService
    {
        private readonly PostRepository _postRepository;
        private readonly FollowRepository _followRepository;

        public PostService(PostRepository postRepository, FollowRepository followRepository)
        {
            _postRepository = postRepository;
            _followRepository = followRepository;
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto dto, long userId)
        {
            if (dto.Media == null || dto.Media.Count == 0)
                throw new ArgumentException("Gönderi oluşturabilmek için en az bir medya eklemelisiniz.");

            if (dto.Media.Count > 10)
                throw new ArgumentException("Bir gönderiye en fazla 10 adet medya ekleyebilirsiniz.");

            var newPost = new MainPost
            {
                UserID = userId,
                Caption = dto.Caption,
                PostCreated = DateTime.UtcNow
            };

            var postMediaList = new List<PostMedia>();
            byte order = 1;

            foreach (var item in dto.Media)
            {
                if (!Enum.TryParse<MediaType>(item.MediaType, true, out var mediaType))
                    throw new ArgumentException($"Geçersiz medya tipi: {item.MediaType}");

                postMediaList.Add(new PostMedia
                {
                    MediaURL = item.Url,
                    MediaType = mediaType,
                    DisplayOrder = order
                });
                order++;
            }

            long newPostId = await _postRepository.CreatePostAsync(newPost, postMediaList);

            var createdPost = await _postRepository.GetPostByIdAsync(newPostId);
            return MapToDto(createdPost!, userId);
        }

        public async Task<PostDto> GetPostByIdAsync(long postId, long currentUserId)
        {
            var post = await _postRepository.GetPostByIdAsync(postId);
            if (post == null)
                throw new KeyNotFoundException("Post bulunamadı.");

            return MapToDto(post, currentUserId);
        }

        public async Task<PostDto> UpdatePostAsync(long postId, UpdatePostDto dto, long userId)
        {
            var ownerId = await _postRepository.GetPostOwnerIdAsync(postId);
            if (ownerId == null)
                throw new KeyNotFoundException("Post bulunamadı.");
            if (ownerId != userId)
                throw new UnauthorizedAccessException("Bu postu güncelleme yetkiniz yok.");

            await _postRepository.UpdatePostAsync(postId, dto.Caption);

            var updatedPost = await _postRepository.GetPostByIdAsync(postId);
            return MapToDto(updatedPost!, userId);
        }

        public async Task DeletePostAsync(long postId, long userId)
        {
            var ownerId = await _postRepository.GetPostOwnerIdAsync(postId);
            if (ownerId == null)
                throw new KeyNotFoundException("Post bulunamadı.");
            if (ownerId != userId)
                throw new UnauthorizedAccessException("Bu postu silme yetkiniz yok.");

            await _postRepository.DeletePostAsync(postId);
        }

        public async Task<bool> TogglePostLikeAsync(long postId, long userId)
        {
            return await _postRepository.TogglePostLikeAsync(postId, userId);
        }

        public async Task<PagedResultDto<PostDto>> GetTimelineAsync(long userId, int page, int pageSize)
        {
            var followingIds = await _followRepository.GetFollowingIdsAsync(userId);

            var (posts, totalCount) = await _postRepository.GetTimelineAsync(followingIds, page, pageSize);

            var postDtos = posts.Select(p => MapToDto(p, userId)).ToList();

            return new PagedResultDto<PostDto>
            {
                Items = postDtos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = (page * pageSize) < totalCount
            };
        }

        private PostDto MapToDto(MainPost post, long currentUserId)
        {
            return new PostDto
            {
                PostID = post.PostID,
                Caption = post.Caption,
                PostCreated = post.PostCreated,
                IsEdited = post.IsEdited,
                Author = new UserSummaryDto
                {
                    UserID = post.User!.UserID,
                    Username = post.User.Username,
                    PPUrl = null
                },
                Media = post.PostMedia.Select(m => new PostMediaDto
                {
                    MediaID = m.MediaID,
                    MediaURL = m.MediaURL,
                    MediaType = m.MediaType.ToString(),
                    DisplayOrder = m.DisplayOrder
                }).ToList(),
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                IsLikedByCurrentUser = post.PostLikes?.Any(l => l.UserID == currentUserId) ?? false
            };
        }
    }
}
              

    

