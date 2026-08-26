using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using socialmedia.Models;

namespace socialmedia.Repositories.UserRepostories

{
    public class UsersRepository
    {
        private readonly string _connectionString = "Server=localhost;Database=SosyalMedyaDB;Trusted_Connection=True;TrustServerCertificate=True;";
        public async Task AddUserAsync(Users user, UserProfile profile, UserSettings settings)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
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
                            newUserId = Convert.ToInt32(await userCommand.ExecuteScalarAsync());
                        }

                        string profileQuery = @"INSERT INTO UserProfile (UserID, FirstName, LastName, Biography, PPUrl, BirthDate) 
                                            VALUES (@UserID, @FirstName, @LastName, @Biography, @PPUrl, @BirthDate)";

                        using (SqlCommand profileCommand = new SqlCommand(profileQuery, connection, transaction))
                        {
                            profileCommand.Parameters.AddWithValue("@UserID", newUserId);
                            profileCommand.Parameters.AddWithValue("@FirstName", profile.FirstName);
                            profileCommand.Parameters.AddWithValue("@LastName", profile.LastName);
                            profileCommand.Parameters.AddWithValue("@Biography", (object)profile.Biography ?? DBNull.Value);
                            profileCommand.Parameters.AddWithValue("@PPUrl", (object)profile.PPUrl ?? DBNull.Value);
                            profileCommand.Parameters.AddWithValue("@BirthDate", (object)profile.BirthDate ?? DBNull.Value);
                            await profileCommand.ExecuteNonQueryAsync();
                        }

                        string settingsQuery = @"INSERT INTO UserSettings (UserID, IsPrivate, IsVerified, IsEmailConfirmed, FailedLoginCount, LastUsernameChanged, LastPasswordChanged, LastLoginIP, LastLoginDate, DeletedDate) 
                                             VALUES (@UserID, @IsPrivate, @IsVerified, @IsEmailConfirmed, @FailedLoginCount, @LastUsernameChanged, @LastPasswordChanged, @LastLoginIP, @LastLoginDate, @DeletedDate)";

                        using (SqlCommand settingsCommand = new SqlCommand(settingsQuery, connection, transaction))
                        {
                            settingsCommand.Parameters.AddWithValue("@UserID", newUserId);
                            settingsCommand.Parameters.AddWithValue("@IsPrivate", settings.IsPrivate);
                            settingsCommand.Parameters.AddWithValue("@IsVerified", settings.IsVerified);
                            settingsCommand.Parameters.AddWithValue("@IsEmailConfirmed", settings.IsEmailConfirmed);
                            settingsCommand.Parameters.AddWithValue("@FailedLoginCount", settings.FailedLoginCount);
                            settingsCommand.Parameters.AddWithValue("@LastUsernameChanged", settings.LastUsernameChanged);
                            settingsCommand.Parameters.AddWithValue("@LastPasswordChanged", settings.LastPasswordChanged);
                            settingsCommand.Parameters.AddWithValue("@LastLoginIP", settings.LastLoginIP);
                            settingsCommand.Parameters.AddWithValue("@LastLoginDate", settings.LastLoginDate);
                            settingsCommand.Parameters.AddWithValue("@DeletedDate", settings.DeletedDate);
                            await settingsCommand.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task UpdateProfileAsync(int userId, string firstName, string lastName, string biography, string ppUrl)
         {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE UserProfile 
                               SET FirstName = @FirstName, 
                                   LastName = @LastName, 
                                   Biography = @Biography, 
                                   PPUrl = @PPUrl 
                               WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Biography", (object)biography ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PPUrl", (object)ppUrl ?? DBNull.Value);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task UpdateStatusAsync(int userId, bool isActive)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Users 
                               SET IsActive = @IsActive 
                               WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@IsActive", isActive);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task DeleteUserAsync(int userId, DateTime deletedDate)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE UserSettings 
                               SET DeletedDate = @DeletedDate 
                               WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@DeletedDate", deletedDate);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<bool> CheckUsernameAsync(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    await connection.OpenAsync();
                    int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }
    }
}




