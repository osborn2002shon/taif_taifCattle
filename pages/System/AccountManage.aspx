<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="AccountManage.aspx.vb" Inherits="taifCattle.AccountManage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <style type="text/css">
        /* 狀態標籤 */
        .status-badge {
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 500;
        }

        .status-active {
            background: #d4edda;
            color: #155724;
        }

        .status-inactive {
            background: #f8d7da;
            color: #721c24;
        }

        .status-locked {
            background: #fff3cd;
            color: #856404;
        }

        /* 角色標籤 */
        .role-badge {
            padding: 4px 10px;
            border-radius: 15px;
            font-size: 0.8rem;
            font-weight: 500;
        }

        .role-admin {
            background: #e2e3e5;
            color: #41464b;
        }

        .role-manager {
            background: #cfe2ff;
            color: #084298;
        }

        .role-operator {
            background: #d1ecf1;
            color: #0c5460;
        }

        .role-viewer {
            background: #f8d7da;
            color: #842029;
        }

        /* 帳號ID樣式 */
        .account-id {
            font-family: 'Courier New', monospace;
            font-weight: 600;
            color: #0f2350;
        }

        /* 操作按鈕 */

        .btn-add-account {
            background: linear-gradient(135deg, #28a745, #20c997);
            border: none;
            color: white;
            padding: 10px 20px;
            border-radius: 8px;
            font-size: 0.95rem;
            font-weight: 500;
            transition: all 0.3s ease;
            box-shadow: 0 2px 8px rgba(40,167,69,0.3);
        }

            .btn-add-account:hover {
                background: linear-gradient(135deg, #20c997, #17a2b8);
                transform: translateY(-1px);
                box-shadow: 0 4px 12px rgba(40,167,69,0.4);
                color: white;
            }

        .btn-edit {
            background: linear-gradient(135deg, #0f2350, #1a3b6b);
            border: none;
            color: white;
            padding: 6px 12px;
            border-radius: 6px;
            font-size: 0.8rem;
            font-weight: 500;
            transition: all 0.3s ease;
            /*margin-right: 5px;*/
        }

            .btn-edit:hover {
                background: linear-gradient(135deg, #1a3b6b, #2c5282);
                transform: translateY(-1px);
                box-shadow: 0 3px 8px rgba(15,35,80,0.3);
                color: white;
            }

        .btn-password {
            background: linear-gradient(135deg, #ffc107, #fd7e14);
            border: none;
            color: white;
            padding: 6px 12px;
            border-radius: 6px;
            font-size: 0.8rem;
            font-weight: 500;
            transition: all 0.3s ease;
        }

            .btn-password:hover {
                background: linear-gradient(135deg, #fd7e14, #dc3545);
                transform: translateY(-1px);
                box-shadow: 0 3px 8px rgba(255,193,7,0.3);
                color: white;
            }


        .btn-approve {
            background: linear-gradient(135deg, #28a745, #20c997);
            border: none;
            color: white;
            padding: 6px 12px;
            border-radius: 6px;
            font-size: 0.8rem;
            font-weight: 500;
            transition: all 0.3s ease;
            /*margin-left: 5px;*/
        }

        .btn-approve:hover {
            background: linear-gradient(135deg, #20c997, #17a2b8);
            transform: translateY(-1px);
            box-shadow: 0 3px 8px rgba(40,167,69,0.3);
            color: white;
        }

        .btn-danger{
            background: linear-gradient(135deg, #dc3545, #c82333);
            border: none;
            color: white;
            padding: 6px 12px;
            border-radius: 6px;
            font-size: 0.8rem;
            font-weight: 500;
            transition: all 0.3s ease;
            /*margin-left: 5px;*/
        }
        .btn-danger:hover {
            background: linear-gradient(135deg, #c82333, #bd2130);
            transform: translateY(-1px);
            box-shadow: 0 3px 8px rgba(220,53,69,0.3);
            color: white;
        }

        /* 最後登入時間 */
        .last-login {
            font-size: 0.85rem;
            color: #666;
        }


        .status-pending {
            background: #fff3cd;
            color: #856404;
        }
    </style>
    <script type="text/javascript">
        function validateAccountForm() {
            var accountInput = document.getElementById('<%= TextBox_account.ClientID %>');
            var contactEmailInput = document.getElementById('<%= TextBox_email.ClientID %>');
            var messageLabel = document.getElementById('<%= Label_formMessage.ClientID %>');

            if (!accountInput) {
                return true;
            }

            if (messageLabel) {
                messageLabel.textContent = '';
                messageLabel.className = 'd-block fw-bold mb-3';
            }

            var pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

            var accountValue = accountInput.value.trim();
            if (accountValue.length > 0 && !pattern.test(accountValue)) {
                if (messageLabel) {
                    messageLabel.textContent = '請輸入正確的登入帳號電子信箱格式。';
                    messageLabel.className = 'd-block fw-bold mb-3 text-danger';
                } else {
                    alert('請輸入正確的登入帳號電子信箱格式。');
                }
                return false;
            }

            if (contactEmailInput) {
                var contactEmailValue = contactEmailInput.value.trim();
                if (contactEmailValue.length > 0 && !pattern.test(contactEmailValue)) {
                    if (messageLabel) {
                        messageLabel.textContent = '請輸入正確的聯絡電子信箱格式。';
                        messageLabel.className = 'd-block fw-bold mb-3 text-danger';
                    } else {
                        alert('請輸入正確的聯絡電子信箱格式。');
                    }
                    return false;
                }
            }

            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    系統管理 > 系統帳號管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    系統帳號管理
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="data-table">
        <div class="table-header">
            <div class="table-title-group">
                <h3 class="table-title">系統帳號查詢與列表</h3>
                <p class="table-subtitle">管理所有系統使用者帳號與權限設定</p>
            </div>
            <div class="table-actions">
                <asp:LinkButton ID="LinkButton_addAccount" runat="server" CssClass="btn btn-add-account" CausesValidation="False">
                    <span><i class="fas fa-user-plus me-1"></i>新增帳號</span>
                </asp:LinkButton>
            </div>
        </div>

        <div class="table-body">
            <div class="p-4 border-bottom">
                <div class="row g-3">
                    <div class="col-md-3">
                        <label class="form-label">帳號狀態</label>
                        <asp:DropDownList ID="DropDownList_status" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">系統權限</label>
                        <asp:DropDownList ID="DropDownList_role" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">關鍵字查詢</label>
                        <asp:TextBox ID="TextBox_keyword" runat="server" CssClass="form-control" placeholder="請輸入電子信箱或使用者姓名"></asp:TextBox>
                    </div>
                </div>
                <div class="row mt-3">
                    <div class="col-12">
                        <asp:LinkButton ID="LinkButton_search" runat="server" CssClass="btn btn-primary me-2" CausesValidation="False">
                            <span><i class="fas fa-search me-1"></i>搜尋</span>
                        </asp:LinkButton>
                        <asp:LinkButton ID="LinkButton_reset" runat="server" CssClass="btn btn-outline-secondary me-2" CausesValidation="False">
                            <span><i class="fas fa-redo me-1"></i>重置</span>
                        </asp:LinkButton>
                        <asp:LinkButton ID="LinkButton_export" runat="server" CssClass="btn btn-success" CausesValidation="False">
                            <span><i class="fas fa-download me-1"></i>匯出Excel</span>
                        </asp:LinkButton>
                    </div>
                </div>
            </div>

            <asp:Label ID="Label_message" runat="server" CssClass="d-block fw-bold px-4 pt-3"></asp:Label>

            <div class="table-responsive">
                <div class="text-muted text-end p-2">
                    共 <asp:Label ID="Label_recordCount" runat="server" Text="0"></asp:Label> 筆
                </div>
                <asp:GridView ID="GridView_accounts" runat="server" CssClass="table" AutoGenerateColumns="False"
                    AllowPaging="True" PageSize="10" DataKeyNames="accountID"
                    OnPageIndexChanging="GridView_accounts_PageIndexChanging"
                    OnRowCommand="GridView_accounts_RowCommand"
                    OnRowDataBound="GridView_accounts_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="登入帳號（電子信箱）">
                            <ItemTemplate>
                                <span class="account-id"><%# Eval("account") %></span>
                                <asp:HiddenField ID="HiddenField_accountID" runat="server" Value='<%# Eval("accountID") %>' />
                                <asp:HiddenField ID="HiddenField_isActive" runat="server" Value='<%# Eval("isActive") %>' />
                                <asp:HiddenField ID="HiddenField_isVerified" runat="server" Value='<%# Eval("isEmailVerified") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="name" HeaderText="使用者姓名" ItemStyle-Width="120px" />
                        <asp:BoundField DataField="auTypeName" HeaderText="使用者角色" ItemStyle-Width="80px" />
                        <asp:TemplateField HeaderText="帳號狀態"  ItemStyle-Width="40px">
                            <ItemTemplate>
                                <asp:Label ID="Label_status" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="insertDateTime" HeaderText="建立日期" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-Width="120px" />
                        <asp:TemplateField HeaderText="最後登入" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:Label ID="Label_lastLogin" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="操作" ItemStyle-Width="300px">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton_edit" runat="server" CssClass="btn btn-edit btn-sm me-1" CommandName="EditAccount" CommandArgument='<%# Eval("accountID") %>' CausesValidation="False">
                                    <i class="fas fa-edit me-1"></i>編輯
                                </asp:LinkButton>
                                <asp:LinkButton ID="LinkButton_resetPassword" runat="server" CssClass="btn btn-password btn-sm me-1" CausesValidation="False"
                                    CommandName="ResetPassword" CommandArgument='<%# Eval("accountID") %>' OnClientClick="return confirm('確定要重設此帳號的密碼並寄送通知信件嗎？');">
                                    <i class="fas fa-key me-1"></i>重設密碼
                                </asp:LinkButton>
                                <asp:LinkButton ID="LinkButton_toggleActive" runat="server" CssClass="btn btn-approve btn-sm me-1" CausesValidation="False"
                                    CommandName="ToggleActive" CommandArgument='<%# Eval("accountID") %>'>
                                    <i class="fas fa-toggle-on me-1"></i><span>啟用</span>
                                </asp:LinkButton>
                                <asp:LinkButton ID="LinkButton_delete" runat="server" CssClass="btn btn-danger btn-sm" CausesValidation="False"
                                    CommandName="DeleteAccount" CommandArgument='<%# Eval("accountID") %>' OnClientClick="return confirm('確定要刪除此尚未驗證的帳號嗎？');">
                                    <i class="fas fa-trash me-1"></i>刪除
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="text-danger text-center py-2 fw-bold">目前沒有符合條件的帳號資料。</div>
                    </EmptyDataTemplate>
                    <PagerStyle HorizontalAlign="Center" />
                </asp:GridView>
            </div>
        </div>
    </div>

    <asp:Panel ID="Panel_editor" runat="server" CssClass="mt-4" Visible="false">
        <div class="card shadow-sm">
            <div class="card-header bg-light">
                <h5 class="mb-0" id="formTitle">帳號維護</h5>
            </div>
            <div class="card-body">
                <asp:HiddenField ID="HiddenField_editAccountID" runat="server" />
                <asp:Label ID="Label_formMessage" runat="server" CssClass="d-block fw-bold mb-3"></asp:Label>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label">登入帳號（電子信箱）<span class="text-danger">*</span></label>
                        <asp:TextBox ID="TextBox_account" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator_account" runat="server" ControlToValidate="TextBox_account"
                            Display="Dynamic" CssClass="text-danger" ErrorMessage="請輸入登入帳號" ValidationGroup="AccountForm"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">使用者姓名<span class="text-danger">*</span></label>
                        <asp:TextBox ID="TextBox_name" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator_name" runat="server" ControlToValidate="TextBox_name"
                            Display="Dynamic" CssClass="text-danger" ErrorMessage="請輸入使用者姓名" ValidationGroup="AccountForm"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">系統權限<span class="text-danger">*</span></label>
                        <asp:DropDownList ID="DropDownList_editRole" runat="server" CssClass="form-select"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator_role" runat="server" ControlToValidate="DropDownList_editRole"
                            InitialValue="" Display="Dynamic" CssClass="text-danger" ErrorMessage="請選擇系統權限" ValidationGroup="AccountForm"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">聯絡電話</label>
                        <asp:TextBox ID="TextBox_mobile" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">服務單位</label>
                        <asp:TextBox ID="TextBox_unit" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">電子信箱</label>
                        <asp:TextBox ID="TextBox_email" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-12">
                        <label class="form-label">備註</label>
                        <asp:TextBox ID="TextBox_memo" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>
                    <div class="col-12">
                        <div class="form-check">
                            <asp:CheckBox ID="CheckBox_isActive" runat="server" CssClass="form-check-input" />
                            <asp:Label ID="Label_isActive" runat="server" CssClass="form-check-label" AssociatedControlID="CheckBox_isActive" Text="啟用此帳號"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-footer text-end">
                <asp:Button ID="Button_save" runat="server" CssClass="btn btn-primary me-2" Text="儲存" ValidationGroup="AccountForm" OnClientClick="return validateAccountForm();" />
                <asp:Button ID="Button_cancel" runat="server" CssClass="btn btn-outline-secondary" Text="取消" CausesValidation="False" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>

