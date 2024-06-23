using LearnHubBackOffice.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LearnHubFO.Models
{
    public class ConsulterChapitre
    {
        public int IdChapitre { get; set; }
        public string TitreChapitre { get; set; }
        public int Ordre { get; set; }
        public string Contenu { get; set; }
        public int IdCours { get; set; }
        public Cours Cours { get; set; }
        public DateTime DateCreationChapitre { get; set; }
        public DateTime DateModificationChapitre { get; set; }
        public int? PreviousChapitreId { get; set; }
        public int? NextChapitreId { get; set; }

        public bool IsCompleted { get; set; }
    }
}
