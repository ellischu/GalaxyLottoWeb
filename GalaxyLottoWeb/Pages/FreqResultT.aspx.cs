using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqResultT : BasePage
    {
        private StuGLSearch _gstuSearch;
        private string _action;
        private string _requestId;
        private string FreqResultTID;
        private Dictionary<string, string> DicNumcssclass { get; set; } = new Dictionary<string, string>();
        private DataTable DtQryFreqFilters { get; set; }
        private DataTable DtFilter { get; set; }
        private DataTable DtFreqSum { get; set; }
        private string LocalBrowserType { get; set; }

        private string LocalIP { get; set; }

        //private string KeySearchOrder { get; set; }

        //Thread part
        private static Dictionary<string, object> DicThreadFreqResultTT011
        {
            get
            {
                if (dicThreadFreqResultTT01 == null) { dicThreadFreqResultTT01 = new Dictionary<string, object>(); }
                return dicThreadFreqResultTT01;
            }

            set => dicThreadFreqResultTT01 = value;
        }

        private static Dictionary<string, object> dicThreadFreqResultTT01;

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            FreqResultTID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqResultTID] != null)
            {
                if (ViewState[FreqResultTID + "_gstuSearch"] == null)
                {
                    ViewState.Add(FreqResultTID + "_gstuSearch", (StuGLSearch)Session[FreqResultTID]);
                }
                else
                {
                    ViewState[FreqResultTID + "_gstuSearch"] = (StuGLSearch)Session[FreqResultTID];
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
            if (ViewState[FreqResultTID + "_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                LocalBrowserType = Request.Browser.Type;
                LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
                //KeySearchOrder = string.Format(InvariantCulture, "{0}#{1}#dtSearchOrder", LocalIP, LocalBrowserType);

                _gstuSearch = (StuGLSearch)ViewState[FreqResultTID + "_gstuSearch"];
                Initialize();
                ShowResult();
            }
        }

        private void Initialize()
        {
            if (ViewState["title"] == null) { ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "頻率總表", new CglDBData().SetTitleString(_gstuSearch))); }
            if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch))); }
            if (ViewState["sort"] == null) { ViewState.Add("sort", ""); }
            if (ViewState["select"] == null) { ViewState.Add("select", ""); }
            if (ViewState["_dicNumcssclass"] == null) { ViewState.Add("_dicNumcssclass", new GalaxyApp().GetNumcssclass(_gstuSearch)); }
            DicNumcssclass = (Dictionary<string, string>)ViewState["_dicNumcssclass"];
            if (ServerOption.Count > 0 &&
                ServerOption.ContainsKey(string.Format(InvariantCulture, "{0}#{1}#dtFrtSearchOrder", LocalIP, LocalBrowserType)) &&
                ((DataSet)ServerOption[string.Format(InvariantCulture, "{0}#{1}#dtFrtSearchOrder", LocalIP, LocalBrowserType)]).Tables.Contains("FrtSearchOrder"))
            {
                btnShowFRTSearchOrder.Visible = ((DataSet)ServerOption[string.Format(InvariantCulture, "{0}#{1}#dtFrtSearchOrder", LocalIP, LocalBrowserType)]).Tables["FrtSearchOrder"].Rows.Count > 0;
            }
            else
            {
                btnShowFRTSearchOrder.Visible = false;
            }
            //btnShowFRTSearchOrder.Visible = Search.DtFrtSearchOrder.Rows.Count > 0 ? true : false;
        }

        private void ShowResult()
        {
            //顯示當前資料
            Page.Title = (string)ViewState["title"];
            lblTitle.Text = (string)ViewState["title"];
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            ShowQryFreqFilters();
            ShowFreqSum();
            CreateFilter();
            if (!DicThreadFreqResultTT011.Keys.Contains(FreqResultTID + "T01")) { CreatThread(); }
        }

        //---------------------------------------------------------------------------------------------------------
        #region GvFreqSum

        private void ShowFreqSum()
        {
            if (Session[FreqResultTID + "dtFreqSum"] != null)
            {
                DtFreqSum = (DataTable)Session[FreqResultTID + "dtFreqSum"];
                gvFreqSum.DataSource = DtFreqSum.DefaultView;
                if (gvFreqSum.Columns.Count != 0)
                {
                    gvFreqSum.Columns.Clear();
                }
                for (int ColumnIndex = 0; ColumnIndex < DtFreqSum.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtFreqSum.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtFreqSum.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtFreqSum.Columns[ColumnIndex].ColumnName,
                    };
                    gvFreqSum.Columns.Add(bfCell);
                }
                foreach (DataControlField dcColumn in gvFreqSum.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if (DicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                    {
                        dcColumn.HeaderStyle.CssClass = DicNumcssclass[strColumnName.Substring(4)];
                    }
                }
                gvFreqSum.DataBind();
            }
        }

        private void CreatFreqSum()
        {
            using DataTable dtFreqSumTemp = ((DataTable)Session[FreqResultTID + "dtQryFreqFilters"]).Select((string)ViewState["select"], (string)ViewState["sort"]).CopyToDataTable();
            Dictionary<string, int> dicNums = new Dictionary<string, int>();
            for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; i++)
            {
                dicNums.Add(string.Format(InvariantCulture, "lngN{0}", i), 0);
            }
            List<string> lstNums = new List<string>();
            foreach (DataRow drFreqSumTemp in dtFreqSumTemp.Rows)
            {
                lstNums.Add(drFreqSumTemp["strFreqNums"].ToString());
            }
            lstNums.Sort();
            List<string> lstNumsDistinct = lstNums.Distinct().ToList();
            foreach (string strNums in lstNumsDistinct)
            {
                foreach (string strNum in strNums.Split('#'))
                {
                    string KeyName = string.Format(InvariantCulture, "lngN{0}", int.Parse(strNum, InvariantCulture));
                    dicNums[KeyName]++;
                }
            }
            Dictionary<string, int> dicFreqSort = dicNums.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            if (Session[FreqResultTID + "dtFreqSum"] == null)
            {

                Session.Add(FreqResultTID + "dtFreqSum", new CglFunc().CDicTOTable(dicFreqSort, null));
            }
            else
            {
                Session[FreqResultTID + "dtFreqSum"] = new CglFunc().CDicTOTable(dicFreqSort, null);
            }
        }

        #endregion GvFreqSum
        //---------------------------------------------------------------------------------------------------------

        #region GvFilter

        private void CreateFilter()
        {

            CreateDataTableFilter();
            if (Session[FreqResultTID + "dtFilter"] != null)
            {
                #region gvFilter
                DtFilter = (DataTable)Session[FreqResultTID + "dtFilter"];
                gvFilter01.DataSource = DtFilter.DefaultView;
                if (gvFilter01.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < DtFilter.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFilter.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFilter.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = DtFilter.Columns[ColumnIndex].ColumnName,
                        };
                        bfCell.HeaderStyle.CssClass = bfCell.DataField;
                        bfCell.ItemStyle.CssClass = bfCell.DataField;
                        gvFilter01.Columns.Add(bfCell);
                        //if (dtQryFreqFilters.Columns[ColumnIndex].ColumnName == "CDataType")
                        //{
                        //    gvQryFreqFilters.Columns[ColumnIndex].Visible = false;
                        //}
                    }
                }

                for (int ColumnIndex = 0; ColumnIndex < DtFilter.Columns.Count; ColumnIndex++)
                {
                    gvFilter01.Columns[ColumnIndex].HeaderText = DtFilter.Columns[ColumnIndex].Caption;
                }
                gvFilter01.RowDataBound += GvFilterRowDataBound;
                gvFilter01.DataBind();
                #endregion gvFilter
            }

            btnYes.Text = Properties.Resources.Yes;
            btnYes.TabIndex = 0;

            btnCancel.Text = Properties.Resources.Cancel;
        }

        private void CreateDataTableFilter()
        {
            if (Session[FreqResultTID + "dtFilter"] == null && DtQryFreqFilters != null)
            {
                using (DtFilter = new DataTable())
                {
                    DtFilter.Locale = InvariantCulture;
                    #region Column
                    using (DataColumn dcFilter = new DataColumn())
                    {
                        dcFilter.ColumnName = "CName";
                        dcFilter.DataType = typeof(string);
                        dcFilter.AllowDBNull = false;
                        dcFilter.Unique = true;
                        dcFilter.Caption = Properties.Resources.lblColumn;
                        DtFilter.Columns.Add(dcFilter);
                    }
                    using (DataColumn dcFilter = new DataColumn())
                    {
                        dcFilter.ColumnName = "CDataType";
                        dcFilter.DataType = typeof(string);
                        dcFilter.AllowDBNull = false;
                        dcFilter.Caption = Properties.Resources.lblColumnDataType;
                        DtFilter.Columns.Add(dcFilter);
                    }
                    using (DataColumn dcFilter = new DataColumn())
                    {
                        dcFilter.ColumnName = "CCaption";
                        dcFilter.DataType = typeof(string);
                        dcFilter.AllowDBNull = false;
                        dcFilter.Caption = Properties.Resources.lblCaption;
                        DtFilter.Columns.Add(dcFilter);
                    }
                    using (DataColumn dcFilter = new DataColumn())
                    {
                        dcFilter.ColumnName = "CSort";
                        dcFilter.DataType = typeof(int);
                        dcFilter.AllowDBNull = false;
                        dcFilter.Caption = Properties.Resources.lblSort;
                        dcFilter.DefaultValue = SortOrder.Unspecified;
                        DtFilter.Columns.Add(dcFilter);
                    }
                    using (DataColumn dcFilter = new DataColumn())
                    {
                        dcFilter.ColumnName = "CSortOrder";
                        dcFilter.DataType = typeof(int);
                        dcFilter.AllowDBNull = false;
                        dcFilter.Caption = Properties.Resources.lblSortOrder;
                        dcFilter.DefaultValue = 100;
                        DtFilter.Columns.Add(dcFilter);
                    }
                    using (DataColumn dcFilter = new DataColumn())
                    {
                        dcFilter.ColumnName = "CWhere";
                        dcFilter.DataType = typeof(string);
                        dcFilter.AllowDBNull = false;
                        dcFilter.Caption = Properties.Resources.lblWhere;
                        dcFilter.DefaultValue = "none";
                        DtFilter.Columns.Add(dcFilter);
                    }
                    #endregion Column

                    foreach (DataColumn dcQryFreqFilters in DtQryFreqFilters.Columns)
                    {
                        DataRow drFilter = DtFilter.NewRow();
                        drFilter["CName"] = dcQryFreqFilters.ColumnName;
                        drFilter["CCaption"] = new CglFunc().ConvertFieldNameId(dcQryFreqFilters.ColumnName, 1);
                        drFilter["CDataType"] = dcQryFreqFilters.DataType.ToString();
                        DtFilter.Rows.Add(drFilter);
                    }
                }
                Session[FreqResultTID + "dtFilter"] = DtFilter;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:不要將常值當做已當地語系化的參數傳遞", MessageId = "System.Web.UI.WebControls.WebControl.set_ToolTip(System.String)")]
        private void GvFilterRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        #region CSort
                        if (strCell_ColumnName == "CSort")
                        {
                            using DropDownList dropDownList = new DropDownList { ID = "ddlCSort" };
                            dropDownList.Items.Add(new ListItem(Properties.Resources.Unspecified, "-1"));
                            dropDownList.Items.Add(new ListItem(Properties.Resources.Ascending, "0"));
                            dropDownList.Items.Add(new ListItem(Properties.Resources.Descending, "1"));
                            dropDownList.SelectedValue = cell.Text;
                            cell.Controls.Add(dropDownList);
                        }
                        #endregion CSort

                        #region CSortOrder
                        if (strCell_ColumnName == "CSortOrder")
                        {
                            using DropDownList dropDownList = new DropDownList { ID = "ddlCSortOrder" };
                            dropDownList.Items.Add(new ListItem(Properties.Resources.Unspecified, "100"));
                            for (int index = 1; index <= DtFilter.Rows.Count; index++)
                            {
                                dropDownList.Items.Add(new ListItem(index.ToString(InvariantCulture), index.ToString(InvariantCulture)));
                            }
                            dropDownList.SelectedValue = cell.Text;
                            cell.Controls.Add(dropDownList);
                        }
                        #endregion CSortOrder

                        #region CWhere
                        if (strCell_ColumnName == "CWhere")
                        {
                            using TextBox textBox = new TextBox
                            {
                                ID = "txtCWhere",
                                Text = (cell.Text == "&nbsp;") ? string.Empty : cell.Text.Replace("&lt;", "<").Replace("&gt;", ">"),
                                ToolTip = " Use ',' to Separate "
                            };
                            cell.Controls.Add(textBox);
                        }
                        #endregion CWhere
                    }
                }
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected void BtnYesClick(object sender, EventArgs e)
        {
            for (int rowIndex = 0; rowIndex < DtFilter.Rows.Count; rowIndex++)
            {
                foreach (DataControlFieldCell cell in gvFilter01.Rows[rowIndex].Cells)
                {
                    string strCell_ColumnName = ((BoundField)cell.ContainingField).DataField;
                    #region CSort
                    if (strCell_ColumnName == "CSort" || strCell_ColumnName == "CSortOrder")
                    {
                        DtFilter.Rows[rowIndex][strCell_ColumnName] = ((DropDownList)cell.Controls[0]).SelectedValue;
                    }
                    #endregion CSor

                    #region CWhere
                    if (strCell_ColumnName == "CWhere")
                    {
                        if (string.IsNullOrEmpty(((TextBox)cell.Controls[0]).Text))
                        {
                            DtFilter.Rows[rowIndex][strCell_ColumnName] = "none";
                        }
                        else
                        {
                            DtFilter.Rows[rowIndex][strCell_ColumnName] = ((TextBox)cell.Controls[0]).Text.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&nbsp;", "none");
                        }
                    }
                    #endregion CWhere
                }
            }
            SetFilterViewState();
            RunFilter();
            Form.DefaultButton = btnOneKeyFilter.ClientID;
        }

        protected void BtnCancelClick(object sender, EventArgs e)
        {
            pnlFilter.Visible = !pnlFilter.Visible;
            Form.DefaultButton = btnOneKeyFilter.ClientID;
        }

        private void SetFilterViewState()
        {
            #region sort
            if (DtFilter.Select("[CSort] >= 0 ", "[CSortOrder] ASC").Length > 0)
            {

                for (int indexRow = 0; indexRow < DtFilter.Select("[CSort] >= 0 ", "[CSortOrder] ASC").Length; indexRow++)
                {
                    DataRow drRow = DtFilter.Select("[CSort] >= 0 ", "[CSortOrder] ASC")[indexRow];
                    DtFilter.Rows[DtFilter.Rows.IndexOf(drRow)]["CSortOrder"] = indexRow + 1;
                }
                using DataTable dtFilterTemp = DtFilter.Select("[CSort] >= 0 ", "[CSortOrder] ASC").CopyToDataTable();
                List<string> lstSort = new List<string>();
                foreach (DataRow drFilterTemp in dtFilterTemp.Rows)
                {
                    lstSort.Add(string.Format(InvariantCulture, "[{0}] {1} ", drFilterTemp["CName"], int.Parse(drFilterTemp["CSort"].ToString(), InvariantCulture) == 0 ? "ASC" : "DESC"));
                }
                ViewState["sort"] = string.Join(",", lstSort.ToArray());
            }
            #endregion sort

            #region select
            if (DtFilter.Select("[CWhere] <> 'none' ").Length > 0)
            {
                using DataTable dtFilterTemp = DtFilter.Select("[CWhere] <> 'none' ").CopyToDataTable();
                List<string> lstSort = new List<string>();
                foreach (DataRow drFilterTemp in dtFilterTemp.Rows)
                {
                    foreach (string strWhere in drFilterTemp["CWhere"].ToString().Split(','))
                    {
                        if (strWhere.Contains(">") || strWhere.Contains("<") || strWhere.Contains("=") || strWhere.Contains("!"))
                        {
                            if (drFilterTemp["CDataType"].ToString() == "System.String")
                            {
                                string strOperator;
                                string Express = strWhere.Replace("!=", "<>");
                                if (Express.Contains("=") || Express.Contains("<>"))
                                {
                                    strOperator = Express.Contains("<>") ? "<>" : "=";
                                    lstSort.Add(string.Format(InvariantCulture, "[{0}] {1} '{2}' ",
                                                                                drFilterTemp["CName"],
                                                                                strOperator,
                                                                                Express.Substring(Express.IndexOf(strOperator, StringComparison.CurrentCulture) + strOperator.Length).Trim()));
                                }
                            }
                            else
                            {
                                lstSort.Add(string.Format(InvariantCulture, "[{0}] {1} ", drFilterTemp["CName"], strWhere));
                            }
                        }
                    }
                    ViewState["select"] = string.Join(" AND ", lstSort.ToArray());
                }
            }
            #endregion select

            Session[FreqResultTID + "dtFilter"] = DtFilter;
        }

        private void RunFilter()
        {
            pnlFilter.Visible = false;
            DtQryFreqFilters = (DataTable)Session[FreqResultTID + "dtQryFreqFilters"];
            DtQryFreqFilters.DefaultView.RowFilter = (string)ViewState["select"];
            DtQryFreqFilters.DefaultView.Sort = (string)ViewState["sort"];
            gvQryFreqFilters.DataSource = DtQryFreqFilters.DefaultView;
            gvQryFreqFilters.DataBind();
            CreatFreqSum();
            ShowFreqSum();
        }

        #endregion GvFilter

        //---------------------------------------------------------------------------------------------------------

        #region GvQryFreqFilters

        private void ShowQryFreqFilters()
        {
            if (Session[FreqResultTID + "dtQryFreqFilters"] != null)
            {
                btnquickSearchOrder.Visible = true;
                btnFilter.Visible = true;
                btnOneKeyFilter.Visible = true;
                btnAIFilter.Visible = true;
                btnNoFilter.Visible = true;
                DtQryFreqFilters = (DataTable)Session[FreqResultTID + "dtQryFreqFilters"];
                lblQryFreqFilters.Text = string.Format(InvariantCulture, "QryFreqFilters ({0}期) : ", DtQryFreqFilters.Rows.Count);
                DtQryFreqFilters.DefaultView.RowFilter = (string)ViewState["select"];
                DtQryFreqFilters.DefaultView.Sort = (string)ViewState["sort"];
                gvQryFreqFilters.DataSource = DtQryFreqFilters.DefaultView;
                //gvQryFreqFilters.Visible = (gvQryFreqFilters.Rows.Count == 0) ? false : true;
                gvQryFreqFilters.PagerTemplate = new PagerTemplate();

                if (gvQryFreqFilters.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < DtQryFreqFilters.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtQryFreqFilters.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtQryFreqFilters.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = DtQryFreqFilters.Columns[ColumnIndex].ColumnName,
                        };
                        bfCell.HeaderStyle.CssClass = bfCell.DataField;
                        bfCell.ItemStyle.CssClass = bfCell.DataField;
                        gvQryFreqFilters.Columns.Add(bfCell);
                        //if (dtQryFreqFilters.Columns[ColumnIndex].ColumnName == "CDataType")
                        //{
                        //    gvQryFreqFilters.Columns[ColumnIndex].Visible = false;
                        //}
                    }
                }

                gvQryFreqFilters.RowDataBound += GvQryFreqFilters_RowDataBound;
                gvQryFreqFilters.DataBound += GvQryFreqFilters_DataBound;
                gvQryFreqFilters.DataBind();
            }
        }

        private DataTable CreatQryFreqFilters(StuGLSearch stuGLSearch)
        {
            using DataTable dtFreqFilters = new CglFreqFilter().GetQryFreqFilterTMutiple(_gstuSearch);
            if (dtFreqFilters.Columns.Contains("lngDateSN")) { dtFreqFilters.Columns.Remove("lngDateSN"); }
            if (!dtFreqFilters.Columns.Contains("intHitRateH"))
            {
                dtFreqFilters.Columns.Add(new DataColumn()
                {
                    ColumnName = "intHitRateH",
                    DataType = typeof(double),
                    DefaultValue = -1
                });
            }
            foreach (DataRow drFreqFilters in dtFreqFilters.Rows)
            {
                if (drFreqFilters["TestPeriods"].ToString() == drFreqFilters["intHistoryTestPeriods"].ToString() &&
                   int.Parse(drFreqFilters["intNumCount"].ToString(), InvariantCulture) >= 9 &&
                   int.Parse(drFreqFilters["intNumCount"].ToString(), InvariantCulture) <= 11)
                {
                    drFreqFilters["intHitRateH"] = GetHistoryHitRate(stuGLSearch, drFreqFilters);
                }
            }
            return dtFreqFilters;
        }

        private double GetHistoryHitRate(StuGLSearch stuGLSearch, DataRow drFreqFilters)
        {
            CheckData = true;
            using SqlCommand sqlCommand = new SqlCommand
            {
                Connection = new SqlConnection(new CglData().SetDataBase(stuGLSearch.LottoType, DatabaseType.FreqFilter)),
                CommandTimeout = CglClass.SqlCommandTimeout
            };
            using SqlDataAdapter sleDataAdapter = new SqlDataAdapter(sqlCommand);
            sqlCommand.CommandText = "SELECT TOP(100) * FROM [qryFreqFilter01] " +
                                             "WHERE [lngTotalSN] < @lngTotalSN " +
                                             //"AND [lngMethodSN] = @lngMethodSN " +
                                             "AND [lngSearchMethodSN] = @lngSearchMethodSN " +
                                             //"AND [lngSecFieldSN] = @lngSecFieldSN " +
                                             "AND [intNumCount] = @intNumCount " +
                                             "AND [intHistoryTestPeriods] = @intHistoryTestPeriods " +
                                             "AND [intHistoryHitPeriods] = @intHistoryHitPeriods ";
            sqlCommand.Parameters.AddWithValue("lngTotalSN", stuGLSearch.LngTotalSN);
            sqlCommand.Parameters.AddWithValue("intNumCount", drFreqFilters["intNumCount"]);
            //sqlCommand.Parameters.AddWithValue("lngMethodSN", drFreqFilters["lngMethodSN"]);
            sqlCommand.Parameters.AddWithValue("lngSearchMethodSN", drFreqFilters["lngSearchMethodSN"]);
            //sqlCommand.Parameters.AddWithValue("lngSecFieldSN", drFreqFilters["lngSecFieldSN"]);
            sqlCommand.Parameters.AddWithValue("intHistoryTestPeriods", drFreqFilters["intHistoryTestPeriods"]);
            sqlCommand.Parameters.AddWithValue("intHistoryHitPeriods", drFreqFilters["intHistoryHitPeriods"]);

            using DataTable dtResult = new DataTable { Locale = InvariantCulture };
            sleDataAdapter.Fill(dtResult);
            double hitcount = dtResult.Select(string.Format(InvariantCulture, "[intHitCount] >= {0}", stuGLSearch.InHitMin)).Length;
            return dtResult.Rows.Count > 0 ?
                   (dtResult.Rows.Count < 10 && hitcount == dtResult.Rows.Count) ? 10 : Math.Round(hitcount / dtResult.Rows.Count * 100d, 3)
                   : 0;
        }

        private void GvQryFreqFilters_DataBound(object sender, EventArgs e)
        {
            if (gvQryFreqFilters.Rows.Count > 0)
            {
                GridViewRow pagerRow = gvQryFreqFilters.TopPagerRow;
                pagerRow.Visible = true;
                DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
                pageList.SelectedIndexChanged += PageDropDownListSelectedIndexChanged; ;
                if (pageList != null)
                {
                    // Create the values for the DropDownList control based on 
                    // the  total number of pages required to display the data
                    // source.
                    for (int i = 0; i < gvQryFreqFilters.PageCount; i++)
                    {
                        // Create a ListItem object to represent a page.
                        int pageNumber = i + 1;
                        ListItem item = new ListItem(pageNumber.ToString(InvariantCulture));

                        // If the ListItem object matches the currently selected
                        // page, flag the ListItem object as being selected. Because
                        // the DropDownList control is recreated each time the pager
                        // row gets created, this will persist the selected item in
                        // the DropDownList control.   
                        if (i == gvQryFreqFilters.PageIndex)
                        {
                            item.Selected = true;
                        }

                        // Add the ListItem object to the Items collection of the 
                        // DropDownList.
                        pageList.Items.Add(item);
                    }
                }

                Button btnPrevPage = (Button)pagerRow.Cells[0].FindControl("btnPrev");
                btnPrevPage.Text = Properties.Resources.btnPrevPage;
                btnPrevPage.ToolTip = Properties.Resources.btnPrevPageTip;
                btnPrevPage.Click += BtnPrevPageClick;

                Button btnPrev10Page = (Button)pagerRow.Cells[0].FindControl("btnPrev10");
                btnPrev10Page.Text = Properties.Resources.btnPrev10Page;
                btnPrev10Page.ToolTip = Properties.Resources.btnPrev10PageTip;
                btnPrev10Page.Click += BtnPrev10Page_Click;

                Label pageLabel = (Label)pagerRow.Cells[0].FindControl("pnlPagerTemplate").FindControl("CurrentPageLabel");
                if (pageLabel != null)
                {
                    // Calculate the current page number.
                    int currentPage = gvQryFreqFilters.PageIndex + 1;
                    // Update the Label control with the current page information.
                    pageLabel.Text = string.Format(InvariantCulture, " ({0:d2} / {1:d2})", currentPage, gvQryFreqFilters.PageCount);
                }

                Button btnNextPage = (Button)pagerRow.Cells[0].FindControl("btnNext");
                btnNextPage.Text = Properties.Resources.btnNextPage;
                btnNextPage.ToolTip = Properties.Resources.btnNextPageTip;
                btnNextPage.Click += BtnNextPage_Click;

                Button btnNext10Page = (Button)pagerRow.Cells[0].FindControl("btnNext10");
                btnNext10Page.Text = Properties.Resources.btnNext10Page;
                btnNext10Page.ToolTip = Properties.Resources.btnNext10PageTip;
                btnNext10Page.Click += BtnNext10PageClick; ;
            }
        }

        private void GvQryFreqFilters_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int intHistoryTestPeriods = 0;
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        #region lngTotalSN

                        if (strCell_ColumnName == "lngTotalSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngMethodSN = long.Parse(e.Row.Cells[2].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSearchMethodSN = long.Parse(e.Row.Cells[3].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSearchMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSecFieldSN = long.Parse(e.Row.Cells[4].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);

                            stuGLSearchTemp.HistoryTestPeriods = float.Parse(e.Row.Cells[10].Text, InvariantCulture);

                            string RequestID = SetRequestId(stuGLSearchTemp);

                            using Button btnFunction = new Button
                            {
                                ID = string.Format(InvariantCulture, "btnFunction_{0}", e.Row.Cells[0].Text),
                                Text = Properties.Resources.View
                            };
                            if (stuGLSearchTemp.LngSecFieldSN == 1)
                            {
                                if (chkFRTSerchOrder.Checked)
                                {
                                    btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('setFRTSearchOrder.aspx?action={0}&id={1}&UrlFileName={2}','_blank');",
                                                                                                        Properties.Resources.SessionsFreqResultSingle,
                                                                                                        RequestID,
                                                                                                        Properties.Resources.PageFreqResultSingle);
                                }
                                else
                                {
                                    btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('{0}?action={1}&id={2}','_blank');",
                                                                                                           Properties.Resources.PageFreqResultSingle,
                                                                                                           Properties.Resources.SessionsFreqResultSingle,
                                                                                                           RequestID);
                                }
                                Session.Add(Properties.Resources.SessionsFreqResultSingle + RequestID, stuGLSearchTemp);
                            }
                            else
                            {
                                if (chkFRTSerchOrder.Checked)
                                {
                                    btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('setFRTSearchOrder.aspx?action={0}&id={1}&UrlFileName={2}','_blank');",
                                                                                                        Properties.Resources.SessionsFreqActiveHSingle,
                                                                                                        RequestID,
                                                                                                        Properties.Resources.PageFreqActiveHSingle);
                                }
                                else
                                {
                                    btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('{0}?action={1}&id={2}','_blank');",
                                                                                                       Properties.Resources.PageFreqActiveHSingle,
                                                                                                       Properties.Resources.SessionsFreqActiveHSingle,
                                                                                                       RequestID);
                                }
                                stuGLSearchTemp.FrtSearchOrder = true;
                                Session.Add(Properties.Resources.SessionsFreqActiveHSingle + RequestID, stuGLSearchTemp);
                            }
                            cell.Controls.Add(btnFunction);
                        }

                        #endregion lngTotalSN

                        #region lngMethodSN
                        if (strCell_ColumnName == "lngMethodSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngMethodSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetMethodWithSN(stuGLSearchTemp);
                            cell.ToolTip = new CglMethod().SetMethodString(stuGLSearchTemp);
                        }
                        #endregion lngMethodSN

                        #region lngSearchMethodSN
                        if (strCell_ColumnName == "lngSearchMethodSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngSearchMethodSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSearchMethodWithSN(stuGLSearchTemp);
                            cell.ToolTip = new CglMethod().SetSearchMethodString(stuGLSearchTemp);
                        }
                        #endregion lngSearchMethodSN                       

                        #region lngSecFieldSN
                        if (strCell_ColumnName == "lngSecFieldSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngSecFieldSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);
                            cell.ToolTip = stuGLSearchTemp.StrSecField;
                        }
                        #endregion lngSecFieldSN         

                        #region intHistoryTestPeriods
                        if (strCell_ColumnName == "intHistoryTestPeriods")
                        {
                            intHistoryTestPeriods = int.Parse(cell.Text, InvariantCulture);
                        }
                        #endregion intHistoryTestPeriods  

                        #region intHitRateH
                        if (strCell_ColumnName == "intHitRateH")
                        {
                            switch (intHistoryTestPeriods)
                            {
                                case 20:
                                    cell.CssClass = "HTP20 ";
                                    break;
                                case 15:
                                    cell.CssClass = "HTP15 ";
                                    break;
                                case 10:
                                    cell.CssClass = "HTP10 ";
                                    break;
                            }
                        }
                        #endregion intHitRateH  
                    }
                }
            }
        }

        #endregion GvQryFreqFilters

        // ---------------------------------------------------------------------------------------------------------

        private void PageDropDownListSelectedIndexChanged(object sender, EventArgs e)
        {
            // Retrieve the PageDropDownList DropDownList from the pager row.

            // Set the PageIndex property to display that page selected by the user.
            gvQryFreqFilters.PageIndex = ((DropDownList)sender).SelectedIndex;
            gvQryFreqFilters.DataBind();
        }

        private void BtnPrev10Page_Click(object sender, EventArgs e)
        {
            int intPageIndex = gvQryFreqFilters.PageIndex;
            intPageIndex -= 10;
            gvQryFreqFilters.PageIndex = (intPageIndex < 0) ? 0 : intPageIndex;
            gvQryFreqFilters.DataBind();
        }

        private void BtnNext10PageClick(object sender, EventArgs e)
        {
            gvQryFreqFilters.PageIndex += 10;
            gvQryFreqFilters.DataBind();
        }

        private void BtnPrevPageClick(object sender, EventArgs e)
        {
            int intPageIndex = gvQryFreqFilters.PageIndex;
            intPageIndex--;
            gvQryFreqFilters.PageIndex = (intPageIndex < 0) ? 0 : intPageIndex;
            gvQryFreqFilters.DataBind();
        }

        private void BtnNextPage_Click(object sender, EventArgs e)
        {
            gvQryFreqFilters.PageIndex++;
            gvQryFreqFilters.DataBind();
        }

        //---------------------------------------------------------------------------------------------------------

        protected void BtnReloadClick(object sender, EventArgs e)
        {
            ReleaseMemory();

            Session[FreqResultTID] = (StuGLSearch)ViewState[FreqResultTID + "_gstuSearch"];

            string strUrl = string.Format(InvariantCulture, "http://{0}/Pages/{1}?action={2}&id={3}",
                                                            Request.Url.Authority,
                                                            Properties.Resources.PageFreqResultT,
                                                            Properties.Resources.SessionsFreqResultT,
                                                            (string)ViewState["id"]);
            Response.Redirect(strUrl, true);
        }

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            Session.Remove(FreqResultTID);
            ViewState.Remove(FreqResultTID + "_gstuSearch");
            ViewState.Remove("action");
            ViewState.Remove("id");
            ReleaseMemory();
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
        }

        protected void BtnFilterClick(object sender, EventArgs e)
        {
            Form.DefaultButton = btnYes.ClientID;
            btnYes.TabIndex = 0;
            pnlFilter.Visible = !pnlFilter.Visible;
        }

        protected void BtnNoFilterClick(object sender, EventArgs e)
        {
            Form.DefaultButton = btnOneKeyFilter.ClientID;
            btnOneKeyFilter.TabIndex = 0;
            pnlFilter.Visible = false;
            ViewState["sort"] = "";
            ViewState["select"] = "";
            DtQryFreqFilters = (DataTable)Session[FreqResultTID + "dtQryFreqFilters"];
            DtQryFreqFilters.DefaultView.RowFilter = (string)ViewState["select"];
            DtQryFreqFilters.DefaultView.Sort = (string)ViewState["sort"];
            gvQryFreqFilters.DataBind();
            Session.Remove(FreqResultTID + "dtFilter");
            CreateDataTableFilter();
        }

        protected void BtnOneKeyFilterClick(object sender, EventArgs e)
        {
            SetdtFilter();
            SetFilterViewState();
            RunFilter();
        }

        private void SetdtFilter()
        {
            pnlFilter.Visible = false;
            Session.Remove(FreqResultTID + "dtFilter");
            CreateDataTableFilter();
            foreach (DataRow drFilter in DtFilter.Rows)
            {
                if (drFilter["CName"].ToString() == "intNumCount")
                {
                    drFilter["CSort"] = 0;
                    drFilter["CSortOrder"] = 1;
                    drFilter["CWhere"] = ">= 9,<= 11";
                }
                if (drFilter["CName"].ToString() == "lngMethodSN")
                {
                    drFilter["CSort"] = 0;
                    drFilter["CSortOrder"] = 2;
                }
                if (drFilter["CName"].ToString() == "lngSecFieldSN")
                {
                    drFilter["CSort"] = 0;
                    drFilter["CSortOrder"] = 3;
                }
                if (drFilter["CName"].ToString() == "strFreqNums")
                {
                    drFilter["CSort"] = 0;
                    drFilter["CSortOrder"] = 4;
                }
                if (drFilter["CName"].ToString() == "intHistoryTestPeriods")
                {
                    drFilter["CSort"] = 1;
                    drFilter["CSortOrder"] = 5;
                }
                if (drFilter["CName"].ToString() == "intHistoryHitRate")
                {
                    drFilter["CSort"] = 1;
                    drFilter["CSortOrder"] = 6;
                    drFilter["CWhere"] = ">= 0.55";
                }
                if (drFilter["CName"].ToString() == "intHitRateH")
                {
                    drFilter["CSort"] = 1;
                    drFilter["CSortOrder"] = 7;
                    drFilter["CWhere"] = "> 10";
                }
            }
        }

        protected void BtnAIFilterClick(object sender, EventArgs e)
        {
        }

        protected void BtnquickSearchOrderClick(object sender, EventArgs e)
        {
            if (Session[FreqResultTID + "dtQryFreqFilters"] != null)
            {
                if (string.IsNullOrEmpty(ViewState["select"].ToString()))
                {
                    SetdtFilter();
                }
                foreach (DataRow drFilter in DtFilter.Rows)
                {
                    if (drFilter["CName"].ToString() == "lngSearchMethodSN") { drFilter["CWhere"] = "none"; }
                }
                SetFilterViewState();
                foreach (int intSearchMethodSN in new List<int>() { 5, 20008, 20010, 2 })
                {
                    string strFilterExpresion = string.Format(InvariantCulture,
                                                "{0} AND [lngSearchMethodSN] = {1} ",
                                                ViewState["select"].ToString(), intSearchMethodSN);
                    string strSort = "[intNumCount] ASC ,[lngMethodSN] ASC ,[lngSecFieldSN] ASC ,[strFreqNums] ASC ,[intHistoryTestPeriods] DESC ,[intHistoryHitRate] DESC ,[intHitRateH] DESC";
                    DataTable dtFreqFilters = ((DataTable)Session[FreqResultTID + "dtQryFreqFilters"]).Select(strFilterExpresion, strSort).CopyToDataTable();
                    long MethodSNTemp = 0, SearchMethodSNTemp = 0, SecFieldSNTemp = 0;
                    int intCount = 0;
                    foreach (DataRow drRow in dtFreqFilters.Rows)
                    {
                        long MethodSN = long.Parse(drRow["lngMethodSN"].ToString(), InvariantCulture);
                        long SearchMethodSN = long.Parse(drRow["lngSearchMethodSN"].ToString(), InvariantCulture);
                        long SecFieldSN = long.Parse(drRow["lngSecFieldSN"].ToString(), InvariantCulture);
                        float HistoryTestPeriods = float.Parse(drRow["intHistoryTestPeriods"].ToString(), InvariantCulture);

                        if (HistoryTestPeriods == 20)
                        {
                            MethodSNTemp = MethodSN;
                            SearchMethodSNTemp = SearchMethodSN;
                            SecFieldSNTemp = SecFieldSN;
                            intCount = 1;
                        }
                        else
                        {
                            if (MethodSN == MethodSNTemp && SearchMethodSN == SearchMethodSNTemp && SecFieldSN == SecFieldSNTemp)
                            {
                                intCount++;
                            }
                            else
                            {
                                intCount = 0;
                            }
                        }

                        if (intCount == 3)
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngMethodSN = MethodSN;
                            stuGLSearchTemp = new CglMethod().GetMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSearchMethodSN = SearchMethodSN;
                            stuGLSearchTemp = new CglMethod().GetSearchMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSecFieldSN = SecFieldSN;
                            stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.HistoryTestPeriods = HistoryTestPeriods;
                            string RequestID = SetRequestId(stuGLSearchTemp);
                            if (stuGLSearchTemp.LngSecFieldSN == 1)
                            {
                                SetFrtSearchOrder(stuGLSearchTemp, Properties.Resources.SessionsFreqResultSingle, RequestID, Properties.Resources.PageFreqResultSingle, LocalIP, LocalBrowserType);
                            }
                            else
                            {
                                SetFrtSearchOrder(stuGLSearchTemp, Properties.Resources.SessionsFreqActiveHSingle, RequestID, Properties.Resources.PageFreqActiveHSingle, LocalIP, LocalBrowserType);
                            }
                        }
                    }
                }

                btnShowFRTSearchOrder.Visible = ((DataSet)ServerOption[string.Format(InvariantCulture, "{0}#{1}#dtFrtSearchOrder", LocalIP, LocalBrowserType)]).Tables["FrtSearchOrder"].Rows.Count > 0;
            }
        }

        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            Thread ThreadFreqActiveHT01 = (Thread)DicThreadFreqResultTT011[FreqResultTID + "T01"];
            if (ThreadFreqActiveHT01.ThreadState == ThreadState.Running)
            {
                //_pauseEventT1.Reset();
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Pause);
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT01.Suspend();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
            if (ThreadFreqActiveHT01.ThreadState == ThreadState.Suspended)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
                //_pauseEventT1.Set();
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT01.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
        }

        //---------------------------------------------------------------------------------------------------------
        protected void Timer1Tick(object sender, EventArgs e)
        {
            CheckThread();
        }

        private void CreatThread()
        {
            Timer1.Enabled = true;
            Thread ThreadFreqResultTT01 = new Thread(() =>
            {
                if (Session[FreqResultTID + "dtQryFreqFilters"] == null)
                {
                    new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
                    if (Session[FreqResultTID + "dtQryFreqFilters"] == null)
                    {
                        Session[FreqResultTID + "dtQryFreqFilters"] = CreatQryFreqFilters(_gstuSearch);
                    }
                    DtQryFreqFilters = (DataTable)Session[FreqResultTID + "dtQryFreqFilters"];
                    CreatFreqSum();
                    new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
                    ResetSearchOrder(FreqResultTID);
                }
            })
            {
                Name = FreqResultTID + "T01"
            };
            ThreadFreqResultTT01.Start();
            DicThreadFreqResultTT011.Add(FreqResultTID + "T01", ThreadFreqResultTT01);

        }



        private void CheckThread()
        {
            Thread ThreadFreqResultTT01 = (Thread)DicThreadFreqResultTT011[FreqResultTID + "T01"];

            if (ThreadFreqResultTT01.IsAlive)
            {
                //Timer1.Enabled = true;
                //Timer1.Interval = 20000;
                lblArgument.Text = string.Format(InvariantCulture, "{0}", DateTime.Now.ToLongTimeString());
                btnT1Start.Text = string.Format(InvariantCulture, "T1:{0} ", new GalaxyApp().GetTheadState(ThreadFreqResultTT01.ThreadState));
            }
            else
            {
                lblArgument.Visible = false;
                btnT1Start.Visible = false;
                Timer1.Enabled = false;
            }
        }

        //---------------------------------------------------------------------------------------------------------
        private void ReleaseMemory()
        {
            Session.Remove(FreqResultTID + "dtQryFreqFilters");
            Session.Remove(FreqResultTID + "dtFreqSum");
            Session.Remove(FreqResultTID + "dtFilter");

            if (DicThreadFreqResultTT011.Keys.Contains(FreqResultTID + "T01"))
            {
                Thread ThreadFreqActiveHT01 = (Thread)DicThreadFreqResultTT011[FreqResultTID + "T01"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActiveHT01.ThreadState == ThreadState.Suspended) { ThreadFreqActiveHT01.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT01.Abort();
                ThreadFreqActiveHT01.Join();
                DicThreadFreqResultTT011.Remove(FreqResultTID + "T01");
            }
        }
    }

    internal class PagerTemplate : ITemplate
    {
        private bool CheckData;
        public void InstantiateIn(Control container)
        {
            if (container == null) { throw new ArgumentNullException(nameof(container)); }

            Panel pnlPagerTemplate = CreatPanel("pnlPagerTemplate");
            pnlPagerTemplate.Controls.Add(CreatLabel("lblSelect", Properties.Resources.Select));
            DropDownList ddlPageIndex = CreatDropDownList("PageDropDownList", true);
            pnlPagerTemplate.Controls.Add(ddlPageIndex);
            Button btnPrev10 = CreatButton("btnPrev10", Properties.Resources.btnPrev10Page);
            pnlPagerTemplate.Controls.Add(btnPrev10);
            Button btnPrev = CreatButton("btnPrev", Properties.Resources.btnPrevPage);
            pnlPagerTemplate.Controls.Add(btnPrev);
            Label pageLabel = CreatLabel("CurrentPageLabel", "CurrentPageLabel");
            pnlPagerTemplate.Controls.Add(pageLabel);
            Button btnNext = CreatButton("btnNext", Properties.Resources.btnNextPage);
            pnlPagerTemplate.Controls.Add(btnNext);
            Button btnNext10 = CreatButton("btnNext10", Properties.Resources.btnNext10Page);
            pnlPagerTemplate.Controls.Add(btnNext10);
            container.Controls.Add(pnlPagerTemplate);
        }

        private Panel CreatPanel(string strId)
        {
            CheckData = true;
            Panel Return;
            using (Panel Temp = new Panel())
            {
                Temp.ID = strId;
                Return = Temp;
            }
            return Return;
        }

        private DropDownList CreatDropDownList(string strId, bool AutoPostBack)
        {
            CheckData = true;
            DropDownList Return;
            using (DropDownList Temp = new DropDownList())
            {
                Temp.ID = strId;
                Temp.AutoPostBack = AutoPostBack;

                Return = Temp;
            }
            return Return;
        }

        private Label CreatLabel(string strId, string strText)
        {
            CheckData = true;
            Label Return;

            using (Label Temp = new Label())
            {
                Temp.ID = strId;
                Temp.Text = strText;
                Return = Temp;
            }
            return Return;
        }

        private Button CreatButton(string strId, string strText)
        {
            CheckData = true;
            Button btReturn;
            using (Button button = new Button())
            {
                button.ID = strId;
                button.Text = strText;
                btReturn = button;
            }
            return CheckData ? btReturn : null;
        }

    }
}