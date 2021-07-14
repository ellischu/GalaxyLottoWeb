using System;
using System.Web.Optimization;
using System.Web.UI;

namespace GalaxyLottoWeb
{
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    public class BundleConfig
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
        // 如需統合的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=303951
        public static void RegisterBundles(BundleCollection bundles)
        {
            if (bundles == null) { throw new ArgumentNullException(nameof(bundles)); }

            bundles.Add(new ScriptBundle("~/bundles/WebFormsJs").Include(
                            "~/Scripts/WebForms/WebForms.js",
                            "~/Scripts/WebForms/WebUIValidation.js",
                            "~/Scripts/WebForms/MenuStandards.js",
                            "~/Scripts/WebForms/Focus.js",
                            "~/Scripts/WebForms/GridView.js",
                            "~/Scripts/WebForms/DetailsView.js",
                            "~/Scripts/WebForms/TreeView.js",
                            "~/Scripts/WebForms/WebParts.js"));

            // 順序對於這些檔案產生作用而言相當重要，它們有明確的相依性
            bundles.Add(new ScriptBundle("~/bundles/MsAjaxJs").Include(
                            "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
                            "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
                            "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
                            "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"));

            // 使用 Modernizr 的開發版本來開發並深入了解。當您準備好量產時，
            // 準備好生產時，請使用 https://modernizr.com 中的建置工具，只挑選您需要的測試
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                            "~/Content/galaxylotto.css",
                            "~/Content/bootstrap.css",
                            "~/Content/jquery.datetimepicker.css",
                            "~/Content/Site.css"));

            ScriptManager.ScriptResourceMapping.AddDefinition(
                "respond",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/respond.min.js",
                    DebugPath = "~/Scripts/respond.js",
                });
            ScriptManager.ScriptResourceMapping.AddDefinition(
                "bootstrap",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/bootstrap.min.js",
                    DebugPath = "~/Scripts/bootstrap.js",
                });
            ScriptManager.ScriptResourceMapping.AddDefinition(
                "glScript",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/glScript.js",
                    DebugPath = "~/Scripts/glScript.js"
                });
            ScriptManager.ScriptResourceMapping.AddDefinition(
                "jDateTimePicker",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/jquery.datetimepicker.js",
                    DebugPath = "~/Scripts/jquery.datetimepicker.js"
                });
            string str = "3.5.1";
            ScriptManager.ScriptResourceMapping.AddDefinition(
                "jquery",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/jquery-" + str + ".min.js",
                    DebugPath = "~/Scripts/jquery-" + str + ".js",
                    CdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-" + str + ".min.js",
                    CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-" + str + ".js",
                    CdnSupportsSecureConnection = true,
                    LoadSuccessExpression = "window.jQuery"
                });
        }
    }
}