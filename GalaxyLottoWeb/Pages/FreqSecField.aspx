<%@ Page Language="C#" Title="FreqSecField" AutoEventWireup="true" EnableSessionState="True" CodeBehind="FreqSecField.aspx.cs" Inherits="GalaxyLottoWeb.Pages.FreqSecField" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title><%:Page.Title %></title>
    <link href="FreqResult.ico" rel="shortcut icon" type="image/x-icon" />
    <webopt:BundleReference runat="server" Path="~/Content/css" />
    <%--    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundles/modernizr") %>
    </asp:PlaceHolder>--%>
</head>
<body class="glbody">
    <%--當期資料--%>
    <%--<asp:Button ID="btnClose" runat="server" Text="關閉" CssClass="btn glbutton-red glclosefix" OnClick="BtnCloseClick" OnClientClick="window.close();" />--%>
    <form runat="server">
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
        <asp:Timer ID="Timer1" OnTick="Timer1Tick" runat="server" Interval="10000" />
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
                <asp:Panel ID="pnlinfo" runat="server" Direction="LeftToRight" HorizontalAlign="Center" BackColor="Black" ForeColor="White" ScrollBars="Auto">
                    <details id="dtlInfo" runat="server" visible="true" class ="gldetails" >
                        <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />
                    </details>
                    <asp:Button ID="btnClose" runat="server" Text="關閉(C)" AccessKey="c" OnClick="BtnCloseClick" BackColor="Black" ForeColor="LightPink" BorderColor="LightPink" BorderWidth="1" />
                    <asp:DropDownList ID="ddlFreq" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" />
                    <asp:Label runat="server" ID="lblTitle" ForeColor="Yellow" Font-Size="Small" />
                    <%--                    <asp:CheckBox ID="chkFreqProcess" runat="server" Text="頻率來源" Font-Size="Medium" AutoPostBack="true" ForeColor="LightPink" />
                    <asp:CheckBox ID="chkFreqFilter01Process" runat="server" Text="機率01來源" Font-Size="Medium" AutoPostBack="true" ForeColor="LightSalmon" />--%>
                    <br />
                    <asp:Label runat="server" ID="lblMethod" ForeColor="Orange" Font-Size="Small" />
                    <br />
                    <asp:Label runat="server" ID="lblSearchMethod" ForeColor="LightGreen" Font-Size="Small" />
                    <br />
                    <asp:Label ID="lblArgument" runat="server" Font-Size="Small" ForeColor="LightGreen" />
                    <asp:Button ID="btnT1Start" runat="server" Text="T1更新中" Visible="false" OnClick="BtnT1StartClick" Font-Size="Smaller" ForeColor="LightGreen" BackColor="Black" BorderColor="Black" BorderWidth="0" />
                    <asp:Button ID="btnT2Start" runat="server" Text="T2更新中" Visible="false" OnClick="BtnT2StartClick" Font-Size="Smaller" ForeColor="LightGreen" BackColor="Black" BorderColor="Black" BorderWidth="0" />
                </asp:Panel>
                <%--搜尋結果--%>
                <asp:Panel ID="pnlDetail" runat="server" CssClass="glColumn750" EnableViewState="false" ViewStateMode="Disabled">
                    <asp:Label ID="lblFreqSecField" runat="server" Text="數字區間出現率" CssClass="gllabel" />
                    <asp:GridView ID="gvFreqSecField" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />

                    <details id="dtlFreqSec" runat="server" style="background-color: lightyellow">
                        <summary runat="server">數字區間表</summary>
                        <%--<asp:Label ID="lblFreqSec" runat="server" Text="數字區間表" CssClass="gllabel" />--%>
                        <asp:GridView ID="gvFreqSec" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                    </details>
                    <%--                    <asp:Label ID="lblFreqFilter01" runat="server" Text="機率01" CssClass="gllabel" />
                    <asp:GridView ID="gvFreqFilter01" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />

                    <asp:Panel ID="pnlFreqFilter01Process" runat="server" Visible="false" BackColor="#ffffea">
                        <asp:Label ID="lblFreqFilter01Process" runat="server" Text="機率01來源" CssClass="gllabel" />
                        <asp:GridView ID="gvFreqFilter01Process" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                        <hr />
                    </asp:Panel>--%>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
