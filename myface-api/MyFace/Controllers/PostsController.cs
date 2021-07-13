using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using System;
using System.Text;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("/posts")]
    public class PostsController : ControllerBase
    {
        private readonly IPostsRepo _posts;

        public PostsController(IPostsRepo posts)
        {
            _posts = posts;
        }

        [HttpGet("")]
        public ActionResult<PostListResponse> Search([FromQuery] PostSearchRequest searchRequest)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var authenticated = _posts.DoBasicAuth(authHeader);

            if (authenticated)
            {
                var posts = _posts.Search(searchRequest);
                var postCount = _posts.Count(searchRequest);
                return PostListResponse.Create(searchRequest, posts, postCount);
            }
            return null;
        }

        [HttpGet("{id}")]
        public ActionResult<PostResponse> GetById([FromRoute] int id)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var authenticated = _posts.DoBasicAuth(authHeader);

            if (authenticated)
            {
                var post = _posts.GetById(id);
                return new PostResponse(post);
            }

            return null;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreatePostRequest newPost)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var authenticated = _posts.DoBasicAuth(authHeader);

            if (authenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var post = _posts.Create(newPost);

                var url = Url.Action("GetById", new { id = post.Id });
                var postResponse = new PostResponse(post);
                return Created(url, postResponse);
            }
            return null;
        }

        [HttpPatch("{id}/update")]
        public ActionResult<PostResponse> Update([FromRoute] int id, [FromBody] UpdatePostRequest update)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var authenticated = _posts.DoBasicAuth(authHeader);

            if (authenticated)
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var post = _posts.Update(id, update);
                return new PostResponse(post);
            }

            return null;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var authenticated = _posts.DoBasicAuth(authHeader);

            if (authenticated)
            {
                _posts.Delete(id);
                return Ok();
            }
            return null;
        }
    }
}