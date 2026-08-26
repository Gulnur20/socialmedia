using socialmedia.Models;
using socialmedia.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace socialmedia.Service
{
    public class PostService
    {
        private readonly PostRepository _postRepository;
        public PostService(PostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task CreatePostAsync(int userId, string caption, List<string> mediaUrls)
        {
            if (mediaUrls == null)
            {
                throw new ArgumentException("Gönderi oluşturabilmek için medya eklemelisiniz.");
            }

            if (mediaUrls.Count > 10)
            {
                throw new ArgumentException("Bir gönderiye en fazla 10 adet medya ekleyebilirsiniz.");
            }
            DateTime createdDate = DateTime.Now;

            MainPost newPost = new MainPost

            {
                PostID = 0,
                UserID = userId,
                Caption= caption,
                PostCreated = DateTime.Now

            };

            List<PostMedia> postMediaList = new List<PostMedia>();

            foreach (var url in mediaUrls)

            {
                postMediaList.Add(new PostMedia
                {
                    MediaID = 0,
                    PostID = 0,
                    MediaURL = url
                });

            }
            await _postRepository.CreatePostAsync(newPost, postMediaList);

        }
    }
}
              

    

