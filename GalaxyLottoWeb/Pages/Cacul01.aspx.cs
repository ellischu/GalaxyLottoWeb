using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class Cacul01 : BasePage
    {
        private StuGLSearch _gstuSearch;
        private string _action;
        private string _requestId;
        private List<int> _lstCurrentNums;
        private Dictionary<string, string> _dicNumcssclass;
        //private Dictionary<string, int> _dicCurrentNums;
        public Dictionary<int, int> DicSectionDP { get; } = new Dictionary<int, int>() { { 5, 20 }, { 10, 40 }, { 25, 100 }, { 50, 200 } };

        private DataTable DtFreqSet01 { get; set; }
        private DataTable DtFreqSet02 { get; set; }
        private DataTable DtPreDateSum { get; set; }
        private DataTable DtPreDate { get; set; }


        private void SetupViewState()
        {
            _action = Request["action"];
            _requestId = Request["id"];

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
                    if (ViewState["title"] == null) { ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "頻率組總表", new CglDBData().SetTitleString(_gstuSearch))); }
                    if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch))); }
                    if (ViewState["lblMethod"] == null) { ViewState.Add("lblMethod", new CglMethod().SetMethodString(_gstuSearch)); }
                    if (ViewState["lblSearchMethod"] == null) { ViewState.Add("lblSearchMethod", new CglMethod().SetSearchMethodString(_gstuSearch)); }
                    if (ViewState["dtFreqSet01"] == null)
                    {
                        using (DtFreqSet01 = new DataTable())
                        {
                            DtFreqSet01.Locale = InvariantCulture;
                            foreach (KeyValuePair<int, int> option in DicSectionDP)
                            {
                                StuGLSearch stuSearchTemp = _gstuSearch;
                                stuSearchTemp.InDataLimit = 0;
                                stuSearchTemp.InDataOffset = 0;
                                stuSearchTemp.InSearchLimit = option.Key;
                                stuSearchTemp.InSearchOffset = 0;
                                stuSearchTemp.InDisplayPeriod = option.Value;
                                stuSearchTemp.StrFreqSet = "gen#strDayFive#strDayTwelve#strDayNine";
                                using DataTable dtTemp = new CglFreqSet().GetFreqSetDS(stuSearchTemp).Tables["dtFreqSet"].Copy();
                                //dtTemp.TableName = string.Format(InvariantCulture, "{0:d2}{1}", option.Key, option.Value);
                                //dsFreqSet.Tables.Add(dtTemp);
                                #region Columns of dtFreqSet
                                if (!DtFreqSet01.Columns.Contains("lngN"))
                                {
                                    DtFreqSet01.Columns.Add(new DataColumn { ColumnName = "lngN", Caption = new CglFunc().ConvertFieldNameId("lngN", 1), DataType = typeof(string) });
                                }
                                if (!DtFreqSet01.Columns.Contains(string.Format(InvariantCulture, "strFreqSet{0:d2}{1}", option.Key, option.Value)))
                                {
                                    DtFreqSet01.Columns.Add(new DataColumn
                                    {
                                        ColumnName = string.Format(InvariantCulture, "strFreqSet{0:d2}{1}", option.Key, option.Value),
                                        Caption = string.Format(InvariantCulture, "FSet{0:d2}{1}", option.Key, option.Value),
                                        DataType = typeof(string)
                                    });
                                }
                                if (!DtFreqSet01.Columns.Contains(string.Format(InvariantCulture, "intHits{0:d2}{1}", option.Key, option.Value)))
                                {
                                    DtFreqSet01.Columns.Add(new DataColumn
                                    {
                                        ColumnName = string.Format(InvariantCulture, "intHits{0:d2}{1}", option.Key, option.Value),
                                        Caption = string.Format(InvariantCulture, "Hits{0:d2}{1}", option.Key, option.Value),
                                        DataType = typeof(string)
                                    });
                                }
                                #endregion Columns of dtFreqSet
                                if (DtFreqSet01.Rows.Count == 0)
                                {
                                    foreach (DataRow drRow in dtTemp.Rows)
                                    {
                                        DataRow drFset = DtFreqSet01.NewRow();
                                        drFset["lngN"] = drRow["lngN"];
                                        drFset[string.Format(InvariantCulture, "strFreqSet{0:d2}{1}", option.Key, option.Value)] = drRow["strFreqSet"];
                                        drFset[string.Format(InvariantCulture, "intHits{0:d2}{1}", option.Key, option.Value)] = drRow["intHits"];
                                        DtFreqSet01.Rows.Add(drFset);
                                    }
                                }
                                else
                                {
                                    foreach (DataRow drRow in DtFreqSet01.Rows)
                                    {
                                        int intRowIndex = DtFreqSet01.Rows.IndexOf(drRow);
                                        drRow[string.Format(InvariantCulture, "strFreqSet{0:d2}{1}", option.Key, option.Value)] = dtTemp.Rows[intRowIndex]["strFreqSet"];
                                        drRow[string.Format(InvariantCulture, "intHits{0:d2}{1}", option.Key, option.Value)] = dtTemp.Rows[intRowIndex]["intHits"];
                                    }
                                }
                            }
                            ViewState.Add("dtFreqSet01", DtFreqSet01);
                        }
                    }
                    if (ViewState["dtFreqSet02"] == null)
                    {
                        using (DtFreqSet02 = new DataTable())
                        {
                            DtFreqSet02.Locale = InvariantCulture;
                            foreach (KeyValuePair<int, int> option in DicSectionDP)
                            {
                                StuGLSearch stuSearchTemp = _gstuSearch;
                                stuSearchTemp.InDataLimit = 0;
                                stuSearchTemp.InDataOffset = 0;
                                stuSearchTemp.InSearchLimit = option.Key;
                                stuSearchTemp.InSearchOffset = 0;
                                stuSearchTemp.InDisplayPeriod = option.Value;
                                stuSearchTemp.StrFreqSet = "gen#strHourT#strHourTwentyEight#strDayEight";
                                using DataTable dtTemp = new CglFreqSet().GetFreqSetDS(stuSearchTemp).Tables["dtFreqSet"].Copy();
                                #region Columns of dtFreqSet
                                if (!DtFreqSet02.Columns.Contains("lngN"))
                                {
                                    DtFreqSet02.Columns.Add(new DataColumn
                                    {
                                        ColumnName = "lngN",
                                        DataType = typeof(string),
                                        Caption = new CglFunc().ConvertFieldNameId("lngN", 1)
                                    });
                                }
                                if (!DtFreqSet02.Columns.Contains(string.Format(InvariantCulture, "strFreqSet{0:d2}{1}", option.Key, option.Value)))
                                {
                                    DtFreqSet02.Columns.Add(new DataColumn
                                    {
                                        ColumnName = string.Format(InvariantCulture, "strFreqSet{0:d2}{1}", option.Key, option.Value),
                                        Caption = string.Format(InvariantCulture, "FSet{0:d2}{1}", option.Key, option.Value),
                                        DataType = typeof(string)
                                    });
                                }
                                if (!DtFreqSet02.Columns.Contains(string.Format(InvariantCulture, "intHits{0:d2}{1}", option.Key, option.Value)))
                                {
                                    using (DataColumn dcColumn = new DataColumn())
                                    {
                                        dcColumn.ColumnName = string.Format(InvariantCulture, "intHits{0:d2}{1}", option.Key, option.Value);
                                        dcColumn.Caption = string.Format(InvariantCulture, "Hits{0:d2}{1}", option.Key, option.Value);
                                        dcColumn.DataType = typeof(string);
                                        DtFreqSet02.Columns.Add(dcColumn);
                                    };
                                }
                                #endregion Columns of dtFreqSet
                                if (DtFreqSet02.Rows.Count == 0)
                                {
                                    foreach (DataRow drRow in dtTemp.Rows)
                                    {
                                        DataRow drFset = DtFreqSet02.NewRow();
                                        drFset["lngN"] = drRow["lngN"];
                                        drFset[string.Format(InvariantCulture, "strFreqSet{0:d2}{1}", option.Key, option.Value)] = drRow["strFreqSet"];
                                        drFset[string.Format(InvariantCulture, "intHits{0:d2}{1}", option.Key, option.Value)] = drRow["intHits"];
                                        DtFreqSet02.Rows.Add(drFset);
                                    }
                                }
                                else
                                {
                                    foreach (DataRow drRow in DtFreqSet02.Rows)
                                    {
                                        int intRowIndex = DtFreqSet02.Rows.IndexOf(drRow);
                                        drRow[string.Format(InvariantCulture, "strFreqSet{0:d2}{1}", option.Key, option.Value)] = dtTemp.Rows[intRowIndex]["strFreqSet"];
                                        drRow[string.Format(InvariantCulture, "intHits{0:d2}{1}", option.Key, option.Value)] = dtTemp.Rows[intRowIndex]["intHits"];
                                    }
                                }
                            }
                            ViewState.Add("dtFreqSet02", DtFreqSet02);
                        }
                    }
                    if (ViewState["dtPreDate"] == null)
                    {
                        using (DtPreDate = CreatPreDate())
                        {
                            List<long> lstTotalSN = new List<long>();
                            Dictionary<string, int> dicSumNumber = new Dictionary<string, int>();
                            for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; i++)
                            {
                                dicSumNumber.Add(string.Format(InvariantCulture, "lngN{0}", i), 0);
                            }
                            foreach (string strOption in CglClass.LstFieldName)
                            {
                                StuGLSearch stuSearchTemp = _gstuSearch;
                                #region Initial data
                                stuSearchTemp.InDataLimit = 0;
                                stuSearchTemp.InDataOffset = 0;
                                stuSearchTemp.InSearchLimit = 0;
                                stuSearchTemp.InSearchOffset = 0;
                                stuSearchTemp.InDisplayPeriod = 0;
                                stuSearchTemp.StrCompares = strOption;
                                stuSearchTemp.ShowProcess = ShowProcess.Visible;
                                stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
                                stuSearchTemp = new CglMethod().GetMethodSN(stuSearchTemp);
                                #endregion Initial data

                                Dictionary<string, object> dicFreqs = new CglFreq().GetFreqDic(stuSearchTemp, CglDBFreq.TableName.QryFreqProcess01, SortOrder.Descending);
                                Dictionary<string, int> dicFreq = ((Dictionary<string, Dictionary<string, int>>)dicFreqs["NoSortResult"])["gen"];
                                using DataTable dtProcess = ((Dictionary<string, DataTable>)dicFreqs["Process"])["gen"].Copy();
                                #region PreDate
                                DataRow drPreRow = DtPreDate.NewRow();
                                bool blRecord = false;
                                drPreRow["strField"] = new CglFunc().ConvertFieldNameId(strOption, 1);
                                drPreRow["intRows"] = dicFreq["intRows"];
                                drPreRow["lngTotalSN"] = dtProcess.Rows[0]["lngTotalSN"];
                                if (int.Parse(drPreRow["intRows"].ToString(), InvariantCulture) >= 140 && !lstTotalSN.Contains(long.Parse(drPreRow["lngTotalSN"].ToString(), InvariantCulture)))
                                {
                                    blRecord = true;
                                    lstTotalSN.Add(long.Parse(drPreRow["lngTotalSN"].ToString(), InvariantCulture));
                                }
                                drPreRow["lngDateSN"] = dtProcess.Rows[0]["lngDateSN"];
                                for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).CountNumber; i++)
                                {
                                    drPreRow[string.Format(InvariantCulture, "lngL{0}", i)] = dtProcess.Rows[0][string.Format(InvariantCulture, "lngL{0}", i)];
                                    if (blRecord)
                                    {
                                        string strN = string.Format(InvariantCulture, "lngN{0}", drPreRow[string.Format(InvariantCulture, "lngL{0}", i)].ToString());
                                        if (dicSumNumber.ContainsKey(strN))
                                        {
                                            dicSumNumber[strN]++;
                                        }
                                        else
                                        {
                                            dicSumNumber.Add(strN, 1);
                                        }
                                    }
                                }
                                drPreRow["intSum"] = dtProcess.Rows[0]["intSum"];
                                #endregion PreDate

                                #region Pre3
                                stuSearchTemp.InSearchLimit = 3;
                                stuSearchTemp.ShowProcess = ShowProcess.Hide;
                                Dictionary<string, int> dicFreq03 = ((Dictionary<string, Dictionary<string, int>>)new CglFreq().GetFreqDic(stuSearchTemp, CglDBFreq.TableName.QryFreqProcess01, SortOrder.Descending)["NoSortResult"])["gen"];
                                dicFreq03.Remove("intRows");
                                stuSearchTemp.LngTotalSN = long.Parse(dtProcess.Rows[0]["lngTotalSN"].ToString(), InvariantCulture);
                                List<int> lstCurrentTemp1 = (List<int>)new CglData().GetDataNumsLst(stuSearchTemp);
                                stuSearchTemp.LngTotalSN = long.Parse(dtProcess.Rows[1]["lngTotalSN"].ToString(), InvariantCulture);
                                List<int> lstCurrentTemp2 = (List<int>)new CglData().GetDataNumsLst(stuSearchTemp);
                                stuSearchTemp.LngTotalSN = long.Parse(dtProcess.Rows[2]["lngTotalSN"].ToString(), InvariantCulture);
                                List<int> lstCurrentTemp3 = (List<int>)new CglData().GetDataNumsLst(stuSearchTemp);
                                foreach (KeyValuePair<string, int> item in dicFreq03)
                                {
                                    if (item.Value == 2)
                                    {
                                        if (lstCurrentTemp1.Contains(int.Parse(item.Key.Substring(4), InvariantCulture)) && lstCurrentTemp2.Contains(int.Parse(item.Key.Substring(4), InvariantCulture)))
                                        {
                                            drPreRow["strPre12"] = drPreRow["strPre12"].ToString().Length > 0 ? string.Format(InvariantCulture, "{0}.{1:d2}", drPreRow["strPre12"], int.Parse(item.Key.Substring(4), InvariantCulture)) : string.Format(InvariantCulture, "{0:d2}", int.Parse(item.Key.Substring(4), InvariantCulture));
                                        }
                                        else if (lstCurrentTemp1.Contains(int.Parse(item.Key.Substring(4), InvariantCulture)) && lstCurrentTemp3.Contains(int.Parse(item.Key.Substring(4), InvariantCulture)))
                                        {
                                            drPreRow["strPre13"] = drPreRow["strPre13"].ToString().Length > 0 ? string.Format(InvariantCulture, "{0}.{1:d2}", drPreRow["strPre13"], int.Parse(item.Key.Substring(4), InvariantCulture)) : string.Format(InvariantCulture, "{0:d2}", int.Parse(item.Key.Substring(4), InvariantCulture));
                                        }
                                        else
                                        {
                                            drPreRow["strPre23"] = drPreRow["strPre23"].ToString().Length > 0 ? string.Format(InvariantCulture, "{0}.{1:d2}", drPreRow["strPre23"], int.Parse(item.Key.Substring(4), InvariantCulture)) : string.Format(InvariantCulture, "{0:d2}", int.Parse(item.Key.Substring(4), InvariantCulture));
                                        }
                                    }
                                    if (item.Value == 3)
                                    {
                                        drPreRow["strPre123"] = drPreRow["strPre123"].ToString().Length > 0 ? string.Format(InvariantCulture, "{0}.{1:d2}", drPreRow["strPre123"], int.Parse(item.Key.Substring(4), InvariantCulture)) : string.Format(InvariantCulture, "{0:d2}", int.Parse(item.Key.Substring(4), InvariantCulture));
                                    }
                                }
                                #endregion Pre3

                                DtPreDate.Rows.Add(drPreRow);

                            }

                            dicSumNumber = dicSumNumber.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                            DtPreDateSum = new CglFunc().CDicTOTable(dicSumNumber, null);

                            ViewState.Add("dtPreDate", DtPreDate);
                            ViewState.Add("dtPreDateSum", DtPreDateSum);
                        }
                    }
                    if (ViewState["_dicNumcssclass"] == null) { ViewState.Add("_dicNumcssclass", new GalaxyApp().GetNumcssclass(_gstuSearch)); }
                    if (ViewState["_lstCurrentNums"] == null) { ViewState.Add("_lstCurrentNums", (List<int>)new CglData().GetDataNumsLst(_gstuSearch)); }
                }
                //_dicCurrentNums = new CglData().GetDataNumsDici(_gstuSearch);
                _lstCurrentNums = (List<int>)ViewState["_lstCurrentNums"];
                _dicNumcssclass = (Dictionary<string, string>)ViewState["_dicNumcssclass"];
                ShowResult();
            }
            CurrentSearchOrderID = string.Empty;
        }


        private void ShowResult()
        {
            Page.Title = (string)ViewState["title"];
            lblTitle.Text = (string)ViewState["title"];
            lblMethod.Text = (string)ViewState["lblMethod"];
            lblSearchMethod.Text = (string)ViewState["lblSearchMethod"];

            //顯示當前資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)base.ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            UpdatePnlDetail(ddlOption.SelectedValue);
        }

        private void UpdatePnlDetail(string selectedValue)
        {
            switch (selectedValue)
            {
                case "FreqSet01":
                    pnlDetail.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lbl{0}", "FreqSet01"), string.Format(InvariantCulture, "{0}", "頻率組01"), "gllabel"));
                    #region gvFreqSet01
                    DtFreqSet01 = (DataTable)ViewState["dtFreqSet01"];
                    GridView gvFreqSet01 = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "{0}gvFreqSet01", "gen"), "gltable", DtFreqSet01, true, false);
                    //gvFreqSet01.AllowSorting = true;
                    gvFreqSet01.RowDataBound += GvFreqSet01_RowDataBound;
                    gvFreqSet01.DataBind();
                    pnlDetail.Controls.Add(gvFreqSet01);
                    #endregion gvFreqSet01
                    break;
                case "FreqSet02":
                    pnlDetail.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lbl{0}", "FreqSet02"), string.Format(InvariantCulture, "{0}", "頻率組02"), "gllabel"));
                    #region gvFreqSet02
                    DtFreqSet02 = (DataTable)ViewState["dtFreqSet02"];
                    GridView gvFreqSet02 = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "{0}gvFreqSet02", "gen"), "gltable", DtFreqSet02, true, false);
                    //gvFreqSet02.AllowSorting = true;
                    gvFreqSet02.RowDataBound += GvFreqSet02_RowDataBound;
                    gvFreqSet02.DataBind();
                    pnlDetail.Controls.Add(gvFreqSet02);
                    #endregion gvFreqSet02
                    break;
                case "PreDate":
                    pnlDetail.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lbl{0}", "PreDate"), string.Format(InvariantCulture, "{0}", "前期資料"), "gllabel"));

                    #region gvPreDateSum
                    DtPreDateSum = (DataTable)ViewState["dtPreDateSum"];
                    GridView gvPreDateSum = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "{0}gvPreDateSum", "gen"), "gltable", DtPreDateSum, false, false);
                    gvPreDateSum.AllowSorting = true;

                    #region Set Columns of DataGrid gvPreDateSum
                    if (gvPreDateSum.Columns.Count == 0)
                    {
                        for (int i = 0; i < DtPreDateSum.Columns.Count; i++)
                        {
                            string strColumnName = DtPreDateSum.Columns[i].ColumnName;
                            BoundField bfCell = new BoundField()
                            {
                                DataField = strColumnName,
                                HeaderText = new CglFunc().ConvertFieldNameId(strColumnName),
                            };
                            if (_dicNumcssclass.ContainsKey(strColumnName.Substring(4)))
                            {
                                bfCell.HeaderStyle.CssClass = _dicNumcssclass[strColumnName.Substring(4)];
                            }
                            gvPreDateSum.Columns.Add(bfCell);
                        }
                    }
                    #endregion

                    //gvPreDate.RowDataBound += GvPreDate_RowDataBound; ;
                    gvPreDateSum.DataBind();
                    pnlDetail.Controls.Add(gvPreDateSum);
                    #endregion gvPreDateSum

                    #region gvPreDate
                    DtPreDate = (DataTable)ViewState["dtPreDate"];
                    GridView gvPreDate = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "{0}gvPreDate", "gen"), "gltable", DtPreDate, true, false);
                    gvPreDate.AllowSorting = true;

                    // gvPreDate.RowDataBound += GvPreDate_RowDataBound; ;
                    gvPreDate.DataBind();
                    pnlDetail.Controls.Add(gvPreDate);
                    #endregion gvPreDate
                    break;
            }
        }

        private DataTable CreatPreDate()
        {
            using DataTable dtPreDateTemp = new DataTable { Locale = InvariantCulture };
            #region Columns
            if (dtPreDateTemp.Columns.Count == 0)
            {
                using (DataColumn dcColumn = new DataColumn())
                {
                    dcColumn.ColumnName = "strField";
                    dcColumn.Caption = new CglFunc().ConvertFieldNameId("strField", 1);
                    dcColumn.DataType = typeof(string);
                    dtPreDateTemp.Columns.Add(dcColumn);
                }
                using (DataColumn dcColumn = new DataColumn())
                {
                    dcColumn.ColumnName = "intRows";
                    dcColumn.Caption = new CglFunc().ConvertFieldNameId("intRows", 1);
                    dcColumn.DataType = typeof(int);
                    dtPreDateTemp.Columns.Add(dcColumn);
                }
                using (DataColumn dcColumn = new DataColumn())
                {
                    dcColumn.ColumnName = "lngTotalSN";
                    dcColumn.Caption = new CglFunc().ConvertFieldNameId("lngTotalSN", 1);
                    dcColumn.DataType = typeof(int);
                    dtPreDateTemp.Columns.Add(dcColumn);
                }
                using (DataColumn dcColumn = new DataColumn())
                {
                    dcColumn.ColumnName = "lngDateSN";
                    dcColumn.Caption = new CglFunc().ConvertFieldNameId("lngDateSN", 1);
                    dcColumn.DataType = typeof(int);
                    dtPreDateTemp.Columns.Add(dcColumn);
                }
                for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).CountNumber; i++)
                {
                    dtPreDateTemp.Columns.Add(new DataColumn
                    {
                        ColumnName = string.Format(InvariantCulture, "lngL{0}", i),
                        Caption = new CglFunc().ConvertFieldNameId(string.Format(InvariantCulture, "lngL{0}", i), 1),
                        DataType = typeof(int)
                    });
                }
                using (DataColumn dcColumn = new DataColumn())
                {
                    dcColumn.ColumnName = "intSum";
                    dcColumn.Caption = new CglFunc().ConvertFieldNameId("intSum", 1);
                    dcColumn.DataType = typeof(float);
                    dtPreDateTemp.Columns.Add(dcColumn);
                }
                using (DataColumn dcColumn = new DataColumn())
                {
                    dcColumn.ColumnName = "strPre12";
                    dcColumn.Caption = new CglFunc().ConvertFieldNameId("strPre12", 1);
                    dcColumn.DataType = typeof(string);
                    dtPreDateTemp.Columns.Add(dcColumn);
                }
                using (DataColumn dcColumn = new DataColumn())
                {
                    dcColumn.ColumnName = "strPre13";
                    dcColumn.Caption = new CglFunc().ConvertFieldNameId("strPre13", 1);
                    dcColumn.DataType = typeof(string);
                    dtPreDateTemp.Columns.Add(dcColumn);
                }
                using (DataColumn dcColumn = new DataColumn())
                {
                    dcColumn.ColumnName = "strPre23";
                    dcColumn.Caption = new CglFunc().ConvertFieldNameId("strPre23", 1);
                    dcColumn.DataType = typeof(string);
                    dtPreDateTemp.Columns.Add(dcColumn);
                }
                using (DataColumn dcColumn = new DataColumn())
                {
                    dcColumn.ColumnName = "strPre123";
                    dcColumn.Caption = new CglFunc().ConvertFieldNameId("strPre123", 1);
                    dcColumn.DataType = typeof(string);
                    dtPreDateTemp.Columns.Add(dcColumn);
                }
            }
            #endregion Columns
            return dtPreDateTemp;
        }

        private void GvFreqSet01_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Dictionary<string, int> dicHitLimit = new Dictionary<string, int>() { { "0520", 5 }, { "1040", 4 }, { "25100", 3 }, { "50200", 1 } };
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        if (strCell_ColumnName == "lngN")
                        {
                            if (_lstCurrentNums.Contains(int.Parse(cell.Text, InvariantCulture)))
                            {
                                cell.CssClass = _dicNumcssclass[int.Parse(cell.Text, InvariantCulture).ToString(InvariantCulture)];
                                e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit";
                            }
                        }
                        if (strCell_ColumnName.Length > 7 && strCell_ColumnName.Substring(0, 7) == "intHits")
                        {
                            if (int.Parse(cell.Text, InvariantCulture) > dicHitLimit[strCell_ColumnName.Substring(7)])
                            {
                                cell.CssClass = " glValueActive glValueMaxNum";
                            }
                        }
                    }
                }
            }
        }

        private void GvFreqSet02_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Dictionary<string, int> dicHitLimit = new Dictionary<string, int>() { { "0520", 5 }, { "1040", 4 }, { "25100", 2 }, { "50200", 1 } };
            //GridView gridView = (GridView)sender;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        if (strCell_ColumnName == "lngN")
                        {
                            if (_lstCurrentNums.Contains(int.Parse(cell.Text, InvariantCulture)))
                            {
                                cell.CssClass = _dicNumcssclass[int.Parse(cell.Text, InvariantCulture).ToString(InvariantCulture)];
                                e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit";
                            }
                        }
                        if (strCell_ColumnName.Length > 7 && strCell_ColumnName.Substring(0, 7) == "intHits")
                        {
                            if (int.Parse(cell.Text, InvariantCulture) > dicHitLimit[strCell_ColumnName.Substring(7)])
                            {
                                cell.CssClass = " glValueActive glValueMaxNum";
                            }
                        }
                    }
                }
            }
        }

        protected void DdlOptionSelectedIndexChanged(object sender, EventArgs e)
        {
            pnlDetail.Controls.Clear();
            UpdatePnlDetail(ddlOption.SelectedValue);
        }

        protected void IBInfoClick(object sender, ImageClickEventArgs e)
        {
            pnlTopData.Visible = !pnlTopData.Visible;
        }
    }
}
