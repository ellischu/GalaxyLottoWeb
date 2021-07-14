<%@ Page Language="C#" Title="FreqActive03" AutoEventWireup="true" EnableSessionState="True" CodeBehind="FreqActive03.aspx.cs" Inherits="GalaxyLottoWeb.Pages.FreqActive03" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title><%:Page.Title %></title>
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundles/modernizr") %>
    </asp:PlaceHolder>
    <webopt:BundleReference runat="server" Path="~/Content/css" />
    <link href="FreqActive.ico" rel="shortcut icon" type="image/x-icon" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
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

        <asp:Timer ID="Timer1" OnTick="Timer1Tick" runat="server" Interval="10000" Enabled="false" />

        <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
            <ProgressTemplate>
                <asp:Image ID="imgProgress" runat="server" AlternateText="progress" ImageUrl="loader.gif" Width="32" />
                Processing...
           
            </ProgressTemplate>
        </asp:UpdateProgress>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
            </Triggers>
            <ContentTemplate>
                <asp:Panel ID="pnlinfo" runat="server" Width="100%" ScrollBars="Auto" Direction="LeftToRight" HorizontalAlign="Center" BackColor="Black" ForeColor="White">
                    <details id="dtlInfo" runat="server" visible="true" class ="gldetails" >
                        <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />
                    </details>
                    <asp:Button ID="btnClose" runat="server" Text="關閉(C)" AccessKey="c" OnClick="BtnCloseClick" BackColor="Black" ForeColor="LightPink" BorderColor="LightPink" BorderWidth="1" />
                    <asp:DropDownList ID="ddlFreq" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" />
                    <asp:Label ID="lblTitle" runat="server" ForeColor="Yellow" Font-Size="Small" />
                    <asp:Label ID="lblMethod" runat="server" ForeColor="Orange" Font-Size="Small" />
                    <br />
                    <asp:Label ID="lblSearchMethod" runat="server" ForeColor="LightGreen" Font-Size="Small" />
                    <asp:Label ID="lblArgument" runat="server" Font-Size="Smaller" ForeColor="LightGreen" />
                    <asp:Button ID="btnT1Start" runat="server" Text="T1更新中" Visible="false" OnClick="BtnT1StartClick" Font-Size="Smaller" ForeColor="LightGreen" BackColor="Black" BorderColor="Black" BorderWidth="0" />
                </asp:Panel>
                <asp:Panel ID="pnlDetail" runat="server" CssClass="glColumn750">
                    <asp:GridView runat="server" ID="gvActiveWithSection" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                    <asp:GridView runat="server" ID="gvLastNumsWithSection" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
