using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class DataN : BasePage
    {
        private StuGLSearch GlobalStuSearch;
        private string PrivateAction;
        private string PrivateRequestId;
        private string DataNID;

        //Tread part
        private static Dictionary<string, object> DicThreadDataN
        {
            get { if (dicThreadDataN == null) { dicThreadDataN = new Dictionary<string, object>(); }; return dicThreadDataN; }
            set => dicThreadDataN = value;
        }
        private static Dictionary<string, object> dicThreadDataN;

        private Thread Thread01;

        // ---------------------------------------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            SetupViewState();
            if (ViewState[DataNID + "GlobalStuSearch"] == null || GlobalStuSearch == null)
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

            DataNID = PrivateAction + PrivateRequestId;
            if (ViewState[DataNID + "GlobalStuSearch"] == null)
            {
                if (!string.IsNullOrEmpty(PrivateAction) && !string.IsNullOrEmpty(PrivateRequestId) && Session[DataNID] != null)
                {
                    ViewState.Add(DataNID + "GlobalStuSearch", (StuGLSearch)Session[DataNID]);
                }
            }
            GlobalStuSearch = (StuGLSearch)ViewState[DataNID + "GlobalStuSearch"];
        }

        private void InitializeArgument()
        {
            if (Session[DataNID + "ListCurrentNumsN"] == null) { Session.Add(DataNID + "ListCurrentNumsN", (List<int>)new CglData().GetDataNumsLst(GlobalStuSearch)); }
            if (Session[DataNID + "lblMethod"] == null) { Session.Add(DataNID + "lblMethod", new CglMethod().SetMethodString(GlobalStuSearch)); }
            if (Session[DataNID + "lblSearchMethod"] == null) { Session.Add(DataNID + "lblSearchMethod", new CglMethod().SetSearchMethodString(GlobalStuSearch)); }
            if (Session[DataNID + "CurrentData"] == null) { Session.Add(DataNID + "CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(GlobalStuSearch))); }
            if (Session[DataNID + "DicNumCssClass"] == null) { Session.Add(DataNID + "DicNumCssClass", new GalaxyApp().GetNumcssclass(GlobalStuSearch)); }
            if (Session[DataNID + "dsDataN"] == null) { Session[DataNID + "dsDataN"] = new DataSet(); }
            //if (Session[DataNID + "dicDataN"] == null && Session[DataNID + "dicDataN"] != null) { Session.Add(DataNID + "dicDataN", Session[DataNID + "dicDataN"]); }
        }

        private void SetddlNums()
        {
            if (ddlNums.Items.Count == 0)
            {
                for (int intLNums = 1; intLNums <= ((List<int>)Session[DataNID + "ListCurrentNumsN"]).Count; intLNums++)
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
            //HttpCookie yoCookie = new HttpCookie("test");
            //Response.Cookies["test"]["username"];
            //Cache["yoyo"] = "1000";
            ShowTitle();
            SetddlNums();
            ShowddlFreq();
            ddlPercent.Visible = false;
            ddlDisplay.Visible = false;
            ddlDays.Visible = false;
            ddlNump.Visible = false;
            ckAns.Visible = false;
            switch (ddlFuntion.SelectedValue)
            {
                case "DataN":
                    if (Session[DataNID + "DataNTitle"] == null) { Session.Add(DataNID + "DataNTitle", string.Format(InvariantCulture, "{0}:{1}", "振盪表", new CglData().SetTitleString(GlobalStuSearch))); }
                    Page.Title = (string)Session[DataNID + "DataNTitle"];
                    lblTitle.Text = Page.Title;
                    ddlDays.Visible = true;
                    ShowDataN(ddlFreq.SelectedValue, ddlNums.SelectedValue);
                    break;
                case "DataNNext":
                    if (Session[DataNID + "DataNNextTitle"] == null) { Session.Add(DataNID + "DataNNextTitle", string.Format(InvariantCulture, "{0}:{1}", "振盪預測表", new CglData().SetTitleString(GlobalStuSearch))); }
                    Page.Title = (string)Session[DataNID + "DataNNextTitle"];
                    lblTitle.Text = Page.Title;
                    ckAns.Visible = true;
                    ddlPercent.Visible = true;
                    ddlDays.Visible = true;
                    ddlNump.Visible = true;
                    ShowDataNNext(ddlFreq.SelectedValue, ddlNums.SelectedValue);
                    break;
            }
        }

        private void ShowTitle()
        {

            lblMethod.Text = (string)Session[DataNID + "lblMethod"];

            lblSearchMethod.Text = (string)Session[DataNID + "lblSearchMethod"];

            //顯示當前資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)Session[DataNID + "CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
        }

        private void ShowddlFreq()
        {
            if (Session[DataNID + "dicDataN"] != null)
            {
                if (ddlFreq.Items.Count == 0)
                {
                    Dictionary<string, object> DicDataN = (Dictionary<string, object>)Session[DataNID + "dicDataN"];
                    foreach (KeyValuePair<string, DataSet> keyval in (Dictionary<string, DataSet>)DicDataN["DataN"])
                    {
                        ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(keyval.Key), keyval.Key));
                    }
                }
            }
            else
            {
                if (!DicThreadDataN.Keys.Contains(DataNID + "T01")) { CreatThread(); }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowDataN(string ddlFreqSelectedValue, string ddlNumsSelectedValue)
        {
            if (Session[DataNID + "dicDataN"] != null)
            {
                pnlDetail.Controls.Clear();
                using DataTable _dtEachNumN = GetDataTable("dtEachNumN");
                Panel pnlDataN = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlDataN{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue), "max-width");
                pnlDetail.Controls.Add(pnlDataN);

                Label lblDataN = new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblDataN{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue),
                                            string.Format(InvariantCulture, "{0}#{1} ({2}期)", ddlFreqSelectedValue, ddlNumsSelectedValue, _dtEachNumN.Rows.Count),
                                            "gllabel");
                pnlDataN.Controls.Add(lblDataN);

                #region gvDataN 
                _dtEachNumN.DefaultView.Sort = "lngTotalSN DESC";
                GridView gvDataN = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "gvDataN{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue),
                                                 "gltable",
                                                 _dtEachNumN, true, false);
                #region Set Css
                foreach (DataControlField dcColumn in gvDataN.Columns)
                {
                    string strColumnName = dcColumn.SortExpression.ToString(InvariantCulture);
                    if ((strColumnName.Substring(0, 4) != "lngN" && strColumnName.Substring(0, 4) != "lngM") || strColumnName == "lngMethodSN")
                    {
                        dcColumn.ItemStyle.CssClass = strColumnName;
                    }
                    if (strColumnName.Contains("AKD"))
                    {
                        dcColumn.Visible = false;
                    }
                }
                #endregion
                gvDataN.RowDataBound += GvDataN_RowDataBound;
                gvDataN.DataBind();
                #endregion gvDataN
                pnlDataN.Controls.Add(gvDataN);
            }
        }

        private void GvDataN_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField.ToString(InvariantCulture);

                        #region Set lngN
                        if (strCell_ColumnName.Substring(startIndex: 0, length: 4) == "lngN" && strCell_ColumnName.Substring(startIndex: 0, length: 4) != "lngMethodSN")
                        {
                            cell.CssClass = strCell_ColumnName;
                        }
                        #endregion Set lngN

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

        private void ShowDataNNext(string ddlFreqSelectedValue, string ddlNumsSelectedValue)
        {
            if (Session[DataNID + "dicDataN"] != null)
            {
                ResetSearchOrder(DataNID);
                pnlDetail.Controls.Clear();
                //Dictionary<string, object> DicDataN = (Dictionary<string, object>)Session[DataNID + "dicDataN"];
                Panel pnlDataN = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlDataN{0}", ddlFreqSelectedValue), "max-width");
                pnlDetail.Controls.Add(pnlDataN);

                #region showNext
                Panel pnlDataNNext = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlDataNext{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue), "max-width");
                pnlDetail.Controls.Add(pnlDataNNext);

                pnlDataNNext.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblDataNext{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue),
                                                     string.Format(InvariantCulture, "{0}=>{1}下期預測資料({2}>{3})", ddlFreqSelectedValue, ddlNumsSelectedValue,
                                                                   GetDataTable("dtEachNumN").Rows[1][string.Format(InvariantCulture, "lngN{0}", ddlNumsSelectedValue)],
                                                                   GetDataTable("dtEachNumN").Rows[0][string.Format(InvariantCulture, "lngN{0}", ddlNumsSelectedValue)]),
                                                     "gllabel"));
                using DataTable _dtEachNumNNext = GetDataTable("dtEachNumNNext");

                #region gvDataNext 
                GridView gvDataNNext = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "gvDataNNext{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue),
                                                    "gltableWhite",
                                                    _dtEachNumNNext, true, false);
                gvDataNNext.AllowSorting = false;
                gvDataNNext.DataKeyNames = new string[] { string.Format(InvariantCulture, "lngN{0}", ddlNumsSelectedValue) };
                #region Set Css
                foreach (DataControlField dcColumn in gvDataNNext.Columns)
                {
                    string strColumnName = dcColumn.SortExpression.ToString(InvariantCulture);
                    if (strColumnName.Contains("lngN"))
                    {
                        dcColumn.ItemStyle.CssClass = strColumnName;
                    }
                    if (strColumnName.Contains("AKD"))
                    {
                        dcColumn.Visible = false;
                    }
                }
                #endregion

                gvDataNNext.RowDataBound += GvDataNNextRowDataBound;
                gvDataNNext.DataBind();
                #endregion gvDataNext

                pnlDataNNext.Controls.Add(gvDataNNext);

                #endregion showNext
            }
        }

        private void GvDataNNextRowDataBound(object sender, GridViewRowEventArgs e)
        {
            int Nums = int.Parse(ddlNums.SelectedValue, InvariantCulture);
            string strCuNum = "1";
            using DataTable dtEachNumNNext = GetDataTable("dtEachNumNNext");
            using DataTable dtEachNumN = GetDataTable("dtEachNumN");
            DataRow drEachNumRange = GetDataTable("dtEachNumNRange").Rows[0];
            DataRow drEachNumNKD = GetDataTable("dtEachNumNKD").Rows[0];
            DataRow drEachNumNKDRange = GetDataTable("dtEachNumNKDRange").Rows[0];
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
                            if (double.Parse(cell.Text, InvariantCulture) <= new GalaxyApp().GetASP(dtEachNumN.Rows[0], Nums.ToString(InvariantCulture)))
                            {
                                cell.CssClass += " glValueLessG ";
                            }
                            if (double.Parse(cell.Text, InvariantCulture) == double.Parse(dtEachNumNNext.Compute(string.Format(InvariantCulture, "Min([{0}])", strCell_ColumnName), string.Empty).ToString(),
                                                                                          InvariantCulture)) { cell.Text += "※"; }
                        }
                        //ASP1
                        if (strCell_ColumnName == string.Format(InvariantCulture, "ASP1{0}", Nums))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), 0d, new GalaxyApp().GetASP1(dtEachNumN.Rows[0], Nums.ToString(InvariantCulture), dtEachNumN.Rows[0]), true))
                            {
                                cell.CssClass += " glValueLessG ";
                            }
                            if (double.Parse(cell.Text, InvariantCulture) == new GalaxyApp().GetMinAbs(dtEachNumNNext, strCell_ColumnName)) { cell.Text += "※"; }
                        }

                        //lngN
                        if (strCell_ColumnName.Contains("lngN"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            strCuNum = cell.Text;
                            if (ckAns.Checked && ((List<int>)Session[DataNID + "ListCurrentNumsN"])[int.Parse(strCell_ColumnName.Substring(4), InvariantCulture) - 1] == int.Parse(cell.Text, InvariantCulture))
                            {
                                cell.CssClass = ((Dictionary<string, string>)Session[DataNID + "DicNumCssClass"])[cell.Text];
                                e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit";
                            }
                            if (cell.Text == dtEachNumN.Rows[0][strCell_ColumnName].ToString()) { e.Row.CssClass = e.Row.CssClass + " glRowLast "; }
                            if (cell.Text == dtEachNumN.Rows[1][strCell_ColumnName].ToString()) { e.Row.CssClass = e.Row.CssClass + " glRowLastOne "; }
                            if (cell.Text == ddlNump.SelectedValue) { e.Row.CssClass = e.Row.CssClass + " glRowLastNext "; }

                        }
                        //sglAvg
                        if (strCell_ColumnName.Contains("sglAvg"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(dtEachNumN.Rows[0][strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glColNumInBlue "; }
                            dblMax = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}GapMax", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}GapMin", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNum "; }
                        }
                        //sglStdEvp
                        if (strCell_ColumnName.Contains("sglStdEvp"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(dtEachNumN.Rows[0][columnName: strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLessG "; }
                            dblMax = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glColNumInGreen "; }
                            dblMax = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}GapMax", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}GapMin", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNum "; }
                            if (double.Parse(cell.Text, InvariantCulture) == double.Parse(dtEachNumNNext.Compute(string.Format(InvariantCulture, "Min([{0}])", strCell_ColumnName), string.Empty).ToString(), InvariantCulture)) { cell.Text += "※"; }
                        }
                        //RSV
                        if (strCell_ColumnName.Contains("RSV"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumNKD[strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            //if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glColNumInBlue "; }
                            //dblMax = double.Parse(drEachNumKDRange[string.Format(InvariantCulture, "{0}GapMax", strCell_ColumnName)].ToString(), InvariantCulture);
                            //dblMin = double.Parse(drEachNumKDRange[string.Format(InvariantCulture, "{0}GapMin", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNumRed "; }
                        }
                        //K
                        if (strCell_ColumnName.Contains("K") && !strCell_ColumnName.Contains("K-D") && !strCell_ColumnName.Contains("AKD"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumNKD[strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNumGreen "; }
                        }
                        //D
                        if (strCell_ColumnName.Contains("D") && !strCell_ColumnName.Contains("K-D") && !strCell_ColumnName.Contains("AKD"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumNKD[strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNumBlue "; }
                        }
                        //K-D
                        if (strCell_ColumnName.Contains("K-D"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (Math.Abs(double.Parse(cell.Text, InvariantCulture)) <= Math.Abs(double.Parse(drEachNumNKD[strCell_ColumnName].ToString(), InvariantCulture))) { cell.CssClass += " glColNumInPink "; }
                            if (!(double.Parse(cell.Text, InvariantCulture) > 0 ^ double.Parse(drEachNumNKD[strCell_ColumnName].ToString(), InvariantCulture) > 0))
                            {
                                if (Math.Abs(double.Parse(cell.Text, InvariantCulture)) <= Math.Abs(double.Parse(drEachNumNKD[strCell_ColumnName].ToString(), InvariantCulture))) { cell.CssClass += " glValueLess "; }
                            }
                            dblMax = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
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
                case "dtEachNumNNext":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)Session[DataNID + "dsDataN"]).Tables.Contains(strTableName))
                    {

                        ((DataSet)Session[DataNID + "dsDataN"]).Tables.Add(GetEachNextDataN(strTableName));
                    }//dtEachNumNNext
                    break;
                case "dtEachNumNGap":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue);
                    if (!((DataSet)Session[DataNID + "dsDataN"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)Session[DataNID + "dsDataN"]).Tables.Add(new GalaxyApp().GetEachDataGap(((Dictionary<string, DataSet>)((Dictionary<string, object>)Session[DataNID + "dicDataN"])["DataN"])[ddlFreq.SelectedValue].Tables["Gap"],
                                                                                            strTableName, ddlNums.SelectedValue));
                    }//dtEachNumNGap
                    break;
                case "dtEachNumNRange":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlPercent.SelectedValue);
                    if (!((DataSet)Session[DataNID + "dsDataN"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)Session[DataNID + "dsDataN"]).Tables.Add(new GalaxyApp().GetDataRange(GetDataTable("dtEachNumNGap"),
                                                                                          strTableName,
                                                                                          ddlNums.SelectedValue,
                                                                                          ddlPercent.SelectedValue));
                    }//dtEachNumNRange
                    break;
                case "dtEachNumNKD":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)Session[DataNID + "dsDataN"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)Session[DataNID + "dsDataN"]).Tables.Add(new GalaxyApp().GetDataKDCoeficient(GetDataTable("dtEachNumN"),
                                                                                                strTableName,
                                                                                                ddlNums.SelectedValue,
                                                                                                int.Parse(ddlDays.SelectedValue, InvariantCulture),
                                                                                                3,
                                                                                                1d / 3d));
                    }//dtEachNumNKD
                    break;
                case "dtEachNumNKDRange":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)Session[DataNID + "dsDataN"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)Session[DataNID + "dsDataN"]).Tables.Add(new GalaxyApp().GetDataKDRange(GetDataTable("dtEachNumNKD"),
                                                                                           strTableName,
                                                                                           ddlPercent.SelectedValue));
                    }//dtEachNumNKDRange
                    break;
                case "dtEachNumN":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlFreq.SelectedValue, ddlNums.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)Session[DataNID + "dsDataN"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)Session[DataNID + "dsDataN"]).Tables.Add(GetEachDataN(strTableName));
                    }//dtEachNumN
                    break;
            }
            return ((DataSet)Session[DataNID + "dsDataN"]).Tables[strTableName];
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

            using DataView dvDataN = new DataView(((Dictionary<string, DataSet>)((Dictionary<string, object>)Session[DataNID + "dicDataN"])["DataN"])[ddlFreq.SelectedValue].Tables[ddlFreq.SelectedValue]);
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
            using DataView dvDataNext = new DataView(((Dictionary<string, DataSet>)((Dictionary<string, object>)Session[DataNID + "dicDataN"])["DataN"])[ddlFreq.SelectedValue].Tables["Next"]);
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
            Session.Remove(DataNID);
            Session.Remove(DataNID + "dicDataN");
            Session.Remove(DataNID + "lblT01");
            ResetSearchOrder(DataNID);
            if (DicThreadDataN.Keys.Contains(DataNID + "T01"))
            {
                Thread ThreadFreqActive01 = (Thread)DicThreadDataN[DataNID + "T01"];
                ThreadFreqActive01.Abort();
                DicThreadDataN.Remove(DataNID + "T01");
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            if (DicThreadDataN != null && DicThreadDataN.Keys.Contains(DataNID + "T01"))
            {
                Thread ThreadFreqActive01 = (Thread)DicThreadDataN[DataNID + "T01"];
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
            if (DicThreadDataN.Keys.Contains(DataNID + "T01"))
            {
                Thread01 = (Thread)DicThreadDataN[DataNID + "T01"];
                if (Thread01.IsAlive)
                {
                    Timer1.Enabled = true;
                    Timer1.Interval = 10000;
                    lblArgument.Text = string.Format(InvariantCulture, "{0} ", DateTime.Now.ToLongTimeString());
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0}{1} ", new GalaxyApp().GetTheadState(Thread01.ThreadState), Session[DataNID + "lblT01"].ToString());
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
            Thread01 = new Thread(() => { StartThread01(); }) { Name = DataNID + "01" };
            Thread01.Start();
            //SetDicThreadDataN(GetDicThreadDataN());
            DicThreadDataN.Add(DataNID + "T01", Thread01);
        }

        private void StartThread01()
        {
            //new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
            Session[DataNID + "lblT01"] = "Get DataN ...";
            if (Session[DataNID + "dicDataN"] == null)
            {
                Session.Add(DataNID + "dicDataN", new CglDataN00().GetDataN00Dic(GlobalStuSearch, CglDataN.TableName.QryDataN00, SortOrder.Descending));
                //dsDataN00.Tables.Add(GetDataN00GapMultiple(stuSearchTemp, TableName.QryDataN00Gap, SortOrder.Descending));
                //dsDataN00.Tables[1].TableName = "Gap";
            }
            //new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
        }

        // ---------------------------------------------------------------------------------------------------------
    }
}