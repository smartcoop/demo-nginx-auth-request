using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationServer.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        [HttpGet("validate")]
        public HttpResponseMessage Validate()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
    }
}
