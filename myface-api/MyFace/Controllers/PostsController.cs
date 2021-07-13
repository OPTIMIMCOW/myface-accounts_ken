using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;

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
            var posts = _posts.Search(searchRequest);
            var postCount = _posts.Count(searchRequest);
            return PostListResponse.Create(searchRequest, posts, postCount);
        }

        [HttpGet("{id}")]
        public ActionResult<PostResponse> GetById([FromRoute] int id)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"];

            //System.Web.HttpContext.Current
            //HttpContext httpContext = HttpContext.Current;
            //HttpContext httpContext = HttpContext.Request;

            //var authHeader = this.httpContext.Request.Headers["Authorization"];
            // get the authentication parameters
            // get user from username = > get salt
            // hash password and compare
            // if true proceed otherwise send bad request
            //69	Brandon	Narraway	bnarraway4	bnarraway4@trellian.com	https://robohash.org/bnarraway4?set=any&bgset=any	https://picsum.photos/id/604/2400/900.jpg	U2Xl2FfWAjKhUJe80758QDCcJh3eW/Wva0b1qUsE2rA=	BoqxLjHCBsAFhYIggwLzmg==

            var post = _posts.GetById(id);
            return new PostResponse(post);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreatePostRequest newPost)
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

        [HttpPatch("{id}/update")]
        public ActionResult<PostResponse> Update([FromRoute] int id, [FromBody] UpdatePostRequest update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = _posts.Update(id, update);
            return new PostResponse(post);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _posts.Delete(id);
            return Ok();
        }
    }
}