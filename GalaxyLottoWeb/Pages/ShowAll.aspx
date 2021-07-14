<%@ Page Language="C#" Title="ShowAll" AutoEventWireup="true" EnableSessionState="True" EnableViewState="true" ViewStateMode="Enabled" CodeBehind="ShowAll.aspx.cs" Inherits="GalaxyLottoWeb.Pages.ShowAll" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="~/planet.ico" rel="shortcut icon" type="image/x-icon" />
    <webopt:BundleReference runat="server" Path="~/Content/css" />
    <title><%:Page.Title %></title>
</head>
<body id="body" class="glbody">
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
                <asp:Panel ID="pnlinfo" runat="server" Width="100%" ScrollBars="Auto" Direction="LeftToRight" HorizontalAlign="Center" BackColor="Black" ForeColor="White">
                    <asp:Label runat="server" ID="lblTitle" Font-Size="Small" />
                    <asp:Button ID="btnClose" runat="server" Text="關閉(C)" AccessKey="C" OnClientClick="window.close();" TabIndex="1" BackColor="Black" ForeColor="White" BorderColor="LightBlue" BorderWidth="1" />
                    <asp:Button ID="btnNewData" runat="server" Text="新增資料(N)" AccessKey="N" OnClick="BtnNewDataClick" BackColor="Black" ForeColor="White" BorderColor="LightBlue" BorderWidth="1" />
                </asp:Panel>
                <asp:Panel ID="pnlDetail" runat="server" CssClass="glColumn750" Height="850">
                    <asp:GridView ID="gvShowAll" runat="server" GridLines="Horizontal" AutoGenerateColumns="false" AllowSorting="false" CssClass="gltable "
                        PagerSettings-Visible="true"
                        ShowHeader="true" AllowPaging="true" PagerSettings-Position="Top" PageSize="28" ShowHeaderWhenEmpty="true" />
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
