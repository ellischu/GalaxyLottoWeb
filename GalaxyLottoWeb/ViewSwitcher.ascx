<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewSwitcher.ascx.cs" Inherits="GalaxyLottoWeb.ViewSwitcher" %>
<div id="viewSwitcher">
    <%: CurrentView %>ª© | <a href="<%: SwitchUrl %>" data-ajax="false"> <%: AlternateView %>ª©</a>
</div>