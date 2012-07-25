using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

    /// <summary>
    /// One header
    /// </summary>
public partial class Components_HeaderCell : System.Web.UI.UserControl
{
    private int _size = 5;
    private string _name = "";
    private string _sort = "";
    private bool IsOrderUp = false;
    private bool isOrderDown = false;
    private CommandEventHandler ehlbUp = null;
    private CommandEventHandler ehlbDown = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lbDown.CommandArgument = _sort;
            lbUP.CommandArgument = _sort;
        }
    }

    protected void lbUP_Command(object sender, CommandEventArgs e)
    {
        if (ehlbUp != null)
            ehlbUp(sender, e);
    }
    protected void lbDown_Command(object sender, CommandEventArgs e)
    {
        if (ehlbDown != null)
            ehlbUp(sender, e);
    }


    public CommandEventHandler lbUp_Click
    {
        set { ehlbUp = value; }
    }
    public CommandEventHandler lbDown_Click
    {
        set { ehlbDown = value; }
    }

    #region Properties
    /// <summary>
    /// Size of search box
    /// </summary>
    public int SearchBoxSize
    {
        get { return Convert.ToInt32(txbSearch.Width.Value); }
        set
        {
            if (value > 0)
            {
                _size = value;
                txbSearch.Width = Unit.Pixel(_size);
            }

        }
    }
    /// <summary>
    /// Name for column
    /// </summary>
    public string HeaderText
    {
        get { return lbName.Text; }
        set
        {
            _name = value;
            lbName.Text = value;
        }
    }
    public string SortExpression
    {
        get { return _sort; }
        set
        {
            _sort = value;
            lbDown.CommandArgument = value;
            lbUP.CommandArgument = value;
        }
    }
    public bool OrderUp
    {
        get { return IsOrderUp; }
        set
        {
            IsOrderUp = value;
            /// TODO: need solutions for highlight
        }
    }
    public bool OrderDown
    {
        get { return isOrderDown; }
        set
        {
            isOrderDown = value;
            /// TODO: need solutions for highlight
        }
    }
    public string SearchTextValue
    {
        get { return txbSearch.Text; }
        set { txbSearch.Text = value; }
    }
    #endregion
}