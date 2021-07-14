<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="True" CodeBehind="Related.aspx.cs" Inherits="GalaxyLottoWeb.Pages.Related" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server" CssClass="glcontainer" HorizontalAlign="Center">
                <h3 class="h3">[樂透相關]</h3>
                <asp:Panel ID="Panel2" runat="server" CssClass="row ">
                    <asp:HyperLink ID="HyperLink1" CssClass="btn btn-success" Width="150" runat="server" Target="_blank" NavigateUrl="http://www.taiwanlottery.com.tw">台灣彩券網站</asp:HyperLink>
                    <asp:HyperLink ID="HyperLink2" CssClass="btn btn-success" Width="150" runat="server" Target="_blank" NavigateUrl="http://www.nfd.com.tw/lottery/index.htm">電腦彩券樂透網</asp:HyperLink>
                    <asp:HyperLink ID="HyperLink3" CssClass="btn btn-success" Width="150" runat="server" Target="_blank" NavigateUrl="http://lotto.arclink.com.tw/">樂透研究院</asp:HyperLink>
                    <asp:HyperLink ID="HyperLink4" CssClass="btn btn-primary" Width="150" runat="server" Target="_blank" NavigateUrl="http://080e.com/">080e贏發贏易</asp:HyperLink>
                    <asp:HyperLink ID="HyperLink5" CssClass="btn btn-primary" Width="150" runat="server" Target="_blank" NavigateUrl="http://www.lotto168.com/">樂透彩168</asp:HyperLink>
                    <asp:HyperLink ID="HyperLink6" CssClass="btn btn-primary" Width="150" runat="server" Target="_blank" NavigateUrl="http://lotto.auzonet.com/">奧索樂透網</asp:HyperLink>
                    <asp:HyperLink ID="HyperLink7" CssClass="btn btn-primary" Width="150" runat="server" Target="_blank" NavigateUrl="http://lotto.bestshop.com.tw/">樂透彩資訊網</asp:HyperLink>
                    <asp:HyperLink ID="HyperLink8" CssClass="btn btn-primary" Width="150" runat="server" Target="_blank" NavigateUrl="http://bet.hkjc.com/marksix/default.aspx">六合彩官網</asp:HyperLink>
                    <asp:HyperLink ID="HyperLink9" CssClass="btn btn-primary" Width="150" runat="server" Target="_blank" NavigateUrl="http://www.pilio.idv.tw/">幸運發財網</asp:HyperLink>
                </asp:Panel>
                <h3 class="h3">[命理相關]</h3>
                <asp:Panel ID="Panel3" runat="server" CssClass="row">
                    <asp:HyperLink ID="HyperLink10" CssClass="btn btn-secondary" Width="150" runat="server" Target="_blank" NavigateUrl="http://www.lead.idv.tw/wgs/joytable.asp">百合命相</asp:HyperLink>
                    <asp:HyperLink ID="HyperLink11" CssClass="btn btn-secondary" Width="150" runat="server" Target="_blank" NavigateUrl="http://www.chu.edu.tw/~anita/">網路農民曆</asp:HyperLink>
                    <asp:HyperLink ID="HyperLink12" CssClass="btn btn-secondary" Width="150" runat="server" Target="_blank" NavigateUrl="https://www.profate.com.tw/nongli">農民曆</asp:HyperLink>
                </asp:Panel>
                <h3 class="h3">[其他連結]</h3>
                <asp:Panel ID="Panel4" runat="server" CssClass="row">
                    <asp:HyperLink ID="HyperLink13" CssClass="btn btn-warning" Width="150" runat="server" Target="_blank" NavigateUrl="http://wdc.kugi.kyoto-u.ac.jp/dstdir/index.html">日本地磁</asp:HyperLink>
                </asp:Panel>
                <h3 class="h3">[日期轉換]</h3>
                <asp:Calendar ID="Calendar01" runat="server" 
                    OnSelectionChanged="Calendar01_SelectionChanged" OnDayRender="Calendar01_DayRender" FirstDayOfWeek="Monday">
                    <TodayDayStyle BackColor="Red" ForeColor="Yellow"></TodayDayStyle>
                    <WeekendDayStyle BackColor="LightGreen" ForeColor="red"></WeekendDayStyle>
                </asp:Calendar>
                <asp:Panel ID="pnlCalendar" runat="server"/>
                <asp:Label ID="lblCC" runat="server" Text="" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
