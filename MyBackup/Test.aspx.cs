using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GalaxyLottoWeb.Pages
{
    public partial class Test : Page
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {


        }

        protected override void OnInit( EventArgs e)
        {
            base.OnInit(e);

            if (User.Identity.IsAuthenticated)
            {
                ViewStateUserKey = User.Identity.Name;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:不要將常值當做已當地語系化的參數傳遞", MessageId = "System.Web.UI.WebControls.Label.set_Text(System.String)")]
        protected void BtnInvokeClick(object sender, EventArgs e)
        {
            lblText.Text = "";
            System.Threading.Thread.Sleep(3000);
            lblText.Text = "Processing completed";
        }
    }
}