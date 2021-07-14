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
    public partial class FreqActive02 : BasePage
    {
        private string _action;
        private string _requestId;
        private StuGLSearch _gstuSearch;
        private Dictionary<string, string> _dicNumcssclass;
        private static Dictionary<string, Dictionary<string, int>> dicActiveSum;
        private readonly List<string> lstFields = new List<string> { "gen", "strDayFive", "strDayTwelve", "strDayNine", "strDayTwentyEight", "strHourT", "strHourTwentyEight", "strDayEight", "strp13" };

        private static Dictionary<string, Dictionary<string, int>> DicActiveSum
        {
            get
            {
                if (dicActiveSum == null)
                {
                    DicActiveSum = new Dictionary<string, Dictionary<string, int>>
                {
                    { "-+", new Dictionary<string, int>() },
                    { "--", new Dictionary<string, int>() },
                    { "+-", new Dictionary<string, int>() },
                    { "++", new Dictionary<string, int>() },
                    { "LastNumS", new Dictionary<string, int>() },
                    { "LastNum", new Dictionary<string, int>() } }
    ;
                }
                return dicActiveSum;
            }

            set => dicActiveSum = value;
        }

        // ---------------------------------------------------------------------------------------------------------
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
                if (!IsPostBack)
                {
                    if (ViewState["title"] == null) { ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "活性表02", new CglDBData().SetTitleString(_gstuSearch))); }
                    if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch))); }
                    if (ViewState["lblMethod"] == null) { ViewState.Add("lblMethod", new CglMethod().SetMethodString(_gstuSearch)); }
                    if (ViewState["lblSearchMethod"] == null) { ViewState.Add("lblSearchMethod", new CglMethod().SetSearchMethodString(_gstuSearch)); }
                    if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch))); }
                    if (ViewState["_dicNumcssclass"] == null) { ViewState.Add("_dicNumcssclass", new GalaxyApp().GetNumcssclass(_gstuSearch)); }
                    GetData();
                }
                _dicNumcssclass = (Dictionary<string, string>)ViewState["_dicNumcssclass"];
                ShowResult();
            }
            ResetSearchOrder();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void GetData()
        {
            if (ViewState["Sum"] == null)
            {

                #region loop
                foreach (string field in lstFields)
                {
                    StuGLSearch stuGLSearchTemp = _gstuSearch;
                    stuGLSearchTemp.FieldMode = (field != "gen");
                    stuGLSearchTemp.StrCompares = field;
                    stuGLSearchTemp = new CglMethod().GetMethodSN(stuGLSearchTemp);
                    // 取資料
                    Dictionary<string, object> _dicLastNumsWithFreqSec = new CglFreqSec().GetLastNumsWithFreqSecDic(stuGLSearchTemp);
                    Dictionary<string, DataTable> _dicFreqSec = new CglFreqSec().GetFreqSecDic(stuGLSearchTemp);

                    foreach (KeyValuePair<string, DataTable> KeyVal in _dicFreqSec)
                    {
                        Dictionary<string, Dictionary<string, int>> dicActiveEach = new Dictionary<string, Dictionary<string, int>>
                    {
                        { "-+", new Dictionary<string, int>() },
                        { "--", new Dictionary<string, int>() },
                        { "+-", new Dictionary<string, int>() },
                        { "++", new Dictionary<string, int>() },
                        { "LastNumS", new Dictionary<string, int>() }
                    };
                        Dictionary<string, List<int>> dicLastNums = (Dictionary<string, List<int>>)_dicLastNumsWithFreqSec[KeyVal.Key];

                        #region Calculate
                        foreach (DataRow drRow in KeyVal.Value.Rows)
                        {
                            string lngN = drRow["lngN"].ToString();
                            double dblACT = double.Parse(drRow["sglACT"].ToString(), InvariantCulture);
                            double dblACT1 = double.Parse(drRow["sglACT1"].ToString(), InvariantCulture);
                            if (dblACT < 0 && dblACT1 > 0)
                            {
                                if (dicActiveEach["-+"].ContainsKey(lngN)) { dicActiveEach["-+"][lngN]++; }
                                else { dicActiveEach["-+"].Add(lngN, 1); }

                                if (DicActiveSum["-+"].ContainsKey(lngN)) { DicActiveSum["-+"][lngN]++; }
                                else { DicActiveSum["-+"].Add(lngN, 1); }

                            };
                            if (dblACT < 0 && dblACT1 < 0)
                            {
                                if (dicActiveEach["--"].ContainsKey(lngN)) { dicActiveEach["--"][lngN]++; }
                                else { dicActiveEach["--"].Add(lngN, 1); }

                                if (DicActiveSum["--"].ContainsKey(lngN)) { DicActiveSum["--"][lngN]++; }
                                else { DicActiveSum["--"].Add(lngN, 1); }

                            };
                            if (dblACT > 0 && dblACT1 < 0)
                            {
                                if (dicActiveEach["+-"].ContainsKey(lngN)) { dicActiveEach["+-"][lngN]++; }
                                else { dicActiveEach["+-"].Add(lngN, 1); }

                                if (DicActiveSum["+-"].ContainsKey(lngN)) { DicActiveSum["+-"][lngN]++; }
                                else { DicActiveSum["+-"].Add(lngN, 1); }

                            };
                            if (dblACT > 0 && dblACT1 > 0)
                            {
                                if (dicActiveEach["++"].ContainsKey(lngN)) { dicActiveEach["++"][lngN]++; }
                                else { dicActiveEach["++"].Add(lngN, 1); }

                                if (DicActiveSum["++"].ContainsKey(lngN)) { DicActiveSum["++"][lngN]++; }
                                else { DicActiveSum["++"].Add(lngN, 1); }

                            };
                        }
                        foreach (var LastNum in dicLastNums)
                        {
                            foreach (int intNum in LastNum.Value)
                            {
                                string lngN = intNum.ToString(InvariantCulture);
                                if (dicActiveEach["LastNumS"].ContainsKey(lngN))
                                {
                                    dicActiveEach["LastNumS"][lngN]++;
                                }
                                else
                                {
                                    dicActiveEach["LastNumS"].Add(lngN, 1);
                                }
                                if (DicActiveSum["LastNumS"].ContainsKey(lngN))
                                {
                                    DicActiveSum["LastNumS"][lngN]++;
                                }
                                else
                                {
                                    DicActiveSum["LastNumS"].Add(lngN, 1);
                                }
                            }
                        }
                        #endregion Calculate

                        #region Show the result
                        using DataTable dtActiveEach = new DataTable { Locale = InvariantCulture };

                        #region Set Column
                        if (dtActiveEach.Columns.Count == 0)
                        {
                            dtActiveEach.Columns.Add(new DataColumn
                            {
                                ColumnName = "item",
                                DataType = typeof(string)
                            });
                            for (int intNums = 1; intNums <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; intNums++)
                            {
                                dtActiveEach.Columns.Add(new DataColumn
                                {
                                    ColumnName = string.Format(InvariantCulture, "lngN{0}", intNums),
                                    DataType = typeof(int),
                                    DefaultValue = 0
                                });
                            }
                        }
                        #endregion Set Column

                        #region translat Dic to DataTable
                        foreach (var ItemActiveEach in dicActiveEach)
                        {
                            DataRow drRow = dtActiveEach.NewRow();
                            drRow["item"] = ItemActiveEach.Key;
                            foreach (var Item in ItemActiveEach.Value)
                            {
                                drRow[string.Format(InvariantCulture, "lngN{0}", Item.Key)] = Item.Value;
                            }
                            dtActiveEach.Rows.Add(drRow);
                        }
                        #endregion translat Dic to DataTable
                        ViewState.Add(field, dtActiveEach);

                        #endregion Show the result
                    }
                    foreach (int intNum in (List<int>)new CglData().GetDataNumsLst(new CglData().GetPrevDate(stuGLSearchTemp)))
                    {
                        string lngN = intNum.ToString(InvariantCulture);
                        if (DicActiveSum["LastNum"].ContainsKey(lngN))
                        {
                            DicActiveSum["LastNum"][lngN]++;
                        }
                        else
                        {
                            DicActiveSum["LastNum"].Add(lngN, 1);
                        }
                    }
                }

                #endregion loop

                using DataTable dtActiveSum = new DataTable { Locale = InvariantCulture };

                #region Set Column
                if (dtActiveSum.Columns.Count == 0)
                {
                    dtActiveSum.Columns.Add(new DataColumn
                    {
                        ColumnName = "item",
                        DataType = typeof(string)
                    });
                    for (int intNums = 1; intNums <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; intNums++)
                    {
                        dtActiveSum.Columns.Add(new DataColumn
                        {
                            ColumnName = string.Format(InvariantCulture, "lngN{0}", intNums),
                            DataType = typeof(int),
                            DefaultValue = 0
                        });
                    }
                }
                #endregion Set Column

                #region translat Dic to DataTable
                foreach (var ItemActiveSum in DicActiveSum)
                {
                    DataRow drRow = dtActiveSum.NewRow();
                    drRow["item"] = ItemActiveSum.Key;
                    foreach (var Item in ItemActiveSum.Value)
                    {
                        drRow[string.Format(InvariantCulture, "lngN{0}", Item.Key)] = Item.Value;
                    }
                    dtActiveSum.Rows.Add(drRow);
                }
                #endregion translat Dic to DataTable
                ViewState.Add("Sum", dtActiveSum);
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void ShowResult()
        {
            Page.Title = (string)ViewState["title"];
            lblTitle.Text = (string)ViewState["title"];
            lblMethod.Text = (string)ViewState["lblMethod"];
            lblSearchMethod.Text = (string)ViewState["lblSearchMethod"];

            // 當期資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            SetddlFreq();

            ShowDetail(ddlFreq.SelectedValue);


        }

        private void ShowDetail(string selectedValue)
        {
            #region Show the result
            Panel pnlActiveSum = new GalaxyApp().CreatPanel("pnlActiveSum", "max-width");
            pnlDetail.Controls.Add(pnlActiveSum);

            Label lblActiveSum = new GalaxyApp().CreatLabel("lblActiveSum", new CglFunc().ConvertFieldNameId(selectedValue), "gllabel");
            pnlActiveSum.Controls.Add(lblActiveSum);

            GridView gvActiveSum = new GalaxyApp().CreatGridView("gvActiveSum", "gltable", (DataTable)ViewState[selectedValue], true, false);
            foreach (DataControlField dcColumn in gvActiveSum.Columns)
            {
                string strColumnName = dcColumn.SortExpression;
                if (_dicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                {
                    dcColumn.HeaderStyle.CssClass = _dicNumcssclass[strColumnName.Substring(4)];
                    dcColumn.ItemStyle.CssClass = "glColNum5";
                }
            }

            if (selectedValue == "Sum")
            {
                gvActiveSum.RowDataBound += GvActiveSum_RowDataBound;

            }
            else
            {
                gvActiveSum.RowDataBound += GvActiveEach_RowDataBound;
            }
            gvActiveSum.DataBind();
            pnlActiveSum.Controls.Add(gvActiveSum);
            #endregion Show the result
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:不要將常值當做已當地語系化的參數傳遞", MessageId = "System.Web.UI.WebControls.ListItem.#ctor(System.String,System.String)")]
        private void SetddlFreq()
        {
            if (ddlFreq.Items.Count == 0)
            {
                ddlFreq.Items.Add(new ListItem("合計", "Sum"));
                foreach (string field in lstFields)
                {
                    ListItem listItem = new ListItem(new CglFunc().ConvertFieldNameId(field), field);
                    if (field == (string)ViewState["selectedValue"]) { listItem.Selected = true; }
                    ddlFreq.Items.Add(listItem);
                }
            }
        }

        private void GvActiveEach_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[0].Text == "LastNumS")
                {
                    foreach (DataControlFieldCell cell in e.Row.Cells)
                    {
                        string strCell_ColumnName = ((BoundField)cell.ContainingField).DataField;
                        if (strCell_ColumnName != "item")
                        {
                            if (_dicNumcssclass.Keys.Contains(strCell_ColumnName.Substring(4)))
                            {
                                cell.CssClass = "glColNum5";
                            }
                            if (int.Parse(cell.Text, InvariantCulture) >= 3)
                            {
                                cell.CssClass += " glValueMaxNum ";
                            }
                        }
                    }
                }
                else
                {
                    foreach (DataControlFieldCell cell in e.Row.Cells)
                    {
                        string strCell_ColumnName = ((BoundField)cell.ContainingField).DataField;
                        if (strCell_ColumnName != "item")
                        {
                            if (_dicNumcssclass.Keys.Contains(strCell_ColumnName.Substring(4)))
                            {
                                cell.CssClass = "glColNum5";
                            }
                            if (int.Parse(cell.Text, InvariantCulture) >= 1)
                            {
                                cell.CssClass += " glValueMaxNum ";
                            }
                        }
                    }
                }
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void GvActiveSum_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[0].Text == "+-")
                {
                    foreach (DataControlFieldCell cell in e.Row.Cells)
                    {
                        string strCell_ColumnName = ((BoundField)cell.ContainingField).DataField;
                        if (strCell_ColumnName != "item")
                        {
                            if (_dicNumcssclass.Keys.Contains(strCell_ColumnName.Substring(4)))
                            {
                                cell.CssClass = "glColNum5";
                            }
                            if (int.Parse(cell.Text, InvariantCulture) >= 3)
                            {
                                cell.CssClass += " glValueMaxNum ";
                            }
                        }
                    }
                }

                if (e.Row.Cells[0].Text == "++")
                {
                    foreach (DataControlFieldCell cell in e.Row.Cells)
                    {
                        string strCell_ColumnName = ((BoundField)cell.ContainingField).DataField;
                        if (strCell_ColumnName != "item")
                        {
                            if (_dicNumcssclass.Keys.Contains(strCell_ColumnName.Substring(4)))
                            {
                                cell.CssClass = "glColNum5";
                            }
                            if (int.Parse(cell.Text, InvariantCulture) <= 4)
                            {
                                cell.CssClass += " glValueMinNum ";
                            }
                            if (int.Parse(cell.Text, InvariantCulture) >= 9)
                            {
                                cell.CssClass += " glValueMaxNum ";
                            }
                            if (int.Parse(cell.Text, InvariantCulture) == 8)
                            {
                                cell.CssClass += " glValueSecondMaxNum ";
                            }
                        }
                    }
                }
                if (e.Row.Cells[0].Text == "LastNumS")
                {
                    foreach (DataControlFieldCell cell in e.Row.Cells)
                    {
                        string strCell_ColumnName = ((BoundField)cell.ContainingField).DataField;
                        if (strCell_ColumnName != "item")
                        {
                            if (_dicNumcssclass.Keys.Contains(strCell_ColumnName.Substring(4)))
                            {
                                cell.CssClass = "glColNum5";
                            }
                            if (int.Parse(cell.Text, InvariantCulture) < 2)
                            {
                                cell.CssClass += " glValueMinNum ";
                            }
                            if (int.Parse(cell.Text, InvariantCulture) >= 9)
                            {
                                cell.CssClass += " glValueMaxNum ";
                            }
                        }
                    }
                }
                if (e.Row.Cells[0].Text == "LastNum")
                {
                    foreach (DataControlFieldCell cell in e.Row.Cells)
                    {
                        string strCell_ColumnName = ((BoundField)cell.ContainingField).DataField;
                        if (strCell_ColumnName != "item")
                        {
                            if (_dicNumcssclass.Keys.Contains(strCell_ColumnName.Substring(4)))
                            {
                                cell.CssClass = "glColNum5";
                            }
                            if (int.Parse(cell.Text, InvariantCulture) >= 3)
                            {
                                cell.CssClass += " glValueMaxNum ";
                            }
                        }
                    }
                }

            }
        }

        // ---------------------------------------------------------------------------------------------------------


        protected void IBInfoClick(object sender, ImageClickEventArgs e)
        {
            pnlTopData.Visible = !pnlTopData.Visible;
        }

        private void ResetSearchOrder()
        {
            if (CurrentSearchOrderID == _action + _requestId)
            {
                CurrentSearchOrderID = string.Empty;
            }
        }
    }
}