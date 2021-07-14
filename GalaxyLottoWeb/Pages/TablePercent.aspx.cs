using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace GalaxyLottoWeb.Pages
{
    public partial class TablePercent : BasePage
    {
        private StuGLSearch _gstuSearch;
        private string TablePercentID;

        private Dictionary<string, int> DicCurrentNums { get; set; }
        private Dictionary<string, string> DicNumcssclass { get; set; }
        /// <summary>
        /// 取值範圍內,中獎號碼計數
        /// </summary>
        public Dictionary<string, int> DicDelHitCount { get; private set; }
        /// <summary>
        /// 取值範圍
        /// </summary>
        private List<string> LstDelete { get; set; }
        /// <summary>
        /// 取值範圍內,排除之數據
        /// </summary>
        private List<string> LstNotDelete { get; set; }

        /// <summary>
        /// 取值範圍內(已排除),號碼計數
        /// </summary>
        private Dictionary<string, int> DicDelNum { get; set; }

        /// <summary>
        /// 所有號碼之數據
        /// </summary>
        private Dictionary<string, string> DicAllNum { get; set; }

        /// <summary>
        /// 計數資料庫 期數是否 >=50
        /// </summary>        
        private static bool blDataTableCount;
        private int intDataRowsLimit = 50;
        private string _action, _requestId, strHtmlDirectory, strFnTPxml, strFnDeletexml;

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
                DicCurrentNums = new CglData().GetDataNumsDici(_gstuSearch);
                if (ViewState["title"] == null)
                {
                    ViewState.Add("title", string.Format(InvariantCulture, "{0}:{1}", "百分比預測表", new CglDBData().SetTitleString(_gstuSearch)));
                }
                Page.Title = ViewState["title"].ToString();
                ShowResult(_gstuSearch);
            }
            ResetSearchOrder(TablePercentID);
            //Search.CurrentSearchOrderID = string.Empty;
        }

        private void SetupViewState()
        {
            _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

            if (ViewState["action"] == null) { ViewState.Add("action", _action); }
            if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

            TablePercentID = _action + _requestId;
            if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[TablePercentID] != null)
            {
                if (ViewState["_gstuSearch"] == null)
                {
                    ViewState.Add("_gstuSearch", (StuGLSearch)Session[TablePercentID]);
                }
                else
                {
                    ViewState["_gstuSearch"] = (StuGLSearch)Session[TablePercentID];
                };
            }

            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
        }

        private void InitialArray()
        {
            strHtmlDirectory = System.IO.Path.Combine(Server.MapPath("~"), "xml");
            #region File TPxml path and name
            strFnTPxml = string.Format(InvariantCulture, "{0}{1}.xml", "TP", string.Format(InvariantCulture, "{0}_{1}",_action,_requestId));
            #endregion File path and name

            #region File NDxml path and name
            strFnDeletexml = string.Format(InvariantCulture, "{0}{1}.xml", "ND", _requestId);
            #endregion File path and name

            DicDelNum = new Dictionary<string, int>();
            for (int i = 1; i <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; i++) { DicDelNum.Add(string.Format(InvariantCulture, "{0}", i), 0); }

            DicDelHitCount = new Dictionary<string, int>();
            DicAllNum = new Dictionary<string, string>();
            UpdatelstDelete(_gstuSearch);
            LstDelete = LstDelete.Distinct().ToList();
            LstNotDelete = LstNotDelete.Distinct().ToList();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:不要將常值當做已當地語系化的參數傳遞", MessageId = "System.Web.UI.WebControls.HyperLink.set_Text(System.String)")]
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

            #region dicTablePercent
            if (ViewState["dicTablePercent"] == null)
            {
                ViewState.Add("dicTablePercent", new CglTablePercent().GetTP(stuGLSearch, strHtmlDirectory, strFnTPxml));
            }
            Dictionary<string, object> dicTablePercent = (Dictionary<string, object>)ViewState["dicTablePercent"];
            #endregion

            #region Show the result
            #region Setup the Css of numbers
            DicNumcssclass = new Dictionary<string, string>();
            foreach (var KeyPair in DicCurrentNums)
            {
                if (KeyPair.Value > 0)
                {
                    DicNumcssclass.Add(KeyPair.Value.ToString(InvariantCulture), KeyPair.Key);
                }
            }
            #endregion Setup the Css of numbers

            #region Loop
            foreach (var KeyVal in dicTablePercent)
            {
                //string strTitle = string.Empty;
                string strButtonText = string.Empty;
                if (KeyVal.Key == "dicTotal")
                {
                    #region SetButtons
                    HyperLink hlButton = new HyperLink()
                    {
                        ID = string.Format(InvariantCulture, "hl{0}", KeyVal.Key),
                        CssClass = "glbutton glbutton-lightblue ",
                        Text = "全域",
                        NavigateUrl = string.Format(InvariantCulture, "#pnl{0}", KeyVal.Key)
                    };
                    pnlButtons.Controls.Add(hlButton);
                    #endregion SetButtons

                    #region Set panel Frequency Result
                    Dictionary<string, object> dicTotal = (Dictionary<string, object>)KeyVal.Value;
                    DataSet dsdicTotal = (DataSet)dicTotal["dsdicTotal"]; ;
                    Panel pnldicTotal = new Panel() { ID = string.Format(InvariantCulture, "pnl{0}", KeyVal.Key), CssClass = "max-width" };

                    foreach (DataTable dtdicTotal in dsdicTotal.Tables)
                    {
                        #region Set table tblEach
                        Table tblEach = new Table() { CssClass = "gltable", Caption = dtdicTotal.TableName };
                        #region Set Columns of table dgFreqResult 
                        TableHeaderRow thrHeader = new TableHeaderRow();
                        TableRow trRow = new TableRow();
                        foreach (DataColumn dcColumn in dtdicTotal.Columns)
                        {
                            TableHeaderCell thcColumnFreq = new TableHeaderCell { Text = string.Format(InvariantCulture, "{0:00}", int.Parse(dcColumn.ColumnName, InvariantCulture)), };
                            if (DicNumcssclass.ContainsKey(dcColumn.ColumnName))
                            {
                                thcColumnFreq.CssClass = DicNumcssclass[dcColumn.ColumnName];
                            }
                            thrHeader.Controls.Add(thcColumnFreq);
                            TableCell tcCell = new TableCell()
                            {
                                Text = string.Format(InvariantCulture, "{0}", dtdicTotal.Rows[0][dcColumn.ColumnName].ToString()),
                            };
                            trRow.Controls.Add(tcCell);
                        }
                        #endregion
                        tblEach.Controls.Add(thrHeader);
                        tblEach.Controls.Add(trRow);
                        #endregion
                        pnldicTotal.Controls.Add(tblEach);
                    }

                    #endregion Set panel Frequency Result

                    pnlDetail.Controls.Add(pnldicTotal);
                }
                else
                {
                    Dictionary<string, object> dicEachCompare = (Dictionary<string, object>)KeyVal.Value;
                    int DataRows = int.Parse(dicEachCompare["DataRows"].ToString(), InvariantCulture);
                    DataSet dsFreq = (DataSet)dicEachCompare["dsFreq"];
                    DataSet dsHot = (DataSet)dicEachCompare["dsHot"];

                    #region SetButtons

                    #region Set Button text
                    List<string> lstCompare = KeyVal.Key.Split('#').ToList();
                    List<string> lstButtonText = new List<string>();
                    foreach (string strCompareOption in lstCompare)
                    {
                        lstButtonText.Add(new CglFunc().ConvertFieldNameId(strCompareOption, 1));
                    }
                    strButtonText = string.Join(",", lstButtonText.ToArray());
                    #endregion Set Button text

                    HyperLink btButton = new HyperLink()
                    {
                        ID = string.Format(InvariantCulture, "hl{0}", KeyVal.Key),
                        CssClass = "glbutton ",
                        Text = strButtonText,
                        NavigateUrl = string.Format(InvariantCulture, "#pnl{0}", KeyVal.Key)
                    };

                    if (DataRows >= intDataRowsLimit)
                    {
                        btButton.CssClass += "glbutton-lightblue  "; blDataTableCount = true;
                    }
                    else { btButton.CssClass += "glbutton-grey  "; blDataTableCount = false; }

                    if (KeyVal.Key == "strDayTwentyEight#strHourTwentyEight") { blDataTableCount = false; };

                    pnlButtons.Controls.Add(btButton);
                    #endregion SetButtons

                    #region Set Each Compare Panel
                    Panel pnlEachCompare = new Panel() { ID = string.Format(InvariantCulture, "pnl{0}", KeyVal.Key), CssClass = "max-width" };
                    pnlEachCompare.Controls.Add(new Label() { Text = string.Format(InvariantCulture, "相同 ({0}) ({1:00}期)", strButtonText, DataRows), CssClass = "gllabel" });
                    #region Hot Part
                    Table tblHotOutline = new Table() { CssClass = "max-width", Width = 20, };
                    TableRow trRowOutline = new TableRow();
                    foreach (DataTable dtHot in dsHot.Tables)
                    {
                        TableCell tcOutline = new TableCell() { VerticalAlign = VerticalAlign.Top, };
                        dtHot.DefaultView.Sort = "[S0115] DESC , [S0105] DESC , [Miss] ASC, [Nums] ASC ";
                        GridView gvHot = new GridView()
                        {
                            ID = string.Format(InvariantCulture, "{0}gvHot", KeyVal.Key),
                            ShowHeaderWhenEmpty = true,
                            AutoGenerateColumns = false,
                            AllowSorting = true,
                            EnableViewState = false,
                            ViewStateMode = ViewStateMode.Disabled,
                            CssClass = "gltable ",
                            GridLines = GridLines.Horizontal,
                            DataSource = dtHot.DefaultView,
                            Caption = dtHot.TableName
                        };
                        if (gvHot.Columns.Count == 0)
                        {
                            for (int i = 0; i < dtHot.Columns.Count; i++)
                            {
                                BoundField bfCell = new BoundField()
                                {
                                    DataField = dtHot.Columns[i].ColumnName,
                                    HeaderText = new CglFunc().ConvertFieldNameId(dtHot.Columns[i].ColumnName, 1),
                                    ReadOnly = true,
                                };
                                gvHot.Columns.Add(bfCell);
                            }
                        }
                        gvHot.RowDataBound += GvHot_RowDataBound; ;
                        gvHot.DataBind();
                        tcOutline.Controls.Add(gvHot);
                        trRowOutline.Controls.Add(tcOutline);
                    }
                    tblHotOutline.Controls.Add(trRowOutline);
                    pnlEachCompare.Controls.Add(tblHotOutline);
                    #endregion Hot Part

                    #region Freq Part
                    foreach (DataTable dtFreq in dsFreq.Tables)
                    {
                        if (dtFreq.TableName != "DataAll" && dtFreq.TableName != "MissAll")
                        {
                            #region Set table tblEach
                            Table tblEach = new Table() { CssClass = "gltable", Caption = dtFreq.TableName };
                            #region Set Columns of table dgFreqResult 
                            TableHeaderRow thrHeader = new TableHeaderRow();
                            TableRow trRow = new TableRow();
                            foreach (DataColumn dcColumn in dtFreq.Columns)
                            {
                                if (dcColumn.ColumnName != "intRows")
                                {
                                    TableHeaderCell thcColumnFreq = new TableHeaderCell { Text = string.Format(InvariantCulture, "{0:00}", int.Parse(dcColumn.ColumnName.Substring(4), InvariantCulture)), };
                                    if (DicNumcssclass.ContainsKey(int.Parse(dcColumn.ColumnName.Substring(4), InvariantCulture).ToString(InvariantCulture)))
                                    {
                                        thcColumnFreq.CssClass = DicNumcssclass[int.Parse(dcColumn.ColumnName.Substring(4), InvariantCulture).ToString(InvariantCulture)];
                                    }
                                    thrHeader.Controls.Add(thcColumnFreq);
                                    TableCell tcCell = new TableCell()
                                    {
                                        Text = string.Format(InvariantCulture, "{0}", dtFreq.Rows[0][dcColumn.ColumnName].ToString()),
                                    };
                                    trRow.Controls.Add(tcCell);
                                }
                            }
                            #endregion
                            tblEach.Controls.Add(thrHeader);
                            tblEach.Controls.Add(trRow);
                            #endregion
                            pnlEachCompare.Controls.Add(tblEach);
                        }
                    }
                    #endregion Freq Part

                    #endregion Set Each Compare Panel
                    pnlDetail.Controls.Add(pnlEachCompare);
                }
            }
            #endregion Loop

            #endregion Show the result
        }

        private void UpdatelstDelete(StuGLSearch stuGLSearch)
        {
            XDocument xDoc;
            string strFilePath = System.IO.Path.Combine(strHtmlDirectory, strFnDeletexml);
            if (!(bool)new CglFunc().FileExist(strHtmlDirectory, strFnDeletexml))
            {
                xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
                xDoc.AddFirst(new XElement("Data"));
                xDoc.Element("Data").Add(new XElement("Delete", ""));
                xDoc.Element("Data").Add(new XElement("NotDelete", ""));
                xDoc.Element("Data").Add(new XElement("DataRowsLimit", stuGLSearch.InDataRowsLimit));
                xDoc.Element("Data").Add(new XElement("command", ""));
                xDoc.Save(strFilePath);
            }
            xDoc = XDocument.Load(strFilePath);
            string strDelete = xDoc.Root.Element("Delete").Value;
            strDelete = new CglFunc().GetTestNum(strDelete);
            //if (strDelete.Length > 0) { lstDelete = strDelete.Split(',').ToList(); } else { lstDelete = new List<string>(); }
            LstDelete = (strDelete.Length > 0) ? strDelete.Split(',').ToList() : new List<string>();

            string strNotDelete = xDoc.Root.Element("NotDelete").Value;
            strNotDelete = new CglFunc().GetTestNum(strNotDelete);
            //if (strNotDelete.Length > 0) { lstNotDelete = strNotDelete.Split(',').ToList(); } else { lstNotDelete = new List<string>(); }
            LstNotDelete = (strNotDelete.Length > 0) ? strNotDelete.Split(',').ToList() : new List<string>();

            intDataRowsLimit = int.Parse(xDoc.Root.Element("DataRowsLimit").Value, InvariantCulture);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void GvHot_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int intNum = int.Parse(e.Row.Cells[0].Text, InvariantCulture);//號碼
                int C0105 = int.Parse(e.Row.Cells[1].Text, InvariantCulture); //1-5 期 數據
                int C0115 = int.Parse(e.Row.Cells[5].Text, InvariantCulture); //1-15 期 數據                
                int Miss = int.Parse(e.Row.Cells[4].Text, InvariantCulture);  //遺漏數
                string strCheck = string.Format(InvariantCulture, "{0}{1}{2:d2}", C0115, C0105, Miss);
                bool blDelete = LstDelete.Contains(strCheck);
                bool blNotDelete = LstNotDelete.Contains(strCheck);
                bool blHitNums = DicNumcssclass.ContainsKey(intNum.ToString(InvariantCulture));
                UpdateAllNum(blDataTableCount, intNum, strCheck);
                switch (C0115)
                {
                    case 0:
                        #region C0115 == 2
                        if (blDelete)
                        {
                            if (UpdateDel(blNotDelete, blDataTableCount, strCheck))
                            {
                                e.Row.CssClass = "row_lightseagreen";
                            }
                        }
                        #endregion C0115 == 2
                        break;
                    case 1:
                        #region C0115 == 2
                        if (blDelete)
                        {
                            if (UpdateDel(blNotDelete, blDataTableCount, strCheck))
                            {
                                e.Row.CssClass = "row_lightseagreen";
                            }
                        }
                        #endregion C0115 == 2
                        break;
                    case 2:
                        #region C0115 == 2
                        if (blDelete)
                        {
                            if (UpdateDel(blNotDelete, blDataTableCount, strCheck))
                            {
                                e.Row.CssClass = "row_lightseagreen";
                            }
                        }
                        #endregion C0115 == 2
                        break;

                    case 3:
                        #region C0115 == 3
                        if (blDelete)
                        {
                            if (UpdateDel(blNotDelete, blDataTableCount, strCheck))
                            {
                                e.Row.CssClass = "row_lightskyblue";
                            }
                        }
                        #endregion C0115 == 3
                        break;

                    case 4:
                        #region C0115 == 4
                        if (blDelete)
                        {
                            if (UpdateDel(blNotDelete, blDataTableCount, strCheck))
                            {
                                e.Row.CssClass = (C0105 > 0) ? "row_lightpink" : "row_lightgreen";
                            }
                        }
                        #endregion C0115 == 4
                        break;

                    case 5:
                        #region C0115 == 5            
                        if (blDelete)
                        {
                            if (UpdateDel(blNotDelete, blDataTableCount, strCheck))
                            {
                                e.Row.CssClass = (C0105 > 0) ? "row_lightblue" : "row_lightgreen";
                            }
                        }
                        #endregion C0115 == 5
                        break;

                    default:
                        #region C0115 >= 6
                        if (blDelete && C0115 >= 6)
                        {
                            if (UpdateDel(blNotDelete, blDataTableCount, strCheck))
                            {
                                e.Row.CssClass = (C0105 > 0) ? "row_yellow" : "row_lightgreen";
                            }
                        }
                        #endregion C0115 >= 6
                        break;
                }

                if (blHitNums)
                {
                    e.Row.Cells[0].CssClass = DicNumcssclass[intNum.ToString(InvariantCulture)];
                }
            }
        }

        /// <summary>
        /// 更新所有號碼之數據
        /// </summary>
        /// <param name="blDataTableCount"></param>
        /// <param name="intNum"></param>
        /// <param name="strCheck"></param>
        private void UpdateAllNum(bool blDataTableCount, int intNum, string strCheck)
        {
            if (blDataTableCount)
            {
                if (DicAllNum.ContainsKey(strCheck))
                {
                    DicAllNum[strCheck] += "," + intNum.ToString(InvariantCulture);
                }
                else
                {
                    DicAllNum.Add(strCheck, intNum.ToString(InvariantCulture));
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]

        private bool UpdateDel(bool blNotDelete, bool blDataTableCount, string strCheck)
        {
            if (strCheck == null) { throw new ArgumentNullException(nameof(strCheck)); }

            bool blChangCss = false;
            if (!blNotDelete)
            {
                if (blDataTableCount)
                {
                    blChangCss = true;
                }
            }
            return blChangCss;
        }

        protected void BtnCloseClick(object sender, EventArgs e)
        {
            _action = Request["action"];
            _requestId = Request["id"];
            Session.Remove(name: _action + _requestId);
        }

    }
}