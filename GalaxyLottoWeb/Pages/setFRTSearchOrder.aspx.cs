using GalaxyLotto.ClassLibrary;
using System;
using System.Net;

namespace GalaxyLottoWeb.Pages
{
    public partial class SetFrtSearchOrder : BasePage
    {
        //private StuGLSearch _stuGLSearch;
        private string WebAction { set; get; }

        private string WebRequestId { set; get; }

        private string WebUrlFileName { set; get; }

        private StuGLSearch GstuSearch { set; get; }
        private string LocalBrowserType { get; set; }

        private string LocalIP { get; set; }

        //private string KeyFrtSearchOrder { get; set; }


#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {

            WebAction = Request["action"] ?? (string)Session["action"] ?? (string)ViewState["action"] ?? string.Empty;
            WebRequestId = Request["id"] ?? (string)Session["id"] ?? (string)ViewState["id"] ?? string.Empty;
            WebUrlFileName = Request["UrlFileName"] ?? (string)Session["UrlFileName"] ?? (string)ViewState["UrlFileName"] ?? string.Empty;

            LocalBrowserType = Request.Browser.Type;
            LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            //KeyFrtSearchOrder = string.Format(InvariantCulture, "{0}#{1}#dtFrtSearchOrder", LocalIP, LocalBrowserType);

            if (string.IsNullOrEmpty(WebAction) || string.IsNullOrEmpty(WebRequestId) || string.IsNullOrEmpty(WebUrlFileName))
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                if (Session[WebAction + WebRequestId] != null)
                {
                    GstuSearch = (StuGLSearch)Session[WebAction + WebRequestId];
                    SetFrtSearchOrder(GstuSearch, WebAction, WebRequestId, WebUrlFileName, LocalIP, LocalBrowserType);
                    Response.Write("<script language='javascript'>window.close();</script>");
                }
            }
        }
    }
}