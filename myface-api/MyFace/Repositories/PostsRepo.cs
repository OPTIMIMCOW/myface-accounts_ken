using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MyFace.Models.Database;
using MyFace.Models.Request;
using MyFace.Repositories;

namespace MyFace.Repositories
{
    public interface IPostsRepo
    {
        IEnumerable<Post> Search(PostSearchRequest search);
        IEnumerable<Post> SearchFeed(FeedSearchRequest search);
        int Count(PostSearchRequest search);
        int CountFeed(FeedSearchRequest searchRequest);
        Post GetById(int id);
        Post Create(CreatePostRequest post);
        Post Update(int id, UpdatePostRequest update);
        void Delete(int id);
        bool IsAthenticated(string authHeader);
    }
    
    public class PostsRepo : IPostsRepo
    {
        private readonly MyFaceDbContext _context;

        public PostsRepo(MyFaceDbContext context)
        {
            _context = context;
        }
        
        public IEnumerable<Post> Search(PostSearchRequest search)
        {
            return _context.Posts
                .OrderByDescending(p => p.PostedAt)
                .Where(p => search.PostedBy == null || p.UserId == search.PostedBy)
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize);
        }
        
        public IEnumerable<Post> SearchFeed(FeedSearchRequest search)
        {
            return _context.Posts
                .OrderByDescending(p => p.PostedAt)
                .Where(p => search.PostedBy == null || p.UserId == search.PostedBy)
                .Where(p => search.LikedBy == null || p.Interactions.Where(i => i.Type == InteractionType.LIKE).Any(i => i.UserId == search.LikedBy))
                .Where(p => search.DislikedBy == null || p.Interactions.Where(i => i.Type == InteractionType.DISLIKE).Any(i => i.UserId == search.DislikedBy))
                .Include(p => p.User)
                .Include(p => p.Interactions).ThenInclude(i => i.User)
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize);
        }

        public int Count(PostSearchRequest search)
        {
            return _context.Posts
                .Count(p => search.PostedBy == null || p.UserId == search.PostedBy);
        }

        public int CountFeed(FeedSearchRequest search)
        {
            return _context.Posts
                .Where(p => search.PostedBy == null || p.UserId == search.PostedBy)
                .Where(p => search.LikedBy == null || p.Interactions.Where(i => i.Type == InteractionType.LIKE)
                                .Any(i => i.UserId == search.LikedBy))
                .Where(p => search.DislikedBy == null || p.Interactions.Where(i => i.Type == InteractionType.DISLIKE)
                                .Any(i => i.UserId == search.DislikedBy))
                .Count();
        }

        public Post GetById(int id)
        {
            return _context.Posts
                .Single(post => post.Id == id);
        }

        public Post Create(CreatePostRequest post)
        {
            var insertResult = _context.Posts.Add(new Post
            {
                ImageUrl = post.ImageUrl,
                Message = post.Message,
                PostedAt = DateTime.Now,
                UserId = post.UserId,
            });
            _context.SaveChanges();
            return insertResult.Entity;
        }

        public Post Update(int id, UpdatePostRequest update)
        {
            var post = GetById(id);

            post.Message = update.Message;
            post.ImageUrl = update.ImageUrl;

            _context.Posts.Update(post);
            _context.SaveChanges();
            
            return post;
        }

        public void Delete(int id)
        {
            var post = GetById(id);
            _context.Posts.Remove(post);
            _context.SaveChanges();
        }

        public bool IsAthenticated(string authHeader)
        {
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var encoding = Encoding.GetEncoding("iso-8859-1");
                var usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var seperatorIndex = usernamePassword.IndexOf(':');
                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);
                try
                {
                    var user = _context.Users.Where(u => u.Username==username).First();
                    var testHashedPassword = UsersRepo.CreateHash(user.Salt, password);
                    return (testHashedPassword == user.HashedPassword) ? true : false;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                return false;
            }
        }
    }
}