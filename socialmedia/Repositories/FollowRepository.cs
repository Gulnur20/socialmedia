using Dapper;
using Microsoft.Data.SqlClient;
using socialmedia.DataTransferObject;
using Microsoft.Extensions.Configuration;
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
            _connectionString = configuration.GetConnectionString("Default");
        }
        public async Task<List<FollowRequestDTO>> GetWaitingFollowRequestsAsync(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
                SELECT
                        fr.RequestID, fr.RequestDate,
                        u.UserID AS FollowerID, u.Username AS FollowerUsername, 
                        up.FirstName AS FollowerFirstName, up.LastName AS FollowerLastName, 
                        up.PPUrl AS FollowerPPUrl,
                        us.IsVerified
                    FROM FollowRequest fr
                    INNER JOIN Users u ON fr.FollowerID = u.UserID
                    LEFT JOIN UserProfile up ON u.UserID = up.UserID
                    LEFT JOIN UserSettings us ON u.UserID = us.UserID
                    WHERE fr.FollowingID = @UserId
                    ORDER BY fr.RequestDate DESC";

                var result = await connection.QueryAsync<FollowRequestDTO>(sql, new { UserId = userId });
                return result.AsList();
            }
        }
        public async Task<bool> FollowUserAsync(int followerID, int followingID)    
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string checkSql = "SELECT IsPrivate FROM UserProfile WHERE UserID = @FollowingID";
                bool isPrivate = await connection.QueryFirstOrDefaultAsync<bool>(checkSql, new { FollowingID = followingID });

                string sql = "";

                if (isPrivate)
                {
                    sql = @"
                        IF NOT EXISTS (SELECT 1 FROM FollowRequest WHERE FollowerID = @FollowerID AND FollowingID = @FollowingID)
                        BEGIN
                            INSERT INTO FollowRequest (FollowerID, FollowingID, RequestDate) 
                            VALUES (@FollowerID, @FollowingID, GETDATE());
                        END";
                }
                else
                { 
                    sql = @"
                        IF NOT EXISTS (SELECT 1 FROM Follows WHERE FollowerID = @FollowerID AND FollowingID = @FollowingID)
                        BEGIN
                            INSERT INTO Follows (FollowerID, FollowingID, FollowDate) 
                            VALUES (@FollowerID, @FollowingID, GETDATE());

                            UPDATE UserProfile SET FollowerCount = FollowerCount + 1 WHERE UserID = @FollowingID;
                            UPDATE UserProfile SET FollowingCount = FollowingCount + 1 WHERE UserID = @FollowerID;
                        END";
                }
                int rowsAffected = await connection.ExecuteAsync(sql, new { FollowerID = followerID, FollowingID = followingID });
                return rowsAffected > 0;
            }
        }
    }
}
    

