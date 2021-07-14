<%@ Page Language="C#" Title="Solar24" AutoEventWireup="true" EnableSessionState="True" EnableViewState="true" ViewStateMode="Enabled" CodeBehind="UpdateSolar24.aspx.cs" Inherits="GalaxyLottoWeb.Pages.Solar24" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="~/planet.ico" rel="shortcut icon" type="image/x-icon" />
    <webopt:BundleReference runat="server" Path="~/Content/css" />
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
    <title><%:Page.Title %></title>
</head>
<body id="body" class="glbody">

    <form runat="server">
        <asp:ScriptManager runat="server" ID="MainScript" EnableScriptGlobalization="true" EnableScriptLocalization="false">
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
                <asp:ScriptReference Name="jDateTimePicker" />
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
                    <asp:Label runat="server" ID="lblTitle" Text="節氣更新" CssClass="gllabel" />
                    <%--                    <asp:Button ID="btnClose" runat="server" Text="關閉(C)" AccessKey="C" CssClass="glbutton" OnClientClick="window.close();"
                        TabIndex="1" BackColor="Black" ForeColor="White" BorderColor="LightBlue" BorderWidth="1" />--%>
                </asp:Panel>
                <asp:Panel ID="pnlDate" runat="server" Direction="LeftToRight">
                    <asp:Label ID="lblSolar24" runat="server" Text="節氣" CssClass="gllabel"></asp:Label>
                    <asp:DropDownList ID="ddlSolar24" runat="server" CssClass="gldropdownlist" Width="50" />
                    <asp:Label ID="lblDate" runat="server" Text="日期" CssClass="gllabel"></asp:Label>
                    <asp:TextBox ID="txtDate" runat="server" CssClass="gltext" AutoPostBack="true" Width="100"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="txtDate_CalendarExtender" runat="server" BehaviorID="txtDate_CalendarExtender" TargetControlID="txtDate"
                        Animated="true" DaysModeTitleFormat="yyyy/MM/dd " Format="yyyy/MM/dd" TodaysDateFormat="yyyy/MM/dd"
                        FirstDayOfWeek="Monday" />
                    <asp:Label ID="lblTime" runat="server" Text="時間" CssClass="gllabel" />
                    <asp:DropDownList ID="ddlHour" runat="server" CssClass="gldropdownlist" Width="50" />
                    <span>：</span>
                    <asp:DropDownList ID="ddlMin" runat="server" CssClass="gldropdownlist" Width="50" />
                    <asp:Button ID="btnUpdate" runat="server" Text="更新(U)" AccessKey="u" CssClass="glbutton" TabIndex="1"
                        BackColor="Black" ForeColor="White" BorderColor="LightBlue" BorderWidth="1"
                        OnClick="BtnUpdate_Click" />
                </asp:Panel>
                <asp:Label ID="lblYear" runat="server" Text="" CssClass="gllabel" />

                <asp:Panel ID="pnlSoalr24" runat="server" />
                <%--                    <asp:Panel ID="pnlCalender" runat="server" CssClass="glCalender">
                        <asp:Calendar ID="Calendar01" runat="server"
                            OnSelectionChanged="Calendar01_SelectionChanged" OnDayRender="Calendar01_DayRender" FirstDayOfWeek="Monday">
                            <TodayDayStyle BackColor="Red" ForeColor="Yellow" />
                            <WeekendDayStyle BackColor="LightGreen" ForeColor="red" />
                        </asp:Calendar>
                    </asp:Panel>--%>

                <%--                <script>
                    jQuery(document).ready(function () {
                        'use strict';
                        jQuery('#demo, #txtDate').datetimepicker({ format: 'Y/m/d H:i', step: 1 });
                    });
                </script>--%>
            </ContentTemplate>
        </asp:UpdatePanel>

    </form>
</body>
</html>
