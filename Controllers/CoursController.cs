using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LearnHubFO.Services;
using LearnHubFO.Models;
using System;
using LearnHubBackOffice.Models;

namespace LearnHubFO.Controllers
{
    public class CoursController : Controller
    {
        private readonly CoursService _coursesService;

        public CoursController(CoursService coursesService)
        {
            _coursesService = coursesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            var totalCourses = await _coursesService.GetTotalCoursesCountAsync();
            var courses = await _coursesService.GetCoursesAsync(pageIndex, pageSize);
            var viewModel = new PagedResult<Cours>
            {
                Items = courses,
                PageIndex = pageIndex,
                TotalItems = totalCourses,
                PageSize = pageSize
            };
            ViewData["PageSize"] = pageSize;
            return View(viewModel);
        }
    }
}
