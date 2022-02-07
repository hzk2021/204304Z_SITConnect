using _204304Z_SITConnect.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _204304Z_SITConnect.Forms
{
    public partial class Registration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Helpers.Helper.AddMonthsToDDL(ddl_CCExpMonth);
            Helpers.Helper.AddYearsToDDL(ddl_CCExpYear);
        }

        protected void btn_Register_Click(object sender, EventArgs e)
        {
            if (ValidateInputFields())
            {
                if (ValidateImage())
                {
                    using (var t = new MemoryStream())
                    {
                        fup_UploadPhoto.FileContent.CopyTo(t);

                        Account newAccount = new Account(
                                        txt_Email.Text,
                                        txt_Password.Text,
                                        txt_FirstName.Text,
                                        txt_LastName.Text,
                                        txt_CCNumber.Text,
                                        ddl_CCExpMonth.Text,
                                        ddl_CCExpYear.Text,
                                        txt_CCName.Text,
                                        txt_CC_CVC.Text,
                                        txt_DateOfBirth.Text,
                                        t.ToArray());

                        var registerResult = newAccount.Register();

                        if (registerResult.Item1 == true)
                        {
                            lbl_Message.ForeColor = Color.Green;
                            lbl_Message.Text = registerResult.Item2;

                            //Response.Redirect("Login.aspx", false);
                        }
                        else
                        {
                            lbl_Message.ForeColor = Color.Red;
                            lbl_Message.Text = registerResult.Item2;
                        }
                    }
                }
            }
        }

        private bool ValidateImage()
        {
            if (fup_UploadPhoto.HasFile)
            {
                string extension = System.IO.Path.GetExtension(fup_UploadPhoto.FileName);

                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    return true;
                }
                else
                {
                    lbl_Message.ForeColor = Color.Red;
                    lbl_Message.Text = "Images need to be in (.jpg | .jpeg | .png) format";
                }
            }
            else
            {
                lbl_Message.ForeColor = Color.Red;
                lbl_Message.Text = "Please upload an image file!";
            }

            return false;
        }

        private bool ValidateInputFields()
        {
            bool emptyFieldError = false;

            if (string.IsNullOrEmpty(txt_Email.Text))
            {
                emptyFieldError = true;
            }
            else if (string.IsNullOrEmpty(txt_Password.Text))
            {
                emptyFieldError = true;
            }
            else if(string.IsNullOrEmpty(txt_FirstName.Text))
            {
                emptyFieldError = true;
            }
            else if(string.IsNullOrEmpty(txt_LastName.Text))
            {
                emptyFieldError = true;
            }
            else if(string.IsNullOrEmpty(txt_CCNumber.Text))
            {
                emptyFieldError = true;
            }
            else if(string.IsNullOrEmpty(ddl_CCExpMonth.Text))
            {
                emptyFieldError = true;
            }
            else if(string.IsNullOrEmpty(ddl_CCExpYear.Text))
            {
                emptyFieldError = true;
            }
            else if (string.IsNullOrEmpty(txt_CCName.Text))
            {
                emptyFieldError = true;
            }
            else if(string.IsNullOrEmpty(txt_CC_CVC.Text))
            {
                emptyFieldError = true;
            }
            else if(string.IsNullOrEmpty(txt_DateOfBirth.Text))
            {
                emptyFieldError = true;
            }

            if (emptyFieldError)
            {
                lbl_Message.ForeColor = Color.Red;
                lbl_Message.Text = "Do not leave empty input fields!";

                return false;
            }
            else
            {
                bool isCCNoANumber = Int64.TryParse(txt_CCNumber.Text, out Int64 ccNo);
                bool isCCCVCANumber = Int64.TryParse(txt_CC_CVC.Text, out Int64 ccCVC);

                bool isValidDate = DateTime.TryParse(txt_DateOfBirth.Text, out DateTime dobDate);

                if (!isCCNoANumber || !isCCCVCANumber)
                {
                    lbl_Message.ForeColor = Color.Red;
                    lbl_Message.Text = "Invalid CC field(s)! Make sure it's a valid card number";

                    return false;
                }

                if (!isValidDate)
                {
                    lbl_Message.ForeColor = Color.Red;
                    lbl_Message.Text = "Invalid Date of birth field!";

                    return false;
                }

            }

            return true;
        }
    }
}