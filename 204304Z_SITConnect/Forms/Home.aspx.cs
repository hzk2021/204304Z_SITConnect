using _204304Z_SITConnect.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _204304Z_SITConnect.Forms
{
    public partial class Home : System.Web.UI.Page
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

                    if (PasswordRecord.GetLastChangedPassDateTime(Session["UserEmailLoggedIn"].ToString()) != new DateTime(2021, 1, 1))
                    {
                        TimeSpan timeElapsed = DateTime.Now - PasswordRecord.GetLastChangedPassDateTime(Session["UserEmailLoggedIn"].ToString());
                        if (timeElapsed.TotalMinutes > 30) // Force Password change after x time
                        {
                            Response.Write("<script>alert('Your password is more than 30 minutes old. Please change it!!')</script>");
                        }
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
                Helper.AccountLogout();
            }
        }
    }
}