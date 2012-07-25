<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HeaderCell.ascx.cs" Inherits="Components_HeaderCell" ViewStateMode="Enabled" %>
<asp:TextBox ID="txbSearch" runat="server" SkinID="SmallTextBox" ViewStateMode="Enabled"></asp:TextBox><br />
<asp:Label ID="lbName" runat="server" Text="DEST" />&nbsp;
<asp:LinkButton ID="lbUP" runat="server" CommandName="Up" OnCommand="lbUP_Command"><%=ZdPo.Constants.CommonConst.ArrowUp %></asp:LinkButton>
<asp:LinkButton ID="lbDown" runat="server" CommandName="Down" OnCommand="lbDown_Command"><%=ZdPo.Constants.CommonConst.ArrowDown %></asp:LinkButton>
