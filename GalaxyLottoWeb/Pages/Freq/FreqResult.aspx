<%@ Page Language="C#" Title="頻率計算" EnableViewState="true" AutoEventWireup="true" CodeBehind="FreqResult.aspx.cs" Inherits="GalaxyLottoWeb.Pages.Freq.FreqResult00" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title><%:Page.Title %></title>
    <link href="../FreqResult.ico" rel="shortcut icon" type="image/x-icon" />
    <webopt:BundleReference runat="server" Path="~/Content/css" />
</head>
<body class="glbody">
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="MainScript">
            <Scripts>
                <%--若要深入了解如何在 ScriptManager 中統合指令碼，請參閱 https://go.microsoft.com/fwlink/?LinkID=301884 --%>
                <%--架構指令碼--%>
                <asp:ScriptReference Name="MsAjaxBundle" />
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
                <asp:ScriptReference Name="jquery" />
                <asp:ScriptReference Name="bootstrap" />
                <asp:ScriptReference Name="glScript" />
                <%--網站指令碼--%>
            </Scripts>
        </asp:ScriptManager>
        <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdateFreqResult" runat="server">
            <ProgressTemplate>
                <asp:Image ID="imgProgress" runat="server" AlternateText="progress" ImageUrl="~/Pages/loader.gif" Width="32" />
                載入中 (Loading) ...           
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="UpdateFreqResult" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnlinfo" runat="server" Direction="LeftToRight" HorizontalAlign="Center" BackColor="Black" ForeColor="White" ScrollBars="Auto">
                    <details id="dtlInfo" runat="server" visible="true" class="gldetails">
                        <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />
                        <asp:Label ID="lblTitle" runat="server" ForeColor="Yellow" Font-Size="Small" />
                        <br />
                        <asp:Label ID="lblSearchMethod" runat="server" ForeColor="SteelBlue" Font-Size="Small" />
                    </details>
                    <asp:Button ID="btnClose" runat="server" Text="關閉(C)" AccessKey="c" OnClick="BtnCloseClick" BackColor="Black" ForeColor="LightPink" BorderColor="LightPink" BorderWidth="1" />
                    <asp:DropDownList ID="ddlFreq" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" OnSelectedIndexChanged="DdlFreq_SelectedIndexChanged" />
                    <asp:DropDownList ID="ddlNexts" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" Visible="false" />
                    <asp:Label ID="lblArgument" runat="server" Font-Size="Small" ForeColor="LightGreen" />
                    <br />
                    <asp:Label ID="lblMethod" runat="server" ForeColor="Orange" Font-Size="Small" />
                </asp:Panel>

                <%--搜尋結果--%>
                <asp:Panel ID="pnlDetail" runat="server">
                    <asp:Label ID="lblFreq" runat="server" Text="頻率" CssClass="gllabel" />
                    <asp:GridView ID="gvFreq" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                </asp:Panel>

                <details id="dtlProcess" runat="server" visible="true" class="gldetails">
                    <summary class="Title">頻率來源</summary>
                    <asp:Panel ID="pnlFreqProcess" runat="server" BackColor="White" ForeColor="Black" Font-Size="Medium">
                        <asp:GridView ID="gvFreqProcess" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                    </asp:Panel>
                </details>

                <%--                    <asp:Label ID="lblFreqFilter01" runat="server" Text="機率01" CssClass="gllabel" />
                    <asp:GridView ID="gvFreqFilter01" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />

                    <asp:Panel ID="pnlFreqFilter01Process" runat="server" Visible="false" BackColor="#ffffea">
                        <asp:Label ID="lblFreqFilter01Process" runat="server" Text="機率01來源" CssClass="gllabel" />
                        <asp:GridView ID="gvFreqFilter01Process" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                        <hr />
                    </asp:Panel>--%>
            </ContentTemplate>

        </asp:UpdatePanel>

    </form>
</body>
</html>
