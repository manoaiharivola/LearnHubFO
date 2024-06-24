using LearnHubBackOffice.Models;
using LearnHubFO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnHubFO.Api
{
    [Route("api/cours")]
    [ApiController]
    [AllowAnonymous]
    public class CoursApiController : ControllerBase
    {
        private readonly CoursService _coursService;

        public CoursApiController(CoursService coursService)
        {
            _coursService = coursService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cours>>> GetAllCourses()
        {
            var courses = await _coursService.GetAllCoursesAsync();
            return Ok(courses);
        }
    }
}
