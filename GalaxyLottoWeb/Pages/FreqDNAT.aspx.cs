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
    public partial class FreqDnaT : BasePage
    {
        private StuGLSearch _gstuSearch;

        private string _action;
        private string _requestId;
        private string FreqDNATID;

        //private Dictionary<string, int> _dicCurrentNums = new Dictionary<string, int>();
        private Dictionary<string, string> _dicNumcssclass = new Dictionary<string, string>();

        private DataTable DtFreqSum { get; set; }
        private DataTable DtFreqDna01 { get; set; }

        private static Dictionary<string, object> DicThreadFreqDna
        {
            get
            { if (dicThreadFreqDna == null) { dicThreadFreqDna = new Dictionary<string, object>(); } return dicThreadFreqDna; }
            set => dicThreadFreqDna = value;
        }

        private static Dictionary<string, object> dicThreadFreqDna = new Dictionary<string, object>();
        private Thread Thread01, Thread02;

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            if (ViewState[FreqDNATID + "_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState[FreqDNATID + "_gstuSearch"];
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

            FreqDNATID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqDNATID] != null)
            {
                if (ViewState[FreqDNATID + "_gstuSearch"] == null)
                {
                    ViewState.Add(FreqDNATID + "_gstuSearch", (StuGLSearch)Session[FreqDNATID]);
                }
                else
                {
                    ViewState[FreqDNATID + "_gstuSearch"] = (StuGLSearch)Session[FreqDNATID];
                };
            }
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        private void InitialArgument()
        {
            if (ViewState["title"] == null) { ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "數字DNA總表", new CglData().SetTitleString(_gstuSearch))); }
            if (ViewState["lblMethod"] == null) { ViewState.Add("lblMethod", new CglMethod().SetMethodString(_gstuSearch)); }
            if (ViewState["lblSearchMethod"] == null) { ViewState.Add("lblSearchMethod", new CglMethod().SetSearchMethodString(_gstuSearch)); }
            if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch))); }

            //if (ViewState["CurrentNums"] == null) { ViewState.Add("CurrentNums", new CglData().GetDataNumsDici(_gstuSearch)); }
            //_dicCurrentNums = (Dictionary<string, int>)ViewState["CurrentNums"];

            if (ViewState["_dicNumcssclass"] == null) { ViewState.Add("_dicNumcssclass", new GalaxyApp().GetNumcssclass(_gstuSearch)); }
            _dicNumcssclass = (Dictionary<string, string>)ViewState["_dicNumcssclass"];
            if (Session[FreqDNATID + "lblT01"] == null) { Session.Add(FreqDNATID + "lblT01", ""); }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowResult()
        {
            ShowTitle();
            ShowddlFreq();
            ShowData(ddlFreq.SelectedValue);
        }

        private void ShowData(string selectedValue)
        {
            pnlSum.Visible = false;
            pnlFreqDNA01.Visible = false;
            switch (selectedValue)
            {
                case "Sum":
                    ShowSumData();
                    break;
                default:
                    ShowFreqDna01(ddlFreq.SelectedValue);
                    break;
            }
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
            if (Session[FreqDNATID + "ddlFreq"] != null)
            {
                if (ddlFreq.Items.Count == 0)
                {
                    foreach (string strItem in (List<string>)Session[FreqDNATID + "ddlFreq"])
                    {
                        ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(strItem), strItem));
                    }
                }
                lblFreq.Text = string.Format(InvariantCulture, "{0}:{1}", ddlFreq.Items.Count - 1, ddlFreq.SelectedItem.Text);
            }
            else
            {
                if (!DicThreadFreqDna.Keys.Contains(FreqDNATID + "T01")) { CreatThread(); }
            }
        }

        private List<string> GetddlFreq()
        {
            if (Session[FreqDNATID + "ddlFreq"] == null)
            {
                List<string> Fields = (List<string>)new CglValidFields().GetValidFieldsLst(_gstuSearch);
                List<string> lstShowddlFields = new List<string> { "Sum" };
                foreach (string strItem in Fields) { lstShowddlFields.Add(strItem); }
                Session.Add(FreqDNATID + "ddlFreq", lstShowddlFields);
                return lstShowddlFields;
            }
            else
            {
                return (List<string>)Session[FreqDNATID + "ddlFreq"];
            }
        }
        // ---------------------------------------------------------------------------------------------------------

        private void ShowSumData()
        {
            if (Session[FreqDNATID + "FreqDna01" + "Sum"] != null)
            {
                pnlSum.Visible = true;
                lblSum.Text = string.Format(InvariantCulture, " 總表 DNA驗證期數: {0} ", _gstuSearch.InTargetTestPeriods);
                DtFreqSum = ((DataTable)Session[FreqDNATID + "FreqDna01" + "Sum"]).Select("", "[dblSum] DESC").CopyToDataTable();

                gvSum.DataSource = DtFreqSum.DefaultView;
                if (gvSum.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < DtFreqSum.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqSum.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqSum.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = DtFreqSum.Columns[ColumnIndex].ColumnName,
                        };
                        gvSum.Columns.Add(bfCell);
                    }
                }
                gvSum.RowDataBound += GvFreqDNA01_RowDataBound; ;
                gvSum.DataBind();
            }

        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowFreqDna01(string selectedValue)
        {
            if (Session[FreqDNATID + "FreqDna01" + selectedValue] != null)
            {
                pnlFreqDNA01.Visible = true;
                lblFreqDNA01.Text = string.Format(InvariantCulture, " DNA驗證期數: {0} ", _gstuSearch.InTargetTestPeriods);
                DtFreqDna01 = ((DataTable)Session[FreqDNATID + "FreqDna01" + selectedValue]).Select("", "[intHistoryHitRate] DESC").CopyToDataTable();
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
            Thread ThreadFreqActiveHT01 = (Thread)DicThreadFreqDna[FreqDNATID + "T01"];
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
            Thread ThreadFreqActiveHT02 = (Thread)DicThreadFreqDna[FreqDNATID + "T02"];
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
            Session.Remove(FreqDNATID);
            ViewState.Remove(FreqDNATID + "_gstuSearch");
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
            if (DicThreadFreqDna.Keys.Contains(FreqDNATID + "T01"))
            {
                Thread01 = (Thread)DicThreadFreqDna[FreqDNATID + "T01"];
                if (Thread01.IsAlive)
                {
                    Timer1.Enabled = true;
                    Timer1.Interval = 10000;
                    lblArgument.Text = string.Format(InvariantCulture, "{0} ", DateTime.Now.ToLongTimeString());
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0}{1} ", new GalaxyApp().GetTheadState(Thread01.ThreadState), Session[FreqDNATID + "lblT01"].ToString());
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

            if (DicThreadFreqDna.Keys.Contains(FreqDNATID + "T02"))
            {
                Thread02 = (Thread)DicThreadFreqDna[FreqDNATID + "T02"];
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
            Thread01 = new Thread(() => { StartThread01(); ResetSearchOrder(FreqDNATID); }) { Name = FreqDNATID };
            Thread01.Start();
            DicThreadFreqDna.Add(FreqDNATID + "T01", Thread01);
        }

        private void StartThread01()
        {
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
            using DataTable dtSum = CreatTableSum();
            foreach (string strItem in GetddlFreq())
            {
                if (strItem != "Sum")
                {
                    StuGLSearch stuSearchTemp = _gstuSearch;
                    stuSearchTemp.FieldMode = strItem != "gen";
                    stuSearchTemp.StrCompares = strItem != "gen" ? strItem : "gen";
                    stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
                    stuSearchTemp = new CglMethod().GetMethodSN(stuSearchTemp);
                    stuSearchTemp = new CglMethod().GetSearchMethodSN(stuSearchTemp);
                    stuSearchTemp = new CglMethod().GetSecFieldSN(stuSearchTemp);
                    Session[FreqDNATID + "lblT01"] = string.Format(InvariantCulture, "Get {0} tblFreqDna01", new CglFunc().ConvertFieldNameId(strItem, 1));
                    using DataTable dtFreqDna01Temp = new CglFreqDna01().GetFreqDna01(stuSearchTemp, CglFreqDna.TableName.QryFreqDna01);
                    dtFreqDna01Temp.TableName = "dtFreqDna01";
                    if (Session[FreqDNATID + "FreqDna01" + strItem] == null)
                    {
                        Session.Add(FreqDNATID + "FreqDna01" + strItem, dtFreqDna01Temp);
                    }
                    if (!dtSum.Columns.Contains(strItem))
                    {
                        dtSum.Columns.Add(new DataColumn()
                        {
                            ColumnName = strItem,
                            DataType = typeof(double),
                            DefaultValue = 0
                        });
                    }
                    foreach (DataRow drFreqDna01Temp in dtFreqDna01Temp.Rows)
                    {
                        DataRow drFindRow = dtSum.Rows.Find(drFreqDna01Temp["lngN"]);
                        if (drFindRow == null)
                        {
                            drFindRow = dtSum.NewRow();
                            drFindRow["lngN"] = drFreqDna01Temp["lngN"];
                            drFindRow[strItem] = drFreqDna01Temp["intHistoryHitRate"];
                            drFindRow["dblSum"] = (double)drFindRow["dblSum"] + (double)drFreqDna01Temp["intHistoryHitRate"];
                            dtSum.Rows.Add(drFindRow);
                        }
                        else
                        {
                            drFindRow[strItem] = drFreqDna01Temp["intHistoryHitRate"];
                            drFindRow["dblSum"] = (double)drFindRow["dblSum"] + (double)drFreqDna01Temp["intHistoryHitRate"];
                        }
                    }
                }
            }
            if (Session[FreqDNATID + "FreqDna01" + "Sum"] == null)
            {
                Session.Add(FreqDNATID + "FreqDna01" + "Sum", dtSum);
            }
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
        }

        private static DataTable CreatTableSum()
        {
            using DataTable dtReturn = new DataTable { Locale = InvariantCulture };
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = "lngN",
                DataType = typeof(int),
                AllowDBNull = false,
                Unique = true
            });
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = "dblSum",
                DataType = typeof(double),
                DefaultValue = 0
            });
            dtReturn.PrimaryKey = new DataColumn[] { dtReturn.Columns["lngN"] };
            return dtReturn.Copy();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Resume")]
        private void ReleaseMemory()
        {
            Session.Remove(FreqDNATID + "ddlFreq");
            Session.Remove(FreqDNATID + "FreqDna01");
            Session.Remove(FreqDNATID + "lblT01");
            Session.Remove(FreqDNATID + "lblT02");
            ResetSearchOrder(FreqDNATID);
            if (DicThreadFreqDna.Keys.Contains(FreqDNATID + "T01"))
            {
                Thread ThreadFreqActiveHT01 = (Thread)DicThreadFreqDna[FreqDNATID + "T01"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActiveHT01.ThreadState == ThreadState.Suspended) { ThreadFreqActiveHT01.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT01.Abort();
                ThreadFreqActiveHT01.Join();
                DicThreadFreqDna.Remove(FreqDNATID + "T01");
            }

            if (DicThreadFreqDna.Keys.Contains(FreqDNATID + "T02"))
            {
                Thread ThreadFreqActiveHT02 = (Thread)DicThreadFreqDna[FreqDNATID + "T02"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActiveHT02.ThreadState == ThreadState.Suspended) { ThreadFreqActiveHT02.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT02.Abort();
                ThreadFreqActiveHT02.Join();
                DicThreadFreqDna.Remove(FreqDNATID + "T02");
            }
        }

    }
}
