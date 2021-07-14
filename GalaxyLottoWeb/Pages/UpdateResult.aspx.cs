using GalaxyLotto.ClassLibrary;
using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class UpdateResult : BasePage
    {
        private string _action;
        private string _requestId;
        private string UpdateResultID;
        private StuGLSearch _stuGLSearch;

        DataTable DtCsvFile { get; set; }

        //private string _action;
        //private string _id;

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            SetupViewState();
            if (ViewState["_gstuSearch"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                //if (ViewState["_stuGLSearch"] == null) { ViewState.Add("_stuGLSearch", (StuGLSearch)Session[Action + RequestId]); }
                _stuGLSearch = (StuGLSearch)ViewState["_gstuSearch"];
                if (ViewState["Title"] == null) { ViewState.Add("Title", string.Format(InvariantCulture, "{0} 資料更新", new CglDataSet(_stuGLSearch.LottoType).LottoDescription)); }
                Page.Title = (string)ViewState["Title"];
                lblTitle.Text = (string)ViewState["Title"];
                if (ViewState["tbData"] == null)
                {
                    switch (_stuGLSearch.LottoType)
                    {
                        case TargetTable.DataPurple:
                            DataTable dtPurpleDate = new CglPurple().PurpleDate.Rows.Count > 1000 ? new CglPurple().PurpleDate.Rows.Cast<DataRow>().Take(1000).CopyToDataTable() : new CglPurple().PurpleDate;
                            DtCsvFile = new CglPurple().GetPurple(dtPurpleDate);
                            for (int rowindex = 0; rowindex < DtCsvFile.Rows.Count; rowindex++)
                            {
                                for (int strp = 1; strp <= 12; strp++)
                                {
                                    if (DtCsvFile.Rows[rowindex][string.Format(InvariantCulture, "strp{0:d2}", strp)].ToString().Length >= 4)
                                    {
                                        DtCsvFile.Rows[rowindex][string.Format(InvariantCulture, "strp{0:d2}", strp)] = DtCsvFile.Rows[rowindex][string.Format(InvariantCulture, "strp{0:d2}", strp)].ToString().Substring(0, 4);
                                    }
                                }
                            }
                            break;
                        case TargetTable.DateWC:
                            DtCsvFile = new CglFunc().GetDateWC();
                            break;
                        default:
                            DtCsvFile = new CglFunc().ReadCsvFile(_stuGLSearch.LottoType, Server.MapPath("../csv"));
                            break;
                    }

                    ViewState.Add("tbData", DtCsvFile);
                    new CglFunc().UpdateDataSilent(_stuGLSearch.LottoType, DtCsvFile);
                }
                if (ViewState["sort"] == null) { ViewState.Add("sort", ""); }

                ShowDetail();
            }
        }

        private void SetupViewState()
        {
            if (ViewState["_gstuSearch"] == null)
            {
                _action = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
                _requestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;

                if (ViewState["action"] == null) { ViewState.Add("action", _action); }
                if (ViewState["id"] == null) { ViewState.Add("id", _requestId); }

                UpdateResultID = _action + _requestId;
                if (!string.IsNullOrEmpty(_action) && !string.IsNullOrEmpty(_requestId) && Session[UpdateResultID] != null)
                {
                    if (ViewState["_gstuSearch"] == null)
                    {
                        ViewState.Add("_gstuSearch", (StuGLSearch)Session[UpdateResultID]);
                    }
                    else
                    {
                        ViewState["_gstuSearch"] = (StuGLSearch)Session[UpdateResultID];
                    };
                }

                Session.Remove("action");
                Session.Remove("id");
                Session.Remove("UrlFileName");
            }
        }

        private void ShowDetail()
        {
            DtCsvFile = (DataTable)ViewState["tbData"];

            DtCsvFile.DefaultView.Sort = (string)ViewState["sort"];
            gvUpdate.DataSource = DtCsvFile.DefaultView;

            if (gvUpdate.Columns.Count == 0)
            {
                for (int i = 0; i < DtCsvFile.Columns.Count; i++)
                {
                    BoundField bfCell = new BoundField()
                    {
                        DataField = DtCsvFile.Columns[i].ColumnName,
                        HeaderText = new CglFunc().ConvertFieldNameId(DtCsvFile.Columns[i].ColumnName, 1),
                        SortExpression = DtCsvFile.Columns[i].ColumnName
                    };
                    string strColumnName = DtCsvFile.Columns[i].ColumnName.Substring(0, 4);
                    if (strColumnName == "lngL" || strColumnName == "lngS")
                    {
                        bfCell.DataFormatString = "{0:d2}";
                    }
                    gvUpdate.Columns.Add(bfCell);
                }
            }
            gvUpdate.DataBind();
        }


        protected void GVUpdateSorting(object sender, GridViewSortEventArgs e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e)); }
            if (ViewState[e.SortExpression] == null)
            {
                ViewState[e.SortExpression] = true;
            }
            else
            {
                ViewState[e.SortExpression] = !(bool)ViewState[e.SortExpression];
            }
            string Sort = string.Format(InvariantCulture, "{0} {1}", e.SortExpression, (bool)ViewState[e.SortExpression] ? "ASC" : "DESC");


            //string sort = string.Format(InvariantCulture, "{0} {1}", e.SortExpression, GetSortDirection(e.SortExpression));
            ViewState["sort"] = Sort;
            DtCsvFile.DefaultView.Sort = (string)ViewState["sort"];
            gvUpdate.DataSource = DtCsvFile.DefaultView;
            gvUpdate.DataBind();
        }
    }
}