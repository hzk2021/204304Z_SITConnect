<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="_204304Z_SITConnect.Forms.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            height: 480px;
        }
    </style>

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
    <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js" integrity="sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q" crossorigin="anonymous"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js" integrity="sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl" crossorigin="anonymous"></script>

    <script>
        function validatePassword() {
            var password = document.getElementById('<%=txt_Password.ClientID%>').value;

            if (password.length < 12) {
                document.getElementById("lbl_PasswordStrength").innerText = "Password Needs To Have At Least 12 Characters";
                document.getElementById("lbl_PasswordStrength").style.color = "Red";
                return "short_password";
            }
            else if (password.search(/[0-9]/) == -1) {
                document.getElementById("lbl_PasswordStrength").innerText = "Password Needs To Have At Least 1 Number";
                document.getElementById("lbl_PasswordStrength").style.color = "Red";
                return "missing_number";
            }
            else if (password.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_PasswordStrength").innerText = "Password Needs To Have At Least 1 Upper Case Character";
                document.getElementById("lbl_PasswordStrength").style.color = "Red";
                return "missing_uppercase";
            }
            else if (password.search(/[a-z]/) == -1) {
                document.getElementById("lbl_PasswordStrength").innerText = "Password Needs To Have At Least 1 Lower Case Character";
                document.getElementById("lbl_PasswordStrength").style.color = "Red";
                return "missing_lowercase";
            }
            else if (password.search(/[^A-Za-z0-9]/) == -1) {
                document.getElementById("lbl_PasswordStrength").innerText = "Password Needs To Have At Least 1 Special Character";
                document.getElementById("lbl_PasswordStrength").style.color = "Red";
                return "missing_special_char";
            }

            document.getElementById("lbl_PasswordStrength").innerText = "Strong Password!";
            document.getElementById("lbl_PasswordStrength").style.color = "Green";
        }

        function validateEmail()
        {
            var email = document.getElementById('<%=txt_Email.ClientID%>').value;

            var emailRegex = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/;
            var isValid = emailRegex.test(email);

            if (isValid) {
                document.getElementById("lbl_EmailValidity").innerText = "Valid Email Address!!";
                document.getElementById("lbl_EmailValidity").style.color = "Green";
            }
            else {
                document.getElementById("lbl_EmailValidity").innerText = "Invalid Email Address!";
                document.getElementById("lbl_EmailValidity").style.color = "Red";
            }

        }
    </script>

</head>
<body>

    <div class="container">
        <div class="row">
          <div class="col-md-6 offset-md-3">
            <h2 class="text-center text-dark mt-5">SITConnect Register</h2>
            <div class="card my-5">

              <form class="card-body cardbody-color p-lg-5" id="form2" runat="server">
                <div class="text-center">
                  <img src="https://cdn.pixabay.com/photo/2016/03/31/19/56/avatar-1295397__340.png" class="img-fluid profile-image-pic img-thumbnail rounded-circle my-3"
                    width="200px" alt="profile">
                </div>

                <div class="mb-3">
                    <asp:TextBox ID="txt_FirstName" runat="server" CssClass="form-control" placeholder="First Name"></asp:TextBox>
                </div>
                <div class="mb-3">
                    <asp:TextBox ID="txt_LastName" runat="server" CssClass="form-control" placeholder="Last Name"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <asp:TextBox ID="txt_CCNumber" runat="server" CssClass="form-control" placeholder="Credit Card Number" onkeydown="return (!(event.keyCode>=65) && event.keyCode!=32);"></asp:TextBox>

                </div>

                <div class="mb-3">
                    <asp:Label ID="lbl_CCExpMonth" runat="server" CssClass="text-primary font-weight-bold" text="Expiration Date: " ></asp:Label>
                    <asp:DropDownList ID="ddl_CCExpMonth" runat="server">
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddl_CCExpYear" runat="server">
                    </asp:DropDownList>
                </div>

                <div class="mb-3">
                    <asp:TextBox ID="txt_CCName" runat="server" CssClass="form-control" placeholder="Name on Credit Card"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <asp:TextBox ID="txt_CC_CVC" runat="server" CssClass="form-control" placeholder="Credit Card CVC" onkeydown="return (!(event.keyCode>=65) && event.keyCode!=32);"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <asp:TextBox ID="txt_Email" runat="server" CssClass="form-control" placeholder="Email Address" onkeyup="javascript:validateEmail()"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <asp:Label ID="lbl_EmailValidity" runat="server" Text="" CssClass="text-center"></asp:Label>
                </div>

                <div class="mb-3">
                    <asp:TextBox ID="txt_Password" runat="server" CssClass="form-control" placeholder="Password" onkeyup="javascript:validatePassword()" TextMode="Password"></asp:TextBox>
                    <asp:Label ID="lbl_PasswordStrength" runat="server"></asp:Label>
                </div>

                <div class="mb-3">
                    <asp:TextBox ID="txt_DateOfBirth" runat="server" CssClass="form-control" placeholder="Date Of Birth" TextMode="Date"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <asp:Label ID="lbl_UploadPhoto" runat="server" CssClass="text-info font-weight-bold" text="Upload Photo: " ></asp:Label>
                    <asp:FileUpload ID="fup_UploadPhoto" runat="server" />
                </div>

                <div class="mb-3">
                    <asp:Label ID="lbl_Message" runat="server" Text="" CssClass="text-center"></asp:Label>
                </div>

                <div class="mb-3">
                    <asp:Button ID="btn_Register" runat="server" OnClick="btn_Register_Click" Text="Register" CssClass="bg-dark text-white text-center btn btn-color px-5 mb-5 w-100" />
                </div>

                <div id="accountHelp" class="form-text text-center mb-5 text-dark">Already have an Account?
                    <a href="/Forms/Login.aspx" class="text-dark font-weight-bold">Login</a>
                </div>

              </form>
            </div>

          </div>
        </div>
  </div>
</body>
</html>
