<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="Config.aspx.vb" Inherits="taifCattle.Config" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
   <i class="fa-solid fa-sliders"></i> 系統參數設定
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
   系統參數設定
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="card formCard" style="margin-bottom:50px;">
        <div class="card-header">密碼安全政策</div>
        <div class="card-body">
            <div class="form-group">
                <label class="form-label">
                    <i class="fas fa-ruler me-1"></i>密碼最小長度
                </label>
                <div class="input-group">
                    <asp:TextBox ID="TextBox_MinLength" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                    <span class="input-group-text">個字元</span>
                </div>
                <div class="form-text">需設定為8個字元以上，提升密碼安全性</div>
            </div>
            <div class="form-group">
                <label class="form-label">
                    <i class="fas fa-puzzle-piece me-1"></i>密碼複雜性要求
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
        <div class="card-footer"></div>
    </div>
 
    <div class="card formCard" style="margin-bottom:50px;">
        <div class="card-header">密碼變更政策</div>
        <div class="card-body">
            <div class="form-group">
                <label class="form-label">
                    <i class="fas fa-clock me-1"></i>密碼變更週期
                </label>
                <div class="input-group">
                    <asp:TextBox ID="TextBox_MaxAge" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                    <span class="input-group-text">天</span>
                </div>
                <div class="form-text">密碼使用超過此天數後，系統將強制要求使用者更換密碼</div>
            </div>
            <div class="form-group">
                <label class="form-label">
                    <i class="fas fa-history me-1"></i>密碼歷程紀錄
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
                    <i class="fas fa-hourglass-start me-1"></i>密碼最短使用期限
                </label>
                <div class="input-group">
                    <asp:TextBox ID="TextBox_MinAge" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                    <span class="input-group-text">天</span>
                </div>
                <div class="form-text">防止使用者頻繁更換密碼來規避歷程紀錄限制</div>
            </div>
        </div>
        <div class="card-footer"></div>
    </div>
    
    <div class="card formCard" style="margin-bottom:50px;">
        <div class="card-header">帳號鎖定政策</div>
        <div class="card-body">
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="form-label">
                                <i class="fas fa-exclamation-triangle me-1"></i>密碼錯誤次數上限
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
                                <i class="fas fa-stopwatch me-1"></i>帳號鎖定時間
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
        <div class="card-footer"></div>
    </div>

    <div class="card formCard">
        <div class="card-header">報表寄送設定</div>
        <div class="card-body">
            <div class="form-group">
                <label class="form-label">
                    <i class="fas fa-users me-1"></i>自動寄送報表收件人
                </label>
                <asp:TextBox ID="TextBox_ReportRecipients" runat="server" CssClass="form-control" TextMode="SingleLine" Placeholder="example1@domain.com;example2@domain.com"></asp:TextBox>
                <div class="form-text">
                    1. 輸入一個或多個電子信箱，請以半形分號「;」區隔。<br />
                    2. 系統每月定期產出(1)滿一歲未投保乳母牛清單、(2)撲殺補償牛籍編號清單。
                </div>
            </div>
        </div>
        <div class="card-footer"></div>
    </div>

    <div class="text-center">
        <asp:LinkButton ID="Button_Save" runat="server" CssClass="btn btn-primary"><i class="fas fa-save me-1 me-1"></i>儲存設定</asp:LinkButton>
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
