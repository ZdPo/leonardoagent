<%@ Page Title="Leonardo - Odlety +/- 60" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Departure60.aspx.cs" Inherits="Departure60" %>

<%@ Register Src="Components/DepartureTable.ascx" TagName="DepartureTable" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <meta http-equiv="refresh" content='<%= ZdPo.Utils.RefreshTag() %>' />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <uc1:DepartureTable ID="DepartureTable1" runat="server" DataSourceID="ObjectDataSource1" Header2="Odlety +/- 60" />
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="DepartureData60"
        TypeName="ZdPo.Data_Proxy"></asp:ObjectDataSource>
</asp:Content>
