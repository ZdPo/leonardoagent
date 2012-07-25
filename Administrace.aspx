<%@ Page Title="Administrace" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Administrace.aspx.cs" Inherits="Administrace" %>

<%@ Register Src="Components/Logon.ascx" TagName="Logon" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:Panel ID="Panel1" runat="server">
        <uc1:Logon ID="Logon1" runat="server" />
    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="ObjectDataSource1"
            DataKeyNames="HDGID">
            <Columns>
                <asp:BoundField DataField="HDGID" HeaderText="HDGID" SortExpression="HDGID" ReadOnly="True" />
                <asp:BoundField DataField="Line1" HeaderText="Line1" SortExpression="Line1" />
                <asp:BoundField DataField="Line2" HeaderText="Line2" SortExpression="Line2" />
                <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
            </Columns>
        </asp:GridView>
        <h2>
            Novy zaznam</h2>
        <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataSourceID="ObjectDataSource1"
            DefaultMode="Insert" Height="50px" Width="125px">
            <Fields>
                <asp:BoundField DataField="HDGID" HeaderText="HDGID" SortExpression="HDGID" />
                <asp:BoundField DataField="Line1" HeaderText="Line1" SortExpression="Line1" />
                <asp:BoundField DataField="Line2" HeaderText="Line2" SortExpression="Line2" />
                <asp:CommandField ShowInsertButton="True" />
            </Fields>
        </asp:DetailsView>
        <br />
        <asp:Button ID="logout" runat="server" Text="Log Off" OnClick="logout_Click"></asp:Button>
    </asp:Panel>
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" DeleteMethod="Delete"
        InsertMethod="Insert" OldValuesParameterFormatString="original_{0}" SelectMethod="GetAllTable"
        TypeName="ZdPo.Admin_Proxy" UpdateMethod="Update">
        <DeleteParameters>
            <asp:Parameter Name="HDGID" Type="String" />
            <asp:Parameter Name="line1" Type="String" />
            <asp:Parameter Name="line2" Type="String" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="HDGID" Type="String" />
            <asp:Parameter Name="line1" Type="String" />
            <asp:Parameter Name="line2" Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="line1" Type="String" />
            <asp:Parameter Name="line2" Type="String" />
            <asp:Parameter Name="original_HDGID" Type="String" />
            <asp:Parameter Name="original_line1" Type="String" />
            <asp:Parameter Name="original_line2" Type="String" />
        </UpdateParameters>
    </asp:ObjectDataSource>
</asp:Content>
