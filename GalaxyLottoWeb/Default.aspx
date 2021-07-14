<%@ Page Title="首頁" UICulture="zh-TW" Culture="zh-TW" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="~/Default.aspx.cs" Inherits="GalaxyLottoWeb.Pages.Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">       
        $(document).ready(function ($) {
            $("#pnlLastNum").slideUp();
            $("#pnlLastNum").slideDown();
            Adjustimg();
        });
        $(window).resize(function () {
            Adjustimg();
        });
        function Adjustimg() {
            var a = $("#pnlLastNum").width();
            $('img').each(function () {
                if (a <= 420) {
                    $(this).width(a / 8);
                } else {
                    $(this).width(50);
                }
            });
        }
    </script>
    <asp:UpdatePanel ID="UpDefault" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlLastNum" runat="server" EnableViewState="true" ClientIDMode="Static" CssClass="container table" HorizontalAlign="Center">
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
