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
    public partial class DataB : BasePage
    {
        private StuGLSearch GlobalStuSearch;
        private string PrivateAction;
        private string PrivateRequestId;
        private string DataBID;

        //Tread part
        private static Dictionary<string, object> DicThreadDataB
        {
            get { if (dicThreadDataB == null) { dicThreadDataB = new Dictionary<string, object>(); } return dicThreadDataB; }
            set => dicThreadDataB = value;
        }
        private static Dictionary<string, object> dicThreadDataB;

        private Thread Thread01;

        //Number Css Class
        //private static Dictionary<int, Color> DicNumColor => new Dictionary<int, Color>() { { 1, Color.DarkGreen }, { 2, Color.DarkBlue }, { 3, Color.Brown }, { 4, Color.Purple }, { 5, Color.DarkRed }, { 6, Color.MediumPurple }, { 7, Color.OliveDrab } };
        private static Dictionary<int, System.Drawing.Color> DicSectionColor => new Dictionary<int, Color>() { { 5, Color.Brown }, { 10, Color.DeepPink }, { 25, Color.Green }, { 50, Color.Purple }, { 100, Color.Blue } };

        // ---------------------------------------------------------------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            SetupViewState();
            if (ViewState[DataBID + "_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                GlobalStuSearch = (StuGLSearch)ViewState[DataBID + "_gstuSearch"];
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

            DataBID = PrivateAction + PrivateRequestId;
            if (!string.IsNullOrEmpty(PrivateAction) && !string.IsNullOrEmpty(PrivateRequestId) && Session[DataBID] != null)
            {
                if (ViewState[DataBID + "_gstuSearch"] == null)
                {
                    ViewState.Add(DataBID + "_gstuSearch", (StuGLSearch)Session[DataBID]);
                }
                else
                {
                    ViewState[DataBID + "_gstuSearch"] = (StuGLSearch)Session[DataBID];
                };
            }
        }

        private void InitializeArgument()
        {
            if (ViewState[DataBID + "ListCurrentNumsB"] == null) { ViewState.Add(DataBID + "ListCurrentNumsB", (List<int>)new CglDataB().GetDataBNumsLst(GlobalStuSearch)); }
            if (ViewState[DataBID + "DicNumCssClass"] == null) { ViewState.Add(DataBID + "DicNumCssClass", new GalaxyApp().GetNumcssclass(GlobalStuSearch)); }
            if (ViewState[DataBID + "dsDataB"] == null) { ViewState[DataBID + "dsDataB"] = new DataSet(); }
            if (ViewState[DataBID + "dicDataB"] == null && Session[DataBID + "dicDataB"] != null) { ViewState.Add(DataBID + "dicDataB", Session[DataBID + "dicDataB"]); }
        }

        private void SetddlNums()
        {
            if (ddlNums.Items.Count == 0)
            {
                for (int intLNums = 1; intLNums <= new CglDataSet(GlobalStuSearch.LottoType).CountNumber; intLNums++)
                {
                    ddlNums.Items.Add(new ListItem(string.Format(InvariantCulture, "N{0:d2}", intLNums), intLNums.ToString(InvariantCulture)));
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
            ckAns.Visible = false;
            switch (ddlFuntion.SelectedValue)
            {
                case "DataB":
                    if (ViewState[DataBID + "DataBTitle"] == null) { ViewState.Add(DataBID + "DataBTitle", string.Format(InvariantCulture, "{0}:{1}", "平衡表", new CglDBData().SetTitleString(GlobalStuSearch))); }
                    Page.Title = (string)ViewState[DataBID + "DataBTitle"];
                    lblTitle.Text = Page.Title;
                    ShowDataB(ddlFreq.SelectedValue, ddlNums.SelectedValue);
                    break;
                case "DataBChart":
                    if (ViewState[DataBID + "DataBChartTitle"] == null) { ViewState.Add(DataBID + "DataBChartTitle", string.Format(InvariantCulture, "{0}:{1}", "平衡圖", new CglDBData().SetTitleString(GlobalStuSearch))); }
                    Page.Title = (string)ViewState[DataBID + "DataBChartTitle"];
                    lblTitle.Text = Page.Title;
                    ddlDays.Visible = true;
                    ddlDisplay.Visible = true;
                    ShowDataBChart(ddlFreq.SelectedValue, ddlNums.SelectedValue);
                    break;
                case "DataBNext":
                    if (ViewState[DataBID + "DataBNextTitle"] == null) { ViewState.Add(DataBID + "DataBNextTitle", string.Format(InvariantCulture, "{0}:{1}", "平衡預測表", new CglDBData().SetTitleString(GlobalStuSearch))); }
                    Page.Title = (string)ViewState[DataBID + "DataBNextTitle"];
                    lblTitle.Text = Page.Title;
                    ckAns.Visible = true;
                    ddlPercent.Visible = true;
                    ddlDays.Visible = true;
                    ShowDataBNext(ddlFreq.SelectedValue, ddlNums.SelectedValue);
                    break;
            }
        }

        private void ShowTitle()
        {

            if (ViewState[DataBID + "lblMethod"] == null) { ViewState.Add(DataBID + "lblMethod", new CglMethod().SetMethodString(GlobalStuSearch)); }
            lblMethod.Text = (string)ViewState[DataBID + "lblMethod"];

            if (ViewState[DataBID + "lblSearchMethod"] == null) { ViewState.Add(DataBID + "lblSearchMethod", new CglMethod().SetSearchMethodString(GlobalStuSearch)); }
            lblSearchMethod.Text = (string)ViewState[DataBID + "lblSearchMethod"];

            //顯示當前資料            
            if (ViewState[DataBID + "CurrentData"] == null) { ViewState.Add(DataBID + "CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(GlobalStuSearch))); }
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState[DataBID + "CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
        }

        private void ShowddlFreq()
        {
            if (ViewState[DataBID + "dicDataB"] != null)
            {
                if (ddlFreq.Items.Count == 0)
                {
                    Dictionary<string, object> DicDataB = (Dictionary<string, object>)ViewState[DataBID + "dicDataB"];
                    foreach (KeyValuePair<string, DataSet> keyval in (Dictionary<string, DataSet>)DicDataB["DataB"])
                    {
                        ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(keyval.Key), keyval.Key));
                    }
                }
            }
            else
            {
                if (!DicThreadDataB.Keys.Contains(DataBID + "T01")) { CreatThread(); }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowDataB(string ddlFreq, string ddlNums)
        {
            if (ViewState[DataBID + "dicDataB"] != null)
            {

                #region show
                using DataTable _dtEachNumB = GetDataTable("dtEachNumB");
                Panel pnlDataB = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlDataB{0}{1}", ddlFreq, ddlNums), "max-width");
                pnlDetail.Controls.Add(pnlDataB);

                Label lblDataB = new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblDataB{0}{1}", ddlFreq, ddlNums),
                                            string.Format(InvariantCulture, "{0}#{1} ({2}期)", ddlFreq, ddlNums, _dtEachNumB.Rows.Count),
                                            "gllabel");
                pnlDataB.Controls.Add(lblDataB);

                #region gvDataB 
                _dtEachNumB.DefaultView.Sort = "lngTotalSN DESC";
                GridView gvDataB = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "gvDataB{0}{1}", ddlFreq, ddlNums),
                                                 "gltable",
                                                 _dtEachNumB, true, false);
                #region Set Css
                foreach (DataControlField dcColumn in gvDataB.Columns)
                {
                    string strColumnName = dcColumn.SortExpression.ToString(InvariantCulture);
                    if ((strColumnName.Substring(0, 4) != "lngB" && strColumnName.Substring(0, 4) != "lngM") || strColumnName == "lngMethodSN")
                    {
                        dcColumn.ItemStyle.CssClass = strColumnName;
                    }
                }
                #endregion
                gvDataB.RowDataBound += GvDataB_RowDataBound;
                gvDataB.DataBind();
                #endregion gvDataB
                pnlDataB.Controls.Add(gvDataB);
                #endregion show
            }
        }

        private void GvDataB_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField.ToString(InvariantCulture);

                        #region Set lngB
                        if (strCell_ColumnName.Contains("lngB"))
                        {
                            cell.Text = new GalaxyApp().ConvertNum(GlobalStuSearch, cell.Text);
                            cell.CssClass = strCell_ColumnName;
                        }
                        #endregion Set lngB

                        #region Set Day of Week
                        if (strCell_ColumnName == "lngDateSN")
                        {
                            string strDateSN = cell.Text.ToString(InvariantCulture);
                            switch (new DateTime(int.Parse(s: strDateSN.Substring(startIndex: 0, length: 4), InvariantCulture),
                                                 int.Parse(s: strDateSN.Substring(startIndex: 4, length: 2), InvariantCulture),
                                                 int.Parse(s: strDateSN.Substring(startIndex: 6, length: 2), InvariantCulture)).DayOfWeek)
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

        private void ShowDataBChart(string ddlFreqSelect, string ddlNumsSelect)
        {
            if (ViewState[DataBID + "dicDataB"] != null)
            {
                //Dictionary<string, object> DicDataB = (Dictionary<string, object>)ViewState[DataBID + "dicDataB"];
                using DataTable dtDataTable = GetDataTable("dtEachNumB");
                dtDataTable.DefaultView.Sort = "[lngDateSN] ASC";
                int intShowRows = Convert.ToInt32(ddlDisplay.SelectedValue, InvariantCulture);
                int intChartWidth = 2200 * intShowRows / 50;
                using DataTable dtShowTable = new GalaxyApp().GetTable(dtDataTable, intShowRows, "lngTotalSN", SortOrder.Descending, SortOrder.Ascending);
                #region Max,Min,Avg
                int intCurrentNum = int.Parse(dtDataTable.Rows[0][string.Format(InvariantCulture, "lngB{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                double dblAvgT = double.Parse(dtDataTable.Rows[0][string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                double dblAvg = double.Parse(dtDataTable.Rows[0][string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                double dblStdEvp = double.Parse(dtDataTable.Rows[0][string.Format(InvariantCulture, "sglStdEvp{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                int intAvgMin = int.Parse(dtDataTable.Rows[0][string.Format(InvariantCulture, "intMin{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                int intAvgMax = int.Parse(dtDataTable.Rows[0][string.Format(InvariantCulture, "intMax{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                #endregion Max,Min,Avg
                // --------------------------------------------------

                #region  chAllNumB
                pnlDetail.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblAllNumB{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                  string.Format(InvariantCulture, "{0}=>Num{1:2d}", ddlFreqSelect, ddlNumsSelect), "gllabel"));

                Chart chAllNumB = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chAllNumB{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                             intChartWidth, 200);
                pnlDetail.Controls.Add(chAllNumB);

                double dblEachNumMin = double.Parse(dtShowTable.Compute(string.Format(InvariantCulture, "MIN([lngB{0}])", ddlNumsSelect),
                                                                        string.Empty).ToString(), InvariantCulture);
                ChartArea chaAllNumB = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAllNumB{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                       dblEachNumMin - 1d);
                chAllNumB.ChartAreas.Add(chaAllNumB);

                #region sirAllNumB
                Series sirAllNumB = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAllNumB{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), chaAllNumB.Name);
                sirAllNumB.Color = Color.SkyBlue;
                sirAllNumB.LabelForeColor = Color.SkyBlue;
                sirAllNumB.IsValueShownAsLabel = true;
                sirAllNumB.LegendText = string.Format(InvariantCulture, "{0}_Num{1}：{2:d02}", ddlFreqSelect, ddlNumsSelect, intCurrentNum);
                sirAllNumB.Points.DataBind(dtShowTable.DefaultView, string.Empty, string.Format(InvariantCulture, "lngB{0}", ddlNumsSelect),
                                           string.Format(InvariantCulture, "Tooltip = lngB{0}", ddlNumsSelect));
                chAllNumB.Series.Add(sirAllNumB);
                #endregion sirAllNumB

                #region sirAvgAllNumB
                Series sirAvgAllNumB = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgAllNumB{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), chaAllNumB.Name);
                sirAvgAllNumB.Color = Color.Red;
                sirAvgAllNumB.LabelForeColor = Color.Red;
                sirAvgAllNumB.IsValueShownAsLabel = true;
                sirAvgAllNumB.LegendText = string.Format(InvariantCulture, "{0}_Avg{1}[Avg:{2} Std:{3} Min:{4} Max:{5}]", ddlFreqSelect, ddlNumsSelect, dblAvg, dblStdEvp, intAvgMin, intAvgMax);
                sirAvgAllNumB.Points.DataBind(dataSource: dtShowTable.DefaultView, string.Empty,
                                              string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                              string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                chAllNumB.Series.Add(sirAvgAllNumB);
                #endregion sirAvgAllNumB

                #endregion  chAllNumB

                #region chAllAvgT
                pnlDetail.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblAllAvgT{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                  string.Format(InvariantCulture, "{0}=>AvgT{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                                  "gllabel"));
                Chart chAllAvgT = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chAllAvgT{0}{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                             intChartWidth, 600);
                pnlDetail.Controls.Add(chAllAvgT);

                ChartArea chaAllAvgT = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAllAvgT{0}{1:2d}", ddlFreqSelect, ddlNumsSelect), 0);
                chAllAvgT.ChartAreas.Add(chaAllAvgT);

                #region sglAvgT
                Series sirAllAvgT = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAllAvgT{0}{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                             chaAllAvgT.Name);
                sirAllAvgT.BorderDashStyle = ChartDashStyle.DashDot;
                sirAllAvgT.Color = Color.Black;
                sirAllAvgT.LabelForeColor = Color.Black;
                sirAllAvgT.IsValueShownAsLabel = true;
                sirAllAvgT.LegendText = string.Format(InvariantCulture, "{0}_AvgT{1}:{2}", ddlFreqSelect, ddlNumsSelect, dblAvgT);
                sirAllAvgT.Points.DataBind(dtShowTable.DefaultView, string.Empty,
                                        string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect),
                                        string.Format(InvariantCulture, "Tooltip = sglAvgT{0}", ddlNumsSelect));
                chAllAvgT.Series.Add(sirAllAvgT);
                #endregion sglAvgT

                #region Series sglAvg
                Series sirAvgAllAvgT = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgAllAvgT{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                               chaAllAvgT.Name);
                sirAvgAllAvgT.Color = Color.Red;
                sirAvgAllAvgT.LegendText = string.Format(InvariantCulture, "{0}_Avg{1}[Avg:{2} Std:{3} Min:{4} Max:{5}]", ddlFreqSelect, ddlNumsSelect, dblAvg, dblStdEvp, intAvgMin, intAvgMax);
                sirAvgAllAvgT.Points.DataBind(dtShowTable.DefaultView, string.Empty,
                                           yFields: string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                           otherFields: string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                chAllAvgT.Series.Add(sirAvgAllAvgT);
                #endregion Series sirAvg

                #region sglAvgTPoly
                DataTable dtsglAvgTPoly = new GalaxyApp().GetTable(new GalaxyApp().CreatPolynomialdt(string.Format(InvariantCulture, "sglAvgTPoly{0}", ddlNumsSelect), dtDataTable,
                                                              new string[] { "lngTotalSN", string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect) },
                                                              3),
                                                          intShowRows, "lngTotalSN", SortOrder.Descending, SortOrder.Ascending);

                Series sirAvgTPoly = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "{0}_sirAvgT{1}Poly", ddlFreqSelect, ddlNumsSelect),
                                                    chaAllAvgT.Name);
                sirAvgTPoly.Color = Color.Orange;
                sirAvgTPoly.LabelForeColor = Color.Orange;
                sirAvgTPoly.IsValueShownAsLabel = true;
                sirAvgTPoly.LegendText = string.Format(InvariantCulture, "{0}_AvgT{1}Poly", ddlFreqSelect, ddlNumsSelect);
                sirAvgTPoly.Points.DataBind(dtsglAvgTPoly.DefaultView, string.Empty, string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect), string.Format(InvariantCulture, "Tooltip = sglAvgT{0}", ddlNumsSelect));
                chAllAvgT.Series.Add(sirAvgTPoly);
                #endregion sglAvgTPoly

                ChartArea chaAvgTKD = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAvgTKD{0}{1:2d}", ddlFreqSelect, ddlNumsSelect), 0);
                chAllAvgT.ChartAreas.Add(chaAvgTKD);

                using DataTable dtsglAvgTKD = new GalaxyApp().GetTable(new GalaxyApp().CreatKDdt(string.Format(InvariantCulture, "dtsglAvgTKD{0}{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                                                dtDataTable, string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect),
                                                                3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d),
                                                       intShowRows, "lngTotalSN", SortOrder.Descending, SortOrder.Ascending);
                #region sglAvgTRSV
                Series sirAvgTRSV = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirsglAvgTRSV{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                    chaAvgTKD.Name);
                sirAvgTRSV.Color = Color.OrangeRed;
                sirAvgTRSV.LabelForeColor = Color.OrangeRed;
                sirAvgTRSV.IsValueShownAsLabel = true;
                sirAvgTRSV.LegendText = string.Format(InvariantCulture, "{0}_AvgT{1}RSV", ddlFreqSelect, ddlNumsSelect);
                sirAvgTRSV.Points.DataBind(dtsglAvgTKD.DefaultView, string.Empty, string.Format(InvariantCulture, "RSV"), string.Format(InvariantCulture, "Tooltip = RSV "));
                chAllAvgT.Series.Add(sirAvgTRSV);
                #endregion sglAvgTRSV

                #region sglAvgTK
                Series sirAvgTK = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgTK{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                    chaAvgTKD.Name);
                sirAvgTK.Color = Color.DarkGreen;
                sirAvgTK.LabelForeColor = Color.DarkGreen;
                sirAvgTK.IsValueShownAsLabel = true;
                sirAvgTK.LegendText = string.Format(InvariantCulture, "{0}_AvgT{1}K", ddlFreqSelect, ddlNumsSelect);
                sirAvgTK.Points.DataBind(dtsglAvgTKD.DefaultView, string.Empty, string.Format(InvariantCulture, "K"), string.Format(InvariantCulture, "Tooltip = K "));
                chAllAvgT.Series.Add(sirAvgTK);
                #endregion sglAvgTK

                #region sglAvgTD
                Series sirAvgTD = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirsglAvgTD{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAvgTKD.Name);
                sirAvgTD.Color = Color.DarkBlue;
                sirAvgTD.LabelForeColor = Color.DarkBlue;
                sirAvgTD.IsValueShownAsLabel = true;
                sirAvgTD.LegendText = string.Format(InvariantCulture, "{0}_AvgT{1}D", ddlFreqSelect, ddlNumsSelect);
                sirAvgTD.Points.DataBind(dtsglAvgTKD.DefaultView, string.Empty, string.Format(InvariantCulture, "D"), string.Format(InvariantCulture, "Tooltip = D "));
                chAllAvgT.Series.Add(sirAvgTD);
                #endregion sglAvgTD

                #region sglAvgTKD
                Series sirAvgTKD = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirsglAvgTKD{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAvgTKD.Name);
                sirAvgTKD.Color = Color.Orange;
                sirAvgTKD.LabelForeColor = Color.Orange;
                sirAvgTKD.IsValueShownAsLabel = true;
                sirAvgTKD.LegendText = string.Format(InvariantCulture, "{0}_AvgT{1}K-D", ddlFreqSelect, ddlNumsSelect);
                sirAvgTKD.Points.DataBind(dtsglAvgTKD.DefaultView, string.Empty, string.Format(InvariantCulture, "AKD"), string.Format(InvariantCulture, "Tooltip = K-D "));
                chAllAvgT.Series.Add(sirAvgTKD);
                #endregion sglAvgTKD

                #endregion chAllAvgT

                #region chAllAvg
                pnlDetail.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                  string.Format(InvariantCulture, "{0}=>Avg{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                                  "gllabel"));

                double dblAllAvgMin = double.Parse(dtDataTable.Compute(string.Format(InvariantCulture, "MIN([sglAvg{0}])", ddlNumsSelect),
                                                                       string.Empty).ToString(), InvariantCulture);

                Chart chAllAvg = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                            intChartWidth, 600);
                pnlDetail.Controls.Add(chAllAvg);

                ChartArea chaAllAvg = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                     dblAllAvgMin - 0.1d);
                chAllAvg.ChartAreas.Add(chaAllAvg);

                #region sirAvgAllAvg
                Series sirAvgAllAvg = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                  chaAllAvg.Name);
                sirAvgAllAvg.Color = Color.Red;
                sirAvgAllAvg.LabelForeColor = Color.Red;
                sirAvgAllAvg.IsValueShownAsLabel = true;
                sirAvgAllAvg.LegendText = string.Format(InvariantCulture, "{0}_Avg{1}[Avg:{2} Std:{3} Min:{4} Max:{5}]"
                                                        , ddlFreqSelect, ddlNumsSelect, dblAvg, dblStdEvp, intAvgMin, intAvgMax);
                sirAvgAllAvg.Points.DataBind(dtShowTable.DefaultView, string.Empty,
                                             string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                             string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                chAllAvg.Series.Add(sirAvgAllAvg);
                #endregion sirAvgAllAvg

                #region sglAvgPolyAllAvg
                using DataTable dtAvgPolyAllAvg = new GalaxyApp().GetTable(new GalaxyApp().CreatPolynomialdt(string.Format(InvariantCulture, "dtAvgPolyAllAvg{0}{1:2d}", ddlFreqSelect, ddlNumsSelect), dtDataTable,
                                                                            new string[] { "lngTotalSN", string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect) },
                                                                            3),
                                                           intShowRows, "lngTotalSN", SortOrder.Descending, SortOrder.Ascending);
                Series sirAvgPolyAllAvg = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgPolyAllAvg{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                      chaAllAvg.Name);
                sirAvgPolyAllAvg.Color = Color.Orange;
                sirAvgPolyAllAvg.LabelForeColor = Color.Orange;
                sirAvgPolyAllAvg.IsValueShownAsLabel = true;
                sirAvgPolyAllAvg.LegendText = string.Format(InvariantCulture, "{0}Avg{1}Poly", ddlFreqSelect, ddlNumsSelect);
                sirAvgPolyAllAvg.Points.DataBind(dtAvgPolyAllAvg.DefaultView, string.Empty,
                                                 string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                                 string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                chAllAvg.Series.Add(sirAvgPolyAllAvg);
                #endregion sglAvgPoly

                ChartArea chaAvgKD = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAvgKD{0}{1:2d}", ddlFreqSelect, ddlNumsSelect), 0);
                chAllAvg.ChartAreas.Add(chaAvgKD);

                using DataTable dtsglAvgKD = new GalaxyApp().GetTable(new GalaxyApp().CreatKDdt(string.Format(InvariantCulture, "dtAvgKD{0}{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                                                dtDataTable, string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                                                3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d),
                                                       intShowRows, "lngTotalSN", SortOrder.Descending, SortOrder.Ascending);
                #region sglAvgRSV
                Series sirAvgRSV = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgRSV{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                    chaAvgKD.Name);
                sirAvgRSV.Color = Color.OrangeRed;
                sirAvgRSV.LabelForeColor = Color.OrangeRed;
                sirAvgRSV.IsValueShownAsLabel = true;
                sirAvgRSV.LegendText = string.Format(InvariantCulture, "{0}_Avg{1}RSV", ddlFreqSelect, ddlNumsSelect);
                sirAvgRSV.Points.DataBind(dtsglAvgKD.DefaultView, string.Empty, string.Format(InvariantCulture, "RSV"),
                                          string.Format(InvariantCulture, "Tooltip = RSV "));
                chAllAvg.Series.Add(sirAvgRSV);
                #endregion sglAvgRSV

                #region sglAvgK
                Series sirAvgK = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgK{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                    chaAvgKD.Name);
                sirAvgK.Color = Color.DarkGreen;
                sirAvgK.LabelForeColor = Color.DarkGreen;
                sirAvgK.IsValueShownAsLabel = true;
                sirAvgK.LegendText = string.Format(InvariantCulture, "{0}_Avg{1}K", ddlFreqSelect, ddlNumsSelect);
                sirAvgK.Points.DataBind(dtsglAvgKD.DefaultView, string.Empty, string.Format(InvariantCulture, "K"),
                                        string.Format(InvariantCulture, "Tooltip = K "));
                chAllAvg.Series.Add(sirAvgK);
                #endregion sglAvgK

                #region sglAvgTD
                Series sirAvgD = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgD{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                             chaAvgKD.Name);
                sirAvgD.Color = Color.DarkBlue;
                sirAvgD.LabelForeColor = Color.DarkBlue;
                sirAvgD.IsValueShownAsLabel = true;
                sirAvgD.LegendText = string.Format(InvariantCulture, "{0}_Avg{1}D", ddlFreqSelect, ddlNumsSelect);
                sirAvgD.Points.DataBind(dtsglAvgKD.DefaultView, string.Empty, string.Format(InvariantCulture, "D"), string.Format(InvariantCulture, "Tooltip = D "));
                chAllAvg.Series.Add(sirAvgD);
                #endregion sglAvgTD

                #region sglAvgKD
                Series sirAvgKD = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirsglAvgKD{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAvgKD.Name);
                sirAvgKD.Color = Color.Orange;
                sirAvgKD.LabelForeColor = Color.Orange;
                sirAvgKD.IsValueShownAsLabel = true;
                sirAvgKD.LegendText = string.Format(InvariantCulture, "{0}_Avg{1}K-D", ddlFreqSelect, ddlNumsSelect);
                sirAvgKD.Points.DataBind(dtsglAvgKD.DefaultView, string.Empty, string.Format(InvariantCulture, "AKD"), string.Format(InvariantCulture, "Tooltip = K-D "));
                chAllAvg.Series.Add(sirAvgKD);
                #endregion sglAvgKD

                #endregion chAllAvg

                // --------------------------------------------------
                #region chEachSection
                foreach (var SectionValue in new string[] { "05", "10", "25", "50", "100" })
                {
                    pnlDetail.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblSection{0}{1:d2}", ddlNumsSelect, SectionValue),
                                                      string.Format(InvariantCulture, "B{0}_S{1}", ddlNumsSelect, SectionValue),
                                                      "gllabel"));

                    Chart chEachSection = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chEachSection{0}{1:d2}{2:d02}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                                                    intChartWidth, 600);
                    pnlDetail.Controls.Add(chEachSection);

                    double dblAvgEachMin = double.Parse(dtDataTable.Compute(string.Format(InvariantCulture, "MIN([sglAvg{0}{1:d2}])", ddlNumsSelect, SectionValue), string.Empty).ToString(), InvariantCulture);
                    double dblAvgTMin = double.Parse(dtDataTable.Compute(string.Format(InvariantCulture, "MIN([sglAvgT{0}])", ddlNumsSelect), string.Empty).ToString(), InvariantCulture);
                    ChartArea chaEachSection = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaEachSection{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                                                           dblAvgEachMin < dblAvgTMin ? dblAvgEachMin - 0.5d : dblAvgTMin - 0.5d);
                    chEachSection.ChartAreas.Add(chaEachSection);

                    #region sirAvgAllEachSection
                    Series sirAvgAllEachSection = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgAllEachSection{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                    chaEachSection.Name);
                    sirAvgAllEachSection.Color = Color.Red;
                    sirAvgAllEachSection.LegendText = string.Format(InvariantCulture, "{0}_Avg{1}", ddlFreqSelect, ddlNumsSelect);
                    sirAvgAllEachSection.Points.DataBind(dtShowTable.DefaultView, string.Empty,
                                                         string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                                         string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                    chEachSection.Series.Add(sirAvgAllEachSection);
                    #endregion sirAvgAllEachSection

                    #region sirAvgEachSection
                    Series sirAvgEachSection = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachSection{0}{1}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                                                                           chaEachSection.Name);
                    sirAvgEachSection.Color = DicSectionColor[key: int.Parse(SectionValue, InvariantCulture)];
                    sirAvgEachSection.LabelForeColor = DicSectionColor[key: int.Parse(SectionValue, InvariantCulture)];
                    sirAvgEachSection.LegendText = string.Format(InvariantCulture, "{0}_Avg{1}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue);
                    sirAvgEachSection.Points.DataBind(dtShowTable.DefaultView, string.Empty, string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, SectionValue), string.Format(InvariantCulture, "Tooltip = sglAvg{0}{1:d2}", ddlNumsSelect, SectionValue));
                    sirAvgEachSection.IsValueShownAsLabel = true;
                    chEachSection.Series.Add(sirAvgEachSection);

                    #endregion sirAvgEachSection
                    #region SeriesEachSectionPolynomial
                    Series sirAvgEachSectionPoly = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachSectionPoly{0}{1}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                                                                               chaEachSection.Name);
                    sirAvgEachSectionPoly.Color = Color.Orange;
                    sirAvgEachSectionPoly.LabelForeColor = Color.DarkOrange;
                    sirAvgEachSectionPoly.IsValueShownAsLabel = true;
                    sirAvgEachSectionPoly.LegendText = string.Format(InvariantCulture, "{0}_sglAvg{1}{2:d2}Poly: {3}", ddlFreqSelect, ddlNumsSelect, SectionValue, int.Parse((dtDataTable.Rows.Count / 5).ToString(InvariantCulture), InvariantCulture));
                    chEachSection.Series.Add(sirAvgEachSectionPoly);
                    #endregion sirAvgEachSection

                    DataTable dtInput = new GalaxyApp().CreatPolynomialdt("AvgEachSectionPoly", dtDataTable,
                                                                          new string[] { "lngTotalSN", string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, SectionValue) },
                                                                          3);
                    #region SeriesEachSectionPolynomial
                    DataTable dtAvgEachSectionPoly = new GalaxyApp().GetTable(dtInput,
                                                                   intShowRows, "lngTotalSN", SortOrder.Descending, SortOrder.Ascending);
                    sirAvgEachSectionPoly.Points.DataBind(dtAvgEachSectionPoly.DefaultView, string.Empty,
                                                          string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, SectionValue),
                                                          string.Format(InvariantCulture, "Tooltip = sglAvg{0}{1:d2}", ddlNumsSelect, SectionValue));
                    #endregion SeriesEachSectionPolynomial


                    ChartArea chaEachSectionKD = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaEachSectionKD{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue), 0);
                    chEachSection.ChartAreas.Add(item: chaEachSectionKD);
                    DataTable dtAvgEachKD = new GalaxyApp().GetTable(new GalaxyApp().CreatKDdt("dtAvgEachKD", dtDataTable,
                                                                    string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, SectionValue),
                                                                    3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d),
                                                            intShowRows, "lngTotalSN", SortOrder.Descending, SortOrder.Ascending);
                    #region sirAvgEachRSV
                    Series sirAvgEachRSV = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachRSV{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                                                        chaEachSectionKD.Name);
                    sirAvgEachRSV.Color = Color.OrangeRed;
                    sirAvgEachRSV.LabelForeColor = Color.OrangeRed;
                    sirAvgEachRSV.IsValueShownAsLabel = true;
                    sirAvgEachRSV.LegendText = string.Format(InvariantCulture, "{0}_Avg{1}{2:d2}RSV", ddlFreqSelect, ddlNumsSelect, SectionValue);
                    sirAvgEachRSV.Points.DataBind(dtAvgEachKD.DefaultView, string.Empty, string.Format(InvariantCulture, "RSV"), string.Format(InvariantCulture, "Tooltip = RSV "));
                    chEachSection.Series.Add(sirAvgEachRSV);
                    #endregion sirAvgEachRSV

                    #region sirAvgEachK
                    Series sirAvgEachK = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachK{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                                                        chaEachSectionKD.Name);
                    sirAvgEachK.Color = Color.DarkGreen;
                    sirAvgEachK.LabelForeColor = Color.DarkGreen;
                    sirAvgEachK.IsValueShownAsLabel = true;
                    sirAvgEachK.LegendText = string.Format(InvariantCulture, "{0}_Avg{1}{2:d2}K", ddlFreqSelect, ddlNumsSelect, SectionValue);
                    sirAvgEachK.Points.DataBind(dtAvgEachKD.DefaultView, string.Empty, string.Format(InvariantCulture, "K"), string.Format(InvariantCulture, "Tooltip = K "));
                    chEachSection.Series.Add(sirAvgEachK);
                    #endregion sirAvgEachK

                    #region sirAvgEachD
                    Series sirAvgEachD = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachD{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                                                        chaEachSectionKD.Name);
                    sirAvgEachD.Color = Color.DarkBlue;
                    sirAvgEachD.LabelForeColor = Color.DarkBlue;
                    sirAvgEachD.IsValueShownAsLabel = true;
                    sirAvgEachD.LegendText = string.Format(InvariantCulture, "{0}_Avg{1}{2:d2}D", ddlFreqSelect, ddlNumsSelect, SectionValue);
                    sirAvgEachD.Points.DataBind(dtAvgEachKD.DefaultView, string.Empty, string.Format(InvariantCulture, "D"), string.Format(InvariantCulture, "Tooltip = D "));
                    chEachSection.Series.Add(sirAvgEachD);
                    #endregion sirAvgEachD

                    #region sglAvgTK-D
                    Series sirAvgEachKD = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachKD{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, SectionValue),
                                                      chaEachSectionKD.Name);
                    sirAvgEachKD.Color = Color.Orange;
                    sirAvgEachKD.LabelForeColor = Color.Orange;
                    sirAvgEachKD.IsValueShownAsLabel = true;
                    sirAvgEachKD.LegendText = string.Format(InvariantCulture, "{0}_Avg{1}{2:d2}K-D", ddlFreqSelect, ddlNumsSelect, SectionValue);
                    sirAvgEachKD.Points.DataBind(dtAvgEachKD.DefaultView, string.Empty, string.Format(InvariantCulture, "AKD"), string.Format(InvariantCulture, "Tooltip = K-D "));
                    chEachSection.Series.Add(sirAvgEachKD);
                    #endregion sglAvgTD

                }
                #endregion chEachSection
                // --------------------------------------------------
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowDataBNext(string ddlFreqSelectedValue, string ddlNumsSelectedValue)
        {
            if (ViewState[DataBID + "dicDataB"] != null)
            {
                ResetSearchOrder(DataBID);
                //Dictionary<string, object> DicDataB = (Dictionary<string, object>)ViewState[DataBID + "dicDataB"];
                Panel pnlDataB = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlDataB{0}", ddlFreqSelectedValue), "max-width");
                pnlDetail.Controls.Add(pnlDataB);
                DataRow LastDataBRow = GetDataTable("dtEachNumB").Rows[0];

                #region showNext
                Panel pnlDataBNext = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlDataBNext{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue), "max-width");
                pnlDetail.Controls.Add(pnlDataBNext);

                pnlDataBNext.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblDataBNext{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue),
                                                     string.Format(InvariantCulture, "{0}=>{1}下期預測資料({2})", ddlFreqSelectedValue, ddlNumsSelectedValue,
                                                                  new GalaxyApp().ConvertNum(GlobalStuSearch, LastDataBRow[string.Format(InvariantCulture, "lngB{0}", ddlNumsSelectedValue)].ToString())),
                                                     "gllabel"));
                using DataTable _dtEachNumBNext = GetDataTable("dtEachNumBNext");
                #region gvDataBext 
                GridView gvDataBNext = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "gvDataBNext{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue),
                                                    "gltable ",
                                                    _dtEachNumBNext, true, false);
                gvDataBNext.AllowSorting = false;
                gvDataBNext.DataKeyNames = new string[] { string.Format(InvariantCulture, "lngB{0}", ddlNumsSelectedValue) };
                #region Set Css
                foreach (DataControlField dcColumn in gvDataBNext.Columns)
                {
                    string strColumnName = dcColumn.SortExpression.ToString(InvariantCulture);
                    if (strColumnName.Contains("lngB"))
                    {
                        dcColumn.ItemStyle.CssClass = strColumnName;
                    }
                }
                #endregion
                gvDataBNext.RowDataBound += GvDataBNextRowDataBound;
                gvDataBNext.DataBind();
                #endregion gvDataBext

                pnlDataBNext.Controls.Add(gvDataBNext);
                #endregion showNext
            }
        }

        private void GvDataBNextRowDataBound(object sender, GridViewRowEventArgs e)
        {
            int Nums = int.Parse(ddlNums.SelectedValue, InvariantCulture);
            string strCuNum = "1";
            using DataTable dtEachNumBNext = GetDataTable("dtEachNumBNext");
            using DataTable dtEachNumB = GetDataTable("dtEachNumB");
            DataRow drEachNumBRange = GetDataTable("dtEachNumBRange").Rows[0];
            DataRow drEachNumBKD = GetDataTable("dtEachNumBKD").Rows[0];
            DataRow drEachNumBKDRange = GetDataTable("dtEachNumBKDRange").Rows[0];
            double dblMax, dblMin;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField.ToString(InvariantCulture);
                        //ASP
                        if (strCell_ColumnName == string.Format(InvariantCulture, "ASP{0}", Nums))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= new GalaxyApp().GetASP(dtEachNumB.Rows[0], Nums.ToString(InvariantCulture)))
                            {
                                cell.CssClass += " glValueLessG ";
                            }
                            if (double.Parse(cell.Text, InvariantCulture) == double.Parse(dtEachNumBNext.Compute(string.Format(InvariantCulture, "Min([{0}])", strCell_ColumnName), string.Empty).ToString(),
                                                                                          InvariantCulture)) { cell.Text += "※"; }
                        }
                        //ASP1
                        if (strCell_ColumnName == string.Format(InvariantCulture, "ASP1{0}", Nums))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), 0d, new GalaxyApp().GetASP1(dtEachNumB.Rows[0], Nums.ToString(InvariantCulture), dtEachNumB.Rows[0]), true))
                            {
                                cell.CssClass += " glValueLessG ";
                            }
                            if (double.Parse(cell.Text, InvariantCulture) == new GalaxyApp().GetMinAbs(dtEachNumBNext, strCell_ColumnName)) { cell.Text += "※"; }
                        }

                        //lngB
                        if (strCell_ColumnName.Contains("lngB"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            strCuNum = cell.Text;
                            if (ckAns.Checked && ((List<int>)ViewState[DataBID + "ListCurrentNumsB"]).Sum() > 0 && new GalaxyApp().ConvertNum(GlobalStuSearch, ((List<int>)ViewState[DataBID + "ListCurrentNumsB"])[int.Parse(strCell_ColumnName.Substring(4), InvariantCulture) - 1]) == cell.Text)
                            {
                                cell.CssClass = ((Dictionary<string, string>)ViewState[DataBID + "DicNumCssClass"])[cell.Text];
                                e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit";
                            }
                            if (cell.Text == new GalaxyApp().ConvertNum(GlobalStuSearch, dtEachNumB.Rows[0][strCell_ColumnName].ToString())) { e.Row.CssClass = e.Row.CssClass + " glRowLast "; }
                            if (cell.Text == new GalaxyApp().ConvertNum(GlobalStuSearch, dtEachNumB.Rows[1][strCell_ColumnName].ToString())) { e.Row.CssClass = e.Row.CssClass + " glRowLastOne "; }
                        }
                        //sglAvg
                        if (strCell_ColumnName.Contains("sglAvg"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(dtEachNumB.Rows[0][strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumBRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumBRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glColNumInBlue "; }
                            dblMax = double.Parse(drEachNumBRange[string.Format(InvariantCulture, "{0}GapMax", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumBRange[string.Format(InvariantCulture, "{0}GapMin", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNum "; }
                        }
                        //sglStdEvp
                        if (strCell_ColumnName.Contains("sglStdEvp"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(dtEachNumB.Rows[0][columnName: strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLessG "; }
                            dblMax = double.Parse(drEachNumBRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumBRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glColNumInGreen "; }
                            dblMax = double.Parse(drEachNumBRange[string.Format(InvariantCulture, "{0}GapMax", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumBRange[string.Format(InvariantCulture, "{0}GapMin", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNum "; }
                            if (double.Parse(cell.Text, InvariantCulture) == double.Parse(dtEachNumBNext.Compute(string.Format(InvariantCulture, "Min([{0}])", strCell_ColumnName), string.Empty).ToString(), InvariantCulture)) { cell.Text += "※"; }
                        }
                        //RSV
                        if (strCell_ColumnName.Contains("RSV"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumBKD[strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            //if (new CglFunc().Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glColNumInBlue "; }
                            //dblMax = double.Parse(drEachNumKDRange[string.Format(InvariantCulture, "{0}GapMax", strCell_ColumnName)].ToString(), InvariantCulture);
                            //dblMin = double.Parse(drEachNumKDRange[string.Format(InvariantCulture, "{0}GapMin", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNumRed "; }
                        }
                        //K
                        if (strCell_ColumnName.Contains("K") && !strCell_ColumnName.Contains("K-D"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumBKD[strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNumGreen "; }
                        }
                        //D
                        if (strCell_ColumnName.Contains("D") && !strCell_ColumnName.Contains("K-D"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumBKD[strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNumBlue "; }
                        }
                        //K-D
                        if (strCell_ColumnName.Contains("K-D"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (Math.Abs(double.Parse(cell.Text, InvariantCulture)) <= Math.Abs(double.Parse(drEachNumBKD[strCell_ColumnName].ToString(), InvariantCulture))) { cell.CssClass += " glColNumInPink "; }

                            if (!(double.Parse(cell.Text, InvariantCulture) > 0 ^ double.Parse(drEachNumBKD[strCell_ColumnName].ToString(), InvariantCulture) > 0))
                            {
                                if (Math.Abs(double.Parse(cell.Text, InvariantCulture)) <= Math.Abs(double.Parse(drEachNumBKD[strCell_ColumnName].ToString(), InvariantCulture))) { cell.CssClass += " glValueLess "; }
                            }
                            dblMax = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNumOrange "; }
                        }
                        cell.ToolTip = string.Format(InvariantCulture, "{0} [{1}]", strCuNum, strCell_ColumnName);
                    }
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private DataTable GetDataTable(string PreTableName)
        {
            string strTableName = string.Empty;
            switch (PreTableName)
            {
                case "dtEachNumBNext":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)ViewState[DataBID + "dsDataB"]).Tables.Contains(strTableName))
                    {

                        ((DataSet)ViewState[DataBID + "dsDataB"]).Tables.Add(GetEachNextDataB(strTableName));
                    }//dtEachNumBNext
                    break;
                case "dtEachNumBGap":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue);
                    if (!((DataSet)ViewState[DataBID + "dsDataB"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataBID + "dsDataB"]).Tables.Add(new GalaxyApp().GetEachDataGap(((Dictionary<string, DataSet>)((Dictionary<string, object>)ViewState[DataBID + "dicDataB"])["DataB"])[ddlFreq.SelectedValue].Tables["Gap"],
                                                                                            strTableName, ddlNums.SelectedValue));
                    }//dtEachNumBGap
                    break;
                case "dtEachNumBRange":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlPercent.SelectedValue);
                    if (!((DataSet)ViewState[DataBID + "dsDataB"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataBID + "dsDataB"]).Tables.Add(new GalaxyApp().GetDataRange(GetDataTable("dtEachNumBGap"),
                                                                                          strTableName,
                                                                                          ddlNums.SelectedValue,
                                                                                          ddlPercent.SelectedValue));
                    }//dtEachNumBRange
                    break;
                case "dtEachNumBKD":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)ViewState[DataBID + "dsDataB"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataBID + "dsDataB"]).Tables.Add(new GalaxyApp().GetDataKDCoeficient(GetDataTable("dtEachNumB"),
                                                                                                strTableName,
                                                                                                ddlNums.SelectedValue,
                                                                                                int.Parse(ddlDays.SelectedValue, InvariantCulture),
                                                                                                3,
                                                                                                1d / 3d));
                    }//dtEachNumBKD
                    break;
                case "dtEachNumBKDRange":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)ViewState[DataBID + "dsDataB"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataBID + "dsDataB"]).Tables.Add(new GalaxyApp().GetDataKDRange(GetDataTable("dtEachNumBKD"),
                                                                                           strTableName,
                                                                                           ddlPercent.SelectedValue));
                    }//dtEachNumBKDRange
                    break;
                case "dtEachNumB":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}", PreTableName, ddlFreq.SelectedValue, ddlNums.SelectedValue);
                    if (!((DataSet)ViewState[DataBID + "dsDataB"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataBID + "dsDataB"]).Tables.Add(GetEachDataB(strTableName));
                    }//dtEachNumB
                    break;
            }
            return ((DataSet)ViewState[DataBID + "dsDataB"]).Tables[strTableName];
        }

        private DataTable GetEachDataB(string strTableName)
        {
            List<string> lstColDataB = new List<string>
                        {
                            "lngTotalSN",   "lngMethodSN",  "lngDateSN",    //"intSum",
                            string.Format(InvariantCulture,"lngB{0}",ddlNums.SelectedValue),
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
                lstColDataB.Add(string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNums.SelectedValue, section));
                //lstColDataB.Add(string.Format(InvariantCulture, "sglAvgGap{0}{1:d2}", ddlNums.SelectedValue, section));
                lstColDataB.Add(string.Format(InvariantCulture, "sglStdEvp{0}{1:d2}", ddlNums.SelectedValue, section));
                //lstColDataB.Add(string.Format(InvariantCulture, "sglStdEvpGap{0}{1:d2}", ddlNums.SelectedValue, section));
                lstColDataB.Add(string.Format(InvariantCulture, "intMin{0}{1:d2}", ddlNums.SelectedValue, section));
                lstColDataB.Add(string.Format(InvariantCulture, "intMax{0}{1:d2}", ddlNums.SelectedValue, section));
            }

            using DataView dvDataB = new DataView(((Dictionary<string, DataSet>)((Dictionary<string, object>)ViewState[DataBID + "dicDataB"])["DataB"])[ddlFreq.SelectedValue].Tables[ddlFreq.SelectedValue]);
            using DataTable dtReturn = dvDataB.ToTable(false, lstColDataB.ToArray());
            dtReturn.TableName = strTableName;
            return dtReturn;
        }

        private DataTable GetEachNextDataB(string strTableName)
        {
            List<string> lstColDataBNext = new List<string>
                            {
                                string.Format(InvariantCulture,"lngB{0}",ddlNums.SelectedValue),
                                string.Format(InvariantCulture,"sglAvgT{0}",ddlNums.SelectedValue),
                                string.Format(InvariantCulture,"sglStdEvpT{0}",ddlNums.SelectedValue),
                                //string.Format(InvariantCulture,"sglDisT{0}",selectedValue2),
                                string.Format(InvariantCulture,"sglAvg{0}",ddlNums.SelectedValue),
                                string.Format(InvariantCulture,"sglStdEvp{0}",ddlNums.SelectedValue),
                                //string.Format(InvariantCulture,"intMin{0}",selectedValue2),
                                //string.Format(InvariantCulture,"intMax{0}",selectedValue2),
                            };
            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
            {
                lstColDataBNext.Add(string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNums.SelectedValue, section));
                lstColDataBNext.Add(string.Format(InvariantCulture, "sglStdEvp{0}{1:d2}", ddlNums.SelectedValue, section));
                //lstColDataBext.Add(string.Format(InvariantCulture, "intMin{0}{1:d2}", selectedValue2, section));
                //lstColDataBext.Add(string.Format(InvariantCulture, "intMax{0}{1:d2}", selectedValue2, section));
            }

            using DataView dvDataBext = new DataView(((Dictionary<string, DataSet>)((Dictionary<string, object>)ViewState[DataBID + "dicDataB"])["DataB"])[ddlFreq.SelectedValue].Tables["Next"].Copy());
            using DataTable dtReturn = dvDataBext.ToTable(false, lstColDataBNext.ToArray());
            dtReturn.PrimaryKey = new DataColumn[] { dtReturn.Columns[string.Format(InvariantCulture, "lngB{0}", ddlNums.SelectedValue)] };
            dtReturn.Locale = InvariantCulture;
            dtReturn.TableName = strTableName;

            #region Add Columns 計算標準差
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "ASP{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "ASP{0}", ddlNums.SelectedValue)].SetOrdinal(1);

            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "ASP1{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "ASP1{0}", ddlNums.SelectedValue)].SetOrdinal(2);
            #endregion Add Columns 計算標準差

            #region Add ColumnsKD Coefficient
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "atRSV{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "atRSV{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "sglStdEvpT{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "atK{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "atK{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "atRSV{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "atD{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "atD{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "atK{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "atK-D{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "atK-D{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "atD{0}", ddlNums.SelectedValue)) + 1);

            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "avRSV{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "avRSV{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "sglStdEvp{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "avK{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "avK{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "avRSV{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "avD{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "avD{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "avK{0}", ddlNums.SelectedValue)) + 1);
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "avK-D{0}", ddlNums.SelectedValue),
                DataType = typeof(double)
            });
            dtReturn.Columns[string.Format(InvariantCulture, "avK-D{0}", ddlNums.SelectedValue)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "avD{0}", ddlNums.SelectedValue)) + 1);

            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
            {
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNums.SelectedValue, section),
                    DataType = typeof(double)
                });
                dtReturn.Columns[string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNums.SelectedValue, section)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "sglStdEvp{0}{1:d2}", ddlNums.SelectedValue, section)) + 1);
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNums.SelectedValue, section),
                    DataType = typeof(double)
                });
                dtReturn.Columns[string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNums.SelectedValue, section)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNums.SelectedValue, section)) + 1);
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNums.SelectedValue, section),
                    DataType = typeof(double)
                });
                dtReturn.Columns[string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNums.SelectedValue, section)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNums.SelectedValue, section)) + 1);
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "aK-D{0}{1:d2}", ddlNums.SelectedValue, section),
                    DataType = typeof(double)
                });
                dtReturn.Columns[string.Format(InvariantCulture, "aK-D{0}{1:d2}", ddlNums.SelectedValue, section)].SetOrdinal(dtReturn.Columns.IndexOf(string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNums.SelectedValue, section)) + 1);
            }
            #endregion Add ColumnsKD Coefficient

            foreach (DataRow drEachNumBNext in dtReturn.Rows)
            {
                #region 計算標準差
                drEachNumBNext[string.Format(InvariantCulture, "ASP{0}", ddlNums.SelectedValue)] = new GalaxyApp().GetASP(drEachNumBNext, ddlNums.SelectedValue);
                drEachNumBNext[string.Format(InvariantCulture, "ASP1{0}", ddlNums.SelectedValue)] = new GalaxyApp().GetASP1(drEachNumBNext,
                                                                                                           ddlNums.SelectedValue,
                                                                                                           GetDataTable("dtEachNumB").Rows[0]);
                #endregion

                #region KD Coefficient
                Dictionary<string, object> DicDataB = (Dictionary<string, object>)ViewState[DataBID + "dicDataB"];
                DataTable dtDataTable = ((Dictionary<string, DataSet>)DicDataB["DataB"])[ddlFreq.SelectedValue].Tables[ddlFreq.SelectedValue];
                DataRow drAvgTemp = new GalaxyApp().GetNextNumKDCoeficient(GlobalStuSearch, dtDataTable, drEachNumBNext,
                                                 string.Format(InvariantCulture, "sglAvgT{0}", ddlNums.SelectedValue),
                                                 3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d);
                drEachNumBNext[string.Format(InvariantCulture, "atRSV{0}", ddlNums.SelectedValue)] = drAvgTemp["RSV"];
                drEachNumBNext[string.Format(InvariantCulture, "atK{0}", ddlNums.SelectedValue)] = drAvgTemp["K"];
                drEachNumBNext[string.Format(InvariantCulture, "atD{0}", ddlNums.SelectedValue)] = drAvgTemp["D"];
                drEachNumBNext[string.Format(InvariantCulture, "atK-D{0}", ddlNums.SelectedValue)] = drAvgTemp["K-D"];

                drAvgTemp = new GalaxyApp().GetNextNumKDCoeficient(GlobalStuSearch, dtDataTable, drEachNumBNext,
                                                 string.Format(InvariantCulture, "sglAvg{0}", ddlNums.SelectedValue),
                                                 3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d);
                drEachNumBNext[string.Format(InvariantCulture, "avRSV{0}", ddlNums.SelectedValue)] = drAvgTemp["RSV"];
                drEachNumBNext[string.Format(InvariantCulture, "avK{0}", ddlNums.SelectedValue)] = drAvgTemp["K"];
                drEachNumBNext[string.Format(InvariantCulture, "avD{0}", ddlNums.SelectedValue)] = drAvgTemp["D"];
                drEachNumBNext[string.Format(InvariantCulture, "avK-D{0}", ddlNums.SelectedValue)] = drAvgTemp["K-D"];

                foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
                {
                    drAvgTemp = new GalaxyApp().GetNextNumKDCoeficient(GlobalStuSearch, dtDataTable, drEachNumBNext,
                                                string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNums.SelectedValue, section),
                                                3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d);
                    drEachNumBNext[string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgTemp["RSV"];
                    drEachNumBNext[string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgTemp["K"];
                    drEachNumBNext[string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgTemp["D"];
                    drEachNumBNext[string.Format(InvariantCulture, "aK-D{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgTemp["K-D"];
                }
                #endregion KD Coefficient
            }


            return dtReturn;
        }

        // ---------------------------------------------------------------------------------------------------------


        protected void BtnCloseClick(object sender, EventArgs e)
        {
            ReleaseMemory();
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);

        }

        private void ReleaseMemory()
        {
            ViewState.Clear();
            Session.Remove(DataBID);
            Session.Remove(DataBID + "dicDataB");

            ResetSearchOrder(DataBID);
            if (DicThreadDataB.Keys.Contains(DataBID + "T01"))
            {
                Thread ThreadFreqActive01 = (Thread)DicThreadDataB[DataBID + "T01"];
                ThreadFreqActive01.Abort();
                DicThreadDataB.Remove(DataBID + "T01");
            }
        }

        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            if (DicThreadDataB != null && DicThreadDataB.Keys.Contains(DataBID + "T01"))
            {
                Thread ThreadFreqActive01 = (Thread)DicThreadDataB[DataBID + "T01"];
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
            if (DicThreadDataB.Keys.Contains(DataBID + "T01"))
            {
                Thread01 = (Thread)DicThreadDataB[DataBID + "T01"];
                if (Thread01.IsAlive)
                {
                    Timer1.Enabled = true;
                    Timer1.Interval = 10000;
                    lblArgument.Text = string.Format(InvariantCulture, "{0} ", DateTime.Now.ToLongTimeString());
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0}{1} ", new GalaxyApp().GetTheadState(Thread01.ThreadState), Session[DataBID + "lblT01"].ToString());
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
            Thread01 = new Thread(() => { StartThread01(); }) { Name = DataBID + "01" };
            Thread01.Start();
            DicThreadDataB.Add(DataBID + "T01", Thread01);
        }

        private void StartThread01()
        {
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
            Session[DataBID + "lblT01"] = "Get DataB ...";
            if (Session[DataBID + "dicDataB"] == null)
            {
                Session.Add(DataBID + "dicDataB", new CglDataB00().GetDataB00Dic(GlobalStuSearch, CglDBDataB.TableName.QryDataB00, SortOrder.Descending));
            }
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
        }

        // ---------------------------------------------------------------------------------------------------------
    }
}