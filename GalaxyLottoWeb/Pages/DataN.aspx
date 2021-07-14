<%@ Page Language="C#" Title="DataN" Async="true" AutoEventWireup="true" EnableSessionState="True" CodeBehind="DataN.aspx.cs" Inherits="GalaxyLottoWeb.Pages.DataN" %>

<%@ OutputCache Duration="600" VaryByParam="action;id" VaryByControl="ddlNums" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="DataN.ico" rel="shortcut icon" type="image/x-icon" />
    <webopt:BundleReference runat="server" Path="~/Content/css" />
    <title><%:Page.Title %></title>
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundles/modernizr") %>
    </asp:PlaceHolder>
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
        <asp:Timer ID="Timer1" OnTick="Timer1Tick" runat="server" Interval="10000" Enabled="true" />
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
                <asp:Panel ID="pnlinfo" runat="server" CssClass="pnlinfo" HorizontalAlign="Center">
                    <details id="dtlInfo" runat="server" visible="true" class="gldetails">
                        <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />
                        <asp:Label runat="server" ID="lblTitle" CssClass="glTitle" />
                        <br />
                        <asp:Label runat="server" ID="lblSearchMethod" CssClass="glSearchMethod" />
                    </details>
                    <asp:Button ID="btnClose" runat="server" Text="關閉(C)" AccessKey="c" OnClick="BtnCloseClick" CssClass="glbtnClose glbutton glbutton-red" />
                    <asp:CheckBox ID="ckAns" runat="server" Checked="true" Text="答案" CssClass=" glcheck glShowAns" AutoPostBack="true" />
                    <asp:Label runat="server" ID="lblMethod" CssClass="glMethod" />
                    <asp:Label ID="lblArgument" runat="server" Font-Size="Smaller" ForeColor="LightGreen" />
                    <asp:Button ID="btnT1Start" runat="server" Text="T1更新中" Visible="false" OnClick="BtnT1StartClick" CssClass="glShowUpdate" />
                    <br />

                    <asp:DropDownList ID="ddlFuntion" runat="server" CssClass="gldropdownlist" AutoPostBack="true">
                        <asp:ListItem Text="振盪預測表" Value="DataNNext" Selected="True" />
                        <asp:ListItem Text="振盪表" Value="DataN" />
                    </asp:DropDownList>

                    <asp:DropDownList ID="ddlFreq" runat="server" CssClass="gldropdownlist" AutoPostBack="true" />
                    <asp:DropDownList ID="ddlNums" runat="server" CssClass="gldropdownlist" AutoPostBack="true" />
                    <asp:DropDownList ID="ddlNump" runat="server" CssClass="gldropdownlist" AutoPostBack="true" Visible="false" />
                    <asp:DropDownList ID="ddlDays" runat="server" CssClass="gldropdownlist" AutoPostBack="true" Visible="false">
                        <asp:ListItem Text="D3" Value="3" />
                        <asp:ListItem Text="D9" Value="9" Selected="True" />
                        <asp:ListItem Text="D12" Value="12" />
                        <asp:ListItem Text="D18" Value="18" />
                        <asp:ListItem Text="D24" Value="24" />
                    </asp:DropDownList>

                    <asp:DropDownList ID="ddlPercent" runat="server" CssClass="gldropdownlist" AutoPostBack="true" Visible="false">
                        <asp:ListItem Text="P100" Value="100" Selected="True" />
                        <asp:ListItem Text="P95" Value="95" />
                        <asp:ListItem Text="P90" Value="90" />
                        <asp:ListItem Text="P75" Value="75" />
                        <asp:ListItem Text="P50" Value="50" />
                        <asp:ListItem Text="P20" Value="20" />
                    </asp:DropDownList>

                    <asp:DropDownList ID="ddlDisplay" runat="server" CssClass="gldropdownlist" AutoPostBack="true" Visible="false">
                        <asp:ListItem Text="V50" Value="50" Selected="True" />
                        <asp:ListItem Text="V100" Value="100" />
                        <asp:ListItem Text="V150" Value="150" />
                        <asp:ListItem Text="V200" Value="200" />
                    </asp:DropDownList>

                    <%--<asp:Button ID="btnRun" runat="server" Text="執行(R)" AccessKey="r" CssClass="glbtnRun glbutton glbutton-red" />--%>
                </asp:Panel>
                <audio id="SoundStartup" src="../Resources/SoundStartup.mp3" preload="auto"></audio>
                <audio id="SoundFinish" src="../Resources/SoundFinish.mp3" preload="auto"></audio>

                <asp:Panel ID="pnlDetail" runat="server" />

                <div id="scrolltop" class="font" style="visibility: hidden;">
                    <a title="返回頂部" onclick="window.scrollTo('0','0')" class="iconBox barIcon scrollIcon goTopIcon">
                        <p class="scrolltoptext">TOP</p>
                    </a>
                </div>
                <script type="text/javascript">
                    _attachEvent(window, 'scroll', function () {
                        showTopLink();
                    });
                    checkBlind();
                    function playSound(soundid) {
                        var thissound = document.getElementById(soundid);
                        thissound.setAttribute('autoplay', 'true');
                    }
                    playSound('SoundStartup');
                </script>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
    <footer />
</body>
</html>
