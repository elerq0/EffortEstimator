using System.Linq;
using System.Security.Claims;
using EffortEstimator.Helpers;
using EffortEstimator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;


namespace EffortEstimator.Controllers
{
    [Authorize]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("Home/Test")]
        public IActionResult Test()
        {
            return Ok("IT WORKS! SOMETIMES");
        }

        [HttpGet]
        [Route("Home/Test2")]
        public IActionResult Test2()
        {
            string token = Request.Headers["Authorization"];

            return Ok(token);
        }

        [HttpGet]
        [Route("Home/Test3")]
        public IActionResult Test3()
        {
            string email = User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault().Value;

            return Ok(email);
        }
    }
}