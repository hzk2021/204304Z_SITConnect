using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace _204304Z_SITConnect.Helpers
{
    public class Helper
    {
        public static void AddMonthsToDDL(DropDownList ddl)
        {
            ddl.Items.Clear();
            string[] monthNames = getMonths();
            foreach (var name in monthNames)
            {
                ddl.Items.Add(name);
            }
            ddl.Items.FindByValue(monthNames[0]).Selected = true;
        }

        public static void AddYearsToDDL(DropDownList ddl)
        {
            ddl.Items.Clear();
            foreach (var year in getYears())
            {
                ddl.Items.Add(year);
            }
            ddl.Items.FindByValue(DateTime.Now.Year.ToString()).Selected = true;
        }

        private static string[] getMonths()
        {
            string[] monthNames = new string[12] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            return monthNames;
        }
        private static List<string> getYears()
        {
            List<string> yearNumbers = new List<string>();

            for (int i = 2021; i <= 2041; i++)
            {
                yearNumbers.Add(i.ToString());
            }

            return yearNumbers;
        }

        public static void ClearSessionsAndCookies()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Abandon();

            if (HttpContext.Current.Request.Cookies["ASP.NET_SessionId"] != null)
            {
                HttpContext.Current.Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                HttpContext.Current.Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-50);
            }

            if (HttpContext.Current.Request.Cookies["AuthToken"] != null)
            {
                HttpContext.Current.Response.Cookies["AuthToken"].Value = string.Empty;
                HttpContext.Current.Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-50);
            }
        }
        public static void AccountLogout()
        {
            Helper.ClearSessionsAndCookies();
            HttpContext.Current.Response.Redirect("Login.aspx", false);
        }

        public static void SendVerificationEmail(string to_email)
        {
            string fromEmail = "SITConnect <CarlsJrY2Y1@gmail.com>";

            MailMessage msg = new MailMessage();

            msg.From = new MailAddress(fromEmail);
            msg.To.Add(to_email);
            msg.Subject = "SITConnect Email Verification";
            msg.Body = "Thank you for the registration. Click here to verify your email: https://localhost:44300/Forms/VerifyEmail.aspx?email=" + to_email;
            //msg.Priority = MailPriority.High;


            using (SmtpClient client = new SmtpClient())
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("CarlsJrY2S1@gmail.com", "C@rlPass"); // Previous project Google SMTP credentials
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                client.Send(msg);
            }
        }

        public static bool VerifyEmail(string email)
        {
            string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET Verified=@VERIFY WHERE Email=@EMAIL"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@VERIFY", true);
                        cmd.Parameters.AddWithValue("@EMAIL", HttpUtility.HtmlEncode(email));

                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }

    }
}