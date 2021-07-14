﻿<%@ Page Language="C#" Title="SmartTestResult" AutoEventWireup="true" EnableSessionState="True" CodeBehind="SmartTestResult.aspx.cs" Inherits="GalaxyLottoWeb.Pages.SmartTestResult" %>

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
    <link href="~/planet.ico" rel="shortcut icon" type="image/x-icon" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
</head>
<body class="glbody">
    <%--當期資料--%>
    <form id="formFreq" runat="server">
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
        <asp:Panel ID="pnlTopData" runat="server" CssClass="glTopdatafix">
            <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />
            <asp:Panel ID="pnlButtons" runat="server" CssClass="glbuttons" />
        </asp:Panel>
        <asp:Label runat="server" CssClass="gllabel"> <%:Page.Title %></asp:Label>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" class="glresult">
            <ContentTemplate>
                <asp:Panel ID="pnlDetail" runat="server" CssClass="glColumn750" EnableViewState="false" ViewStateMode="Disabled" />

                <%--搜尋結果--%>
                <asp:PlaceHolder ID="phSmart" runat="server"></asp:PlaceHolder>
                <%--詳細列表--%>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
