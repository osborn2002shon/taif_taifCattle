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
            <div class="row g-3 align-items-end mb-3">
                <div class="col-md-4">
                    <label for="DropDownList_role" class="form-label">請選擇系統權限角色</label>
                    <asp:DropDownList ID="DropDownList_role" runat="server" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="DropDownList_role_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col-md-8">
                    <asp:Label ID="Label_status" runat="server" CssClass="text-muted"></asp:Label>
                </div>
            </div>

            <asp:Panel ID="Panel_permission" runat="server" Visible="False">
                <div class="alert alert-info" role="alert">
                    <strong>目前設定角色：</strong>
                    <asp:Literal ID="Literal_roleName" runat="server"></asp:Literal>
                </div>

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
                                            <th scope="col" class="text-center" style="width:12%;">查詢</th>
                                            <th scope="col" class="text-center" style="width:12%;">新增</th>
                                            <th scope="col" class="text-center" style="width:12%;">修改</th>
                                            <th scope="col" class="text-center" style="width:12%;">刪除</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="Repeater_menus" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <asp:HiddenField ID="HiddenField_menuID" runat="server" Value='<%# Eval("MenuID") %>' />
                                                        <div class="fw-semibold"><%# Eval("MenuName") %></div>
                                                        <div class="text-muted small"><%# Eval("MenuURL") %></div>
                                                    </td>
                                                    <td class="text-center">
                                                        <asp:CheckBox ID="CheckBox_read" runat="server" CssClass="position-static" Checked='<%# Eval("CanRead") %>' />
                                                    </td>
                                                    <td class="text-center">
                                                        <asp:CheckBox ID="CheckBox_create" runat="server" CssClass="position-static" Checked='<%# Eval("CanCreate") %>' />
                                                    </td>
                                                    <td class="text-center">
                                                        <asp:CheckBox ID="CheckBox_update" runat="server" CssClass="position-static" Checked='<%# Eval("CanUpdate") %>' />
                                                    </td>
                                                    <td class="text-center">
                                                        <asp:CheckBox ID="CheckBox_delete" runat="server" CssClass="position-static" Checked='<%# Eval("CanDelete") %>' />
                                                    </td>
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
