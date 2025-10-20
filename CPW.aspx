<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CPW.aspx.vb" Inherits="taifCattle.CPW" %>

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
    <style>
        .back-login-btn {
        border: 2px solid #0f2350;
        color: #0f2350;
        background: transparent;
        border-radius: 12px;
        padding: 15px;
        font-size: 1rem;
        font-weight: 600;
        width: 100%;
        transition: all 0.3s ease;
        }

        .back-login-btn:hover {
            background: #0f2350;
            color: white;
            transform: translateY(-1px);
        }
    </style>
</head>
<body>
    <div class="login-container">
        <div class="login-card">
            <div class="login-header">
                <div class="logo-icon">
                    <%--<i class="fas fa-cow"></i>--%>
                    <img src="_img/logo.png" />
                </div>
                <h1 class="system-title">變更密碼</h1>
                <p class="system-subtitle">Change Password</p>
            </div>            
            <div class="login-body">
                <form id="loginForm" runat="server">

                   
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
                    <button type="button" class="btn back-login-btn backLogin">
                        <i class="fas fa-sign-in-alt me-2"></i>取消
                    </button>
                    <asp:Label ID="Label_resetMsg" runat="server" ForeColor="Tomato"></asp:Label>
                    
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
            $('.password-toggle').on('click', function() {
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

        });
        
        //頁面載入動畫
        $(window).on('load', function() {
            $('.login-card').hide().fadeIn(800);
        });

        $(".backLogin").on("click", function () {
            window.location.href = "Login.aspx";
        });
    </script>
</body>
</html>
