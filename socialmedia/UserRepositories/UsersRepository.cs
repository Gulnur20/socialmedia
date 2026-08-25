using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using socialmedia.Models;

namespace socialmedia.ado.UserRepostories

{
    public class UsersRepository 
    {
        private readonly string _connectionString = "Server=localhost;Database=SosyalMedyaDB;Trusted_Connection=True;TrustServerCertificate=True;";
            public UsersRepository()
            {
            }
             public List<Users> GetAllUsers()
            {
                List<Users> users = new List<Users>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT UserID, Username, Password, Email, IsActive, UserCreated FROM Users";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new Users
                                {
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    UserCreated = Convert.ToDateTime(reader["UserCreated"])
                                });
                            }
                        }
                    }
                }
                return users;
            }

            public void AddUser(Users user)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"INSERT INTO Users (Username, Password, Email, IsActive, UserCreated) 
                                 VALUES (@Username, @Password, @Email, @IsActive, @UserCreated)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                         command.Parameters.AddWithValue("@Username", user.Username);
                         command.Parameters.AddWithValue("@Password", user.Password);
                         command.Parameters.AddWithValue("@Email", user.Email);
                         command.Parameters.AddWithValue("@IsActive", user.IsActive);
                         command.Parameters.AddWithValue("@UserCreated", user.UserCreated);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }

        
            public void UpdateUser(Users user)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"UPDATE Users 
                                 SET Username = @Username, Password = @Password, Email = @Email, IsActive = @IsActive 
                                 WHERE UserID = @UserID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
 
                    command.Parameters.AddWithValue("@UserID", user.UserID);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@IsActive", user.IsActive);

                    connection.Open();
                    command.ExecuteNonQuery();
                    }
                }
            }

     
            public void DeleteUser(int userId)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "DELETE FROM Users WHERE UserID = @UserID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
