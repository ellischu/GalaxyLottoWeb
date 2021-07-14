﻿<%@ Page Language="C#" Title="FreqActive02" Async="true" AutoEventWireup="true" EnableSessionState="True" CodeBehind="FreqActive02.aspx.cs" Inherits="GalaxyLottoWeb.Pages.FreqActive02" %>

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
        <%--<asp:Button ID="btnClose" runat="server" Text="關閉" CssClass="btn glbutton-red glclosefix" OnClick="BtnCloseClick" OnClientClick="window.close();" />--%>
        <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
            <ProgressTemplate>
                <asp:Image ID="imgProgress" runat="server" AlternateText="progress" ImageUrl="loader.gif" Width="32" />
                Processing...
           
            </ProgressTemplate>
        </asp:UpdateProgress>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnlinfo" runat="server" Direction="LeftToRight" HorizontalAlign="Center" BackColor="Black" ForeColor="White" ScrollBars="Auto">
                    <asp:ImageButton ID="ibInfo" runat="server" CssClass="glinformation" ImageUrl="~/Resources/Information.png" OnClick="IBInfoClick" />
                    <asp:DropDownList ID="ddlFreq" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" />
                    <asp:Label ID="lblTitle" runat="server" Font-Size="Small" />
                    <br />
                    <asp:Label ID="lblMethod" runat="server" Font-Size="Small" />
                    <asp:Label ID="lblSearchMethod" runat="server" Font-Size="Small" />
                </asp:Panel>

                <asp:Panel ID="pnlTopData" runat="server" ScrollBars="Auto" ForeColor="White" BackColor="Black" Visible="false">
                    <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />
                </asp:Panel>

                <asp:Panel ID="pnlDetail" runat="server" CssClass="glColumn750" EnableViewState="false" ViewStateMode="Disabled" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
