using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using socialmedia.Models;
using System;

namespace socialmedia.Repositories.UserRepostories

{
    public class UsersRepository
    {
        private readonly string _connectionString;

        public UsersRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Connection string 'Default' bulunamadı.");
        }

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
                                         SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                        long newUserId;
                        using (SqlCommand userCommand = new SqlCommand(userQuery, connection, transaction))
                        {
                            userCommand.Parameters.AddWithValue("@Username", user.Username);
                            userCommand.Parameters.AddWithValue("@Password", user.Password);
                            userCommand.Parameters.AddWithValue("@Email", user.Email);
                            userCommand.Parameters.AddWithValue("@IsActive", user.IsActive);
                            userCommand.Parameters.AddWithValue("@UserCreated", user.UserCreated);
                            newUserId = Convert.ToInt64(await userCommand.ExecuteScalarAsync());
                        }

                        user.UserID = newUserId;

                        string profileQuery = @"INSERT INTO UserProfile (UserID, FirstName, LastName, Biography, PPUrl, BirthDate) 
                                            VALUES (@UserID, @FirstName, @LastName, @Biography, @PPUrl, @BirthDate)";

                        using (SqlCommand profileCommand = new SqlCommand(profileQuery, connection, transaction))
                        {
                            profileCommand.Parameters.AddWithValue("@UserID", newUserId);
                            profileCommand.Parameters.AddWithValue("@FirstName", profile.FirstName);
                            profileCommand.Parameters.AddWithValue("@LastName", profile.LastName);
                            profileCommand.Parameters.AddWithValue("@Biography", (object?)profile.Biography ?? DBNull.Value);
                            profileCommand.Parameters.AddWithValue("@PPUrl", (object?)profile.PPUrl ?? DBNull.Value);
                            profileCommand.Parameters.AddWithValue("@BirthDate", profile.BirthDate);
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
                            settingsCommand.Parameters.AddWithValue("@LastLoginIP", (object?)settings.LastLoginIP ?? DBNull.Value);
                            settingsCommand.Parameters.AddWithValue("@LastLoginDate", settings.LastLoginDate);
                            settingsCommand.Parameters.AddWithValue("@DeletedDate", (object?)settings.DeletedDate ?? DBNull.Value);
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

        public async Task UpdateProfileAsync(long userId, string firstName, string lastName, string? biography, string? ppUrl, DateTime birthDate)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE UserProfile 
                               SET FirstName = @FirstName, 
                                   LastName = @LastName, 
                                   Biography = @Biography, 
                                   PPUrl = @PPUrl,
                                   BirthDate = @BirthDate
                               WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Biography", (object?)biography ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PPUrl", (object?)ppUrl ?? DBNull.Value);
                    command.Parameters.AddWithValue("@BirthDate", birthDate);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateStatusAsync(long userId, bool isActive)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Users SET IsActive = @IsActive WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@IsActive", isActive);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteUserAsync(long userId, DateTime deletedDate)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string settingsSql = @"UPDATE UserSettings SET DeletedDate = @DeletedDate WHERE UserID = @UserID";
                        using (SqlCommand settingsCommand = new SqlCommand(settingsSql, connection, transaction))
                        {
                            settingsCommand.Parameters.AddWithValue("@UserID", userId);
                            settingsCommand.Parameters.AddWithValue("@DeletedDate", deletedDate);
                            await settingsCommand.ExecuteNonQueryAsync();
                        }

                        string usersSql = @"UPDATE Users SET IsActive = 0 WHERE UserID = @UserID";
                        using (SqlCommand usersCommand = new SqlCommand(usersSql, connection, transaction))
                        {
                            usersCommand.Parameters.AddWithValue("@UserID", userId);
                            await usersCommand.ExecuteNonQueryAsync();
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

        public async Task<Users?> GetByUsernameAsync(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT UserID, Username, Password, Email, IsActive, UserCreated 
                               FROM Users WHERE Username = @Username";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Users
                            {
                                UserID = (long)reader["UserID"],
                                Username = (string)reader["Username"],
                                Password = (string)reader["Password"], 
                                Email = (string)reader["Email"],
                                IsActive = (bool)reader["IsActive"],
                                UserCreated = (DateTime)reader["UserCreated"]
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public async Task UpdateLoginSuccessAsync(long userId, string ipAddress, DateTime loginDate)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE UserSettings 
                               SET LastLoginIP = @LastLoginIP, 
                                   LastLoginDate = @LastLoginDate,
                                   FailedLoginCount = 0
                               WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@LastLoginIP", ipAddress);
                    command.Parameters.AddWithValue("@LastLoginDate", loginDate);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task IncrementFailedLoginAsync(long userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE UserSettings SET FailedLoginCount = FailedLoginCount + 1 WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

       
        public async Task<UserFullData?> GetUserFullDataAsync(long userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            { 
                string sql = @"
            SELECT u.UserID, u.Username, u.Email, u.IsActive, u.UserCreated,
                   p.FirstName, p.LastName, p.Biography, p.PPUrl, p.BirthDate, p.FollowerCount, p.FollowingCount,
                   s.IsPrivate, s.IsVerified, s.IsEmailConfirmed
            FROM Users u
            JOIN UserProfile p ON u.UserID = p.UserID
            JOIN UserSettings s ON u.UserID = s.UserID
            WHERE u.UserID = @UserID";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new UserFullData
                            {
                                User = new Users
                                {
                                    UserID = (long)reader["UserID"],
                                    Username = (string)reader["Username"],
                                    Password = string.Empty,
                                    Email = (string)reader["Email"],
                                    IsActive = (bool)reader["IsActive"],
                                    UserCreated = (DateTime)reader["UserCreated"]
                                },
                                Profile = new UserProfile
                                {
                                    UserID = (long)reader["UserID"],
                                    FirstName = (string)reader["FirstName"],
                                    LastName = (string)reader["LastName"],
                                    Biography = reader["Biography"] as string,
                                    PPUrl = reader["PPUrl"] as string,
                                    BirthDate = (DateTime)reader["BirthDate"],                     
                                    FollowerCount = (int)reader["FollowerCount"],
                                    FollowingCount = (int)reader["FollowingCount"]
                                },
                                Settings = new UserSettings
                                {
                                    UserID = (long)reader["UserID"],
                                    IsPrivate = (bool)reader["IsPrivate"],
                                    IsVerified = (bool)reader["IsVerified"],
                                    IsEmailConfirmed = (bool)reader["IsEmailConfirmed"]
                                }
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public async Task UpdatePrivacyAsync(long userId, bool isPrivate)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE UserSettings SET IsPrivate = @IsPrivate WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@IsPrivate", isPrivate);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }

    public class UserFullData
    {
        public Users User { get; set; } = null!;
        public UserProfile Profile { get; set; } = null!;
        public UserSettings Settings { get; set; } = null!;
    }
}
        






