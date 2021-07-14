using GalaxyLotto.ClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace GalaxyLottoWeb.Pages
{
    public partial class ShortCut : BasePage
    {
        //private StuGLSearch _gstuSearch;
        private string localAction;
        private string localRequestID;
        private string AspFileName;
        private string LocalBrowserType { get; set; }

        private string LocalIP { get; set; }

        private Thread Thread01;
        //private string KeySearchOrder { get; set; }
        //private Dictionary<string, DataSet> _dicFreqDataSet;

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            LocalBrowserType = Request.Browser.Type;
            LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            //KeySearchOrder = string.Format(InvariantCulture, "{0}#{1}#dtSearchOrder", LocalIP, LocalBrowserType);

            if (Session["SearchOption"] == null )
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                StuGLSearch stuGLSearchTemp = (StuGLSearch)Session["SearchOption"];
                stuGLSearchTemp = new CglSearch().InitSearch(stuGLSearchTemp);
                stuGLSearchTemp = new CglMethod().GetMethodSN(stuGLSearchTemp);
                stuGLSearchTemp = new CglMethod().GetSearchMethodSN(stuGLSearchTemp);
                localAction = stuGLSearchTemp.Action;
                AspFileName = stuGLSearchTemp.PageFileName;
                localRequestID = SetRequestId(stuGLSearchTemp);
                Session[localAction + localRequestID] = stuGLSearchTemp;
                if (Session[localAction + localRequestID] == null || string.IsNullOrEmpty(localAction) || string.IsNullOrEmpty(localRequestID) || string.IsNullOrEmpty(AspFileName))
                {
                    Response.Write("<script language='javascript'>window.close();</script>");
                }
                else
                {
                    Thread01 = new Thread(() => { StartThread01(stuGLSearchTemp); }) { Name =  "ShortCut01" };
                    Thread01.Start();


                    Response.Write("<script language='javascript'>window.close();</script>");
                }
            }
        }

        private void StartThread01(StuGLSearch _gstuSearch)
        {
            if (localAction == Properties.Resources.SessionsFreqActiveHT01 || localAction == Properties.Resources.SessionsFreqActiveHT01P)
            {
                StuGLSearch stuGLSearchTemp = _gstuSearch;
                stuGLSearchTemp.FilterRange = true;
                stuGLSearchTemp.StrFilterRange = "1";
                stuGLSearchTemp.SglFilterMin = 0;
                stuGLSearchTemp.SglFilterMax = 0;
                SetSearchOrder(stuGLSearchTemp, localAction, SetRequestId(stuGLSearchTemp), AspFileName, LocalIP, LocalBrowserType);

                stuGLSearchTemp.FilterRange = true;
                stuGLSearchTemp.StrFilterRange = "1#2";
                stuGLSearchTemp.SglFilterMin = 0;
                stuGLSearchTemp.SglFilterMax = 0;
                SetSearchOrder(stuGLSearchTemp, localAction, SetRequestId(stuGLSearchTemp), AspFileName, LocalIP, LocalBrowserType);

                stuGLSearchTemp.FilterRange = true;
                stuGLSearchTemp.StrFilterRange = "2#3";
                stuGLSearchTemp.SglFilterMin = 0;
                stuGLSearchTemp.SglFilterMax = 0;
                SetSearchOrder(stuGLSearchTemp, localAction, SetRequestId(stuGLSearchTemp), AspFileName, LocalIP, LocalBrowserType);

                stuGLSearchTemp.FilterRange = true;
                stuGLSearchTemp.StrFilterRange = "none";
                stuGLSearchTemp.SglFilterMin = 1;
                stuGLSearchTemp.SglFilterMax = 1000;
                SetSearchOrder(stuGLSearchTemp, localAction, SetRequestId(stuGLSearchTemp), AspFileName, LocalIP, LocalBrowserType);

            }
            if (localAction == Properties.Resources.SessionsDataB || localAction == Properties.Resources.SessionsDataN)
            {
                List<string> Fields = (List<string>)new CglValidFields().GetValidFieldsLst(_gstuSearch);
                foreach (string field in Fields)
                {
                    StuGLSearch stuGLSearchTemp = _gstuSearch;
                    stuGLSearchTemp.FieldMode = field != "gen";
                    stuGLSearchTemp.StrCompares = field != "gen" ? field : "gen";
                    stuGLSearchTemp = new CglSearch().InitSearch(stuGLSearchTemp);
                    stuGLSearchTemp = new CglMethod().GetMethodSN(stuGLSearchTemp);
                    stuGLSearchTemp = new CglMethod().GetSearchMethodSN(stuGLSearchTemp);
                    stuGLSearchTemp = new CglMethod().GetSecFieldSN(stuGLSearchTemp);
                    SetSearchOrder(stuGLSearchTemp, localAction, SetRequestId(stuGLSearchTemp), AspFileName, LocalIP, LocalBrowserType);
                }
            }
        }
    }
}
