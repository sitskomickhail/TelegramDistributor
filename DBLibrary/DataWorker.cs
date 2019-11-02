using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public static class DataWorker
    {
        private static MySqlConnection conn;

        static DataWorker()
        {
            var connection = System.Configuration.ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            conn = new MySqlConnection(connection);
        }

        public static bool AddInfoToDB(string user, string sendingChannel, bool withPhoto, string sendingText, DateTime time, bool status, string exception)
        {
            conn.Open();
            //string sql = "INSERT INTO LicenseTBO (User, SendingChannel, active_key) VALUES (@key, @date, @active)";
            string sql = "INSERT INTO DistributionTBO (User, SendingChannel, isWithPhoto, SendingText, Time, Status, Exception) VALUES (@user, @sendingChannel, @isWithPhoto, @sendingText, @time, @status, @exception)";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@user", MySqlDbType.String).Value = user;
                cmd.Parameters.Add("@sendingChannel", MySqlDbType.String).Value = sendingChannel;
                cmd.Parameters.Add("@isWithPhoto", MySqlDbType.String).Value = withPhoto.ToString();
                cmd.Parameters.Add("@sendingText", MySqlDbType.String).Value = sendingText;
                cmd.Parameters.Add("@time", MySqlDbType.DateTime).Value = time;
                cmd.Parameters.Add("@status", MySqlDbType.String).Value = status.ToString();
                cmd.Parameters.Add("@exception", MySqlDbType.String).Value = exception;

                try
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
                }
                catch { return false; }
            }
            return true;
        }

        public static List<string> GetInfoFromDB()
        {
            List<string> list = new List<string>();
            conn.Open();
            string sql = "SELECT * FROM DistributionTBO WHERE 1";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                using (MySqlDataReader oReader = cmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        string tempStr = $"User: {oReader["User"].ToString()}\nStatus: {oReader["Status"].ToString()}\nChannel: {oReader["SendingChannel"].ToString()}\nWithPhoto? {oReader["isWithPhoto"].ToString()}" +
                            $"\nText: {oReader["SendingText"].ToString()}\nTime: {oReader["Time"].ToString()}\nException: {oReader["Exception"].ToString()}\n\n";
                        list.Add(tempStr);
                    }
                }
                conn.Close();
                conn.Dispose();
            }

            return list;
        }
    }
}
