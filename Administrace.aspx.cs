using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZdPo;
using System.Web.Security;

public partial class Administrace : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Utils.ShowImage(true, (System.Web.UI.WebControls.Image)this.Master.FindControl("ImageAir"));
    }
    protected void Page_Init(object sender, EventArgs e)
    {
        this.Title = string.Format(Constants.CommonConst.TitleFormat, Constants.CommonConst.AppName, Constants.CommonConst.PageNameAdmin);
        Panel1.Enabled = true;
        Panel1.Visible = true;
        Panel2.Enabled = false;
        Panel2.Visible = false;
        if (!string.IsNullOrWhiteSpace(Session[Constants.CommonConst.AppName] as string))
        {
            if (Session[Constants.CommonConst.AppName].ToString() == "yes")
            {
                Panel1.Enabled = false;
                Panel1.Visible = false;
                Panel2.Enabled = true;
                Panel2.Visible = true;
            }
        }
    }
    protected void logout_Click(object sender, EventArgs e)
    {
        Session[Constants.CommonConst.AppName] = "no";
        Response.Redirect(Request.Url.ToString());
    }
}