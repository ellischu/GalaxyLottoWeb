using System;
using System.Data;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class FrtSearchOrder : BasePage
    {
        //private StuGLSearch _stuGLSearch;
        private static string StrNoOrder { get; } = "沒有排程";
        private static DataTable DtFrtSearchOrder { get; set; }

        private string LocalBrowserType { get; set; }

        private string LocalIP { get; set; }

        //private string KeyFrtSearchOrder { get; set; }

        // ---------------------------------------------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            LocalBrowserType = Request.Browser.Type;
            LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            //KeyFrtSearchOrder = string.Format(InvariantCulture, "{0}#{1}#dtFrtSearchOrder", LocalIP, LocalBrowserType);

            DtFrtSearchOrder = (DataTable)ServerOption[string.Format(InvariantCulture, "{0}#{1}#dtFrtSearchOrder", LocalIP, LocalBrowserType)];
            ShowFRTSearchOrder();
        }

        private void ShowFRTSearchOrder()
        {
            if (DtFrtSearchOrder.Rows.Count > 0)
            {
                gvFrtSearchOrder.Visible = true;
                gvFrtSearchOrder.DataSource = DtFrtSearchOrder.DefaultView;
                gvFrtSearchOrder.DataKeyNames = new string[] { "ActionID" };
                gvFrtSearchOrder.RowDeleting += GvFrtSearchOrder_RowDeleting;
                gvFrtSearchOrder.DataBind();
            }
            else
            {
                gvFrtSearchOrder.Visible = false;
            }
        }

        private void GvFrtSearchOrder_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            GridViewRow row = gvFrtSearchOrder.Rows[e.RowIndex];
            DtFrtSearchOrder.Rows[row.DataItemIndex].Delete();
            gvFrtSearchOrder.DataBind();
        }

        protected void Timer1Tick(object sender, EventArgs e)
        {
            lblTitle.Text = string.Format(InvariantCulture, "{0}:{1}", DateTime.Now.ToLongTimeString(), CurrentFrtSearchOrderID);
            lblArgument.Text = DtFrtSearchOrder.Rows.Count > 0 ? string.Format(InvariantCulture, "{0} 排程", DtFrtSearchOrder.Rows.Count) : StrNoOrder;
            CheckFrtSearchOrder();
        }

        protected void CheckFrtSearchOrder()
        {
            if (DtFrtSearchOrder.Rows.Count > 0) { CreatFrtSearchOrder(); }
        }

        private void CreatFrtSearchOrder()
        {
            if (string.IsNullOrEmpty(CurrentFrtSearchOrderID) && chkStart.Checked)
            {
                if (!string.IsNullOrEmpty(DtFrtSearchOrder.Rows[0]["ActionID"].ToString()) &&
                    !string.IsNullOrEmpty(DtFrtSearchOrder.Rows[0]["Action"].ToString()) &&
                    !string.IsNullOrEmpty(DtFrtSearchOrder.Rows[0]["requestId"].ToString()) &&
                    !string.IsNullOrEmpty(DtFrtSearchOrder.Rows[0]["urlFileName"].ToString()))
                {
                    CurrentFrtSearchOrderID = DtFrtSearchOrder.Rows[0]["ActionID"].ToString();
                    Session["action"] = DtFrtSearchOrder.Rows[0]["Action"].ToString();
                    Session["id"] = DtFrtSearchOrder.Rows[0]["requestId"].ToString();
                    Session["UrlFileName"] = DtFrtSearchOrder.Rows[0]["urlFileName"].ToString();
                    Session[CurrentFrtSearchOrderID] = dicFrtSearchOrder[CurrentFrtSearchOrderID];
                    string url = string.Format(InvariantCulture, "http://{0}/Pages/{1}?action={2}&id={3}", Request.Url.Authority, DtFrtSearchOrder.Rows[0]["urlFileName"].ToString(), DtFrtSearchOrder.Rows[0]["Action"].ToString(), DtFrtSearchOrder.Rows[0]["requestId"].ToString());
                    string fullURL = "window.open('" + url + "', '_blank');";
                    ScriptManager.RegisterStartupScript(this, typeof(string), "OPEN_WINDOW", fullURL, true);
                }
                dicFrtSearchOrder.Remove(CurrentFrtSearchOrderID);
                DtFrtSearchOrder.Rows[0].Delete();
            }
        }

        protected void BtnRestartClick(object sender, EventArgs e)
        {
            CurrentFrtSearchOrderID = string.Empty;
        }
        protected void BtnClearClick(object sender, EventArgs e)
        {
            DtFrtSearchOrder.Clear();
            dicFrtSearchOrder.Clear();
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
            Session.Remove(CurrentFrtSearchOrderID);
            CurrentFrtSearchOrderID = string.Empty;
        }

    }
}