using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using ZdPo;

public partial class Components_Logon : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void cmdSubmit_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if (Admin_Proxy.Authenticate(txtUserName.Text.Trim(), txtPassword.Text.Trim()))
            {
                Session[Constants.CommonConst.AppName] = "yes";
                Response.Redirect(Request.Url.ToString());
            }
            else
            {
                lblMessage.Text = "Invalid Login, please try again!";
                Session[Constants.CommonConst.AppName] = "no";
            }
        }
    }
}