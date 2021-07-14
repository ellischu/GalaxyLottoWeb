<%@ Page Language="C#" Title="FreqActiveHTAll" AutoEventWireup="true" EnableSessionState="True" CodeBehind="FreqActiveHTAll.aspx.cs" Inherits="GalaxyLottoWeb.Pages.FreqActiveHTAll" %>

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
    <link href="FreqActiveH.ico" rel="shortcut icon" type="image/x-icon" />
    <style type="text/css">
        #UpdateProgress1, #UpdateProgress2 {
            width: 200px;
            background-color: #FFC080;
            position: absolute;
            bottom: 0px;
            left: 0px;
        }
    </style>
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
        <asp:Timer ID="Timer1" OnTick="Timer1Tick" runat="server" Interval="30000" />
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
                    <%--<asp:Button ID="btnClose" runat="server" Text="關閉" OnClick="BtnCloseClick" BackColor="Black" ForeColor="White" BorderColor="LightBlue" BorderWidth="1" />--%>
                    <asp:Button ID="btnReload" runat="server" Text="重載" OnClick="BtnReloadClick" BackColor="Black" ForeColor="LightGreen" BorderColor="LightGreen" BorderWidth="1" />
                    <asp:ImageButton ID="ibInfo" runat="server" CssClass="glinformation" ImageUrl="~/Resources/Information.png" OnClick="IBInfoClick" />
                    <asp:Label runat="server" ID="lblTitle" Font-Size="Small" />
                    <br />
                    <asp:Label ID="lblFreqFilter" runat="server" Font-Size="Small" />
                    <asp:DropDownList ID="ddlFieldName" runat="server" BackColor="Black" ForeColor="LightPink" AutoPostBack="true" />
                    <asp:Label ID="lblArgument" runat="server" Font-Size="Smaller" ForeColor="LightGreen" />
                </asp:Panel>
                <asp:Panel ID="pnlTopData" runat="server" ScrollBars="Auto" ForeColor="White" BackColor="Black" Visible="false">
                    <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />
                </asp:Panel>
                <asp:Panel ID="pnlDetail" runat="server" Height="850" CssClass="glColumn750" EnableViewState="true" ViewStateMode="Enabled">
                    <asp:GridView ID="gvFreqFilter" runat="server"
                        AllowPaging="true" PageSize="20" PagerSettings-Position="Top"
                        ShowHeader="true" ShowHeaderWhenEmpty="true"
                        AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                    <asp:GridView ID="gvFreqSecHis" runat="server" />
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>

</body>
</html>
