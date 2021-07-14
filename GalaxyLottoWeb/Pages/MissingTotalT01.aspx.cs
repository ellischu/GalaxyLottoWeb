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
    public partial class MissingTotalT01 : BasePage
    {
        private StuGLSearch GlobalStuSearch;
        private string MissingTotalT01ID;
        //private List<int> _lstCurrentNums;
        //private bool _blLngM0 { get; set; } = false;

        private static Dictionary<string, object> DicThreadMissingTotalT01
        {
            get
            {
                if (dicThreadMissingTotalT01 == null) { dicThreadMissingTotalT01 = new Dictionary<string, object>(); }
                return dicThreadMissingTotalT01;
            }
            set => dicThreadMissingTotalT01 = value;
        }

        private static Dictionary<string, object> dicThreadMissingTotalT01;

        private Thread Thread01;


        // ---------------------------------------------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            if (string.IsNullOrEmpty(MissingTotalT01ID)) { MissingTotalT01ID = GetID(); }
            SetupViewState(MissingTotalT01ID);
            if (ViewState[MissingTotalT01ID + "_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                GlobalStuSearch = (StuGLSearch)ViewState[MissingTotalT01ID + "_gstuSearch"];
                InitializeArgument();
                ShowTitle();

                ShowMissSum();
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        private string GetID()
        {
            string _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            string _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null && !string.IsNullOrEmpty(_action)) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null && !string.IsNullOrEmpty(_requestId)) { ViewState.Add("id", _requestId); }

            return !string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) ? _action + _requestId : string.Empty;
        }

        private void SetupViewState(string DataID)
        {
            if (!string.IsNullOrEmpty(DataID) && ViewState[DataID + "_gstuSearch"] == null && Session[DataID] != null)
            {
                ViewState.Add(DataID + "_gstuSearch", (StuGLSearch)Session[DataID]);
            }
        }

        private void InitializeArgument()
        {
            if (ViewState[MissingTotalT01ID + "Argument"] == null)
            {
                ViewState.Add(MissingTotalT01ID + "Argument", new Dictionary<string, object>());
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("title"))
            {
                ((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).Add("title", string.Format(InvariantCulture, "{0}:{1}", "遺漏整合總表01", new CglDBData().SetTitleString(GlobalStuSearch)));
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("lblMethod"))
            {
                ((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).Add("lblMethod", new CglMethod().SetMethodString(GlobalStuSearch));
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("lblSearchMethod"))
            {
                ((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).Add("lblSearchMethod", new CglMethod().SetSearchMethodString(GlobalStuSearch));
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("CurrentData"))
            {
                ((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(GlobalStuSearch)));
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("lstCurrentNums"))
            {
                ((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).Add("lstCurrentNums", new CglData().GetDataNumsLst(GlobalStuSearch));
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("dicCurrentNums"))
            {
                ((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).Add("dicCurrentNums", new CglData().GetDataNumsDici(this.GlobalStuSearch));
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("lblMissSum200") && Session[MissingTotalT01ID + "lblMissSum200"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).Add("lblMissSum200", Session[MissingTotalT01ID + "lblMissSum200"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("dtMissSum200") && Session[MissingTotalT01ID + "dtMissSum200"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).Add("dtMissSum200", Session[MissingTotalT01ID + "dtMissSum200"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("dtMissTotalRow00200") && Session[MissingTotalT01ID + "dtMissTotalRow00200"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).Add("dtMissTotalRow00200", Session[MissingTotalT01ID + "dtMissTotalRow00200"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("lblMissSum500") && Session[MissingTotalT01ID + "lblMissSum500"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).Add("lblMissSum500", Session[MissingTotalT01ID + "lblMissSum500"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("dtMissSum500") && Session[MissingTotalT01ID + "dtMissSum500"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).Add("dtMissSum500", Session[MissingTotalT01ID + "dtMissSum500"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("dtMissTotalRow00500") && Session[MissingTotalT01ID + "dtMissTotalRow00500"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).Add("dtMissTotalRow00500", Session[MissingTotalT01ID + "dtMissTotalRow00500"]);
            }

            if (Session[MissingTotalT01ID + "lblT01"] == null) { Session[MissingTotalT01ID + "lblT01"] = string.Empty; }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowTitle()
        {
            Page.Title = (string)((Dictionary<string, object>)base.ViewState[MissingTotalT01ID + "Argument"])["title"];
            lblTitle.Text = (string)((Dictionary<string, object>)base.ViewState[MissingTotalT01ID + "Argument"])["title"];
            lblMethod.Text = (string)((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"])["lblMethod"];
            lblSearchMethod.Text = (string)((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"])["lblSearchMethod"];

            // 當期資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ",
                                                   (DataTable)((Dictionary<string, object>)base.ViewState[MissingTotalT01ID + "Argument"])["CurrentData"],
                                                   true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
            lblBriefDate.Text = new GalaxyApp().ShowBriefDate((DataTable)((Dictionary<string, object>)base.ViewState[MissingTotalT01ID + "Argument"])["CurrentData"],
                                              (List<int>)((Dictionary<string, object>)base.ViewState[MissingTotalT01ID + "Argument"])["lstCurrentNums"]);
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowMissSum()
        {
            if (((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("dtMissSum200") &&
                ((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"]).ContainsKey("dtMissSum500"))
            {
                ResetSearchOrder(MissingTotalT01ID);
                ShowMissSum200();
                ShowMissRow0200();
                ShowMissSum500();
                ShowMissRow0500();
            }
            else
            {
                if (!DicThreadMissingTotalT01.Keys.Contains(MissingTotalT01ID + "T01")) { CreatThread(); }
            }
        }


        private void ShowMissSum200()
        {
            Label lblMissSum200 = new GalaxyApp().CreatLabel("lblMissSum200",
                                             (string)((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"])["lblMissSum200"],
                                             "gllabelwhite");
            pnlDetail.Controls.Add(lblMissSum200);
            DataTable DtMissSum = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"])["dtMissSum200"];
            GridView gvMissSum200 = new GalaxyApp().CreatGridView("gvMissSum200", "gltable", DtMissSum, false, false);
            gvMissSum200.DataSource = DtMissSum.DefaultView;
            if (gvMissSum200.Columns.Count == 0)
            {
                for (int i = 0; i < DtMissSum.Columns.Count; i++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtMissSum.Columns[i].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtMissSum.Columns[i].ColumnName, 1),
                        SortExpression = DtMissSum.Columns[i].ColumnName,
                    };
                    gvMissSum200.Columns.Add(bfCell);
                }
            }
            if (gvMissSum200.Columns.Count != 0)
            {
                int intlngm = 1;
                foreach (BoundField bfCell in gvMissSum200.Columns)
                {
                    string strColumnName = bfCell.DataField;
                    #region Show the number of lngL,lngS,lngM with 2 digital
                    if (strColumnName.Substring(0, 4) == "lngN")
                    {
                        if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalT01ID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strColumnName.Substring(4), InvariantCulture)))
                        {
                            bfCell.HeaderStyle.CssClass = string.Format(InvariantCulture, "lngM{0}", intlngm);
                            bfCell.ItemStyle.CssClass = "glColNum5";
                            intlngm++;
                        }
                    }
                    #endregion
                }
            }
            gvMissSum200.RowDataBound += GvMissSum200_RowDataBound;
            gvMissSum200.DataBind();
            pnlDetail.Controls.Add(gvMissSum200);
        }

        private void GvMissSum200_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Dictionary<string, int> dicNumCount = CountNums(e.Row);
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        #region Set lngL , lngS
                        if (strCell_ColumnName.Substring(0, 4) == "lngN" || strCell_ColumnName.Substring(0, 4) == "lngM")
                        {
                            if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalT01ID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strCell_ColumnName.Substring(4), InvariantCulture)))
                            {
                                cell.CssClass = "glColNum5";
                            }
                            int value = int.Parse(cell.Text, InvariantCulture);
                            if (value < 14 && e.Row.Cells[0].Text == "合計") { cell.CssClass += " glValueMaxNum "; }
                            cell.ToolTip = dicNumCount[cell.Text].ToString(InvariantCulture);
                        }
                        #endregion Set lngL , lngS

                    }
                }
            }
        }
        // ---------------------------------------------------------------------------------------------------------
        private void ShowMissRow0200()
        {
            Label lblMissRow0200 = new GalaxyApp().CreatLabel("lblMissRow0200", "MissRow0", "gllabelwhite");
            pnlDetail.Controls.Add(lblMissRow0200);
            DataTable dtMissRow0200 = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"])["dtMissTotalRow00200"];
            GridView gvMissRow0200 = new GalaxyApp().CreatGridView("gvMissRow0200", "gltable", dtMissRow0200, false, false);
            gvMissRow0200.DataSource = dtMissRow0200.DefaultView;
            if (gvMissRow0200.Columns.Count == 0)
            {
                for (int i = 0; i < dtMissRow0200.Columns.Count; i++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = dtMissRow0200.Columns[i].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(dtMissRow0200.Columns[i].ColumnName, 1),
                        SortExpression = dtMissRow0200.Columns[i].ColumnName,
                    };
                    gvMissRow0200.Columns.Add(bfCell);
                }
            }
            if (gvMissRow0200.Columns.Count != 0)
            {
                int intlngm = 1;
                foreach (BoundField bfCell in gvMissRow0200.Columns)
                {
                    string strColumnName = bfCell.DataField;
                    #region Show the number of lngL,lngS,lngM with 2 digital
                    if (strColumnName.Substring(0, 4) == "lngM")
                    {
                        if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalT01ID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strColumnName.Substring(4), InvariantCulture)))
                        {
                            bfCell.HeaderStyle.CssClass = string.Format(InvariantCulture, "lngM{0}", intlngm);
                            intlngm++;
                        }
                    }
                    #endregion
                }
            }
            gvMissRow0200.RowDataBound += GvMissRow0200_RowDataBound; ;
            gvMissRow0200.DataBind();
            pnlDetail.Controls.Add(gvMissRow0200);
        }

        private void GvMissRow0200_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Dictionary<string, int> dicNumCount = CountNums(e.Row);
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        #region Set lngL , lngS
                        if (strCell_ColumnName.Substring(0, 4) == "lngN" || strCell_ColumnName.Substring(0, 4) == "lngM")
                        {
                            if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalT01ID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strCell_ColumnName.Substring(4), InvariantCulture)))
                            {
                                cell.CssClass = "glColNum5";
                            }
                            int value = int.Parse(cell.Text, InvariantCulture);
                            if (value > 10 && e.Row.Cells[0].Text != "合計") { cell.CssClass += " glValueMaxNum "; }
                            if (value > 90 && e.Row.Cells[0].Text == "合計") { cell.CssClass += " glValueMaxNum "; }
                            cell.ToolTip = dicNumCount[cell.Text].ToString(InvariantCulture);
                        }
                        #endregion Set lngL , lngS
                    }
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowMissSum500()
        {
            Label lblMissSum500 = new GalaxyApp().CreatLabel("lblMissSum500",
                                             (string)((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"])["lblMissSum500"],
                                             "gllabelwhite");
            pnlDetail.Controls.Add(lblMissSum500);
            DataTable DtMissSum500 = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"])["dtMissSum500"];
            GridView gvMissSum500 = new GalaxyApp().CreatGridView("gvMissSum500", "gltable", DtMissSum500, false, false);
            gvMissSum500.DataSource = DtMissSum500.DefaultView;
            if (gvMissSum500.Columns.Count == 0)
            {
                for (int i = 0; i < DtMissSum500.Columns.Count; i++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtMissSum500.Columns[i].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtMissSum500.Columns[i].ColumnName, 1),
                        SortExpression = DtMissSum500.Columns[i].ColumnName,
                    };
                    gvMissSum500.Columns.Add(bfCell);
                }
            }
            if (gvMissSum500.Columns.Count != 0)
            {
                int intlngm = 1;
                foreach (BoundField bfCell in gvMissSum500.Columns)
                {
                    string strColumnName = bfCell.DataField;
                    #region Show the number of lngL,lngS,lngM with 2 digital
                    if (strColumnName.Substring(0, 4) == "lngN")
                    {
                        if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalT01ID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strColumnName.Substring(4), InvariantCulture)))
                        {
                            bfCell.HeaderStyle.CssClass = string.Format(InvariantCulture, "lngM{0}", intlngm);
                            bfCell.ItemStyle.CssClass = "glColNum5";
                            intlngm++;
                        }
                    }
                    #endregion
                }
            }
            gvMissSum500.RowDataBound += GvMissSum500_RowDataBound;
            gvMissSum500.DataBind();
            pnlDetail.Controls.Add(gvMissSum500);
        }
        private void GvMissSum500_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Dictionary<string, int> dicNumCount = CountNums(e.Row);
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        #region Set lngL , lngS
                        if (strCell_ColumnName.Substring(0, 4) == "lngN" || strCell_ColumnName.Substring(0, 4) == "lngM")
                        {
                            if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalT01ID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strCell_ColumnName.Substring(4), InvariantCulture)))
                            {
                                cell.CssClass = "glColNum5";
                            }
                            int value = int.Parse(cell.Text, InvariantCulture);
                            if (value > 14 && e.Row.Cells[0].Text == "合計") { cell.CssClass += " glValueMaxNum "; }
                            cell.ToolTip = dicNumCount[cell.Text].ToString(InvariantCulture);
                        }
                        #endregion Set lngL , lngS

                    }
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowMissRow0500()
        {
            Label lblMissRow0500 = new GalaxyApp().CreatLabel("lblMissRow0500", "MissRow0", "gllabelwhite");
            pnlDetail.Controls.Add(lblMissRow0500);
            DataTable dtMissRow0500 = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalT01ID + "Argument"])["dtMissTotalRow00500"];
            GridView gvMissRow0500 = new GalaxyApp().CreatGridView("gvMissRow0500", "gltable", dtMissRow0500, false, false);
            gvMissRow0500.DataSource = dtMissRow0500.DefaultView;
            if (gvMissRow0500.Columns.Count == 0)
            {
                for (int i = 0; i < dtMissRow0500.Columns.Count; i++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = dtMissRow0500.Columns[i].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(dtMissRow0500.Columns[i].ColumnName, 1),
                        SortExpression = dtMissRow0500.Columns[i].ColumnName,
                    };
                    gvMissRow0500.Columns.Add(bfCell);
                }
            }
            if (gvMissRow0500.Columns.Count != 0)
            {
                int intlngm = 1;
                foreach (BoundField bfCell in gvMissRow0500.Columns)
                {
                    string strColumnName = bfCell.DataField;
                    #region Show the number of lngL,lngS,lngM with 2 digital
                    if (strColumnName.Substring(0, 4) == "lngM")
                    {
                        if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalT01ID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strColumnName.Substring(4), InvariantCulture)))
                        {
                            bfCell.HeaderStyle.CssClass = string.Format(InvariantCulture, "lngM{0}", intlngm);
                            intlngm++;
                        }
                    }
                    #endregion
                }
            }
            gvMissRow0500.RowDataBound += GvMissRow0500_RowDataBound; ;
            gvMissRow0500.DataBind();
            pnlDetail.Controls.Add(gvMissRow0500);
        }

        private void GvMissRow0500_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Dictionary<string, int> dicNumCount = CountNums(e.Row);
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        #region Set lngL , lngS
                        if (strCell_ColumnName.Substring(0, 4) == "lngN" || strCell_ColumnName.Substring(0, 4) == "lngM")
                        {
                            if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalT01ID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strCell_ColumnName.Substring(4), InvariantCulture)))
                            {
                                cell.CssClass = "glColNum5";
                            }
                            int value = int.Parse(cell.Text, InvariantCulture);
                            if (value > 10 && e.Row.Cells[0].Text != "合計") { cell.CssClass += " glValueMaxNum "; }
                            if (value > 50 && e.Row.Cells[0].Text == "合計") { cell.CssClass += " glValueMaxNum "; }
                            cell.ToolTip = dicNumCount[cell.Text].ToString(InvariantCulture);
                        }
                        #endregion Set lngL , lngS
                    }
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------

        private static Dictionary<string, int> CountNums(GridViewRow row)
        {
            Dictionary<string, int> dicReturn = new Dictionary<string, int>();
            foreach (DataControlFieldCell cell in row.Cells)
            {
                if (dicReturn.Keys.Contains(cell.Text))
                {
                    dicReturn[cell.Text]++;
                }
                else
                {
                    dicReturn.Add(cell.Text, 1);
                }
            }
            return dicReturn;
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            Session.Remove(MissingTotalT01ID);
            ViewState.Remove(MissingTotalT01ID + "_gstuSearch");
            ReleaseMemory();
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Resume")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Threading.Thread.Suspend")]
        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            if (DicThreadMissingTotalT01 != null && DicThreadMissingTotalT01.Keys.Contains(MissingTotalT01ID + "T01"))
            {
                Thread01 = (Thread)DicThreadMissingTotalT01[MissingTotalT01ID + "T01"];
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
        }

        // ---------------------------------------------------------------------------------------------------------

        protected void Timer1Tick(object sender, EventArgs e)
        {
            CheckThread();
        }

        private void CheckThread()
        {
            if (DicThreadMissingTotalT01.Keys.Contains(MissingTotalT01ID + "T01"))
            {
                Thread01 = (Thread)DicThreadMissingTotalT01[MissingTotalT01ID + "T01"];
                if (Thread01.IsAlive)
                {
                    lblArgument.Visible = true;
                    lblArgument.Text = string.Format(InvariantCulture, "{0}", DateTime.Now.ToLongTimeString());
                    btnT1Start.Visible = true;
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0} {1} ", Session[MissingTotalT01ID + "lblT01"].ToString(), new GalaxyApp().GetTheadState(Thread01.ThreadState));
                    //btnT1Start.Text = string.Format(InvariantCulture, "T1:{0} ", GetTheadState(Thread01.ThreadState));
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
            Thread01 = new Thread(() => { StartThread01(); ResetSearchOrder(MissingTotalT01ID); })
            {
                Name = MissingTotalT01ID + "T01"
            };
            Thread01.Start();
            DicThreadMissingTotalT01.Add(MissingTotalT01ID + "T01", Thread01);
        }

        private void StartThread01()
        {
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);

            #region MissSum200
            StuGLSearch stuGLSearchTemp = GlobalStuSearch;
            stuGLSearchTemp.InFieldPeriodLimit = 200;
            Session[MissingTotalT01ID + "lblMissSum200"] = string.Format(InvariantCulture, "#{0}", stuGLSearchTemp.InFieldPeriodLimit);
            using DataSet dsMissSum200 = GetDtMissSum(stuGLSearchTemp);
            Session[MissingTotalT01ID + "dtMissSum200"] = dsMissSum200.Tables["dtMissSum"];
            Session[MissingTotalT01ID + "dtMissTotalRow00200"] = dsMissSum200.Tables["dtMissTotalRow00"];
            #endregion MissSum200
            #region MissSum500
            stuGLSearchTemp = GlobalStuSearch;
            List<string> Fields = (List<string>)new CglValidFields().GetValidFieldsLst(stuGLSearchTemp);
            while (Fields.Count > 4)
            {
                stuGLSearchTemp.InFieldPeriodLimit += 10;
                Fields = (List<string>)new CglValidFields().GetValidFieldsLst(stuGLSearchTemp);
            }
            using DataSet dsMissSum500 = GetDtMissSum(stuGLSearchTemp);
            Session[MissingTotalT01ID + "lblMissSum500"] = string.Format(InvariantCulture, "#{0}", stuGLSearchTemp.InFieldPeriodLimit);
            Session[MissingTotalT01ID + "dtMissSum500"] = dsMissSum500.Tables["dtMissSum"];
            Session[MissingTotalT01ID + "dtMissTotalRow00500"] = dsMissSum500.Tables["dtMissTotalRow00"];
            #endregion MissSum500
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
        }

        private DataSet GetDtMissSum(StuGLSearch stuGLSearch)
        {
            List<string> Fields = ChangFieldOrder((List<string>)new CglValidFields().GetValidFieldsLst(stuGLSearch));
            Dictionary<string, int> dicMissN1 = new Dictionary<string, int>();
            Dictionary<string, int> dicMissN3 = new Dictionary<string, int>();
            Dictionary<string, int> dicMissN5 = new Dictionary<string, int>();
            Dictionary<string, int> dicMissN10 = new Dictionary<string, int>();
            Dictionary<string, int> dicMissN15 = new Dictionary<string, int>();
            using DataSet dsMissingTotal = new DataSet() { Locale = InvariantCulture };
            using DataTable dtMissTotalRow00 = CreatTableMissTotal(stuGLSearch);
            foreach (string strField in Fields)
            {
                StuGLSearch stuSearchTemp = stuGLSearch;
                stuSearchTemp.FieldMode = strField != "gen";
                stuSearchTemp.StrCompares = strField != "gen" ? strField : "gen";
                stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
                stuSearchTemp = new CglMethod().GetMethodSN(stuSearchTemp);
                stuSearchTemp = new CglMethod().GetSearchMethodSN(stuSearchTemp);
                stuSearchTemp = new CglMethod().GetSecFieldSN(stuSearchTemp);

                using DataTable dtMissAllTemp00 = new CglMissAll().GetMissAll00Multiple(stuSearchTemp, CglMissAll.TableName.QryMissAll0001, SortOrder.Descending);
                //if (dtMissAllTemp00 == null) { dtMissAllTemp00 = new DataTable(); }
                dtMissAllTemp00.Locale = InvariantCulture;
                dtMissAllTemp00.TableName = strField;
                dsMissingTotal.Tables.Add(dtMissAllTemp00);
                DataRow drMissTotalRow00 = dtMissTotalRow00.NewRow();
                Session[MissingTotalT01ID + "lblT01"] = string.Format(InvariantCulture, "#{0}_{1}", stuGLSearch.InFieldPeriodLimit, new CglFunc().ConvertFieldNameId(strField));
                drMissTotalRow00["strField"] = new CglFunc().ConvertFieldNameId(strField);
                #region deal with MissN
                for (int index = 1; index <= new CglDataSet(stuGLSearch.LottoType).LottoNumbers; index++)
                {
                    string strMissNum = string.Format(InvariantCulture, "lngM{0}", index);
                    if (!dicMissN1.Keys.Contains(strMissNum)) { dicMissN1.Add(strMissNum, 0); }
                    if (!dicMissN3.Keys.Contains(strMissNum)) { dicMissN3.Add(strMissNum, 0); }
                    if (!dicMissN5.Keys.Contains(strMissNum)) { dicMissN5.Add(strMissNum, 0); }
                    if (!dicMissN10.Keys.Contains(strMissNum)) { dicMissN10.Add(strMissNum, 0); }
                    if (!dicMissN15.Keys.Contains(strMissNum)) { dicMissN15.Add(strMissNum, 0); }
                    if (dtMissAllTemp00.Rows.Count > 0)
                    {
                        int intlngM = int.Parse(dtMissAllTemp00.Rows[0][strMissNum].ToString(), InvariantCulture);
                        drMissTotalRow00[strMissNum] = intlngM;
                        if (intlngM <= 1) { dicMissN1[strMissNum]++; }
                        if (intlngM <= 3) { dicMissN3[strMissNum]++; }
                        if (intlngM <= 5) { dicMissN5[strMissNum]++; }
                        if (intlngM <= 10) { dicMissN10[strMissNum]++; }
                        if (intlngM <= 15) { dicMissN15[strMissNum]++; }
                    }
                }
                dtMissTotalRow00.Rows.Add(drMissTotalRow00);
                #endregion deal with MissN
            }
            dsMissingTotal.Tables.Add(CountMissRow0(dtMissTotalRow00));
            using DataTable dtMissSumTemp = CreatTableMissSum();
            for (int index = 1; index <= new CglDataSet(stuGLSearch.LottoType).LottoNumbers; index++)
            {
                DataRow drMissSum = dtMissSumTemp.NewRow();
                string strMissNum = string.Format(InvariantCulture, "lngM{0}", index);
                drMissSum["lngN"] = index;
                drMissSum["lngM1"] = Fields.Count - dicMissN1[strMissNum];
                drMissSum["lngM3"] = Fields.Count - dicMissN3[strMissNum];
                drMissSum["lngM5"] = Fields.Count - dicMissN5[strMissNum];
                drMissSum["lngM10"] = Fields.Count - dicMissN10[strMissNum];
                drMissSum["lngM15"] = Fields.Count - dicMissN15[strMissNum];
                dtMissSumTemp.Rows.Add(drMissSum);
            }
            using DataTable dtMissSum = TransposedMatrix(dtMissSumTemp);
            dtMissSum.TableName = "dtMissSum";
            dsMissingTotal.Tables.Add(dtMissSum);
            return dsMissingTotal;
        }

        private List<string> ChangFieldOrder(List<string> LstInput)
        {
            CheckData = true;
            List<string> LstReturn = new List<string>();
            List<string> LstFieldName = new List<string> {
                "strDayTwelve","strHourTwentyEight","strDayEight", "strDayNine","strHourT",
                "gen","strDayFive", "strDayTwentyEight", "strp13",
                "strp01", "strp02", "strp03", "strp04", "strp05",
                "strp06", "strp07", "strp08", "strp09", "strp10" ,
                "strp11", "strp12"};
            foreach (string item in LstFieldName)
            {
                if (LstInput.Contains(item))
                {
                    LstReturn.Add(item);
                }
            }
            return LstReturn;
        }

        private static DataTable CountMissRow0(DataTable dtMissTotalRow00)
        {
            using DataTable dtRow00 = dtMissTotalRow00.Copy();
            dtRow00.TableName = "dtMissTotalRow00";
            if (dtMissTotalRow00.Rows.Count > 0)
            {
                DataRow dr = dtRow00.NewRow();
                dr["strField"] = "合計";
                foreach (DataColumn dc in dtMissTotalRow00.Columns)
                {
                    if (dc.ColumnName != "strField")
                    {
                        dr[dc.ColumnName] = dtMissTotalRow00.Compute(string.Format(InvariantCulture, "Sum([{0}])", dc.ColumnName),
                                                                     string.Empty);
                    }
                }
                dtRow00.Rows.Add(dr);
            }
            return dtRow00;
        }

        private static DataTable TransposedMatrix(DataTable dtMissSum)
        {
            if (dtMissSum == null) { throw new ArgumentNullException(nameof(dtMissSum)); }

            using DataTable dtTransposedMatrix = new DataTable();
            #region Column
            dtTransposedMatrix.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "strSetion"),
                Caption = string.Format(InvariantCulture, "strSetion"),
                DataType = typeof(string),
                AllowDBNull = false,
                Unique = true,
            });
            for (int nums = 1; nums <= dtMissSum.Rows.Count; nums++)
            {
                dtTransposedMatrix.Columns.Add(new DataColumn()
                {
                    ColumnName = string.Format(InvariantCulture, "lngN{0}", nums),
                    Caption = string.Format(InvariantCulture, "lngN{0}", nums),
                    DataType = typeof(int),
                    DefaultValue = 0
                });
            }
            #endregion Column

            foreach (int insec in new int[] { 1, 3, 5, 10, 15 })
            {
                DataRow drTM = dtTransposedMatrix.NewRow();
                drTM["strSetion"] = string.Format(InvariantCulture, "{0:d2}", insec);
                for (int nums = 1; nums <= dtMissSum.Rows.Count; nums++)
                {
                    drTM[string.Format(InvariantCulture, "lngN{0}", nums)] =
                        dtMissSum.Rows[nums - 1][string.Format(InvariantCulture, "lngM{0}", insec)];
                }
                dtTransposedMatrix.Rows.Add(drTM);
            }

            using DataTable dtTemp = dtTransposedMatrix.Copy();
            DataRow drTransposedMatrix = dtTransposedMatrix.NewRow();
            drTransposedMatrix["strSetion"] = "合計";
            for (int nums = 1; nums <= dtMissSum.Rows.Count; nums++)
            {
                drTransposedMatrix[string.Format(InvariantCulture, "lngN{0}", nums)] =
                    dtTemp.Compute(string.Format(InvariantCulture, "Sum([lngN{0}])", nums), string.Empty);
            }
            dtTransposedMatrix.Rows.Add(drTransposedMatrix);

            return dtTransposedMatrix;
        }

        private static DataTable CreatTableMissTotal(StuGLSearch stuGLSearch)
        {
            using DataTable dtReturn = new DataTable();
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = string.Format(InvariantCulture, "strField"),
                Caption = string.Format(InvariantCulture, "strField"),
                DataType = typeof(string),
                Unique = true,
            });

            for (int index = 1; index <= new CglDataSet(stuGLSearch.LottoType).LottoNumbers; index++)
            {
                dtReturn.Columns.Add(new DataColumn()
                {
                    ColumnName = string.Format(InvariantCulture, "lngM{0}", index),
                    Caption = string.Format(InvariantCulture, "lngM{0}", index),
                    DataType = typeof(int),
                    DefaultValue = 0
                });
            }
            return dtReturn.Copy();
        }

        private static DataTable CreatTableMissSum()
        {
            using DataTable dtReturn = new DataTable();
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = string.Format(InvariantCulture, "lngN"),
                Caption = string.Format(InvariantCulture, "lngN"),
                DataType = typeof(int),
                AllowDBNull = false,
                Unique = true
            });
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = string.Format(InvariantCulture, "lngM1"),
                Caption = string.Format(InvariantCulture, "lngM1"),
                DataType = typeof(int),
                DefaultValue = 0,
            });
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = string.Format(InvariantCulture, "lngM3"),
                Caption = string.Format(InvariantCulture, "lngM3"),
                DataType = typeof(int),
                DefaultValue = 0,
            });
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = string.Format(InvariantCulture, "lngM5"),
                Caption = string.Format(InvariantCulture, "lngM5"),
                DataType = typeof(int),
                DefaultValue = 0,
            });
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = string.Format(InvariantCulture, "lngM10"),
                Caption = string.Format(InvariantCulture, "lngM10"),
                DataType = typeof(int),
                DefaultValue = 0,
            });
            dtReturn.Columns.Add(new DataColumn()
            {
                ColumnName = string.Format(InvariantCulture, "lngM15"),
                Caption = string.Format(InvariantCulture, "lngM15"),
                DataType = typeof(int),
                DefaultValue = 0,
            });
            return dtReturn.Copy();
        }

        private void ReleaseMemory()
        {
            Session.Remove(MissingTotalT01ID + "dsMissingTotal");
            Session.Remove(MissingTotalT01ID + "_ddlFreq");
            Session.Remove(MissingTotalT01ID + "dtMissN1");
            Session.Remove(MissingTotalT01ID + "dtMissN1Hit");
            Session.Remove(MissingTotalT01ID + "dtMissN3");
            Session.Remove(MissingTotalT01ID + "dtMissN3Hit");
            Session.Remove(MissingTotalT01ID + "dtMissN5");
            Session.Remove(MissingTotalT01ID + "dtMissN5Hit");
            Session.Remove(MissingTotalT01ID + "dtMissN10");
            Session.Remove(MissingTotalT01ID + "dtMissN10Hit");
            ViewState.Clear();
            ResetSearchOrder(MissingTotalT01ID);
            if (DicThreadMissingTotalT01.Keys.Contains(MissingTotalT01ID + "T01"))
            {
                Thread01 = (Thread)DicThreadMissingTotalT01[MissingTotalT01ID + "T01"];
                if (Thread01.ThreadState == ThreadState.Suspended)
                {
#pragma warning disable CS0618 // 類型或成員已經過時
                    Thread01.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
                }
                Thread01.Abort();
                Thread01.Join();
                DicThreadMissingTotalT01.Remove(MissingTotalT01ID + "T01");
            }
        }
    }
}