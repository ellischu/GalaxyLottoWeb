using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Media;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqActiveHT01P : BasePage
    {
        private string _action;
        private string _requestId;
        private string FreqActiveHT01PID;
        private StuGLSearch _gstuSearch;
        //private DropDownList _pageList;

        public static IList<string> LstFields => lstFields;
        public static IList<string> LstFieldsF => lstFieldsF;

        private static Dictionary<string, object> DicThreadFreqActiveHT01P
        {
            get { if (dicThreadFreqActiveHT01P == null) dicThreadFreqActiveHT01P = new Dictionary<string, object>(); return dicThreadFreqActiveHT01P; }
            set => dicThreadFreqActiveHT01P = value;
        }

        private static Dictionary<string, object> dicThreadFreqActiveHT01P;

        private static readonly List<string> lstFields = CglClass.LstFieldName;
        private static readonly List<string> lstFieldsF = new List<string>() { "sglFreq05", "sglFreq10", "sglFreq25", "sglFreq50", "sglFreq100", "lngM" };

        private DataTable dtFreqFilter;


        // ---------------------------------------------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {

            //if (!_dsData.IsInitialized) { _dsData = new DataSet { Locale = InvariantCulture }; }
            SetupViewState();
            if (ViewState[FreqActiveHT01PID + "_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState[FreqActiveHT01PID + "_gstuSearch"];
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

            FreqActiveHT01PID = _action + _requestId;

            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[FreqActiveHT01PID] != null)
            {
                if (ViewState[FreqActiveHT01PID + "_gstuSearch"] == null)
                {
                    ViewState.Add(FreqActiveHT01PID + "_gstuSearch", (StuGLSearch)Session[FreqActiveHT01PID]);
                }
                else
                {
                    ViewState[FreqActiveHT01PID + "_gstuSearch"] = (StuGLSearch)Session[FreqActiveHT01PID];
                };
            }
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        private void InitialArgument()
        {
            if (ViewState[FreqActiveHT01PID + "title"] == null) { ViewState.Add(FreqActiveHT01PID + "title", string.Format(InvariantCulture, "{0}:{1}", "活性歷史總表01(預載)", new CglDBData().SetTitleString(_gstuSearch))); }
            if (ViewState[FreqActiveHT01PID + "CurrentData"] == null) { ViewState.Add(FreqActiveHT01PID + "CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch))); }
            if (ViewState[FreqActiveHT01PID + "sort"] == null) { ViewState.Add(FreqActiveHT01PID + "sort", ""); }
            if (ViewState[FreqActiveHT01PID + "select"] == null) { ViewState.Add(FreqActiveHT01PID + "select", ""); }

            if (Session[FreqActiveHT01PID + "UpdateField"] == null) { Session.Add(FreqActiveHT01PID + "UpdateField", "gen"); }
        }

        // ---------------------------------------------------------------------------------------------------------

        private List<string> GetFields()
        {
            if (Session[FreqActiveHT01PID + "Fileds"] == null)
            {
                List<string> lstCaculateFields = new List<string>();
                List<string> lstValidFields = new List<string> { "select" };
                Dictionary<string, long> dicValidFieldTotalSN = new Dictionary<string, long>();
                Dictionary<string, string> dicCurrentData = new CglData().GetCurrentDataDics(_gstuSearch);
                using (DataTable dtTotalSN = GetData(_gstuSearch))
                {
                    foreach (string field in LstFields)
                    {
                        using DataTable dtFieldTemp = field == "gen" ?
                            dtTotalSN.Select("[intSum] = 0", "[lngTotalSN] ASC").CopyToDataTable() :
                            dtTotalSN.Select(string.Format(InvariantCulture, "[{0}] = '{1}' AND [intSum] = 0", field, dicCurrentData[field]), "[lngTotalSN] ASC").CopyToDataTable();
                        StuGLSearch stuGLSearchTemp = _gstuSearch;
                        stuGLSearchTemp.LngTotalSN = dtFieldTemp.Rows.Count > 0 ? long.Parse(dtFieldTemp.Rows[0]["lngTotalSN"].ToString(), InvariantCulture) : _gstuSearch.LngTotalSN;
                        stuGLSearchTemp.FieldMode = field != "gen";
                        stuGLSearchTemp.StrCompares = field;
                        stuGLSearchTemp = new CglSearch().InitSearch(stuGLSearchTemp);
                        stuGLSearchTemp = new CglMethod().GetMethodSN(stuGLSearchTemp);
                        stuGLSearchTemp = new CglMethod().GetSearchMethodSN(stuGLSearchTemp);
                        stuGLSearchTemp = new CglMethod().GetSecFieldSN(stuGLSearchTemp);
                        using DataTable dtFreqProcess = CglFreqProcess.GetFreqProcAlls(stuGLSearchTemp, CglDBFreq.TableName.TblFreqProcess, SortOrder.Descending, 0);
                        string strlngTotalSN = dtFreqProcess.Rows[0]["lngTotalSN"].ToString();
                        string strRowsCount = dtFreqProcess.Rows.Count.ToString(InvariantCulture);
                        if (!lstCaculateFields.Contains(strlngTotalSN + strRowsCount) && dtFreqProcess.Rows.Count >= 120)
                        {
                            lstCaculateFields.Add(strlngTotalSN + strRowsCount);
                            lstValidFields.Add(field);
                            dicValidFieldTotalSN.Add(field, stuGLSearchTemp.LngTotalSN);
                        }
                    }
                }
                Session.Add(FreqActiveHT01PID + "Fileds", lstValidFields);
                Session.Add(FreqActiveHT01PID + "TotalSN", dicValidFieldTotalSN);
                return lstValidFields;
            }
            else
            {
                return (List<string>)Session[FreqActiveHT01PID + "Fileds"];
            }
        }

        private DataTable GetData(StuGLSearch stuGLSearch)
        {
            CheckData = true;
            //DataTable dtData;
            using SqlCommand sqlCommand = new SqlCommand
            {
                Connection = new SqlConnection(new CglDBData().SetDataBase(stuGLSearch.LottoType, DatabaseType.Data)),
                CommandText = "SELECT TOP(1000) * FROM [qryData] WHERE [lngTotalSN] <= @lngTotalSN ORDER BY [lngTotalSN] DESC; "
            };
            sqlCommand.Parameters.AddWithValue("lngTotalSN", stuGLSearch.LngTotalSN);
            using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            using DataTable dtReturn = new DataTable { Locale = InvariantCulture };
            sqlDataAdapter.Fill(dtReturn);
            return dtReturn;
        }

        private void ShowResult()
        {
            Page.Title = (string)ViewState[FreqActiveHT01PID + "title"];
            lblTitle.Text = (string)ViewState[FreqActiveHT01PID + "title"];

            //顯示當前資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState[FreqActiveHT01PID + "CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            ShowddlFieldName();
            ShowFreqFilter(ddlFieldName.SelectedValue);
        }

        private void ShowddlFieldName()
        {
            if (Session[FreqActiveHT01PID + "Fileds"] != null)
            {
                if (ddlFieldName.Items.Count == 0)
                {
                    foreach (string strFieldName in (List<string>)Session[FreqActiveHT01PID + "Fileds"])
                    {
                        ddlFieldName.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(strFieldName), strFieldName));
                    }
                }
            }
            else
            {
                if (!DicThreadFreqActiveHT01P.Keys.Contains(FreqActiveHT01PID + "T01")) { CreatThread(); }
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowFreqFilter(string selectedValue)
        {
            if (Session[FreqActiveHT01PID + "dsFreqFilter"] != null && ((DataSet)Session[FreqActiveHT01PID + "dsFreqFilter"]).Tables.Contains(selectedValue))
            {
                gvFreqFilter.Visible = true;
                //GetdsFreqSecHis(_gstuSearch, selectedValue);
                #region dtFreqFilter
                dtFreqFilter = ((DataSet)Session[FreqActiveHT01PID + "dsFreqFilter"]).Tables[selectedValue];
                lblFreqFilter.Text = string.Format(InvariantCulture, "{0} ({1}期)", new CglFunc().ConvertFieldNameId(selectedValue), dtFreqFilter.Rows.Count);
                dtFreqFilter.DefaultView.RowFilter = (string)ViewState[FreqActiveHT01PID + "select"];
                dtFreqFilter.DefaultView.Sort = (string)ViewState[FreqActiveHT01PID + "sort"];
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
                            cell.ToolTip = e.Row.Cells[1].Text;
                            stuGLSearchTemp.LngTotalSN = long.Parse(e.Row.Cells[1].Text, InvariantCulture);
                            stuGLSearchTemp = new CglSearch().InitSearchDataRange(stuGLSearchTemp);
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

            gvFreqFilter.PageIndex = ((DropDownList)sender).SelectedIndex;
            gvFreqFilter.DataBind();
        }

        // ---------------------------------------------------------------------------------------------------------
        protected void BtnCloseClick(object sender, EventArgs e)
        {
            Session.Remove(FreqActiveHT01PID);
            ViewState.Remove(FreqActiveHT01PID + "_gstuSearch");
            ViewState.Remove("action");
            ViewState.Remove("id");
            ReleaseMemory();
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
        }

        protected void BtnReloadClick(object sender, EventArgs e)
        {
            ReleaseMemory();

            Session[FreqActiveHT01PID] = (StuGLSearch)ViewState[FreqActiveHT01PID + "_gstuSearch"];

            Redirector();
        }

        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            Thread ThreadFreqActiveHT01 = (Thread)DicThreadFreqActiveHT01P[FreqActiveHT01PID + "T01"];
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
            Thread ThreadFreqActiveHT02 = (Thread)DicThreadFreqActiveHT01P[FreqActiveHT01PID + "T02"];
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

        // ---------------------------------------------------------------------------------------------------------

        protected void Timer1Tick(object sender, EventArgs e)
        {
            CheckThread();
        }

        private void CreatThread()
        {
            Timer1.Enabled = true;
            Thread ThreadFreqActiveHT01 = new Thread(() =>
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);
                List<string> lstFieldsTemp = GetFields();
                Dictionary<string, long> dicField = (Dictionary<string, long>)base.Session[FreqActiveHT01PID + "TotalSN"];
                for (int FiledIndex = 1; FiledIndex < lstFieldsTemp.Count; FiledIndex++)
                {
                    string strFieldName = lstFieldsTemp[FiledIndex];
                    if (base.Session[FreqActiveHT01PID + "dsFreqFilter"] == null)
                    {
                        base.Session[FreqActiveHT01PID + "dsFreqFilter"] = new DataSet() { Locale = InvariantCulture };
                        base.Session[FreqActiveHT01PID + "WorkPool"] = new List<string>();
                    }
                    if (!((List<string>)base.Session[FreqActiveHT01PID + "WorkPool"]).Contains(strFieldName))
                    {
                        ((List<string>)base.Session[FreqActiveHT01PID + "WorkPool"]).Add(strFieldName);
                        if (!((DataSet)base.Session[FreqActiveHT01PID + "dsFreqFilter"]).Tables.Contains(strFieldName))
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngTotalSN = dicField[strFieldName];
                            GetdsFreqSecHis(stuGLSearchTemp, strFieldName, "T01");
                        }
                    }
                }
            })
            {
                Name = FreqActiveHT01PID + "T01"
            };
            ThreadFreqActiveHT01.Start();
            DicThreadFreqActiveHT01P.Add(FreqActiveHT01PID + "T01", ThreadFreqActiveHT01);

            Thread.Sleep(30000);

            Thread ThreadFreqActiveHT02 = new Thread(() =>
            {
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);

                List<string> lstFieldsTemp = GetFields();
                Dictionary<string, long> dicField = (Dictionary<string, long>)base.Session[FreqActiveHT01PID + "TotalSN"];
                for (int FiledIndex = 1; FiledIndex < lstFieldsTemp.Count; FiledIndex++)
                {
                    string strFieldName = lstFieldsTemp[FiledIndex];
                    if (base.Session[FreqActiveHT01PID + "dsFreqFilter"] == null)
                    {
                        base.Session[FreqActiveHT01PID + "dsFreqFilter"] = new DataSet() { Locale = InvariantCulture };
                        base.Session[FreqActiveHT01PID + "WorkPool"] = new List<string>();
                    }
                    if (!((List<string>)base.Session[FreqActiveHT01PID + "WorkPool"]).Contains(strFieldName))
                    {
                        ((List<string>)base.Session[FreqActiveHT01PID + "WorkPool"]).Add(strFieldName);
                        if (!((DataSet)base.Session[FreqActiveHT01PID + "dsFreqFilter"]).Tables.Contains(strFieldName))
                        {
                            StuGLSearch stuGLSearchTemp = _gstuSearch;
                            stuGLSearchTemp.LngTotalSN = dicField[strFieldName];
                            GetdsFreqSecHis(stuGLSearchTemp, strFieldName, "T02");
                        }
                    }
                }
            })
            {
                Name = FreqActiveHT01PID + "T02"
            };
            ThreadFreqActiveHT02.Start();
            DicThreadFreqActiveHT01P.Add(FreqActiveHT01PID + "T02", ThreadFreqActiveHT02);
        }

        private void CheckThread()
        {

            Thread ThreadFreqActiveHT01 = (Thread)DicThreadFreqActiveHT01P[FreqActiveHT01PID + "T01"];
            Thread ThreadFreqActiveHT02 = (Thread)DicThreadFreqActiveHT01P[FreqActiveHT01PID + "T02"];
            //List<string> lstFieldsTemp = (List<string>)ViewState[FreqActiveHT01ID + "Fileds"];

            if (ThreadFreqActiveHT01.IsAlive || ThreadFreqActiveHT02.IsAlive)
            {
                lblArgument.Text = string.Format(InvariantCulture, "{0}", DateTime.Now.ToLongTimeString());
                btnT1Start.Text = string.Format(InvariantCulture, "T1:{0} {1}", (string)Session[FreqActiveHT01PID + "strSecField" + "T01"], new GalaxyApp().GetTheadState(ThreadFreqActiveHT01.ThreadState));
                btnT2Start.Text = string.Format(InvariantCulture, "T2:{0} {1}", (string)Session[FreqActiveHT01PID + "strSecField" + "T02"], new GalaxyApp().GetTheadState(ThreadFreqActiveHT02.ThreadState));
            }
            else
            {
                lblArgument.Text = string.Format(InvariantCulture, "{0} 更新完畢. ", DateTime.Now.ToLongTimeString());
                btnT1Start.Visible = false;
                btnT2Start.Visible = false;
                Timer1.Enabled = false;
                new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
                ResetSearchOrder(FreqActiveHT01PID);
            }
        }

        private void GetdsFreqSecHis(StuGLSearch stuGLSearch, string selectedValue, string ThreadT)
        {
            if (!((DataSet)Session[FreqActiveHT01PID + "dsFreqFilter"]).Tables.Contains(selectedValue))
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

                        using DataTable dtFreqFilterTemp = new CglFreqFilter().GetFreqFilter(stuGLSearchTemp, CglFreqFilter.TableName.QryFreqFilter);
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
                        Session[FreqActiveHT01PID + "strSecField" + ThreadT] = string.Format(InvariantCulture, "{0}({1}) : ({2}/{3}) [{4}/{5}]", new CglFunc().ConvertFieldNameId(selectedValue), stuGLSearch.LngTotalSN, intCom, LstFieldsF.Count, lstSecFieldAll.IndexOf(strSecField) + 1, lstSecFieldAll.Count);
                    }
                }
                Session[FreqActiveHT01PID + "strSecField" + ThreadT] = string.Format(InvariantCulture, "{0} : 完成 ", new CglFunc().ConvertFieldNameId(selectedValue));
                if (dtFreqFilterTemp00 == null)
                {
                    if (!((DataSet)Session[FreqActiveHT01PID + "dsFreqFilter"]).Tables.Contains(selectedValue))
                    {
                        ((DataSet)Session[FreqActiveHT01PID + "dsFreqFilter"]).Tables.Add(new DataTable(selectedValue));
                    }
                }
                else
                {
                    if (!((DataSet)Session[FreqActiveHT01PID + "dsFreqFilter"]).Tables.Contains(selectedValue))
                    {
                        dtFreqFilterTemp00.Locale = InvariantCulture;
                        dtFreqFilterTemp00.TableName = selectedValue;
                        ((DataSet)Session[FreqActiveHT01PID + "dsFreqFilter"]).Tables.Add(dtFreqFilterTemp00);
                    }
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void Redirector()
        {
            string strUrl = string.Format(InvariantCulture, "http://{0}/Pages/{1}?action={2}&id={3}",
                                                            Request.Url.Authority,
                                                            Properties.Resources.PageFreqActiveHT01P,
                                                            Properties.Resources.SessionsFreqActiveHT01P,
                                                            (string)ViewState["id"]);
            Response.Redirect(strUrl, true);
        }

        private void ReleaseMemory()
        {
            Session.Remove(FreqActiveHT01PID + "dsFreqFilter");
            Session.Remove(FreqActiveHT01PID + "dsFreqSecHis");
            Session.Remove(FreqActiveHT01PID + "UpdateField");
            Session.Remove(FreqActiveHT01PID + "WorkPool");
            Session.Remove(FreqActiveHT01PID + "TotalSN");
            Session.Remove(FreqActiveHT01PID + "Fileds");
            Session.Remove(FreqActiveHT01PID + "strSecField" + "T01");
            Session.Remove(FreqActiveHT01PID + "strSecField" + "T02");
            ResetSearchOrder(FreqActiveHT01PID);
            if (DicThreadFreqActiveHT01P.Keys.Contains(FreqActiveHT01PID + "T01"))
            {
                Thread ThreadFreqActiveHT01 = (Thread)DicThreadFreqActiveHT01P[FreqActiveHT01PID + "T01"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActiveHT01.ThreadState == ThreadState.Suspended) { ThreadFreqActiveHT01.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT01.Abort();
                ThreadFreqActiveHT01.Join();
                DicThreadFreqActiveHT01P.Remove(FreqActiveHT01PID + "T01");
            }

            if (DicThreadFreqActiveHT01P.Keys.Contains(FreqActiveHT01PID + "T02"))
            {
                Thread ThreadFreqActiveHT02 = (Thread)DicThreadFreqActiveHT01P[FreqActiveHT01PID + "T02"];
#pragma warning disable CS0618 // 類型或成員已經過時
                if (ThreadFreqActiveHT02.ThreadState == ThreadState.Suspended) { ThreadFreqActiveHT02.Resume(); }
#pragma warning restore CS0618 // 類型或成員已經過時
                ThreadFreqActiveHT02.Abort();
                ThreadFreqActiveHT02.Join();
                DicThreadFreqActiveHT01P.Remove(FreqActiveHT01PID + "T02");
            }
        }
    }
}
