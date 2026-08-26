using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using socialmedia.Models;

using System;
using System.Data;
using System.Data.SqlClient;

namespace socialmedia.Repositories.UserRepostories

{
    public class UsersRepository
    {
        private readonly string _connectionString = "Server=localhost;Database=SosyalMedyaDB;Trusted_Connection=True;TrustServerCertificate=True;";
        public void AddUser(Users user, UserProfile profile, UserSettings settings)

        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string userQuery = @"INSERT INTO Users (Username, Password, Email, IsActive, UserCreated) 
                                         VALUES (@Username, @Password, @Email, @IsActive, @UserCreated);
                                         SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        int newUserId;
                        using (SqlCommand userCommand = new SqlCommand(userQuery, connection, transaction))
                        {
                            userCommand.Parameters.AddWithValue("@Username", user.Username);
                            userCommand.Parameters.AddWithValue("@Password", user.Password);
                            userCommand.Parameters.AddWithValue("@Email", user.Email);
                            userCommand.Parameters.AddWithValue("@IsActive", user.IsActive);
                            userCommand.Parameters.AddWithValue("@UserCreated", user.UserCreated);
                            newUserId = Convert.ToInt32(userCommand.ExecuteScalar());
                        }
                        string profileQuery = @"INSERT INTO UserProfile (UserID, FirstName, LastName, Biography, PPUrl, BirthDate) 
                                            VALUES (@UserID, @FirstName, @LastName, @Biography, @PPUrl)";

                        using (SqlCommand profileCommand = new SqlCommand(profileQuery, connection, transaction))
                        {
                            profileCommand.Parameters.AddWithValue("@UserID", newUserId);
                            profileCommand.Parameters.AddWithValue("@FirstName", profile.FirstName);
                            profileCommand.Parameters.AddWithValue("@LastName", profile.LastName);
                            profileCommand.Parameters.AddWithValue("@Biography", (object)profile.Biography ?? DBNull.Value);
                            profileCommand.Parameters.AddWithValue("@PPUrl", (object)profile.PPUrl ?? DBNull.Value);
                            profileCommand.ExecuteNonQuery();
                        }
                        string settingsQuery = @"INSERT INTO UserSettings (UserID, IsPrivate, IsVerified, IsEmailConfirmed, FailedLoginCount, LastLoginIP) 
                                             VALUES (@UserID, @IsPrivate, @IsVerified, @IsEmailConfirmed, @FailedLoginCount, @LastLoginIP)";

                        using (SqlCommand settingsCommand = new SqlCommand(settingsQuery, connection, transaction))
                        {
                            settingsCommand.Parameters.AddWithValue("@UserID", newUserId);
                            settingsCommand.Parameters.AddWithValue("@IsPrivate", settings.IsPrivate);
                            settingsCommand.Parameters.AddWithValue("@IsVerified", settings.IsVerified);
                            settingsCommand.Parameters.AddWithValue("@IsEmailConfirmed", settings.IsEmailConfirmed);
                            settingsCommand.Parameters.AddWithValue("@FailedLoginCount", settings.FailedLoginCount);
                            settingsCommand.Parameters.AddWithValue("@LastLoginIP", settings.LastLoginIP );
                            settingsCommand.ExecuteNonQuery();
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}

