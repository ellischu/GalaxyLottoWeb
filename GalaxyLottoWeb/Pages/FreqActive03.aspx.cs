using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Media;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqActive03 : BasePage
    {
        private StuGLSearch _gstuSearch;
        private string _action;
        private string _requestId;
        private string FreqActive03ID;
        private Dictionary<string, List<int>> _dicLastNums = new Dictionary<string, List<int>>();
        private Dictionary<string, string> DicNumcssclass { set; get; }

        private static Dictionary<string, object> DicThreadFreqActive03
        {
            get
            {
                if (dicThreadFreqActive03 == null) { dicThreadFreqActive03 = new Dictionary<string, object>(); }
                return dicThreadFreqActive03;
            }
            set => dicThreadFreqActive03 = value;
        }
        private static Dictionary<string, object> dicThreadFreqActive03;

        private List<int> _lstCurrentNums;
        private readonly Dictionary<string, int> _dicSectionLimit = new Dictionary<string, int> { { "05", 3 }, { "10", 4 }, { "25", 7 }, { "50", 13 }, { "100", 21 }, };
        private DataTable dtActiveWithSection;
        private DataTable DtLastNums { get; set; }

        // ---------------------------------------------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            if (ViewState[FreqActive03ID + "_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState[FreqActive03ID + "_gstuSearch"];

                InitializeArgument(_gstuSearch);
                ShowResult();
            }

        }

        // ---------------------------------------------------------------------------------------------------------
        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            FreqActive03ID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqActive03ID] != null)
            {
                if (ViewState[FreqActive03ID + "_gstuSearch"] == null)
                {
                    ViewState.Add(FreqActive03ID + "_gstuSearch", (StuGLSearch)Session[FreqActive03ID]);
                }
                else
                {
                    ViewState[FreqActive03ID + "_gstuSearch"] = (StuGLSearch)Session[FreqActive03ID];
                };
            }

            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        private void InitializeArgument(StuGLSearch stuGLSearch)
        {

            if (ViewState["title"] == null) { ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "活性表03", new CglDBData().SetTitleString(stuGLSearch))); }
            if (ViewState["lstCurrentNums"] == null) { ViewState.Add("lstCurrentNums", new CglData().GetDataNumsLst(stuGLSearch)); }
            _lstCurrentNums = (List<int>)ViewState["lstCurrentNums"];
            if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(stuGLSearch))); }
            if (ViewState["lblMethod"] == null) { ViewState.Add("lblMethod", new CglMethod().SetMethodString(stuGLSearch)); }
            if (ViewState["lblSearchMethod"] == null) { ViewState.Add("lblSearchMethod", new CglMethod().SetSearchMethodString(stuGLSearch)); }
            if (ViewState["_dicNumcssclass"] == null) { ViewState.Add("_dicNumcssclass", new GalaxyApp().GetNumcssclass(stuGLSearch)); }
            if (ViewState["_ddlFreq"] == null) { ViewState.Add("_ddlFreq", GetdllFreq(stuGLSearch)); }
            DicNumcssclass = (Dictionary<string, string>)ViewState["_dicNumcssclass"];
            if (Session[FreqActive03ID + "select"] == null) { Session[FreqActive03ID + "select"] = string.Empty; }
            //if (ViewState["FreqSec"] == null) { ViewState.Add("FreqSec", new CglFreqSec().GetFreqSecDic(stuGLSearch)); }
            SetddlFreq();
        }

        private static List<string> GetdllFreq(StuGLSearch stuGLSearch)
        {
            List<string> lstShowFields = new List<string> { "gen" };
            if (stuGLSearch.NextNumsMode)
            {
                foreach (string Nums in stuGLSearch.StrNextNumT.Split(';').ToList())
                {
                    lstShowFields.Add(Nums);
                }
            }
            return lstShowFields;
        }

        private void SetddlFreq()
        {
            if (ddlFreq.Items.Count == 0 && ViewState["_ddlFreq"] != null)
            {
                foreach (string keyval in (List<string>)ViewState["_ddlFreq"])
                {
                    ListItem listItem = new ListItem(new CglFunc().ConvertFieldNameId(keyval), keyval);
                    if (keyval == (string)ViewState["selectedValue"]) { listItem.Selected = true; }
                    ddlFreq.Items.Add(listItem);
                }
            }
        }

        private DataTable LastNumsDictoDt(Dictionary<string, List<int>> dicLastNums)
        {
            Dictionary<int, int> dicLastNumsAll = new Dictionary<int, int>();
            for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; i++)
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
            return new CglFunc().CDicTOTable(dicLastNumTB, null);
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowResult()
        {
            Page.Title = (string)ViewState["title"];
            lblTitle.Text = (string)ViewState["title"];
            lblMethod.Text = (string)ViewState["lblMethod"];
            lblSearchMethod.Text = (string)ViewState["lblSearchMethod"];

            // 當期資料
            //pnlCurrentData.Controls.Add(child: To_Table(dtInput: new CglFunc().CDicTOTable(new CglData().GetCurrentData(stuGLSearch), null), base.ViewState[key: "title"].ToString())); //顯示當前資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            ShowLastNumsWithFreqSec(ddlFreq.SelectedValue);

            ShowActiveWithSection(ddlFreq.SelectedValue);

        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowActiveWithSection(string selectedValue)
        {
            if (Session[FreqActive03ID + "dsActiveWithSection"] != null && ((DataSet)Session[FreqActive03ID + "dsActiveWithSection"]).Tables.Contains(selectedValue))
            {
                gvActiveWithSection.Visible = true;
                dtActiveWithSection = ((DataSet)Session[FreqActive03ID + "dsActiveWithSection"]).Tables[selectedValue];
                if (dtActiveWithSection.Columns.Contains("lngFreqSecT01SN")) { dtActiveWithSection.Columns.Remove("lngFreqSecT01SN"); }
                if (dtActiveWithSection.Columns.Contains("lngFreqSecTSN")) { dtActiveWithSection.Columns.Remove("lngFreqSecTSN"); }
                if (dtActiveWithSection.Columns.Contains("lngTotalSN")) { dtActiveWithSection.Columns.Remove("lngTotalSN"); }
                if (dtActiveWithSection.Columns.Contains("lngMethodSN")) { dtActiveWithSection.Columns.Remove("lngMethodSN"); }
                if (dtActiveWithSection.Columns.Contains("lngDateSN")) { dtActiveWithSection.Columns.Remove("lngDateSN"); }
                if (dtActiveWithSection.Columns.Contains("lngFreqSecSN")) { dtActiveWithSection.Columns.Remove("lngFreqSecSN"); }
                dtActiveWithSection.DefaultView.Sort = "[lngN] ASC";
                gvActiveWithSection.DataSource = dtActiveWithSection.DefaultView;

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

                gvActiveWithSection.RowDataBound += GvActiveWithSection_RowDataBound;
                gvActiveWithSection.DataBind();
            }
            else
            {
                if (selectedValue != "select") { CreatThread(selectedValue); }
                gvActiveWithSection.Visible = false;
            }
        }

        private void GvActiveWithSection_RowDataBound(object sender, GridViewRowEventArgs e)
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
                if (_lstCurrentNums.Contains(int.Parse(dicCells["lngN"].Text, InvariantCulture)))
                {
                    dicCells["lngN"].CssClass = DicNumcssclass[dicCells["lngN"].Text];
                    e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit";
                }
                #endregion Set lngN

                #region  Set sglACT
                if (double.Parse(dicCells["sglACT"].Text, InvariantCulture) < 0)
                {
                    dicCells["sglACT"].CssClass = "sglACT glValueMax";
                }
                #endregion Set sglACT

                #region  Set sglACT1
                if (double.Parse(dicCells["sglACT1"].Text, InvariantCulture) < 0)
                {
                    dicCells["sglACT1"].CssClass = "sglACT1 glValueMax";
                }
                #endregion Set sglACT1

                foreach (int section in new int[] { 5, 10, 25, 50, 100 })
                {
                    #region sglAC
                    string strCell_ColumnName = string.Format(InvariantCulture, "sglAC{0:d2}", section);
                    if (double.Parse(dicCells[strCell_ColumnName].Text, InvariantCulture) < 0)
                    {
                        dicCells[strCell_ColumnName].CssClass = strCell_ColumnName + " glValueMax";
                    }
                    #endregion

                    #region sglFreq
                    strCell_ColumnName = string.Format(InvariantCulture, "sglFreq{0:d2}", section);
                    if (int.Parse(dicCells[strCell_ColumnName].Text, InvariantCulture) >= _dicSectionLimit[string.Format(InvariantCulture, "{0:d2}", section)] &&
                        int.Parse(dicCells[strCell_ColumnName].Text, InvariantCulture) == int.Parse(dicCells[string.Format(InvariantCulture, "intMax{0:d2}", section)].Text, InvariantCulture))
                    {

                        dicCells[strCell_ColumnName].CssClass = dicCells[strCell_ColumnName].CssClass + " glValueActive";

                    }
                    #endregion

                    #region intMin
                    strCell_ColumnName = string.Format(InvariantCulture, "intMin{0:d2}", section);
                    if (_dicLastNums[string.Format(InvariantCulture, "{0:d2}", section)].Contains(int.Parse(dicCells["lngN"].Text, InvariantCulture)))
                    {
                        dicCells[strCell_ColumnName].CssClass = strCell_ColumnName + " glSectionNumLast";
                    }
                    #endregion

                    #region intMax
                    strCell_ColumnName = string.Format(InvariantCulture, "intMax{0:d2}", section);
                    if (section == 50)
                    {
                        if (int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", section)].Text, InvariantCulture) == int.Parse(dicCells[strCell_ColumnName].Text, InvariantCulture))
                        {
                            if (int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", section)].Text, InvariantCulture) > 10)
                            {
                                dicCells[strCell_ColumnName].CssClass = strCell_ColumnName + " glValueMaxNum";
                            }
                            else
                            {
                                dicCells[strCell_ColumnName].CssClass = strCell_ColumnName + " glValueSecondMaxNum";
                            }
                        }
                    }
                    if (section == 100)
                    {
                        if (int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", section)].Text, InvariantCulture) == int.Parse(dicCells[strCell_ColumnName].Text, InvariantCulture))
                        {
                            if (int.Parse(dicCells[string.Format(InvariantCulture, "sglFreq{0:d2}", section)].Text, InvariantCulture) >= 19)
                            {
                                dicCells[strCell_ColumnName].CssClass = strCell_ColumnName + " glValueMaxNum";
                            }
                            else
                            {
                                dicCells[strCell_ColumnName].CssClass = strCell_ColumnName + " glValueSecondMaxNum";
                            }
                        }
                    }
                    #endregion

                }

            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowLastNumsWithFreqSec(string selectedValue)
        {
            if (Session[FreqActive03ID + "LastNumsWithSectiont"] != null && ((Dictionary<string, object>)Session[FreqActive03ID + "LastNumsWithSectiont"]).Keys.Contains(selectedValue))
            {
                gvLastNumsWithSection.Visible = true;

                _dicLastNums = (Dictionary<string, List<int>>)((Dictionary<string, object>)Session[FreqActive03ID + "LastNumsWithSectiont"])[selectedValue];

                DtLastNums = LastNumsDictoDt(_dicLastNums);

                gvLastNumsWithSection.DataSource = DtLastNums.DefaultView;

                //GridView gvLastNums = CreatGridView(string.Format(InvariantCulture, "gvLastNums{0}", selectedValue),
                //                                    "gltable",
                //                                    , true, false);
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
                    if (DicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                    {
                        dcColumn.HeaderStyle.CssClass = DicNumcssclass[strColumnName.Substring(4)];
                    }
                }
                gvLastNumsWithSection.DataBind();
            }
            else
            {
                if (selectedValue != "select") { CreatThread(selectedValue); }
                gvLastNumsWithSection.Visible = false;
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        protected void BtnCloseClick(object sender, EventArgs e)
        {
            Session.Remove(FreqActive03ID);
            ViewState.Remove(FreqActive03ID + "_gstuSearch");
            ReleaseMemory();
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);

        }

        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            if (DicThreadFreqActive03.Keys.Contains(FreqActive03ID + "T01"))
            {
                Thread ThreadFreqActive01 = (Thread)DicThreadFreqActive03[FreqActive03ID + "T01"];
                if (ThreadFreqActive01.ThreadState == ThreadState.Running)
                {
                    new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Pause);
#pragma warning disable CS0618 // 類型或成員已經過時
                    ThreadFreqActive01.Suspend();
#pragma warning restore CS0618 // 類型或成員已經過時
                }
                if (ThreadFreqActive01.ThreadState == ThreadState.Suspended)
                {
                    new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
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
            if (DicThreadFreqActive03.Keys.Contains(FreqActive03ID + "T01"))
            {
                Thread ThreadFreqActive01 = (Thread)DicThreadFreqActive03[FreqActive03ID + "T01"];
                if (ThreadFreqActive01.IsAlive)
                {
                    lblArgument.Visible = true;
                    lblArgument.Text = string.Format(InvariantCulture, "{0}", DateTime.Now.ToLongTimeString());
                    btnT1Start.Visible = true;
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0} ", new GalaxyApp().GetTheadState(ThreadFreqActive01.ThreadState));
                }
                else
                {
                    lblArgument.Visible = false;
                    btnT1Start.Visible = false;
                    Timer1.Enabled = false;
                    new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
                    ResetSearchOrder(FreqActive03ID);
                }
            }
        }

        private void CreatThread(string selectedValue)
        {
            Timer1.Enabled = true;
            if (string.IsNullOrEmpty((string)Session[FreqActive03ID + "select"]))
            {
                if (DicThreadFreqActive03.Keys.Contains(FreqActive03ID + "T01"))
                {
#pragma warning disable CS0618 // 類型或成員已經過時
                    if (((Thread)DicThreadFreqActive03[FreqActive03ID + "T01"]).ThreadState == ThreadState.Suspended) { ((Thread)DicThreadFreqActive03[FreqActive03ID + "T01"]).Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ((Thread)DicThreadFreqActive03[FreqActive03ID + "T01"]).Abort();
                    DicThreadFreqActive03.Remove(FreqActive03ID + "T01");
                }
                Thread ThreadFreqActive01 = new Thread(() =>
                {
                    new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
                    Session[FreqActive03ID + "select"] = selectedValue;
                    if (base.Session[FreqActive03ID + "dsActiveWithSection"] == null)
                    {
                        base.Session[FreqActive03ID + "dsActiveWithSection"] = new DataSet() { Locale = InvariantCulture };
                    }
                    if (!((DataSet)Session[FreqActive03ID + "dsActiveWithSection"]).Tables.Contains(selectedValue))
                    {
                        StuGLSearch stuSearchTemp = _gstuSearch;
                        stuSearchTemp.NextNumsMode = selectedValue != "gen";
                        stuSearchTemp.StrNextNums = selectedValue != "gen" ? selectedValue : "none";
                        stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
                        stuSearchTemp = new CglMethod().GetMethodSN(stuSearchTemp);
                        stuSearchTemp = new CglMethod().GetSearchMethodSN(stuSearchTemp);
                        stuSearchTemp = new CglMethod().GetSecFieldSN(stuSearchTemp);
                        DataTable dtFreqFilterTemp00 = new CglFreqSecT01().GetQryFreqSecT01Multiple(stuSearchTemp);

                        if (dtFreqFilterTemp00 == null) { dtFreqFilterTemp00 = new DataTable(); }
                        dtFreqFilterTemp00.Locale = InvariantCulture;
                        dtFreqFilterTemp00.TableName = selectedValue;
                        ((DataSet)Session[FreqActive03ID + "dsActiveWithSection"]).Tables.Add(dtFreqFilterTemp00);
                    }
                    if (base.Session[FreqActive03ID + "LastNumsWithSectiont"] == null)
                    {
                        base.Session[FreqActive03ID + "LastNumsWithSectiont"] = new Dictionary<string, object>();
                    }
                    if (!((Dictionary<string, object>)Session[FreqActive03ID + "LastNumsWithSectiont"]).Keys.Contains(selectedValue))
                    {
                        StuGLSearch stuSearchTemp = _gstuSearch;
                        stuSearchTemp.NextNumsMode = selectedValue != "gen";
                        stuSearchTemp.StrNextNums = selectedValue != "gen" ? selectedValue : "none";
                        stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
                        stuSearchTemp = new CglMethod().GetMethodSN(stuSearchTemp);
                        stuSearchTemp = new CglMethod().GetSearchMethodSN(stuSearchTemp);
                        stuSearchTemp = new CglMethod().GetSecFieldSN(stuSearchTemp);
                        ((Dictionary<string, object>)Session[FreqActive03ID + "LastNumsWithSectiont"]).Add(selectedValue, new CglFreqSec().GetLastNumsWithFreqSec(stuSearchTemp));
                    }
                    Session[FreqActive03ID + "select"] = string.Empty;
                })
                {
                    Name = FreqActive03ID + "T01"
                };
                ThreadFreqActive01.Start();
                DicThreadFreqActive03.Add(FreqActive03ID + "T01", ThreadFreqActive01);
            }
        }

        private void ReleaseMemory()
        {
            Session.Remove(FreqActive03ID + "dsActiveWithSection");
            Session.Remove(FreqActive03ID + "LastNumsWithSectiont");
            Session.Remove(FreqActive03ID + "select");
            ResetSearchOrder(FreqActive03ID);
            if (DicThreadFreqActive03.Keys.Contains(FreqActive03ID + "T01"))
            {
                Thread ThreadFreqActive01 = (Thread)DicThreadFreqActive03[FreqActive03ID + "T01"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActive01.ThreadState == ThreadState.Suspended) { ThreadFreqActive01.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActive01.Abort();
                ThreadFreqActive01.Join();
                DicThreadFreqActive03.Remove(FreqActive03ID + "T01");
            }
        }

        // ---------------------------------------------------------------------------------------------------------




    }
}