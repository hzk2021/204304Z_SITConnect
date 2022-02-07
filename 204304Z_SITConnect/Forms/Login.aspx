<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="_204304Z_SITConnect.Forms.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
    <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js" integrity="sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q" crossorigin="anonymous"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js" integrity="sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl" crossorigin="anonymous"></script>

    <script>
        function validateEmail() {
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

    <script src="https://www.google.com/recaptcha/api.js?render=6LcvOtQdAAAAAHIr8lwRsboKoBm7vmNhYX9ZDJzm"></script>

    <style>
    div.g-recaptcha {
      margin: 0 auto;
      width: fit-content;
    }
    </style>
</head>

<body>
    <div class="container">
    <div class="row">
      <div class="col-md-6 offset-md-3">
        <h2 class="text-center text-dark mt-5">SITConnect Login</h2>
        <div class="card my-5">

          <form class="card-body cardbody-color p-lg-5" id="form1" runat="server">

            <div class="text-center">
              <img src="https://cdn.pixabay.com/photo/2016/03/31/19/56/avatar-1295397__340.png" class="img-fluid profile-image-pic img-thumbnail rounded-circle my-3"
                width="200px" alt="profile">
            </div>

            <div class="mb-3">
                <asp:TextBox ID="txt_Email" runat="server" CssClass="form-control" placeholder="Email" onkeyup="javascript:validateEmail()"></asp:TextBox>
            </div>

            <div class="mb-3">
                <asp:Label ID="lbl_EmailValidity" runat="server" Text="" CssClass="text-center"></asp:Label>
            </div>

            <div class="mb-3">
                <asp:TextBox ID="txt_Password" runat="server" CssClass="form-control" placeholder="Password" TextMode="Password"></asp:TextBox>
            </div>

            <div class="mb-3">
                <asp:Label ID="lbl_Message" runat="server" Text="" CssClass="text-center text-danger"></asp:Label>
            </div>

            <div class="mb-3">
                <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
            </div>
            <div class="mb-3">
                <asp:Button ID="btn_Login" runat="server" OnClick="btn_Login_Click" Text="Login" CssClass="bg-dark text-white text-center btn btn-color px-5 mb-5 w-100" />
            </div>


            <div id="accountHelp" class="form-text text-center mb-5 text-dark">Not Registered?
                <a href="/Forms/Registration.aspx" class="text-dark font-weight-bold">Create an Account</a>
            </div>
        
          </form>

        <script>
            grecaptcha.ready(function () {
                grecaptcha.execute('6LcvOtQdAAAAAHIr8lwRsboKoBm7vmNhYX9ZDJzm', { action: 'login' }).then(function (token) {
                    document.getElementById("g-recaptcha-response").value = token;
                });
            });
        </script>

        </div>

      </div>
    </div>
  </div>
</body>
</html>
