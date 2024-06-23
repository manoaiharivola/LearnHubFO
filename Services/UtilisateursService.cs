using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using LearnHubBackOffice.Models;

namespace LearnHubFO.Services
{
    public class UtilisateursService
    {
        private readonly string _connectionString;

        public UtilisateursService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public bool EmailExists(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Utilisateurs WHERE Email = @Email", connection);
                command.Parameters.AddWithValue("@Email", email);
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        public void Register(Utilisateur user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(
                    "INSERT INTO Utilisateurs (NomUtilisateur, Email, MotDePasseHash) VALUES (@NomUtilisateur, @Email, @MotDePasseHash)",
                    connection);
                command.Parameters.AddWithValue("@NomUtilisateur", user.NomUtilisateur);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@MotDePasseHash", user.MotDePasseHash);
                command.ExecuteNonQuery();
            }
        }
    }
}
