using _204304Z_SITConnect.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _204304Z_SITConnect.Forms
{
    public partial class AuditLog : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            var UserSession = Session["UserEmailLoggedIn"];
            var AuthTokenSession = Session["AuthToken"];
            var AuthTokenCookie = Request.Cookies["AuthToken"];

            if (UserSession != null && AuthTokenSession != null && AuthTokenCookie != null)
            {
                if (AuthTokenSession.ToString() == AuthTokenCookie.Value)
                {
                    if (!IsPostBack)
                    {
                        GetHistory();
                    }
                }
                else
                {
                    // Fake user
                    Helper.AccountLogout();
                }
            }
            else
            {
                Helper.ClearSessionsAndCookies();
                Response.Redirect("Login.aspx", false);
            }
        }

        private void GetHistory()

        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);

            SqlCommand sqlCMD = new SqlCommand("SELECT * from Log WHERE Email = @EMAIL", connection);
            sqlCMD.Parameters.AddWithValue("@EMAIL", Session["UserEmailLoggedIn"]);

            SqlDataAdapter sda = new SqlDataAdapter(sqlCMD);

            DataTable dt = new DataTable();

            sda.Fill(dt);

            dl_History.DataSource = dt;

            dl_History.DataBind();

        }
    }
}