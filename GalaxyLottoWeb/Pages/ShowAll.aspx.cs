using GalaxyLotto.ClassLibrary;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class ShowAll : BasePage
    {
        private string _action;
        private string _requestId;
        private StuGLSearch _gstuSearch;
        //private DropDownList _pageList;
        private string ShowAllID;
        private DataTable dtShowAll00;
        //private SqlCommand sqlCommand;
        //private SqlDataAdapter sqlDataAdapter;
        //private SqlCommandBuilder sqlCommandBuilder;
        //private SqlConnection sqlConnection;
        //private DataRow drShowAll;

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            ShowAllID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[ShowAllID] != null)
            {
                if (ViewState["_gstuSearch"] == null)
                {
                    ViewState.Add("_gstuSearch", (StuGLSearch)Session[ShowAllID]);
                }
                else
                {
                    ViewState["_gstuSearch"] = (StuGLSearch)Session[ShowAllID];
                };
            }
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            if (ViewState["_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState["_gstuSearch"];
                if (!IsPostBack) { Session.Remove(ShowAllID + "dtShowAll"); }
                if (ViewState["Title"] == null) { ViewState.Add("Title", new CglDataSet(_gstuSearch.LottoType).LottoDescription); }

                ShowAllData();
            }
        }


        private void ShowAllData()
        {
            Page.Title = (string)ViewState["Title"];
            lblTitle.Text = (string)ViewState["Title"];

            if (Session[ShowAllID + "dtShowAll"] == null)
            {
                ImportData();
            }

            #region GridView 
            ShowDataAll();
            if (gvShowAll.EditIndex == -1)
            {
                gvShowAll.DataBind();
            }
            #endregion GridView 
        }

        private void ShowDataAll()
        {
            dtShowAll00 = (DataTable)Session[ShowAllID + "dtShowAll"];
            dtShowAll00.DefaultView.Sort = "[lngTotalSN] DESC";
            gvShowAll.DataSource = dtShowAll00.DefaultView;
            gvShowAll.PagerTemplate = new PagerTemplate();
            if (gvShowAll.Columns.Count == 0)
            {
                gvShowAll.Columns.Add(new CommandField() { ShowEditButton = true, ShowDeleteButton = true });
                for (int i = 0; i < dtShowAll00.Columns.Count; i++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = dtShowAll00.Columns[i].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(dtShowAll00.Columns[i].ColumnName, 1),
                        SortExpression = dtShowAll00.Columns[i].ColumnName,
                    };
                    gvShowAll.Columns.Add(bfCell);
                }
            }
            gvShowAll.DataKeyNames = new string[] { "lngTotalSN" };
            gvShowAll.DataBound += GvShowAll_DataBound;
            gvShowAll.RowDeleting += GvShowAll_RowDeleting;
            gvShowAll.RowCancelingEdit += GvShowAll_RowCancelingEdit;
            gvShowAll.RowEditing += GvShowAll_RowEditing;
            gvShowAll.RowUpdating += GvShowAll_RowUpdating;
        }

        private void ImportData()
        {
            using SqlCommand sqlCommand = new SqlCommand
            {
                Connection = new SqlConnection(new CglDBData().SetDataBase(_gstuSearch.LottoType, DatabaseType.Data)),
                CommandText = "SELECT * FROM [tblData] ORDER BY [lngTotalSN] DESC"
            };
            using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            using DataTable dtShowAll = new DataTable() { Locale = InvariantCulture };
            //sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
            //sqlDataAdapter.InsertCommand = sqlCommandBuilder.GetInsertCommand();
            //sqlDataAdapter.DeleteCommand = sqlCommandBuilder.GetDeleteCommand();
            //sqlDataAdapter.UpdateCommand = sqlCommandBuilder.GetUpdateCommand();
            sqlDataAdapter.Fill(dtShowAll);
            Session.Add(ShowAllID + "dtShowAll", dtShowAll);
        }

        //--------------------------------------------------------------------------------------------------
        #region pager

        private void BtnPrevPage10_Click(object sender, EventArgs e)
        {
            int intPageIndex = gvShowAll.PageIndex;
            intPageIndex -= 10;
            gvShowAll.PageIndex = (intPageIndex < 0) ? 0 : intPageIndex;
            gvShowAll.DataBind();
        }

        private void BtnNextPage10_Click(object sender, EventArgs e)
        {
            gvShowAll.PageIndex += 10;
            gvShowAll.DataBind();
        }

        private void BtnPrevPage_Click(object sender, EventArgs e)
        {
            int intPageIndex = gvShowAll.PageIndex;
            intPageIndex--;
            gvShowAll.PageIndex = (intPageIndex < 0) ? 0 : intPageIndex;
            gvShowAll.DataBind();
        }

        private void BtnNextPage_Click(object sender, EventArgs e)
        {
            gvShowAll.PageIndex++;
            gvShowAll.DataBind();
        }

        private void PageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender == null) { throw new ArgumentNullException(nameof(sender)); }
            // Retrieve the PageDropDownList DropDownList from the pager row.

            // Set the PageIndex property to display that page selected by the user.
            gvShowAll.PageIndex = ((DropDownList)sender).SelectedIndex;
            gvShowAll.DataBind();
        }

        #endregion pager

        //--------------------------------------------------------------------------------------------------
        #region GvShowAll

        private void GvShowAll_DataBound(object sender, EventArgs e)
        {
            // Retrieve the PagerRow.
            GridViewRow pagerRow = gvShowAll.TopPagerRow;

            // Retrieve the DropDownList and Label controls from the row.

            DropDownList _pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            _pageList.SelectedIndexChanged += PageList_SelectedIndexChanged; ;
            if (_pageList != null)
            {
                // Create the values for the DropDownList control based on 
                // the  total number of pages required to display the data
                // source.
                for (int i = 0; i < gvShowAll.PageCount; i++)
                {
                    // Create a ListItem object to represent a page.
                    int pageNumber = i + 1;
                    ListItem item = new ListItem(pageNumber.ToString(InvariantCulture));

                    // If the ListItem object matches the currently selected
                    // page, flag the ListItem object as being selected. Because
                    // the DropDownList control is recreated each time the pager
                    // row gets created, this will persist the selected item in
                    // the DropDownList control.   
                    if (i == gvShowAll.PageIndex)
                    {
                        item.Selected = true;
                    }

                    // Add the ListItem object to the Items collection of the 
                    // DropDownList.
                    _pageList.Items.Add(item);

                }
            }

            Button btnNextPage = (Button)pagerRow.Cells[0].FindControl("btnNext");
            btnNextPage.ToolTip = Properties.Resources.btnNextPageTip;
            btnNextPage.Click += BtnNextPage_Click;

            Button btnPrevPage = (Button)pagerRow.Cells[0].FindControl("btnPrev");
            btnPrevPage.ToolTip = Properties.Resources.btnPrevPageTip;
            btnPrevPage.Click += BtnPrevPage_Click;

            Button btnNextPage10 = (Button)pagerRow.Cells[0].FindControl("btnNext10");
            btnNextPage10.ToolTip = Properties.Resources.btnNext10PageTip;
            btnNextPage10.Click += BtnNextPage10_Click; ;

            Button btnPrevPage10 = (Button)pagerRow.Cells[0].FindControl("btnPrev10");
            btnPrevPage10.ToolTip = Properties.Resources.btnPrev10PageTip;
            btnPrevPage10.Click += BtnPrevPage10_Click; ;

            Label pageLabel = (Label)pagerRow.Cells[0].FindControl("pnlPagerTemplate").FindControl("CurrentPageLabel");
            if (pageLabel != null)
            {
                // Calculate the current page number.
                int currentPage = gvShowAll.PageIndex + 1;
                // Update the Label control with the current page information.
                pageLabel.Text = string.Format(InvariantCulture, "({0:d2} / {1:d2})", currentPage, gvShowAll.PageCount);
            }
        }

        private void GvShowAll_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridViewRow row = gvShowAll.Rows[e.RowIndex];
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = new SqlConnection(new CglDBData().SetDataBase(_gstuSearch.LottoType, DatabaseType.Data));
                sqlCommand.CommandText = "SELECT * FROM [tblData] ORDER BY [lngTotalSN] DESC";
                using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
                sqlDataAdapter.InsertCommand = sqlCommandBuilder.GetInsertCommand();
                sqlDataAdapter.DeleteCommand = sqlCommandBuilder.GetDeleteCommand();
                sqlDataAdapter.UpdateCommand = sqlCommandBuilder.GetUpdateCommand();
                using DataTable dtShowAll = new DataTable() { Locale = InvariantCulture };
                sqlDataAdapter.Fill(dtShowAll);
                dtShowAll.Rows[row.DataItemIndex].Delete();
                sqlDataAdapter.Update(dtShowAll);
                Session[ShowAllID + "dtShowAll"] = dtShowAll;
            }

            dtShowAll00 = (DataTable)Session[ShowAllID + "dtShowAll"];
            dtShowAll00.DefaultView.Sort = "[lngTotalSN] DESC";
            gvShowAll.DataSource = dtShowAll00.DefaultView;
            gvShowAll.DataBind();
        }

        private void GvShowAll_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvShowAll.Rows[e.RowIndex];
            using SqlCommand sqlCommand = new SqlCommand
            {
                Connection = new SqlConnection(new CglDBData().SetDataBase(_gstuSearch.LottoType, DatabaseType.Data)),
                CommandText = "SELECT * FROM [tblData] ORDER BY [lngTotalSN] DESC"
            };
            using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            using SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
            sqlDataAdapter.InsertCommand = sqlCommandBuilder.GetInsertCommand();
            sqlDataAdapter.DeleteCommand = sqlCommandBuilder.GetDeleteCommand();
            sqlDataAdapter.UpdateCommand = sqlCommandBuilder.GetUpdateCommand();
            using DataTable dtShowAll = new DataTable { Locale = InvariantCulture };
            sqlDataAdapter.Fill(dtShowAll);
            dtShowAll.Rows[row.DataItemIndex]["lngYearSN"] = long.Parse(((TextBox)row.Cells[2].Controls[0]).Text, InvariantCulture);
            dtShowAll.Rows[row.DataItemIndex]["lngDateSN"] = long.Parse(((TextBox)row.Cells[3].Controls[0]).Text, InvariantCulture);
            dtShowAll.Rows[row.DataItemIndex]["strSN"] = long.Parse(((TextBox)row.Cells[4].Controls[0]).Text, InvariantCulture);

            int intSum = 0;
            for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).CountNumber; i++)
            {
                string strNumName = string.Format(InvariantCulture, "lngL{0}", i);
                string strNum = ((TextBox)row.Cells[i + 4].Controls[0]).Text;
                int intNum = (string.IsNullOrEmpty(strNum)) ? 0 : int.Parse(strNum, InvariantCulture);
                dtShowAll.Rows[row.DataItemIndex][strNumName] = intNum;
                intSum += intNum;
            }
            for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).SCountNumber; i++)
            {
                string strNumName = string.Format(InvariantCulture, "lngS{0}", i);
                string strNum = ((TextBox)row.Cells[i + 4 + new CglDataSet(_gstuSearch.LottoType).CountNumber].Controls[0]).Text;
                int intNum = (string.IsNullOrEmpty(strNum)) ? 0 : int.Parse(strNum, InvariantCulture);
                dtShowAll.Rows[row.DataItemIndex][strNumName] = intNum;
                intSum += intNum;
            }
            dtShowAll.Rows[row.DataItemIndex]["intSum"] = intSum;
            sqlDataAdapter.Update(dtShowAll);
            Session[ShowAllID + "dtShowAll"] = dtShowAll;
            dtShowAll00 = (DataTable)Session[ShowAllID + "dtShowAll"];
            dtShowAll00.DefaultView.Sort = "[lngTotalSN] DESC";
            gvShowAll.DataSource = dtShowAll00.DefaultView;
            gvShowAll.EditIndex = -1;
            gvShowAll.DataBind();
        }

        private void GvShowAll_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvShowAll.EditIndex = -1;
            gvShowAll.DataBind();
        }

        private void GvShowAll_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvShowAll.EditIndex = e.NewEditIndex;
            gvShowAll.DataBind();
        }

        #endregion GvShowAll

        //--------------------------------------------------------------------------------------------------
        #region Button
        protected void BtnCloseClick(object sender, EventArgs e)
        {
            Session.Remove(ShowAllID);
            Session.Remove(ShowAllID + "dtShowAll");
        }

        protected void BtnNewDataClick(object sender, EventArgs e)
        {
            using SqlCommand sqlCommand = new SqlCommand
            {
                Connection = new SqlConnection(new CglData().SetDataBase(_gstuSearch.LottoType, DatabaseType.Data)),
                CommandText = "SELECT * FROM [tblData] ORDER BY [lngTotalSN] DESC "
            };
            using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            using SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
            sqlDataAdapter.InsertCommand = sqlCommandBuilder.GetInsertCommand();
            sqlDataAdapter.DeleteCommand = sqlCommandBuilder.GetDeleteCommand();
            sqlDataAdapter.UpdateCommand = sqlCommandBuilder.GetUpdateCommand();
            using DataTable dtShowAll = new DataTable { Locale = InvariantCulture };
            sqlDataAdapter.Fill(dtShowAll);
            DataRow drShowAll = dtShowAll.NewRow();
            drShowAll["lngTotalSN"] = long.Parse(dtShowAll.Rows[0]["lngTotalSN"].ToString(), InvariantCulture) + 1;
            drShowAll["lngYearSN"] = long.Parse(dtShowAll.Rows[0]["lngYearSN"].ToString(), InvariantCulture) + 1;
            drShowAll["lngDateSN"] = string.Format(InvariantCulture, "{0}{1:d2}{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            drShowAll["strSN"] = string.Format(InvariantCulture, "{0}{1:d3}", drShowAll["lngDateSN"], int.Parse(drShowAll["lngYearSN"].ToString(), InvariantCulture));
            for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).CountNumber; i++)
            {
                drShowAll[string.Format(InvariantCulture, "lngL{0}", i)] = 0;
            }
            for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).SCountNumber; i++)
            {
                drShowAll[string.Format(InvariantCulture, "lngS{0}", i)] = 0;
            }

            drShowAll["intSum"] = 0;
            dtShowAll.Rows.Add(drShowAll);
            sqlDataAdapter.Update(dtShowAll);
            Session[ShowAllID + "dtShowAll"] = dtShowAll;
            dtShowAll00 = (DataTable)Session[ShowAllID + "dtShowAll"];
            dtShowAll00.DefaultView.Sort = "[lngTotalSN] DESC";
            gvShowAll.DataSource = dtShowAll00.DefaultView;
            gvShowAll.DataBind();
        }
        #endregion Button

    }
}