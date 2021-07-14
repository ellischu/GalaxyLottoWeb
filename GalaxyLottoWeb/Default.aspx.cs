using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
#pragma warning disable CA1716 // Identifiers should not match keywords
    public partial class Default : BasePage
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        private StuGLSearch gstuSearch;
        Dictionary<int, TargetTable> lottoType;
#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            //Form.Target = "_blank";
            if (!IsPostBack)
            {
                //DateTime.Parse("20:30", InvariantCulture) > DateTime.Parse(DateTime.Now.ToString("T", InvariantCulture), InvariantCulture) || DateTime.Parse("23:59", InvariantCulture) < DateTime.Parse(System.DateTime.Now.ToString("T", InvariantCulture), InvariantCulture))

                //if (new CglClass().Between(DateTime.Parse(DateTime.Now.ToString("T", InvariantCulture), InvariantCulture), DateTime.Parse("00:00", InvariantCulture), DateTime.Parse("20:30", InvariantCulture), true))
                //{
                //    DateTime dtToday = DateTime.Now.AddDays(-1).Date;
                //    if (GetLastDate(TargetTable.Lotto539).Date < dtToday)
                //    {
                //        new CglFunc().TWLotteryUpadteOnLine(TargetTable.Lotto539);
                //    }
                //    if (GetLastDate(TargetTable.LottoDafu).Date < dtToday)
                //    {
                //        new CglFunc().TWLotteryUpadteOnLine(TargetTable.LottoDafu);
                //    }
                //    if (GetLastDate(TargetTable.LottoBig).Date < dtToday)
                //    {
                //        new CglFunc().TWLotteryUpadteOnLine(TargetTable.LottoBig);
                //    }
                //    if (GetLastDate(TargetTable.LottoWeli).Date < dtToday)
                //    {
                //        new CglFunc().TWLotteryUpadteOnLine(TargetTable.LottoWeli);
                //    }
                //    if (GetLastDate(TargetTable.LottoTwinWin).Date < dtToday)
                //    {
                //        new CglFunc().TWLotteryUpadteOnLine(TargetTable.LottoTwinWin);
                //    }
                //}
            }
            ShowLastNumbers();
        }
        public void ShowLastNumbers() //顯示末期資料
        {
            CglDataSet GLdataSet00 = new CglDataSet(TargetTable.LottoBig);
            #region Set LottoType dictionary
            lottoType = new Dictionary<int, TargetTable>
            {
                { 0, TargetTable.Lotto539},
                { 1, TargetTable.LottoBig },
                { 2, TargetTable.LottoWeli },
                { 3, TargetTable.LottoSix },
                { 4, TargetTable.LottoTwinWin }
            };
            //{ 1, TargetTable.LottoDafu },
            #endregion
            //string[] strLastNumbers = new string[6];

            for (int intIndex = 0; intIndex < 5; intIndex++)
            {
                #region Set Data
                GLdataSet00 = new CglDataSet(lottoType[intIndex]);
                DataTable dtLastData;
                #endregion

                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = new SqlConnection(new CglDBData().SetDataBase(lottoType[intIndex], DatabaseType.Data));
                    sqlCommand.CommandText = "SELECT TOP(1) * FROM [tblData] WHERE [lngL1] > @lngL1 ORDER BY [lngDateSN] DESC ;";
                    sqlCommand.Parameters.AddWithValue("lngL1", 0);
                    dtLastData = new CglFunc().GetDataTable(sqlCommand);
                }

                #region Set Last Nums
                Panel pnlLotto = new GalaxyApp().CreatPanel("pnl" + GLdataSet00.Id, "row");
                pnlLotto.ClientIDMode = ClientIDMode.Static;
                pnlLastNum.Controls.Add(pnlLotto);

                Panel pnlLottoPre = new GalaxyApp().CreatPanel(string.Format(InvariantCulture, "pnlLottoPre{0}", intIndex), "col-md-4");
                pnlLotto.Controls.Add(pnlLottoPre);

                #region Set ShowAll Button
                Button btnShowAll = new GalaxyApp().CreatButton(string.Format(InvariantCulture, "btn{0}_ShowAll", lottoType[intIndex].ToString()), "glbutton glbutton-blue", "顯示");
                btnShowAll.CommandName = string.Format(InvariantCulture, "{0}", GLdataSet00.LottoDescription);
                btnShowAll.CommandArgument = intIndex.ToString(InvariantCulture);

                btnShowAll.Command += BtnShowAll_Command;
                pnlLottoPre.Controls.Add(btnShowAll);
                #endregion Set Button

                #region Set Update Button
                Button btnUpdate = new GalaxyApp().CreatButton(string.Format(InvariantCulture, "btn{0}_Update", lottoType[intIndex].ToString()), "glbutton glbutton-lightblue", "更新");
                btnUpdate.CommandName = string.Format(InvariantCulture, "{0}", GLdataSet00.LottoDescription);
                btnUpdate.CommandArgument = intIndex.ToString(InvariantCulture);

                btnUpdate.Command += BtnUpdate_Command; ;
                pnlLottoPre.Controls.Add(btnUpdate);
                #endregion Set Button

                Label lblName = new GalaxyApp().CreatLabel(string.Format(InvariantCulture, "lblName{0}", intIndex), GLdataSet00.LottoDescription, "glLottoName");
                pnlLottoPre.Controls.Add(lblName);

                foreach (DataRow drRow in dtLastData.Rows)
                {
                    Label lblDate = new Label()
                    {
                        Text = string.Format(InvariantCulture, "{0}", drRow["lngDateSN"]),
                        CssClass = "glLottoDate"
                    };
                    pnlLottoPre.Controls.Add(lblDate);
                    Panel pnlNum = new Panel()
                    {
                        CssClass = "col-lg-8"
                    };
                    pnlLotto.Controls.Add(pnlNum);

                    for (int intNumImg = 1; intNumImg <= GLdataSet00.CountNumber; intNumImg++)
                    {

                        Image pic001 = new Image()
                        {
                            ImageUrl = string.Format(InvariantCulture, "~/Resources/N{0:D2}.png", drRow[string.Format(InvariantCulture, "lngL{0}", intNumImg)])
                        };
                        //new Uri("pack://siteoforigin:,,,/Resources/N" + string.Format(InvariantCulture, "{0:D2}", datarow00["lngL" + Number00.ToString()]) + ".png",UriKind.Absolute).ToString()                                
                        pnlNum.Controls.Add(pic001);
                    }
                    if (GLdataSet00.SCountNumber > 0)
                    {
                        Image picspe = new Image()
                        {
                            ImageUrl = "~/Resources/spe.gif"
                        };
                        pnlNum.Controls.Add(picspe);
                        Image pic001 = new Image()
                        {
                            ImageUrl = string.Format(InvariantCulture, "~/Resources/N{0:D2}.png", drRow["lngS1"])
                        };
                        pnlNum.Controls.Add(pic001);
                    }
                }
                #endregion

            }
        }

        private void BtnUpdate_Command(object sender, CommandEventArgs e)
        {
            new CglFunc().TWLotteryUpadteOnLine(lottoType[int.Parse(e.CommandArgument.ToString(), InvariantCulture)]);
        }

        private void BtnShowAll_Command(object sender, CommandEventArgs e)
        {
            Session.Remove("action");
            Session.Remove(Properties.Resources.SessionsShowAll);
            gstuSearch = new StuGLSearch() { LottoType = lottoType[int.Parse(e.CommandArgument.ToString(), InvariantCulture)] };
            SetSession(gstuSearch, Properties.Resources.SessionsShowAll, "update", Properties.Resources.PageShowAll);

            string url = string.Format(InvariantCulture, "http://{0}/Pages/{1}?action={2}&id={3}",
                                                          Request.Url.Authority, "ShowAll.aspx", Properties.Resources.SessionsShowAll, "update");
            string fullURL = "window.open('" + url + "', '_blank');";
            ScriptManager.RegisterStartupScript(this, typeof(string), "OPEN_WINDOW", fullURL, true);


            //Response.Redirect(string.Format(InvariantCulture, "Pages/ShowAll.aspx?action={0}&id={1}", Properties.Resources.SessionsShowAll, "update"));
        }

        private void SetSession(StuGLSearch stuGLSearch, string action, string id, string UrlFileName)
        {
            if (Session["action"] == null) { Session.Add("action", action); } else { Session["action"] = action; }
            if (Session["id"] == null) { Session.Add("id", id); } else { Session["id"] = id; }
            if (Session["UrlFileName"] == null) { Session.Add("UrlFileName", UrlFileName); } else { Session["UrlFileName"] = UrlFileName; }
            Session.Add(action + id, stuGLSearch);
        }

    }
}