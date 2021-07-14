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
    public partial class FreqActiveHT02 : BasePage
    {
        private string _action;
        private string _requestId;
        private StuGLSearch _gstuSearch;
        private string FreqActiveHT02ID;
        //private DropDownList _pageList;
        private static IList<string> LstFieldsF => new List<string>() { "sglFreq05", "sglFreq10", "sglFreq25", "sglFreq50", "sglFreq100", "lngM" };

        private static IList<string> LstFields => CglClass.LstFieldName;

        private static Dictionary<string, object> DicThreadFreqActiveHT02
        {
            get { if (dicThreadFreqActiveHT02 == null) { dicThreadFreqActiveHT02 = new Dictionary<string, object>(); } return dicThreadFreqActiveHT02; }
            set => dicThreadFreqActiveHT02 = value;
        }
        private static Dictionary<string, object> dicThreadFreqActiveHT02;

        private static Dictionary<string, string> DicUpdateFieldFreqActiveHT02 { get => dicUpdateFieldFreqActiveHT02 ?? new Dictionary<string, string>(); set => dicUpdateFieldFreqActiveHT02 = value; }
        private static Dictionary<string, string> dicUpdateFieldFreqActiveHT02;

        private DataTable DtFreqFilter { get; set; }

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

            FreqActiveHT02ID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqActiveHT02ID] != null)
            {
                if (ViewState["_gstuSearch"] == null)
                {
                    ViewState.Add("_gstuSearch", (StuGLSearch)Session[FreqActiveHT02ID]);
                }
                else
                {
                    ViewState["_gstuSearch"] = (StuGLSearch)Session[FreqActiveHT02ID];
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
            if (ViewState[FreqActiveHT02ID + "Fileds"] == null) { GetFields(); }
            if (Session[FreqActiveHT02ID + "UpdateField"] == null) { Session.Add(FreqActiveHT02ID + "UpdateField", "gen"); }
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
            ViewState.Add(FreqActiveHT02ID + "Fileds", lstShowFields);
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
            if (!DicThreadFreqActiveHT02.Keys.Contains(FreqActiveHT02ID)) { CreatThread(); }
        }

        private void ShowFreqFilter(string selectedValue)
        {
            GetdsFreqSecHis(_gstuSearch, ddlFieldName.SelectedValue);
            #region dtFreqFilter
            DtFreqFilter = ((DataSet)Session[FreqActiveHT02ID + "dsFreqFilter"]).Tables[selectedValue];
            lblFreqFilter.Text = string.Format(InvariantCulture, "{0} ({1}期)", new CglFunc().ConvertFieldNameId(selectedValue), DtFreqFilter.Rows.Count);
            DtFreqFilter.DefaultView.RowFilter = (string)ViewState["select"];
            DtFreqFilter.DefaultView.Sort = (string)ViewState["sort"];
            if (DtFreqFilter.Columns.Contains("lngDateSN")) { DtFreqFilter.Columns.Remove("lngDateSN"); }

            gvFreqFilter.DataSource = DtFreqFilter.DefaultView;
            gvFreqFilter.PagerTemplate = new PagerTemplate();
            if (gvFreqFilter.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtFreqFilter.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtFreqFilter.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtFreqFilter.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtFreqFilter.Columns[ColumnIndex].ColumnName,
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
            gvFreqFilter.RowDataBound += GvFreqFilter_RowDataBound;
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
            List<string> lstShowField = (List<string>)ViewState[FreqActiveHT02ID + "Fileds"];
            if (lstShowField.Contains(selectedValue))
            {
                if (Session[FreqActiveHT02ID + "dsFreqFilter"] != null)
                {
                    using DataSet dsTemp = (DataSet)Session[FreqActiveHT02ID + "dsFreqFilter"];
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
            using DataSet dsFreqSecHisTemp = new DataSet
            {
                Locale = InvariantCulture
            };
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
                        DataTable dtFreqFilterTemp = new CglFreqFilter().GetFreqFilter(stuGLSearchTemp, CglFreqFilter.TableName.QryFreqFilter02);
                        if (dtFreqFilterTemp.Columns.Contains("TestPeriods")) { dtFreqFilterTemp.Columns.Remove("TestPeriods"); }
                        dsFreqSecHisTemp.Tables.Add(dtFreqFilterTemp.Clone());
                        dsFreqSecHisTemp.Tables[0].TableName = "FreqFilter";
                        foreach (DataRow drInput in dtFreqFilterTemp.Rows)
                        {
                            dsFreqSecHisTemp.Tables["FreqFilter"].ImportRow(drInput);
                        }
                    }
                }
                using (DataTable dtfilterTemp = dsFreqSecHisTemp.Tables.Contains("FreqFilter") ? dsFreqSecHisTemp.Tables["FreqFilter"].Copy() : new DataTable())
                {
                    dtfilterTemp.Locale = InvariantCulture;
                    dtfilterTemp.TableName = selectedValue;
                    dsTemp.Tables.Add(dtfilterTemp);
                }
                Session[FreqActiveHT02ID + "dsFreqFilter"] = dsTemp;
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void GvFreqFilter_DataBound(object sender, EventArgs e)
        {
            if (gvFreqFilter.Rows.Count > 0)
            {
                GridViewRow pagerRow = gvFreqFilter.TopPagerRow;
                pagerRow.Visible = true;
                DropDownList _pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
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

        private void GvFreqFilter_RowDataBound(object sender, GridViewRowEventArgs e)
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
            if (sender == null) { throw new ArgumentNullException(nameof(sender)); }
            // Retrieve the PageDropDownList DropDownList from the pager row.

            // Set the PageIndex property to display that page selected by the user.
            gvFreqFilter.PageIndex = ((DropDownList)sender).SelectedIndex;
            gvFreqFilter.DataBind();
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void BtnReloadClick(object sender, EventArgs e)
        {
            Session.Remove(FreqActiveHT02ID + "dsFreqFilter");
            Session.Remove(FreqActiveHT02ID + "dsFreqSecHis");
            Session.Remove(FreqActiveHT02ID + "UpdateField");
            ((Thread)DicThreadFreqActiveHT02[FreqActiveHT02ID]).Abort();
            DicThreadFreqActiveHT02.Remove(FreqActiveHT02ID);

            string strUrl = string.Format(InvariantCulture, "http://{0}/Pages/{1}?action={2}&id={3}",
                                                            Request.Url.Authority,
                                                            Properties.Resources.PageFreqActiveHT02,
                                                            Properties.Resources.SessionsFreqActiveHT02,
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
                     if (!((DataSet)Session[FreqActiveHT02ID + "dsFreqFilter"]).Tables.Contains(strFieldName))
                     {
                         if (!DicUpdateFieldFreqActiveHT02.Keys.Contains(FreqActiveHT02ID)) { DicUpdateFieldFreqActiveHT02.Add(FreqActiveHT02ID, strFieldName); } else { DicUpdateFieldFreqActiveHT02[FreqActiveHT02ID] = strFieldName; }
                         GetdsFreqSecHis(_gstuSearch, strFieldName);
                     }
                 }
             })
            {
                Name = FreqActiveHT02ID
            };
            ThreadFreqActiveHT.Start();

            DicThreadFreqActiveHT02.Add(FreqActiveHT02ID, ThreadFreqActiveHT);
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
            Thread ThreadFreqActiveHT = (Thread)DicThreadFreqActiveHT02[FreqActiveHT02ID];

            if (ThreadFreqActiveHT.Name == FreqActiveHT02ID && !ThreadFreqActiveHT.IsAlive && LstFields.IndexOf(DicUpdateFieldFreqActiveHT02[FreqActiveHT02ID]) != LstFields.Count - 1)
            {
                ThreadFreqActiveHT.Start();
            }
            if (ThreadFreqActiveHT.Name == FreqActiveHT02ID && ThreadFreqActiveHT.IsAlive)
            {
                if (!DicUpdateFieldFreqActiveHT02.Keys.Contains(FreqActiveHT02ID)) { DicUpdateFieldFreqActiveHT02.Add(FreqActiveHT02ID, "gen"); }
                lblArgument.Text = string.Format(InvariantCulture, "{0} {1} updating ... ", DateTime.Now.ToLongTimeString(), new ListItem(new CglFunc().ConvertFieldNameId(DicUpdateFieldFreqActiveHT02[FreqActiveHT02ID])));
            }
            if (ThreadFreqActiveHT.Name == FreqActiveHT02ID && !ThreadFreqActiveHT.IsAlive)
            {
                //Timer1.Enabled = false;
                lblArgument.Text = string.Format(InvariantCulture, "{0} updating completed. ", DateTime.Now.ToLongTimeString());
                CurrentSearchOrderID = string.Empty;
            }
        }

        // ---------------------------------------------------------------------------------------------------------
    }
}
