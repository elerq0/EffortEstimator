using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
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
        public async Task<IActionResult> CreateConference([FromForm] string groupName, [FromForm] string topic, [FromForm] string description, [FromForm] DateTime startDate, IFormFile file)
        {
            try
            {
                return Ok(await IGroup.CreateConference(User.Claims.Where(x => x.Type == ClaimTypes.Email).Single().Value, groupName, topic, description, startDate, file));
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
                return Ok(JsonConvert.SerializeObject(IGroup.GetChannel(User.Claims.Where(x => x.Type == ClaimTypes.Email).Single().Value, groupName, conferenceId)));
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
                return Ok(JsonConvert.SerializeObject(IGroup.GetConferences(User.Claims.Where(x => x.Type == ClaimTypes.Email).Single().Value).Where(x => x.GroupName == groupName)));
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
                return Ok(JsonConvert.SerializeObject(IGroup.GetConferences(User.Claims.Where(x => x.Type == ClaimTypes.Email).Single().Value)));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Conference/CheckFile/{channelName}")]
        public IActionResult CheckFile(string channelName)
        {
            try
            {
                return Ok(IGroup.CheckIfFileExist(channelName));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Conference/GetFile/{channelName}")]
        public async Task<IActionResult> GetFile(string channelName)
        {
            try
            {
                string filePath = IGroup.GetFilePath(channelName);
                string fileName = Path.GetFileName(filePath);

                MemoryStream memory = new MemoryStream();
                using FileStream fileStream = new FileStream(filePath, FileMode.Open);
                await fileStream.CopyToAsync(memory);
                memory.Position = 0;

                FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
                string contentType;
                if (!provider.TryGetContentType(fileName, out contentType))
                {
                    contentType = "application/octet-stream";
                }

                return File(memory, contentType, Path.GetFileName(filePath));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("Conference/Vote")]
        public IActionResult VoteInConference([FromForm] string chaName, [FromForm] string result)
        {
            try
            {
                return Ok(IGroup.VoteInConference(User.Claims.Where(x => x.Type == ClaimTypes.Email).Single().Value, chaName, Double.Parse(result.Replace('.',','))));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Conference/GetEstimationForm/{channelName}")]
        public IActionResult GetEstimationForm(string channelName)
        {
            try
            {
                return Ok(IGroup.GetEstimationForm(User.Claims.Where(x => x.Type == ClaimTypes.Email).Single().Value, channelName));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("Conference/Increment")]
        public IActionResult IncrementConferenceState([FromForm] string channelName)
        {
            try
            {
                return Ok(IGroup.IncrementConferenceState(User.Claims.Where(x => x.Type == ClaimTypes.Email).Single().Value, channelName));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("Conference/Finish")]
        public IActionResult FinishConference([FromForm] string channelName)
        {
            try
            {
                return Ok(IGroup.ZeroConferenceState(User.Claims.Where(x => x.Type == ClaimTypes.Email).Single().Value, channelName));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}