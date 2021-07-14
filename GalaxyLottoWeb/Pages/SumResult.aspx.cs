using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class SumResult : BasePage
    {
        private StuGLSearch _gstuSearch;
        private Dictionary<string, int> _dicCurrentNums;
        private string _action;
        private string _requestId;
        private string SumResultID;

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
                _dicCurrentNums = new CglData().GetDataNumsDici(_gstuSearch);
                if (ViewState["title"] == null)
                {
                    ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "和數值表", new CglDBData().SetTitleString(_gstuSearch)));
                }
                Page.Title = ViewState["title"].ToString();
                ShowResult(_gstuSearch);
            }
            CurrentSearchOrderID = string.Empty;
        }

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            SumResultID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[SumResultID] != null)
            {
                if (ViewState["_gstuSearch"] == null)
                {
                    ViewState.Add("_gstuSearch", (StuGLSearch)Session[SumResultID]);
                }
                else
                {
                    ViewState["_gstuSearch"] = (StuGLSearch)Session[SumResultID];
                };
            }

            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        private void ShowResult(StuGLSearch stuGLSearch)
        {
            lblTitle.Text = Page.Title;
            lblMethod.Text = new CglMethod().SetMethodString(stuGLSearch);
            lblSearchMethod.Text = new CglMethod().SetSearchMethodString(stuGLSearch);

            #region Current Data pnlCurrentData
            //顯示當前資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(stuGLSearch)), true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
            #endregion

            #region Import Data
            if (ViewState["dicSum"] == null)
            {
                ViewState.Add("dicSum", new CglSum().GetSumDic(stuGLSearch, CglSum.TableName.QrySum01, SortOrder.Descending));
            }

            #region Initialize 
            #endregion Initialize 
            Dictionary<string, object> dicSum = (Dictionary<string, object>)ViewState["dicSum"];
            #region Show the result

            #region  views index
            int intView = 0;
            #endregion  views index
            #region Loop
            foreach (KeyValuePair<string, DataSet> KeyVal in (Dictionary<string, DataSet>)dicSum["Sum"])
            {
                Panel pnlSum = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlSum_{0}", KeyVal.Key), "max-width");

                pnlDetail00.Controls.Add(pnlSum);
                #region SetButtons
                HyperLink btnSwitchView = new GalaxyApp().CreatHyperLink(string.Format(InvariantCulture, "btn{0}", KeyVal.Key),
                                                         "glbutton glbutton-grey",
                                                         string.Format(InvariantCulture, "{0}", KeyVal.Key),
                                                         new Uri(string.Format(InvariantCulture, "#{0}", pnlSum.ID)));

                btnSwitchView.Attributes.Add("onclick", string.Format(InvariantCulture, "twoFocus('{0}lblSum,{0}lblSumNext');", KeyVal.Key));
                //btnSwitchView.Command += BtnSwitchViewCommand;
                pnlButtons.Controls.Add(btnSwitchView);
                #endregion SetButtons

                #region Data Convert
                DataSet dsSumT = KeyVal.Value;
                DataTable dtResult = dsSumT.Tables["Result"];
                DataTable dtNextData = dsSumT.Tables["NextData"];
                #endregion Data Convert

                #region Sum Result part
                HyperLink hySum = new GalaxyApp().CreatHyperLink(string.Format(InvariantCulture, "{0}hySum", KeyVal.Key), "gllabel",
                                                                 string.Format(InvariantCulture, "{0} ({1}期)", KeyVal.Key, dtResult.Rows.Count),
                                                                 new Uri(string.Format(InvariantCulture, "#{0}lblSumNext", KeyVal.Key)));
                hySum.TabIndex = -1;

                pnlSum.Controls.Add(hySum);
                #region GridView 
                dtResult.DefaultView.Sort = "lngTotalSN DESC";
                GridView gvMain = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "gvMain{0}", KeyVal.Key), "gltable ", dtResult, false, false);
                #region Set Columns of DataGrid dgMissAll
                if (gvMain.Columns.Count == 0)
                {
                    for (int i = 0; i < dtResult.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = dtResult.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(dtResult.Columns[i].ColumnName, 1),
                            SortExpression = dtResult.Columns[i].ColumnName
                        };
                        string strColumnName = dtResult.Columns[i].ColumnName;
                        #region Css
                        if ((strColumnName.Substring(0, 4) != "lngL" && strColumnName.Substring(0, 4) != "lngM") || strColumnName == "lngMethodSN")
                        {
                            bfCell.ItemStyle.CssClass = strColumnName;
                        }
                        #endregion 
                        #region Show the number of lngL,lngS,lngM with 2 digital
                        if (strColumnName.Substring(0, 4) == "lngL" ||
                            strColumnName.Substring(0, 4) == "lngS" ||
                            strColumnName.Substring(0, 4) == "lngM")
                        {
                            bfCell.DataFormatString = "{0:d2}";
                        }
                        #endregion 
                        gvMain.Columns.Add(bfCell);
                    }
                }
                #endregion
                //gvMain.HeaderStyle.CssClass = "FixedHeader ";
                gvMain.RowDataBound += GvMissAll_RowDataBound;
                gvMain.DataBind();
                #endregion GridView
                pnlSum.Controls.Add(gvMain);
                #endregion Sum Result part

                #region Sum NextData part
                Panel pnlSumNext = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "{0}pnlSumNext", KeyVal.Key), "max-width");

                pnlDetail01.Controls.Add(pnlSumNext);
                Label lblSumNext = new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "{0}lblSumNext", KeyVal.Key), string.Format(InvariantCulture, "{0} ({1}期)", KeyVal.Key, dtNextData.Rows.Count), "gllabel");
                lblSumNext.TabIndex = -1;
                pnlSumNext.Controls.Add(lblSumNext);
                #region GridView 
                GridView gvMain01 = new GalaxyApp().CreatGridView(string.Format(InvariantCulture, "gvMain01{0}", KeyVal.Key),
                                                   "gltable",
                                                   dtNextData, true, false);
                #region Set Columns of DataGrid gvSumNextData
                if (gvMain01.Columns.Count == 0)
                {
                    for (int i = 0; i < dtNextData.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = dtNextData.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(dtNextData.Columns[i].ColumnName, 1),
                            SortExpression = dtNextData.Columns[i].ColumnName
                        };
                        string strColumnName = dtNextData.Columns[i].ColumnName;
                        #region Css
                        if ((strColumnName.Substring(0, 4) != "lngL" && strColumnName.Substring(0, 4) != "lngM") || strColumnName == "lngMethodSN")
                        {
                            bfCell.ItemStyle.CssClass = strColumnName;
                        }
                        #endregion 
                        #region Show the number of lngL,lngS,lngM with 2 digital
                        if (strColumnName.Substring(0, 4) == "lngL" ||
                            strColumnName.Substring(0, 4) == "lngS" ||
                            strColumnName.Substring(0, 4) == "lngM")
                        {
                            bfCell.DataFormatString = "{0:d2}";
                        }
                        #endregion 
                        gvMain01.Columns.Add(bfCell);
                    }
                }
                #endregion
                gvMain01.RowDataBound += GvMissAll_RowDataBound;
                gvMain01.DataBind();
                #endregion GridView
                pnlSumNext.Controls.Add(gvMain01);
                #endregion Sum NextData part

                intView++;
            }
            #endregion Loop

            #region Update the controls
            #endregion Update the controls

            #endregion Show the result;

            #endregion

        }

        //private void BtnSwitchViewCommand(object sender, CommandEventArgs e)
        //{
        //if (mvViews.ActiveViewIndex != int.Parse(e.CommandArgument.ToString()))
        //{
        //    mvViews.ActiveViewIndex = int.Parse(e.CommandArgument.ToString());
        //    mvViews01.ActiveViewIndex = int.Parse(e.CommandArgument.ToString());
        //}
        //else
        //{
        //    mvViews.ActiveViewIndex = -1;
        //    mvViews01.ActiveViewIndex = -1;
        //}
        // pnlDetail.Height = Unit.Pixel(ScreenResolution.Height - (int)pnlCurrentData.Height.Value - (int)pnlButtuns00.Height.Value - 30);
        //}

        private void GvMissAll_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        #region Set lngL , lngS
                        if (strCell_ColumnName != "lngSumSN" && (strCell_ColumnName.Substring(0, 4) == "lngL" || strCell_ColumnName.Substring(0, 4) == "lngS"))
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

                        #region Set Saturday
                        if (strCell_ColumnName == "lngDateSN")
                        {
                            string strDateSN = cell.Text;
                            if (new DateTime(int.Parse(strDateSN.Substring(0, 4), InvariantCulture), int.Parse(strDateSN.Substring(4, 2), InvariantCulture), int.Parse(strDateSN.Substring(6, 2), InvariantCulture)).DayOfWeek == DayOfWeek.Saturday)
                            {
                                e.Row.CssClass = "glSaturday";
                            }
                        }
                        #endregion Set Saturday
                    }

                }
            }
        }


        protected void BtnCloseClick(object sender, EventArgs e)
        {
            _action = Request["action"];
            _requestId = Request["id"];
            Session.Remove(name: _action + _requestId);
        }


    }
}