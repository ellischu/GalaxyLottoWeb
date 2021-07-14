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
    public partial class FreqResult : BasePage
    {
        private StuGLSearch _gstuSearch;
        private Dictionary<string, int> _dicCurrentNums;
        private Dictionary<string, string> _dicNumcssclass;

        private DataTable DtFreq { get; set; }

        private DataTable DtFreqView { get; set; }

        private DataTable DtFreqProcess { get; set; }

        private DataTable dtFreqFilter01, dtFreqFilter01Process;
        private string FreqResultID;
        private List<string> lstdllFreqItems;

        private void SetupViewState()
        {

            string action = Request["action"] ?? (string)base.Session["action"] ?? (string)base.ViewState["action"] ?? string.Empty;
            string id = Request["id"] ?? (string)base.Session["id"] ?? (string)base.ViewState["id"] ?? string.Empty;
            if (ViewState["action"] == null) { base.ViewState.Add("action", action); }

            if (ViewState["id"] == null) { base.ViewState.Add("id", id); }

            FreqResultID = action + id;

            if (ViewState["_gstuSearch"] == null)
            {
                if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(id) && base.Session[FreqResultID] != null)
                {
                    ViewState.Add(FreqResultID + "_gstuSearch", (StuGLSearch)Session[FreqResultID]);
                }
            }
            _gstuSearch = (StuGLSearch)ViewState[FreqResultID + "_gstuSearch"];
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetupViewState();
            if (ViewState["_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState["_gstuSearch"];

                InitialArgument(_gstuSearch);

                SetddlFreq(_gstuSearch);

                ShowTitle(_gstuSearch);

                ShowResult(_gstuSearch);
            }
        }

        private void ShowTitle(StuGLSearch stuSearch)
        {
            if (ViewState["title"] == null) { ViewState.Add("title", new CglDBData().SetTitleString(stuSearch)); }
            Page.Title = (string)ViewState["title"];
            lblTitle.Text = (string)ViewState["title"];

            if (ddlNexts.SelectedValue != "gen")
            {
                stuSearch.StrNextNums = ddlNexts.SelectedValue;
            }
            if (ViewState["lblMethod"] == null) { ViewState.Add("lblMethod", string.Format(InvariantCulture, "{0}:{1}", "頻率總表", new CglMethod().SetMethodString(stuSearch))); }
            lblMethod.Text = (string)ViewState["lblMethod"];


            if (ViewState["lblSearchMethod"] == null) { ViewState.Add("lblSearchMethod", new CglMethod().SetSearchMethodString(stuSearch)); }
            lblSearchMethod.Text = (string)ViewState["lblSearchMethod"];

            //顯示當前資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
        }

        private void InitialArgument(StuGLSearch stuSearch)
        {
            if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(stuSearch))); }
            if (ViewState["CurrentNums"] == null) { ViewState.Add("CurrentNums", new CglData().GetDataNumsDici(stuSearch)); }
            //if (ViewState["selectedValue"] == null) { ViewState.Add("selectedValue", "gen"); }
            //if (ViewState["ResultFreq"] == null) { ViewState.Add("ResultFreq", new CglFreq().GetFreqDicDataSet(_gstuSearch)); }
            if (ViewState["_dicNumcssclass"] == null) { ViewState.Add("_dicNumcssclass", new GalaxyApp().GetNumcssclass(stuSearch)); }
            _dicNumcssclass = (Dictionary<string, string>)ViewState["_dicNumcssclass"];
            if (Session[FreqResultID + "lstdllFreqItems"] == null)
            {
                if (lstdllFreqItems == null) { lstdllFreqItems = new List<string> { "gen" }; }
                Session.Add(FreqResultID + "lstdllFreqItems", lstdllFreqItems);
            }
            _dicCurrentNums = (Dictionary<string, int>)ViewState["CurrentNums"];
        }

        private void ShowResult(StuGLSearch stuGLSearch)
        {

            StuGLSearch stuSearchTemp = stuGLSearch;
            if (ddlFreq.SelectedValue == "gen")
            {
                stuSearchTemp.NextNumsMode = false;
            }
            else
            {
                stuSearchTemp.NextNumsMode = true;
                stuSearchTemp.StrNextNums = ddlFreq.SelectedValue;
            }

            stuSearchTemp = new CglMethod().GetMethodSN(stuSearchTemp);
            stuSearchTemp = new CglMethod().GetSearchMethodSN(stuSearchTemp);
            stuSearchTemp = new CglMethod().GetSecFieldSN(stuSearchTemp);

            ShowFreq(stuSearchTemp, ddlFreq.SelectedValue);
            ShowFreqProcess(stuSearchTemp, ddlFreq.SelectedValue);

            ShowFreqFilter01(stuSearchTemp, ddlFreq.SelectedValue);
            ShowFilte01Process(stuSearchTemp, ddlFreq.SelectedValue);

        }

        private void SetddlFreq(StuGLSearch stuSearch)
        {
            if (ddlFreq.Items.Count == 0)
            {
                if (stuSearch.FieldMode)
                {
                    ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(stuSearch.StrCompares), stuSearch.StrCompares));
                }
                else
                {
                    ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId("gen"), "gen"));
                }
                ddlNexts.Visible = false;
            }

            if (stuSearch.NextNumsMode && ddlNexts.Items.Count == 0)
            {
                ddlNexts.Visible = true;
                ddlNexts.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId("gen"), "gen"));
                stuSearch = new CglSearch().InitSearch(stuSearch);
                List<string> NextNums = stuSearch.StrNextNumT.Split(separator: ';').ToList();
                foreach (string Nums in NextNums)
                {
                    ddlNexts.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(Nums), Nums));
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowFreq(StuGLSearch stuSearchTemp, string selectedValue)
        {
            if (Session[FreqResultID + selectedValue + "dtFreq"] == null)
            {
                Session.Add(FreqResultID + selectedValue + "dtFreq", new CglFreq().GetFreq(stuSearchTemp));
            }

            DtFreq = (DataTable)Session[FreqResultID + selectedValue + "dtFreq"];
            lblFreq.Text = string.Format(InvariantCulture, "{0} ({1}期)", new CglFunc().ConvertFieldNameId(selectedValue), DtFreq.Rows[0]["intRows"]);
            DtFreqView = new CglFunc().SortFreq(_gstuSearch, DtFreq);
            gvFreq.DataSource = DtFreqView.DefaultView;
            if (gvFreq.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtFreqView.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtFreqView.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtFreqView.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtFreqView.Columns[ColumnIndex].ColumnName,
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

        private void ShowFreqProcess(StuGLSearch stuSearchTemp, string selectedValue)
        {
            if (chkFreqProcess.Checked)
            {
                pnlFreqProcess.Visible = true;
                if (Session[FreqResultID + selectedValue + "dtFreqProcess"] == null)
                {
                    Session.Add(FreqResultID + selectedValue + "dtFreqProcess", CglFreqProcess.GetFreqProcs(stuSearchTemp, CglDBFreq.TableName.QryFreqProcess, SortOrder.Descending, DtFreq));
                }
                DtFreqProcess = (DataTable)Session[FreqResultID + selectedValue + "dtFreqProcess"];
                if (DtFreqProcess.Columns.Contains("lngFreqProcessSN")) { DtFreqProcess.Columns.Remove("lngFreqProcessSN"); }
                if (DtFreqProcess.Columns.Contains("lngFreqSN")) { DtFreqProcess.Columns.Remove("lngFreqSN"); }
                gvFreqProcess.DataSource = new CglFunc().CTableShow(DtFreqProcess).DefaultView;
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
                #region Set Columns of DataGrid gvProcess
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

        // ---------------------------------------------------------------------------------------------------------

        private void ShowFreqFilter01(StuGLSearch stuSearchTemp, string selectedValue)
        {
            if (Session[FreqResultID + selectedValue + "dtFreqFilter01"] == null) { Session.Add(FreqResultID + selectedValue + "dtFreqFilter01", new CglFreqFilter().GetFreqFilter(stuSearchTemp, CglFreqFilter.TableName.QryFreqFilter01)); }

            dtFreqFilter01 = (DataTable)Session[FreqResultID + selectedValue + "dtFreqFilter01"];
            if (dtFreqFilter01.Columns.Contains("lngDateSN")) { dtFreqFilter01.Columns.Remove("lngDateSN"); }
            gvFreqFilter01.DataSource = dtFreqFilter01.DefaultView;
            if (gvFreqFilter01.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < dtFreqFilter01.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = dtFreqFilter01.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(dtFreqFilter01.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = dtFreqFilter01.Columns[ColumnIndex].ColumnName,
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

        private void ShowFilte01Process(StuGLSearch stuSearchTemp, string selectedValue)
        {
            if (chkFreqFilter01Process.Checked)
            {
                pnlFreqFilter01Process.Visible = true;
                if (Session[FreqResultID + selectedValue + "dtFreqFilter01Process"] == null) { Session.Add(FreqResultID + selectedValue + "dtFreqFilter01Process", new CglFreqFilter().GetFreqFilterProcess(stuSearchTemp, CglFreqFilter.TableName.QryFreqFilter01)); }
                dtFreqFilter01Process = (DataTable)Session[FreqResultID + selectedValue + "dtFreqFilter01Process"];
                if (dtFreqFilter01Process.Columns.Contains("strcheck")) { dtFreqFilter01Process.Columns.Remove("strcheck"); }
                gvFreqFilter01Process.DataSource = dtFreqFilter01Process.DefaultView;
                if (gvFreqFilter01Process.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < dtFreqFilter01Process.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = dtFreqFilter01Process.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(dtFreqFilter01Process.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = dtFreqFilter01Process.Columns[ColumnIndex].ColumnName,
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
                pnlFreqFilter01Process.Visible = false;
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

        // ---------------------------------------------------------------------------------------------------------
        protected void BtnCloseClick(object sender, EventArgs e)
        {
            ViewState.Clear();
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
        }

        //protected void IBInfoClick(object sender, ImageClickEventArgs e)
        //{
        //    pnlTopData.Visible = !pnlTopData.Visible;
        //}

        // ---------------------------------------------------------------------------------------------------------

        protected void Timer1Tick(object sender, EventArgs e)
        {
            //if (CheckThread()) { Timer1.Interval = 360000; };
        }

        //private bool CheckThread()
        //{
        //    if (!DicThreadFreqResult.Keys.Contains(FreqResultID)) { CreatThread(); }
        //    Thread ThreadFreqActiveHSingle = (Thread)DicThreadFreqResult[FreqResultID];
        //    if (ThreadFreqActiveHSingle.IsAlive)
        //    {
        //        lblArgument.Text = Properties.Resources.Updating;
        //    }
        //    else
        //    {
        //        lblArgument.Text = "";
        //        //SetddlFreq();
        //        ResetSearchOrder(FreqResultID);
        //        Timer1.Enabled = false;
        //    }
        //    return !ThreadFreqActiveHSingle.IsAlive;
        //}

        //private void CreatThread()
        //{
        //    Thread ThreadFreqResult = new Thread(() =>
        //    {
        //        if (_gstuSearch.NextNumsMode)
        //        {
        //            //lstdllFreqItems = (List<string>)Session[FreqResultID + "lstdllFreqItems"];
        //            List<string> NextNums = _gstuSearch.StrNextNumT.Split(separator: ';').ToList();
        //            foreach (string Nums in NextNums)
        //            {
        //                StuGLSearch stuSearchTemp = _gstuSearch;
        //                stuSearchTemp.NextNumsMode = true;
        //                stuSearchTemp.StrNextNums = Nums;
        //                stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
        //                stuSearchTemp = new CglMethod().GetMethodSN(stuSearchTemp);
        //                stuSearchTemp = new CglMethod().GetSearchMethodSN(stuSearchTemp);
        //                stuSearchTemp = new CglMethod().GetSecFieldSN(stuSearchTemp);
        //                Session.Add(FreqResultID + Nums + "dtFreq", new CglFreq().GetFreq(stuSearchTemp));
        //                Session.Add(FreqResultID + Nums + "dtFreqProcess", new CglFreqProcess().GetFreqProcs(stuSearchTemp, CglDBFreq.TableName.QryFreqProcess, SortOrder.Descending, DtFreq));
        //                Session.Add(FreqResultID + Nums + "dtFreqFilter01", new CglFreqFilter().GetFreqFilter(stuSearchTemp, CglFreqFilter.TableName.QryFreqFilter01));
        //                Session.Add(FreqResultID + Nums + "dtFreqFilter01Process", new CglFreqFilter().GetFreqFilterProcess(stuSearchTemp, CglFreqFilter.TableName.QryFreqFilter01));
        //                //lstdllFreqItems.Add(Nums);
        //            }
        //            //Session[FreqResultID + "lstdllFreqItems"] = lstdllFreqItems;
        //        }
        //    })
        //    {
        //        Name = FreqResultID
        //    };
        //    ThreadFreqResult.Start();

        //    DicThreadFreqResult.Add(FreqResultID, ThreadFreqResult);
        //}
    }
}
