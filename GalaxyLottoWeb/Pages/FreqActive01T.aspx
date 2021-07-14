<%@ Page Language="C#" Title="FreqActive01T" Async="true" AutoEventWireup="true" EnableSessionState="True" CodeBehind="FreqActive01T.aspx.cs" Inherits="GalaxyLottoWeb.Pages.FreqActive01T" %>

<%@ OutputCache Duration="600" VaryByParam="action;id" %>
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
    <style>
        .Percent {
            background-color: rgba(10, 200, 252, 0.35);
        }

        .TotalPercent {
            background-color: rgba(64, 252, 10, 0.35);
        }

        .BackGroundBlue {
            background-color: rgba(33, 173, 243, 0.45)
        }

        .BackGroundGreen {
            background-color: rgba(0, 215, 18, 0.45)
        }

        .fullup {
            background-color: rgba(10, 0, 255, 0.40) !important;
        }
    </style>

</head>
<body id="body" class="glbody">
    <%--當期資料--%>
    <form id="formDataN02" runat="server" class="glresult">
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
                    </details>
                    <asp:Button ID="btnClose" runat="server" Text="關閉(C)" AccessKey="c" OnClick="BtnCloseClick" BackColor="Black" ForeColor="LightPink" BorderColor="LightPink" BorderWidth="1" />
                    <asp:CheckBox ID="ckAns" runat="server" Text="答案" Checked="true" CssClass=" glcheck glShowAns" AutoPostBack="true" />
                    <asp:CheckBox ID="chkFullup" runat="server" Text="全上升" CssClass=" glcheck glShowAns" AutoPostBack="true" />
                    <asp:DropDownList ID="ddlOption" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true">
                        <asp:ListItem Text="活性表01" Value="OptionFreqActive01" Selected="True" />
                        <asp:ListItem Text="機率表01" Value="OptionPercent01" />
                        <asp:ListItem Text="機率表02" Value="OptionPercent02" />
                        <asp:ListItem Text="總合" Value="OptionSum" />
                    </asp:DropDownList>

                    <asp:DropDownList ID="ddlFields" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" Visible="false" />
                    <asp:Label runat="server" ID="lblTitle" ForeColor="Yellow" Font-Size="Small" />
                    <asp:Label runat="server" ID="lblMethod" ForeColor="Orange" Font-Size="Small" />
                    <br />
                    <asp:Label runat="server" ID="lblSearchMethod" ForeColor="LightGreen" Font-Size="Small" />
                    <asp:Panel ID="pnlPercent" runat="server" HorizontalAlign="Center" Visible="false">
                        <asp:CheckBox ID="chkLngM" runat="server" Text="lngM" CssClass="glcheck " AutoPostBack="true" TextAlign="Right" OnCheckedChanged="ChangeLabelColor" />
                        <asp:CheckBox ID="chksglFreq05" runat="server" Text="sglFreq05" Checked="true" CssClass="glcheck " AutoPostBack="true" TextAlign="Right" OnCheckedChanged="ChangeLabelColor" />
                        <asp:CheckBox ID="chksglAC05" runat="server" Text="sglAC05" CssClass="glcheck " AutoPostBack="true" TextAlign="Right" OnCheckedChanged="ChangeLabelColor" />
                        <asp:CheckBox ID="chksglFreq10" runat="server" Text="sglFreq10" Checked="true" CssClass="glcheck " AutoPostBack="true" TextAlign="Right" OnCheckedChanged="ChangeLabelColor" />
                        <asp:CheckBox ID="chksglAC10" runat="server" Text="sglAC10" CssClass="glcheck " AutoPostBack="true" TextAlign="Right" OnCheckedChanged="ChangeLabelColor" />
                        <asp:CheckBox ID="chksglFreq25" runat="server" Text="sglFreq25" CssClass="glcheck " AutoPostBack="true" TextAlign="Right" OnCheckedChanged="ChangeLabelColor" />
                        <asp:CheckBox ID="chksglAC25" runat="server" Text="sglAC25" CssClass="glcheck " AutoPostBack="true" TextAlign="Right" OnCheckedChanged="ChangeLabelColor" />
                        <asp:CheckBox ID="chksglFreq50" runat="server" Text="sglFreq50" CssClass="glcheck " AutoPostBack="true" TextAlign="Right" OnCheckedChanged="ChangeLabelColor" />
                        <asp:CheckBox ID="chksglAC50" runat="server" Text="sglAC50" CssClass="glcheck " AutoPostBack="true" TextAlign="Right" OnCheckedChanged="ChangeLabelColor" />
                        <asp:CheckBox ID="chksglFreq100" runat="server" Text="sglFreq100" CssClass="glcheck " AutoPostBack="true" TextAlign="Right" OnCheckedChanged="ChangeLabelColor" />
                        <asp:CheckBox ID="chksglAC100" runat="server" Text="sglAC100" CssClass="glcheck " AutoPostBack="true" TextAlign="Right" OnCheckedChanged="ChangeLabelColor" />
                        <asp:CheckBox ID="chkTotal" runat="server" Text="Total" CssClass="glcheck " AutoPostBack="true" TextAlign="Right" OnCheckedChanged="ChangeLabelColor" />
                        <asp:Button ID="BtnRun" runat="server" BackColor="Black" ForeColor="White" Text="查詢" Font-Bold="true" BorderColor="LightGray" BorderWidth="1" />
                    </asp:Panel>
                    <asp:Label ID="lblArgument" runat="server" Font-Size="Smaller" ForeColor="LightGreen" />
                    <asp:Button ID="btnT1Start" runat="server" Text="T1更新中" Visible="false" OnClick="BtnT1StartClick" Font-Size="Smaller" ForeColor="LightGreen" BackColor="Black" BorderColor="Black" BorderWidth="0" />
                </asp:Panel>
                <audio id="SoundStartup" src="../Resources/SoundStartup.mp3" preload="auto"></audio>
                <audio id="SoundFinish" src="../Resources/SoundFinish.mp3" preload="auto"></audio>
                <script type="text/javascript">
                    function playSound(soundid) {
                        var thissound = document.getElementById(soundid);
                        thissound.setAttribute('autoplay', 'true');
                    }
                    playSound('SoundStartup');
                </script>
                <asp:Panel ID="pnlDetail" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>

    </form>
</body>
</html>
