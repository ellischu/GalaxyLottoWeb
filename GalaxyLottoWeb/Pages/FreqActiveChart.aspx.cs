using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqActiveChart : BasePage
    {
        //private Thread thread;
        private string _action;
        private string _requestId;
        private string FreqActiveChartID;
        private StuGLSearch GlstuSearch;
        //private Dictionary<string, int> _dicCurrentNums;
        private Dictionary<string, string> _dicNumcssclass;
        private Dictionary<string, DataTable> _dicFreqActive;
        private DataTable dtCurrentData;

        protected void Page_Load(object sender, EventArgs e)
        {
            SetupViewState();
            GlstuSearch = (StuGLSearch)ViewState["_gstuSearch"];
            if (GlstuSearch.LottoType == TargetTable.None || GlstuSearch.LngTotalSN == 0)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                if (!IsPostBack)
                {
                    if (ViewState["title"] == null)
                    {
                        ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "活性總表圖", new CglDBData().SetTitleString(GlstuSearch)));
                    }
                    if (ViewState["CurrentNums"] == null)
                    {
                        ViewState["CurrentNums"] = new CglData().GetDataNumsDici(GlstuSearch);
                    }
                    if (ViewState["CurrentData"] == null)
                    {
                        ViewState["CurrentData"] = new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(GlstuSearch));
                    }
                    if (ViewState["FreqActive"] == null)
                    {
                        StuGLSearch stuGLSearchTemp = GlstuSearch;
                        stuGLSearchTemp.ShowProcess = ShowProcess.Visible;
                        stuGLSearchTemp.InDisplayPeriod = (stuGLSearchTemp.InDisplayPeriod < stuGLSearchTemp.InSearchLimit) ? stuGLSearchTemp.InSearchLimit : stuGLSearchTemp.InDisplayPeriod;
                        ViewState.Add("FreqActive", new CglFreq().GetFreqActiveDic(stuGLSearchTemp));
                    }
                }
                //_dicCurrentNums = (Dictionary<string, int>)ViewState["CurrentNums"];
                _dicNumcssclass = new GalaxyApp().GetNumcssclass(GlstuSearch);
                dtCurrentData = (DataTable)ViewState["CurrentData"];
                _dicFreqActive = (Dictionary<string, DataTable>)ViewState["FreqActive"];
                Page.Title = ViewState["title"].ToString();
                SetddlNums();
                ShowResult(GlstuSearch);
            }
            CurrentSearchOrderID = string.Empty;
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

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            FreqActiveChartID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqActiveChartID] != null)
            {
                if (ViewState["_gstuSearch"] == null)
                {
                    ViewState.Add("_gstuSearch", (StuGLSearch)Session[FreqActiveChartID]);
                }
                else
                {
                    ViewState["_gstuSearch"] = (StuGLSearch)Session[FreqActiveChartID];
                };
            }

            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        private void ShowResult(StuGLSearch stuGLSearch)
        {
            lblTitle.Text = Page.Title;
            lblMethod.Text = new CglMethod().SetMethodString(stuGLSearch);
            lblSearchMethod.Text = new CglMethod().SetSearchMethodString(stuGLSearch);

            // 當期資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", dtCurrentData, true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            #region Show the result

            #region Loop
            foreach (var dicEachResult in _dicFreqActive)
            {
                #region lstFirstNums,lstSeachLimitNums
                List<int> lstFirstNums = (List<int>)new GalaxyApp().GetLstNumbers(stuGLSearch, dicEachResult.Value, 2);
                List<int> lstSeachLimitNums = (List<int>)new GalaxyApp().GetLstNumbers(stuGLSearch, dicEachResult.Value, stuGLSearch.InSearchLimit + 1);
                #endregion lstFirstNums,lstSeachLimitNums

                DataTable dtFreqResult = dicEachResult.Value.Copy();
                dtFreqResult.Columns.Remove("lngTotalSN");

                #region Set Chart 

                #region pnlChartActive
                using Panel pnlChartActive = new Panel { ID = string.Format(InvariantCulture, "pnlActive{0}", dicEachResult.Key), CssClass = "max-width" };
                pnlDetail.Controls.Add(pnlChartActive);
                #endregion pnlChartActive

                DataTable dtCurve = dicEachResult.Value.Copy();
                dtCurve.DefaultView.Sort = "[lngDateSN] ASC";
                int intLNums = int.Parse(ddlNums.SelectedValue, InvariantCulture);
                //for (int intLNums = 1; intLNums <= new CglDataSet(stuGLSearch.LottoType).LottoNumbers; intLNums++)
                //{
                #region Max,Min,Avg
                int intCurrent = int.Parse(dtFreqResult.Rows[0][string.Format(InvariantCulture, "lngN{0}", intLNums)].ToString(), InvariantCulture);
                DataTable dataTable = stuGLSearch.InSearchLimit > 0 ? dtFreqResult.Rows.Cast<DataRow>().Take(stuGLSearch.InSearchLimit).CopyToDataTable() : dtFreqResult;
                double dblSumS = double.Parse(dataTable.Compute(string.Format(InvariantCulture, "SUM([lngN{0}])", intLNums), string.Empty).ToString(), InvariantCulture);
                double dblAvgS = Math.Round(dblSumS / (double)stuGLSearch.InSearchLimit, 2);
                int intMaxS = int.Parse(dataTable.Compute(string.Format(InvariantCulture, "MAX([lngN{0}])", intLNums), string.Empty).ToString(), InvariantCulture);
                int intMinS = int.Parse(dataTable.Compute(string.Format(InvariantCulture, "MIN([lngN{0}])", intLNums), string.Empty).ToString(), InvariantCulture);
                double dblSumT = double.Parse(dtFreqResult.Compute(string.Format(InvariantCulture, "SUM([lngN{0}])", intLNums), string.Empty).ToString(), InvariantCulture);
                double dblAvgT = Math.Round(dblSumT / (double)dtFreqResult.Rows.Count, 2);
                int intMaxT = int.Parse(dtFreqResult.Compute(string.Format(InvariantCulture, "MAX([lngN{0}])", intLNums), string.Empty).ToString(), InvariantCulture);
                int intMinT = int.Parse(dtFreqResult.Compute(string.Format(InvariantCulture, "MIN([lngN{0}])", intLNums), string.Empty).ToString(), InvariantCulture);

                #endregion Max,Min,Avg

                #region Chart chActive

                Chart chActive = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chActive{0}{1:d2}", dicEachResult.Key, intLNums),
                                                            stuGLSearch.InDisplayPeriod * 20, 500);

                chActive.Titles.Add(new GalaxyApp().CreatTitle(string.Format(InvariantCulture, "sNum{0}{1:d2}", dicEachResult.Key, intLNums),
                                               string.Format(InvariantCulture, "{0:d02} (T:{1} C:{2}) \n (AvgS:{3} MaS:{4} MiS:{5}) \n (AvgS:{6} Ma:{7} Mi:{8})",
                                                                                intLNums, CglFreq.GetTrend(stuGLSearch, dtFreqResult, intLNums), intCurrent, dblAvgS, intMaxS, intMinS, dblAvgT, intMaxT, intMinT),
                                               Docking.Top,
                                               (int.Parse(dtFreqResult.Rows[0][string.Format(InvariantCulture, "lngN{0}", intLNums)].ToString(), InvariantCulture) >= stuGLSearch.InCriticalNum) ? Color.Red : Color.Black, Color.White));
                pnlChartActive.Controls.Add(chActive);
                #endregion


                #region ChartArea chaArea
                ChartArea chaArea = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaArea{0}{1:d2}", dicEachResult.Key, intLNums), 0);
                chaArea.BackColor = _dicNumcssclass.ContainsKey(intLNums.ToString(InvariantCulture)) ? Color.FromArgb(50, 217, 161, 199) : Color.White;
                chaArea.AxisY.Maximum = intMaxT + 1;
                if (lstFirstNums.Contains(intLNums))
                {
                    chaArea.BorderColor = Color.Blue;
                    chaArea.BorderWidth = 2;
                }
                if (lstSeachLimitNums.Contains(intLNums))
                {
                    chaArea.BorderColor = Color.Green;
                    chaArea.BorderWidth = 2;
                }
                if (lstFirstNums.Contains(intLNums) && lstSeachLimitNums.Contains(intLNums))
                {
                    chaArea.BorderColor = Color.Red;
                    chaArea.BorderWidth = 2;
                }
                #endregion
                chActive.ChartAreas.Add(chaArea);

                #region Set buttons
                //HyperLink hlNumG = CreatHyperLink(string.Format(InvariantCulture, "hlNumG{0}{1:d2}", dicEachResult.Key, intLNums),
                //                                  "glbutton glbutton-lightblue ",
                //                                  string.Format(InvariantCulture, "{0}::{1:d2}", dicEachResult.Key, intLNums),
                //                                  string.Format(InvariantCulture, "#{0}", chActive.ID));
                //pnlButtons.Controls.Add(hlNumG);

                #endregion Set buttons

                #region Series
                Series sirNum = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirNum{0}{1:d2}", dicEachResult.Key, intLNums), chaArea.Name);
                sirNum.IsValueShownAsLabel = true;
                sirNum.Color = Color.Red;
                sirNum.LegendText = string.Format(InvariantCulture, "{0}:lngN{1}", sirNum.Name, intLNums);
                sirNum.Points.DataBind(dtCurve.DefaultView, "", string.Format(InvariantCulture, "lngN{0}", intLNums), string.Format(InvariantCulture, "Tooltip = lngN{0}", intLNums));
                #endregion Series
                chActive.Series.Add(sirNum);

                #region LineAnnotation
                HorizontalLineAnnotation hlaAvgS = new GalaxyApp().CreatHorizontalLineAnnotation(string.Format(InvariantCulture, "sirAvgS{0}{1:d2}", dicEachResult.Key, intLNums), chaArea);
                hlaAvgS.LineColor = Color.Blue;
                hlaAvgS.ToolTip = intMaxT.ToString(InvariantCulture);
                hlaAvgS.AnchorY = intMaxT;
                hlaAvgS.LineDashStyle = ChartDashStyle.DashDot;
                chActive.Annotations.Add(hlaAvgS);

                using HorizontalLineAnnotation hlaAvgT = new GalaxyApp().CreatHorizontalLineAnnotation(string.Format(InvariantCulture, "sirAvgT{0}{1:d2}", dicEachResult.Key, intLNums), chaArea);
                hlaAvgT.LineColor = Color.Purple;
                hlaAvgT.ToolTip = intMinT.ToString(InvariantCulture);
                hlaAvgT.AnchorY = intMinT;
                hlaAvgT.LineDashStyle = ChartDashStyle.DashDot;
                chActive.Annotations.Add(hlaAvgT);

                using VerticalLineAnnotation vlaSearchLimit = new GalaxyApp().CreatVerticalLineAnnotation(string.Format(InvariantCulture, "sirSL{0}{1:d2}", dicEachResult.Key, intLNums), chaArea);
                vlaSearchLimit.LineColor = Color.Green;
                vlaSearchLimit.ToolTip = vlaSearchLimit.AnchorX.ToString(InvariantCulture);
                vlaSearchLimit.AnchorX = (dtCurve.Rows.Count - stuGLSearch.InSearchLimit + 1) > 0 ? dtCurve.Rows.Count - stuGLSearch.InSearchLimit + 1 : 0;
                chActive.Annotations.Add(vlaSearchLimit);

                #endregion
                //}
                #endregion Set Chart 


            }
            #endregion Loop
            #endregion
        }

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
        }

    }
}
