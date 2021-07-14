using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace GalaxyLottoWeb.Pages
{
    public partial class TablePercent01 : BasePage
    {
        private StuGLSearch _gstuSearch;
        private Dictionary<string, int> dicCurrentNums;
        private Dictionary<string, string> dicNumcssclass;

        /// <summary>
        /// 取值範圍
        /// </summary>
        private List<string> lstDelete;
        /// <summary>
        /// 取值範圍內,排除之數據
        /// </summary>
        private List<string> lstNotDelete;

        private string _action;
        private string _requestId;
        private string TablePercent01ID;
        private string StrXmlDirectory { get; set; }
        private string StrFnTPxml { get; set; }
        private string strFnTPHit10xml;
        private string strFnDeletexml;

        /// <summary>
        /// 取值範圍內(已排除),中獎號碼之數據
        /// </summary>
        private Dictionary<int, string> DicDelNum_Hit { get; set; }


        /// <summary>
        /// 取值範圍內,中獎號碼計數
        /// </summary>
        // private Dictionary<string, int> _dicDelHitCount;

        /// <summary>
        /// 取值範圍內(已排除),號碼計數
        /// </summary>
        //Dictionary<string, int> dicDelNum;

        /// <summary>
        /// 所有號碼之數據
        /// </summary>
        //private Dictionary<string, string> _dicAllNum;

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
                dicCurrentNums = new CglData().GetDataNumsDici(_gstuSearch);
                if (ViewState["title"] == null)
                {
                    ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "百分比預測表01", new CglDBData().SetTitleString(_gstuSearch)));
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

            TablePercent01ID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[TablePercent01ID] != null)
            {
                if (ViewState["_gstuSearch"] == null)
                {
                    ViewState.Add("_gstuSearch", (StuGLSearch)Session[TablePercent01ID]);
                }
                else
                {
                    ViewState["_gstuSearch"] = (StuGLSearch)Session[TablePercent01ID];
                };
            }

            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }


        private void InitialArray()
        {
            StrXmlDirectory = System.IO.Path.Combine(Server.MapPath("~"), "xml");

            #region File TPxml path and name
            StrFnTPxml = string.Format(InvariantCulture, "{0}{1}.xml", string.Format(InvariantCulture, "{0}{1}", _gstuSearch.LottoType.ToString(), new CglData().GetCurrentDataDics(_gstuSearch)["lngDateSN"]), "TP");
            #endregion File path and name

            #region File FnTPHit10xml path and name
            strFnTPHit10xml = string.Format(InvariantCulture, "{0}{1}.xml", _requestId, "TPHit10");
            #endregion File path and name

            #region File NDxml path and name
            strFnDeletexml = string.Format(InvariantCulture, "{0}{1}.xml", _requestId, "ND");
            #endregion File path and name

            //dicDelNum = new Dictionary<string, int>();
            //for (int i = 1; i <= new CGLDataSet(gstuSearch.LottoType).LottoNumbers; i++) { dicDelNum.Add(string.Format(InvariantCulture, "{0}", i), 0); }

            //_dicDelHitCount = new Dictionary<string, int>();
            //_dicAllNum = new Dictionary<string, string>();
            UpdatelstDelete(_gstuSearch);
            lstDelete = lstDelete.Distinct().ToList();
            lstNotDelete = lstNotDelete.Distinct().ToList();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void ShowResult(StuGLSearch stuGLSearch)
        {
            lblTitle.Text = Page.Title;
            // 當期資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(stuGLSearch)), true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);

            #region Initialize 
            InitialArray();
            #endregion Initialize 

            #region Setup the Css of numbers
            dicNumcssclass = new Dictionary<string, string>();
            foreach (var KeyPair in dicCurrentNums)
            {
                if (KeyPair.Value > 0)
                {
                    dicNumcssclass.Add(KeyPair.Value.ToString(InvariantCulture), KeyPair.Key);
                }
            }
            #endregion Setup the Css of numbers

            #region dicTablePercent
            if (ViewState["dicTablePercent"] == null)
            {
                ViewState.Add("dicTablePercent", new CglTablePercent().GetTP(stuGLSearch, StrXmlDirectory, StrFnTPxml));
                //Update10(stuSearch00);
            }
            Dictionary<string, object> dicTablePercent = (Dictionary<string, object>)ViewState["dicTablePercent"];
            #endregion

            #region Data Part

            #region dtTpHit
            DataTable dtTpHit = new CglTablePercent().GetTablePercentHit(stuGLSearch, dicTablePercent);
            dtTpHit.TableName = string.Format(InvariantCulture, "dtTpHit_{0}", stuGLSearch.InDataRowsLimit);
            if (dtTpHit.Columns.Contains("TpHit02ID")) { dtTpHit.Columns.Remove("TpHit02ID"); }
            if (dtTpHit.Columns.Contains("TpHit01ID")) { dtTpHit.Columns.Remove("TpHit01ID"); }
            if (dtTpHit.Columns.Contains("lngTotalSN")) { dtTpHit.Columns.Remove("lngTotalSN"); }
            if (dtTpHit.Columns.Contains("intDataRowsLimit")) { dtTpHit.Columns.Remove("intDataRowsLimit"); }
            dtTpHit.DefaultView.Sort = "[srtCheck] DESC , [dblHitRate] ASC , [intTotal] DESC , [intHit] ASC";
            #endregion dtTpHit

            #region dtDHigh0125
            DataTable dtDHigh0125 = new CglTablePercent().GetDHigh0125(stuGLSearch, dicTablePercent);
            dtDHigh0125.TableName = string.Format(InvariantCulture, "dtDHigh0125_{0}_{1}", stuGLSearch.InDataRowsLimit, stuGLSearch.InCriticalNum);
            #endregion dtDHigh0125

            #region dtTpHit10
            DataTable dtTpHit10 = new CglTablePercent().GetTablePercentHit10(stuGLSearch, StrXmlDirectory, strFnTPHit10xml);
            dtTpHit10.TableName = string.Format(InvariantCulture, "TpHit10_{0}", stuGLSearch.InDataRowsLimit);
            dtTpHit10.DefaultView.Sort = "[srtCheck] DESC ";
            #endregion dtTpHit10

            #region dtdicDelNum
            Dictionary<string, string> dicDelNum_All = new CglTablePercent().GetTPDelNum(stuGLSearch, dtTpHit, lstDelete, lstNotDelete);
            Dictionary<string, int> dicDelNum = ConvertToDicDelNum(dicDelNum_All);
            dicDelNum = dicDelNum.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            DataTable dtdicDelNum = new CglFunc().CDicTOTable(dicDelNum, null);
            dtdicDelNum.TableName = string.Format(CultureInfo.InvariantCulture, "DelNum_{0}", stuGLSearch.InDataRowsLimit);
            #endregion dtdicDelNum

            #region dtdicDelNum_Hit
            DicDelNum_Hit = ConvertToDicDelNum_Hit(stuGLSearch, dicDelNum_All);
            DicDelNum_Hit = DicDelNum_Hit.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            DataTable dtdicDelNum_Hit = new CglFunc().CDicTOTable(DicDelNum_Hit, null);
            dtdicDelNum_Hit.TableName = string.Format(CultureInfo.InvariantCulture, "DelNum_Hit{0}", stuGLSearch.InDataRowsLimit);
            #endregion dtdicDelNum_Hit

            #endregion Data Part

            #region Show the result

            StringBuilder sbDelete = new StringBuilder();
            sbDelete.AppendLine(string.Format(InvariantCulture, "DataRowsLimit : {0} ", stuGLSearch.InDataRowsLimit));
            sbDelete.AppendLine(string.Format(InvariantCulture, "Delete : {0} ", string.Join(",", lstDelete.ToArray())));
            sbDelete.AppendLine(string.Format(InvariantCulture, "NotDelete : {0} ", string.Join(",", lstNotDelete.ToArray())));
            pnlDetail.Controls.Add(new GalaxyApp().CreatLabel("lblDelete", sbDelete.ToString(), ""));

            #region dtdicDelNum
            Panel pnlDelNum = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnl{0}", "DelNum"), "max-width");
            pnlDetail.Controls.Add(pnlDelNum);

            #region SetButtons
            HyperLink hlDelNum = new GalaxyApp().CreatHyperLink("hlDelNum", "glbutton glbutton-lightblue ", "統計",
                                                                new Uri(string.Format(InvariantCulture, "#{0}", pnlDelNum.ID)));
            pnlButtons.Controls.Add(hlDelNum);
            #endregion SetButtons

            #region Set table tblDelNum
            Table tblDelNum = new GalaxyApp().CreatTable("gltable", dtdicDelNum.TableName);
            #region Set Columns of table tblHitCount 
            TableHeaderRow thrHeader_DelNum = new TableHeaderRow();
            TableRow trRow_DelNum = new TableRow();
            foreach (DataColumn dcColumn in dtdicDelNum.Columns)
            {
                TableHeaderCell thcColumnFreq = new TableHeaderCell
                {
                    Text = string.Format(InvariantCulture, "{0:00}", int.Parse(dcColumn.ColumnName, InvariantCulture))
                };
                if (dicNumcssclass.ContainsKey(dcColumn.ColumnName))
                {
                    thcColumnFreq.CssClass = dicNumcssclass[dcColumn.ColumnName];
                }
                thrHeader_DelNum.Controls.Add(thcColumnFreq);

                using TableCell tcCell = new TableCell
                {
                    Text = string.Format(InvariantCulture, "{0}", dtdicDelNum.Rows[0][dcColumn.ColumnName].ToString())
                };
                trRow_DelNum.Controls.Add(tcCell);
            }
            tblDelNum.Controls.Add(thrHeader_DelNum);
            tblDelNum.Controls.Add(trRow_DelNum);
            #endregion
            #endregion

            pnlDelNum.Controls.Add(tblDelNum);

            #endregion dtdicDelNum

            #region dtdicDelNum_Hit

            #region Set panel pnlDelNum_Hit
            Panel pnlDelNum_Hit = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnl{0}", "DelNum_Hit"), "max-width");
            pnlDetail.Controls.Add(pnlDelNum_Hit);
            #endregion Set panel pnlHitCount

            GridView gvDelNum_Hit = new GalaxyApp().CreatGridView(dtdicDelNum_Hit.TableName, "gltable ", dtdicDelNum_Hit, true, false);
            gvDelNum_Hit.ShowHeaderWhenEmpty = true;
            gvDelNum_Hit.AllowSorting = true;
            gvDelNum_Hit.Caption = dtdicDelNum_Hit.TableName;
            if (gvDelNum_Hit.Columns.Count == 0)
            {
                for (int i = 0; i < dtdicDelNum_Hit.Columns.Count; i++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = dtdicDelNum_Hit.Columns[i].ColumnName,
                        HeaderText = dtdicDelNum_Hit.Columns[i].ColumnName,
                        ReadOnly = true,
                    };
                    gvDelNum_Hit.Columns.Add(bfCell);
                }
            }
            gvDelNum_Hit.DataBind();
            pnlDelNum_Hit.Controls.Add(gvDelNum_Hit);
            #endregion dtdicDelNum_Hit

            #region dtDHigh0125
            #region SetButtons
            //HyperLink hlDHigh0125 = new HyperLink()
            //{
            //    ID = "hlDelNum",
            //    CssClass = "glbutton glbutton-lightblue ",
            //    Text = "統計",
            //    NavigateUrl = string.Format(InvariantCulture, "#pnl{0}", "DHigh0125")
            //};
            pnlButtons.Controls.Add(hlDelNum);
            #endregion SetButtons

            #region Set panel pnlDelNum
            //Panel pnlDHigh0125 = new Panel() { ID = string.Format(InvariantCulture, "pnl{0}", "DHigh0125"), CssClass = "max-width" };
            pnlDetail.Controls.Add(pnlDelNum);
            #endregion Set panel pnlHitCount

            #region Set table tblDelNum
            Table tblDHigh0125 = new GalaxyApp().CreatTable("gltable", string.Format(InvariantCulture, "{0}({1})", dtDHigh0125.TableName, dtDHigh0125.Columns.Count));
            #region Set Columns of table tblHitCount 
            using (TableHeaderRow thrHeader_DHigh0125 = new TableHeaderRow())
            {

                using TableRow trRow_DHigh0125 = new TableRow();
                foreach (DataColumn dcColumn in dtDHigh0125.Columns)
                {
                    using (TableHeaderCell thcColumnFreq = new TableHeaderCell())
                    {

                        thcColumnFreq.Text = string.Format(InvariantCulture, "{0:00}", int.Parse(dcColumn.ColumnName, InvariantCulture));
                        thrHeader_DHigh0125.Controls.Add(thcColumnFreq);
                        if (dicNumcssclass.ContainsKey(int.Parse(dcColumn.ColumnName, InvariantCulture).ToString(InvariantCulture)))
                        {
                            thcColumnFreq.CssClass = dicNumcssclass[int.Parse(dcColumn.ColumnName, InvariantCulture).ToString(InvariantCulture)];
                        }
                    }
                    using TableCell tcCell = new TableCell
                    {
                        Text = string.Format(InvariantCulture, "{0}", dtDHigh0125.Rows[0][dcColumn.ColumnName].ToString())
                    };
                    trRow_DHigh0125.Controls.Add(tcCell);
                }
                #endregion
                tblDHigh0125.Controls.Add(thrHeader_DHigh0125);
                tblDHigh0125.Controls.Add(trRow_DHigh0125);
            }
            #endregion

            pnlDelNum.Controls.Add(tblDHigh0125);

            #endregion dtdicDelNum

            #region dtTpHit

            Panel pnlTpHit = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnl{0}", "TpHit"), "max-width");
            pnlDetail.Controls.Add(pnlTpHit);

            #region SetButtons
            HyperLink hlTpHit = new GalaxyApp().CreatHyperLink("hlTpHit", "glbutton glbutton-lightblue ", "TpHit",
                                                               new Uri(string.Format(InvariantCulture, "#{0}", pnlTpHit.ID)));
            pnlButtons.Controls.Add(hlTpHit);
            #endregion SetButtons

            #region gvdtTpHit
            GridView gvdtTpHit = new GalaxyApp().CreatGridView(dtTpHit.TableName, "gltable ", dtTpHit, true, false);
            gvdtTpHit.AllowSorting = true;
            gvdtTpHit.Caption = string.Format(InvariantCulture, "{0}({1})", dtTpHit.TableName, dtTpHit.Rows.Count);
            gvdtTpHit.ShowHeaderWhenEmpty = true;
            if (gvdtTpHit.Columns.Count == 0)
            {
                for (int i = 0; i < dtTpHit.Columns.Count; i++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = dtTpHit.Columns[i].ColumnName,
                        HeaderText = dtTpHit.Columns[i].ColumnName,
                        ReadOnly = true,
                    };

                    if (i > 3)
                    {
                        bfCell.HeaderText = string.Format(InvariantCulture, "{0:00}", int.Parse(dtTpHit.Columns[i].ColumnName.Substring(4), InvariantCulture));
                        if (dicNumcssclass.ContainsKey(int.Parse(dtTpHit.Columns[i].ColumnName.Substring(4), InvariantCulture).ToString(InvariantCulture)))
                        {
                            bfCell.HeaderStyle.CssClass = dicNumcssclass[int.Parse(dtTpHit.Columns[i].ColumnName.Substring(4), InvariantCulture).ToString(InvariantCulture)];
                            bfCell.ItemStyle.CssClass = dicNumcssclass[int.Parse(dtTpHit.Columns[i].ColumnName.Substring(4), InvariantCulture).ToString(InvariantCulture)];
                        }
                    }
                    gvdtTpHit.Columns.Add(bfCell);
                }
            }
            gvdtTpHit.RowDataBound += GvdtTpHit_RowDataBound;
            gvdtTpHit.DataBind();
            pnlTpHit.Controls.Add(gvdtTpHit);
            #endregion gvdtTpHit

            #endregion dtTpHit

            #region dtTpHit10

            Panel pnlTpHit10 = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnl{0}", "TpHit10"), "max-width");
            pnlDetail.Controls.Add(pnlTpHit10);

            #region SetButtons
            HyperLink hlTpHit10 = new GalaxyApp().CreatHyperLink("hlTpHit10", "glbutton glbutton-lightblue ", "TpHit10",
                                                                 new Uri(string.Format(InvariantCulture, "#{0}", pnlTpHit10.ID)));
            pnlButtons.Controls.Add(hlTpHit10);
            #endregion SetButtons

            #region gvTpHit10
            GridView gvTpHit10 = new GalaxyApp().CreatGridView(dtTpHit10.TableName, "gltable ", dtTpHit10, true, false);
            gvTpHit10.AllowSorting = true;
            gvTpHit10.Caption = string.Format(InvariantCulture, "{0}({1})", dtTpHit10.TableName, dtTpHit10.Rows.Count);
            gvTpHit10.ShowHeaderWhenEmpty = true;
            if (gvTpHit10.Columns.Count == 0)
            {
                for (int i = 0; i < dtTpHit10.Columns.Count; i++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = dtTpHit10.Columns[i].ColumnName,
                        HeaderText = dtTpHit10.Columns[i].Caption,
                        ReadOnly = true,
                    };
                    if (i > 0)
                    {
                        if (bfCell.HeaderText.Contains("T"))
                        {
                            bfCell.HeaderStyle.CssClass = "row_lightyellow";
                            bfCell.ItemStyle.CssClass = "row_lightyellow";
                        }
                    }
                    gvTpHit10.Columns.Add(bfCell);
                }
            }
            gvTpHit10.RowDataBound += GvTpHit10_RowDataBound; ;
            gvTpHit10.DataBind();
            pnlTpHit10.Controls.Add(gvTpHit10);
            #endregion gvTpHit10

            #endregion dtTpHit10

            #endregion Show the result
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private Dictionary<int, string> ConvertToDicDelNum_Hit(StuGLSearch stuSearch00, Dictionary<string, string> dicDelNum_All)
        {
            Dictionary<int, string> dicDelNumHit = new Dictionary<int, string>();
            List<int> lstCurrentNums = (List<int>)new CglData().GetDataNumsLst(stuSearch00);
            foreach (var item in dicDelNum_All)
            {
                foreach (var num in item.Value.Split(',').ToArray())
                {
                    if (lstCurrentNums.Contains(int.Parse(num, InvariantCulture)))
                    {

                        if (dicDelNumHit.ContainsKey(int.Parse(num, InvariantCulture)))
                        {
                            dicDelNumHit[int.Parse(num, InvariantCulture)] += "," + item.Key;
                        }
                        else
                        {
                            dicDelNumHit.Add(int.Parse(num, InvariantCulture), item.Key);
                        }
                    }
                }
            }
            return dicDelNumHit;
        }

        private Dictionary<string, int> ConvertToDicDelNum(Dictionary<string, string> dicDelNum_All)
        {
            Dictionary<string, int> dicDelNum = new Dictionary<string, int>();
            for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; i++) { dicDelNum.Add(string.Format(InvariantCulture, "{0}", i), 0); }
            foreach (var item in dicDelNum_All)
            {
                foreach (var num in item.Value.Split(',').ToArray())
                {
                    if (dicDelNum.ContainsKey(num))
                    {
                        dicDelNum[num]++;
                    }
                }
            }
            return dicDelNum;
        }

        private void GvTpHit10_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 1; i < e.Row.Cells.Count - 2; i++)
                {
                    if (int.Parse(e.Row.Cells[i].Text, InvariantCulture) == 0)
                    {
                        if (i % 2 == 1)
                        {
                            e.Row.Cells[i].CssClass = "row_lightyellow fontcolor_white";
                        }
                        else
                        {
                            e.Row.Cells[i].CssClass = "fontcolor_white";
                        }
                    }
                }
                if (lstDelete.Contains(e.Row.Cells[0].Text) && !lstNotDelete.Contains(e.Row.Cells[0].Text))
                {
                    e.Row.CssClass = "row_lightseagreen";
                }
            }

        }

        private void GvdtTpHit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 4; i < e.Row.Cells.Count; i++)
                {
                    if (int.Parse(e.Row.Cells[i].Text, InvariantCulture) == 0)
                    {
                        if (dicNumcssclass.ContainsKey((i - 3).ToString(InvariantCulture)))
                        {
                            e.Row.Cells[i].CssClass = dicNumcssclass[(i - 3).ToString(InvariantCulture)];
                        }
                        e.Row.Cells[i].CssClass = e.Row.Cells[i].CssClass + " fontcolor_white";
                    }
                }
                if (lstDelete.Contains(e.Row.Cells[0].Text) && !lstNotDelete.Contains(e.Row.Cells[0].Text))
                {
                    e.Row.CssClass = "row_lightseagreen";
                }
            }
        }

        private void UpdatelstDelete(StuGLSearch stuSearch00)
        {
            XDocument xDoc;
            string strFilePath = System.IO.Path.Combine(StrXmlDirectory, strFnDeletexml);
            if (!(bool)new CglFunc().FileExist(StrXmlDirectory, strFnDeletexml))
            {
                xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
                xDoc.AddFirst(new XElement("Data"));
                xDoc.Element("Data").Add(new XElement("Delete", ""));
                xDoc.Element("Data").Add(new XElement("NotDelete", ""));
                xDoc.Element("Data").Add(new XElement("DataRowsLimit", stuSearch00.InDataRowsLimit));
                xDoc.Element("Data").Add(new XElement("command", ""));
                xDoc.Save(strFilePath);
            }
            xDoc = XDocument.Load(strFilePath);
            string strDelete = xDoc.Root.Element("Delete").Value;
            strDelete = new CglFunc().GetTestNum(strDelete);
            //if (strDelete.Length > 0) { lstDelete = strDelete.Split(',').ToList(); } else { lstDelete = new List<string>(); }
            lstDelete = (strDelete.Length > 0) ? strDelete.Split(',').ToList() : new List<string>();
            if (stuSearch00.StrDeletes.Length > 0) { strDelete = new CglFunc().GetTestNum(stuSearch00.StrDeletes); lstDelete = lstDelete.Concat(strDelete.Split(',').ToList()).ToList(); }

            string strNotDelete = xDoc.Root.Element("NotDelete").Value;
            strNotDelete = new CglFunc().GetTestNum(strNotDelete);
            //if (strNotDelete.Length > 0) { lstNotDelete = strNotDelete.Split(',').ToList(); } else { lstNotDelete = new List<string>(); }
            lstNotDelete = (strNotDelete.Length > 0) ? strNotDelete.Split(',').ToList() : new List<string>();
            if (stuSearch00.StrNotDeletes.Length > 0) { strNotDelete = new CglFunc().GetTestNum(stuSearch00.StrNotDeletes); lstNotDelete = lstNotDelete.Concat(strNotDelete.Split(',').ToList()).ToList(); }
            //intDataRowsLimit = int.Parse(xDoc.Root.Element("DataRowsLimit").Value);
        }

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            _action = Request["action"];
            _requestId = Request["id"];
            Session.Remove(name: _action + _requestId);
        }

    }
}