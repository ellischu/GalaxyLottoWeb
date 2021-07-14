<%@ Page Language="C#" Title="setFRTSearchOrder" AutoEventWireup="true" EnableSessionState="True" CodeBehind="setFRTSearchOrder.aspx.cs" Inherits="GalaxyLottoWeb.Pages.SetFrtSearchOrder" %>

<!DOCTYPE html>

<html lang='zh-tw' xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title><%:Page.Title %></title>
    <meta name="viewport" content='width=device-width, initial-scale=1' />
    <webopt:BundleReference runat="server" Path="~/Content/css" />
    <%--<link rel='stylesheet' href='../Content/bootstrap.css' />--%>
    <%--<link rel='stylesheet' href='../Content/site.css' />--%>
    <%--<link rel='stylesheet' href='../Content/galaxylotto.css' />--%>
    <%--<script src="../Scripts/jquery-3.1.1.min.js"></script>--%>
    <%--<script src="../Scripts/bootstrap.js"></script>--%>
    <link href="SearchOrder2.ico" rel="shortcut icon" type="image/x-icon" />
</head>
<body id="body" class="glbody">
    <form runat="server">
        <asp:ScriptManager runat="server">
            <Scripts>
                <asp:ScriptReference Name="jquery" />
                <asp:ScriptReference Name="bootstrap" />
                <asp:ScriptReference Name="glScript" />
            </Scripts>
        </asp:ScriptManager>
        <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
            <ProgressTemplate>
                <asp:Image ID="imgProgress" runat="server" AlternateText="progress" ImageUrl="loader.gif" Width="32" />
                Processing...
           
            </ProgressTemplate>
        </asp:UpdateProgress>

        <asp:UpdatePanel ID="UpdatePanel00" runat="server">
            <ContentTemplate>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
