<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_public.master" CodeBehind="CPW.aspx.vb" Inherits="taifCattle.CPW" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    變更密碼
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_body" runat="server">
    <asp:Label ID="Label_title" runat="server" Text="" CssClass="text-danger text-center"></asp:Label>
    <!-- 新密碼 -->
    <div class="form-floating position-relative mb-3">
        <asp:TextBox ID="TextBox_pw" runat="server" CssClass="form-control" placeholder="請輸入新密碼"
            TextMode="Password"></asp:TextBox>
        <asp:Label ID="Label_pw" runat="server" AssociatedControlID="TextBox_pw">
            <i class="fas fa-lock me-2"></i>新密碼
        </asp:Label>
        <button type="button" class="btn password-toggle" id="passwordToggle1" title="顯示/隱藏密碼">
            <i class="fas fa-eye"></i>
        </button>
    </div>

    <!-- 確認新密碼 -->
    <div class="form-floating position-relative mb-3">
        <asp:TextBox ID="TextBox_pwConfirm" runat="server" CssClass="form-control" placeholder="請再次輸入新密碼"
            TextMode="Password"></asp:TextBox>
        <asp:Label ID="Label_pwConfirm" runat="server" AssociatedControlID="TextBox_pwConfirm">
            <i class="fas fa-key me-2"></i>確認新密碼
        </asp:Label>
        <button type="button" class="btn password-toggle" id="passwordToggle2" title="顯示/隱藏密碼">
            <i class="fas fa-eye"></i>
        </button>
    </div>

    <asp:Button ID="Button_change" runat="server" CssClass="btn btn-login mb-3" Text="確認變更" />
    <asp:Button ID="Button_back" runat="server" CssClass="btn btn-back" Text="返回登入頁面" />
    <asp:Label ID="Label_resetMsg" runat="server" ForeColor="Tomato"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_jsBottom" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            //密碼顯示/隱藏切換
            $('.password-toggle').on('click', function () {
                const container = $(this).closest('.form-floating');
                const passwordInput = container.find('input[type="password"], input[type="text"]');
                const toggleIcon = $(this).find('i');

                if (passwordInput.attr('type') === 'password') {
                    passwordInput.attr('type', 'text');
                    toggleIcon.removeClass('fa-eye').addClass('fa-eye-slash');
                } else {
                    passwordInput.attr('type', 'password');
                    toggleIcon.removeClass('fa-eye-slash').addClass('fa-eye');
                }
            });
            
            //輸入框焦點效果
            $('.form-control').on('focus', function() {
                $(this).parent().addClass('focused');
            }).on('blur', function() {
                if (!$(this).val()) {
                    $(this).parent().removeClass('focused');
                }
            });

        });
        
        //頁面載入動畫
        $(window).on('load', function() {
            $('.login-card').hide().fadeIn(800);
        });

        $(".backLogin").on("click", function () {
            window.location.href = "Login.aspx";
        });
    </script>
</asp:Content>
