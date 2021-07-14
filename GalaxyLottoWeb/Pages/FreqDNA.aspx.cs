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
    public partial class FreqDna : BasePage
    {
        private StuGLSearch _gstuSearch;

        private string _action;
        private string _requestId;
        private string FreqDNAID;

        //private Dictionary<string, int> _dicCurrentNums = new Dictionary<string, int>();
        private Dictionary<string, string> _dicNumcssclass = new Dictionary<string, string>();
        private DataTable DtFreqDna01 { get; set; }

        private static Dictionary<string, object> DicThreadFreqDna
        {
            get { if (dicThreadFreqDna == null) { dicThreadFreqDna = new Dictionary<string, object>(); } return dicThreadFreqDna; }
            set => dicThreadFreqDna = value;
        }

        private static Dictionary<string, object> dicThreadFreqDna = new Dictionary<string, object>();

        private Thread Thread01, Thread02;

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            if (ViewState[FreqDNAID + "_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState[FreqDNAID + "_gstuSearch"];
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

            FreqDNAID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqDNAID] != null)
            {
                if (ViewState[FreqDNAID + "_gstuSearch"] == null)
                {
                    ViewState.Add(FreqDNAID + "_gstuSearch", (StuGLSearch)Session[FreqDNAID]);
                }
                else
                {
                    ViewState[FreqDNAID + "_gstuSearch"] = (StuGLSearch)Session[FreqDNAID];
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
            ShowFreqDna01(ddlFreq.SelectedValue);
            //ShowFreqDna(ddlFreq.SelectedValue);
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
            if (Session[FreqDNAID + "ddlFreq"] != null)
            {
                if (ddlFreq.Items.Count == 0)
                {
                    foreach (string strFieldName in (List<string>)Session[FreqDNAID + "ddlFreq"])
                    {
                        ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(strFieldName), strFieldName));
                    }
                }
            }
            else
            {
                if (!DicThreadFreqDna.Keys.Contains(FreqDNAID + "T01")) { CreatThread(); }
            }
        }

        private List<string> GetddlFreq()
        {
            if (Session[FreqDNAID + "ddlFreq"] == null)
            {
                List<string> lstShowddlFreq = new List<string> { "gen" };
                if (_gstuSearch.NextNumsMode)
                {
                    foreach (string item in _gstuSearch.StrNextNumT.Split(separator: ';').ToList()) { lstShowddlFreq.Add(item); }
                }
                Session.Add(FreqDNAID + "ddlFreq", lstShowddlFreq);
                return lstShowddlFreq;
            }
            else
            {
                return (List<string>)Session[FreqDNAID + "ddlFreq"];
            }
        }
        // ---------------------------------------------------------------------------------------------------------

        //private void ShowFreqDna(string selectedValue)
        //{
        //    if (Session[FreqDNAID + "dsFreqDna"] != null && ((Dictionary<string, DataSet>)Session[FreqDNAID + "dsFreqDna"]).ContainsKey(selectedValue))
        //    {
        //        lblFreqDNA.Text = string.Format(InvariantCulture, "DNA長度： {0} ", _gstuSearch.InFreqDnaLength);
        //        //dtFreqDna = ConvertDt(((Dictionary<string, DataSet>)Session[FreqDNAID + "dsFreqDna"])[selectedValue].Tables["dtFreqDna"].Select("", "[lngN] ASC").CopyToDataTable());
        //        dtFreqDna = ((Dictionary<string, DataSet>)Session[FreqDNAID + "dsFreqDna"])[selectedValue].Tables["dtFreqDna"].Select("", "[lngN] ASC").CopyToDataTable();
        //        gvFreqDNA.DataSource = dtFreqDna.DefaultView;
        //        if (gvFreqDNA.Columns.Count == 0)
        //        {
        //            for (int ColumnIndex = 0; ColumnIndex < dtFreqDna.Columns.Count; ColumnIndex++)
        //            {
        //                BoundField bfCell = new BoundField()
        //                {
        //                    DataField = dtFreqDna.Columns[ColumnIndex].ColumnName,
        //                    HeaderText = new CglFunc().ConvertFieldNameId(dtFreqDna.Columns[ColumnIndex].ColumnName, 1),
        //                    SortExpression = dtFreqDna.Columns[ColumnIndex].ColumnName,
        //                };
        //                gvFreqDNA.Columns.Add(bfCell);
        //            }
        //            //foreach (DataControlField dcColumn in gvFreqDNA.Columns)
        //            //{
        //            //    string strColumnName = dcColumn.SortExpression;
        //            //    if (strColumnName != "Item" && _dicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
        //            //    {
        //            //        dcColumn.HeaderStyle.CssClass = _dicNumcssclass[strColumnName.Substring(4)];
        //            //    }
        //            //}
        //        }
        //        gvFreqDNA.RowDataBound += GvFreqDNA_RowDataBound;
        //        gvFreqDNA.DataBind();
        //    }
        //}

        //private void GvFreqDNA_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        foreach (DataControlFieldCell cell in e.Row.Cells)
        //        {
        //            if (cell.ContainingField is BoundField)
        //            {
        //                string strCell_ColumnName = ((BoundField)cell.ContainingField).DataField;

        //                #region Set lngN
        //                if (strCell_ColumnName == "lngN" && _dicNumcssclass.ContainsKey(cell.Text))
        //                {
        //                    cell.CssClass = _dicNumcssclass[cell.Text];
        //                    //  e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit";
        //                }
        //                #endregion Set lngN
        //            }
        //        }
        //    }
        //}

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
        private void ShowFreqDna01(string selectedValue)
        {
            if (Session[FreqDNAID + "dsFreqDna"] != null && ((Dictionary<string, DataSet>)Session[FreqDNAID + "dsFreqDna"]).ContainsKey(selectedValue))
            {
                lblFreqDNA01.Text = string.Format(InvariantCulture, " DNA驗證期數: {0} ", _gstuSearch.InTargetTestPeriods);
                //dtFreqDna = ConvertDt(((Dictionary<string, DataSet>)Session[FreqDNAID + "dsFreqDna"])[selectedValue].Tables["dtFreqDna"].Select("", "[lngN] ASC").CopyToDataTable());
                DtFreqDna01 = ((Dictionary<string, DataSet>)Session[FreqDNAID + "dsFreqDna"])[selectedValue].Tables["dtFreqDna01"].Select("", "[intHistoryHitRate] DESC").CopyToDataTable();
                DtFreqDna01.Columns.Remove("lngFreqDNA01SN");
                DtFreqDna01.Columns.Remove("lngFreqDNASN");
                DtFreqDna01.Columns.Remove("lngTotalSN");
                DtFreqDna01.Columns.Remove("lngMethodSN");
                DtFreqDna01.Columns.Remove("intDNALength");
                gvFreqDNA01.DataSource = DtFreqDna01.DefaultView;
                if (gvFreqDNA01.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < DtFreqDna01.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqDna01.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqDna01.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = DtFreqDna01.Columns[ColumnIndex].ColumnName,
                        };
                        gvFreqDNA01.Columns.Add(bfCell);
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
                gvFreqDNA01.RowDataBound += GvFreqDNA01_RowDataBound;
                gvFreqDNA01.DataBind();
            }
        }

        private void GvFreqDNA01_RowDataBound(object sender, GridViewRowEventArgs e)
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

        // ---------------------------------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Suspend")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Resume")]
        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            Thread ThreadFreqActiveHT01 = (Thread)DicThreadFreqDna[FreqDNAID + "T01"];
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
            Thread ThreadFreqActiveHT02 = (Thread)DicThreadFreqDna[FreqDNAID + "T02"];
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
            Session.Remove(FreqDNAID);
            ViewState.Remove(FreqDNAID + "_gstuSearch");
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
            if (DicThreadFreqDna.Keys.Contains(FreqDNAID + "T01"))
            {
                Thread01 = (Thread)DicThreadFreqDna[FreqDNAID + "T01"];
                if (Thread01.IsAlive)
                {
                    Timer1.Enabled = true;
                    Timer1.Interval = 10000;
                    lblArgument.Text = string.Format(InvariantCulture, "{0} ", DateTime.Now.ToLongTimeString());
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0}{1} ", new GalaxyApp().GetTheadState(Thread01.ThreadState), Session[FreqDNAID + "lblT01"].ToString());
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

            if (DicThreadFreqDna.Keys.Contains(FreqDNAID + "T02"))
            {
                Thread02 = (Thread)DicThreadFreqDna[FreqDNAID + "T02"];
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
            Thread01 = new Thread(() => { StartThread01(); ResetSearchOrder(FreqDNAID); }) { Name = FreqDNAID };
            Thread01.Start();
            DicThreadFreqDna.Add(FreqDNAID + "T01", Thread01);
        }

        private void StartThread01()
        {
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
            if (Session[FreqDNAID + "dsFreqDna"] == null)
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
                    dicFreqDna.Add(item, GetdsFreqDna(stuSearchTemp));
                }
                Session.Add(FreqDNAID + "dsFreqDna", dicFreqDna);
            }
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
        }

        private DataSet GetdsFreqDna(StuGLSearch stuGLSearch)
        {
            CheckData = true;
            DataSet dsReturn = null;
            using (DataSet dsFreqDna = new DataSet())
            {
                dsFreqDna.Locale = InvariantCulture;
                Session[FreqDNAID + "lblT01"] = "1/2 Get tblFreqDna";
                DataTable dtFreqDnaTemp = new CglFreqDna().GetFreqDna(stuGLSearch, CglFreqDna.TableName.QryFreqDna);
                dtFreqDnaTemp.TableName = "dtFreqDna";
                dsFreqDna.Tables.Add(dtFreqDnaTemp);
                Session[FreqDNAID + "lblT01"] = "2/2 Get tblFreqDna01";
                DataTable dtFreqDna01Temp = new CglFreqDna01().GetFreqDna01(stuGLSearch, CglFreqDna.TableName.QryFreqDna01);
                dtFreqDna01Temp.TableName = "dtFreqDna01";
                dsFreqDna.Tables.Add(dtFreqDna01Temp);

                dsReturn = dsFreqDna;
            }
            return dsReturn;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Resume")]
        private void ReleaseMemory()
        {
            Session.Remove(FreqDNAID + "ddlFreq");
            Session.Remove(FreqDNAID + "dsFreqDna");
            Session.Remove(FreqDNAID + "lblT01");
            Session.Remove(FreqDNAID + "lblT02");
            ResetSearchOrder(FreqDNAID);
            if (DicThreadFreqDna.Keys.Contains(FreqDNAID + "T01"))
            {
                Thread ThreadFreqActiveHT01 = (Thread)DicThreadFreqDna[FreqDNAID + "T01"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActiveHT01.ThreadState == ThreadState.Suspended) { ThreadFreqActiveHT01.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT01.Abort();
                ThreadFreqActiveHT01.Join();
                DicThreadFreqDna.Remove(FreqDNAID + "T01");
            }

            if (DicThreadFreqDna.Keys.Contains(FreqDNAID + "T02"))
            {
                Thread ThreadFreqActiveHT02 = (Thread)DicThreadFreqDna[FreqDNAID + "T02"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActiveHT02.ThreadState == ThreadState.Suspended) { ThreadFreqActiveHT02.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT02.Abort();
                ThreadFreqActiveHT02.Join();
                DicThreadFreqDna.Remove(FreqDNAID + "T02");
            }
        }

    }
}
