using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Media;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqActiveHT01 : BasePage
    {
        private string _action;
        private string _requestId;
        private string FreqActiveHT01ID;
        private StuGLSearch _gstuSearch;
        //private DropDownList _pageList;

        //public static IList<string> LstFields { get => lstFields; }
        public static IList<string> LstFieldsF => lstFieldsF;
        private static readonly List<string> lstFieldsF = new List<string>() { "sglFreq05", "sglFreq10", "sglFreq25", "sglFreq50", "sglFreq100", "lngM" };

        private static Dictionary<string, object> DicThreadFreqActiveHT01
        {
            get
            {
                if (dicThreadFreqActiveHT01 == null) { dicThreadFreqActiveHT01 = new Dictionary<string, object>(); }
                return dicThreadFreqActiveHT01;
            }
            set => dicThreadFreqActiveHT01 = value;
        }
        private static Dictionary<string, object> dicThreadFreqActiveHT01;
        private Thread Thread01, Thread02;

        private DataTable dtFreqFilter;


        // ---------------------------------------------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {

            //if (!_dsData.IsInitialized) { _dsData = new DataSet { Locale = InvariantCulture }; }
            SetupViewState();
            if (ViewState[FreqActiveHT01ID + "_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState[FreqActiveHT01ID + "_gstuSearch"];
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

            FreqActiveHT01ID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqActiveHT01ID] != null)
            {
                if (ViewState[FreqActiveHT01ID + "_gstuSearch"] == null)
                {
                    ViewState.Add(FreqActiveHT01ID + "_gstuSearch", (StuGLSearch)Session[FreqActiveHT01ID]);
                }
                else
                {
                    ViewState[FreqActiveHT01ID + "_gstuSearch"] = (StuGLSearch)Session[FreqActiveHT01ID];
                };
            }
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        private void InitialArgument()
        {
            if (ViewState[FreqActiveHT01ID + "title"] == null) { ViewState.Add(FreqActiveHT01ID + "title", string.Format(InvariantCulture, "{0}:{1}", "活性歷史總表01", new CglDBData().SetTitleString(_gstuSearch))); }
            if (ViewState[FreqActiveHT01ID + "CurrentData"] == null) { ViewState.Add(FreqActiveHT01ID + "CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch))); }
            if (ViewState[FreqActiveHT01ID + "sort"] == null) { ViewState.Add(FreqActiveHT01ID + "sort", ""); }
            if (ViewState[FreqActiveHT01ID + "select"] == null) { ViewState.Add(FreqActiveHT01ID + "select", ""); }
            //if (Session[FreqActiveHT01ID + "Fileds"] == null) { GetFields(); }
            if (Session[FreqActiveHT01ID + "UpdateField"] == null) { Session.Add(FreqActiveHT01ID + "UpdateField", "gen"); }
            //if (DicThreadFreqActiveHT01 == null) { DicThreadFreqActiveHT01 = new Dictionary<string, object>(); }
        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowResult()
        {
            ShowTitle();
            ShowFieldName();
            ShowFreqFilter(ddlFieldName.SelectedValue);
        }

        private void ShowTitle()
        {
            Page.Title = (string)ViewState[FreqActiveHT01ID + "title"];
            lblTitle.Text = (string)ViewState[FreqActiveHT01ID + "title"];

            //顯示當前資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState[FreqActiveHT01ID + "CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
        }

        private void ShowFieldName()
        {
            if (Session[FreqActiveHT01ID + "Fileds"] != null)
            {
                if (ddlFieldName.Items.Count == 0)
                {
                    foreach (string strFieldName in (List<string>)Session[FreqActiveHT01ID + "Fileds"])
                    {
                        ddlFieldName.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(strFieldName), strFieldName));
                    }
                }
            }
            else
            {
                if (!DicThreadFreqActiveHT01.Keys.Contains(FreqActiveHT01ID + "T01")) { CreatThread(); }
            }
        }

        private List<string> GetFields()
        {
            if (Session[FreqActiveHT01ID + "Fileds"] == null)
            {
                List<string> lstShowFields = new List<string> { "select" };
                foreach (string field in (List<string>)new CglValidFields().GetValidFieldsLst(_gstuSearch)) { lstShowFields.Add(field); }
                Session.Add(FreqActiveHT01ID + "Fileds", lstShowFields);
                return lstShowFields;
            }
            else
            {
                return (List<string>)Session[FreqActiveHT01ID + "Fileds"];
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowFreqFilter(string selectedValue)
        {
            if (Session[FreqActiveHT01ID + "dsFreqFilter"] != null && ((DataSet)Session[FreqActiveHT01ID + "dsFreqFilter"]).Tables.Contains(selectedValue))
            {
                gvFreqFilter.Visible = true;
                //GetdsFreqSecHis(_gstuSearch, selectedValue);
                #region dtFreqFilter
                dtFreqFilter = ((DataSet)Session[FreqActiveHT01ID + "dsFreqFilter"]).Tables[selectedValue];
                lblFreqFilter.Text = string.Format(InvariantCulture, "{0} ({1}期)", new CglFunc().ConvertFieldNameId(selectedValue), dtFreqFilter.Rows.Count);
                dtFreqFilter.DefaultView.RowFilter = (string)ViewState[FreqActiveHT01ID + "select"];
                dtFreqFilter.DefaultView.Sort = (string)ViewState[FreqActiveHT01ID + "sort"];
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
                    }
                }
                gvFreqFilter.RowDataBound += GvFreqFilter_RowDataBound;
                gvFreqFilter.DataBound += GvFreqFilter_DataBound;
                gvFreqFilter.DataBind();
                #endregion dtFreqFilter
            }
            else
            {
                lblFreqFilter.Text = string.Format(InvariantCulture, "{0} ({1})", new CglFunc().ConvertFieldNameId(selectedValue), "NoUpdate");
                gvFreqFilter.Visible = false;
            }
        }

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

        // ---------------------------------------------------------------------------------------------------------

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
        protected void BtnCloseClick(object sender, EventArgs e)
        {
            Session.Remove(FreqActiveHT01ID);
            ViewState.Remove(FreqActiveHT01ID + "_gstuSearch");
            ReleaseMemory();
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
        }

        protected void BtnReloadClick(object sender, EventArgs e)
        {
            ReleaseMemory();
            Session[FreqActiveHT01ID] = (StuGLSearch)ViewState[FreqActiveHT01ID + "_gstuSearch"];
            Redirector();
        }

        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            Thread01 = (Thread)DicThreadFreqActiveHT01[FreqActiveHT01ID + "T01"];
            if (Thread01.ThreadState == ThreadState.Running)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Pause);
#pragma warning disable CS0618 // 類型或成員已經過時
                Thread01.Suspend();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
            if (Thread01.ThreadState == ThreadState.Suspended)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
#pragma warning disable CS0618 // 類型或成員已經過時
                Thread01.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
        }

        protected void BtnT2StartClick(object sender, EventArgs e)
        {
            Thread02 = (Thread)DicThreadFreqActiveHT01[FreqActiveHT01ID + "T02"];
            if (Thread02.ThreadState == ThreadState.Running)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Pause);
#pragma warning disable CS0618 // 類型或成員已經過時
                Thread02.Suspend();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
            if (Thread02.ThreadState == ThreadState.Suspended)
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
#pragma warning disable CS0618 // 類型或成員已經過時
                Thread02.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void Timer1Tick(object sender, EventArgs e)
        {
            CheckThread();
        }

        private void CreatThread()
        {
            Timer1.Enabled = true;

            Thread01 = new Thread(() =>
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);

                List<string> lstFieldsTemp = GetFields();
                for (int FiledIndex = 1; FiledIndex < lstFieldsTemp.Count; FiledIndex++)
                {
                    string strFieldName = lstFieldsTemp[FiledIndex];
                    if (base.Session[FreqActiveHT01ID + "dsFreqFilter"] == null)
                    {
                        base.Session[FreqActiveHT01ID + "dsFreqFilter"] = new DataSet() { Locale = InvariantCulture };
                        base.Session[FreqActiveHT01ID + "WorkPool"] = new List<string>();
                    }
                    if (!((List<string>)base.Session[FreqActiveHT01ID + "WorkPool"]).Contains(strFieldName))
                    {
                        ((List<string>)base.Session[FreqActiveHT01ID + "WorkPool"]).Add(strFieldName);
                        if (!((DataSet)base.Session[FreqActiveHT01ID + "dsFreqFilter"]).Tables.Contains(strFieldName))
                        {
                            GetdsFreqSecHis(_gstuSearch, strFieldName, "T01");
                        }
                    }
                }
            })
            {
                Name = FreqActiveHT01ID + "T01"
            };
            Thread01.Start();
            DicThreadFreqActiveHT01.Add(FreqActiveHT01ID + "T01", Thread01);

            Thread.Sleep(30000);

            Thread02 = new Thread(() =>
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);

                List<string> lstFieldsTemp = GetFields();
                for (int FiledIndex = 1; FiledIndex < lstFieldsTemp.Count; FiledIndex++)
                {
                    string strFieldName = lstFieldsTemp[FiledIndex];
                    if (base.Session[FreqActiveHT01ID + "dsFreqFilter"] == null)
                    {
                        base.Session[FreqActiveHT01ID + "dsFreqFilter"] = new DataSet() { Locale = InvariantCulture };
                        base.Session[FreqActiveHT01ID + "WorkPool"] = new List<string>();
                    }
                    if (!((List<string>)base.Session[FreqActiveHT01ID + "WorkPool"]).Contains(strFieldName))
                    {
                        ((List<string>)base.Session[FreqActiveHT01ID + "WorkPool"]).Add(strFieldName);
                        if (!((DataSet)base.Session[FreqActiveHT01ID + "dsFreqFilter"]).Tables.Contains(strFieldName))
                        {
                            GetdsFreqSecHis(_gstuSearch, strFieldName, "T02");
                        }
                    }
                }
            })
            {
                Name = FreqActiveHT01ID + "T02"
            };
            Thread02.Start();
            DicThreadFreqActiveHT01.Add(FreqActiveHT01ID + "T02", Thread02);
        }

        private void CheckThread()
        {

            Thread01 = (Thread)DicThreadFreqActiveHT01[FreqActiveHT01ID + "T01"];
            Thread02 = (Thread)DicThreadFreqActiveHT01[FreqActiveHT01ID + "T02"];
            //List<string> lstFieldsTemp = (List<string>)ViewState[FreqActiveHT01ID + "Fileds"];

            if (Thread01.IsAlive || Thread02.IsAlive)
            {
                lblArgument.Text = string.Format(InvariantCulture, "{0}", DateTime.Now.ToLongTimeString());
                btnT1Start.Text = string.Format(InvariantCulture, "T1:{0} {1}", (string)Session[FreqActiveHT01ID + "strSecField" + "T01"], new GalaxyApp().GetTheadState(Thread01.ThreadState));
                btnT2Start.Text = string.Format(InvariantCulture, "T2:{0} {1}", (string)Session[FreqActiveHT01ID + "strSecField" + "T02"], new GalaxyApp().GetTheadState(Thread02.ThreadState));
                Timer1.Enabled = true;
                Timer1.Interval = 10000;
            }
            else
            {
                lblArgument.Text = string.Format(InvariantCulture, "{0} 更新完畢. ", DateTime.Now.ToLongTimeString());
                btnT1Start.Visible = false;
                btnT2Start.Visible = false;
                Timer1.Enabled = false;
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
                ResetSearchOrder(FreqActiveHT01ID);
            }
        }

        private void GetdsFreqSecHis(StuGLSearch stuGLSearch, string selectedValue, string ThreadT)
        {
            if (!((DataSet)Session[FreqActiveHT01ID + "dsFreqFilter"]).Tables.Contains(selectedValue))
            {
                DataTable dtFreqFilterTemp00 = null;
                for (int intCom = 1; intCom <= LstFieldsF.Count; intCom++)
                {
                    List<string> lstSecFieldAll = new CglFunc().Combination(LstFieldsF.ToArray(), intCom).TrimEnd(';').Split(';').ToList();
                    foreach (string strSecField in lstSecFieldAll)
                    {
                        StuGLSearch stuGLSearchTemp = stuGLSearch;
                        stuGLSearchTemp.FieldMode = selectedValue != "gen";
                        stuGLSearchTemp.StrCompares = selectedValue;
                        List<string> lstSecField = strSecField.Split('#').ToList();
                        lstSecField.Sort();
                        stuGLSearchTemp.StrSecField = string.Join("#", lstSecField.Distinct());

                        stuGLSearchTemp = new CglSearch().InitSearch(stuGLSearchTemp);
                        stuGLSearchTemp = new CglMethod().GetMethodSN(stuGLSearchTemp);
                        stuGLSearchTemp = new CglMethod().GetSearchMethodSN(stuGLSearchTemp);
                        stuGLSearchTemp = new CglMethod().GetSecFieldSN(stuGLSearchTemp);

                        using (DataTable dtFreqFilterTemp = new CglFreqFilter().GetFreqFilter(stuGLSearchTemp, CglFreqFilter.TableName.QryFreqFilter))
                        {
                            int FreqFilterNumCount = int.Parse(dtFreqFilterTemp.Rows[0]["intNumCount"].ToString(), InvariantCulture);
                            if (FreqFilterNumCount >= 9 && FreqFilterNumCount <= 11)
                            {
                                using DataTable dtFreqFilter01Temp = new CglFreqFilter().GetFreqFilter(stuGLSearchTemp, CglFreqFilter.TableName.QryFreqFilter01);
                                if (dtFreqFilter01Temp.Columns.Contains("TestPeriods")) { dtFreqFilter01Temp.Columns.Remove("TestPeriods"); }
                                if (dtFreqFilterTemp00 == null) { dtFreqFilterTemp00 = dtFreqFilter01Temp.Clone(); }
                                foreach (DataRow drInput in dtFreqFilter01Temp.Rows)
                                {
                                    dtFreqFilterTemp00.ImportRow(drInput);
                                }
                            }
                        }
                        Session[FreqActiveHT01ID + "strSecField" + ThreadT] = string.Format(InvariantCulture, "{0} : ({1}/{2}) [{3}/{4}]", new CglFunc().ConvertFieldNameId(selectedValue), intCom, LstFieldsF.Count, lstSecFieldAll.IndexOf(strSecField) + 1, lstSecFieldAll.Count);
                    }
                }
                Session[FreqActiveHT01ID + "strSecField" + ThreadT] = string.Format(InvariantCulture, "{0} : 完成 ", new CglFunc().ConvertFieldNameId(selectedValue));
                if (!((DataSet)Session[FreqActiveHT01ID + "dsFreqFilter"]).Tables.Contains(selectedValue))
                {
                    if (dtFreqFilterTemp00 == null)
                    {
                        ((DataSet)Session[FreqActiveHT01ID + "dsFreqFilter"]).Tables.Add(new DataTable(selectedValue));
                    }
                    else
                    {
                        dtFreqFilterTemp00.Locale = InvariantCulture;
                        dtFreqFilterTemp00.TableName = selectedValue;
                        ((DataSet)Session[FreqActiveHT01ID + "dsFreqFilter"]).Tables.Add(dtFreqFilterTemp00);
                    }
                }
            }

        }

        // ---------------------------------------------------------------------------------------------------------

        private void Redirector()
        {
            string strUrl = string.Format(InvariantCulture, "http://{0}/Pages/{1}?action={2}&id={3}",
                                                            Request.Url.Authority,
                                                            Properties.Resources.PageFreqActiveHT01,
                                                            Properties.Resources.SessionsFreqActiveHT01,
                                                            _requestId);
            Response.Redirect(strUrl, true);
        }

        private void ReleaseMemory()
        {
            Session.Remove(FreqActiveHT01ID + "dsFreqFilter");
            Session.Remove(FreqActiveHT01ID + "dsFreqSecHis");
            Session.Remove(FreqActiveHT01ID + "UpdateField");
            Session.Remove(FreqActiveHT01ID + "WorkPool");
            Session.Remove(FreqActiveHT01ID + "strSecField" + "T01");
            Session.Remove(FreqActiveHT01ID + "strSecField" + "T02");
            ResetSearchOrder(FreqActiveHT01ID);
            if (DicThreadFreqActiveHT01.Keys.Contains(FreqActiveHT01ID + "T01"))
            {
                Thread01 = (Thread)DicThreadFreqActiveHT01[FreqActiveHT01ID + "T01"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (Thread01.ThreadState == ThreadState.Suspended) { Thread01.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                Thread01.Abort();
                Thread01.Join();
                DicThreadFreqActiveHT01.Remove(FreqActiveHT01ID + "T01");
            }

            if (DicThreadFreqActiveHT01.Keys.Contains(FreqActiveHT01ID + "T02"))
            {
                Thread02 = (Thread)DicThreadFreqActiveHT01[FreqActiveHT01ID + "T02"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (Thread02.ThreadState == ThreadState.Suspended) { Thread02.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                Thread02.Abort();
                Thread02.Join();
                DicThreadFreqActiveHT01.Remove(FreqActiveHT01ID + "T02");
            }
        }
    }
}
