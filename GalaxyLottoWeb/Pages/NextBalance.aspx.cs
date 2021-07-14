using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class NextBalance : BasePage
    {
        private StuGLSearch _gstuSearch = new StuGLSearch(TargetTable.Lotto539);
        private Dictionary<string, string> _dicNumcssclass;

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            if (!IsPostBack)
            {
                DataRangeStartInit(TargetTable.Lotto539);
            }

            SearchOption_init();
            //if (ViewState["_dicNumcssclass"] == null) { ViewState.Add("_dicNumcssclass", GetNumcssclass(_gstuSearch)); }
            _dicNumcssclass = new GalaxyApp().GetNumcssclass(_gstuSearch);
            // 當期資料
            GridView gvCurrentData = new GalaxyApp().CreatGridView("gvCurrentData", "gltable ", new CglFunc().CDicTOTable(new CglData().GetCurrentDataDics(_gstuSearch)), true, true);
            gvCurrentData.DataBind();
            pnlCurrentData.Controls.Add(gvCurrentData);
        }

        private void SearchOption_init()
        {
            switch (cmbLottoType.SelectedValue)
            {
                case "LottoBig":
                    _gstuSearch.LottoType = TargetTable.LottoBig;
                    break;
                case "Lotto539":
                    _gstuSearch.LottoType = TargetTable.Lotto539;
                    break;
                case "LottoWeli":
                    _gstuSearch.LottoType = TargetTable.LottoWeli;
                    break;
                case "LottoSix":
                    _gstuSearch.LottoType = TargetTable.LottoSix;
                    break;
            }
            if (((StuGLSearch)ViewState["_gstuSearch"]).LottoType != _gstuSearch.LottoType)
            {
                DataRangeStartInit(_gstuSearch.LottoType);
            }
            _gstuSearch.LngDataEnd = long.Parse(cmbDataRangeEnd.SelectedValue, InvariantCulture);
            _gstuSearch.LngTotalSN = long.Parse(cmbDataRangeEnd.SelectedValue, InvariantCulture);
        }

        private void DataRangeStartInit(TargetTable lottoType)
        {

            using DataTable dtDataRangeEnd = GetDataRangeEndDT(lottoType);
            cmbDataRangeEnd.DataSource = dtDataRangeEnd.DefaultView;
            cmbDataRangeEnd.DataValueField = "lngTotalSN";
            cmbDataRangeEnd.DataTextField = "lngDateSN";
            cmbDataRangeEnd.SelectedIndex = 0;
            cmbDataRangeEnd.DataBind();
            _gstuSearch.LngDataStart = long.Parse(dtDataRangeEnd.Rows[dtDataRangeEnd.Rows.Count - 1]["lngTotalSN"].ToString(), InvariantCulture);
            _gstuSearch.LngDataEnd = long.Parse(dtDataRangeEnd.Rows[0]["lngTotalSN"].ToString(), InvariantCulture);
            _gstuSearch.LngTotalSN = long.Parse(dtDataRangeEnd.Rows[0]["lngTotalSN"].ToString(), InvariantCulture);
            ViewState["_gstuSearch"] = _gstuSearch;
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
                using DataTable dtImport = new DataTable { Locale = InvariantCulture };
                sqlDataAdapter.Fill(dtImport);
                strlngTotalSN = dtImport.Rows[0]["lngTotalSN"].ToString();
                #endregion

                using SqlCommand sqlCommand01 = new SqlCommand
                {
                    Connection = new SqlConnection(new CglData().SetDataBase(lottoType, DatabaseType.Data)),
                    CommandText = "SELECT [lngTotalSN] , [lngDateSN] FROM [tblData] WHERE [lngTotalSN] <= @lngTotalSN ORDER BY [lngTotalSN] DESC ;"
                };
                sqlCommand01.Parameters.AddWithValue("lngTotalSN", strlngTotalSN);
                using SqlDataAdapter sqlDataAdapter01 = new SqlDataAdapter(sqlCommand01);
                using DataTable dtImport01 = new DataTable { Locale = InvariantCulture };
                sqlDataAdapter01.Fill(dtImport01);
                ViewState["DataRangeEnd" + lottoType.ToString()] = dtImport01;
                return dtImport01;
            }
            else
            {
                return (DataTable)ViewState["DataRangeEnd" + lottoType.ToString()];
            }
        }
        // ---------------------------------------------------------------------------------------------------------
        protected void BtnCalculateClick(object sender, EventArgs e)
        {
            List<string> lstNextMin = new List<string> { txtNextMin1.Text, txtNextMin2.Text, txtNextMin3.Text, txtNextMin4.Text, txtNextMin5.Text };
            txtNextMin1.ToolTip = txtNextMin1.Text; txtNextMin2.ToolTip = txtNextMin2.Text; txtNextMin3.ToolTip = txtNextMin3.Text; txtNextMin4.ToolTip = txtNextMin4.Text; txtNextMin5.ToolTip = txtNextMin5.Text;
            List<string> lstNextMax = new List<string> { txtNextMax1.Text, txtNextMax2.Text, txtNextMax3.Text, txtNextMax4.Text, txtNextMax5.Text };
            txtNextMax1.ToolTip = txtNextMax1.Text; txtNextMax2.ToolTip = txtNextMax2.Text; txtNextMax3.ToolTip = txtNextMax3.Text; txtNextMax4.ToolTip = txtNextMax4.Text; txtNextMax5.ToolTip = txtNextMax5.Text;
            List<string> lstNext = CombindList(lstNextMin, lstNextMax);
            ShowGvSumNext(lstNext);

            List<string> lstBalanceMin = new List<string> { txtBalanceMin1.Text, txtBalanceMin2.Text, txtBalanceMin3.Text, txtBalanceMin4.Text, txtBalanceMin5.Text };
            txtBalanceMin1.ToolTip = txtBalanceMin1.Text; txtBalanceMin2.ToolTip = txtBalanceMin2.Text; txtBalanceMin3.ToolTip = txtBalanceMin3.Text; txtBalanceMin4.ToolTip = txtBalanceMin4.Text; txtBalanceMin5.ToolTip = txtBalanceMin5.Text;
            List<string> lstBalanceMax = new List<string> { txtBalanceMax1.Text, txtBalanceMax2.Text, txtBalanceMax3.Text, txtBalanceMax4.Text, txtBalanceMax5.Text };
            txtBalanceMax1.ToolTip = txtBalanceMax1.Text; txtBalanceMax2.ToolTip = txtBalanceMax2.Text; txtBalanceMax3.ToolTip = txtBalanceMax3.Text; txtBalanceMax4.ToolTip = txtBalanceMax4.Text; txtBalanceMax5.ToolTip = txtBalanceMax5.Text;
            List<string> lstBalance = CombindList(lstBalanceMin, lstBalanceMax);
            ShowGvSumBalance(lstBalance);

            txtSideColNext.Text = CheckSideColumn(lstNextMin, lstNextMax);
            txtSideColNext.ToolTip = txtSideColNext.Text;
            txtSideColBalance.Text = CheckSideColumn(lstBalanceMin, lstBalanceMax);
            txtSideColBalance.ToolTip = txtSideColBalance.Text;

            txtSameColNext.Text = CheckSameColumn(lstNextMin, lstNextMax);
            txtSameColNext.ToolTip = txtSameColNext.Text;
            txtSameColBalance.Text = CheckSameColumn(lstBalanceMin, lstBalanceMax);
            txtSameColBalance.ToolTip = txtSameColBalance.Text;

            using DataTable dtCompare = CreatCompareTable();
            for (int i = 0; i <= 4; i++)
            {
                DataRow drCompare = dtCompare.NewRow();
                //Panel pnlR = new Panel { ID = string.Format(InvariantCulture, "pnlR{0}", i) }; pnlResualt.Controls.Add(pnlR);
                for (int j = 0; j <= 4; j++)
                {
                    int FirstIndex = i + 2 + j;
                    FirstIndex = FirstIndex <= 4 ? FirstIndex : FirstIndex - 5;
                    FirstIndex = FirstIndex <= 4 ? FirstIndex : FirstIndex - 5;
                    //TextBox txtNextBalance = new TextBox
                    //{
                    //    ID = string.Format(InvariantCulture, "txtNextBalance{0}{1}", FirstIndex, j),
                    //    ReadOnly = true,
                    //    Width = 220
                    //};
                    //txtNextBalance.Text = Compare(lstNext[FirstIndex], lstBalance[j]);
                    //txtNextBalance.ToolTip = txtNextBalance.Text;
                    //pnlR.Controls.Add(txtNextBalance);
                    drCompare[string.Format(InvariantCulture, "Compare{0:d2}", j + 1)] = Compare(lstNext[FirstIndex], lstBalance[j]);
                }
                dtCompare.Rows.Add(drCompare);
            }
            ShowGvCompare(dtCompare);
            ShowGvSumCompare(dtCompare);
        }

        private string CheckSameColumn(List<string> lstMin, List<string> lstMax)
        {
            string strReturn = string.Empty;
            for (int i = 0; i < lstMin.Count; i++)
            {
                if (!string.IsNullOrEmpty(Compare(lstMin[i], lstMax[i])))
                {
                    strReturn += "," + Compare(lstMin[i], lstMax[i]);
                }
            }
            List<string> lstRetun = strReturn.Split(',').ToList();
            lstRetun.Sort();
            return lstRetun.Count > 0 ? string.Join(",", lstRetun.Distinct().ToArray()).Trim(',') : string.Empty;
        }

        private string CheckSideColumn(List<string> lstMin, List<string> lstMax)
        {
            string strReturn = string.Empty;
            for (int i = 0; i < lstMin.Count; i++)
            {
                int indexMin = i + 1 < lstMin.Count ? i + 1 : i + 1 - lstMin.Count;
                if (!string.IsNullOrEmpty(Compare(lstMin[indexMin], lstMax[i])))
                {
                    strReturn += "," + Compare(lstMin[indexMin], lstMax[i]);
                }
            }
            List<string> lstRetun = strReturn.Split(',').ToList();
            lstRetun.Sort();
            return lstRetun.Count > 0 ? string.Join(",", lstRetun.Distinct().ToArray()).Trim(',') : string.Empty;
        }

        private static List<string> CombindList(List<string> lstMin, List<string> lstMax)
        {
            List<string> lstCombind = new List<string>();
            for (int i = 0; i < lstMin.Count; i++)
            {
                if (!string.IsNullOrEmpty(lstMin[i]) && !string.IsNullOrEmpty(lstMax[i]))
                {
                    lstCombind.Add(lstMin[i] + "," + lstMax[i]);
                }
                if (string.IsNullOrEmpty(lstMin[i]) && string.IsNullOrEmpty(lstMax[i]))
                {
                    lstCombind.Add(string.Empty);
                }
                if (!string.IsNullOrEmpty(lstMin[i]) && string.IsNullOrEmpty(lstMax[i]))
                {
                    lstCombind.Add(lstMin[i]);
                }
                if (string.IsNullOrEmpty(lstMin[i]) && !string.IsNullOrEmpty(lstMax[i]))
                {
                    lstCombind.Add(lstMax[i]);
                }

            }
            return lstCombind;
        }

        private void ShowGvCompare(DataTable dtCompare)
        {
            GvCompare.DataSource = dtCompare.DefaultView;
            //if (GvCompare.Columns.Count == 0)
            //{
            //    for (int i = 0; i < dtCompare.Columns.Count; i++)
            //    {
            //        BoundField bfCell = new BoundField()
            //        {
            //            DataField = dtCompare.Columns[i].ColumnName,
            //            HeaderText = new CglFunc().ConvertFieldNameId(dtCompare.Columns[i].ColumnName, 1),
            //            SortExpression = dtCompare.Columns[i].ColumnName,
            //        };
            //        GvCompare.Columns.Add(bfCell);
            //    }
            //}
            GvCompare.DataBind();
        }

        private void ShowGvSumNext(List<string> lstNext)
        {
            DataTable dtNext = CountNums(lstNext);
            GvSumNext.DataSource = dtNext.DefaultView;
            GvSumNext.Columns.Clear();
            if (GvSumNext.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < dtNext.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = dtNext.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(dtNext.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = dtNext.Columns[ColumnIndex].ColumnName,
                    };
                    GvSumNext.Columns.Add(bfCell);
                }
            }
            foreach (DataControlField dcColumn in GvSumNext.Columns)
            {
                string strColumnName = dcColumn.SortExpression;
                if (_dicNumcssclass.Keys.Contains(int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)))
                {
                    dcColumn.HeaderStyle.CssClass = _dicNumcssclass[int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)];
                }
                else
                {
                    dcColumn.HeaderStyle.CssClass = "";
                }
            }
            GvSumNext.DataBind();
        }

        private void ShowGvSumBalance(List<string> lstBalance)
        {
            using DataTable dtBalance = CountNums(lstBalance);
            GvSumBalance.DataSource = dtBalance.DefaultView;
            GvSumBalance.Columns.Clear();
            if (GvSumBalance.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < dtBalance.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = dtBalance.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(dtBalance.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = dtBalance.Columns[ColumnIndex].ColumnName,
                    };
                    GvSumBalance.Columns.Add(bfCell);
                }
            }
            foreach (DataControlField dcColumn in GvSumBalance.Columns)
            {
                string strColumnName = dcColumn.SortExpression;
                if (_dicNumcssclass.Keys.Contains(int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)))
                {
                    dcColumn.HeaderStyle.CssClass = _dicNumcssclass[int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)];
                }
                else
                {
                    dcColumn.HeaderStyle.CssClass = "";
                }
            }
            GvSumBalance.DataBind();
        }

        private void ShowGvSumCompare(DataTable dtCompare)
        {
            Dictionary<string, int> dicNums = new Dictionary<string, int>();
            for (int index = 1; index <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; index++) { dicNums.Add(string.Format(InvariantCulture, "{0:d2}", index), 0); }
            foreach (DataRow dr in dtCompare.Rows)
            {
                foreach (DataColumn dc in dtCompare.Columns)
                {
                    if (!string.IsNullOrEmpty(dr[dc.ColumnName].ToString()))
                    {
                        foreach (string v in dr[dc.ColumnName].ToString().Split(','))
                        {
                            if (dicNums.ContainsKey(int.Parse(v, InvariantCulture).ToString("d2", InvariantCulture)))
                            {
                                dicNums[int.Parse(v, InvariantCulture).ToString("d2", InvariantCulture)]++;
                            }
                            else
                            {
                                dicNums.Add(int.Parse(v, InvariantCulture).ToString("d2", InvariantCulture), 1);
                            }
                        }
                    }
                }
            }

            //dicNums = dicNums.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            Dictionary<string, int> dicNumsSort = dicNums.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            using DataTable dtNums = new CglFunc().CDicTOTable(dicNumsSort, null);
            GvSumCompare.DataSource = dtNums.DefaultView;
            GvSumCompare.Columns.Clear();
            if (GvSumCompare.Columns.Count == 0)
            {
                for (int ColumnIndex = 0; ColumnIndex < dtNums.Columns.Count; ColumnIndex++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = dtNums.Columns[ColumnIndex].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(dtNums.Columns[ColumnIndex].ColumnName, 1),
                        SortExpression = dtNums.Columns[ColumnIndex].ColumnName,
                    };
                    GvSumCompare.Columns.Add(bfCell);
                }
            }
            foreach (DataControlField dcColumn in GvSumCompare.Columns)
            {
                string strColumnName = dcColumn.SortExpression;
                if (_dicNumcssclass.Keys.Contains(int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)))
                {
                    dcColumn.HeaderStyle.CssClass = _dicNumcssclass[int.Parse(strColumnName, InvariantCulture).ToString(InvariantCulture)];
                }
                else
                {
                    dcColumn.HeaderStyle.CssClass = "";
                }
            }
            GvSumCompare.DataBind();

        }

        private DataTable CountNums(List<string> lstInput)
        {
            Dictionary<string, int> dicNums = new Dictionary<string, int>();
            for (int index = 1; index <= new CglDataSet(_gstuSearch.LottoType).LottoNumbers; index++) { dicNums.Add(string.Format(InvariantCulture, "{0:d2}", index), 0); }
            foreach (string Nums in lstInput)
            {
                if (!string.IsNullOrEmpty(Nums))
                {
                    foreach (string num in Nums.Split(','))
                    {
                        if (dicNums.ContainsKey(int.Parse(num, InvariantCulture).ToString("d2", InvariantCulture)))
                        {
                            dicNums[int.Parse(num, InvariantCulture).ToString("d2", InvariantCulture)]++;
                        }
                        else
                        {
                            dicNums.Add(int.Parse(num, InvariantCulture).ToString("d2", InvariantCulture), 1);
                        }
                    }
                }
            }
            //dicNums = dicNums.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            Dictionary<string, int> dicNumsSort = dicNums.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            return new CglFunc().CDicTOTable(dicNumsSort, null);
        }

        private static DataTable CreatCompareTable()
        {
            using DataTable dtReturn = new DataTable();
            for (int i = 1; i <= 5; i++)
            {
                dtReturn.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "Compare{0:d2}", i),
                    Caption = string.Format(InvariantCulture, "Compare{0:d2}", i),
                    DataType = typeof(string),
                    AllowDBNull = true
                });
            }
            return dtReturn.Copy();
        }

        private string Compare(string strNext, string strBalance)
        {
            CheckData = true;
            if (!string.IsNullOrEmpty(strNext) && !string.IsNullOrEmpty(strBalance))
            {
                List<string> lstNext = strNext.Split(',').ToList();
                List<string> lstBalance = strBalance.Split(',').ToList();
                List<int> lstIntNext = new List<int>();
                foreach (string NumNext in lstNext) { lstIntNext.Add(int.Parse(NumNext, InvariantCulture)); }
                List<int> lstIntBalance = new List<int>();
                foreach (string NumBalance in lstBalance) { lstIntBalance.Add(int.Parse(NumBalance, InvariantCulture)); }
                List<string> Compare = new List<string>();
                foreach (int NumNext in lstIntNext)
                {
                    if (lstIntBalance.Contains(NumNext))
                    {
                        Compare.Add(string.Format(InvariantCulture, "{0:d2}", NumNext));
                    }
                }
                if (Compare.Count > 0)
                {
                    Compare.Sort();
                    return string.Join(",", Compare.Distinct().ToArray());
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        // ---------------------------------------------------------------------------------------------------------
    }
}