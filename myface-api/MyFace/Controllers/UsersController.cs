using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using System.Web;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepo _users;
        private readonly IPostsRepo _posts;

        public UsersController(IUsersRepo users, IPostsRepo posts)
        {
            _users = users;
            _posts = posts;
        }
        
        [HttpGet("")]
        public ActionResult<UserListResponse> Search([FromQuery] UserSearchRequest searchRequest)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var authenticated = _posts.DoBasicAuth(authHeader);

            if (authenticated)
            {
                var users = _users.Search(searchRequest);
                var userCount = _users.Count(searchRequest);
                return UserListResponse.Create(searchRequest, users, userCount);
            }
            return null;
        }

        [HttpGet("{id}")]
        public ActionResult<UserResponse> GetById([FromRoute] int id)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var authenticated = _posts.DoBasicAuth(authHeader);

            if (authenticated)
            {
                var user = _users.GetById(id);
                return new UserResponse(user);
            }
            return null;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateUserRequest newUser)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var authenticated = _posts.DoBasicAuth(authHeader);

            if (authenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _users.Create(newUser);

                var url = Url.Action("GetById", new { id = user.Id });
                var responseViewModel = new UserResponse(user);
                return Created(url, responseViewModel);
            }
            return null;
        }

        [HttpPatch("{id}/update")]
        public ActionResult<UserResponse> Update([FromRoute] int id, [FromBody] UpdateUserRequest update)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var authenticated = _posts.DoBasicAuth(authHeader);

            if (authenticated)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _users.Update(id, update);
                return new UserResponse(user);
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
                _users.Delete(id);
                return Ok();
            }
            return null;
        }
    }
}