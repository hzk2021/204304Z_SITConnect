using _204304Z_SITConnect.Algorithms;
using _204304Z_SITConnect.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _204304Z_SITConnect.Forms
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_Login_Click(object sender, EventArgs e)
        {
            if (ValidateGoogleCaptcha() == true)
            {
                Account existingAccount = new Account(txt_Email.Text, txt_Password.Text);

                if (existingAccount.EmailIsInDataBase()) //  Automatic account recovery.
                {
                    if (existingAccount.IsLocked())
                    {
                        TimeSpan timeElapsed = DateTime.Now - existingAccount.GetLockOutDateTime();
                        if (timeElapsed.TotalMinutes > 30) // Duration = 30 minutes rn
                        {
                            existingAccount.UnlockAccount();
                        }
                    }
                }

                var loginResult = existingAccount.Login();

                if (loginResult.Item1 == true)
                {
                    lbl_Message.ForeColor = Color.Green;
                    lbl_Message.Text = loginResult.Item2;

                    Session["UserEmailLoggedIn"] = txt_Email.Text;
                    string randomGUID = Guid.NewGuid().ToString();
                    Session["AuthToken"] = randomGUID;
                    Response.Cookies.Add(new HttpCookie("AuthToken", randomGUID));

                    existingAccount.ClearFailedCount(txt_Email.Text);
                    existingAccount.UnlockAccount();
                    bool addSuccessAttempt = Log.AddSuccessfulLoginAttempts(txt_Email.Text);

                    Response.Redirect("Home.aspx", false);
                }
                else
                {
                    lbl_Message.ForeColor = Color.Red;
                    lbl_Message.Text = loginResult.Item2;

                    if (existingAccount.EmailIsInDataBase())
                    {
                        existingAccount.AddFailedCount(txt_Email.Text);
                        bool addFailedAttempt = Log.AddFailedLoginAttempt(txt_Email.Text);

                        if (existingAccount.GetFailedAttempts(txt_Email.Text) >= 3)
                        {
                            existingAccount.LockAccount();
                        }
                    }
                }
            }
            else
            {
                lbl_Message.ForeColor = Color.Red;
                lbl_Message.Text = "You failed the reCAPTCHA v3";
            }
        }

        public bool ValidateGoogleCaptcha()
        {
            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LcvOtQdAAAAAGsF_-LidIuEVrEC56KuAzYDbDGS &response=" + captchaResponse);

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        // The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        //Create jsonObject to handle the response e.g. success or error
                        //Deserialize Json
                        GObject jsonObject = js.Deserialize<GObject>(jsonResponse);

                        // Convert the string "False" to bool false or "True" to bool true
                        if (jsonObject.success != "True" || jsonObject.score <= 0.5m)
                        {
                            return false;
                        }

                        return true;
                    }
                }

            }
            catch (WebException ex)
            {
                return false;
            }

        }
    }
}