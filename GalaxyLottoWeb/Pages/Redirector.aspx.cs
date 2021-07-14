using GalaxyLotto.ClassLibrary;
using GalaxyLottoWeb.GlobalApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.UI;

namespace GalaxyLottoWeb.Pages
{
    public partial class Redirector : BasePage
    {
        //private StuGLSearch _gstuSearch;
        private string _action;
        private string _requestId;
        private string _UrlFileName;

        private string LocalBrowserType { get; set; }

        private string LocalIP { get; set; }

        //private string KeySearchOrder { get; set; }

        //private Dictionary<string, DataSet> _dicFreqDataSet;

#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            LocalBrowserType = Request.Browser.Type;
            LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            //KeySearchOrder = string.Format(InvariantCulture, "{0}#{1}#dtSearchOrder", LocalIP, LocalBrowserType);

            if (Session["SearchOption"] == null)
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            else
            {
                StuGLSearch stuGLSearchTemp = (StuGLSearch)Session["SearchOption"];
                _action = stuGLSearchTemp.Action;
                _UrlFileName = stuGLSearchTemp.PageFileName;
                if (_action == Properties.Resources.SessionsFreqActiveHT01P)
                {
                    stuGLSearchTemp.LngTotalSN += 1;
                }
                //stuGLSearchTemp = new CglSearch().InitSearch(stuGLSearchTemp);
                //stuGLSearchTemp = new CglMethod().GetMethodSN(stuGLSearchTemp);
                //stuGLSearchTemp = new CglMethod().GetSearchMethodSN(stuGLSearchTemp);
                _requestId = SetRequestId(stuGLSearchTemp);
                Session[_action + _requestId] = stuGLSearchTemp;

                if (Session[_action + _requestId] == null || string.IsNullOrEmpty(_action) || string.IsNullOrEmpty(_requestId) || string.IsNullOrEmpty(_UrlFileName))
                {
                    Response.Write("<script language='javascript'>window.close();</script>");
                }
                else
                {
                    if (stuGLSearchTemp.SearchOrder)
                    {
                        Thread Thread01 = new Thread(() => { StartThread01(stuGLSearchTemp); });
                        Thread01.Start();
                        Response.Write("<script language='javascript'>window.close();</script>");
                    }
                    else
                    {
                        OpenPage(stuGLSearchTemp, _action, _requestId, _UrlFileName, "_self");
                    }
                }
            }
        }

        private void StartThread01(StuGLSearch stuGLSearchTemp)
        {
            SetSearchOrder(stuGLSearchTemp, _action, _requestId, _UrlFileName, LocalIP, LocalBrowserType);
            if (stuGLSearchTemp.InTestPeriods > 1)
            {
                OpenPageMuti(stuGLSearchTemp, _action, _UrlFileName, "_blank", LocalIP, LocalBrowserType);
            }

            if (stuGLSearchTemp.SearchFileds)
            {
                OpenFieldPage(stuGLSearchTemp, _action, _UrlFileName, "_blank", LocalIP, LocalBrowserType);
            }
        }

        private void OpenPage(StuGLSearch stuGLSearch, string action, string requestId, string urlFileName, string windowName)
        {
            string strScript;
            string strUrl = string.Format(InvariantCulture, "http://{0}/Pages/{1}?action={2}&id={3}", Request.Url.Authority, urlFileName, action, requestId);
            if (stuGLSearch.ShowStaticHtml)
            {
                List<int> lstCurrentNums = (List<int>)new CglData().GetDataNumsLst(stuGLSearch);
                #region Check File                    
                string strFileName = lstCurrentNums.Sum().Equals(0) ? string.Format(InvariantCulture, "{0}{1}Temp.html", action, requestId) : string.Format(InvariantCulture, "{0}{1}.html", action, requestId);
                string strCurrentDirectory = Server.MapPath("~");
                string strHtmlDirectory = System.IO.Path.Combine(strCurrentDirectory, "html");
                if (!lstCurrentNums.Sum().Equals(0))
                {
                    DeleteTempFile(strHtmlDirectory, action, requestId);
                }
                #endregion Check File

                #region Creat static HTML file
                if (!new CglFunc().FileExist(strHtmlDirectory, strFileName) || stuGLSearch.Recalculate)
                {
                    string strFilePathName = Path.Combine(strHtmlDirectory, strFileName);
                    new GalaxyApp().CreatStaticHtml(new Uri(strUrl, UriKind.Absolute), strFilePathName, 3000000);
                }
                #endregion Creat static HTML file

                strUrl = string.Format(InvariantCulture, "http://{0}/html/{1}", Request.Url.Authority, strFileName);
            }
            strScript = string.Format(InvariantCulture, "window.open('{0}','{1}');", strUrl, windowName);
            ScriptManager.RegisterStartupScript(this, typeof(string), "OPEN_WINDOW", strScript, true);
            //Response.Write(string.Format(InvariantCulture, "<script language='javascript'>{0}</script>", strScript));
        }

        private void DeleteTempFile(string strHtmlDirectory, string action, string requestId)
        {
            CheckData = true;
            string strFileName = string.Format(InvariantCulture, "{0}{1}Temp.html", action, requestId);
            if (new CglFunc().FileExist(strHtmlDirectory, strFileName))
            {
                string strFilePath = Path.Combine(strHtmlDirectory, strFileName);
                File.Delete(strFilePath);
            }
        }

        private void OpenFieldPage(StuGLSearch stuGLSearch, string action, string fileName, string windowName, string LocalIP, string LocalBrowserType)
        {
            Session["action"] = action;
            Session["UrlFileName"] = fileName;
            foreach (string field in new CglValidFields().GetValidFieldsLst(stuGLSearch))
            {
                if (stuGLSearch.StrCompares != field)
                {
                    StuGLSearch stuSearchTemp = stuGLSearch;
                    stuSearchTemp.FieldMode = field != "gen";
                    stuSearchTemp.StrCompares = field;
                    stuSearchTemp.SearchFileds = false;
                    //stuSearchTemp = new CglSearch().InitSearch(stuSearchTemp);
                    //stuSearchTemp = new CglMethod().GetMethodSN(stuSearchTemp);
                    string RequestId = SetRequestId(stuSearchTemp);
                    Session[action + RequestId] = stuSearchTemp;
                    //SetDicSession(stuSearchTemp, action, RequestId);
                    if (stuGLSearch.SearchOrder)
                    {
                        SetSearchOrder(stuSearchTemp, action, RequestId, fileName, LocalIP, LocalBrowserType);
                    }
                    else
                    {
                        OpenPage(stuSearchTemp, Request.Url.Authority, action, fileName, windowName);
                    }
                }
            }
        }

        private void OpenPageMuti(StuGLSearch stuGLSearch, string action, string fileName, string windowName, string LocalIP, string LocalBrowserType)
        {
            Session["action"] = action;
            //Session["action"] = action;
            Session["UrlFileName"] = fileName;
            //stuGLSearch = new CglSearch().InitSearch(stuGLSearch);
            if (stuGLSearch.InTestPeriods > 1)
            {
                DataTable dtProcess = CglFreqProcess.GetFreqProcAlls(stuGLSearch, CglDBFreq.TableName.QryFreqProcess01, SortOrder.Descending, stuGLSearch.InTestPeriods - 1);

                foreach (DataRow drRow in dtProcess.Rows)
                {
                    StuGLSearch stuSearchTemp = stuGLSearch;
                    stuSearchTemp.LngTotalSN = long.Parse(drRow["lngTotalSN"].ToString(), InvariantCulture);
                    stuSearchTemp.InTestPeriods = 1;
                    Session["id"] = SetRequestId(stuSearchTemp);
                    Session[action + SetRequestId(stuSearchTemp)] = stuSearchTemp;
                    if (stuGLSearch.SearchOrder)
                    {
                        SetSearchOrder(stuSearchTemp, action, SetRequestId(stuSearchTemp), fileName, LocalIP, LocalBrowserType);
                    }
                    else
                    {
                        OpenPage(stuSearchTemp, Request.Url.Authority, action, fileName, windowName);
                    }
                }
            }
        }

    }
}
