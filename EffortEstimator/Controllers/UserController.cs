using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EffortEstimator.Services;
using Newtonsoft.Json;

namespace EffortEstimator.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService IUser;
        public UserController(UserService IUser)
        {
            this.IUser = IUser;
        }

        [HttpPost]
        [Route("User/Login")]
        public IActionResult Login([FromForm] string email, [FromForm] string password)
        {
            try
            {
                string token = IUser.Login(email, password);
                return Ok(JsonConvert.SerializeObject(token));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("User/Register")]
        public IActionResult Register([FromForm] string email, [FromForm] string password, [FromForm] string name, [FromForm] string surname)
        {
            try
            {
                return Ok(IUser.Register(email, password, name, surname));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("User/Activate")]
        public IActionResult Activate([FromForm] string email, [FromForm] string activationKey)
        {
            try
            {
                return Ok(IUser.ActivateUser(email, activationKey));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("User/GetCurrent")]
        public IActionResult GetCurrentUser()
        {
            return Ok(User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault().Value);
        }


    }
}