using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LearnHubBackOffice.Models;
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

        public async Task<List<Cours>> GetCoursesAsync(int pageIndex, int pageSize)
        {
            var courses = new List<Cours>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT c.*, f.NomFormateur, f.Email AS FormateurEmail, cc.NomCoursCategorie AS CoursCategorieNom " +
                    "FROM Courses c " +
                    "JOIN Formateurs f ON c.IdFormateur = f.IdFormateur " +
                    "JOIN CoursCategories cc ON c.IdCoursCategorie = cc.IdCoursCategorie " +
                    "ORDER BY c.DateCreationCours DESC " +
                    "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY",
                    connection);
                command.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                command.Parameters.AddWithValue("@PageSize", pageSize);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        courses.Add(new Cours
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
                        });
                    }
                }
            }
            return courses;
        }

        public async Task<int> GetTotalCoursesCountAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT COUNT(*) FROM Courses", connection);
                await connection.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }
    }
}
