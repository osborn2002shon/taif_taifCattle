<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="taifCattle.Login" %>
<!DOCTYPE html>
<html lang="zh-TW">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>農業保險基金*牛籍管理系統</title>
    <script src="_js/bootstrap.bundle.js"></script>
    <script src="_js/jquery-3.7.1.js"></script>
    <link href="_css/bootstrap.css" rel="stylesheet" />
    <link href="_css/fontawesome-all.css" rel="stylesheet" />
    <link href="_css/login.css" rel="stylesheet" />
</head>
<body>
    <div class="login-container">
        <div class="login-card">
            <div class="login-header">
                <div class="logo-icon">
                    <%--<i class="fas fa-cow"></i>--%>
                    <img src="_img/logo.png" />
                </div>
                <h1 class="system-title">牛籍管理系統</h1>
                <p class="system-subtitle">Cattle Management System</p>
            </div>            
            <div class="login-body">
                <form id="loginForm" runat="server">
                    <div class="form-floating">
                        <asp:TextBox ID="TextBox_ac" runat="server" CssClass="form-control" placeholder="請輸入帳號"></asp:TextBox>
                       <asp:Label ID="Label_ac" runat="server" AssociatedControlID="TextBox_ac">
                            <i class="fas fa-user me-2"></i>帳號
                        </asp:Label>
                    </div>
                    
                    <div class="form-floating position-relative">
                        <asp:TextBox ID="TextBox_pw" runat="server" CssClass="form-control" placeholder="請輸入密碼" TextMode="Password"></asp:TextBox>
                        <asp:Label ID="Label_pw" runat="server" AssociatedControlID="TextBox_pw">
                            <i class="fas fa-lock me-2"></i>密碼
                        </asp:Label>
                        <button type="button" class="btn password-toggle" id="passwordToggle">
                            <i class="fas fa-eye"></i>
                        </button>
                    </div>

                    <div class="form-floating position-relative">
                        <asp:TextBox ID="TextBox_Captcha" runat="server" class="form-control" placeholder="請輸入驗證碼" autocomplete="off" Width="100%" ></asp:TextBox>
                        <asp:Label ID="Label_Captcha" runat="server" AssociatedControlID="TextBox_Captcha">
                            <i class="fa-solid fa-hashtag me-2"></i>驗證碼
                        </asp:Label>
                        <asp:Image ID="Image_Captcha" runat="server" ImageUrl="~/_uc/captcha.ashx" Height="30"/>                    
                    </div> 
                    <asp:Button ID="Button_login" runat="server" CssClass="btn btn-login mb-3" Text="登入" />
                    <asp:Label ID="Label_msg" runat="server" ForeColor="Tomato"></asp:Label>
                    <div class="text-end">
                        <a href="#" class="forgot-password">忘記密碼</a>
                    </div>
                </form>
                <div style="font-size:0.85rem;text-align:center;margin-top:10px">
                    Copyright © 2025 Taiwan Agricultural Insurance Fund<br />
                    (02)2396-2381
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function() {
            //密碼顯示/隱藏切換
            $('#passwordToggle').on('click', function() {
                const passwordInput = $('#<%= TextBox_pw.ClientID%>');
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
            
            //Enter鍵快速登入
            $(document).on('keypress', function(e) {
                if (e.which === 13 && !$('#loginForm').find('button[type="submit"]').prop('disabled')) {
                    $('#loginForm').submit();
                }
            });
        });
        
        //頁面載入動畫
        $(window).on('load', function() {
            $('.login-card').hide().fadeIn(800);
        });
    </script>
</body>
</html>
