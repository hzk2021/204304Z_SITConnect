using _204304Z_SITConnect.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _204304Z_SITConnect.Forms
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var UserSession = Session["UserEmailLoggedIn"];
            var AuthTokenSession = Session["AuthToken"];
            var AuthTokenCookie = Request.Cookies["AuthToken"];

            if (UserSession != null && AuthTokenSession != null && AuthTokenCookie != null)
            {
                if (AuthTokenSession.ToString() == AuthTokenCookie.Value)
                {
                    // Real user
                }
                else
                {
                    // Fake user
                    Helper.AccountLogout();
                }
            }
            else
            {
                Helper.AccountLogout();
            }
        }

        protected void btn_ChangePassword_Click(object sender, EventArgs e)
        {
            ChangePass();
        }

        public void ChangePass()
        {
            string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT * FROM Account WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", Session["UserEmailLoggedIn"]);

            var passHash = "";
            var passSalt = "";
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
                                passHash = reader["PasswordHash"].ToString();
                            }
                        }

                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                passSalt = reader["PasswordSalt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { 
                connection.Close();

                var oldPass = HttpUtility.HtmlEncode(txt_OldPassword.Text);
                var newPass = HttpUtility.HtmlEncode(txt_NewPassword.Text);

                if (string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass))
                {
                    lbl_Message.ForeColor = Color.Red;
                    lbl_Message.Text = "Do not leave blank(s)";
                }
                else if (!Validator.CustomRegex.IsValidPassword(newPass))
                {
                    lbl_Message.ForeColor = Color.Red;
                    lbl_Message.Text = "New password does not meet the requirement!";
                }
                else if (Crypto.GetHashedString(oldPass, passSalt) == passHash)
                {
                    TimeSpan timeElapsed = DateTime.Now - PasswordRecord.GetLastChangedPassDateTime(Session["UserEmailLoggedIn"].ToString());

                    if (timeElapsed.TotalMinutes > 10) // currently set to 10 minutes before you can change password again
                    {
                        string newSalt = Helpers.Crypto.GetRandomSalt();
                        string newHashedPW = Helpers.Crypto.GetHashedString(newPass, newSalt);

                        bool isReuse = false;

                        foreach (var salt in PasswordRecord.GetAllSalt(Session["UserEmailLoggedIn"].ToString()))
                        {
                            string tempPWHashed = Helpers.Crypto.GetHashedString(newPass, salt);

                            if (PasswordRecord.GetAllHashedPW(Session["UserEmailLoggedIn"].ToString()).Contains(tempPWHashed))
                            {
                                isReuse = true;
                                break;
                            }
                        }

                        if (!isReuse) // Prevent reusing old passwords
                        {

                            UpdatePassword(newHashedPW, newSalt);
                            PasswordRecord.StorePassword(Session["UserEmailLoggedIn"].ToString(), newHashedPW, newSalt);
                            PasswordRecord.SetChangedPWDateTime(Session["UserEmailLoggedIn"].ToString());

                            lbl_Message.ForeColor = Color.Green;
                            lbl_Message.Text = "Password has been updated!";
                        }
                        else
                        {
                            lbl_Message.ForeColor = Color.Red;
                            lbl_Message.Text = "Please avoid reusing old passwords!";
                        }
                    }
                    else
                    {
                        lbl_Message.ForeColor = Color.Red;
                        lbl_Message.Text = "You are changing passwords way too fast. Please wait at least 10 minutes!";
                    }
                }
                else
                {
                    lbl_Message.ForeColor = Color.Red;
                    lbl_Message.Text = "Old password is incorrect!";
                }
            }
        }

        public void UpdatePassword(string pass_hash, string pass_salt)
        {
            string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

            string sql = "UPDATE Account SET PasswordHash = @PASSWORDHASH, PasswordSalt = @PASSWORDSALT WHERE Email=@EMAIL;";

            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PASSWORDHASH", pass_hash);
                    cmd.Parameters.AddWithValue("@PASSWORDSALT", pass_salt);
                    cmd.Parameters.AddWithValue("@EMAIL", Session["UserEmailLoggedIn"]);


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
    }
}