using LearnHubBackOffice.Models;
using LearnHubFO.Models;
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

        [HttpGet("utilisateur/{userId}")]
        public async Task<ActionResult<List<CoursSuivi>>> GetCoursesByUserId(int userId)
        {
            var courses = await _coursService.GetCoursesByUserIdAsync(userId);
            foreach (var course in courses)
            {
                var (totalChapitres, completedChapitres) = await _coursService.GetChapitreProgressAsync(course.IdCours, userId);
                course.TotalChapitres = totalChapitres;
                course.CompletedChapitres = completedChapitres;
            }
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cours>> GetCoursById(int id)
        {
            var cours = await _coursService.GetCoursByIdAsync(id);
            if (cours == null)
            {
                return NotFound(new { message = "Course not found" });
            }
            return Ok(cours);
        }

        [HttpGet("{courseId}/chapitres/utilisateur/{userId}")]
        public async Task<IActionResult> GetChapitresOfaCourse(int courseId, int userId)
        {
            var chapitres = await _coursService.GetChapitresByCourseIdAsync(courseId, userId);
            return Ok(chapitres);
        }
    }
}
