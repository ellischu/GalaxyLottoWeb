using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class MissingTotal : BasePage
    {
        //private Thread thread;
        private string _action;
        private string _requestId;
        private StuGLSearch GlobalStuSearch;
        private Dictionary<string, int> _dicCurrentNums;
        private Dictionary<string, object> _dicMissAll;
        private bool _blLngM0;

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }


            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[_action + _requestId] != null)
            {
                if (ViewState["_gstuSearch"] == null)
                {
                    ViewState.Add("_gstuSearch", (StuGLSearch)Session[_action + _requestId]);
                }
                else
                {
                    ViewState["_gstuSearch"] = (StuGLSearch)Session[_action + _requestId];
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
                GlobalStuSearch = (StuGLSearch)ViewState["_gstuSearch"];
                if (ViewState["MissAll"] == null) { ViewState.Add("MissAll", new CglMissAll().GetMissAll00Dic(GlobalStuSearch, CglMissAll.TableName.QryMissAll0001, SortOrder.Descending)); }
                if (ViewState["title"] == null) { ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "遺漏總表", new CglDBData().SetTitleString(GlobalStuSearch))); }
                if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(GlobalStuSearch))); }
                if (ViewState["CurrentNums"] == null) { ViewState.Add("CurrentNums", new CglData().GetDataNumsDici(GlobalStuSearch)); }
                _dicCurrentNums = (Dictionary<string, int>)ViewState["CurrentNums"];
                if (ViewState["lblMethod"] == null) { ViewState.Add("lblMethod", new CglMethod().SetMethodString(GlobalStuSearch)); }
                if (ViewState["lblSearchMethod"] == null) { ViewState.Add("lblSearchMethod", new CglMethod().SetSearchMethodString(GlobalStuSearch)); }

                _dicMissAll = (Dictionary<string, object>)ViewState["MissAll"];
                ShowResult(GlobalStuSearch);
            }
            ResetSearchOrder(_action + _requestId);
        }

        private void SetddlFreq()
        {
            if (ddlFreq.Items.Count == 0)
            {
                foreach (KeyValuePair<string, DataTable> keyval in (Dictionary<string, DataTable>)_dicMissAll["MissAll"])
                {
                    ListItem listItem = new ListItem(new CglFunc().ConvertFieldNameId(keyval.Key), keyval.Key);
                    if (keyval.Key == (string)ViewState["selectedValue"]) { listItem.Selected = true; }
                    ddlFreq.Items.Add(listItem);
                }
            }
        }

        private void ShowResult(StuGLSearch stuGLSearch)
        {
            Page.Title = (string)ViewState["title"];
            lblTitle.Text = (string)ViewState["title"];
            lblMethod.Text = (string)ViewState["lblMethod"];
            lblSearchMethod.Text = (string)ViewState["lblSearchMethod"];

            //顯示當前資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)base.ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            SetddlFreq();

            ShowDetail(stuGLSearch, ddlFreq.SelectedValue);

        }

        private void ShowDetail(StuGLSearch stuGLSearch, string selectedValue)
        {
            Panel pnlMiss = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlMissAll_{0}", selectedValue), "max-width");


            DataTable dtDataTable = ((Dictionary<string, DataTable>)_dicMissAll["MissAll"])[selectedValue];
            //DataTable dtDataTable = KeyVal.Value.Rows.Cast<DataRow>().Take(stuSearch00.IntDisplayPeriod).CopyToDataTable();
            pnlMiss.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "{0}lblMissAll", selectedValue), string.Format(InvariantCulture, "{0} MissAll({1}期)", selectedValue, dtDataTable.Rows.Count), "gllabel"));

            #region Convert 0 to -1 to -7
            for (int intRow = 0; intRow < dtDataTable.Rows.Count; intRow++)
            {
                int intIndexofZero = -1;
                for (int intNum = 1; intNum <= new CglDataSet(stuGLSearch.LottoType).LottoNumbers; intNum++)
                {
                    string strColName = string.Format(InvariantCulture, "lngM{0}", intNum);
                    if (int.Parse(dtDataTable.Rows[intRow][strColName].ToString(), InvariantCulture) == 0)
                    {
                        dtDataTable.Rows[intRow][strColName] = intIndexofZero;
                        intIndexofZero--;
                    }
                }
            }
            #endregion

            #region GridView 
            dtDataTable.DefaultView.Sort = chkOrder.Checked ? "lngTotalSN ASC" : "lngTotalSN DESC";
            GridView gvMissAll = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "{0} MissAll", selectedValue), "gltable table-hover", dtDataTable, true, false);
            gvMissAll.ShowHeaderWhenEmpty = true;
            #region Set Columns of DataGrid gvMissAll
            if (gvMissAll.Columns.Count != 0)
            {
                int intlngm = 1;
                foreach (BoundField bfCell in gvMissAll.Columns)
                {
                    string strColumnName = bfCell.DataField;
                    #region Css
                    if ((strColumnName.Substring(0, 4) != "lngL" && strColumnName.Substring(0, 4) != "lngM") || strColumnName == "lngMethodSN")
                    {
                        bfCell.ItemStyle.CssClass = strColumnName;
                    }
                    #endregion
                    #region Show the number of lngL,lngS,lngM with 2 digital
                    if (strColumnName.Substring(0, 4) == "lngM" && strColumnName != "lngMethodSN" && strColumnName != "lngMiss00SN")
                    {
                        bfCell.DataFormatString = "{0:d2}";
                        if (_dicCurrentNums.Values.Contains(int.Parse(strColumnName.Substring(4), InvariantCulture)) && ckAns.Checked)
                        {
                            bfCell.HeaderStyle.CssClass = string.Format(InvariantCulture, "lngM{0}", intlngm);
                            intlngm++;
                        }
                        int intNum = int.Parse(strColumnName.Substring(4), InvariantCulture);
                        if (intNum % 5 == 0 && strColumnName.Substring(0, 4) == "lngM")
                        {
                            bfCell.ItemStyle.CssClass = "glColNum5";
                        }
                    }
                    #endregion
                }
            }
            #endregion
            gvMissAll.RowDataBound += GvMissAll_RowDataBound;
            gvMissAll.DataBind();
            #endregion GridView
            pnlMiss.Controls.Add(gvMissAll);
            pnlDetail.Controls.Add(pnlMiss);
        }

        /// <summary>
        /// change CSS of the cells when datarow loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void GvMissAll_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Dictionary<string, DataControlFieldCell> dicCells = new Dictionary<string, DataControlFieldCell>();
            List<int> lstNums = new List<int>();
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

                for (int intNum = 1; intNum <= new CglDataSet(GlobalStuSearch.LottoType).CountNumber; intNum++)
                {
                    string strColName = string.Format(InvariantCulture, "lngL{0}", intNum);
                    lstNums.Add(int.Parse(dicCells[strColName].Text, InvariantCulture));
                }
                lstNums.Sort();

                _blLngM0 = false;
                if ((e.Row.RowIndex + 1) % 5 == 0) { e.Row.CssClass = "glRow5"; }
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        cell.ToolTip = strCell_ColumnName;
                        #region Set lngL , lngS
                        if (strCell_ColumnName.Contains("lngL") || strCell_ColumnName.Contains("lngS"))
                        {
                            if (_dicCurrentNums.ContainsValue(int.Parse(cell.Text, InvariantCulture)))
                            {
                                foreach (KeyValuePair<string, int> kv in _dicCurrentNums)
                                {
                                    if (kv.Value == int.Parse(cell.Text, InvariantCulture))
                                    {
                                        cell.CssClass = kv.Key;
                                    }
                                }
                            }
                        }
                        #endregion Set lngL , lngS

                        #region Set lngM
                        if (strCell_ColumnName.Contains("lngM") && strCell_ColumnName != "lngMethodSN")
                        {
                            int intColumnValue = int.Parse(strCell_ColumnName.Substring(4), InvariantCulture);
                            cell.ToolTip = string.Format(InvariantCulture, "{0}", intColumnValue);
                            int intValue = int.Parse(cell.Text, InvariantCulture);
                            _blLngM0 = (intValue < 0 && Math.Abs(intValue) == 1) || _blLngM0;
                            _blLngM0 = (intValue >= 0 || Math.Abs(intValue) != new CglDataSet(GlobalStuSearch.LottoType).CountNumber) && _blLngM0;

                            cell.CssClass = string.Empty;

                            cell.CssClass = intValue < 0 ? string.Format(InvariantCulture, " lngM{0} ", Math.Abs(intValue)) : cell.CssClass;

                            cell.Text = intValue < 0 ? lstNums[Math.Abs(intValue) - 1].ToString(InvariantCulture) : cell.Text;


                            cell.CssClass = _blLngM0 ? cell.CssClass + string.Format(InvariantCulture, " lngM{0} ", 0) : cell.CssClass;

                            cell.CssClass = (intColumnValue % 5 == 0) ? cell.CssClass + string.Format(InvariantCulture, " glColNum5 ") : cell.CssClass;

                        }
                        #endregion Set lngM

                        #region Set Day of Week
                        if (strCell_ColumnName == "lngDateSN")
                        {
                            string strDateSN = cell.Text;
                            switch (new DateTime(int.Parse(strDateSN.Substring(0, 4), InvariantCulture), int.Parse(strDateSN.Substring(4, 2), InvariantCulture), int.Parse(strDateSN.Substring(6, 2), InvariantCulture)).DayOfWeek)
                            {
                                case DayOfWeek.Monday:
                                    e.Row.CssClass = "glMonday";
                                    break;
                                case DayOfWeek.Tuesday:
                                    e.Row.CssClass = "glTuesday";
                                    break;
                                case DayOfWeek.Wednesday:
                                    e.Row.CssClass = "glWednesday";
                                    break;
                                case DayOfWeek.Thursday:
                                    e.Row.CssClass = "glThursday";
                                    break;
                                case DayOfWeek.Friday:
                                    e.Row.CssClass = "glFriday";
                                    break;
                                case DayOfWeek.Saturday:
                                    e.Row.CssClass = "glSaturday";
                                    break;
                                case DayOfWeek.Sunday:
                                    e.Row.CssClass = "glSunday";
                                    break;
                            }
                        }
                        #endregion Set Saturday

                    }

                }
            }
        }

        //------------------------------------------------------------------------------------
        protected void BtnCloseClick(object sender, EventArgs e)
        {
            ViewState.Clear();
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);

        }
    }
}