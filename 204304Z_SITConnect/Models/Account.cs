using _204304Z_SITConnect.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace _204304Z_SITConnect.Models
{
    public class Account
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        string uEmail;
        string uPassword;
        string uFName;
        string uLName;
        Nullable<Int64> uCCNumber;
        string uCCExpiryMonth;
        string uCCExpiryYear;
        string uCCName;
        Nullable<int> uCC_CVC;
        Nullable<DateTime> uDOB;
        byte[] imgBytes;

        public Account(
            string email,
            string password,
            string fname = null,
            string lname = null,
            string ccNumber = "0",
            string ccExpiryMonth = null,
            string ccExpiryYear = null,
            string ccName = null,
            string ccCVC = "0",
            string DateOfBirth = "01/01/2003",
            byte[] imageBytes = null)
        {
            uEmail = HttpUtility.HtmlEncode(email);
            uPassword = HttpUtility.HtmlEncode(password);
            uFName = HttpUtility.HtmlEncode(fname);
            uLName = HttpUtility.HtmlEncode(lname);
            uCCNumber = Int64.Parse(HttpUtility.HtmlEncode(ccNumber));
            uCCExpiryMonth = HttpUtility.HtmlEncode(ccExpiryMonth);
            uCCExpiryYear = HttpUtility.HtmlEncode(ccExpiryYear);
            uCCName = HttpUtility.HtmlEncode(ccName);
            uCC_CVC = int.Parse(HttpUtility.HtmlEncode(ccCVC));
            uDOB = DateTime.Parse(HttpUtility.HtmlEncode(DateOfBirth));

            if (imageBytes != null)
            {
                imgBytes = Encoding.ASCII.GetBytes(HttpUtility.HtmlEncode(imageBytes));
            }
        }

        public Tuple<bool, string> Register()
        {
            List<string> ErrorMessages = new List<string>();

            if (EmailIsInDataBase())
                return Tuple.Create(false, "Email is already being used!");
            if (!Validator.CustomRegex.IsValidEmail(uEmail))
                return Tuple.Create(false, "Email does not meet the requirement!");
            if (!Validator.CustomRegex.IsValidPassword(uPassword))
                return Tuple.Create(false, "Password does not meet the requirement!");

            string salt = Helpers.Crypto.GetRandomSalt();
            string hashedPW = Helpers.Crypto.GetHashedString(uPassword, salt);

            byte[] IV = Helpers.Crypto.GetRandomIVAndKey().Item1;
            byte[] Key = Helpers.Crypto.GetRandomIVAndKey().Item2;

            string encryptCCNumber = Helpers.Crypto.GetEncryptedText(uCCNumber.ToString(), IV, Key);
            string encryptedCCExpiryDate = Helpers.Crypto.GetEncryptedText($"{uCCExpiryMonth},{uCCExpiryYear}", IV, Key);
            string encryptCCName = Helpers.Crypto.GetEncryptedText(uCCName, IV, Key);
            string encryptCC_CVC = Helpers.Crypto.GetEncryptedText(uCC_CVC.ToString(), IV, Key);

            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@FirstName, @LastName, @CCNumber, @CCExpiryDate, @CCName, @CC_CVC, @Email, @PasswordHash, @PasswordSalt, @DateOfBirth, @IV, @Key, @AccountLocked, @ImgBytes, @FailedLoginAttempts, @LockOutDateTime, @Verified, @LastChangedPassword)"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@FirstName", uFName);
                    cmd.Parameters.AddWithValue("@LastName", uLName);
                    cmd.Parameters.AddWithValue("@CCNumber", encryptCCNumber);
                    cmd.Parameters.AddWithValue("@CCExpiryDate", encryptedCCExpiryDate);
                    cmd.Parameters.AddWithValue("@CCName", encryptCCName);
                    cmd.Parameters.AddWithValue("@CC_CVC", encryptCC_CVC);
                    cmd.Parameters.AddWithValue("@Email", uEmail);
                    cmd.Parameters.AddWithValue("@PasswordHash", hashedPW);
                    cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                    cmd.Parameters.AddWithValue("@DateOfBirth", uDOB);
                    cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                    cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                    cmd.Parameters.AddWithValue("@AccountLocked", 0);
                    cmd.Parameters.AddWithValue("@ImgBytes", imgBytes);
                    cmd.Parameters.AddWithValue("@FailedLoginAttempts", 0);
                    cmd.Parameters.AddWithValue("@LockOutDateTime", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Verified", 0);
                    cmd.Parameters.AddWithValue("@LastChangedPassword", new DateTime(2021, 1, 1));

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

            PasswordRecord.StorePassword(uEmail, hashedPW, salt);

            Helper.SendVerificationEmail(uEmail);
            return Tuple.Create(true, "Account Created Successfully! A verification link has been sent to your email.");
        }

        public Tuple<bool, string> Login()
        {
            if (!Validator.CustomRegex.IsValidEmail(uEmail))
                return Tuple.Create(false, "Email does not meet the requirement!");
            if (!Validator.CustomRegex.IsValidPassword(uPassword))
                return Tuple.Create(false, "Password does not meet the requirement!");
            if (!EmailIsInDataBase() || !IsVerified())
                return Tuple.Create(false, "Invalid Credential(s) or Email is not verified!");
            if (IsLocked())
                return Tuple.Create(false, "Account is locked due to multiple failed login attempts!");

            string hash = RetrieveHashFromDB();
            string salt = RetrieveSaltFromDB();

            if (Helpers.Crypto.GetHashedString(uPassword, salt) == hash)
                return Tuple.Create(true, "Account Login Successfully!");
            else
                return Tuple.Create(false, "Invalid Credential(s) or Email is not verified!");

        }

        #region Database Related Methods
        public bool IsLocked()
        {
            bool isLocked = false;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT AccountLocked FROM Account WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", uEmail);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["AccountLocked"] != null)
                        {
                            if (reader["AccountLocked"] != DBNull.Value)
                            {
                                isLocked = (bool)reader["AccountLocked"];
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

            return isLocked;
        }

        public bool IsVerified()
        {
            bool isVerified = false;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT Verified FROM Account WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", uEmail);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Verified"] != null)
                        {
                            if (reader["Verified"] != DBNull.Value)
                            {
                                isVerified = (bool)reader["Verified"];
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

            return isVerified;
        }

        public bool EmailIsInDataBase()
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT count(*) from ACCOUNT WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", uEmail);
            try
            {
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
        }


        private string RetrieveHashFromDB()
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT PasswordHash FROM Account WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", uEmail);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
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
            return h;

        }

        private string RetrieveSaltFromDB()
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT PasswordSalt FROM ACCOUNT WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", uEmail);
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
                                s = reader["PasswordSalt"].ToString();
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
            return s;
        }

        public void LockAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET AccountLocked=@LOCKED, LockOutDateTime=@DATETIMECURRENT WHERE Email=@EMAIL"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@LOCKED", true);
                        cmd.Parameters.AddWithValue("@DATETIMECURRENT", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EMAIL", uEmail);

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

        public void UnlockAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET AccountLocked=@LOCKED, LockOutDateTime=@DATETIMECURRENT, FailedLoginAttempts=@RESETCOUNT WHERE Email=@EMAIL"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@LOCKED", false);
                        cmd.Parameters.AddWithValue("@DATETIMECURRENT", DBNull.Value);
                        cmd.Parameters.AddWithValue("@RESETCOUNT", 0);
                        cmd.Parameters.AddWithValue("@EMAIL", uEmail);

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

        public void AddFailedCount(string email)
        {
            int failedAttempts = GetFailedAttempts(email);
            int newFailAttempts = failedAttempts += 1;

            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET FailedLoginAttempts = @FAILATTEMPTSPLUS WHERE Email=@EMAIL;"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@FAILATTEMPTSPLUS", newFailAttempts);
                        cmd.Parameters.AddWithValue("@EMAIL", email);

                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void ClearFailedCount(string email)
        {
            int clearFailedAttempts = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET FailedLoginAttempts = @FAILATTEMPTSPLUS WHERE Email=@EMAIL;"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@FAILATTEMPTSPLUS", clearFailedAttempts);
                        cmd.Parameters.AddWithValue("@EMAIL", email);

                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        public int GetFailedAttempts(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT FailedLoginAttempts from Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);

            try
            {
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());

                return count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();

            }
            return 0;

        }

        public DateTime GetLockOutDateTime()
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT LockOutDateTime from Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", uEmail);

            try
            {
                connection.Open();
                DateTime lockOutDT = Convert.ToDateTime(command.ExecuteScalar());

                return lockOutDT;
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

        #endregion

    }
}