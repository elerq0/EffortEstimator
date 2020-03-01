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
    public class ChannelController : ControllerBase
    {
        private readonly GroupService IGroup;
        public ChannelController(GroupService IGroup)
        {
            this.IGroup = IGroup;
        }

        [HttpGet]
        [Route("Channel/{groupName}")]
        public IActionResult GetGroups(string groupName)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(IGroup.GetChannel(User.Claims.Where(x => x.Type == ClaimTypes.Email).Single().Value, groupName, 0)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}