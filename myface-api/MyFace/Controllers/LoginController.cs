using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("/login")]
    public class LoginController : ControllerBase
    {
        private readonly IPostsRepo _posts;
        public LoginController(IPostsRepo posts)
        {
            _posts = posts;
        }

        [HttpGet("")]
        public ActionResult<bool> TryLogin([FromQuery] FeedSearchRequest searchRequest)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var authenticated = _posts.IsAthenticated(authHeader);

            if (!authenticated)
            {
                return Unauthorized();
            }

            return authenticated;
        }
    }
}