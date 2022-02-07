using _204304Z_SITConnect.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _204304Z_SITConnect.Forms
{
    public partial class VerifyEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string verifyEmailAddr = HttpUtility.HtmlEncode(Request.QueryString["email"]).ToString();
            Account verifyAccount = new Account(verifyEmailAddr, "");

            if (verifyAccount.EmailIsInDataBase() && Helpers.Helper.VerifyEmail(verifyEmailAddr))
            {
                lbl_veMessage.ForeColor = Color.Green;
                lbl_veMessage.Text = $"Your email: {verifyEmailAddr} has been verified!";
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }
    }
}