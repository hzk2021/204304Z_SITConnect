using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace _204304Z_SITConnect.Helpers
{
    public class PasswordRecord
    {
        static string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        public static void StorePassword(string email, string passwordhashed , string passwordsalt)
        {
            string sql = "INSERT INTO PasswordRecord VALUES(@EMAIL, @PASSWORDHASHED, @PASSWORDSALT)";

            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@EMAIL", email);
                    cmd.Parameters.AddWithValue("@PASSWORDHASHED", passwordhashed);
                    cmd.Parameters.AddWithValue("@PASSWORDSALT", passwordsalt);

                    cmd.Connection = con;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new HttpException(502, "Error With DB");
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        public static List<string> GetAllSalt(string email)
        {
            List<string> tempSaltStore = new List<string>();

            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT PasswordSalt FROM PasswordRecord WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                h = reader["PasswordSalt"].ToString();

                                tempSaltStore.Add(h);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }

            return tempSaltStore;
        }

        public static List<string> GetAllHashedPW(string email)
        {
            List<string> tempHashedPWStore = new List<string>();

            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT PasswordHashed FROM PasswordRecord WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHashed"] != null)
                        {
                            if (reader["PasswordHashed"] != DBNull.Value)
                            {
                                h = reader["PasswordHashed"].ToString();

                                tempHashedPWStore.Add(h);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }

            return tempHashedPWStore;
        }

        public static DateTime GetLastChangedPassDateTime(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT LastChangedPassword from Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", HttpUtility.HtmlEncode(email));

            try
            {
                connection.Open();

                DateTime lastchangedPW = Convert.ToDateTime(command.ExecuteScalar());

                return lastchangedPW;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();

            }

        }


        public static void SetChangedPWDateTime(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LastChangedPassword=@DATETIMECURRENT WHERE Email=@EMAIL"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@DATETIMECURRENT", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EMAIL", HttpUtility.HtmlEncode(email));

                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}