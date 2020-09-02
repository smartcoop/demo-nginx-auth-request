using System;
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
        public string Validate()
        {
            Console.WriteLine("token validation request");
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return "Ok";
            }
            HttpContext.Response.StatusCode = 401;
            return null;

        }
    }
}
