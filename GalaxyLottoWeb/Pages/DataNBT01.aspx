﻿<%@ Page Language="C#" Title="DataNBT01" Async="true" AutoEventWireup="true" EnableSessionState="True" CodeBehind="DataNBT01.aspx.cs" Inherits="GalaxyLottoWeb.Pages.DataNBT01" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="~/planet.ico" rel="shortcut icon" type="image/x-icon" />
    <webopt:BundleReference runat="server" Path="~/Content/css" />
    <title><%:Page.Title %></title>
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundles/modernizr") %>
    </asp:PlaceHolder>
</head>
<body id="body" class="glbody">
    <%--當期資料--%>
    <form id="formDataNBT01" runat="server">
        <%--class="glresult"--%>
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
                <asp:Panel ID="pnlinfo" runat="server" CssClass="pnlinfo" ScrollBars="Auto" HorizontalAlign="Center">
                    <details id="dtlInfo" runat="server" visible="true" class="gldetails">
                        <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />
                        <asp:Label runat="server" ID="lblTitle" CssClass="glTitle" />
                        <br />
                        <asp:Label runat="server" ID="lblSearchMethod" CssClass="glSearchMethod" />
                    </details>
                    <asp:Button ID="btnClose" runat="server" Text="關閉(C)" AccessKey="c" OnClick="BtnCloseClick" CssClass="glbtnClose glbutton glbutton-red" />
                    <asp:Label runat="server" ID="lblMethod" CssClass="glMethod" />
                    <asp:Label ID="lblArgument" runat="server" Font-Size="Smaller" ForeColor="LightGreen" />
                    <asp:Button ID="btnT1Start" runat="server" Text="T1更新中" Visible="false" OnClick="BtnT1StartClick" Font-Size="Smaller" ForeColor="LightGreen" BackColor="Black" BorderColor="Black" BorderWidth="0" />

                    <asp:DropDownList ID="ddlOption" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true">
                        <asp:ListItem Text="總合" Value="OptionSum" Selected="True" />
                        <asp:ListItem Text="總合統計" Value="OptionSum01" />
                        <asp:ListItem Text="振盪" Value="OptionDataN" />
                        <asp:ListItem Text="平衡" Value="OptionDataB" />
                    </asp:DropDownList>

                    <asp:DropDownList ID="ddlFields" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" Visible="false" />

                    <asp:DropDownList ID="ddlDataN" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" Visible="false">
                        <asp:ListItem Text="振盪預測表" Value="DataNNext" Selected="True" />
                        <asp:ListItem Text="振盪表" Value="DataN" />
                        <asp:ListItem Text="振盪圖表" Value="DataNChart" />
                    </asp:DropDownList>

                    <asp:DropDownList ID="ddlDataB" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" Visible="false">
                        <asp:ListItem Text="平衡預測表" Value="DataBNext" Selected="True" />
                        <asp:ListItem Text="平衡表" Value="DataB" />
                        <asp:ListItem Text="平衡圖表" Value="DataBChart" />
                    </asp:DropDownList>

                    <asp:DropDownList ID="ddlFreq" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" Visible="false" />
                    <asp:DropDownList ID="ddlNums" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" Visible="false" />

                    <asp:DropDownList ID="ddlPercent" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" Visible="false">
                        <asp:ListItem Text="100" Value="100" Selected="True" />
                        <asp:ListItem Text="95" Value="95" />
                        <asp:ListItem Text="90" Value="90" />
                        <asp:ListItem Text="75" Value="75" />
                        <asp:ListItem Text="50" Value="50" />
                        <asp:ListItem Text="20" Value="20" />
                    </asp:DropDownList>

                    <asp:DropDownList ID="ddlDays" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" Visible="false">
                        <asp:ListItem Text="3" Value="3" />
                        <asp:ListItem Text="6" Value="6" />
                        <asp:ListItem Text="9" Value="9" Selected="True" />
                        <asp:ListItem Text="12" Value="12" />
                        <asp:ListItem Text="18" Value="18" />
                        <asp:ListItem Text="24" Value="24" />
                    </asp:DropDownList>

                </asp:Panel>
                <asp:Label ID="lblBriefDate" runat="server" CssClass="NotDisplay" Font-Size="X-Large" />
                <asp:Panel ID="pnlDetail" runat="server" />
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
