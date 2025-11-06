<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="MyAccount.aspx.vb" Inherits="taifCattle.MyAccount" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-address-card"></i> 我的帳號設定
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    我的帳號設定
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="card formCard" style="margin-bottom:50px;">
        <div class="card-header">基本資料</div>
        <div class="card-body">
            <div class="row">
                <div class="col">
                    <label>最後登入時間</label>
                    <asp:Label ID="Label_lastLogin" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                </div>
                <div class="col">
                    <label>系統權限</label>
                    <asp:Label ID="Label_role" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <label>帳號名稱</label>
                    <asp:Label ID="Label_account" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                </div>
                <div class="col">
                    <label>我的姓名</label>
                    <asp:Label ID="Label_name" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                </div>
            </div>
            <div class="row">
                <asp:Panel ID="Panel_govCity" runat="server" CssClass="col" Visible="false">
                    <label>縣市</label>
                    <asp:Label ID="Label_govCity" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                </asp:Panel>
                <div class="col">
                    <label>聯絡電話</label>
                    <asp:TextBox ID="TextBox_mobile" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                </div>
                <div class="col">
                    <label>服務單位</label>
                    <asp:TextBox ID="TextBox_unit" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="LinkButton_saveBasic" runat="server" CssClass="btn btn-primary"><i class="fas fa-save me-1 me-1"></i>儲存設定</asp:LinkButton>
        </div>
    </div>
    <div class="card formCard">
        <div class="card-header">密碼變更</div>
        <div class="card-body">
            <div class="row">
                <div class="col">
                    <label>密碼最後變更時間</label>
                    <asp:Label ID="Label_passwordChanged" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <label>輸入新密碼<span class="text-danger">*</span></label>
                    <asp:TextBox ID="TextBox_newPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                </div>
                <div class="col">
                    <label>確認新密碼<span class="text-danger">*</span></label>
                    <asp:TextBox ID="TextBox_confirmPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="LinkButton_changePassword" runat="server" CssClass="btn btn-warning"><i class="fas fa-save me-1 me-1"></i>變更密碼</asp:LinkButton>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
    系統訊息
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
    <asp:Label ID="Label_Message" runat="server" CssClass="msg"></asp:Label>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
