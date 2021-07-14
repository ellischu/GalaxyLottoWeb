<%@ Page Title="NextBalance" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="NextBalance.aspx.cs" Inherits="GalaxyLottoWeb.Pages.NextBalance" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <details id="dtlInfo" runat="server" visible="true" >
        <summary title="當期資料">當期資料</summary>
        <asp:Panel ID="pnlCurrentData" runat="server" CssClass="glcurrentdata" />
    </details>
    <h2>振盪平衡計算</h2>
    <asp:Panel ID="pnlBaseC" runat="server" CssClass="glsearch-content gltext_center" ViewStateMode="Enabled" EnableViewState="true">
        <asp:Label ID="lblLottoType" runat="server" ForeColor="White" Text="樂透種類 " />
        <asp:DropDownList ID="cmbLottoType" runat="server" AutoPostBack="True" CssClass="gldropdownlist" EnableViewState="true" ViewStateMode="Enabled">
            <asp:ListItem Text="今彩539" Value="Lotto539" Selected="True" />
            <asp:ListItem Text="大樂透" Value="LottoBig" />
            <asp:ListItem Text="威力彩" Value="LottoWeli" />
            <asp:ListItem Text="六合彩" Value="LottoSix" />
        </asp:DropDownList>
        <asp:Label ID="lblDataRangeEnd" runat="server" Text="最後日期" ForeColor="White" />
        <asp:DropDownList ID="cmbDataRangeEnd" AutoPostBack="true" runat="server" CssClass="gldropdownlist" EnableViewState="true" />
    </asp:Panel>
    <asp:Label ID="lblNext" runat="server" Text="振盪" />
    <asp:Panel ID="pnlNextMin" runat="server">
        <asp:Label ID="lblNextMin" runat="server" Text="Min =>" Width="57" />
        <asp:TextBox ID="txtNextMin1" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtNextMin2" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtNextMin3" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtNextMin4" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtNextMin5" runat="server"></asp:TextBox>
    </asp:Panel>
    <asp:Panel ID="pnlNextMax" runat="server">
        <asp:Label ID="lblNextMax" runat="server" Text="Max =>" Width="57" />
        <asp:TextBox ID="txtNextMax1" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtNextMax2" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtNextMax3" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtNextMax4" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtNextMax5" runat="server"></asp:TextBox>
    </asp:Panel>
    <asp:Label ID="lblBalance" runat="server" Text="平衡" />
    <asp:Panel ID="pnlBalanceMin" runat="server">
        <asp:Label ID="lblBalanceMin" runat="server" Text="Min =>" Width="57" />
        <asp:TextBox ID="txtBalanceMin1" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtBalanceMin2" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtBalanceMin3" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtBalanceMin4" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtBalanceMin5" runat="server"></asp:TextBox>
    </asp:Panel>
    <asp:Panel ID="pnlBalanceMax" runat="server">
        <asp:Label ID="lblBalanceMax" runat="server" Text="Max =>" Width="57" />
        <asp:TextBox ID="txtBalanceMax1" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtBalanceMax2" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtBalanceMax3" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtBalanceMax4" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtBalanceMax5" runat="server"></asp:TextBox>
    </asp:Panel>

    <asp:Button ID="btnCalculate" runat="server" Text="計算" OnClick="BtnCalculateClick" />
    <hr />
    <asp:Panel ID="pnlResualt" runat="server">
        <asp:GridView ID="GvCompare" runat="server" CssClass="gltable" GridLines="Horizontal" />
    </asp:Panel>
    <asp:Panel ID="pnlSideColNext" runat="server" Direction="LeftToRight">
        <asp:Label ID="lblSideColNext" runat="server" Text="振盪斜柱(□)" Width="100" />
        <asp:TextBox ID="txtSideColNext" runat="server" ReadOnly="true" Width="400" />
    </asp:Panel>
    <asp:Panel ID="pnlSideColBalance" runat="server" Direction="LeftToRight">
        <asp:Label ID="lblSideColBalance" runat="server" Text="平衡斜柱(□)" Width="100" />
        <asp:TextBox ID="txtSideColBalance" runat="server" ReadOnly="true" Width="400" />
    </asp:Panel>
    <asp:Panel ID="pnlSameColNext" runat="server" Direction="LeftToRight">
        <asp:Label ID="lblSameColNext" runat="server" Text="振盪同柱(○)" Width="100" />
        <asp:TextBox ID="txtSameColNext" runat="server" ReadOnly="true" Width="400" />
    </asp:Panel>
    <asp:Panel ID="pnlSameColBalance" runat="server" Direction="LeftToRight">
        <asp:Label ID="lblSameColBalance" runat="server" Text="平衡同柱(○)" Width="100" />
        <asp:TextBox ID="txtSameColBalance" runat="server" ReadOnly="true" Width="400" />
    </asp:Panel>

    <hr />
    <asp:Panel ID="pnlSum" runat="server">
        <asp:Label ID="lblSumNext" runat="server" Text="振盪統計" />
        <asp:GridView ID="GvSumNext" runat="server" CssClass="gltable" GridLines="Horizontal" AutoGenerateColumns="false" />
        <asp:Label ID="lblSumBalance" runat="server" Text="平衡統計" />
        <asp:GridView ID="GvSumBalance" runat="server" CssClass="gltable" GridLines="Horizontal" AutoGenerateColumns="false" />
        <asp:Label ID="lblSumCompare" runat="server" Text="比較統計" />
        <asp:GridView ID="GvSumCompare" runat="server" CssClass="gltable" GridLines="Horizontal" AutoGenerateColumns="false" />
    </asp:Panel>
</asp:Content>
