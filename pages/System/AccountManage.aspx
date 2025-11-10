<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="AccountManage.aspx.vb" Inherits="taifCattle.AccountManage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">

    <script type="text/javascript">
        function validateAccountForm() {
            var accountInput = document.getElementById('<%= TextBox_account.ClientID %>');
            var nameInput = document.getElementById('<%= TextBox_name.ClientID %>');
            var roleSelect = document.getElementById('<%= DropDownList_editRole.ClientID %>');
            var citySelect = document.getElementById('<%= DropDownList_editCity.ClientID %>');
            var messageLabel = document.getElementById('<%= Label_message.ClientID %>');

            if (!accountInput) {
                return true;
            }

            if (messageLabel) {
                messageLabel.textContent = '';
                messageLabel.className = 'd-block fw-bold mb-3';
            }

            var pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

            var accountValue = accountInput.value.trim();
            if (accountValue.length === 0) {
                if (messageLabel) {
                    messageLabel.textContent = '請輸入登入帳號。';
                    messageLabel.className = 'd-block fw-bold mb-3 star';
                    showModal();
                } else {
                    alert('請輸入登入帳號。');
                }
                return false;
            }

            if (!pattern.test(accountValue)) {
                if (messageLabel) {
                    messageLabel.textContent = '請輸入正確的登入帳號電子信箱格式。';
                    messageLabel.className = 'd-block fw-bold mb-3 star';
                    showModal();
                } else {
                    alert('請輸入正確的登入帳號電子信箱格式。');
                }
                return false;
            }

            if (nameInput && nameInput.value.trim().length === 0) {
                if (messageLabel) {
                    messageLabel.textContent = '請輸入使用者姓名。';
                    messageLabel.className = 'd-block fw-bold mb-3 star';
                    showModal();
                } else {
                    alert('請輸入使用者姓名。');
                }
                return false;
            }

            if (roleSelect && roleSelect.value.trim().length === 0) {
                if (messageLabel) {
                    messageLabel.textContent = '請選擇系統權限。';
                    messageLabel.className = 'd-block fw-bold mb-3 star';
                    showModal();
                } else {
                    alert('請選擇系統權限。');
                }
                return false;
            }

            if (roleSelect && roleSelect.value.trim() === '3') {
                if (!citySelect || citySelect.value.trim().length === 0) {
                    if (messageLabel) {
                        messageLabel.textContent = '請選擇縣市。';
                        messageLabel.className = 'd-block fw-bold mb-3 star';
                        showModal();
                    } else {
                        alert('請選擇縣市。');
                    }
                    return false;
                }
            }

            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-users"></i> 系統帳號管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    系統帳號管理
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">

    <asp:Panel ID="Panel_query" runat="server">
        <div class="queryBox">
            <div class="queryBox-header">
                系統帳號查詢與列表
            </div>
            <div class="queryBox-body">
                <div class="row">
                    <div class="col">
                        <label>系統權限</label>
                        <asp:DropDownList ID="DropDownList_role" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="col">
                        <label>帳號狀態</label>
                        <asp:DropDownList ID="DropDownList_status" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>

                    <div class="col">
                        <label>關鍵字查詢</label>
                        <div class="input-group">
                            <asp:TextBox ID="TextBox_keyword" runat="server" CssClass="form-control" placeholder="請輸入電子信箱或使用者姓名"></asp:TextBox>
                            <span class="input-group-text" onclick="clearControl('<%= TextBox_keyword.ClientID %>')" style="cursor: pointer;"><i class="fa-solid fa-xmark"></i></span>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col text-center">
                        <asp:LinkButton ID="LinkButton_search" runat="server" CssClass="btn btn-primary me-2" CausesValidation="False">
                            <span><i class="fas fa-search me-1"></i>查詢</span>
                        </asp:LinkButton>

                    </div>
                </div>
            </div>
            <div class="queryBox-footer"></div>
        </div>
        <div class="row m-0 mt-3 mb-3 align-items-center">
            <div class="col p-0">
                <asp:LinkButton ID="LinkButton_permission" PostBackUrl="~/pages/System/Permission.aspx" runat="server" CssClass="btn btn-primary" CausesValidation="False">
                    <span><i class="fa-solid fa-wrench me-1"></i>權限設定</span>
                </asp:LinkButton>
                <asp:LinkButton ID="LinkButton_addAccount" runat="server" CssClass="btn btn-success" CausesValidation="False">
                    <span><i class="fas fa-user-plus me-1"></i>新增帳號</span>
                </asp:LinkButton>
                <asp:LinkButton ID="LinkButton_export" runat="server" CssClass="btn btn-outline-success" CausesValidation="False">
                    <i class="fa-solid fa-file-arrow-down me-1"></i>列表下載
                </asp:LinkButton>
            </div>
            <div class="col p-0 text-end">
                共
                <asp:Label ID="Label_recordCount" runat="server" Text="0"></asp:Label>
                筆
            </div>
        </div>
        
        <div class="table-responsive gv-tb">
            <asp:GridView ID="GridView_accounts" runat="server" CssClass="gv" AutoGenerateColumns="False"
                AllowPaging="True" PageSize="10" DataKeyNames="accountID"
                OnPageIndexChanging="GridView_accounts_PageIndexChanging"
                OnRowCommand="GridView_accounts_RowCommand"
                OnRowDataBound="GridView_accounts_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="auTypeName" HeaderText="系統權限" ItemStyle-Width="100px" />
                    <asp:TemplateField HeaderText="帳號狀態" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label ID="Label_status" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="登入帳號（電子信箱）">
                        <ItemTemplate>
                            <span class="account-id"><%# Eval("account") %></span>
                            <asp:HiddenField ID="HiddenField_accountID" runat="server" Value='<%# Eval("accountID") %>' />
                            <asp:HiddenField ID="HiddenField_isActive" runat="server" Value='<%# Eval("isActive") %>' />
                            <asp:HiddenField ID="HiddenField_isVerified" runat="server" Value='<%# Eval("isEmailVerified") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="name" HeaderText="使用者姓名" ItemStyle-Width="120px" />
                    <%--<asp:BoundField  DataField="insertDateTime" HeaderText="建立日期" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-Width="120px" />--%>
                    <asp:TemplateField HeaderText="最後登入時間" ItemStyle-Width="180px">
                        <ItemTemplate>
                            <asp:Label ID="Label_lastLogin" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="操作" ItemStyle-Width="300px">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton_edit" runat="server" CssClass="btn btn-warning btn-sm me-1" CommandName="EditAccount" CommandArgument='<%# Eval("accountID") %>' CausesValidation="False">
                             <i class="fas fa-edit me-1"></i>編輯
                            </asp:LinkButton>
                            <asp:LinkButton ID="LinkButton_resetPassword" runat="server" CssClass="btn btn-outline-warning btn-sm me-1" CausesValidation="False"
                                CommandName="ResetPassword" CommandArgument='<%# Eval("accountID") %>' OnClientClick="return confirm('確定要重設此帳號的密碼並寄送通知信件嗎？');">
                             <i class="fas fa-key me-1"></i>重設密碼
                            </asp:LinkButton>
                            <asp:LinkButton ID="LinkButton_toggleActive" runat="server" CssClass="btn btn-approve btn-sm me-1" CausesValidation="False"
                                CommandName="ToggleActive" CommandArgument='<%# Eval("accountID") %>'>
                             <i class="fas fa-toggle-on me-1"></i><span>啟用</span>
                            </asp:LinkButton>
                            <asp:LinkButton ID="LinkButton_delete" runat="server" CssClass="btn btn-outline-danger btn-sm" CausesValidation="False"
                                CommandName="DeleteAccount" CommandArgument='<%# Eval("accountID") %>' OnClientClick="return confirm('確定要刪除此尚未驗證的帳號嗎？');">
                             <i class="fas fa-trash me-1"></i>刪除
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="star text-center py-2 fw-bold">目前沒有符合條件的帳號資料。</div>
                </EmptyDataTemplate>
                <PagerStyle HorizontalAlign="Center" />
            </asp:GridView>
        </div>
    </asp:Panel>
    <asp:Panel ID="Panel_editor" runat="server" Visible="false">
        <div class="text-start mb-3">
            <asp:LinkButton ID="Button_cancel" runat="server" CssClass="btn btn-outline-secondary"  CausesValidation="False"><i class="fa-solid fa-arrow-left"></i> 返回列表</asp:LinkButton>
        </div>
        <div class="card formCard">
            <div class="card-header">
                帳號資料
                <asp:HiddenField ID="HiddenField_editAccountID" runat="server" />
                
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col">
                        <label>系統權限<span class="star">*</span></label>
                        <asp:DropDownList ID="DropDownList_editRole" runat="server" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="DropDownList_editRole_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <asp:Panel ID="Panel_citySelector" runat="server" CssClass="col" Visible="false">
                        <label>縣市政府<span class="star">*</span></label>
                        <asp:DropDownList ID="DropDownList_editCity" runat="server" CssClass="form-select"></asp:DropDownList>
                    </asp:Panel>
                </div>
                <div class="row">
                    <div class="col">
                        <label>電子信箱<span class="star">*</span></label>
                        <asp:TextBox ID="TextBox_account" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col">
                        <label>使用者姓名<span class="star">*</span></label>
                        <asp:TextBox ID="TextBox_name" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        <label>聯絡電話</label>
                        <asp:TextBox ID="TextBox_mobile" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col">
                        <label>服務單位</label>
                        <asp:TextBox ID="TextBox_unit" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        <label>備註</label>
                        <asp:TextBox ID="TextBox_memo" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        <div class="form-check">
                            <asp:CheckBox ID="CheckBox_isActive" runat="server" />
                            <asp:Label ID="Label_isActive" runat="server" CssClass="form-check-label" AssociatedControlID="CheckBox_isActive" Text="啟用此帳號"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-footer">
                <asp:LinkButton ID="Button_save" runat="server" CssClass="btn btn-success" CommandName="cattleAdd"  OnClientClick="return validateAccountForm();" ><i class="fa-solid fa-floppy-disk me-1"></i>儲存</asp:LinkButton>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
    系統訊息
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
    <asp:Label ID="Label_message" runat="server"></asp:Label>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
