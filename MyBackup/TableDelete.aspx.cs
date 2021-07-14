using GalaxyLotto.ClassLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class TableDelete : BasePage
    {
        private DataSet DsDataSet { get; set; }

        private StuGLSearch _stuSearch00;
        private DropDownList ddlLottos;
        private DropDownList ddlDataBase;
        private DropDownList ddlTableName;
        private TextBox txtYes;
        private Label lblArgument;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:不要將常值當做已當地語系化的參數傳遞", MessageId = "System.Web.UI.WebControls.TextBox.set_Text(System.String)")]
#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            if (DsDataSet == null)
            {
                InitialDataSet();
                Panel pnlLottos = CreatPanel("pnlLottos");
                Label lblLottos = CreatLabel("lblLottos", Properties.Resources.lblLottos);
                pnlLottos.Controls.Add(lblLottos);
                ddlLottos = CreatDropDownList("ddlLottos", true);
                ddlLottos.DataSource = DsDataSet.Tables["dtLottos"].DefaultView;
                ddlLottos.DataValueField = "Lotto";
                ddlLottos.DataTextField = "Lotto";
                ddlLottos.DataBind();
                ddlLottos.SelectedIndex = 0;
                ddlLottos.SelectedIndexChanged += DdlLottos_SelectedIndexChanged;
                pnlLottos.Controls.Add(ddlLottos);

                lblArgument = CreatLabel("lblArgument");
                pnlLottos.Controls.Add(lblArgument);

                pnlDetail.Controls.Add(pnlLottos);

                Panel pnlDataBase = CreatPanel("pnlDataBase");
                Label lblDataBase = CreatLabel("lblDataBase", "資料庫");
                pnlDataBase.Controls.Add(lblDataBase);
                ddlDataBase = CreatDropDownList("ddlDataBase", true);
                ddlDataBase.DataSource = DsDataSet.Tables["dtDataBase"].DefaultView;
                ddlDataBase.DataValueField = "DataBase";
                ddlDataBase.DataTextField = "DataBase";
                ddlDataBase.DataBind();
                ddlDataBase.SelectedIndex = 0;
                ddlDataBase.SelectedIndexChanged += DdlDataBase_SelectedIndexChanged;
                pnlDataBase.Controls.Add(ddlDataBase);
                pnlDetail.Controls.Add(pnlDataBase);

                Panel pnlTableName = CreatPanel("pnlTableName");
                Label lblTableName = CreatLabel("lblTableName", "資料表");
                pnlTableName.Controls.Add(lblTableName);
                ddlTableName = CreatDropDownList("ddlTableName", true);
                SetTableName("Active");
                ddlTableName.SelectedIndexChanged += DdlTableName_SelectedIndexChanged;
                pnlTableName.Controls.Add(ddlTableName);
                pnlDetail.Controls.Add(pnlTableName);

                txtYes = CreatTextBox("txtYes", "輸入Yes", 60, true);
                pnlDetail.Controls.Add(txtYes);
                Button btnDelete = CreatButton("btnDelete", "", "刪除");
                btnDelete.TabIndex = 0;
                btnDelete.Click += BtnDelete_Click;
                pnlDetail.Controls.Add(btnDelete);
                Showlbl();
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:不要將常值當做已當地語系化的參數傳遞", MessageId = "System.Web.UI.WebControls.TextBox.set_Text(System.String)")]
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (txtYes.Text == "Yes")
            {
                txtYes.Text = ddlTableName.SelectedValue;
                SetLottos(ddlLottos.SelectedValue);
                SetDataBase(ddlDataBase.SelectedValue);
                _stuSearch00.StrCompares = ddlTableName.SelectedValue;
                switch (ddlTableName.SelectedValue)
                {
                    case "tblFreq":
                        _stuSearch00.StrCompares = "tblFreqProcess";
                        DeleteTable(_stuSearch00);
                        _stuSearch00.StrCompares = "tblFreq";
                        DeleteTable(_stuSearch00);
                        break;
                    case "tblTpHc01":
                        _stuSearch00.StrCompares = "tblTpHc02";
                        DeleteTable(_stuSearch00);
                        _stuSearch00.StrCompares = "tblTpHc01";
                        DeleteTable(_stuSearch00);
                        break;
                    case "tblTpHit01":
                        _stuSearch00.StrCompares = "tblTpHit02";
                        DeleteTable(_stuSearch00);
                        _stuSearch00.StrCompares = "tblTpHit01";
                        DeleteTable(_stuSearch00);
                        break;
                    case "tblTpHit1001":
                        _stuSearch00.StrCompares = "tblTpHit1002";
                        DeleteTable(_stuSearch00);
                        _stuSearch00.StrCompares = "tblTpHit1001";
                        DeleteTable(_stuSearch00);
                        break;
                    default:
                        DeleteTable(_stuSearch00);
                        break;
                }
                txtYes.Text = "輸入Yes";
            }
            else
            {
                txtYes.Text = "輸入Yes";
            }
        }

        private void DeleteTable(StuGLSearch stuGLSearch)
        {
            CheckData = true;
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = new SqlConnection(new CglData().SetDataBase(stuGLSearch.LottoType, stuGLSearch.DataBaseType));
                sqlCommand.CommandText = "SELECT * FROM @tableName ";
                sqlCommand.Parameters.AddWithValue("tableName", stuGLSearch.StrCompares);
                using (SqlDataAdapter sdaDelTable = new SqlDataAdapter(sqlCommand))
                {
                    using (DataTable dtDelTable = new DataTable())
                    {
                        dtDelTable.Locale = InvariantCulture;
                        sdaDelTable.Fill(dtDelTable);
                        if (dtDelTable.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtDelTable.Rows.Count; i++)
                            {
                                dtDelTable.Rows[i].Delete();
                            }
                        }
                        sdaDelTable.Update(dtDelTable);
                    }
                }
            }
        }

        private void SetTableName(string strTableName)
        {
            ddlTableName.DataSource = DsDataSet.Tables[strTableName].DefaultView;
            ddlTableName.DataValueField = "TableName";
            ddlTableName.DataTextField = "TableName";
            ddlTableName.SelectedIndex = 0;
            ddlTableName.DataBind();
        }

        private void DdlTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                Showlbl();
            }
        }

        private void DdlDataBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                SetDataBase(ddlDataBase.SelectedValue);
                switch (ddlDataBase.SelectedValue)
                {
                    case "Active":
                        SetTableName("Active");
                        break;
                    case "DataN":
                        SetTableName("DataN");
                        break;
                    case "Freq":
                        SetTableName("Freq");
                        break;
                    case "HighLow":
                        SetTableName("HighLow");
                        break;
                    case "Hit":
                        SetTableName("Hit");
                        break;
                    case "LastHit":
                        SetTableName("LastHit");
                        break;
                    case "Miss":
                        SetTableName("Miss");
                        break;
                    case "OddEven":
                        SetTableName("OddEven");
                        break;
                    case "Percent":
                        SetTableName("Percent");
                        break;
                    case "Sum":
                        SetTableName("Sum");
                        break;
                }

                Showlbl();
            }
        }

        private void SetDataBase(string strDataBase)
        {
            switch (strDataBase)
            {
                case "Active":
                    _stuSearch00.DataBaseType = DatabaseType.Active;
                    break;
                case "DataN":
                    _stuSearch00.DataBaseType = DatabaseType.DataN;
                    break;
                case "Freq":
                    _stuSearch00.DataBaseType = DatabaseType.Freq;
                    break;
                case "HighLow":
                    _stuSearch00.DataBaseType = DatabaseType.HighLow;
                    break;
                case "Hit":
                    _stuSearch00.DataBaseType = DatabaseType.Hit;
                    break;
                case "LastHit":
                    _stuSearch00.DataBaseType = DatabaseType.LastHit;
                    break;
                case "Miss":
                    _stuSearch00.DataBaseType = DatabaseType.Miss;
                    break;
                case "OddEven":
                    _stuSearch00.DataBaseType = DatabaseType.OddEven;
                    break;
                case "Percent":
                    _stuSearch00.DataBaseType = DatabaseType.Percent;
                    break;
                case "Sum":
                    _stuSearch00.DataBaseType = DatabaseType.Sum;
                    break;
            }
        }

        private void DdlLottos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                SetLottos(ddlLottos.SelectedValue);
                Showlbl();
            }
        }

        private void SetLottos(string strLottos)
        {
            switch (strLottos)
            {
                case "Lotto539":
                    _stuSearch00.LottoType = TargetTable.Lotto539;
                    break;
                case "LottoBig":
                    _stuSearch00.LottoType = TargetTable.LottoBig;
                    break;
                case "LottoDafu":
                    _stuSearch00.LottoType = TargetTable.LottoDafu;
                    break;
                case "LottoWeli":
                    _stuSearch00.LottoType = TargetTable.LottoWeli;
                    break;
                case "LottoSix":
                    _stuSearch00.LottoType = TargetTable.LottoSix;
                    break;
            }
        }

        private void Showlbl()
        {
            if (IsPostBack)
            {
                lblArgument.Text = string.Format(InvariantCulture, " (Lotto:{0} DataBase:{1} Table:{2} )", ddlLottos.SelectedValue, ddlDataBase.SelectedValue, ddlTableName.SelectedValue);
            }
        }

        private void InitialDataSet()
        {
            _stuSearch00 = new StuGLSearch { LottoType = TargetTable.Lotto539 };
            _stuSearch00.DataBaseType = DatabaseType.Active;
            _stuSearch00.StrCompares = "tblActive";

            using (DsDataSet = new DataSet())
            {
                DsDataSet.Locale = InvariantCulture;
                List<string> lstLottos = new List<string>() { "Lotto539", "LottoBig", "LottoDafu", "LottoWeli", "LottoSix" };
                Dictionary<string, List<string>> dicDataBase = new Dictionary<string, List<string>>
            {
                { "Active", new List<string>() { "tblActive" } },
                { "DataN", new List<string>() { "tblDataN00" } },
                { "Freq", new List<string>() { "tblFreq", "tblFreqSet" , "tblFreqSet01" } },
                { "HighLow", new List<string>() { "tblHighLow" } },
                { "Hit", new List<string>() { "tblHit", "tblHitAll" } },
                { "LastHit", new List<string>() { "tblLastHit00", "tblLastHit01", "tblLastHit02" , "tblLastHit03", "tblLastHit04" } },
                { "Miss", new List<string>() { "tblMissAll00", "tblMissAll01", "tblMissAll02" } },
                { "OddEven", new List<string>() { "tblOddEven" } },
                { "Percent", new List<string>() { "tblPercent", "tblPercentAll" , "tblTpHc01", "tblTpHit01", "tblTpHit1001" } },
                { "Sum", new List<string>() { "tblSum" } },
            };

                #region dtLottos
                using (DataTable dtLottos = new DataTable("dtLottos"))
                {
                    dtLottos.Locale = InvariantCulture;
                    using (DataColumn dcColumn = new DataColumn())
                    {
                        dcColumn.ColumnName = "Lotto";
                        dcColumn.DataType = typeof(string);
                        dtLottos.Columns.Add(dcColumn);
                    }
                    foreach (string item in lstLottos)
                    {
                        DataRow drRow = dtLottos.NewRow();
                        drRow["Lotto"] = item;
                        dtLottos.Rows.Add(drRow);
                    }
                    DsDataSet.Tables.Add(dtLottos);
                }
                #endregion

                using (DataTable dtDataBase = new DataTable("dtDataBase"))
                {
                    dtDataBase.Locale = InvariantCulture;
                    using (DataColumn dcColumn = new DataColumn())
                    {
                        dcColumn.ColumnName = "DataBase";
                        dcColumn.DataType = typeof(string);
                        dtDataBase.Columns.Add(dcColumn);
                        DsDataSet.Tables.Add(dtDataBase);
                    }
                    foreach (var item in dicDataBase)
                    {
                        DataRow drRow = dtDataBase.NewRow();
                        drRow["DataBase"] = item.Key;
                        dtDataBase.Rows.Add(drRow);
                        using (DataTable dtTableName = new DataTable(item.Key))
                        {
                            dtTableName.Locale = InvariantCulture;
                            using (DataColumn dcColumn = new DataColumn())
                            {
                                dcColumn.ColumnName = "TableName";
                                dcColumn.DataType = typeof(string);
                                dtTableName.Columns.Add(dcColumn);
                                DsDataSet.Tables.Add(dtTableName);
                            }
                            foreach (string tb in item.Value)
                            {
                                drRow = dtTableName.NewRow();
                                drRow["TableName"] = tb;
                                dtTableName.Rows.Add(drRow);
                            }
                        }
                    }
                }
            }
        }

    }
}
