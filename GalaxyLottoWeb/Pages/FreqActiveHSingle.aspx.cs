using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Media;
using System.Net;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqActiveHSingle : BasePage
    {
        private StuGLSearch _gstuSearch;
        private string _action;
        private string _requestId;
        private string FreqActiveHSingleID;
        private Dictionary<string, string> _dicNumcssclass;
        private DataTable DtFreq { get; set; }
        private DataTable DtFreqProcess { get; set; }
        private DataTable DtFreqFilter01 { get; set; }
        private DataTable DtFreqFilter01Process { get; set; }
        private DataTable DtFreqFilter02 { get; set; }
        private DataTable DtFreqFilter02Process { get; set; }

        private static Dictionary<string, object> DicThreadFreqActiveHSingle
        {
            get { if (dicThreadFreqActiveHSingle == null) { dicThreadFreqActiveHSingle = new Dictionary<string, object>(); } return dicThreadFreqActiveHSingle; }
            set => dicThreadFreqActiveHSingle = value;
        }

        private static Dictionary<string, object> dicThreadFreqActiveHSingle;

        private string LocalBrowserType { get; set; }

        private string LocalIP { get; set; }

        ////////private string KeySearchOrder { get; set; }
        // ---------------------------------------------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            LocalBrowserType = Request.Browser.Type;
            LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            //KeySearchOrder = string.Format(InvariantCulture, "{0}#{1}#dtSearchOrder", LocalIP, LocalBrowserType);

            if (ViewState[FreqActiveHSingleID + "_gstuSearch"] == null || ((StuGLSearch)ViewState[FreqActiveHSingleID + "_gstuSearch"]).StrSecField == "none")
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState[FreqActiveHSingleID + "_gstuSearch"];
                InitialFilter();
                InitialArgument();
                ShowResult();
            }
        }

        private void InitialArgument()
        {
            if (ViewState["Title"] == null) { ViewState.Add("Title", string.Format(InvariantCulture, "{0}:{1}", "活性歷史表(單筆)", new CglDBData().SetTitleString(_gstuSearch))); }
            if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch))); }
            if (ViewState["lblMethod"] == null) { ViewState.Add("lblMethod", new CglMethod().SetMethodString(_gstuSearch)); }
            if (ViewState["lblSearchMethod"] == null) { ViewState.Add("lblSearchMethod", new CglMethod().SetSearchMethodString(_gstuSearch)); }
            if (ViewState["_dicNumcssclass"] == null) { ViewState.Add("_dicNumcssclass", new GalaxyApp().GetNumcssclass(_gstuSearch)); }
            _dicNumcssclass = (Dictionary<string, string>)ViewState["_dicNumcssclass"];
        }

        private void ShowResult()
        {
            Page.Title = (string)ViewState["Title"];
            lblTitle.Text = (string)ViewState["Title"];
            lblMethod.Text = (string)ViewState["lblMethod"];
            lblSearchMethod.Text = (string)ViewState["lblSearchMethod"];

            // 當期資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            ShowFreq();
            ShowFreqProcess();
            ShowFreqFilter01();
            ShowFilter01Process();
            ShowFreqFilter02();
            ShowFilter02Process();

            if (!DicThreadFreqActiveHSingle.Keys.Contains(FreqActiveHSingleID + "T01")) { CreatThread01(); }
            //if (chkShowProcess.Checked && !dicThreadFreqActiveHSingle.Keys.Contains(FreqActiveHSingleID + "T02")) { CreatThread02(); }
            if (!chkShowProcess.Checked)
            {
                dtlFreqProcess.Visible = false;
                dtlFreqFilter01Process.Visible = false;
                dtlFreqFilter02Process.Visible = false;
            }
        }

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            FreqActiveHSingleID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqActiveHSingleID] != null)
            {
                if (ViewState[FreqActiveHSingleID + "_gstuSearch"] == null)
                {
                    ViewState.Add(FreqActiveHSingleID + "_gstuSearch", (StuGLSearch)Session[FreqActiveHSingleID]);
                }
                else
                {
                    ViewState[FreqActiveHSingleID + "_gstuSearch"] = (StuGLSearch)Session[FreqActiveHSingleID];
                };
            }
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        private void InitialFilter()
        {
            txtHistoryPeriods.Text = IsPostBack ? txtHistoryPeriods.Text : _gstuSearch.InHistoryPeriods.ToString(InvariantCulture);
            txtHistoryTestPeriods.Text = IsPostBack ? txtHistoryTestPeriods.Text : _gstuSearch.HistoryTestPeriods.ToString(InvariantCulture);
            txtHitMin.Text = IsPostBack ? txtHitMin.Text : _gstuSearch.InHitMin.ToString(InvariantCulture);

            chkFilterRange.Checked = IsPostBack ? chkFilterRange.Checked : _gstuSearch.FilterRange;
            phFilterRange.Visible = IsPostBack ? phFilterRange.Visible : _gstuSearch.FilterRange;
            txtFilterRange.Text = IsPostBack ? txtFilterRange.Text : _gstuSearch.StrFilterRange;
            txtFilterMin.Text = IsPostBack ? txtFilterMin.Text : _gstuSearch.SglFilterMin.ToString(InvariantCulture);
            txtFilterMax.Text = IsPostBack ? txtFilterMax.Text : _gstuSearch.SglFilterMax.ToString(InvariantCulture);
            btnFilter.Text = Properties.Resources.Filter;
        }

        // ---------------------------------------------------------------------------------------------------------
        #region Freq
        private void ShowFreq()
        {
            if (Session[FreqActiveHSingleID + "dtFreq"] != null)
            {
                DtFreq = (DataTable)Session[FreqActiveHSingleID + "dtFreq"];
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
        }

        private void ShowFreqProcess()
        {
            if (Session[FreqActiveHSingleID + "dtFreq"] != null && Session[FreqActiveHSingleID + "dtFreqProcess"] != null && chkShowProcess.Checked)
            {
                DtFreqProcess = (DataTable)Session[FreqActiveHSingleID + "dtFreqProcess"];
                DtFreqProcess.DefaultView.Sort = "lngTotalSN DESC , lngN ASC";
                gvFreqProcess.DataSource = DtFreqProcess.DefaultView;

                #region Set Columns of DataGrid gvProcess
                if (gvFreqProcess.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < DtFreqProcess.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqProcess.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqProcess.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = DtFreqProcess.Columns[ColumnIndex].ColumnName,
                        };
                        gvFreqProcess.Columns.Add(bfCell);
                    }
                }
                foreach (BoundField dcColumn in gvFreqProcess.Columns)
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
                #endregion
                gvFreqProcess.RowDataBound += GvFreqProcess_RowDataBound;
                gvFreqProcess.DataBind();
                dtlFreqProcess.Visible = true;
            }
        }

        private void GvFreqProcess_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowIndex + 1) % 5 == 0) { e.Row.CssClass = "glRow5"; }
            }
        }

        #endregion Freq
        // ---------------------------------------------------------------------------------------------------------
        #region FreqFilter01
        private void ShowFreqFilter01()
        {
            if (Session[FreqActiveHSingleID + "dtFreqFilter01"] != null)
            {
                DtFreqFilter01 = (DataTable)Session[FreqActiveHSingleID + "dtFreqFilter01"];
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

        private void ShowFilter01Process()
        {
            if (Session[FreqActiveHSingleID + "dtFreqFilter01"] != null && Session[FreqActiveHSingleID + "dtFreqFilter01Process"] != null && chkShowProcess.Checked)
            {
                DtFreqFilter01Process = (DataTable)Session[FreqActiveHSingleID + "dtFreqFilter01Process"];
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
                dtlFreqFilter01Process.Visible = true;
            }
        }

        private void GvFreqFilter01Process_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowIndex + 1) % 5 == 0) { e.Row.CssClass = "glRow5"; }

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

        #endregion FreqFilter01
        // ---------------------------------------------------------------------------------------------------------
        #region FreqFilter02

        private void ShowFreqFilter02()
        {
            if (Session[FreqActiveHSingleID + "dtFreqFilter02"] != null)
            {
                DtFreqFilter02 = ((DataTable)Session[FreqActiveHSingleID + "dtFreqFilter02"]).Copy();
                if (DtFreqFilter02.Columns.Contains("lngFreqFilter02SN")) { DtFreqFilter02.Columns.Remove("lngFreqFilter02SN"); }
                if (DtFreqFilter02.Columns.Contains("lngTotalSN")) { DtFreqFilter02.Columns.Remove("lngTotalSN"); }
                if (int.Parse(DtFreqFilter02.Rows[0]["intHistoryTestPeriods"].ToString(), InvariantCulture) == int.Parse(DtFreqFilter02.Rows[0]["TestPeriods"].ToString(), InvariantCulture) &&
                    float.Parse(DtFreqFilter02.Rows[0]["intHistoryHitRate"].ToString(), InvariantCulture) >= 0.5)
                {
                    btnQuickFilter.Visible = true;
                }
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
            if (Session[FreqActiveHSingleID + "dtFreqFilter02"] != null && Session[FreqActiveHSingleID + "dtFreqFilter02Process"] != null && chkShowProcess.Checked)
            {
                //if (ViewState["dtFreqFilter02Process"] == null) { ViewState.Add("dtFreqFilter02Process", new CglFreq().GetFreqFilterProcess(_gstuSearch, CglFreq.TableName.QryFreqFilter02)); }
                DtFreqFilter02Process = (DataTable)Session[FreqActiveHSingleID + "dtFreqFilter02Process"];
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
                dtlFreqFilter02Process.Visible = true;
            }
        }

        private void GvFreqFilter02Process_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowIndex + 1) % 5 == 0) { e.Row.CssClass = "glRow5"; }
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

        protected void IBHistoryPeriodsClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            txtHistoryPeriods.Text = Properties.Resources.defaultHistoryPeriodsValue;
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
            ReleaseMemory();

            _gstuSearch = SetSearchOptions(_gstuSearch);
            _requestId = SetRequestId(_gstuSearch);
            FreqActiveHSingleID = _action + _requestId;
            Session[FreqActiveHSingleID] = _gstuSearch;
            string strUrl = string.Format(InvariantCulture, "http://{0}/Pages/{1}?action={2}&id={3}",
                                                            Request.Url.Authority,
                                                            Properties.Resources.PageFreqActiveHSingle,
                                                            Properties.Resources.SessionsFreqActiveHSingle,
                                                            _requestId);

            Response.Redirect(strUrl, true);
        }

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            ReleaseMemory();
            Session.Remove(FreqActiveHSingleID);
            ViewState.Remove(FreqActiveHSingleID + "_gstuSearch");
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
        }

        protected void BtnQuickFilterClick(object sender, EventArgs e)
        {
            if (Session[FreqActiveHSingleID + "dtFreqFilter01"] != null)
            {
                DataTable dtQuickFilter = (DataTable)Session[FreqActiveHSingleID + "dtFreqFilter01"];
                long MethodSN = long.Parse(dtQuickFilter.Rows[0]["lngMethodSN"].ToString(), InvariantCulture);
                long SearchMethodSN = long.Parse(dtQuickFilter.Rows[0]["lngSearchMethodSN"].ToString(), InvariantCulture);
                long SecFieldSN = long.Parse(dtQuickFilter.Rows[0]["lngSecFieldSN"].ToString(), InvariantCulture);
                float HistoryTestPeriods = float.Parse(dtQuickFilter.Rows[0]["intHistoryTestPeriods"].ToString(), InvariantCulture);

                foreach (float TestPeriods in new List<float>() { 20, 15, 10 })
                {
                    if (TestPeriods != HistoryTestPeriods)
                    {
                        StuGLSearch stuGLSearchTemp = _gstuSearch;
                        stuGLSearchTemp.LngMethodSN = MethodSN;
                        stuGLSearchTemp = new CglMethod().GetMethodWithSN(stuGLSearchTemp);
                        stuGLSearchTemp.LngSearchMethodSN = SearchMethodSN;
                        stuGLSearchTemp = new CglMethod().GetSearchMethodWithSN(stuGLSearchTemp);
                        stuGLSearchTemp.LngSecFieldSN = SecFieldSN;
                        stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);
                        stuGLSearchTemp.HistoryTestPeriods = TestPeriods;
                        string RequestID = SetRequestId(stuGLSearchTemp);
                        SetFrtSearchOrder(stuGLSearchTemp, Properties.Resources.SessionsFreqActiveHSingle, RequestID, Properties.Resources.PageFreqActiveHSingle, LocalIP, LocalBrowserType);
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Suspend")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Resume")]
        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            Thread ThreadFreqActiveHSingle01 = (Thread)DicThreadFreqActiveHSingle[FreqActiveHSingleID + "T01"];
            if (ThreadFreqActiveHSingle01.ThreadState == ThreadState.Running)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Pause);
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHSingle01.Suspend();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
            if (ThreadFreqActiveHSingle01.ThreadState == ThreadState.Suspended)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHSingle01.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Resume")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Suspend")]
        protected void BtnT2StartClick(object sender, EventArgs e)
        {
            Thread ThreadFreqActiveHSingle02 = (Thread)DicThreadFreqActiveHSingle[FreqActiveHSingleID + "T02"];
            if (ThreadFreqActiveHSingle02.ThreadState == ThreadState.Running)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Pause);
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHSingle02.Suspend();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
            if (ThreadFreqActiveHSingle02.ThreadState == ThreadState.Suspended)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHSingle02.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void Timer1Tick(object sender, EventArgs e)
        {
            CheckThread();
        }

        protected void ChkShowProcessCheckedChanged(object sender, EventArgs e)
        {
            if (chkShowProcess.Checked && !DicThreadFreqActiveHSingle.Keys.Contains(FreqActiveHSingleID + "T02")) { CreatThread02(); }
            dtlFreqProcess.Visible = chkShowProcess.Checked;
            dtlFreqFilter01Process.Visible = chkShowProcess.Checked;
            dtlFreqFilter02Process.Visible = chkShowProcess.Checked;

        }

        private void CreatThread01()
        {
            Timer1.Enabled = true;
            Timer1.Interval = 10000;
            Thread ThreadFreqActiveHSingle01 = new Thread(() =>
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
                base.Session[FreqActiveHSingleID + "dtFreq"] = new CglFunc().SortFreq(_gstuSearch, new CglFreqSecHis().GetFreqSecHis(_gstuSearch, CglFreqSecHis.TableName.TblFreqSecHis));
                base.Session[FreqActiveHSingleID + "dtFreqFilter01"] = new CglFreqFilter().GetFreqFilter(_gstuSearch, CglFreqFilter.TableName.QryFreqFilter01);
                base.Session[FreqActiveHSingleID + "dtFreqFilter02"] = new CglFreqFilter().GetFreqFilter(_gstuSearch, CglFreqFilter.TableName.QryFreqFilter02);
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
                ResetFrtSearchOrder(FreqActiveHSingleID);
            })
            {
                Name = FreqActiveHSingleID + "T01"
            };
            ThreadFreqActiveHSingle01.Start();

            DicThreadFreqActiveHSingle.Add(FreqActiveHSingleID + "T01", ThreadFreqActiveHSingle01);
            CheckThread();
        }

        private void CreatThread02()
        {
            Timer1.Enabled = true;
            Timer1.Interval = 10000;
            Thread ThreadFreqActiveHSingle02 = new Thread(() =>
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
                base.Session[FreqActiveHSingleID + "dtFreqProcess"] = new CglFreqSecHis().GetFreqSecHisProcess(_gstuSearch, CglFreqSecHis.TableName.QryFreqSecHisProcess, SortOrder.Descending);
                base.Session[FreqActiveHSingleID + "dtFreqFilter01Process"] = new CglFreqFilter().GetFreqFilterProcess(_gstuSearch, CglFreqFilter.TableName.QryFreqFilter01);
                base.Session[FreqActiveHSingleID + "dtFreqFilter02Process"] = new CglFreqFilter02().GetFreqFilter02Process(_gstuSearch, (DataTable)base.Session[FreqActiveHSingleID + "dtFreqFilter02"]);
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
            })
            {
                Name = FreqActiveHSingleID + "T02"
            };
            ThreadFreqActiveHSingle02.Start();

            DicThreadFreqActiveHSingle.Add(FreqActiveHSingleID + "T02", ThreadFreqActiveHSingle02);
            CheckThread();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:不要將常值當做已當地語系化的參數傳遞", MessageId = "System.Web.UI.WebControls.Label.set_Text(System.String)")]
        private void CheckThread()
        {
            if (DicThreadFreqActiveHSingle.Keys.Contains(FreqActiveHSingleID + "T01"))
            {
                Thread ThreadFreqActiveHSingle01 = (Thread)DicThreadFreqActiveHSingle[FreqActiveHSingleID + "T01"];
                if (ThreadFreqActiveHSingle01.IsAlive)
                {
                    Timer1.Enabled = true;
                    Timer1.Interval = 10000;
                    lblArgument.Text = string.Format(InvariantCulture, "{0} ", DateTime.Now.ToLongTimeString());
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0} ", new GalaxyApp().GetTheadState(ThreadFreqActiveHSingle01.ThreadState));
                    lblArgument.Visible = true;
                    btnT1Start.Visible = true;
                    //btnClose.Visible = false;
                    btnFilter.Visible = false;
                }
                else
                {
                    lblArgument.Visible = false;
                    btnT1Start.Visible = false;
                    chkShowProcess.Visible = true;
                    //btnClose.Visible = true;
                    btnFilter.Visible = true;
                    Timer1.Enabled = false;
                }
            }

            if (DicThreadFreqActiveHSingle.Keys.Contains(FreqActiveHSingleID + "T02"))
            {
                Thread ThreadFreqActiveHSingle02 = (Thread)DicThreadFreqActiveHSingle[FreqActiveHSingleID + "T02"];
                if (ThreadFreqActiveHSingle02.IsAlive)
                {
                    Timer1.Enabled = true;
                    Timer1.Interval = 10000;
                    lblArgument.Text = string.Format(InvariantCulture, "{0} ", DateTime.Now.ToLongTimeString());
                    btnT2Start.Text = string.Format(InvariantCulture, "T2:{0} ", new GalaxyApp().GetTheadState(ThreadFreqActiveHSingle02.ThreadState));
                    lblArgument.Visible = true;
                    btnT2Start.Visible = true;
                    //btnClose.Visible = false;
                    chkShowProcess.Visible = false;
                    btnFilter.Visible = false;
                }
                else
                {
                    lblArgument.Visible = false;
                    btnT2Start.Visible = false;
                    chkShowProcess.Visible = true;
                    //btnClose.Visible = true;
                    btnFilter.Visible = true;
                    Timer1.Enabled = false;
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------
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

            stuSearch00.InHistoryPeriods = int.Parse(txtHistoryPeriods.Text, InvariantCulture);

            stuSearch00.HistoryTestPeriods = float.Parse(txtHistoryTestPeriods.Text, InvariantCulture);

            stuSearch00.InHitMin = int.Parse(txtHitMin.Text, InvariantCulture);

            stuSearch00 = new CglMethod().GetSearchMethodSN(stuSearch00);

            return stuSearch00;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Resume")]
        private void ReleaseMemory()
        {
            base.Session.Remove(FreqActiveHSingleID + "dtFreq");
            base.Session.Remove(FreqActiveHSingleID + "dtFreqProcess");

            base.Session.Remove(FreqActiveHSingleID + "dtFreqFilter01");
            base.Session.Remove(FreqActiveHSingleID + "dtFreqFilter01Process");

            base.Session.Remove(FreqActiveHSingleID + "dtFreqFilter02");
            base.Session.Remove(FreqActiveHSingleID + "dtFreqFilter02Process");

            ResetFrtSearchOrder(FreqActiveHSingleID);
            if (DicThreadFreqActiveHSingle.Keys.Contains(FreqActiveHSingleID + "T01"))
            {
                Thread ThreadFreqActiveHSingle01 = (Thread)DicThreadFreqActiveHSingle[FreqActiveHSingleID + "T01"];

                if (ThreadFreqActiveHSingle01.IsAlive)
                {
                    if (ThreadFreqActiveHSingle01.ThreadState == ThreadState.Suspended)
                    {
#pragma warning disable CS0618 // 類型或成員已經過時
                        ThreadFreqActiveHSingle01.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
                    }
                    ThreadFreqActiveHSingle01.Abort();
                    ThreadFreqActiveHSingle01.Join();
                }
                DicThreadFreqActiveHSingle.Remove(FreqActiveHSingleID + "T01");
            }
            if (DicThreadFreqActiveHSingle.Keys.Contains(FreqActiveHSingleID + "T02"))
            {
                Thread ThreadFreqActiveHSingle02 = (Thread)DicThreadFreqActiveHSingle[FreqActiveHSingleID + "T02"];

                if (ThreadFreqActiveHSingle02.IsAlive)
                {
                    if (ThreadFreqActiveHSingle02.ThreadState == ThreadState.Suspended)
                    {
#pragma warning disable CS0618 // 類型或成員已經過時
                        ThreadFreqActiveHSingle02.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
                    }
                    ThreadFreqActiveHSingle02.Abort();
                    ThreadFreqActiveHSingle02.Join();
                }
                DicThreadFreqActiveHSingle.Remove(FreqActiveHSingleID + "T02");
            }
        }
    }
}