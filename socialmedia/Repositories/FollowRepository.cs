using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using socialmedia.DTOs;
using socialmedia.DTOs.Follow.Response;
using socialmedia.DTOs.Users.Response;
using socialmedia.Models;
using System.Collections.Generic;
using System.Linq;

namespace socialmedia.Repositories

{
    public class FollowRepository
    {

        private readonly string _connectionString;

        public FollowRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Connection string 'Default' bulunamadı.");
        }

    
        public async Task<List<FollowRequestDto>> GetWaitingFollowRequestsAsync(long userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                    SELECT
                        fr.RequestID, fr.RequestedDate,
                        u.UserID, u.Username,
                        up.PPUrl
                    FROM FollowRequest fr
                    INNER JOIN Users u ON fr.FollowerID = u.UserID
                    LEFT JOIN UserProfile up ON u.UserID = up.UserID
                    WHERE fr.FollowingID = @UserId
                    ORDER BY fr.RequestedDate DESC";

                var rows = await connection.QueryAsync<FollowRequestRow>(sql, new { UserId = userId });

                return rows.Select(r => new FollowRequestDto
                {
                    RequestID = r.RequestID,
                    RequestedDate = r.RequestedDate,
                    RequestingUser = new UserSummaryDto
                    {
                        UserID = r.UserID,
                        Username = r.Username,
                        PPUrl = r.PPUrl
                    }
                }).ToList();
            }
        }
        public async Task<List<long>> GetFollowingIdsAsync(long userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT FollowingID FROM Follow WHERE FollowerID = @UserId";
                var result = await connection.QueryAsync<long>(sql, new { UserId = userId });
                return result.AsList();
            }
        }


        public async Task<string> FollowUserAsync(long followerID, long followingID)
        {
            if (followerID == followingID)
                throw new ArgumentException("Kullanıcı kendini takip edemez.");

            using (var connection = new SqlConnection(_connectionString))
            {
            
                string checkSql = "SELECT IsPrivate FROM UserSettings WHERE UserID = @FollowingID";
                bool isPrivate = await connection.QueryFirstOrDefaultAsync<bool>(checkSql, new { FollowingID = followingID });

                if (isPrivate)
                {
                    string sql = @"
                        IF NOT EXISTS (SELECT 1 FROM FollowRequest WHERE FollowerID = @FollowerID AND FollowingID = @FollowingID)
                        BEGIN
                            INSERT INTO FollowRequest (FollowerID, FollowingID, RequestedDate) 
                            VALUES (@FollowerID, @FollowingID, GETUTCDATE());
                        END";

                    await connection.ExecuteAsync(sql, new { FollowerID = followerID, FollowingID = followingID });
                    return "RequestSent";
                }
                else
                {
                    string sql = @"
                        IF NOT EXISTS (SELECT 1 FROM Follow WHERE FollowerID = @FollowerID AND FollowingID = @FollowingID)
                        BEGIN
                            INSERT INTO Follow (FollowerID, FollowingID, FollowDate) 
                            VALUES (@FollowerID, @FollowingID, GETUTCDATE());

                            UPDATE UserProfile SET FollowerCount = FollowerCount + 1 WHERE UserID = @FollowingID;
                            UPDATE UserProfile SET FollowingCount = FollowingCount + 1 WHERE UserID = @FollowerID;
                        END";

                    await connection.ExecuteAsync(sql, new { FollowerID = followerID, FollowingID = followingID });
                    return "Following";
                }
            }
        }

    
        public async Task<bool> UnfollowAsync(long followerID, long followingID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                    IF EXISTS (SELECT 1 FROM Follow WHERE FollowerID = @FollowerID AND FollowingID = @FollowingID)
                    BEGIN
                        DELETE FROM Follow WHERE FollowerID = @FollowerID AND FollowingID = @FollowingID;

                        UPDATE UserProfile SET FollowerCount = CASE WHEN FollowerCount > 0 THEN FollowerCount - 1 ELSE 0 END WHERE UserID = @FollowingID;
                        UPDATE UserProfile SET FollowingCount = CASE WHEN FollowingCount > 0 THEN FollowingCount - 1 ELSE 0 END WHERE UserID = @FollowerID;
                    END";

                int rowsAffected = await connection.ExecuteAsync(sql, new { FollowerID = followerID, FollowingID = followingID });
                return rowsAffected > 0;
            }
        }
        public async Task<bool> AcceptFollowRequestAsync(long requestId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string getSql = "SELECT FollowerID, FollowingID FROM FollowRequest WHERE RequestID = @RequestId";
                var request = await connection.QueryFirstOrDefaultAsync<(long FollowerID, long FollowingID)>(getSql, new { RequestId = requestId });

                if (request.FollowerID == 0 && request.FollowingID == 0)
                    return false;

                string sql = @"
                    DELETE FROM FollowRequest WHERE RequestID = @RequestId;

                    INSERT INTO Follow (FollowerID, FollowingID, FollowDate) 
                    VALUES (@FollowerID, @FollowingID, GETUTCDATE());

                    UPDATE UserProfile SET FollowerCount = FollowerCount + 1 WHERE UserID = @FollowingID;
                    UPDATE UserProfile SET FollowingCount = FollowingCount + 1 WHERE UserID = @FollowerID;";

                int rowsAffected = await connection.ExecuteAsync(sql, new { RequestId = requestId, request.FollowerID, request.FollowingID });
                return rowsAffected > 0;
            }
        }

        public async Task<bool> RejectFollowRequestAsync(long requestId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM FollowRequest WHERE RequestID = @RequestId";
                int rowsAffected = await connection.ExecuteAsync(sql, new { RequestId = requestId });
                return rowsAffected > 0;
            }
        }

        public async Task<bool> IsFollowingAsync(long followerId, long followingId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT COUNT(1) FROM Follow WHERE FollowerID = @FollowerId AND FollowingID = @FollowingId";
                int count = await connection.ExecuteScalarAsync<int>(sql, new { FollowerId = followerId, FollowingId = followingId });
                return count > 0;
            }
        }
    }

    internal class FollowRequestRow
    {
        public long RequestID { get; set; }
        public DateTime RequestedDate { get; set; }
        public long UserID { get; set; }
        public string Username { get; set; }
        public string? PPUrl { get; set; }
     }
    
     
} 
