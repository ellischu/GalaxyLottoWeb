using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Media;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqActiveH : BasePage
    {
        private StuGLSearch _gstuSearch;
        private string _action;
        private string _requestId;
        private string FreqActiveHID;
        private Dictionary<string, string> _dicNumcssclass;
        private Dictionary<string, DataSet> dicFreqSecHis;

        private DataTable DtFreqSecHis { get; set; }
        private DataTable DtFreqSecHisFilter { get; set; }
        private DataTable DtFreqSecHisProcess { get; set; }

        private static Dictionary<string, object> DicThreadFreqActiveH
        {
            get
            {
                if (dicThreadFreqActiveH == null) dicThreadFreqActiveH = new Dictionary<string, object>();
                return dicThreadFreqActiveH;
            }
            set => dicThreadFreqActiveH = value;
        }
        private static Dictionary<string, object> dicThreadFreqActiveH;

        private Thread Thread01, Thread02;

        // ---------------------------------------------------------------------------------------------------------
#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            if (ViewState[FreqActiveHID + "_gstuSearch"] == null || ((StuGLSearch)ViewState[FreqActiveHID + "_gstuSearch"]).StrSecField == "none")
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState[FreqActiveHID + "_gstuSearch"];
                InitialArgument();
                ShowddlOption();
                ShowResult(ddlOption.SelectedValue);
            }
        }

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            FreqActiveHID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[_action + _requestId] != null)
            {
                if (ViewState[FreqActiveHID + "_gstuSearch"] == null)
                {
                    ViewState.Add(FreqActiveHID + "_gstuSearch", (StuGLSearch)Session[_action + _requestId]);
                }
                else
                {
                    ViewState[FreqActiveHID + "_gstuSearch"] = (StuGLSearch)Session[_action + _requestId];
                };
            }
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowddlOption()
        {
            if (Session[FreqActiveHID + "_ddlOption"] != null)
            {
                if (ddlOption.Items.Count == 0)
                {
                    foreach (string keyval in (List<string>)Session[FreqActiveHID + "_ddlOption"])
                    {
                        if (!ddlOption.Items.Contains(new ListItem(new CglFunc().ConvertFieldNameId(keyval), keyval)))
                        {
                            ddlOption.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(keyval), keyval));
                        }
                    }
                }
            }
            else
            {
                if (!DicThreadFreqActiveH.Keys.Contains(FreqActiveHID + "T01")) { CreatThread01(); }
            }
        }

        private void SetddlOption()
        {
            if (Session[FreqActiveHID + "_ddlOption"] == null)
            {
                List<string> lstShowFields = new List<string> { "gen" };
                if (_gstuSearch.NextNumsMode)
                {
                    List<string> NextNums = _gstuSearch.StrNextNumT.Split(separator: ';').ToList();
                    foreach (string item in NextNums)
                    {
                        if (!ddlOption.Items.Contains(new ListItem(new CglFunc().ConvertFieldNameId(item), item)))
                        {
                            ddlOption.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(item), item));
                        }
                    }
                }
                Session.Add(FreqActiveHID + "_ddlOption", lstShowFields);
            };
        }

        // ---------------------------------------------------------------------------------------------------------

        private void InitialArgument()
        {
            if (ViewState[FreqActiveHID + "Title"] == null) { ViewState.Add(FreqActiveHID + "Title", string.Format(InvariantCulture, "{0}:{1}", "活性歷史表", new CglDBData().SetTitleString(_gstuSearch))); }
            if (ViewState[FreqActiveHID + "CurrentData"] == null) { ViewState.Add(FreqActiveHID + "CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch))); }
            if (ViewState[FreqActiveHID + "lblMethod"] == null) { ViewState.Add(FreqActiveHID + "lblMethod", new CglMethod().SetMethodString(_gstuSearch)); }
            if (ViewState[FreqActiveHID + "lblSearchMethod"] == null) { ViewState.Add(FreqActiveHID + "lblSearchMethod", new CglMethod().SetSearchMethodString(_gstuSearch)); }
            if (ViewState[FreqActiveHID + "_dicNumcssclass"] == null) { ViewState.Add(FreqActiveHID + "_dicNumcssclass", new GalaxyApp().GetNumcssclass(_gstuSearch)); }
            _dicNumcssclass = (Dictionary<string, string>)ViewState[FreqActiveHID + "_dicNumcssclass"];
            if (Session[FreqActiveHID + "FreqSecHis"] != null) { dicFreqSecHis = (Dictionary<string, DataSet>)Session[FreqActiveHID + "FreqSecHis"]; }
        }

        private void ShowResult(string selectedValue)
        {
            ShowTitle();
            ShowFreqSecHis(selectedValue);
            ShowFreqSecHisFilter(selectedValue);
            ShowFreqSecHisProcess(selectedValue);
        }

        private void ShowTitle()
        {
            Page.Title = (string)ViewState[FreqActiveHID + "Title"];
            lblTitle.Text = (string)ViewState[FreqActiveHID + "Title"];
            lblMethod.Text = (string)ViewState[FreqActiveHID + "lblMethod"];
            lblSearchMethod.Text = (string)ViewState[FreqActiveHID + "lblSearchMethod"];

            // 當期資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState[FreqActiveHID + "CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
        }
        // ---------------------------------------------------------------------------------------------------------
        private void ShowFreqSecHis(string selectedValue)
        {
            if (dicFreqSecHis != null && dicFreqSecHis.ContainsKey(selectedValue))
            {
                DtFreqSecHis = new CglFunc().SortFreq(_gstuSearch, dicFreqSecHis[selectedValue].Tables["FreqSecHis"]);
                gvFreqSecHis.DataSource = DtFreqSecHis.DefaultView;

                if (gvFreqSecHis.Columns.Count == 0)
                {
                    for (int i = 0; i < DtFreqSecHis.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqSecHis.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqSecHis.Columns[i].ColumnName, 1),
                            SortExpression = DtFreqSecHis.Columns[i].ColumnName,
                        };
                        gvFreqSecHis.Columns.Add(bfCell);
                    }
                }

                foreach (DataControlField dcColumn in gvFreqSecHis.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if (_dicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                    {
                        dcColumn.HeaderStyle.CssClass = _dicNumcssclass[strColumnName.Substring(4)];
                    }
                }

                gvFreqSecHis.RowDataBound += GvFreqSecHis_RowDataBound;
                gvFreqSecHis.DataBind();
            }
        }

        private void GvFreqSecHis_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        if (strCell_ColumnName == "intRows")
                        {
                            cell.Visible = false;
                        }
                    }
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowFreqSecHisFilter(string selectedValue)
        {
            if (dicFreqSecHis != null && dicFreqSecHis.ContainsKey(selectedValue))
            {
                DtFreqSecHisFilter = dicFreqSecHis[selectedValue].Tables["FreqFilter"];
                gvFreqSecHisFilter.DataSource = DtFreqSecHisFilter.DefaultView;

                if (DtFreqSecHisFilter.Columns.Contains("TestPeriods")) { DtFreqSecHisFilter.Columns.Remove("TestPeriods"); }

                if (gvFreqSecHisFilter.Columns.Count == 0)
                {
                    for (int i = 0; i < DtFreqSecHisFilter.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqSecHisFilter.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqSecHisFilter.Columns[i].ColumnName, 1),
                            SortExpression = DtFreqSecHisFilter.Columns[i].ColumnName,
                        };
                        gvFreqSecHisFilter.Columns.Add(bfCell);
                    }
                }

                gvFreqSecHisFilter.RowDataBound += GvFreqSecHisFilter_RowDataBound;
                gvFreqSecHisFilter.DataBind();
            }
        }

        private void GvFreqSecHisFilter_RowDataBound(object sender, GridViewRowEventArgs e)
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

        private void ShowFreqSecHisProcess(string selectedValue)
        {
            if (dicFreqSecHis != null && dicFreqSecHis.ContainsKey(selectedValue))
            {
                DtFreqSecHisProcess = dicFreqSecHis[selectedValue].Tables["FreqSecHisProcess"];
                gvFreqSecHisProcess.DataSource = DtFreqSecHisProcess.DefaultView;

                if (DtFreqSecHisProcess.Columns.Contains("lngFreqSecHisSN")) { DtFreqSecHisProcess.Columns.Remove("lngFreqSecHisSN"); }
                if (DtFreqSecHisProcess.Columns.Contains("lngFSHistorySN")) { DtFreqSecHisProcess.Columns.Remove("lngFSHistorySN"); }

                if (gvFreqSecHisProcess.Columns.Count == 0)
                {
                    for (int i = 0; i < DtFreqSecHisProcess.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqSecHisProcess.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqSecHisProcess.Columns[i].ColumnName, 1),
                            SortExpression = DtFreqSecHisProcess.Columns[i].ColumnName,
                        };
                        gvFreqSecHisProcess.Columns.Add(bfCell);
                    }
                }
                gvFreqSecHisProcess.DataBind();
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        protected void BtnCloseClick(object sender, EventArgs e)
        {
            ReleaseMemory();
            Session.Remove(FreqActiveHID);
            ViewState.Remove(FreqActiveHID + "_gstuSearch");
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Suspend")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Resume")]
        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            Thread01 = (Thread)DicThreadFreqActiveH[FreqActiveHID + "T01"];
            if (Thread01.ThreadState == ThreadState.Running)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Pause);
#pragma warning disable CS0618 // 類型或成員已經過時
                Thread01.Suspend();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
            if (Thread01.ThreadState == ThreadState.Suspended)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
#pragma warning disable CS0618 // 類型或成員已經過時
                Thread01.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Resume")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Suspend")]
        protected void BtnT2StartClick(object sender, EventArgs e)
        {
            Thread02 = (Thread)DicThreadFreqActiveH[FreqActiveHID + "T02"];
            if (Thread02.ThreadState == ThreadState.Running)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Pause);
#pragma warning disable CS0618 // 類型或成員已經過時
                Thread02.Suspend();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
            if (Thread02.ThreadState == ThreadState.Suspended)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
#pragma warning disable CS0618 // 類型或成員已經過時
                Thread02.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        protected void Timer1Tick(object sender, EventArgs e)
        {
            CheckThread();
        }

        //protected void ChkShowProcessCheckedChanged(object sender, EventArgs e)
        //{
        //    if (chkShowProcess.Checked && !dicThreadFreqActiveH.Keys.Contains(FreqActiveHID + "T02")) { CreatThread02(); }
        //    dtlFreqSecHisProcess.Visible = chkShowProcess.Checked ;
        //}

        private void CreatThread01()
        {
            Timer1.Enabled = true;
            Timer1.Interval = 10000;
            Thread01 = new Thread(() => { StartThread01(); }) { Name = FreqActiveHID + "T01" };
            Thread01.Start();
            DicThreadFreqActiveH.Add(FreqActiveHID + "T01", Thread01);
            CheckThread();
        }

        private void StartThread01()
        {
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
            SetddlOption();
            if (Session[FreqActiveHID + "FreqSecHis"] == null)
            {
                Session.Add(FreqActiveHID + "FreqSecHis", new CglFreqSecHis().GetFreqSecHisDic(_gstuSearch));
            }
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
            ResetSearchOrder(FreqActiveHID);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:不要將常值當做已當地語系化的參數傳遞", MessageId = "System.Web.UI.WebControls.Label.set_Text(System.String)")]
        private void CheckThread()
        {
            if (DicThreadFreqActiveH.Keys.Contains(FreqActiveHID + "T01"))
            {
                Thread01 = (Thread)DicThreadFreqActiveH[FreqActiveHID + "T01"];
                if (Thread01.IsAlive)
                {
                    Timer1.Enabled = true;
                    Timer1.Interval = 10000;
                    lblArgument.Text = string.Format(InvariantCulture, "{0} ", DateTime.Now.ToLongTimeString());
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0} ", new GalaxyApp().GetTheadState(Thread01.ThreadState));
                    lblArgument.Visible = true;
                    btnT1Start.Visible = true;
                }
                else
                {
                    lblArgument.Visible = false;
                    btnT1Start.Visible = false;
                    Timer1.Enabled = false;
                }
            }

            if (DicThreadFreqActiveH.Keys.Contains(FreqActiveHID + "T02"))
            {
                Thread02 = (Thread)DicThreadFreqActiveH[FreqActiveHID + "T02"];
                if (Thread02.IsAlive)
                {
                    Timer1.Enabled = true;
                    Timer1.Interval = 10000;
                    lblArgument.Text = string.Format(InvariantCulture, "{0} ", DateTime.Now.ToLongTimeString());
                    btnT2Start.Text = string.Format(InvariantCulture, "T2:{0} ", new GalaxyApp().GetTheadState(Thread02.ThreadState));
                    lblArgument.Visible = true;
                    btnT2Start.Visible = true;
                }
                else
                {
                    lblArgument.Visible = false;
                    btnT2Start.Visible = false;
                    Timer1.Enabled = false;
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Resume")]
        private void ReleaseMemory()
        {
            base.Session.Remove(FreqActiveHID + "FreqSecHis");
            base.Session.Remove(FreqActiveHID + "_ddlOption");

            if (DicThreadFreqActiveH.Keys.Contains(FreqActiveHID + "T01"))
            {
                Thread01 = (Thread)DicThreadFreqActiveH[FreqActiveHID + "T01"];

                if (Thread01.IsAlive)
                {
                    if (Thread01.ThreadState == ThreadState.Suspended)
                    {
#pragma warning disable CS0618 // 類型或成員已經過時
                        Thread01.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
                    }
                    Thread01.Abort();
                    Thread01.Join();
                }
                DicThreadFreqActiveH.Remove(FreqActiveHID + "T01");
            }
            if (DicThreadFreqActiveH.Keys.Contains(FreqActiveHID + "T02"))
            {
                Thread02 = (Thread)DicThreadFreqActiveH[FreqActiveHID + "T02"];

                if (Thread02.IsAlive)
                {
                    if (Thread02.ThreadState == ThreadState.Suspended)
                    {
#pragma warning disable CS0618 // 類型或成員已經過時
                        Thread02.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
                    }
                    Thread02.Abort();
                    Thread02.Join();
                }
                DicThreadFreqActiveH.Remove(FreqActiveHID + "T02");
            }
        }
    }
}