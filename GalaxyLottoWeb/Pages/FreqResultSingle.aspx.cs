using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqResultSingle : BasePage
    {
        private StuGLSearch _gstuSearch;
        private string _action;
        private string _requestId;
        private string FreqResultSingleID;
        private Dictionary<string, int> _dicCurrentNums;
        private Dictionary<string, string> _dicNumcssclass;
        private DataTable DtFreq { get; set; }
        private DataTable DtFreqProcess { get; set; }
        private DataTable DtFreqFilter01 { get; set; }
        private DataTable DtFreqFilter01Process { get; set; }
        private DataTable DtFreqFilter02 { get; set; }
        private DataTable DtFreqFilter02Process { get; set; }

        private static Dictionary<string, object> DicThreadFreqResultSingle
        {
            get
            {
                if (dicThreadFreqResultSingle == null) { dicThreadFreqResultSingle = new Dictionary<string, object>(); }
                return dicThreadFreqResultSingle;
            }
            set => dicThreadFreqResultSingle = value;
        }

        private static Dictionary<string, object> dicThreadFreqResultSingle;

        // ---------------------------------------------------------------------------------------------------------
        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            FreqResultSingleID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqResultSingleID] != null)
            {
                if (ViewState[FreqResultSingleID + "_gstuSearch"] == null)
                {
                    ViewState.Add(FreqResultSingleID + "_gstuSearch", (StuGLSearch)Session[FreqResultSingleID]);
                }
                else
                {
                    ViewState[FreqResultSingleID + "_gstuSearch"] = (StuGLSearch)Session[FreqResultSingleID];
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
            if (ViewState[FreqResultSingleID + "_gstuSearch"] == null || ((StuGLSearch)ViewState[FreqResultSingleID + "_gstuSearch"]).LngMethodSN == 0 || ((StuGLSearch)ViewState[FreqResultSingleID + "_gstuSearch"]).LngSearchMethodSN == 0)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState[FreqResultSingleID + "_gstuSearch"];
                InitialFilter();
                if (ViewState["title"] == null) { ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "頻率表(單筆)", new CglData().SetTitleString(_gstuSearch))); }
                if (ViewState["CurrentData"] == null) { ViewState["CurrentData"] = new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch)); }
                if (ViewState["CurrentNums"] == null) { ViewState["CurrentNums"] = new CglData().GetDataNumsDici(_gstuSearch); }
                if (ViewState["lblMethod"] == null) { ViewState.Add("lblMethod", new CglMethod().SetMethodString(_gstuSearch)); }
                if (ViewState["lblSearchMethod"] == null) { ViewState.Add("lblSearchMethod", new CglMethod().SetSearchMethodString(_gstuSearch)); }
                if (ViewState["_dicNumcssclass"] == null) { ViewState.Add("_dicNumcssclass", new GalaxyApp().GetNumcssclass(_gstuSearch)); }
                _dicNumcssclass = (Dictionary<string, string>)ViewState["_dicNumcssclass"];
                _dicCurrentNums = (Dictionary<string, int>)ViewState["CurrentNums"];
                ShowResult();
            }
        }

        private void InitialFilter()
        {
            txtHistoryTestPeriods.Text = IsPostBack ? txtHistoryTestPeriods.Text : _gstuSearch.HistoryTestPeriods.ToString(InvariantCulture);
            txtHitMin.Text = IsPostBack ? txtHitMin.Text : _gstuSearch.InHitMin.ToString(InvariantCulture);
            txtHitMin.Text = IsPostBack ? txtHitMin.Text : _gstuSearch.InHitMin.ToString(InvariantCulture);
            chkFilterRange.Checked = IsPostBack ? chkFilterRange.Checked : _gstuSearch.FilterRange;
            phFilterRange.Visible = IsPostBack ? phFilterRange.Visible : _gstuSearch.FilterRange;
            txtFilterRange.Text = IsPostBack ? txtFilterRange.Text : _gstuSearch.StrFilterRange;
            txtFilterMin.Text = IsPostBack ? txtFilterMin.Text : _gstuSearch.SglFilterMin.ToString(InvariantCulture);
            txtFilterMax.Text = IsPostBack ? txtFilterMax.Text : _gstuSearch.SglFilterMax.ToString(InvariantCulture);
            btnFilter.Text = Properties.Resources.Filter;
        }

        private void ShowResult()
        {
            Page.Title = (string)ViewState["title"];
            lblTitle.Text = (string)ViewState["title"];
            lblMethod.Text = (string)ViewState["lblMethod"];
            lblSearchMethod.Text = (string)ViewState["lblSearchMethod"];

            //顯示當前資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)base.ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            ShowFreq();
            ShowFreqProcess();
            ShowFreqFilter01();
            ShowFilter01Process();
            ShowFreqFilter02();
            ShowFilter02Process();
        }

        // ---------------------------------------------------------------------------------------------------------
        #region Freq
        private void ShowFreq()
        {
            if (ViewState["Freq"] == null) { ViewState.Add("Freq", new CglFreq().GetFreq(_gstuSearch)); }
            DtFreq = new CglFunc().SortFreq(_gstuSearch, (DataTable)ViewState["Freq"]);
            gvFreq.DataSource = DtFreq.DefaultView;
            if (gvFreq.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtFreq.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtFreq.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtFreq.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtFreq.Columns[ColumnIndex].ColumnName,
                    };
                    gvFreq.Columns.Add(bfCell);
                }
                foreach (DataControlField dcColumn in gvFreq.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if (_dicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                    {
                        dcColumn.HeaderStyle.CssClass = _dicNumcssclass[strColumnName.Substring(4)];
                    }
                }
            }
            gvFreq.DataBind();
        }

        private void ShowFreqProcess()
        {
            if (chkFreqProcess.Checked)
            {
                pnlFreqProcess.Visible = true;
                if (ViewState["FreqProcess"] == null) { ViewState.Add("FreqProcess", CglFreqProcess.GetFreqProcs(_gstuSearch, CglDBFreq.TableName.QryFreqProcess, SortOrder.Descending, (DataTable)ViewState["Freq"])); }
                DtFreqProcess = new CglFunc().CTableShow((DataTable)ViewState["FreqProcess"]);
                gvProcess.DataSource = DtFreqProcess.DefaultView;

                if (gvProcess.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < DtFreqProcess.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqProcess.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqProcess.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = DtFreqProcess.Columns[ColumnIndex].ColumnName,
                        };
                        gvProcess.Columns.Add(bfCell);
                    }
                }
                foreach (BoundField dcColumn in gvProcess.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if ((strColumnName.Substring(0, 4) != "lngL" && strColumnName.Substring(0, 4) != "lngM") || strColumnName == "lngMethodSN")
                    {
                        dcColumn.HeaderStyle.CssClass = strColumnName;
                        dcColumn.ItemStyle.CssClass = strColumnName;
                    }

                    if (strColumnName.Substring(0, 4) == "lngL" || strColumnName.Substring(0, 4) == "lngS")
                    {
                        dcColumn.HeaderStyle.CssClass = strColumnName;
                        dcColumn.DataFormatString = "{0:d2}";
                    }
                }
                gvProcess.RowDataBound += GvFreqProcess_RowDataBound;
                gvProcess.DataBind();
            }
            else
            {
                pnlFreqProcess.Visible = false;
            }
        }

        /// <summary>
        /// change CSS of the cells when datarow loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GvFreqProcess_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        #region Set lngL , lngS
                        string strCell_ColumnName = field.DataField;
                        if (strCell_ColumnName.Substring(0, 4) == "lngL" || strCell_ColumnName.Substring(0, 4) == "lngS")
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

        #endregion Freq

        // ---------------------------------------------------------------------------------------------------------
        #region FreqFilter01
        private void ShowFreqFilter01()
        {
            if (ViewState["FreqFilter"] == null) { ViewState.Add("FreqFilter", new CglFreqFilter().GetFreqFilter(_gstuSearch, CglFreqFilter.TableName.QryFreqFilter01)); }
            DtFreqFilter01 = (DataTable)ViewState["FreqFilter"];
            if (DtFreqFilter01.Columns.Contains("lngTotalSN")) { DtFreqFilter01.Columns.Remove("lngTotalSN"); }
            gvFreqFilter01.DataSource = DtFreqFilter01.DefaultView;
            if (gvFreqFilter01.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtFreqFilter01.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtFreqFilter01.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtFreqFilter01.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtFreqFilter01.Columns[ColumnIndex].ColumnName,
                    };
                    bfCell.HeaderStyle.CssClass = bfCell.DataField;
                    bfCell.ItemStyle.CssClass = bfCell.DataField;
                    gvFreqFilter01.Columns.Add(bfCell);
                }
            }
            gvFreqFilter01.RowDataBound += GvFreqFilter01_RowDataBound;
            gvFreqFilter01.DataBind();
        }

        private void GvFreqFilter01_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        #region lngTotalSN
                        if (strCell_ColumnName == "lngTotalSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngTotalSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglSearch().InitSearchDataRange(stuGLSearchTemp);
                            stuGLSearchTemp.LngMethodSN = long.Parse(e.Row.Cells[2].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSearchMethodSN = long.Parse(e.Row.Cells[3].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSearchMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSecFieldSN = long.Parse(e.Row.Cells[4].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);
                            using Button btnFunction = new Button
                            {
                                ID = string.Format(InvariantCulture, "btnFunction_{0}", e.Row.Cells[0].Text),
                                Text = Properties.Resources.View
                            };
                            //btnFunction.CssClass = "glbutton glbutton-grey ";
                            if (stuGLSearchTemp.LngSecFieldSN == 1)
                            {
                                btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('{0}?action={1}&id={2}','_blank');",
                                                                                                       Properties.Resources.PageFreqResultSingle,
                                                                                                       Properties.Resources.SessionsFreqResultSingle,
                                                                                                      SetRequestId(stuGLSearchTemp));
                                Session.Add(Properties.Resources.SessionsFreqResultSingle + SetRequestId(stuGLSearchTemp), stuGLSearchTemp);
                            }
                            else
                            {
                                btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('{0}?action={1}&id={2}','_blank');",
                                                                                                       Properties.Resources.PageFreqActiveHSingle,
                                                                                                       Properties.Resources.SessionsFreqActiveHSingle,
                                                                                                       SetRequestId(stuGLSearchTemp));
                                Session.Add(Properties.Resources.SessionsFreqActiveHSingle + SetRequestId(stuGLSearchTemp), stuGLSearchTemp);
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

                        #region LngSecFieldSN
                        if (strCell_ColumnName == "lngSecFieldSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngSecFieldSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);
                            cell.ToolTip = stuGLSearchTemp.StrSecField;
                        }
                        #endregion LngSecFieldSN
                    }
                }
            }
        }

        private void ShowFilter01Process()
        {
            if (chkFreqFilter01Process.Checked)
            {
                pnlFreqFilter01Process.Visible = true;
                if (ViewState["dtFreqFilter01Process"] == null) { ViewState.Add("dtFreqFilter01Process", new CglFreqFilter().GetFreqFilterProcess(_gstuSearch, CglFreqFilter.TableName.QryFreqFilter01)); }
                DtFreqFilter01Process = (DataTable)ViewState["dtFreqFilter01Process"];
                if (DtFreqFilter01Process.Columns.Contains("strcheck")) { DtFreqFilter01Process.Columns.Remove("strcheck"); }
                gvFreqFilter01Process.DataSource = DtFreqFilter01Process.DefaultView;
                if (gvFreqFilter01Process.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < DtFreqFilter01Process.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqFilter01Process.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqFilter01Process.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = DtFreqFilter01Process.Columns[ColumnIndex].ColumnName,
                        };
                        bfCell.HeaderStyle.CssClass = bfCell.DataField;
                        bfCell.ItemStyle.CssClass = bfCell.DataField;
                        gvFreqFilter01Process.Columns.Add(bfCell);
                    }
                }
                gvFreqFilter01Process.RowDataBound += GvFreqFilter01Process_RowDataBound;
                gvFreqFilter01Process.DataBind();
            }
            else
            {
                gvFreqFilter01Process.Visible = false;
            }
        }

        private void GvFreqFilter01Process_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        #region lngTotalSN
                        if (strCell_ColumnName == "lngTotalSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngTotalSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglSearch().InitSearchDataRange(stuGLSearchTemp);
                            stuGLSearchTemp.LngMethodSN = long.Parse(e.Row.Cells[3].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSearchMethodSN = long.Parse(e.Row.Cells[4].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSearchMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSecFieldSN = long.Parse(e.Row.Cells[5].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);
                            if (cell.Controls.Count == 0)
                            {
                                using Button btnFunction = new Button
                                {
                                    ID = string.Format(InvariantCulture, "btnFunction_{0}", e.Row.Cells[0].Text),
                                    Text = Properties.Resources.View
                                };
                                //btnFunction.CssClass = "glbutton glbutton-grey ";
                                if (stuGLSearchTemp.LngSecFieldSN == 1)
                                {
                                    btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('{0}?action={1}&id={2}','_blank');",
                                                                                                           Properties.Resources.PageFreqResultSingle,
                                                                                                           Properties.Resources.SessionsFreqResultSingle,
                                                                                                           SetRequestId(stuGLSearchTemp));
                                    Session.Add(Properties.Resources.SessionsFreqResultSingle + SetRequestId(stuGLSearchTemp), stuGLSearchTemp);
                                }
                                else
                                {
                                    btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('{0}?action={1}&id={2}','_blank');",
                                                                                                           Properties.Resources.PageFreqActiveHSingle,
                                                                                                           Properties.Resources.SessionsFreqActiveHSingle,
                                                                                                           SetRequestId(stuGLSearchTemp));
                                    Session.Add(Properties.Resources.SessionsFreqActiveHSingle + SetRequestId(stuGLSearchTemp), stuGLSearchTemp);
                                }
                                cell.Controls.Add(btnFunction);
                            }
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

                        #region LngSecFieldSN
                        if (strCell_ColumnName == "lngSecFieldSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngSecFieldSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);
                            cell.ToolTip = stuGLSearchTemp.StrSecField;
                        }
                        #endregion LngSecFieldSN
                    }
                }
            }
        }

        #endregion FreqFilter01

        // ---------------------------------------------------------------------------------------------------------
        #region FreqFilter02
        private void ShowFreqFilter02()
        {
            if (CheckThread() && chkFreqFilter02.Checked)
            {
                pnlFreqFilter02.Visible = true;
                DtFreqFilter02 = (DataTable)Session[FreqResultSingleID + "dtFreqFilter02"];
                if (DtFreqFilter02.Columns.Contains("lngFreqFilter02SN")) { DtFreqFilter02.Columns.Remove("lngFreqFilter02SN"); }
                if (DtFreqFilter02.Columns.Contains("lngTotalSN")) { DtFreqFilter02.Columns.Remove("lngTotalSN"); }
                gvFreqFilter02.DataSource = DtFreqFilter02.DefaultView;
                if (gvFreqFilter02.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < DtFreqFilter02.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqFilter02.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqFilter02.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = DtFreqFilter02.Columns[ColumnIndex].ColumnName,
                        };
                        bfCell.HeaderStyle.CssClass = bfCell.DataField;
                        bfCell.ItemStyle.CssClass = bfCell.DataField;
                        gvFreqFilter02.Columns.Add(bfCell);
                    }
                }
                gvFreqFilter02.RowDataBound += GvFreqFilter02_RowDataBound;
                gvFreqFilter02.DataBind();
            }
            else
            {
                pnlFreqFilter02.Visible = false;
            }
        }

        private void GvFreqFilter02_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        #region lngTotalSN
                        if (strCell_ColumnName == "lngTotalSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngTotalSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglSearch().InitSearchDataRange(stuGLSearchTemp);

                            stuGLSearchTemp.LngMethodSN = long.Parse(e.Row.Cells[3].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSearchMethodSN = long.Parse(e.Row.Cells[4].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSearchMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSecFieldSN = long.Parse(e.Row.Cells[5].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);
                            string RequestID = SetRequestId(stuGLSearchTemp);

                            using Button btnFunction = new Button
                            {
                                ID = string.Format(InvariantCulture, "btnFunction_{0}", e.Row.Cells[0].Text),
                                Text = Properties.Resources.View
                            };
                            //btnFunction.CssClass = "glbutton glbutton-grey ";
                            if (stuGLSearchTemp.LngSecFieldSN == 1)
                            {
                                btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('{0}?action={1}&id={2}','_blank');",
                                                                                                       Properties.Resources.PageFreqResultSingle,
                                                                                                       Properties.Resources.SessionsFreqResultSingle,
                                                                                                       RequestID);
                                base.Session.Add(Properties.Resources.SessionsFreqResultSingle + RequestID, stuGLSearchTemp);
                            }
                            else
                            {
                                btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('{0}?action={1}&id={2}','_blank');",
                                                                                                       Properties.Resources.PageFreqActiveHSingle,
                                                                                                       Properties.Resources.SessionsFreqActiveHSingle,
                                                                                                       RequestID);
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

                        #region LngSecFieldSN
                        if (strCell_ColumnName == "lngSecFieldSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngSecFieldSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);
                            cell.ToolTip = stuGLSearchTemp.StrSecField;
                        }
                        #endregion LngSecFieldSN
                    }
                }
            }

        }

        private void ShowFilter02Process()
        {
            if (CheckThread() && chkFreqFilter02Process.Checked && chkFreqFilter02.Checked)
            {
                pnlFreqFilter02Process.Visible = true;
                DtFreqFilter02Process = (DataTable)Session[FreqResultSingleID + "dtFreqFilter02Process"];
                if (DtFreqFilter02Process.Columns.Contains("lngFreqFilter02SN")) { DtFreqFilter02Process.Columns.Remove("lngFreqFilter02SN"); }
                if (DtFreqFilter02Process.Columns.Contains("strcheck")) { DtFreqFilter02Process.Columns.Remove("strcheck"); }
                gvFreqFilter02Process.DataSource = DtFreqFilter02Process.DefaultView;
                if (gvFreqFilter02Process.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < DtFreqFilter02Process.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqFilter02Process.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqFilter02Process.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = DtFreqFilter02Process.Columns[ColumnIndex].ColumnName,
                        };
                        bfCell.HeaderStyle.CssClass = bfCell.DataField;
                        bfCell.ItemStyle.CssClass = bfCell.DataField;
                        gvFreqFilter02Process.Columns.Add(bfCell);
                    }
                }
                gvFreqFilter02Process.RowDataBound += GvFreqFilter02Process_RowDataBound;
                gvFreqFilter02Process.DataBind();
            }
            else
            {
                chkFreqFilter02Process.Checked = false;
                pnlFreqFilter02Process.Visible = false;
            }
        }

        private void GvFreqFilter02Process_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        #region lngTotalSN
                        if (strCell_ColumnName == "lngTotalSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngTotalSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglSearch().InitSearchDataRange(stuGLSearchTemp);

                            stuGLSearchTemp.LngMethodSN = long.Parse(e.Row.Cells[3].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSearchMethodSN = long.Parse(e.Row.Cells[4].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSearchMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSecFieldSN = long.Parse(e.Row.Cells[5].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);
                            string RequestID = SetRequestId(stuGLSearchTemp);
                            if (cell.Controls.Count == 0)
                            {
                                using Button btnFunction = new Button
                                {
                                    ID = string.Format(InvariantCulture, "btnFunction_{0}", e.Row.Cells[0].Text),
                                    Text = Properties.Resources.View
                                };
                                //btnFunction.CssClass = "glbutton glbutton-grey ";
                                if (stuGLSearchTemp.LngSecFieldSN == 1)
                                {
                                    btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('{0}?action={1}&id={2}','_blank');",
                                                                                                           Properties.Resources.PageFreqResultSingle,
                                                                                                           Properties.Resources.SessionsFreqResultSingle,
                                                                                                           RequestID);
                                    base.Session.Add(Properties.Resources.SessionsFreqResultSingle + RequestID, stuGLSearchTemp);
                                }
                                else
                                {
                                    btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('{0}?action={1}&id={2}','_blank');",
                                                                                                           Properties.Resources.PageFreqActiveHSingle,
                                                                                                           Properties.Resources.SessionsFreqActiveHSingle,
                                                                                                           RequestID);
                                    Session.Add(Properties.Resources.SessionsFreqActiveHSingle + RequestID, stuGLSearchTemp);
                                }
                                cell.Controls.Add(btnFunction);
                            }
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

                        #region LngSecFieldSN
                        if (strCell_ColumnName == "lngSecFieldSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngSecFieldSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);
                            cell.ToolTip = stuGLSearchTemp.StrSecField;
                        }
                        #endregion LngSecFieldSN
                    }
                }
            }
        }

        #endregion FreqFilter02
        // ---------------------------------------------------------------------------------------------------------

        protected void ChkFilterRangeCheckedChanged(object sender, EventArgs e)
        {
            phFilterRange.Visible = chkFilterRange.Checked;
        }

        protected void IBFilterRangeClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            txtFilterRange.Text = string.Empty;
        }

        protected void IBFilterMaxClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            txtFilterMax.Text = Properties.Resources.defaultFilterMaxValue;
        }

        protected void IBFilterMinClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            txtFilterMin.Text = Properties.Resources.defaultFilterMinValue;
        }

        protected void IBHistoryTestPeriodsClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            txtHistoryTestPeriods.Text = Properties.Resources.defaultHistoryTestPeriodsValue;
        }

        protected void IBHitMinClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            txtHitMin.Text = Properties.Resources.defaultHitMinValue;
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void BtnFreqClick(object sender, EventArgs e)//頻率
        {
            Session.Remove(FreqResultSingleID + "dtFreqFilter02");
            Session.Remove(FreqResultSingleID + "dtFreqFilter02Process");
            ((Thread)DicThreadFreqResultSingle[FreqResultSingleID]).Abort();
            DicThreadFreqResultSingle.Remove(FreqResultSingleID);

            _gstuSearch = SetSearchOptions(_gstuSearch);
            _requestId = SetRequestId(_gstuSearch);
            Session[_action + _requestId] = _gstuSearch;

            string strUrl = string.Format(InvariantCulture, "http://{0}/Pages/{1}?action={2}&id={3}",
                                                            Request.Url.Authority,
                                                            Properties.Resources.PageFreqResultSingle,
                                                            Properties.Resources.SessionsFreqResultSingle,
                                                            _requestId);

            Response.Redirect(strUrl, true);
        }

        private StuGLSearch SetSearchOptions(StuGLSearch stuSearch00)
        {
            chkFilterRange.Checked = phFilterRange.Visible && chkFilterRange.Checked;
            stuSearch00.FilterRange = chkFilterRange.Checked;
            phFilterRange.Visible = chkFilterRange.Checked;

            txtFilterRange.Text = chkFilterRange.Checked ? txtFilterRange.Text : string.Empty;
            stuSearch00.StrFilterRange = string.IsNullOrEmpty(txtFilterRange.Text) ? "none" : txtFilterRange.Text;

            txtFilterMin.Text = chkFilterRange.Checked ? txtFilterMin.Text : Properties.Resources.defaultFilterMinValue;
            stuSearch00.SglFilterMin = float.Parse(txtFilterMin.Text, InvariantCulture);

            txtFilterMax.Text = chkFilterRange.Checked ? txtFilterMax.Text : Properties.Resources.defaultFilterMaxValue;
            stuSearch00.SglFilterMax = float.Parse(txtFilterMax.Text, InvariantCulture);

            txtHistoryTestPeriods.Text = txtHistoryTestPeriods.Text;
            stuSearch00.HistoryTestPeriods = float.Parse(txtHistoryTestPeriods.Text, InvariantCulture);

            txtHitMin.Text = txtHitMin.Text;
            stuSearch00.InHitMin = int.Parse(txtHitMin.Text, InvariantCulture);

            stuSearch00 = new CglMethod().GetSearchMethodSN(stuSearch00);

            return stuSearch00;
        }

        protected void IBInfoClick(object sender, ImageClickEventArgs e)
        {
            pnlTopData.Visible = !pnlTopData.Visible;
        }
        // ---------------------------------------------------------------------------------------------------------
        protected void Timer1Tick(object sender, EventArgs e)
        {
            if (CheckThread()) { Timer1.Interval = 360000; };
        }

        private void CreatThread()
        {
            if (Session[FreqResultSingleID + "dtFreqFilter02"] == null)
            {

                Thread ThreadFreqActiveHSingle = new Thread(() =>
                {
                    DataTable dtFreqFilter02Temp = new CglFreqFilter().GetFreqFilter(_gstuSearch, CglFreqFilter.TableName.QryFreqFilter02);
                    Session.Add(FreqResultSingleID + "dtFreqFilter02", dtFreqFilter02Temp);
                    DataTable dtFreqFilter02ProcessTemp = new CglFreqFilter02().GetFreqFilter02Process(_gstuSearch, dtFreqFilter02Temp);
                    Session.Add(FreqResultSingleID + "dtFreqFilter02Process", dtFreqFilter02ProcessTemp);
                })
                {
                    Name = FreqResultSingleID
                };
                ThreadFreqActiveHSingle.Start();

                DicThreadFreqResultSingle.Add(FreqResultSingleID, ThreadFreqActiveHSingle);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:不要將常值當做已當地語系化的參數傳遞", MessageId = "System.Web.UI.WebControls.Label.set_Text(System.String)")]
        private bool CheckThread()
        {
            if (!DicThreadFreqResultSingle.Keys.Contains(FreqResultSingleID)) { CreatThread(); }
            Thread ThreadFreqResultSingle = (Thread)DicThreadFreqResultSingle[FreqResultSingleID];
            if (ThreadFreqResultSingle.IsAlive)
            {
                lblArgument.Text = string.Format(InvariantCulture, "{0} 機率02(同支數) 處理中 ... ", DateTime.Now.ToLongTimeString());
            }
            else
            {
                chkFreqFilter02.Visible = true;
                chkFreqFilter02Process.Visible = true;
                lblArgument.Text = "";
                ResetFrtSearchOrder(FreqResultSingleID);
            };
            return (Session[FreqResultSingleID + "dtFreqFilter02"] != null && Session[FreqResultSingleID + "dtFreqFilter02Process"] != null && !ThreadFreqResultSingle.IsAlive);
        }

        // ---------------------------------------------------------------------------------------------------------

    }
}
