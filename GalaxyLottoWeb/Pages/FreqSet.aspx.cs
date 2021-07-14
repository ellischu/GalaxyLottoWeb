using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FreqSet : BasePage
    {
        private StuGLSearch _gstuSearch;
        private List<int> _lstCurrentNums;
        private string _action;
        private string _requestId;
        private DataSet dsFreqSet;
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
                    if (ViewState["title"] == null) { ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "頻率組總表", new CglDBData().SetTitleString(_gstuSearch))); }
                    if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch))); }
                    if (ViewState["_lstCurrentNums"] == null) { ViewState.Add("_lstCurrentNums", (List<int>)new CglData().GetDataNumsLst(_gstuSearch)); }
                    if (ViewState["FreqSetDS"] == null)
                    {
                        StuGLSearch stuGLSearch = _gstuSearch;
                        stuGLSearch.StrFreqSet = "gen#strDayFive#strDayTwelve#strDayNine";
                        ViewState.Add("FreqSetDS", new CglFreqSet().GetFreqSetDS(stuGLSearch));
                    }
                }
                _lstCurrentNums = (List<int>)ViewState["_lstCurrentNums"];
                ShowResult();
            }
            CurrentSearchOrderID = string.Empty;
        }

        private void ShowResult()
        {
            Page.Title = (string)ViewState["title"];
            lblTitle.Text = (string)ViewState["title"];

            // 當期資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)base.ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            #region Import Data

            dsFreqSet = (DataSet)ViewState["FreqSetDS"];
            #endregion Import Data

            #region Show the result

            #region FreqSet Part
            #region Set panel Frequency Result
            //pnlFreq.Controls.Add(new Label() { Text = string.Format(InvariantCulture, "{0} Freq", dicFreqResult.Key) });
            Panel pnlFreqSet = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "{0}pnlFreqSet", "gen"), "max-width");

            GridView gvFreqSet = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "{0}gvFreqSet", "gen"),
                                               "gltable",
                                               dsFreqSet.Tables["dtFreqSet"], true, false);

            #region Set Columns of DataGrid gvProcess
            if (gvFreqSet.Columns.Count == 0)
            {
                for (int i = 0; i < dsFreqSet.Tables["dtFreqSet"].Columns.Count; i++)
                {
                    string strColumnName = dsFreqSet.Tables["dtFreqSet"].Columns[i].ColumnName;
                    BoundField bfCell = new BoundField()
                    {
                        DataField = strColumnName,
                        HeaderText = dsFreqSet.Tables["dtFreqSet"].Columns[i].Caption,
                        //SortExpression = strColumnName
                    };
                    gvFreqSet.Columns.Add(bfCell);
                }
            }
            #endregion
            gvFreqSet.RowDataBound += GvFreqSet_RowDataBound;
            gvFreqSet.DataBind();
            pnlFreqSet.Controls.Add(gvFreqSet);
            #endregion Set panel Frequency Result
            pnlDetail.Controls.Add(pnlFreqSet);
            #endregion FreqSet Part

            #endregion Show the result
        }

        private void GvFreqSet_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //GridView gridView = (GridView)sender;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (_lstCurrentNums.Contains(int.Parse(e.Row.Cells[0].Text, InvariantCulture)))
                {
                    e.Row.Cells[0].CssClass = "lngL1";
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------
        protected void IBInfoClick(object sender, ImageClickEventArgs e)
        {
            pnlTopData.Visible = !pnlTopData.Visible;
        }
    }
}
