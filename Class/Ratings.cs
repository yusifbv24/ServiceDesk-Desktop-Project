using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceDesk.Class
{
    public static class Ratings
    {
        private readonly static string _firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("yyyy-MM-dd");
        private readonly static string _today = DateTime.Today.ToString("yyyy-MM-dd");
        private readonly static List<float> _ratingValues = [];
        private static float _sumOfRatingValues = 0;
        private static float CSATValue = 0;
        private static SqlConnection _connection { get; set; } = null;
        private static async Task ConnectToTheDatabase()
        {
            if (_connection == null)
            {
                _connection = await ConnectionDatabase.ConnectToTheServer();
                await _connection.OpenAsync();
            }
            if (_connection.State == ConnectionState.Closed)
            {
                await _connection.OpenAsync();
            }
        }
        private static async Task SearchForRatingValues(string fullname,Guid sessionId)
        {
            string query = @"SELECT Rating.rating
                                     FROM Rating
                                     INNER JOIN Ticket WITH (NOLOCK) ON Rating.ID = Ticket.ID
                                     INNER JOIN Status WITH (NOLOCK) ON Rating.ID=Status.ID
                                     WHERE Status.time BETWEEN @before30daysago AND @today 
                                     AND Ticket.fullname LIKE @fullname ";
            if (!string.IsNullOrEmpty(fullname)) //to avoid error
            {
                try
                {
                    if (_connection == null || _connection.State == ConnectionState.Closed)
                    {
                        await ConnectToTheDatabase();
                    }
                    using SqlCommand cm = new(query, _connection);
                    cm.Parameters.AddWithValue("@before30daysago", _firstDayOfMonth);
                    cm.Parameters.AddWithValue("@today", _today);
                    cm.Parameters.AddWithValue("@fullname", $"%{fullname}%");
                    using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                    if (!dr.HasRows)
                    {
                        _ratingValues.Add(0);
                    }
                    while (await dr.ReadAsync())
                    {
                        if (dr["rating"] == DBNull.Value)
                        {
                            continue;
                        }
                        if (float.TryParse(dr["rating"]?.ToString() ?? "0", out float ratingValue))
                        {
                            _ratingValues.Add(ratingValue);
                        }
                    }   
                }
                catch (Exception ex)
                {
                    Notifications.Error(ex.Message);
                    await Logger.Log(fullname, $" | Error occured in Ratings Class while running SearchForRatingValue. | Error is: {ex.Message}");
                }
            }
        }
        public static async Task<float> CalculateCSAT(string fullname,Guid sessionId)
        {
            try
            {
                _sumOfRatingValues = 0;
                _ratingValues.Clear(); // Clear previous values
                await SearchForRatingValues(fullname, sessionId);

                if (_ratingValues.Count > 0)
                {
                    float totalRatings = 5 * _ratingValues.Count;
                    _sumOfRatingValues = _ratingValues.Sum();
                    CSATValue = (_sumOfRatingValues / totalRatings) * 100;
                }
                return CSATValue;
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message);
                await Logger.Log(fullname, $" | Error in Ratings class while running CalculateCSAT for {fullname}. | Error: {ex.Message}");
                return 0;
            }
            finally
            {
                await UpdateCSATValue(fullname, sessionId);
            }
        }
        private static async Task UpdateCSATValue(string fullname,Guid sessionId)
        {
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using SqlCommand cm = new("UPDATE Users SET csat=@csat WHERE fullname LIKE @fullname", _connection);
                cm.Parameters.AddWithValue("@fullname", fullname);
                if (float.TryParse(CSATValue.ToString(), out float parsedCSATValue))
                {
                    int roundedCSATValue = (int)Math.Round(parsedCSATValue);
                    cm.Parameters.AddWithValue("@csat", roundedCSATValue);
                }
                else
                {
                    cm.Parameters.AddWithValue("@csat", DBNull.Value); // Handle invalid numbers
                }
                await cm.ExecuteNonQueryAsync();
                _connection.Close();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message);
                await Logger.Log(fullname, $" | Error occured in Ratings Class while running UpdateCSATValue. | Error is: {ex.Message}");
            }
        }
    }
}