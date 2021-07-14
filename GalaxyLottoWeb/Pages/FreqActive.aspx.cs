using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqActive : BasePage
    {
        private string _action;
        private string _requestId;
        private StuGLSearch _gstuSearch;
        private Dictionary<string, string> _dicNumcssclass;
        private Dictionary<string, DataTable> _dicFreqActive;
        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }


            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[_action + _requestId] != null)
            {
                if (ViewState["_gstuSearch"] == null)
                {
                    ViewState.Add("_gstuSearch", (StuGLSearch)Session[_action + _requestId]);
                }
                else
                {
                    ViewState["_gstuSearch"] = (StuGLSearch)Session[_action + _requestId];
                };
            }
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            if (ViewState["_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState["_gstuSearch"];
                if (ViewState["title"] == null)
                {
                    ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "活性總表", new CglDBData().SetTitleString(_gstuSearch)));
                }
                if (ViewState["CurrentData"] == null)
                {
                    ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch)));
                }
                if (ViewState["_dicNumcssclass"] == null)
                {
                    ViewState.Add("_dicNumcssclass", new GalaxyApp().GetNumcssclass(_gstuSearch));
                }
                if (ViewState["_dicFreqActive"] == null)
                {
                    _gstuSearch.InDisplayPeriod = (_gstuSearch.InDisplayPeriod < _gstuSearch.InSearchLimit) ? _gstuSearch.InSearchLimit : _gstuSearch.InDisplayPeriod;
                    ViewState.Add("_dicFreqActive", new CglFreq().GetFreqActiveDic(_gstuSearch));
                }

                _dicNumcssclass = (Dictionary<string, string>)ViewState["_dicNumcssclass"];
                _dicFreqActive = (Dictionary<string, DataTable>)ViewState["_dicFreqActive"];
                ShowResult(_gstuSearch);
            }
            CurrentSearchOrderID = string.Empty;
        }

        private void ShowResult(StuGLSearch stuGLSearch)
        {
            Page.Title = (string)ViewState["title"];
            lblTitle.Text = (string)ViewState["title"];
            lblMethod.Text = new CglMethod().SetMethodString(stuGLSearch);
            lblSearchMethod.Text = new CglMethod().SetSearchMethodString(stuGLSearch);

            // 當期資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            SetddlFreq();
            ShowDetail(ddlFreq.SelectedValue);
        }

        private void ShowDetail(string selectedValue)
        {
            foreach (var dicEachResult in _dicFreqActive)
            {
                Panel pnlFreq = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlFrequency_{0}{1}", dicEachResult.Key, selectedValue),
                                                           "max-width");

                pnlFreq.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "FreqActive_{0}{1}", dicEachResult.Key, selectedValue),
                                                string.Format(InvariantCulture, "FreqActive_{0}({1})", selectedValue, _dicFreqActive[selectedValue].Rows.Count),
                                                "gllabel"));
                pnlDetail.Controls.Add(pnlFreq);


                #region Set panel Frequency Result
                DataTable dtFreqResult = _dicFreqActive[selectedValue].Copy();
                dtFreqResult.Columns.Remove("lngFreqSN");
                dtFreqResult.Columns.Remove("lngMethodSN");

                GridView gvFreqActive = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "gvFreqActive_{0}", dicEachResult.Key), "gltable", dtFreqResult, true, false);

                #region Set Css
                foreach (DataControlField dcColumn in gvFreqActive.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if (_dicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                    {
                        dcColumn.HeaderStyle.CssClass = _dicNumcssclass[strColumnName.Substring(4)];
                    }
                }
                #endregion

                gvFreqActive.RowDataBound += GvFreqActive_RowDataBound;
                gvFreqActive.DataBind();
                pnlFreq.Controls.Add(gvFreqActive);
                #endregion Set panel Frequency Result

            }
        }

        private void SetddlFreq()
        {
            if (ddlFreq.Items.Count == 0)
            {
                foreach (var keyval in _dicFreqActive)
                {
                    ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(keyval.Key), keyval.Key));
                }
            }
        }

        /// <summary>
        /// change CSS of the cells when datarow loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GvFreqActive_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Dictionary<string, string> dicNumcssclass = new Dictionary<string, string>();
                bool RangeBool = false;
                int RangeCount = 0;

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
                            if (strCell_ColumnName == "lngTotalSN")
                            {
                                StuGLSearch stuGLSearchTemp = _gstuSearch;
                                stuGLSearchTemp.LngTotalSN = long.Parse(cell.Text, InvariantCulture);
                                List<int> lstDataNums = (List<int>)new CglData().GetDataNumsLst(stuGLSearchTemp);
                                if (lstDataNums.Sum() > 0)
                                {
                                    for (int index = 0; index < lstDataNums.Count; index++)
                                    {
                                        dicNumcssclass.Add(lstDataNums[index].ToString(InvariantCulture), string.Format(InvariantCulture, " lngM{0} ", index + 1));
                                    }
                                }
                                //dicNumcssclass = GetdicNumcssclass(stuGLSearchTemp);
                            }
                            else
                            {
                                if (dicNumcssclass.Keys.Contains(strCell_ColumnName.Substring(4)))
                                {
                                    RangeBool = true;
                                    cell.CssClass = dicNumcssclass[strCell_ColumnName.Substring(4)];
                                    RangeCount++;
                                }
                                if (RangeCount == new CglDataSet(_gstuSearch.LottoType).CountNumber)
                                {
                                    RangeBool = false;
                                }
                                if (RangeBool)
                                {
                                    cell.CssClass += " lngM0 ";
                                }
                                if (int.Parse(cell.Text, InvariantCulture) == _gstuSearch.InCriticalNum)
                                {
                                    cell.CssClass += " lngC1 ";
                                }
                                if (int.Parse(cell.Text, InvariantCulture) == _gstuSearch.InCriticalNum + 1)
                                {
                                    cell.CssClass += " lngC2 ";
                                }
                                if (int.Parse(cell.Text, InvariantCulture) >= _gstuSearch.InCriticalNum + 2)
                                {
                                    cell.CssClass += "lngC3";
                                }
                                cell.CssClass = int.Parse(strCell_ColumnName.Substring(4), InvariantCulture) % 5 == 0 ? cell.CssClass + " glColNum5 " : cell.CssClass;
                            }
                        }

                        #endregion Set Saturday
                    }

                }
                if (e.Row.RowIndex == _gstuSearch.InSearchLimit - 1)
                {
                    e.Row.CssClass = !string.IsNullOrEmpty(e.Row.CssClass) ? e.Row.CssClass + " glRowSearchLimit" : "glRowSearchLimit";
                }
            }
        }
        protected void IBInfoClick(object sender, ImageClickEventArgs e)
        {
            pnlTopData.Visible = !pnlTopData.Visible;
        }

    }
}
