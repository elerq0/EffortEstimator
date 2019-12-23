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
    public class ConferenceController : ControllerBase
    {
        private readonly GroupService IGroup;
        public ConferenceController(GroupService IGroup)
        {
            this.IGroup = IGroup;
        }

        [HttpPost]
        [Route("Conference/Create")]
        public IActionResult CreaqteConference([FromForm] string groupName, [FromForm] string topic, [FromForm] string description, [FromForm] DateTime startDate)
        {
            try
            {
                return Ok(IGroup.CreateConference(User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault().Value, groupName, topic, description, startDate));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Conference/{groupName}/{conferenceId}")]
        public IActionResult GetChannel(string groupName, int conferenceId)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(IGroup.GetChannel(User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault().Value, groupName, conferenceId)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Conferences/{groupName}")]
        public IActionResult GetConferences(string groupName)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(IGroup.GetConferences(User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault().Value).Where(x => x.GroupName == groupName)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Conferences")]
        public IActionResult GetMyConferences()
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(IGroup.GetConferences(User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault().Value)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}