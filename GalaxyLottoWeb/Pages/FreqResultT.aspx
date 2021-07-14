<%@ Page Language="C#" Title="FreqResultT" AutoEventWireup="true" CodeBehind="FreqResultT.aspx.cs" Inherits="GalaxyLottoWeb.Pages.FreqResultT" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title><%:Page.Title %></title>
    <link href="FreqResultT.ico" runat="server" rel="shortcut icon" type="image/x-icon" />
    <webopt:BundleReference runat="server" Path="~/Content/css" />
</head>
<body class="glbody">
    <form runat="server">

        <asp:ScriptManager runat="server">
            <Scripts>
                <asp:ScriptReference Name="jquery" />
                <asp:ScriptReference Name="bootstrap" />
                <asp:ScriptReference Name="glScript" />
            </Scripts>
        </asp:ScriptManager>

        <asp:Timer ID="Timer1" OnTick="Timer1Tick" runat="server" Interval="10000" Enabled="false" />

        <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel01" runat="server">
            <ProgressTemplate>
                <asp:Image ID="imgProgress" runat="server" AlternateText="progress" ImageUrl="loader.gif" Width="32" />
                Processing...
           
            </ProgressTemplate>
        </asp:UpdateProgress>

        <asp:UpdatePanel ID="UpdatePanel01" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnlFilter" runat="server" CssClass="glFilterfix" ScrollBars="auto" Visible="false">
                    <asp:GridView ID="gvFilter01" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                    <asp:Button ID="btnYes" runat="server" AccessKey="y" CssClass="glbutton-black-lightblue" OnClick="BtnYesClick" />
                    <asp:Button ID="btnCancel" runat="server" AccessKey="e" CssClass="glbutton-black-lightblue" OnClick="BtnCancelClick" />
                </asp:Panel>

                <asp:Panel ID="pnlinfo" runat="server" Width="100%" ScrollBars="Auto" HorizontalAlign="Center" BackColor="Black" ForeColor="White" Direction="LeftToRight">
                    <details id="dtlInfo" runat="server" visible="true" class ="gldetails" >
                        <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />
                    </details>

                    <asp:Button ID="btnClose" runat="server" Text="關閉(C)" AccessKey="c" OnClick="BtnCloseClick" BackColor="Black" ForeColor="LightPink" BorderColor="LightPink" BorderWidth="1" TabIndex="100" />
                    <asp:Button ID="btnReload" runat="server" Text="重載(R)" AccessKey="r" OnClick="BtnReloadClick" BackColor="Black" ForeColor="LightGreen" BorderColor="LightBlue" BorderWidth="1" TabIndex="101" />
                    <%--<asp:ImageButton ID="ibInfo" runat="server" CssClass="glinformation" ImageUrl="~/Resources/Information.png" OnClick="IBInfoClick" />--%>
                    <asp:Label runat="server" ID="lblTitle" Font-Size="Small" />
                    <asp:CheckBox ID="chkFRTSerchOrder" runat="server" Text="排程" TextAlign="Right" AutoPostBack="true" CssClass="glcheck" />
                    <asp:Button ID="btnquickSearchOrder" runat="server" Text="快速排程" Visible="false" OnClick="BtnquickSearchOrderClick" Font-Size="Medium" BackColor="Black" ForeColor="LightBlue" BorderColor="LightBlue" BorderWidth="1" TabIndex="102" />
                    <asp:Button ID="btnShowFRTSearchOrder" runat="server" Text="啟動排程" OnClientClick="window.open('FRTSearchOrder.aspx','FRTSearchOrder');" Font-Size="Medium" BackColor="Black" ForeColor="LightBlue" BorderColor="LightBlue" BorderWidth="1" TabIndex="103" />
                    <asp:Label ID="lblArgument" runat="server" Font-Size="Smaller" ForeColor="LightGreen" />
                    <asp:Button ID="btnT1Start" runat="server" Text="T1更新中" OnClick="BtnT1StartClick" Font-Size="Smaller" ForeColor="LightGreen" BackColor="Black" BorderColor="Black" BorderWidth="0" />
                    <br />
                    <asp:Label ID="lblQryFreqFilters" runat="server" />
                    <asp:Button ID="btnFilter" runat="server" Text="篩選(L)" AccessKey="l" Visible="false" OnClick="BtnFilterClick" BackColor="Black" ForeColor="LightBlue" BorderColor="LightBlue" BorderWidth="1" TabIndex="104" />
                    <asp:Button ID="btnOneKeyFilter" runat="server" Text="一鍵篩選(O)" AccessKey="o" Visible="false" OnClick="BtnOneKeyFilterClick" BackColor="Black" ForeColor="LightBlue" BorderColor="LightBlue" BorderWidth="1" TabIndex="0" />
                    <asp:Button ID="btnAIFilter" runat="server" Text="AI篩選" Visible="false" OnClick="BtnAIFilterClick" BackColor="Black" ForeColor="LightBlue" BorderColor="LightBlue" BorderWidth="1" TabIndex="105" />
                    <asp:Button ID="btnNoFilter" runat="server" Text="取消篩選(N)" Visible="false" AccessKey="n" OnClick="BtnNoFilterClick" BackColor="Black" ForeColor="LightPink" BorderColor="LightBlue" BorderWidth="1" TabIndex="106" />
                </asp:Panel>
                <%--                <asp:Panel ID="pnlTopData" runat="server" ScrollBars="Auto" ForeColor="White" BackColor="Black" Visible="false">
                </asp:Panel>--%>
                <asp:Panel ID="pnlDetail" runat="server" CssClass="glColumn750" Height="850" EnableViewState="true" ViewStateMode="Enabled">
                    <asp:GridView ID="gvQryFreqFilters" runat="server"
                        AllowPaging="true" PageSize="20" PagerSettings-Position="Top"
                        ShowHeader="true" ShowHeaderWhenEmpty="true"
                        AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                    <asp:GridView ID="gvFreqSum" runat="server" AutoGenerateColumns="false" CssClass="gltable" GridLines="Horizontal" />
                </asp:Panel>

            </ContentTemplate>
        </asp:UpdatePanel>

    </form>
</body>
</html>
