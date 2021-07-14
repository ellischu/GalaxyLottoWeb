using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqActiveHTAll : BasePage
    {
        private string _action;
        private string _requestId;
        private StuGLSearch _gstuSearch;
        private string FreqActiveHTAllID;
        //private DropDownList _pageList;

        private static List<string> LstFields => CglClass.LstFieldName;
        private static List<string> LstFieldsF => new List<string>() { "sglFreq05", "sglFreq10", "sglFreq25", "sglFreq50", "sglFreq100", "lngM" };

        private static Dictionary<string, object> DicThreadFreqActiveHTAll
        {
            get
            {
                if (dicThreadFreqActiveHTAll == null) dicThreadFreqActiveHTAll = new Dictionary<string, object>();
                return dicThreadFreqActiveHTAll;
            }
            set => dicThreadFreqActiveHTAll = value;
        }

        private static Dictionary<string, object> dicThreadFreqActiveHTAll;

        private static Dictionary<string, string> DicUpdateFieldFreqActiveHTAll
        {
            get
            {
                if (dicUpdateFieldFreqActiveHTAll == null) dicUpdateFieldFreqActiveHTAll = new Dictionary<string, string>();
                return dicUpdateFieldFreqActiveHTAll;
            }
            set => dicUpdateFieldFreqActiveHTAll = value;
        }

        private DataTable dtFreqFilter;
        private static Dictionary<string, string> dicUpdateFieldFreqActiveHTAll;


        // ---------------------------------------------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {

            //if (!_dsData.IsInitialized) { _dsData = new DataSet { Locale = InvariantCulture }; }
            SetupViewState();
            if (ViewState["_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState["_gstuSearch"];
                InitialArgument();
                ShowResult();
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            FreqActiveHTAllID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqActiveHTAllID] != null)
            {
                if (ViewState["_gstuSearch"] == null)
                {
                    ViewState.Add("_gstuSearch", (StuGLSearch)Session[FreqActiveHTAllID]);
                }
                else
                {
                    ViewState["_gstuSearch"] = (StuGLSearch)Session[FreqActiveHTAllID];
                };
            }
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        private void InitialArgument()
        {
            if (ViewState["title"] == null) { ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "活性歷史表01", new CglDBData().SetTitleString(_gstuSearch))); }
            if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch))); }
            if (ViewState["sort"] == null) { ViewState.Add("sort", ""); }
            if (ViewState["select"] == null) { ViewState.Add("select", ""); }
            if (ViewState[FreqActiveHTAllID + "Fileds"] == null) { GetFields(); }
            if (Session[FreqActiveHTAllID + "UpdateField"] == null) { Session.Add(FreqActiveHTAllID + "UpdateField", "gen"); }
            SetddlFieldName();
        }

        private void GetFields()
        {
            List<string> lstCaculateFields = new List<string>();
            List<string> lstShowFields = new List<string>();
            foreach (string field in LstFields)
            {
                StuGLSearch stuGLSearchTemp = _gstuSearch;
                stuGLSearchTemp.FieldMode = field != "gen";
                stuGLSearchTemp.StrCompares = field;
                stuGLSearchTemp = new CglSearch().InitSearch(stuGLSearchTemp);
                stuGLSearchTemp = new CglMethod().GetMethodSN(stuGLSearchTemp);
                stuGLSearchTemp = new CglMethod().GetSearchMethodSN(stuGLSearchTemp);
                stuGLSearchTemp = new CglMethod().GetSecFieldSN(stuGLSearchTemp);
                using DataTable dtFreqProcess = CglFreqProcess.GetFreqProcAlls(stuGLSearchTemp, CglDBFreq.TableName.TblFreqProcess, SortOrder.Descending, 0);
                string strlngTotalSN = dtFreqProcess.Rows[0]["lngTotalSN"].ToString();
                string strRowsCount = dtFreqProcess.Rows.Count.ToString(InvariantCulture);
                if (!lstCaculateFields.Contains(strlngTotalSN + strRowsCount))
                {
                    lstCaculateFields.Add(strlngTotalSN + strRowsCount);
                    lstShowFields.Add(field);
                }
            }
            ViewState.Add(FreqActiveHTAllID + "Fileds", lstShowFields);
        }

        private void ShowResult()
        {
            Page.Title = (string)ViewState["title"];
            lblTitle.Text = (string)ViewState["title"];

            //顯示當前資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            ShowFreqFilter(ddlFieldName.SelectedValue);
            if (!DicThreadFreqActiveHTAll.Keys.Contains(FreqActiveHTAllID)) { CreatThread(); }
        }

        private void ShowFreqFilter(string selectedValue)
        {
            GetdsFreqSecHis(_gstuSearch, ddlFieldName.SelectedValue);
            #region dtFreqFilter
            dtFreqFilter = ((DataSet)Session[FreqActiveHTAllID + "dsFreqFilter"]).Tables[selectedValue];
            lblFreqFilter.Text = string.Format(InvariantCulture, "{0} ({1}期)", new CglFunc().ConvertFieldNameId(selectedValue), dtFreqFilter.Rows.Count);
            dtFreqFilter.DefaultView.RowFilter = (string)ViewState["select"];
            dtFreqFilter.DefaultView.Sort = (string)ViewState["sort"];
            if (dtFreqFilter.Columns.Contains("lngDateSN")) { dtFreqFilter.Columns.Remove("lngDateSN"); }

            gvFreqFilter.DataSource = dtFreqFilter.DefaultView;
            gvFreqFilter.PagerTemplate = new PagerTemplate();
            if (gvFreqFilter.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < dtFreqFilter.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = dtFreqFilter.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(dtFreqFilter.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = dtFreqFilter.Columns[ColumnIndex].ColumnName,
                    };
                    bfCell.HeaderStyle.CssClass = bfCell.DataField;
                    bfCell.ItemStyle.CssClass = bfCell.DataField;
                    gvFreqFilter.Columns.Add(bfCell);
                    //if (dtFreqFilter.Columns[ColumnIndex].ColumnName == "CDataType")
                    //{
                    //    gvFreqFilter.Columns[ColumnIndex].Visible = false;
                    //}
                }
            }
            gvFreqFilter.RowDataBound += GvFreqFilterRowDataBound;
            gvFreqFilter.DataBound += GvFreqFilter_DataBound;
            gvFreqFilter.DataBind();
            #endregion dtFreqFilter
        }

        private void SetddlFieldName()
        {
            if (ddlFieldName.Items.Count == 0)
            {
                #region ddlFieldName
                foreach (string strFieldName in LstFields)
                {
                    ddlFieldName.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(strFieldName), strFieldName));
                }
                #endregion ddlFieldName
            }
        }

        private void GetdsFreqSecHis(StuGLSearch stuGLSearch, string selectedValue)
        {
            CheckData = true;
            List<string> lstShowField = (List<string>)ViewState[FreqActiveHTAllID + "Fileds"];
            if (lstShowField.Contains(selectedValue))
            {
                if (Session[FreqActiveHTAllID + "dsFreqFilter"] != null)
                {
                    using DataSet dsTemp = (DataSet)Session[FreqActiveHTAllID + "dsFreqFilter"];
                    dsTemp.Locale = InvariantCulture;
                    UpadtedsFreqFilter(stuGLSearch, selectedValue, dsTemp);
                }
                else
                {
                    using DataSet dsTemp = new DataSet
                    {
                        Locale = InvariantCulture
                    };
                    UpadtedsFreqFilter(stuGLSearch, selectedValue, dsTemp);
                }
            }
            //else
            //{
            //    for (int intCom = 1; intCom <= LstFieldsF.Count; intCom++)
            //    {
            //        foreach (string strSecField in new CglFunc().Combination(LstFieldsF.ToArray(), intCom).TrimEnd(';').Split(';').ToList())
            //        {
            //            StuGLSearch stuGLSearchTemp = stuGLSearch;
            //            stuGLSearchTemp.FieldMode = selectedValue == "gen" ? false : true;
            //            stuGLSearchTemp.StrCompares = selectedValue;
            //            List<string> lstSecField = strSecField.Split('#').ToList();
            //            lstSecField.Sort();
            //            stuGLSearchTemp.StrSecField = string.Join("#", lstSecField.Distinct());
            //            stuGLSearchTemp = new CglSearch().InitSearch(stuGLSearchTemp);
            //            stuGLSearchTemp = new CglMethod().GetMethodSN(stuGLSearchTemp);
            //            stuGLSearchTemp = new CglMethod().GetSearchMethodSN(stuGLSearchTemp);
            //            stuGLSearchTemp = new CglMethod().GetSecFieldSN(stuGLSearchTemp);
            //            new CglFreqFilter().DeleteFreqFilter(stuGLSearchTemp);
            //        }
            //    }
            //}
        }

        private void UpadtedsFreqFilter(StuGLSearch stuGLSearch, string selectedValue, DataSet dsTemp)
        {
            using DataSet dsFreqSecHisTemp = new DataSet { Locale = InvariantCulture };
            if (!dsTemp.Tables.Contains(selectedValue))
            {
                for (int intCom = 1; intCom <= LstFieldsF.Count; intCom++)
                {
                    foreach (string strSecField in new CglFunc().Combination(LstFieldsF.ToArray(), intCom).TrimEnd(';').Split(';').ToList())
                    {
                        StuGLSearch stuGLSearchTemp = stuGLSearch;
                        stuGLSearchTemp.FieldMode = selectedValue != "gen";
                        stuGLSearchTemp.StrCompares = selectedValue;
                        List<string> lstSecField = strSecField.Split('#').ToList();
                        lstSecField.Sort();
                        stuGLSearchTemp.StrSecField = string.Join("#", lstSecField.Distinct());
                        stuGLSearchTemp.FilterRange = true;
                        stuGLSearchTemp.SglFilterMin = 1;
                        stuGLSearchTemp.SglFilterMax = 1000;
                        stuGLSearchTemp = new CglSearch().InitSearch(stuGLSearchTemp);
                        stuGLSearchTemp = new CglMethod().GetMethodSN(stuGLSearchTemp);
                        stuGLSearchTemp = new CglMethod().GetSearchMethodSN(stuGLSearchTemp);
                        stuGLSearchTemp = new CglMethod().GetSecFieldSN(stuGLSearchTemp);
                        DataTable dtFreqFilterTemp = new CglFreqFilter().GetFreqFilter(stuGLSearchTemp, CglFreqFilter.TableName.QryFreqFilterT);
                        if (dtFreqFilterTemp.Columns.Contains("TestPeriods")) { dtFreqFilterTemp.Columns.Remove("TestPeriods"); }
                        dsFreqSecHisTemp.Tables.Add(dtFreqFilterTemp.Clone());
                        dsFreqSecHisTemp.Tables[0].TableName = "FreqFilter";
                        foreach (DataRow drInput in dtFreqFilterTemp.Rows)
                        {
                            dsFreqSecHisTemp.Tables["FreqFilter"].ImportRow(drInput);
                        }
                    }
                }
                using DataTable dtfilterTemp = dsFreqSecHisTemp.Tables.Contains("FreqFilter") ? dsFreqSecHisTemp.Tables["FreqFilter"].Copy() : new DataTable();
                dtfilterTemp.Locale = InvariantCulture;
                dtfilterTemp.TableName = selectedValue;
                dsTemp.Tables.Add(dtfilterTemp);
                Session[FreqActiveHTAllID + "dsFreqFilter"] = dsTemp;
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void GvFreqFilter_DataBound(object sender, EventArgs e)
        {
            if (gvFreqFilter.Rows.Count > 0)
            {
                GridViewRow pagerRow = gvFreqFilter.TopPagerRow;
                pagerRow.Visible = true;
                using (DropDownList _pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList"))
                {
                    _pageList.SelectedIndexChanged += PageDropDownListSelectedIndexChanged; ;
                    if (_pageList != null)
                    {
                        // Create the values for the DropDownList control based on 
                        // the  total number of pages required to display the data
                        // source.
                        for (int i = 0; i < gvFreqFilter.PageCount; i++)
                        {
                            // Create a ListItem object to represent a page.
                            int pageNumber = i + 1;
                            ListItem item = new ListItem(pageNumber.ToString(InvariantCulture));

                            // If the ListItem object matches the currently selected
                            // page, flag the ListItem object as being selected. Because
                            // the DropDownList control is recreated each time the pager
                            // row gets created, this will persist the selected item in
                            // the DropDownList control.   
                            if (i == gvFreqFilter.PageIndex)
                            {
                                item.Selected = true;
                            }

                            // Add the ListItem object to the Items collection of the 
                            // DropDownList.
                            _pageList.Items.Add(item);
                        }
                    }
                }

                Button btnPrevPage = (Button)pagerRow.Cells[0].FindControl("btnPrev");
                btnPrevPage.Text = Properties.Resources.btnPrevPage;
                btnPrevPage.ToolTip = Properties.Resources.btnPrevPageTip;
                btnPrevPage.Click += BtnPrevPageClick;

                Button btnPrev10Page = (Button)pagerRow.Cells[0].FindControl("btnPrev10");
                btnPrev10Page.Text = Properties.Resources.btnPrev10Page;
                btnPrev10Page.ToolTip = Properties.Resources.btnPrev10PageTip;
                btnPrev10Page.Click += BtnPrev10PageClick;

                Label pageLabel = (Label)pagerRow.Cells[0].FindControl("pnlPagerTemplate").FindControl("CurrentPageLabel");
                if (pageLabel != null)
                {
                    // Calculate the current page number.
                    int currentPage = gvFreqFilter.PageIndex + 1;
                    // Update the Label control with the current page information.
                    pageLabel.Text = string.Format(InvariantCulture, " ({0:d2} / {1:d2})", currentPage, gvFreqFilter.PageCount);
                }

                Button btnNextPage = (Button)pagerRow.Cells[0].FindControl("btnNext");
                btnNextPage.Text = Properties.Resources.btnNextPage;
                btnNextPage.ToolTip = Properties.Resources.btnNextPageTip;
                btnNextPage.Click += BtnNextPageClick;

                Button btnNext10Page = (Button)pagerRow.Cells[0].FindControl("btnNext10");
                btnNext10Page.Text = Properties.Resources.btnNext10Page;
                btnNext10Page.ToolTip = Properties.Resources.btnNext10PageTip;
                btnNext10Page.Click += BtnNext10PageClick; ;
            }

        }

        private void GvFreqFilterRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;

                        #region lngTotalSN
                        if (strCell_ColumnName == "lngTotalSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngMethodSN = long.Parse(e.Row.Cells[2].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSearchMethodSN = long.Parse(e.Row.Cells[3].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSearchMethodWithSN(stuGLSearchTemp);
                            stuGLSearchTemp.LngSecFieldSN = long.Parse(e.Row.Cells[4].Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);
                            string RequestID = SetRequestId(stuGLSearchTemp);
                            if (cell.Controls.Count == 0)
                            {
                                using Button btnFunction = new Button
                                {
                                    ID = string.Format(InvariantCulture, "btnFunction_{0}", e.Row.Cells[0].Text),
                                    Text = Properties.Resources.View
                                };
                                //btnFunction.CssClass = "glbutton glbutton-grey ";
                                if (stuGLSearchTemp.LngSecFieldSN == 1)
                                {
                                    btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('{0}?action={1}&id={2}','_blank');",
                                                                                                           Properties.Resources.PageFreqResultSingle,
                                                                                                           Properties.Resources.SessionsFreqResultSingle,
                                                                                                           RequestID);
                                    Session.Add(Properties.Resources.SessionsFreqResultSingle + RequestID, stuGLSearchTemp);
                                }
                                else
                                {
                                    btnFunction.OnClientClick = string.Format(InvariantCulture, "window.open('{0}?action={1}&id={2}','_blank');",
                                                                                                           Properties.Resources.PageFreqActiveHSingle,
                                                                                                           Properties.Resources.SessionsFreqActiveHSingle,
                                                                                                           RequestID);
                                    Session.Add(Properties.Resources.SessionsFreqActiveHSingle + RequestID, stuGLSearchTemp);
                                }
                                cell.Controls.Add(btnFunction);
                            }
                        }
                        #endregion lngTotalSN

                        #region lngMethodSN
                        if (strCell_ColumnName == "lngMethodSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngMethodSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetMethodWithSN(stuGLSearchTemp);
                            cell.ToolTip = new CglMethod().SetMethodString(stuGLSearchTemp);
                        }
                        #endregion lngMethodSN

                        #region lngSearchMethodSN
                        if (strCell_ColumnName == "lngSearchMethodSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngSearchMethodSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSearchMethodWithSN(stuGLSearchTemp);
                            cell.ToolTip = new CglMethod().SetSearchMethodString(stuGLSearchTemp);
                        }
                        #endregion lngSearchMethodSN

                        #region LngSecFieldSN
                        if (strCell_ColumnName == "lngSecFieldSN")
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngSecFieldSN = long.Parse(cell.Text, InvariantCulture);
                            stuGLSearchTemp = new CglMethod().GetSecFieldWithSN(stuGLSearchTemp);
                            cell.ToolTip = stuGLSearchTemp.StrSecField;
                        }
                        #endregion LngSecFieldSN
                    }
                }
            }
        }

        protected void BtnPrev10PageClick(object sender, EventArgs e)
        {
            int intPageIndex = gvFreqFilter.PageIndex;
            intPageIndex -= 10;
            gvFreqFilter.PageIndex = (intPageIndex < 0) ? 0 : intPageIndex;
            gvFreqFilter.DataBind();
        }

        protected void BtnNext10PageClick(object sender, EventArgs e)
        {
            gvFreqFilter.PageIndex += 10;
            gvFreqFilter.DataBind();
        }

        protected void BtnPrevPageClick(object sender, EventArgs e)
        {
            int intPageIndex = gvFreqFilter.PageIndex;
            intPageIndex--;
            gvFreqFilter.PageIndex = (intPageIndex < 0) ? 0 : intPageIndex;
            gvFreqFilter.DataBind();
        }

        protected void BtnNextPageClick(object sender, EventArgs e)
        {
            gvFreqFilter.PageIndex++;
            gvFreqFilter.DataBind();
        }

        protected void PageDropDownListSelectedIndexChanged(object sender, EventArgs e)
        {
            // Retrieve the PageDropDownList DropDownList from the pager row.

            ViewState["pageList"] = (DropDownList)sender;
            // Set the PageIndex property to display that page selected by the user.
            gvFreqFilter.PageIndex = ((DropDownList)ViewState["pageList"]).SelectedIndex;
            gvFreqFilter.DataBind();
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void BtnReloadClick(object sender, EventArgs e)
        {
            Session.Remove(FreqActiveHTAllID + "dsFreqFilter");
            Session.Remove(FreqActiveHTAllID + "UpdateField");
            ((Thread)DicThreadFreqActiveHTAll[FreqActiveHTAllID]).Abort();
            DicThreadFreqActiveHTAll.Remove(FreqActiveHTAllID);
            string strUrl = string.Format(InvariantCulture, "http://{0}/Pages/{1}?action={2}&id={3}",
                                                            Request.Url.Authority,
                                                            Properties.Resources.PageFreqActiveHTAll,
                                                            Properties.Resources.SessionsFreqActiveHTAll,
                                                            _requestId);
            Response.Redirect(strUrl, true);
        }

        protected void IBInfoClick(object sender, ImageClickEventArgs e)
        {
            pnlTopData.Visible = !pnlTopData.Visible;
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void Timer1Tick(object sender, EventArgs e)
        {
            CheckThread();
        }

        private void CreatThread()
        {

            Thread ThreadFreqActiveHT = new Thread(() =>
             {
                 for (int FiledIndex = 1; FiledIndex < LstFields.Count; FiledIndex++)
                 {
                     string strFieldName = LstFields[FiledIndex];
                     if (!((DataSet)Session[FreqActiveHTAllID + "dsFreqFilter"]).Tables.Contains(strFieldName))
                     {
                         if (!DicUpdateFieldFreqActiveHTAll.Keys.Contains(FreqActiveHTAllID)) { DicUpdateFieldFreqActiveHTAll.Add(FreqActiveHTAllID, strFieldName); } else { DicUpdateFieldFreqActiveHTAll[FreqActiveHTAllID] = strFieldName; }
                         GetdsFreqSecHis(_gstuSearch, strFieldName);
                     }
                 }
             })
            {
                Name = FreqActiveHTAllID
            };
            ThreadFreqActiveHT.Start();

            DicThreadFreqActiveHTAll.Add(FreqActiveHTAllID, ThreadFreqActiveHT);
            CheckThread();
        }

        //protected void BtnCloseClick(object sender, EventArgs e)
        //{
        //    Session.Remove(FreqActiveHTID);
        //    Session.Remove(FreqActiveHTID + "dsFreqFilter");
        //    Session.Remove(FreqActiveHTID + "dsFreqSecHis");
        //    Session.Remove(FreqActiveHTID + "UpdateField");
        //    Thread ThreadFreqActiveHT = (Thread)dicThreadFreqActiveHT[FreqActiveHTID];
        //    ThreadFreqActiveHT.Abort();
        //    Response.Write("<script language='javascript'>window.close();</script>");
        //}

        private void CheckThread()
        {
            Thread ThreadFreqActiveHT = (Thread)DicThreadFreqActiveHTAll[FreqActiveHTAllID];

            if (ThreadFreqActiveHT.Name == FreqActiveHTAllID && !ThreadFreqActiveHT.IsAlive && LstFields.IndexOf(DicUpdateFieldFreqActiveHTAll[FreqActiveHTAllID]) != LstFields.Count - 1)
            {
                ThreadFreqActiveHT.Start();
            }
            if (ThreadFreqActiveHT.Name == FreqActiveHTAllID && ThreadFreqActiveHT.IsAlive)
            {
                if (!DicUpdateFieldFreqActiveHTAll.Keys.Contains(FreqActiveHTAllID)) { DicUpdateFieldFreqActiveHTAll.Add(FreqActiveHTAllID, "gen"); }
                lblArgument.Text = string.Format(InvariantCulture, "{0} {1} updating ... ", DateTime.Now.ToLongTimeString(), new ListItem(new CglFunc().ConvertFieldNameId(DicUpdateFieldFreqActiveHTAll[FreqActiveHTAllID])));
            }
            if (ThreadFreqActiveHT.Name == FreqActiveHTAllID && !ThreadFreqActiveHT.IsAlive)
            {
                //Timer1.Enabled = false;
                lblArgument.Text = string.Format(InvariantCulture, "{0} updating completed. ", DateTime.Now.ToLongTimeString());
                CurrentSearchOrderID = string.Empty;
            }
        }

        // ---------------------------------------------------------------------------------------------------------
    }
}
