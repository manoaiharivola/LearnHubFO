using LearnHubFO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnHubFO.Api
{
    [Route("api/chapitres")]
    [ApiController]
    [AllowAnonymous]
    public class ChapitresUtilisateurApiController : ControllerBase
    {
        private readonly ChapitreUtilisateurService _chapitreUtilisateurService;

        public ChapitresUtilisateurApiController(ChapitreUtilisateurService chapitreUtilisateurService)
        {
            _chapitreUtilisateurService = chapitreUtilisateurService;
        }

        [HttpPost("{chapitreId}/utilisateur/{utilisateurId}/terminer")]
        public async Task<IActionResult> TermineChapitre(int chapitreId, int utilisateurId)
        {
            if (utilisateurId <= 0 || chapitreId <= 0)
            {
                return BadRequest(new { message = "Invalid user ID or course ID" });
            }

            var estSuivi = await _chapitreUtilisateurService.IsChapitreCompletedAsync(chapitreId, utilisateurId);
            if (estSuivi)
            {
                return Conflict(new { message = "Chapter already completed by user" });
            }

            await _chapitreUtilisateurService.MarquerCommeTermineAsync(chapitreId, utilisateurId);
            return Ok(new { message = "Chapter completed successfully" });
        }

        [HttpDelete("{chapitreId}/utilisateur/{utilisateurId}/nePasTerminer")]
        public async Task<IActionResult> NePasTermineChapitre(int chapitreId, int utilisateurId)
        {
            if (utilisateurId <= 0 || chapitreId <= 0)
            {
                return BadRequest(new { message = "Invalid user ID or course ID" });
            }

            var estSuivi = await _chapitreUtilisateurService.IsChapitreCompletedAsync(chapitreId, utilisateurId);
            if (!estSuivi)
            {
                return Conflict(new { message = "Chapter already incompleted by user" });
            }

            await _chapitreUtilisateurService.NePasMarquerCommeTermineAsync(chapitreId, utilisateurId);
            return Ok(new { message = "Chapter incompleted successfully" });
        }
    }
}
