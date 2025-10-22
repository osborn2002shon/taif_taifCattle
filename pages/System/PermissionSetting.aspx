<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="PermissionSetting.aspx.vb" Inherits="taifCattle.PermissionSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-users"></i>系統權限管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    系統權限管理
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="card shadow-sm">
         <div class="card-header bg-light">
             <h5 class="mb-0" id="formTitle">系統權限管理</h5>
         </div>
        <div class="card-body">

        </div>
        <div class="card-footer text-end">
            <asp:Button ID="Button_save" runat="server" CssClass="btn btn-primary me-2" Text="儲存" ValidationGroup="AccountForm" OnClientClick="return validateAccountForm();" />
            <asp:Button ID="Button_cancel" PostBackUrl="~/pages/System/AccountManage.aspx" runat="server" CssClass="btn btn-outline-secondary" Text="返回" CausesValidation="False" />
        </div>
    </div>
</asp:Content>
