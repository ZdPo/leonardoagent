<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DepartureTable.ascx.cs"
    Inherits="Components_DepartureTable" %>
<%@ Register Src="HeaderCell.ascx" TagName="HeaderCell" TagPrefix="uc1" %>
<asp:Label ID="lbFilter" runat="server" Text="" Visible="false" />
<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
    OnRowDataBound="GridView1_RowDataBound" AllowSorting="True" OnSorting="GridView1_Sorting" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="DEST" SortExpression="S1">
            <HeaderTemplate>
                <uc1:HeaderCell ID="HeaderCellDEST" runat="server" SearchBoxSize="58" HeaderText="DEST"
                    SortExpression="S1" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="Label1" runat="server" Text='<%# Bind("S1") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="DOPRAVCE" SortExpression="IATAICAO">
            <HeaderTemplate>
                <uc1:HeaderCell ID="HeaderCellDOPRAVCE" runat="server" SearchBoxSize="58" HeaderText="DOPRAVCE"
                    SortExpression="IATAICAO" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="Label2" runat="server" Text='<%# Bind("IATAICAO") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="LET" SortExpression="FLGTNUM">
            <HeaderTemplate>
                <uc1:HeaderCell ID="HeaderCellLET" runat="server" SearchBoxSize="58" HeaderText="LET"
                    SortExpression="FLGTNUM" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="Label3" runat="server" Text='<%# Bind("FLGTNUM") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="SHARECODE">
            <ItemTemplate>
                <asp:Label ID="Label13" runat="server"></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="PLANOVANY CAS" SortExpression="TIME_LT">
            <HeaderTemplate>
                <uc1:HeaderCell ID="HeaderCellCAS" runat="server" SearchBoxSize="58" HeaderText="PLANOVANY CAS"
                    SortExpression="TIME_LT" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="Label4" runat="server" Text='<%# Bind("TIME_LT", "{0:dd.MM HH:mm}") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="PLANOVANY CAS" SortExpression="TIME_LT">
            <HeaderTemplate>
                <uc1:HeaderCell ID="HeaderCellCAS1" runat="server" SearchBoxSize="58" HeaderText="PLANOVANY CAS"
                    SortExpression="TIME_LT" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="Label5" runat="server" Text='<%# Bind("TIME_LT", "{0:dd.MM HH:mm}") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="WALLBOARD" SortExpression="REMARK">
            <HeaderTemplate>
                <uc1:HeaderCell ID="HeaderCellWALLBOARD" runat="server" SearchBoxSize="58" HeaderText="WALLBOARD"
                    SortExpression="REMARK" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="Label6" runat="server" Text='<%# Bind("REMARK") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="T" SortExpression="HAL">
            <HeaderTemplate>
                <uc1:HeaderCell ID="HeaderCellT" runat="server" SearchBoxSize="20" HeaderText="T"
                    SortExpression="HAL" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="Label7" runat="server" Text='<%# Bind("HAL") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="CHECK-IN" SortExpression="CHIN">
            <HeaderTemplate>
                <uc1:HeaderCell ID="HeaderCellCHECKIN" runat="server" SearchBoxSize="58" HeaderText="CHECK-IN"
                    SortExpression="CHIN" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="Label8" runat="server" Text='<%# Bind("CHIN") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="GATE" SortExpression="GATE">
            <HeaderTemplate>
                <uc1:HeaderCell ID="HeaderCellGATE" runat="server" SearchBoxSize="58" HeaderText="GATE"
                    SortExpression="GATE" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="Label9" runat="server" Text='<%# Bind("GATE") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="HANDLING" SortExpression="HDG">
            <HeaderTemplate>
                <uc1:HeaderCell ID="HeaderCellHDG" runat="server" SearchBoxSize="40" HeaderText="HANDLING"
                    SortExpression="HDG" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="Label10" runat="server" Text='<%# Bind("HDG") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="PRES" SortExpression="S2">
            <HeaderTemplate>
                <uc1:HeaderCell ID="HeaderCellPRES" runat="server" SearchBoxSize="58" HeaderText="PRES"
                    SortExpression="S2" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="Label11" runat="server" Text='<%# Bind("S2") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="TYP" SortExpression="AIRCRAFT">
            <HeaderTemplate>
                <uc1:HeaderCell ID="HeaderCellTYP" runat="server" SearchBoxSize="58" HeaderText="TYP"
                    SortExpression="AIRCRAFT" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="Label12" runat="server" Text='<%# Bind("AIRCRAFT") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
