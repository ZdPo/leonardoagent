<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Logon.ascx.cs" Inherits="Components_Logon" %>
<table id="mainTable" border="0">
    <tr>
        <td>
            <table class="t_border" id="loginTable" cellspacing="15" cellpadding="0">
                <tr>
                    <td>
                        <b>Login: </b>
                    </td>
                    <td>
                        <asp:TextBox ID="txtUserName" runat="server" Width="160px"></asp:TextBox><asp:RequiredFieldValidator
                            ID="rvUserValidator" runat="server" ControlToValidate="txtUserName" ErrorMessage="You must supply a Username!"
                            Display="None"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Password: </b>
                    </td>
                    <td>
                        <asp:TextBox ID="txtPassword" runat="server" Width="160px" TextMode="Password"></asp:TextBox><asp:RequiredFieldValidator
                            ID="rvPasswordValidator" runat="server" ControlToValidate="txtPassword" ErrorMessage="Empty Passwords not accepted"
                            Display="None"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <asp:Button ID="cmdSubmit" runat="server" Text="Submit" BorderStyle="Solid" 
                            onclick="cmdSubmit_Click"></asp:Button>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table id="messageDisplay">
                <tr>
                    <td>
                        <asp:ValidationSummary ID="Validationsummary1" runat="server" Width="472px" DisplayMode="BulletList">
                        </asp:ValidationSummary>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:label id="lblMessage" runat="server" width="288px" font-bold="True" font-italic="True"
            font-size="Medium" forecolor="#C00000"></asp:label>
        <asp:label id="lblMessage2" runat="server" width="288px" font-bold="True" font-italic="True"
            font-size="Medium" forecolor="#C00000"></asp:label>
