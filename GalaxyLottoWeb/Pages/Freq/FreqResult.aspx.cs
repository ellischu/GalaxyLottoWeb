using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages.Freq
{
    public partial class FreqResult00 : BasePage
    {
        private StuGLSearch GlobalStuSearch;

        private void SetupViewState()
        {
            string uid = Request["uid"] ?? (string)base.Session["uid"] ?? (string)base.ViewState["uid"] ?? string.Empty;

            if (ViewState["uid"] == null) { base.ViewState.Add("uid", uid); }

            if (ViewState["GlobalStuSearch"] == null)
            {
                if (!string.IsNullOrEmpty(uid) && base.Session[uid] != null)
                {
                    ViewState.Add("GlobalStuSearch", (StuGLSearch)Session[uid]);
                }
            }
            GlobalStuSearch = (StuGLSearch)ViewState["GlobalStuSearch"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetupViewState();
            if (ViewState["GlobalStuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                InitialArgument();
                Setddl();
                ShowTitle();
                ShowFreq(GlobalStuSearch);
                ShowFreqProcess(GlobalStuSearch);
            }
        }

        private void ShowFreq(StuGLSearch stuSearchTemp)
        {
            Dictionary<string, string> _dicNumcssclass = (Dictionary<string, string>)ViewState["_dicNumcssclass"];
            stuSearchTemp.FieldMode = ddlFreq.SelectedValue != "gen";
            stuSearchTemp.NextNumsMode = ddlNexts.Visible && ddlNexts.SelectedValue != "gen";
            stuSearchTemp.StrNextNums = stuSearchTemp.NextNumsMode ? ddlNexts.SelectedValue : "none";

            DataTable DtFreqView = new CglFreq().GetFreq(stuSearchTemp);
            lblFreq.Text = string.Format(InvariantCulture, "{0} ({1}期)", new CglFunc().ConvertFieldNameId(ddlFreq.SelectedValue), DtFreqView.Rows[0]["intRows"]);
            DtFreqView = new CglFunc().SortFreq(stuSearchTemp, DtFreqView);
            gvFreq.DataSource = DtFreqView.DefaultView;
            if (gvFreq.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtFreqView.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtFreqView.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtFreqView.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtFreqView.Columns[ColumnIndex].ColumnName,
                    };
                    gvFreq.Columns.Add(bfCell);
                }
                foreach (DataControlField dcColumn in gvFreq.Columns)
                {
                    string strColumnName = dcColumn.SortExpression;
                    if (_dicNumcssclass.Keys.Contains(strColumnName.Substring(4)))
                    {
                        dcColumn.HeaderStyle.CssClass = _dicNumcssclass[strColumnName.Substring(4)];
                    }
                }
            }
            gvFreq.DataBind();
        }

        private void ShowFreqProcess(StuGLSearch stuSearchTemp)
        {
            dtlProcess.Visible=true;
            DataTable DtFreqProcess = CglFreqProcess.GetFreqProcs(stuSearchTemp, CglDBFreq.TableName.QryFreqProcess, SortOrder.Descending);
            if (DtFreqProcess.Columns.Contains("lngFreqProcessSN")) { DtFreqProcess.Columns.Remove("lngFreqProcessSN"); }
            if (DtFreqProcess.Columns.Contains("lngFreqSN")) { DtFreqProcess.Columns.Remove("lngFreqSN"); }
            gvFreqProcess.DataSource = new CglFunc().CTableShow(DtFreqProcess).DefaultView;
            if (gvFreqProcess.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < DtFreqProcess.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtFreqProcess.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtFreqProcess.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = DtFreqProcess.Columns[ColumnIndex].ColumnName,
                    };
                    gvFreqProcess.Columns.Add(bfCell);
                }
            }
            #region Set Columns of DataGrid gvProcess
            foreach (BoundField dcColumn in gvFreqProcess.Columns)
            {
                string strColumnName = dcColumn.SortExpression;
                if ((strColumnName.Substring(0, 4) != "lngL" && strColumnName.Substring(0, 4) != "lngM") || strColumnName == "lngMethodSN")
                {
                    dcColumn.HeaderStyle.CssClass = strColumnName;
                    dcColumn.ItemStyle.CssClass = strColumnName;
                }

                if (strColumnName.Substring(0, 4) == "lngL" || strColumnName.Substring(0, 4) == "lngS")
                {
                    dcColumn.HeaderStyle.CssClass = strColumnName;
                    dcColumn.DataFormatString = "{0:d2}";
                }
            }
            #endregion
            gvFreqProcess.RowDataBound += GvFreqProcess_RowDataBound;
            gvFreqProcess.DataBind();
        }

        /// <summary>
        /// change CSS of the cells when datarow loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GvFreqProcess_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Dictionary<string, int> _dicCurrentNums = (Dictionary<string, int>)ViewState["CurrentNums"];
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        #region Set lngL , lngS
                        string strCell_ColumnName = field.DataField;
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
            }
        }

        //-----------------------------------------------------------------------------

        private void InitialArgument()
        {
            if (ViewState["CurrentNums"] == null) { ViewState.Add("CurrentNums", new CglData().GetDataNumsDici(GlobalStuSearch)); }
            if (ViewState["_dicNumcssclass"] == null) { ViewState.Add("_dicNumcssclass", new GalaxyApp().GetNumcssclass(GlobalStuSearch)); }
        }

        private void Setddl()
        {
            if (ddlFreq.Items.Count == 0)
            {
                if (GlobalStuSearch.FieldMode)
                {
                    ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(GlobalStuSearch.StrCompares), GlobalStuSearch.StrCompares));
                }
                else
                {
                    ddlFreq.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId("gen"), "gen"));
                }
                if (GlobalStuSearch.NextNumsMode)
                {
                    StuGLSearch stuSearchTemp = GlobalStuSearch;
                    ddlNexts.Visible = true;
                    ddlNexts.Items.Clear();
                    ddlNexts.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId("gen"), "gen"));

                    if (ddlFreq.SelectedValue == "gen") { stuSearchTemp.FieldMode = false; }
                    //stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
                    List<string> NextNums = stuSearchTemp.StrNextNumT.Split(separator: ';').ToList();
                    foreach (string Nums in NextNums)
                    {
                        ddlNexts.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(Nums), Nums));
                    }
                }
            }
        }

        private void ShowTitle()
        {
            StuGLSearch stuSearchTemp = GlobalStuSearch;
            if (ViewState["title"] == null) { ViewState.Add("title", new CglDBData().SetTitleString(stuSearchTemp)); }
            Page.Title = (string)ViewState["title"];
            lblTitle.Text = (string)ViewState["title"];

            if (ddlNexts.SelectedValue != "gen")
            {
                stuSearchTemp.StrNextNums = ddlNexts.SelectedValue;
            }
            // if (ViewState["lblMethod"] == null) { ViewState.Add("lblMethod", string.Format(InvariantCulture, "{0}:{1}", "頻率總表", new CglMethod().SetMethodString(stuSearch))); }
            lblMethod.Text = string.Format(InvariantCulture, "{0}:{1}", "頻率總表", new CglMethod().SetMethodString(stuSearchTemp));


            if (ViewState["lblSearchMethod"] == null) { ViewState.Add("lblSearchMethod", new CglMethod().SetSearchMethodString(stuSearchTemp)); }
            lblSearchMethod.Text = (string)ViewState["lblSearchMethod"];

            //顯示當前資料
            if (ViewState["CurrentData"] == null) { ViewState.Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(stuSearchTemp))); }
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", (DataTable)ViewState["CurrentData"], true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
        }

        //-----------------------------------------------------------------------------

        protected void DdlFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlNexts.Visible)
            {
                StuGLSearch stuSearchTemp = GlobalStuSearch;
                ddlNexts.Items.Clear();
                ddlNexts.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId("gen"), "gen"));

                if (ddlFreq.SelectedValue == "gen") { stuSearchTemp.FieldMode = false; }
                //stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
                List<string> NextNums = stuSearchTemp.StrNextNumT.Split(separator: ';').ToList();
                foreach (string Nums in NextNums)
                {
                    ddlNexts.Items.Add(new ListItem(new CglFunc().ConvertFieldNameId(Nums), Nums));
                }
            }
        }

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
        }

    }
}