<%@ Page Title="Leonardo - Odlety vse" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Departure.aspx.cs" Inherits="Departure" %>

<%@ Register src="Components/DepartureTable.ascx" tagname="DepartureTable" tagprefix="uc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <meta http-equiv="refresh" content='<%= ZdPo.Utils.RefreshTag() %>' />
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <uc1:DepartureTable ID="DepartureTable1" runat="server" DataSourceID="ObjectDataSource1" Header2="Odlety vse" />
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="DepartureData"
            TypeName="ZdPo.Data_Proxy"></asp:ObjectDataSource>
</asp:Content>
