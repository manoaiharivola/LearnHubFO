using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LearnHubBackOffice.Models;
using LearnHubBO.Models;
using Microsoft.Extensions.Configuration;

namespace LearnHubFO.Services
{
    public class CoursUtilisateurService
    {
        private readonly string _connectionString;

        public CoursUtilisateurService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task SuivreCoursAsync(int userId, int courseId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "INSERT INTO CoursUtilisateur (IdCours, IdUtilisateur, DateCreationCoursUtilisateur) VALUES (@IdCours, @IdUtilisateur, @DateCreation)",
                    connection);
                command.Parameters.AddWithValue("@IdCours", courseId);
                command.Parameters.AddWithValue("@IdUtilisateur", userId);
                command.Parameters.AddWithValue("@DateCreation", DateTime.Now);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
