using GalaxyLotto.ClassLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace GalaxyLottoWeb.Pages.Freq
{
    public partial class SearchFreq : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Session.Add("SearchOption", stuSearchTemp);
                DataRangeStartInit(new StuGLSearch(TargetTable.Lotto539).LottoType);
                SetupDdlCompare();
            };
            lblTitle.Text = string.Format(InvariantCulture, "{0}({1})", Page.Title, Form.Target);
        }
        //--------------------------------------------------------------------------------

        private void SearchOption_init()
        {
            //StuGLSearch stuSearchTemp = (StuGLSearch)Session["SearchOption"];

            #region base data
            cmbCompareType.SelectedIndex = 0;
            chkGeneral.Checked = true;
            chkPeriod.Checked = false;
            txtTestPeriods.Text = Properties.Resources.defaultTestPeriodsValue;            
            #endregion base data

            #region FieldMode
            chkField.Checked = false;
            cmbCompare01.SelectedIndex = 0;
            cmbCompare02.SelectedIndex = 0;
            cmbCompare03.SelectedIndex = 0;
            cmbCompare04.SelectedIndex = 0;
            cmbCompare05.SelectedIndex = 0;            
            #endregion FieldMode

            #region NextMode
            chkNextNums.Checked = false;
            cmbNextNums.SelectedIndex = 0;
            cmbNextStep.SelectedIndex = 0;            
            #endregion NextMode

            #region Search Range
            txtDataLimit.Text = Properties.Resources.defaultDataLimitValue;
            txtDataOffset.Text = Properties.Resources.defaultDataOffsetValue;
            txtSearchLimit.Text = Properties.Resources.defaultSearchLimitValue;
            txtSearchOffset.Text = Properties.Resources.defaultSearchOffsetValue;            
            #endregion Search Range

            //chkSearchFields.Checked = false;
            //stuSearchTemp.SearchFileds = chkSearchFields.Checked;

            #region Display
            txtDisplayPeriod.Text = Properties.Resources.defaultDisplayPeriodValue;            
            #endregion Display

            #region Filter
            //txtHistorySNs.Text = string.Empty;
            //stuSearchTemp.FreqFilterSNs = string.IsNullOrEmpty(txtHistorySNs.Text) ? "none" : txtHistorySNs.Text;
            //txtHistoryPeriods.Text = Properties.Resources.defaultHistoryPeriodsValue;
            //stuSearchTemp.InHistoryPeriods = int.Parse(txtHistoryPeriods.Text, InvariantCulture);
            //txtHistoryTestPeriods.Text = Properties.Resources.defaultHistoryTestPeriodsValue;
            //stuSearchTemp.HistoryTestPeriods = float.Parse(txtHistoryTestPeriods.Text, InvariantCulture);
            //txtHitMin.Text = Properties.Resources.defaultHitMinValue;
            //stuSearchTemp.InHitMin = int.Parse(txtHitMin.Text, InvariantCulture);
            //chkFilterRange.Checked = false;
            //stuSearchTemp.FilterRange = chkFilterRange.Checked;
            //txtFilterRange.Text = string.Empty;
            //stuSearchTemp.StrFilterRange = string.IsNullOrEmpty(txtFilterRange.Text) ? "none" : txtFilterRange.Text;
            //txtFilterMin.Text = Properties.Resources.defaultFilterMinValue;
            //stuSearchTemp.SglFilterMin = float.Parse(txtFilterMin.Text, InvariantCulture);
            //txtFilterMax.Text = Properties.Resources.defaultFilterMaxValue;
            //stuSearchTemp.SglFilterMax = float.Parse(txtFilterMax.Text, InvariantCulture);
            #endregion Filter
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

        private static void SetCombobox(ref DropDownList comboBox, DataTable dataTable)
        {
            if (comboBox.DataSource != null && (comboBox.ID == "cmbLottoType")) { return; }
            comboBox.DataSource = dataTable.DefaultView;
            comboBox.DataValueField = "id";
            comboBox.DataTextField = "description";
            comboBox.DataBind();
            comboBox.SelectedIndex = 0;
        }

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

        //--------------------------------------------------------------------------------
        #region 基本設置

        protected void CmbLottoTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            //StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            switch (cmbLottoType.SelectedValue)
            {
                case "LottoBig":
                    DataRangeStartInit(TargetTable.LottoBig);
                    break;
                case "Lotto539":
                    DataRangeStartInit(TargetTable.Lotto539);
                    break;
                case "LottoWeli":
                    DataRangeStartInit(TargetTable.LottoWeli);
                    break;
                case "LottoSix":
                    DataRangeStartInit(TargetTable.LottoSix);
                    break;
            }
            //Session["SearchOption"] = stuGLSearch;
            //DataRangeStartInit(stuGLSearch.LottoType);
        }

        private void DataRangeStartInit(TargetTable lottoType)
        {
            //StuGLSearch searchTemp = (StuGLSearch)Session["SearchOption"];
            using DataTable dtDataRangeEnd = GetDataRangeEndDT(lottoType);
            cmbDataRangeEnd.DataSource = dtDataRangeEnd.DefaultView;
            cmbDataRangeEnd.DataValueField = "lngTotalSN";
            cmbDataRangeEnd.DataTextField = "lngDateSN";
            cmbDataRangeEnd.SelectedIndex = 0;
            cmbDataRangeEnd.DataBind();
            //searchTemp.LngDataStart = long.Parse(dtDataRangeEnd.Rows[dtDataRangeEnd.Rows.Count - 1]["lngTotalSN"].ToString(), InvariantCulture);
            //searchTemp.LngDataEnd = long.Parse(dtDataRangeEnd.Rows[0]["lngTotalSN"].ToString(), InvariantCulture);
            //searchTemp.LngTotalSN = long.Parse(dtDataRangeEnd.Rows[0]["lngTotalSN"].ToString(), InvariantCulture);
            //Session["SearchOption"] = searchTemp;
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

        //protected void CmbDataRangeEndSelectedIndexChanged(object sender, EventArgs e)
        //{
        //    StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
        //    stuGLSearch.LngDataEnd = long.Parse(cmbDataRangeEnd.SelectedValue, InvariantCulture);
        //    stuGLSearch.LngTotalSN = long.Parse(cmbDataRangeEnd.SelectedValue, InvariantCulture);
        //    Session["SearchOption"] = stuGLSearch;
        //}

        //private void SetupCompareType()
        //{
        //    StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
        //    stuGLSearch.StrCompareType = cmbCompareType.Visible ? cmbCompareType.SelectedValue : "AND";
        //    Session["SearchOption"] = stuGLSearch;
        //}

        //protected void CmbCompareTypeSelectedIndexChanged(object sender, EventArgs e)
        //{
        //    SetupCompareType();
        //}

        #endregion 基本設置

        //--------------------------------------------------------------------------------

        #region 資料範圍
        //protected void TxtDataRangeTextChanged(object sender, EventArgs e)
        //{
        //    SetupDataRange();
        //}

        //private void SetupDataRange()
        //{
        //    StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];

        //    txtDataLimit.Text = string.IsNullOrEmpty(txtDataLimit.Text) ? Properties.Resources.defaultDataLimitValue : txtDataLimit.Text;
        //    txtDataLimit.Text = dtlDataRange.Visible ? txtDataLimit.Text : Properties.Resources.defaultDataLimitValue;
        //    stuGLSearch.InDataLimit = int.Parse(txtDataLimit.Text, InvariantCulture);

        //    txtDataOffset.Text = string.IsNullOrEmpty(txtDataOffset.Text) ? Properties.Resources.defaultDataOffsetValue : txtDataOffset.Text;
        //    txtDataOffset.Text = dtlDataRange.Visible ? txtDataOffset.Text : Properties.Resources.defaultDataOffsetValue;
        //    stuGLSearch.InDataOffset = int.Parse(txtDataOffset.Text, InvariantCulture);

        //    txtSearchLimit.Text = string.IsNullOrEmpty(txtSearchLimit.Text) ? Properties.Resources.defaultSearchLimitValue : txtSearchLimit.Text;
        //    txtSearchLimit.Text = dtlDataRange.Visible ? txtSearchLimit.Text : Properties.Resources.defaultSearchLimitValue;
        //    stuGLSearch.InSearchLimit = int.Parse(txtSearchLimit.Text, InvariantCulture);

        //    txtSearchOffset.Text = string.IsNullOrEmpty(txtSearchOffset.Text) ? Properties.Resources.defaultSearchOffsetValue : txtSearchOffset.Text;
        //    txtSearchOffset.Text = dtlDataRange.Visible ? txtSearchOffset.Text : Properties.Resources.defaultSearchOffsetValue;
        //    stuGLSearch.InSearchOffset = int.Parse(txtSearchOffset.Text, InvariantCulture);

        //    //if (phFieldPeiodLimit.Visible && cmdFieldPeriodLimit.Visible)
        //    //{
        //    //    stuGLSearch.InFieldPeriodLimit = int.Parse(cmdFieldPeriodLimit.SelectedValue, InvariantCulture);
        //    //}
        //    //else
        //    //{
        //    //    cmdFieldPeriodLimit.SelectedIndex = 14;
        //    //    stuGLSearch.InFieldPeriodLimit = int.Parse(cmdFieldPeriodLimit.SelectedValue, InvariantCulture);
        //    //}
        //    Session["SearchOption"] = stuGLSearch;
        //}

        protected void IBDataLimitClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtDataLimit.Text = Properties.Resources.defaultDataLimitValue;
            //stuGLSearch.InDataLimit = int.Parse(txtDataLimit.Text, InvariantCulture);
            //Session["SearchOption"] = stuGLSearch;

        }

        protected void IBDataOffsetClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtDataOffset.Text = Properties.Resources.defaultDataOffsetValue;
            //stuGLSearch.InDataOffset = int.Parse(txtDataOffset.Text, InvariantCulture);
            //Session["SearchOption"] = stuGLSearch;
        }

        protected void IBSearchLimitClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtSearchLimit.Text = Properties.Resources.defaultSearchLimitValue;
            //stuGLSearch.InSearchLimit = int.Parse(txtSearchLimit.Text, InvariantCulture);
            //Session["SearchOption"] = stuGLSearch;
        }

        protected void IBSearchOffsetClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtSearchOffset.Text = Properties.Resources.defaultSearchOffsetValue;
            //stuGLSearch.InSearchOffset = int.Parse(txtSearchOffset.Text, InvariantCulture);
            //Session["SearchOption"] = stuGLSearch;
        }

        #endregion 資料範圍
        //--------------------------------------------------------------------------------
        #region 比較欄位

        protected void ChkFieldCheckedChanged(object sender, EventArgs e)
        {
            SetupFields();
        }

        //protected void CmbCompareSelectedIndexChanged(object sender, EventArgs e)
        //{
        //    SetupFields();
        //}

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
            //StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            chkField.Checked = chkField.Visible && chkField.Checked;
            dtlCompare.Visible = chkField.Checked;
            //stuGLSearch.FieldMode = chkField.Checked;
            //stuGLSearch = SetCompareString(stuGLSearch);
            //Session["SearchOption"] = stuGLSearch;
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
                stuReturn = new CglMethod().SetComparesDetail(stuReturn);
            }
            else
            {
                stuReturn.StrCompares = "gen";
                stuReturn.StrComparesDetail = "none";
            }
            return stuReturn;
        }

        #endregion 比較欄位
        //--------------------------------------------------------------------------------

        #region  拖牌設定
        protected void ChkNextNumsCheckedChanged(object sender, EventArgs e)
        {
            SetupNextNum();
        }

        //protected void CmbNextNums_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    SetupNextNum();
        //}

        private void SetupNextNum()
        {
            //CheckBox checkBox = (CheckBox)sender;
            if (chkNextNums.Checked)
            {
                cmbNextNums.SelectedIndex = cmbNextNums.SelectedIndex == 0 ? 1 : cmbNextNums.SelectedIndex;
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
                cmbNextStep.SelectedIndex = 0;
            }
            chkNextNums.Checked = chkNextNums.Visible && chkNextNums.Checked;
            //StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            //stuGLSearch.NextNumsMode = chkNextNums.Checked;
            //cmbNextNums.SelectedValue = chkNextNums.Checked ? cmbNextNums.SelectedValue : "1";

            //stuGLSearch.InNextNums = int.Parse(cmbNextNums.SelectedValue, InvariantCulture);

            //cmbNextStep.SelectedValue = chkNextNums.Checked ? cmbNextStep.SelectedValue : "1";
            //stuGLSearch.InNextStep = int.Parse(cmbNextStep.SelectedValue, InvariantCulture);
            //stuGLSearch.StrNextNumT = "none";
            //stuGLSearch.StrNextNums = "none";
            //stuGLSearch.StrNextNumSpe = "none";
            //Session["SearchOption"] = stuGLSearch;

        }

        #endregion  拖牌設定


        //--------------------------------------------------------------------------------


        //protected void DisplayCheckedChanged(object sender, EventArgs e)
        //{
        //    SetupDisplayChecked();
        //}

        //protected void TxtDisplayPeriodTextChanged(object sender, EventArgs e)
        //{
        //    SetupDisplayPeriod();
        //}

        //private void SetupDisplayPeriod()
        //{
        //    StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
        //    txtDisplayPeriod.Text = string.IsNullOrEmpty(txtDisplayPeriod.Text) ? Properties.Resources.defaultDisplayPeriodValue : txtDisplayPeriod.Text;
        //    stuGLSearch.InDisplayPeriod = txtDisplayPeriod.Visible ? int.Parse(txtDisplayPeriod.Text, InvariantCulture) : int.Parse(Properties.Resources.defaultDisplayPeriodValue, InvariantCulture);
        //    Session["SearchOption"] = stuGLSearch;
        //}

        //private void SetupDisplayChecked()
        //{
        //    StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
        //    stuGLSearch.ShowProcess = chkShowProcess.Visible && chkShowProcess.Checked ? ShowProcess.Visible : ShowProcess.Hide;
        //    stuGLSearch.ShowGraphic = chkshowGraphic.Visible && chkshowGraphic.Checked ? ShowGraphic.Visible : ShowGraphic.Hide;
        //    stuGLSearch.ShowStaticHtml = chkShowStaticHtml.Visible && chkShowStaticHtml.Checked;
        //    chkSearchOrder.Checked = chkSearchFields.Checked || chkSearchOrder.Checked;
        //    stuGLSearch.SearchFileds = chkSearchFields.Visible && chkSearchFields.Checked;
        //    stuGLSearch.SearchOrder = chkSearchOrder.Visible && chkSearchOrder.Checked;
        //    chkRecalc.Checked = chkRecalc.Visible && chkRecalc.Checked;
        //    stuGLSearch.Recalculate = chkRecalc.Checked;
        //    stuGLSearch.GeneralMode = chkGeneral.Visible && chkGeneral.Checked;
        //    stuGLSearch.PeriodMode = chkPeriod.Visible && chkPeriod.Checked;
        //    Session["SearchOption"] = stuGLSearch;
        //}

        protected void IBDisplayPeriodClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtDisplayPeriod.Text = Properties.Resources.defaultDisplayPeriodValue;
            //stuGLSearch.InDisplayPeriod = int.Parse(txtDisplayPeriod.Text, InvariantCulture);
            //Session["SearchOption"] = stuGLSearch;
        }

        protected void IBTestPeriodsClick(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
            txtTestPeriods.Text = Properties.Resources.defaultTestPeriodsValue;
            //stuGLSearch.InTestPeriods = int.Parse(txtTestPeriods.Text, InvariantCulture);
            //Session["SearchOption"] = stuGLSearch;
        }

        //--------------------------------------------------------------------------------

        //protected void TxtTestPeriodsTextChanged(object sender, EventArgs e)
        //{
        //    SetupTestPeriods();
        //}

        //private void SetupTestPeriods()
        //{
        //    StuGLSearch stuGLSearch = (StuGLSearch)Session["SearchOption"];
        //    txtTestPeriods.Text = string.IsNullOrEmpty(txtTestPeriods.Text) ? "1" : txtTestPeriods.Text;
        //    chkSearchOrder.Checked = int.Parse(txtTestPeriods.Text, InvariantCulture) > 1 || chkSearchOrder.Checked;
        //    stuGLSearch.SearchOrder = chkSearchOrder.Visible && chkSearchOrder.Checked;
        //    txtTestPeriods.Text = txtTestPeriods.Visible ? txtTestPeriods.Text : Properties.Resources.defaultTestPeriodsValue;
        //    stuGLSearch.InTestPeriods = int.Parse(txtTestPeriods.Text, InvariantCulture);
        //    Session["SearchOption"] = stuGLSearch;
        //}

        protected void BtnRun_Click(object sender, EventArgs e)
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("function OpenWindowWithPost(url, windowoption, targetname, params){" +
                "var outform = document.createElement('form');" +
                "outform.setAttribute('id', 'postform');" +
                "outform.setAttribute('method', 'get');" +
                "outform.setAttribute('action', url);" +
                "outform.setAttribute('target', targetname);" +
                "outform.setAttribute('accept-charset', 'utf-8');" +
                "outform.setAttribute('enctype', 'application/x-www-form-urlencoded');" +
                "for (var i in params) {" +
                "if (params.hasOwnProperty(i)) {" +
                "var input = document.createElement('input'); " +
                "input.type = 'textbox';" +
                "input.name = i;" +
                "input.value = params[i];" +
                "outform.appendChild(input);}}" +
                "document.body.appendChild(outform);" +
                "outform.submit();" +
                "document.body.removeChild(outform);}");
            stringBuilder.AppendLine("function OpenNewFile(filename, windowoptions, targetname){" +
                                     "var param = [];" +
                                     "param['uid'] = '" + SetupSession() + "';" +
                                     "OpenWindowWithPost(filename, windowoptions, targetname, param);}" +
                                     "OpenNewFile('FreqResult.aspx', '', '_blank')");

            ScriptManager.RegisterStartupScript(this, typeof(string), "OPEN_WINDOW", stringBuilder.ToString(), true);
        }

        private string SetupSession()
        {
            var targetTable = cmbLottoType.SelectedValue switch
            {
                "LottoBig" => TargetTable.LottoBig,
                "Lotto539" => TargetTable.Lotto539,
                "LottoWeli" => TargetTable.LottoWeli,
                "LottoSix" => TargetTable.LottoSix,
                _ => TargetTable.Lotto539,
            };

            StuGLSearch stuGLSearch = new StuGLSearch(targetTable)
            {
                LngDataEnd = long.Parse(cmbDataRangeEnd.SelectedValue, InvariantCulture),
                LngTotalSN = long.Parse(cmbDataRangeEnd.SelectedValue, InvariantCulture),
                StrCompareType = cmbCompareType.Visible ? cmbCompareType.SelectedValue : "AND" //包含方式
            };

            //資料限制
            txtDataLimit.Text = string.IsNullOrEmpty(txtDataLimit.Text) ? Properties.Resources.defaultDataLimitValue : txtDataLimit.Text;
            txtDataLimit.Text = dtlDataRange.Visible ? txtDataLimit.Text : Properties.Resources.defaultDataLimitValue;
            stuGLSearch.InDataLimit = int.Parse(txtDataLimit.Text, InvariantCulture);

            //資料位移
            txtDataOffset.Text = string.IsNullOrEmpty(txtDataOffset.Text) ? Properties.Resources.defaultDataOffsetValue : txtDataOffset.Text;
            txtDataOffset.Text = dtlDataRange.Visible ? txtDataOffset.Text : Properties.Resources.defaultDataOffsetValue;
            stuGLSearch.InDataOffset = int.Parse(txtDataOffset.Text, InvariantCulture);

            //搜尋限制
            txtSearchLimit.Text = string.IsNullOrEmpty(txtSearchLimit.Text) ? Properties.Resources.defaultSearchLimitValue : txtSearchLimit.Text;
            txtSearchLimit.Text = dtlDataRange.Visible ? txtSearchLimit.Text : Properties.Resources.defaultSearchLimitValue;
            stuGLSearch.InSearchLimit = int.Parse(txtSearchLimit.Text, InvariantCulture);

            //搜尋位移
            txtSearchOffset.Text = string.IsNullOrEmpty(txtSearchOffset.Text) ? Properties.Resources.defaultSearchOffsetValue : txtSearchOffset.Text;
            txtSearchOffset.Text = dtlDataRange.Visible ? txtSearchOffset.Text : Properties.Resources.defaultSearchOffsetValue;
            stuGLSearch.InSearchOffset = int.Parse(txtSearchOffset.Text, InvariantCulture);

            // 欄位
            stuGLSearch.FieldMode = chkField.Visible && chkField.Checked;
            stuGLSearch = SetCompareString(stuGLSearch);

            //拖牌
            stuGLSearch.NextNumsMode = chkNextNums.Visible && chkNextNums.Checked;
            stuGLSearch.InNextNums = int.Parse(chkNextNums.Checked ? cmbNextNums.SelectedValue : "1", InvariantCulture);
            stuGLSearch.InNextStep = int.Parse(chkNextNums.Checked ? cmbNextStep.SelectedValue : "1", InvariantCulture);
            stuGLSearch = new CglMethod().SetNextNums(stuGLSearch);
            stuGLSearch.StrNextNums = "none";
            stuGLSearch.StrNextNumSpe = "none";

            //顯示期數DP
            txtDisplayPeriod.Text = string.IsNullOrEmpty(txtDisplayPeriod.Text) ? Properties.Resources.defaultDisplayPeriodValue : txtDisplayPeriod.Text;
            stuGLSearch.InDisplayPeriod = txtDisplayPeriod.Visible ? int.Parse(txtDisplayPeriod.Text, InvariantCulture) : int.Parse(Properties.Resources.defaultDisplayPeriodValue, InvariantCulture);

            //連續測試期數
            txtTestPeriods.Text = string.IsNullOrEmpty(txtTestPeriods.Text) ? "1" : txtTestPeriods.Text;
            txtTestPeriods.Text = txtTestPeriods.Visible ? txtTestPeriods.Text : Properties.Resources.defaultTestPeriodsValue;
            stuGLSearch.InTestPeriods = int.Parse(txtTestPeriods.Text, InvariantCulture);

            stuGLSearch.GeneralMode = chkGeneral.Visible && chkGeneral.Checked;
            stuGLSearch.PeriodMode = chkPeriod.Visible && chkPeriod.Checked;

            //產生uid
            string uid = string.Format(InvariantCulture, "{0}_{1}", "FreqResult", stuGLSearch.GetHashCode());
            Session.Add(uid, stuGLSearch);

            return uid;
        }

        protected void BtnResetClick(object sender, EventArgs e)
        {
            SearchOption_init();
        }
    }
}