using LearnHubFO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnHubFO.Api
{
    [Route("api/chapitres")]
    [ApiController]
    [AllowAnonymous]
    public class ChapitresApiController : ControllerBase
    {
        private readonly ChapitreService _chapitreService;

        public ChapitresApiController(ChapitreService chapitreService)
        {
            _chapitreService = chapitreService;
        }
    }
}
