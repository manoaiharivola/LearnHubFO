using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LearnHubBackOffice.Models;
using LearnHubBO.Models;
using LearnHubFO.Models;
using Microsoft.Extensions.Configuration;

namespace LearnHubFO.Services
{
    public class CoursService
    {
        private readonly string _connectionString;

        public CoursService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<ListeCoursUtilisateur>> GetCoursesWithFollowingStatusOfUserAsync(int pageIndex, int pageSize, string searchTerm, int userId)
        {
            var courses = new List<ListeCoursUtilisateur>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT c.*, f.NomFormateur, f.Email AS FormateurEmail, cc.NomCoursCategorie AS CoursCategorieNom, " +
                    "CASE WHEN cu.IdCours IS NOT NULL THEN 1 ELSE 0 END AS EstSuivi " +
                    "FROM Courses c " +
                    "JOIN Formateurs f ON c.IdFormateur = f.IdFormateur " +
                    "JOIN CoursCategories cc ON c.IdCoursCategorie = cc.IdCoursCategorie " +
                    "LEFT JOIN CoursUtilisateur cu ON c.IdCours = cu.IdCours AND cu.IdUtilisateur = @UserId " +
                    "WHERE (@SearchTerm IS NULL OR c.TitreCours LIKE '%' + @SearchTerm + '%') " +
                    "ORDER BY c.DateCreationCours DESC " +
                    "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY",
                    connection);
                command.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@SearchTerm", string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : searchTerm);
                command.Parameters.AddWithValue("@UserId", userId);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        courses.Add(new ListeCoursUtilisateur
                        {
                            IdCours = reader.GetInt32(reader.GetOrdinal("IdCours")),
                            TitreCours = reader.IsDBNull(reader.GetOrdinal("TitreCours")) ? "N/A" : reader.GetString(reader.GetOrdinal("TitreCours")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "N/A" : reader.GetString(reader.GetOrdinal("Description")),
                            DateCreationCours = reader.GetDateTime(reader.GetOrdinal("DateCreationCours")),
                            DateModificationCours = reader.GetDateTime(reader.GetOrdinal("DateModificationCours")),
                            IdFormateur = reader.GetInt32(reader.GetOrdinal("IdFormateur")),
                            IdCoursCategorie = reader.GetInt32(reader.GetOrdinal("IdCoursCategorie")),
                            Formateur = new Formateur
                            {
                                IdFormateur = reader.GetInt32(reader.GetOrdinal("IdFormateur")),
                                Email = reader.IsDBNull(reader.GetOrdinal("FormateurEmail")) ? "N/A" : reader.GetString(reader.GetOrdinal("FormateurEmail")),
                                NomFormateur = reader.IsDBNull(reader.GetOrdinal("NomFormateur")) ? "N/A" : reader.GetString(reader.GetOrdinal("NomFormateur"))
                            },
                            CoursCategorie = new CoursCategorie
                            {
                                IdCoursCategorie = reader.GetInt32(reader.GetOrdinal("IdCoursCategorie")),
                                NomCoursCategorie = reader.IsDBNull(reader.GetOrdinal("CoursCategorieNom")) ? "N/A" : reader.GetString(reader.GetOrdinal("CoursCategorieNom"))
                            },
                            EstSuivi = reader.GetInt32(reader.GetOrdinal("EstSuivi"))
                        });
                    }
                }
            }
            return courses;
        }

        public async Task<int> GetTotalCoursesWithFollowingStatusOfUserCountAsync(string searchTerm)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT COUNT(*) FROM Courses c " +
                    "WHERE (@SearchTerm IS NULL OR c.TitreCours LIKE '%' + @SearchTerm + '%')",
                    connection);
                command.Parameters.AddWithValue("@SearchTerm", string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : searchTerm);
                await connection.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task<Cours> GetCourseByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT c.*, f.NomFormateur, f.Email AS FormateurEmail, cc.NomCoursCategorie AS CoursCategorieNom " +
                    "FROM Courses c " +
                    "JOIN Formateurs f ON c.IdFormateur = f.IdFormateur " +
                    "JOIN CoursCategories cc ON c.IdCoursCategorie = cc.IdCoursCategorie " +
                    "WHERE c.IdCours = @IdCours",
                    connection);
                command.Parameters.AddWithValue("@IdCours", id);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Cours
                        {
                            IdCours = reader.GetInt32(reader.GetOrdinal("IdCours")),
                            TitreCours = reader.IsDBNull(reader.GetOrdinal("TitreCours")) ? "N/A" : reader.GetString(reader.GetOrdinal("TitreCours")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "N/A" : reader.GetString(reader.GetOrdinal("Description")),
                            DateCreationCours = reader.GetDateTime(reader.GetOrdinal("DateCreationCours")),
                            DateModificationCours = reader.GetDateTime(reader.GetOrdinal("DateModificationCours")),
                            IdFormateur = reader.GetInt32(reader.GetOrdinal("IdFormateur")),
                            IdCoursCategorie = reader.GetInt32(reader.GetOrdinal("IdCoursCategorie")),
                            Formateur = new Formateur
                            {
                                IdFormateur = reader.GetInt32(reader.GetOrdinal("IdFormateur")),
                                Email = reader.IsDBNull(reader.GetOrdinal("FormateurEmail")) ? "N/A" : reader.GetString(reader.GetOrdinal("FormateurEmail")),
                                NomFormateur = reader.IsDBNull(reader.GetOrdinal("NomFormateur")) ? "N/A" : reader.GetString(reader.GetOrdinal("NomFormateur"))
                            },
                            CoursCategorie = new CoursCategorie
                            {
                                IdCoursCategorie = reader.GetInt32(reader.GetOrdinal("IdCoursCategorie")),
                                NomCoursCategorie = reader.IsDBNull(reader.GetOrdinal("CoursCategorieNom")) ? "N/A" : reader.GetString(reader.GetOrdinal("CoursCategorieNom"))
                            }
                        };
                    }
                }
            }
            return null;
        }

        public async Task<List<Chapitre>> GetChapitresByCourseIdAsync(int courseId)
        {
            var chapitres = new List<Chapitre>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Chapitres " +
                    "WHERE IdCours = @IdCours " +
                    "ORDER BY Ordre",
                    connection);
                command.Parameters.AddWithValue("@IdCours", courseId);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        chapitres.Add(new Chapitre
                        {
                            IdChapitre = reader.GetInt32(reader.GetOrdinal("IdChapitre")),
                            TitreChapitre = reader.GetString(reader.GetOrdinal("TitreChapitre")),
                            Ordre = reader.GetInt32(reader.GetOrdinal("Ordre")),
                            Contenu = reader.GetString(reader.GetOrdinal("Contenu")),
                            IdCours = reader.GetInt32(reader.GetOrdinal("IdCours")),
                            DateCreationChapitre = reader.GetDateTime(reader.GetOrdinal("DateCreationChapitre")),
                            DateModificationChapitre = reader.GetDateTime(reader.GetOrdinal("DateModificationChapitre"))
                        });
                    }
                }
            }
            return chapitres;
        }
    }
}
