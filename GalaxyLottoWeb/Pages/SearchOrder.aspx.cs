using System;
using System.Data;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class SearchOrder : BasePage
    {
        //private StuGLSearch _stuGLSearch;
        private static string StrNoOrder { get; } = "沒有排程";
        private static DataTable DtSearchOrder { get; set; }
        private string LocalBrowserType { get; set; }

        private string LocalIP { get; set; }

        private string KeySearchOrder { get; set; }

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            LocalBrowserType = Request.Browser.Type;
            LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            KeySearchOrder = string.Format(InvariantCulture, "{0}#{1}#dtSearchOrder", LocalIP, LocalBrowserType);
            try
            {
                if (ServerOption != null || ServerOption.Count > 0)
                {
                    DtSearchOrder = (DataTable)ServerOption[KeySearchOrder];
                    ShowSearchOrder();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (NullReferenceException)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void ShowSearchOrder()
        {
            if (DtSearchOrder.Rows.Count > 0)
            {
                gvSearchOrder.Visible = true;
                gvSearchOrder.DataSource = DtSearchOrder.DefaultView;
                gvSearchOrder.DataKeyNames = new string[] { "ActionID" };
                gvSearchOrder.RowDeleting += GvSearchOrder_RowDeleting;
                gvSearchOrder.DataBind();
            }
            else
            {
                gvSearchOrder.Visible = false;
            }
        }

        private void GvSearchOrder_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            GridViewRow row = gvSearchOrder.Rows[e.RowIndex];
            DtSearchOrder.Rows[row.DataItemIndex].Delete();
            gvSearchOrder.DataBind();
        }

        protected void Timer1Tick(object sender, EventArgs e)
        {
            lblTitle.Text = string.Format(InvariantCulture, "{0}:{1}", DateTime.Now.ToLongTimeString(), CurrentSearchOrderID);
            lblArgument00.Text = DtSearchOrder.Rows.Count > 0 ? string.Format(InvariantCulture, "{0} 排程", DtSearchOrder.Rows.Count) : StrNoOrder;
            CheckThreadSearchOrder();
        }

        protected void CheckThreadSearchOrder()
        {
            if (DtSearchOrder.Rows.Count > 0)
            {
                CreatThreadSearchOrder();
            }
            else
            {
                chkStart.Checked = false;
                ScriptManager.RegisterStartupScript(this, typeof(string), "CLOSE_WINDOW", "window.close();", true);
            }
        }

        //private void RestarThreadSearchOrder()
        //{
        //    ThreadSearchOrder.Join();
        //    ThreadSearchOrder.Abort();
        //    ThreadSearchOrder = null;
        //    CheckThreadSearchOrder();
        //}

        private void CreatThreadSearchOrder()
        {
            if (string.IsNullOrEmpty(CurrentSearchOrderID) && chkStart.Checked)
            {
                if (!string.IsNullOrEmpty(DtSearchOrder.Rows[0]["ActionID"].ToString()) &&
                    !string.IsNullOrEmpty(DtSearchOrder.Rows[0]["Action"].ToString()) &&
                    !string.IsNullOrEmpty(DtSearchOrder.Rows[0]["requestId"].ToString()) &&
                    !string.IsNullOrEmpty(DtSearchOrder.Rows[0]["urlFileName"].ToString()))
                {
                    CurrentSearchOrderID = DtSearchOrder.Rows[0]["ActionID"].ToString();
                    Session["action"] = DtSearchOrder.Rows[0]["Action"].ToString();
                    Session["id"] = DtSearchOrder.Rows[0]["requestId"].ToString();
                    Session["UrlFileName"] = DtSearchOrder.Rows[0]["urlFileName"].ToString();
                    Session[CurrentSearchOrderID] = dicSearchOrder[CurrentSearchOrderID];
                    string url = string.Format(InvariantCulture, "http://{0}/Pages/{1}?action={2}&id={3}", Request.Url.Authority, DtSearchOrder.Rows[0]["urlFileName"].ToString(), DtSearchOrder.Rows[0]["Action"].ToString(), DtSearchOrder.Rows[0]["requestId"].ToString());
                    string fullURL = string.Format(InvariantCulture, "window.open('{0}', '_blank');", url);
                    ScriptManager.RegisterStartupScript(this, typeof(string), "OPEN_WINDOW", fullURL, true);
                }
                dicSearchOrder.Remove(CurrentSearchOrderID);
                DtSearchOrder.Rows[0].Delete();
            }
        }

        protected void BtnRestartClick(object sender, EventArgs e)
        {
            CurrentSearchOrderID = string.Empty;
        }

        protected void BtnClearClick(object sender, EventArgs e)
        {
            DtSearchOrder.Clear();
            dicSearchOrder.Clear();
            Session.Remove("action");
            Session.Remove("id");
            Session.Remove("UrlFileName");
            Session.Remove(CurrentSearchOrderID);
            CurrentSearchOrderID = string.Empty;
        }
    }
}