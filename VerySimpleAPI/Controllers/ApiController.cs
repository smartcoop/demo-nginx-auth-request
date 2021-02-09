using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VerySimpleAPI.Controllers {
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase {

        [HttpGet]
        [Route("")]
        public string EmptyGet(string text) {
            return "No string to display";
        }

        [HttpGet]
        [Route("{text}")]
        public string Get(string text) {
            if (string.IsNullOrEmpty(text)) {
                return "";
            }
            return text;
        }

    }
}
