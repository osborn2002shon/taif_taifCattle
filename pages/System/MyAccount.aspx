<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="MyAccount.aspx.vb" Inherits="taifCattle.MyAccount" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <style type="text/css">

        .nav-tabs .nav-link {
            color:#000;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    我的帳號管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    我的帳號管理
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <asp:HiddenField ID="HiddenField_activeTab" runat="server" />
    <div class="card shadow-sm">
        <div class="card-header bg-light">
            <ul class="nav nav-tabs card-header-tabs" id="accountTab" role="tablist">
                <li class="nav-item" role="presentation">
                    <button class="nav-link active" id="tabBasicLink" data-bs-toggle="tab" data-bs-target="#tabBasic" type="button" role="tab" aria-controls="tabBasic" aria-selected="true">
                        <i class="fa-solid fa-id-card me-1"></i>基本資料
                    </button>
                </li>
                <li class="nav-item" role="presentation">
                    <button class="nav-link" id="tabPasswordLink" data-bs-toggle="tab" data-bs-target="#tabPassword" type="button" role="tab" aria-controls="tabPassword" aria-selected="false">
                        <i class="fa-solid fa-key me-1"></i>變更密碼
                    </button>
                </li>
            </ul>
        </div>
        <div class="card-body">
            <div class="tab-content" id="accountTabContent">
                <div class="tab-pane fade show active" id="tabBasic" role="tabpanel" aria-labelledby="tabBasicLink">
                    <asp:Label ID="Label_basicMessage" runat="server" CssClass="d-block fw-bold mb-3"></asp:Label>
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label">登入帳號</label>
                            <asp:Label ID="Label_account" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">使用者姓名</label>
                            <asp:Label ID="Label_name" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">系統權限</label>
                            <asp:Label ID="Label_role" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                        </div>
                        <asp:Panel ID="Panel_govCity" runat="server" CssClass="col-md-6" Visible="false">
                            <label class="form-label">縣市</label>
                            <asp:Label ID="Label_govCity" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                        </asp:Panel>
                        <div class="col-md-6">
                            <label class="form-label">聯絡電話</label>
                            <asp:TextBox ID="TextBox_mobile" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">服務單位</label>
                            <asp:TextBox ID="TextBox_unit" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">最後登入時間</label>
                            <asp:Label ID="Label_lastLogin" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">密碼最後變更時間</label>
                            <asp:Label ID="Label_passwordChanged" runat="server" CssClass="form-control-plaintext d-block border rounded px-3 py-2 bg-light"></asp:Label>
                        </div>
                    </div>
                    <div class="text-end mt-4">
                        <asp:Button ID="Button_saveBasic" runat="server" CssClass="btn btn-primary" Text="儲存基本資料" />
                    </div>
                </div>
                <div class="tab-pane fade" id="tabPassword" role="tabpanel" aria-labelledby="tabPasswordLink">
                    <asp:Label ID="Label_passwordMessage" runat="server" CssClass="d-block fw-bold mb-3"></asp:Label>
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label">新密碼<span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_newPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">確認新密碼<span class="text-danger">*</span></label>
                            <asp:TextBox ID="TextBox_confirmPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                        </div>
                    </div>
                    <div class="text-end mt-4">
                        <asp:Button ID="Button_changePassword" runat="server" CssClass="btn btn-warning" Text="變更密碼" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            var activeField = document.getElementById('<%= HiddenField_activeTab.ClientID %>');
            var triggerTabList = document.querySelectorAll('#accountTab button[data-bs-toggle="tab"]');
            triggerTabList.forEach(function (triggerEl) {
                triggerEl.addEventListener('shown.bs.tab', function (event) {
                    if (activeField) {
                        activeField.value = event.target.getAttribute('data-bs-target');
                    }
                });
            });
            if (activeField && activeField.value) {
                var tabTrigger = document.querySelector('#accountTab button[data-bs-target="' + activeField.value + '"]');
                if (tabTrigger && typeof bootstrap !== 'undefined') {
                    var tab = new bootstrap.Tab(tabTrigger);
                    tab.show();
                }
            }

            var passwordTab = document.getElementById('tabPassword');
            var changePasswordButton = document.getElementById('<%= Button_changePassword.ClientID %>');
            var passwordInputs = [
                document.getElementById('<%= TextBox_newPassword.ClientID %>'),
                document.getElementById('<%= TextBox_confirmPassword.ClientID %>')
            ].filter(function (input) { return input; });

            var handlePasswordEnter = function (event) {
                if (event.key === 'Enter' && passwordTab && passwordTab.classList.contains('show') && passwordTab.classList.contains('active')) {
                    event.preventDefault();
                    if (changePasswordButton) {
                        changePasswordButton.click();
                    }
                }
            };

            passwordInputs.forEach(function (input) {
                input.addEventListener('keydown', handlePasswordEnter);
            });
        });
    </script>
</asp:Content>
