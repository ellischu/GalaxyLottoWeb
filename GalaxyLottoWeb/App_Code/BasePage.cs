using GalaxyLotto.ClassLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;

[assembly: CLSCompliant(false)]
namespace GalaxyLottoWeb.Pages
{
    [Serializable]
    public partial class BasePage : Page
    {
        public bool CheckData { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (User.Identity.IsAuthenticated) ViewStateUserKey = Session.SessionID;

        }


        public static CultureInfo InvariantCulture { get; } = CultureInfo.InvariantCulture;

        // ---------------------------------------------------------------------------------------------------------

        internal static Dictionary<string, object> ServerOption { get; set; }

        // ---------------------------------------------------------------------------------------------------------

        internal static Dictionary<string, StuGLSearch> dicSearchOrder = new Dictionary<string, StuGLSearch>();

        internal static Dictionary<string, StuGLSearch> dicFrtSearchOrder = new Dictionary<string, StuGLSearch>();

        // internal static DataTable DtSearchOrder { get; set; } = new DataTable();

        //internal static DataTable DtFrtSearchOrder { get; set; } = new DataTable();

        internal static string CurrentSearchOrderID { get; set; } = string.Empty;

        internal static string CurrentFrtSearchOrderID { get; set; } = string.Empty;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:使用簡單的 'using' 陳述式", Justification = "<暫止>")]
        internal static void SetSearchOrder(StuGLSearch stuGLSearch, string action, string requestId, string FileName, string LocalIP, string LocalBrowserType)
        {
            if (FileName == null) { throw new ArgumentNullException(nameof(FileName)); }
            if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(requestId) && !string.IsNullOrEmpty(FileName))
            {
                string keySearchOrder = string.Format(InvariantCulture, "{0}#{1}#dtSearchOrder", LocalIP, LocalBrowserType);
                if (!ServerOption.ContainsKey(keySearchOrder))
                {
                    ServerOption.Add(keySearchOrder, CreatSearchOrderDT());
                }
                using (DataTable DtSearchOrder = (DataTable)ServerOption[keySearchOrder])
                {
                    if (DtSearchOrder.Rows.Count == 0 || DtSearchOrder.Rows.Count > 0 && DtSearchOrder.Rows.Find(action + requestId) == null)
                    {
                        DataRow drRow = DtSearchOrder.NewRow();
                        drRow["ActionID"] = action + requestId;
                        drRow["Action"] = action;
                        drRow["requestId"] = requestId;
                        drRow["urlFileName"] = FileName;
                        drRow["strFilterRange"] = stuGLSearch.StrFilterRange;
                        drRow["sglFilterMin"] = stuGLSearch.SglFilterMin;
                        drRow["sglFilterMax"] = stuGLSearch.SglFilterMax;
                        ((DataTable)ServerOption[keySearchOrder]).Rows.Add(drRow);
                        //stuGLSearch.SearchOrder = false;
                        if (!dicSearchOrder.ContainsKey(action + requestId))
                        {
                            dicSearchOrder.Add(action + requestId, stuGLSearch);
                        }
                        else
                        {
                            dicSearchOrder[action + requestId] = stuGLSearch;
                        }
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:使用簡單的 'using' 陳述式", Justification = "<暫止>")]
        internal static void SetFrtSearchOrder(StuGLSearch stuGLSearch, string action, string requestId, string FileName, string LocalIP, string LocalBrowserType)
        {
            if (FileName == null) { throw new ArgumentNullException(nameof(FileName)); }
            if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(requestId) && !string.IsNullOrEmpty(FileName))
            {
                if (((DataSet)ServerOption[string.Format(InvariantCulture, "{0}#{1}#dtFrtSearchOrder", LocalIP, LocalBrowserType)]).Tables["FrtSearchOrder"] == null
                    || ((DataSet)ServerOption[string.Format(InvariantCulture, "{0}#{1}#dtFrtSearchOrder", LocalIP, LocalBrowserType)]).Tables["FrtSearchOrder"].Columns.Count == 0)
                {
                    ((DataSet)ServerOption[string.Format(InvariantCulture, "{0}#{1}#dtFrtSearchOrder", LocalIP, LocalBrowserType)]).Tables.Add(CreatFrtSearchOrderDT());

                }
                using (DataTable DtFrtSearchOrder = ((DataSet)ServerOption[string.Format(InvariantCulture, "{0}#{1}#dtFrtSearchOrder", LocalIP, LocalBrowserType)]).Tables["FrtSearchOrder"])
                {
                    if (DtFrtSearchOrder.Rows.Count == 0 ||
                        DtFrtSearchOrder.Rows.Count > 0 &&
                        DtFrtSearchOrder.Select(string.Format(InvariantCulture, "[ActionID] = '{0}'", action + requestId)).Length == 0)
                    {
                        DataRow drRow = DtFrtSearchOrder.NewRow();
                        drRow["ActionID"] = action + requestId;
                        drRow["Action"] = action;
                        drRow["requestId"] = requestId;
                        drRow["urlFileName"] = FileName;
                        drRow["lngMethodSN"] = stuGLSearch.LngMethodSN;
                        drRow["lngSearchMethodSN"] = stuGLSearch.LngSearchMethodSN;
                        drRow["lngSecFieldSN"] = stuGLSearch.LngSecFieldSN;
                        DtFrtSearchOrder.Rows.Add(drRow);
                        //stuGLSearch.SearchOrder = false

                        if (!dicFrtSearchOrder.ContainsKey(action + requestId))
                        {
                            dicFrtSearchOrder.Add(action + requestId, stuGLSearch);
                        }
                        else
                        {
                            dicFrtSearchOrder[action + requestId] = stuGLSearch;
                        }
                    }
                }
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:使用簡單的 'using' 陳述式", Justification = "<暫止>")]
        private static DataTable CreatSearchOrderDT()
        {
            using (DataTable dtReturn = new DataTable() { Locale = InvariantCulture, TableName = " SearchOrder" })
            {
                dtReturn.Columns.Add(new DataColumn() { ColumnName = "strFilterRange", DataType = typeof(string), AllowDBNull = true });
                dtReturn.Columns.Add(new DataColumn() { ColumnName = "sglFilterMin", DataType = typeof(string), AllowDBNull = true });
                dtReturn.Columns.Add(new DataColumn() { ColumnName = "sglFilterMax", DataType = typeof(string), AllowDBNull = true });
                dtReturn.Columns.Add(new DataColumn() { ColumnName = "urlFileName", DataType = typeof(string), AllowDBNull = false });
                dtReturn.Columns.Add(new DataColumn() { ColumnName = "ActionID", DataType = typeof(string), AllowDBNull = false, Unique = true });
                dtReturn.Columns.Add(new DataColumn() { ColumnName = "Action", DataType = typeof(string), AllowDBNull = false, });
                dtReturn.Columns.Add(new DataColumn() { ColumnName = "requestId", DataType = typeof(string), AllowDBNull = false, });
                dtReturn.PrimaryKey = new DataColumn[] { dtReturn.Columns["ActionID"] };
                return dtReturn;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:使用簡單的 'using' 陳述式", Justification = "<暫止>")]
        private static DataTable CreatFrtSearchOrderDT()
        {
            using (DataTable dtReturn = new DataTable { Locale = InvariantCulture, TableName = "FrtSearchOrder" })
            {
                dtReturn.Columns.Add(new DataColumn { ColumnName = "lngMethodSN", DataType = typeof(string), AllowDBNull = true });
                dtReturn.Columns.Add(new DataColumn { ColumnName = "lngSearchMethodSN", DataType = typeof(string), AllowDBNull = true });
                dtReturn.Columns.Add(new DataColumn { ColumnName = "lngSecFieldSN", DataType = typeof(string), AllowDBNull = true });
                dtReturn.Columns.Add(new DataColumn { ColumnName = "urlFileName", DataType = typeof(string), AllowDBNull = false });
                dtReturn.Columns.Add(new DataColumn { ColumnName = "ActionID", DataType = typeof(string), AllowDBNull = false, Unique = true });
                dtReturn.Columns.Add(new DataColumn { ColumnName = "Action", DataType = typeof(string), AllowDBNull = false });
                dtReturn.Columns.Add(new DataColumn { ColumnName = "requestId", DataType = typeof(string), AllowDBNull = false });
                dtReturn.PrimaryKey = new DataColumn[] { dtReturn.Columns["ActionID"] };
                return dtReturn;
            }
        }

        internal static void ResetSearchOrder(string SearchOrderID)
        {
            if (CurrentSearchOrderID == SearchOrderID)
            {
                CurrentSearchOrderID = string.Empty;
            }
        }

        internal static void ResetFrtSearchOrder(string FRTSearchOrderID)
        {
            if (CurrentFrtSearchOrderID == FRTSearchOrderID)
            {
                CurrentFrtSearchOrderID = string.Empty;
            }
        }

        internal string SetRequestId(StuGLSearch stuGLSearch)
        {
            CheckData = true;
            //stuGLSearch = new CglMethod().GetMethodSN(stuGLSearch);
            //StuGLSearch stuGLSearchTemp = stuGLSearch;
            //stuGLSearchTemp = new CglMethod().GetSearchMethodSN(stuGLSearchTemp);
            //stuGLSearchTemp = new CglMethod().GetSecFieldSN(stuGLSearchTemp);
            int idhash = string.Format(InvariantCulture, "{0}{1}", Environment.MachineName, DateTime.Now.ToString(InvariantCulture)).GetHashCode();
            //stuGLSearch.LngTotalSN,
            //stuGLSearch.LngMethodSN,
            //stuGLSearch.LngSearchMethodSN,
            //stuGLSearch.LngSecFieldSN,
            //stuGLSearch.InDisplayPeriod,
            //stuGLSearch.HistoryTestPeriods,
            //stuGLSearch.InFreqDnaLength,
            //stuGLSearch.InTargetTestPeriods,
            //stuGLSearch.InFieldPeriodLimit

            return string.Format(InvariantCulture, "{0}{1}_{2}",
                                  stuGLSearch.LottoType.ToString(),
                                  new CglData().GetCurrentDataDics(stuGLSearch)["lngDateSN"],
                                  idhash
                                  );
        }

    }
}