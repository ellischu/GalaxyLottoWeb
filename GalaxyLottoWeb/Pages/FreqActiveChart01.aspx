<%@ Page Language="C#" Title="FreqActiveChart01" AutoEventWireup="true" EnableViewState="true" EnableSessionState="True" ViewStateEncryptionMode="Always" CodeBehind="FreqActiveChart01.aspx.cs" Inherits="GalaxyLottoWeb.Pages.FreqActiveChart01" %>

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
    <link href="FreqActive.ico" rel="shortcut icon" type="image/x-icon" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
</head>
<body class="glbody">
    <%--當期資料--%>
    <%--<asp:Button ID="btnClose" runat="server" Text="關閉" CssClass="btn glbutton-red glclosefix" OnClick="BtnCloseClick" OnClientClick="window.close();" />--%>
    <form id="formFreq" runat="server" class="glresult">
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

        <%--        <asp:Panel runat="server" ID="pnlinfo" Height="75" ScrollBars="Auto">
            <asp:Label runat="server" ID="lblTitle" CssClass="gllabel" />
            <asp:Label runat="server" ID="lblMethod" CssClass="gllabel" />
            <br />
            <asp:Label runat="server" ID="lblSearchMethod" CssClass="gllabel" />
        </asp:Panel>--%>

        <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
            <ProgressTemplate>
                <asp:Image ID="imgProgress" runat="server" AlternateText="progress" ImageUrl="loader.gif" Width="32" />
                Processing...           
            </ProgressTemplate>
        </asp:UpdateProgress>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <%--                <asp:Panel ID="pnlTopData" runat="server" CssClass="glTopdatafix"> --%>
                <%--  <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />--%>
                <%--                    <asp:Panel ID="pnlButtons" runat="server" CssClass="glbuttons" />--%>
                <%--</asp:Panel>--%>
                <asp:Panel ID="Panel1" runat="server" CssClass="pnlinfo" ScrollBars="Auto" HorizontalAlign="Center">
                    <details id="dtlInfo" runat="server" visible="true" class="gldetails">
                        <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />
                    </details>
                    <asp:Button ID="btnClose" runat="server" Text="關閉(C)" AccessKey="c" OnClick="BtnCloseClick" BackColor="Black" ForeColor="LightPink" BorderColor="LightPink" BorderWidth="1" />
                    <%--<asp:DropDownList ID="ddlFreq" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" />--%>
                    <asp:DropDownList ID="ddlNums" runat="server" BackColor="Black" ForeColor="White" AutoPostBack="true" />
                    <asp:Button ID="BtnUp" runat="server" BackColor="Black" ForeColor="White" Text="＋" Font-Bold="true" BorderColor="LightGray" BorderWidth="1" Height="26" Width="26" OnClick="BtnUp_Click" />
                    <asp:Button ID="BtnDowns" runat="server" BackColor="Black" ForeColor="White" Text="－" Font-Bold="true" BorderColor="LightGray" BorderWidth="1" Height="26" Width="26" OnClick="BtnDowns_Click" />
                    <asp:CheckBox ID="chk05" runat="server" Text="05" CssClass="glcheck glcheck-yellow" TextAlign="Right" Checked="true" AutoPostBack="true" />
                    <asp:CheckBox ID="chk10" runat="server" Text="10" CssClass="glcheck glcheck-yellow" TextAlign="Right" Checked="true" AutoPostBack="true" />
                    <asp:CheckBox ID="chk25" runat="server" Text="25" CssClass="glcheck glcheck-yellow" TextAlign="Right" Checked="true" AutoPostBack="true" />
                    <asp:CheckBox ID="chk50" runat="server" Text="50" CssClass="glcheck glcheck-yellow" TextAlign="Right" Checked="true" AutoPostBack="true" />
                    <asp:CheckBox ID="chk100" runat="server" Text="100" CssClass="glcheck glcheck-yellow" TextAlign="Right" Checked="true" AutoPostBack="true" />
                    <asp:CheckBox ID="chkm" runat="server" Text="Last" CssClass="glcheck glcheck-yellow" TextAlign="Right" Checked="true" AutoPostBack="true" />

                    <asp:Label ID="lblDisplay" runat="server" Text="顯示期數" CssClass="glcheck glcheck-skyblue" />
                    <asp:TextBox ID="txtDisplay" runat="server" Text="200" Width="40" AutoPostBack="true" />

                    <asp:Label ID="lblTitle" runat="server" ForeColor="Yellow" Font-Size="Small" />
                    <asp:Label ID="lblArgument" runat="server" Font-Size="Smaller" ForeColor="LightGreen" />
                    <%--<asp:Button ID="btnT1Start" runat="server" Text="T1更新中" Visible="false" OnClick="BtnT1StartClick" Font-Size="Smaller" ForeColor="LightGreen" BackColor="Black" BorderColor="Black" BorderWidth="0" />--%>
                    <br />
                    <asp:Label ID="lblMethod" runat="server" ForeColor="Orange" Font-Size="Small" />
                    <asp:Label ID="lblSearchMethod" runat="server" ForeColor="LightGreen" Font-Size="Small" />
                </asp:Panel>

                <%--搜尋結果--%>
                <asp:Panel ID="pnlLastHit" runat="server" Direction="LeftToRight" HorizontalAlign="Center">
                    <asp:PlaceHolder ID="ph05" runat="server" Visible="true">
                        <asp:Image ID="Img05" runat="server" Height="16" ImageUrl="~/Resources/1024px-Button_Icon_Brown.svg.png" />
                        <asp:Label ID="lbl05" runat="server" Text="05" CssClass="gllabel" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ph10" runat="server" Visible="true">
                        <asp:Image ID="Img10" runat="server" Height="16" ImageUrl="~/Resources/1024px-Button_Icon_Cyan.svg.png" />
                        <asp:Label ID="lbl10" runat="server" Text="10" CssClass="gllabel" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ph25" runat="server" Visible="true">
                        <asp:Image ID="Img25" runat="server" Height="16" ImageUrl="~/Resources/1024px-Button_Icon_Green.svg.png" />
                        <asp:Label ID="lbl25" runat="server" Text="25" CssClass="gllabel" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ph50" runat="server" Visible="true">
                        <asp:Image ID="Img50" runat="server" Height="16" ImageUrl="~/Resources/1024px-Button_Icon_Purple.svg.png" />
                        <asp:Label ID="lbl50" runat="server" Text="50" CssClass="gllabel" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ph100" runat="server" Visible="true">
                        <asp:Image ID="Img100" runat="server" Height="16" ImageUrl="~/Resources/1024px-Button_Icon_Blue.svg.png" />
                        <asp:Label ID="lbl100" runat="server" Text="100" CssClass="gllabel" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phLast" runat="server" Visible="true">
                        <asp:Image ID="ImgLast" runat="server" Height="16" ImageUrl="~/Resources/1024px-Button_Icon_Orange.svg.png" />
                        <asp:Label ID="lblLast" runat="server" Text="Last" CssClass="gllabel" />
                    </asp:PlaceHolder>
                </asp:Panel>
                <asp:Panel ID="pnlDetail" runat="server" CssClass="glColumn750" />

            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
    <%--<asp:Label ID="lblArgument" runat="server" />--%>
</body>
</html>
