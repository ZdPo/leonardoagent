using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZdPo;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Utils.ShowImage(true, (System.Web.UI.WebControls.Image)this.Master.FindControl("ImageAir"));
    }
    protected void Page_Init(object sender, EventArgs e)
    {
        this.Title = string.Format(Constants.CommonConst.TitleFormat, Constants.CommonConst.AppName, Constants.CommonConst.PageNameArrive60);
    }
}
