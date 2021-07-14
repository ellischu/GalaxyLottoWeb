using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqActive01T : BasePage
    {
        private string _action;
        private string _requestId;
        private string FreqActive01TID;
        private StuGLSearch GlobalStuSearch;
        private GridView gvActiveWithSection;
        private static readonly Dictionary<string, int> _dicSectionLimit = new Dictionary<string, int> { { "05", 3 }, { "10", 4 }, { "25", 7 }, { "50", 13 }, { "100", 21 }, };

        private static Dictionary<string, object> DicThreadFreqActive01T
        {
            get { if (dicThreadFreqActive01T == null) { dicThreadFreqActive01T = new Dictionary<string, object>(); } return dicThreadFreqActive01T; }
            set => dicThreadFreqActive01T = value;
        }
        private static Dictionary<string, object> dicThreadFreqActive01T;

        private Thread Thread01;
        // ---------------------------------------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            SetupViewState();
            if (ViewState[FreqActive01TID + "_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                InitializeArgument();
                ShowResult();
            }
        }

        private void InitializeArgument()
        {
            if (ViewState[FreqActive01TID + "dicNumcssclass"] == null) { ViewState.Add(FreqActive01TID + "dicNumcssclass", new GalaxyApp().GetNumcssclass(GlobalStuSearch)); }
            if (ViewState[FreqActive01TID + "LstCurrentNums"] == null) { ViewState.Add(FreqActive01TID + "LstCurrentNums", (List<int>)new CglData().GetDataNumsLst(GlobalStuSearch)); }
            if (Session[FreqActive01TID + "lblT01"] == null) { Session[FreqActive01TID + "lblT01"] = string.Empty; }
            if (Session[FreqActive01TID + "sort"] == null) { Session.Add(FreqActive01TID + "sort", ""); }
            if (ViewState[FreqActive01TID + "ddlFields"] == null && Session[FreqActive01TID + "ddlFields"] != null) { ViewState[FreqActive01TID + "ddlFields"] = Session[FreqActive01TID + "ddlFields"]; }

        }

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            FreqActive01TID = _action + _requestId;

            if (ViewState[FreqActive01TID + "_gstuSearch"] == null)
            {
                if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqActive01TID] != null)
                {
                    ViewState.Add(FreqActive01TID + "_gstuSearch", (StuGLSearch)Session[FreqActive01TID]);
                }
            }
            GlobalStuSearch = (StuGLSearch)ViewState[FreqActive01TID + "_gstuSearch"];

            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowResult()
        {
            ShowTitle();
            ShowddlFields();
            ddlFields.Visible = false;
            pnlPercent.Visible = false;
            switch (ddlOption.SelectedValue)
            {
                case "OptionFreqActive01":
                    ddlFields.Visible = true;
                    ShowActiveWithSection();
                    ShowLastNumsWithFreqSec();
                    break;
                case "OptionPercent01":
                    ddlFields.Visible = true;
                    pnlPercent.Visible = true;
                    ShowPercent01();
                    break;
                case "OptionPercent02":
                    ddlFields.Visible = true;
                    pnlPercent.Visible = true;
                    ShowPercent02();
                    break;
                case "OptionSum":
                    ShowDataSum();
                    break;
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowPercent02()
        {
            if (Session[FreqActive01TID + "dsFreqSecHistory"] != null
                && ((DataSet)Session[FreqActive01TID + "dsFreqSecHistory"]).Tables.Contains(ddlFields.SelectedValue)
                && Session[FreqActive01TID + "dsActiveWithSection"] != null
                && ((DataSet)Session[FreqActive01TID + "dsActiveWithSection"]).Tables.Contains(ddlFields.SelectedValue))
            {
                pnlDetail.Controls.Clear();
                using DataTable dtPercent = GetPercentTable02(GlobalStuSearch, GetCompare());
                dtPercent.DefaultView.Sort = chkTotal.Checked ? "[TotalPercent] DESC" :
                                             string.Format(InvariantCulture, "[{0}Percent] DESC", ddlFields.SelectedValue);
                GridView gvPercentTable = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "gvPercentTable{0}", ddlFields.SelectedValue), "gltable", dtPercent, false, false);

                if (gvPercentTable.Columns.Count == 0)
                {
                    for (int i = 0; i < dtPercent.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = dtPercent.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(dtPercent.Columns[i].ColumnName, 1),
                            SortExpression = dtPercent.Columns[i].ColumnName,
                        };
                        bfCell.HeaderStyle.CssClass = bfCell.DataField;
                        bfCell.ItemStyle.CssClass = dtPercent.Columns[i].ColumnName.Contains("Percent") ? " Percent " : bfCell.DataField;
                        if (dtPercent.Columns[i].ColumnName == "TotalPercent") bfCell.ItemStyle.CssClass = bfCell.DataField;
                        bfCell.Visible = !dtPercent.Columns[i].ColumnName.Contains("Comp");
                        gvPercentTable.Columns.Add(bfCell);
                    }
                }

                gvPercentTable.RowDataBound += GvPercentTable_RowDataBound;
                gvPercentTable.DataBind();
                pnlDetail.Controls.Add(gvPercentTable);
            }
        }

        private DataTable GetPercentTable02(StuGLSearch stuGLSearch, List<string> lstCompare)
        {
            using DataTable dtReturn = new DataTable { Locale = InvariantCulture, TableName = "dtPercent" };
            dtReturn.Columns.Add(new DataColumn { ColumnName = "lngN", DataType = typeof(int), Unique = true });
            dtReturn.Columns.Add(new DataColumn { ColumnName = "TotalPercent", DataType = typeof(double), });

            for (int intNums = 1; intNums <= new CglDataSet(stuGLSearch.LottoType).LottoNumbers; intNums++)
            {
                double TotalPercent = 1;
                DataRow drReturn = dtReturn.NewRow();
                drReturn["lngN"] = intNums;
                foreach (string strItem in (List<string>)ViewState[FreqActive01TID + "ddlFields"])
                {
                    string Compare = string.Format(InvariantCulture, "{0}Comp", strItem);
                    if (!dtReturn.Columns.Contains(Compare))
                        dtReturn.Columns.Add(new DataColumn { ColumnName = Compare, DataType = typeof(string) });

                    string Percents = string.Format(InvariantCulture, "{0}Percent", strItem);
                    if (!dtReturn.Columns.Contains(Percents))
                        dtReturn.Columns.Add(new DataColumn { ColumnName = Percents, DataType = typeof(double), });
                    using DataTable dtFreqSec = ((DataSet)Session[FreqActive01TID + "dsActiveWithSection"]).Tables[strItem];
                    using DataTable dtFreqSecHistory = ((DataSet)Session[FreqActive01TID + "dsFreqSecHistory"]).Tables[strItem];

                    using DataTable dtLng = dtFreqSec.Select(string.Format(InvariantCulture, "[lngN] = {0}", intNums), string.Empty).CopyToDataTable();
                    drReturn[Compare] = StringComparer(dtLng, lstCompare);

                    using DataTable dtLngHistory = dtFreqSecHistory.Select(string.Format(InvariantCulture, "[lngN] = {0}", intNums), string.Empty).CopyToDataTable();
                    double dblPercent = 1;
                    foreach (string item in lstCompare)
                    {
                        dblPercent *= GetFrequancy02(dtLng, item, dtLngHistory);
                    }
                    drReturn[Percents] = Math.Round(dblPercent * Math.Pow(100d, lstCompare.Count), 2);

                    TotalPercent = dblPercent > 0 ? TotalPercent * dblPercent : TotalPercent * 0.00001;
                }
                drReturn["TotalPercent"] = Math.Round(TotalPercent * Math.Pow(100d, ddlFields.Items.Count), 2);
                dtReturn.Rows.Add(drReturn);
            }
            return dtReturn;
        }

        private static double GetFrequancy02(DataTable dtLng, string item, DataTable dtLngHistory)
        {
            List<string> lstField = new List<string> { "sglAC05", "sglAC10", "sglAC25", "sglAC50", "sglAC100" };
            string filterExpression;
            if (lstField.Contains(item))
            {
                filterExpression = (double.Parse(dtLng.Rows[0][item].ToString(), InvariantCulture) > 0 ?
                                  string.Format(InvariantCulture, "[{0}] > 0 ", item) :
                                  string.Format(InvariantCulture, "[{0}] < 0 ", item));
            }
            else
            {
                filterExpression = string.Format(InvariantCulture, "[{0}] = {1}", item, dtLng.Rows[0][item]);
            }
            using DataTable dtLngHistoryHit = dtLngHistory.Select(filterExpression, string.Empty).Length > 0 ?
                                              dtLngHistory.Select(filterExpression, string.Empty).CopyToDataTable() :
                                              dtLngHistory.Clone();

            return dtLngHistory.Rows.Count == 0 ? 0 : double.Parse(dtLngHistoryHit.Rows.Count.ToString(InvariantCulture), InvariantCulture) / double.Parse(dtLngHistory.Rows.Count.ToString(InvariantCulture), InvariantCulture);
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowPercent01()
        {
            if (Session[FreqActive01TID + "dsFreqSecHistory"] != null
                && ((DataSet)Session[FreqActive01TID + "dsFreqSecHistory"]).Tables.Contains(ddlFields.SelectedValue)
                && Session[FreqActive01TID + "dsActiveWithSection"] != null
                && ((DataSet)Session[FreqActive01TID + "dsActiveWithSection"]).Tables.Contains(ddlFields.SelectedValue))
            {
                pnlDetail.Controls.Clear();
                using DataTable dtPercent = GetPercentTable01(GlobalStuSearch, GetCompare());
                dtPercent.DefaultView.Sort = chkTotal.Checked ? "[TotalPercent] DESC" :
                                             string.Format(InvariantCulture, "[{0}Percent] DESC", ddlFields.SelectedValue);
                GridView gvPercentTable = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "gvPercentTable{0}", ddlFields.SelectedValue), "gltable", dtPercent, false, false);

                if (gvPercentTable.Columns.Count == 0)
                {
                    for (int i = 0; i < dtPercent.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = dtPercent.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(dtPercent.Columns[i].ColumnName, 1),
                            SortExpression = dtPercent.Columns[i].ColumnName,
                        };
                        bfCell.HeaderStyle.CssClass = bfCell.DataField;
                        bfCell.ItemStyle.CssClass = dtPercent.Columns[i].ColumnName.Contains("Percent") ? " Percent " : bfCell.DataField;
                        if (dtPercent.Columns[i].ColumnName == "TotalPercent") bfCell.ItemStyle.CssClass = bfCell.DataField;
                        bfCell.Visible = !dtPercent.Columns[i].ColumnName.Contains("Comp");
                        gvPercentTable.Columns.Add(bfCell);
                    }
                }

                gvPercentTable.RowDataBound += GvPercentTable_RowDataBound;
                gvPercentTable.DataBind();
                pnlDetail.Controls.Add(gvPercentTable);
            }
        }

        private void GvPercentTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Dictionary<string, DataControlFieldCell> dicCells = new Dictionary<string, DataControlFieldCell>();
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        dicCells.Add(strCell_ColumnName, cell);
                    }
                }

                #region Set lngN
                if (((List<int>)ViewState[FreqActive01TID + "LstCurrentNums"]).Contains(int.Parse(dicCells["lngN"].Text, InvariantCulture)))
                {
                    dicCells["lngN"].CssClass = ((Dictionary<string, string>)ViewState[FreqActive01TID + "dicNumcssclass"])[dicCells["lngN"].Text] + " ";
                    e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit ";
                }
                #endregion Set lngN

                if ((e.Row.RowIndex + 1) % 5 == 0)
                {
                    e.Row.CssClass = e.Row.CssClass + " glRow5 ";
                }
            }
        }

        private DataTable GetPercentTable01(StuGLSearch stuGLSearch, List<string> lstCompare)
        {

            using DataTable dtReturn = new DataTable { Locale = InvariantCulture, TableName = "dtPercent" };
            dtReturn.Columns.Add(new DataColumn { ColumnName = "lngN", DataType = typeof(int), Unique = true });
            dtReturn.Columns.Add(new DataColumn { ColumnName = "TotalPercent", DataType = typeof(double), });

            for (int intNums = 1; intNums <= new CglDataSet(stuGLSearch.LottoType).LottoNumbers; intNums++)
            {
                double TotalPercent = 1;
                //double GenRows = 100;
                DataRow drReturn = dtReturn.NewRow();
                drReturn["lngN"] = intNums;
                foreach (string strItem in (List<string>)ViewState[FreqActive01TID + "ddlFields"])
                {
                    string Compare = string.Format(InvariantCulture, "{0}Comp", strItem);
                    if (!dtReturn.Columns.Contains(Compare))
                        dtReturn.Columns.Add(new DataColumn { ColumnName = Compare, DataType = typeof(string) });

                    string Rows = string.Format(InvariantCulture, "{0}Rows", strItem);
                    if (!dtReturn.Columns.Contains(Rows))
                        dtReturn.Columns.Add(new DataColumn { ColumnName = Rows, DataType = typeof(int), });

                    string Hits = string.Format(InvariantCulture, "{0}Hits", strItem);
                    if (!dtReturn.Columns.Contains(Hits))
                        dtReturn.Columns.Add(new DataColumn { ColumnName = Hits, DataType = typeof(int), });

                    string Percents = string.Format(InvariantCulture, "{0}Percent", strItem);
                    if (!dtReturn.Columns.Contains(Percents))
                        dtReturn.Columns.Add(new DataColumn { ColumnName = Percents, DataType = typeof(double), });

                    using DataTable dtFreqSec = ((DataSet)Session[FreqActive01TID + "dsActiveWithSection"]).Tables[strItem];
                    using DataTable dtFreqSecHistory = ((DataSet)Session[FreqActive01TID + "dsFreqSecHistory"]).Tables[strItem];

                    using DataTable dtLng = dtFreqSec.Select(string.Format(InvariantCulture, "[lngN] = {0}", intNums), string.Empty).CopyToDataTable();
                    using DataTable dtLngHistory = dtFreqSecHistory.Select(string.Format(InvariantCulture, "[lngN] = {0}", intNums), string.Empty).CopyToDataTable();
                    string filterExpression = StringComparer(dtLng, lstCompare);
                    using DataTable dtLngHistoryHit = dtLngHistory.Select(filterExpression, string.Empty).Length > 0 ?
                                                      dtLngHistory.Select(filterExpression, string.Empty).CopyToDataTable() :
                                                      dtLngHistory.Clone();
                    drReturn[Compare] = filterExpression;
                    drReturn[Rows] = dtLngHistory.Rows.Count;
                    //if (strItem == "gen") GenRows = double.Parse(dtLngHistory.Rows.Count.ToString(InvariantCulture), InvariantCulture);
                    drReturn[Hits] = dtLngHistoryHit.Rows.Count;
                    double dblPercent = dtLngHistory.Rows.Count == 0 ? 0 : double.Parse(dtLngHistoryHit.Rows.Count.ToString(InvariantCulture), InvariantCulture) / double.Parse(dtLngHistory.Rows.Count.ToString(InvariantCulture), InvariantCulture);
                    drReturn[Percents] = Math.Round(dblPercent * 100d, 3);
                    TotalPercent = dblPercent > 0 ? TotalPercent * dblPercent : TotalPercent * 0.00001;
                    //TotalPercent = dblPercent > 0 ? TotalPercent * double.Parse(dtLngHistory.Rows.Count.ToString(InvariantCulture), InvariantCulture) / GenRows * dblPercent : TotalPercent * 0.001;
                }
                drReturn["TotalPercent"] = Math.Round(TotalPercent * Math.Pow(100d, ddlFields.Items.Count), 3);
                dtReturn.Rows.Add(drReturn);
            }
            return dtReturn;
        }

        private static string StringComparer(DataTable dtLng, List<string> lstCompare)
        {
            List<string> lstReturn = new List<string>();
            List<string> lstField = new List<string> { "sglAC05", "sglAC10", "sglAC25", "sglAC50", "sglAC100" };
            foreach (string ColumnName in lstCompare)
            {
                if (lstField.Contains(ColumnName))
                {
                    lstReturn.Add(double.Parse(dtLng.Rows[0][ColumnName].ToString(), InvariantCulture) > 0 ?
                                  string.Format(InvariantCulture, "[{0}] > 0 ", ColumnName) :
                                  string.Format(InvariantCulture, "[{0}] < 0 ", ColumnName));
                }
                else
                {
                    lstReturn.Add(string.Format(InvariantCulture, "[{0}] = {1}", ColumnName, dtLng.Rows[0][ColumnName]));
                }

            }
            return string.Join(" AND ", lstReturn.ToArray());
        }

        private List<string> GetCompare()
        {
            Dictionary<string, CheckBox> dicField = new Dictionary<string, CheckBox>
            {
                { "lngM", chkLngM },
                { "sglFreq05", chksglFreq05 },
                { "sglAC05", chksglAC05 },
                { "sglFreq10", chksglFreq10 },
                { "sglAC10", chksglAC10 },
                { "sglFreq25", chksglFreq25 },
                { "sglAC25", chksglAC25 },
                { "sglFreq50", chksglFreq50 },
                { "sglAC50", chksglAC50 },
                { "sglFreq100", chksglFreq100 },
                { "sglAC100", chksglAC100 }
            };
            List<string> lstReturn = new List<string>();
            foreach (KeyValuePair<string, CheckBox> item in dicField)
            {
                if (item.Value.Checked)
                {
                    lstReturn.Add(item.Key);
                    item.Value.ForeColor = Color.Yellow;
                }
                else
                {
                    item.Value.ForeColor = Color.White;
                }
            }
            if (lstReturn.Count == 0) { lstReturn.Add("sglFreq05"); chksglFreq05.Checked = true; chksglFreq05.ForeColor = Color.Yellow; }
            return lstReturn;
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowDataSum()
        {
            if (Session[FreqActive01TID + "dsDataSum"] != null && ((DataSet)Session[FreqActive01TID + "dsDataSum"]).Tables.Contains("LastNumsWithSection"))
            {
                pnlDetail.Controls.Clear();
                ResetSearchOrder(FreqActive01TID);
                #region DataNSum
                Panel pnlDataSum = new GalaxyApp().CreatPanel("pnlDataSum", "max-width");
                pnlDetail.Controls.Add(pnlDataSum);

                //Label
                Label lblLastNumsWithSectionSum = new GalaxyApp().CreatLabel("lblLastNumsWithSection", "末期號碼統計");
                pnlDataSum.Controls.Add(lblLastNumsWithSectionSum);
                //Datatable
                GridView gvDataSum = new GalaxyApp().CreatGridView("gvDataSum", "gltable",
                                                    ((DataSet)base.Session[FreqActive01TID + "dsDataSum"]).Tables["LastNumsWithSection"], true, false);
                foreach (DataControlField dcColumn in gvDataSum.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if (((Dictionary<string, string>)ViewState[FreqActive01TID + "dicNumcssclass"]).Keys.Contains(strColumnName.Substring(4)))
                    {
                        dcColumn.HeaderStyle.CssClass = ((Dictionary<string, string>)ViewState[FreqActive01TID + "dicNumcssclass"])[strColumnName.Substring(4)];
                    }
                }
                gvDataSum.ShowHeaderWhenEmpty = true;
                gvDataSum.DataBind();
                pnlDataSum.Controls.Add(gvDataSum);

                #endregion DataNSum
            }
        }
        // ---------------------------------------------------------------------------------------------------------
        private void ShowActiveWithSection()
        {
            if (Session[FreqActive01TID + "dsActiveWithSection"] != null && ((DataSet)Session[FreqActive01TID + "dsActiveWithSection"]).Tables.Contains(ddlFields.SelectedValue))
            {
                pnlDetail.Controls.Clear();
                Panel pnlActiveWithSection = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlActiveWithSection{0}", ddlFields.SelectedValue), "max-width");
                pnlDetail.Controls.Add(pnlActiveWithSection);
                using DataTable dtActiveWithSection = ((DataSet)Session[FreqActive01TID + "dsActiveWithSection"]).Tables[ddlFields.SelectedValue];
                if (dtActiveWithSection.Columns.Contains("lngTotalSN")) { dtActiveWithSection.Columns.Remove("lngTotalSN"); }
                if (dtActiveWithSection.Columns.Contains("lngMethodSN")) { dtActiveWithSection.Columns.Remove("lngMethodSN"); }
                if (dtActiveWithSection.Columns.Contains("lngDateSN")) { dtActiveWithSection.Columns.Remove("lngDateSN"); }
                if (dtActiveWithSection.Columns.Contains("lngFreqSecSN")) { dtActiveWithSection.Columns.Remove("lngFreqSecSN"); }
                dtActiveWithSection.DefaultView.Sort = (string)Session[FreqActive01TID + "sort"];
                //dtActiveWithSection.DefaultView.Sort = "[lngN] ASC";
                gvActiveWithSection = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "gvActiveWithSection{0}", ddlFields.SelectedValue), "gltable", dtActiveWithSection, false, false);

                if (gvActiveWithSection.Columns.Count == 0)
                {
                    for (int i = 0; i < dtActiveWithSection.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = dtActiveWithSection.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(dtActiveWithSection.Columns[i].ColumnName, 1),
                            SortExpression = dtActiveWithSection.Columns[i].ColumnName,
                        };
                        bfCell.HeaderStyle.CssClass = bfCell.DataField;
                        bfCell.ItemStyle.CssClass = bfCell.DataField;
                        gvActiveWithSection.Columns.Add(bfCell);
                    }
                }
                gvActiveWithSection.AllowSorting = true;
                gvActiveWithSection.RowDataBound += GvActiveWithSection_RowDataBound;
                gvActiveWithSection.Sorting += GvActiveWithSection_Sorting;
                gvActiveWithSection.DataBind();
                pnlActiveWithSection.Controls.Add(gvActiveWithSection);
                pnlDetail.Controls.Add(new Label
                {
                    Text = "<script type='text/javascript'>playSound('SoundFinish');</script>"
                });
            }

        }

        private void GvActiveWithSection_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e)); }
            if (ViewState[e.SortExpression] == null)
            {
                ViewState[e.SortExpression] = true;
            }
            else
            {
                ViewState[e.SortExpression] = !(bool)ViewState[e.SortExpression];
            }
            string Sort = string.Format(InvariantCulture, "[{0}] {1} , [lngN] ASC ", e.SortExpression, (bool)ViewState[e.SortExpression] ? "ASC" : "DESC");


            //string sort = string.Format(InvariantCulture, "{0} {1}", e.SortExpression, GetSortDirection(e.SortExpression));
            Session[FreqActive01TID + "sort"] = Sort;
            using DataTable dtActiveWithSection = ((DataSet)Session[FreqActive01TID + "dsActiveWithSection"]).Tables[ddlFields.SelectedValue];
            if (dtActiveWithSection.Columns.Contains("lngTotalSN")) { dtActiveWithSection.Columns.Remove("lngTotalSN"); }
            if (dtActiveWithSection.Columns.Contains("lngMethodSN")) { dtActiveWithSection.Columns.Remove("lngMethodSN"); }
            if (dtActiveWithSection.Columns.Contains("lngDateSN")) { dtActiveWithSection.Columns.Remove("lngDateSN"); }
            if (dtActiveWithSection.Columns.Contains("lngFreqSecSN")) { dtActiveWithSection.Columns.Remove("lngFreqSecSN"); }
            dtActiveWithSection.DefaultView.Sort = (string)Session[FreqActive01TID + "sort"];
            gvActiveWithSection.DataSource = dtActiveWithSection.DefaultView;
            gvActiveWithSection.DataBind();
        }

        private void GvActiveWithSection_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Dictionary<string, DataControlFieldCell> dicCells = new Dictionary<string, DataControlFieldCell>();
            Dictionary<int, double> dicSectionPercent = new Dictionary<int, double> { { 5, 0.9 }, { 10, 0.75 }, { 25, 0.65 }, { 50, 0.8 }, { 100, 0.75 } };
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        dicCells.Add(strCell_ColumnName, cell);
                    }
                }
                e.Row.ToolTip = dicCells["lngN"].Text;

                #region Set lngN
                if (((List<int>)ViewState[FreqActive01TID + "LstCurrentNums"]).Contains(int.Parse(dicCells["lngN"].Text, InvariantCulture)) && ckAns.Checked)
                {
                    dicCells["lngN"].CssClass = ((Dictionary<string, string>)ViewState[FreqActive01TID + "dicNumcssclass"])[dicCells["lngN"].Text] + " ";
                    e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit ";
                }
                #endregion Set lngN

                #region  Set lngM
                if (int.Parse(dicCells["lngM"].Text, InvariantCulture) >= int.Parse(dicCells["intAvgM"].Text, InvariantCulture))
                {
                    dicCells["intAvgM"].CssClass += " BackGroundBlue ";
                }
                if (int.Parse(dicCells["lngM"].Text, InvariantCulture) >= int.Parse(dicCells["intMaxM"].Text, InvariantCulture))
                {
                    dicCells["intMaxM"].CssClass += " BackGroundBlue ";
                }
                #endregion Set lngM

                foreach (int section in new int[] { 5, 10, 25, 50, 100 })
                {
                    #region sglAC
                    string strCell_ColumnNameAC = string.Format(InvariantCulture, "sglAC{0:d2}", section);
                    if (double.Parse(dicCells[strCell_ColumnNameAC].Text, InvariantCulture) < 0)
                    {
                        dicCells[strCell_ColumnNameAC].CssClass = strCell_ColumnNameAC + " glValueMax ";
                    }
                    #endregion

                    string strCell_ColumnNameFreq = string.Format(InvariantCulture, "sglFreq{0:d2}", section);
                    #region sglFreq
                    if (int.Parse(dicCells[strCell_ColumnNameFreq].Text, InvariantCulture) >= _dicSectionLimit[string.Format(InvariantCulture, "{0:d2}", section)] &&
                        int.Parse(dicCells[strCell_ColumnNameFreq].Text, InvariantCulture) == int.Parse(dicCells[string.Format(InvariantCulture, "intMax{0:d2}", section)].Text, InvariantCulture))
                    {
                        dicCells[strCell_ColumnNameFreq].CssClass += " glValueActive ";
                    }
                    #endregion

                    string strCell_ColumnNameMin = string.Format(InvariantCulture, "intMin{0:d2}", section);
                    #region intMin

                    if (((Dictionary<string, List<int>>)((Dictionary<string, object>)Session[FreqActive01TID + "dicLastNumsWithSectiont"])[ddlFields.SelectedValue])[string.Format(InvariantCulture, "{0:d2}", section)].Contains(int.Parse(dicCells["lngN"].Text, InvariantCulture)))
                    {
                        dicCells[strCell_ColumnNameMin].CssClass = strCell_ColumnNameMin + " glSectionNumLast ";
                    }
                    #endregion

                    string strCell_ColumnNameMax = string.Format(InvariantCulture, "intMax{0:d2}", section);
                    #region intMax

                    if (section == 5)
                    {
                        if (int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", section)].Text, InvariantCulture) == int.Parse(dicCells[strCell_ColumnNameMax].Text, InvariantCulture))
                        {
                            dicCells[strCell_ColumnNameMax].CssClass = strCell_ColumnNameMax + " glValueSecondMaxNum ";
                        }
                    }

                    if (section == 10)
                    {
                        if (int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", section)].Text, InvariantCulture) == int.Parse(dicCells[strCell_ColumnNameMax].Text, InvariantCulture))
                        {
                            dicCells[strCell_ColumnNameMax].CssClass = strCell_ColumnNameMax + " glValueSecondMaxNum ";
                        }
                    }

                    if (section == 25)
                    {
                        if (int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", section)].Text, InvariantCulture) * 2 > int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", 50)].Text, InvariantCulture))
                        {
                            dicCells[strCell_ColumnNameFreq].CssClass = dicCells[strCell_ColumnNameFreq].CssClass + strCell_ColumnNameFreq + " glValueMinNum ";
                        }
                        if (int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", section)].Text, InvariantCulture) == int.Parse(dicCells[strCell_ColumnNameMax].Text, InvariantCulture))
                        {
                            dicCells[strCell_ColumnNameMax].CssClass = strCell_ColumnNameMax + " glValueSecondMaxNum ";
                        }
                    }

                    if (section == 50)
                    {
                        if (int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", section)].Text, InvariantCulture) == int.Parse(dicCells[strCell_ColumnNameMax].Text, InvariantCulture))
                        {
                            if (int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", section)].Text, InvariantCulture) > 10)
                            {
                                dicCells[strCell_ColumnNameMax].CssClass = strCell_ColumnNameMax + " glValueMaxNum  glValueActive ";
                            }
                            else
                            {
                                dicCells[strCell_ColumnNameMax].CssClass = strCell_ColumnNameMax + " glValueSecondMaxNum ";
                            }
                        }
                        if (int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", section)].Text, InvariantCulture) * 2 > int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", 100)].Text, InvariantCulture))
                        {
                            dicCells[strCell_ColumnNameFreq].CssClass = dicCells[strCell_ColumnNameFreq].CssClass + strCell_ColumnNameFreq + " glValueMinNum ";
                        }
                    }

                    if (section == 100)
                    {
                        if (int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", section)].Text, InvariantCulture) == int.Parse(dicCells[strCell_ColumnNameMax].Text, InvariantCulture))
                        {
                            if (int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", section)].Text, InvariantCulture) >= 19)
                            {
                                dicCells[strCell_ColumnNameMax].CssClass = strCell_ColumnNameMax + " glValueMaxNum  glValueActive ";
                            }
                            else
                            {
                                dicCells[strCell_ColumnNameMax].CssClass = strCell_ColumnNameMax + " glValueSecondMaxNum ";
                            }
                        }
                    }

                    #endregion

                    if (BreakPoint(dicCells[strCell_ColumnNameFreq].Text, dicCells[strCell_ColumnNameMax].Text) >= dicSectionPercent[section] &&
                        double.Parse(dicCells[strCell_ColumnNameAC].Text, InvariantCulture) < 0)
                    {
                        dicCells[strCell_ColumnNameMin].CssClass += " BackGroundGreen ";
                    }
                }

                string strCell_ColumnNameAC05 = string.Format(InvariantCulture, "sglAC{0:d2}", 5);
                string strCell_ColumnNameAC10 = string.Format(InvariantCulture, "sglAC{0:d2}", 10);
                string strCell_ColumnNameAC25 = string.Format(InvariantCulture, "sglAC{0:d2}", 25);
                string strCell_ColumnNameAC50 = string.Format(InvariantCulture, "sglAC{0:d2}", 50);
                string strCell_ColumnNameAC100 = string.Format(InvariantCulture, "sglAC{0:d3}", 100);

                if (chkFullup.Checked &&
                    double.Parse(dicCells[strCell_ColumnNameAC05].Text, InvariantCulture) > 0 &&
                    double.Parse(dicCells[strCell_ColumnNameAC10].Text, InvariantCulture) > 0 &&
                    double.Parse(dicCells[strCell_ColumnNameAC25].Text, InvariantCulture) > 0 &&
                    double.Parse(dicCells[strCell_ColumnNameAC50].Text, InvariantCulture) > 0 &&
                    double.Parse(dicCells[strCell_ColumnNameAC100].Text, InvariantCulture) > 0
                    )
                {
                    e.Row.CssClass = e.Row.CssClass + " fullup ";
                }
            }
        }

        private static double BreakPoint(string text1, string text2)
        {
            return double.Parse(text1, InvariantCulture) / double.Parse(text2, InvariantCulture);
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowLastNumsWithFreqSec()
        {
            if (Session[FreqActive01TID + "dsLastNumsWithSectiont"] != null && ((DataSet)Session[FreqActive01TID + "dsLastNumsWithSectiont"]).Tables.Contains(ddlFields.SelectedValue))
            {
                Panel pnlLastNumsWithSectiont = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlLastNumsWithSectiont{0}", ddlFields.SelectedValue), "max-width");
                pnlDetail.Controls.Add(pnlLastNumsWithSectiont);


                DataTable DtLastNums = ((DataSet)Session[FreqActive01TID + "dsLastNumsWithSectiont"]).Tables[ddlFields.SelectedValue];

                GridView gvLastNumsWithSection = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "gvLastNumsWithSection{0}", ddlFields.SelectedValue), "gltable", DtLastNums, false, false);

                if (gvLastNumsWithSection.Columns.Count == 0)
                {
                    for (int i = 0; i < DtLastNums.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtLastNums.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtLastNums.Columns[i].ColumnName, 1),
                            SortExpression = DtLastNums.Columns[i].ColumnName,
                        };
                        gvLastNumsWithSection.Columns.Add(bfCell);
                    }
                }


                foreach (DataControlField dcColumn in gvLastNumsWithSection.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if (((Dictionary<string, string>)ViewState[FreqActive01TID + "dicNumcssclass"]).Keys.Contains(strColumnName.Substring(4)))
                    {
                        dcColumn.HeaderStyle.CssClass = ((Dictionary<string, string>)ViewState[FreqActive01TID + "dicNumcssclass"])[strColumnName.Substring(4)];
                    }
                }
                gvLastNumsWithSection.DataBind();
                pnlLastNumsWithSectiont.Controls.Add(gvLastNumsWithSection);
            }
        }


        // ---------------------------------------------------------------------------------------------------------

        private void ShowddlFields()
        {
            if (Session[FreqActive01TID + "ddlFields"] != null)
            {
                if (ddlFields.Items.Count == 0)
                {
                    foreach (string strItem in (List<string>)Session[FreqActive01TID + "ddlFields"])
                    {
                        ListItem listItem = new ListItem(new CglFunc().ConvertFieldNameId(strItem), strItem);
                        ddlFields.Items.Add(listItem);
                    }
                }
            }
            else
            {
                if (!DicThreadFreqActive01T.Keys.Contains(FreqActive01TID + "T01")) { CreatThread(); }
            }
        }

        private void ShowTitle()
        {
            if (ViewState[FreqActive01TID + "title"] == null) { ViewState.Add(FreqActive01TID + "title", string.Format(InvariantCulture, "{0}:{1}", "活性表01總表", new CglDBData().SetTitleString(GlobalStuSearch))); }
            Page.Title = (string)ViewState[FreqActive01TID + "title"];
            lblTitle.Text = (string)ViewState[FreqActive01TID + "title"];

            if (ViewState[FreqActive01TID + "lblMethod"] == null) { ViewState.Add(FreqActive01TID + "lblMethod", new CglMethod().SetMethodString(GlobalStuSearch)); }
            lblMethod.Text = (string)ViewState[FreqActive01TID + "lblMethod"];

            if (ViewState[FreqActive01TID + "lblSearchMethod"] == null) { ViewState.Add(FreqActive01TID + "lblSearchMethod", new CglMethod().SetSearchMethodString(GlobalStuSearch)); }
            lblSearchMethod.Text = (string)ViewState[FreqActive01TID + "lblSearchMethod"];

            //顯示當前資料
            if (ViewState[FreqActive01TID + "CurrentData"] == null) { ViewState.Add(FreqActive01TID + "CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(GlobalStuSearch))); }
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState[FreqActive01TID + "CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
        }

        // ---------------------------------------------------------------------------------------------------------


        protected void BtnCloseClick(object sender, EventArgs e)
        {
            ReleaseMemory();
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
        }

        private void ReleaseMemory()
        {
            ViewState.Remove("_gstuSearch");
            ViewState.Remove(FreqActive01TID + "dicNumcssclass");
            ViewState.Remove(FreqActive01TID + "LstCurrentNums");

            Session.Remove(FreqActive01TID + "ddlFields");
            Session.Remove(FreqActive01TID + "lblT01");
            Session.Remove(FreqActive01TID + "dsActiveWithSection");
            Session.Remove(FreqActive01TID + "dsFreqSecHistory");
            Session.Remove(FreqActive01TID + "dsLastNumsWithSectiont");
            Session.Remove(FreqActive01TID + "dicLastNumsWithSectiont");
            Session.Remove(FreqActive01TID + "dsDataSum");
            Session.Remove(FreqActive01TID);
            ResetSearchOrder(FreqActive01TID);
            if (DicThreadFreqActive01T.Keys.Contains(FreqActive01TID + "T01"))
            {
                Thread ThreadFreqActive01 = (Thread)DicThreadFreqActive01T[FreqActive01TID + "T01"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActive01.ThreadState == ThreadState.Suspended) { ThreadFreqActive01.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActive01.Abort();
                ThreadFreqActive01.Join();
                DicThreadFreqActive01T.Remove(FreqActive01TID + "T01");
            }
        }

        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            if (DicThreadFreqActive01T != null && DicThreadFreqActive01T.Keys.Contains(FreqActive01TID + "T01"))
            {
                Thread ThreadFreqActive01 = (Thread)DicThreadFreqActive01T[FreqActive01TID + "T01"];
                if (ThreadFreqActive01.ThreadState == ThreadState.Running)
                {
                    new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Pause, 0.2);

#pragma warning disable CS0618 // 類型或成員已經過時
                    ThreadFreqActive01.Suspend();
#pragma warning restore CS0618 // 類型或成員已經過時

                }
                if (ThreadFreqActive01.ThreadState == ThreadState.Suspended)
                {
                    new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start, 0.2);

#pragma warning disable CS0618 // 類型或成員已經過時
                    ThreadFreqActive01.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時

                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void Timer1Tick(object sender, EventArgs e)
        {
            CheckThread();
        }

        private void CheckThread()
        {
            if (DicThreadFreqActive01T.Keys.Contains(FreqActive01TID + "T01"))
            {
                Thread01 = (Thread)DicThreadFreqActive01T[FreqActive01TID + "T01"];
                if (Thread01.IsAlive)
                {
                    lblArgument.Visible = true;
                    lblArgument.Text = string.Format(InvariantCulture, "{0}", DateTime.Now.ToLongTimeString());
                    btnT1Start.Visible = true;
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0} {1} ", Session[FreqActive01TID + "lblT01"].ToString(), new GalaxyApp().GetTheadState(Thread01.ThreadState));
                }
                else
                {
                    ((Thread)DicThreadFreqActive01T[FreqActive01TID + "T01"]).Abort();
                    DicThreadFreqActive01T.Remove(FreqActive01TID + "T01");
                    lblArgument.Visible = false;
                    btnT1Start.Visible = false;
                    Timer1.Enabled = false;
                }
            }
        }

        private void CreatThread()
        {
            Timer1.Enabled = true;
            Thread01 = new Thread(() => { StartThread01(); ResetSearchOrder(FreqActive01TID); })
            {
                Name = FreqActive01TID + "T01"
            };
            Thread01.Start();
            DicThreadFreqActive01T.Add(FreqActive01TID + "T01", Thread01);
        }

        private void StartThread01()
        {
            //new GalaxyApp().PlaySound(GalaxyClass.SoundOption.Start);            
            Timer1.Enabled = true;
            List<string> Fields = (List<string>)new CglValidFields().GetValidFieldsLst(GlobalStuSearch);
            Session[FreqActive01TID + "ddlFields"] = Fields;
            using DataSet dsActiveWithSection = new DataSet() { Locale = InvariantCulture };
            using DataSet dsLastNumsWithSection = new DataSet() { Locale = InvariantCulture };
            using DataSet dsFreqSecHistory = new DataSet() { Locale = InvariantCulture };
            using DataSet dsDataSum = new DataSet() { Locale = InvariantCulture };

            Dictionary<string, object> dicLastNumsWithSectionAll = new Dictionary<string, object>();
            Dictionary<string, int> dicLastNumsSumZero = new Dictionary<string, int>();
            for (int num = 1; num <= new CglDataSet(GlobalStuSearch.LottoType).LottoNumbers; num++)
            {
                dicLastNumsSumZero.Add(string.Format(InvariantCulture, "lngN{0}", num), 0);
            }
            foreach (string strField in Fields)
            {
                StuGLSearch stuSearchTemp = GlobalStuSearch;
                stuSearchTemp.FieldMode = strField != "gen";
                stuSearchTemp.StrCompares = strField != "gen" ? strField : "gen";
                stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
                stuSearchTemp = new CglMethod().GetMethodSN(stuSearchTemp);
                stuSearchTemp = new CglMethod().GetSearchMethodSN(stuSearchTemp);
                stuSearchTemp = new CglMethod().GetSecFieldSN(stuSearchTemp);

                Session[FreqActive01TID + "lblT01"] = string.Format(InvariantCulture, "FreqActive01:{0}", new CglFunc().ConvertFieldNameId(strField));
                using DataTable dtFreqSec = new CglFreqSec().GetFreqSec(stuSearchTemp, CglDBFreqSec.TableName.QryFreqSec, SortOrder.Ascending);
                dtFreqSec.TableName = strField;
                dsActiveWithSection.Tables.Add(dtFreqSec);

                stuSearchTemp.InHistoryPeriods = stuSearchTemp.InFieldPeriodLimit;
                using DataTable dtFreqSecHistory = new CglDataFreqSecHis().GetDataFregSecHistoryMultiple(stuSearchTemp, CglDataFreqSecHis.TableName.QryDataFreqSecHistory, SortOrder.Ascending);
                dtFreqSecHistory.TableName = strField;
                dsFreqSecHistory.Tables.Add(dtFreqSecHistory);

                Session[FreqActive01TID + "lblT01"] = string.Format(InvariantCulture, "LastNumsWithFreqSec:{0}", new CglFunc().ConvertFieldNameId(strField));
                Dictionary<string, List<int>> dicLastNumsWithFreqSec = (Dictionary<string, List<int>>)new CglFreqSec().GetLastNumsWithFreqSec(stuSearchTemp);
                dicLastNumsWithSectionAll.Add(strField, dicLastNumsWithFreqSec);

                Dictionary<string, int> dicLastNumsSum = LastNumsSumDic(dicLastNumsWithFreqSec);
                foreach (KeyValuePair<string, int> num in dicLastNumsSum)
                {
                    if (num.Value == 0) { dicLastNumsSumZero[num.Key]++; };
                }
                DataTable dtLastNumsWithFreqSec = new CglFunc().CDicTOTable(dicLastNumsSum, null);
                dtLastNumsWithFreqSec.TableName = strField;
                dsLastNumsWithSection.Tables.Add(dtLastNumsWithFreqSec);
            }
            dicLastNumsSumZero = dicLastNumsSumZero.OrderByDescending(keySelector: x => x.Value).ToDictionary(keySelector: x => x.Key, elementSelector: x => x.Value);
            using (DataTable dtDataSum = new CglFunc().CDicTOTable(dicLastNumsSumZero, null))
            {
                dtDataSum.Columns.Add(new DataColumn()
                {
                    ColumnName = "Field",
                    DataType = typeof(string),
                    DefaultValue = string.Empty,
                });
                dtDataSum.Columns["Field"].SetOrdinal(0);
                foreach (string strField in Fields)
                {
                    DataRow drDataSum = dtDataSum.NewRow();
                    drDataSum["Field"] = new CglFunc().ConvertFieldNameId(strField);
                    foreach (DataColumn dataColumn in dsLastNumsWithSection.Tables[strField].Columns)
                    {
                        drDataSum[dataColumn.ColumnName] = dsLastNumsWithSection.Tables[strField].Rows[0][dataColumn.ColumnName];
                    }
                    dtDataSum.Rows.Add(drDataSum);
                }
                dtDataSum.TableName = "LastNumsWithSection";
                dsDataSum.Tables.Add(dtDataSum);
            }

            Session[FreqActive01TID + "dsActiveWithSection"] = dsActiveWithSection;
            Session[FreqActive01TID + "dsFreqSecHistory"] = dsFreqSecHistory;
            Session[FreqActive01TID + "dsLastNumsWithSectiont"] = dsLastNumsWithSection;
            Session[FreqActive01TID + "dicLastNumsWithSectiont"] = dicLastNumsWithSectionAll;
            Session[FreqActive01TID + "dsDataSum"] = dsDataSum;
            //new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish, 0.2);
        }

        private Dictionary<string, int> LastNumsSumDic(Dictionary<string, List<int>> dicLastNums)
        {
            Dictionary<int, int> dicLastNumsAll = new Dictionary<int, int>();
            for (int i = 1; i <= new CglDataSet(GlobalStuSearch.LottoType).LottoNumbers; i++)
            {
                dicLastNumsAll.Add(key: i, value: 0);
            }
            foreach (KeyValuePair<string, List<int>> KeyVal in dicLastNums)
            {
                foreach (int num in KeyVal.Value)
                {
                    dicLastNumsAll[num]++;
                }
            }
            dicLastNumsAll = dicLastNumsAll.OrderByDescending(keySelector: x => x.Value).ToDictionary(keySelector: x => x.Key, elementSelector: x => x.Value);
            #region Convert 
            Dictionary<string, int> dicLastNumTB = new Dictionary<string, int>();

            foreach (KeyValuePair<int, int> item in dicLastNumsAll)
            {
                dicLastNumTB.Add(string.Format(InvariantCulture, "lngN{0}", item.Key), item.Value);
            }
            #endregion
            return dicLastNumTB;
        }

        protected void ChangeLabelColor(object sender, EventArgs e)
        {
            if (sender is null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            CheckBox checkBox = (CheckBox)sender;
            checkBox.ForeColor = checkBox.Checked ? Color.Yellow : Color.White;
        }

    }
}