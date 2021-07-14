using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using MathNet.Numerics.LinearAlgebra.Complex.Solvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Windows;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqActiveChart01 : BasePage
    {
        //private Thread thread;
        private string _action;
        private string _requestId;
        private string FreqActiveChart01ID;
        private StuGLSearch GlstuSearch;
        private int intDisplay;
        //private Dictionary<string, int> _dicCurrentNums;
        private Dictionary<string, string> priDicNumcssclass;
        private Dictionary<string, DataTable> priDicFreqActive;
        private Dictionary<string, object> priDicSecFreqActive;
        private static Dictionary<string, Color> PriDicSectionColor => new Dictionary<string, Color>() { { "5", Color.Brown }, { "10", Color.DeepSkyBlue }, { "25", Color.DarkGreen }, { "50", Color.Purple }, { "100", Color.DarkBlue }, { "m", Color.DarkOrange } };

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            if (GlstuSearch == null || GlstuSearch.LottoType == TargetTable.None || GlstuSearch.LngTotalSN == 0)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                if (!IsPostBack)
                {
                    txtDisplay.Text = GlstuSearch.InDisplayPeriod > 0 ? GlstuSearch.InDisplayPeriod.ToString(InvariantCulture) : "200";
                }
                InitializeArgument();
                CheckDisplay();
                SetddlNums();
                ShowTitle();
                ShowResult(GlstuSearch);
                CurrentSearchOrderID = string.Empty;
            }
        }

        private void InitializeArgument()
        {
            if (ViewState[FreqActiveChart01ID + "dicNumcssclass"] == null)
            {
                ViewState.Add(FreqActiveChart01ID + "dicNumcssclass", new GalaxyApp().GetNumcssclass(GlstuSearch));
            }
            priDicNumcssclass = (Dictionary<string, string>)ViewState[FreqActiveChart01ID + "dicNumcssclass"];
            if (ViewState[FreqActiveChart01ID + "SecFreqActive"] == null) { SetData(); }
            priDicSecFreqActive = (Dictionary<string, object>)ViewState[FreqActiveChart01ID + "SecFreqActive"];
            if (ViewState[FreqActiveChart01ID + "title"] == null) { ViewState.Add(FreqActiveChart01ID + "title", string.Format(InvariantCulture, "{0}:{1}", "活性總表圖01", new CglDBData().SetTitleString(GlstuSearch))); }
            if (ViewState[FreqActiveChart01ID + "lblMethod"] == null) { ViewState.Add(FreqActiveChart01ID + "lblMethod", new CglMethod().SetMethodString(GlstuSearch)); }
            if (ViewState[FreqActiveChart01ID + "lblSearchMethod"] == null) { ViewState.Add(FreqActiveChart01ID + "lblSearchMethod", new CglMethod().SetSearchMethodString(GlstuSearch)); }
            if (ViewState[FreqActiveChart01ID + "CurrentData"] == null) { ViewState[FreqActiveChart01ID + "CurrentData"] = new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(GlstuSearch)); }
        }

        private void CheckDisplay()
        {
            if (int.TryParse(txtDisplay.Text, out int intOut))
            {
                intDisplay = intOut == 0 ? 200 : Math.Abs(intOut);
            }
            else
            {
                intDisplay = 200;
                txtDisplay.Text = string.Format(InvariantCulture, "{0}", 200);
            }

            if (intDisplay > GlstuSearch.InDisplayPeriod)
            {
                GlstuSearch.InDisplayPeriod = intDisplay;
                ViewState[FreqActiveChart01ID + "_gstuSearch"] = GlstuSearch;
                ViewState.Remove(FreqActiveChart01ID + "SecFreqActive");
            }
            if (intDisplay < 200 && GlstuSearch.InDisplayPeriod > 200)
            {
                GlstuSearch.InDisplayPeriod = 200;
                ViewState[FreqActiveChart01ID + "_gstuSearch"] = GlstuSearch;
                ViewState.Remove(FreqActiveChart01ID + "SecFreqActive");
            }
        }


        #region Initial Page Data
        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;
            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }
            FreqActiveChart01ID = _action + _requestId;

            if (ViewState[FreqActiveChart01ID + "_gstuSearch"] == null)
            {
                if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqActiveChart01ID] != null)
                {
                    ViewState.Add(FreqActiveChart01ID + "_gstuSearch", (StuGLSearch)Session[FreqActiveChart01ID]);
                }
            }
            GlstuSearch = (StuGLSearch)ViewState[FreqActiveChart01ID + "_gstuSearch"];

            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        private void SetddlNums()
        {
            if (ddlNums.Items.Count == 0)
            {
                for (int intLNums = 1; intLNums <= new CglDataSet(GlstuSearch.LottoType).LottoNumbers; intLNums++)
                {
                    ddlNums.Items.Add(new ListItem(string.Format(InvariantCulture, "{0:d2}", intLNums), string.Format(InvariantCulture, "{0}", intLNums)));
                }
            }
        }

        private void ShowTitle()
        {
            Page.Title = ViewState[FreqActiveChart01ID + "title"].ToString();
            lblTitle.Text = Page.Title;

            lblMethod.Text = (string)ViewState[FreqActiveChart01ID + "lblMethod"];

            lblSearchMethod.Text = (string)ViewState[FreqActiveChart01ID + "lblSearchMethod"];

            // 當期資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState[FreqActiveChart01ID + "CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
        }

        private void SetData()
        {
            StuGLSearch stuGLSearchTemp;
            priDicSecFreqActive = new Dictionary<string, object>();
            stuGLSearchTemp = GlstuSearch;
            stuGLSearchTemp.InDisplayPeriod = stuGLSearchTemp.InDisplayPeriod < 200 ? 200 : stuGLSearchTemp.InDisplayPeriod;
            priDicSecFreqActive.Add("m", new CglMissAll().GetMissAll00Dic(stuGLSearchTemp, CglMissAll.TableName.QryMissAll0001, SortOrder.Descending)["MissAll"]);
            foreach (int sec in new int[] { 100, 50, 25, 10, 5 })
            {
                stuGLSearchTemp = GlstuSearch;
                stuGLSearchTemp.ShowProcess = ShowProcess.Visible;
                stuGLSearchTemp.InSearchLimit = sec;
                stuGLSearchTemp.InDisplayPeriod = stuGLSearchTemp.InDisplayPeriod < 200 ? 200 : stuGLSearchTemp.InDisplayPeriod;
                priDicSecFreqActive.Add(sec.ToString(InvariantCulture), new CglFreq().GetFreqActiveDic(stuGLSearchTemp));
            }
            ViewState.Add(FreqActiveChart01ID + "SecFreqActive", priDicSecFreqActive);
        }

        #endregion Initial Page Data

        private void ShowResult(StuGLSearch stuGLSearch)
        {
            //Img05.Visible = false; Img10.Visible = false; Img25.Visible = false; Img50.Visible = false; Img100.Visible = false; ImgLast.Visible = false;
            pnlDetail.Controls.Clear();
            stuGLSearch.InDisplayPeriod = stuGLSearch.InDisplayPeriod == 0 ? int.Parse(Properties.Resources.defaultDisplayPeriodValue, InvariantCulture) : stuGLSearch.InDisplayPeriod;
            int intLNums = int.Parse(ddlNums.SelectedValue, InvariantCulture);

            #region pnlChartActive
            Panel pnlChartActive = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlActive{0:d2}", intLNums), "max-width");
            pnlDetail.Controls.Add(pnlChartActive);
            #endregion pnlChartActive

            #region Chart chActive

            Chart chActive = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chActive{0:d2}", intLNums),
                                                        intDisplay * 20,
                                                        800);
            chActive.Titles.Add(new GalaxyApp().CreatTitle(string.Format(InvariantCulture, "sNum{0:d2}", intLNums),
                                               string.Format(InvariantCulture, "{0:d02} ", intLNums),
                                               Docking.Top, Color.Yellow, Color.Black));
            pnlChartActive.Controls.Add(chActive);
            #endregion

            #region ChartArea chaArea
            ChartArea chaArea = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaArea{0:d2}", intLNums), 0);
            chaArea.BackColor = priDicNumcssclass.ContainsKey(intLNums.ToString(InvariantCulture)) ? Color.FromArgb(50, 217, 161, 199) : Color.White;
            chaArea.AxisY.Minimum = -1;
            chActive.ChartAreas.Add(chaArea);
            #endregion

            CreatLegent(chActive, chaArea, "series", Docking.Bottom, false, StringAlignment.Center, Color.LightYellow, 1, Color.Orange, 11);

            #region Loop
            foreach (KeyValuePair<string, object> dicSecEachResult in priDicSecFreqActive)
            {
                priDicFreqActive = (Dictionary<string, DataTable>)dicSecEachResult.Value;
                stuGLSearch.InSearchLimit = dicSecEachResult.Key == "m" ? 0 : int.Parse(dicSecEachResult.Key, InvariantCulture);
                #region lstFirstNums,lstSeachLimitNums
                List<int> lstFirstNums = new GalaxyApp().GetLstNumbers(stuGLSearch, priDicFreqActive["gen"], 2);
                List<int> lstSeachLimitNums = new GalaxyApp().GetLstNumbers(stuGLSearch, priDicFreqActive["gen"], stuGLSearch.InSearchLimit + 1);
                switch (dicSecEachResult.Key)
                {
                    case "5":
                        ph05.Visible = lstSeachLimitNums.Contains(intLNums);
                        phLast.Visible = lstFirstNums.Contains(intLNums);
                        break;
                    case "10":
                        ph10.Visible = lstSeachLimitNums.Contains(intLNums);
                        phLast.Visible = lstFirstNums.Contains(intLNums);
                        break;
                    case "25":
                        ph25.Visible = lstSeachLimitNums.Contains(intLNums);
                        phLast.Visible = lstFirstNums.Contains(intLNums);
                        break;
                    case "50":
                        ph50.Visible = lstSeachLimitNums.Contains(intLNums);
                        phLast.Visible = lstFirstNums.Contains(intLNums);
                        break;
                    case "100":
                        ph100.Visible = lstSeachLimitNums.Contains(intLNums);
                        phLast.Visible = lstFirstNums.Contains(intLNums);
                        break;
                }
                #endregion lstFirstNums,lstSeachLimitNums

                string lngF = dicSecEachResult.Key == "m" ? "lngM" : "lngN";

                using DataTable dtFreqResult = priDicFreqActive["gen"].Select(string.Empty, "[lngTotalSN] ASC").Take(priDicFreqActive["gen"].Rows.Count - 1).CopyToDataTable();
                double dblSumT = double.Parse(dtFreqResult.Compute(string.Format(InvariantCulture, "SUM([{0}{1}])", lngF, intLNums), string.Empty).ToString(), InvariantCulture);
                double dblAvgT = Math.Round(dblSumT / (double)dtFreqResult.Rows.Count, 2);
                int intMaxT = int.Parse(dtFreqResult.Compute(string.Format(InvariantCulture, "MAX([{0}{1}])", lngF, intLNums), string.Empty).ToString(), InvariantCulture);
                int intMinT = int.Parse(dtFreqResult.Compute(string.Format(InvariantCulture, "MIN([{0}{1}])", lngF, intLNums), string.Empty).ToString(), InvariantCulture);
                //dtFreqResult.Columns.Remove("lngTotalSN");

                #region Max,Min,Avg
                int intCurrent = int.Parse(dtFreqResult.Rows[0][string.Format(InvariantCulture, "{0}{1}", lngF, intLNums)].ToString(), InvariantCulture);
                using (DataTable dataTable = stuGLSearch.InSearchLimit > 0 ? dtFreqResult.Rows.Cast<DataRow>().Take(stuGLSearch.InDisplayPeriod).CopyToDataTable() : dtFreqResult)
                {
                    double dblSumS = double.Parse(dataTable.Compute(string.Format(InvariantCulture, "SUM([{0}{1}])", lngF, intLNums), string.Empty).ToString(), InvariantCulture);
                    double dblAvgS = Math.Round(dblSumS / (double)stuGLSearch.InSearchLimit, 2);
                    int intMaxS = int.Parse(dataTable.Compute(string.Format(InvariantCulture, "MAX([{0}{1}])", lngF, intLNums), string.Empty).ToString(), InvariantCulture);
                    int intMinS = int.Parse(dataTable.Compute(string.Format(InvariantCulture, "MIN([{0}{1}])", lngF, intLNums), string.Empty).ToString(), InvariantCulture);
                }

                #endregion Max,Min,Avg

                using DataTable dtCurve = priDicFreqActive["gen"].Select("", "lngTotalSN DESC").Take<DataRow>(intDisplay).CopyToDataTable();
                dtCurve.DefaultView.Sort = "[lngDateSN] ASC";

                bool display = true;
                display = dicSecEachResult.Key switch
                {
                    "5" => chk05.Checked,
                    "10" => chk10.Checked,
                    "25" => chk25.Checked,
                    "50" => chk50.Checked,
                    "100" => chk100.Checked,
                    "m" => chkm.Checked,
                    _ => true,
                };
                if (display)
                {
                    #region Series
                    Series sirNum = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirNum{0}{1:d2}", dicSecEachResult.Key, intLNums), chaArea.Name);
                    sirNum.IsValueShownAsLabel = true;
                    sirNum.ChartType = SeriesChartType.Line;
                    sirNum.Color = PriDicSectionColor[dicSecEachResult.Key];
                    sirNum.LegendText = string.Format(InvariantCulture, " {0}區, 大:[{1}], 小[{2}], 號[{3}] ", dicSecEachResult.Key, intMaxT, intMinT, string.Join(",", lstSeachLimitNums));
                    sirNum.Legend = "series";
                    sirNum.Points.DataBind(dtCurve.DefaultView, "", string.Format(InvariantCulture, "{0}{1}", lngF, intLNums), string.Format(InvariantCulture, "Tooltip = {0}{1}", lngF, intLNums));
                    chActive.Series.Add(sirNum);
                    #endregion Series

                    #region LineAnnotation
                    HorizontalLineAnnotation hlaMax = new GalaxyApp().CreatHorizontalLineAnnotation(string.Format(InvariantCulture, "sirhlaMax{0}{1:d2}", dicSecEachResult.Key, intLNums), chaArea);
                    hlaMax.LineColor = PriDicSectionColor[dicSecEachResult.Key];
                    hlaMax.ToolTip = intMaxT.ToString(InvariantCulture);
                    hlaMax.AnchorY = intMaxT;
                    hlaMax.LineDashStyle = ChartDashStyle.Dot;
                    chActive.Annotations.Add(hlaMax);

                    HorizontalLineAnnotation hlaMin = new GalaxyApp().CreatHorizontalLineAnnotation(string.Format(InvariantCulture, "sirhlaMin{0}{1:d2}", dicSecEachResult.Key, intLNums), chaArea);
                    hlaMin.LineColor = PriDicSectionColor[dicSecEachResult.Key];
                    hlaMin.ToolTip = intMinT.ToString(InvariantCulture);
                    hlaMin.AnchorY = intMinT;
                    hlaMin.LineDashStyle = ChartDashStyle.Dot;
                    chActive.Annotations.Add(hlaMin);

                    VerticalLineAnnotation vlaSearchLimit = new GalaxyApp().CreatVerticalLineAnnotation(string.Format(InvariantCulture, "sirSL{0}{1:d2}", dicSecEachResult.Key, intLNums), chaArea);
                    vlaSearchLimit.LineColor = PriDicSectionColor[dicSecEachResult.Key];
                    hlaMin.LineDashStyle = ChartDashStyle.Dash;
                    vlaSearchLimit.ToolTip = vlaSearchLimit.AnchorX.ToString(InvariantCulture);
                    vlaSearchLimit.AnchorX = (dtCurve.Rows.Count - stuGLSearch.InSearchLimit + 1) > 0 ? dtCurve.Rows.Count - stuGLSearch.InSearchLimit + 1 : 0;
                    chActive.Annotations.Add(vlaSearchLimit);

                    #endregion
                }
            }
            #endregion Loop
        }

        private static void CreatLegent(Chart chActive, ChartArea chaArea, string legendName, Docking docking, bool isDockedIn,
                                        StringAlignment stringAlignment, Color BackColor, int BorderWidth, Color BorderColor,
                                        int AutoFitMinFontSize)
        {
            Legend legend = new Legend(legendName)
            {
                Name = legendName,
                Docking = docking,
                DockedToChartArea = chaArea.Name,
                IsDockedInsideChartArea = isDockedIn,
                Alignment = stringAlignment,
                BackColor = BackColor,
                BorderWidth = BorderWidth,
                BorderColor = BorderColor,
                AutoFitMinFontSize = AutoFitMinFontSize,
                IsTextAutoFit = true
            };
            chActive.Legends.Add(legend);
        }

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            ViewState.Clear();
            Session.Remove(FreqActiveChart01ID);
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
        }

        protected void BtnUp_Click(object sender, EventArgs e)
        {
            ChangeddlNums(1);
        }

        protected void BtnDowns_Click(object sender, EventArgs e)
        {
            ChangeddlNums(-1);
        }

        private void ChangeddlNums(int index)
        {
            int IntSelect = ddlNums.SelectedIndex + index;
            ddlNums.SelectedIndex = IntSelect < 0 ? 0 : IntSelect >= ddlNums.Items.Count ? ddlNums.Items.Count - 1 : IntSelect;
            ShowResult(GlstuSearch);
        }
    }
}
