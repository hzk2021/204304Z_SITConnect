using _204304Z_SITConnect.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _204304Z_SITConnect
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_LogOut_Click(object sender, EventArgs e)
        {
            Helper.AccountLogout();
        }
    }
}