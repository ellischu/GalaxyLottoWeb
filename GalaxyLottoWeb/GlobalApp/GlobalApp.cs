using GalaxyLotto.ClassLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using MathNet.Numerics;
using System.Web.UI.WebControls;
using System.Xml;
using System.Data.SqlClient;
using System.Threading;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI;
using System.Globalization;

namespace GalaxyLottoWeb.GlobalApp
{

    [Serializable]
    public partial class GalaxyClass
    {
        public bool CheckData { get; set; }

        public static CultureInfo InvariantCulture { get; } = CultureInfo.InvariantCulture;

        public enum SoundOption : int { Start, Pause, Finish }
    }


    public partial class GalaxyApp : GalaxyClass
    {
        public void MediaPlay(SoundOption soundOption, double volumn)
        {
            CheckData = true;
            System.Windows.Media.MediaPlayer mediaPlayer = new System.Windows.Media.MediaPlayer();
            Uri uri = soundOption switch
            {
                SoundOption.Start => new Uri(@"D:\WebMedia\SoundStartup.mp3", UriKind.RelativeOrAbsolute),
                SoundOption.Pause => new Uri(@"D:\WebMedia\SounfPause.mp3", UriKind.RelativeOrAbsolute),
                SoundOption.Finish => new Uri(@"D:\WebMedia\SoundFinish.mp3", UriKind.RelativeOrAbsolute),
                _ => new Uri(@"D:\WebMedia\SoundStartup.mp3", UriKind.RelativeOrAbsolute),
            };
            mediaPlayer.Open(uri);
            mediaPlayer.Volume = volumn;
            mediaPlayer.Play();
        }

        public void MediaPlay(SoundOption soundOption)
        {
            MediaPlay(soundOption, 0.5);
        }

        public void PlaySound(SoundOption soundOption)
        {
            CheckData = true;
            using SoundPlayer player01 = soundOption switch
            {
                SoundOption.Start => new SoundPlayer(Properties.Resources.startup),
                SoundOption.Pause => new SoundPlayer(Properties.Resources.pause),
                SoundOption.Finish => new SoundPlayer(Properties.Resources.Finish),
                _ => new SoundPlayer(Properties.Resources.startup),
            };
            player01.Play();
        }

        public Table ToTable(DataTable dtInput, string strCaption)
        {
            CheckData = true;
            if (dtInput == null) { throw new ArgumentNullException(nameof(dtInput)); }

            using Table tblReturn = new Table { CssClass = "gltable" };
            if (!string.IsNullOrEmpty(strCaption))
            {
                tblReturn.Caption = strCaption;
            }
            using (TableHeaderRow thrHeader = new TableHeaderRow())
            {
                TableRow trRow = new TableRow();
                for (int i = 0; i < dtInput.Columns.Count; i++)
                {
                    thrHeader.Controls.Add(new TableHeaderCell
                    {
                        Text = new CglFunc().ConvertFieldNameId(dtInput.Columns[i].ColumnName, 1),
                        ID = dtInput.Columns[i].ColumnName,
                        CssClass = dtInput.Columns[i].ColumnName
                    });
                    TableCell tcCell = new TableCell();
                    if (dtInput.Columns[i].ColumnName.Substring(0, 4) == "lngL" || dtInput.Columns[i].ColumnName.Substring(0, 4) == "lngS")
                    {
                        tcCell.Text = string.Format(InvariantCulture, "{0:d2}", int.Parse(dtInput.Rows[0][i].ToString(), InvariantCulture));
                        tcCell.CssClass = dtInput.Columns[i].ColumnName;
                    }
                    else
                    {
                        tcCell.Text = string.Format(InvariantCulture, "{0}", dtInput.Rows[0][i].ToString());
                    }
                    trRow.Controls.Add(tcCell);
                }
                tblReturn.Controls.Add(thrHeader);
                tblReturn.Controls.Add(trRow);
            }
            return tblReturn;
        }

        public DataTable GetTable(DataTable dtInput, int intRows, string strPriColName, SortOrder TakeOrder, SortOrder ReturnOrder)
        {
            CheckData = true;
            if (dtInput is null) { throw new ArgumentNullException(nameof(dtInput)); }
            DataTable dtReturn;
            using DataTable dtTemp = dtInput.Select(string.Empty, string.Format(InvariantCulture, "[{0}] {1}",
                                                    strPriColName,
                                                    TakeOrder == SortOrder.Ascending ? "ASC" : TakeOrder == SortOrder.Descending ? "DESC" : string.Empty))
                                                    .CopyToDataTable();
            if (dtTemp.Rows.Count > 0 && intRows > 0)
            {
                using DataTable dtOutput = dtTemp.Rows.Cast<DataRow>().Take(intRows).CopyToDataTable();
                dtOutput.Select("", string.Format(InvariantCulture, "[{0}] {1}",
                                      strPriColName,
                                      ReturnOrder == SortOrder.Ascending ? "ASC" : ReturnOrder == SortOrder.Descending ? "DESC" : string.Empty))
                              .CopyToDataTable();

                dtReturn = dtOutput;
            }
            else
            {
                using DataTable table = new DataTable() { Locale = InvariantCulture };
                dtReturn = table;
            }
            return dtReturn;
        }

        public string GetTheadState(ThreadState threadState)
        {
            CheckData = true;
            return threadState switch
            {
                ThreadState.Aborted => "結束...",
                ThreadState.AbortRequested => "要求 結束...",
                ThreadState.Suspended => "暫止...",
                ThreadState.SuspendRequested => "要求 暫止...",
                ThreadState.Stopped => "停止...",
                ThreadState.StopRequested => "要求 停止...",
                ThreadState.Background => "背景執行",
                ThreadState.Running => "執行...",
                ThreadState.Unstarted => "已無法呼叫",
                ThreadState.WaitSleepJoin => "等待同步...",
                _ => string.Empty,
            };
        }

        // ---------------------------------------------------------------------------------------------------------

        public Panel CreatPanel(string strId)
        {
            CheckData = true;
            Panel pnlReturn;
            using (Panel panel = new Panel()) { panel.ID = strId; pnlReturn = panel; }
            return pnlReturn;
        }

        public DropDownList CreatDropDownList(string strId, bool autoPostBack)
        {
            CheckData = true;
            DropDownList Return;
            using (DropDownList dropDownList = new DropDownList())
            {
                dropDownList.ID = strId;
                dropDownList.AutoPostBack = autoPostBack;
                Return = dropDownList;
            }
            return Return;
        }

        public TextBox CreatTextBox(string strId, string strText, int width, bool autoPostBack)
        {
            CheckData = true;
            TextBox Return;
            using (TextBox textBox = new TextBox())
            {
                textBox.ID = strId;
                textBox.Text = strText;
                textBox.Width = width;
                textBox.AutoPostBack = autoPostBack;
                Return = textBox;
            }
            return Return;
        }

        public HyperLink CreatHyperLink(string strId, string strCss, string strText, string strNavigateUrl)
        {
            CheckData = true;
            return new HyperLink() { ID = strId, CssClass = strCss, Text = strText, NavigateUrl = strNavigateUrl };
        }

        public HyperLink CreatHyperLink(string strId, string strCss, string strText, Uri strNavigateUrl)
        {
            if (strNavigateUrl is null)
            {
                throw new ArgumentNullException(nameof(strNavigateUrl));
            }

            CheckData = true;
            return new HyperLink() { ID = strId, CssClass = strCss, Text = strText, NavigateUrl = strNavigateUrl.OriginalString };
        }

        public Label CreatLabel(string strId, string strText)
        {
            CheckData = true;
            return new Label()
            {
                ID = strId,
                Text = strText,
            };
        }

        public Label CreatLabel(string strId)
        {
            CheckData = true;
            return new Label()
            {
                ID = strId
            };
        }

        public Label CreatLabel(string strId, string strText, string strCss)
        {
            CheckData = true;
            return new Label()
            {
                ID = strId,
                Text = strText,
                CssClass = strCss
            };
        }

        public Button CreatButton(string strId, string strCss, string strText)
        {
            CheckData = true;
            return new Button
            {
                ID = strId,
                CssClass = strCss,
                Text = strText
            };
        }

        public Panel CreatPanel(string strId, string strCss)
        {
            CheckData = true;
            if (strId is null)
            {
                throw new ArgumentNullException(nameof(strId));
            }
            if (strCss is null)
            {
                throw new ArgumentNullException(nameof(strCss));
            }

            return new Panel() { ID = strId, CssClass = strCss };
        }

        /// <summary>
        /// Creat GridView
        /// </summary>
        /// <param name="strId">ID</param>
        /// <param name="strCss">CssClass</param>
        /// <param name="dtEachNum">DataSource</param>
        /// <param name="creatColumn">Is CreatColumn ?</param>
        /// <param name="creatCss">Is creatCss ?</param>
        /// <returns></returns>
        public GridView CreatGridView(string strId, string strCss, DataTable dtEachNum, bool creatColumn, bool creatCss)
        {
            CheckData = true;
            if (dtEachNum == null) { throw new ArgumentNullException(nameof(dtEachNum)); }

            GridView gridView = new GridView
            {
                AllowSorting = false,
                AutoGenerateColumns = false,
                CssClass = strCss,
                DataSource = dtEachNum.DefaultView,
                EnableSortingAndPagingCallbacks = false,
                EnableViewState = false,
                GridLines = GridLines.Horizontal,
                ID = strId,
                ViewStateMode = ViewStateMode.Disabled
            };
            #region Set Columns of DataGrid gvMissAll
            if (creatColumn)
            {
                if (gridView.Columns.Count == 0)
                {
                    for (int i = 0; i < dtEachNum.Columns.Count; i++)
                    {
                        string columnName = dtEachNum.Columns[i].ColumnName;
                        BoundField bfCell = new BoundField()
                        {
                            DataField = columnName,
                            HeaderText = new CglFunc().ConvertFieldNameId(dtEachNum.Columns[i].ColumnName, 1),
                            SortExpression = columnName,
                        };
                        bfCell.HeaderStyle.CssClass = creatCss ? columnName : "";
                        bfCell.ItemStyle.CssClass = creatCss ? columnName : "";
                        //string strColumnName = dtEachNum.Columns[i].ColumnName;
                        gridView.Columns.Add(bfCell);
                    }
                }
            }
            #endregion
            return gridView;
        }

        public Table CreatTable(string strCss, string strCaption)
        {
            CheckData = true;
            Table tblReturn;
            using Table table = new Table { CssClass = strCss, Caption = strCaption };
            tblReturn = table;
            return tblReturn;
        }

        public Chart CreatChart(string strId, int width, int height)
        {
            CheckData = true;
            Chart chChart = new Chart { ID = strId, Width = width, Height = height };
            chChart.Legends.Add(new Legend { IsTextAutoFit = true, Docking = Docking.Right });
            return chChart;
        }

        public ChartArea CreatChartArea(string strName)
        {
            double intAxisYMin = 0d;
            return CreatChartArea(strName, intAxisYMin);
        }

        public ChartArea CreatChartArea(string strName, double intAxisYMin)
        {
            CheckData = true;
            ChartArea chaChartArea = new ChartArea
            {
                Name = strName,
                //BorderDashStyle = dicNumcssclass.ContainsKey(intLNums.ToString()) ? ChartDashStyle.Dash : ChartDashStyle.Solid,
                BorderDashStyle = ChartDashStyle.Solid,
                BorderWidth = 1,
                BorderColor = System.Drawing.Color.Black,
                BackColor = System.Drawing.Color.White
            };
            chaChartArea.AxisX.Enabled = AxisEnabled.True;
            chaChartArea.AxisX.Interval = 10;
            chaChartArea.AxisX.LabelStyle.Angle = -45;
            chaChartArea.AxisX.LabelStyle.Enabled = true;
            chaChartArea.AxisX.LabelStyle.ForeColor = System.Drawing.Color.LightGray;
            chaChartArea.AxisX.LineColor = System.Drawing.Color.LightGray;
            chaChartArea.AxisX.InterlacedColor = System.Drawing.Color.LightGray;
            chaChartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;

            chaChartArea.AxisY.Enabled = AxisEnabled.True;
            chaChartArea.AxisY.LabelStyle.Enabled = true;
            chaChartArea.AxisY.LabelStyle.ForeColor = System.Drawing.Color.LightGray;
            chaChartArea.AxisY.LineColor = System.Drawing.Color.LightGray;
            chaChartArea.AxisY.InterlacedColor = System.Drawing.Color.LightGray;
            chaChartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chaChartArea.AxisY.Minimum = intAxisYMin != 0 ? intAxisYMin : chaChartArea.AxisY.Minimum;
            return chaChartArea;
        }

        //public Series CreatSeries(string strName, ChartArea chaChartArea)
        //{
        //    if (chaChartArea == null) { throw new ArgumentNullException(nameof(chaChartArea)); }
        //    Series sirReturn;
        //    SmartLabelStyle alasirNum = new SmartLabelStyle()
        //    {
        //        MovingDirection = LabelAlignmentStyles.Top,
        //        CalloutLineColor = Color.LightGray,
        //    };
        //    using (Series series = new Series())
        //    {
        //        series.Name = strName;
        //        series.ChartArea = chaChartArea.Name;
        //        series.ChartType = SeriesChartType.Spline;
        //        series.IsVisibleInLegend = true;
        //        series.MarkerStyle = MarkerStyle.Circle;
        //        series.MarkerSize = 2;
        //        series.MarkerColor = Color.Black;
        //        series.LabelForeColor = Color.LightGray;
        //        series.SmartLabelStyle = alasirNum;
        //        sirReturn = series;
        //    }
        //    return sirReturn;
        //}

        public Series CreatSeries(string strName, string strChartAreaName)
        {
            CheckData = true;
            return new Series
            {
                Name = strName,
                ChartArea = strChartAreaName,
                ChartType = SeriesChartType.Spline,
                IsVisibleInLegend = true,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 2,
                MarkerColor = System.Drawing.Color.Black,
                LabelForeColor = System.Drawing.Color.LightGray,
                SmartLabelStyle = new SmartLabelStyle()
                {
                    MovingDirection = LabelAlignmentStyles.Top,
                    CalloutLineColor = System.Drawing.Color.LightGray,
                }
            };
        }

        public HorizontalLineAnnotation CreatHorizontalLineAnnotation(string strName, ChartArea chaArea)
        {
            CheckData = true;
            if (chaArea == null) { throw new ArgumentNullException(nameof(chaArea)); }

            return new HorizontalLineAnnotation
            {
                Name = strName,
                ClipToChartArea = chaArea.Name,
                IsInfinitive = true,
                IsSizeAlwaysRelative = false,
                AxisX = chaArea.AxisX,
                AxisY = chaArea.AxisY,
                LineWidth = 1,
                LineDashStyle = ChartDashStyle.Dash,
                Visible = true
            };
        }

        public VerticalLineAnnotation CreatVerticalLineAnnotation(string strName, ChartArea chaArea)
        {
            CheckData = true;
            if (chaArea == null) { throw new ArgumentNullException(nameof(chaArea)); }

            return new VerticalLineAnnotation
            {
                Name = strName,
                ClipToChartArea = chaArea.Name,
                IsInfinitive = true,
                IsSizeAlwaysRelative = false,
                AxisX = chaArea.AxisX,
                AxisY = chaArea.AxisY,
                LineWidth = 1,
                LineDashStyle = ChartDashStyle.Dash,
                Visible = true
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="dtInput"></param>
        /// <param name="strColName">string[2]</param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public DataTable CreatPolynomialdt(string strTableName, DataTable dataTable, string[] strColName, int digits)
        {
            CheckData = true;
            if (dataTable == null) { throw new ArgumentNullException(nameof(dataTable)); }

            if (strColName is null) { throw new ArgumentNullException(nameof(strColName)); }
            DataTable dtReturn;
            using DataTable dtInput = dataTable.Select(string.Empty, "[lngTotalSN] ASC").CopyToDataTable();
            double[] dblX = new double[dtInput.Rows.Count];
            double[] dblY = new double[dtInput.Rows.Count];
            for (int intRow = 0; intRow < dtInput.Rows.Count; intRow++)
            {
                dblX[intRow] = intRow + 1;
                dblY[intRow] = double.Parse(dtInput.Rows[intRow][strColName[1]].ToString(), InvariantCulture);
            }
            int intPolyOrder = int.Parse((dtInput.Rows.Count / 10).ToString(InvariantCulture), InvariantCulture);
            Func<double, double> dblT = Fit.PolynomialFunc(x: dblX, y: dblY, order: intPolyOrder, method: MathNet.Numerics.LinearRegression.DirectRegressionMethod.QR);
            using DataTable dtPolynomial = new DataTable() { Locale = InvariantCulture, TableName = strTableName };
            dtPolynomial.Columns.Add(new DataColumn { ColumnName = strColName[0], DataType = typeof(double) });
            dtPolynomial.Columns.Add(new DataColumn { ColumnName = strColName[1], DataType = typeof(double) });
            for (int intRow = 0; intRow < dtInput.Rows.Count; intRow++)
            {
                DataRow drRow = dtPolynomial.NewRow();
                drRow[strColName[0]] = double.Parse(dtInput.Rows[intRow][strColName[0]].ToString(), InvariantCulture);
                drRow[strColName[1]] = Math.Round(dblT(intRow + 1), digits);
                dtPolynomial.Rows.Add(drRow);
            }
            dtReturn = dtPolynomial;
            return dtReturn;
        }

        public DataTable CreatKDdt(string strTableName, DataTable dtInput, string strColName, int RoundDigits, int intDayN, double AlphaCoefficient)
        {
            CheckData = true;
            if (dtInput is null) { throw new ArgumentNullException(nameof(dtInput)); }
            DataTable dtReturn;
            using DataTable dtKD = new DataTable() { Locale = InvariantCulture, TableName = strTableName };
            dtKD.Columns.Add(new DataColumn() { ColumnName = "lngTotalSN", DataType = typeof(long) });
            dtKD.PrimaryKey = new DataColumn[] { dtKD.Columns["lngTotalSN"] };
            dtKD.Columns.Add(new DataColumn() { ColumnName = "RSV", DataType = typeof(double) });
            dtKD.Columns.Add(new DataColumn() { ColumnName = "K", DataType = typeof(double) });
            dtKD.Columns.Add(new DataColumn() { ColumnName = "D", DataType = typeof(double) });
            dtKD.Columns.Add(new DataColumn() { ColumnName = "K-D", DataType = typeof(double) });
            dtKD.Columns.Add(new DataColumn() { ColumnName = "AKD", DataType = typeof(double) });
            for (int rowIndex = 0; rowIndex < dtInput.Select(string.Empty, "[lngTotalSN] ASC").CopyToDataTable().Rows.Count; rowIndex++)
            {
                DataRow drKD = dtKD.NewRow();
                drKD["lngTotalSN"] = dtInput.Select(string.Empty, "[lngTotalSN] ASC").CopyToDataTable().Rows[rowIndex]["lngTotalSN"];
                if (rowIndex == 0)
                {
                    drKD["RSV"] = Math.Round(100d, RoundDigits);
                    drKD["K"] = Math.Round(50d, RoundDigits);
                    drKD["D"] = Math.Round(50d, RoundDigits);
                    drKD["K-D"] = Math.Round(0d, RoundDigits);
                    drKD["AKD"] = Math.Round(0d, RoundDigits);
                }
                else
                {
                    string filter = string.Format(InvariantCulture, "[lngTotalSN] >= {0}  AND [lngTotalSN] < {1}", rowIndex - intDayN < 0 ? dtInput.Select(string.Empty, "[lngTotalSN] ASC").CopyToDataTable().Rows[0]["lngTotalSN"] : dtInput.Select(string.Empty, "[lngTotalSN] ASC").CopyToDataTable().Rows[rowIndex - intDayN]["lngTotalSN"], dtInput.Select(string.Empty, "[lngTotalSN] ASC").CopyToDataTable().Rows[rowIndex]["lngTotalSN"]);
                    string min = dtInput.Select(string.Empty, "[lngTotalSN] ASC").CopyToDataTable().Compute(string.Format(InvariantCulture, "MIN([{0}])", strColName),
                                                               filter).ToString();
                    double Min = double.Parse(string.IsNullOrEmpty(min) ? dtInput.Select(string.Empty, "[lngTotalSN] ASC").CopyToDataTable().Rows[rowIndex - 1][strColName].ToString() : min, InvariantCulture);
                    string max = dtInput.Select(string.Empty, "[lngTotalSN] ASC").CopyToDataTable().Compute(string.Format(InvariantCulture, "MAX([{0}])", strColName),
                                                               filter).ToString();
                    double Max = double.Parse(string.IsNullOrEmpty(max) ? dtInput.Select(string.Empty, "[lngTotalSN] ASC").CopyToDataTable().Rows[rowIndex - 1][strColName].ToString() : max, InvariantCulture);
                    double dblCurrent = double.Parse(dtInput.Select(string.Empty, "[lngTotalSN] ASC").CopyToDataTable().Rows[rowIndex][strColName].ToString(), InvariantCulture);
                    double dblRSV = Max - Min > 0 ? (dblCurrent - Min) / (Max - Min) * 100d : double.Parse(dtKD.Rows[rowIndex - 1]["RSV"].ToString(), InvariantCulture);
                    double dblK = (1 - AlphaCoefficient) * (double.Parse(dtKD.Rows[rowIndex - 1]["K"].ToString(), InvariantCulture)) + AlphaCoefficient * dblRSV;
                    double dblD = (1 - AlphaCoefficient) * (double.Parse(dtKD.Rows[rowIndex - 1]["D"].ToString(), InvariantCulture)) + AlphaCoefficient * dblK;

                    drKD["RSV"] = Math.Round(dblRSV, RoundDigits);
                    drKD["K"] = Math.Round(dblK, RoundDigits);
                    drKD["D"] = Math.Round(dblD, RoundDigits);
                    drKD["K-D"] = Math.Round(dblK - dblD, RoundDigits);
                    drKD["AKD"] = Math.Round(Math.Abs(dblK - dblD), RoundDigits);
                }
                dtKD.Rows.Add(drKD);
            }
            dtReturn = dtKD;
            return dtReturn;
        }

        public DataTable GetDataKDCoeficient(DataTable dtInput, string strTableName, string ddlNumsSelectedValue, int intDays, int RoundDigits, double AlphaCoefficient)
        {
            DataTable dtReturn;
            using DataTable dtKDC = new DataTable() { Locale = InvariantCulture, TableName = strTableName };
            #region Add ColumnsKD Coefficient                
            dtKDC.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "lngTotalSN"),
                DataType = typeof(long)
            });//lngTotalSN
            dtKDC.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "atRSV{0}", ddlNumsSelectedValue),
                DataType = typeof(double)
            });//atRSV
            dtKDC.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "atK{0}", ddlNumsSelectedValue),
                DataType = typeof(double)
            });//atK
            dtKDC.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "atD{0}", ddlNumsSelectedValue),
                DataType = typeof(double)
            });//atD
            dtKDC.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "atK-D{0}", ddlNumsSelectedValue),
                DataType = typeof(double)
            });//atK-D
            dtKDC.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "avRSV{0}", ddlNumsSelectedValue),
                DataType = typeof(double)
            });//avRSV
            dtKDC.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "avK{0}", ddlNumsSelectedValue),
                DataType = typeof(double)
            });//avK
            dtKDC.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "avD{0}", ddlNumsSelectedValue),
                DataType = typeof(double)
            });//avD
            dtKDC.Columns.Add(new DataColumn
            {
                ColumnName = string.Format(InvariantCulture, "avK-D{0}", ddlNumsSelectedValue),
                DataType = typeof(double)
            });//avK-D
            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
            {
                dtKDC.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNumsSelectedValue, section),
                    DataType = typeof(double)
                });//aRSV
                dtKDC.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNumsSelectedValue, section),
                    DataType = typeof(double)
                });//aK
                dtKDC.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNumsSelectedValue, section),
                    DataType = typeof(double)
                });//aD
                dtKDC.Columns.Add(new DataColumn
                {
                    ColumnName = string.Format(InvariantCulture, "aK-D{0}{1:d2}", ddlNumsSelectedValue, section),
                    DataType = typeof(double)
                });//aK-D
            }
            dtKDC.PrimaryKey = new DataColumn[] { dtKDC.Columns["lngTotalSN"] };
            #endregion Add ColumnsKD Coefficient

            foreach (DataRow drKD in CreatKDdt("dtKDAvgT", dtInput,
                                               string.Format(InvariantCulture, "sglAvgT{0}", ddlNumsSelectedValue),
                                               RoundDigits, intDays, AlphaCoefficient).Select(string.Empty, "lngTotalSN DESC").CopyToDataTable().Rows)
            {
                DataRow drReturn = dtKDC.NewRow();
                drReturn[string.Format(InvariantCulture, "lngTotalSN")] = drKD["lngTotalSN"];
                drReturn[string.Format(InvariantCulture, "atRSV{0}", ddlNumsSelectedValue)] = drKD["RSV"];
                drReturn[string.Format(InvariantCulture, "atK{0}", ddlNumsSelectedValue)] = drKD["K"];
                drReturn[string.Format(InvariantCulture, "atD{0}", ddlNumsSelectedValue)] = drKD["D"];
                drReturn[string.Format(InvariantCulture, "atK-D{0}", ddlNumsSelectedValue)] = drKD["K-D"];
                dtKDC.Rows.Add(drReturn);
            }

            foreach (DataRow drKD in CreatKDdt("dtKDAvg", dtInput,
                                               string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelectedValue),
                                               RoundDigits, intDays, AlphaCoefficient).Rows)
            {
                DataRow drReturn = dtKDC.Rows.Find(drKD["lngTotalSN"]);
                drReturn[string.Format(InvariantCulture, "avRSV{0}", ddlNumsSelectedValue)] = drKD["RSV"];
                drReturn[string.Format(InvariantCulture, "avK{0}", ddlNumsSelectedValue)] = drKD["K"];
                drReturn[string.Format(InvariantCulture, "avD{0}", ddlNumsSelectedValue)] = drKD["D"];
                drReturn[string.Format(InvariantCulture, "avK-D{0}", ddlNumsSelectedValue)] = drKD["K-D"];
            }


            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
            {
                foreach (DataRow drKD in CreatKDdt("dtKDAvgs", dtInput, string.Format(InvariantCulture, "sglAvg{0}{1:d2}",
                                                   ddlNumsSelectedValue, section), RoundDigits, intDays, AlphaCoefficient).Rows)
                {
                    DataRow drReturn = dtKDC.Rows.Find(drKD["lngTotalSN"]);
                    drReturn[string.Format(InvariantCulture, "aRSV{0}{1:d2}", ddlNumsSelectedValue, section)] = drKD["RSV"];
                    drReturn[string.Format(InvariantCulture, "aK{0}{1:d2}", ddlNumsSelectedValue, section)] = drKD["K"];
                    drReturn[string.Format(InvariantCulture, "aD{0}{1:d2}", ddlNumsSelectedValue, section)] = drKD["D"];
                    drReturn[string.Format(InvariantCulture, "aK-D{0}{1:d2}", ddlNumsSelectedValue, section)] = drKD["K-D"];
                }
            }
            dtReturn = dtKDC;
            return dtReturn;
        }

        public DataTable GetDataKDRange(DataTable dtInput, string strTableName, string Percent)
        {
            if (dtInput is null) { throw new ArgumentNullException(nameof(dtInput)); }
            DataTable dtReturn;
            using DataTable dtDataKDRange = new DataTable { Locale = InvariantCulture, TableName = strTableName };
            #region Column
            foreach (DataColumn dcBase in dtInput.Columns)
            {
                if (dcBase.ColumnName != "lngTotalSN")
                {
                    dtDataKDRange.Columns.Add(new DataColumn
                    {
                        ColumnName = string.Format(InvariantCulture, "{0}", dcBase.ColumnName),
                        DataType = typeof(double)
                    });//ColumnName
                    dtDataKDRange.Columns.Add(new DataColumn
                    {
                        ColumnName = string.Format(InvariantCulture, "{0}Max", dcBase.ColumnName),
                        DataType = typeof(double)
                    });//Max
                    dtDataKDRange.Columns.Add(new DataColumn
                    {
                        ColumnName = string.Format(InvariantCulture, "{0}Min", dcBase.ColumnName),
                        DataType = typeof(double)
                    });//Min
                }
            }
            #endregion Column

            #region Import Data
            DataRow drRow = dtDataKDRange.NewRow();

            foreach (DataColumn dcBase in dtInput.Columns)
            {
                if (dcBase.ColumnName != "lngTotalSN")
                {
                    double CurrentValue = double.Parse(dtInput.Rows[0][dcBase.ColumnName].ToString(), InvariantCulture);
                    List<double> lstRange = (List<double>)CglFunc.GetRangeSD(GetFieldLst(dtInput, dcBase.ColumnName), double.Parse(Percent, InvariantCulture));
                    drRow[dcBase.ColumnName] = dtInput.Rows[0][dcBase.ColumnName];
                    drRow[string.Format(InvariantCulture, "{0}Min", dcBase.ColumnName)] = dcBase.ColumnName.Contains("Gap") ? CurrentValue - Math.Abs(lstRange[0]) : lstRange[0];
                    drRow[string.Format(InvariantCulture, "{0}Max", dcBase.ColumnName)] = dcBase.ColumnName.Contains("Gap") ? CurrentValue + Math.Abs(lstRange[1]) : lstRange[1];
                }
            }
            dtDataKDRange.Rows.Add(drRow);

            #endregion Import Data
            dtReturn = dtDataKDRange;
            return dtReturn;
        }

        public DataRow GetNextNumKDCoeficient(StuGLSearch stuGLSearch, DataTable dtDataTable, DataRow drEachNumNext, string strColName, int RoundDigits, int intDayN, double AlphaCoefficient)
        {
            if (drEachNumNext is null) { throw new ArgumentNullException(nameof(drEachNumNext)); }
            DataRow drReturn;
            using DataView dvDataView = new DataView(dtDataTable);
            string[] selectedColumns = new[] { "lngTotalSN", strColName };
            using DataTable dtAvg = dvDataView.ToTable(false, selectedColumns);
            DataRow drtemp = dtAvg.NewRow();
            drtemp["lngTotalSN"] = stuGLSearch.LngTotalSN;
            drtemp[strColName] = drEachNumNext[strColName];
            dtAvg.Rows.Add(drtemp);
            using DataTable dtsglAvgTKD = CreatKDdt("dtKD", dtAvg, strColName, RoundDigits, intDayN, AlphaCoefficient);
            drReturn = dtsglAvgTKD.Rows[dtsglAvgTKD.Rows.Count - 1];
            return drReturn;

        }

        /// <summary>
        /// DataTable with lngTotalSN DESC
        /// </summary>
        /// <param name="dtInput"></param>
        /// <param name="ddlNumsSelectedValue"></param>
        /// <param name="intDays">RSV Days</param>
        /// <param name="RoundDigits"></param>
        /// <param name="AlphaCoefficient"></param>
        /// <returns></returns>

        public Title CreatTitle(string strName, string strText, Docking docking, System.Drawing.Color Fcolor, System.Drawing.Color Bcolor)
        {
            CheckData = true;
            return new Title
            {
                Name = strName,
                Text = strText,
                Docking = docking,
                ForeColor = Fcolor,
                BackColor = Bcolor
            };
        }

        public List<double> GetFieldLst(DataTable dtInput, string strColumnName)
        {
            CheckData = true;
            if (dtInput is null) { throw new ArgumentNullException(nameof(dtInput)); }

            List<double> lstReturn = new List<double>();
            foreach (DataRow dr in dtInput.Rows)
            {
                lstReturn.Add(double.Parse(dr[strColumnName].ToString(), InvariantCulture));
            }
            return lstReturn;
        }

        // ---------------------------------------------------------------------------------------------------------
        public double GetASP(DataRow drEachNumNext, string ddlNumsSelectedValue)
        {
            CheckData = true;
            if (drEachNumNext is null) { throw new ArgumentNullException(nameof(drEachNumNext)); }

            double ASPTemp = 0;
            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
            {
                ASPTemp += Math.Pow(
                  double.Parse(drEachNumNext[string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelectedValue, section)].ToString(), InvariantCulture)
                - double.Parse(drEachNumNext[string.Format(InvariantCulture, "sglAvg{0}", ddlNumsSelectedValue)].ToString(), InvariantCulture)
                    , 2);
            }
            return Math.Round(Math.Sqrt(ASPTemp / 5), 3);
        }

        public double GetASP1(DataRow drEachNumNext, string ddlNumsSelectedValue, DataRow drLastDataRow)
        {
            CheckData = true;
            if (drEachNumNext is null) { throw new ArgumentNullException(nameof(drEachNumNext)); }

            if (drLastDataRow is null) { throw new ArgumentNullException(nameof(drLastDataRow)); }

            double ASP1Temp = 0;
            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
            {
                ASP1Temp +=
                  (double.Parse(drEachNumNext[string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelectedValue, section)].ToString(), InvariantCulture)
                - double.Parse(drLastDataRow[string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelectedValue, section)].ToString(), InvariantCulture))
                * double.Parse((section / 5).ToString(InvariantCulture), InvariantCulture);
            }
            return Math.Round(ASP1Temp, 3);
        }

        public string ConvertNum(StuGLSearch stuGLSearch, string Num)
        {
            CheckData = true;
            int intNum = int.Parse(Num, InvariantCulture);
            if (intNum < 1) { return string.Format(InvariantCulture, "{0}", intNum + new CglDataSet(stuGLSearch.LottoType).LottoNumbers); }
            if (intNum > new CglDataSet(stuGLSearch.LottoType).LottoNumbers) { return string.Format(InvariantCulture, "{0}", intNum - new CglDataSet(stuGLSearch.LottoType).LottoNumbers); }
            return Num;
        }

        public string ConvertNum(StuGLSearch stuGLSearch, int intNum)
        {
            CheckData = true;
            if (intNum < 1) { return string.Format(InvariantCulture, "{0}", intNum + new CglDataSet(stuGLSearch.LottoType).LottoNumbers); }
            if (intNum > new CglDataSet(stuGLSearch.LottoType).LottoNumbers) { return string.Format(InvariantCulture, "{0}", intNum - new CglDataSet(stuGLSearch.LottoType).LottoNumbers); }
            return string.Format(InvariantCulture, "{0}", intNum);
        }

        public DataTable GetDataRange(DataTable dtInput, string strTableName, string strNums, string Percent)
        {
            if (dtInput is null) { throw new ArgumentNullException(nameof(dtInput)); }
            int Nums = int.Parse(strNums, InvariantCulture);
            #region need columns
            List<string> lstNeedCol = new List<string>
                            {
                                "lngTotalSN",
                                string.Format(InvariantCulture,"sglAvgT{0}",Nums),
                                string.Format(InvariantCulture,"sglAvgT{0}Gap",Nums),
                                string.Format(InvariantCulture,"sglStdEvpT{0}",Nums),
                                string.Format(InvariantCulture,"sglStdEvpT{0}Gap",Nums),
                                string.Format(InvariantCulture,"sglAvg{0}",Nums),
                                string.Format(InvariantCulture,"sglAvg{0}Gap",Nums),
                                string.Format(InvariantCulture,"sglStdEvp{0}",Nums),
                                string.Format(InvariantCulture,"sglStdEvp{0}Gap",Nums),
                            };
            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
            {
                lstNeedCol.Add(string.Format(InvariantCulture, "sglAvg{0}{1:d2}", Nums, section));
                lstNeedCol.Add(string.Format(InvariantCulture, "sglAvg{0}{1:d2}Gap", Nums, section));
                lstNeedCol.Add(string.Format(InvariantCulture, "sglStdEvp{0}{1:d2}", Nums, section));
                lstNeedCol.Add(string.Format(InvariantCulture, "sglStdEvp{0}{1:d2}Gap", Nums, section));
            }
            #endregion need columns

            using DataView dvView = new DataView(dtInput.Select(string.Empty, "lngTotalSN DESC").CopyToDataTable());
            using DataTable dtBase = dvView.ToTable(false, lstNeedCol.ToArray());
            using DataTable dtReturn = new DataTable() { Locale = InvariantCulture, TableName = strTableName };
            #region Column
            foreach (DataColumn dcBase in dtBase.Columns)
            {
                if (dcBase.ColumnName != "lngTotalSN")
                {
                    dtReturn.Columns.Add(new DataColumn
                    {
                        ColumnName = string.Format(InvariantCulture, "{0}", dcBase.ColumnName),
                        DataType = typeof(double)
                    });//ColumnName
                    dtReturn.Columns.Add(new DataColumn
                    {
                        ColumnName = string.Format(InvariantCulture, "{0}Max", dcBase.ColumnName),
                        DataType = typeof(double)
                    });//Max
                    dtReturn.Columns.Add(new DataColumn
                    {
                        ColumnName = string.Format(InvariantCulture, "{0}Min", dcBase.ColumnName),
                        DataType = typeof(double)
                    });//Min
                }
            }
            #endregion Column

            #region Import Data
            DataRow drRow = dtReturn.NewRow();
            foreach (DataColumn dcBase in dtBase.Columns)
            {
                if (dcBase.ColumnName != "lngTotalSN")
                {
                    double CurrentValue = dcBase.ColumnName.Contains("Gap") ?
                        double.Parse(dtBase.Rows[0][dcBase.ColumnName.TrimEnd(new char[] { 'G', 'a', 'p' })].ToString(), InvariantCulture) :
                        double.Parse(dtBase.Rows[0][dcBase.ColumnName].ToString(), InvariantCulture);
                    List<double> lstRange = (List<double>)CglFunc.GetRangeSD(GetFieldLst(dtInput, dcBase.ColumnName), double.Parse(Percent, InvariantCulture));
                    drRow[dcBase.ColumnName] = dtBase.Rows[0][dcBase.ColumnName];
                    drRow[string.Format(InvariantCulture, "{0}Min", dcBase.ColumnName)] = dcBase.ColumnName.Contains("Gap") ? CurrentValue - Math.Abs(lstRange[0]) : lstRange[0];
                    drRow[string.Format(InvariantCulture, "{0}Max", dcBase.ColumnName)] = dcBase.ColumnName.Contains("Gap") ? CurrentValue + Math.Abs(lstRange[1]) : lstRange[1];
                }
            }
            dtReturn.Rows.Add(drRow);

            #endregion Import Data

            return dtReturn;
        }


        public double GetMinAbs(DataTable dataTable, string strCell_ColumnName)
        {
            CheckData = true;
            if (dataTable is null) { throw new ArgumentNullException(nameof(dataTable)); }

            if (strCell_ColumnName is null) { throw new ArgumentNullException(nameof(strCell_ColumnName)); }

            double dblreturn;
            double max = double.Parse(dataTable.Compute(string.Format(InvariantCulture, "Max([{0}])", strCell_ColumnName),
                                                        string.Format(InvariantCulture, "[{0}] < 0 ", strCell_ColumnName)).ToString(),
                                      InvariantCulture);
            double min = double.Parse(dataTable.Compute(string.Format(InvariantCulture, "Min([{0}])", strCell_ColumnName),
                                                        string.Format(InvariantCulture, "[{0}] >= 0 ", strCell_ColumnName)).ToString(),
                                      InvariantCulture);
            dblreturn = Math.Abs(min) < Math.Abs(max) ? min : max;
            return dblreturn;
        }

        public DataTable GetEachDataGap(DataTable dtInput, string strTableName, string ddlNumsSelectedValue)
        {
            CheckData = true;
            if (dtInput is null) { throw new ArgumentNullException(nameof(dtInput)); }
            List<string> lstColDataN = new List<string>
                            {
                                "lngTotalSN",   "lngMethodSN",  "lngDateSN",
                                //"intSum",
                                //string.Format(InvariantCulture,"lngN{0}",ddlNumsSelectedValue),
                                string.Format(InvariantCulture,"sglAvgT{0}",ddlNumsSelectedValue),
                                string.Format(InvariantCulture,"sglAvgT{0}Gap",ddlNumsSelectedValue),
                                string.Format(InvariantCulture,"sglStdEvpT{0}",ddlNumsSelectedValue),
                                string.Format(InvariantCulture,"sglStdEvpT{0}Gap",ddlNumsSelectedValue),
                                //string.Format(InvariantCulture,"sglDisT{0}",selectedValue2),
                                string.Format(InvariantCulture,"sglAvg{0}",ddlNumsSelectedValue),
                                string.Format(InvariantCulture,"sglAvg{0}Gap",ddlNumsSelectedValue),
                                string.Format(InvariantCulture,"sglStdEvp{0}",ddlNumsSelectedValue),
                                string.Format(InvariantCulture,"sglStdEvp{0}Gap",ddlNumsSelectedValue),
                                //string.Format(InvariantCulture,"intMin{0}",selectedValue2),
                                //string.Format(InvariantCulture,"intMax{0}",selectedValue2),
                            };
            foreach (int section in new List<int>() { 5, 10, 25, 50, 100 })
            {
                lstColDataN.Add(string.Format(InvariantCulture, "sglAvg{0}{1:d2}", ddlNumsSelectedValue, section));
                lstColDataN.Add(string.Format(InvariantCulture, "sglAvg{0}{1:d2}Gap", ddlNumsSelectedValue, section));
                lstColDataN.Add(string.Format(InvariantCulture, "sglStdEvp{0}{1:d2}", ddlNumsSelectedValue, section));
                lstColDataN.Add(string.Format(InvariantCulture, "sglStdEvp{0}{1:d2}Gap", ddlNumsSelectedValue, section));
                //lstColDataN.Add(string.Format(InvariantCulture, "intMin{0}{1:d2}", selectedValue2, section));
                //lstColDataN.Add(string.Format(InvariantCulture, "intMax{0}{1:d2}", selectedValue2, section));
            }
            using DataView dvDataView = new DataView(dtInput);
            using DataTable dtReturn = dvDataView.ToTable(false, lstColDataN.ToArray());
            dtReturn.Locale = InvariantCulture;
            dtReturn.TableName = strTableName;
            return dtReturn;
        }
        // ---------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Creat Static Html Page
        /// </summary>
        /// <param name="targetUrl"></param>
        /// <param name="htmlFilePathName"></param>
        /// <param name="timeout"></param>
        public void CreatStaticHtml(Uri targetUrl, string htmlFilePathName, int timeout)
        {
            CheckData = true;
            timeout = timeout == 0 ? 30000 : timeout;
            string strResult = new CglFunc().GetPage(targetUrl, null, timeout);
            using FileStream htmFile = new FileStream(htmlFilePathName, FileMode.Create, FileAccess.ReadWrite);
            using StreamWriter swWriter = new StreamWriter(htmFile);
            swWriter.Write(strResult);
            //htmFile.Close();
        }

        public string ShowBriefDate(DataTable dtCurrentData, List<int> lstCurrentNumsN)
        {
            CheckData = true;
            if (dtCurrentData is null)
            {
                throw new ArgumentNullException(nameof(dtCurrentData));
            }

            if (lstCurrentNumsN is null)
            {
                throw new ArgumentNullException(nameof(lstCurrentNumsN));
            }

            string strDateSN = dtCurrentData.Rows[0]["lngDateSN"].ToString();
            string strWeek = string.Empty;
            switch (new DateTime(int.Parse(strDateSN.Substring(startIndex: 0, length: 4), InvariantCulture),
                                 int.Parse(strDateSN.Substring(startIndex: 4, length: 2), InvariantCulture),
                                 int.Parse(strDateSN.Substring(startIndex: 6, length: 2), InvariantCulture)).DayOfWeek)
            {
                case DayOfWeek.Monday:
                    strWeek = "一";
                    break;
                case DayOfWeek.Tuesday:
                    strWeek = "二";
                    break;
                case DayOfWeek.Wednesday:
                    strWeek = "三";
                    break;
                case DayOfWeek.Thursday:
                    strWeek = "四";
                    break;
                case DayOfWeek.Friday:
                    strWeek = "五";
                    break;
                case DayOfWeek.Saturday:
                    strWeek = "六";
                    break;
                case DayOfWeek.Sunday:
                    strWeek = "日";
                    break;
            }
            string strNums = string.Empty;
            string strNumsSum = string.Empty;
            if (lstCurrentNumsN.Sum() > 0)
            {
                List<string> lstCNum = new List<string>();
                foreach (var v in lstCurrentNumsN) { lstCNum.Add(string.Format(InvariantCulture, "{0:d2}", v)); }
                strNums = string.Format(InvariantCulture, " => {0}", string.Join(",", lstCNum.ToArray()));
                strNumsSum = string.Format(InvariantCulture, "[{0}]", lstCurrentNumsN.Sum());
            }
            return string.Format(InvariantCulture, "{0}({1}){2}{3}", strDateSN, strWeek, strNums, strNumsSum);
        }

        // ---------------------------------------------------------------------------------------------------------

        public Dictionary<string, string> GetNumcssclass(StuGLSearch stuGLSearch)
        {
            CheckData = true;
            Dictionary<string, int> dicCurrentNums = new CglData().GetDataNumsDici(stuGLSearch);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (KeyValuePair<string, int> KeyPair in dicCurrentNums)
            {
                if (KeyPair.Value > 0)
                {
                    dictionary.Add(KeyPair.Value.ToString(InvariantCulture), KeyPair.Key);
                }
            }
            return dictionary;
        }

        public List<int> GetLstNumbers(StuGLSearch stuGLSearch, DataTable dtDataTable, int rowIndex)
        {
            CheckData = true;
            if (dtDataTable == null) { throw new ArgumentNullException(nameof(dtDataTable)); }
            StuGLSearch stuSearchTemp = stuGLSearch;
            if (dtDataTable.Rows.Count >= rowIndex)
            {
                stuSearchTemp.LngTotalSN = long.Parse(dtDataTable.Rows[index: rowIndex - 1][columnName: "lngTotalSN"].ToString(), InvariantCulture);
            }
            else
            {
                stuSearchTemp.LngTotalSN = long.Parse(dtDataTable.Rows[index: dtDataTable.Rows.Count - 1][columnName: "lngTotalSN"].ToString(), InvariantCulture);
            }
            return (List<int>)new CglData().GetDataNumsLst(stuSearchTemp);
        }

        public string CheckArgument(StuGLSearch stuGLSearch)
        {
            CheckData = true;
            StringBuilder sbArgument = new StringBuilder();
            sbArgument.AppendLine("<ul>");
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> LottoType: {0} </li>", stuGLSearch.LottoType.ToString()));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> StrCompareType: {0} </li>", stuGLSearch.StrCompareType));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> BoolFieldMode: {0} </li>", stuGLSearch.FieldMode));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> BoolNextNumsMode: {0} </li>", stuGLSearch.NextNumsMode));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> BoolGeneralMode: {0} </li>", stuGLSearch.GeneralMode));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> BoolPeriodMode: {0} </li>", stuGLSearch.PeriodMode));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> BoolRecalc: {0} </li>", stuGLSearch.Recalculate));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> LngDataEnd: {0} </li>", stuGLSearch.LngDataEnd));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> IntDataLimit: {0} </li>", stuGLSearch.InDataLimit));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> IntDataOffset: {0} </li>", stuGLSearch.InDataOffset));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> IntSearchLimit: {0} </li>", stuGLSearch.InSearchLimit));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> IntSearchOffset: {0} </li>", stuGLSearch.InSearchOffset));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> IntTestTimes: {0} </li>", stuGLSearch.InTestPeriods));

            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> StrCompares: {0} </li>", stuGLSearch.StrCompares));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> StrComparesDetail: {0} </li>", stuGLSearch.StrComparesDetail));

            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> IntNextNums: {0} </li>", stuGLSearch.InNextNums));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> IntNextStep: {0} </li>", stuGLSearch.InNextStep));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> StrNextNumT: {0} </li>", stuGLSearch.StrNextNumT));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> StrNextNums: {0} </li>", stuGLSearch.StrNextNums));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> StrNextNumSpe: {0} </li>", stuGLSearch.StrNextNumSpe));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> chkShowProcess: {0} </li>", stuGLSearch.ShowProcess.ToString()));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> chkshowGraphic: {0} </li>", stuGLSearch.ShowGraphic.ToString()));
            sbArgument.AppendLine(string.Format(InvariantCulture, "<li> LngMethodSN: {0} </li>", stuGLSearch.LngMethodSN));
            sbArgument.AppendLine("</ul>");
            return sbArgument.ToString();
        }

        // ---------------------------------------------------------------------------------------------------------

    }

    /// <summary>
    /// This class skips all nodes which has some kind of prefix. This trick does the job 
    /// to clean up MS Word/Outlook HTML markups.
    /// </summary>
    public class HtmlReader : Sgml.SgmlReader
    {
        public HtmlReader(TextReader reader) : base()
        {
            InputStream = reader;
            DocType = "HTML";
        }

        public HtmlReader(string content) : base()
        {
            InputStream = new StringReader(content);
            DocType = "HTML";
        }

        public override bool Read()
        {
            bool status = Read();
            if (status)
            {
                if (NodeType == XmlNodeType.Element)
                {
                    // Got a node with prefix. This must be one of those "<o:p>" or something else.
                    // Skip this node entirely. We want prefix less nodes so that the resultant XML 
                    // requires not namespace.
                    if (Name.IndexOf(':') > 0)
                    {
                        Skip();
                    }
                }
            }
            return status;
        }
    }

    /// <summary>
    /// Extends XmlTextWriter to provide Html writing feature which is not as strict as Xml
    /// writing. For example, Xml Writer encodes content passed to WriteString which encodes special markups like
    /// &nbsp to &amp;bsp. So, WriteString is bypassed by calling WriteRaw.
    /// </summary>
    public class HtmlWriter : XmlTextWriter
    {
        /// <summary>
        /// If set to true, it will filter the output by using tag and attribute filtering,
        /// space reduce etc
        /// </summary>
        public bool FilterOutput { get => _filterOutput; set => _filterOutput = value; }
        private bool _filterOutput;

        /// <summary>
        /// If true, it will reduce consecutive &nbsp; with one instance
        /// </summary>
        public bool ReduceConsecutiveSpace { get => _reduceConsecutiveSpace; set => _reduceConsecutiveSpace = value; }
        private bool _reduceConsecutiveSpace = true;

        /// <summary>
        /// Set the tag names in lower case which are allowed to go to output
        /// </summary>
        public string[] GetAllowedTags()
        {
            return _allowedTags;
        }

        /// <summary>
        /// Set the tag names in lower case which are allowed to go to output
        /// </summary>
        public void SetAllowedTags(string[] value)
        {
            _allowedTags = value;
        }

        private string[] _allowedTags = new string[] { "p", "b", "i", "u", "em", "big", "small",
            "div", "img", "span", "blockquote", "code", "pre", "br", "hr",
            "ul", "ol", "li", "del", "ins", "strong", "a", "font", "dd", "dt","td","tr","table"};

        /// <summary>
        /// If any tag found which is not allowed, it is replaced by this tag.
        /// Specify a tag which has least impact on output
        /// </summary>
        public string ReplacementTag { get => _replacementTag; set => _replacementTag = value; }
        private string _replacementTag = "";

        /// <summary>
        /// New lines \r\n are replaced with space which saves space and makes the
        /// output compact
        /// </summary>
        public bool RemoveNewLines { get => _removeNewLines; set => _removeNewLines = value; }
        private bool _removeNewLines = true;

        /// <summary>
        /// Specify which attributes are allowed. Any other attribute will be discarded
        /// </summary>
        public string[] GetAllowedAttributes()
        {
            return new string[] { "class", "href", "target",
            "border", "src", "align", "width", "height", "color", "size" };
        }

        //private readonly string[] _allowedAttributes = new string[] { "class", "href", "target", "border", "src", "align", "width", "height", "color", "size" };

        public HtmlWriter(TextWriter writer) : base(writer)
        {
        }

        public HtmlWriter(StringBuilder builder) : base(new StringWriter(builder, CultureInfo.InvariantCulture))
        {
        }

        public HtmlWriter(Stream stream, Encoding enc) : base(stream, enc)
        {
        }

        /// <summary>
        /// The reason why we are overriding this method is, we do not want the output to be
        /// encoded for texts inside attribute and inside node elements. For example, all the &nbsp;
        /// gets converted to &amp;nbsp in output. But this does not 
        /// apply to HTML. In HTML, we need to have &nbsp; as it is.
        /// </summary>
        /// <param name="text"></param>
        public override void WriteString(string text)
        {
            if (text == null) { throw new ArgumentNullException(nameof(text)); }
            // Change all non-breaking space to normal space
            text = text.Replace("?", "&nbsp;");
            /// When you are reading RSS feed and writing Html, this line helps remove
            /// those CDATA tags
            text = text.Replace("<![CDATA[", "");
            text = text.Replace("]]>", "");

            // Do some encoding of our own because we are going to use WriteRaw which won't
            // do any of the necessary encoding
            text = text.Replace("<", "&lt;");
            text = text.Replace(">", "&gt;");
            text = text.Replace("'", "&apos;");
            text = text.Replace("\"", "&quote;");
            text = text.Replace("&nbsp;", "");

            if (FilterOutput)
            {
                text = text.Trim();

                // We want to replace consecutive spaces to one space in order to save horizontal
                // width
                if (ReduceConsecutiveSpace)
                {
                    text = text.Replace("&nbsp;&nbsp;&nbsp;", "&nbsp;");
                }

                if (RemoveNewLines)
                {
                    text = text.Replace(Environment.NewLine, " ");
                }

                WriteRaw(text);
            }
            else
            {
                WriteRaw(text);
            }
        }

        public override void WriteWhitespace(string ws)
        {
            if (!FilterOutput)
            {
                WriteWhitespace(ws);
            }
        }

        public override void WriteComment(string text)
        {
            if (!FilterOutput)
            {
                WriteComment(text);
            }
        }

        /// <summary>
        /// This method is overriden to filter out tags which are not allowed
        /// </summary>
        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            if (localName == null) { throw new ArgumentNullException(nameof(localName)); }

            if (FilterOutput)
            {
                bool canWrite = false;
                string tagLocalName = localName.ToUpperInvariant();
                foreach (string name in GetAllowedTags())
                {
                    if (name == tagLocalName)
                    {
                        canWrite = true;
                        break;
                    }
                }

                if (!canWrite)
                {
                    localName = "dd";
                }
            }

            WriteStartElement(prefix, localName, ns);
        }

        /// <summary>
        /// This method is overriden to filter out attributes which are not allowed
        /// </summary>
        public override void WriteAttributes(XmlReader reader, bool defattr)
        {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
            if (FilterOutput)
            {
                // The following code is copied from implementation of XmlWriter's
                // WriteAttributes method. 

                if ((reader.NodeType == XmlNodeType.Element) || (reader.NodeType == XmlNodeType.XmlDeclaration))
                {
                    if (reader.MoveToFirstAttribute())
                    {
                        WriteAttributes(reader, defattr);
                        reader.MoveToElement();
                    }
                }
                else
                {
                    if (reader.NodeType != XmlNodeType.Attribute)
                    {
                        throw new XmlException("Xml_InvalidPosition");
                    }
                    do
                    {
                        if (defattr || !reader.IsDefault)
                        {
                            // Check if the attribute is allowed 
                            bool canWrite = false;
                            string attributeLocalName = reader.LocalName.ToUpperInvariant();
                            foreach (string name in GetAllowedAttributes())
                            {
                                if (name == attributeLocalName)
                                {
                                    canWrite = true;
                                    break;
                                }
                            }

                            // If allowed, write the attribute
                            if (canWrite)
                            {
                                WriteStartAttribute(reader.Prefix, attributeLocalName,
                                    reader.NamespaceURI);
                            }

                            while (reader.ReadAttributeValue())
                            {
                                if (reader.NodeType == XmlNodeType.EntityReference)
                                {
                                    if (canWrite)
                                    {
                                        WriteEntityRef(reader.Name);
                                    }

                                    continue;
                                }
                                if (canWrite)
                                {
                                    WriteString(reader.Value);
                                }
                            }
                            if (canWrite)
                            {
                                WriteEndAttribute();
                            }
                        }
                    } while (reader.MoveToNextAttribute());
                }
            }
            else
            {
                WriteAttributes(reader, defattr);
            }
        }

    }
}