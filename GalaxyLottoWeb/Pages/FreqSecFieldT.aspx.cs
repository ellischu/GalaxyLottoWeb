using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqSecFieldT : BasePage
    {
        private StuGLSearch _gstuSearch;

        //ID part
        private string _action;
        private string _requestId;
        private string FreqSecFieldTID;

        private Dictionary<string, string> _dicNumcssclass = new Dictionary<string, string>();
        private Dictionary<string, int> _dicCurrentNums;
        private bool _blLngM0;

        private DataTable DtFreqSum { get; set; }
        private DataTable DtSimilarity { get; set; }
        private DataTable DtLastNums { get; set; }
        private DataTable DtOverLoad { get; set; }
        private DataTable DtLastMissAll { get; set; }
        private DataTable DtLastMissAllSec { get; set; }
        private DataTable DtSimFreqSum { get; set; }
        private DataTable DtSimFreq { get; set; }
        private DataTable DtSimFreq01 { get; set; }
        private DataTable DtFreqSecField { get; set; }
        private DataTable DtFreqSec { get; set; }
        private DataTable DtMissingTotal { get; set; }

        private Dictionary<string, int> dicSectionLimit = new Dictionary<string, int>();
        private Dictionary<string, List<int>> _dicLastNums = new Dictionary<string, List<int>>();

        //Tread part
        //Tread part
        private static Dictionary<string, object> DicThreadFreqSecFieldT
        {
            get
            {
                if (dicThreadFreqSecFieldT == null) { dicThreadFreqSecFieldT = new Dictionary<string, object>(); }
                return dicThreadFreqSecFieldT;
            }
            set => dicThreadFreqSecFieldT = value;
        }

        private static Dictionary<string, object> dicThreadFreqSecFieldT;

        private Thread Thread01, Thread02;

        /// <summary>
        /// string: Sum ,Fields 欄位
        ///<para> DataSet: dtFreqSec , dtFreqSecField</para>
        /// </summary>
        private Dictionary<string, DataSet> DicFreqSecField { get; set; } = new Dictionary<string, DataSet>();

        private string LocalBrowserType { get; set; }

        private string LocalIP { get; set; }

        //private string KeySearchOrder { get; set; }


#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            if (ViewState[FreqSecFieldTID + "_gstuSearch"] == null || ((StuGLSearch)ViewState[FreqSecFieldTID + "_gstuSearch"]).StrSecField == "none")
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                LocalBrowserType = Request.Browser.Type;
                LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
                //KeySearchOrder = string.Format(InvariantCulture, "{0}#{1}#dtSearchOrder", LocalIP, LocalBrowserType);

                _gstuSearch = (StuGLSearch)ViewState[FreqSecFieldTID + "_gstuSearch"];
                InitialArgument();
                ShowResult();
            }
        }

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            FreqSecFieldTID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqSecFieldTID] != null)
            {
                if (ViewState[FreqSecFieldTID + "_gstuSearch"] == null)
                {
                    ViewState.Add(FreqSecFieldTID + "_gstuSearch", (StuGLSearch)Session[FreqSecFieldTID]);
                }
            }
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        private void InitialArgument()
        {
            if (ViewState["CurrentNums"] == null) { ViewState.Add("CurrentNums", new CglData().GetDataNumsDici(_gstuSearch)); }
            _dicCurrentNums = (Dictionary<string, int>)ViewState["CurrentNums"];

            if (ViewState["_dicNumcssclass"] == null) { ViewState.Add("_dicNumcssclass", new GalaxyApp().GetNumcssclass(_gstuSearch)); }
            _dicNumcssclass = (Dictionary<string, string>)ViewState["_dicNumcssclass"];

            if (Session[FreqSecFieldTID + "lblT01"] == null) { Session.Add(FreqSecFieldTID + "lblT01", ""); }
            if (Session[FreqSecFieldTID + "dicFreqSecField"] != null) { ViewState[FreqSecFieldTID + "dicFreqSecField"] = Session[FreqSecFieldTID + "dicFreqSecField"]; }
            if (Session[FreqSecFieldTID + "ddlFreq"] != null) { ViewState[FreqSecFieldTID + "ddlFreq"] = Session[FreqSecFieldTID + "ddlFreq"]; }
            if (Session[FreqSecFieldTID + "LastNumsWithSectiont"] != null) { ViewState[FreqSecFieldTID + "LastNumsWithSectiont"] = Session[FreqSecFieldTID + "LastNumsWithSectiont"]; }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowResult()
        {
            ShowTitle();
            ShowddlFreq();
            ShowData(ddlFreq.SelectedValue);
        }

        private void ShowTitle()
        {
            if (ViewState["title"] == null) { ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "數字區間總表", new CglDBData().SetTitleString(_gstuSearch))); }
            Page.Title = (string)ViewState["title"];
            lblTitle.Text = (string)ViewState["title"];

            if (ViewState["lblMethod"] == null) { ViewState.Add("lblMethod", new CglMethod().SetMethodString(_gstuSearch)); }
            lblMethod.Text = (string)ViewState["lblMethod"];

            if (ViewState["lblSearchMethod"] == null) { ViewState.Add("lblSearchMethod", new CglMethod().SetSearchMethodString(_gstuSearch)); }
            lblSearchMethod.Text = (string)ViewState["lblSearchMethod"];

            //顯示當前資料
            if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch))); }
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowddlFreq()
        {
            if (ViewState[FreqSecFieldTID + "ddlFreq"] != null)
            {
                if (ddlFreq.Items.Count == 0)
                {
                    foreach (string strItem in (List<string>)ViewState[FreqSecFieldTID + "ddlFreq"])
                    {
                        ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(strItem), strItem));
                    }
                }
                lblFreq.Text = string.Format(InvariantCulture, "{0}:{1}", ddlFreq.Items.Count - 1, ddlFreq.SelectedItem.Text);
            }
            else
            {
                if (!DicThreadFreqSecFieldT.Keys.Contains(FreqSecFieldTID + "T01")) { CreatThread(); }
            }
        }

        private List<string> GetddlFreq()
        {
            if (Session[FreqSecFieldTID + "ddlFreq"] == null)
            {
                List<string> Fields = (List<string>)new CglValidFields().GetValidFieldsLst(_gstuSearch);
                List<string> lstShowddlFields = new List<string> { "Sum" };
                foreach (string strItem in Fields) { lstShowddlFields.Add(strItem); }
                Session.Add(FreqSecFieldTID + "ddlFreq", lstShowddlFields);
                return lstShowddlFields;
            }
            else
            {
                return (List<string>)Session[FreqSecFieldTID + "ddlFreq"];
            }
        }
        // ---------------------------------------------------------------------------------------------------------
        private void ShowData(string selectedValue)
        {
            pnlSum.Visible = false;
            pnlFreqSecField.Visible = false;
            switch (selectedValue)
            {
                case "Sum":
                    ShowSumData();
                    break;
                default:
                    ShowFreqSecField(ddlFreq.SelectedValue);
                    break;
            }
        }
        // ---------------------------------------------------------------------------------------------------------
        private void ShowSumData()
        {
            if (ViewState[FreqSecFieldTID + "dicFreqSecField"] != null && ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"]).ContainsKey("Sum"))
            {
                pnlSum.Visible = true;
                ShowSum();
                ShowSimilarity();
                ShowLastNum();
                ShowOverLoad();
                ShowLastMissAll();
                ShowLastMissAllSec();
                ShowSimFreq();
                ShowSimFreq01();
                ShowSimFreqSum();
            }

        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowSum()
        {
            lblSum.Text = string.Format(InvariantCulture, " 總表 數字區間驗證期數: {0} ", _gstuSearch.InTargetTestPeriods);
            DtFreqSum = ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"])["Sum"].Tables["dtSum"].Select("", "[dblSum] DESC").CopyToDataTable();
            gvSum.DataSource = DtFreqSum.DefaultView;
            if (gvSum.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtFreqSum.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtFreqSum.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtFreqSum.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtFreqSum.Columns[ColumnIndex].ColumnName,
                    };
                    gvSum.Columns.Add(bfCell);
                }
            }
            gvSum.RowDataBound += GvSum_RowDataBound;
            gvSum.DataBind();
        }
        private void GvSum_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        #region Set lngN
                        if (strCell_ColumnName == "lngN" && _dicNumcssclass.ContainsKey(cell.Text))
                        {
                            cell.CssClass = _dicNumcssclass[cell.Text];
                        }
                        #endregion Set lngN
                    }
                }
            }
        }
        // ---------------------------------------------------------------------------------------------------------
        private void ShowSimilarity()
        {
            DtSimilarity = ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"])["Sum"].Tables["dtSimilarity"].Select("", "[intSimilar] DESC , [intSimilarDetail] ASC , [lngTotalSN01] DESC ").CopyToDataTable();
            gvSimilarity.DataSource = DtSimilarity.DefaultView;
            if (gvSimilarity.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtSimilarity.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtSimilarity.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtSimilarity.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtSimilarity.Columns[ColumnIndex].ColumnName,
                    };
                    gvSimilarity.Columns.Add(bfCell);
                }
            }
            gvSimilarity.RowDataBound += GvSimilarity_RowDataBound;
            gvSimilarity.DataBind();

            btnquickSearchOrder.Visible = true;
            btnShowSearchOrder.Visible = true;
        }
        private void GvSimilarity_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        #region Set lngN
                        if (strCell_ColumnName.Substring(0, 4) == "lngL")
                        {
                            if (_dicNumcssclass.ContainsKey(cell.Text))
                            {
                                cell.CssClass = _dicNumcssclass[cell.Text];
                            }
                        }
                        #endregion Set lngN
                    }
                }
            }

        }
        // ---------------------------------------------------------------------------------------------------------

        private void ShowLastNum()
        {
            DtLastNums = ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"])["Sum"].Tables["dtLastNums"];
            gvLastNum.DataSource = DtLastNums.DefaultView;
            if (gvLastNum.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtLastNums.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtLastNums.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtLastNums.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtLastNums.Columns[ColumnIndex].ColumnName,
                    };
                    gvLastNum.Columns.Add(bfCell);
                }
            }
            gvLastNum.RowDataBound += GvLastNum_RowDataBound;
            gvLastNum.DataBind();
        }

        private void GvLastNum_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        #region Set lngN
                        if (strCell_ColumnName.Substring(0, 4) == "lngN")
                        {
                            if (_dicNumcssclass.ContainsKey(cell.Text))
                            {
                                cell.CssClass = _dicNumcssclass[cell.Text];
                            }
                        }
                        #endregion Set lngN

                        #region Set strField
                        if (strCell_ColumnName == "strField")
                        {
                            cell.Text = new CglFunc().ConvertFieldNameId(cell.Text, 1);
                        }
                        #endregion Set strField
                    }
                }
            }

        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowOverLoad()
        {
            DtOverLoad = ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"])["Sum"].Tables["dtOverLoad"];
            gvOverLoad.DataSource = DtOverLoad.DefaultView;
            if (gvOverLoad.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtOverLoad.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtOverLoad.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtOverLoad.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtOverLoad.Columns[ColumnIndex].ColumnName,
                    };
                    bfCell.HeaderStyle.CssClass = bfCell.DataField;
                    bfCell.ItemStyle.CssClass = bfCell.DataField;
                    gvOverLoad.Columns.Add(bfCell);
                }
            }
            gvOverLoad.RowDataBound += GvOverLoad_RowDataBound; ;
            gvOverLoad.DataBind();
        }

        private void GvOverLoad_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Dictionary<string, DataControlFieldCell> dicCells = new Dictionary<string, DataControlFieldCell>();

                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        dicCells.Add(strCell_ColumnName, cell);
                    }
                }
                Dictionary<string, int> dicSectionLimit00 = GetSectionLimit(((Dictionary<string, DataSet>)base.ViewState[FreqSecFieldTID + "dicFreqSecField"])[dicCells["strField"].Text].Tables["dtFreqSec"]);
                Dictionary<string, List<int>> dicLastNums00 = (Dictionary<string, List<int>>)((Dictionary<string, object>)ViewState[FreqSecFieldTID + "LastNumsWithSectiont"])[dicCells["strField"].Text];
                #region Set lngN
                if (_dicNumcssclass.ContainsKey(dicCells["lngN"].Text))
                {
                    dicCells["lngN"].CssClass = _dicNumcssclass[dicCells["lngN"].Text] + " ";
                }
                #endregion Set lngN

                #region Set lngM
                if (int.Parse(dicCells["lngM"].Text, InvariantCulture) == 0)
                {
                    dicCells["lngM"].CssClass = " glValueActive ";
                }
                #endregion Set lngM

                #region Set sglFreq

                foreach (int section in new int[] { 5, 10, 25, 50, 100 })
                {
                    //#region sglAC
                    //string strCell_ColumnNameAC = string.Format(InvariantCulture, "sglAC{0:d2}", section);
                    //if (double.Parse(dicCells[strCell_ColumnNameAC].Text, InvariantCulture) < 0)
                    //{
                    //    dicCells[strCell_ColumnNameAC].CssClass = strCell_ColumnNameAC + " glValueMax ";
                    //}
                    //#endregion

                    string strCell_ColumnNameFreq = string.Format(InvariantCulture, "sglFreq{0:d2}", section);
                    #region sglFreq
                    if (int.Parse(dicCells[strCell_ColumnNameFreq].Text, InvariantCulture) == int.Parse(dicCells[string.Format(InvariantCulture, "intMax{0:d2}", section)].Text, InvariantCulture))
                    {
                        if (int.Parse(dicCells[strCell_ColumnNameFreq].Text, InvariantCulture) >= dicSectionLimit00[strCell_ColumnNameFreq])
                        {
                            dicCells[strCell_ColumnNameFreq].CssClass = dicCells[strCell_ColumnNameFreq].CssClass + " glValueActive ";
                        }
                        else
                        {
                            dicCells[strCell_ColumnNameFreq].CssClass = dicCells[strCell_ColumnNameFreq].CssClass + " glValueSecondMaxNum ";
                        }
                    }
                    #endregion

                    #region intMin
                    string strCell_ColumnNameMin = string.Format(InvariantCulture, "intMax{0:d2}", section);
                    if (dicLastNums00[string.Format(InvariantCulture, "{0:d2}", section)].Contains(int.Parse(dicCells["lngN"].Text, InvariantCulture)))
                    {
                        dicCells[strCell_ColumnNameMin].CssClass = strCell_ColumnNameMin + " glSectionNumLast ";
                    }
                    #endregion
                }
                #endregion Set sglFreq

                dicCells["strField"].Text = new CglFunc().ConvertFieldNameId(dicCells["strField"].Text, 1);
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowLastMissAll()
        {
            DtLastMissAll = ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"])["Sum"].Tables["dtLastMissAll"];
            #region Convert 0 to -1 to -7
            for (int intRow = 0; intRow < DtLastMissAll.Rows.Count; intRow++)
            {
                int intIndexofZero = -1;
                for (int intNum = 1; intNum <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; intNum++)
                {
                    string strColName = string.Format(InvariantCulture, "lngM{0}", intNum);
                    if (int.Parse(DtLastMissAll.Rows[intRow][strColName].ToString(), InvariantCulture) == 0)
                    {
                        DtLastMissAll.Rows[intRow][strColName] = intIndexofZero;
                        intIndexofZero--;
                    }
                }
            }
            #endregion

            gvLastMissAll.DataSource = DtLastMissAll.DefaultView;

            if (gvLastMissAll.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtLastMissAll.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtLastMissAll.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtLastMissAll.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtLastMissAll.Columns[ColumnIndex].ColumnName,
                    };
                    gvLastMissAll.Columns.Add(bfCell);
                }
            }


            if (gvLastMissAll.Columns.Count != 0)
            {
                int intlngm = 1;
                foreach (BoundField bfCell in gvLastMissAll.Columns)
                {
                    string strColumnName = bfCell.DataField;

                    #region Show the number of lngL,lngS,lngM with 2 digital
                    if (strColumnName.Substring(0, 4) == "lngM" && strColumnName != "lngMethodSN")
                    {
                        bfCell.DataFormatString = "{0:d2}";
                        if (_dicCurrentNums.Values.Contains(int.Parse(strColumnName.Substring(4), InvariantCulture)))
                        {
                            bfCell.HeaderStyle.CssClass = string.Format(InvariantCulture, "lngM{0}", intlngm);
                            intlngm++;
                        }
                        int intNum = int.Parse(strColumnName.Substring(4), InvariantCulture);
                        if (intNum % 5 == 0 && strColumnName.Substring(0, 4) == "lngM")
                        {
                            bfCell.ItemStyle.CssClass = "glColNum5";
                        }
                    }
                    #endregion
                }
            }
            gvLastMissAll.RowDataBound += GvLastMissAll_RowDataBound; ;

            //gvLastMissAll.RowDataBound += GvLastNum_RowDataBound;
            gvLastMissAll.DataBind();
        }

        private void GvLastMissAll_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        #region Set lngM
                        if (strCell_ColumnName.Substring(0, 4) == "lngM" && strCell_ColumnName != "lngMethodSN")
                        {
                            int intColumnValue = int.Parse(strCell_ColumnName.Substring(4), InvariantCulture);
                            int intValue = int.Parse(cell.Text, InvariantCulture);
                            if (intValue < 0)
                            {
                                if (Math.Abs(intValue) == 1) { _blLngM0 = true; }
                                if (Math.Abs(intValue) == new CglDataSet(_gstuSearch.LottoType).CountNumber) { _blLngM0 = false; }
                                cell.CssClass = (intColumnValue % 5 != 0) ? string.Format(InvariantCulture, "lngM{0}", Math.Abs(intValue)) : string.Format(InvariantCulture, "lngM{0} glColNum5", Math.Abs(intValue));
                            }
                            else
                            {
                                if (_blLngM0)
                                {
                                    cell.CssClass = (intColumnValue % 5 != 0) ? string.Format(InvariantCulture, "lngM{0}", 0) : cell.CssClass + string.Format(InvariantCulture, "lngM{0} glColNum5", 0);
                                }
                            }
                        }
                        #endregion Set lngM

                        #region Set strField
                        if (strCell_ColumnName == "strField")
                        {
                            cell.Text = new CglFunc().ConvertFieldNameId(cell.Text, 1);
                        }
                        #endregion Set strField
                    }
                }
            }

        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowLastMissAllSec()
        {
            DtLastMissAllSec = ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"])["Sum"].Tables["dtMissAllSec"];
            gvMissAllSec.DataSource = DtLastMissAllSec.DefaultView;
            if (gvMissAllSec.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtLastMissAllSec.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtLastMissAllSec.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtLastMissAllSec.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtLastMissAllSec.Columns[ColumnIndex].ColumnName,
                    };
                    gvMissAllSec.Columns.Add(bfCell);
                }
            }


            if (gvMissAllSec.Columns.Count != 0)
            {
                int intlngm = 1;
                foreach (BoundField bfCell in gvMissAllSec.Columns)
                {
                    string strColumnName = bfCell.DataField;

                    #region Show the number of lngL,lngS,lngM with 2 digital
                    if (strColumnName.Substring(0, 4) == "lngM" && strColumnName != "lngMethodSN")
                    {
                        if (_dicCurrentNums.Values.Contains(int.Parse(strColumnName.Substring(4), InvariantCulture)))
                        {
                            bfCell.HeaderStyle.CssClass = _dicNumcssclass[strColumnName.Substring(4)];
                            //string.Format(InvariantCulture, "lngM{0}", intlngm);
                            bfCell.ItemStyle.CssClass = "glColNum5";
                            intlngm++;
                        }
                    }
                    #endregion
                }
            }
            gvMissAllSec.RowDataBound += GvMissAllSec_RowDataBound; ;

            gvMissAllSec.DataBind();
        }

        private void GvMissAllSec_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                bool checkMissAll = false;
                bool SecCheck01 = false;
                bool SecCheck10 = false;
                bool SecCheck20 = false;
                bool SecCheck25 = false;
                bool SecCheck30 = false;

                //bool checkFreqSecSum = false;
                //Dictionary<string, int> dicCriticle = new Dictionary<string, int>() { { "01", 5 }, { "10", 3 }, { "20", 2 }, { "25", 0 }, { "30", 0 } };
                //string strSec = string.Empty;
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        #region Sec01
                        if (strCell_ColumnName == "strSetion" && cell.Text == "01") { SecCheck01 = true; }
                        if (SecCheck01 && strCell_ColumnName != "strSetion" && int.Parse(cell.Text, InvariantCulture) < 5) { cell.CssClass = "glValueActive"; }
                        #endregion Sec01

                        #region Sec10
                        if (strCell_ColumnName == "strSetion" && cell.Text == "10") { SecCheck10 = true; }
                        if (SecCheck10 && strCell_ColumnName != "strSetion" && int.Parse(cell.Text, InvariantCulture) > 3) { cell.CssClass = "glValueActive"; }
                        #endregion Sec10

                        #region Sec20
                        if (strCell_ColumnName == "strSetion" && cell.Text == "20") { SecCheck20 = true; }
                        if (SecCheck20 && strCell_ColumnName != "strSetion" && int.Parse(cell.Text, InvariantCulture) > 2) { cell.CssClass = "glValueActive"; }
                        #endregion Sec20

                        #region Sec25
                        if (strCell_ColumnName == "strSetion" && cell.Text == "25") { SecCheck25 = true; }
                        if (SecCheck25 && strCell_ColumnName != "strSetion" && int.Parse(cell.Text, InvariantCulture) > DtLastMissAll.Rows.Count - 8) { cell.CssClass = "glValueActive"; }
                        #endregion Sec25

                        #region Sec2530
                        if (strCell_ColumnName == "strSetion" && cell.Text == "30") { SecCheck30 = true; }
                        if (SecCheck30 && strCell_ColumnName != "strSetion" && int.Parse(cell.Text, InvariantCulture) > 0) { cell.CssClass = "glValueActive"; }
                        #endregion Sec2530

                        #region Set LastMissAll
                        if (strCell_ColumnName == "strSetion" && cell.Text == string.Format(InvariantCulture, "LastMissAll({0})", DtLastMissAll.Rows.Count))
                        {
                            checkMissAll = true;
                        }
                        else
                        {
                            if (checkMissAll && !CglFunc.Between(int.Parse(cell.Text, InvariantCulture), 30, 90, false))
                            {
                                //(int.Parse(cell.Text, InvariantCulture) < 30 || int.Parse(cell.Text, InvariantCulture) > 90))
                                cell.CssClass = "glValueActive";
                            }
                        }
                        #endregion Set LastMissAll                                               

                        #region Set strSetion
                        //if (strCell_ColumnName == "strSetion" && new string[] { "01", "10", "20", "25", "30" }.Contains(cell.Text))
                        //{
                        //    strSec = cell.Text;
                        //    Check = true;
                        //}
                        //else
                        //{
                        //    if (Check)
                        //    {
                        //        if (strSec == "01" && int.Parse(cell.Text, InvariantCulture) < dicCriticle[strSec])
                        //        {
                        //            cell.CssClass = "glValueActive";
                        //        }
                        //        if (strSec != "01" && int.Parse(cell.Text, InvariantCulture) > dtLastMissAll.Rows.Count - dicCriticle[strSec])
                        //        {
                        //            cell.CssClass = "glValueActive";
                        //        }
                        //    }
                        //}
                        #endregion Set strSetion     

                        #region Set FreqSecSum
                        //if (strCell_ColumnName == "strSetion" && cell.Text == "FreqSecSum")
                        //{
                        //    checkFreqSecSum = true;
                        //}
                        //else
                        //{
                        //    if (checkFreqSecSum && int.Parse(cell.Text, InvariantCulture) == 0)
                        //    {
                        //        cell.CssClass = "glValueActive";
                        //    }
                        //}
                        #endregion Set FreqSecSum                                               



                    }
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowSimFreq()
        {
            DtSimFreq = ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"])["Sum"].Tables["dtSimFreq"];
            gvSimFreq.DataSource = DtSimFreq.DefaultView;
            if (gvSimFreq.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtSimFreq.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtSimFreq.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtSimFreq.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtSimFreq.Columns[ColumnIndex].ColumnName,
                    };
                    gvSimFreq.Columns.Add(bfCell);
                }
            }
            foreach (DataControlField dcColumn in gvSimFreq.Columns)
            {
                string strColumnName = dcColumn.SortExpression;
                if (_dicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                {
                    dcColumn.HeaderStyle.CssClass = _dicNumcssclass[strColumnName.Substring(4)];
                }
            }

            gvSimFreq.DataBind();
        }

        private void ShowSimFreq01()
        {
            DtSimFreq01 = ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"])["Sum"].Tables["dtSimFreq01"];
            gvSimFreq01.DataSource = DtSimFreq01.DefaultView;
            if (gvSimFreq01.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtSimFreq01.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtSimFreq01.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtSimFreq01.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtSimFreq01.Columns[ColumnIndex].ColumnName,
                    };
                    gvSimFreq01.Columns.Add(bfCell);
                }
            }
            foreach (DataControlField dcColumn in gvSimFreq01.Columns)
            {
                string strColumnName = dcColumn.SortExpression;
                if (_dicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                {
                    dcColumn.HeaderStyle.CssClass = _dicNumcssclass[strColumnName.Substring(4)];
                }
            }

            gvSimFreq01.DataBind();
        }

        private void ShowSimFreqSum()
        {
            DtSimFreqSum = ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"])["Sum"].Tables["dtSimFreqSum"];
            gvSimFreqSum.DataSource = DtSimFreqSum.DefaultView;
            if (gvSimFreqSum.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtSimFreqSum.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtSimFreqSum.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtSimFreqSum.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtSimFreqSum.Columns[ColumnIndex].ColumnName,
                    };
                    gvSimFreqSum.Columns.Add(bfCell);
                }
            }
            foreach (DataControlField dcColumn in gvSimFreqSum.Columns)
            {
                string strColumnName = dcColumn.SortExpression;
                if (_dicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                {
                    dcColumn.HeaderStyle.CssClass = _dicNumcssclass[strColumnName.Substring(4)];
                    dcColumn.ItemStyle.CssClass = "glColNum5";
                }
            }
            gvSimFreqSum.RowDataBound += GvSimFreqSum_RowDataBound;
            gvSimFreqSum.DataBind();
        }

        private void GvSimFreqSum_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField)
                    {
                        //string strCell_ColumnName = ((BoundField)cell.ContainingField).DataField;

                    }
                }
            }

        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowFreqSecField(string selectedValue)
        {
            if (ViewState[FreqSecFieldTID + "dicFreqSecField"] != null && ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"]).ContainsKey(selectedValue))
            {
                pnlFreqSecField.Visible = true;
                lblFreqSecField.Text = string.Format(InvariantCulture, " 驗證期數: {0} ", _gstuSearch.InTargetTestPeriods);
                DtFreqSecField = ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"])[selectedValue].Tables["dtFreqSecField"].Select("", "[intHistoryHitRate] DESC ,[intCorrespondPeriods] DESC").CopyToDataTable();
                DtFreqSec = ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"])[selectedValue].Tables["dtFreqSec"];
                _dicLastNums = (Dictionary<string, List<int>>)((Dictionary<string, object>)ViewState[FreqSecFieldTID + "LastNumsWithSectiont"])[selectedValue];


                DtFreqSecField.Columns.Remove("lngTotalSN");
                gvFreqSecField.DataSource = DtFreqSecField.DefaultView;
                if (gvFreqSecField.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < DtFreqSecField.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqSecField.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqSecField.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = DtFreqSecField.Columns[ColumnIndex].ColumnName,
                        };
                        gvFreqSecField.Columns.Add(bfCell);
                    }
                }
                gvFreqSecField.RowDataBound += GvFreqSecField_RowDataBound;
                gvFreqSecField.DataBind();
                ShowFreqSec(selectedValue);
                ShowMissAll(selectedValue);
            }
        }

        private void GvFreqSecField_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                long lngFreqSecFieldSN = 0;

                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        #region Set lngFreqSecFieldSN
                        if (strCell_ColumnName == "lngFreqSecFieldSN")
                        {
                            lngFreqSecFieldSN = long.Parse(cell.Text, InvariantCulture);
                        }
                        #endregion Set lngFreqSecFieldSN

                        #region Set lngSecFieldSN
                        if (strCell_ColumnName == "lngSecFieldSN")
                        {
                            cell.ToolTip = _gstuSearch.StrSecField;
                            //  e.Row.CssClass = e.Row.CssClass + " glRowSearchLimit";
                        }
                        #endregion Set lngSecFieldSN

                        #region Set lngN
                        if (strCell_ColumnName == "lngN")
                        {
                            List<int> lstField = new List<int>();
                            foreach (string strFieldItem in _gstuSearch.StrSecField.Split('#').ToList())
                            {
                                lstField.Add(int.Parse(
                                             DtFreqSec.Select(string.Format(InvariantCulture, "[lngN] = {0}",
                                             int.Parse(cell.Text, InvariantCulture)), "").CopyToDataTable().Rows[0][strFieldItem].ToString(), InvariantCulture));
                            }
                            cell.ToolTip = string.Join("#", lstField);
                            if (_dicNumcssclass.ContainsKey(cell.Text))
                            {
                                cell.CssClass = _dicNumcssclass[cell.Text];
                            }
                        }
                        #endregion Set lngN

                        #region Set intCorrespondPeriods
                        if (strCell_ColumnName == "intCorrespondPeriods" && lngFreqSecFieldSN != 0)
                        {
                            DtFreqSecField.PrimaryKey = new DataColumn[] { DtFreqSecField.Columns["lngFreqSecFieldSN"] };
                            DataRow drFreqSecField = DtFreqSecField.Rows.Find(lngFreqSecFieldSN);
                            if (int.Parse(drFreqSecField["intCorrespondPeriods"].ToString(), InvariantCulture) > 0 && drFreqSecField["intCorrespondPeriods"].ToString() == drFreqSecField["intCorrespondHistoryPeriods"].ToString())
                            {
                                cell.CssClass = " glValueActive ";
                            }
                        }
                        #endregion Set intCorrespondPeriods

                    }
                }
            }
        }

        private void ShowFreqSec(string selectedValue)
        {
            if (ViewState[FreqSecFieldTID + "dicFreqSecField"] != null && ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"]).ContainsKey(selectedValue))
            {
                //lblFreqSec.Text = string.Format(InvariantCulture, "數字區間表： {0} ", _gstuSearch.InFreqDnaLength);
                //dtFreqDna = ConvertDt(((Dictionary<string, DataSet>)Session[FreqDNAID + "dsFreqSecField"])[selectedValue].Tables["dtFreqDna"].Select("", "[lngN] ASC").CopyToDataTable());
                DtFreqSec = ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"])[selectedValue].Tables["dtFreqSec"].Select("", "[lngN] ASC").CopyToDataTable();
                dicSectionLimit = GetSectionLimit(DtFreqSec);
                if (DtFreqSec.Columns.Contains("lngTotalSN")) { DtFreqSec.Columns.Remove("lngTotalSN"); }
                if (DtFreqSec.Columns.Contains("lngMethodSN")) { DtFreqSec.Columns.Remove("lngMethodSN"); }
                //if (dtFreqSec.Columns.Contains("lngDateSN")) { dtFreqSec.Columns.Remove("lngDateSN"); }

                gvFreqSec.DataSource = DtFreqSec.DefaultView;
                if (gvFreqSec.Columns.Count == 0)
                {
                    for (int ColumnIndex = 0; ColumnIndex < DtFreqSec.Columns.Count; ColumnIndex++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtFreqSec.Columns[ColumnIndex].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtFreqSec.Columns[ColumnIndex].ColumnName, 1),
                            SortExpression = DtFreqSec.Columns[ColumnIndex].ColumnName,
                        };
                        bfCell.HeaderStyle.CssClass = bfCell.DataField;
                        bfCell.ItemStyle.CssClass = bfCell.DataField;
                        gvFreqSec.Columns.Add(bfCell);
                    }
                }

                gvFreqSec.RowDataBound += GvFreqSec_RowDataBound;
                gvFreqSec.DataBind();
            }
        }

        private void GvFreqSec_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Dictionary<string, DataControlFieldCell> dicCells = new Dictionary<string, DataControlFieldCell>();
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        dicCells.Add(strCell_ColumnName, cell);
                    }
                }

                #region Set lngN
                if (_dicNumcssclass.ContainsKey(dicCells["lngN"].Text))
                {
                    dicCells["lngN"].CssClass = _dicNumcssclass[dicCells["lngN"].Text] + " ";
                }
                #endregion Set lngN

                #region Set lngM
                if (int.Parse(dicCells["lngM"].Text, InvariantCulture) == 0)
                {
                    dicCells["lngM"].CssClass = " glValueActive ";
                }
                #endregion Set lngM

                #region Set sglFreq

                foreach (int section in new int[] { 5, 10, 25, 50, 100 })
                {
                    #region sglAC
                    string strCell_ColumnNameAC = string.Format(InvariantCulture, "sglAC{0:d2}", section);
                    if (double.Parse(dicCells[strCell_ColumnNameAC].Text, InvariantCulture) < 0)
                    {
                        dicCells[strCell_ColumnNameAC].CssClass = strCell_ColumnNameAC + " glValueMax ";
                    }
                    #endregion

                    string strCell_ColumnNameFreq = string.Format(InvariantCulture, "sglFreq{0:d2}", section);
                    #region sglFreq
                    dicCells[strCell_ColumnNameFreq].CssClass = strCell_ColumnNameFreq;
                    if (int.Parse(dicCells[strCell_ColumnNameFreq].Text, InvariantCulture) == int.Parse(dicCells[string.Format(InvariantCulture, "intMax{0:d2}", section)].Text, InvariantCulture)
                        && !_dicLastNums[string.Format(InvariantCulture, "{0:d2}", section)].Contains(int.Parse(dicCells["lngN"].Text, InvariantCulture)))
                    {
                        if (int.Parse(dicCells[strCell_ColumnNameFreq].Text, InvariantCulture) >= dicSectionLimit[strCell_ColumnNameFreq])
                        {
                            dicCells[strCell_ColumnNameFreq].CssClass = dicCells[strCell_ColumnNameFreq].CssClass + " glValueActive ";
                        }
                        else
                        {
                            dicCells[strCell_ColumnNameFreq].CssClass = dicCells[strCell_ColumnNameFreq].CssClass + " glValueSecondMaxNum ";
                        }
                    }
                    if (int.Parse(dicCells[strCell_ColumnNameFreq].Text, InvariantCulture) >= dicSectionLimit[strCell_ColumnNameFreq]
                        && !_dicLastNums[string.Format(InvariantCulture, "{0:d2}", section)].Contains(int.Parse(dicCells["lngN"].Text, InvariantCulture)))
                    {
                        dicCells[strCell_ColumnNameFreq].CssClass = dicCells[strCell_ColumnNameFreq].CssClass + " glValueCritical ";
                    }
                    #endregion

                    #region intMin
                    string strCell_ColumnNameMin = string.Format(InvariantCulture, "intMin{0:d2}", section);
                    if (_dicLastNums[string.Format(InvariantCulture, "{0:d2}", section)].Contains(int.Parse(dicCells["lngN"].Text, InvariantCulture)))
                    {
                        dicCells[strCell_ColumnNameMin].CssClass = strCell_ColumnNameMin + " glSectionNumLast ";
                    }
                    #endregion
                }
                #endregion Set sglFreq

            }
        }


        private void ShowMissAll(string selectedValue)
        {
            if (ViewState[FreqSecFieldTID + "dicFreqSecField"] != null && ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"]).ContainsKey(selectedValue))
            {
                //lblFreqSec.Text = string.Format(InvariantCulture, "數字區間表： {0} ", _gstuSearch.InFreqDnaLength);
                //dtFreqDna = ConvertDt(((Dictionary<string, DataSet>)Session[FreqDNAID + "dsFreqSecField"])[selectedValue].Tables["dtFreqDna"].Select("", "[lngN] ASC").CopyToDataTable());
                DtMissingTotal = ((Dictionary<string, DataSet>)ViewState[FreqSecFieldTID + "dicFreqSecField"])[selectedValue].Tables["dtMissAll"].Select("", "[lngTotalSN] DESC").CopyToDataTable();
                if (DtMissingTotal.Columns.Contains("lngTotalSN")) { DtMissingTotal.Columns.Remove("lngTotalSN"); }
                if (DtMissingTotal.Columns.Contains("lngMethodSN")) { DtMissingTotal.Columns.Remove("lngMethodSN"); }

                #region Convert 0 to -1 to -7
                for (int intRow = 0; intRow < DtMissingTotal.Rows.Count; intRow++)
                {
                    int intIndexofZero = -1;
                    for (int intNum = 1; intNum <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; intNum++)
                    {
                        string strColName = string.Format(InvariantCulture, "lngM{0}", intNum);
                        if (int.Parse(DtMissingTotal.Rows[intRow][strColName].ToString(), InvariantCulture) == 0)
                        {
                            DtMissingTotal.Rows[intRow][strColName] = intIndexofZero;
                            intIndexofZero--;
                        }
                    }
                }
                #endregion
                gvMissAll.DataSource = DtMissingTotal.DefaultView;
                if (gvMissAll.Columns.Count == 0)
                {
                    for (int i = 0; i < DtMissingTotal.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtMissingTotal.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtMissingTotal.Columns[i].ColumnName, 1),
                            SortExpression = DtMissingTotal.Columns[i].ColumnName,
                        };
                        gvMissAll.Columns.Add(field: bfCell);
                    }
                }
                if (gvMissAll.Columns.Count != 0)
                {
                    int intlngm = 1;
                    foreach (BoundField bfCell in gvMissAll.Columns)
                    {
                        string strColumnName = bfCell.DataField;
                        #region Css
                        if ((strColumnName.Substring(0, 4) != "lngL" && strColumnName.Substring(0, 4) != "lngM") || strColumnName == "lngMethodSN")
                        {
                            bfCell.ItemStyle.CssClass = strColumnName;
                        }
                        #endregion
                        #region Show the number of lngL,lngS,lngM with 2 digital
                        if (strColumnName.Substring(0, 4) == "lngM" && strColumnName != "lngMethodSN")
                        {
                            bfCell.DataFormatString = "{0:d2}";
                            if (_dicCurrentNums.Values.Contains(int.Parse(strColumnName.Substring(4), InvariantCulture)))
                            {
                                bfCell.HeaderStyle.CssClass = string.Format(InvariantCulture, "lngM{0}", intlngm);
                                intlngm++;
                            }
                            int intNum = int.Parse(strColumnName.Substring(4), InvariantCulture);
                            if (intNum % 5 == 0 && strColumnName.Substring(0, 4) == "lngM")
                            {
                                bfCell.ItemStyle.CssClass = "glColNum5";
                            }
                        }
                        #endregion
                    }
                }
                gvMissAll.RowDataBound += GvMissAll_RowDataBound;
                gvMissAll.DataBind();

                gvFreqSec.RowDataBound += GvFreqSec_RowDataBound;
                gvFreqSec.DataBind();
            }
        }

        private void GvMissAll_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowIndex + 1) % 5 == 0) { e.Row.CssClass = "glRow5"; }
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        #region Set lngL , lngS
                        if (strCell_ColumnName.Substring(0, 4) == "lngL" || strCell_ColumnName.Substring(0, 4) == "lngS")
                        {
                            if (_dicCurrentNums.ContainsValue(int.Parse(cell.Text, InvariantCulture)))
                            {
                                foreach (KeyValuePair<string, int> kv in _dicCurrentNums)
                                {
                                    if (kv.Value == int.Parse(cell.Text, InvariantCulture))
                                    {
                                        cell.CssClass = kv.Key;
                                    }
                                }
                            }
                        }
                        #endregion Set lngL , lngS

                        #region Set lngM
                        if (strCell_ColumnName.Substring(0, 4) == "lngM" && strCell_ColumnName != "lngMethodSN")
                        {
                            int intColumnValue = int.Parse(strCell_ColumnName.Substring(4), InvariantCulture);
                            int intValue = int.Parse(cell.Text, InvariantCulture);
                            if (intValue < 0)
                            {
                                if (Math.Abs(intValue) == 1) { _blLngM0 = true; }
                                if (Math.Abs(intValue) == new CglDataSet(_gstuSearch.LottoType).CountNumber) { _blLngM0 = false; }
                                cell.CssClass = (intColumnValue % 5 != 0) ? string.Format(InvariantCulture, "lngM{0}", Math.Abs(intValue)) : string.Format(InvariantCulture, "lngM{0} glColNum5", Math.Abs(intValue));
                            }
                            else
                            {
                                if (_blLngM0)
                                {
                                    cell.CssClass = (intColumnValue % 5 != 0) ? string.Format(InvariantCulture, "lngM{0}", 0) : cell.CssClass + string.Format(InvariantCulture, "lngM{0} glColNum5", 0);
                                }
                            }
                        }
                        #endregion Set lngM

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
                        #endregion Set Saturday

                    }

                }
                if (e.Row.RowIndex < 4)
                {
                    e.Row.CssClass = !string.IsNullOrEmpty(e.Row.CssClass) ? e.Row.CssClass + " glRowSearchLimit" : "glRowSearchLimit";
                }
            }
        }

        private static Dictionary<string, int> GetSectionLimit(DataTable dtFreqSec)
        {
            Dictionary<string, int> dicSectionLimit = new Dictionary<string, int>();
            foreach (int section in new int[] { 5, 10, 25, 50, 100 })
            {
                string strCell_ColumnNameFreq = string.Format(InvariantCulture, "sglFreq{0:d2}", section);
                string strCell_ColumnNameMax = string.Format(InvariantCulture, "intMax{0:d2}", section);
                string expression = string.Format(InvariantCulture, "AVG([{0}])", strCell_ColumnNameMax);
                int value = (int)Math.Round(double.Parse(dtFreqSec.Compute(expression, string.Empty).ToString(), InvariantCulture), 0);
                dicSectionLimit.Add(strCell_ColumnNameFreq, value);
            }
            return dicSectionLimit;
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            Thread ThreadFreqActiveHT01 = (Thread)DicThreadFreqSecFieldT[FreqSecFieldTID + "T01"];
            if (ThreadFreqActiveHT01.ThreadState == ThreadState.Running)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Pause);
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT01.Suspend();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
            if (ThreadFreqActiveHT01.ThreadState == ThreadState.Suspended)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT01.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
        }

        protected void BtnT2StartClick(object sender, EventArgs e)
        {
            Thread ThreadFreqActiveHT02 = (Thread)DicThreadFreqSecFieldT[FreqSecFieldTID + "T02"];
            if (ThreadFreqActiveHT02.ThreadState == ThreadState.Running)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Pause);
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT02.Suspend();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
            if (ThreadFreqActiveHT02.ThreadState == ThreadState.Suspended)
            {

                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
#pragma warning disable CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT02.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
        }

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            Session.Remove(FreqSecFieldTID);
            ViewState.Remove(FreqSecFieldTID + "_gstuSearch");
            ReleaseMemory();
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
        }

        protected void BtnquickSearchOrderClick(object sender, EventArgs e)
        {
            DtSimilarity = ((Dictionary<string, DataSet>)Session[FreqSecFieldTID + "dicFreqSecField"])["Sum"].Tables["dtSimilarity"].Select("", "[intSimilar] DESC , [lngTotalSN01] DESC ").CopyToDataTable();
            foreach (DataRow drSimilarity in DtSimilarity.Rows)
            {
                StuGLSearch stuGLSearchTemp = _gstuSearch;
                stuGLSearchTemp.LngTotalSN = long.Parse(drSimilarity["lngTotalSN01"].ToString(), InvariantCulture);
                //stuGLSearchTemp = new CglSearch().InitSearch(stuGLSearchTemp);
                string RequestID = SetRequestId(stuGLSearchTemp);
                SetSearchOrder(stuGLSearchTemp, Properties.Resources.SessionsFreqSecFieldT, RequestID, Properties.Resources.PageFreqSecFieldT, LocalIP, LocalBrowserType);
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void Timer1Tick(object sender, EventArgs e)
        {
            CheckThread();
        }

        private void CheckThread()
        {
            if (DicThreadFreqSecFieldT.Keys.Contains(FreqSecFieldTID + "T01"))
            {
                Thread01 = (Thread)DicThreadFreqSecFieldT[FreqSecFieldTID + "T01"];
                if (Thread01.IsAlive)
                {
                    Timer1.Enabled = true;
                    Timer1.Interval = 10000;
                    lblArgument.Text = string.Format(InvariantCulture, "{0} ", DateTime.Now.ToLongTimeString());
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0}{1} ", new GalaxyApp().GetTheadState(Thread01.ThreadState), Session[FreqSecFieldTID + "lblT01"].ToString());
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

            if (DicThreadFreqSecFieldT.Keys.Contains(FreqSecFieldTID + "T02"))
            {
                Thread02 = (Thread)DicThreadFreqSecFieldT[FreqSecFieldTID + "T02"];
                if (Thread02.IsAlive)
                {
                    Timer1.Enabled = true;
                    Timer1.Interval = 10000;
                    lblArgument.Text = string.Format(InvariantCulture, "{0} ", DateTime.Now.ToLongTimeString());
                    btnT2Start.Text = string.Format(InvariantCulture, "T2:{0} ", new GalaxyApp().GetTheadState(Thread02.ThreadState));
                    lblArgument.Visible = true;
                    btnT2Start.Visible = true;
                }
                else
                {
                    lblArgument.Visible = false;
                    btnT2Start.Visible = false;
                    Timer1.Enabled = false;
                }
            }
        }

        private void CreatThread()
        {
            Thread01 = new Thread(() => { StartThread01(); ResetSearchOrder(FreqSecFieldTID); }) { Name = FreqSecFieldTID };
            Thread01.Start();
            DicThreadFreqSecFieldT.Add(FreqSecFieldTID + "T01", Thread01);
        }

        private void StartThread01()
        {
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
            DataSet dsSum = new DataSet();
            DataTable dtSum = CreatTableSum();
            DataTable dtTableLastNums = CreatTableLastNums();
            DataTable dtOverLoadTemp = CreatTableOverLoad();
            DataTable dtLastMissAllTemp = CreatTableLastMissAll();
            DataTable dtMissAllSecTemp = CreatTableMissAllSec();
            Dictionary<string, object> dicLastNumsWithSectiont = new Dictionary<string, object>();
            List<string> lstLastNums = new List<string>();
            foreach (string strItem in GetddlFreq())
            {
                if (strItem != "Sum")
                {
                    #region tblFreqSec
                    StuGLSearch stuSearchTemp = _gstuSearch;
                    stuSearchTemp.FieldMode = strItem != "gen";
                    stuSearchTemp.StrCompares = strItem != "gen" ? strItem : "gen";
                    stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
                    stuSearchTemp = new CglMethod().GetMethodSN(stuSearchTemp);
                    stuSearchTemp = new CglMethod().GetSearchMethodSN(stuSearchTemp);
                    stuSearchTemp = new CglMethod().GetSecFieldSN(stuSearchTemp);
                    Session[FreqSecFieldTID + "lblT01"] = string.Format(InvariantCulture, "Get {0} tblFreqSecField", new CglFunc().ConvertFieldNameId(strItem, 1));
                    DataSet dsFreqSecField = new DataSet { Locale = InvariantCulture };
                    DataTable dtFreqSecTemp = new CglFreqSec().GetFreqSec(stuSearchTemp, CglFreqSec.TableName.QryFreqSec, SortOrder.Ascending);
                    dtFreqSecTemp.TableName = "dtFreqSec";
                    dsFreqSecField.Tables.Add(dtFreqSecTemp);

                    DataTable dtMissAllTemp00 = new CglMissAll().GetMissAll00Multiple(stuSearchTemp, CglMissAll.TableName.QryMissAll0001, SortOrder.Descending);
                    dtMissAllTemp00.TableName = "dtMissAll";
                    #region LastMissAll
                    DataRow drLastMissAllTemp = dtLastMissAllTemp.NewRow();
                    drLastMissAllTemp["strField"] = strItem;
                    foreach (DataColumn dcLastMissAllTemp in dtLastMissAllTemp.Columns)
                    {
                        if (dcLastMissAllTemp.ColumnName.Substring(0, 4) == "lngM")
                        {
                            drLastMissAllTemp[dcLastMissAllTemp.ColumnName] = dtMissAllTemp00.Rows[0][dcLastMissAllTemp.ColumnName];
                        }
                    }
                    dtLastMissAllTemp.Rows.Add(drLastMissAllTemp);
                    #endregion LastMissAll
                    dsFreqSecField.Tables.Add(dtMissAllTemp00);

                    #endregion tblFreqSec

                    #region dtTableLastNums
                    DataRow drTableLastNums = dtTableLastNums.NewRow();
                    drTableLastNums["strField"] = strItem;
                    int numCount = 1;
                    List<string> lstLastNumsTemp = new List<string>();
                    foreach (DataRow drFreqSecTemp in dtFreqSecTemp.Rows)
                    {
                        if (int.Parse(drFreqSecTemp["lngM"].ToString(), InvariantCulture) == 0)
                        {
                            drTableLastNums[string.Format(InvariantCulture, "lngN{0}", numCount)] = drFreqSecTemp["lngN"];
                            lstLastNumsTemp.Add(drFreqSecTemp["lngN"].ToString());
                            numCount++;
                        }
                        #region dtOverLoadTemp
                        if (CheckOverLoad(stuSearchTemp, drFreqSecTemp, GetSectionLimit(dtFreqSecTemp)))
                        {
                            DataRow drOverLoad = dtOverLoadTemp.NewRow();
                            drOverLoad["strField"] = strItem;
                            drOverLoad["lngN"] = drFreqSecTemp["lngN"];
                            drOverLoad["lngM"] = drFreqSecTemp["lngM"];
                            foreach (int intsec in new int[] { 5, 10, 25, 50, 100 })
                            {
                                drOverLoad[string.Format(InvariantCulture, "sglFreq{0:d2}", intsec)] =
                                    drFreqSecTemp[string.Format(InvariantCulture, "sglFreq{0:d2}", intsec)];
                                drOverLoad[string.Format(InvariantCulture, "intMax{0:d2}", intsec)] =
                                    drFreqSecTemp[string.Format(InvariantCulture, "intMax{0:d2}", intsec)];
                            }
                            dtOverLoadTemp.Rows.Add(drOverLoad);
                        }
                        #endregion dtOverLoadTemp
                    }
                    if (!lstLastNums.Contains(string.Join("#", lstLastNumsTemp)))
                    {
                        lstLastNums.Add(string.Join("#", lstLastNumsTemp));
                    }
                    dtTableLastNums.Rows.Add(drTableLastNums);
                    #endregion dtTableLastNums

                    #region tblFreqSecField
                    DataTable dtFreqSecFieldTemp = new CglFreqSecField().GetFreqSecField(stuSearchTemp, CglFreqSec.TableName.QryFreqSecField);
                    dtFreqSecFieldTemp.TableName = "dtFreqSecField";
                    dsFreqSecField.Tables.Add(dtFreqSecFieldTemp);
                    DicFreqSecField.Add(strItem, dsFreqSecField);
                    // if (Session[FreqSecFieldTID + "LastNumsWithSectiont"] == null) { Session.Add(FreqSecFieldTID + "LastNumsWithSectiont", new Dictionary<string, object>()); }
                    if (dicLastNumsWithSectiont.ContainsKey(strItem))
                    {
                        dicLastNumsWithSectiont[strItem] = new CglFreqSec().GetLastNumsWithFreqSec(stuSearchTemp);
                    }
                    else
                    {
                        dicLastNumsWithSectiont.Add(strItem, new CglFreqSec().GetLastNumsWithFreqSec(stuSearchTemp));
                    }
                    Session[FreqSecFieldTID + "dicFreqSecField"] = DicFreqSecField;
                    Session[FreqSecFieldTID + "LastNumsWithSectiont"] = dicLastNumsWithSectiont;
                    if (!dtSum.Columns.Contains(strItem))
                    {
                        dtSum.Columns.Add(new DataColumn()
                        {
                            ColumnName = strItem,
                            DataType = typeof(double),
                            DefaultValue = 0,
                            Caption = strItem
                        });
                    }
                    foreach (DataRow drFreqSecFieldTemp in dtFreqSecFieldTemp.Rows)
                    {
                        DataRow drFindRow = dtSum.Rows.Find(drFreqSecFieldTemp["lngN"]);
                        if (drFindRow == null)
                        {
                            drFindRow = dtSum.NewRow();
                            drFindRow["lngN"] = drFreqSecFieldTemp["lngN"];
                            drFindRow[strItem] = drFreqSecFieldTemp["intHistoryHitRate"];
                            drFindRow["dblSum"] = (double)drFreqSecFieldTemp["intCorrespondPeriods"] == (double)drFreqSecFieldTemp["intCorrespondHistoryPeriods"] ? (double)drFindRow["dblSum"] : (double)drFindRow["dblSum"] + (double)drFreqSecFieldTemp["intHistoryHitRate"];
                            //drFindRow["dblSum"] = (double)drFindRow["dblSum"] + (double)drFreqSecFieldTemp["intHistoryHitRate"];
                            dtSum.Rows.Add(drFindRow);
                        }
                        else
                        {
                            drFindRow[strItem] = drFreqSecFieldTemp["intHistoryHitRate"];
                            drFindRow["dblSum"] = (double)drFreqSecFieldTemp["intCorrespondPeriods"] == (double)drFreqSecFieldTemp["intCorrespondHistoryPeriods"] ? (double)drFindRow["dblSum"] : (double)drFindRow["dblSum"] + (double)drFreqSecFieldTemp["intHistoryHitRate"];
                            //drFindRow["dblSum"] = (double)drFindRow["dblSum"] + (double)drFreqSecFieldTemp["intHistoryHitRate"];
                        }
                    }
                    #endregion tblFreqSecField
                }
            }
            // ---------------------------------------------------------------------------------------------------------
            #region tblSimilarity
            dsSum.Tables.Add(dtTableLastNums);//dtTableLastNums
            dsSum.Tables.Add(dtLastMissAllTemp);//dtLastMissAllTemp
            dsSum.Tables.Add(dtOverLoadTemp);//dtOverLoadTemp

            dsSum.Tables.Add(dtSum);//dtSum

            Session[FreqSecFieldTID + "lblT01"] = "Get tblSimilarity";
            DataTable dtSimilaraty00 = new CglSimilarity().GetSimilarityMultiple(_gstuSearch, _gstuSearch.InFieldPeriodLimit, true, 0);
            int point = dtLastMissAllTemp.Rows.Count;
            int InTotalSnRange = 800;
            while (dtSimilaraty00.Select(string.Format(InvariantCulture, "[lngTotalSN01] >= {0} AND [intSimilar] > {1}", _gstuSearch.LngTotalSN - InTotalSnRange, point)).Length < 5 && point > 0) { point--; }
            string select = string.Format(InvariantCulture, "[lngTotalSN01] >= {0} AND [intSimilar] >= {1}", _gstuSearch.LngTotalSN - InTotalSnRange, point);
            DataTable dtSimilarityTemp = dtSimilaraty00.Select(select, "").Length > 0 ?
                                         dtSimilaraty00.Select(select, "").CopyToDataTable() :
                                         dtSimilaraty00.Clone();
            dtSimilarityTemp.Locale = InvariantCulture;
            dtSimilarityTemp.TableName = "dtSimilarity";
            dsSum.Tables.Add(dtSimilarityTemp);//dtSimilarityTemp

            DataTable dtSimFreq = GetSimFreq(_gstuSearch, dtSimilarityTemp);
            dtSimFreq.Locale = InvariantCulture;
            dtSimFreq.TableName = "dtSimFreq";
            dsSum.Tables.Add(dtSimFreq);//dtSimFreq

            select = string.Format(InvariantCulture, "[lngTotalSN01] > {0} AND [intSimilar] = {1}", _gstuSearch.LngTotalSN - InTotalSnRange, point);
            DataTable dtSimilarity01 = dtSimilaraty00.Select(select, "").Length > 0 ?
                                       dtSimilaraty00.Select(select, "").CopyToDataTable() :
                                       dtSimilaraty00.Clone();
            DataTable dtSimFreq01 = GetSimFreq(_gstuSearch, dtSimilarity01);
            dtSimFreq01.Locale = InvariantCulture;
            dtSimFreq01.TableName = "dtSimFreq01";
            dsSum.Tables.Add(dtSimFreq01);//dtSimFreq01
            #endregion tblSimilarity

            // ---------------------------------------------------------------------------------------------------------
            #region dtSimFreqSum
            DataTable dtSimFreqSum = dtSimFreq01.Clone();
            //import dtSimFreq01 ---------------------
            dtSimFreqSum.ImportRow(dtSimFreq01.Rows[0]);
            using (DataColumn dcColumn = new DataColumn())
            {
                dcColumn.ColumnName = "strItem";
                dcColumn.DataType = typeof(string);
                dcColumn.Caption = dcColumn.ColumnName;
                dtSimFreqSum.Columns.Add(dcColumn);
            } //Creat new column strItem
            dtSimFreqSum.Columns["strItem"].SetOrdinal(0);
            dtSimFreqSum.Rows[0]["strItem"] = string.Format(InvariantCulture, "SSN = {0} ({1})", point, dtSimilarity01.Rows.Count);

            //import dtSimFreq ---------------------
            DataRow drSimFreqSum = dtSimFreqSum.NewRow();
            foreach (DataColumn dcSimFreqSum in dtSimFreqSum.Columns)
            {
                if (dcSimFreqSum.ColumnName != "strItem")
                {
                    drSimFreqSum[dcSimFreqSum.ColumnName] = dtSimFreq.Rows[0][dcSimFreqSum.ColumnName];
                }
                else
                {
                    drSimFreqSum[dcSimFreqSum.ColumnName] = string.Format(InvariantCulture, "SSN > {0} ({1})", point, dtSimilarityTemp.Rows.Count - dtSimilarity01.Rows.Count);
                }
            }
            dtSimFreqSum.Rows.Add(drSimFreqSum);

            //Caculate SSNM ---------------------
            drSimFreqSum = dtSimFreqSum.NewRow();
            foreach (DataColumn dcSimFreqSum in dtSimFreqSum.Columns)
            {
                if (dcSimFreqSum.ColumnName != "strItem")
                {
                    drSimFreqSum[dcSimFreqSum.ColumnName] = int.Parse(dtSimFreqSum.Rows[1][dcSimFreqSum.ColumnName].ToString(), InvariantCulture)
                                                      - int.Parse(dtSimFreqSum.Rows[0][dcSimFreqSum.ColumnName].ToString(), InvariantCulture);
                }
                else
                {
                    drSimFreqSum[dcSimFreqSum.ColumnName] = "SSNM";
                }
            }
            dtSimFreqSum.Rows.Add(drSimFreqSum);

            //import LastNum ---------------------
            Dictionary<string, int> dicLastNums = GetLastNumsCount(lstLastNums);
            drSimFreqSum = dtSimFreqSum.NewRow();
            drSimFreqSum["strItem"] = string.Format(InvariantCulture, "LastNum({0})", lstLastNums.Count);
            foreach (KeyValuePair<string, int> varLastNum in dicLastNums)
            {
                drSimFreqSum[varLastNum.Key] = varLastNum.Value;
            }
            dtSimFreqSum.Rows.Add(drSimFreqSum);

            //OverLoad ---------------------
            drSimFreqSum = dtSimFreqSum.NewRow();
            foreach (DataColumn dcSimFreqSum in dtSimFreqSum.Columns)
            {
                if (dcSimFreqSum.ColumnName != "strItem")
                {
                    select = string.Format(InvariantCulture, "[lngN] = {0}", dcSimFreqSum.ColumnName.Substring(4)); ;
                    drSimFreqSum[dcSimFreqSum.ColumnName] = dtOverLoadTemp.Select(select, "").Length;
                }
                else
                {
                    drSimFreqSum[dcSimFreqSum.ColumnName] = string.Format(InvariantCulture, "OverLoad({0})", dtOverLoadTemp.Rows.Count);
                }
            }
            dtSimFreqSum.Rows.Add(drSimFreqSum);

            dtSimFreqSum.TableName = "dtSimFreqSum";
            dsSum.Tables.Add(dtSimFreqSum);//dtSimFreqSum
            #endregion dtSimFreqSum

            //dtMissAllSec---------------------
            #region dtMissAllSec
            foreach (int insec in new int[] { 1, 5, 10, 20, 25, 30 })
            {
                DataRow drMissAllSecTemp = dtMissAllSecTemp.NewRow();
                drMissAllSecTemp["strSetion"] = string.Format(InvariantCulture, "{0:d2}", insec);
                foreach (DataRow drLastMissAll in dtLastMissAllTemp.Rows)
                {
                    foreach (DataColumn dcLastMissAll in dtLastMissAllTemp.Columns)
                    {
                        if (dcLastMissAll.ColumnName != "strField")
                        {
                            if (int.Parse(drLastMissAll[dcLastMissAll.ColumnName].ToString(), InvariantCulture) > insec)
                            {
                                drMissAllSecTemp[dcLastMissAll.ColumnName] = int.Parse(drMissAllSecTemp[dcLastMissAll.ColumnName].ToString(), InvariantCulture) + 1;
                            }
                        }
                    }
                }
                dtMissAllSecTemp.Rows.Add(drMissAllSecTemp);
            }
            //LastMissAll---------------------            
            DataRow drMissAllSecTemp01 = dtMissAllSecTemp.NewRow();
            foreach (DataColumn dcMissAllSecTemp in dtMissAllSecTemp.Columns)
            {
                if (dcMissAllSecTemp.ColumnName == "strSetion")
                {
                    drMissAllSecTemp01["strSetion"] = string.Format(InvariantCulture, "LastMissAll({0})", dtLastMissAllTemp.Rows.Count);
                }
                else
                {
                    drMissAllSecTemp01[dcMissAllSecTemp.ColumnName] = dtLastMissAllTemp.Compute(string.Format(InvariantCulture, "SUM([{0}])", dcMissAllSecTemp.ColumnName), string.Empty);
                }
            }

            dtMissAllSecTemp.Rows.Add(drMissAllSecTemp01);
            //FreqSecSum ---------------------
            drMissAllSecTemp01 = dtMissAllSecTemp.NewRow();
            drMissAllSecTemp01["strSetion"] = "FreqSecSum";
            foreach (DataRow drSum in dtSum.Rows)
            {
                string strColumnName = string.Format(InvariantCulture, "lngM{0}", drSum["lngN"].ToString());
                drMissAllSecTemp01[strColumnName] = double.Parse(drSum["dblSum"].ToString(), InvariantCulture) > 0 ? 1 : 0;
            }

            dtMissAllSecTemp.Rows.Add(drMissAllSecTemp01);
            dtMissAllSecTemp.TableName = "dtMissAllSec";
            dsSum.Tables.Add(dtMissAllSecTemp);
            #endregion dtMissAllSec

            // ---------------------------------------------------------------------------------------------------------
            DicFreqSecField.Add("Sum", dsSum);
            Session[FreqSecFieldTID + "dicFreqSecField"] = DicFreqSecField;
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
        }

        private static bool CheckOverLoad(StuGLSearch stuSearchTemp, DataRow drFreqSecTemp, Dictionary<string, int> dicSecLimit)
        {
            bool check = false;
            foreach (int intsec in new int[] { 5, 10, 25, 50, 100 })
            {
                string strCell_ColumnNameFreq = string.Format(InvariantCulture, "sglFreq{0:d2}", intsec);
                if (int.Parse(drFreqSecTemp[strCell_ColumnNameFreq].ToString(), InvariantCulture)
                    == int.Parse(drFreqSecTemp[string.Format(InvariantCulture, "intMax{0:d2}", intsec)].ToString(), InvariantCulture)
                    && int.Parse(drFreqSecTemp[strCell_ColumnNameFreq].ToString(), InvariantCulture) > dicSecLimit[strCell_ColumnNameFreq])
                {
                    if (!((Dictionary<string, List<int>>)new CglFreqSec().GetLastNumsWithFreqSec(stuSearchTemp))[string.Format(InvariantCulture, "{0:d2}", intsec)].Contains(int.Parse(drFreqSecTemp["lngN"].ToString(), InvariantCulture)))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return check;
        }

        private Dictionary<string, int> GetLastNumsCount(List<string> lstLastNums)
        {
            Dictionary<string, int> dicLastNums = new Dictionary<string, int>();
            for (int intColum = 1; intColum <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; intColum++)
            {
                dicLastNums.Add(string.Format(InvariantCulture, "lngN{0}", intColum), 0);
            }
            foreach (string stritem in lstLastNums)
            {
                foreach (string intNum in stritem.Split('#').ToList())
                {
                    string strColumnName = string.Format(InvariantCulture, "lngN{0}", int.Parse(intNum, InvariantCulture));
                    dicLastNums[strColumnName]++;
                }
            }

            return dicLastNums;
        }

        private static DataTable GetSimFreq(StuGLSearch stuGLSearch, DataTable dtData)
        {
            if (dtData == null) { throw new ArgumentNullException(nameof(dtData)); }

            Dictionary<string, int> dicSimFre = new Dictionary<string, int>();
            for (int intColum = 1; intColum <= new CglDataSet(stuGLSearch.LottoType).LottoNumbers; intColum++)
            {
                dicSimFre.Add(string.Format(InvariantCulture, "lngN{0}", intColum), 0);
            }
            foreach (DataRow dataRow in dtData.Rows)
            {
                for (int nums = 1; nums <= new CglDataSet(stuGLSearch.LottoType).CountNumber; nums++)
                {
                    string strFieldName = string.Format(InvariantCulture, "lngN{0}", dataRow[string.Format(InvariantCulture, "lngL{0}", nums)]);
                    dicSimFre[strFieldName]++;
                }
                if (new CglDataSet(stuGLSearch.LottoType).SCountNumber > 0 && stuGLSearch.LottoType != TargetTable.LottoWeli)
                {
                    string strFieldName = string.Format(InvariantCulture, "lngN{0}", dataRow["lngS1"]);
                    dicSimFre[strFieldName]++;
                }
            }
            Dictionary<string, int> dicFreqSort = dicSimFre.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            using DataTable dtReturn = new DataTable { Locale = InvariantCulture };
            #region Columns
            foreach (KeyValuePair<string, int> Num in dicFreqSort)
                dtReturn.Columns.Add(new DataColumn()
                {
                    ColumnName = Num.Key,
                    DataType = typeof(int),
                    DefaultValue = Num.Value
                });
            #endregion Columns
            dtReturn.Rows.Add(dtReturn.NewRow());
            return dtReturn;
        }

        private static DataTable CreatTableOverLoad()
        {
            using DataTable dtReturn = new DataTable() { Locale = InvariantCulture };
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = "strField",
                DataType = typeof(string),
                AllowDBNull = false,
            });

            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = "lngN",
                DataType = typeof(int),
                DefaultValue = 0
            });

            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = "lngM",
                DataType = typeof(int),
                DefaultValue = 0
            });

            foreach (int intSec in new int[] { 5, 10, 25, 50, 100 })
            {
                dtReturn.Columns.Add(new DataColumn()
                {
                    ColumnName = string.Format(InvariantCulture, "sglFreq{0:d2}", intSec),
                    DataType = typeof(int),
                    DefaultValue = 0
                });
                dtReturn.Columns.Add(new DataColumn()
                {
                    ColumnName = string.Format(InvariantCulture, "intMax{0:d2}", intSec),
                    DataType = typeof(int),
                    DefaultValue = 0
                });
            }

            //dtReturn.PrimaryKey = new DataColumn[] { dtReturn.Columns["strField"] };
            dtReturn.TableName = "dtOverLoad";
            return dtReturn.Copy();
        }

        private static DataTable CreatTableSum()
        {
            using DataTable dtReturn = new DataTable("dtSum");
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = "lngN",
                DataType = typeof(int),
                AllowDBNull = false,
                Unique = true
            });
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = "dblSum",
                DataType = typeof(double),
                DefaultValue = 0
            });
            dtReturn.PrimaryKey = new DataColumn[] { dtReturn.Columns["lngN"] };
            return dtReturn.Copy();
        }

        private DataTable CreatTableLastNums()
        {
            using DataTable dtReturn = new DataTable("dtLastNums");
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = "strField",
                DataType = typeof(string),
                AllowDBNull = false,
                Unique = true
            });
            for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).CountNumber + new CglDataSet(_gstuSearch.LottoType).SCountNumber; i++)
            {
                dtReturn.Columns.Add(new DataColumn()
                {
                    ColumnName = string.Format(InvariantCulture, "lngN{0}", i),
                    DataType = typeof(int),
                    DefaultValue = 0
                });
            }
            dtReturn.PrimaryKey = new DataColumn[] { dtReturn.Columns["strField"] };
            return dtReturn.Copy();
        }

        private DataTable CreatTableLastMissAll()
        {
            using DataTable dtReturn = new DataTable();
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "strField"),
                Caption = string.Format(InvariantCulture, "strField"),
                DataType = typeof(string),
                AllowDBNull = false,
                Unique = true
            });
            for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; i++)
            {
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "lngM{0}", i),
                    Caption = string.Format(InvariantCulture, "lngM{0}", i),
                    DataType = typeof(int),
                    DefaultValue = 0
                });
            }
            dtReturn.PrimaryKey = new DataColumn[] { dtReturn.Columns["strField"] };
            dtReturn.TableName = "dtLastMissAll";
            return dtReturn.Copy();
        }

        private DataTable CreatTableMissAllSec()
        {
            using DataTable dtReturn = new DataTable();
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "strSetion"),
                Caption = string.Format(InvariantCulture, "strSetion"),
                DataType = typeof(string),
                AllowDBNull = false,
                Unique = true
            });
            for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; i++)
            {
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "lngM{0}", i),
                    Caption = string.Format(InvariantCulture, "lngM{0}", i),
                    DataType = typeof(int),
                    DefaultValue = 0
                });
            }
            dtReturn.PrimaryKey = new DataColumn[] { dtReturn.Columns["strField"] };
            dtReturn.TableName = "dtLastMissAll";
            return dtReturn.Copy();

        }

        private void ReleaseMemory()
        {
            Session.Remove(FreqSecFieldTID + "ddlFreq");
            Session.Remove(FreqSecFieldTID + "dicFreqSecField");
            Session.Remove(FreqSecFieldTID + "LastNumsWithSectiont");
            Session.Remove(FreqSecFieldTID + "lblT01");
            Session.Remove(FreqSecFieldTID + "lblT02");
            ResetSearchOrder(FreqSecFieldTID);
            if (DicThreadFreqSecFieldT.Keys.Contains(FreqSecFieldTID + "T01"))
            {
                Thread ThreadFreqActiveHT01 = (Thread)DicThreadFreqSecFieldT[FreqSecFieldTID + "T01"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActiveHT01.ThreadState == ThreadState.Suspended) { ThreadFreqActiveHT01.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT01.Abort();
                ThreadFreqActiveHT01.Join();
                DicThreadFreqSecFieldT.Remove(FreqSecFieldTID + "T01");
            }

            if (DicThreadFreqSecFieldT.Keys.Contains(FreqSecFieldTID + "T02"))
            {
                Thread ThreadFreqActiveHT02 = (Thread)DicThreadFreqSecFieldT[FreqSecFieldTID + "T02"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActiveHT02.ThreadState == ThreadState.Suspended) { ThreadFreqActiveHT02.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT02.Abort();
                ThreadFreqActiveHT02.Join();
                DicThreadFreqSecFieldT.Remove(FreqSecFieldTID + "T02");
            }
        }

    }
}
