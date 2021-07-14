using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class Solar24 : BasePage
    {
        private readonly Dictionary<string, string> dicSolar24 = new Dictionary<string, string>
                {
                    {"BS","立春"},{"RW","雨水"},{"WI","驚蟄"},{"SE","春分"},{"PB","清明"},{"GR","穀雨"},{"BU","立夏"},{"GF","小滿"},
                    {"GE","芒種"},{"SS","夏至"},{"SH","小暑"},{"GH","大暑"},{"BA","立秋"},{"LH","處暑"},{"WD","白露"},{"AE","秋分"},
                    {"CD","寒露"},{"FD","霜降"},{"BW","立冬"},{"SN","小雪"},{"GN","大雪"},{"WS","冬至"},{"SC","小寒"},{"GC","大寒"}
                };


        protected void Page_Load(object sender, EventArgs e)
        {
            ShowSolar24();
            ShowHour();
            ShowMin();
            ShowData();
        }

        private void ShowData()
        {
            int intYear = string.IsNullOrEmpty(txtDate.Text) ? 0 : int.Parse(txtDate.Text.Substring(0, 4), InvariantCulture) + 1911;
            using DataTable dtSolar24 = GetSolar24Data(intYear);
            GridView gvSolar24 = new GalaxyApp().CreatGridView("gvSolar24", "gltable", dtSolar24, true, false);
            gvSolar24.RowDataBound += GvSolar24_RowDataBound;
            gvSolar24.DataBind();
            pnlSoalr24.Controls.Add(gvSolar24);
        }

        private void GvSolar24_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    if (cell.ContainingField is BoundField field)
                    {
                        string strCell_ColumnName = field.DataField.ToString(InvariantCulture);
                        if (strCell_ColumnName == "SolarTerm")
                        {
                            cell.Text = dicSolar24[cell.Text.Replace(" ", "")];
                        }

                    }
                }
            }
        }

        private DataTable GetSolar24Data(int intYear)
        {
            CheckData = false;
            using SqlCommand sqlCommand = new SqlCommand
            {
                Connection = new SqlConnection(new CglData().SetDataBase(TargetTable.DataOption, DatabaseType.Data)),
                CommandTimeout = 1000
            };
#pragma warning disable IDE0066 // 將 switch 陳述式轉換為運算式
            switch (intYear)
#pragma warning restore IDE0066 // 將 switch 陳述式轉換為運算式
            {
                case 0:
                    sqlCommand.CommandText = "SELECT * FROM [tbl24SolarTems] ORDER BY [intYear] DESC";
                    break;
                default:
                    sqlCommand.CommandText = "SELECT * FROM [tbl24SolarTems] WHERE [intYear] = @intYear ORDER BY [intYear] DESC";
                    break;
            };
            sqlCommand.Parameters.AddWithValue("intYear", intYear);
            using SqlDataAdapter sleDataAdapter = new SqlDataAdapter(sqlCommand);
            using DataTable dtReturn = new DataTable { Locale = InvariantCulture };
            sleDataAdapter.Fill(dtReturn);
            return dtReturn;

        }

        private static void UpdateSolar24(string SolarTerm, DateTime dateTime)
        {
            using SqlCommand sqlCommand = new SqlCommand
            {
                Connection = new SqlConnection(new CglDBData().SetDataBase(TargetTable.DataOption, DatabaseType.Data)),
                CommandTimeout = 1000
            };
            sqlCommand.CommandText = "SELECT * FROM [tbl24SolarTems] WHERE [intYear] = @intYear AND [SolarTerm] = @SolarTerm";
            sqlCommand.Parameters.AddWithValue("intYear", dateTime.Year);
            sqlCommand.Parameters.AddWithValue("SolarTerm", SolarTerm);
            using SqlDataAdapter sleDataAdapter = new SqlDataAdapter(sqlCommand);
            using SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(sleDataAdapter);
            sleDataAdapter.UpdateCommand = sqlCommandBuilder.GetUpdateCommand();
            sleDataAdapter.DeleteCommand = sqlCommandBuilder.GetDeleteCommand();
            sleDataAdapter.InsertCommand = sqlCommandBuilder.GetInsertCommand();
            using DataTable dtReturn = new DataTable { Locale = InvariantCulture };
            sleDataAdapter.Fill(dtReturn);
            if (dtReturn.Rows.Count == 0)
            {
                DataRow drReturn = dtReturn.NewRow();
                drReturn["intYear"] = dateTime.Year;
                drReturn["SolarTerm"] = SolarTerm;
                drReturn["SolarTermDate"] = dateTime;
                dtReturn.Rows.Add(drReturn);
            }
            else
            {
                dtReturn.Rows[0]["intYear"] = dateTime.Year;
                dtReturn.Rows[0]["SolarTerm"] = SolarTerm;
                dtReturn.Rows[0]["SolarTermDate"] = dateTime;
            }
            sleDataAdapter.Update(dtReturn);
        }

        private void ShowMin()
        {
            if (ddlMin.Items.Count == 0)
            {
                for (int index = 0; index <= 59; index++)
                {
                    ddlMin.Items.Add(new ListItem(string.Format(InvariantCulture, "{0:d2}", index), string.Format(InvariantCulture, "{0:d2}", index)));
                }
            }
        }

        private void ShowHour()
        {
            if (ddlHour.Items.Count == 0)
            {
                for (int index = 0; index <= 23; index++)
                {
                    ddlHour.Items.Add(new ListItem(string.Format(InvariantCulture, "{0:d2}", index), string.Format(InvariantCulture, "{0:d2}", index)));
                }
            }
        }

        private void ShowSolar24()
        {
            if (ddlSolar24.Items.Count == 0)
            {
                foreach (KeyValuePair<string, string> item in dicSolar24)
                {
                    ddlSolar24.Items.Add(new ListItem(item.Value, item.Key));
                }
            }
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            string[] strDate = txtDate.Text.Split('/');
            int intYear = int.Parse(strDate[0], InvariantCulture);
            DateTime dateTime = Convert.ToDateTime(string.Format(InvariantCulture, "{0}/{1}/{2} {3}:{4}",
                                                                    intYear < 1911 ? intYear + 1911 : intYear, strDate[1], strDate[2],
                                                                    ddlHour.SelectedValue, ddlMin.SelectedValue), InvariantCulture);
            UpdateSolar24(ddlSolar24.SelectedValue, dateTime);
            ddlSolar24.SelectedIndex = ddlSolar24.SelectedIndex == ddlSolar24.Items.Count - 1 ? 0 : ddlSolar24.SelectedIndex + 1;
            pnlSoalr24.Controls.Clear();

            ShowData();
        }

    }
}