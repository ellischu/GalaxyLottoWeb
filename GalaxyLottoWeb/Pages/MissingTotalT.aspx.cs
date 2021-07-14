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
    public partial class MissingTotalT : BasePage
    {
        private StuGLSearch GlobalStuSearch;
        private string _action;
        private string _requestId;
        private string MissingTotalTID;
        private bool _blLngM0;

        //private List<int> _lstCurrentNums;
        //private bool _blLngM0 { get; set; } = false;

        private DataTable DtMissingTotal { get; set; }
        private DataTable DtMissSum { get; set; }
        //private DataTable DtMissN1 { get; set; }
        private DataTable DtMissN1Hit { get; set; }
        private DataTable DtMissN3 { get; set; }
        private DataTable DtMissN3Hit { get; set; }
        private DataTable DtMissN5 { get; set; }
        private DataTable DtMissN5Hit { get; set; }
        private DataTable DtMissN10 { get; set; }
        private DataTable DtMissN10Hit { get; set; }
        private DataTable DtMissN15 { get; set; }
        private DataTable DtMissN15Hit { get; set; }

        private static Dictionary<string, object> DicThreadMissingTotalT
        {
            get
            {
                if (dicThreadMissingTotalT == null) { dicThreadMissingTotalT = new Dictionary<string, object>(); }
                return dicThreadMissingTotalT;
            }
            set => dicThreadMissingTotalT = value;
        }

        private static Dictionary<string, object> dicThreadMissingTotalT;


        private Thread Thread01;


        // ---------------------------------------------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            if (ViewState[MissingTotalTID + "_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                GlobalStuSearch = (StuGLSearch)ViewState[MissingTotalTID + "_gstuSearch"];

                InitializeArgument();
                ShowTitle();
                ShowdllFreq();
                ShowData(ddlFreq.SelectedValue);
            }

        }

        // ---------------------------------------------------------------------------------------------------------
        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            MissingTotalTID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[MissingTotalTID] != null)
            {
                if (ViewState[MissingTotalTID + "_gstuSearch"] == null)
                {
                    ViewState.Add(MissingTotalTID + "_gstuSearch", (StuGLSearch)Session[MissingTotalTID]);
                }
                else
                {
                    ViewState[MissingTotalTID + "_gstuSearch"] = (StuGLSearch)Session[MissingTotalTID];
                };
            }

        }

        private void InitializeArgument()
        {
            if (ViewState[MissingTotalTID + "Argument"] == null)
            {
                ViewState.Add(MissingTotalTID + "Argument", new Dictionary<string, object>());
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("title"))
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("title", string.Format(InvariantCulture, "{0}:{1}", "遺漏整合總表", new CglDBData().SetTitleString(GlobalStuSearch)));
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("lblMethod"))
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("lblMethod", new CglMethod().SetMethodString(GlobalStuSearch));
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("lblSearchMethod"))
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("lblSearchMethod", new CglMethod().SetSearchMethodString(GlobalStuSearch));
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("CurrentData"))
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("CurrentData", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(GlobalStuSearch)));
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("lstCurrentNums"))
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("lstCurrentNums", new CglData().GetDataNumsLst(GlobalStuSearch));
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dicCurrentNums"))
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("dicCurrentNums", new CglData().GetDataNumsDici(this.GlobalStuSearch));
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissSum") && Session[MissingTotalTID + "dtMissSum"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("dtMissSum", Session[MissingTotalTID + "dtMissSum"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN1") && Session[MissingTotalTID + "dtMissSum"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("dtMissN1", Session[MissingTotalTID + "dtMissN1"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN1Hit") && Session[MissingTotalTID + "dtMissSum"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("dtMissN1Hit", Session[MissingTotalTID + "dtMissN1Hit"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN3") && Session[MissingTotalTID + "dtMissSum"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("dtMissN3", Session[MissingTotalTID + "dtMissN3"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN3Hit") && Session[MissingTotalTID + "dtMissSum"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("dtMissN3Hit", Session[MissingTotalTID + "dtMissN3Hit"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN5") && Session[MissingTotalTID + "dtMissSum"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("dtMissN5", Session[MissingTotalTID + "dtMissN5"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN5Hit") && Session[MissingTotalTID + "dtMissSum"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("dtMissN5Hit", Session[MissingTotalTID + "dtMissN5Hit"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN10") && Session[MissingTotalTID + "dtMissSum"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("dtMissN10", Session[MissingTotalTID + "dtMissN10"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN10Hit") && Session[MissingTotalTID + "dtMissSum"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("dtMissN10Hit", Session[MissingTotalTID + "dtMissN10Hit"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN15") && Session[MissingTotalTID + "dtMissSum"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("dtMissN15", Session[MissingTotalTID + "dtMissN15"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN15Hit") && Session[MissingTotalTID + "dtMissSum"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("dtMissN15Hit", Session[MissingTotalTID + "dtMissN15Hit"]);
            }
            if (!((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dsMissingTotal") && Session[MissingTotalTID + "dsMissingTotal"] != null)
            {
                ((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).Add("dsMissingTotal", Session[MissingTotalTID + "dsMissingTotal"]);
            }
            //    ViewState[MissingTotalTID + "dsMissingTotal"]
            //((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dsMissingTotal"]

            if (ViewState[MissingTotalTID + "_ddlFreq"] == null && Session[MissingTotalTID + "_ddlFreq"] != null)
            {
                ViewState.Add(MissingTotalTID + "_ddlFreq", Session[MissingTotalTID + "_ddlFreq"]);
            }

        }

        // ---------------------------------------------------------------------------------------------------------

        private void ShowTitle()
        {
            base.Page.Title = (string)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["title"];
            lblTitle.Text = (string)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["title"];
            lblMethod.Text = (string)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["lblMethod"];
            lblSearchMethod.Text = (string)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["lblSearchMethod"];

            // 當期資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ",
                                                   (DataTable)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["CurrentData"],
                                                   true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
        }

        private void ShowdllFreq()
        {
            if (Session[MissingTotalTID + "_ddlFreq"] != null)
            {
                if (ddlFreq.Items.Count == 0)
                {
                    foreach (string strItem in (List<string>)Session[MissingTotalTID + "_ddlFreq"])
                    {
                        ListItem listItem = new ListItem(new CglFunc().ConvertFieldNameId(strItem), strItem);
                        ddlFreq.Items.Add(listItem);
                    }
                }
                lblFreq.Text = string.Format(InvariantCulture, "{0}:{1}", ddlFreq.Items.Count - 1, ddlFreq.SelectedItem.Text);
            }
            else
            {
                if (!DicThreadMissingTotalT.Keys.Contains(MissingTotalTID + "T01")) { CreatThread(); }
            }
        }

        private void ShowData(string selectedValue)
        {
            pnlSum.Visible = false;
            pnlFields.Visible = false;
            switch (selectedValue)
            {
                case "Sum":
                    ShowSumData();
                    break;
                default:
                    ShowMissAll(selectedValue);
                    break;
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        private void ShowSumData()
        {
            lblBriefDate.Text = new GalaxyApp().ShowBriefDate((DataTable)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["CurrentData"], (List<int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["lstCurrentNums"]);

            if (((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN1"))
            {
                pnlSum.Visible = true;

                using DataTable DtMissN1 = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dtMissN1"];
                gvSumMissN1.DataSource = DtMissN1.DefaultView;
                if (gvSumMissN1.Columns.Count == 0)
                {
                    for (int i = 0; i < DtMissN1.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtMissN1.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtMissN1.Columns[i].ColumnName, 1),
                            SortExpression = DtMissN1.Columns[i].ColumnName,
                        };
                        gvSumMissN1.Columns.Add(bfCell);
                    }
                }

                if (gvSumMissN1.Columns.Count != 0)
                {
                    int intlngm = 1;

                    foreach (BoundField bfCell in gvSumMissN1.Columns)
                    {
                        string strColumnName = bfCell.DataField;
                        #region Show the number of lngL,lngS,lngM with 2 digital

                        if (strColumnName.Substring(0, 4) == "lngM" && strColumnName != "lngMethodSN")
                        {
                            bfCell.DataFormatString = "{0:d2}";
                            if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strColumnName.Substring(4), InvariantCulture)))
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
                gvSumMissN1.DataBind();
            }

            if (((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN1Hit"))
            {
                pnlSum.Visible = true;
                DtMissN1Hit = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dtMissN1Hit"];
                gvSumMissN1Hit.DataSource = DtMissN1Hit.DefaultView;
                if (gvSumMissN1Hit.Columns.Count == 0)
                {
                    for (int i = 0; i < DtMissN1Hit.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtMissN1Hit.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtMissN1Hit.Columns[i].ColumnName, 1),
                            SortExpression = DtMissN1Hit.Columns[i].ColumnName,
                        };
                        gvSumMissN1Hit.Columns.Add(bfCell);
                    }
                }
                gvSumMissN1Hit.DataBind();
            }

            if (((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN3"))
            {
                pnlSum.Visible = true;
                DtMissN3 = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dtMissN3"];
                gvSumMissN3.DataSource = DtMissN3.DefaultView;
                if (gvSumMissN3.Columns.Count == 0)
                {
                    for (int i = 0; i < DtMissN3.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtMissN3.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtMissN3.Columns[i].ColumnName, 1),
                            SortExpression = DtMissN3.Columns[i].ColumnName,
                        };
                        gvSumMissN3.Columns.Add(bfCell);
                    }
                }
                if (gvSumMissN3.Columns.Count != 0)
                {
                    int intlngm = 1;
                    foreach (BoundField bfCell in gvSumMissN3.Columns)
                    {
                        string strColumnName = bfCell.DataField;
                        #region Show the number of lngL,lngS,lngM with 2 digital
                        if (strColumnName.Substring(0, 4) == "lngM" && strColumnName != "lngMethodSN")
                        {
                            bfCell.DataFormatString = "{0:d2}";
                            if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strColumnName.Substring(4), InvariantCulture)))
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
                gvSumMissN3.DataBind();
            }

            if (((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN3Hit"))
            {
                pnlSum.Visible = true;
                DtMissN3Hit = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dtMissN3Hit"];
                gvSumMissN3Hit.DataSource = DtMissN3Hit.DefaultView;
                if (gvSumMissN3Hit.Columns.Count == 0)
                {
                    for (int i = 0; i < DtMissN3Hit.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtMissN3Hit.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtMissN3Hit.Columns[i].ColumnName, 1),
                            SortExpression = DtMissN3Hit.Columns[i].ColumnName,
                        };
                        gvSumMissN3Hit.Columns.Add(bfCell);
                    }
                }
                gvSumMissN3Hit.DataBind();
            }

            if (((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN5"))
            {
                pnlSum.Visible = true;
                DtMissN5 = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dtMissN5"];
                gvSumMissN5.DataSource = DtMissN5.DefaultView;
                if (gvSumMissN5.Columns.Count == 0)
                {
                    for (int i = 0; i < DtMissN5.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtMissN5.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtMissN5.Columns[i].ColumnName, 1),
                            SortExpression = DtMissN5.Columns[i].ColumnName,
                        };
                        gvSumMissN5.Columns.Add(bfCell);
                    }
                }
                if (gvSumMissN5.Columns.Count != 0)
                {
                    int intlngm = 1;
                    foreach (BoundField bfCell in gvSumMissN5.Columns)
                    {
                        string strColumnName = bfCell.DataField;
                        #region Show the number of lngL,lngS,lngM with 2 digital
                        if (strColumnName.Substring(0, 4) == "lngM" && strColumnName != "lngMethodSN")
                        {
                            bfCell.DataFormatString = "{0:d2}";
                            if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strColumnName.Substring(4), InvariantCulture)))
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
                gvSumMissN5.DataBind();
            }

            if (((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN5Hit"))
            {
                pnlSum.Visible = true;
                DtMissN5Hit = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dtMissN5Hit"];
                gvSumMissN5Hit.DataSource = DtMissN5Hit.DefaultView;
                if (gvSumMissN5Hit.Columns.Count == 0)
                {
                    for (int i = 0; i < DtMissN5Hit.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtMissN5Hit.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtMissN5Hit.Columns[i].ColumnName, 1),
                            SortExpression = DtMissN5Hit.Columns[i].ColumnName,
                        };
                        gvSumMissN5Hit.Columns.Add(bfCell);
                    }
                }
                gvSumMissN5Hit.DataBind();
            }

            if (((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN10"))
            {
                pnlSum.Visible = true;
                DtMissN10 = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dtMissN10"];
                gvSumMissN10.DataSource = DtMissN10.DefaultView;
                if (gvSumMissN10.Columns.Count == 0)
                {
                    for (int i = 0; i < DtMissN10.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtMissN10.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtMissN10.Columns[i].ColumnName, 1),
                            SortExpression = DtMissN10.Columns[i].ColumnName,
                        };
                        gvSumMissN10.Columns.Add(bfCell);
                    }
                }
                if (gvSumMissN10.Columns.Count != 0)
                {
                    int intlngm = 1;
                    foreach (BoundField bfCell in gvSumMissN10.Columns)
                    {
                        string strColumnName = bfCell.DataField;
                        #region Show the number of lngL,lngS,lngM with 2 digital
                        if (strColumnName.Substring(0, 4) == "lngM" && strColumnName != "lngMethodSN")
                        {
                            bfCell.DataFormatString = "{0:d2}";
                            if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strColumnName.Substring(4), InvariantCulture)))
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
                gvSumMissN10.DataBind();
            }

            if (((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN10Hit"))
            {
                pnlSum.Visible = true;
                DtMissN10Hit = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dtMissN10Hit"];
                gvSumMissN10Hit.DataSource = DtMissN10Hit.DefaultView;
                if (gvSumMissN10Hit.Columns.Count == 0)
                {
                    for (int i = 0; i < DtMissN10Hit.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtMissN10Hit.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtMissN10Hit.Columns[i].ColumnName, 1),
                            SortExpression = DtMissN10Hit.Columns[i].ColumnName,
                        };
                        gvSumMissN10Hit.Columns.Add(bfCell);
                    }
                }
                gvSumMissN10Hit.DataBind();
            }

            if (((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN15"))
            {
                pnlSum.Visible = true;
                DtMissN15 = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dtMissN15"];
                gvSumMissN15.DataSource = DtMissN15.DefaultView;
                if (gvSumMissN15.Columns.Count == 0)
                {
                    for (int i = 0; i < DtMissN15.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtMissN15.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtMissN15.Columns[i].ColumnName, 1),
                            SortExpression = DtMissN15.Columns[i].ColumnName,
                        };
                        gvSumMissN15.Columns.Add(bfCell);
                    }
                }
                if (gvSumMissN15.Columns.Count != 0)
                {
                    int intlngm = 1;
                    foreach (BoundField bfCell in gvSumMissN15.Columns)
                    {
                        string strColumnName = bfCell.DataField;
                        #region Show the number of lngL,lngS,lngM with 2 digital
                        if (strColumnName.Substring(0, 4) == "lngM" && strColumnName != "lngMethodSN")
                        {
                            bfCell.DataFormatString = "{0:d2}";
                            if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strColumnName.Substring(4), InvariantCulture)))
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
                gvSumMissN15.DataBind();
            }

            if (((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissN15Hit"))
            {
                pnlSum.Visible = true;
                DtMissN15Hit = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dtMissN15Hit"];
                gvSumMissN15Hit.DataSource = DtMissN15Hit.DefaultView;
                if (gvSumMissN15Hit.Columns.Count == 0)
                {
                    for (int i = 0; i < DtMissN15Hit.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtMissN15Hit.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtMissN15Hit.Columns[i].ColumnName, 1),
                            SortExpression = DtMissN15Hit.Columns[i].ColumnName,
                        };
                        gvSumMissN15Hit.Columns.Add(bfCell);
                    }
                }
                gvSumMissN15Hit.DataBind();
            }

            ShowMissSum();

        }

        private void ShowMissSum()
        {
            if (((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dtMissSum"))
            {
                ResetSearchOrder(MissingTotalTID);
                DtMissSum = (DataTable)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dtMissSum"];
                gvMissSum.DataSource = DtMissSum.DefaultView;
                if (gvMissSum.Columns.Count == 0)
                {
                    for (int i = 0; i < DtMissSum.Columns.Count; i++)
                    {
                        BoundField bfCell = new BoundField()
                        {
                            DataField = DtMissSum.Columns[i].ColumnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(DtMissSum.Columns[i].ColumnName, 1),
                            SortExpression = DtMissSum.Columns[i].ColumnName,
                        };
                        gvMissSum.Columns.Add(bfCell);
                    }
                }
                if (gvMissSum.Columns.Count != 0)
                {
                    int intlngm = 1;
                    foreach (BoundField bfCell in gvMissSum.Columns)
                    {
                        string strColumnName = bfCell.DataField;
                        #region Show the number of lngL,lngS,lngM with 2 digital
                        if (strColumnName.Substring(0, 4) == "lngN")
                        {
                            if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strColumnName.Substring(4), InvariantCulture)))
                            {
                                bfCell.HeaderStyle.CssClass = string.Format(InvariantCulture, "lngM{0}", intlngm);
                                bfCell.ItemStyle.CssClass = "glColNum5";
                                intlngm++;
                            }
                        }
                        #endregion
                    }
                }

                gvMissSum.RowDataBound += GvMissSum_RowDataBound;
                gvMissSum.DataBind();
            }
        }

        private void GvMissSum_RowDataBound(object sender, GridViewRowEventArgs e)
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
                        if (strCell_ColumnName.Substring(0, 4) == "lngN")
                        {
                            cell.ToolTip = dicNumCount[cell.Text].ToString(InvariantCulture);
                        }
                        #endregion Set lngL , lngS

                    }
                }
            }
        }

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

        private void ShowMissAll(string selectedValue)
        {
            if (((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"]).ContainsKey("dsMissingTotal") && ((DataSet)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dsMissingTotal"]).Tables.Contains(selectedValue))
            {
                using DataSet dsMissingTotal = (DataSet)((Dictionary<string, object>)ViewState[MissingTotalTID + "Argument"])["dsMissingTotal"];
                //lblFreq.Text = string.Format(InvariantCulture, "{0}:{1}", dsMissingTotal.Tables.Count, ddlFreq.SelectedItem.Text);
                pnlFields.Visible = true;
                DtMissingTotal = dsMissingTotal.Tables[selectedValue];
                #region Convert 0 to -1 to -7
                for (int intRow = 0; intRow < DtMissingTotal.Rows.Count; intRow++)
                {
                    int intIndexofZero = -1;
                    for (int intNum = 1; intNum <= new CglDataSet(GlobalStuSearch.LottoType).LottoNumbers; intNum++)
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
                DtMissingTotal.DefaultView.Sort = "lngTotalSN DESC";
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
                            if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(strColumnName.Substring(4), InvariantCulture)))
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
            }
        }

        private void GvMissAll_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                _blLngM0 = false;
                if ((e.Row.RowIndex + 1) % 5 == 0) { e.Row.CssClass = "glRow5"; }
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField;
                        #region Set lngL , lngS
                        if (strCell_ColumnName.Contains("lngL") || strCell_ColumnName.Contains("lngS"))
                        {
                            if (((Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["dicCurrentNums"]).ContainsValue(int.Parse(cell.Text, InvariantCulture)))
                            {
                                foreach (KeyValuePair<string, int> kv in (Dictionary<string, int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["dicCurrentNums"])
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
                        if (strCell_ColumnName.Contains("lngM") && strCell_ColumnName != "lngMethodSN")
                        {
                            int intColumnValue = int.Parse(strCell_ColumnName.Substring(4), InvariantCulture);
                            cell.ToolTip = string.Format(InvariantCulture, "{0}", intColumnValue);
                            int intValue = int.Parse(cell.Text, InvariantCulture);
                            _blLngM0 = (intValue < 0 && Math.Abs(intValue) == 1) || _blLngM0;
                            _blLngM0 = (intValue >= 0 || Math.Abs(intValue) != new CglDataSet(GlobalStuSearch.LottoType).CountNumber) && _blLngM0;

                            cell.CssClass = string.Empty;

                            cell.CssClass = intValue < 0 ? string.Format(InvariantCulture, " lngM{0} ", Math.Abs(intValue)) : cell.CssClass;

                            cell.CssClass = _blLngM0 ? cell.CssClass + string.Format(InvariantCulture, " lngM{0} ", 0) : cell.CssClass;

                            cell.CssClass = (intColumnValue % 5 == 0) ? cell.CssClass + string.Format(InvariantCulture, " glColNum5 ") : cell.CssClass;

                        }
                        #endregion Set lngM

                        #region Set Day of Week
                        if (strCell_ColumnName == "lngDateSN")
                        {
                            string strDateSN = cell.Text;
                            DayOfWeek dayOfWeek = new DateTime(int.Parse(strDateSN.Substring(0, 4), InvariantCulture), int.Parse(strDateSN.Substring(4, 2), InvariantCulture), int.Parse(strDateSN.Substring(6, 2), InvariantCulture)).DayOfWeek;
                            e.Row.CssClass = dayOfWeek switch
                            {
                                DayOfWeek.Monday => "glMonday",
                                DayOfWeek.Tuesday => "glTuesday",
                                DayOfWeek.Wednesday => "glWednesday",
                                DayOfWeek.Thursday => "glThursday",
                                DayOfWeek.Friday => "glFriday",
                                DayOfWeek.Saturday => "glSaturday",
                                _ => "glSunday",
                            };
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

        // ---------------------------------------------------------------------------------------------------------
        protected void BtnCloseClick(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
            ReleaseMemory();
        }

        protected void BtnT1StartClick(object sender, EventArgs e)
        {
            if (DicThreadMissingTotalT != null && DicThreadMissingTotalT.Keys.Contains(MissingTotalTID + "T01"))
            {
                Thread01 = (Thread)DicThreadMissingTotalT[MissingTotalTID + "T01"];
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
            if (DicThreadMissingTotalT.Keys.Contains(MissingTotalTID + "T01"))
            {
                Thread01 = (Thread)DicThreadMissingTotalT[MissingTotalTID + "T01"];
                if (Thread01.IsAlive)
                {
                    lblArgument.Visible = true;
                    lblArgument.Text = string.Format(InvariantCulture, "{0}", DateTime.Now.ToLongTimeString());
                    btnT1Start.Visible = true;
                    btnT1Start.Text = string.Format(InvariantCulture, "T1:{0} {1} ", Session[MissingTotalTID + "lblT01"].ToString(), new GalaxyApp().GetTheadState(Thread01.ThreadState));
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
            Thread01 = new Thread(() => { StartThread01(); ResetSearchOrder(MissingTotalTID); })
            {
                Name = MissingTotalTID + "T01"
            };
            Thread01.Start();
            DicThreadMissingTotalT.Add(MissingTotalTID + "T01", Thread01);
        }

        private void StartThread01()
        {
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Start);

            Dictionary<string, int> dicMissN1 = new Dictionary<string, int>();
            Dictionary<string, int> dicMissN3 = new Dictionary<string, int>();
            Dictionary<string, int> dicMissN5 = new Dictionary<string, int>();
            Dictionary<string, int> dicMissN10 = new Dictionary<string, int>();
            Dictionary<string, int> dicMissN15 = new Dictionary<string, int>();

            List<string> Fields = (List<string>)new CglValidFields().GetValidFieldsLst(GlobalStuSearch);
            if (Session[MissingTotalTID + "_ddlFreq"] == null)
            {
                List<string> lstShowFields = new List<string> { "Sum" };

                foreach (string field in Fields) { lstShowFields.Add(field); }
                Session.Add(MissingTotalTID + "_ddlFreq", lstShowFields);
            }

            using DataSet dsMissingTotal = new DataSet { Locale = InvariantCulture };
            foreach (string strField in Fields)
            {
                Session[MissingTotalTID + "lblT01"] = string.Format(InvariantCulture, "#{0}_{1}", GlobalStuSearch.InFieldPeriodLimit, new CglFunc().ConvertFieldNameId(strField));
                StuGLSearch stuSearchTemp = GlobalStuSearch;
                stuSearchTemp.FieldMode = strField != "gen";
                stuSearchTemp.StrCompares = strField != "gen" ? strField : "gen";
                stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
                stuSearchTemp = new CglMethod().GetMethodSN(stuSearchTemp);
                stuSearchTemp = new CglMethod().GetSearchMethodSN(stuSearchTemp);
                stuSearchTemp = new CglMethod().GetSecFieldSN(stuSearchTemp);

                DataTable dtMissAllTemp00 = new CglMissAll().GetMissAll00Multiple(stuSearchTemp, CglMissAll.TableName.QryMissAll0001, SortOrder.Descending);
                //if (dtMissAllTemp00 == null) { dtMissAllTemp00 = new DataTable(); }
                dtMissAllTemp00.Locale = InvariantCulture;
                dtMissAllTemp00.TableName = strField;
                dsMissingTotal.Tables.Add(dtMissAllTemp00);
                #region deal with MissN
                for (int index = 1; index <= new CglDataSet(GlobalStuSearch.LottoType).LottoNumbers; index++)
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
                        if (intlngM <= 1) { dicMissN1[strMissNum]++; }
                        if (intlngM <= 3) { dicMissN3[strMissNum]++; }
                        if (intlngM <= 5) { dicMissN5[strMissNum]++; }
                        if (intlngM <= 10) { dicMissN10[strMissNum]++; }
                        if (intlngM <= 15) { dicMissN15[strMissNum]++; }
                    }
                }
                #endregion deal with MissN
            }

            if (Session[MissingTotalTID + "dsMissingTotal"] == null)
            {
                Session[MissingTotalTID + "dsMissingTotal"] = dsMissingTotal;
            }
            DtMissSum = CreatTableMissSum();
            for (int index = 1; index <= new CglDataSet(GlobalStuSearch.LottoType).LottoNumbers; index++)
            {
                DataRow drMissSum = DtMissSum.NewRow();
                string strMissNum = string.Format(InvariantCulture, "lngM{0}", index);
                drMissSum["lngN"] = index;
                drMissSum["lngM1"] = Fields.Count - dicMissN1[strMissNum];
                drMissSum["lngM3"] = Fields.Count - dicMissN3[strMissNum];
                drMissSum["lngM5"] = Fields.Count - dicMissN5[strMissNum];
                drMissSum["lngM10"] = Fields.Count - dicMissN10[strMissNum];
                drMissSum["lngM15"] = Fields.Count - dicMissN15[strMissNum];
                DtMissSum.Rows.Add(drMissSum);
            }

            Session[MissingTotalTID + "dtMissN1"] = new CglFunc().CDicTOTable(dicMissN1, null);
            Session[MissingTotalTID + "dtMissN3"] = new CglFunc().CDicTOTable(dicMissN3, null);
            Session[MissingTotalTID + "dtMissN5"] = new CglFunc().CDicTOTable(dicMissN5, null);
            Session[MissingTotalTID + "dtMissN10"] = new CglFunc().CDicTOTable(dicMissN10, null);
            Session[MissingTotalTID + "dtMissN15"] = new CglFunc().CDicTOTable(dicMissN15, null);
            Session[MissingTotalTID + "dtMissSum"] = TransposedMatrix(DtMissSum);

            Dictionary<int, List<string>> dicHit1Nums = new Dictionary<int, List<string>>();
            Dictionary<int, List<string>> dicHit3Nums = new Dictionary<int, List<string>>();
            Dictionary<int, List<string>> dicHit5Nums = new Dictionary<int, List<string>>();
            Dictionary<int, List<string>> dicHit10Nums = new Dictionary<int, List<string>>();
            Dictionary<int, List<string>> dicHit15Nums = new Dictionary<int, List<string>>();

            DtMissN1Hit = CreatTableMissNHit();
            DtMissN3Hit = CreatTableMissNHit();
            DtMissN5Hit = CreatTableMissNHit();
            DtMissN10Hit = CreatTableMissNHit();
            DtMissN15Hit = CreatTableMissNHit();

            for (int index = 0; index <= Fields.Count; index++)
            {
                if (!dicHit1Nums.Keys.Contains(index)) { dicHit1Nums.Add(index, new List<string>()); }
                if (!dicHit3Nums.Keys.Contains(index)) { dicHit3Nums.Add(index, new List<string>()); }
                if (!dicHit5Nums.Keys.Contains(index)) { dicHit5Nums.Add(index, new List<string>()); }
                if (!dicHit10Nums.Keys.Contains(index)) { dicHit10Nums.Add(index, new List<string>()); }
                if (!dicHit15Nums.Keys.Contains(index)) { dicHit15Nums.Add(index, new List<string>()); }

                foreach (KeyValuePair<string, int> item in dicMissN1)
                {
                    if (item.Value == index)
                    {
                        dicHit1Nums[index].Add(string.Format(InvariantCulture, "{0:d2}", int.Parse(item.Key.Substring(4), InvariantCulture)));
                    }
                }

                foreach (KeyValuePair<string, int> item in dicMissN3)
                {
                    if (item.Value == index)
                    {
                        dicHit3Nums[index].Add(string.Format(InvariantCulture, "{0:d2}", int.Parse(item.Key.Substring(4), InvariantCulture)));
                    }
                }

                foreach (KeyValuePair<string, int> item in dicMissN5)
                {
                    if (item.Value == index)
                    {
                        dicHit5Nums[index].Add(string.Format(InvariantCulture, "{0:d2}", int.Parse(item.Key.Substring(4), InvariantCulture)));
                    }
                }

                foreach (KeyValuePair<string, int> item in dicMissN10)
                {
                    if (item.Value == index)
                    {
                        dicHit10Nums[index].Add(string.Format(InvariantCulture, "{0:d2}", int.Parse(item.Key.Substring(4), InvariantCulture)));
                    }
                }

                foreach (KeyValuePair<string, int> item in dicMissN15)
                {
                    if (item.Value == index)
                    {
                        dicHit15Nums[index].Add(string.Format(InvariantCulture, "{0:d2}", int.Parse(item.Key.Substring(4), InvariantCulture)));
                    }
                }

                DataRow drRow1 = DtMissN1Hit.NewRow();
                drRow1["CSum"] = index;
                drRow1["intNumCount"] = dicHit1Nums[index].Count;
                drRow1["intHitCount"] = ((List<int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["lstCurrentNums"]).Sum() == 0 ? -1 : GetHit(dicHit1Nums[index]).Count;
                drRow1["strFreqNums"] = string.Join("#", dicHit1Nums[index].ToArray());
                drRow1["strHitNums"] = ((List<int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["lstCurrentNums"]).Sum() == 0 ? string.Empty : string.Join("#", GetHit(dicHit1Nums[index]).ToArray());
                DtMissN1Hit.Rows.Add(drRow1);

                DataRow drRow3 = DtMissN3Hit.NewRow();
                drRow3["CSum"] = index;
                drRow3["intNumCount"] = dicHit3Nums[index].Count;
                drRow3["intHitCount"] = ((List<int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["lstCurrentNums"]).Sum() == 0 ? -1 : GetHit(dicHit3Nums[index]).Count;
                drRow3["strFreqNums"] = string.Join("#", dicHit3Nums[index].ToArray());
                drRow3["strHitNums"] = ((List<int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["lstCurrentNums"]).Sum() == 0 ? string.Empty : string.Join("#", GetHit(dicHit3Nums[index]).ToArray());
                DtMissN3Hit.Rows.Add(drRow3);

                DataRow drRow5 = DtMissN5Hit.NewRow();
                drRow5["CSum"] = index;
                drRow5["intNumCount"] = dicHit5Nums[index].Count;
                drRow5["intHitCount"] = ((List<int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["lstCurrentNums"]).Sum() == 0 ? -1 : GetHit(dicHit5Nums[index]).Count;
                drRow5["strFreqNums"] = string.Join("#", dicHit5Nums[index].ToArray());
                drRow5["strHitNums"] = ((List<int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["lstCurrentNums"]).Sum() == 0 ? string.Empty : string.Join("#", GetHit(dicHit5Nums[index]).ToArray());
                DtMissN5Hit.Rows.Add(drRow5);

                DataRow drRow10 = DtMissN10Hit.NewRow();
                drRow10["CSum"] = index;
                drRow10["intNumCount"] = dicHit10Nums[index].Count;
                drRow10["intHitCount"] = ((List<int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["lstCurrentNums"]).Sum() == 0 ? -1 : GetHit(dicHit10Nums[index]).Count;
                drRow10["strFreqNums"] = string.Join("#", dicHit10Nums[index].ToArray());
                drRow10["strHitNums"] = ((List<int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["lstCurrentNums"]).Sum() == 0 ? string.Empty : string.Join("#", GetHit(dicHit10Nums[index]).ToArray());
                DtMissN10Hit.Rows.Add(drRow10);

                DataRow drRow15 = DtMissN15Hit.NewRow();
                drRow15["CSum"] = index;
                drRow15["intNumCount"] = dicHit15Nums[index].Count;
                drRow15["intHitCount"] = ((List<int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["lstCurrentNums"]).Sum() == 0 ? -1 : GetHit(dicHit15Nums[index]).Count;
                drRow15["strFreqNums"] = string.Join("#", dicHit15Nums[index].ToArray());
                drRow15["strHitNums"] = ((List<int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["lstCurrentNums"]).Sum() == 0 ? string.Empty : string.Join("#", GetHit(dicHit15Nums[index]).ToArray());
                DtMissN15Hit.Rows.Add(drRow15);
            }

            Session[MissingTotalTID + "dtMissN1Hit"] = DtMissN1Hit;
            Session[MissingTotalTID + "dtMissN3Hit"] = DtMissN3Hit;
            Session[MissingTotalTID + "dtMissN5Hit"] = DtMissN5Hit;
            Session[MissingTotalTID + "dtMissN10Hit"] = DtMissN10Hit;
            Session[MissingTotalTID + "dtMissN15Hit"] = DtMissN15Hit;
            new GalaxyApp().MediaPlay(GalaxyApp.SoundOption.Finish);
            ResetSearchOrder(MissingTotalTID);
        }

        private static DataTable TransposedMatrix(DataTable dtMissSum)
        {
            if (dtMissSum == null) { throw new ArgumentNullException(nameof(dtMissSum)); }

            using DataTable dtTransposedMatrix = new DataTable { Locale = InvariantCulture };
            dtTransposedMatrix.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "strSetion"),
                Caption = string.Format(InvariantCulture, "strSetion"),
                DataType = typeof(string),
                AllowDBNull = false,
                Unique = true
            });
            for (int nums = 1; nums <= dtMissSum.Rows.Count; nums++)
            {
                dtTransposedMatrix.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "lngN{0}", nums),
                    Caption = string.Format(InvariantCulture, "lngN{0}", nums),
                    DataType = typeof(int),
                    DefaultValue = 0
                });
            }

            foreach (int insec in new int[] { 1, 3, 5, 10, 15 })
            {
                DataRow drTransposedMatrixSec = dtTransposedMatrix.NewRow();
                drTransposedMatrixSec["strSetion"] = string.Format(InvariantCulture, "{0:d2}", insec);
                for (int nums = 1; nums <= dtMissSum.Rows.Count; nums++)
                {
                    drTransposedMatrixSec[string.Format(InvariantCulture, "lngN{0}", nums)] =
                        dtMissSum.Rows[nums - 1][string.Format(InvariantCulture, "lngM{0}", insec)];
                }
                dtTransposedMatrix.Rows.Add(drTransposedMatrixSec);
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

        private List<string> GetHit(List<string> lstInput)
        {
            List<string> lstHit = new List<string>();
            foreach (string num in lstInput)
            {
                if (((List<int>)((Dictionary<string, object>)base.ViewState[MissingTotalTID + "Argument"])["lstCurrentNums"]).Contains(int.Parse(num, InvariantCulture)))
                {
                    lstHit.Add(num);
                }
            }
            return lstHit;
        }

        private static DataTable CreatTableMissNHit()
        {
            using DataTable dtReturn = new DataTable();
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "CSum"),
                Caption = string.Format(InvariantCulture, "CSum"),
                DataType = typeof(int),
                AllowDBNull = false,
                Unique = true
            });
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "intNumCount"),
                Caption = string.Format(InvariantCulture, "intNumCount"),
                DataType = typeof(int),
                DefaultValue = 0
            });
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "intHitCount"),
                Caption = string.Format(InvariantCulture, "intHitCount"),
                DataType = typeof(int),
                DefaultValue = 0
            });
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "strFreqNums"),
                Caption = string.Format(InvariantCulture, "strFreqNums"),
                DataType = typeof(string),
                DefaultValue = string.Empty
            });
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "strHitNums"),
                Caption = string.Format(InvariantCulture, "strHitNums"),
                DataType = typeof(string),
                DefaultValue = string.Empty
            });
            return dtReturn.Copy();
        }

        private static DataTable CreatTableMissSum()
        {
            using DataTable dtReturn = new DataTable();
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "lngN"),
                Caption = string.Format(InvariantCulture, "lngN"),
                DataType = typeof(int),
                AllowDBNull = false,
                Unique = true
            });
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "lngM1"),
                Caption = string.Format(InvariantCulture, "lngM1"),
                DataType = typeof(int),
                DefaultValue = 0
            });
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "lngM3"),
                Caption = string.Format(InvariantCulture, "lngM3"),
                DataType = typeof(int),
                DefaultValue = 0
            });
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "lngM5"),
                Caption = string.Format(InvariantCulture, "lngM5"),
                DataType = typeof(int),
                DefaultValue = 0
            });
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "lngM10"),
                Caption = string.Format(InvariantCulture, "lngM10"),
                DataType = typeof(int),
                DefaultValue = 0
            });
            dtReturn.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "lngM15"),
                Caption = string.Format(InvariantCulture, "lngM15"),
                DataType = typeof(int),
                DefaultValue = 0
            });
            return dtReturn.Copy();
        }

        private void ReleaseMemory()
        {
            Session.Remove(MissingTotalTID + "dsMissingTotal");
            Session.Remove(MissingTotalTID + "_ddlFreq");
            Session.Remove(MissingTotalTID + "dtMissN1");
            Session.Remove(MissingTotalTID + "dtMissN1Hit");
            Session.Remove(MissingTotalTID + "dtMissN3");
            Session.Remove(MissingTotalTID + "dtMissN3Hit");
            Session.Remove(MissingTotalTID + "dtMissN5");
            Session.Remove(MissingTotalTID + "dtMissN5Hit");
            Session.Remove(MissingTotalTID + "dtMissN10");
            Session.Remove(MissingTotalTID + "dtMissN10Hit");
            Session.Remove(MissingTotalTID);
            ViewState.Clear();
            if (DicThreadMissingTotalT.Keys.Contains(MissingTotalTID + "T01"))
            {
                Thread01 = (Thread)DicThreadMissingTotalT[MissingTotalTID + "T01"];
                if (Thread01.ThreadState == ThreadState.Suspended)
                {
#pragma warning disable CS0618 // 類型或成員已經過時
                    Thread01.Resume();
#pragma warning restore CS0618 // 類型或成員已經過時
                }
                Thread01.Abort();
                Thread01.Join();
                DicThreadMissingTotalT.Remove(MissingTotalTID + "T01");
            }
        }
    }
}