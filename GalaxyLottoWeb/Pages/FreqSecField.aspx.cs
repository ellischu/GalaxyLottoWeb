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
    public partial class FreqSecField : BasePage
    {
        private StuGLSearch _gstuSearch;

        private string _action;
        private string _requestId;
        private string FreqSecFieldID;
        private Dictionary<string, string> _dicNumcssclass = new Dictionary<string, string>();

        private DataTable DtFreqSec { get; set; }
        private DataTable DtFreqSecField { get; set; }

        private static Dictionary<string, object> DicThreadFreqDna
        {
            get
            {
                if (dicThreadFreqDna == null) { dicThreadFreqDna = new Dictionary<string, object>(); }
                return dicThreadFreqDna;
            }
            set => dicThreadFreqDna = value;
        }
        private static Dictionary<string, object> dicThreadFreqDna;

        private Thread Thread01, Thread02;

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            if (ViewState[FreqSecFieldID + "_gstuSearch"] == null || ((StuGLSearch)ViewState[FreqSecFieldID + "_gstuSearch"]).StrSecField == "none")
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState[FreqSecFieldID + "_gstuSearch"];
                InitialArgument();
                ShowResult();
            }
        }

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            FreqSecFieldID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqSecFieldID] != null)
            {
                if (ViewState[FreqSecFieldID + "_gstuSearch"] == null)
                {
                    ViewState.Add(FreqSecFieldID + "_gstuSearch", (StuGLSearch)Session[FreqSecFieldID]);
                }
                else
                {
                    ViewState[FreqSecFieldID + "_gstuSearch"] = (StuGLSearch)Session[FreqSecFieldID];
                };
            }
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        private void InitialArgument()
        {
            if (ViewState["title"] == null) { ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "數字DNA", new CglData().SetTitleString(_gstuSearch))); }
            if (ViewState["lblMethod"] == null) { ViewState.Add("lblMethod", new CglMethod().SetMethodString(_gstuSearch)); }
            if (ViewState["lblSearchMethod"] == null) { ViewState.Add("lblSearchMethod", new CglMethod().SetSearchMethodString(_gstuSearch)); }
            if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch))); }

            //if (ViewState["CurrentNums"] == null) { ViewState.Add("CurrentNums", new CglData().GetDataNumsDici(_gstuSearch)); }
            //_dicCurrentNums = (Dictionary<string, int>)ViewState["CurrentNums"];

            if (ViewState["_dicNumcssclass"] == null) { ViewState.Add("_dicNumcssclass", new GalaxyApp().GetNumcssclass(_gstuSearch)); }
            _dicNumcssclass = (Dictionary<string, string>)ViewState["_dicNumcssclass"];
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowResult()
        {
            ShowTitle();
            ShowddlFreq();
            ShowFreqSecField(ddlFreq.SelectedValue);
            ShowFreqSec(ddlFreq.SelectedValue);
        }

        private void ShowTitle()
        {
            Page.Title = (string)ViewState["title"];
            lblTitle.Text = (string)ViewState["title"];
            lblMethod.Text = (string)ViewState["lblMethod"];
            lblSearchMethod.Text = (string)ViewState["lblSearchMethod"];

            //顯示當前資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowddlFreq()
        {
            if (Session[FreqSecFieldID + "ddlFreq"] != null)
            {
                if (ddlFreq.Items.Count == 0)
                {
                    foreach (string strFieldName in (List<string>)Session[FreqSecFieldID + "ddlFreq"])
                    {
                        ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(strFieldName), strFieldName));
                    }
                }
            }
            else
            {
                if (!DicThreadFreqDna.Keys.Contains(FreqSecFieldID + "T01")) { CreatThread(); }
            }
        }

        private List<string> GetddlFreq()
        {
            if (Session[FreqSecFieldID + "ddlFreq"] == null)
            {
                List<string> lstShowddlFreq = new List<string> { "gen" };
                if (_gstuSearch.NextNumsMode)
                {
                    foreach (string item in _gstuSearch.StrNextNumT.Split(separator: ';').ToList()) { lstShowddlFreq.Add(item); }
                }
                Session.Add(FreqSecFieldID + "ddlFreq", lstShowddlFreq);
                return lstShowddlFreq;
            }
            else
            {
                return (List<string>)Session[FreqSecFieldID + "ddlFreq"];
            }
        }
        // ---------------------------------------------------------------------------------------------------------

        private void ShowFreqSec(string selectedValue)
        {
            if (Session[FreqSecFieldID + "dsFreqSecField"] != null && ((Dictionary<string, DataSet>)Session[FreqSecFieldID + "dsFreqSecField"]).ContainsKey(selectedValue))
            {
                //lblFreqSec.Text = string.Format(InvariantCulture, "數字區間表： {0} ", _gstuSearch.InFreqDnaLength);
                //dtFreqDna = ConvertDt(((Dictionary<string, DataSet>)Session[FreqDNAID + "dsFreqSecField"])[selectedValue].Tables["dtFreqDna"].Select("", "[lngN] ASC").CopyToDataTable());
                DtFreqSec = ((Dictionary<string, DataSet>)Session[FreqSecFieldID + "dsFreqSecField"])[selectedValue].Tables["dtFreqSec"].Select("", "[lngN] ASC").CopyToDataTable();
                gvFreqSec.DataSource = DtFreqSec.DefaultView;
                if (gvFreqSec.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < DtFreqSec.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqSec.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqSec.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = DtFreqSec.Columns[ColumnIndex].ColumnName,
                        };
                        gvFreqSec.Columns.Add(bfCell);
                    }
                    //foreach (DataControlField dcColumn in gvFreqDNA.Columns)
                    //{
                    //    string strColumnName = dcColumn.SortExpression;
                    //    if (strColumnName != "Item" && _dicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                    //    {
                    //        dcColumn.HeaderStyle.CssClass = _dicNumcssclass[strColumnName.Substring(4)];
                    //    }
                    //}
                }
                gvFreqSec.RowDataBound += GvFreqSec_RowDataBound;
                gvFreqSec.DataBind();
            }
        }

        private void GvFreqSec_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        #region Set lngN
                        if (strCell_ColumnName == "lngN" && _dicNumcssclass.ContainsKey(cell.Text))
                        {
                            cell.CssClass = _dicNumcssclass[cell.Text];
                            //  e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit";
                        }
                        #endregion Set lngN
                    }
                }
            }
        }

        protected DataTable ConvertDataTable(DataTable dtInput)
        {
            if (dtInput == null) { throw new ArgumentNullException(nameof(dtInput)); }

            CheckData = true;
            DataTable dtReturn = null;
            using (DataTable dtFreqDnaTemp = CreatTable())
            {
                dtFreqDnaTemp.Locale = InvariantCulture;
                DataRow drFreqDnaTemp = dtFreqDnaTemp.NewRow();
                drFreqDnaTemp["Item"] = "DNA";
                foreach (DataRow drFreqDna in dtInput.Rows)
                {
                    string strColumnName = string.Format(InvariantCulture, "lngN{0}", drFreqDna["lngN"]);
                    drFreqDnaTemp[strColumnName] = string.Format(InvariantCulture, "{0}", drFreqDna["strDNA"]);
                }
                dtFreqDnaTemp.Rows.Add(drFreqDnaTemp);

                drFreqDnaTemp = dtFreqDnaTemp.NewRow();
                drFreqDnaTemp["Item"] = "lngM";
                foreach (DataRow drFreqDna in dtInput.Rows)
                {
                    string strColumnName = string.Format(InvariantCulture, "lngN{0}", drFreqDna["lngN"]);
                    drFreqDnaTemp[strColumnName] = string.Format(InvariantCulture, "{0}", drFreqDna["lngM"]);
                }
                dtFreqDnaTemp.Rows.Add(drFreqDnaTemp);
                dtReturn = dtFreqDnaTemp;
            }
            return dtReturn;
        }

        private DataTable CreatTable()
        {
            CheckData = true;
            using DataTable dtReturn = new DataTable { Locale = InvariantCulture };
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = "Item",
                DataType = typeof(string),
                AllowDBNull = false
            });
            for (int intColum = 1; intColum <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; intColum++)
            {
                dtReturn.Columns.Add(new DataColumn()
                {
                    ColumnName = string.Format(InvariantCulture, "lngN{0}", intColum),
                    DataType = typeof(string),
                    AllowDBNull = false
                });
            }
            return dtReturn.Copy();
        }
        // ---------------------------------------------------------------------------------------------------------
        private void ShowFreqSecField(string selectedValue)
        {
            if (Session[FreqSecFieldID + "dsFreqSecField"] != null && ((Dictionary<string, DataSet>)Session[FreqSecFieldID + "dsFreqSecField"]).ContainsKey(selectedValue))
            {
                lblFreqSecField.Text = string.Format(InvariantCulture, " 驗證期數: {0} ", _gstuSearch.InTargetTestPeriods);
                //dtFreqDna = ConvertDt(((Dictionary<string, DataSet>)Session[FreqDNAID + "dsFreqSecField"])[selectedValue].Tables["dtFreqDna"].Select("", "[lngN] ASC").CopyToDataTable());
                DtFreqSecField = ((Dictionary<string, DataSet>)Session[FreqSecFieldID + "dsFreqSecField"])[selectedValue].Tables["dtFreqSecField"].Select("", "[intHistoryHitRate] DESC , [intCorrespondPeriods] DESC , [intCorrespondHistoryPeriods] DESC ").CopyToDataTable();
                DtFreqSec = ((Dictionary<string, DataSet>)Session[FreqSecFieldID + "dsFreqSecField"])[selectedValue].Tables["dtFreqSec"];
                DtFreqSecField.Columns.Remove("lngFreqSecFieldSN");
                DtFreqSecField.Columns.Remove("lngTotalSN");
                gvFreqSecField.DataSource = DtFreqSecField.DefaultView;
                if (gvFreqSecField.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < DtFreqSecField.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqSecField.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqSecField.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = DtFreqSecField.Columns[ColumnIndex].ColumnName,
                        };
                        gvFreqSecField.Columns.Add(bfCell);
                    }
                    //foreach (DataControlField dcColumn in gvFreqDNA01.Columns)
                    //{
                    //    string strColumnName = dcColumn.SortExpression;
                    //    if (strColumnName != "Item" && _dicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                    //    {
                    //        dcColumn.HeaderStyle.CssClass = _dicNumcssclass[strColumnName.Substring(4)];
                    //    }
                    //}
                }
                gvFreqSecField.RowDataBound += GvFreqSecField1_RowDataBound;
                gvFreqSecField.DataBind();
            }
        }

        private void GvFreqSecField1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        #region Set lngSecFieldSN
                        if (strCell_ColumnName == "lngSecFieldSN")
                        {
                            cell.ToolTip = _gstuSearch.StrSecField;
                            //  e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit";
                        }
                        #endregion Set lngSecFieldSN

                        #region Set lngN
                        if (strCell_ColumnName == "lngN")
                        {
                            List<int> lstField = new List<int>();
                            foreach (string strFieldItem in _gstuSearch.StrSecField.Split('#').ToList())
                            {
                                lstField.Add(int.Parse(
                                             DtFreqSec.Select(string.Format(InvariantCulture, "[lngN] = {0}",
                                             int.Parse(cell.Text, InvariantCulture)), "").CopyToDataTable().Rows[0][strFieldItem].ToString(), InvariantCulture));
                            }
                            cell.ToolTip = string.Join("#", lstField);
                            if (_dicNumcssclass.ContainsKey(cell.Text))
                            {
                                cell.CssClass = _dicNumcssclass[cell.Text];
                            }
                            //  e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit";
                        }
                        #endregion Set lngN
                    }
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Suspend")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Resume")]
        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            Thread ThreadFreqActiveHT01 = (Thread)DicThreadFreqDna[FreqSecFieldID + "T01"];
            if (ThreadFreqActiveHT01.ThreadState == ThreadState.Running)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Pause);
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT01.Suspend();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
            if (ThreadFreqActiveHT01.ThreadState == ThreadState.Suspended)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT01.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Resume")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Suspend")]
        protected void BtnT2StartClick(object sender, EventArgs e)
        {
            Thread ThreadFreqActiveHT02 = (Thread)DicThreadFreqDna[FreqSecFieldID + "T02"];
            if (ThreadFreqActiveHT02.ThreadState == ThreadState.Running)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Pause);
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT02.Suspend();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
            if (ThreadFreqActiveHT02.ThreadState == ThreadState.Suspended)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT02.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
        }

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            Session.Remove(FreqSecFieldID);
            ViewState.Remove(FreqSecFieldID + "_gstuSearch");
            ReleaseMemory();
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void Timer1Tick(object sender, EventArgs e)
        {
            CheckThread();
        }

        private void CheckThread()
        {
            if (DicThreadFreqDna.Keys.Contains(FreqSecFieldID + "T01"))
            {
                Thread01 = (Thread)DicThreadFreqDna[FreqSecFieldID + "T01"];
                if (Thread01.IsAlive)
                {
                    Timer1.Enabled = true;
                    Timer1.Interval = 10000;
                    lblArgument.Text = string.Format(InvariantCulture, "{0} ", DateTime.Now.ToLongTimeString());
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0}{1} ", new GalaxyApp().GetTheadState(Thread01.ThreadState), Session[FreqSecFieldID + "lblT01"].ToString());
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

            if (DicThreadFreqDna.Keys.Contains(FreqSecFieldID + "T02"))
            {
                Thread02 = (Thread)DicThreadFreqDna[FreqSecFieldID + "T02"];
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

        private void CreatThread()
        {
            Thread01 = new Thread(() =>
            {
                StartThread01();
                ResetSearchOrder(FreqSecFieldID);
            })
            { Name = FreqSecFieldID };
            Thread01.Start();
            DicThreadFreqDna.Add(FreqSecFieldID + "T01", Thread01);
        }

        private void StartThread01()
        {
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
            if (Session[FreqSecFieldID + "dsFreqSecField"] == null)
            {

                Dictionary<string, DataSet> dicFreqDna = new Dictionary<string, DataSet>();
                foreach (string item in GetddlFreq())
                {
                    StuGLSearch stuSearchTemp = _gstuSearch;
                    if (item == "gen")
                    {
                        stuSearchTemp.NextNumsMode = false;
                    }
                    else
                    {
                        stuSearchTemp.NextNumsMode = true;
                        stuSearchTemp.StrNextNums = item;
                    }
                    stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
                    stuSearchTemp = new CglMethod().GetMethodSN(stuSearchTemp);
                    stuSearchTemp = new CglMethod().GetSearchMethodSN(stuSearchTemp);
                    stuSearchTemp = new CglMethod().GetSecFieldSN(stuSearchTemp);
                    dicFreqDna.Add(item, GetdsFreqSecField(stuSearchTemp));
                }
                Session.Add(FreqSecFieldID + "dsFreqSecField", dicFreqDna);
            }
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
        }

        private DataSet GetdsFreqSecField(StuGLSearch stuGLSearch)
        {
            CheckData = true;
            DataSet dsReturn = null;
            using (DataSet dsFreqSec = new DataSet())
            {
                dsFreqSec.Locale = InvariantCulture;
                Session[FreqSecFieldID + "lblT01"] = "1/2 Get QryFreqSec";
                DataTable dtFreqSecTemp = new CglFreqSec().GetFreqSec(stuGLSearch, CglFreqSec.TableName.QryFreqSec, SortOrder.Descending);
                dtFreqSecTemp.TableName = "dtFreqSec";
                dsFreqSec.Tables.Add(dtFreqSecTemp);
                Session[FreqSecFieldID + "lblT01"] = "2/2 Get QryFreqSecField";
                DataTable dtFreqSecFieldTemp = new CglFreqSecField().GetFreqSecField(stuGLSearch, CglFreqSec.TableName.QryFreqSecField);
                dtFreqSecFieldTemp.TableName = "dtFreqSecField";
                dsFreqSec.Tables.Add(dtFreqSecFieldTemp);

                dsReturn = dsFreqSec;
            }
            return dsReturn;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Resume")]
        private void ReleaseMemory()
        {
            Session.Remove(FreqSecFieldID + "ddlFreq");
            Session.Remove(FreqSecFieldID + "dsFreqSecField");
            Session.Remove(FreqSecFieldID + "lblT01");
            Session.Remove(FreqSecFieldID + "lblT02");
            ResetSearchOrder(FreqSecFieldID);
            if (DicThreadFreqDna.Keys.Contains(FreqSecFieldID + "T01"))
            {
                Thread ThreadFreqActiveHT01 = (Thread)DicThreadFreqDna[FreqSecFieldID + "T01"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActiveHT01.ThreadState == ThreadState.Suspended) { ThreadFreqActiveHT01.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT01.Abort();
                ThreadFreqActiveHT01.Join();
                DicThreadFreqDna.Remove(FreqSecFieldID + "T01");
            }

            if (DicThreadFreqDna.Keys.Contains(FreqSecFieldID + "T02"))
            {
                Thread ThreadFreqActiveHT02 = (Thread)DicThreadFreqDna[FreqSecFieldID + "T02"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActiveHT02.ThreadState == ThreadState.Suspended) { ThreadFreqActiveHT02.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT02.Abort();
                ThreadFreqActiveHT02.Join();
                DicThreadFreqDna.Remove(FreqSecFieldID + "T02");
            }
        }

    }
}
