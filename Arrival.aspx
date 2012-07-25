<%@ Page Title="Leonardo - Prilety vse" Language="C#" MasterPageFile="~/SiteA.master"
    AutoEventWireup="true" CodeFile="Arrival.aspx.cs" Inherits="_Default" %>

<%@ Register Src="Components/ArriveTable.ascx" TagName="ArriveTable" TagPrefix="uc1" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <meta http-equiv="refresh" content='<%= ZdPo.Utils.RefreshTag() %>' />
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
        <uc1:ArriveTable ID="ArriveTable1" runat="server" DataSourceID="ObjectDataSource1" Header2="Prilety vse" />
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="ArriveData"
        TypeName="ZdPo.Data_Proxy"></asp:ObjectDataSource>
</asp:Content>
