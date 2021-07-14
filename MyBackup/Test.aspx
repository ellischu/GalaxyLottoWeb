<%@ Page Language="C#" EnableViewState="true" ViewStateMode="Enabled" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="GalaxyLottoWeb.Pages.Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style type="text/css">
        #UpdateProgress1, #UpdateProgress2 {
            width: 200px;
            background-color: #FFC080;
            position: absolute;
            bottom: 0px;
            left: 0px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
                <Scripts>
                    <asp:ScriptReference Name="jquery" Path="~/Scripts/jquery-3.3.1.min.js" />
                    <asp:ScriptReference Name="bootstrap" Path="~/Scripts/bootstrap.min.js" />
                    <asp:ScriptReference Name="glScript" Path="~/Scripts/glScript.js" />
                </Scripts>
            </asp:ScriptManager>
            <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
                <ProgressTemplate>
                    <img alt="progress" src="loader.gif" width="50" />
                    Processing...           
                </ProgressTemplate>
            </asp:UpdateProgress>

            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:Label ID="lblText" runat="server" Text=""></asp:Label>
                    <br />
                    <asp:Button ID="btnInvoke" runat="server" Text="Click"
                        OnClick="BtnInvokeClick" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>

</body>
</html>
