using System.Data.SqlClient;

namespace LearnHubFO.Services
{
    public class ChapitreUtilisateurService
    {
        private readonly string _connectionString;

        public ChapitreUtilisateurService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> IsChapitreCompletedAsync(int chapitreId, int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT COUNT(*) FROM ChapitreUtilisateur WHERE IdChapitre = @IdChapitre AND IdUtilisateur = @IdUtilisateur",
                    connection);
                command.Parameters.AddWithValue("@IdChapitre", chapitreId);
                command.Parameters.AddWithValue("@IdUtilisateur", userId);
                await connection.OpenAsync();
                var count = (int)await command.ExecuteScalarAsync();
                return count > 0;
            }
        }

        public async Task MarquerCommeTermineAsync(int chapitreId, int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "INSERT INTO ChapitreUtilisateur (IdChapitre, IdUtilisateur, DateCreationChapitreUtilisateur) VALUES (@IdChapitre, @IdUtilisateur, @DateCreationChapitreUtilisateur)",
                    connection);
                command.Parameters.AddWithValue("@IdChapitre", chapitreId);
                command.Parameters.AddWithValue("@IdUtilisateur", userId);
                command.Parameters.AddWithValue("@DateCreationChapitreUtilisateur", DateTime.UtcNow);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task NePasMarquerCommeTermineAsync(int chapitreId, int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "DELETE FROM ChapitreUtilisateur WHERE IdChapitre = @IdChapitre AND IdUtilisateur = @IdUtilisateur",
                    connection);
                command.Parameters.AddWithValue("@IdChapitre", chapitreId);
                command.Parameters.AddWithValue("@IdUtilisateur", userId);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
