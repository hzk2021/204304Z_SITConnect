<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="_204304Z_SITConnect.Forms.ChangePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
    <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js" integrity="sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q" crossorigin="anonymous"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js" integrity="sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl" crossorigin="anonymous"></script>
    
    <script>
        function validatecPassword() {
            var password = document.getElementById('<%=txt_NewPassword.ClientID%>').value;

            if (password.length < 12) {
                document.getElementById("lbl_cPasswordStrength").innerText = "Password Needs To Have At Least 12 Characters";
                document.getElementById("lbl_cPasswordStrength").style.color = "Red";
                return "short_password";
            }
            else if (password.search(/[0-9]/) == -1) {
                document.getElementById("lbl_cPasswordStrength").innerText = "Password Needs To Have At Least 1 Number";
                document.getElementById("lbl_cPasswordStrength").style.color = "Red";
                return "missing_number";
            }
            else if (password.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_cPasswordStrength").innerText = "Password Needs To Have At Least 1 Upper Case Character";
                document.getElementById("lbl_cPasswordStrength").style.color = "Red";
                return "missing_uppercase";
            }
            else if (password.search(/[a-z]/) == -1) {
                document.getElementById("lbl_cPasswordStrength").innerText = "Password Needs To Have At Least 1 Lower Case Character";
                document.getElementById("lbl_cPasswordStrength").style.color = "Red";
                return "missing_lowercase";
            }
            else if (password.search(/[^A-Za-z0-9]/) == -1) {
                document.getElementById("lbl_cPasswordStrength").innerText = "Password Needs To Have At Least 1 Special Character";
                document.getElementById("lbl_cPasswordStrength").style.color = "Red";
                return "missing_special_char";
            }

            document.getElementById("lbl_cPasswordStrength").innerText = "Strong Password!";
            document.getElementById("lbl_cPasswordStrength").style.color = "Green";
        }
    </script>

        <div class="container">
        <div class="row">
          <div class="col-md-6 offset-md-3">
            <h2 class="text-center text-dark mt-5">Change Password</h2>
            <div class="card my-5">

                <div class="mb-3">
                    <asp:TextBox ID="txt_OldPassword" runat="server" CssClass="form-control" placeholder="Old Password"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <asp:TextBox ID="txt_NewPassword" runat="server" CssClass="form-control" placeholder="New Password" onkeyup="javascript:validatecPassword()" TextMode="Password"></asp:TextBox>
                    <asp:Label ID="lbl_cPasswordStrength" runat="server"></asp:Label>
                    <br />
                    <asp:Label ID="lbl_Message" runat="server"></asp:Label>
                </div>

                <div class="mb-3">
                    <asp:Button ID="btn_ChangePassword" runat="server" Text="Change Password" CssClass="bg-dark text-white text-center btn btn-color px-5 mb-5 w-100" OnClick="btn_ChangePassword_Click" />
                </div>
            </div>

          </div>
        </div>
  </div>

</asp:Content>
