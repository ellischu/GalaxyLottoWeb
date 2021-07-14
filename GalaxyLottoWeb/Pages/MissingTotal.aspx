<%@ Page Language="C#" Title="MissTotal" AutoEventWireup="true" EnableSessionState="True" CodeBehind="MissingTotal.aspx.cs" Inherits="GalaxyLottoWeb.Pages.MissingTotal" %>

<%@ OutputCache Duration="600" VaryByParam="action;id" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title><%:Page.Title %></title>
    <webopt:BundleReference runat="server" Path="~/Content/css" />
    <link href="MissingTotal.ico" rel="shortcut icon" type="image/x-icon" />
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundles/modernizr") %>
    </asp:PlaceHolder>
</head>
<body id="body" class="glbody">
    <%--當期資料--%>
    <form runat="server">
        <asp:ScriptManager runat="server" ID="MainScript">
            <Scripts>
                <%--若要深入了解如何在 ScriptManager 中統合指令碼，請參閱 https://go.microsoft.com/fwlink/?LinkID=301884 --%>
                <%--架構指令碼--%>
                <asp:ScriptReference Name="MsAjaxBundle" />
                <asp:ScriptReference Name="jquery" />
                <asp:ScriptReference Name="respond" />
                <asp:ScriptReference Name="WebForms.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebForms.js" />
                <asp:ScriptReference Name="WebUIValidation.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebUIValidation.js" />
                <asp:ScriptReference Name="MenuStandards.js" Assembly="System.Web" Path="~/Scripts/WebForms/MenuStandards.js" />
                <asp:ScriptReference Name="GridView.js" Assembly="System.Web" Path="~/Scripts/WebForms/GridView.js" />
                <asp:ScriptReference Name="DetailsView.js" Assembly="System.Web" Path="~/Scripts/WebForms/DetailsView.js" />
                <asp:ScriptReference Name="TreeView.js" Assembly="System.Web" Path="~/Scripts/WebForms/TreeView.js" />
                <asp:ScriptReference Name="WebParts.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebParts.js" />
                <asp:ScriptReference Name="Focus.js" Assembly="System.Web" Path="~/Scripts/WebForms/Focus.js" />
                <asp:ScriptReference Name="WebFormsBundle" />
                <asp:ScriptReference Name="bootstrap" />
                <asp:ScriptReference Name="glScript" />
                <%--網站指令碼--%>
            </Scripts>
        </asp:ScriptManager>
        <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
            <ProgressTemplate>
                <asp:Image ID="imgProgress" runat="server" AlternateText="progress" ImageUrl="loader.gif" Width="32" />
                Processing...
           
            </ProgressTemplate>
        </asp:UpdateProgress>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnlinfo" runat="server" Width="100%" ScrollBars="Auto" HorizontalAlign="Center" BackColor="Black" ForeColor="White" Direction="LeftToRight">
                    <details id="dtlInfo" runat="server" visible="true" class="gldetails">
                        <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />
                        <asp:Label runat="server" ID="lblTitle" ForeColor="Yellow" Font-Size="Small" />
                        <br />
                        <asp:Label runat="server" ID="lblSearchMethod" ForeColor="LightGreen" Font-Size="Small" />
                    </details>
                    <asp:Button ID="btnClose" runat="server" Text="關閉(C)" AccessKey="c" OnClick="BtnCloseClick" BackColor="Black" ForeColor="LightPink" BorderColor="LightPink" BorderWidth="1" />
                    <asp:CheckBox ID="ckAns" runat="server" Text="顯示答案" CssClass=" glcheck glShowAns" AutoPostBack="true" />
                    <asp:CheckBox ID="chkOrder" runat="server" Text="倒序" CssClass=" glcheck glShowAns" AutoPostBack="true" />
                    <asp:DropDownList ID="ddlFreq" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" />
                    <asp:Label runat="server" ID="lblMethod" ForeColor="Orange" Font-Size="Small" />
                </asp:Panel>
                <asp:Panel ID="pnlDetail" runat="server" EnableViewState="false" ViewStateMode="Disabled" />
                <%--CssClass="glColumn750"--%>
                <div id="scrolltop" class="font" style="visibility: hidden;">
                    <a title="返回頂部" onclick="window.scrollTo('0','0')" class="iconBox barIcon scrollIcon goTopIcon">
                        <p class="scrolltoptext">TOP</p>
                    </a>
                </div>
                <script type="text/javascript">_attachEvent(window, 'scroll', function () { showTopLink(); }); checkBlind();</script>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
