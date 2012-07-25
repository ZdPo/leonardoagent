using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using ZdPo;

public partial class Components_DepartureTable : System.Web.UI.UserControl
{
    private enum Colums
    {
        S1 = 0,
        IATAICAO,
        FLGTNUM,
        FLGSHARE,
        TIME_LT,
        TIME_LT_FULL,
        REPARK,
        HAL,
        CHIN,
        GATE,
        HDG,
        S2,
        AIRCRAFT
    };
    private bool SeparateLineIsShow = false;
    private int RowCount = 0;

    protected void Page_Init(object sender, EventArgs e)
    {
        Button fnd = Page.Master.FindControl("btnFind") as Button;
        if (fnd != null)
        {
            fnd.Click += new EventHandler(fnd_Click);
        }
        Button clr = Page.Master.FindControl("btnClear") as Button;
        if (clr != null)
            clr.Click += new EventHandler(clr_Click);
    }

    public void clr_Click(object sender, EventArgs e)
    {
        Components_HeaderCell hc;
        if (GridView1.HeaderRow != null)
        {
            foreach (TableCell tc in GridView1.HeaderRow.Cells)
            {
                for (int i = 0; i < tc.Controls.Count; i++)
                {
                    if (tc.Controls[i] is Components_HeaderCell)
                    {
                        hc = (Components_HeaderCell)tc.Controls[i];
                        hc.SearchTextValue = "";
                    }
                }
            }
        }
        ObjectDataSource os = GridView1.DataSourceObject as ObjectDataSource;
        if (os != null)
            os.FilterExpression = "";
        ClearAll();
    }

    public void fnd_Click(object sender, EventArgs e)
    {
        Components_HeaderCell hc;
        string sb = "";
        string sbt = "";
        int j = 0;
        if (GridView1.HeaderRow != null)
        {
            foreach (TableCell tc in GridView1.HeaderRow.Cells)
            {
                for (int i = 0; i < tc.Controls.Count; i++)
                {
                    if (tc.Controls[i] is Components_HeaderCell)
                    {
                        hc = (Components_HeaderCell)tc.Controls[i];
                        if (string.IsNullOrWhiteSpace(hc.SearchTextValue))
                            continue;
                        sb = Utils.CreateFilter(sb, GridView1.Columns[j].SortExpression, hc.SearchTextValue.ToUpper());
                        sbt = Utils.CreateFilterText(sbt, GridView1.Columns[j].HeaderText, hc.SearchTextValue.ToUpper());
                    }
                }
                j++;
            }
        }
        ObjectDataSource os = GridView1.DataSourceObject as ObjectDataSource;
        if (os != null)
        {
            os.FilterExpression = sb;
            Filtering(sbt);
        }
        if (string.IsNullOrWhiteSpace(sbt))
            ClearAll();
    }

    private void ClearAll()
    {
        Filtering("");
        ObjectDataSource os = GridView1.DataSourceObject as ObjectDataSource;
        if (os != null)
        {
            os.FilterExpression = "";
            GridView1.DataBind();
        }
    }

    private void Filtering(string co)
    {
        lbFilter.Text = "";
        lbFilter.Visible = false;
        if (!string.IsNullOrWhiteSpace(co))
        {
            lbFilter.Text = "Filtr: " + co;
            lbFilter.Visible = true;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GridView1.Columns[(int)Colums.TIME_LT].Visible = true;
            GridView1.Columns[(int)Colums.TIME_LT_FULL].Visible = false;
            GridView1.Sort(Colums.TIME_LT.ToString(), SortDirection.Ascending);
        }
        Components_HeaderCell hc;
        if (GridView1.HeaderRow != null)
        {
            foreach (TableCell tc in GridView1.HeaderRow.Cells)
            {
                for (int i = 0; i < tc.Controls.Count; i++)
                {
                    if (tc.Controls[i] is Components_HeaderCell)
                    {
                        hc = (Components_HeaderCell)tc.Controls[i];
                        hc.lbDown_Click = btnClick;
                        hc.lbUp_Click = btnClick;
                    }
                }
            }
        }
    }
    public void btnClick(object sender, CommandEventArgs e)
    {
        string dle = e.CommandArgument.ToString();
        SortDirection sd = SortDirection.Ascending;
        if (e.CommandName == "Down")
            sd = SortDirection.Descending;
        GridView1.Sort(dle, sd);
        GridView1.DataBind();
    }
    string currentDate = DateTime.Now.ToString("yyyyMMdd");

    /// <summary>
    /// Manipulate with every row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            RowCount = 2;
            e.Row.Cells[(int)Colums.FLGTNUM].ColumnSpan = 2;
            e.Row.Cells.RemoveAt((int)Colums.FLGSHARE);
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Data_Schema.DepartureShowRow row = (Data_Schema.DepartureShowRow)((System.Data.DataRowView)e.Row.DataItem).Row;
            if (row.FLIGHTID <= 0)
            {
                if ((GridView1.SortExpression == "") || (GridView1.SortExpression == Colums.TIME_LT.ToString()))
                {
                    while (e.Row.Cells.Count > 1) { e.Row.Cells.RemoveAt(e.Row.Cells.Count - 1); }
                    e.Row.Cells[0].ColumnSpan = GridView1.Columns.Count - 1;
                    e.Row.Cells[0].Text = string.Format(Constants.ShowData.SeparatorFormat, row.TIME_LT);
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.BackColor = Color.Black;
                    e.Row.ForeColor = Color.White;
                    e.Row.Visible = true;
                }
                else
                {
                    while (e.Row.Cells.Count > 0) { e.Row.Cells.RemoveAt(e.Row.Cells.Count - 1); }
                    e.Row.Visible = false;
                }
                return;
            }

            if ((GridView1.SortExpression == "") && (!SeparateLineIsShow)) // && (GridView1.SortDirection == SortDirection.Ascending))
            {

            }
            RowCount++;
            if (!row.IsFLGTSHARENull())
            {
                e.Row.Cells[(int)Colums.FLGTNUM].ToolTip = row.FLGTSHARE;
                e.Row.Cells[(int)Colums.FLGSHARE].ToolTip = row.FLGTSHARE;
                e.Row.Cells[(int)Colums.FLGSHARE].Text = string.Format("&nbsp;{0}&nbsp;", Constants.CommonConst.Block);
            }
            if (!row.IsS2CITNull())
            {
                e.Row.Cells[(int)Colums.S2].ToolTip = row.S2CIT;
            }
            if (!row.IsS1CITNull())
            {
                e.Row.Cells[(int)Colums.S1].ToolTip = row.S1CIT;
            }
            //            if (!row.IsIATAICAONull())
            if (!row.IsICAO_AIRNull())
                e.Row.Cells[(int)Colums.IATAICAO].ToolTip = row.ICAO_AIR;
            if (!row.IsHDGDETAILNull())
                e.Row.Cells[(int)Colums.HDG].ToolTip = row.HDGDETAIL;
            string testedDate = row.TIME_LT.ToString("yyyyMMdd");
            if (string.Compare(testedDate, currentDate) < 0)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                    e.Row.Cells[i].BackColor = Color.LightGray;
            }
            if (!row.IsCOLORNull())
                for (int i = 0; i < e.Row.Cells.Count; i++)
                    e.Row.Cells[i].ForeColor = Color.FromName(row.COLOR);
        }
    }
    /// <summary>
    /// Change how show data in TIME_LT
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
    {
        if ((e.SortExpression == "TIME_LT") && (e.SortDirection == SortDirection.Ascending))
        {
            GridView1.Columns[(int)Colums.TIME_LT].Visible = true;
            GridView1.Columns[(int)Colums.TIME_LT_FULL].Visible = false;
        }
        else
        {
            GridView1.Columns[(int)Colums.TIME_LT].Visible = false;
            GridView1.Columns[(int)Colums.TIME_LT_FULL].Visible = true;
        }
    }

    #region Parameters
    /// <summary>
    /// Data Source ID for working with this data
    /// </summary>
    public string DataSourceID
    {
        get { return GridView1.DataSourceID; }
        set { GridView1.DataSourceID = value; }
    }
    /// <summary>
    /// Data Source - second way
    /// </summary>
    public object DataSource
    {
        private get { return null; }
        set { GridView1.DataSource = value; }
    }
    /// <summary>
    /// Return current grid
    /// </summary>
    public GridView MyGridView
    {
        get { return GridView1; }
        private set { }
    }
    #endregion
}