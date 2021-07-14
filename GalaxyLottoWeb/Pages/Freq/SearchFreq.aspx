<%@ Page Title="頻率表" Language="C#" MasterPageFile="~/MasterPages/Site.Master" EnableViewState="true" EnableSessionState="True" AutoEventWireup="true" CodeBehind="SearchFreq.aspx.cs" Inherits="GalaxyLottoWeb.Pages.Freq.SearchFreq" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="lblTitle" runat="server" CssClass="gllabel" />
            <%--基本設置--%>
            <asp:Panel ID="pnlBaseC" runat="server" CssClass="glsearch-content gltext_center">
                <asp:Label ID="lblLottoType" runat="server" CssClass="gllabel" Text="樂透種類 " />
                <asp:DropDownList ID="cmbLottoType" runat="server" OnSelectedIndexChanged="CmbLottoTypeSelectedIndexChanged" AutoPostBack="True" CssClass="gldropdownlist">
                    <asp:ListItem Text="今彩539" Value="Lotto539" Selected="True" />
                    <asp:ListItem Text="大樂透" Value="LottoBig" />
                    <asp:ListItem Text="威力彩" Value="LottoWeli" />
                    <asp:ListItem Text="六合彩" Value="LottoSix" />
                </asp:DropDownList>

                <asp:Label ID="lblDataRangeEnd" runat="server" Text="最後日期" CssClass="gllabel" />
                <asp:DropDownList ID="cmbDataRangeEnd" runat="server" AutoPostBack="true" CssClass="gldropdownlist" />

                <asp:Label ID="lblCompareType" runat="server" Text="包含方式 " CssClass="gllabel" />
                <asp:DropDownList ID="cmbCompareType" runat="server" AutoPostBack="true" CssClass="gldropdownlist">
                    <asp:ListItem Text="AND" Value="AND" Selected="True" />
                    <asp:ListItem Text="OR" Value="OR" />
                </asp:DropDownList>

                <asp:Button ID="btnReset" runat="server" Text="重設(R)" AccessKey="r" OnClick="BtnResetClick" CssClass="glbutton glbutton-red" ToolTip="重設為初始設定" TabIndex="100" />
            </asp:Panel>

            <%--資料範圍--%>
            <details id="dtlDataRange" runat="server" open="open" class="gldetails">
                <summary class="Title">資料範圍</summary>
                <asp:Panel ID="pnlRangeC" runat="server" CssClass="glsearch-content gltext_center ">
                    <asp:PlaceHolder ID="phDataLimit" runat="server">
                        <asp:Label ID="lblDataLimit" runat="server" Text="資料期數" CssClass="gllabel" />
                        <asp:TextBox ID="txtDataLimit" runat="server" Text="0" TextMode="Number" AutoPostBack="true" Width="60px" CssClass="gltext gltext_YellowBlack" />
                        <asp:ImageButton ID="ibDataLimit" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBDataLimitClick" BackColor="White" CssClass="glclear" />

                        <asp:Label ID="lblDataOffset" runat="server" Text="資料位移" CssClass="gllabel" />
                        <asp:TextBox ID="txtDataOffset" runat="server" Text="0" TextMode="Number" AutoPostBack="true" Width="60px" CssClass="gltext gltext_YellowBlack" />
                        <asp:ImageButton ID="ibDataOffset" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBDataOffsetClick" BackColor="White" CssClass="glclear" />

                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phSearchLimit" runat="server">
                        <asp:Label ID="lblSearchLimit" runat="server" Text="查詢期數" CssClass="gllabel" />
                        <asp:TextBox ID="txtSearchLimit" runat="server" Text="0" TextMode="Number" AutoPostBack="true" Width="60px" CssClass="gltext gltext_YellowBlack" />
                        <asp:ImageButton ID="ibSearchLimit" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBSearchLimitClick" BackColor="White" CssClass="glclear" />

                        <asp:Label ID="lblSearchOffset" runat="server" Text="查詢位移" CssClass="gllabel" />
                        <asp:TextBox ID="txtSearchOffset" runat="server" Text="0" TextMode="Number" AutoPostBack="true" Width="60px" CssClass="gltext gltext_YellowBlack" />
                        <asp:ImageButton ID="ibSearchOffset" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBSearchOffsetClick" BackColor="White" CssClass="glclear" />

                    </asp:PlaceHolder>
                </asp:Panel>
            </details>

            <%--顯示選項--%>
            <details id="dtlShow" runat="server" visible="true" open="open" class="gldetails">
                <summary class="Title">篩選項目</summary>
                <asp:Panel ID="pnlShowC" runat="server" CssClass="glsearch-content gltext_center ">

                    <asp:CheckBox ID="chkField" runat="server" Text="欄位" OnCheckedChanged="ChkFieldCheckedChanged" AutoPostBack="true" CssClass="glcheck" TextAlign="Right" />
                    <asp:CheckBox ID="chkNextNums" runat="server" Text="托牌" OnCheckedChanged="ChkNextNumsCheckedChanged" AutoPostBack="true" CssClass="glcheck" TextAlign="Right" />
                    <asp:CheckBox ID="chkGeneral" runat="server" Text="一般" Checked="true" AutoPostBack="true" CssClass="glcheck" TextAlign="Right" />
                    <asp:CheckBox ID="chkPeriod" runat="server" Text="週期" AutoPostBack="true" CssClass="glcheck" TextAlign="Right" />

                </asp:Panel>
            </details>

            <%--比較欄位--%>
            <details id="dtlCompare" runat="server" visible="false" open="open" class="gldetails">
                <summary class="Title">比較欄位</summary>
                <asp:Panel ID="pnlCompare" runat="server" CssClass="glsearch-content gltext_center">
                    <asp:PlaceHolder ID="phCompare01" runat="server">
                        <asp:Label ID="lblCompare01" runat="server" Text="欄位1" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbCompare01" runat="server" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phCompare02" runat="server">
                        <asp:Label ID="lblCompare02" runat="server" Text="欄位2" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbCompare02" runat="server" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phCompare03" runat="server">
                        <asp:Label ID="lblCompare03" runat="server" Text="欄位3" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbCompare03" runat="server" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phCompare04" runat="server">
                        <asp:Label ID="lblCompare04" runat="server" Text="欄位4" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbCompare04" runat="server" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phCompare05" runat="server">
                        <asp:Label ID="lblCompare05" runat="server" Text="欄位5" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbCompare05" runat="server" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                </asp:Panel>
            </details>

            <%--拖牌設定--%>
            <details id="dtlNextNum" runat="server" visible="false" open="open" class="gldetails">
                <summary class="Title">拖牌設定</summary>
                <asp:Panel ID="pnlNextNumC" runat="server" CssClass="glsearch-content gltext_center">
                    <asp:PlaceHolder ID="phNextNums" runat="server">
                        <asp:Label ID="lblNextNums" runat="server" Text="星數資料" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbNextNums" runat="server" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Text="0" Value="0" Selected="True" />
                            <asp:ListItem Text="1" Value="1" />
                            <asp:ListItem Text="2" Value="2" />
                            <asp:ListItem Text="3" Value="3" />
                            <asp:ListItem Text="4" Value="4" />
                        </asp:DropDownList>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phNextStep" runat="server">
                        <asp:Label ID="lblNextStep" runat="server" Text="間隔期數" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbNextStep" runat="server" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Text="1" Value="1" Selected="True" />
                            <asp:ListItem Text="2" Value="2" />
                            <asp:ListItem Text="3" Value="3" />
                            <asp:ListItem Text="4" Value="4" />
                            <asp:ListItem Text="5" Value="5" />
                        </asp:DropDownList>
                    </asp:PlaceHolder>
                </asp:Panel>
            </details>

            <%--輸出選項--%>
            <details id="Details1" runat="server" visible="true" open="open" class="gldetails">
                <summary class="Title">輸出選項</summary>
                <asp:Panel ID="pnlButtons" runat="server" CssClass="glsearch-content gltext_center ">

                    <asp:PlaceHolder ID="phDisplayPeriod" runat="server">
                        <asp:Label ID="lblDisplayPeriod" runat="server" Text="顯示期數DP" CssClass="gllabel" />
                        <asp:TextBox ID="txtDisplayPeriod" runat="server" Text="200" TextMode="Number" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60px" />
                        <asp:ImageButton ID="ibDisplayPeriod" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBDisplayPeriodClick" CssClass="glclear" BackColor="White" />
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phTestPeriods" runat="server">
                        <asp:Label ID="lblTestPeriods" runat="server" Text="連續測試期數" CssClass="gllabel" />
                        <asp:TextBox ID="txtTestPeriods" runat="server" Text="1" TextMode="Number" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60" />
                        <asp:ImageButton ID="ibTestPeriods" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBTestPeriodsClick" CssClass="glclear" BackColor="White" />
                    </asp:PlaceHolder>

                    <asp:Button ID="btnRun" runat="server" Text="查詢(G)" OnClick="BtnRun_Click" AccessKey="g" CssClass="glbutton glbutton-blue" TabIndex="1" />
                    <asp:Button ID="btnOrder" runat="server" Text="排程(O)" PostBackUrl="~/Pages/Freq/FreqResult.aspx" AccessKey="g" CssClass="glbutton glbutton-blue" TabIndex="1" />
                    <%--OnClientClick="var main = window.open('FreqResult.aspx','_blank');"--%>
                </asp:Panel>
            </details>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
