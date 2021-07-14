<%@ Page Language="C#" Title="TableDelete" AutoEventWireup="true" Async="true" EnableViewState="true" EnableSessionState="True" CodeBehind="TableDelete.aspx.cs" Inherits="GalaxyLottoWeb.Pages.TableDelete" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title><%:Page.Title %></title>
    <meta name="viewport" content='width=device-width, initial-scale=1' />
    <link rel='stylesheet' href='../Content/galaxylotto.css' />
    <link rel='stylesheet' href='../Content/bootstrap.css' />
    <link rel='stylesheet' href='../Content/site.css' />
    <link href="../planet.ico" rel="shortcut icon" type="image/x-icon" />
    <script src="../Scripts/jquery-3.1.1.min.js"></script>
    <script src="../Scripts/bootstrap.js"></script>
</head>
<body class="glbody">
    <%--當期資料--%>
    <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdatafix" />
    <%--<asp:Button ID="btnClose" runat="server" Text="關閉" CssClass="btn glbutton-red glclosefix" OnClick="BtnCloseClick" OnClientClick="window.close();" />--%>
    <form id="formFreq" runat="server" class="glresult">
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <%--搜尋結果--%>
                <asp:Panel ID="pnlButtons" runat="server" CssClass="glColumn1750" />
                <asp:Panel ID="pnlDetail" runat="server" CssClass="glColumn750" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
