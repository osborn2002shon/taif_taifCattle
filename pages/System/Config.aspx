<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="Config.aspx.vb" Inherits="taifCattle.Config" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <style type="text/css">
        .form-group {
            margin-bottom: 1.5rem;
        }

        h4 {
            margin-bottom: 2rem;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
   系統管理 > 系統參數設定
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
   系統參數設定
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="alert alert-info">
        <h6 class="alert-heading"><i class="fas fa-info-circle me-2"></i>設定說明</h6>
        以下設定將影響所有系統使用者的密碼規則，請謹慎調整各項參數。修改後的設定將立即生效。
    </div>
    <asp:Label ID="Label_Message" runat="server" CssClass="fw-bold d-block mb-3"></asp:Label>
    <!-- 密碼政策設定卡片 -->
    <div class="data-table">
        <div class="table-header">
            <div class="table-title-group">
                <h3 class="table-title">
                    <i class="fas fa-shield-alt me-2"></i>密碼安全政策
                </h3>
                <p class="table-subtitle">設定系統密碼安全規則與限制</p>
            </div>
            <div class="table-actions"></div>
        </div>

        <div class="table-body">
            <div class="p-4 border-bottom">
                <h4 class="section-title">
                    <i class="fas fa-key"></i>基本密碼規則
                </h4>
                
                <!-- 基本密碼規則 -->
                <div class="form-group">
                    <label class="form-label">
                        <i class="fas fa-ruler"></i>密碼最小長度
                    </label>
                    <div class="input-group">
                        <asp:TextBox ID="TextBox_MinLength" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                        <span class="input-group-text">個字元</span>
                    </div>
                    <div class="form-text">需設定為8個字元以上，提升密碼安全性</div>
                </div>
                <div class="form-group">
                    <label class="form-label">
                        <i class="fas fa-puzzle-piece"></i>密碼複雜性要求
                    </label>
                    <div class="form-check">
                        <asp:CheckBox ID="CheckBox_RequireUppercase" runat="server" />
                        <label class="form-check-label" for="<%= CheckBox_RequireUppercase.ClientID %>">
                            必須包含大寫英文字母 (A-Z)
                        </label>
                    </div>
                    <div class="form-check">
                        <asp:CheckBox ID="CheckBox_RequireLowercase" runat="server" />
                        <label class="form-check-label" for="<%= CheckBox_RequireLowercase.ClientID %>">
                            必須包含小寫英文字母 (a-z)
                        </label>
                    </div>
                    <div class="form-check">
                        <asp:CheckBox ID="CheckBox_RequireNumbers" runat="server" />
                        <label class="form-check-label" for="<%= CheckBox_RequireNumbers.ClientID %>">
                            必須包含數字 (0-9)
                        </label>
                    </div>
                    <div class="form-check">
                        <asp:CheckBox ID="CheckBox_RequireSymbols" runat="server" />
                        <label class="form-check-label" for="<%= CheckBox_RequireSymbols.ClientID %>">
                            必須包含特殊符號 (!@#$%^&*等)
                        </label>
                    </div>
                    <div class="form-text">至少需勾選 1 項以上，確保密碼具備足夠複雜性</div>
                </div>
            </div>

            <div class="p-4 border-bottom">
                <h4 class="section-title">
                    <i class="fas fa-calendar-alt"></i>密碼變更政策
                </h4>
               
                <!-- 密碼變更政策 -->
                <div class="form-group">
                    <label class="form-label">
                        <i class="fas fa-clock"></i>密碼變更週期
                    </label>
                    <div class="input-group">
                        <asp:TextBox ID="TextBox_MaxAge" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                        <span class="input-group-text">天</span>
                    </div>
                    <div class="form-text">密碼使用超過此天數後，系統將強制要求使用者更換密碼</div>
                </div>
                <div class="form-group">
                    <label class="form-label">
                        <i class="fas fa-history"></i>密碼歷程紀錄
                    </label>
                    <div class="input-group">
                        <span class="input-group-text">記住前</span>
                        <asp:TextBox ID="TextBox_HistoryCount" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                        <span class="input-group-text">代密碼</span>
                    </div>
                    <div class="form-text">防止使用者重複使用最近用過的密碼</div>
                </div>
                <div class="form-group">
                    <label class="form-label">
                        <i class="fas fa-hourglass-start"></i>密碼最短使用期限
                    </label>
                    <div class="input-group">
                        <asp:TextBox ID="TextBox_MinAge" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                        <span class="input-group-text">天</span>
                    </div>
                    <div class="form-text">防止使用者頻繁更換密碼來規避歷程紀錄限制</div>
                </div>
            </div>


            <div class="p-4 border-bottom">
                <h4 class="section-title">
                    <i class="fas fa-lock"></i>帳號鎖定政策
                </h4>
                
                <!-- 帳號鎖定政策 -->
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="form-label">
                                <i class="fas fa-exclamation-triangle"></i>密碼錯誤次數上限
                            </label>
                            <div class="input-group">
                                <asp:TextBox ID="TextBox_MaxFailAttempts" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                                <span class="input-group-text">次</span>
                            </div>
                            <div class="form-text">連續密碼輸入錯誤超過此次數將鎖定帳號</div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="form-label">
                                <i class="fas fa-stopwatch"></i>帳號鎖定時間
                            </label>
                            <div class="input-group">
                                <asp:TextBox ID="TextBox_LockoutDuration" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                                <span class="input-group-text">分鐘</span>
                            </div>
                            <div class="form-text">帳號被鎖定後的解鎖等待時間</div>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="p-4 border-bottom">
                 <!-- 按鈕群組 -->
                <div class="btn-group-custom">
                    <asp:LinkButton ID="Button_Cancel" runat="server" CssClass="btn btn-cancel " CausesValidation="False">
                        <i class="fas fa-times me-2"></i>取消
                    </asp:LinkButton>

                     <asp:LinkButton ID="Button_Save" runat="server" CssClass="btn btn-success">
                        <i class="fas fa-download me-1"></i>儲存設定
                    </asp:LinkButton>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
