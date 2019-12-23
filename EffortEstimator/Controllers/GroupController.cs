using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using EffortEstimator.Services;

namespace EffortEstimator.Controllers
{
    [Authorize]
    [ApiController]
    public class GroupController : ControllerBase
    {

        private readonly GroupService IGroup;
        public GroupController(GroupService IGroup)
        {
            this.IGroup = IGroup;
        }

        [HttpPost]
        [Route("Group/Create")]
        public IActionResult CreateGroup([FromForm] string groupName)
        {
            try
            {
                return Ok(IGroup.CreateGroup(groupName, User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault().Value));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("Group/CreateJoiningKey")]
        public IActionResult CreateJoiningKey([FromForm] string groupName)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(IGroup.CreateJoiningKey(groupName, User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault().Value)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("Group/Join")]
        public IActionResult JoinGroup([FromForm] string joiningKey)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(IGroup.JoinGroup(User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault().Value, joiningKey)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Groups")]
        public IActionResult GetGroups()
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(IGroup.GetGroups(User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault().Value)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}