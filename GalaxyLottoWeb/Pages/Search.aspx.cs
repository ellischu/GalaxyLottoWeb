using GalaxyLotto.ClassLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

namespace GalaxyLottoWeb.Pages
{
    [Serializable]
    public partial class Search : BasePage
    {
        //private StuGLSearch _gstuSearch = new StuGLSearch(TargetTable.Lotto539);
        private Dictionary<string, object> DicButtons { get; set; }

        private Dictionary<string, object> DicOptions { get; set; }

        //private string Action { get; set; } = string.Empty;

        //private string RequestId { get; set; } = string.Empty;

        //private string PageFileName { get; set; } = string.Empty;

        private string LocalBrowserType { get; set; }

        private string LocalIP { get; set; }

        private string KeySearchOrder { get; set; }

        // ---------------------------------------------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            Form.Target = "_blank";
            CheckSearchOrder();
            SetupDdlCompare();
            SetupDdlFCheck();
            SetupOptions();
            SetupButtons();
            if (!IsPostBack)
            {
                StuGLSearch stuSearchTemp = new StuGLSearch(TargetTable.Lotto539);
                Session.Add("SearchOption", stuSearchTemp);
                DataRangeStartInit(stuSearchTemp.LottoType);
                TurnOnButtons("cFreq");
            };
            if (ViewState["SearchOption"] == null && Session["SearchOption"] != null) { ViewState["SearchOption"] = Session["SearchOption"]; }
            if (Session["SearchOption"] == null && ViewState["SearchOption"] != null) { Session["SearchOption"] = ViewState["SearchOption"]; }
        }

        #region initialize
        // ---------------------------------------------------------------------------------------------------------

        private void CheckSearchOrder()
        {
            if (ServerOption == null) { ServerOption = new Dictionary<string, object>(); }
            LocalBrowserType = Request.Browser.Type;
            LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            KeySearchOrder = string.Format(InvariantCulture, "{0}#{1}#dtSearchOrder", LocalIP, LocalBrowserType);
            if (ServerOption.ContainsKey(KeySearchOrder))
            {
                btnShowSearchOrder.Visible = ((DataTable)ServerOption[KeySearchOrder]).Rows.Count > 0;
            }
            else
            {
                btnShowSearchOrder.Visible = false;
            }
        }

        private void SetupDdlCompare()
        {
            if (ViewState["daOptions"] == null) { ViewState["daOptions"] = SetOptionsFromXML(); }
            if (cmbCompare01.Items.Count == 0 && ViewState["daOptions"] != null)
            {
                SetCombobox(ref cmbCompare01, ((DataSet)ViewState["daOptions"]).Tables["cmbCompare"]);
                SetCombobox(ref cmbCompare02, ((DataSet)ViewState["daOptions"]).Tables["cmbCompare"]);
                SetCombobox(ref cmbCompare03, ((DataSet)ViewState["daOptions"]).Tables["cmbCompare"]);
                SetCombobox(ref cmbCompare04, ((DataSet)ViewState["daOptions"]).Tables["cmbCompare"]);
                SetCombobox(ref cmbCompare05, ((DataSet)ViewState["daOptions"]).Tables["cmbCompare"]);
            }
        }

        private void SetupDdlFCheck()
        {
            if (ViewState["daOptions"] == null) { ViewState["daOptions"] = SetOptionsFromXML(); }
            if (cmbFCheck01.Items.Count == 0 && ViewState["daOptions"] != null)
            {
                SetCombobox(ref cmbFCheck01, ((DataSet)ViewState["daOptions"]).Tables["cmbFCheck"]);
                SetCombobox(ref cmbFCheck02, ((DataSet)ViewState["daOptions"]).Tables["cmbFCheck"]);
                SetCombobox(ref cmbFCheck03, ((DataSet)ViewState["daOptions"]).Tables["cmbFCheck"]);
                SetCombobox(ref cmbFCheck04, ((DataSet)ViewState["daOptions"]).Tables["cmbFCheck"]);
                SetCombobox(ref cmbFCheck05, ((DataSet)ViewState["daOptions"]).Tables["cmbFCheck"]);
                SetCombobox(ref cmbFCheck06, ((DataSet)ViewState["daOptions"]).Tables["cmbFCheck"]);
            }
        }

        /// <summary>
        /// Read From XML file
        /// </summary>
        /// <returns></returns>
        private static DataSet SetOptionsFromXML()
        {
            DataSet dsReturn;
            using (DataSet dsOptions = new DataSet("Options"))
            {
                dsOptions.Locale = InvariantCulture;
                //string strXmlFileName;
                //strXmlFileName = string.Format(InvariantCulture, "{0}\\{1}", Environment.CurrentDirectory, "OptionSearch.xml");            
                XmlDocument xmldoc = new XmlDocument { XmlResolver = null };
                string optionSearch = Properties.Resources.OptionSearch;
                System.IO.StringReader sreader = new System.IO.StringReader(optionSearch);
                using (XmlReader reader = XmlReader.Create(sreader, new XmlReaderSettings() { XmlResolver = null }))
                {
                    xmldoc.Load(reader);
                }
                //xmldoc.LoadXml(optionSearch);
                //xmldoc.Load(@strXmlFileName);
                foreach (XmlNode node in xmldoc.DocumentElement)
                {
                    if (node.HasChildNodes)
                    {
                        string TableName = string.Empty;
                        DataRow drDataRow;
                        foreach (XmlNode child in node.ChildNodes)
                        {
                            if (child.Name == "name")
                            {
                                TableName = child.InnerText;
                                dsOptions.Tables.Add(TableName);
                                if (dsOptions.Tables[TableName].Columns.Count == 0)
                                {
                                    using (DataColumn dcColumn = new DataColumn())
                                    {
                                        dcColumn.ColumnName = "id";
                                        dcColumn.DataType = typeof(string);
                                        dsOptions.Tables[TableName].Columns.Add(dcColumn);
                                    }
                                    using (DataColumn dcColumn = new DataColumn())
                                    {
                                        dcColumn.ColumnName = "description";
                                        dcColumn.DataType = typeof(string);
                                        dsOptions.Tables[TableName].Columns.Add(dcColumn);
                                    }
                                }
                            }
                            else
                            {
                                if (child.HasChildNodes)
                                {
                                    Dictionary<string, string> dicDataRow = new Dictionary<string, string>();
                                    foreach (XmlNode child1 in child.ChildNodes)
                                    {
                                        dicDataRow.Add(child1.Name, child1.InnerText);
                                        if (child1.Name == "description")
                                        {
                                            drDataRow = dsOptions.Tables[TableName].NewRow();
                                            drDataRow["id"] = dicDataRow["id"];
                                            drDataRow["description"] = dicDataRow["description"];
                                            dsOptions.Tables[TableName].Rows.Add(drDataRow);
                                            dicDataRow.Clear();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                dsReturn = dsOptions;
            };
            return dsReturn;
        }

        private static void SetCombobox(ref DropDownList comboBox, DataTable dataTable)
        {
            if (comboBox.DataSource != null && (comboBox.ID == "cmbLottoType")) { return; }
            comboBox.DataSource = dataTable.DefaultView;
            comboBox.DataValueField = "id";
            comboBox.DataTextField = "description";
            comboBox.DataBind();
            comboBox.SelectedIndex = 0;
        }

        #endregion initialize

        private void SearchOption_init()
        {
            StuGLSearch stuSearchTemp = (StuGLSearch)Session["SearchOption"];

            #region base data
            cmbCompareType.SelectedIndex = 0;
            stuSearchTemp.StrCompareType = cmbCompareType.SelectedValue;
            chkGeneral.Checked = true;
            stuSearchTemp.GeneralMode = chkGeneral.Checked;
            chkPeriod.Checked = false;
            stuSearchTemp.PeriodMode = chkPeriod.Checked;
            chkshowGraphic.Checked = false;
            if (chkshowGraphic.Checked) { stuSearchTemp.ShowGraphic = ShowGraphic.Visible; } else { stuSearchTemp.ShowGraphic = ShowGraphic.Hide; }
            chkShowProcess.Checked = false;
            if (chkShowProcess.Checked) { stuSearchTemp.ShowProcess = ShowProcess.Visible; } else { stuSearchTemp.ShowProcess = ShowProcess.Hide; }
            chkShowStaticHtml.Checked = false;
            stuSearchTemp.ShowStaticHtml = chkShowStaticHtml.Checked;
            chkRecalc.Checked = false;
            stuSearchTemp.Recalculate = chkRecalc.Checked;
            txtTestPeriods.Text = Properties.Resources.defaultTestPeriodsValue;
            stuSearchTemp.InTestPeriods = int.Parse(txtTestPeriods.Text, InvariantCulture);
            #endregion base data

            #region FieldMode
            chkField.Checked = false;
            cmbCompare01.SelectedIndex = 0;
            cmbCompare02.SelectedIndex = 0;
            cmbCompare03.SelectedIndex = 0;
            cmbCompare04.SelectedIndex = 0;
            cmbCompare05.SelectedIndex = 0;
            stuSearchTemp.FieldMode = chkField.Checked;
            stuSearchTemp.StrCompares = "gen";
            stuSearchTemp.StrComparesDetail = "none";
            #endregion FieldMode

            #region NextMode
            chkNextNums.Checked = false;
            cmbNextNums.SelectedIndex = 0;
            cmbNextStep.SelectedIndex = 0;
            stuSearchTemp.NextNumsMode = chkNextNums.Checked;
            stuSearchTemp.InNextNums = int.Parse(Properties.Resources.defaultNextNums, InvariantCulture);
            stuSearchTemp.InNextStep = int.Parse(Properties.Resources.defaultNextStep, InvariantCulture);
            stuSearchTemp.StrNextNumT = "none";
            stuSearchTemp.StrNextNums = "none";
            stuSearchTemp.StrNextNumSpe = "none";
            #endregion NextMode

            #region Search Range
            txtDataLimit.Text = Properties.Resources.defaultDataLimitValue;
            txtDataOffset.Text = Properties.Resources.defaultDataOffsetValue;
            txtSearchLimit.Text = Properties.Resources.defaultSearchLimitValue;
            txtSearchOffset.Text = Properties.Resources.defaultSearchOffsetValue;
            stuSearchTemp.InDataLimit = int.Parse(txtDataLimit.Text, InvariantCulture);
            stuSearchTemp.InDataOffset = int.Parse(txtDataOffset.Text, InvariantCulture);
            stuSearchTemp.InSearchLimit = int.Parse(txtSearchLimit.Text, InvariantCulture);
            stuSearchTemp.InSearchOffset = int.Parse(txtSearchOffset.Text, InvariantCulture);
            #endregion Search Range

            cmdFieldPeriodLimit.SelectedIndex = 14;
            stuSearchTemp.InFieldPeriodLimit = int.Parse(cmdFieldPeriodLimit.SelectedValue, InvariantCulture);

            chkSearchOrder.Checked = false;
            stuSearchTemp.SearchOrder = chkSearchOrder.Checked;

            chkSearchFields.Checked = false;
            stuSearchTemp.SearchFileds = chkSearchFields.Checked;

            #region Display
            txtDisplayPeriod.Text = Properties.Resources.defaultDisplayPeriodValue;
            stuSearchTemp.InDisplayPeriod = int.Parse(txtDisplayPeriod.Text, InvariantCulture);
            #endregion Display

            #region Filter
            txtHistorySNs.Text = string.Empty;
            stuSearchTemp.FreqFilterSNs = string.IsNullOrEmpty(txtHistorySNs.Text) ? "none" : txtHistorySNs.Text;
            txtHistoryPeriods.Text = Properties.Resources.defaultHistoryPeriodsValue;
            stuSearchTemp.InHistoryPeriods = int.Parse(txtHistoryPeriods.Text, InvariantCulture);
            txtHistoryTestPeriods.Text = Properties.Resources.defaultHistoryTestPeriodsValue;
            stuSearchTemp.HistoryTestPeriods = float.Parse(txtHistoryTestPeriods.Text, InvariantCulture);
            txtHitMin.Text = Properties.Resources.defaultHitMinValue;
            stuSearchTemp.InHitMin = int.Parse(txtHitMin.Text, InvariantCulture);
            chkFilterRange.Checked = false;
            stuSearchTemp.FilterRange = chkFilterRange.Checked;
            txtFilterRange.Text = string.Empty;
            stuSearchTemp.StrFilterRange = string.IsNullOrEmpty(txtFilterRange.Text) ? "none" : txtFilterRange.Text;
            txtFilterMin.Text = Properties.Resources.defaultFilterMinValue;
            stuSearchTemp.SglFilterMin = float.Parse(txtFilterMin.Text, InvariantCulture);
            txtFilterMax.Text = Properties.Resources.defaultFilterMaxValue;
            stuSearchTemp.SglFilterMax = float.Parse(txtFilterMax.Text, InvariantCulture);
            #endregion Filter

            #region 區間欄位
            cmbFCheck01.SelectedIndex = 0;
            cmbFCheck02.SelectedIndex = 0;
            cmbFCheck03.SelectedIndex = 0;
            cmbFCheck04.SelectedIndex = 0;
            cmbFCheck05.SelectedIndex = 0;
            cmbFCheck06.SelectedIndex = 0;
            stuSearchTemp.StrSecField = "none";
            #endregion 區間欄位

            #region 百分比
            txtDelete.Text = string.Empty;
            stuSearchTemp.StrDeletes = txtDelete.Text;

            txtNotDelete.Text = string.Empty;
            stuSearchTemp.StrNotDeletes = txtNotDelete.Text;

            txtDataRowsLimit.Text = Properties.Resources.defaultDataRowsLimitValue;
            stuSearchTemp.InDataRowsLimit = int.Parse(txtDataRowsLimit.Text, InvariantCulture);
            #endregion 百分比

            stuSearchTemp.StrSmartTests = string.Empty;

            #region 數字DNA
            txtFreqDNALength.Text = Properties.Resources.defaultFreqDNALength;
            stuSearchTemp.InFreqDnaLength = int.Parse(txtFreqDNALength.Text, InvariantCulture);
            txtTargetTestPeriods.Text = Properties.Resources.defaultTargetTestPeriods;
            stuSearchTemp.InTargetTestPeriods = int.Parse(txtTargetTestPeriods.Text, InvariantCulture);
            #endregion 數字DNA

            Session["SearchOption"] = stuSearchTemp;
        }

        // ---------------------------------------------------------------------------------------------------------
        #region SetupOptions
        private void SetupOptions()
        {
            if (DicOptions == null)
            {
                DicOptions = new Dictionary<string, object>();
                DicOptions = AddOptShow(DicOptions);
                DicOptions = AddOptDataRange(DicOptions);
                DicOptions = AddOptCompare(DicOptions);
                DicOptions = AddOptNextNum(DicOptions);
                DicOptions = AddOptFieldCheck(DicOptions);
                DicOptions = AddOptSearch(DicOptions);
                DicOptions = AddOptTP(DicOptions);
                DicOptions = AddOptSmartRun(DicOptions);
                DicOptions = AddOptSmartTest(DicOptions);
                DicOptions = AddOptFreqDna(DicOptions);
                //ViewState["Options"] = DicOptions;
            }
            //DicOptions = (Dictionary<string, object>)ViewState["Options"];
        }

        private static Dictionary<string, object> AddOptFreqDna(Dictionary<string, object> dicOptions)
        {
            //dicOptions.Add("dtlFreqDNA", dtlFreqDNA);
            //dicOptions.Add("phFreqDNALength", phFreqDNALength);
            //dicOptions.Add("phTargetTestPeriods", phTargetTestPeriods);
            return dicOptions;
        }

        private Dictionary<string, object> AddOptSmartTest(Dictionary<string, object> dicOptions)
        {
            //dicOptions.Add("dtlSmartTest", dtlSmartTest);
            dicOptions.Add("phRndNums", phRndNums);
            dicOptions.Add("phSmartTestNum", phSmartTestNum);
            return dicOptions;
        }

        private Dictionary<string, object> AddOptSmartRun(Dictionary<string, object> dicOptions)
        {
            //dicOptions.Add("dtlSmartRun", dtlSmartRun);
            dicOptions.Add("phOdds", phOdds);
            dicOptions.Add("phHigh", phHigh);
            dicOptions.Add("phHot", phHot);
            dicOptions.Add("phSumMin", phSumMin);
            dicOptions.Add("phSumMax", phSumMax);
            dicOptions.Add("phTestNum", phTestNum);
            return dicOptions;
        }

        private static Dictionary<string, object> AddOptTP(Dictionary<string, object> dicOptions)
        {
            //dicOptions.Add("dtlTP", dtlTP);
            return dicOptions;
        }

        private Dictionary<string, object> AddOptSearch(Dictionary<string, object> dicOptions)
        {
            //dicOptions.Add("dtlSearch", dtlSearch);
            //dicOptions.Add("phHistorySNs", phHistorySNs);
            dicOptions.Add("phHistoryPeriods", phHistoryPeriods);
            dicOptions.Add("phHistoryTestPeriods", phHistoryTestPeriods);

            //dicOptions.Add("phHitMin", phHitMin);
            //dicOptions.Add("chkFilterRange", chkFilterRange);
            //dicOptions.Add("phFilterRange", phFilterRange);
            return dicOptions;
        }

        private Dictionary<string, object> AddOptShow(Dictionary<string, object> dicOptions)
        {
            //dicOptions.Add("chkSearchFields", chkSearchFields);
            //dicOptions.Add("chkShowStaticHtml", chkShowStaticHtml);
            dicOptions.Add("phCriticalNum", phCriticalNum);
            dicOptions.Add("phTestPeriods", phTestPeriods);
            dicOptions.Add("phDisplayPeriod", phDisplayPeriod);
            return dicOptions;
        }

        private static Dictionary<string, object> AddOptFieldCheck(Dictionary<string, object> dicOptions)
        {
            //dicOptions.Add("dtlFCheck", dtlFCheck);
            //dicOptions.Add("phFCheck01", phFCheck01);
            //dicOptions.Add("phFCheck02", phFCheck02);
            //dicOptions.Add("phFCheck03", phFCheck03);
            //dicOptions.Add("phFCheck04", phFCheck04);
            //dicOptions.Add("phFCheck05", phFCheck05);
            //dicOptions.Add("phFCheck06", phFCheck06);
            return dicOptions;
        }

        private static Dictionary<string, object> AddOptCompare(Dictionary<string, object> dicOptions)
        {
            //dicOptions.Add("dtlCompare", dtlCompare);
            //dicOptions.Add("phCompare01", phCompare01);
            //dicOptions.Add("phCompare02", phCompare02);
            //dicOptions.Add("phCompare03", phCompare03);
            //dicOptions.Add("phCompare04", phCompare04);
            //dicOptions.Add("phCompare05", phCompare05);
            return dicOptions;
        }

        private static Dictionary<string, object> AddOptNextNum(Dictionary<string, object> dicOptions)
        {
            //dicOptions.Add("dtlNextNum", dtlNextNum);
            //dicOptions.Add("phNextNums", phNextNums);
            //dicOptions.Add("phNextStep", phNextStep);
            return dicOptions;
        }

        private Dictionary<string, object> AddOptDataRange(Dictionary<string, object> dicOptions)
        {
            //dicOptions.Add("dtlDataRange", dtlDataRange);
            dicOptions.Add("phDataLimit", phDataLimit);
            dicOptions.Add("phSearchLimit", phSearchLimit);
            dicOptions.Add("phFieldPeiodLimit", phFieldPeiodLimit);
            return dicOptions;
        }

        #endregion SetupOptions
        // ---------------------------------------------------------------------------------------------------------
        #region SetupButtons

        private void SetupButtons()
        {
            if (DicButtons == null)
            {
                DicButtons = new Dictionary<string, object>();
                DicButtons = AddBtnFreq(DicButtons);
                DicButtons = AddBtnFreqActive(DicButtons);
                DicButtons = AddBtnFreqActiveH(DicButtons);
                DicButtons = AddBtnMissingTotal(DicButtons);
                DicButtons = AddBtnFreqDna(DicButtons);
                DicButtons = AddBtnDataN(DicButtons);
                DicButtons = AddBtnSum(DicButtons);
                DicButtons = AddBtnTablePercent(DicButtons);
                DicButtons = AddBtnTest(DicButtons);
                //ViewState["SearchButtons"] = DicButtons;
            }
            //DicButtons = (Dictionary<string, object>)ViewState["SearchButtons"];
        }

        private Dictionary<string, object> AddBtnTest(Dictionary<string, object> dicButtons)
        {
            dicButtons.Add("cSmart00", cmdSmart);
            return dicButtons;
        }

        private Dictionary<string, object> AddBtnTablePercent(Dictionary<string, object> dicButtons)
        {
            dicButtons.Add("cTablePercent00", cmdTablePercent);
            return dicButtons;
        }

        private Dictionary<string, object> AddBtnSum(Dictionary<string, object> dicButtons)
        {
            dicButtons.Add("cTableSum00", cmdTableSum);
            return dicButtons;
        }

        private Dictionary<string, object> AddBtnDataN(Dictionary<string, object> dicButtons)
        {
            dicButtons.Add("cTableDataN00", cmdTableDataN);
            return dicButtons;
        }

        private Dictionary<string, object> AddBtnMissingTotal(Dictionary<string, object> dicButtons)
        {
            dicButtons.Add("cTableMissingTotal00", cmdTableMissingTotal);
            return dicButtons;
        }

        private Dictionary<string, object> AddBtnFreqDna(Dictionary<string, object> dicButtons)
        {
            dicButtons.Add("cFreqDNA00", cmdFreqDNA);
            return dicButtons;
        }

        private Dictionary<string, object> AddBtnFreqActiveH(Dictionary<string, object> dicButtons)
        {
            dicButtons.Add("cFreqActiveH00", cmdFreqActiveH);
            dicButtons.Add("cFreqActiveHT01Shortcut", btnShortcut);
            return dicButtons;
        }

        private Dictionary<string, object> AddBtnFreqActive(Dictionary<string, object> dicButtons)
        {
            dicButtons.Add("cFreqActive00", cmdFreqActive);
            return dicButtons;
        }

        private Dictionary<string, object> AddBtnFreq(Dictionary<string, object> dicButtons)
        {
            dicButtons.Add("cFreq00", cmdFreq);
            return dicButtons;
        }

        #endregion SetupButtons
        // ---------------------------------------------------------------------------------------------------------

        private void TurnOnButtons(string option)
        {
            foreach (KeyValuePair<string, object> button in DicButtons)
            {
                switch (button.Value.GetType().ToString())
                {
                    case "System.Web.UI.WebControls.DropDownList":
                        ((DropDownList)button.Value).Visible = false;
                        break;
                    case "System.Web.UI.WebControls.Button":
                        ((Button)button.Value).Visible = false;
                        break;
                }
            }
            TurnOffOptios(null);
            txtSearchLimit.Text = Properties.Resources.defaultSearchLimitValue;
            //SearchOption_init();
            //頻率
            List<string> lstFreq = new List<string> { "cFreq00", "cFreq", "cFreqT", "cFreqSet", "cCacul01" };
            if (lstFreq.Contains(option)) { ShowFreq(option); }
            //活性表
            lstFreq = new List<string> { "cFreqActive00", "cFreqActive", "cFreqActive01", "cFreqActive01T", "cFreqActive01TR", "cFreqActive02", "cFreqActive03", "cFreqActiveChart", "cFreqActiveChart01" };
            if (lstFreq.Contains(option)) { ShowFreqActive(option); }
            //活性歷史表
            lstFreq = new List<string> { "cFreqActiveH00", "cFreqActiveH", "cFreqActiveHT", "cFreqActiveHT01", "cFreqActiveHT01P", "cFreqActiveHT02", "cFreqActiveHTAll" };
            if (lstFreq.Contains(option)) { ShowFreqActiveH(option); }
            //遺漏表
            lstFreq = new List<string> { "cTableMissingTotal00", "cTableMissingTotal", "cTableMissingTotalT", "cTableMissingTotalT01" };
            if (lstFreq.Contains(option)) { ShowMissTotal(option); }
            //數字DNA
            lstFreq = new List<string> { "cFreqDNA00", "cFreqDNA", "cFreqDNAT", "cFreqSecField", "cFreqSecFieldT" };
            if (lstFreq.Contains(option)) { ShowFreqDna(option); }
            //平衡振盪表
            lstFreq = new List<string> { "cTableDataN00", "cTableDataN", "cTableDataNChart", "cTableDataB", "cTableDataNBT01" };
            if (lstFreq.Contains(option)) { ShowDataN(option); }
            //和數表
            lstFreq = new List<string> { "cTableSum00", "cTableSum" };
            if (lstFreq.Contains(option)) { ShowSum(option); }
            //奇-偶數表
            lstFreq = new List<string> { "cTableOddEven" };
            if (lstFreq.Contains(option)) { ShowOddEven(option); }
            //百分表
            lstFreq = new List<string> { "cTablePercent00", "cTablePercent", "cTablePercent01", "cTablePercent02" };
            if (lstFreq.Contains(option)) { ShowPercent(option); }
            //組合
            lstFreq = new List<string> { "cSmart00", "cSmartRun", "cSmartTest" };
            if (lstFreq.Contains(option)) { ShowSmart(option); }

            UpdateSearchOptions();
        }

        private void ShowSmart(string option)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            switch (option)
            {
                case "cSmartTest":
                    TurnOnButtons(cmdSmart);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    dtlSmartTest.Visible = true;
                    TurnOnOptios(phRndNums);
                    TurnOnOptios(phSmartTestNum);
                    TurnOffOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsSmartTest;
                    stuGLSearch.PageFileName = Properties.Resources.PageSmartTest;
                    btnRun.Text = Properties.Resources.LabelSmartTest;
                    break;
                default:
                    cmdSmart.SelectedIndex = 0;
                    TurnOnButtons(cmdSmart);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    dtlSmartRun.Visible = true;
                    TurnOnOptios(phOdds);
                    TurnOnOptios(phHigh);
                    TurnOnOptios(phHot);
                    TurnOnOptios(phSumMin);
                    TurnOnOptios(phSumMax);
                    TurnOnOptios(phTestNum);
                    TurnOffOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsSmartRun;
                    stuGLSearch.PageFileName = Properties.Resources.PageSmartRun;
                    btnRun.Text = Properties.Resources.LabelSmartRun;
                    break;
            }
            Session["SearchOption"] = stuGLSearch;
        }

        private void ShowPercent(string option)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            switch (option)
            {
                #region 百分表
                case "cTablePercent01":
                    TurnOnButtons(cmdTablePercent);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    dtlTP.Visible = true;
                    TurnOnOptios(phDelete);
                    TurnOnOptios(phNotDelete);
                    TurnOnOptios(phDataRowsLimit);
                    TurnOnOptios(phCriticalNum);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsTablePercent01;
                    stuGLSearch.PageFileName = Properties.Resources.PageTablePercent01;
                    btnRun.Text = Properties.Resources.LabelTablePercent01;
                    break;

                case "cTablePercent02":
                    TurnOnButtons(cmdTablePercent);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    dtlTP.Visible = true;
                    TurnOnOptios(phDataRowsLimit);
                    TurnOnOptios(phCriticalNum);
                    dtlDataRange.Visible = true;
                    TurnOnOptios(phDataLimit);
                    TurnOnOptios(phSearchLimit);
                    txtSearchLimit.Text = txtSearchLimit.Text == "0" ? Properties.Resources.defaultSearchLimitValue01 : txtSearchLimit.Text;
                    //TurnOnOptios("dtlSearch");
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsTablePercent02;
                    stuGLSearch.PageFileName = Properties.Resources.PageTablePercent02;
                    btnRun.Text = Properties.Resources.LabelTablePercent02;
                    break;

                default:
                    TurnOnButtons(cmdTablePercent);
                    cmdTablePercent.SelectedIndex = 0;
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    dtlTP.Visible = true;
                    TurnOnOptios(phDataRowsLimit);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsTablePercent;
                    stuGLSearch.PageFileName = Properties.Resources.PageTablePercent;
                    btnRun.Text = Properties.Resources.LabelTablePercent;
                    break;
                    #endregion 百分表
            }
            Session["SearchOption"] = stuGLSearch;
        }

        private void ShowOddEven(string option)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            switch (option)
            {
                #region 奇-偶數表
                default:
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    dtlDataRange.Visible = true;
                    TurnOnOptios(phDataLimit);
                    TurnOnOptios(phSearchLimit);
                    dtlSearch.Visible = true;
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsOddEven;
                    stuGLSearch.PageFileName = Properties.Resources.PageOddEven;
                    btnRun.Text = Properties.Resources.LabelOddEven;
                    break;
                    #endregion 奇-偶數表
            }
            Session["SearchOption"] = stuGLSearch;
        }

        private void ShowSum(string option)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            switch (option)
            {
                #region 和數表
                default:
                    TurnOnButtons(cmdTableSum);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    dtlDataRange.Visible = true;
                    TurnOnOptios(phDataLimit);
                    TurnOnOptios(phSearchLimit);
                    dtlSearch.Visible = true;
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsSum;
                    stuGLSearch.PageFileName = Properties.Resources.PageSum;
                    btnRun.Text = Properties.Resources.LabelSum;
                    break;
                    #endregion 和數表
            }
            Session["SearchOption"] = stuGLSearch;
        }

        private void ShowDataN(string option)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            switch (option)
            {
                case "cTableDataNBT01":
                    TurnOnButtons(cmdTableDataN);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    TurnOnOptios(phFieldPeiodLimit);
                    stuGLSearch.Action = Properties.Resources.SessionsDataNBT01;
                    stuGLSearch.PageFileName = Properties.Resources.PageDataNBT01;
                    btnRun.Text = Properties.Resources.LabelDataNBT01;
                    break;

                case "cTableDataB":
                    TurnOnButtons(cmdTableDataN);
                    TurnOnButtons(btnShortcut);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    TurnOnOptios(chkSearchFields);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    TurnOnOptios(phFieldPeiodLimit);
                    stuGLSearch.Action = Properties.Resources.SessionsDataB;
                    stuGLSearch.PageFileName = Properties.Resources.PageDataB;
                    btnRun.Text = Properties.Resources.LabelDataB;
                    break;

                case "cTableDataNChart":
                    TurnOnButtons(cmdTableDataN);
                    cmdTableDataN.SelectedIndex = 0;
                    TurnOnButtons(btnShortcut);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    TurnOnOptios(chkSearchFields);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    TurnOnOptios(phFieldPeiodLimit);
                    stuGLSearch.Action = Properties.Resources.SessionsDataNChart;
                    stuGLSearch.PageFileName = Properties.Resources.PageDataNChart;
                    btnRun.Text = Properties.Resources.LabelDataNChart;
                    break;

                default:
                    TurnOnButtons(cmdTableDataN);
                    cmdTableDataN.SelectedIndex = 0;
                    TurnOnButtons(btnShortcut);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    TurnOnOptios(chkSearchFields);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    TurnOnOptios(phFieldPeiodLimit);
                    stuGLSearch.Action = Properties.Resources.SessionsDataN;
                    stuGLSearch.PageFileName = Properties.Resources.PageDataN;
                    btnRun.Text = Properties.Resources.LabelDataN;
                    break;
            }
            Session["SearchOption"] = stuGLSearch;
        }

        private void ShowFreqDna(string option)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            switch (option)
            {
                case "cFreqDNAT":
                    TurnOnButtons(cmdFreqDNA);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    dtlFreqDNA.Visible = true;
                    TurnOffOptios(phFreqDNALength);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqDNAT;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqDNAT;
                    btnRun.Text = Properties.Resources.LabelFreqDNAT;
                    break;
                case "cFreqSecField":
                    TurnOnButtons(cmdFreqDNA);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    TurnOnOptios(chkSearchFields);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    dtlFreqDNA.Visible = true;
                    dtlFCheck.Visible = true;
                    TurnOffOptios(phFreqDNALength);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqSecField;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqSecField;
                    btnRun.Text = Properties.Resources.LabelFreqSecField;
                    break;
                case "cFreqSecFieldT":
                    TurnOnButtons(cmdFreqDNA);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    dtlFreqDNA.Visible = true;
                    dtlFCheck.Visible = true;
                    TurnOffOptios(phFreqDNALength);
                    TurnOnOptios(phFieldPeiodLimit);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqSecFieldT;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqSecFieldT;
                    btnRun.Text = Properties.Resources.LabelFreqSecFieldT;
                    break;

                default:
                    cmdFreqDNA.SelectedIndex = 0;
                    TurnOnButtons(cmdFreqDNA);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    TurnOnOptios(chkSearchFields);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    dtlFreqDNA.Visible = true;
                    TurnOnOptios(phFreqDNALength);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqDNA;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqDNA;
                    btnRun.Text = Properties.Resources.LabelFreqDNA;
                    break;
            }
            Session["SearchOption"] = stuGLSearch;
        }

        private void ShowMissTotal(string option)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            switch (option)
            {
                case "cTableMissingTotalT":
                    TurnOnButtons(cmdTableMissingTotal);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    TurnOnOptios(phFieldPeiodLimit);

                    stuGLSearch.Action = Properties.Resources.SessionsMissingTotalT;
                    stuGLSearch.PageFileName = Properties.Resources.PageMissingTotalT;
                    btnRun.Text = Properties.Resources.LabelMissingTotalT;
                    break;
                case "cTableMissingTotalT01":
                    TurnOnButtons(cmdTableMissingTotal);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    //TurnOnOptios(phFieldPeiodLimit);

                    stuGLSearch.Action = Properties.Resources.SessionsMissingTotalT01;
                    stuGLSearch.PageFileName = Properties.Resources.PageMissingTotalT01;
                    btnRun.Text = Properties.Resources.LabelMissingTotalT01;
                    break;
                default:
                    cmdTableMissingTotal.SelectedIndex = 0;
                    TurnOnButtons(cmdTableMissingTotal);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    TurnOnOptios(chkSearchFields);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsMissingTotal;
                    stuGLSearch.PageFileName = Properties.Resources.PageMissingTotal;
                    btnRun.Text = Properties.Resources.LabelMissingTotal;
                    break;
            }
            Session["SearchOption"] = stuGLSearch;
        }

        private void ShowFreqActiveH(string option)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            switch (option)
            {
                case "cFreqActiveHT":
                    TurnOnButtons(cmdFreqActiveH);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    TurnOffOptios(phHistorySNs);
                    dtlSearch.Visible = true;
                    TurnOnOptios(phHistoryPeriods);
                    TurnOnOptios(phHistoryTestPeriods);
                    TurnOnOptios(phTestPeriods);

                    TurnOnOptios(chkFilterRange);
                    TurnOffOptios(phFilterRange);

                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActiveHT;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActiveHT;
                    btnRun.Text = Properties.Resources.LabelFreqActiveHT;
                    break;

                case "cFreqActiveHT01":
                    TurnOnButtons(cmdFreqActiveH);
                    TurnOnButtons(btnShortcut);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    TurnOffOptios(phHistorySNs);

                    dtlSearch.Visible = true;
                    TurnOnOptios(phHistoryPeriods);
                    TurnOnOptios(phHistoryTestPeriods);
                    TurnOnOptios(phTestPeriods);

                    TurnOnOptios(chkFilterRange);
                    TurnOffOptios(phFilterRange);

                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActiveHT01;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActiveHT01;
                    btnRun.Text = Properties.Resources.LabelFreqActiveHT01;
                    break;

                case "cFreqActiveHT01P":
                    TurnOnButtons(cmdFreqActiveH);
                    TurnOnButtons(btnShortcut);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    TurnOffOptios(phHistorySNs);

                    dtlSearch.Visible = true;
                    TurnOnOptios(phHistoryPeriods);
                    TurnOnOptios(phHistoryTestPeriods);
                    TurnOnOptios(phTestPeriods);

                    TurnOnOptios(chkFilterRange);
                    TurnOffOptios(phFilterRange);

                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActiveHT01P;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActiveHT01P;
                    btnRun.Text = Properties.Resources.LabelFreqActiveHT01P;
                    break;

                case "cFreqActiveHT02":
                    TurnOnButtons(cmdFreqActiveH);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    dtlSearch.Visible = true;
                    TurnOnOptios(phHistoryPeriods);
                    TurnOnOptios(phHistoryTestPeriods);
                    TurnOnOptios(phTestPeriods);

                    TurnOffOptios(phHistorySNs);
                    TurnOffOptios(chkFilterRange);
                    TurnOffOptios(phFilterRange);

                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActiveHT02;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActiveHT02;
                    btnRun.Text = Properties.Resources.LabelFreqActiveHT02;

                    break;

                case "cFreqActiveHTAll":
                    TurnOnButtons(cmdFreqActiveH);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    dtlSearch.Visible = true;
                    TurnOnOptios(phHistoryPeriods);
                    TurnOnOptios(phHistoryTestPeriods);
                    TurnOnOptios(phTestPeriods);

                    TurnOffOptios(phHistorySNs);
                    TurnOffOptios(chkFilterRange);
                    TurnOffOptios(phFilterRange);

                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActiveHTAll;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActiveHTAll;
                    btnRun.Text = Properties.Resources.LabelFreqActiveHTAll;
                    break;

                default:
                    cmdFreqActiveH.SelectedIndex = 0;
                    TurnOnButtons(cmdFreqActiveH);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    dtlSearch.Visible = true;
                    dtlFCheck.Visible = true;
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phHistoryPeriods);
                    TurnOnOptios(phHistoryTestPeriods);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActiveH;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActiveH;
                    btnRun.Text = Properties.Resources.LabelFreqActiveH;
                    break;
            }
            Session["SearchOption"] = stuGLSearch;
        }

        private void ShowFreqActive(string option)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            switch (option)
            {
                case "cFreqActive01":
                    TurnOnButtons(cmdFreqActive);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    TurnOnOptios(chkSearchFields);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActive01;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActive01;
                    btnRun.Text = Properties.Resources.LabelFreqActive01;
                    break;

                case "cFreqActive01T":
                    TurnOnButtons(cmdFreqActive);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    TurnOnOptios(phFieldPeiodLimit);
                    dtlSearch.Visible = true;
                    TurnOnOptios(phHistoryPeriods);
                    txtHistoryPeriods.Text = cmdFieldPeriodLimit.SelectedValue;
                    stuGLSearch.InHistoryPeriods = stuGLSearch.InFieldPeriodLimit;
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActive01T;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActive01T;
                    btnRun.Text = Properties.Resources.LabelFreqActive01T;

                    break;

                case "cFreqActive01TR":
                    TurnOnButtons(cmdFreqActive);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    TurnOnOptios(phFieldPeiodLimit);
                    dtlSearch.Visible = true;
                    TurnOnOptios(phHistoryPeriods);
                    txtHistoryPeriods.Text = cmdFieldPeriodLimit.SelectedValue;
                    stuGLSearch.InHistoryPeriods = stuGLSearch.InFieldPeriodLimit;
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActive01TR;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActive01TR;
                    btnRun.Text = Properties.Resources.LabelFreqActive01TR;

                    break;
                case "cFreqActive02":
                    TurnOnButtons(cmdFreqActive);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActive02;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActive02;
                    btnRun.Text = Properties.Resources.LabelFreqActive02;
                    break;

                case "cFreqActive03":
                    TurnOnButtons(cmdFreqActive);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActive03;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActive03;
                    btnRun.Text = Properties.Resources.LabelFreqActive03;
                    break;

                case "cFreqActiveChart":
                    TurnOnButtons(cmdFreqActive);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    dtlDataRange.Visible = true;
                    TurnOnOptios(phDataLimit);
                    TurnOnOptios(phSearchLimit);
                    txtSearchLimit.Text = txtSearchLimit.Text == "0" ? Properties.Resources.defaultSearchLimitValue01 : txtSearchLimit.Text;
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(phCriticalNum);
                    TurnOffOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActiveChart;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActiveChart;
                    btnRun.Text = Properties.Resources.LabelFreqActiveChart;
                    break;

                case "cFreqActiveChart01":
                    TurnOnButtons(cmdFreqActive);
                    TurnOnOptios(chkField);
                    //TurnOnOptios(chkNextNums);
                    dtlDataRange.Visible = false;
                    //TurnOnOptios(phDataLimit);
                    //TurnOnOptios(phSearchLimit);
                    //txtSearchLimit.Text = txtSearchLimit.Text == "0" ? Properties.Resources.defaultSearchLimitValue01 : txtSearchLimit.Text;
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(phFieldPeiodLimit);
                    //TurnOnOptios(phCriticalNum);
                    TurnOffOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActiveChart01;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActiveChart01;
                    btnRun.Text = Properties.Resources.LabelFreqActiveChart01;
                    break;
                default:
                    cmdFreqActive.SelectedIndex = 0;
                    TurnOnButtons(cmdFreqActive);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    dtlDataRange.Visible = true;
                    TurnOnOptios(phDataLimit);
                    TurnOnOptios(phSearchLimit);
                    txtSearchLimit.Text = txtSearchLimit.Text == "0" ? Properties.Resources.defaultSearchLimitValue01 : txtSearchLimit.Text;
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(phCriticalNum);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqActive;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqActive;
                    btnRun.Text = Properties.Resources.LabelFreqActive;
                    break;
            }
            Session["SearchOption"] = stuGLSearch;
        }

        private void ShowFreq(string option)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            switch (option)
            {
                case "cFreqT":
                    TurnOnButtons(cmdFreq);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    TurnOffOptios(chkField);
                    TurnOffOptios(chkNextNums);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqResultT;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqResultT;
                    btnRun.Text = Properties.Resources.LabelFreqResultT;
                    break;

                case "cFreqSet":
                    TurnOnButtons(cmdFreq);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    dtlDataRange.Visible = true;
                    TurnOnOptios(phDataLimit);
                    TurnOnOptios(phSearchLimit);
                    dtlSearch.Visible = true;
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqSet;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqSet;
                    btnRun.Text = Properties.Resources.LabelFreqSet;
                    break;

                case "cCacul01":
                    TurnOnButtons(cmdFreq);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsCacul01;
                    stuGLSearch.PageFileName = Properties.Resources.PageCacul01;
                    btnRun.Text = Properties.Resources.LabelCacul01;
                    break;

                default:
                    cmdFreq.SelectedIndex = 0;
                    TurnOnButtons(cmdFreq);
                    TurnOnOptios(chkField);
                    TurnOnOptios(chkNextNums);
                    dtlDataRange.Visible = true;
                    TurnOnOptios(phDataLimit);
                    TurnOnOptios(phSearchLimit);
                    dtlSearch.Visible = true;
                    TurnOnOptios(chkSearchFields);
                    TurnOnOptios(phHistoryTestPeriods);
                    TurnOnOptios(phDisplayPeriod);
                    TurnOnOptios(phTestPeriods);
                    TurnOnOptios(chkShowStaticHtml);
                    stuGLSearch.Action = Properties.Resources.SessionsFreqResult;
                    stuGLSearch.PageFileName = Properties.Resources.PageFreqResult;
                    btnRun.Text = Properties.Resources.LabelFreqResult;
                    break;
            }
            Session["SearchOption"] = stuGLSearch;
        }

        // ---------------------------------------------------------------------------------------------------------

        private void TurnOnButtons(object option)
        {
            CheckData = true;
            switch (option.GetType().ToString())
            {
                case "System.Web.UI.WebControls.DropDownList":
                    ((DropDownList)option).Visible = true;
                    break;
                case "System.Web.UI.WebControls.Button":
                    ((Button)option).Visible = true;
                    break;
            }
        }
        // ---------------------------------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void TurnOffOptios(object option)
        {
            if (option is null)
            {
                foreach (KeyValuePair<string, object> optionitem in DicOptions)
                {
                    if (optionitem.Key.Substring(0, 3) != "dtl")
                    {
                        switch (optionitem.Value.GetType().ToString())
                        {
                            case "System.Web.UI.WebControls.PlaceHolder":
                                ((PlaceHolder)optionitem.Value).Visible = false;
                                break;
                            case "System.Web.UI.WebControls.CheckBox":
                                ((CheckBox)optionitem.Value).Visible = false;
                                break;
                            case "System.Web.UI.WebControls.DropDownList":
                                ((DropDownList)optionitem.Value).Visible = false;
                                break;

                        }
                    }
                }
                dtlCompare.Visible = false;
                dtlDataRange.Visible = false;
                dtlNextNum.Visible = false;
                dtlFCheck.Visible = false;
                dtlSearch.Visible = false;
                dtlTP.Visible = false;
                dtlSmartRun.Visible = false;
                dtlSmartTest.Visible = false;
                dtlFreqDNA.Visible = false;
            }
            else
            {
                switch (option.GetType().ToString())
                {
                    case "System.Web.UI.WebControls.CheckBox":
                        CheckBox option1 = ((CheckBox)option);
                        option1.Visible = false;
                        option1.Checked = false;
                        break;
                    case " System.Web.UI.HtmlControls.HtmlGenericControl":
                        ((HtmlGenericControl)option).Visible = false;
                        break;
                    case "System.Web.UI.WebControls.PlaceHolder":
                        ((PlaceHolder)option).Visible = false;
                        break;
                    case "System.Web.UI.WebControls.DropDownList":
                        ((DropDownList)option).Visible = false;
                        break;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void TurnOnOptios(object option)
        {
            switch (option.GetType().ToString())
            {
                case "System.Web.UI.WebControls.CheckBox":
                    ((CheckBox)option).Visible = true;
                    break;
                case " System.Web.UI.HtmlControls.HtmlGenericControl":
                    ((HtmlGenericControl)option).Visible = true;
                    break;
                case "System.Web.UI.WebControls.PlaceHolder":
                    ((PlaceHolder)option).Visible = true;
                    break;
                case "System.Web.UI.WebControls.DropDownList":
                    ((DropDownList)option).Visible = true;
                    break;
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        private void UpdateSearchOptions()
        {
            SetupDisplayChecked();
            SetupDataRange();
            SetupDisplayPeriod();
            SetupFields();
            SetupNextNum();
            SetupFCheck();
            SetupTablePercent();
            SetupFreqDNA();
            SetupSmartTestNum();
            SetupHistorySearch();
            SetupTestPeriods();
            ViewState["SearchOption"] = Session["SearchOption"];
        }


        // ---------------------------------------------------------------------------------------------------------

        #region 基本設置

        protected void BtnResetClick(object sender, EventArgs e)
        {
            SearchOption_init();
        }

        private void DataRangeStartInit(TargetTable lottoType)
        {
            StuGLSearch searchTemp = (StuGLSearch)Session["SearchOption"];
            using DataTable dtDataRangeEnd = GetDataRangeEndDT(lottoType);
            cmbDataRangeEnd.DataSource = dtDataRangeEnd.DefaultView;
            cmbDataRangeEnd.DataValueField = "lngTotalSN";
            cmbDataRangeEnd.DataTextField = "lngDateSN";
            cmbDataRangeEnd.SelectedIndex = 0;
            cmbDataRangeEnd.DataBind();
            searchTemp.LngDataStart = long.Parse(dtDataRangeEnd.Rows[dtDataRangeEnd.Rows.Count - 1]["lngTotalSN"].ToString(), InvariantCulture);
            searchTemp.LngDataEnd = long.Parse(dtDataRangeEnd.Rows[0]["lngTotalSN"].ToString(), InvariantCulture);
            searchTemp.LngTotalSN = long.Parse(dtDataRangeEnd.Rows[0]["lngTotalSN"].ToString(), InvariantCulture);

            #region  Set DropDownList ddlRndNums
            using DataTable dtRndNums = GetRndNumsDT(lottoType);
            dtRndNums.Locale = InvariantCulture;
            ddlRndNums.DataSource = dtRndNums.DefaultView;
            ddlRndNums.DataValueField = "CountNums";
            ddlRndNums.DataTextField = "CountNums";
            ddlRndNums.SelectedIndex = 0;
            ddlRndNums.DataBind();
            #endregion

            Session["SearchOption"] = searchTemp;
        }

        private DataTable GetRndNumsDT(TargetTable lottoType)
        {
            if (ViewState["RndNums" + lottoType.ToString()] == null)
            {
                using DataTable dtRndNums = new DataTable { Locale = InvariantCulture };
                if (dtRndNums.Columns.Count == 0)
                {
                    dtRndNums.Columns.Add(new DataColumn
                    {
                        DataType = typeof(short),
                        ColumnName = "CountNums"
                    });
                }
                for (int i = new CglDataSet(lottoType).CountNumber; i <= new CglDataSet(lottoType).LottoNumbers; i++)
                {
                    DataRow drRndNums = dtRndNums.NewRow();
                    drRndNums[0] = i;
                    dtRndNums.Rows.Add(drRndNums);
                }
                ViewState["RndNums" + lottoType.ToString()] = dtRndNums;
                return dtRndNums;
            }
            else
            {
                return (DataTable)ViewState["RndNums" + lottoType.ToString()];
            }
        }

        private DataTable GetDataRangeEndDT(TargetTable lottoType)
        {
            if (ViewState["DataRangeEnd" + lottoType.ToString()] == null)
            {
                string strlngTotalSN;
                #region get the first 0 in ingL1 as th last ingTotalSN
                using SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = new SqlConnection(new CglDBData().SetDataBase(lottoType, DatabaseType.Data)),
                    CommandText = "SELECT [lngTotalSN] , [lngDateSN] FROM [tblData] WHERE [lngL1] = 0 ORDER BY [lngTotalSN] ASC ;"
                };
                using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using DataTable dtImportFirst = new DataTable { Locale = InvariantCulture };
                sqlDataAdapter.Fill(dtImportFirst);
                strlngTotalSN = dtImportFirst.Rows[0]["lngTotalSN"].ToString();
                #endregion

                using SqlCommand sqlCommand01 = new SqlCommand
                {
                    Connection = new SqlConnection(new CglData().SetDataBase(lottoType, DatabaseType.Data)),
                    CommandText = "SELECT [lngTotalSN] , [lngDateSN] FROM [tblData] WHERE [lngTotalSN] <= @lngTotalSN ORDER BY [lngTotalSN] DESC ;"
                };
                sqlCommand01.Parameters.AddWithValue("lngTotalSN", strlngTotalSN);
                using SqlDataAdapter sqlDataAdapter01 = new SqlDataAdapter(sqlCommand01);
                using DataTable dtImport = new DataTable { Locale = InvariantCulture };
                sqlDataAdapter01.Fill(dtImport);
                ViewState["DataRangeEnd" + lottoType.ToString()] = dtImport;
                return dtImport;
            }
            else
            {
                return (DataTable)ViewState["DataRangeEnd" + lottoType.ToString()];
            }
        }

        protected void CmbDataRangeEndSelectedIndexChanged(object sender, EventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            stuGLSearch.LngDataEnd = long.Parse(cmbDataRangeEnd.SelectedValue, InvariantCulture);
            stuGLSearch.LngTotalSN = long.Parse(cmbDataRangeEnd.SelectedValue, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }

        protected void CmbCompareTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            SetupCompareType();

        }

        private void SetupCompareType()
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            stuGLSearch.StrCompareType = cmbCompareType.Visible ? cmbCompareType.SelectedValue : "AND";
            Session["SearchOption"] = stuGLSearch;
        }

        #endregion 基本設置

        #region 顯示選項
        protected void CmbLottoTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            switch (cmbLottoType.SelectedValue)
            {
                case "LottoBig":
                    stuGLSearch.LottoType = TargetTable.LottoBig;
                    break;
                case "Lotto539":
                    stuGLSearch.LottoType = TargetTable.Lotto539;
                    break;
                case "LottoWeli":
                    stuGLSearch.LottoType = TargetTable.LottoWeli;
                    break;
                case "LottoSix":
                    stuGLSearch.LottoType = TargetTable.LottoSix;
                    break;
            }
            Session["SearchOption"] = stuGLSearch;
            DataRangeStartInit(stuGLSearch.LottoType);
        }

        protected void DisplayCheckedChanged(object sender, EventArgs e)
        {
            SetupDisplayChecked();
        }

        private void SetupDisplayChecked()
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            stuGLSearch.ShowProcess = chkShowProcess.Visible && chkShowProcess.Checked ? ShowProcess.Visible : ShowProcess.Hide;
            stuGLSearch.ShowGraphic = chkshowGraphic.Visible && chkshowGraphic.Checked ? ShowGraphic.Visible : ShowGraphic.Hide;
            stuGLSearch.ShowStaticHtml = chkShowStaticHtml.Visible && chkShowStaticHtml.Checked;
            chkSearchOrder.Checked = chkSearchFields.Checked || chkSearchOrder.Checked;
            stuGLSearch.SearchFileds = chkSearchFields.Visible && chkSearchFields.Checked;
            stuGLSearch.SearchOrder = chkSearchOrder.Visible && chkSearchOrder.Checked;
            chkRecalc.Checked = chkRecalc.Visible && chkRecalc.Checked;
            stuGLSearch.Recalculate = chkRecalc.Checked;
            stuGLSearch.GeneralMode = chkGeneral.Visible && chkGeneral.Checked;
            stuGLSearch.PeriodMode = chkPeriod.Visible && chkPeriod.Checked;
            Session["SearchOption"] = stuGLSearch;
        }

        protected void TxtDisplayPeriodTextChanged(object sender, EventArgs e)
        {
            SetupDisplayPeriod();
        }

        private void SetupDisplayPeriod()
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtDisplayPeriod.Text = string.IsNullOrEmpty(txtDisplayPeriod.Text) ? Properties.Resources.defaultDisplayPeriodValue : txtDisplayPeriod.Text;
            stuGLSearch.InDisplayPeriod = txtDisplayPeriod.Visible ? int.Parse(txtDisplayPeriod.Text, InvariantCulture) : int.Parse(Properties.Resources.defaultDisplayPeriodValue, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }

        protected void IBDisplayPeriodClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtDisplayPeriod.Text = Properties.Resources.defaultDisplayPeriodValue;
            stuGLSearch.InDisplayPeriod = int.Parse(txtDisplayPeriod.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }

        protected void TxtCriticalNumTextChanged(object sender, EventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            stuGLSearch.InCriticalNum = int.Parse(txtCriticalNum.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }

        protected void IBCriticalNumClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtCriticalNum.Text = Properties.Resources.defaultCriticalNumValue;
            stuGLSearch.InCriticalNum = int.Parse(txtCriticalNum.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }

        protected void TxtTestPeriodsTextChanged(object sender, EventArgs e)
        {
            SetupTestPeriods();
        }

        private void SetupTestPeriods()
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtTestPeriods.Text = string.IsNullOrEmpty(txtTestPeriods.Text) ? "1" : txtTestPeriods.Text;
            chkSearchOrder.Checked = int.Parse(txtTestPeriods.Text, InvariantCulture) > 1 || chkSearchOrder.Checked;
            stuGLSearch.SearchOrder = chkSearchOrder.Visible && chkSearchOrder.Checked;
            txtTestPeriods.Text = txtTestPeriods.Visible ? txtTestPeriods.Text : Properties.Resources.defaultTestPeriodsValue;
            stuGLSearch.InTestPeriods = int.Parse(txtTestPeriods.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }

        protected void IBTestPeriodsClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtTestPeriods.Text = Properties.Resources.defaultTestPeriodsValue;
            stuGLSearch.InTestPeriods = int.Parse(txtTestPeriods.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }


        #endregion 顯示選項

        #region 計算方式
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected void RblButtonsSelectedIndexChanged(object sender, EventArgs e)
        {
            TurnOnButtons(rblButtons.SelectedValue);
        }

        #endregion 計算方式

        #region 比較欄位
        protected void ChkFieldCheckedChanged(object sender, EventArgs e)
        {
            SetupFields();
        }

        protected void CmbCompareSelectedIndexChanged(object sender, EventArgs e)
        {
            SetupFields();
        }

        private void SetupFields()
        {
            if (chkField.Checked)
            {
                phCompare01.Visible = true;
                phCompare02.Visible = true;
                phCompare03.Visible = true;
                phCompare04.Visible = true;
                phCompare05.Visible = true;
            }
            else
            {
                phCompare01.Visible = false;
                phCompare02.Visible = false;
                phCompare03.Visible = false;
                phCompare04.Visible = false;
                phCompare05.Visible = false;
                cmbCompare01.SelectedIndex = 0;
                cmbCompare02.SelectedIndex = 0;
                cmbCompare03.SelectedIndex = 0;
                cmbCompare04.SelectedIndex = 0;
                cmbCompare05.SelectedIndex = 0;
            }
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            chkField.Checked = chkField.Visible && chkField.Checked;
            dtlCompare.Visible = chkField.Checked;
            stuGLSearch.FieldMode = chkField.Checked;
            stuGLSearch = SetCompareString(stuGLSearch);
            Session["SearchOption"] = stuGLSearch;
        }

        private StuGLSearch SetCompareString(StuGLSearch stuSearch00)
        {
            StuGLSearch stuReturn = stuSearch00;
            List<string> lstCompare = new List<string>();
            //stuReturn.StrCompares = string.Empty;
            #region FieldMode

            if (stuReturn.FieldMode)
            {
                if (cmbCompare01.SelectedValue != "none")
                {
                    lstCompare.Add(cmbCompare01.SelectedValue);
                }
                if (cmbCompare02.SelectedValue != "none")
                {
                    lstCompare.Add(cmbCompare02.SelectedValue);
                }
                if (cmbCompare03.SelectedValue != "none")
                {
                    lstCompare.Add(cmbCompare03.SelectedValue);
                }
                if (cmbCompare04.SelectedValue != "none")
                {
                    lstCompare.Add(cmbCompare04.SelectedValue);
                }
                if (cmbCompare05.SelectedValue != "none")
                {
                    lstCompare.Add(cmbCompare05.SelectedValue);
                }
            }
            #endregion FieldMode

            if (lstCompare.Count > 0)
            {
                lstCompare.Sort();
                stuReturn.StrCompares = string.Join("#", lstCompare.Distinct());
            }
            else
            {
                stuReturn.StrCompares = "gen";
                stuReturn.StrComparesDetail = "none";
            }
            return stuReturn;
        }

        #endregion 比較欄位

        #region  拖牌設定
        protected void ChkNextNumsCheckedChanged(object sender, EventArgs e)
        {
            SetupNextNum();
        }

        protected void CmbNextNumsSelectedIndexChanged(object sender, EventArgs e)
        {
            SetupNextNum();
        }

        protected void CmbNextStepSelectedIndexChanged(object sender, EventArgs e)
        {
            SetupNextNum();
        }

        private void SetupNextNum()
        {
            //CheckBox checkBox = (CheckBox)sender;
            if (IsPostBack)
            {
                if (chkNextNums.Checked)
                {
                    dtlNextNum.Visible = true;
                    phNextNums.Visible = true;
                    phNextStep.Visible = true;
                }
                else
                {
                    dtlNextNum.Visible = false;
                    phNextNums.Visible = false;
                    phNextStep.Visible = false;
                    cmbNextNums.SelectedIndex = 0;
                }
            }
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            chkNextNums.Checked = chkNextNums.Visible && chkNextNums.Checked;
            stuGLSearch.NextNumsMode = chkNextNums.Checked;

            cmbNextNums.SelectedValue = chkNextNums.Checked ? cmbNextNums.SelectedValue : "1";
            stuGLSearch.InNextNums = int.Parse(cmbNextNums.SelectedValue, InvariantCulture);

            cmbNextStep.SelectedValue = chkNextNums.Checked ? cmbNextStep.SelectedValue : "1";
            stuGLSearch.InNextStep = int.Parse(cmbNextStep.SelectedValue, InvariantCulture);
            stuGLSearch.StrNextNumT = "none";
            stuGLSearch.StrNextNums = "none";
            stuGLSearch.StrNextNumSpe = "none";
            Session["SearchOption"] = stuGLSearch;

        }

        #endregion  拖牌設定

        #region 資料範圍

        protected void TxtDataRangeTextChanged(object sender, EventArgs e)
        {
            SetupDataRange();
        }

        private void SetupDataRange()
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];

            txtDataLimit.Text = string.IsNullOrEmpty(txtDataLimit.Text) ? Properties.Resources.defaultDataLimitValue : txtDataLimit.Text;
            txtDataLimit.Text = dtlDataRange.Visible ? txtDataLimit.Text : Properties.Resources.defaultDataLimitValue;
            stuGLSearch.InDataLimit = int.Parse(txtDataLimit.Text, InvariantCulture);

            txtDataOffset.Text = string.IsNullOrEmpty(txtDataOffset.Text) ? Properties.Resources.defaultDataOffsetValue : txtDataOffset.Text;
            txtDataOffset.Text = dtlDataRange.Visible ? txtDataOffset.Text : Properties.Resources.defaultDataOffsetValue;
            stuGLSearch.InDataOffset = int.Parse(txtDataOffset.Text, InvariantCulture);

            txtSearchLimit.Text = string.IsNullOrEmpty(txtSearchLimit.Text) ? Properties.Resources.defaultSearchLimitValue : txtSearchLimit.Text;
            txtSearchLimit.Text = dtlDataRange.Visible ? txtSearchLimit.Text : Properties.Resources.defaultSearchLimitValue;
            stuGLSearch.InSearchLimit = int.Parse(txtSearchLimit.Text, InvariantCulture);

            txtSearchOffset.Text = string.IsNullOrEmpty(txtSearchOffset.Text) ? Properties.Resources.defaultSearchOffsetValue : txtSearchOffset.Text;
            txtSearchOffset.Text = dtlDataRange.Visible ? txtSearchOffset.Text : Properties.Resources.defaultSearchOffsetValue;
            stuGLSearch.InSearchOffset = int.Parse(txtSearchOffset.Text, InvariantCulture);

            if (phFieldPeiodLimit.Visible && cmdFieldPeriodLimit.Visible)
            {
                stuGLSearch.InFieldPeriodLimit = int.Parse(cmdFieldPeriodLimit.SelectedValue, InvariantCulture);
            }
            else
            {
                cmdFieldPeriodLimit.SelectedIndex = 14;
                stuGLSearch.InFieldPeriodLimit = int.Parse(cmdFieldPeriodLimit.SelectedValue, InvariantCulture);
            }
            Session["SearchOption"] = stuGLSearch;
        }

        protected void IBDataLimitClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtDataLimit.Text = Properties.Resources.defaultDataLimitValue;

            stuGLSearch.InDataLimit = int.Parse(txtDataLimit.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;

        }
        protected void IBDataOffsetClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtDataOffset.Text = Properties.Resources.defaultDataOffsetValue;
            stuGLSearch.InDataOffset = int.Parse(txtDataOffset.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }
        protected void IBSearchLimitClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtSearchLimit.Text = Properties.Resources.defaultSearchLimitValue;
            stuGLSearch.InSearchLimit = int.Parse(txtSearchLimit.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }
        protected void IBSearchOffsetClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtSearchOffset.Text = Properties.Resources.defaultSearchOffsetValue;
            stuGLSearch.InSearchOffset = int.Parse(txtSearchOffset.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }
        #endregion 資料範圍

        #region 區間欄位

        protected void CmbFCheckSelectedIndexChanged(object sender, EventArgs e)
        {
            SetupFCheck();
        }

        private void SetupFCheck()
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            if (dtlFCheck.Visible)
            {

                List<string> lstFCheck = new List<string>();
                #region FieldMode

                if (cmbFCheck01.SelectedValue != "none")
                {
                    lstFCheck.Add(cmbFCheck01.SelectedValue);
                }
                if (cmbFCheck02.SelectedValue != "none")
                {
                    lstFCheck.Add(cmbFCheck02.SelectedValue);
                }
                if (cmbFCheck03.SelectedValue != "none")
                {
                    lstFCheck.Add(cmbFCheck03.SelectedValue);
                }
                if (cmbFCheck04.SelectedValue != "none")
                {
                    lstFCheck.Add(cmbFCheck04.SelectedValue);
                }
                if (cmbFCheck05.SelectedValue != "none")
                {
                    lstFCheck.Add(cmbFCheck05.SelectedValue);
                }
                if (cmbFCheck06.SelectedValue != "none")
                {
                    lstFCheck.Add(cmbFCheck06.SelectedValue);
                }
                #endregion FieldMode

                if (lstFCheck.Count > 0)
                {
                    lstFCheck.Sort();
                    stuGLSearch.StrSecField = string.Join("#", lstFCheck.Distinct());
                }
                else
                {
                    stuGLSearch.StrSecField = "none";
                }
            }
            else
            {
                cmbFCheck01.SelectedIndex = 0;
                cmbFCheck02.SelectedIndex = 0;
                cmbFCheck03.SelectedIndex = 0;
                cmbFCheck04.SelectedIndex = 0;
                cmbFCheck05.SelectedIndex = 0;
                cmbFCheck06.SelectedIndex = 0;
                stuGLSearch.StrSecField = "none";
            }
            Session["SearchOption"] = stuGLSearch;
        }

        #endregion 區間欄位

        #region 搜尋方法
        protected void TxtHistorySearchTextChanged(object sender, EventArgs e)
        {
            SetupHistorySearch();
        }

        private void SetupHistorySearch()
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtHistorySNs.Text = dtlSearch.Visible ? txtHistorySNs.Text : string.Empty;
            stuGLSearch.FreqFilterSNs = string.IsNullOrEmpty(txtHistorySNs.Text) ? "none" : txtHistorySNs.Text;

            txtHistoryPeriods.Text = string.IsNullOrEmpty(txtHistoryPeriods.Text) ? Properties.Resources.defaultHistoryPeriodsValue : txtHistoryPeriods.Text;
            txtHistoryPeriods.Text = dtlSearch.Visible ? txtHistoryPeriods.Text : Properties.Resources.defaultHistoryPeriodsValue;
            stuGLSearch.InHistoryPeriods = int.Parse(txtHistoryPeriods.Text, InvariantCulture);

            txtHistoryTestPeriods.Text = string.IsNullOrEmpty(txtHistoryTestPeriods.Text) ? Properties.Resources.defaultHistoryTestPeriodsValue : txtHistoryTestPeriods.Text;
            txtHistoryTestPeriods.Text = dtlSearch.Visible ? txtHistoryTestPeriods.Text : Properties.Resources.defaultHistoryTestPeriodsValue;
            stuGLSearch.HistoryTestPeriods = float.Parse(txtHistoryTestPeriods.Text, InvariantCulture);

            txtHitMin.Text = string.IsNullOrEmpty(txtHitMin.Text) ? Properties.Resources.defaultHitMinValue : txtHitMin.Text;
            txtHitMin.Text = dtlSearch.Visible ? txtHitMin.Text : Properties.Resources.defaultHitMinValue;
            stuGLSearch.InHitMin = int.Parse(txtHitMin.Text, InvariantCulture);

            chkFilterRange.Checked = dtlSearch.Visible && chkFilterRange.Checked;
            stuGLSearch.FilterRange = chkFilterRange.Checked;
            phFilterRange.Visible = chkFilterRange.Checked;

            txtFilterRange.Text = chkFilterRange.Checked ? txtFilterRange.Text : string.Empty;
            stuGLSearch.StrFilterRange = string.IsNullOrEmpty(txtFilterRange.Text) ? "none" : txtFilterRange.Text;

            txtFilterMin.Text = string.IsNullOrEmpty(txtFilterMin.Text) ? Properties.Resources.defaultFilterMinValue : txtFilterMin.Text;
            txtFilterMin.Text = chkFilterRange.Checked ? txtFilterMin.Text : Properties.Resources.defaultFilterMinValue;
            stuGLSearch.SglFilterMin = float.Parse(txtFilterMin.Text, InvariantCulture);

            txtFilterMax.Text = string.IsNullOrEmpty(txtFilterMax.Text) ? Properties.Resources.defaultFilterMaxValue : txtFilterMax.Text;
            txtFilterMax.Text = chkFilterRange.Checked ? txtFilterMax.Text : Properties.Resources.defaultFilterMaxValue;
            stuGLSearch.SglFilterMax = float.Parse(txtFilterMax.Text, InvariantCulture);

            Session["SearchOption"] = stuGLSearch;
        }

        protected void ChkFilterRangeCheckedChanged(object sender, EventArgs e)
        {
            phFilterRange.Visible = chkFilterRange.Checked;
        }

        protected void IBFilterRangeClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtFilterRange.Text = string.Empty;
            stuGLSearch.StrFilterRange = string.IsNullOrEmpty(txtFilterRange.Text) ? "none" : txtFilterRange.Text;
            Session["SearchOption"] = stuGLSearch;
        }

        protected void IBFilterMinClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtFilterMin.Text = Properties.Resources.defaultFilterMinValue;
            stuGLSearch.SglFilterMin = float.Parse(txtFilterMin.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }

        protected void IBFilterMaxClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtFilterMax.Text = Properties.Resources.defaultFilterMaxValue;
            stuGLSearch.SglFilterMax = float.Parse(txtFilterMax.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }
        protected void IBHistoryPeriodsClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtHistoryPeriods.Text = Properties.Resources.defaultHistoryPeriodsValue;
            stuGLSearch.InHistoryPeriods = int.Parse(txtHistoryPeriods.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }
        protected void IBHistoryTestPeriodsClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtHistoryTestPeriods.Text = Properties.Resources.defaultHistoryTestPeriodsValue;
            stuGLSearch.HistoryTestPeriods = float.Parse(txtHistoryTestPeriods.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }
        protected void IBHitMinClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtHitMin.Text = Properties.Resources.defaultHitMinValue;
            stuGLSearch.InHitMin = int.Parse(txtHitMin.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }
        #endregion 搜尋方法

        #region 百分比
        private void SetupTablePercent()
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            stuGLSearch.StrDeletes = new CglFunc().GetTestNum(dtlTP.Visible ? txtDelete.Text : string.Empty);

            stuGLSearch.StrNotDeletes = new CglFunc().GetTestNum(dtlTP.Visible ? txtNotDelete.Text : string.Empty);

            stuGLSearch.InDataRowsLimit = int.Parse(dtlTP.Visible ? txtDataRowsLimit.Text : Properties.Resources.defaultDataRowsLimitValue, InvariantCulture);

            Session["SearchOption"] = stuGLSearch;
        }
        protected void TxtTablePercentTextChanged(object sender, EventArgs e)
        {
            SetupTablePercent();
        }

        protected void IBDeleteClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtDelete.Text = string.Empty;
            stuGLSearch.StrDeletes = new CglFunc().GetTestNum(txtDelete.Text);
            Session["SearchOption"] = stuGLSearch;
        }
        protected void IBNotDeleteClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtNotDelete.Text = string.Empty;
            stuGLSearch.StrNotDeletes = new CglFunc().GetTestNum(txtNotDelete.Text);
            Session["SearchOption"] = stuGLSearch;
        }
        protected void IBDataRowsLimitClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtDataRowsLimit.Text = Properties.Resources.defaultDataRowsLimitValue;
            stuGLSearch.InDataRowsLimit = int.Parse(txtDataRowsLimit.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }
        #endregion 百分比

        #region 聰明組合
        protected void TxtSmartTextChanged(object sender, EventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];

            Session["SearchOption"] = stuGLSearch;
        }

        protected void IBTestNumClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtTestNum.Text = string.Empty;
            stuGLSearch.StrSmartTests = txtTestNum.Text;
            Session["SearchOption"] = stuGLSearch;
        }

        protected void BtnRndNumsClick(object sender, EventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            List<int> lstNums = new List<int>();
            if (int.Parse(ddlRndNums.SelectedValue, InvariantCulture) == new CglDataSet(stuGLSearch.LottoType).LottoNumbers)
            {
                for (int num = 1; num <= new CglDataSet(stuGLSearch.LottoType).LottoNumbers; num++)
                {
                    lstNums.Add(num);
                }
            }
            else
            {
                while (lstNums.Count < int.Parse(ddlRndNums.SelectedValue, InvariantCulture))
                {
                    Random rndRndNum = new Random((int)DateTime.Now.Ticks);
                    int intRndNums = rndRndNum.Next(1, new CglDataSet(stuGLSearch.LottoType).LottoNumbers);
                    if (!lstNums.Contains(intRndNums))
                    {
                        lstNums.Add(intRndNums);
                    }
                }
            }
            txtSmartTestNum.Text = string.Join("#", lstNums.ToArray());
            stuGLSearch.StrSmartTests = new CglFunc().GetTestNum(txtSmartTestNum.Text);
            Session["SearchOption"] = stuGLSearch;
        }

        protected void TxtSmartTestNumTextChanged(object sender, EventArgs e)
        {
            SetupSmartTestNum();
        }

        private void SetupSmartTestNum()
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            stuGLSearch.StrSmartTests = dtlSmartTest.Visible ? new CglFunc().GetTestNum(txtSmartTestNum.Text) : string.Empty;
            txtSmartTestNum.Text = stuGLSearch.StrSmartTests;
            Session["SearchOption"] = stuGLSearch;
        }

        protected void IBSmartTestNumClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtSmartTestNum.Text = string.Empty;
            stuGLSearch.StrSmartTests = new CglFunc().GetTestNum(txtSmartTestNum.Text);
            Session["SearchOption"] = stuGLSearch;
        }
        #endregion 聰明組合

        #region 數字DNA

        private void SetupFreqDNA()
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtFreqDNALength.Text = dtlFreqDNA.Visible && !string.IsNullOrEmpty(txtFreqDNALength.Text) ? txtFreqDNALength.Text : Properties.Resources.defaultFreqDNALength;
            stuGLSearch.InFreqDnaLength = int.Parse(txtFreqDNALength.Text, InvariantCulture);
            txtTargetTestPeriods.Text = dtlFreqDNA.Visible && !string.IsNullOrEmpty(txtTargetTestPeriods.Text) ? txtTargetTestPeriods.Text : Properties.Resources.defaultTargetTestPeriods;
            stuGLSearch.InTargetTestPeriods = int.Parse(txtTargetTestPeriods.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }

        protected void TxtFreqDNATextChanged(object sender, EventArgs e)
        {
            SetupFreqDNA();
        }

        protected void IBFreqDnaLengthClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtFreqDNALength.Text = Properties.Resources.defaultFreqDNALength;
            stuGLSearch.InFreqDnaLength = int.Parse(txtFreqDNALength.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }
        protected void IBTargetTestPeriodsClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtTargetTestPeriods.Text = Properties.Resources.defaultTargetTestPeriods;
            stuGLSearch.InTargetTestPeriods = int.Parse(txtTargetTestPeriods.Text, InvariantCulture);
            Session["SearchOption"] = stuGLSearch;
        }
        #endregion 數字DNA

        #region buttons
        protected void CmdOptionSelectedIndexChanged(object sender, EventArgs e) //按鈕選項
        {
            if (sender == null) { throw new ArgumentNullException(nameof(sender)); }

            TurnOnButtons(((DropDownList)sender).SelectedValue);
        }

        #endregion buttons

        // ---------------------------------------------------------------------------------------------------------

        #region Output


        protected void BtnShortcutClick(object sender, EventArgs e)
        {
            btnShowSearchOrder.Visible = ServerOption.Count > 0 && ((DataTable)ServerOption[KeySearchOrder]).Rows.Count > 0;
        }

        #endregion Output

        // ---------------------------------------------------------------------------------------------------------
    }
}