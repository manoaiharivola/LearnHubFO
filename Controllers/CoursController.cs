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
        private const int PageSize = 1;

        public CoursController(CoursService coursesService)
        {
            _coursesService = coursesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            var totalCourses = await _coursesService.GetTotalCoursesCountAsync();
            var courses = await _coursesService.GetCoursesAsync(pageIndex, PageSize);
            var viewModel = new PagedResult<Cours>
            {
                Items = courses,
                PageIndex = pageIndex,
                TotalItems = totalCourses,
                PageSize = PageSize
            };
            return View(viewModel);
        }
    }
}
