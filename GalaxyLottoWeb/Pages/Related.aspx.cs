using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;

namespace GalaxyLottoWeb.Pages
{
    public partial class Related : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Calendar01_SelectionChanged(object sender, EventArgs e)
        {
            using DataTable dtDate = new CglData().GetDateData(Calendar01.SelectedDate.Date);

            DataRow drDAte = dtDate.NewRow();

            // 找農曆
            TaiwanLunisolarCalendar tlc = new TaiwanLunisolarCalendar();
            int leapMouth = tlc.GetLeapMonth(tlc.GetYear(Calendar01.SelectedDate.Date));
            int LuniMouth = tlc.GetMonth(Calendar01.SelectedDate.Date);
            if (leapMouth > 0)
            {
                if (LuniMouth == leapMouth)
                {
                    LuniMouth = leapMouth - 1;
                }
                else if (LuniMouth > leapMouth)
                {
                    LuniMouth -= 1;
                }
            }
            drDAte["lngDateSN"] = string.Format(InvariantCulture, "{0}{1:d2}{2:d2}", Calendar01.SelectedDate.Year, Calendar01.SelectedDate.Month, Calendar01.SelectedDate.Day);
            drDAte["lngCDateSN"] = string.Format(InvariantCulture, "{0}{1:d2}{2:d2}", tlc.GetYear(Calendar01.SelectedDate.Date), LuniMouth, tlc.GetDayOfMonth(Calendar01.SelectedDate.Date));

            //找紫微
            StuPurple stuTemp = new StuPurple
            {
                StrWYear = Calendar01.SelectedDate.Year.ToString(InvariantCulture),
                StrWMonth = Calendar01.SelectedDate.Month.ToString("d2", InvariantCulture),
                StrWDay = Calendar01.SelectedDate.Day.ToString("d2", InvariantCulture),
                StrWHour = "YY11",
                IntGender = CglPurple.Gender.MalePlus
            };
            stuTemp.Init();
            foreach (KeyValuePair<string, string> KeyPair in stuTemp.DicUpdateData)
            {
                if (KeyPair.Key != "lngDateSN")
                {

                    if (KeyPair.Value.Length >= 4)
                    {
                        drDAte[KeyPair.Key] = KeyPair.Value.Substring(0, 4);
                    }
                    else
                    {
                        drDAte[KeyPair.Key] = KeyPair.Value;
                    }
                }
            }

            //

            dtDate.Rows.Add(drDAte);
            GridView gvDate = new GalaxyApp().CreatGridView("gvDate", " gltabel ", dtDate, true, true);
            gvDate.DataBind();
            pnlCalendar.Controls.Add(gvDate);

            // 一些使用的範例程式
            //StringBuilder stringBuilder = new StringBuilder(0,8000);
            //for (int i = 2007; i <= 2050; i++)
            //{
            //    CNDate cd1 = new CNDate(new DateTime(i, Calendar01.SelectedDate.Date.Month, Calendar01.SelectedDate.Date.Day));
            //    stringBuilder.AppendLine("計算 " + i + "年隔年農曆過年的國曆日期：" + cd1.GetNextLunarNewYearDate().ToLongDateString() + "<br/>");
            //}

            //CNDate cd = new CNDate(Calendar01.SelectedDate);

            //stringBuilder.AppendLine("今年農曆天數：" + cd.LunarYearDays(2007).ToString(InvariantCulture) + "<br/>");
            //stringBuilder.AppendLine("今年	01	月農曆天數：" + cd.LunarMonthDays(2007, 1).ToString(InvariantCulture) + "<br/>");
            //stringBuilder.AppendLine("今年	02	月農曆天數：" + cd.LunarMonthDays(2007, 2).ToString(InvariantCulture) + "<br/>");
            //stringBuilder.AppendLine("今年	03	月農曆天數：" + cd.LunarMonthDays(2007, 3).ToString(InvariantCulture) + "<br/>");
            //stringBuilder.AppendLine("今年	04	月農曆天數：" + cd.LunarMonthDays(2007, 4).ToString(InvariantCulture) + "<br/>");
            //stringBuilder.AppendLine("今年	05	月農曆天數：" + cd.LunarMonthDays(2007, 5).ToString(InvariantCulture) + "<br/>");
            //stringBuilder.AppendLine("今年	06	月農曆天數：" + cd.LunarMonthDays(2007, 6).ToString(InvariantCulture) + "<br/>");
            //stringBuilder.AppendLine("今年	07	月農曆天數：" + cd.LunarMonthDays(2007, 7).ToString(InvariantCulture) + "<br/>");
            //stringBuilder.AppendLine("今年	08	月農曆天數：" + cd.LunarMonthDays(2007, 8).ToString(InvariantCulture) + "<br/>");
            //stringBuilder.AppendLine("今年	09	月農曆天數：" + cd.LunarMonthDays(2007, 9).ToString(InvariantCulture) + "<br/>");
            //stringBuilder.AppendLine("今年	10	月農曆天數：" + cd.LunarMonthDays(2007, 10).ToString(InvariantCulture) + "<br/>");
            //stringBuilder.AppendLine("今年	11	月農曆天數：" + cd.LunarMonthDays(2007, 11).ToString(InvariantCulture) + "<br/>");
            //stringBuilder.AppendLine("今年	12	月農曆天數：" + cd.LunarMonthDays(2007, 12).ToString(InvariantCulture) + "<br/>");
            //stringBuilder.AppendLine("今天的農曆日期：" + cd.GetLunarHolDay() + "<br/>");
            //stringBuilder.AppendLine("今年的農曆潤月月份：" + cd.GetLeapMonth(2007) + "<br/>");
            //stringBuilder.AppendLine("計算國曆當天對應的節氣：" + cd.LGetLunarHolDay() + "<br/>");
            //stringBuilder.AppendLine("計算今年農曆過年的國曆日期：" + cd.GetLunarNewYearDate().ToLongDateString() + "<br/>");
            //lblCC.Text = stringBuilder.ToString();
        }

#pragma warning disable CA1822 // 將成員標記為靜態
        protected void Calendar01_DayRender(object sender, DayRenderEventArgs e)
#pragma warning restore CA1822 // 將成員標記為靜態
        {
            if (sender is null) { throw new ArgumentNullException(nameof(sender)); }
            if (e is null) { throw new ArgumentNullException(nameof(e)); }
            if (e.Day.IsOtherMonth && !e.Day.IsWeekend)
                e.Cell.BackColor = System.Drawing.Color.LightGray;
        }
    }
}