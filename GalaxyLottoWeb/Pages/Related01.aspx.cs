using System;
using System.Web.UI.WebControls;
using GalaxyLotto.ClassLibrary;

namespace GalaxyLottoWeb.Pages
{
    public partial class Relatetion : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        //protected override void OnInit(EventArgs e)
        //{
        //    base.OnInit(e);

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        ViewStateUserKey = User.Identity.Name;
        //    }
        //}

        protected void Calendar01_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DateTime day in Calendar1.SelectedDates)
            {

                Message.Text += day.Date.ToShortDateString() + "<br />";

            }
        }

        protected void Calendar01_DayRender(object sender, DayRenderEventArgs e)
        {
            if (sender is null) { throw new ArgumentNullException(nameof(sender)); }

            if (e is null) { throw new ArgumentNullException(nameof(e)); }
            if (e.Day.IsOtherMonth && !e.Day.IsWeekend)
                e.Cell.BackColor = System.Drawing.Color.LightGray;
        }
    }
}