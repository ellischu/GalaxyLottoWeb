<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewSwitcher.ascx.cs" Inherits="GalaxyLottoWeb.ViewSwitcher" %>
<div id="viewSwitcher">
    <%: CurrentView %>�� | <a href="<%: SwitchUrl %>" data-ajax="false"> <%: AlternateView %>��</a>
</div>