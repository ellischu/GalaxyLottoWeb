<%@ Page Title="更新" Language="C#" MasterPageFile="~/MasterPages/Site.Master" EnableSessionState="True" EnableViewState="true" AutoEventWireup="true" CodeBehind="Update.aspx.cs" Inherits="GalaxyLottoWeb.Pages.Update" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" CssClass="table" HorizontalAlign="Center">
                <%--資料更新--%>
                <h3>[手動更新資料] </h3>
                <div runat="server">
                    <asp:Button ID="btn539" runat="server" Text="５３９更新" CssClass="btn btn-primary btn-lg" OnClick="Btn539Click" OnClientClick="window.open('UpdateResult.aspx','Update539');" />
                    <asp:Button ID="btnBig" runat="server" Text="大樂透更新" CssClass="btn btn-info btn-lg" OnClick="BtnBigClick" OnClientClick="window.open('UpdateResult.aspx','UpdateBig');" />
                    <%--<asp:Button ID="btnDafu" runat="server" Text="大福彩更新" CssClass="btn btn-danger btn-lg" OnClick="BtnDafuClick" OnClientClick="window.open('UpdateResult.aspx','UpdateDafu');" />--%>
                    <asp:Button ID="btnWeli" runat="server" Text="威力彩更新" CssClass="btn btn-warning btn-lg" OnClick="BtnWeliClick" OnClientClick="window.open('UpdateResult.aspx','UpdateWeli');" />
                    <asp:Button ID="btnTwinWin" runat="server" Text="雙贏彩更新" CssClass="btn btn-primary btn-lg" OnClick="BtnTwinWinClick" OnClientClick="window.open('UpdateResult.aspx','UpdateTwinWin');" />
                    <asp:Button ID="btnSix" runat="server" Text="六合彩更新" CssClass="btn btn-success btn-lg" OnClick="BtnSixClick" OnClientClick="window.open('UpdateResult.aspx','UpdateSix');" />
                </div>
                <p />
                <div runat="server">
                    <asp:Button ID="btnWC" runat="server" Text="中西曆更新" CssClass="btn btn-link btn-lg" OnClick="BtnWClick" OnClientClick="window.open('UpdateResult.aspx','UpdateWC');" />
                    <asp:Button ID="btnPurple" runat="server" Text="紫微斗更新" CssClass="btn btn-link btn-lg" OnClick="BtnPurpleClick" OnClientClick="window.open('UpdateResult.aspx','UpdatePurple');" />
                    <asp:Button ID="btnOptions" runat="server" Text="參數檔更新" CssClass="btn btn-link btn-lg" OnClick="BtnOptionsClick" OnClientClick="window.open('UpdateResult.aspx','UpdateOptions');" />
                    <asp:Button ID="btn24SolarTrems" runat="server" Text="節氣更新" CssClass="btn btn-link btn-lg" OnClientClick="window.open('UpdateSolar24.aspx','UpdateSolar24');" />
                </div>
                <hr />

                <%--                <asp:TextBox ID="txtResual" runat="server" Width="800px" Height="80 px" TextMode="MultiLine" />--%>
                <asp:DropDownList ID="ddlLottoType" runat="server" AutoPostBack="True" CssClass="gldropdownlist" EnableViewState="true">
                    <asp:ListItem Enabled="true" Text="雙贏彩" Value="TwinWin" Selected="True" />
                    <asp:ListItem Enabled="true" Text="539" Value="539" />
                    <asp:ListItem Enabled="true" Text="大樂透" Value="Big" />
                    <asp:ListItem Enabled="true" Text="威力彩" Value="Weli" />
                </asp:DropDownList>
                <asp:FileUpload ID="FileInput" runat="server" Enabled="true" ViewStateMode="Enabled" EnableViewState="true" />
                <asp:Button ID="btnUpdate" runat="server" Text="更新" CssClass="btn btn-primary btn-lg" OnClick="BtnUpdateClick" />
                <asp:Label ID="UploadStatusLabel" runat="server" />
                <hr />
                <h3>[線上更新資料] </h3>
                <div runat="server">
                    <asp:Button ID="btn539OnLine" runat="server" Text="５３９更新" CssClass="btn btn-primary btn-lg" OnClick="Btn539OnLineClick" />
                    <asp:Button ID="btnBigOnLine" runat="server" Text="大樂透更新" CssClass="btn btn-info btn-lg" OnClick="BtnBigOnLineClick" />
                    <asp:Button ID="btnWeliOnLine" runat="server" Text="威力彩更新" CssClass="btn btn-warning btn-lg" OnClick="BtnWeliOnLineClick" />
                    <asp:Button ID="btnTwinWinOnLine" runat="server" Text="雙贏彩更新" CssClass="btn btn-primary btn-lg" OnClick="BtnTwinWinOnLineClick" />
                </div>
                <asp:Label ID="lblAlaem" runat="server"></asp:Label>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpdate" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
