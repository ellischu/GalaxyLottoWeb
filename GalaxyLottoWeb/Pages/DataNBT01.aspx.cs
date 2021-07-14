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
using System.Xml.Linq;

namespace GalaxyLottoWeb.Pages
{
    public partial class DataNBT01 : BasePage
    {
        private string DataNT01ID;

        private StuGLSearch GlobalStuSearch;

        //private DataSet DsData { get; set; }

        private static Dictionary<string, object> DicThreadDataNB
        {
            get { if (dicThreadDataNB == null) { dicThreadDataNB = new Dictionary<string, object>(); } return dicThreadDataNB; }
            set => dicThreadDataNB = value;
        }

        private static Dictionary<string, object> dicThreadDataNB;

        private Thread Thread01;

        private static Dictionary<int, Color> DicSectionColor => new Dictionary<int, Color>() { { 5, Color.Brown }, { 10, Color.DeepPink }, { 25, Color.Green }, { 50, Color.Purple }, { 100, Color.Blue } };
        // ---------------------------------------------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            if (string.IsNullOrEmpty(DataNT01ID)) { DataNT01ID = GetID(); }
            SetupViewState(DataNT01ID);
            if (ViewState[DataNT01ID + "_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                GlobalStuSearch = (StuGLSearch)ViewState[DataNT01ID + "_gstuSearch"];
                InitializeArgument();
                ShowResult();
            }
            //Search.CurrentSearchOrderID = string.Empty;
        }

        private string GetID()
        {
            string _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            string _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null && !string.IsNullOrEmpty(_action)) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null && !string.IsNullOrEmpty(_requestId)) { ViewState.Add("id", _requestId); }

            return !string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) ? _action + _requestId : string.Empty;
        }

        private void InitializeArgument()
        {
            if (ViewState[DataNT01ID + "DicNumCssClass"] == null) { ViewState.Add(DataNT01ID + "DicNumCssClass", new GalaxyApp().GetNumcssclass(GlobalStuSearch)); }
            if (ViewState[DataNT01ID + "ListCurrentNumsN"] == null) { ViewState.Add(DataNT01ID + "ListCurrentNumsN", (List<int>)new CglData().GetDataNumsLst(GlobalStuSearch)); }
            if (ViewState[DataNT01ID + "ListCurrentNumsB"] == null) { ViewState.Add(DataNT01ID + "ListCurrentNumsB", (List<int>)new CglDataB().GetDataBNumsLst(GlobalStuSearch)); }
            if (Session[DataNT01ID + "lblT01"] == null) { Session[DataNT01ID + "lblT01"] = string.Empty; }
            if (ViewState[DataNT01ID + "dsDataNB"] == null) { ViewState[DataNT01ID + "dsDataNB"] = new DataSet(); }
            if (Session[DataNT01ID + "dicDataN"] != null) { ViewState[DataNT01ID + "dicDataN"] = Session[DataNT01ID + "dicDataN"]; }
            if (Session[DataNT01ID + "dicDataB"] != null) { ViewState[DataNT01ID + "dicDataB"] = Session[DataNT01ID + "dicDataB"]; }
        }

        private void SetupViewState(string DataID)
        {
            if (!string.IsNullOrEmpty(DataID) && ViewState[DataID + "_gstuSearch"] == null && Session[DataID] != null)
            {
                ViewState.Add(DataID + "_gstuSearch", (StuGLSearch)Session[DataID]);
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowResult()
        {
            ShowTitle();
            ShowddlNums();
            ShowddlFields();
            ddlFields.Visible = false;
            ddlDataN.Visible = false;
            ddlDataB.Visible = false;
            ddlNums.Visible = false;
            ddlPercent.Visible = false;
            ddlDays.Visible = false;
            switch (ddlOption.SelectedValue)
            {
                case "OptionDataN":
                    ddlFields.Visible = true;
                    ddlDataN.Visible = true;
                    ddlNums.Visible = true;
                    ShowddlFreqN();
                    ShowOptionDataN();
                    break;
                case "OptionDataB":
                    ddlFields.Visible = true;
                    ddlDataB.Visible = true;
                    ddlNums.Visible = true;
                    ShowddlFreqB();
                    ShowOptionDataB();
                    break;
                case "OptionSum":
                    ShowDataSum();
                    break;
                case "OptionSum01":
                    ShowDataSum01();
                    break;
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowDataSum()
        {
            if (Session[DataNT01ID + "dsDataNSum"] != null && Session[DataNT01ID + "dsDataBSum"] != null)
            {
                ResetSearchOrder(DataNT01ID);

                #region DataNSum
                Panel pnlDataNSum = new GalaxyApp().CreatPanel("pnlDataNSum", "max-width");
                pnlDetail.Controls.Add(pnlDataNSum);

                //Label
                Label lblNext = new GalaxyApp().CreatLabel("lblNext", "振盪");
                pnlDataNSum.Controls.Add(lblNext);
                //Datatable
                GridView gvDataNSum = new GalaxyApp().CreatGridView("gvDataNSum", "gltable ",
                                                    ((DataSet)Session[DataNT01ID + "dsDataNSum"]).Tables["dtDataNSum"], false, false);
                if (gvDataNSum.Columns.Count == 0)
                {
                    List<string> lstColumnName = new List<string>() { "LNum", "NMin", "SMin" };
                    BoundField bfCell = new BoundField()
                    {
                        DataField = "Field",
                        HeaderText = new CglFunc().ConvertFieldNameId("Field", 1),
                        SortExpression = "Field"
                    };
                    bfCell.ItemStyle.CssClass = "Field";
                    gvDataNSum.Columns.Add(bfCell);

                    for (int num = 1; num <= new CglDataSet(GlobalStuSearch.LottoType).CountNumber; num++)
                    {
                        foreach (string item in lstColumnName)
                        {
                            string ColumnName = string.Format(InvariantCulture, "{0}{1}", item, num);
                            BoundField bfCell01 = new BoundField()
                            {
                                DataField = ColumnName,
                                HeaderText = new CglFunc().ConvertFieldNameId(ColumnName, 1),
                                SortExpression = ColumnName
                            };
                            bfCell01.ItemStyle.CssClass = ColumnName;
                            gvDataNSum.Columns.Add(bfCell01);
                        }
                    }
                }
                gvDataNSum.ShowHeader = false;
                gvDataNSum.RowDataBound += GvDataNSum_RowDataBound;
                gvDataNSum.DataBind();
                pnlDataNSum.Controls.Add(gvDataNSum);

                GridView gvDataNRange = new GalaxyApp().CreatGridView("gvDataNRange", "gltable ",
                                                    ((DataSet)Session[DataNT01ID + "dsDataNSum"]).Tables["dtDataNRange"], true, true);
                gvDataNRange.ShowHeader = false;
                gvDataNRange.GridLines = GridLines.Both;
                gvDataNRange.RowDataBound += GvDataRange_RowDataBound;
                gvDataNRange.DataBind();
                pnlDataNSum.Controls.Add(gvDataNRange);

                //振盪統計
                Label lblNextCount = new GalaxyApp().CreatLabel("lblNextCount", "振盪統計");
                //lblNextCount.CssClass = "NotPrint";
                pnlDataNSum.Controls.Add(lblNextCount);

                GridView gvNextCount = new GalaxyApp().CreatGridView("gvNextCount", "gltable",
                                    ((DataSet)Session[DataNT01ID + "dsDataNSum"]).Tables["dtNextCount"], true, true);
                foreach (DataControlField dcColumn in gvNextCount.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)))
                    {
                        dcColumn.HeaderStyle.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)];
                    }
                }
                gvNextCount.DataBind();
                pnlDataNSum.Controls.Add(gvNextCount);

                #endregion DataNSum

                #region DataBSum
                Panel pnlDataBSum = new GalaxyApp().CreatPanel("pnlDataBSum", "max-width");
                pnlDetail.Controls.Add(pnlDataBSum);
                //Label
                Label lblBalance = new GalaxyApp().CreatLabel("lblBalance", "平衡");
                pnlDataBSum.Controls.Add(lblBalance);
                //Datatable
                GridView gvDataBSum = new GalaxyApp().CreatGridView("gvDataBSum", "gltable ",
                                                    ((DataSet)Session[DataNT01ID + "dsDataBSum"]).Tables["dtDataBSum"], false, false);
                if (gvDataBSum.Columns.Count == 0)
                {
                    List<string> lstColumnName = new List<string>() { "LNum", "NMin", "SMin" };
                    BoundField bfCell = new BoundField()
                    {
                        DataField = "Field",
                        HeaderText = new CglFunc().ConvertFieldNameId("Field", 1),
                        SortExpression = "Field"
                    };
                    bfCell.ItemStyle.CssClass = "Field";
                    gvDataBSum.Columns.Add(bfCell);

                    for (int num = 1; num <= new CglDataSet(GlobalStuSearch.LottoType).CountNumber; num++)
                    {
                        foreach (string item in lstColumnName)
                        {
                            string ColumnName = string.Format(InvariantCulture, "{0}{1}", item, num);
                            BoundField bfCell01 = new BoundField()
                            {
                                DataField = ColumnName,
                                HeaderText = new CglFunc().ConvertFieldNameId(ColumnName, 1),
                                SortExpression = ColumnName
                            };
                            bfCell01.ItemStyle.CssClass = ColumnName;
                            gvDataBSum.Columns.Add(bfCell01);
                        }
                    }
                }
                gvDataBSum.ShowHeader = false;
                gvDataBSum.RowDataBound += GvDataBSum_RowDataBound; ;
                gvDataBSum.DataBind();
                pnlDataBSum.Controls.Add(gvDataBSum);

                GridView gvDataBRange = new GalaxyApp().CreatGridView("gvDataBRange", "gltable ",
                                                    ((DataSet)Session[DataNT01ID + "dsDataBSum"]).Tables["dtDataBRange"], true, true);
                gvDataBRange.ShowHeader = false;
                gvDataBRange.GridLines = GridLines.Both;
                gvDataBRange.RowDataBound += GvDataRange_RowDataBound;
                gvDataBRange.DataBind();
                pnlDataBSum.Controls.Add(gvDataBRange);

                //平衡統計
                Label lblBalanceCount = new GalaxyApp().CreatLabel("lblBalanceCount", "平衡統計");
                //lblBalanceCount.CssClass = "NotPrint";
                pnlDataBSum.Controls.Add(lblBalanceCount);

                GridView gvBalanceCount = new GalaxyApp().CreatGridView("gvBalanceCount", "gltable",
                                    ((DataSet)Session[DataNT01ID + "dsDataBSum"]).Tables["dtBalanceCount"], true, true);
                foreach (DataControlField dcColumn in gvBalanceCount.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)))
                    {
                        dcColumn.HeaderStyle.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)];
                    }
                }
                gvBalanceCount.DataBind();
                pnlDataBSum.Controls.Add(gvBalanceCount);

                #endregion DataBSum

                #region Compare
                Panel pnlCompare = new GalaxyApp().CreatPanel("pnlCompare", "max-width");
                pnlDetail.Controls.Add(pnlCompare);

                //檢查數柱
                Label lblCheckColumn = new GalaxyApp().CreatLabel("lblCheckColumn", "檢查數柱");
                pnlCompare.Controls.Add(lblCheckColumn);

                GridView gvCheckColumn = new GalaxyApp().CreatGridView("gvCheckColumn", "gltable ",
                    ((DataSet)Session[DataNT01ID + "dsDataNSum"]).Tables["dtCheckColumn"], true, true);
                gvCheckColumn.GridLines = GridLines.Both;
                gvCheckColumn.ShowHeader = false;
                gvCheckColumn.RowDataBound += GvCheckColumn_RowDataBound;
                gvCheckColumn.DataBind();
                pnlCompare.Controls.Add(gvCheckColumn);

                //比較
                Label lblCompare = new GalaxyApp().CreatLabel("lblCompare", "比較");
                pnlCompare.Controls.Add(lblCompare);

                GridView gvCompare = new GalaxyApp().CreatGridView("gvCompare", "gltable ",
                    ((DataSet)Session[DataNT01ID + "dsDataNSum"]).Tables["dtCompare"], true, true);
                gvCompare.GridLines = GridLines.Both;
                gvCompare.ShowHeader = false;
                gvCompare.RowDataBound += GvCompare_RowDataBound;
                gvCompare.DataBind();
                pnlCompare.Controls.Add(gvCompare);

                //比較統計
                Label lblCompareCount = new GalaxyApp().CreatLabel("lblCompareCount", "比較統計");
                //lblCompareCount.CssClass = "NotPrint";
                pnlCompare.Controls.Add(lblCompareCount);

                GridView gvCompareCount = new GalaxyApp().CreatGridView("gvCompareCount", "gltable",
                    ((DataSet)Session[DataNT01ID + "dsDataNSum"]).Tables["dtCompareCount"], true, true);
                foreach (DataControlField dcColumn in gvCompareCount.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)))
                    {
                        dcColumn.HeaderStyle.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)];
                    }
                }
                gvCompareCount.DataBind();
                pnlCompare.Controls.Add(gvCompareCount);
                #endregion Compare
            }
        }

        private void GvCompare_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField)
                    {
                        //string strCell_ColumnName = ((BoundField)cell.ContainingField).DataField;

                        if (!string.IsNullOrEmpty(cell.Text) && cell.Text != Properties.Resources.HtmlSpace)
                        {
                            foreach (string num in cell.Text.Split(','))
                            {
                                Label lblNum = new Label { Text = num };
                                if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(int.Parse(num, InvariantCulture).ToString(InvariantCulture)))
                                {
                                    lblNum.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[int.Parse(num, InvariantCulture).ToString(InvariantCulture)];
                                }
                                else
                                {
                                    lblNum.CssClass = "lblNum";
                                }
                                cell.Controls.Add(lblNum);
                            }
                        }

                    }
                }
            }
        }

        private void GvDataRange_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        if (strCell_ColumnName.Substring(0, 4) == "RNum" || strCell_ColumnName.Substring(0, 4) == "Nums")
                        {
                            if (!string.IsNullOrEmpty(cell.Text) && cell.Text != Properties.Resources.HtmlSpace)
                            {
                                foreach (string num in cell.Text.Split(','))
                                {
                                    Label lblNum = new Label { Text = num };
                                    if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(int.Parse(num, InvariantCulture).ToString(InvariantCulture)))
                                    {
                                        lblNum.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[int.Parse(num, InvariantCulture).ToString(InvariantCulture)];
                                    }
                                    else
                                    {
                                        lblNum.CssClass = "lblNum";
                                    }
                                    cell.Controls.Add(lblNum);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GvCheckColumn_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        if (strCell_ColumnName == "DataN" || strCell_ColumnName == "DataB")
                        {
                            if (!string.IsNullOrEmpty(cell.Text) && cell.Text != Properties.Resources.HtmlSpace)
                            {
                                foreach (string num in cell.Text.Split(','))
                                {
                                    Label lblNum = new Label { Text = num };
                                    if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(int.Parse(num, InvariantCulture).ToString(InvariantCulture)))
                                    {
                                        lblNum.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[int.Parse(num, InvariantCulture).ToString(InvariantCulture)];
                                    }
                                    else
                                    {
                                        lblNum.CssClass = "lblNum";
                                    }
                                    cell.Controls.Add(lblNum);
                                }
                            }
                        }
                    }
                }
            }

        }

        private void GvDataNSum_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataTable dtDataNSum = ((DataSet)Session[DataNT01ID + "dsDataNSum"]).Tables["dtDataNSum"];
                int RowIndex = e.Row.RowIndex;
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        if (strCell_ColumnName.Substring(0, 4) == "LNum")
                        {
                            string strNum = strCell_ColumnName.Substring(4, 1);
                            Table tbl = new Table();
                            //Avg
                            TableRow tblRowAvg = new TableRow();
                            TableCell tblCellAvg = new TableCell();
                            string strAvg = dtDataNSum.Rows[RowIndex][string.Format(InvariantCulture, "Avg{0}", strNum)].ToString();
                            Label lblNumAvg = new Label() { Text = strAvg };
                            if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(strAvg))
                            {
                                lblNumAvg.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[strAvg];
                            }
                            else
                            {
                                lblNumAvg.CssClass = "lblNum";
                            }
                            tblCellAvg.Controls.Add(lblNumAvg);
                            tblRowAvg.Cells.Add(tblCellAvg);
                            tbl.Rows.Add(tblRowAvg);
                            //LNum
                            TableRow tblRowLNum = new TableRow();
                            TableCell tblCellLNum = new TableCell();
                            string strLNum = dtDataNSum.Rows[RowIndex][string.Format(InvariantCulture, "LNum{0}", strNum)].ToString();
                            Label lblNumLNum = new Label() { Text = strLNum };
                            if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(strLNum))
                            {
                                lblNumLNum.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[strLNum];
                            }
                            else
                            {
                                lblNumLNum.CssClass = "lblNum";
                            }
                            tblCellLNum.Controls.Add(lblNumLNum);
                            tblRowLNum.Cells.Add(tblCellLNum);
                            tbl.Rows.Add(tblRowLNum);

                            cell.Controls.Add(tbl);
                        }

                        if (strCell_ColumnName.Substring(0, 4) == "NMin")
                        {
                            string strNum = strCell_ColumnName.Substring(4, 1);
                            Table tbl = new Table();
                            //NMin
                            TableRow tblRowNMin = new TableRow();
                            TableCell tblCellNMin = new TableCell();
                            string strNMin = dtDataNSum.Rows[RowIndex][string.Format(InvariantCulture, "NMin{0}", strNum)].ToString();
                            Label lblNumNMin = new Label() { Text = strNMin };
                            if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(strNMin))
                            {
                                lblNumNMin.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[strNMin];
                            }
                            else
                            {
                                lblNumNMin.CssClass = "lblNum";
                            }
                            tblCellNMin.Controls.Add(lblNumNMin);
                            tblRowNMin.Cells.Add(tblCellNMin);
                            tbl.Rows.Add(tblRowNMin);
                            //NMax
                            TableRow tblRowNMax = new TableRow();
                            TableCell tblCellNMax = new TableCell();
                            string strNMax = dtDataNSum.Rows[RowIndex][string.Format(InvariantCulture, "NMax{0}", strNum)].ToString();
                            Label lblNumNMax = new Label() { Text = strNMax };
                            if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(strNMax))
                            {
                                lblNumNMax.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[strNMax];
                            }
                            else
                            {
                                lblNumNMax.CssClass = "lblNum";
                            }
                            tblCellNMax.Controls.Add(lblNumNMax);
                            tblRowNMax.Cells.Add(tblCellNMax);
                            tbl.Rows.Add(tblRowNMax);

                            cell.Controls.Add(tbl);
                        }

                        if (strCell_ColumnName.Substring(0, 4) == "SMin")
                        {
                            string strNum = strCell_ColumnName.Substring(4, 1);
                            Table tbl = new Table();
                            TableRow tblRowSMin = new TableRow();
                            TableCell tblCellSMin = new TableCell();
                            //AMin
                            string strAMin = dtDataNSum.Rows[RowIndex][string.Format(InvariantCulture, "AMin{0}", strNum)].ToString();
                            if (!string.IsNullOrEmpty(strAMin) && strAMin != Properties.Resources.HtmlSpace)
                            {
                                Label lblNumAMin = new Label() { Text = strAMin };
                                if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(strAMin))
                                {
                                    lblNumAMin.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[strAMin];
                                }
                                else
                                {
                                    lblNumAMin.CssClass = " lblNum ";
                                }
                                lblNumAMin.CssClass += " AMin ";
                                tblCellSMin.Controls.Add(lblNumAMin);
                            }
                            else
                            {
                                Label lblNumAMin = new Label() { Text = Properties.Resources.HtmlSpace };
                                tblCellSMin.Controls.Add(lblNumAMin);
                            }
                            //SMin
                            string strSMin = dtDataNSum.Rows[RowIndex][string.Format(InvariantCulture, "SMin{0}", strNum)].ToString();
                            if (!string.IsNullOrEmpty(strSMin) && strSMin != Properties.Resources.HtmlSpace)
                            {
                                foreach (string num in strSMin.Split(','))
                                {
                                    Label lblNum = new Label { Text = num };
                                    if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(int.Parse(num, InvariantCulture).ToString(InvariantCulture)))
                                    {
                                        lblNum.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[int.Parse(num, InvariantCulture).ToString(InvariantCulture)];
                                    }
                                    else
                                    {
                                        lblNum.CssClass = "lblNum";
                                    }
                                    tblCellSMin.Controls.Add(lblNum);
                                }
                            }
                            else
                            {
                                Label lblNum = new Label { Text = Properties.Resources.HtmlSpace };
                                tblCellSMin.Controls.Add(lblNum);
                            }
                            tblRowSMin.Cells.Add(tblCellSMin);

                            tbl.Rows.Add(tblRowSMin);


                            TableRow tblRowSMax = new TableRow { CssClass = "SMax1" };
                            //AMax
                            TableCell tblCellSMax = new TableCell();
                            string strAMax = dtDataNSum.Rows[RowIndex][string.Format(InvariantCulture, "AMax{0}", strNum)].ToString();
                            if (!string.IsNullOrEmpty(strAMax) && strAMax != Properties.Resources.HtmlSpace)
                            {
                                Label lblNumAMax = new Label() { Text = strAMax };
                                if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(strAMax))
                                {
                                    lblNumAMax.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[strAMax];
                                }
                                else
                                {
                                    lblNumAMax.CssClass = " lblNum ";
                                }
                                lblNumAMax.CssClass += " AMin ";
                                tblCellSMax.Controls.Add(lblNumAMax);
                            }
                            else
                            {
                                Label lblNumAMax = new Label() { Text = Properties.Resources.HtmlSpace };
                                tblCellSMax.Controls.Add(lblNumAMax);
                            }
                            tblRowSMax.Cells.Add(tblCellSMax);

                            //SMax
                            string strSMax = dtDataNSum.Rows[RowIndex][string.Format(InvariantCulture, "SMax{0}", strNum)].ToString();
                            if (!string.IsNullOrEmpty(strSMax) && strSMax != Properties.Resources.HtmlSpace)
                            {
                                foreach (string num in strSMax.Split(','))
                                {
                                    Label lblNum = new Label { Text = num };
                                    if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(int.Parse(num, InvariantCulture).ToString(InvariantCulture)))
                                    {
                                        lblNum.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[int.Parse(num, InvariantCulture).ToString(InvariantCulture)];
                                    }
                                    else
                                    {
                                        lblNum.CssClass = "lblNum";
                                    }
                                    tblCellSMax.Controls.Add(lblNum);
                                }
                            }
                            else
                            {
                                Label lblNum = new Label { Text = Properties.Resources.HtmlSpace };
                                tblCellSMax.Controls.Add(lblNum);
                            }
                            tblRowSMax.Cells.Add(tblCellSMax);

                            tbl.Rows.Add(tblRowSMax);

                            cell.Controls.Add(tbl);
                        }

                    }
                }
            }
        }

        private void GvDataBSum_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataTable dtDataBSum = ((DataSet)Session[DataNT01ID + "dsDataBSum"]).Tables["dtDataBSum"];
                int RowIndex = e.Row.RowIndex;
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        if (strCell_ColumnName.Substring(0, 4) == "LNum")
                        {
                            string strNum = strCell_ColumnName.Substring(4, 1);
                            Table tbl = new Table();
                            //Avg
                            TableRow tblRowAvg = new TableRow();
                            TableCell tblCellAvg = new TableCell();
                            string strAvg = dtDataBSum.Rows[RowIndex][string.Format(InvariantCulture, "Avg{0}", strNum)].ToString();
                            Label lblNumAvg = new Label() { Text = strAvg };
                            if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(strAvg))
                            {
                                lblNumAvg.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[strAvg];
                            }
                            else
                            {
                                lblNumAvg.CssClass = "lblNum";
                            }
                            tblCellAvg.Controls.Add(lblNumAvg);
                            tblRowAvg.Cells.Add(tblCellAvg);
                            tbl.Rows.Add(tblRowAvg);
                            //LNum
                            TableRow tblRowLNum = new TableRow();
                            TableCell tblCellLNum = new TableCell();
                            string strLNum = dtDataBSum.Rows[RowIndex][string.Format(InvariantCulture, "LNum{0}", strNum)].ToString();
                            Label lblNumLNum = new Label() { Text = strLNum };
                            if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(strLNum))
                            {
                                lblNumLNum.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[strLNum];
                            }
                            else
                            {
                                lblNumLNum.CssClass = "lblNum";
                            }
                            tblCellLNum.Controls.Add(lblNumLNum);
                            tblRowLNum.Cells.Add(tblCellLNum);
                            tbl.Rows.Add(tblRowLNum);

                            cell.Controls.Add(tbl);
                        }

                        if (strCell_ColumnName.Substring(0, 4) == "NMin")
                        {
                            string strNum = strCell_ColumnName.Substring(4, 1);
                            Table tbl = new Table();
                            //NMin
                            TableRow tblRowNMin = new TableRow();
                            TableCell tblCellNMin = new TableCell();
                            string strNMin = dtDataBSum.Rows[RowIndex][string.Format(InvariantCulture, "NMin{0}", strNum)].ToString();
                            Label lblNumNMin = new Label() { Text = strNMin };
                            if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(strNMin))
                            {
                                lblNumNMin.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[strNMin];
                            }
                            else
                            {
                                lblNumNMin.CssClass = "lblNum";
                            }
                            tblCellNMin.Controls.Add(lblNumNMin);
                            tblRowNMin.Cells.Add(tblCellNMin);
                            tbl.Rows.Add(tblRowNMin);
                            //NMax
                            TableRow tblRowNMax = new TableRow();
                            TableCell tblCellNMax = new TableCell();
                            string strNMax = dtDataBSum.Rows[RowIndex][string.Format(InvariantCulture, "NMax{0}", strNum)].ToString();
                            Label lblNumNMax = new Label() { Text = strNMax };
                            if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(strNMax))
                            {
                                lblNumNMax.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[strNMax];
                            }
                            else
                            {
                                lblNumNMax.CssClass = "lblNum";
                            }
                            tblCellNMax.Controls.Add(lblNumNMax);
                            tblRowNMax.Cells.Add(tblCellNMax);
                            tbl.Rows.Add(tblRowNMax);

                            cell.Controls.Add(tbl);
                        }

                        if (strCell_ColumnName.Substring(0, 4) == "SMin")
                        {
                            string strNum = strCell_ColumnName.Substring(4, 1);
                            Table tbl = new Table();
                            TableRow tblRowSMin = new TableRow();
                            TableCell tblCellSMin = new TableCell();
                            //AMin
                            string strAMin = dtDataBSum.Rows[RowIndex][string.Format(InvariantCulture, "AMin{0}", strNum)].ToString();
                            if (!string.IsNullOrEmpty(strAMin) && strAMin != Properties.Resources.HtmlSpace)
                            {
                                Label lblNumAMin = new Label() { Text = strAMin };
                                if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(strAMin))
                                {
                                    lblNumAMin.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[strAMin];
                                }
                                else
                                {
                                    lblNumAMin.CssClass = " lblNum ";
                                }
                                lblNumAMin.CssClass += " AMin ";
                                tblCellSMin.Controls.Add(lblNumAMin);
                            }
                            else
                            {
                                Label lblNumAMin = new Label() { Text = Properties.Resources.HtmlSpace };
                                tblCellSMin.Controls.Add(lblNumAMin);
                            }
                            //SMin
                            string strSMin = dtDataBSum.Rows[RowIndex][string.Format(InvariantCulture, "SMin{0}", strNum)].ToString();
                            if (!string.IsNullOrEmpty(strSMin) && strSMin != Properties.Resources.HtmlSpace)
                            {
                                foreach (string num in strSMin.Split(','))
                                {
                                    Label lblNum = new Label { Text = num };
                                    if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(int.Parse(num, InvariantCulture).ToString(InvariantCulture)))
                                    {
                                        lblNum.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[int.Parse(num, InvariantCulture).ToString(InvariantCulture)];
                                    }
                                    else
                                    {
                                        lblNum.CssClass = "lblNum";
                                    }
                                    tblCellSMin.Controls.Add(lblNum);
                                }
                            }
                            else
                            {
                                Label lblNum = new Label { Text = Properties.Resources.HtmlSpace };
                                tblCellSMin.Controls.Add(lblNum);
                            }
                            tblRowSMin.Cells.Add(tblCellSMin);

                            tbl.Rows.Add(tblRowSMin);


                            TableRow tblRowSMax = new TableRow { CssClass = "SMax1" };
                            //AMax
                            TableCell tblCellSMax = new TableCell();
                            string strAMax = dtDataBSum.Rows[RowIndex][string.Format(InvariantCulture, "AMax{0}", strNum)].ToString();
                            if (!string.IsNullOrEmpty(strAMax) && strAMax != Properties.Resources.HtmlSpace)
                            {
                                Label lblNumAMax = new Label() { Text = strAMax };
                                if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(strAMax))
                                {
                                    lblNumAMax.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[strAMax];
                                }
                                else
                                {
                                    lblNumAMax.CssClass = " lblNum ";
                                }
                                lblNumAMax.CssClass += " AMin ";
                                tblCellSMax.Controls.Add(lblNumAMax);
                            }
                            else
                            {
                                Label lblNumAMax = new Label() { Text = Properties.Resources.HtmlSpace };
                                tblCellSMax.Controls.Add(lblNumAMax);
                            }
                            tblRowSMax.Cells.Add(tblCellSMax);

                            //SMax
                            string strSMax = dtDataBSum.Rows[RowIndex][string.Format(InvariantCulture, "SMax{0}", strNum)].ToString();
                            if (!string.IsNullOrEmpty(strSMax) && strSMax != Properties.Resources.HtmlSpace)
                            {
                                foreach (string num in strSMax.Split(','))
                                {
                                    Label lblNum = new Label { Text = num };
                                    if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(int.Parse(num, InvariantCulture).ToString(InvariantCulture)))
                                    {
                                        lblNum.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[int.Parse(num, InvariantCulture).ToString(InvariantCulture)];
                                    }
                                    else
                                    {
                                        lblNum.CssClass = "lblNum";
                                    }
                                    tblCellSMax.Controls.Add(lblNum);
                                }
                            }
                            else
                            {
                                Label lblNum = new Label { Text = Properties.Resources.HtmlSpace };
                                tblCellSMax.Controls.Add(lblNum);
                            }
                            tblRowSMax.Cells.Add(tblCellSMax);

                            tbl.Rows.Add(tblRowSMax);

                            cell.Controls.Add(tbl);
                        }
                    }
                }
            }
        }

        private void ShowDataSum01()
        {
            if (Session[DataNT01ID + "dsDataNSum"] != null && Session[DataNT01ID + "dsDataBSum"] != null)
            {
                Panel pnlDataNSum = new GalaxyApp().CreatPanel("pnlDataNSum", "max-width");
                pnlDetail.Controls.Add(pnlDataNSum);
                //振盪統計
                GridView gvNextCount = new GalaxyApp().CreatGridView("gvNextCount", "gltable",
                                                     ((DataSet)Session[DataNT01ID + "dsDataNSum"]).Tables["dtNextCount"], true, true);
                foreach (DataControlField dcColumn in gvNextCount.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)))
                    {
                        dcColumn.HeaderStyle.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)];
                    }
                }
                gvNextCount.DataBind();
                pnlDataNSum.Controls.Add(gvNextCount);
                //平衡統計
                GridView gvBalanceCount = new GalaxyApp().CreatGridView("gvBalanceCount", "gltable",
                                                        ((DataSet)Session[DataNT01ID + "dsDataBSum"]).Tables["dtBalanceCount"], true, true);
                foreach (DataControlField dcColumn in gvBalanceCount.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)))
                    {
                        dcColumn.HeaderStyle.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)];
                    }
                }
                gvBalanceCount.DataBind();
                pnlDataNSum.Controls.Add(gvBalanceCount);
                //比較統計
                GridView gvCompareCount = new GalaxyApp().CreatGridView("gvCompareCount", "gltable",
                                                        ((DataSet)Session[DataNT01ID + "dsDataNSum"]).Tables["dtCompareCount"], true, true);
                foreach (DataControlField dcColumn in gvCompareCount.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if (((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"]).Keys.Contains(int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)))
                    {
                        dcColumn.HeaderStyle.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)];
                    }
                }
                gvCompareCount.DataBind();
                pnlDataNSum.Controls.Add(gvCompareCount);
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowddlFreqB()
        {
            if (ViewState[DataNT01ID + "dicDataB"] != null)
            {
                ddlFreq.Items.Clear();
                Dictionary<string, object> DicDataNFiels = (Dictionary<string, object>)ViewState[DataNT01ID + "dicDataB"];
                Dictionary<string, object> DicDataN = (Dictionary<string, object>)DicDataNFiels[ddlFields.SelectedValue];
                foreach (KeyValuePair<string, DataSet> keyval in (Dictionary<string, DataSet>)DicDataN["DataB"])
                {
                    ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(keyval.Key), keyval.Key));
                }
            }
        }

        private void ShowOptionDataB()
        {
            switch (ddlDataB.SelectedValue)
            {
                case "DataB":
                    ddlPercent.Visible = false;
                    ddlDays.Visible = false;
                    ShowDataB(ddlFreq.SelectedValue, ddlNums.SelectedValue);

                    break;
                case "DataBChart":
                    ddlPercent.Visible = false;
                    ddlDays.Visible = true;
                    ShowDataBChart(ddlFreq.SelectedValue, ddlNums.SelectedValue);
                    break;
                case "DataBNext":
                    ddlPercent.Visible = true;
                    ddlDays.Visible = true;
                    ShowDataBNext(ddlFreq.SelectedValue, ddlNums.SelectedValue);
                    break;
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowDataB(string ddlFreq, string ddlNums)
        {
            if (ViewState[DataNT01ID + "dicDataB"] != null)
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
                    if (strColumnName.Contains("lngB"))
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
                        string strCell_ColumnName = field.DataField;

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
            if (ViewState[DataNT01ID + "dicDataB"] != null)
            {
                //Dictionary<string, object> DicDataB = (Dictionary<string, object>)ViewState[DataNT01ID + "dicDataB"];
                using DataTable dtDataTable = GetDataTable("dtEachNumB");
                dtDataTable.DefaultView.Sort = "[lngDateSN] ASC";
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
                                             GlobalStuSearch.InDisplayPeriod * 25, 200);
                pnlDetail.Controls.Add(chAllNumB);

                double dblEachNumMin = double.Parse(dtDataTable.Compute(string.Format(InvariantCulture, "MIN([lngB{0}])", ddlNumsSelect),
                                                                        string.Empty).ToString(), InvariantCulture);
                ChartArea chaAllNumB = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAllNumB{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                       dblEachNumMin - 1d);
                chAllNumB.ChartAreas.Add(chaAllNumB);

                #region sirAllNumB
                Series sirAllNumB = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAllNumB{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), chaAllNumB.Name);
                sirAllNumB.Color = Color.SkyBlue;
                sirAllNumB.LabelForeColor = Color.SkyBlue;
                sirAllNumB.IsValueShownAsLabel = true;
                sirAllNumB.LegendText = string.Format(InvariantCulture, "Num{0}：{1:d02}", ddlNumsSelect, intCurrentNum);
                sirAllNumB.Points.DataBind(dtDataTable.DefaultView, "", string.Format(InvariantCulture, "lngB{0}", ddlNumsSelect),
                                           string.Format(InvariantCulture, "Tooltip = lngB{0}", ddlNumsSelect));
                chAllNumB.Series.Add(sirAllNumB);
                #endregion sirAllNumB

                #region sirAvgAllNumB
                Series sirAvgAllNumB = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgAllNumB{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), chaAllNumB.Name);
                sirAvgAllNumB.Color = Color.Red;
                sirAvgAllNumB.LabelForeColor = Color.Red;
                sirAvgAllNumB.IsValueShownAsLabel = true;
                sirAvgAllNumB.LegendText = string.Format(InvariantCulture, "Avg[Avg:{0} Std:{1} Min:{2} Max:{3}]", dblAvg, dblStdEvp, intAvgMin, intAvgMax);
                sirAvgAllNumB.Points.DataBind(dataSource: dtDataTable.DefaultView, "",
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
                                             GlobalStuSearch.InDisplayPeriod * 25,
                                             750);
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
                sirAllAvgT.LegendText = string.Format(InvariantCulture, "AvgT:{0}", dblAvgT);
                sirAllAvgT.Points.DataBind(dtDataTable.DefaultView,
                                        xField: "",
                                        string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect),
                                        string.Format(InvariantCulture, "Tooltip = sglAvgT{0}", ddlNumsSelect));
                chAllAvgT.Series.Add(sirAllAvgT);
                #endregion sglAvgT

                #region Series sglAvg
                Series sirAvgAllAvgT = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgAllAvgT{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                               chaAllAvgT.Name);
                sirAvgAllAvgT.Color = Color.Red;
                sirAvgAllAvgT.LegendText = string.Format(InvariantCulture, "Avg[Avg:{0} Std:{1} Min:{2} Max:{3}]", dblAvg, dblStdEvp, intAvgMin, intAvgMax);
                sirAvgAllAvgT.Points.DataBind(dataSource: dtDataTable.DefaultView,
                                           xField: "",
                                           yFields: string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                           otherFields: string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                chAllAvgT.Series.Add(sirAvgAllAvgT);
                #endregion Series sirAvg

                #region sglAvgTPoly
                using (DataTable dtsglAvgTPoly = new GalaxyApp().CreatPolynomialdt(string.Format(InvariantCulture, "sglAvgTPoly{0}", ddlNumsSelect), dtDataTable,
                                                              new string[] { "lngTotalSN", string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect) },
                                                              3))
                {

                    Series sirAvgTPoly = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgTPoly{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                        chaAllAvgT.Name);
                    sirAvgTPoly.Color = Color.Orange;
                    sirAvgTPoly.LabelForeColor = Color.Orange;
                    sirAvgTPoly.IsValueShownAsLabel = true;
                    sirAvgTPoly.LegendText = string.Format(InvariantCulture,
                                                           "{0}",
                                                           sirAvgTPoly.Name);
                    sirAvgTPoly.Points.DataBind(dtsglAvgTPoly.DefaultView, "", string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect), string.Format(InvariantCulture, "Tooltip = sglAvgT{0}", ddlNumsSelect));
                    chAllAvgT.Series.Add(item: sirAvgTPoly);
                }
                #endregion sglAvgTPoly

                ChartArea chaAvgTKD = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAvgTKD{0}{1:2d}", ddlFreqSelect, ddlNumsSelect), 0);
                chAllAvgT.ChartAreas.Add(chaAvgTKD);

                using (DataTable dtsglAvgTKD = new GalaxyApp().CreatKDdt(string.Format(InvariantCulture, "dtsglAvgTKD{0}{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                                  dtDataTable, string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect),
                                                  3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d))
                {
                    #region sglAvgTK
                    Series sirAvgTK = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgTK{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                        chaAvgTKD.Name);
                    sirAvgTK.Color = Color.DarkGreen;
                    sirAvgTK.LabelForeColor = Color.DarkGreen;
                    sirAvgTK.IsValueShownAsLabel = true;
                    sirAvgTK.LegendText = string.Format(InvariantCulture, "{0}", sirAvgTK.Name); sirAvgTK.Points.DataBind(dtsglAvgTKD.DefaultView, "", string.Format(InvariantCulture, "K"), string.Format(InvariantCulture, "Tooltip = K "));
                    chAllAvgT.Series.Add(sirAvgTK);
                    #endregion sglAvgTK

                    #region sglAvgTD
                    Series sirAvgTD = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirsglAvgTD{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAvgTKD.Name);
                    sirAvgTD.Color = Color.DarkBlue;
                    sirAvgTD.LabelForeColor = Color.DarkBlue;
                    sirAvgTD.IsValueShownAsLabel = true;
                    sirAvgTD.LegendText = string.Format(InvariantCulture, "{0}", sirAvgTD.Name);
                    sirAvgTD.Points.DataBind(dtsglAvgTKD.DefaultView, "", string.Format(InvariantCulture, "D"), string.Format(InvariantCulture, "Tooltip = D "));
                    chAllAvgT.Series.Add(sirAvgTD);
                    #endregion sglAvgTD

                    #region sglAvgTRSV
                    Series sirAvgTRSV = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirsglAvgTRSV{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                        chaAvgTKD.Name);
                    sirAvgTRSV.Color = Color.OrangeRed;
                    sirAvgTRSV.LabelForeColor = Color.OrangeRed;
                    sirAvgTRSV.IsValueShownAsLabel = true;
                    sirAvgTRSV.LegendText = string.Format(InvariantCulture, "{0}", sirAvgTRSV.Name);
                    sirAvgTRSV.Points.DataBind(dtsglAvgTKD.DefaultView, "", string.Format(InvariantCulture, "RSV"), string.Format(InvariantCulture, "Tooltip = RSV "));
                    chAllAvgT.Series.Add(sirAvgTRSV);
                    #endregion sglAvgTRSV
                }

                #endregion chAllAvgT

                #region chAllAvg
                pnlDetail.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                  string.Format(InvariantCulture, "{0}=>Avg{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                                  "gllabel"));

                double dblAllAvgMin = double.Parse(dtDataTable.Compute(string.Format(InvariantCulture, "MIN([sglAvg{0}])", ddlNumsSelect),
                                                                       string.Empty).ToString(), InvariantCulture);

                Chart chAllAvg = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                            GlobalStuSearch.InDisplayPeriod * 25,
                                            600);
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
                sirAvgAllAvg.LegendText = string.Format(InvariantCulture, "Avg[Avg:{0} Std:{1} Min:{2} Max:{3}]", dblAvg, dblStdEvp, intAvgMin, intAvgMax);
                sirAvgAllAvg.Points.DataBind(dtDataTable.DefaultView, "",
                                             string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                             string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                chAllAvg.Series.Add(sirAvgAllAvg);
                #endregion sirAvgAllAvg

                #region sglAvgPolyAllAvg
                using (DataTable dtAvgPolyAllAvg = new GalaxyApp().CreatPolynomialdt(string.Format(InvariantCulture, "dtAvgPolyAllAvg{0}{1:2d}", ddlFreqSelect, ddlNumsSelect), dtDataTable,
                                                                     new string[] { "lngTotalSN", string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect) },
                                                                     3))
                {
                    Series sirAvgPolyAllAvg = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgPolyAllAvg{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                          chaAllAvg.Name);
                    sirAvgPolyAllAvg.Color = Color.Orange;
                    sirAvgPolyAllAvg.LabelForeColor = Color.Orange;
                    sirAvgPolyAllAvg.IsValueShownAsLabel = true;
                    sirAvgPolyAllAvg.LegendText = string.Format(InvariantCulture, "AvgPoly");
                    sirAvgPolyAllAvg.Points.DataBind(dtAvgPolyAllAvg.DefaultView, string.Empty,
                                                     string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                                     string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                    chAllAvg.Series.Add(sirAvgPolyAllAvg);
                }
                #endregion sglAvgPoly

                ChartArea chaAvgKD = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAvgKD{0}{1:2d}", ddlFreqSelect, ddlNumsSelect), 0);
                chAllAvg.ChartAreas.Add(chaAvgKD);

                using (DataTable dtsglAvgKD = new GalaxyApp().CreatKDdt(string.Format(InvariantCulture, "dtAvgKD{0}{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                                 dtDataTable,
                                                 string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                                 3,
                                                 int.Parse(ddlDays.SelectedValue, InvariantCulture),
                                                 1d / 3d))
                {
                    #region sglAvgTK
                    Series sirAvgK = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgK{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                        chaAvgKD.Name);
                    sirAvgK.Color = Color.DarkGreen;
                    sirAvgK.LabelForeColor = Color.DarkGreen;
                    sirAvgK.IsValueShownAsLabel = true;
                    sirAvgK.LegendText = string.Format(InvariantCulture,
                                                           "{0}",
                                                           sirAvgK.Name);
                    sirAvgK.Points.DataBind(dtsglAvgKD.DefaultView, "", string.Format(InvariantCulture, "K"), string.Format(InvariantCulture, "Tooltip = K "));
                    chAllAvg.Series.Add(sirAvgK);
                    #endregion sglAvgTK

                    #region sglAvgTD
                    Series sirAvgD = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgD{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                 chaAvgKD.Name);
                    sirAvgD.Color = Color.DarkBlue;
                    sirAvgD.LabelForeColor = Color.DarkBlue;
                    sirAvgD.IsValueShownAsLabel = true;
                    sirAvgD.LegendText = string.Format(InvariantCulture,
                                                           "{0}",
                                                           sirAvgD.Name);
                    sirAvgD.Points.DataBind(dtsglAvgKD.DefaultView, "", string.Format(InvariantCulture, "D"), string.Format(InvariantCulture, "Tooltip = D "));
                    chAllAvg.Series.Add(sirAvgD);
                    #endregion sglAvgTD

                    #region sglAvgTRSV
                    Series sirAvgRSV = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgRSV{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                        chaAvgKD.Name);
                    sirAvgRSV.Color = Color.OrangeRed;
                    sirAvgRSV.LabelForeColor = Color.OrangeRed;
                    sirAvgRSV.IsValueShownAsLabel = true;
                    sirAvgRSV.LegendText = string.Format(InvariantCulture, "{0}", sirAvgRSV.Name);
                    sirAvgRSV.Points.DataBind(dtsglAvgKD.DefaultView, "", string.Format(InvariantCulture, "RSV"),
                                              string.Format(InvariantCulture, "Tooltip = RSV "));
                    chAllAvg.Series.Add(sirAvgRSV);
                    #endregion sglAvgTD
                }

                #endregion chAllAvg

                // --------------------------------------------------
                #region chEachSection
                foreach (var ddlSectionSelectedValue in new string[] { "05", "10", "25", "50", "100" })
                {
                    pnlDetail.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblSection{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue),
                                                      string.Format(InvariantCulture, "N{0}_S{1}", ddlNumsSelect, ddlSectionSelectedValue),
                                                      "gllabel"));

                    Chart chEachSection = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chEachSection{0}{1:d2}{2:d02}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue),
                                                 GlobalStuSearch.InDisplayPeriod * 25,
                                                 750);
                    pnlDetail.Controls.Add(chEachSection);

                    double dblAvgEachMin = double.Parse(dtDataTable.Compute(string.Format(InvariantCulture, "MIN([sglAvg{0}{1:d2}])", ddlNumsSelect, ddlSectionSelectedValue), string.Empty).ToString(), InvariantCulture);
                    double dblAvgTMin = double.Parse(dtDataTable.Compute(string.Format(InvariantCulture, "MIN([sglAvgT{0}])", ddlNumsSelect), string.Empty).ToString(), InvariantCulture);
                    ChartArea chaEachSection = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaEachSection{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue),
                                                           dblAvgEachMin < dblAvgTMin ? dblAvgEachMin - 0.5d : dblAvgTMin - 0.5d);
                    chEachSection.ChartAreas.Add(chaEachSection);

                    #region sirAvgAllEachSection
                    Series sirAvgAllEachSection = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgAllEachSection{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                    chaEachSection.Name);
                    sirAvgAllEachSection.Color = Color.Red;
                    sirAvgAllEachSection.LegendText = string.Format(provider: InvariantCulture, format: "{0}>sglAvg{1}", sirAvgAllEachSection.Name, ddlNumsSelect);
                    sirAvgAllEachSection.Points.DataBind(dataSource: dtDataTable.DefaultView,
                                                         string.Empty,
                                                         string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                                         string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                    chEachSection.Series.Add(sirAvgAllEachSection);
                    #endregion sirAvgAllEachSection

                    #region sirAvgEachSection
                    Series sirAvgEachSection = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachSection{0}{1}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue),
                                                           chaEachSection.Name);
                    sirAvgEachSection.Color = DicSectionColor[key: int.Parse(ddlSectionSelectedValue, InvariantCulture)];
                    sirAvgEachSection.LabelForeColor = DicSectionColor[key: int.Parse(ddlSectionSelectedValue, InvariantCulture)];
                    sirAvgEachSection.LegendText = string.Format(InvariantCulture, "{0}>sglAvg{1}{2:d2}", sirAvgEachSection.Name, ddlNumsSelect, ddlSectionSelectedValue);
                    sirAvgEachSection.Points.DataBind(dtDataTable.DefaultView, "", string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue), string.Format(InvariantCulture, "Tooltip = sglAvg{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue));
                    sirAvgEachSection.IsValueShownAsLabel = true;
                    chEachSection.Series.Add(sirAvgEachSection);
                    #endregion sirAvgEachSection

                    #region SeriesEachSectionPolynomial
                    using (DataTable dtAvgEachSectionPoly = new GalaxyApp().CreatPolynomialdt("AvgEachSectionPoly", dtDataTable,
                                                                              new string[] { "lngTotalSN", string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue) },
                                                                              3))
                    {
                        Series sirAvgEachSectionPoly = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachSectionPoly{0}{1}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue), chaEachSection.Name);
                        sirAvgEachSectionPoly.Color = Color.Orange;
                        sirAvgEachSectionPoly.LabelForeColor = Color.DarkOrange;
                        sirAvgEachSectionPoly.IsValueShownAsLabel = true;
                        sirAvgEachSectionPoly.LegendText = string.Format(InvariantCulture, "{0} > sglAvg{1} {2:d2} {3}", sirAvgEachSectionPoly.Name, ddlNumsSelect, ddlSectionSelectedValue, int.Parse((dtDataTable.Rows.Count / 5).ToString(InvariantCulture), InvariantCulture));
                        sirAvgEachSectionPoly.Points.DataBind(dtAvgEachSectionPoly.DefaultView,
                                                              "",
                                                              string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue),
                                                              string.Format(InvariantCulture, "Tooltip = sglAvg{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue));
                        chEachSection.Series.Add(sirAvgEachSectionPoly);
                    }
                    #endregion SeriesEachSectionPolynomial


                    ChartArea chaEachSectionKD = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaEachSectionKD{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue), 0);
                    chEachSection.ChartAreas.Add(item: chaEachSectionKD);
                    using DataTable dtAvgEachKD = new GalaxyApp().CreatKDdt("dtAvgEachKD",
                                                                              dtDataTable,
                                                                              string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue),
                                                                              3,
                                                                              int.Parse(ddlDays.SelectedValue, InvariantCulture),
                                                                              1d / 3d);
                    #region sirAvgEachK
                    Series sirAvgEachK = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachK{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue),
                                                        chaEachSectionKD.Name);
                    sirAvgEachK.Color = Color.DarkGreen;
                    sirAvgEachK.LabelForeColor = Color.DarkGreen;
                    sirAvgEachK.IsValueShownAsLabel = true;
                    sirAvgEachK.LegendText = string.Format(InvariantCulture,
                                                           "{0}",
                                                           sirAvgEachK.Name);
                    sirAvgEachK.Points.DataBind(dtAvgEachKD.DefaultView, "", string.Format(InvariantCulture, "K"), string.Format(InvariantCulture, "Tooltip = K "));
                    chEachSection.Series.Add(sirAvgEachK);
                    #endregion sirAvgEachK

                    #region sirAvgEachD
                    Series sirAvgEachD = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachD{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue),
                                                        chaEachSectionKD.Name);
                    sirAvgEachD.Color = Color.DarkBlue;
                    sirAvgEachD.LabelForeColor = Color.DarkBlue;
                    sirAvgEachD.IsValueShownAsLabel = true;
                    sirAvgEachD.LegendText = string.Format(InvariantCulture,
                                                           "{0}",
                                                           sirAvgEachD.Name);
                    sirAvgEachD.Points.DataBind(dtAvgEachKD.DefaultView, "", string.Format(InvariantCulture, "D"), string.Format(InvariantCulture, "Tooltip = D "));
                    chEachSection.Series.Add(sirAvgEachD);
                    #endregion sirAvgEachD

                    #region sirAvgEachRSV
                    Series sirAvgEachRSV = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachRSV{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue),
                                                        chaEachSectionKD.Name);
                    sirAvgEachRSV.Color = Color.OrangeRed;
                    sirAvgEachRSV.LabelForeColor = Color.OrangeRed;
                    sirAvgEachRSV.IsValueShownAsLabel = true;
                    sirAvgEachRSV.LegendText = string.Format(InvariantCulture,
                                                           "{0}",
                                                           sirAvgEachRSV.Name);
                    sirAvgEachRSV.Points.DataBind(dtAvgEachKD.DefaultView, "", string.Format(InvariantCulture, "RSV"), string.Format(InvariantCulture, "Tooltip = RSV "));
                    chEachSection.Series.Add(sirAvgEachRSV);
                    #endregion sirAvgEachRSV
                }
                #endregion chEachSection
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowDataBNext(string ddlFreqSelectedValue, string ddlNumsSelectedValue)
        {
            if (ViewState[DataNT01ID + "dicDataB"] != null)
            {
                ResetSearchOrder(DataNT01ID);
                //Dictionary<string, object> DicDataB = (Dictionary<string, object>)ViewState[DataNT01ID + "dicDataB"];
                Panel pnlDataB = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlDataB{0}", ddlFreqSelectedValue), "max-width");
                pnlDetail.Controls.Add(pnlDataB);
                DataRow LastDataBRow = GetDataTable("dtEachNumB").Rows[0];

                #region showNext
                Panel pnlDataBNext = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlDataBNext{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue), "max-width");
                pnlDetail.Controls.Add(pnlDataBNext);

                pnlDataBNext.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblDataBNext{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue),
                                                     string.Format(InvariantCulture, "{0}=>{1}下期預測資料({2})", ddlFreqSelectedValue, ddlNumsSelectedValue,
                                                                   LastDataBRow[string.Format(InvariantCulture, "lngB{0}", ddlNumsSelectedValue)]),
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
                    if ((strColumnName.Substring(startIndex: 0, length: 4) != "lngB" && strColumnName.Substring(startIndex: 0, length: 4) != "lngM") || strColumnName == "lngMethodSN")
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
            DataTable dtEachNumBNext = GetDataTable("dtEachNumBNext");
            DataRow drEachNumB = GetDataTable("dtEachNumB").Rows[0];
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
                        string strCell_ColumnName = field.DataField;
                        //ASP
                        if (strCell_ColumnName == string.Format(InvariantCulture, "ASP{0}", Nums))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= new GalaxyApp().GetASP(drEachNumB, Nums.ToString(InvariantCulture)))
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
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), 0d, new GalaxyApp().GetASP1(drEachNumB, Nums.ToString(InvariantCulture), drEachNumB), true))
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
                            if (((List<int>)ViewState[DataNT01ID + "ListCurrentNumsB"]).Sum() > 0 && new GalaxyApp().ConvertNum(GlobalStuSearch, ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsB"])[int.Parse(strCell_ColumnName.Substring(4), InvariantCulture) - 1]) == cell.Text)
                            {
                                cell.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[cell.Text];
                                e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit";
                            }
                            if (cell.Text == new GalaxyApp().ConvertNum(GlobalStuSearch, drEachNumB[strCell_ColumnName].ToString())) { e.Row.CssClass = e.Row.CssClass + " glRowLast "; }
                        }
                        //sglAvg
                        if (strCell_ColumnName.Contains("sglAvg"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumB[strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
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
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumB[columnName: strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLessG "; }
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
                            //if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glColNumInBlue "; }
                            //dblMax = double.Parse(drEachNumKDRange[string.Format(InvariantCulture, "{0}GapMax", strCell_ColumnName)].ToString(), InvariantCulture);
                            //dblMin = double.Parse(drEachNumKDRange[string.Format(InvariantCulture, "{0}GapMin", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNum "; }
                        }
                        //K
                        if (strCell_ColumnName.Contains("K") && !strCell_ColumnName.Contains("K-D"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumBKD[strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNum "; }
                        }
                        //D
                        if (strCell_ColumnName.Contains("D") && !strCell_ColumnName.Contains("K-D"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumBKD[strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNum "; }
                        }
                        //K-D
                        if (strCell_ColumnName.Contains("K-D"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (Math.Abs(double.Parse(cell.Text, InvariantCulture)) <= Math.Abs(double.Parse(drEachNumBKD[strCell_ColumnName].ToString(), InvariantCulture))) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumBKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNum "; }
                        }
                        cell.ToolTip = string.Format(InvariantCulture, "{0} [{1}]", strCuNum, strCell_ColumnName);
                    }
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowddlFields()
        {
            if (Session[DataNT01ID + "ddlFields"] != null)
            {
                if (ddlFields.Items.Count == 0)
                {
                    foreach (string strItem in (List<string>)Session[DataNT01ID + "ddlFields"])
                    {
                        ListItem listItem = new ListItem(new CglFunc().ConvertFieldNameId(strItem), strItem);
                        ddlFields.Items.Add(listItem);
                    }
                }
            }
            else
            {
                if (!DicThreadDataNB.Keys.Contains(DataNT01ID + "T01")) { CreatThread(); }
            }
        }

        private void ShowddlNums()
        {
            if (ddlNums.Items.Count == 0)
            {
                for (int intLNums = 1; intLNums <= new CglDataSet(GlobalStuSearch.LottoType).CountNumber; intLNums++)
                {
                    ddlNums.Items.Add(new ListItem(string.Format(InvariantCulture, "Num{0:d2}", intLNums), intLNums.ToString(InvariantCulture)));
                }
            }
        }

        private void ShowTitle()
        {
            if (ViewState[DataNT01ID + "title"] == null) { ViewState.Add(DataNT01ID + "title", string.Format(InvariantCulture, "{0}:{1}", "振盪平衡總表表01", new CglDBData().SetTitleString(GlobalStuSearch))); }
            Page.Title = (string)ViewState[DataNT01ID + "title"];
            lblTitle.Text = (string)ViewState[DataNT01ID + "title"];

            if (ViewState[DataNT01ID + "lblMethod"] == null) { ViewState.Add(DataNT01ID + "lblMethod", new CglMethod().SetMethodString(GlobalStuSearch)); }
            lblMethod.Text = (string)ViewState[DataNT01ID + "lblMethod"];

            if (ViewState[DataNT01ID + "lblSearchMethod"] == null) { ViewState.Add(DataNT01ID + "lblSearchMethod", new CglMethod().SetSearchMethodString(GlobalStuSearch)); }
            lblSearchMethod.Text = (string)ViewState[DataNT01ID + "lblSearchMethod"];

            //顯示當前資料
            if (ViewState[DataNT01ID + "CurrentData"] == null) { ViewState.Add(DataNT01ID + "CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(GlobalStuSearch))); }
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState[DataNT01ID + "CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            lblBriefDate.Text = new GalaxyApp().ShowBriefDate((DataTable)ViewState[DataNT01ID + "CurrentData"], (List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]);

        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowddlFreqN()
        {
            if (ViewState[DataNT01ID + "dicDataN"] != null)
            {
                ddlFreq.Items.Clear();
                Dictionary<string, object> DicDataNFiels = (Dictionary<string, object>)ViewState[DataNT01ID + "dicDataN"];
                Dictionary<string, object> DicDataN = (Dictionary<string, object>)DicDataNFiels[ddlFields.SelectedValue];
                foreach (KeyValuePair<string, DataSet> keyval in (Dictionary<string, DataSet>)DicDataN["DataN"])
                {
                    ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(keyval.Key), keyval.Key));
                }
            }
        }

        private void ShowOptionDataN()
        {
            switch (ddlDataN.SelectedValue)
            {
                case "DataN":
                    ddlPercent.Visible = false;
                    ddlDays.Visible = false;
                    ShowDataN(ddlFreq.SelectedValue, ddlNums.SelectedValue);
                    break;
                case "DataNChart":
                    ddlPercent.Visible = false;
                    ddlDays.Visible = true;
                    ShowDataNChart(ddlFreq.SelectedValue, ddlNums.SelectedValue);
                    break;
                case "DataNNext":
                    ddlPercent.Visible = true;
                    ddlDays.Visible = true;
                    ShowDataNNext(ddlFreq.SelectedValue, ddlNums.SelectedValue);
                    break;
            }
        }

        private void ShowDataNNext(string ddlFreqSelectedValue, string ddlNumsSelectedValue)
        {
            if (ViewState[DataNT01ID + "dicDataN"] != null)
            {
                ResetSearchOrder(DataNT01ID);
                //Dictionary<string, object> DicDataN = (Dictionary<string, object>)ViewState[DataNID + "dicDataN"];
                Panel pnlDataN = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlDataN{0}", ddlFreqSelectedValue), "max-width");
                pnlDetail.Controls.Add(pnlDataN);
                DataRow LastDataNRow = GetDataTable("dtEachNumN").Rows[0];

                #region showNext
                Panel pnlDataNNext = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlDataNext{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue), "max-width");
                pnlDetail.Controls.Add(pnlDataNNext);

                pnlDataNNext.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblDataNext{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue),
                                                     string.Format(InvariantCulture, "{0}=>{1}下期預測資料({2})", ddlFreqSelectedValue, ddlNumsSelectedValue,
                                                                   LastDataNRow[string.Format(InvariantCulture, "lngN{0}", ddlNumsSelectedValue)]),
                                                     "gllabel"));
                using DataTable _dtEachNumNNext = GetDataTable("dtEachNumNNext");

                #region gvDataNext 
                GridView gvDataNNext = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "gvDataNNext{0}{1}", ddlFreqSelectedValue, ddlNumsSelectedValue),
                                                    "gltable ",
                                                    _dtEachNumNNext, true, false);
                gvDataNNext.AllowSorting = false;
                gvDataNNext.DataKeyNames = new string[] { string.Format(InvariantCulture, "lngN{0}", ddlNumsSelectedValue) };
                #region Set Css
                foreach (DataControlField dcColumn in gvDataNNext.Columns)
                {
                    string strColumnName = dcColumn.SortExpression.ToString(InvariantCulture);
                    if ((strColumnName.Substring(startIndex: 0, length: 4) != "lngN" && strColumnName.Substring(startIndex: 0, length: 4) != "lngM") || strColumnName == "lngMethodSN")
                    {
                        dcColumn.ItemStyle.CssClass = strColumnName;
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
            DataTable dtEachNumNNext = GetDataTable("dtEachNumNNext");
            DataRow drEachNumN = GetDataTable("dtEachNumN").Rows[0];
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
                        string strCell_ColumnName = field.DataField;
                        //ASP
                        if (strCell_ColumnName == string.Format(InvariantCulture, "ASP{0}", Nums))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= new GalaxyApp().GetASP(drEachNumN, Nums.ToString(InvariantCulture)))
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
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), 0d, new GalaxyApp().GetASP1(drEachNumN, Nums.ToString(InvariantCulture), drEachNumN), true))
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
                            if (((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"])[int.Parse(strCell_ColumnName.Substring(4), InvariantCulture) - 1] == int.Parse(cell.Text, InvariantCulture))
                            {
                                cell.CssClass = ((Dictionary<string, string>)ViewState[DataNT01ID + "DicNumCssClass"])[cell.Text];
                                e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit";
                            }
                            if (cell.Text == drEachNumN[strCell_ColumnName].ToString()) { e.Row.CssClass = e.Row.CssClass + " glRowLast "; }
                        }
                        //sglAvg
                        if (strCell_ColumnName.Contains("sglAvg"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumN[strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
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
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumN[columnName: strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLessG "; }
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
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNum "; }
                        }
                        //K
                        if (strCell_ColumnName.Contains("K") && !strCell_ColumnName.Contains("K-D"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumNKD[strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNum "; }
                        }
                        //D
                        if (strCell_ColumnName.Contains("D") && !strCell_ColumnName.Contains("K-D"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (double.Parse(cell.Text, InvariantCulture) <= double.Parse(drEachNumNKD[strCell_ColumnName].ToString(), InvariantCulture)) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNum "; }
                        }
                        //K-D
                        if (strCell_ColumnName.Contains("K-D"))
                        {
                            cell.CssClass = strCell_ColumnName;
                            if (Math.Abs(double.Parse(cell.Text, InvariantCulture)) <= Math.Abs(double.Parse(drEachNumNKD[strCell_ColumnName].ToString(), InvariantCulture))) { cell.CssClass += " glValueLess "; }
                            dblMax = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Max", strCell_ColumnName)].ToString(), InvariantCulture);
                            dblMin = double.Parse(drEachNumNKDRange[string.Format(InvariantCulture, "{0}Min", strCell_ColumnName)].ToString(), InvariantCulture);
                            if (CglFunc.Between(double.Parse(cell.Text, InvariantCulture), dblMin, dblMax, true)) { cell.CssClass += " glValueMaxNum "; }
                        }
                        cell.ToolTip = string.Format(InvariantCulture, "{0} [{1}]", strCuNum, strCell_ColumnName);
                    }
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowDataN(string ddlFreqSelectedValue, string ddlNumsSelectedValue)
        {
            if (ViewState[DataNT01ID + "dicDataN"] != null)
            {
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
                        string strCell_ColumnName = field.DataField;

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

        private void ShowDataNChart(string ddlFreqSelect, string ddlNumsSelect)
        {
            if (ViewState[DataNT01ID + "dicDataN"] != null)
            {
                //Dictionary<string, object> DicDataN = (Dictionary<string, object>)ViewState[DataNID + "dicDataN"];
                using DataTable dtDataTable = GetDataTable("dtEachNumN");

                dtDataTable.DefaultView.Sort = "[lngDateSN] ASC";
                #region Max,Min,Avg
                int intCurrentNum = int.Parse(dtDataTable.Rows[0][string.Format(InvariantCulture, "lngN{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                double dblAvgT = double.Parse(dtDataTable.Rows[0][string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                double dblAvg = double.Parse(dtDataTable.Rows[0][string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                double dblStdEvp = double.Parse(dtDataTable.Rows[0][string.Format(InvariantCulture, "sglStdEvp{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                int intAvgMin = int.Parse(dtDataTable.Rows[0][string.Format(InvariantCulture, "intMin{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                int intAvgMax = int.Parse(dtDataTable.Rows[0][string.Format(InvariantCulture, "intMax{0}", ddlNumsSelect)].ToString(), InvariantCulture);
                #endregion Max,Min,Avg
                // --------------------------------------------------

                #region  chAllNumN
                pnlDetail.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblAllNumN{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                  string.Format(InvariantCulture, "{0}=>Num{1:2d}", ddlFreqSelect, ddlNumsSelect), "gllabel"));

                Chart chAllNumN = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chAllNumN{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                             GlobalStuSearch.InDisplayPeriod * 25, 200);
                pnlDetail.Controls.Add(chAllNumN);

                double dblEachNumMin = double.Parse(dtDataTable.Compute(string.Format(InvariantCulture, "MIN([lngN{0}])", ddlNumsSelect),
                                                                        string.Empty).ToString(), InvariantCulture);
                ChartArea chaAllNumN = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAllNumN{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                       dblEachNumMin - 1d);
                chAllNumN.ChartAreas.Add(chaAllNumN);

                chAllNumN.Legends.Add(new Legend("serieschAllNumN"));
                chAllNumN.Legends["serieschAllNumN"].Docking = Docking.Bottom;
                chAllNumN.Legends["serieschAllNumN"].DockedToChartArea = chaAllNumN.Name;
                chAllNumN.Legends["serieschAllNumN"].IsDockedInsideChartArea = false;
                chAllNumN.Legends["serieschAllNumN"].Alignment = StringAlignment.Center;
                chAllNumN.Legends["serieschAllNumN"].BackColor = Color.LightYellow;
                chAllNumN.Legends["serieschAllNumN"].BorderWidth = 1;
                chAllNumN.Legends["serieschAllNumN"].BorderColor = Color.GreenYellow;
                chAllNumN.Legends["serieschAllNumN"].AutoFitMinFontSize = 11;

                #region sirAllNumN
                Series sirAllNumN = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAllNumN{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), chaAllNumN.Name);
                sirAllNumN.Color = Color.SkyBlue;
                sirAllNumN.LabelForeColor = Color.SkyBlue;
                sirAllNumN.IsValueShownAsLabel = true;
                sirAllNumN.Legend = "serieschAllNumN";
                sirAllNumN.LegendText = string.Format(InvariantCulture, "Num{0}：{1:d02}", ddlNumsSelect, intCurrentNum);
                sirAllNumN.Points.DataBind(dtDataTable.DefaultView, "", string.Format(InvariantCulture, "lngN{0}", ddlNumsSelect),
                                           string.Format(InvariantCulture, "Tooltip = lngN{0}", ddlNumsSelect));
                chAllNumN.Series.Add(sirAllNumN);
                #endregion sirAllNumN

                #region sirAvgAllNumN
                Series sirAvgAllNumN = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgAllNumN{0}{1:d2}", ddlFreqSelect, ddlNumsSelect), chaAllNumN.Name);
                sirAvgAllNumN.Color = Color.Red;
                sirAvgAllNumN.LabelForeColor = Color.Red;
                sirAvgAllNumN.IsValueShownAsLabel = true;
                sirAvgAllNumN.Legend = "serieschAllNumN";
                sirAvgAllNumN.LegendText = string.Format(InvariantCulture, "Avg[Avg:{0} Std:{1} Min:{2} Max:{3}]", dblAvg, dblStdEvp, intAvgMin, intAvgMax);
                sirAvgAllNumN.Points.DataBind(dataSource: dtDataTable.DefaultView, "",
                                              string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                              string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                chAllNumN.Series.Add(sirAvgAllNumN);
                #endregion sirAvgAllNumN

                #endregion  chAllNumN

                #region chAllAvgT
                pnlDetail.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblAllAvgT{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                  string.Format(InvariantCulture, "{0}=>AvgT{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                                  "gllabel"));
                Chart chAllAvgT = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chAllAvgT{0}{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                             GlobalStuSearch.InDisplayPeriod * 25,
                                             750);
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
                sirAllAvgT.LegendText = string.Format(InvariantCulture, "AvgT:{0}", dblAvgT);
                sirAllAvgT.Points.DataBind(dtDataTable.DefaultView,
                                        xField: "",
                                        string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect),
                                        string.Format(InvariantCulture, "Tooltip = sglAvgT{0}", ddlNumsSelect));
                chAllAvgT.Series.Add(sirAllAvgT);
                #endregion sglAvgT

                #region Series sglAvg
                Series sirAvgAllAvgT = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgAllAvgT{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                               chaAllAvgT.Name);
                sirAvgAllAvgT.Color = Color.Red;
                sirAvgAllAvgT.LegendText = string.Format(InvariantCulture, "Avg[Avg:{0} Std:{1} Min:{2} Max:{3}]", dblAvg, dblStdEvp, intAvgMin, intAvgMax);
                sirAvgAllAvgT.Points.DataBind(dataSource: dtDataTable.DefaultView,
                                           xField: "",
                                           yFields: string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                           otherFields: string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                chAllAvgT.Series.Add(sirAvgAllAvgT);
                #endregion Series sirAvg

                #region sglAvgTPoly
                using (DataTable dtsglAvgTPoly = new GalaxyApp().CreatPolynomialdt(string.Format(InvariantCulture, "sglAvgTPoly{0}", ddlNumsSelect), dtDataTable,
                                                                   new string[] { "lngTotalSN", string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect) },
                                                                   3))
                {

                    Series sirAvgTPoly = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgTPoly{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                        chaAllAvgT.Name);
                    sirAvgTPoly.Color = Color.Orange;
                    sirAvgTPoly.LabelForeColor = Color.Orange;
                    sirAvgTPoly.IsValueShownAsLabel = true;
                    sirAvgTPoly.LegendText = string.Format(InvariantCulture,
                                                           "{0}",
                                                           sirAvgTPoly.Name);
                    sirAvgTPoly.Points.DataBind(dtsglAvgTPoly.DefaultView, "", string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect), string.Format(InvariantCulture, "Tooltip = sglAvgT{0}", ddlNumsSelect));
                    chAllAvgT.Series.Add(item: sirAvgTPoly);
                }
                #endregion sglAvgTPoly

                ChartArea chaAvgTKD = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAvgTKD{0}{1:2d}", ddlFreqSelect, ddlNumsSelect), 0);
                chAllAvgT.ChartAreas.Add(chaAvgTKD);

                using (DataTable dtsglAvgTKD = new GalaxyApp().CreatKDdt(string.Format(InvariantCulture, "dtsglAvgTKD{0}{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                                  dtDataTable, string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelect),
                                                  3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d))
                {
                    #region sglAvgTK
                    Series sirAvgTK = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgTK{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                        chaAvgTKD.Name);
                    sirAvgTK.Color = Color.DarkGreen;
                    sirAvgTK.LabelForeColor = Color.DarkGreen;
                    sirAvgTK.IsValueShownAsLabel = true;
                    sirAvgTK.LegendText = string.Format(InvariantCulture, "{0}", sirAvgTK.Name); sirAvgTK.Points.DataBind(dtsglAvgTKD.DefaultView, "", string.Format(InvariantCulture, "K"), string.Format(InvariantCulture, "Tooltip = K "));
                    chAllAvgT.Series.Add(sirAvgTK);
                    #endregion sglAvgTK

                    #region sglAvgTD
                    Series sirAvgTD = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirsglAvgTD{0}{1}", ddlFreqSelect, ddlNumsSelect), chaAvgTKD.Name);
                    sirAvgTD.Color = Color.DarkBlue;
                    sirAvgTD.LabelForeColor = Color.DarkBlue;
                    sirAvgTD.IsValueShownAsLabel = true;
                    sirAvgTD.LegendText = string.Format(InvariantCulture, "{0}", sirAvgTD.Name);
                    sirAvgTD.Points.DataBind(dtsglAvgTKD.DefaultView, "", string.Format(InvariantCulture, "D"), string.Format(InvariantCulture, "Tooltip = D "));
                    chAllAvgT.Series.Add(sirAvgTD);
                    #endregion sglAvgTD

                    #region sglAvgTRSV
                    Series sirAvgTRSV = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirsglAvgTRSV{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                        chaAvgTKD.Name);
                    sirAvgTRSV.Color = Color.OrangeRed;
                    sirAvgTRSV.LabelForeColor = Color.OrangeRed;
                    sirAvgTRSV.IsValueShownAsLabel = true;
                    sirAvgTRSV.LegendText = string.Format(InvariantCulture, "{0}", sirAvgTRSV.Name);
                    sirAvgTRSV.Points.DataBind(dtsglAvgTKD.DefaultView, "", string.Format(InvariantCulture, "RSV"), string.Format(InvariantCulture, "Tooltip = RSV "));
                    chAllAvgT.Series.Add(sirAvgTRSV);
                    #endregion sglAvgTRSV
                }

                #endregion chAllAvgT

                #region chAllAvg
                pnlDetail.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                  string.Format(InvariantCulture, "{0}=>Avg{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                                  "gllabel"));

                double dblAllAvgMin = double.Parse(dtDataTable.Compute(string.Format(InvariantCulture, "MIN([sglAvg{0}])", ddlNumsSelect),
                                                                       string.Empty).ToString(), InvariantCulture);

                Chart chAllAvg = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chAllAvg{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                            GlobalStuSearch.InDisplayPeriod * 25,
                                            600);
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
                sirAvgAllAvg.LegendText = string.Format(InvariantCulture, "Avg[Avg:{0} Std:{1} Min:{2} Max:{3}]", dblAvg, dblStdEvp, intAvgMin, intAvgMax);
                sirAvgAllAvg.Points.DataBind(dtDataTable.DefaultView, "",
                                             string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                             string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                chAllAvg.Series.Add(sirAvgAllAvg);
                #endregion sirAvgAllAvg

                #region sglAvgPolyAllAvg
                using (DataTable dtAvgPolyAllAvg = new GalaxyApp().CreatPolynomialdt(string.Format(InvariantCulture, "dtAvgPolyAllAvg{0}{1:2d}", ddlFreqSelect, ddlNumsSelect), dtDataTable,
                                                                     new string[] { "lngTotalSN", string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect) },
                                                                     3))
                {
                    Series sirAvgPolyAllAvg = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgPolyAllAvg{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                          chaAllAvg.Name);
                    sirAvgPolyAllAvg.Color = Color.Orange;
                    sirAvgPolyAllAvg.LabelForeColor = Color.Orange;
                    sirAvgPolyAllAvg.IsValueShownAsLabel = true;
                    sirAvgPolyAllAvg.LegendText = string.Format(InvariantCulture, "AvgPoly");
                    sirAvgPolyAllAvg.Points.DataBind(dtAvgPolyAllAvg.DefaultView, string.Empty,
                                                     string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                                     string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                    chAllAvg.Series.Add(sirAvgPolyAllAvg);
                }
                #endregion sglAvgPoly

                ChartArea chaAvgKD = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaAvgKD{0}{1:2d}", ddlFreqSelect, ddlNumsSelect), 0);
                chAllAvg.ChartAreas.Add(chaAvgKD);

                using (DataTable dtsglAvgKD = new GalaxyApp().CreatKDdt(string.Format(InvariantCulture, "dtAvgKD{0}{1:2d}", ddlFreqSelect, ddlNumsSelect),
                                                 dtDataTable,
                                                 string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                                 3,
                                                 int.Parse(ddlDays.SelectedValue, InvariantCulture),
                                                 1d / 3d))
                {
                    #region sglAvgTK
                    Series sirAvgK = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgK{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                        chaAvgKD.Name);
                    sirAvgK.Color = Color.DarkGreen;
                    sirAvgK.LabelForeColor = Color.DarkGreen;
                    sirAvgK.IsValueShownAsLabel = true;
                    sirAvgK.LegendText = string.Format(InvariantCulture,
                                                           "{0}",
                                                           sirAvgK.Name);
                    sirAvgK.Points.DataBind(dtsglAvgKD.DefaultView, "", string.Format(InvariantCulture, "K"), string.Format(InvariantCulture, "Tooltip = K "));
                    chAllAvg.Series.Add(sirAvgK);
                    #endregion sglAvgTK

                    #region sglAvgTD
                    Series sirAvgD = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgD{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                 chaAvgKD.Name);
                    sirAvgD.Color = Color.DarkBlue;
                    sirAvgD.LabelForeColor = Color.DarkBlue;
                    sirAvgD.IsValueShownAsLabel = true;
                    sirAvgD.LegendText = string.Format(InvariantCulture,
                                                           "{0}",
                                                           sirAvgD.Name);
                    sirAvgD.Points.DataBind(dtsglAvgKD.DefaultView, "", string.Format(InvariantCulture, "D"), string.Format(InvariantCulture, "Tooltip = D "));
                    chAllAvg.Series.Add(sirAvgD);
                    #endregion sglAvgTD

                    #region sglAvgTRSV
                    Series sirAvgRSV = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgRSV{0}{1}", ddlFreqSelect, ddlNumsSelect),
                                                        chaAvgKD.Name);
                    sirAvgRSV.Color = Color.OrangeRed;
                    sirAvgRSV.LabelForeColor = Color.OrangeRed;
                    sirAvgRSV.IsValueShownAsLabel = true;
                    sirAvgRSV.LegendText = string.Format(InvariantCulture, "{0}", sirAvgRSV.Name);
                    sirAvgRSV.Points.DataBind(dtsglAvgKD.DefaultView, "", string.Format(InvariantCulture, "RSV"),
                                              string.Format(InvariantCulture, "Tooltip = RSV "));
                    chAllAvg.Series.Add(sirAvgRSV);
                    #endregion sglAvgTD
                }

                #endregion chAllAvg

                // --------------------------------------------------
                #region chEachSection
                foreach (var ddlSectionSelectedValue in new string[] { "05", "10", "25", "50", "100" })
                {
                    pnlDetail.Controls.Add(new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblSection{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue),
                                                      string.Format(InvariantCulture, "N{0}_S{1}", ddlNumsSelect, ddlSectionSelectedValue),
                                                      "gllabel"));

                    Chart chEachSection = new GalaxyApp().CreatChart(string.Format(InvariantCulture, "chEachSection{0}{1:d2}{2:d02}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue),
                                                 GlobalStuSearch.InDisplayPeriod * 25,
                                                 750);
                    pnlDetail.Controls.Add(chEachSection);

                    double dblAvgEachMin = double.Parse(dtDataTable.Compute(string.Format(InvariantCulture, "MIN([sglAvg{0}{1:d2}])", ddlNumsSelect, ddlSectionSelectedValue), string.Empty).ToString(), InvariantCulture);
                    double dblAvgTMin = double.Parse(dtDataTable.Compute(string.Format(InvariantCulture, "MIN([sglAvgT{0}])", ddlNumsSelect), string.Empty).ToString(), InvariantCulture);
                    ChartArea chaEachSection = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaEachSection{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue),
                                                           dblAvgEachMin < dblAvgTMin ? dblAvgEachMin - 0.5d : dblAvgTMin - 0.5d);
                    chEachSection.ChartAreas.Add(chaEachSection);

                    #region sirAvgAllEachSection
                    Series sirAvgAllEachSection = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgAllEachSection{0}{1:d2}", ddlFreqSelect, ddlNumsSelect),
                                                    chaEachSection.Name);
                    sirAvgAllEachSection.Color = Color.Red;
                    sirAvgAllEachSection.LegendText = string.Format(provider: InvariantCulture, format: "{0}>sglAvg{1}", sirAvgAllEachSection.Name, ddlNumsSelect);
                    sirAvgAllEachSection.Points.DataBind(dataSource: dtDataTable.DefaultView,
                                                         string.Empty,
                                                         string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelect),
                                                         string.Format(InvariantCulture, "Tooltip = sglAvg{0}", ddlNumsSelect));
                    chEachSection.Series.Add(sirAvgAllEachSection);
                    #endregion sirAvgAllEachSection

                    #region sirAvgEachSection
                    Series sirAvgEachSection = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachSection{0}{1}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue),
                                                           chaEachSection.Name);
                    sirAvgEachSection.Color = DicSectionColor[key: int.Parse(ddlSectionSelectedValue, InvariantCulture)];
                    sirAvgEachSection.LabelForeColor = DicSectionColor[key: int.Parse(ddlSectionSelectedValue, InvariantCulture)];
                    sirAvgEachSection.LegendText = string.Format(InvariantCulture, "{0}>sglAvg{1}{2:d2}", sirAvgEachSection.Name, ddlNumsSelect, ddlSectionSelectedValue);
                    sirAvgEachSection.Points.DataBind(dtDataTable.DefaultView, "", string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue), string.Format(InvariantCulture, "Tooltip = sglAvg{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue));
                    sirAvgEachSection.IsValueShownAsLabel = true;
                    chEachSection.Series.Add(sirAvgEachSection);
                    #endregion sirAvgEachSection

                    #region SeriesEachSectionPolynomial
                    DataTable dtAvgEachSectionPoly = new GalaxyApp().CreatPolynomialdt("AvgEachSectionPoly", dtDataTable,
                                                                              new string[] { "lngTotalSN", string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue) },
                                                                              3);

                    Series sirAvgEachSectionPoly = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachSectionPoly{0}{1}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue), chaEachSection.Name);
                    sirAvgEachSectionPoly.Color = Color.Orange;
                    sirAvgEachSectionPoly.LabelForeColor = Color.DarkOrange;
                    sirAvgEachSectionPoly.IsValueShownAsLabel = true;
                    sirAvgEachSectionPoly.LegendText = string.Format(InvariantCulture, "{0} > sglAvg{1} {2:d2} {3}", sirAvgEachSectionPoly.Name, ddlNumsSelect, ddlSectionSelectedValue, int.Parse((dtDataTable.Rows.Count / 5).ToString(InvariantCulture), InvariantCulture));
                    sirAvgEachSectionPoly.Points.DataBind(dtAvgEachSectionPoly.DefaultView,
                                                          "",
                                                          string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue),
                                                          string.Format(InvariantCulture, "Tooltip = sglAvg{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue));
                    chEachSection.Series.Add(sirAvgEachSectionPoly);
                    #endregion SeriesEachSectionPolynomial

                    ChartArea chaEachSectionKD = new GalaxyApp().CreatChartArea(string.Format(InvariantCulture, "chaEachSectionKD{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue), 0);
                    chEachSection.ChartAreas.Add(item: chaEachSectionKD);
                    DataTable dtAvgEachKD = new GalaxyApp().CreatKDdt("dtAvgEachKD",
                                                                              dtDataTable,
                                                                              string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelect, ddlSectionSelectedValue),
                                                                              3,
                                                                              int.Parse(ddlDays.SelectedValue, InvariantCulture),
                                                                              1d / 3d);
                    #region sirAvgEachK
                    Series sirAvgEachK = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachK{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue),
                                                        chaEachSectionKD.Name);
                    sirAvgEachK.Color = Color.DarkGreen;
                    sirAvgEachK.LabelForeColor = Color.DarkGreen;
                    sirAvgEachK.IsValueShownAsLabel = true;
                    sirAvgEachK.LegendText = string.Format(InvariantCulture,
                                                           "{0}",
                                                           sirAvgEachK.Name);
                    sirAvgEachK.Points.DataBind(dtAvgEachKD.DefaultView, "", string.Format(InvariantCulture, "K"), string.Format(InvariantCulture, "Tooltip = K "));
                    chEachSection.Series.Add(sirAvgEachK);
                    #endregion sirAvgEachK

                    #region sirAvgEachD
                    Series sirAvgEachD = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachD{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue),
                                                        chaEachSectionKD.Name);
                    sirAvgEachD.Color = Color.DarkBlue;
                    sirAvgEachD.LabelForeColor = Color.DarkBlue;
                    sirAvgEachD.IsValueShownAsLabel = true;
                    sirAvgEachD.LegendText = string.Format(InvariantCulture,
                                                           "{0}",
                                                           sirAvgEachD.Name);
                    sirAvgEachD.Points.DataBind(dtAvgEachKD.DefaultView, "", string.Format(InvariantCulture, "D"), string.Format(InvariantCulture, "Tooltip = D "));
                    chEachSection.Series.Add(sirAvgEachD);
                    #endregion sirAvgEachD

                    #region sirAvgEachRSV
                    Series sirAvgEachRSV = new GalaxyApp().CreatSeries(string.Format(InvariantCulture, "sirAvgEachRSV{0}{1:d2}{2:d2}", ddlFreqSelect, ddlNumsSelect, ddlSectionSelectedValue),
                                                        chaEachSectionKD.Name);
                    sirAvgEachRSV.Color = Color.OrangeRed;
                    sirAvgEachRSV.LabelForeColor = Color.OrangeRed;
                    sirAvgEachRSV.IsValueShownAsLabel = true;
                    sirAvgEachRSV.LegendText = string.Format(InvariantCulture,
                                                           "{0}",
                                                           sirAvgEachRSV.Name);
                    sirAvgEachRSV.Points.DataBind(dtAvgEachKD.DefaultView, "", string.Format(InvariantCulture, "RSV"), string.Format(InvariantCulture, "Tooltip = RSV "));
                    chEachSection.Series.Add(sirAvgEachRSV);
                    #endregion sirAvgEachRSV
                }
                #endregion chEachSection

                // --------------------------------------------------
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private DataTable GetDataTable(string PreTableName)
        {
            string strTableName = string.Empty;
            switch (PreTableName)
            {
                case "dtEachNumNNext":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}{4}", PreTableName, ddlFields.SelectedValue, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Contains(strTableName))
                    {

                        ((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Add(GetEachNextDataN(strTableName));
                    }//dtEachNumNNext
                    break;
                case "dtEachNumNGap":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlFields.SelectedValue, ddlNums.SelectedValue, ddlFreq.SelectedValue);
                    if (!((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Contains(strTableName))
                    {
                        Dictionary<string, object> d1 = (Dictionary<string, object>)((Dictionary<string, object>)ViewState[DataNT01ID + "dicDataN"])[ddlFields.SelectedValue];
                        Dictionary<string, DataSet> d2 = ((Dictionary<string, DataSet>)d1["DataN"]);
                        DataTable dt = d2[ddlFreq.SelectedValue].Tables["Gap"];
                        ((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Add(new GalaxyApp().GetEachDataGap(dt, strTableName, ddlNums.SelectedValue));
                    }//dtEachNumNGap
                    break;
                case "dtEachNumNRange":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}{4}", PreTableName, ddlFields.SelectedValue, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlPercent.SelectedValue);
                    if (!((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Add(new GalaxyApp().GetDataRange(GetDataTable("dtEachNumNGap"),
                                                                                          strTableName,
                                                                                          ddlNums.SelectedValue,
                                                                                          ddlPercent.SelectedValue));
                    }//dtEachNumNRange
                    break;
                case "dtEachNumNKD":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}{4}", PreTableName, ddlFields.SelectedValue, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Add(new GalaxyApp().GetDataKDCoeficient(GetDataTable("dtEachNumN"),
                                                                                                strTableName,
                                                                                                ddlNums.SelectedValue,
                                                                                                int.Parse(ddlDays.SelectedValue, InvariantCulture),
                                                                                                3,
                                                                                                1d / 3d));
                    }//dtEachNumNKD
                    break;
                case "dtEachNumNKDRange":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}{4}", PreTableName, ddlFields.SelectedValue, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Add(new GalaxyApp().GetDataKDRange(GetDataTable("dtEachNumNKD"),
                                                                                           strTableName,
                                                                                           ddlPercent.SelectedValue));
                    }//dtEachNumNKDRange
                    break;
                case "dtEachNumN":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlFields.SelectedValue, ddlFreq.SelectedValue, ddlNums.SelectedValue);
                    if (!((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Add(GetEachDataN(strTableName));
                    }//dtEachNumN
                    break;
                case "dtEachNumBNext":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}{4}", PreTableName, ddlFields.SelectedValue, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Contains(strTableName))
                    {

                        ((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Add(GetEachNextDataB(strTableName));
                    }//dtEachNumBNext
                    break;
                case "dtEachNumBGap":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlFields.SelectedValue, ddlNums.SelectedValue, ddlFreq.SelectedValue);
                    if (!((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Contains(strTableName))
                    {
                        Dictionary<string, object> d1 = (Dictionary<string, object>)((Dictionary<string, object>)ViewState[DataNT01ID + "dicDataB"])[ddlFields.SelectedValue];
                        Dictionary<string, DataSet> d2 = ((Dictionary<string, DataSet>)d1["DataB"]);
                        DataTable dt = d2[ddlFreq.SelectedValue].Tables["Gap"];
                        ((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Add(new GalaxyApp().GetEachDataGap(dt, strTableName, ddlNums.SelectedValue));
                    }//dtEachNumBGap
                    break;
                case "dtEachNumBRange":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}{4}", PreTableName, ddlFields.SelectedValue, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlPercent.SelectedValue);
                    if (!((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Add(new GalaxyApp().GetDataRange(GetDataTable("dtEachNumBGap"),
                                                                                          strTableName,
                                                                                          ddlNums.SelectedValue,
                                                                                          ddlPercent.SelectedValue));
                    }//dtEachNumBRange
                    break;
                case "dtEachNumBKD":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}{4}", PreTableName, ddlFields.SelectedValue, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Add(new GalaxyApp().GetDataKDCoeficient(GetDataTable("dtEachNumB"),
                                                                                                strTableName,
                                                                                                ddlNums.SelectedValue,
                                                                                                int.Parse(ddlDays.SelectedValue, InvariantCulture),
                                                                                                3,
                                                                                                1d / 3d));
                    }//dtEachNumBKD
                    break;
                case "dtEachNumBKDRange":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}{4}", PreTableName, ddlFields.SelectedValue, ddlNums.SelectedValue, ddlFreq.SelectedValue, ddlDays.SelectedValue);
                    if (!((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Add(new GalaxyApp().GetDataKDRange(GetDataTable("dtEachNumBKD"),
                                                                                           strTableName,
                                                                                           ddlPercent.SelectedValue));
                    }//dtEachNumBKDRange
                    break;
                case "dtEachNumB":
                    strTableName = string.Format(InvariantCulture, "{0}{1}{2}{3}", PreTableName, ddlFields.SelectedValue, ddlFreq.SelectedValue, ddlNums.SelectedValue);
                    if (!((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Contains(strTableName))
                    {
                        ((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables.Add(GetEachDataB(strTableName));
                    }//dtEachNumB
                    break;
            }
            return ((DataSet)ViewState[DataNT01ID + "dsDataNB"]).Tables[strTableName];
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
            using DataView dvDataN = new DataView(((Dictionary<string, DataSet>)((Dictionary<string, object>)((Dictionary<string, object>)ViewState[DataNT01ID + "dicDataN"])[ddlFields.SelectedValue])["DataN"])[ddlFreq.SelectedValue].Tables[ddlFreq.SelectedValue]);
            using DataTable dtReturn = dvDataN.ToTable(false, lstColDataN.ToArray());

            dtReturn.TableName = strTableName;
            return dtReturn;
        }

        private DataTable GetEachNextDataN(string strTableName)
        {
            List<string> lstColDataNext = new List<string>
                            {
                                string.Format(InvariantCulture,"lngN{0}",ddlNums.SelectedValue),
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
                lstColDataNext.Add(string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNums.SelectedValue, section));
                lstColDataNext.Add(string.Format(InvariantCulture, "sglStdEvp{0}{1:d2}", ddlNums.SelectedValue, section));
                //lstColDataNext.Add(string.Format(InvariantCulture, "intMin{0}{1:d2}", selectedValue2, section));
                //lstColDataNext.Add(string.Format(InvariantCulture, "intMax{0}{1:d2}", selectedValue2, section));
            }

            using DataView dvDataNext = new DataView(((Dictionary<string, DataSet>)((Dictionary<string, object>)((Dictionary<string, object>)ViewState[DataNT01ID + "dicDataN"])[ddlFields.SelectedValue])["DataN"])[ddlFreq.SelectedValue].Tables["Next"]);
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
                DataTable dtDataTable = GetDataTable("dtEachNumN");

                DataRow drAvgTemp = new GalaxyApp().GetNextNumKDCoeficient(GlobalStuSearch, dtDataTable, drEachNumNext,
                                                 string.Format(InvariantCulture, "sglAvgT{0}", ddlNums.SelectedValue),
                                                 3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d);
                drEachNumNext[string.Format(InvariantCulture, "atRSV{0}", ddlNums.SelectedValue)] = drAvgTemp["RSV"];
                drEachNumNext[string.Format(InvariantCulture, "atK{0}", ddlNums.SelectedValue)] = drAvgTemp["K"];
                drEachNumNext[string.Format(InvariantCulture, "atD{0}", ddlNums.SelectedValue)] = drAvgTemp["D"];
                drEachNumNext[string.Format(InvariantCulture, "atK-D{0}", ddlNums.SelectedValue)] = drAvgTemp["K-D"];

                drAvgTemp = new GalaxyApp().GetNextNumKDCoeficient(GlobalStuSearch, dtDataTable, drEachNumNext,
                                                 string.Format(InvariantCulture, "sglAvg{0}", ddlNums.SelectedValue),
                                                 3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d);
                drEachNumNext[string.Format(InvariantCulture, "avRSV{0}", ddlNums.SelectedValue)] = drAvgTemp["RSV"];
                drEachNumNext[string.Format(InvariantCulture, "avK{0}", ddlNums.SelectedValue)] = drAvgTemp["K"];
                drEachNumNext[string.Format(InvariantCulture, "avD{0}", ddlNums.SelectedValue)] = drAvgTemp["D"];
                drEachNumNext[string.Format(InvariantCulture, "avK-D{0}", ddlNums.SelectedValue)] = drAvgTemp["K-D"];

                foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
                {
                    drAvgTemp = new GalaxyApp().GetNextNumKDCoeficient(GlobalStuSearch, dtDataTable, drEachNumNext,
                                                string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNums.SelectedValue, section),
                                                3, int.Parse(ddlDays.SelectedValue, InvariantCulture), 1d / 3d);
                    drEachNumNext[string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgTemp["RSV"];
                    drEachNumNext[string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgTemp["K"];
                    drEachNumNext[string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgTemp["D"];
                    drEachNumNext[string.Format(InvariantCulture, "aK-D{0}{1:d2}", ddlNums.SelectedValue, section)] = drAvgTemp["K-D"];
                }
                #endregion KD Coefficient
            }

            dtReturn.PrimaryKey = new DataColumn[] { dtReturn.Columns[string.Format(InvariantCulture, "lngN{0}", ddlNums.SelectedValue)] };
            dtReturn.Locale = InvariantCulture;
            dtReturn.TableName = strTableName;

            return dtReturn;
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
            using DataView dvDataB = new DataView(((Dictionary<string, DataSet>)((Dictionary<string, object>)((Dictionary<string, object>)ViewState[DataNT01ID + "dicDataB"])[ddlFields.SelectedValue])["DataB"])[ddlFreq.SelectedValue].Tables[ddlFreq.SelectedValue]);
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
            using DataView dvDataBext = new DataView(((Dictionary<string, DataSet>)((Dictionary<string, object>)((Dictionary<string, object>)ViewState[DataNT01ID + "dicDataB"])[ddlFields.SelectedValue])["DataB"])[ddlFreq.SelectedValue].Tables["Next"]);
            using DataTable dtReturn = dvDataBext.ToTable(false, lstColDataBNext.ToArray());
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
                //Dictionary<string, object> DicDataB = (Dictionary<string, object>)ViewState[DataNT01ID + "dicDataB"];
                DataTable dtDataTable = GetDataTable("dtEachNumB");

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

            dtReturn.PrimaryKey = new DataColumn[] { dtReturn.Columns[string.Format(InvariantCulture, "lngB{0}", ddlNums.SelectedValue)] };
            dtReturn.Locale = InvariantCulture;
            dtReturn.TableName = strTableName;

            return dtReturn;
        }

        // ---------------------------------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:不要將常值當做已當地語系化的參數傳遞", MessageId = "System.Web.UI.WebControls.ListItem.#ctor(System.String,System.String)")]

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
            ReleaseMemory();
        }

        private void ReleaseMemory()
        {
            ViewState.Clear();

            Session.Remove(DataNT01ID + "ddlFields");
            Session.Remove(DataNT01ID + "lblT01");
            Session.Remove(DataNT01ID + "dicDataN");
            Session.Remove(DataNT01ID + "dicDataB");
            Session.Remove(DataNT01ID + "dsDataNSum");
            Session.Remove(DataNT01ID + "dsDataBSum");
            ResetSearchOrder(DataNT01ID);
            if (DicThreadDataNB.Keys.Contains(DataNT01ID + "T01"))
            {
                Thread ThreadFreqActive01 = (Thread)DicThreadDataNB[DataNT01ID + "T01"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActive01.ThreadState == ThreadState.Suspended) { ThreadFreqActive01.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActive01.Abort();
                ThreadFreqActive01.Join();
                DicThreadDataNB.Remove(DataNT01ID + "T01");
            }
        }

        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            if (DicThreadDataNB != null && DicThreadDataNB.Keys.Contains(DataNT01ID + "T01"))
            {
                Thread ThreadFreqActive01 = (Thread)DicThreadDataNB[DataNT01ID + "T01"];
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
            if (DicThreadDataNB.Keys.Contains(DataNT01ID + "T01"))
            {
                Thread01 = (Thread)DicThreadDataNB[DataNT01ID + "T01"];
                if (Thread01.IsAlive)
                {
                    lblArgument.Visible = true;
                    lblArgument.Text = string.Format(InvariantCulture, "{0}", DateTime.Now.ToLongTimeString());
                    btnT1Start.Visible = true;
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0} {1} ", Session[DataNT01ID + "lblT01"].ToString(), new GalaxyApp().GetTheadState(Thread01.ThreadState));
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
            Timer1.Enabled = true;
            Thread01 = new Thread(() => { StartThread01(); ResetSearchOrder(DataNT01ID); })
            {
                Name = DataNT01ID + "T01"
            };
            Thread01.Start();
            DicThreadDataNB.Add(DataNT01ID + "T01", Thread01);
        }

        private void StartThread01()
        {
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
            Timer1.Enabled = true;
            string strHtmlDirectory = System.IO.Path.Combine(Server.MapPath("~"), "xml");
            string strXmlFileName = string.Format(InvariantCulture, "{0}.xml", DataNT01ID);
            List<string> Fields = (List<string>)new CglValidFields().GetValidFieldsLst(GlobalStuSearch);
            Dictionary<string, object> DicDataN = new Dictionary<string, object>();
            Dictionary<string, object> DicDataB = new Dictionary<string, object>();
            DataSet dsDataNSum = new DataSet() { Locale = InvariantCulture, DataSetName = "dsDataNSum" };
            DataSet dsDataBSum = new DataSet() { Locale = InvariantCulture, DataSetName = "dsDataBSum" };
            if (Session[DataNT01ID + "ddlFields"] == null) { Session.Add(DataNT01ID + "ddlFields", Fields); }

            if (!new CglFunc().FileExist(strHtmlDirectory, strXmlFileName))
            {
                bool WednesdayCheck = new DateTime(int.Parse(((DataTable)ViewState[DataNT01ID + "CurrentData"]).Rows[0]["lngDateSN"].ToString().Substring(0, 4), InvariantCulture), int.Parse(((DataTable)ViewState[DataNT01ID + "CurrentData"]).Rows[0]["lngDateSN"].ToString().Substring(4, 2), InvariantCulture), int.Parse(((DataTable)ViewState[DataNT01ID + "CurrentData"]).Rows[0]["lngDateSN"].ToString().Substring(6, 2), InvariantCulture)).DayOfWeek == DayOfWeek.Wednesday;
                DataTable dtDataNSum = CreatDataSum();
                dtDataNSum.TableName = "dtDataNSum";
                DataTable dtDataBSum = CreatDataSum();
                dtDataBSum.TableName = "dtDataBSum";
                foreach (string strField in Fields)
                {
                    StuGLSearch stuSearchTemp = GlobalStuSearch;
                    stuSearchTemp.FieldMode = strField != "gen";
                    stuSearchTemp.StrCompares = strField != "gen" ? strField : "gen";
                    stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
                    stuSearchTemp = new CglMethod().GetMethodSN(stuSearchTemp);
                    stuSearchTemp = new CglMethod().GetSearchMethodSN(stuSearchTemp);
                    stuSearchTemp = new CglMethod().GetSecFieldSN(stuSearchTemp);

                    #region DataN
                    Session[DataNT01ID + "lblT01"] = string.Format(InvariantCulture, "DataN:{0}", new CglFunc().ConvertFieldNameId(strField));
                    DicDataN.Add(strField, new CglDataN00().GetDataN00Dic(stuSearchTemp, CglDataN.TableName.QryDataN00, SortOrder.Descending));
                    #region dtDataNSum
                    DataRow drDataNSum = dtDataNSum.NewRow();
                    DataTable dtDataN = ((Dictionary<string, DataSet>)((Dictionary<string, object>)DicDataN[strField])["DataN"])["gen"].Tables["gen"];
                    DataTable dtDataNGap = ((Dictionary<string, DataSet>)((Dictionary<string, object>)DicDataN[strField])["DataN"])["gen"].Tables["Gap"];
                    DataTable dtDataNNext = ((Dictionary<string, DataSet>)((Dictionary<string, object>)DicDataN[strField])["DataN"])["gen"].Tables["Next"];
                    drDataNSum["Field"] = new CglFunc().ConvertFieldNameId(strField);
                    for (int num = 1; num <= ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count; num++)
                    {
                        drDataNSum[string.Format(InvariantCulture, "LNum{0}", num)] = dtDataN.Rows[0][string.Format(InvariantCulture, "lngN{0}", num)];
                        drDataNSum[string.Format(InvariantCulture, "Avg{0}", num)] =
                            Math.Round(double.Parse(dtDataN.Rows[0][string.Format(InvariantCulture, "sglAvg{0}", num)].ToString(), InvariantCulture), 0);

                        List<int> lstStdEvp = (List<int>)GetLstStdEvp(dtDataNNext, dtDataNGap, num, "lngN");

                        if (lstStdEvp.Count > 0)
                        {
                            drDataNSum[string.Format(InvariantCulture, "NMin{0}", num)] = lstStdEvp.First();
                            drDataNSum[string.Format(InvariantCulture, "NMax{0}", num)] = lstStdEvp.Last();
                            List<int> lstAvg = GetLstAvg(dtDataNNext, dtDataNGap, num, "lngN");
                            drDataNSum[string.Format(InvariantCulture, "AMin{0}", num)] = lstAvg.Count > 0 && lstStdEvp.Contains(lstAvg.First()) ? lstAvg.First() != 1 ? string.Format(InvariantCulture, "{0:d2}", lstAvg.First()) : string.Empty : string.Empty;
                            drDataNSum[string.Format(InvariantCulture, "AMax{0}", num)] = lstAvg.Count > 0 && lstStdEvp.Contains(lstAvg.Last()) ? lstAvg.Last() != new CglDataSet(GlobalStuSearch.LottoType).LottoNumbers ? string.Format(InvariantCulture, "{0:d2}", lstAvg.Last()) : string.Empty : string.Empty;
                            List<int> lstAvgSecMin = new List<int>();
                            List<int> lstAvgSecMax = new List<int>();
                            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
                            {
                                List<int> lstAvgSec = GetLstAvgSec(dtDataNNext, dtDataNGap, num, section, "lngN");
                                if (lstStdEvp.Contains(lstAvgSec.First()) && !lstAvgSecMin.Contains(lstAvgSec.First()))
                                {
                                    lstAvgSecMin.Add(lstAvgSec.First());
                                }
                                if (lstStdEvp.Contains(lstAvgSec.Last()) && !lstAvgSecMax.Contains(lstAvgSec.Last()))
                                {
                                    lstAvgSecMax.Add(lstAvgSec.Last());
                                }
                            }
                            drDataNSum[string.Format(InvariantCulture, "SMin{0}", num)] = lstAvgSecMin.Count > 0 ? string.Join(",", ((List<string>)ConvertLst(lstAvgSecMin)).ToArray()) : string.Empty;
                            drDataNSum[string.Format(InvariantCulture, "SMax{0}", num)] = lstAvgSecMax.Count > 0 ? string.Join(",", ((List<string>)ConvertLst(lstAvgSecMax)).ToArray()) : string.Empty;
                        }
                    }
                    dtDataNSum.Rows.Add(drDataNSum);
                    #endregion dtDataNSum
                    #endregion DataN

                    #region DataB
                    Session[DataNT01ID + "lblT01"] = string.Format(InvariantCulture, "DataB:{0}", new CglFunc().ConvertFieldNameId(strField));
                    DicDataB.Add(strField, new CglDataB00().GetDataB00Dic(stuSearchTemp, CglDBDataB.TableName.QryDataB00, SortOrder.Descending));
                    #region dtDataBSum
                    DataRow drDataBSum = dtDataBSum.NewRow();
                    DataTable dtDataB = ((Dictionary<string, DataSet>)((Dictionary<string, object>)DicDataB[strField])["DataB"])["gen"].Tables["gen"];
                    DataTable dtDataBGap = ((Dictionary<string, DataSet>)((Dictionary<string, object>)DicDataB[strField])["DataB"])["gen"].Tables["Gap"];
                    DataTable dtDataBNext = ((Dictionary<string, DataSet>)((Dictionary<string, object>)DicDataB[strField])["DataB"])["gen"].Tables["Next"];
                    drDataBSum["Field"] = new CglFunc().ConvertFieldNameId(strField);
                    for (int num = 1; num <= ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count; num++)
                    {
                        int numB = num;
                        if (GlobalStuSearch.LottoType == TargetTable.Lotto539 && strField == "strHourTwentyEight" && WednesdayCheck)
                        {
                            switch (num)
                            {
                                case 2:
                                    numB = num + 2;
                                    break;
                                case 4:
                                    numB = num - 2;
                                    break;
                            }
                        }
                        drDataBSum[string.Format(InvariantCulture, "LNum{0}", numB)] = new GalaxyApp().ConvertNum(GlobalStuSearch, dtDataB.Rows[0][string.Format(InvariantCulture, "lngB{0}", num)].ToString());
                        drDataBSum[string.Format(InvariantCulture, "Avg{0}", numB)] = new GalaxyApp().ConvertNum(GlobalStuSearch, Math.Round(double.Parse(dtDataB.Rows[0][string.Format(InvariantCulture, "sglAvg{0}", num)].ToString(), InvariantCulture), 0).ToString(InvariantCulture));

                        //DataRow drEachnumBRange = GetDataRange(dtDataBGap, num, "gen").Rows[0];


                        List<int> lstStdEvp = (List<int>)GetLstStdEvp(dtDataBNext, dtDataBGap, num, "lngB");

                        if (lstStdEvp.Count > 0)
                        {
                            drDataBSum[string.Format(InvariantCulture, "NMin{0}", numB)] = lstStdEvp.First();
                            drDataBSum[string.Format(InvariantCulture, "NMax{0}", numB)] = lstStdEvp.Last();
                            List<int> lstAvg = GetLstAvg(dtDataBNext, dtDataBGap, num, "lngB");
                            drDataBSum[string.Format(InvariantCulture, "AMin{0}", numB)] = lstAvg.Count > 0 && lstStdEvp.Contains(lstAvg.First()) ? string.Format(InvariantCulture, "{0:d2}", lstAvg.First()) : string.Empty;
                            drDataBSum[string.Format(InvariantCulture, "AMax{0}", numB)] = lstAvg.Count > 0 && lstStdEvp.Contains(lstAvg.Last()) ? string.Format(InvariantCulture, "{0:d2}", lstAvg.Last()) : string.Empty;
                            List<int> lstAvgSecMin = new List<int>();
                            List<int> lstAvgSecMax = new List<int>();
                            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
                            {
                                List<int> lstAvgSec = GetLstAvgSec(dtDataBNext, dtDataBGap, num, section, "lngB");
                                if (lstStdEvp.Contains(lstAvgSec.First()) && !lstAvgSecMin.Contains(lstAvgSec.First()))
                                {
                                    lstAvgSecMin.Add(lstAvgSec.First());
                                }
                                if (lstStdEvp.Contains(lstAvgSec.Last()) && !lstAvgSecMax.Contains(lstAvgSec.Last()))
                                {
                                    lstAvgSecMax.Add(lstAvgSec.Last());
                                }
                            }
                            drDataBSum[string.Format(InvariantCulture, "SMin{0}", numB)] = lstAvgSecMin.Count > 0 ? string.Join(",", ((List<string>)ConvertLst(lstAvgSecMin)).ToArray()) : string.Empty;
                            drDataBSum[string.Format(InvariantCulture, "SMax{0}", numB)] = lstAvgSecMax.Count > 0 ? string.Join(",", ((List<string>)ConvertLst(lstAvgSecMax)).ToArray()) : string.Empty;
                        }
                    }
                    dtDataBSum.Rows.Add(drDataBSum);
                    #endregion dtDataBSum
                    #endregion DataB
                }
                dsDataNSum.Tables.Add(dtDataNSum);//Add Table dtDataNSum
                dsDataBSum.Tables.Add(dtDataBSum);//Add Table dtDataNSum

                Session[DataNT01ID + "lblT01"] = string.Format(InvariantCulture, "Data:{0}", "統計資料");

                #region dtDataNRange
                DataTable dtDataNRange = CreatDataRange();
                dtDataNRange.TableName = "dtDataNRange";
                //Min
                List<string> lstNextMin = new List<string>();
                DataRow drDataNRange = dtDataNRange.NewRow();
                drDataNRange["Item"] = "Min";
                for (int num = 1; num <= ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count; num++)
                {
                    drDataNRange[string.Format(InvariantCulture, "RNum{0}", num)] =
                        dtDataNSum.Compute(string.Format(InvariantCulture, "Max([NMin{0}])", num), string.Empty).ToString();
                    List<string> lstNums = new List<string>();
                    foreach (DataRow drDataNSum00 in dtDataNSum.Rows)
                    {
                        if (!string.IsNullOrEmpty(drDataNSum00[string.Format(InvariantCulture, "AMin{0}", num)].ToString()))
                        {
                            lstNums.Add(drDataNSum00[string.Format(InvariantCulture, "AMin{0}", num)].ToString());
                        }
                        if (!string.IsNullOrEmpty(drDataNSum00[string.Format(InvariantCulture, "SMin{0}", num)].ToString()))
                        {
                            lstNums.Add(drDataNSum00[string.Format(InvariantCulture, "SMin{0}", num)].ToString());
                        }
                    }
                    drDataNRange[string.Format(InvariantCulture, "Nums{0}", num)] =
                        string.Join(",", ((List<string>)ConvertLst((List<int>)ConvertLst(lstNums))).ToArray());
                    lstNextMin.Add(string.Join(",", ((List<string>)ConvertLst((List<int>)ConvertLst(lstNums))).ToArray()));
                }
                dtDataNRange.Rows.Add(drDataNRange);
                //Max
                List<string> lstNextMax = new List<string>();
                drDataNRange = dtDataNRange.NewRow();
                drDataNRange["Item"] = "Max";
                for (int num = 1; num <= ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count; num++)
                {
                    drDataNRange[string.Format(InvariantCulture, "RNum{0}", num)] =
                        dtDataNSum.Compute(string.Format(InvariantCulture, "Min([NMax{0}])", num), string.Empty).ToString();
                    List<string> lstNums = new List<string>();
                    foreach (DataRow drDataNSum00 in dtDataNSum.Rows)
                    {
                        if (!string.IsNullOrEmpty(drDataNSum00[string.Format(InvariantCulture, "AMax{0}", num)].ToString()))
                        {
                            lstNums.Add(drDataNSum00[string.Format(InvariantCulture, "AMax{0}", num)].ToString());
                        }
                        if (!string.IsNullOrEmpty(drDataNSum00[string.Format(InvariantCulture, "SMax{0}", num)].ToString()))
                        {
                            lstNums.Add(drDataNSum00[string.Format(InvariantCulture, "SMax{0}", num)].ToString());
                        }
                    }
                    drDataNRange[string.Format(InvariantCulture, "Nums{0}", num)] =
                        string.Join(",", ((List<string>)ConvertLst((List<int>)ConvertLst(lstNums))).ToArray());
                    lstNextMax.Add(string.Join(",", ((List<string>)ConvertLst((List<int>)ConvertLst(lstNums))).ToArray()));
                }
                dtDataNRange.Rows.Add(drDataNRange);
                #endregion dtDataNRange
                dsDataNSum.Tables.Add(dtDataNRange);//Add Table dtDataNRange
                List<string> lstNext = CombindList(lstNextMin, lstNextMax);
                DataTable dtNextCount = CountNums(lstNext);
                dtNextCount.TableName = "dtNextCount";
                dsDataNSum.Tables.Add(dtNextCount);

                #region dtDataBRange
                DataTable dtDataBRange = CreatDataRange();
                dtDataBRange.TableName = "dtDataBRange";
                //Min
                List<string> lstBalanceMin = new List<string>();
                DataRow drDataBRange = dtDataBRange.NewRow();
                drDataBRange["Item"] = "Min";
                for (int num = 1; num <= ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count; num++)
                {
                    drDataBRange[string.Format(InvariantCulture, "RNum{0}", num)] =
                        dtDataBSum.Compute(string.Format(InvariantCulture, "Max([NMin{0}])", num), string.Empty).ToString();
                    List<string> lstNums = new List<string>();
                    foreach (DataRow drDataBSum00 in dtDataBSum.Rows)
                    {
                        if (!string.IsNullOrEmpty(drDataBSum00[string.Format(InvariantCulture, "AMin{0}", num)].ToString()))
                        {
                            lstNums.Add(drDataBSum00[string.Format(InvariantCulture, "AMin{0}", num)].ToString());
                        }
                        if (!string.IsNullOrEmpty(drDataBSum00[string.Format(InvariantCulture, "SMin{0}", num)].ToString()))
                        {
                            lstNums.Add(drDataBSum00[string.Format(InvariantCulture, "SMin{0}", num)].ToString());
                        }
                    }
                    drDataBRange[string.Format(InvariantCulture, "Nums{0}", num)] =
                        string.Join(",", ((List<string>)ConvertLst((List<int>)ConvertLst(lstNums))).ToArray());
                    lstBalanceMin.Add(string.Join(",", ((List<string>)ConvertLst((List<int>)ConvertLst(lstNums))).ToArray()));
                }
                dtDataBRange.Rows.Add(drDataBRange);
                //Max
                List<string> lstBalanceMax = new List<string>();
                drDataBRange = dtDataBRange.NewRow();
                drDataBRange["Item"] = "Max";
                for (int num = 1; num <= ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count; num++)
                {
                    drDataBRange[string.Format(InvariantCulture, "RNum{0}", num)] =
                        dtDataBSum.Compute(string.Format(InvariantCulture, "Min([NMax{0}])", num), string.Empty).ToString();
                    List<string> lstNums = new List<string>();
                    foreach (DataRow drDataBSum00 in dtDataBSum.Rows)
                    {
                        if (!string.IsNullOrEmpty(drDataBSum00[string.Format(InvariantCulture, "AMax{0}", num)].ToString()))
                        {
                            lstNums.Add(drDataBSum00[string.Format(InvariantCulture, "AMax{0}", num)].ToString());
                        }
                        if (!string.IsNullOrEmpty(drDataBSum00[string.Format(InvariantCulture, "SMax{0}", num)].ToString()))
                        {
                            lstNums.Add(drDataBSum00[string.Format(InvariantCulture, "SMax{0}", num)].ToString());
                        }
                    }
                    drDataBRange[string.Format(InvariantCulture, "Nums{0}", num)] =
                        string.Join(",", ((List<string>)ConvertLst((List<int>)ConvertLst(lstNums))).ToArray());
                    lstBalanceMax.Add(string.Join(",", ((List<string>)ConvertLst((List<int>)ConvertLst(lstNums))).ToArray()));
                }
                dtDataBRange.Rows.Add(drDataBRange);
                #endregion dtDataBRange
                dsDataBSum.Tables.Add(dtDataBRange);//Add Table dtDataBRange
                List<string> lstBalance = CombindList(lstBalanceMin, lstBalanceMax);
                DataTable dtBalanceCount = CountNums(lstBalance);
                dtBalanceCount.TableName = "dtBalanceCount";
                dsDataBSum.Tables.Add(dtBalanceCount);

                #region dtCheckColumn
                DataTable dtCheckColumn = CreatCheckColumn();
                DataRow drCheckColumn = dtCheckColumn.NewRow();
                drCheckColumn["Item"] = "斜柱(□)";
                drCheckColumn["DataN"] = CheckSideColumn(lstNextMin, lstNextMax);
                drCheckColumn["DataB"] = CheckSideColumn(lstBalanceMin, lstBalanceMax);
                dtCheckColumn.Rows.Add(drCheckColumn);

                drCheckColumn = dtCheckColumn.NewRow();
                drCheckColumn["Item"] = "同柱(○)";
                drCheckColumn["DataN"] = CheckSameColumn(lstNextMin, lstNextMax);
                drCheckColumn["DataB"] = CheckSameColumn(lstBalanceMin, lstBalanceMax);
                dtCheckColumn.Rows.Add(drCheckColumn);

                #endregion dtCheckColumn
                dsDataNSum.Tables.Add(dtCheckColumn);

                #region dtCompare
                DataTable dtCompare = CreatCompareTable();
                for (int i = 0; i < ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count; i++)
                {
                    DataRow drCompare = dtCompare.NewRow();
                    //Panel pnlR = new Panel { ID = string.Format(InvariantCulture, "pnlR{0}", i) }; pnlResualt.Controls.Add(pnlR);
                    for (int j = 0; j < ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count; j++)
                    {
                        int FirstIndex = i + 2 + j;
                        FirstIndex = FirstIndex < ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count ? FirstIndex : FirstIndex - ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count;
                        FirstIndex = FirstIndex < ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count ? FirstIndex : FirstIndex - ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count;
                        //TextBox txtNextBalance = new TextBox
                        //{
                        //    ID = string.Format(InvariantCulture, "txtNextBalance{0}{1}", FirstIndex, j),
                        //    ReadOnly = true,
                        //    Width = 220
                        //};
                        //txtNextBalance.Text = Compare(lstNext[FirstIndex], lstBalance[j]);
                        //txtNextBalance.ToolTip = txtNextBalance.Text;
                        //pnlR.Controls.Add(txtNextBalance);
                        drCompare[string.Format(InvariantCulture, "Compare{0:d2}", j + 1)] = Compare(lstNext[FirstIndex], lstBalance[j]);
                    }
                    dtCompare.Rows.Add(drCompare);
                }
                #endregion dtCompare
                dsDataNSum.Tables.Add(dtCompare);

                #region dtCompareCount
                DataTable dtCompareCount = CountNums(dtCompare);
                dtCompareCount.TableName = "dtCompareCount";
                #endregion dtCompareCount
                dsDataNSum.Tables.Add(dtCompareCount);

                if (new CglData().IsHasNumber(GlobalStuSearch))
                {
                    SaveToXml(new Dictionary<string, object>
                    {
                    { "DicDataN", DicDataN },
                    { "DicDataB", DicDataB },
                    { "DsDataNSum", dsDataNSum },
                    { "DsDataBSum", dsDataBSum }
                    }, strHtmlDirectory, strXmlFileName);
                }
            }
            else
            {
                Dictionary<string, object> dicDataNB = LoadFromXml(strHtmlDirectory, strXmlFileName);
                DicDataN = (Dictionary<string, object>)dicDataNB["DicDataN"];
                DicDataB = (Dictionary<string, object>)dicDataNB["DicDataB"];
                dsDataNSum = (DataSet)dicDataNB["DsDataNSum"];
                dsDataBSum = (DataSet)dicDataNB["DsDataBSum"];
            }

            Session[DataNT01ID + "dicDataN"] = DicDataN;
            Session[DataNT01ID + "dicDataB"] = DicDataB;
            Session[DataNT01ID + "dsDataNSum"] = dsDataNSum;
            Session[DataNT01ID + "dsDataBSum"] = dsDataBSum;
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
        }

        #region Xml

        private static Dictionary<string, object> LoadFromXml(string strHtmlDirectory, string strFileTPxml)
        {
            Dictionary<string, object> dicCompares = new Dictionary<string, object>();
            #region File path and name
            string strFilePath = System.IO.Path.Combine(strHtmlDirectory, strFileTPxml);
            #endregion File path and name
            XDocument xDoc = XDocument.Load(strFilePath);
            foreach (var Compares in xDoc.Root.Element("Dictioinary").Elements("Compares"))
            {
                if (Compares.Attribute("ID").Value == "DsDataNSum" || Compares.Attribute("ID").Value == "DsDataBSum")
                {
                    foreach (var Element in Compares.Elements("DataSet"))
                    {
                        dicCompares.Add(Compares.Attribute("ID").Value, CglFunc.XmlToDataSet(Element));
                    }
                }
                else
                {
                    Dictionary<string, object> dicElement = new Dictionary<string, object>();
                    foreach (var Element in Compares.Elements("Element"))
                    {
                        Dictionary<string, object> dicElement01 = new Dictionary<string, object>();
                        foreach (var Element01 in Element.Elements("Element01"))
                        {
                            Dictionary<string, DataSet> dicElement02 = new Dictionary<string, DataSet>();
                            foreach (var Element02 in Element01.Elements("Element02"))
                            {
                                dicElement02.Add(Element02.Attribute("ID").Value, CglFunc.XmlToDataSet(Element02.Element("DataSet")));
                            }
                            dicElement01.Add(Element01.Attribute("ID").Value, dicElement02);
                        }
                        dicElement.Add(Element.Attribute("ID").Value, dicElement01);
                    }
                    dicCompares.Add(Compares.Attribute("ID").Value, dicElement);
                }
            }
            return dicCompares;
        }

        private void SaveToXml(Dictionary<string, object> DicInput, string strXmlDirectory, string strXmlFileName)
        {
            CheckData = true;
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
            xDoc.AddFirst(new XElement("FileName", new XAttribute("Name", strXmlFileName)));
            XElement xItem = ToXML(DicInput);
            xDoc.Element("FileName").Add(xItem);
            xDoc.Save(System.IO.Path.Combine(strXmlDirectory, strXmlFileName));
        }
        private XElement ToXML(Dictionary<string, object> dicInput)
        {
            CheckData = true;
            XElement xRoot = new XElement("Dictioinary");
            foreach (var KeyVal in dicInput)
            {
                XElement xCompares = new XElement("Compares", new XAttribute("ID", KeyVal.Key));
                if (KeyVal.Key == "DsDataNSum" || KeyVal.Key == "DsDataBSum")
                {
                    xCompares.Add(CglFunc.DataSetToXml((DataSet)KeyVal.Value));
                }
                else
                {
                    Dictionary<string, object> dicDataNB00 = (Dictionary<string, object>)KeyVal.Value;
                    foreach (KeyValuePair<string, object> KeyVal01 in dicDataNB00)
                    {
                        XElement xElement = new XElement("Element", new XAttribute("ID", KeyVal01.Key));
                        foreach (KeyValuePair<string, object> KeyVal02 in (Dictionary<string, object>)KeyVal01.Value)
                        {
                            XElement Element01 = new XElement("Element01", new XAttribute("ID", KeyVal02.Key));
                            foreach (var KeyVal03 in (Dictionary<string, DataSet>)KeyVal02.Value)
                            {
                                XElement Element02 = new XElement("Element02", new XAttribute("ID", KeyVal03.Key));
                                Element02.Add(CglFunc.DataSetToXml(KeyVal03.Value));
                                Element01.Add(Element02);
                                xElement.Add(Element01);
                            }
                        }
                        xCompares.Add(xElement);
                    }
                }
                xRoot.Add(xCompares);
            }
            return xRoot;
        }

        #endregion Xml

        private string CheckSameColumn(List<string> lstMin, List<string> lstMax)
        {
            string strReturn = string.Empty;
            for (int i = 0; i < lstMin.Count; i++)
            {
                if (!string.IsNullOrEmpty(Compare(lstMin[i], lstMax[i])))
                {
                    strReturn += "," + Compare(lstMin[i], lstMax[i]);
                }
            }
            List<string> lstRetun = strReturn.Split(',').ToList();
            lstRetun.Sort();
            return lstRetun.Count > 0 ? string.Join(",", lstRetun.Distinct().ToArray()).Trim(',') : string.Empty;
        }

        private string CheckSideColumn(List<string> lstMin, List<string> lstMax)
        {
            string strReturn = string.Empty;
            for (int i = 0; i < lstMin.Count; i++)
            {
                int indexMin = i + 1 < lstMin.Count ? i + 1 : i + 1 - lstMin.Count;
                if (!string.IsNullOrEmpty(Compare(lstMin[indexMin], lstMax[i])))
                {
                    strReturn += "," + Compare(lstMin[indexMin], lstMax[i]);
                }
            }
            List<string> lstRetun = strReturn.Split(',').ToList();
            lstRetun.Sort();
            return lstRetun.Count > 0 ? string.Join(",", lstRetun.Distinct().ToArray()).Trim(',') : string.Empty;
        }

        private string Compare(string strNext, string strBalance)
        {
            CheckData = true;
            if (!string.IsNullOrEmpty(strNext) && !string.IsNullOrEmpty(strBalance))
            {
                List<string> lstNext = strNext.Split(',').ToList();
                List<string> lstBalance = strBalance.Split(',').ToList();
                List<int> lstIntNext = new List<int>();
                foreach (string NumNext in lstNext) { lstIntNext.Add(int.Parse(NumNext, InvariantCulture)); }
                List<int> lstIntBalance = new List<int>();
                foreach (string NumBalance in lstBalance) { lstIntBalance.Add(int.Parse(NumBalance, InvariantCulture)); }
                List<string> Compare = new List<string>();
                foreach (int NumNext in lstIntNext)
                {
                    if (lstIntBalance.Contains(NumNext)) { Compare.Add(string.Format(InvariantCulture, "{0:d2}", NumNext)); }
                }
                if (Compare.Count > 0)
                {
                    Compare.Sort();
                    return string.Join(",", Compare.Distinct().ToArray());
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        private DataTable CountNums(DataTable dtCompare)
        {
            Dictionary<string, int> dicNums = new Dictionary<string, int>();
            for (int index = 1; index <= new CglDataSet(GlobalStuSearch.LottoType).LottoNumbers; index++) { dicNums.Add(string.Format(InvariantCulture, "{0:d2}", index), 0); }
            foreach (DataRow dr in dtCompare.Rows)
            {
                foreach (DataColumn dc in dtCompare.Columns)
                {
                    if (!string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                    {
                        foreach (string v in dr[dc.ColumnName].ToString().Split(','))
                        {
                            if (dicNums.ContainsKey(int.Parse(v, InvariantCulture).ToString("d2", InvariantCulture)))
                            {
                                dicNums[int.Parse(v, InvariantCulture).ToString("d2", InvariantCulture)]++;
                            }
                            else
                            {
                                dicNums.Add(int.Parse(v, InvariantCulture).ToString("d2", InvariantCulture), 1);
                            }
                        }
                    }
                }
            }

            //dicNums = dicNums.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            Dictionary<string, int> dicNumsSort = dicNums.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            return new CglFunc().CDicTOTable(dicNumsSort, null);

        }

        private DataTable CountNums(List<string> lstInput)
        {
            Dictionary<string, int> dicNums = new Dictionary<string, int>();
            for (int index = 1; index <= new CglDataSet(GlobalStuSearch.LottoType).LottoNumbers; index++) { dicNums.Add(string.Format(InvariantCulture, "{0:d2}", index), 0); }
            foreach (string Nums in lstInput)
            {
                if (!string.IsNullOrEmpty(Nums))
                {
                    foreach (string num in Nums.Split(','))
                    {
                        if (dicNums.ContainsKey(int.Parse(num, InvariantCulture).ToString("d2", InvariantCulture)))
                        {
                            dicNums[int.Parse(num, InvariantCulture).ToString("d2", InvariantCulture)]++;
                        }
                        else
                        {
                            dicNums.Add(int.Parse(num, InvariantCulture).ToString("d2", InvariantCulture), 1);
                        }
                    }
                }
            }
            //dicNums = dicNums.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            Dictionary<string, int> dicNumsSort = dicNums.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            return new CglFunc().CDicTOTable(dicNumsSort, null);
        }

        private static List<string> CombindList(List<string> lstMin, List<string> lstMax)
        {
            List<string> lstCombind = new List<string>();
            for (int i = 0; i < lstMin.Count; i++)
            {
                if (!string.IsNullOrEmpty(lstMin[i]) && !string.IsNullOrEmpty(lstMax[i]))
                {
                    lstCombind.Add(lstMin[i] + "," + lstMax[i]);
                }
                if (string.IsNullOrEmpty(lstMin[i]) && string.IsNullOrEmpty(lstMax[i]))
                {
                    lstCombind.Add(string.Empty);
                }
                if (!string.IsNullOrEmpty(lstMin[i]) && string.IsNullOrEmpty(lstMax[i]))
                {
                    lstCombind.Add(lstMin[i]);
                }
                if (string.IsNullOrEmpty(lstMin[i]) && !string.IsNullOrEmpty(lstMax[i]))
                {
                    lstCombind.Add(lstMax[i]);
                }

            }
            return lstCombind;
        }

        private static object ConvertLst(List<int> lstInput)
        {
            lstInput.Sort();
            List<string> lstOutput = new List<string>();
            foreach (int item in lstInput.Distinct().ToList())
            {
                lstOutput.Add(string.Format(InvariantCulture, "{0:d2}", item));
            }
            return lstOutput;
        }

        private static object ConvertLst(List<string> lstInput)
        {
            List<int> lstOutput = new List<int>();
            foreach (string item in string.Join(",", lstInput.ToArray()).Split(',').ToList().Distinct().ToList())
            {
                if (!string.IsNullOrEmpty(item))
                {
                    lstOutput.Add(int.Parse(item, InvariantCulture));
                }
            }
            return lstOutput;
        }

        private static List<int> GetLstAvgSec(DataTable dtDataNNext, DataTable dtDataNGap, int num, int section, string NumColumnName)
        {
            List<int> lstAvgSec = new List<int>();
            using DataTable dtEachNumRange = new GalaxyApp().GetDataRange(dtDataNGap, "", num.ToString(InvariantCulture), "100");
            DataRow drEachNumRange = dtEachNumRange.Rows[0];
            foreach (DataRow drDataNNext in dtDataNNext.Select("", string.Format(InvariantCulture, "sglAvg{0} ASC", num)).CopyToDataTable().Rows)
            {
                string ColumnName = string.Format(InvariantCulture, "sglAvg{0}{1:d2}", num, section);
                double dblMinStdEvp = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}Min", ColumnName)].ToString(), InvariantCulture);
                double dblMaxStdEvp = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}Max", ColumnName)].ToString(), InvariantCulture);
                double dblMinGapStdEvp = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}GapMin", ColumnName)].ToString(), InvariantCulture);
                double dblMaxGapStdEvp = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}GapMax", ColumnName)].ToString(), InvariantCulture);

                if (CglFunc.Between(double.Parse(drDataNNext[ColumnName].ToString(), InvariantCulture), dblMinStdEvp, dblMaxStdEvp, true)
                 && CglFunc.Between(double.Parse(drDataNNext[ColumnName].ToString(), InvariantCulture), dblMinGapStdEvp, dblMaxGapStdEvp, true))
                {
                    lstAvgSec.Add(int.Parse(drDataNNext[string.Format(InvariantCulture, "{0}{1}", NumColumnName, num)].ToString(), InvariantCulture));
                }
            }
            return lstAvgSec;
        }

        private List<int> GetLstAvg(DataTable dtDataNNext, DataTable dtDataNGap, int num, string NumColumnName)
        {
            CheckData = true;
            List<int> lstAvg = new List<int>();
            using DataTable dtEachNumRange = new GalaxyApp().GetDataRange(dtDataNGap, "", num.ToString(InvariantCulture), "100");
            DataRow drEachNumRange = dtEachNumRange.Rows[0];
            foreach (DataRow drDataNNext in dtDataNNext.Select("", string.Format(InvariantCulture, "sglAvg{0} ASC", num)).CopyToDataTable().Rows)
            {
                string ColumnName = string.Format(InvariantCulture, "sglAvg{0}", num);
                double dblMinStdEvp = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}Min", ColumnName)].ToString(), InvariantCulture);
                double dblMaxStdEvp = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}Max", ColumnName)].ToString(), InvariantCulture);
                double dblMinGapStdEvp = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}GapMin", ColumnName)].ToString(), InvariantCulture);
                double dblMaxGapStdEvp = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}GapMax", ColumnName)].ToString(), InvariantCulture);

                if (CglFunc.Between(double.Parse(drDataNNext[ColumnName].ToString(), InvariantCulture), dblMinStdEvp, dblMaxStdEvp, true)
                 && CglFunc.Between(double.Parse(drDataNNext[ColumnName].ToString(), InvariantCulture), dblMinGapStdEvp, dblMaxGapStdEvp, true))
                {
                    lstAvg.Add(int.Parse(drDataNNext[string.Format(InvariantCulture, "{0}{1}", NumColumnName, num)].ToString(), InvariantCulture));
                }
            }
            return lstAvg;
        }

        private IList<int> GetLstStdEvp(DataTable dtDataNNext, DataTable dtDataNGap, int num, string NumColumnName)
        {
            CheckData = true;
            List<int> lstStdEvp = new List<int>();
            using DataTable dtEachNumRange = new GalaxyApp().GetDataRange(dtDataNGap, "", num.ToString(InvariantCulture), "100");
            DataRow drEachNumRange = dtEachNumRange.Rows[0];
            foreach (DataRow drDataNNext in dtDataNNext.Select("", string.Format(InvariantCulture, "sglAvg{0} ASC", num)).CopyToDataTable().Rows)
            {
                string ColumnName = string.Format(InvariantCulture, "sglStdEvp{0}", num);
                //double dblMinStdEvp = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}Min", ColumnName)].ToString(), InvariantCulture);
                //double dblMaxStdEvp = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}Max", ColumnName)].ToString(), InvariantCulture);
                double dblMinGapStdEvp = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}GapMin", ColumnName)].ToString(), InvariantCulture);
                double dblMaxGapStdEvp = double.Parse(drEachNumRange[string.Format(InvariantCulture, "{0}GapMax", ColumnName)].ToString(), InvariantCulture);
                if (double.Parse(drDataNNext[ColumnName].ToString(), InvariantCulture) <= double.Parse(dtDataNGap.Rows[0][ColumnName].ToString(), InvariantCulture)
                    || CglFunc.Between(double.Parse(drDataNNext[ColumnName].ToString(), InvariantCulture), dblMinGapStdEvp, dblMaxGapStdEvp, true))
                {
                    lstStdEvp.Add(int.Parse(drDataNNext[string.Format(InvariantCulture, "{0}{1}", NumColumnName, num)].ToString(), InvariantCulture));
                }
            }
            return lstStdEvp;
        }

        private DataTable CreatDataSum()
        {
            using DataTable dtReturn = new DataTable();
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "Field"),
                Caption = string.Format(InvariantCulture, "Field"),
                DataType = typeof(string),
                DefaultValue = string.Empty
            });
            for (int num = 1; num <= ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count; num++)
            {
                //LastNum{0}
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "LNum{0}", num),
                    Caption = string.Format(InvariantCulture, "LNum{0}", num),
                    DataType = typeof(int),
                    DefaultValue = 0
                });
                //sglAvg{0}
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "Avg{0}", num),
                    Caption = string.Format(InvariantCulture, "Avg{0}", num),
                    DataType = typeof(int),
                    DefaultValue = 0
                });

                //NumMin{0}
                dtReturn.Columns.Add(new DataColumn()
                {
                    ColumnName = string.Format(InvariantCulture, "NMin{0}", num),
                    Caption = string.Format(InvariantCulture, "NMin{0}", num),
                    DataType = typeof(int),
                    DefaultValue = 0
                });

                //NumMax{0}
                dtReturn.Columns.Add(new DataColumn()
                {
                    ColumnName = string.Format(InvariantCulture, "NMax{0}", num),
                    Caption = string.Format(InvariantCulture, "NMax{0}", num),
                    DataType = typeof(int),
                    DefaultValue = 0
                });
                //sglAvgMin{0}
                dtReturn.Columns.Add(new DataColumn()
                {
                    ColumnName = string.Format(InvariantCulture, "AMin{0}", num),
                    Caption = string.Format(InvariantCulture, "AMin{0}", num),
                    DataType = typeof(string),
                    DefaultValue = string.Empty
                });
                //sglSecMin{0}
                dtReturn.Columns.Add(new DataColumn()
                {
                    ColumnName = string.Format(InvariantCulture, "SMin{0}", num),
                    Caption = string.Format(InvariantCulture, "SMin{0}", num),
                    DataType = typeof(string),
                    DefaultValue = string.Empty
                });
                //sglAvgMax{0}
                using (DataColumn dcColumn = new DataColumn())
                {
                    dcColumn.ColumnName = string.Format(InvariantCulture, "AMax{0}", num);
                    dcColumn.DataType = typeof(string);
                    dcColumn.DefaultValue = string.Empty;
                    dcColumn.Caption = dcColumn.ColumnName;
                    dtReturn.Columns.Add(dcColumn);
                }
                //sglSecMax{0}
                dtReturn.Columns.Add(new DataColumn()
                {
                    ColumnName = string.Format(InvariantCulture, "SMax{0}", num),
                    Caption = string.Format(InvariantCulture, "SMax{0}", num),
                    DataType = typeof(string),
                    DefaultValue = string.Empty,
                });
            }
            return dtReturn.Copy();
        }

        private DataTable CreatDataRange()
        {
            using DataTable dtReturn = new DataTable();
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "Item"),
                Caption = string.Format(InvariantCulture, "Item"),
                DataType = typeof(string),
                DefaultValue = string.Empty
            });
            for (int num = 1; num <= ((List<int>)ViewState[DataNT01ID + "ListCurrentNumsN"]).Count; num++)
            {
                //RNum{0}
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "RNum{0}", num),
                    Caption = string.Format(InvariantCulture, "RNum{0}", num),
                    DataType = typeof(int),
                    DefaultValue = 0
                });

                //Nums{0}
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "Nums{0}", num),
                    Caption = string.Format(InvariantCulture, "Nums{0}", num),
                    DataType = typeof(string),
                    DefaultValue = string.Empty
                });
            }
            return dtReturn.Copy();
        }

        private static DataTable CreatCheckColumn()
        {
            using DataTable dtReturn = new DataTable { Locale = InvariantCulture, TableName = "dtCheckColumn" };
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "Item"),
                Caption = string.Format(InvariantCulture, "Item"),
                DataType = typeof(string),
                DefaultValue = string.Empty
            });
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "DataN"),
                Caption = string.Format(InvariantCulture, "DataN"),
                DataType = typeof(string),
                DefaultValue = string.Empty
            });
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "DataB"),
                Caption = string.Format(InvariantCulture, "DataB"),
                DataType = typeof(string),
                DefaultValue = string.Empty
            });
            return dtReturn.Copy();
        }

        private static DataTable CreatCompareTable()
        {
            using DataTable dtReturn = new DataTable { Locale = InvariantCulture, TableName = "dtCompare" };
            for (int i = 1; i <= 5; i++)
            {
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "Compare{0:d2}", i),
                    Caption = string.Format(InvariantCulture, "Compare{0:d2}", i),
                    DataType = typeof(string),
                    AllowDBNull = true
                });
            }
            return dtReturn.Copy();
        }
    }
}