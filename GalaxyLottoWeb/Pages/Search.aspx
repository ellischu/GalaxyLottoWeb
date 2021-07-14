<%@ Page Title="計算" Language="C#" MasterPageFile="~/MasterPages/Site.Master" EnableViewState="true" EnableSessionState="True" AutoEventWireup="True" CodeBehind="Search.aspx.cs" Inherits="GalaxyLottoWeb.Pages.Search" %>

<%--<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
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
                <asp:DropDownList ID="cmbDataRangeEnd" runat="server" OnSelectedIndexChanged="CmbDataRangeEndSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist" />

                <asp:Label ID="lblCompareType" runat="server" Text="包含方式 " CssClass="gllabel" />
                <asp:DropDownList ID="cmbCompareType" runat="server" OnSelectedIndexChanged="CmbCompareTypeSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist">
                    <asp:ListItem Text="AND" Value="AND" Selected="True" />
                    <asp:ListItem Text="OR" Value="OR" />
                </asp:DropDownList>

                <asp:Button ID="btnReset" runat="server" Text="重設(R)" AccessKey="r" OnClick="BtnResetClick" CssClass="glbutton glbutton-red" ToolTip="重設為初始設定" TabIndex="100" />
                <%--<asp:CheckBox ID="chkTimer" runat="server" Text="計時器" OnCheckedChanged="ChkTimer_CheckedChanged" CssClass="glcheck" TextAlign="Right" AutoPostBack="true" Checked="false" />--%>
            </asp:Panel>

            <%--計算方式--%>
            <details id="dtlCalc" runat="server" visible="true" open="open" class="gldetails">
                <summary class="Title">計算方式</summary>
                <asp:Panel ID="pnlButtons" runat="server" CssClass="glsearch-content gltext_center ">
                    <div>
                        <asp:DropDownList ID="rblButtons" runat="server" OnSelectedIndexChanged="CmdOptionSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Selected="True" Text="頻率" Value="cFreq00"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="活性表" Value="cFreqActive00" />
                            <asp:ListItem Selected="False" Text="活性歷史表" Value="cFreqActiveH00" />
                            <asp:ListItem Selected="False" Text="遺漏表" Value="cTableMissingTotal00"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="數字DNA" Value="cFreqDNA00"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="平衡振盪表" Value="cTableDataN00"></asp:ListItem>
                            <%--<asp:ListItem Selected="False" Text="奇-偶數表" Value="cTableOddEven"></asp:ListItem>--%>
                            <%--<asp:ListItem Selected="False" Text="大-小數表" Value="cTableHighLow"></asp:ListItem>--%>
                            <asp:ListItem Selected="False" Text="百分表" Value="cTablePercent00"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="和數表" Value="cTableSum00"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="組合" Value="cSmart00"></asp:ListItem>
                        </asp:DropDownList>
                        <span>-> </span>
                        <asp:DropDownList ID="cmdFreq" runat="server" OnSelectedIndexChanged="CmdOptionSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Selected="True" Text="頻率" Value="cFreq" />
                            <asp:ListItem Selected="False" Text="頻率總表" Value="cFreqT"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="頻率組" Value="cFreqSet"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="綜合計算01" Value="cCacul01"></asp:ListItem>
                        </asp:DropDownList>

                        <asp:DropDownList ID="cmdFreqActive" runat="server" OnSelectedIndexChanged="CmdOptionSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Selected="True" Text="活性表" Value="cFreqActive" />
                            <asp:ListItem Selected="False" Text="活性表01" Value="cFreqActive01"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="活性表01總表" Value="cFreqActive01T"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="活性表01R總表" Value="cFreqActive01TR"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="活性表02" Value="cFreqActive02"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="活性表03" Value="cFreqActive03"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="活性圖" Value="cFreqActiveChart"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="活性圖01" Value="cFreqActiveChart01"></asp:ListItem>
                        </asp:DropDownList>

                        <asp:DropDownList ID="cmdFreqActiveH" runat="server" OnSelectedIndexChanged="CmdOptionSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Selected="False" Text="活性歷史表" Value="cFreqActiveH"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="活性歷史總表" Value="cFreqActiveHT"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="活性歷史總表01" Value="cFreqActiveHT01"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="活性歷史總表01(預載)" Value="cFreqActiveHT01P"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="活性歷史總表02" Value="cFreqActiveHT02"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="活性歷史總表All" Value="cFreqActiveHTAll"></asp:ListItem>
                        </asp:DropDownList>

                        <asp:DropDownList ID="cmdTableMissingTotal" runat="server" OnSelectedIndexChanged="CmdOptionSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Selected="False" Text="遺漏表" Value="cTableMissingTotal"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="遺漏整合表" Value="cTableMissingTotalT"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="遺漏整合表01" Value="cTableMissingTotalT01"></asp:ListItem>
                        </asp:DropDownList>

                        <asp:DropDownList ID="cmdFreqDNA" runat="server" OnSelectedIndexChanged="CmdOptionSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Selected="False" Text="數字DNA" Value="cFreqDNA"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="DNA整合表" Value="cFreqDNAT"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="數字區間" Value="cFreqSecField"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="數字區間整合表" Value="cFreqSecFieldT"></asp:ListItem>
                        </asp:DropDownList>

                        <asp:DropDownList ID="cmdTableDataN" runat="server" OnSelectedIndexChanged="CmdOptionSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Selected="False" Text="振盪表" Value="cTableDataN"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="振盪圖" Value="cTableDataNChart"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="平衡表" Value="cTableDataB"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="振盪平衡總表01" Value="cTableDataNBT01"></asp:ListItem>
                        </asp:DropDownList>

                        <asp:DropDownList ID="cmdTablePercent" runat="server" OnSelectedIndexChanged="CmdOptionSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Selected="False" Text="TP00" Value="cTablePercent"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="TP01" Value="cTablePercent01"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="TP02" Value="cTablePercent02"></asp:ListItem>
                        </asp:DropDownList>

                        <asp:DropDownList ID="cmdTableSum" runat="server" OnSelectedIndexChanged="CmdOptionSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Selected="False" Text="和數表" Value="cTableSum"></asp:ListItem>
                        </asp:DropDownList>

                        <asp:DropDownList ID="cmdSmart" runat="server" OnSelectedIndexChanged="CmdOptionSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Selected="False" Text="計算組合" Value="cSmartRun"></asp:ListItem>
                            <asp:ListItem Selected="False" Text="聰明組合" Value="cSmartTest"></asp:ListItem>
                        </asp:DropDownList>

                        <asp:PlaceHolder ID="phFieldPeiodLimit" runat="server">
                            <asp:Label ID="lblFieldPeriodLimit" runat="server" Text="欄位限制期數" CssClass="gllabel" />
                            <asp:DropDownList ID="cmdFieldPeriodLimit" runat="server" OnSelectedIndexChanged="TxtDataRangeTextChanged" AutoPostBack="true" CssClass="gldropdownlist">
                                <asp:ListItem Text="200" Value="200"></asp:ListItem>
                                <asp:ListItem Text="100" Value="100"></asp:ListItem>
                                <asp:ListItem Text="180" Value="180"></asp:ListItem>
                                <asp:ListItem Text="150" Value="150"></asp:ListItem>
                                <asp:ListItem Text="120" Value="120"></asp:ListItem>
                                <asp:ListItem Text="250" Value="250"></asp:ListItem>
                                <asp:ListItem Text="300" Value="300"></asp:ListItem>
                                <asp:ListItem Text="400" Value="400"></asp:ListItem>
                                <asp:ListItem Text="450" Value="450"></asp:ListItem>
                                <asp:ListItem Text="460" Value="460"></asp:ListItem>
                                <asp:ListItem Text="470" Value="470"></asp:ListItem>
                                <asp:ListItem Text="480" Value="480"></asp:ListItem>
                                <asp:ListItem Text="490" Value="490"></asp:ListItem>
                                <asp:ListItem Text="500" Value="500"></asp:ListItem>
                                <asp:ListItem Selected="True" Text="550" Value="550"></asp:ListItem>
                                <asp:ListItem Text="600" Value="600"></asp:ListItem>
                                <asp:ListItem Text="650" Value="650"></asp:ListItem>
                            </asp:DropDownList>
                        </asp:PlaceHolder>

                        <%-- Buttons --%>
                        <asp:Button ID="btnShortcut" runat="server" Text="快捷" OnClick="BtnShortcutClick" OnClientClick="window.open('ShortCut.aspx','_blank');" CssClass="glbutton glbutton-blue" />
                        <asp:Button ID="btnRun" runat="server" Text="查詢(G)" AccessKey="g" OnClientClick="window.open('Redirector.aspx','_blank');" CssClass="glbutton glbutton-blue" TabIndex="1" />
                        <asp:CheckBox ID="chkSearchOrder" runat="server" Text="排程" OnCheckedChanged="DisplayCheckedChanged" AutoPostBack="true" CssClass="glcheck glcheck-yellow" TextAlign="Right" />
                        <asp:Button ID="btnShowSearchOrder" runat="server" Text="啟動排程" OnClientClick="window.open('SearchOrder.aspx','SearchOrder');" CssClass="glbutton" Font-Size="Medium" BackColor="Black" ForeColor="LightBlue" BorderColor="LightBlue" BorderWidth="1" TabIndex="102" />
                    </div>
                </asp:Panel>
                <%--<asp:Button ID="btnMove" runat="server" Text="移動" Visible="false" OnClick="BtnMove_Click" BackColor="Black" ForeColor="LightGreen" BorderWidth="1" />--%>
            </details>

            <%--資料範圍--%>
            <details id="dtlDataRange" runat="server" open="open" class="gldetails">
                <summary class="Title">資料範圍</summary>
                <asp:Panel ID="pnlRangeC" runat="server" CssClass="glsearch-content gltext_center ">
                    <asp:PlaceHolder ID="phDataLimit" runat="server">
                        <asp:Label ID="lblDataLimit" runat="server" Text="資料期數" CssClass="gllabel" />
                        <asp:TextBox ID="txtDataLimit" runat="server" Text="0" OnTextChanged="TxtDataRangeTextChanged" AutoPostBack="true" Width="60px" CssClass="gltext gltext_YellowBlack" />
                        <asp:ImageButton ID="ibDataLimit" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBDataLimitClick" BackColor="White" CssClass="glclear" />
                        <%--
                                <asp:ImageButton ID="btnDLup" runat="server" ImageUrl="~/Resources/up.gif" AlternateText="Up" Width="15" Height="15" />
                                <asp:ImageButton ID="btnDLdown" runat="server" ImageUrl="~/Resources/down.gif" AlternateText="Down" Width="15" Height="15" />
                                <ajaxToolkit:NumericUpDownExtender ID="NUDDL" runat="server" TargetControlID="txtDataLimit" TargetButtonUpID="btnDLup" TargetButtonDownID="btnDLdown" Width="60" Minimum="0" />
                        --%>
                        <asp:Label ID="lblDataOffset" runat="server" Text="資料位移" CssClass="gllabel" />
                        <asp:TextBox ID="txtDataOffset" runat="server" Text="0" OnTextChanged="TxtDataRangeTextChanged" AutoPostBack="true" Width="60px" CssClass="gltext gltext_YellowBlack" />
                        <asp:ImageButton ID="ibDataOffset" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBDataOffsetClick" BackColor="White" CssClass="glclear" />
                        <%--
                                <asp:ImageButton ID="btnDOup" runat="server" ImageUrl="~/Resources/up.gif" AlternateText="Up" Width="15" Height="15" /> 
                                <asp:ImageButton ID="btnDOdown" runat="server" ImageUrl="~/Resources/down.gif" AlternateText="Down" Width="15" Height="15" />
                                <ajaxToolkit:NumericUpDownExtender ID="NUDDO" runat="server" TargetControlID="txtDataOffset" TargetButtonUpID="btnDOup" TargetButtonDownID="btnDOdown" Width="60" Minimum="0" />
                        --%>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phSearchLimit" runat="server">
                        <asp:Label ID="lblSearchLimit" runat="server" Text="查詢期數" CssClass="gllabel" />
                        <asp:TextBox ID="txtSearchLimit" runat="server" Text="0" OnTextChanged="TxtDataRangeTextChanged" AutoPostBack="true" Width="60px" CssClass="gltext gltext_YellowBlack" />
                        <asp:ImageButton ID="ibSearchLimit" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBSearchLimitClick" BackColor="White" CssClass="glclear" />
                        <%--
                                <asp:ImageButton ID="btnSLup" runat="server" ImageUrl="~/Resources/up.gif" AlternateText="Up" Width="15" Height="15" />
                                <asp:ImageButton ID="btnSLdown" runat="server" ImageUrl="~/Resources/down.gif" AlternateText="Down" Width="15" Height="15" />
                                <ajaxToolkit:NumericUpDownExtender ID="NumericUpDownExtender1" runat="server" TargetControlID="txtSearchLimit" TargetButtonUpID="btnSLup" TargetButtonDownID="btnSLdown" Width="60" Minimum="0" />
                        --%>
                        <asp:Label ID="lblSearchOffset" runat="server" Text="查詢位移" CssClass="gllabel" />
                        <asp:TextBox ID="txtSearchOffset" runat="server" Text="0" OnTextChanged="TxtDataRangeTextChanged" AutoPostBack="true" Width="60px" CssClass="gltext gltext_YellowBlack" />
                        <asp:ImageButton ID="ibSearchOffset" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBSearchOffsetClick" BackColor="White" CssClass="glclear" />
                        <%--
                                    <asp:ImageButton ID="btnSOup" runat="server" ImageUrl="~/Resources/up.gif" AlternateText="Up" Width="15" Height="15" />
                                    <asp:ImageButton ID="btnSOdown" runat="server" ImageUrl="~/Resources/down.gif" AlternateText="Down" Width="15" Height="15" />
                                    <ajaxToolkit:NumericUpDownExtender ID="NumericUpDownExtender2" runat="server" TargetControlID="txtSearchOffset" TargetButtonUpID="btnSOup" TargetButtonDownID="btnSOdown" Width="60" Minimum="0" />
                        --%>
                    </asp:PlaceHolder>
                </asp:Panel>
            </details>

            <%--顯示選項--%>
            <details id="dtlShow" runat="server" visible="true" open="open" class="gldetails">
                <summary class="Title">顯示選項</summary>
                <asp:Panel ID="pnlShowC" runat="server" CssClass="glsearch-content gltext_center ">
                    <div runat="server" style="color: white;">
                        <div>
                            <asp:CheckBox ID="chkField" runat="server" Text="欄位" OnCheckedChanged="ChkFieldCheckedChanged" AutoPostBack="true" CssClass="glcheck" TextAlign="Right" />
                            <asp:CheckBox ID="chkNextNums" runat="server" Text="托牌" OnCheckedChanged="ChkNextNumsCheckedChanged" AutoPostBack="true" CssClass="glcheck" TextAlign="Right" />
                            <asp:CheckBox ID="chkGeneral" runat="server" Text="一般" Checked="true" OnCheckedChanged="DisplayCheckedChanged" AutoPostBack="true" CssClass="glcheck" TextAlign="Right" />
                            <asp:CheckBox ID="chkPeriod" runat="server" Text="週期" OnCheckedChanged="DisplayCheckedChanged" AutoPostBack="true" CssClass="glcheck" TextAlign="Right" />
                            <asp:CheckBox ID="chkRecalc" runat="server" Text="重算" OnCheckedChanged="DisplayCheckedChanged" AutoPostBack="true" CssClass="glcheck glcheck-yellow" TextAlign="Right" />
                        </div>

                        <div>
                            <asp:CheckBox ID="chkShowProcess" runat="server" Text="顯示過程" OnCheckedChanged="DisplayCheckedChanged" AutoPostBack="true" CssClass="glcheck" TextAlign="Right" />
                            <asp:CheckBox ID="chkshowGraphic" runat="server" Text="顯示圖形" OnCheckedChanged="DisplayCheckedChanged" AutoPostBack="true" CssClass="glcheck" TextAlign="Right" />
                            <asp:CheckBox ID="chkShowStaticHtml" runat="server" Text="顯示靜態頁面" OnCheckedChanged="DisplayCheckedChanged" AutoPostBack="true" CssClass="glcheck" TextAlign="Right" />
                        </div>
                        <%--<asp:ImageButton ID="btnTTup" runat="server" ImageUrl="~/Resources/up.gif"
                            AlternateText="Up" Width="15" Height="15" />
                        <asp:ImageButton ID="btnTTdown" runat="server" ImageUrl="~/Resources/down.gif"
                            AlternateText="Down" Width="15" Height="15" />
                        <ajaxToolkit:NumericUpDownExtender ID="NumericUpDownExtender3" runat="server"
                            TargetControlID="txtTestTimes"
                            TargetButtonUpID="btnTTup"
                            TargetButtonDownID="btnTTdown"
                            Width="60" Minimum="1" />--%>
                    </div>
                    <div runat="server" style="color: white;">
                        <asp:CheckBox ID="chkSearchFields" runat="server" Text="多欄位計算 " OnCheckedChanged="DisplayCheckedChanged" AutoPostBack="true" CssClass="glcheck glcheck-yellow" TextAlign="Right" />
                        <asp:PlaceHolder ID="phDisplayPeriod" runat="server">
                            <asp:Label ID="lblDisplayPeriod" runat="server" Text="顯示期數DP" CssClass="gllabel" />
                            <asp:TextBox ID="txtDisplayPeriod" runat="server" Text="200" OnTextChanged="TxtDisplayPeriodTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60px" />
                            <asp:ImageButton ID="ibDisplayPeriod" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBDisplayPeriodClick" CssClass="glclear" BackColor="White" />
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phCriticalNum" runat="server">
                            <asp:Label ID="lblCriticalNum" runat="server" Text="臨界值CN=>" CssClass="gllabel" />
                            <asp:TextBox ID="txtCriticalNum" runat="server" Text="7" OnTextChanged="TxtCriticalNumTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60" />
                            <asp:ImageButton ID="ibCriticalNum" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBCriticalNumClick" CssClass="glclear" BackColor="White" />
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phTestPeriods" runat="server">
                            <asp:Label ID="lblTestPeriods" runat="server" Text="連續測試期數" CssClass="gllabel" />
                            <asp:TextBox ID="txtTestPeriods" runat="server" Text="1" OnTextChanged="TxtTestPeriodsTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60" />
                            <asp:ImageButton ID="ibTestPeriods" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBTestPeriodsClick" CssClass="glclear" BackColor="White" />
                        </asp:PlaceHolder>
                    </div>
                </asp:Panel>
            </details>

            <%--比較欄位--%>
            <details id="dtlCompare" runat="server" visible="false" open="open" class="gldetails">
                <summary class="Title">比較欄位</summary>
                <asp:Panel ID="pnlCompare" runat="server" CssClass="glsearch-content gltext_center">
                    <asp:PlaceHolder ID="phCompare01" runat="server">
                        <asp:Label ID="lblCompare01" runat="server" Text="欄位1" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbCompare01" runat="server" OnSelectedIndexChanged="CmbCompareSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phCompare02" runat="server">
                        <asp:Label ID="lblCompare02" runat="server" Text="欄位2" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbCompare02" runat="server" OnSelectedIndexChanged="CmbCompareSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phCompare03" runat="server">
                        <asp:Label ID="lblCompare03" runat="server" Text="欄位3" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbCompare03" runat="server" OnSelectedIndexChanged="CmbCompareSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phCompare04" runat="server">
                        <asp:Label ID="lblCompare04" runat="server" Text="欄位4" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbCompare04" runat="server" OnSelectedIndexChanged="CmbCompareSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phCompare05" runat="server">
                        <asp:Label ID="lblCompare05" runat="server" Text="欄位5" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbCompare05" runat="server" OnSelectedIndexChanged="CmbCompareSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                </asp:Panel>
            </details>

            <%--拖牌設定--%>
            <details id="dtlNextNum" runat="server" visible="false" open="open" class="gldetails">
                <summary class="Title">拖牌設定</summary>
                <asp:Panel ID="pnlNextNumC" runat="server" CssClass="glsearch-content gltext_center">
                    <asp:PlaceHolder ID="phNextNums" runat="server">
                        <asp:Label ID="lblNextNums" runat="server" Text="星數資料" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbNextNums" runat="server" OnSelectedIndexChanged="CmbNextNumsSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Text="0" Value="0" Selected="True" />
                            <asp:ListItem Text="1" Value="1" />
                            <asp:ListItem Text="2" Value="2" />
                            <asp:ListItem Text="3" Value="3" />
                            <asp:ListItem Text="4" Value="4" />
                        </asp:DropDownList>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phNextStep" runat="server">
                        <asp:Label ID="lblNextStep" runat="server" Text="間隔期數" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbNextStep" runat="server" OnSelectedIndexChanged="CmbNextStepSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist">
                            <asp:ListItem Text="1" Value="1" Selected="True" />
                            <asp:ListItem Text="2" Value="2" />
                            <asp:ListItem Text="3" Value="3" />
                            <asp:ListItem Text="4" Value="4" />
                            <asp:ListItem Text="5" Value="5" />
                        </asp:DropDownList>
                    </asp:PlaceHolder>
                </asp:Panel>
            </details>

            <%--
                        <asp:Panel ID="pnlNextNumT" CssClass="glsearch-title-even" Font-Size="Large" runat="server" Visible="false">
                        <img id="imgNextNum" style="width: 20px;" src="../Resources/btnDownarrow.png" />
                        <asp:Label ID="Label12" runat="server" Text="Label">拖牌設定</asp:Label>
                        </asp:Panel>
                        <ajaxToolkit:CollapsiblePanelExtender runat="server" ID="cpeNextNum" TargetControlID="pnlNextNumC" CollapseControlID="pnlNextNumT" ExpandControlID="pnlNextNumT" ImageControlID="imgNextNum" CollapsedImage="~/Resources/btnDownarrow.png" ExpandedImage="~/Resources/btnUparrow.png" Collapsed="false" SuppressPostBack="true" />
            --%>

            <%-- 區間欄位 --%>
            <details id="dtlFCheck" runat="server" visible="false" open="open" class="gldetails">
                <summary class="Title">區間欄位</summary>
                <asp:Panel ID="pnlFCheck" runat="server" CssClass="glsearch-content gltext_center">
                    <asp:PlaceHolder ID="phFCheck01" runat="server">
                        <asp:Label ID="lblFCheck01" runat="server" Text="欄位1" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbFCheck01" runat="server" OnSelectedIndexChanged="CmbFCheckSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phFCheck02" runat="server">
                        <asp:Label ID="lblFCheck02" runat="server" Text="欄位2" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbFCheck02" runat="server" OnSelectedIndexChanged="CmbFCheckSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phFCheck03" runat="server">
                        <asp:Label ID="lblFCheck03" runat="server" Text="欄位3" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbFCheck03" runat="server" OnSelectedIndexChanged="CmbFCheckSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phFCheck04" runat="server">
                        <asp:Label ID="lblFCheck04" runat="server" Text="欄位4" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbFCheck04" runat="server" OnSelectedIndexChanged="CmbFCheckSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phFCheck05" runat="server">
                        <asp:Label ID="lblFCheck05" runat="server" Text="欄位5" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbFCheck05" runat="server" OnSelectedIndexChanged="CmbFCheckSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phFCheck06" runat="server">
                        <asp:Label ID="lblFCheck06" runat="server" Text="欄位6" CssClass="gllabel" />
                        <asp:DropDownList ID="cmbFCheck06" runat="server" OnSelectedIndexChanged="CmbFCheckSelectedIndexChanged" AutoPostBack="true" CssClass="gldropdownlist" />
                    </asp:PlaceHolder>

                </asp:Panel>
            </details>

            <%-- 數字DNA --%>
            <details id="dtlFreqDNA" runat="server" visible="false" open="open" class="gldetails">
                <summary class="Title">數字DNA</summary>
                <asp:Panel ID="pnlFreqDNA" runat="server" CssClass="glsearch-content gltext_center ">
                    <asp:PlaceHolder ID="phFreqDNALength" runat="server">
                        <asp:Label ID="lblFreqDNALength" runat="server" Text="DNA長度" CssClass="gllabel" />
                        <asp:TextBox ID="txtFreqDNALength" runat="server" Text="5" OnTextChanged="TxtFreqDNATextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60px" />
                        <asp:ImageButton ID="ibFreqDNALength" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBFreqDnaLengthClick" CssClass="glclear" BackColor="White" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phTargetTestPeriods" runat="server">
                        <asp:Label ID="lblTargetTestPeriods" runat="server" Text="預計驗證期數" CssClass="gllabel" />
                        <asp:TextBox ID="txtTargetTestPeriods" runat="server" Text="200" OnTextChanged="TxtFreqDNATextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60px" />
                        <asp:ImageButton ID="ibTargetTestPeriods" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBTargetTestPeriodsClick" CssClass="glclear" BackColor="White" />
                    </asp:PlaceHolder>
                </asp:Panel>

            </details>

            <%--搜尋方法--%>
            <details id="dtlSearch" runat="server" open="open" class="gldetails">
                <summary class="Title">搜尋方法</summary>
                <asp:Panel ID="pnlSearch" runat="server" CssClass="glsearch-content gltext_center ">

                    <asp:PlaceHolder ID="phHistorySNs" runat="server">
                        <asp:Label ID="lblHistorySNs" runat="server" Text="多序號" CssClass="gllabel" />
                        <asp:TextBox ID="txtHistorySNs" runat="server" Text="" OnTextChanged="TxtHistorySearchTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60" />
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phHistoryPeriods" runat="server">
                        <asp:Label ID="lblHistoryPeriods" runat="server" Text="歷史搜尋期數" CssClass="gllabel" />
                        <asp:TextBox ID="txtHistoryPeriods" runat="server" Text="30" OnTextChanged="TxtHistorySearchTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60px" />
                        <asp:ImageButton ID="ibHistoryPeriods" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBHistoryPeriodsClick" CssClass="glclear" BackColor="White" />
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phHistoryTestPeriods" runat="server">
                        <asp:Label ID="lblHistoryTestPeriods" runat="server" Text="往前驗證期數" CssClass="gllabel" />
                        <asp:TextBox ID="txtHistoryTestPeriods" runat="server" Text="20" OnTextChanged="TxtHistorySearchTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60px" />
                        <asp:ImageButton ID="ibHistoryTestPeriods" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBHistoryTestPeriodsClick" CssClass="glclear" BackColor="White" />
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phHitMin" runat="server">
                        <asp:Label ID="lblHitMin" runat="server" Text="最小中獎數" CssClass="gllabel" />
                        <asp:TextBox ID="txtHitMin" runat="server" Text="2" OnTextChanged="TxtHistorySearchTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60px" />
                        <asp:ImageButton ID="ibHitMin" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBHitMinClick" CssClass="glclear" BackColor="White" />

                        <%--                         <asp:ImageButton ID="btnHMinUp" runat="server" ImageUrl="~/Resources/up.gif" AlternateText="Up" Width="15" Height="15" />
                        <asp:ImageButton ID="btnHMinDown" runat="server" ImageUrl="~/Resources/down.gif" AlternateText="Down" Width="15" Height="15" />
                        <ajaxToolkit:NumericUpDownExtender ID="NumericHMin" runat="server" TargetControlID="txtHitMin" TargetButtonUpID="btnHMinUp" TargetButtonDownID="btnHMinDown" Width="60" Minimum="0" />--%>

                    </asp:PlaceHolder>
                    <br />

                    <asp:CheckBox ID="chkFilterRange" runat="server" Text="次數篩選" OnCheckedChanged="ChkFilterRangeCheckedChanged" AutoPostBack="true" CssClass="glcheck glcheck-yellow" TextAlign="Right" />
                    <asp:PlaceHolder ID="phFilterRange" runat="server" Visible="false">
                        <asp:Label ID="lblFilterRange" runat="server" Text="指定篩選值" CssClass="gllabel" />
                        <asp:TextBox ID="txtFilterRange" runat="server" Text="" OnTextChanged="TxtHistorySearchTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60" />
                        <asp:ImageButton ID="ibFilterRange" runat="server" ImageUrl="~/Resources/btncancel.png" BackColor="White" CssClass="glclear" OnClick="IBFilterRangeClick" />

                        <asp:Label ID="lblFilterMin" runat="server" Text="最小篩選值" CssClass="gllabel" />
                        <asp:TextBox ID="txtFilterMin" runat="server" Text="0" OnTextChanged="TxtHistorySearchTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60" />
                        <asp:ImageButton ID="ibFilterMin" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBFilterMinClick" CssClass="glclear" BackColor="White" />

                        <%--                        <asp:ImageButton ID="btnFMinUp" runat="server" ImageUrl="~/Resources/up.gif" AlternateText="Up" Width="15" Height="15" />
                        <asp:ImageButton ID="btnFMinDown" runat="server" ImageUrl="~/Resources/down.gif" AlternateText="Down" Width="15" Height="15" />
                        <ajaxToolkit:NumericUpDownExtender ID="NumericFMin" runat="server" TargetControlID="txtFilterMin" TargetButtonUpID="btnFMinUp" TargetButtonDownID="btnFMinDown" Width="60" Minimum="0" />--%>

                        <asp:Label ID="lblFilterMax" runat="server" Text="最大篩選值" CssClass="gllabel" />
                        <asp:TextBox ID="txtFilterMax" runat="server" Text="0" OnTextChanged="TxtHistorySearchTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60" />
                        <asp:ImageButton ID="ibFilterMax" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBFilterMaxClick" CssClass="glclear" BackColor="White" />

                        <%--                        <asp:ImageButton ID="btnFMaxUp" runat="server" ImageUrl="~/Resources/up.gif" AlternateText="Up" Width="15" Height="15" />
                        <asp:ImageButton ID="btnFMaxDown" runat="server" ImageUrl="~/Resources/down.gif" AlternateText="Down" Width="15" Height="15" />
                        <ajaxToolkit:NumericUpDownExtender ID="NumericFMax" runat="server" TargetControlID="txtFilterMax" TargetButtonUpID="btnFMaxUp" TargetButtonDownID="btnFMaxDown" Width="60" Minimum="0" />--%>

                    </asp:PlaceHolder>

                </asp:Panel>
            </details>

            <%-- 百分比 --%>
            <details id="dtlTP" runat="server" open="open" class="gldetails">
                <summary class="Title">TablePercent</summary>
                <asp:Panel ID="pnlTP" runat="server" CssClass="glsearch-content gltext_center ">
                    <asp:PlaceHolder ID="phDelete" runat="server">
                        <asp:Label ID="lblDelete" runat="server" Text="刪除" CssClass="gllabel" />
                        <asp:TextBox ID="txtDelete" runat="server" OnTextChanged="TxtTablePercentTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="260px" />
                        <asp:ImageButton ID="ibDelete" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBDeleteClick" CssClass="glclear" BackColor="White" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phNotDelete" runat="server">
                        <asp:Label ID="lblNotDelete" runat="server" Text="排除" CssClass="gllabel" />
                        <asp:TextBox ID="txtNotDelete" runat="server" OnTextChanged="TxtTablePercentTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="260px" />
                        <asp:ImageButton ID="ibNotDelete" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBNotDeleteClick" CssClass="glclear" BackColor="White" />
                    </asp:PlaceHolder>
                    <%--<gc:GTextBox ID="DataRowsLimit" runat="server" LabelClass="gllabel" LabelText="期數DRL=>" TextBoxText="50" TextBoxClass="gltext" TextBoxStyle="width:60px" />--%>
                    <asp:PlaceHolder ID="phDataRowsLimit" runat="server">
                        <asp:Label ID="lblDataRowsLimit" runat="server" Text="期數DRL=>" CssClass="gllabel" />
                        <asp:TextBox ID="txtDataRowsLimit" runat="server" Text="50" OnTextChanged="TxtTablePercentTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="60px" />
                        <asp:ImageButton ID="ibDataRowsLimit" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBDataRowsLimitClick" CssClass="glclear" BackColor="White" />
                    </asp:PlaceHolder>

                </asp:Panel>
            </details>

            <%--聰明組合--%>
            <details id="dtlSmartRun" runat="server" visible="true" open="open" class="gldetails">
                <summary class="Title">聰明組合</summary>
                <asp:Panel ID="pnlSmart" runat="server" CssClass="glsearch-content gltext_center">
                    <asp:PlaceHolder ID="phOdds" runat="server">
                        <asp:Label ID="lblOdds" runat="server" Text="奇數" CssClass="gllabel" />
                        <asp:TextBox ID="txtOdds" runat="server" OnTextChanged="TxtSmartTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="50px" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phHigh" runat="server">
                        <asp:Label ID="lblHigh" runat="server" Text="大數" CssClass="gllabel" />
                        <asp:TextBox ID="txtHigh" runat="server" OnTextChanged="TxtSmartTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="50px" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phHot" runat="server">
                        <asp:Label ID="lblHot" runat="server" Text="熱門" CssClass="gllabel" />
                        <asp:TextBox ID="txtHot" runat="server" OnTextChanged="TxtSmartTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="50px" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phSumMin" runat="server">
                        <asp:Label ID="lblSumMin" runat="server" Text="和數低值" CssClass="gllabel" />
                        <asp:TextBox ID="txtSumMin" runat="server" OnTextChanged="TxtSmartTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="50px" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phSumMax" runat="server">
                        <asp:Label ID="lblSumMax" runat="server" Text="和數高值" CssClass="gllabel" />
                        <asp:TextBox ID="txtSumMax" runat="server" OnTextChanged="TxtSmartTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="50px" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phTestNum" runat="server">
                        <asp:Label ID="lblTestNum" runat="server" Text="測試數字" CssClass="gllabel" />
                        <asp:TextBox ID="txtTestNum" runat="server" OnTextChanged="TxtSmartTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="400px" />
                        <asp:ImageButton ID="ibTestNum" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBTestNumClick" CssClass="glclear" />
                    </asp:PlaceHolder>
                </asp:Panel>
            </details>

            <details id="dtlSmartTest" runat="server" visible="true" open="open" class="gldetails">
                <summary class="Title">聰明組合</summary>
                <asp:Panel ID="Panel2" runat="server" CssClass="glsearch-content gltext_center">
                    <asp:PlaceHolder ID="phRndNums" runat="server">
                        <asp:Label ID="lblRndNums" runat="server" Text="隨機數字數目" CssClass="gllabel" />
                        <asp:DropDownList ID="ddlRndNums" runat="server" AutoPostBack="true" CssClass="gldropdownlist" />
                        <asp:Button ID="btnRndNums" runat="server" Text="隨機取值" OnClick="BtnRndNumsClick" CssClass="glbutton glbutton-blue" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phSmartTestNum" runat="server">
                        <asp:Label ID="lblSmartTestNum" runat="server" Text="測試數字" CssClass="gllabel" />
                        <asp:TextBox ID="txtSmartTestNum" runat="server" OnTextChanged="TxtSmartTestNumTextChanged" AutoPostBack="true" CssClass="gltext gltext_YellowBlack" Width="600px" />
                        <asp:ImageButton ID="ibSmartTestNum" runat="server" ImageUrl="~/Resources/btncancel.png" OnClick="IBSmartTestNumClick" CssClass="glclear" />
                        (數字分隔： ',' ，分柱分隔：'#' ，分隔階層：" , ＜ # ")
                    </asp:PlaceHolder>
                </asp:Panel>
            </details>

            <%--<asp:Timer ID="Timer1" runat="server" Interval="300000" OnTick="Timer1Tick" />--%>

            <asp:Label ID="lblArgument" runat="server" CssClass="gllabel" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
