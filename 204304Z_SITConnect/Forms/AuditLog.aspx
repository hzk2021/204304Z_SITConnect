<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AuditLog.aspx.cs" Inherits="_204304Z_SITConnect.Forms.AuditLog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .customStyle {
            border: 2px solid black;
            margin: 10px;
        }

    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <div class="row">
          <div class="col-md-6 offset-md-3">
            <h2 class="text-center text-dark mt-5">Audit Log (Login History)</h2>
            <div class="card my-5">


            </div>

          </div>
        </div>

        <div class="row">
            <asp:DataList ID="dl_History" runat="server" RepeatDirection="Horizontal" RepeatColumns="4">

                <ItemTemplate>

                    <div class="col customStyle">

                                <p><strong>Email:</strong>:<%# Eval("Email") %></p>

                                <p>
                                    <strong>Login Successful </strong>:<%# Eval("attemptResult") %>
                                </p>

                                <p><strong>Date & Time</strong>:<%# Eval("loginDateTime") %></p>

                        </div>

                    </div>

                </ItemTemplate>

            </asp:DataList>
          </div>

    </div>
</asp:Content>
