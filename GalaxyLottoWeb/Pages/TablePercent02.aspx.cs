using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class TablePercent02 : BasePage
    {
        private StuGLSearch _gstuSearch;
        private Dictionary<string, string> dicNumcssclass;
        private string _action, _requestId, strHtmlDirectory, strFnTPxml;
        private string TablePercent02ID;

        private string LocalBrowserType { get; set; }

        private string LocalIP { get; set; }

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            LocalBrowserType = Request.Browser.Type;
            LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();

            if (!IsPostBack) { SetupViewState(); }
            if (_gstuSearch.LottoType == TargetTable.None || _gstuSearch.LngTotalSN == 0)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState["_gstuSearch"];
                dicNumcssclass = new GalaxyApp().GetNumcssclass(_gstuSearch);
                //dicCurrentNums = new CglData().GetCurrentDataNums(gstuSearch);
                if (ViewState["title"] == null)
                {
                    ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "百分比活性表", new CglDBData().SetTitleString(_gstuSearch)));
                }
                Page.Title = ViewState["title"].ToString();
                ShowResult(_gstuSearch);
            }
            CurrentSearchOrderID = string.Empty;
        }

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            TablePercent02ID = _action + _requestId;

            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[TablePercent02ID] != null)
            {
                if (ViewState["_gstuSearch"] == null)
                {
                    ViewState.Add("_gstuSearch", (StuGLSearch)Session[TablePercent02ID]);
                }
                else
                {
                    ViewState["_gstuSearch"] = (StuGLSearch)Session[TablePercent02ID];
                };
            }
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void ShowResult(StuGLSearch stuGLSearch)
        {
            lblTitle.Text = Page.Title;
            // 當期資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(stuGLSearch)), true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            #region dicTablePercent
            if (ViewState["dicTablePercent"] == null)
            {
                strHtmlDirectory = System.IO.Path.Combine(Server.MapPath("~"), "xml");
                strFnTPxml = string.Format(InvariantCulture, "{0}{1}.xml", "TP", string.Format(InvariantCulture, "{0}{1}", _gstuSearch.LottoType.ToString(), new CglData().GetCurrentDataDics(_gstuSearch)["lngDateSN"]));
                ViewState.Add("dicTablePercent", new CglTablePercent().GetTP(stuGLSearch, strHtmlDirectory, strFnTPxml));
            }
            Dictionary<string, object> dicTablePercent = (Dictionary<string, object>)ViewState["dicTablePercent"];
            #endregion

            #region Show the result
            //dtTrend
            using DataTable dtTrend = new DataTable("dtTrend") { Locale = InvariantCulture };
            #region Columns
            DataColumn[] ColumnKey = new DataColumn[1];
            dtTrend.Columns.Add(new DataColumn
            {
                ColumnName = "strCompares",
                DataType = typeof(string)
            });
            ColumnKey[0] = dtTrend.Columns["strCompares"];
            dtTrend.PrimaryKey = ColumnKey;
            for (int i = 1; i <= new CglDataSet(stuGLSearch.LottoType).LottoNumbers; i++)
            {
                dtTrend.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "lngN{0}", i),
                    DataType = typeof(double),
                    DefaultValue = 0
                });
            }
            #endregion Columns

            #region Creat each trend Table

            #region Button
            HyperLink btButtonTrend = new GalaxyApp().CreatHyperLink(string.Format(InvariantCulture, "hl{0}", "btButtonTrend"), "glbutton glbutton-lightblue ", "趨勢",
                                                                     new Uri(string.Format(InvariantCulture, "#pnl{0}", "Trend")));
            pnlButtons.Controls.Add(btButtonTrend);
            #endregion Button

            #region Panel
            Panel pnlTrend = new Panel() { ID = string.Format(InvariantCulture, "pnl{0}", "Trend"), CssClass = "max-width" };
            pnlDetail.Controls.Add(pnlTrend);
            #endregion Panel

            #endregion Creat each trend Table

            #region Loop
            foreach (var KeyVal in dicTablePercent)
            {
                //string strTitle = string.Empty;
                string strButtonText = string.Empty;
                if ((KeyVal.Key != "dicTotal") && (KeyVal.Key != "strDayTwentyEight#strHourTwentyEight"))
                {
                    int DataRows = int.Parse(((Dictionary<string, object>)KeyVal.Value)["DataRows"].ToString(), InvariantCulture);
                    if (DataRows >= stuGLSearch.InDataRowsLimit)
                    {
                        #region Open FreqActive Pages
                        if (stuGLSearch.ShowProcess == ShowProcess.Visible)
                        {
                            StuGLSearch stuSearchNew = stuGLSearch;
                            stuSearchNew.StrCompares = KeyVal.Key;
                            stuSearchNew.InTestPeriods = 1;
                            OpenPageMuti(stuSearchNew, Properties.Resources.SessionsFreqActiveChart, Properties.Resources.PageFreqActiveChart, Properties.Resources.SessionsFreqActiveChart, LocalIP, LocalBrowserType);
                        }
                        #endregion Open FreqActive Pages

                        #region SetButtons
                        #region Set Button text
                        List<string> lstCompare = KeyVal.Key.Split('#').ToList();
                        List<string> lstButtonText = new List<string>();
                        foreach (string strCompareOption in lstCompare)
                        {
                            lstButtonText.Add(new CglFunc().ConvertFieldNameId(strCompareOption, 1));
                        }
                        strButtonText = string.Join(",", lstButtonText.ToArray());
                        #endregion Set Button text
                        HyperLink btButton = new GalaxyApp().CreatHyperLink(string.Format(InvariantCulture, "hl{0}", KeyVal.Key), "glbutton glbutton-lightblue ", strButtonText,
                                                                            new Uri(string.Format(InvariantCulture, "#pnl{0}", KeyVal.Key)));
                        pnlButtons.Controls.Add(btButton);
                        #endregion SetButtons

                        #region Set Each Compare Panel
                        Panel pnlEachCompare = new Panel() { ID = string.Format(InvariantCulture, "pnl{0}", KeyVal.Key), CssClass = "max-width" };
                        pnlEachCompare.Controls.Add(new Label() { Text = string.Format(InvariantCulture, "相同 ({0}) ({1:00}期)", strButtonText, DataRows), CssClass = "gllabel" });

                        #region Set Compare 
                        StuGLSearch stuSearchTemp = stuGLSearch;
                        stuSearchTemp.StrCompares = KeyVal.Key;
                        Dictionary<string, DataTable> dicActive = new CglFreq().GetFreqActiveDic(stuSearchTemp);
                        #endregion Set Compare 

                        #region Freq Part
                        foreach (var dtFreqActive in dicActive)
                        {
                            #region Set panel Frequency Result
                            //pnlFreq.Controls.Add(new Label() { Text = string.Format(InvariantCulture, "{0} Freq", dicFreqResult.Key) });
                            Panel pnlFreq = new Panel()
                            {
                                ID = string.Format(InvariantCulture, "pnl{0}", KeyVal.Key),
                                CssClass = "max-width"
                            };
                            pnlFreq.Controls.Add(new Label() { ID = string.Format(InvariantCulture, "{0}FreqActive", KeyVal.Key), CssClass = "gllabel", Text = string.Format(InvariantCulture, "顯示期數 {0}", dtFreqActive.Value.Rows.Count) });

                            //DataTable dtProResult = new CGLFunc().CTableShow(dicProcessResultT[dicFreqResult.Key]);
                            DataTable dtFreqResult = dtFreqActive.Value;
                            dtFreqResult.Columns.Remove("lngTotalSN");
                            dtFreqResult.Columns.Remove("lngFreqSN");
                            //dtProcessResult.DefaultView.Sort = "[lngTotalSN] DESC ";
                            GridView gvFreqActive = new GridView()
                            {
                                ID = string.Format(InvariantCulture, "{0}gvFreqActive", KeyVal.Key),
                                AutoGenerateColumns = false,
                                AllowSorting = true,
                                EnableViewState = false,
                                ViewStateMode = ViewStateMode.Disabled,
                                CssClass = "gltable",
                                GridLines = GridLines.Horizontal,
                                DataSource = dtFreqResult.DefaultView
                            };

                            #region Set Columns of DataGrid gvProcess
                            if (gvFreqActive.Columns.Count == 0)
                            {
                                for (int i = 0; i < dtFreqResult.Columns.Count; i++)
                                {
                                    string strColumnName = dtFreqResult.Columns[i].ColumnName;
                                    BoundField bfCell = new BoundField()
                                    {
                                        DataField = strColumnName,
                                        HeaderText = new CglFunc().ConvertFieldNameId(strColumnName, 1),
                                        //SortExpression = strColumnName
                                    };
                                    if (strColumnName.Substring(0, 4) == "lngN")
                                    {
                                        if (dicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                                        {
                                            bfCell.HeaderStyle.CssClass = dicNumcssclass[strColumnName.Substring(4)];
                                        }
                                        if (int.Parse(strColumnName.Substring(4), InvariantCulture) % 5 == 0)
                                        {
                                            bfCell.ItemStyle.CssClass = "glColNum5";
                                        }
                                    }
                                    gvFreqActive.Columns.Add(bfCell);
                                }
                            }
                            #endregion Set Columns of DataGrid gvProcess
                            gvFreqActive.RowDataBound += GvProcess_RowDataBound;
                            gvFreqActive.DataBind();
                            pnlFreq.Controls.Add(gvFreqActive);
                            #endregion Set panel Frequency Result
                            pnlEachCompare.Controls.Add(pnlFreq);

                            #region dtTrend
                            DataRow drTrend = dtTrend.NewRow();
                            drTrend["strCompares"] = strButtonText;
                            for (int i = 1; i <= new CglDataSet(stuGLSearch.LottoType).LottoNumbers; i++)
                            {
                                drTrend[string.Format(InvariantCulture, "lngN{0}", i)] = CglFreq.GetTrend(stuSearchTemp, dtFreqActive.Value, i);
                            }
                            #endregion dtTrend
                            dtTrend.Rows.Add(drTrend);
                        }
                        #endregion Freq Part

                        #endregion Set Each Compare Panel
                        pnlDetail.Controls.Add(pnlEachCompare);
                    }
                }
            }
            #endregion Loop

            #region Sum of dtTrend
            DataRow drTrendSum = dtTrend.NewRow();
            drTrendSum["strCompares"] = "SUM";
            for (int i = 1; i <= new CglDataSet(stuGLSearch.LottoType).LottoNumbers; i++)
            {
                drTrendSum[string.Format(InvariantCulture, "lngN{0}", i)] = Math.Round((double)dtTrend.Compute(string.Format(InvariantCulture, "SUM([lngN{0}])", i), string.Empty), 2);
            }
            dtTrend.Rows.Add(drTrendSum);
            #endregion Sum of dtTrend

            GridView gvTrend = new GridView()
            {
                ID = string.Format(InvariantCulture, "gvTrend"),
                AutoGenerateColumns = false,
                AllowSorting = true,
                EnableViewState = false,
                ViewStateMode = ViewStateMode.Disabled,
                CssClass = "gltable",
                GridLines = GridLines.Horizontal,
                DataSource = dtTrend.DefaultView
            };

            #region Set Columns of DataGrid gvProcess
            if (gvTrend.Columns.Count == 0)
            {
                for (int i = 0; i < dtTrend.Columns.Count; i++)
                {
                    string strColumnName = dtTrend.Columns[i].ColumnName;
                    BoundField bfCell = new BoundField()
                    {
                        DataField = strColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(strColumnName, 1),
                        DataFormatString = "{0:F2}",
                        //SortExpression = strColumnName
                    };
                    if (strColumnName.Substring(0, 4) == "lngN")
                    {
                        if (dicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                        {
                            bfCell.HeaderStyle.CssClass = dicNumcssclass[strColumnName.Substring(4)];
                        }
                        if (int.Parse(strColumnName.Substring(4), InvariantCulture) % 5 == 0)
                        {
                            bfCell.ItemStyle.CssClass = "glColNum5";
                        }
                    }
                    gvTrend.Columns.Add(bfCell);
                }
            }
            #endregion
            gvTrend.DataBind();
            pnlTrend.Controls.Add(gvTrend);
            #endregion Show the result
        }

        private void GvProcess_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

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
                        else
                        {
                            if (strCell_ColumnName != "lngMethodSN")
                            {
                                if (int.Parse(cell.Text, InvariantCulture) == _gstuSearch.InCriticalNum)
                                {
                                    cell.CssClass = "lngM1";
                                }
                                if (int.Parse(cell.Text, InvariantCulture) == _gstuSearch.InCriticalNum + 1)
                                {
                                    cell.CssClass = "lngM4";
                                }
                                if (int.Parse(cell.Text, InvariantCulture) >= _gstuSearch.InCriticalNum + 2)
                                {
                                    cell.CssClass = "lngM3";
                                }
                                cell.CssClass = int.Parse(strCell_ColumnName.Substring(4), InvariantCulture) % 5 == 0 ? cell.CssClass + " glColNum5 " : cell.CssClass;
                            }
                        }

                        #endregion Set Saturday
                    }

                }
            }
        }

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            _action = Request["action"];
            _requestId = Request["id"];
            Session.Remove(name: _action + _requestId);
        }

        internal void OpenPageMuti(StuGLSearch stuGLSearch, string action, string fileName, string windowName, string LocalIP, string LocalBrowserType)
        {
            Session["action"] = action;
            //Session["action"] = action;
            Session["UrlFileName"] = fileName;
            //stuGLSearch = new CglSearch().InitSearch(stuGLSearch);
            if (stuGLSearch.InTestPeriods > 1)
            {
                DataTable dtProcess = CglFreqProcess.GetFreqProcAlls(stuGLSearch, CglDBFreq.TableName.QryFreqProcess01, SortOrder.Descending, stuGLSearch.InTestPeriods - 1);

                foreach (DataRow drRow in dtProcess.Rows)
                {
                    StuGLSearch stuSearchTemp = stuGLSearch;
                    stuSearchTemp.LngTotalSN = long.Parse(drRow["lngTotalSN"].ToString(), InvariantCulture);
                    stuSearchTemp.InTestPeriods = 1;
                    Session["id"] = SetRequestId(stuSearchTemp);
                    Session[action + SetRequestId(stuSearchTemp)] = stuSearchTemp;
                    if (stuGLSearch.SearchOrder)
                    {
                        SetSearchOrder(stuSearchTemp, action, SetRequestId(stuSearchTemp), fileName, LocalIP, LocalBrowserType);
                    }
                    else
                    {
                        OpenPage(stuSearchTemp, Request.Url.Authority, action, fileName, windowName);
                    }
                }
            }
        }

        private void OpenPage(StuGLSearch stuGLSearch, string requestUrl, string action, string fileName, string windowName)
        {
            //string action = "DataN";
            string strScript;
            string id = SetRequestId(stuGLSearch);
            Session["id"] = id;
            Session[action + id] = stuGLSearch;
            string strUrl = string.Format(InvariantCulture, "http://{0}/Pages/{1}?action={2}&id={3}", requestUrl, fileName, action, id);
            if (stuGLSearch.ShowStaticHtml)
            {
                List<int> lstCurrentNums = (List<int>)new CglData().GetDataNumsLst(stuGLSearch);
                #region Check File                    
                string strFileName = lstCurrentNums.Sum().Equals(0) ? string.Format(InvariantCulture, "{0}{1}Temp.html", action, id) : string.Format(InvariantCulture, "{0}{1}.html", action, id);
                string strCurrentDirectory = Server.MapPath("~");
                string strHtmlDirectory = System.IO.Path.Combine(strCurrentDirectory, "html");
                if (!lstCurrentNums.Sum().Equals(0))
                {
                    DeleteTempFile(strHtmlDirectory, action, id);
                }
                #endregion Check File

                #region Creat static HTML file
                if (!new CglFunc().FileExist(strHtmlDirectory, strFileName) || stuGLSearch.Recalculate)
                {
                    string strFilePathName = Path.Combine(strHtmlDirectory, strFileName);
                    new GalaxyApp().CreatStaticHtml(new Uri(strUrl, UriKind.Absolute), strFilePathName, 3000000);
                }
                #endregion Creat static HTML file

                strUrl = string.Format(InvariantCulture, "http://{0}/html/{1}", requestUrl, strFileName);
            }
            strScript = string.Format(InvariantCulture, "window.open('{0}','{1}');", strUrl, windowName);
            Response.Write(string.Format(InvariantCulture, "<script language='javascript'>{0};</script>", strScript));
        }
        private static void DeleteTempFile(string strHtmlDirectory, string action, string id)
        {
            string strFileName = string.Format(InvariantCulture, "{0}{1}Temp.html", action, id);
            if (new CglFunc().FileExist(strHtmlDirectory, strFileName))
            {
                string strFilePath = Path.Combine(strHtmlDirectory, strFileName);
                File.Delete(strFilePath);
            }
        }
    }
}