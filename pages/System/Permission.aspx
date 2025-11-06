<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="Permission.aspx.vb" Inherits="taifCattle.Permission" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-users"></i> 系統帳號管理：權限設定
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="card formCard">
        <div class="card-header">
            權限設定
        </div>
        <div class="card-body">
            <div class="mb-3">
                <asp:Label ID="Label_status" runat="server" CssClass="text-muted"></asp:Label>
            </div>
            <asp:Panel ID="Panel_permission" runat="server" Visible="False">
                <asp:Repeater ID="Repeater_groups" runat="server" OnItemDataBound="Repeater_groups_ItemDataBound">
                    <ItemTemplate>
                        <div style="margin-bottom:15px">
                            <div style="font-weight:700;font-size:1.25rem;">
                                <i class="fa-solid fa-tag me-1"></i><%# Eval("GroupName") %>
                            </div>                        
                            <table class="tb">
                                <tr>
                                    <th scope="col" style="width:45%;">功能名稱</th>
                                    <asp:Repeater ID="Repeater_roleHeader" runat="server">
                                        <ItemTemplate>
                                            <th scope="col" class="text-center"><%# Eval("AuTypeName") %></th>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tr>
                                <tbody>
                                    <asp:Repeater ID="Repeater_menus" runat="server" OnItemDataBound="Repeater_menus_ItemDataBound">
                                        <ItemTemplate>
                                            <tr>
                                                <td>
                                                    <asp:HiddenField ID="HiddenField_menuID" runat="server" Value='<%# Eval("MenuID") %>' />
                                                    <div class="fw-semibold"><%# Eval("MenuName") %></div>
                                                    <div class="text-muted small"><%# Eval("MenuURL") %></div>
                                                </td>
                                                <asp:Repeater ID="Repeater_rolePermissions" runat="server">
                                                    <ItemTemplate>
                                                        <td class="text-center">
                                                            <asp:HiddenField ID="HiddenField_roleID" runat="server" Value='<%# Eval("AuTypeID") %>' />
                                                            <asp:CheckBox ID="CheckBox_enabled" runat="server" CssClass="position-static" Checked='<%# Eval("IsEnabled") %>' />
                                                        </td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:Panel>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="Button_save" runat="server" CssClass="btn btn-primary">
                <i class="fa-solid fa-floppy-disk me-1"></i>儲存設定</asp:LinkButton>
            <asp:LinkButton ID="Button_reload" runat="server" CssClass="btn btn-warning"  CausesValidation="False" OnClick="Button_reload_Click">
                    <i class="fa-solid fa-rotate-right me-1"></i>重新載入</asp:LinkButton>
            <asp:LinkButton ID="Button_cancel"  PostBackUrl="~/pages/System/AccountManage.aspx" runat="server" CssClass="btn btn-outline-secondary"  CausesValidation="False"><i class="fa-solid fa-arrow-left"></i> 返回帳號列表</asp:LinkButton>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
    系統訊息
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
    <asp:Label ID="Label_result" runat="server" CssClass="msg"></asp:Label>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
