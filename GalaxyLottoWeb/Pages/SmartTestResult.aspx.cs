using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using static GalaxyLotto.ClassLibrary.CglFunc;

namespace GalaxyLottoWeb.Pages
{
    public partial class SmartTestResult : BasePage
    {
        private StuGLSearch _gstuSearch;
        private string _action, _requestId, test;
        private string SmartTestResultID;


        //MultiView mlViews = new MultiView();

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            if (_gstuSearch.LottoType == TargetTable.None || _gstuSearch.LngTotalSN == 0)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                _gstuSearch = (StuGLSearch)ViewState["_gstuSearch"];
                if (ViewState["title"] == null)
                {
                    ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "遺漏數字振盪表", new CglDBData().SetTitleString(_gstuSearch)));
                }
                Page.Title = ViewState["title"].ToString();
                ShowResult();
            }
            CurrentSearchOrderID = string.Empty;
        }

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            SmartTestResultID = _action + _requestId;

            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[SmartTestResultID] != null)
            {
                if (ViewState["_gstuSearch"] == null)
                {
                    ViewState.Add("_gstuSearch", (StuGLSearch)Session[SmartTestResultID]);
                }
                else
                {
                    ViewState["_gstuSearch"] = (StuGLSearch)Session[SmartTestResultID];
                };
            }

            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }


        private void ShowResult()
        {
            CglDataSet DataSet00 = new CglDataSet(_gstuSearch.LottoType);

            // 當期資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch)), true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            #region 測試號碼
            Label lblTestNum = new GalaxyApp().CreatLabel("lblTestNum", string.Format(InvariantCulture, "測試號碼({0})支  :  {1} ", _gstuSearch.StrSmartTests.Split(',').ToList().Count, _gstuSearch.StrSmartTests), "gllabel");
            pnlDetail.Controls.Add(lblTestNum);
            #endregion

            #region 設定處理動作
            List<string> lstAction = new List<string>();
            Dictionary<string, ChoseAndHit> dicChoseAndHit = new Dictionary<string, ChoseAndHit>
            {
                { "C2H2", ChoseAndHit.Chose2Hit2 },
                { "C3H3", ChoseAndHit.Chose3Hit3 },
                { "C4H3", ChoseAndHit.Chose4Hit3 },
                { "C5H4", ChoseAndHit.Chose5Hit4 },
                { "C5H3", ChoseAndHit.Chose5Hit3 },
            };
            switch (_gstuSearch.LottoType)
            {
                case TargetTable.Lotto539:
                    lstAction = new List<string> { "all", "C2H2", "C3H3", "C4H3", "C5H4", "C5H3" };
                    break;
                case TargetTable.LottoBig:
                    lstAction = new List<string> { "all" };
                    break;
                case TargetTable.LottoSix:
                    lstAction = new List<string> { "all" };
                    break;
                case TargetTable.LottoDafu:
                    lstAction = new List<string> { "all" };
                    break;
            };
            #endregion
            //int intView = 0;
            foreach (string straction in lstAction)
            {
                List<string> lstSmartTest;
                Panel pnlAction = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnl{0}", straction), "max-width");
                pnlDetail.Controls.Add(pnlAction);
                if (straction == "all")
                {
                    #region Button
                    HyperLink btButton = new GalaxyApp().CreatHyperLink(string.Format(InvariantCulture, "hl{0}", straction),
                                                        "glbutton glbutton-grey ",
                                                        string.Format(InvariantCulture, "{0}", straction),
                                                        new Uri(string.Format(InvariantCulture, "#pnl{0}", straction)));

                    pnlButtons.Controls.Add(btButton);
                    #endregion Button
                    lstSmartTest = new CglFunc().Combination(_gstuSearch.StrSmartTests.Split(','), DataSet00.CountNumber).TrimEnd(';').Split(';').ToList();

                    Label lblAction = new GalaxyApp().CreatLabel("lblAction", string.Format(InvariantCulture, "[{0}] 支數:{1} ", straction, lstSmartTest.Count), "gllabel");
                    pnlAction.Controls.Add(lblAction);

                    lblAction = new GalaxyApp().CreatLabel("lblAction", string.Format(InvariantCulture, " 投資金額:{0:N0} ", lstSmartTest.Count * DataSet00.DblRCos), "gllabel");
                    pnlAction.Controls.Add(lblAction);

                }
                else
                {
                    lstSmartTest = (List<string>)GetSmartSet(_gstuSearch, _gstuSearch.StrSmartTests, dicChoseAndHit[straction]);
                    if (ViewState[straction] == null) { ViewState.Add(straction, GetHitTable(_gstuSearch, lstSmartTest)); }

                    #region Button
                    HyperLink btButton = new GalaxyApp().CreatHyperLink(string.Format(InvariantCulture, "hl{0}", straction),
                                                        "glbutton glbutton-grey ",
                                                        string.Format(InvariantCulture, "{0}", straction),
                                                        new Uri(string.Format(InvariantCulture, "#pnl{0}", straction)));
                    pnlButtons.Controls.Add(btButton);

                    #endregion Button

                    DataTable dtSmartTest = (DataTable)ViewState[straction];
                    dtSmartTest.TableName = straction;

                    Label lblAction = new GalaxyApp().CreatLabel("lblAction", string.Format(InvariantCulture, "[{0}] 支數:{1} ", straction, lstSmartTest.Count), "gllabel");
                    pnlAction.Controls.Add(lblAction);
                    lblAction = new GalaxyApp().CreatLabel("lblAction01", string.Format(InvariantCulture, " 投資金額:{0:N0} ", lstSmartTest.Count * DataSet00.DblRCos), "gllabel");
                    pnlAction.Controls.Add(lblAction);
                    double dblsum;
                    if (lstSmartTest.Count > 0) { dblsum = double.Parse(dtSmartTest.Compute("SUM([HitMoney])", string.Empty).ToString(), InvariantCulture); }
                    else { dblsum = 0; }
                    lblAction = new GalaxyApp().CreatLabel("lblAction02", string.Format(InvariantCulture, " 中獎金額:{0:N0} ", dblsum), "gllabel");
                    pnlAction.Controls.Add(lblAction);

                    GridView gvTable = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "gv{0}", straction), "gltable table-hover", dtSmartTest, true, false);
                    gvTable.AllowSorting = true;
                    dtSmartTest.DefaultView.Sort = "[HitMoney] DESC";
                    gvTable.DataBind();
                    pnlAction.Controls.Add(gvTable);
                }
            }
        }
        protected void BtnSwitchViewCommand(object sender, CommandEventArgs e)
        {
            //btnTest.Text = string.Format(InvariantCulture, "Name: {0} , Arg: {1}", e.CommandName, e.CommandArgument);

            //Button btnView = (Button)sender;
            //mlViews.ActiveViewIndex = int.Parse(e.CommandArgument.ToString());

            //if (mlViews.ActiveViewIndex + 1 < mlViews.Views.Count)
            //{
            //    mlViews.ActiveViewIndex++;
            //    ViewState["view"] = mlViews.ActiveViewIndex;
            //}
            //else
            //{
            //    mlViews.ActiveViewIndex = 0;
            //    ViewState["view"] = mlViews.ActiveViewIndex;
            //}
        }

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            _action = Request["action"];
            _requestId = Request["id"];
            test = Request["test"];
            Session.Remove(name: _action + _requestId + test);
        }

    }
}