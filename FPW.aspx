<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_public.master" CodeBehind="FPW.aspx.vb" Inherits="taifCattle.FPW" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    忘記密碼
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_body" runat="server">
    <asp:MultiView ID="MultiView_main" runat="server">
        <%-- 輸入信箱 --%>
        <asp:View ID="View_verification" runat="server">
            <div class="form-floating">
                <asp:TextBox ID="TextBox_email" runat="server" CssClass="form-control" placeholder="請輸入電子信箱"></asp:TextBox>
                <asp:Label ID="Label_email" runat="server" AssociatedControlID="TextBox_email">
                    <i class="fas fa-envelope me-2"></i>電子信箱
                </asp:Label>
            </div>
            <asp:Button ID="Button_email" runat="server" CssClass="btn btn-login mb-3" Text="確認" />
            <asp:Button ID="Button_back" runat="server" CssClass="btn btn-back" Text="返回登入" />
            <asp:Label ID="Label_msg" runat="server" ForeColor="Tomato"></asp:Label>
        </asp:View>

        <%-- 重設密碼 --%>
        <asp:View ID="View_reset" runat="server">
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
            <asp:Button ID="Button_back2" runat="server" CssClass="btn btn-back" Text="取消變更" />
            <asp:Label ID="Label_resetMsg" runat="server" ForeColor="Tomato"></asp:Label>
        </asp:View>

        <%-- 驗證碼失效 --%>
        <asp:View ID="View_err" runat="server">
            <span style="color:tomato">
                您的驗證碼已失效、已使用或不正確，請重新申請密碼變更。
            </span>
            <asp:Button ID="Button_back3" runat="server" CssClass="btn btn-back" Text="返回登入" />
        </asp:View>

        <%-- 同一組信箱發現有對應多個帳號時，輸入要變更的帳號（循正常管道建立帳號不會出現） --%>
        <asp:View ID="View_account" runat="server">
            <div class="form-floating">
                <asp:TextBox ID="TextBox_account" runat="server" CssClass="form-control" placeholder="請輸入帳號"></asp:TextBox>
                <asp:Label ID="Label_account" runat="server" AssociatedControlID="TextBox_account">
                    <i class="fas fa-user me-2"></i>帳號
                </asp:Label>
            </div>
            <asp:Button ID="Button_account" runat="server" CssClass="btn btn-login mb-3" Text="確認" />
            <asp:Button ID="Button_back4" runat="server" CssClass="btn btn-back" Text="取消變更" />
            <asp:Label ID="Label_msg_account" runat="server" ForeColor="Tomato"></asp:Label>
        </asp:View>
    </asp:MultiView>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_jsBottom" runat="server">
</asp:Content>
