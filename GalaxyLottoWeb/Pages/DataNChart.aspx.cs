using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class DataNChart : BasePage
    {
        private StuGLSearch GlobalStuSearch;
        private string PrivateAction;
        private string PrivateRequestId;
        private string DataNChartID;

        //Tread part
        private static Dictionary<string, object> DicThreadDataN
        {
            get { if (dicThreadDataN == null) { dicThreadDataN = new Dictionary<string, object>(); }; return dicThreadDataN; }
            set => dicThreadDataN = value;
        }
        private static Dictionary<string, object> dicThreadDataN;

        private Thread Thread01;

        private static Dictionary<int, Color> DicSectionColor => new Dictionary<int, Color>() { { 5, Color.Brown }, { 10, Color.DeepPink }, { 25, Color.Green }, { 50, Color.Purple }, { 100, Color.Blue } };


        // ---------------------------------------------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            if (ViewState[DataNChartID + "GlobalStuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                InitializeArgument();
                ShowResult();
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        private void SetupViewState()
        {
            PrivateAction = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            PrivateRequestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", PrivateAction); }
            if (ViewState["id"] == null) { ViewState.Add("id", PrivateRequestId); }

            DataNChartID = PrivateAction + PrivateRequestId;
            if (ViewState[DataNChartID + "GlobalStuSearch"] == null)
            {
                if (!string.IsNullOrEmpty(PrivateAction) && !string.IsNullOrEmpty(PrivateRequestId) && Session[DataNChartID] != null)
                {
                    ViewState.Add(DataNChartID + "GlobalStuSearch", (StuGLSearch)Session[DataNChartID]);
                }
            }
            GlobalStuSearch = (StuGLSearch)ViewState[DataNChartID + "GlobalStuSearch"];
        }

        private void InitializeArgument()
        {
            if (ViewState[DataNChartID + "ListCurrentNumsN"] == null) { ViewState.Add(DataNChartID + "ListCurrentNumsN", (List<int>)new CglData().GetDataNumsLst(GlobalStuSearch)); }
            if (ViewState[DataNChartID + "lblMethod"] == null) { ViewState.Add(DataNChartID + "lblMethod", new CglMethod().SetMethodString(GlobalStuSearch)); }
            if (ViewState[DataNChartID + "lblSearchMethod"] == null) { ViewState.Add(DataNChartID + "lblSearchMethod", new CglMethod().SetSearchMethodString(GlobalStuSearch)); }
            if (ViewState[DataNChartID + "CurrentData"] == null) { ViewState.Add(DataNChartID + "CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(GlobalStuSearch))); }
            if (ViewState[DataNChartID + "DicNumCssClass"] == null) { ViewState.Add(DataNChartID + "DicNumCssClass", new GalaxyApp().GetNumcssclass(GlobalStuSearch)); }
            if (ViewState[DataNChartID + "dsDataN"] == null) { ViewState[DataNChartID + "dsDataN"] = new DataSet(); }
            if (ViewState[DataNChartID + "dicDataN"] == null && Session[DataNChartID + "dicDataN"] != null) { ViewState.Add(DataNChartID + "dicDataN", Session[DataNChartID + "dicDataN"]); }
        }

        private void SetddlNums()
        {
            if (ddlNums.Items.Count == 0)
            {
                for (int intLNums = 1; intLNums <= ((List<int>)ViewState[DataNChartID + "ListCurrentNumsN"]).Count; intLNums++)
                {
                    ddlNums.Items.Add(new ListItem(string.Format(InvariantCulture, "N{0:d2}", intLNums), intLNums.ToString(InvariantCulture)));
                }
            }
            if (ddlNump.Items.Count == 0)
            {
                for (int intLNums = 0; intLNums <= new CglDataSet(GlobalStuSearch.LottoType).LottoNumbers; intLNums++)
                {
                    ddlNump.Items.Add(new ListItem(string.Format(InvariantCulture, "N{0:d2}", intLNums), intLNums.ToString(InvariantCulture)));
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowResult()
        {
            ShowTitle();
            SetddlNums();
            ShowddlFreq();
            ddlPercent.Visible = false;
            ddlDisplay.Visible = false;
            ddlDays.Visible = false;
            ddlNump.Visible = false;
            ckAns.Visible = false;

            if (ViewState[DataNChartID + "DataNChartTitle"] == null) { ViewState.Add(DataNChartID + "DataNChartTitle", string.Format(InvariantCulture, "{0}:{1}", "振盪圖", new CglDBData().SetTitleString(GlobalStuSearch))); }
            Page.Title = (string)ViewState[DataNChartID + "DataNChartTitle"];
            lblTitle.Text = Page.Title;
            ddlDays.Visible = true;
            ddlDisplay.Visible = true;
            ddlNump.Visible = true;
            ShowDataNChart(ddlFreq.SelectedValue, ddlNums.SelectedValue);

        }

        private void ShowTitle()
        {

            lblMethod.Text = (string)ViewState[DataNChartID + "lblMethod"];

            lblSearchMethod.Text = (string)ViewState[DataNChartID + "lblSearchMethod"];

            //顯示當前資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState[DataNChartID + "CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
        }

        private void ShowddlFreq()
        {
            if (ViewState[DataNChartID + "dicDataN"] != null)
            {
                if (ddlFreq.Items.Count == 0)
                {
                    Dictionary<string, object> DicDataN = (Dictionary<string, object>)ViewState[DataNChartID + "dicDataN"];
                    foreach (KeyValuePair<string, DataSet> keyval in (Dictionary<string, DataSet>)DicDataN["DataN"])
                    {
                        ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(keyval.Key), keyval.Key));
                    }
                }
            }
            else
            {
                if (!DicThreadDataN.Keys.Contains(DataNChartID + "T01")) { CreatThread(); }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowDataNChart(string ddlFreqSelect, string ddlNumsSelect)
        {
            if (ViewState[DataNChartID + "dicDataN"] != null)
            {
                pnlDetail.Controls.Clear();
                //Dictionary<string, object> DicDataN = (Dictionary<string, object>)ViewState[DataNID + "dicDataN"];
                int intShowRows = Convert.ToInt32(ddlDisplay.SelectedValue, InvariantCulture);
                int intChartWidth = intShowRows * 40;
                using DataTable dtDataTable = GetDataTable("dtEachNumN");
                dtDataTable.DefaultView.Sort = "[lngDateSN] ASC";
                int nump = Convert.ToInt32(ddlNump.SelectedValue, InvariantCulture);
                using DataTable dtShowTable = GetTableNext(new GalaxyApp().GetTable(dtDataTable, intShowRows, "lngTotalSN", SortOrder.Descending, SortOrder.Ascending), nump);

                #region Max,Min,Avg
                int intCurrentNum = int.Parse(dtShowTable.Rows[dtShowTable.Rows.Count - 1][string.Format(InvariantCulture, "lngN{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                double dblAvgT = double.Parse(dtShowTable.Rows[dtShowTable.Rows.Count - 1][string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                double dblAvg = double.Parse(dtShowTable.Rows[dtShowTable.Rows.Count - 1][string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                //double dblStdEvp = double.Parse(dtShowTable.Rows[0][string.Format(InvariantCulture, "sglStdEvp{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                //int intAvgMin = int.Parse(dtShowTable.Rows[0][string.Format(InvariantCulture, "intMin{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                //int intAvgMax = int.Parse(dtShowTable.Rows[0][string.Format(InvariantCulture, "intMax{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                double dblAvgTMin = double.Parse(dtShowTable.Compute(string.Format(InvariantCulture, "MIN([sglAvgT{0}])", ddlNumsSelect), string.Empty).ToString(), InvariantCulture);
                double dblAvgMin = double.Parse(dtShowTable.Compute(string.Format(InvariantCulture, "MIN([sglAvg{0}])", ddlNumsSelect), string.Empty).ToString(), InvariantCulture);
                #endregion Max,Min,Avg
                // --------------------------------------------------

                #region  chAllNumN
                //pnlDetail.Controls.Add(CreatLabel(string.Format(InvariantCulture, "lblAllNumN{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                //                                  string.Format(InvariantCulture, "{0}=>Num{1:d2}", ddlFreqSelect, ddlNumsSelect), "gllabel"));

                Chart chAllNumN = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chAllNumN{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                             intChartWidth, 200);
                chAllNumN.Titles.Add(new GalaxyApp().CreatTitle(string.Format(InvariantCulture, "lblAllNumN{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                string.Format(InvariantCulture, "{0}=>Num{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                Docking.Top, Color.White, Color.Black));
                pnlDetail.Controls.Add(chAllNumN);

                double dblEachNumMin = double.Parse(dtShowTable.Compute(string.Format(InvariantCulture, "MIN([lngN{0}])", ddlNumsSelect),
                                                                        string.Empty).ToString(), InvariantCulture);
                ChartArea chaAllNumN = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAllNumN{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                       dblEachNumMin - 1d);
                chAllNumN.ChartAreas.Add(chaAllNumN);

                #region sirAllNumN
                NewSeries(string.Format(InvariantCulture, "sirAllNumN{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), chaAllNumN, chAllNumN, Color.SkyBlue,
                          string.Format(InvariantCulture, "{0}_Num{1}：{2:d02}", ddlFreqSelect, ddlNumsSelect, intCurrentNum), "AllNumNLegend",
                          dtShowTable, "", string.Format(InvariantCulture, "lngN{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = lngN{0}", ddlNumsSelect));
                #endregion sirAllNumN

                #region sirAvgAllNumN
                NewSeries(string.Format(InvariantCulture, "sirAvgAllNumN{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), chaAllNumN, chAllNumN, Color.Red,
                          string.Format(InvariantCulture, "{0}_Avg{1}[Avg:{2}]", ddlFreqSelect, ddlNumsSelect, dblAvg), "AllNumNLegend",
                          dtShowTable, "", string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                #endregion sirAvgAllNumN

                #endregion  chAllNumN

                #region chAllAvgT
                //pnlDetail.Controls.Add(CreatLabel(string.Format(InvariantCulture, "lblAllAvgT{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                //                                  string.Format(InvariantCulture, "{0}=>AvgT{1:d2} ", ddlFreqSelect, ddlNumsSelect),
                //                                  "gllabel"));
                Chart chAllAvgT = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chAllAvgT{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                             intChartWidth, 600);

                chAllAvgT.Titles.Add(new GalaxyApp().CreatTitle(string.Format(InvariantCulture, "lblAllAvgT{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                string.Format(InvariantCulture, "{0}=>AvgT{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                Docking.Top, Color.Yellow, Color.Black));

                pnlDetail.Controls.Add(chAllAvgT);

                ChartArea chaAllAvgT = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAllAvgT{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), dblAvgTMin - 0.5d);
                chAllAvgT.ChartAreas.Add(chaAllAvgT);

                #region sglAvgT
                NewSeries(string.Format(InvariantCulture, "sirAllAvgT{0}{1:d2}", ddlNumsSelect, ddlFreqSelect), chaAllAvgT, chAllAvgT, Color.Black,
                          string.Format(InvariantCulture, "{0}_AvgT{1}:{2}", ddlFreqSelect, ddlNumsSelect, dblAvgT), "AllAvgTLegend",
                          dtShowTable, "", string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = sglAvgT{0}", ddlNumsSelect));
                #endregion sglAvgT

                #region Series sglAvg
                NewSeries(string.Format(InvariantCulture, "sirAvgAllAvgT{0}{1:d2}", ddlNumsSelect, ddlFreqSelect), chaAllAvgT, chAllAvgT, Color.Red,
                          string.Format(InvariantCulture, "{0}_Avg{1}[Avg:{2}]", ddlFreqSelect, ddlNumsSelect, dblAvg), "AllAvgTLegend",
                          dtShowTable, "", string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                #endregion Series sirAvg

                using DataTable dtsglAvgTPoly = new GalaxyApp().GetTable(new GalaxyApp().CreatPolynomialdt(string.Format(InvariantCulture, "sglAvgTPoly{0}", ddlNumsSelect), GetTableNext(dtDataTable, nump),
                                                                           new string[] { "lngTotalSN", string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect) },
                                                                           3),
                                                         intShowRows, "lngTotalSN", SortOrder.Descending, SortOrder.Ascending);
                #region sglAvgTPoly
                NewSeries(string.Format(InvariantCulture, "{0}_sirAvgT{1}Poly", ddlFreqSelect, ddlNumsSelect), chaAllAvgT, chAllAvgT, Color.Orange,
                          string.Format(InvariantCulture, "{0}_AvgT{1}Poly", ddlFreqSelect, ddlNumsSelect), "AllAvgTLegend",
                          dtsglAvgTPoly, "", string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = sglAvgT{0}", ddlNumsSelect));
                #endregion sglAvgTPoly

                ChartArea chaAvgTKD = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAvgTKD{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), 0);
                chAllAvgT.ChartAreas.Add(chaAvgTKD);

                #region sglAvgTRSV
                NewSeries(string.Format(InvariantCulture, "sirsglAvgTRSV{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAvgTKD, chAllAvgT, Color.OrangeRed,
                          string.Format(InvariantCulture, "{0}_AvgT{1}RSV", ddlFreqSelect, ddlNumsSelect), "AvgTKDLegend",
                          dtShowTable, "", string.Format(InvariantCulture, "atRSV{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = atRSV{0}", ddlNumsSelect));
                #endregion sglAvgTRSV

                #region sglAvgTK
                NewSeries(string.Format(InvariantCulture, "sirAvgTK{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAvgTKD, chAllAvgT, Color.DarkGreen,
                          string.Format(InvariantCulture, "{0}_AvgT{1}K", ddlFreqSelect, ddlNumsSelect), "AvgTKDLegend",
                          dtShowTable, "", string.Format(InvariantCulture, "atK{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = atK{0}", ddlNumsSelect));
                #endregion sglAvgTK

                #region sglAvgTD
                NewSeries(string.Format(InvariantCulture, "sirsglAvgTD{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAvgTKD, chAllAvgT, Color.DarkBlue,
                          string.Format(InvariantCulture, "{0}_AvgT{1}D", ddlFreqSelect, ddlNumsSelect), "AvgTKDLegend",
                          dtShowTable, "", string.Format(InvariantCulture, "atD{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = atD{0}", ddlNumsSelect));
                #endregion sglAvgTD

                #region sglAvgTKD
                NewSeries(string.Format(InvariantCulture, "sirsglAvgTKD{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAvgTKD, chAllAvgT, Color.DarkOrange,
                          string.Format(InvariantCulture, "{0}_AvgT{1}K-D", ddlFreqSelect, ddlNumsSelect), "AvgTKDLegend",
                          dtShowTable, "", string.Format(InvariantCulture, "atAKD{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = atK-D{0}", ddlNumsSelect));
                #endregion sglAvgTKD

                #endregion chAllAvgT

                #region chAllAvg
                //pnlDetail.Controls.Add(CreatLabel(string.Format(InvariantCulture, "lblAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                //                                  string.Format(InvariantCulture, "{0}=>Avg{1:d2}", ddlFreqSelect, ddlNumsSelect),
                //                                  "gllabel"));

                double dblAllAvgMin = double.Parse(dtDataTable.Compute(string.Format(InvariantCulture, "MIN([sglAvg{0}])", ddlNumsSelect),
                                                                       string.Empty).ToString(), InvariantCulture);

                Chart chAllAvg = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                            intChartWidth, 600);

                chAllAvg.Titles.Add(new GalaxyApp().CreatTitle(string.Format(InvariantCulture, "lblAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                               string.Format(InvariantCulture, "{0}=>Avg{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                               Docking.Top, Color.LightPink, Color.Black));

                pnlDetail.Controls.Add(chAllAvg);

                ChartArea chaAllAvg = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), dblAvgMin - 0.5d);
                chAllAvg.ChartAreas.Add(chaAllAvg);

                #region sirAvgAllAvg
                NewSeries(string.Format(InvariantCulture, "sirAvgAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), chaAllAvg, chAllAvg, Color.Red,
                          string.Format(InvariantCulture, "{0}_Avg{1}[Avg:{2}]", ddlFreqSelect, ddlNumsSelect, dblAvg), "AllAvgLegend",
                          dtShowTable, "", string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                #endregion sirAvgAllAvg

                using DataTable dtAvgPolyAllAvg = new GalaxyApp().GetTable(
                                                  new GalaxyApp().CreatPolynomialdt(string.Format(InvariantCulture, "dtAvgPolyAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), GetTableNext(dtDataTable, nump),
                                                                    new string[] { "lngTotalSN", string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect) }, 3),
                                                           intShowRows, "lngTotalSN", SortOrder.Descending, SortOrder.Ascending);
                #region sglAvgPolyAllAvg
                NewSeries(string.Format(InvariantCulture, "sirAvgPolyAllAvg{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAllAvg, chAllAvg, Color.Orange,
                          string.Format(InvariantCulture, "{0}Avg{1}Poly", ddlFreqSelect, ddlNumsSelect), "AllAvgLegend",
                          dtAvgPolyAllAvg, "", string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                #endregion sglAvgPoly

                ChartArea chaAvgKD = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAvgKD{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), 0);
                chAllAvg.ChartAreas.Add(chaAvgKD);

                #region sglAvgRSV
                NewSeries(string.Format(InvariantCulture, "sirAvgRSV{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAvgKD, chAllAvg, Color.OrangeRed,
                          string.Format(InvariantCulture, "{0}_Avg{1}RSV", ddlFreqSelect, ddlNumsSelect), "AvgKDLegend",
                          dtShowTable, "", string.Format(InvariantCulture, "avRSV{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = avRSV{0}", ddlNumsSelect));
                #endregion sglAvgRSV

                #region sglAvgK
                NewSeries(string.Format(InvariantCulture, "sirAvgK{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAvgKD, chAllAvg, Color.DarkGreen,
                          string.Format(InvariantCulture, "{0}_Avg{1}K", ddlFreqSelect, ddlNumsSelect), "AvgKDLegend",
                          dtShowTable, "", string.Format(InvariantCulture, "avK{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = avK{0}", ddlNumsSelect));
                #endregion sglAvgK

                #region sglAvgD
                NewSeries(string.Format(InvariantCulture, "sirAvgD{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAvgKD, chAllAvg, Color.DarkBlue,
                          string.Format(InvariantCulture, "{0}_Avg{1}D", ddlFreqSelect, ddlNumsSelect), "AvgKDLegend",
                          dtShowTable, "", string.Format(InvariantCulture, "avD{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = avD{0}", ddlNumsSelect));
                #endregion sglAvgD

                #region sglAvgKD
                NewSeries(string.Format(InvariantCulture, "sirsglAvgKD{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAvgKD, chAllAvg, Color.DarkOrange,
                          string.Format(InvariantCulture, "{0}_Avg{1}K-D", ddlFreqSelect, ddlNumsSelect), "AvgKDLegend",
                          dtShowTable, "", string.Format(InvariantCulture, "avAKD{0}", ddlNumsSelect),
                          string.Format(InvariantCulture, "Tooltip = avK-D{0}", ddlNumsSelect));
                #endregion sglAvgKD

                #endregion chAllAvg

                // --------------------------------------------------
                #region chEachSection
                foreach (var SectionValue in new string[] { "05", "10", "25", "50", "100" })
                {
                    //pnlDetail.Controls.Add(CreatLabel(string.Format(InvariantCulture, "lblSection{0}{1:d2}", ddlNumsSelect, SectionValue),
                    //                                  string.Format(InvariantCulture, "N{0}_S{1}", ddlNumsSelect, SectionValue),
                    //                                  "gllabel"));

                    Chart chEachSection = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chEachSection{0}{1:d2}{2:d02}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                                                    intChartWidth, 600);

                    chEachSection.Titles.Add(new GalaxyApp().CreatTitle(string.Format(InvariantCulture, "lblSection{0}{1:d2}", ddlNumsSelect, SectionValue),
                                                        string.Format(InvariantCulture, "N{0}_S{1}", ddlNumsSelect, SectionValue),
                                                        Docking.Top, Color.SkyBlue, Color.Black));
                    pnlDetail.Controls.Add(chEachSection);

                    double dblAvgEachMin = double.Parse(dtDataTable.Compute(string.Format(InvariantCulture, "MIN([sglAvg{0}{1:d2}])", ddlNumsSelect, SectionValue), string.Empty).ToString(), InvariantCulture);
                    ChartArea chaEachSection = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaEachSection{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                                                           dblAvgEachMin < dblAvgMin ? dblAvgEachMin - 0.5d : dblAvgMin - 0.5d);
                    chEachSection.ChartAreas.Add(chaEachSection);

                    #region sirAvgAllEachSection
                    NewSeries(string.Format(InvariantCulture, "sirAvgAllEachSection{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), chaEachSection, chEachSection, Color.Red,
                              string.Format(InvariantCulture, "{0}_Avg{1}", ddlFreqSelect, ddlNumsSelect), string.Format(InvariantCulture, "EachSectionLegend{0}{1:d2}{2:d02}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                              dtShowTable, "", string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                              string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                    #endregion sirAvgAllEachSection

                    #region sirAvgEachSection
                    NewSeries(string.Format(InvariantCulture, "sirAvgEachSection{0}{1}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue), chaEachSection, chEachSection, DicSectionColor[key: int.Parse(SectionValue, InvariantCulture)],
                              string.Format(InvariantCulture, "{0}_Avg{1}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue), string.Format(InvariantCulture, "EachSectionLegend{0}{1:d2}{2:d02}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                              dtShowTable, "", string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, SectionValue),
                              string.Format(InvariantCulture, "Tooltip = sglAvg{0}{1:d2}", ddlNumsSelect, SectionValue));
                    #endregion sirAvgEachSection

                    using DataTable dtAvgEachSectionPoly = new GalaxyApp().GetTable(new GalaxyApp().CreatPolynomialdt("AvgEachSectionPoly", GetTableNext(dtDataTable, nump),
                                                                          new string[] { "lngTotalSN", string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, SectionValue) },
                                                                          3),
                                                                    intShowRows, "lngTotalSN", SortOrder.Descending, SortOrder.Ascending);
                    #region SeriesEachSectionPolynomial
                    NewSeries(string.Format(InvariantCulture, "sirAvgEachSectionPoly{0}{1}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue), chaEachSection, chEachSection, Color.Orange,
                              string.Format(InvariantCulture, "{0}_sglAvg{1}{2:d2}Poly: {3}", ddlFreqSelect, ddlNumsSelect, SectionValue, int.Parse((dtDataTable.Rows.Count / 10).ToString(InvariantCulture), InvariantCulture)), string.Format(InvariantCulture, "EachSectionLegend{0}{1:d2}{2:d02}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                              dtAvgEachSectionPoly, "", string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, SectionValue),
                              string.Format(InvariantCulture, "Tooltip = sglAvg{0}{1:d2}", ddlNumsSelect, SectionValue));
                    #endregion SeriesEachSectionPolynomial

                    ChartArea chaEachSectionKD = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaEachSectionKD{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue), 0);
                    chEachSection.ChartAreas.Add(item: chaEachSectionKD);

                    #region sirAvgEachRSV
                    NewSeries(string.Format(InvariantCulture, "sirAvgEachRSV{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue), chaEachSectionKD, chEachSection, Color.OrangeRed,
                              string.Format(InvariantCulture, "{0}_Avg{1}{2:d2}RSV", ddlFreqSelect, ddlNumsSelect, SectionValue), string.Format(InvariantCulture, "EachSectionKDLegend{0}{1:d2}{2:d02}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                              dtShowTable, "", string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNumsSelect, SectionValue),
                              string.Format(InvariantCulture, "Tooltip = aRSV{0}{1:d2}", ddlNumsSelect, SectionValue));

                    #endregion sirAvgEachRSV

                    #region sirAvgEachK
                    NewSeries(string.Format(InvariantCulture, "sirAvgEachK{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue), chaEachSectionKD, chEachSection, Color.DarkGreen,
                              string.Format(InvariantCulture, "{0}_Avg{1}{2:d2}K", ddlFreqSelect, ddlNumsSelect, SectionValue), string.Format(InvariantCulture, "EachSectionKDLegend{0}{1:d2}{2:d02}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                              dtShowTable, "", string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNumsSelect, SectionValue),
                              string.Format(InvariantCulture, "Tooltip = aK{0}{1:d2}", ddlNumsSelect, SectionValue));
                    #endregion sirAvgEachK

                    #region sirAvgEachD
                    NewSeries(string.Format(InvariantCulture, "sirAvgEachD{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue), chaEachSectionKD, chEachSection, Color.DarkBlue,
                              string.Format(InvariantCulture, "{0}_Avg{1}{2:d2}D", ddlFreqSelect, ddlNumsSelect, SectionValue), string.Format(InvariantCulture, "EachSectionKDLegend{0}{1:d2}{2:d02}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                              dtShowTable, "", string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNumsSelect, SectionValue),
                              string.Format(InvariantCulture, "Tooltip = aD{0}{1:d2}", ddlNumsSelect, SectionValue));
                    #endregion sirAvgEachD

                    #region sglAvgTK-D
                    NewSeries(string.Format(InvariantCulture, "sirAvgEachKD{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue), chaEachSectionKD, chEachSection, Color.DarkOrange,
                              string.Format(InvariantCulture, "{0}_Avg{1}{2:d2}K-D", ddlFreqSelect, ddlNumsSelect, SectionValue), string.Format(InvariantCulture, "EachSectionKDLegend{0}{1:d2}{2:d02}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                              dtShowTable, "", string.Format(InvariantCulture, "aAKD{0}{1:d2}", ddlNumsSelect, SectionValue),
                              string.Format(InvariantCulture, "Tooltip = aK-D{0}{1:d2}", ddlNumsSelect, SectionValue));
                    #endregion sglAvgTD
                }
                #endregion chEachSection
            }
        }

        private DataTable GetTableNext(DataTable dtInput, int num)
        {
            dtInput.PrimaryKey = new DataColumn[] { dtInput.Columns["lngTotalSN"] };
            if (dtInput.Rows.Contains(GlobalStuSearch.LngTotalSN))
            {
                DataRow drInput = dtInput.Rows.Find(GlobalStuSearch.LngTotalSN);
                dtInput.Rows.Remove(drInput);
            }
            using DataTable dtNext = GetDataTable("dtEachNumNNext");
            dtNext.PrimaryKey = new DataColumn[] { dtNext.Columns[string.Format(InvariantCulture, "lngN{0}", ddlNums.SelectedValue)] };
            if (num > 0)
            {
                DataRow drNect = dtNext.Rows.Find(num);
                DataRow drInput = dtInput.NewRow();
                foreach (DataColumn dcInput in dtInput.Columns)
                {
                    if (dcInput.ColumnName == "lngTotalSN") { drInput["lngTotalSN"] = GlobalStuSearch.LngTotalSN; }
                    if (dtNext.Columns.Contains(dcInput.ColumnName))
                    {
                        drInput[dcInput.ColumnName] = drNect[dcInput.ColumnName];
                    }
                }
                dtInput.Rows.Add(drInput);
            }
            return dtInput;
        }

        private static void NewSeries(string SeriesName, ChartArea chartArea, Chart chart, Color color, string legendText, string legendName, DataTable dataTable, string xField, string yField, string tooltip)
        {
            if (chart.Legends.FindByName(legendName) == null)
            {
                chart.Legends.Add(new Legend(legendName));
                chart.Legends[legendName].Docking = Docking.Bottom;
                chart.Legends[legendName].DockedToChartArea = chartArea.Name;
                chart.Legends[legendName].IsDockedInsideChartArea = false;
                chart.Legends[legendName].Alignment = StringAlignment.Center;
                chart.Legends[legendName].BackColor = Color.LightYellow;
                chart.Legends[legendName].BorderWidth = 1;
                chart.Legends[legendName].BorderColor = Color.GreenYellow;
                chart.Legends[legendName].AutoFitMinFontSize = 11;
            }

            Series sirAvgAllAvg = new GalaxyApp().CreatSeries(SeriesName, chartArea.Name);
            sirAvgAllAvg.Color = color;
            sirAvgAllAvg.LabelForeColor = color;
            sirAvgAllAvg.IsValueShownAsLabel = true;
            sirAvgAllAvg.LegendText = legendText;
            sirAvgAllAvg.Legend = legendName;
            sirAvgAllAvg.Points.DataBind(dataTable.DefaultView, xField, yField, tooltip);
            chart.Series.Add(sirAvgAllAvg);
        }

        // ---------------------------------------------------------------------------------------------------------

        private DataTable GetDataTable(string PreTableName)
        {
            string strTableName = string.Empty;
            switch (PreTableName)
            {
                case "dtEachNumNNext":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)ViewState[DataNChartID + "dsDataN"]).Tables.Contains(strTableName))
                    {

                        ((DataSet)ViewState[DataNChartID + "dsDataN"]).Tables.Add(GetEachNextDataN(strTableName));
                    }//dtEachNumNNext
                    break;
                case "dtEachNumNGap":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue);
                    if (!((DataSet)ViewState[DataNChartID + "dsDataN"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataNChartID + "dsDataN"]).Tables.Add(new GalaxyApp().GetEachDataGap(((Dictionary<string, DataSet>)((Dictionary<string, object>)ViewState[DataNChartID + "dicDataN"])["DataN"])[ddlFreq.SelectedValue].Tables["Gap"],
                                                                                            strTableName, ddlNums.SelectedValue));
                    }//dtEachNumNGap
                    break;
                case "dtEachNumNRange":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlPercent.SelectedValue);
                    if (!((DataSet)ViewState[DataNChartID + "dsDataN"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataNChartID + "dsDataN"]).Tables.Add(new GalaxyApp().GetDataRange(GetDataTable("dtEachNumNGap"),
                                                                                          strTableName,
                                                                                          ddlNums.SelectedValue,
                                                                                          ddlPercent.SelectedValue));
                    }//dtEachNumNRange
                    break;
                case "dtEachNumNKD":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)ViewState[DataNChartID + "dsDataN"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataNChartID + "dsDataN"]).Tables.Add(new GalaxyApp().GetDataKDCoeficient(GetDataTable("dtEachNumN"),
                                                                                                strTableName,
                                                                                                ddlNums.SelectedValue,
                                                                                                int.Parse(ddlDays.SelectedValue, InvariantCulture),
                                                                                                3,
                                                                                                1d / 3d));
                    }//dtEachNumNKD
                    break;
                case "dtEachNumNKDRange":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)ViewState[DataNChartID + "dsDataN"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataNChartID + "dsDataN"]).Tables.Add(new GalaxyApp().GetDataKDRange(GetDataTable("dtEachNumNKD"),
                                                                                           strTableName,
                                                                                           ddlPercent.SelectedValue));
                    }//dtEachNumNKDRange
                    break;
                case "dtEachNumN":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlFreq.SelectedValue, ddlNums.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)ViewState[DataNChartID + "dsDataN"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataNChartID + "dsDataN"]).Tables.Add(GetEachDataN(strTableName));
                    }//dtEachNumN
                    break;
            }
            return ((DataSet)ViewState[DataNChartID + "dsDataN"]).Tables[strTableName];
        }

        private DataTable GetEachDataN(string strTableName)
        {
            List<string> lstColDataN = new List<string>
                        {
                            "lngTotalSN",   "lngMethodSN",  "lngDateSN",    //"intSum",
                            string.Format(InvariantCulture,"lngN{0}",ddlNums.SelectedValue),
                            string.Format(InvariantCulture,"sglAvgT{0}",ddlNums.SelectedValue),
                            //string.Format(InvariantCulture,"sglDisT{0}",ddlNums.SelectedValue),
                            //string.Format(InvariantCulture,"sglAvgGapT{0}",ddlNums.SelectedValue),
                            string.Format(InvariantCulture,"sglStdEvpT{0}",ddlNums.SelectedValue),
                            //string.Format(InvariantCulture,"sglStdEvpGapT{0}",ddlNums.SelectedValue),
                            string.Format(InvariantCulture,"sglAvg{0}",ddlNums.SelectedValue),
                            //string.Format(InvariantCulture,"sglAvgGap{0}",ddlNums.SelectedValue),
                            string.Format(InvariantCulture,"sglStdEvp{0}",ddlNums.SelectedValue),
                            //string.Format(InvariantCulture,"sglStdEvpGap{0}",ddlNums.SelectedValue),
                            string.Format(InvariantCulture,"intMin{0}",ddlNums.SelectedValue),
                            string.Format(InvariantCulture,"intMax{0}",ddlNums.SelectedValue),
                        };
            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
            {
                lstColDataN.Add(string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNums.SelectedValue, section));
                //lstColDataN.Add(string.Format(InvariantCulture, "sglAvgGap{0}{1:d2}", ddlNums.SelectedValue, section));
                lstColDataN.Add(string.Format(InvariantCulture, "sglStdEvp{0}{1:d2}", ddlNums.SelectedValue, section));
                //lstColDataN.Add(string.Format(InvariantCulture, "sglStdEvpGap{0}{1:d2}", ddlNums.SelectedValue, section));
                lstColDataN.Add(string.Format(InvariantCulture, "intMin{0}{1:d2}", ddlNums.SelectedValue, section));
                lstColDataN.Add(string.Format(InvariantCulture, "intMax{0}{1:d2}", ddlNums.SelectedValue, section));
            }

            using DataView dvDataN = new DataView(((Dictionary<string, DataSet>)((Dictionary<string, object>)ViewState[DataNChartID + "dicDataN"])["DataN"])[ddlFreq.SelectedValue].Tables[ddlFreq.SelectedValue]);
            using DataTable dtReturn = dvDataN.ToTable(false, lstColDataN.ToArray());
            dtReturn.TableName = strTableName;
            #region Add ColumnsKD Coefficient
            string[] atColum = new string[] { "sglStdEvpT", "atRSV", "atK", "atD", "atK-D", "atAKD" };
            for (int index = 1; index < atColum.Length; index++)
            {
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "{0}{1}", atColum[index], ddlNums.SelectedValue),
                    Caption = string.Format(InvariantCulture, "{0}{1}", atColum[index], ddlNums.SelectedValue),
                    DataType = typeof(double)
                });
                dtReturn.Columns[string.Format(InvariantCulture, "{0}{1}", atColum[index], ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "{0}{1}", atColum[index - 1], ddlNums.SelectedValue)) + 1);
            }

            string[] avColum = new string[] { "sglStdEvp", "avRSV", "avK", "avD", "avK-D", "avAKD" };
            for (int index = 1; index < avColum.Length; index++)
            {
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "{0}{1}", avColum[index], ddlNums.SelectedValue),
                    Caption = string.Format(InvariantCulture, "{0}{1}", avColum[index], ddlNums.SelectedValue),
                    DataType = typeof(double)
                });
                dtReturn.Columns[string.Format(InvariantCulture, "{0}{1}", avColum[index], ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "{0}{1}", avColum[index - 1], ddlNums.SelectedValue)) + 1);
            }

            string[] aColum = new string[] { "sglStdEvp", "aRSV", "aK", "aD", "aK-D", "aAKD" };
            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
            {
                for (int index = 1; index < aColum.Length; index++)
                {
                    dtReturn.Columns.Add(new DataColumn
                    {
                        ColumnName = string.Format(InvariantCulture, "{0}{1}{2:d2}", aColum[index], ddlNums.SelectedValue, section),
                        Caption = string.Format(InvariantCulture, "{0}{1}{2:d2}", aColum[index], ddlNums.SelectedValue, section),
                        DataType = typeof(double)
                    });
                    dtReturn.Columns[string.Format(InvariantCulture, "{0}{1}{2:d2}", aColum[index], ddlNums.SelectedValue, section)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "{0}{1}{2:d2}", aColum[index - 1], ddlNums.SelectedValue, section)) + 1);
                }
            }
            #endregion Add ColumnsKD Coefficient

            #region KD Coefficient
            using DataSet dsKD = new DataSet() { Locale = InvariantCulture };
            dsKD.Tables.Add(new GalaxyApp().CreatKDdt(string.Format(InvariantCulture, "dtAvgTKD{0}{1:d2}", ddlFreq.SelectedValue, ddlNums.SelectedValue),
                                      dtReturn, string.Format(InvariantCulture, "sglAvgT{0}", ddlNums.SelectedValue),
                                      3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d));

            dsKD.Tables.Add(new GalaxyApp().CreatKDdt(string.Format(InvariantCulture, "dtAvgKD{0}{1:d2}", ddlFreq.SelectedValue, ddlNums.SelectedValue),
                                      dtReturn, string.Format(InvariantCulture, "sglAvg{0}", ddlNums.SelectedValue),
                                      3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d));
            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
            {
                dsKD.Tables.Add(new GalaxyApp().CreatKDdt(string.Format(InvariantCulture, "dtAvgEachKD{0}{1:d2}", ddlNums.SelectedValue, section),
                                          dtReturn, string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNums.SelectedValue, section),
                                          3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d));
            }

            string[] KDColumn = new string[] { "RSV", "K", "D", "K-D", "AKD" };
            foreach (DataRow drEachNum in dtReturn.Rows)
            {
                DataRow drAvgTTemp = dsKD.Tables[string.Format(InvariantCulture, "dtAvgTKD{0}{1:d2}", ddlFreq.SelectedValue, ddlNums.SelectedValue)].Rows.Find(drEachNum["lngTotalSN"]);
                for (int index = 1; index < atColum.Length; index++)
                {
                    drEachNum[string.Format(InvariantCulture, "{0}{1}", atColum[index], ddlNums.SelectedValue)] = drAvgTTemp[KDColumn[index - 1]];
                }
                //drEachNum[string.Format(InvariantCulture, "atRSV{0}", ddlNums.SelectedValue)] = drAvgTTemp["RSV"];
                //drEachNum[string.Format(InvariantCulture, "atK{0}", ddlNums.SelectedValue)] = drAvgTTemp["K"];
                //drEachNum[string.Format(InvariantCulture, "atD{0}", ddlNums.SelectedValue)] = drAvgTTemp["D"];
                //drEachNum[string.Format(InvariantCulture, "atK-D{0}", ddlNums.SelectedValue)] = drAvgTTemp["K-D"];
                //drEachNum[string.Format(InvariantCulture, "atAKD{0}", ddlNums.SelectedValue)] = drAvgTTemp["AKD"];

                DataRow drAvgTemp = dsKD.Tables[string.Format(InvariantCulture, "dtAvgKD{0}{1:d2}", ddlFreq.SelectedValue, ddlNums.SelectedValue)].Rows.Find(drEachNum["lngTotalSN"]);
                for (int index = 1; index < avColum.Length; index++)
                {
                    drEachNum[string.Format(InvariantCulture, "{0}{1}", avColum[index], ddlNums.SelectedValue)] = drAvgTemp[KDColumn[index - 1]];
                }
                //drEachNum[string.Format(InvariantCulture, "avRSV{0}", ddlNums.SelectedValue)] = drAvgTemp["RSV"];
                //drEachNum[string.Format(InvariantCulture, "avK{0}", ddlNums.SelectedValue)] = drAvgTemp["K"];
                //drEachNum[string.Format(InvariantCulture, "avD{0}", ddlNums.SelectedValue)] = drAvgTemp["D"];
                //drEachNum[string.Format(InvariantCulture, "avK-D{0}", ddlNums.SelectedValue)] = drAvgTemp["K-D"];
                //drEachNum[string.Format(InvariantCulture, "avAKD{0}", ddlNums.SelectedValue)] = drAvgTemp["AKD"];

                foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
                {
                    DataRow drAvgEachTemp = dsKD.Tables[string.Format(InvariantCulture, "dtAvgEachKD{0}{1:d2}", ddlNums.SelectedValue, section)].Rows.Find(drEachNum["lngTotalSN"]);
                    for (int index = 1; index < aColum.Length; index++)
                    {
                        drEachNum[string.Format(InvariantCulture, "{0}{1}{2:d2}", aColum[index], ddlNums.SelectedValue, section)] = drAvgEachTemp[KDColumn[index - 1]];
                    }
                    //drEachNum[string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgEachTemp["RSV"];
                    //drEachNum[string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgEachTemp["K"];
                    //drEachNum[string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgEachTemp["D"];
                    //drEachNum[string.Format(InvariantCulture, "aK-D{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgEachTemp["K-D"];
                    //drEachNum[string.Format(InvariantCulture, "aAKD{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgEachTemp["AKD"];
                }
            }
            #endregion KD Coefficient
            return dtReturn;
        }

        private DataTable GetEachNextDataN(string strTableName)
        {
            List<string> lstColDataNext = new List<string>
                            {
                                string.Format(InvariantCulture,"lngN{0}",ddlNums.SelectedValue),
                                string.Format(InvariantCulture,"sglAvgT{0}",ddlNums.SelectedValue),
                                string.Format(InvariantCulture,"sglStdEvpT{0}",ddlNums.SelectedValue),
                                string.Format(InvariantCulture,"sglAvg{0}",ddlNums.SelectedValue),
                                string.Format(InvariantCulture,"sglStdEvp{0}",ddlNums.SelectedValue),
                            };
            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
            {
                lstColDataNext.Add(string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNums.SelectedValue, section));
                lstColDataNext.Add(string.Format(InvariantCulture, "sglStdEvp{0}{1:d2}", ddlNums.SelectedValue, section));
            }
            using DataView dvDataNext = new DataView(((Dictionary<string, DataSet>)((Dictionary<string, object>)ViewState[DataNChartID + "dicDataN"])["DataN"])[ddlFreq.SelectedValue].Tables["Next"]);
            using DataTable dtReturn = dvDataNext.ToTable(false, lstColDataNext.ToArray());
            #region Add Columns 計算標準差
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "ASP{0}", ddlNums.SelectedValue),
                Caption = string.Format(InvariantCulture, "ASP{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "ASP{0}", ddlNums.SelectedValue)].SetOrdinal(1);

            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "ASP1{0}", ddlNums.SelectedValue),
                Caption = string.Format(InvariantCulture, "ASP1{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "ASP1{0}", ddlNums.SelectedValue)].SetOrdinal(2);
            #endregion Add Columns 計算標準差

            #region Add ColumnsKD Coefficient
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "atRSV{0}", ddlNums.SelectedValue),
                Caption = string.Format(InvariantCulture, "atRSV{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "atRSV{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "sglStdEvpT{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "atK{0}", ddlNums.SelectedValue),
                Caption = string.Format(InvariantCulture, "atK{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "atK{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "atRSV{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "atD{0}", ddlNums.SelectedValue),
                Caption = string.Format(InvariantCulture, "atD{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "atD{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "atK{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "atK-D{0}", ddlNums.SelectedValue),
                Caption = string.Format(InvariantCulture, "atK-D{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "atK-D{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "atD{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "atAKD{0}", ddlNums.SelectedValue),
                Caption = string.Format(InvariantCulture, "atAKD{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "atAKD{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "atK-D{0}", ddlNums.SelectedValue)) + 1);

            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "avRSV{0}", ddlNums.SelectedValue),
                Caption = string.Format(InvariantCulture, "avRSV{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "avRSV{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "sglStdEvp{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "avK{0}", ddlNums.SelectedValue),
                Caption = string.Format(InvariantCulture, "avK{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "avK{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "avRSV{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "avD{0}", ddlNums.SelectedValue),
                Caption = string.Format(InvariantCulture, "avD{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "avD{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "avK{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "avK-D{0}", ddlNums.SelectedValue),
                Caption = string.Format(InvariantCulture, "avK-D{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "avK-D{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "avD{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "avAKD{0}", ddlNums.SelectedValue),
                Caption = string.Format(InvariantCulture, "avAKD{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "avAKD{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "avK-D{0}", ddlNums.SelectedValue)) + 1);

            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
            {
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNums.SelectedValue, section),
                    Caption = string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNums.SelectedValue, section),
                    DataType = typeof(double)
                });
                dtReturn.Columns[string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNums.SelectedValue, section)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "sglStdEvp{0}{1:d2}", ddlNums.SelectedValue, section)) + 1);
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNums.SelectedValue, section),
                    Caption = string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNums.SelectedValue, section),
                    DataType = typeof(double)
                });
                dtReturn.Columns[string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNums.SelectedValue, section)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNums.SelectedValue, section)) + 1);
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNums.SelectedValue, section),
                    Caption = string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNums.SelectedValue, section),
                    DataType = typeof(double)
                });
                dtReturn.Columns[string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNums.SelectedValue, section)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNums.SelectedValue, section)) + 1);
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "aK-D{0}{1:d2}", ddlNums.SelectedValue, section),
                    Caption = string.Format(InvariantCulture, "aK-D{0}{1:d2}", ddlNums.SelectedValue, section),
                    DataType = typeof(double)
                });
                dtReturn.Columns[string.Format(InvariantCulture, "aK-D{0}{1:d2}", ddlNums.SelectedValue, section)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNums.SelectedValue, section)) + 1);
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "aAKD{0}{1:d2}", ddlNums.SelectedValue, section),
                    Caption = string.Format(InvariantCulture, "aAKD{0}{1:d2}", ddlNums.SelectedValue, section),
                    DataType = typeof(double)
                });
                dtReturn.Columns[string.Format(InvariantCulture, "aAKD{0}{1:d2}", ddlNums.SelectedValue, section)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "aK-D{0}{1:d2}", ddlNums.SelectedValue, section)) + 1);
            }
            #endregion Add ColumnsKD Coefficient

            foreach (DataRow drEachNumNext in dtReturn.Rows)
            {
                #region 計算標準差
                drEachNumNext[string.Format(InvariantCulture, "ASP{0}", ddlNums.SelectedValue)] = new GalaxyApp().GetASP(drEachNumNext, ddlNums.SelectedValue);
                drEachNumNext[string.Format(InvariantCulture, "ASP1{0}", ddlNums.SelectedValue)] = new GalaxyApp().GetASP1(drEachNumNext,
                                                                                                           ddlNums.SelectedValue,
                                                                                                           GetDataTable("dtEachNumN").Rows[0]);
                #endregion

                #region KD Coefficient

                using DataTable dtDataTable = GetDataTable("dtEachNumN");
                DataRow drAvgTemp = new GalaxyApp().GetNextNumKDCoeficient(GlobalStuSearch, dtDataTable, drEachNumNext,
                                                           string.Format(InvariantCulture, "sglAvgT{0}", ddlNums.SelectedValue),
                                                           3, int.Parse(ddlDays.SelectedValue, InvariantCulture),
                                                           1d / 3d);
                drEachNumNext[string.Format(InvariantCulture, "atRSV{0}", ddlNums.SelectedValue)] = drAvgTemp["RSV"];
                drEachNumNext[string.Format(InvariantCulture, "atK{0}", ddlNums.SelectedValue)] = drAvgTemp["K"];
                drEachNumNext[string.Format(InvariantCulture, "atD{0}", ddlNums.SelectedValue)] = drAvgTemp["D"];
                drEachNumNext[string.Format(InvariantCulture, "atK-D{0}", ddlNums.SelectedValue)] = drAvgTemp["K-D"];
                drEachNumNext[string.Format(InvariantCulture, "atAKD{0}", ddlNums.SelectedValue)] = drAvgTemp["AKD"];

                drAvgTemp = new GalaxyApp().GetNextNumKDCoeficient(GlobalStuSearch, dtDataTable, drEachNumNext,
                                                   string.Format(InvariantCulture, "sglAvg{0}", ddlNums.SelectedValue),
                                                   3, int.Parse(ddlDays.SelectedValue, InvariantCulture),
                                                   1d / 3d);
                drEachNumNext[string.Format(InvariantCulture, "avRSV{0}", ddlNums.SelectedValue)] = drAvgTemp["RSV"];
                drEachNumNext[string.Format(InvariantCulture, "avK{0}", ddlNums.SelectedValue)] = drAvgTemp["K"];
                drEachNumNext[string.Format(InvariantCulture, "avD{0}", ddlNums.SelectedValue)] = drAvgTemp["D"];
                drEachNumNext[string.Format(InvariantCulture, "avK-D{0}", ddlNums.SelectedValue)] = drAvgTemp["K-D"];
                drEachNumNext[string.Format(InvariantCulture, "avAKD{0}", ddlNums.SelectedValue)] = drAvgTemp["AKD"];

                foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
                {
                    drAvgTemp = new GalaxyApp().GetNextNumKDCoeficient(GlobalStuSearch, dtDataTable, drEachNumNext,
                                                string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNums.SelectedValue, section),
                                                3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d);
                    drEachNumNext[string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgTemp["RSV"];
                    drEachNumNext[string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgTemp["K"];
                    drEachNumNext[string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgTemp["D"];
                    drEachNumNext[string.Format(InvariantCulture, "aK-D{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgTemp["K-D"];
                    drEachNumNext[string.Format(InvariantCulture, "aAKD{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgTemp["AKD"];
                }

                #endregion KD Coefficient
            }

            dtReturn.PrimaryKey = new DataColumn[] { dtReturn.Columns[string.Format(InvariantCulture, "lngN{0}", ddlNums.SelectedValue)] };
            dtReturn.Locale = InvariantCulture;
            dtReturn.TableName = strTableName;

            return dtReturn;
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
            ReleaseMemory();
        }

        private void ReleaseMemory()
        {
            ViewState.Clear();
            Session.Remove(DataNChartID);
            Session.Remove(DataNChartID + "dicDataN");
            Session.Remove(DataNChartID + "lblT01");
            ResetSearchOrder(DataNChartID);
            if (DicThreadDataN.Keys.Contains(DataNChartID + "T01"))
            {
                Thread ThreadFreqActive01 = (Thread)DicThreadDataN[DataNChartID + "T01"];
                ThreadFreqActive01.Abort();
                DicThreadDataN.Remove(DataNChartID + "T01");
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            if (DicThreadDataN != null && DicThreadDataN.Keys.Contains(DataNChartID + "T01"))
            {
                Thread ThreadFreqActive01 = (Thread)DicThreadDataN[DataNChartID + "T01"];
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

        protected void Timer1Tick(object sender, EventArgs e)
        {
            CheckThread();
        }

        private void CheckThread()
        {
            if (DicThreadDataN.Keys.Contains(DataNChartID + "T01"))
            {
                Thread01 = (Thread)DicThreadDataN[DataNChartID + "T01"];
                if (Thread01.IsAlive)
                {
                    Timer1.Enabled = true;
                    Timer1.Interval = 5000;
                    lblArgument.Text = string.Format(InvariantCulture, "{0} ", DateTime.Now.ToLongTimeString());
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0}{1} ", new GalaxyApp().GetTheadState(Thread01.ThreadState), Session[DataNChartID + "lblT01"].ToString());
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

        }

        private void CreatThread()
        {
            Thread01 = new Thread(() => { StartThread01(); }) { Name = DataNChartID + "01" };
            Thread01.Start();
            //SetDicThreadDataN(GetDicThreadDataN());
            DicThreadDataN.Add(DataNChartID + "T01", Thread01);
        }

        private void StartThread01()
        {
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
            Session[DataNChartID + "lblT01"] = "Get DataN ...";
            if (Session[DataNChartID + "dicDataN"] == null)
            {
                Session.Add(DataNChartID + "dicDataN", new CglDataN00().GetDataN00Dic(GlobalStuSearch, CglDataN.TableName.QryDataN00, SortOrder.Descending));
            }
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
        }

        // ---------------------------------------------------------------------------------------------------------
    }
}