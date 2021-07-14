<%@ Page Language="C#" Title="FreqActiveHSingle" AutoEventWireup="true" EnableSessionState="True" CodeBehind="FreqActiveHSingle.aspx.cs" Inherits="GalaxyLottoWeb.Pages.FreqActiveHSingle" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title><%:Page.Title %></title>
    <link href="FreqActiveH.ico" runat="server" rel="shortcut icon" type="image/x-icon" />
    <webopt:BundleReference runat="server" Path="~/Content/css" />
    <%--    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundles/modernizr") %>
    </asp:PlaceHolder>--%>
</head>
<body id="body" class="glbody">
    <%--當期資料--%>
    <form runat="server">
        <asp:ScriptManager runat="server" ID="MainScript">
            <Scripts>
                <%--若要深入了解如何在 ScriptManager 中統合指令碼，請參閱 https://go.microsoft.com/fwlink/?LinkID=301884 --%>
                <%--架構指令碼--%>
                <asp:ScriptReference Name="MsAjaxBundle" />
                <asp:ScriptReference Name="respond" />
                <asp:ScriptReference Name="jquery" />
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
        <asp:Timer ID="Timer1" OnTick="Timer1Tick" runat="server" Interval="10000" Enabled="false" />
        <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel01" runat="server">
            <ProgressTemplate>
                <asp:Image ID="imgProgress" runat="server" AlternateText="progress" ImageUrl="loader.gif" Width="32" />
                Processing...
           
            </ProgressTemplate>
        </asp:UpdateProgress>

        <asp:UpdatePanel ID="UpdatePanel01" runat="server">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
            </Triggers>
            <ContentTemplate>
                <asp:Panel ID="pnlinfo" runat="server" Width="100%" ScrollBars="Auto" Direction="LeftToRight" HorizontalAlign="Center" BackColor="Black" ForeColor="White">
                    <details id="dtlInfo" runat="server" visible="true" class ="gldetails" >
                        <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />
                    </details>
                    <asp:Button ID="btnClose" runat="server" Text="關閉(C)" AccessKey="c" OnClick="BtnCloseClick" BackColor="Black" ForeColor="LightPink" BorderColor="LightPink" BorderWidth="1" />
                    <asp:Label runat="server" ID="lblTitle" ForeColor="Yellow" Font-Size="Small" />
                    <asp:CheckBox ID="chkShowProcess" runat="server" Visible="false" OnCheckedChanged="ChkShowProcessCheckedChanged" Text="顯示來源(S)" AccessKey="s" Font-Size="Medium" AutoPostBack="true" ForeColor="LightPink" />
                    <br />
                    <asp:Label runat="server" ID="lblMethod" ForeColor="Orange" Font-Size="Small" />
                    <br />
                    <asp:Label runat="server" ID="lblSearchMethod" ForeColor="LightGreen" Font-Size="Small" />
                    <%--<asp:CheckBox ID="chkFreqFilter01Process" runat="server" Text="機率01來源" Font-Size="Medium" AutoPostBack="true" ForeColor="LightSalmon" />
                    <asp:CheckBox ID="chkFreqFilter02" runat="server" Text="機率02(同支數)" Visible="false" Font-Size="Medium" AutoPostBack="true" ForeColor="LightYellow" />
                    <asp:CheckBox ID="chkFreqFilter02Process" runat="server" Text="機率02來源(同支數)" Visible="false" Font-Size="Medium" AutoPostBack="true" ForeColor="LightYellow" />--%>
                </asp:Panel>

                <asp:Panel ID="pnlFilter" runat="server" Width="100%" Direction="LeftToRight" HorizontalAlign="Center" ForeColor="White" BackColor="Black" BorderColor="LightBlue">
                    <asp:PlaceHolder ID="phHistoryPeriods" runat="server">
                        <asp:Label ID="lblHistoryPeriods" runat="server" Text="歷史搜尋期數" Font-Size="Medium" />
                        <asp:TextBox ID="txtHistoryPeriods" runat="server" Text="30" Font-Size="Medium" AutoPostBack="true" CssClass="gltext" Width="30px" />
                        <asp:ImageButton ID="ibHistoryPeriods" runat="server" ImageUrl="~/Resources/btncancel.png" BackColor="White" CssClass="glclear" OnClick="IBHistoryPeriodsClick" />
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phHistoryTestPeriods" runat="server">
                        <asp:Label ID="lblHistoryTestPeriods" runat="server" Text="往前驗證期數" Font-Size="Medium" />
                        <asp:TextBox ID="txtHistoryTestPeriods" runat="server" Text="10" Font-Size="Medium" AutoPostBack="true" CssClass="gltext" Width="30px" />
                        <asp:ImageButton ID="ibHistoryTestPeriods" runat="server" ImageUrl="~/Resources/btncancel.png" BackColor="White" CssClass="glclear" OnClick="IBHistoryTestPeriodsClick" />
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phHitMin" runat="server">
                        <asp:Label ID="lblHitMin" runat="server" Text="最小中獎數" Font-Size="Medium" />
                        <asp:TextBox ID="txtHitMin" runat="server" Text="2" Font-Size="Medium" AutoPostBack="true" CssClass="gltext" Width="20px" />
                        <asp:ImageButton ID="ibHitMin" runat="server" ImageUrl="~/Resources/btncancel.png" BackColor="White" CssClass="glclear" OnClick="IBHitMinClick" />
                    </asp:PlaceHolder>
                    <asp:Label ID="lblArgument" runat="server" Font-Size="Small" ForeColor="LightGreen" />
                    <asp:Button ID="btnT1Start" runat="server" Text="T1更新中" Visible="false" OnClick="BtnT1StartClick" Font-Size="Smaller" ForeColor="LightGreen" BackColor="Black" BorderColor="Black" BorderWidth="0" />
                    <asp:Button ID="btnT2Start" runat="server" Text="T2更新中" Visible="false" OnClick="BtnT2StartClick" Font-Size="Smaller" ForeColor="LightGreen" BackColor="Black" BorderColor="Black" BorderWidth="0" />
                    <br />
                    <asp:CheckBox ID="chkFilterRange" runat="server" Text="次數篩選" Font-Size="Medium" ForeColor="Yellow" TextAlign="Right" AutoPostBack="true" OnCheckedChanged="ChkFilterRangeCheckedChanged" />
                    <asp:PlaceHolder ID="phFilterRange" runat="server" Visible="false">
                        <asp:Label ID="lblFilterRange" runat="server" Text="指定篩選值" Font-Size="Medium" ForeColor="Yellow" />
                        <asp:TextBox ID="txtFilterRange" runat="server" Text="" Font-Size="Medium" Width="80" CssClass="gltext" />
                        <asp:ImageButton ID="ibFilterRange" runat="server" ImageUrl="~/Resources/btncancel.png" BackColor="White" CssClass="glclear" OnClick="IBFilterRangeClick" />
                        <asp:Label ID="lblFilterMin" runat="server" Text="最小篩選值" Font-Size="Medium" ForeColor="Yellow" />
                        <asp:TextBox ID="txtFilterMin" runat="server" Text="0" Font-Size="Medium" Width="30" CssClass="gltext" />
                        <asp:ImageButton ID="ibFilterMin" runat="server" ImageUrl="~/Resources/btncancel.png" BackColor="White" CssClass="glclear" OnClick="IBFilterMinClick" />
                        <asp:Label ID="lblFilterMax" runat="server" Text="最大篩選值" Font-Size="Medium" ForeColor="Yellow" />
                        <asp:TextBox ID="txtFilterMax" runat="server" Text="0" Font-Size="Medium" Width="50" CssClass="gltext" />
                        <asp:ImageButton ID="ibFilterMax" runat="server" ImageUrl="~/Resources/btncancel.png" BackColor="White" CssClass="glclear" OnClick="IBFilterMaxClick" />
                    </asp:PlaceHolder>
                    <asp:Button ID="btnFilter" runat="server" OnClick="BtnFreqClick" Font-Size="Medium" TabIndex="1" BackColor="Black" ForeColor="LightBlue" BorderColor="LightBlue" BorderWidth="1" />
                    <asp:Button ID="btnQuickFilter" runat="server" Text="其他驗證期數" Visible="false" OnClick="BtnQuickFilterClick" Font-Size="Medium" TabIndex="2" BackColor="Black" ForeColor="LightBlue" BorderColor="LightBlue" BorderWidth="1" />
                </asp:Panel>

                <asp:Panel ID="pnlDetail" runat="server" Height="730" CssClass="glColumn750" EnableViewState="false" ViewStateMode="Disabled">
                    <asp:Label ID="lblFreq" runat="server" Text="頻率" CssClass="gllabel" />
                    <asp:GridView ID="gvFreq" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />

                    <details id="dtlFreqProcess" runat="server" visible="false" style="background-color: white; color: black; font-size: small;">
                        <summary style="background-color: lightgray; color: brown;">頻率來源</summary>
                        <%--<asp:Label ID="lblFreqProcess" runat="server" Text="頻率來源" CssClass="gllabel" />--%>
                        <asp:Panel ID="pnlFreqProcess" runat="server">
                            <asp:GridView ID="gvFreqProcess" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                            <hr />
                        </asp:Panel>
                    </details>

                    <asp:Label ID="lblFreqFilter01" runat="server" Text="機率01" CssClass="gllabel" />
                    <asp:GridView ID="gvFreqFilter01" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />

                    <details id="dtlFreqFilter01Process" runat="server" visible="false" style="background-color: white; color: black; font-size: small;">
                        <summary style="background-color: lightgray; color: brown;">機率01來源</summary>
                        <asp:Panel ID="pnlFreqFilter01Process" runat="server" BackColor="#ffffea">
                            <%--<asp:Label ID="lblFreqFilter01Process" runat="server" Text="機率01來源" CssClass="gllabel" />--%>
                            <asp:GridView ID="gvFreqFilter01Process" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                            <hr />
                        </asp:Panel>
                    </details>

                    <asp:Label ID="lblFreqFilter02" runat="server" Text="機率02(同支數)" CssClass="gllabel" />
                    <asp:GridView ID="gvFreqFilter02" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                    <%--<asp:Panel ID="pnlFreqFilter02" runat="server">
                    </asp:Panel>--%>

                    <details id="dtlFreqFilter02Process" runat="server" visible="false" style="background-color: white; color: black; font-size: small;">
                        <summary style="background-color: lightgray; color: brown;">機率02(同支數)來源</summary>
                        <asp:Panel ID="pnlFreqFilter02Process" runat="server" BackColor="#ffffea">
                            <%--<asp:Label ID="lblFreqFilter02Process" runat="server" Text="機率02(同支數)來源" CssClass="gllabel" />--%>
                            <asp:GridView ID="gvFreqFilter02Process" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                            <hr />
                        </asp:Panel>
                    </details>

                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
