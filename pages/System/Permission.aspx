<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="Permission.aspx.vb" Inherits="taifCattle.Permission" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-users-gear"></i>系統權限管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    系統權限管理
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="card shadow-sm">
        <div class="card-header bg-light">
            <h5 class="mb-0">系統權限設定</h5>
        </div>
        <div class="card-body">
            <div class="mb-3">
                <asp:Label ID="Label_status" runat="server" CssClass="text-muted"></asp:Label>
            </div>

            <asp:Panel ID="Panel_permission" runat="server" Visible="False">
                <asp:Repeater ID="Repeater_groups" runat="server" OnItemDataBound="Repeater_groups_ItemDataBound">
                    <ItemTemplate>
                        <div class="mb-4 border rounded">
                            <div class="bg-light border-bottom px-3 py-2">
                                <h6 class="mb-0"><%# Eval("GroupName") %></h6>
                            </div>
                            <div class="table-responsive">
                                <table class="table table-hover align-middle mb-0">
                                    <thead class="table-light">
                                        <tr>
                                            <th scope="col" style="width:45%;">功能名稱</th>
                                            <asp:Repeater ID="Repeater_roleHeader" runat="server">
                                                <ItemTemplate>
                                                    <th scope="col" class="text-center"><%# Eval("AuTypeName") %></th>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tr>
                                    </thead>
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
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:Panel>
        </div>
        <div class="card-footer text-end">
            <asp:Label ID="Label_result" runat="server" CssClass="me-auto text-start d-inline-block"></asp:Label>
            <asp:Button ID="Button_reload" runat="server" CssClass="btn btn-outline-secondary me-2" Text="重新載入" CausesValidation="False" OnClick="Button_reload_Click" />
            <asp:Button ID="Button_save" runat="server" CssClass="btn btn-primary" Text="儲存設定" />
            <asp:Button ID="Button_cancel" PostBackUrl="~/pages/System/AccountManage.aspx" runat="server" CssClass="btn btn-outline-secondary" Text="返回" CausesValidation="False" />
        </div>
    </div>
</asp:Content>
