using System.Web.Optimization;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(GalaxyLottoWeb.AppStart.AjaxHelperBundleConfig), "RegisterBundles")]

namespace GalaxyLottoWeb.AppStart
{
    public static class AjaxHelperBundleConfig
	{
		public static void RegisterBundles()
		{
			BundleTable.Bundles.Add(new ScriptBundle("~/bundles/ajaxhelper").Include("~/Scripts/jquery.ajaxhelper.js"));
		}
	}
}