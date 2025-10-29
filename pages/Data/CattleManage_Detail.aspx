<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="CattleManage_Detail.aspx.vb" Inherits="taifCattle.CattleManage_Detail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-database"></i> 牛籍資料管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <asp:MultiView ID="MultiView_main" runat="server">
        <%-- 新增 --%>
        <asp:View ID="View_new" runat="server">
            <div class="card formCard">
                <div class="card-header">
                    <i class="fa-regular fa-id-card"></i> 新增牛籍資料
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col">
                            <label>牛籍編號<span class="star">*</span></label>
                            <asp:TextBox ID="TextBox_tagNo" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col">
                            <label>編號備註</label>
                            <asp:TextBox ID="TextBox_tagMemo" runat="server" CssClass="form-control"></asp:TextBox>
                            <span class="info">若此牛有其他編號可以文字備註方式進行補充</span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label>類型規格<span class="star">*</span></label>
                            <asp:DropDownList ID="DropDownList_typeName" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col">
                            <label>出生年度（西元）</label>
                            <div class="input-group">
                                <asp:TextBox ID="TextBox_birthYear" runat="server" CssClass="form-control" TextMode="Number" MaxLength="4" min="2000"></asp:TextBox>
                                    <span class="input-group-text" onclick="clearControl('<%= TextBox_birthYear.ClientID %>')" style="cursor: pointer;"><i class="fa-solid fa-xmark"></i></span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label>牛籍備註</label>
                            <asp:TextBox ID="TextBox_memo" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="card-footer text-center">
                    <div class="msg">
                        <asp:Label ID="Label_addMsg" runat="server"></asp:Label>
                    </div>
                    <asp:LinkButton ID="LinkButton_save" runat="server" CssClass="btn btn-primary"><i class="fa-solid fa-floppy-disk"></i> 新增</asp:LinkButton>
                    <asp:LinkButton ID="LinkButton_cancel" runat="server" CssClass="btn btn-outline-secondary"><i class="fa-solid fa-xmark"></i> 取消</asp:LinkButton>
                </div>
            </div>
        </asp:View>

        <%-- 編輯 --%>
        <asp:View ID="View_edit" runat="server">
            <div class="text-start mb-3">
                <asp:LinkButton ID="LinkButton_backList" runat="server" CssClass="btn btn-outline-secondary"><i class="fa-solid fa-arrow-left"></i> 返回列表</asp:LinkButton>
            </div>
            <div class="row" style="margin-bottom:50px;">
                <div class="col-9">
                    <%-- 基本 --%>
                    <div class="card formCard h-100">
                        <div class="card-header">
                            <i class="fa-regular fa-id-card"></i> 編輯牛籍資料
                        </div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col">
                                    <label>牛籍編號<span class="star">*</span></label>
                                    <asp:TextBox ID="TextBox_edit_tagNo" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    <asp:HiddenField ID="HiddenField_cattleID" runat="server" />
                                </div>
                                <div class="col">
                                    <label>編號備註</label>
                                    <asp:TextBox ID="TextBox_edit_tagMemo" runat="server" CssClass="form-control"></asp:TextBox>
                                    <span class="info">若此牛有其他編號可以文字備註方式進行補充</span>
                                </div>
                                <div class="col">
                                    <label>類型規格<span class="star">*</span></label>
                                    <asp:DropDownList ID="DropDownList_edit_typeName" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row"> 
                                <div class="col">
                                    <label>出生年度（西元）</label>
                                    <div class="input-group">
                                        <asp:TextBox ID="TextBox_edit_birthYear" runat="server" CssClass="form-control" TextMode="Number" MaxLength="4" min="2000"></asp:TextBox>
                                            <span class="input-group-text" onclick="clearControl('<%= TextBox_edit_birthYear.ClientID %>')" style="cursor: pointer;"><i class="fa-solid fa-xmark"></i></span>
                                    </div>
                                </div>
                                <div class="col">
                                    <label>牛籍歲齡（自動）</label>
                                    <asp:TextBox ID="TextBox_edit_cattleAge" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>
                                <div class="col">
                                    <label>平均產乳量（自動）</label>
                                    <asp:TextBox ID="TextBox_edit_milkProduction" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col">
                                    <label>牛籍備註</label>
                                    <asp:TextBox ID="TextBox_edit_memo" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="card-footer text-center">
                            <div class="msg">
                                <asp:Label ID="Label_edit_msg" runat="server"></asp:Label>
                            </div>
                            <asp:LinkButton ID="LinkButton_edit_update" runat="server" CssClass="btn btn-primary"><i class="fa-solid fa-floppy-disk"></i> 更新</asp:LinkButton>
                        </div>
                    </div>
                </div>
                <div class="col-3">
                    <%-- 保險 --%>
                    <div class="card formCard h-100">
                        <div class="card-header red">
                            <i class="fa-regular fa-money-bill-1"></i> 最新保險狀態
                        </div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-12">
                                    <label>投保狀態</label>
                                    <div>
                                        <asp:TextBox ID="TextBox_insStatus_ins" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-12 mt-3">
                                    <label>理賠狀態</label>
                                    <div>
                                        <asp:TextBox ID="TextBox_insStatus_claim" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <span class="info">保險狀態由家畜保險系統提供</span>
                        </div>
                        <div class="card-footer"></div>
                    </div>
                </div>
            </div>

            <%-- 旅程 --%>
            <div class="card formCard">
                <div class="card-header green">
                    <i class="fa-regular fa-truck"></i> 新增旅程紀錄
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col">
                            <label>日期<span class="star">*</span></label>
                            <asp:TextBox ID="TextBox_edit_hisDef_date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col">
                            <label>類型<span class="star">*</span></label>
                            <asp:DropDownList ID="DropDownList_edit_hisDef_hisType" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col">
                            <label>證號（牧場登記證、飼養登記證、身分證）<span class="star">*</span></label>
                            <asp:TextBox ID="TextBox_edit_hisDef_farmCode" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                            <span class="info">若新增時查無畜牧場，請至牧場資料管理建立畜牧場。</span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label>旅程備註</label>
                            <asp:TextBox ID="TextBox_edit_hisDef_memo" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="card-footer text-center">
                    <div class="msg">
                        <asp:Label ID="Label_edit_msg_hisDef" runat="server"></asp:Label>
                    </div>
                    <asp:LinkButton ID="LinkButton_edit_insert_hisDef" runat="server" CssClass="btn btn-primary"><i class="fa-solid fa-floppy-disk"></i> 新增</asp:LinkButton>
                </div>
            </div>

            <div class="container-fluid gv-tb" style="margin-bottom:50px;">
                <div class="table-responsive">
                    <asp:GridView ID="GridView_his_def" runat="server" CssClass="gv" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true">
                        <Columns>
                            <asp:TemplateField HeaderText="類型" ItemStyle-Width="120px">
                                <ItemTemplate>
                                    <%# Eval("typeName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="日期" ItemStyle-Width="120px">
                                <ItemTemplate>
                                    <%#CDate(Eval("dataDate")).ToString("yyyy/MM/dd") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="縣市" ItemStyle-Width="80px">
                                <ItemTemplate>
                                    <%# Eval("city") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="鄉鎮" ItemStyle-Width="80px">
                                <ItemTemplate>
                                    <%# Eval("area") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="證號" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <%# Eval("placeCode") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="名稱" ItemStyle-Width="180px">
                                <ItemTemplate>
                                    <%# Eval("placeName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="負責人" ItemStyle-Width="120px">
                                <ItemTemplate>
                                    <%# Eval("placeOwner") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="資料來源" ItemStyle-Width="130px">
                                <ItemTemplate>
                                    <%# Eval("insertType") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="備註">
                                <ItemTemplate>
                                    <%# Eval("memo") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="操作" ItemStyle-Width="120px">
                                <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton_edit" runat="server" CssClass="btn btn-sm btn-danger"
                                            CommandName="hisRemove" CommandArgument='<%# Eval("hisID") %>' >
                                            <i class="fa-solid fa-trash"></i> 刪除</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            無資料。
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>

            <%-- 除籍 --%>
            <div class="card formCard">
                <div class="card-header orange">
                    <i class="fa-regular fa-flag"></i> 新增除籍紀錄
                </div>
                <div class="card-body">
                    <asp:UpdatePanel runat="server" class="row">
                        <ContentTemplate>
                            <div class="col">
                                <label>日期<span class="star">*</span></label>
                                <asp:TextBox ID="TextBox_edit_hisEnd_date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col">
                                <label>類型<span class="star">*</span></label>
                                <asp:DropDownList ID="DropDownList_edit_hisEnd_hisType" runat="server" CssClass="form-select" AutoPostBack="true"></asp:DropDownList>
                            </div>
                            <div class="col">
                                <label>除籍場域<span class="star">*</span></label>
                                <asp:DropDownList ID="DropDownList_edit_hisEnd_place" runat="server" CssClass="form-select"></asp:DropDownList>
                                <span class="info">僅屠宰、化製需選擇場所，若清單無所需項目請洽系統管理者。</span>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="row">
                        <div class="col">
                            <label>除籍備註</label>
                            <asp:TextBox ID="TextBox_edit_hisEnd_memo" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="card-footer text-center">
                    <div class="msg">
                        <asp:Label ID="Label_edit_msg_hisEnd" runat="server"></asp:Label>
                    </div>
                    <asp:LinkButton ID="LinkButton_edit_insert_hisEnd" runat="server" CssClass="btn btn-primary"><i class="fa-solid fa-floppy-disk"></i> 新增</asp:LinkButton>
                </div>
            </div>

            <div class="container-fluid gv-tb">
                <div class="table-responsive">
                    <asp:GridView ID="GridView_his_end" runat="server" CssClass="gv" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true">
                        <Columns>
                            <asp:TemplateField HeaderText="類型" ItemStyle-Width="120px">
                                <ItemTemplate>
                                    <%# Eval("typeName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="日期" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <%#CDate(Eval("dataDate")).ToString("yyyy/MM/dd") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="縣市" ItemStyle-Width="80px">
                                <ItemTemplate>
                                    <%# Eval("city") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="鄉鎮" ItemStyle-Width="80px">
                                <ItemTemplate>
                                    <%# Eval("area") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="名稱" ItemStyle-Width="180px">
                                <ItemTemplate>
                                    <%# Eval("placeName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="負責人" ItemStyle-Width="120px">
                                <ItemTemplate>
                                    <%# Eval("placeOwner") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="資料來源" ItemStyle-Width="130px">
                                <ItemTemplate>
                                    <%# Eval("insertType") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="備註">
                                <ItemTemplate>
                                    <%# Eval("memo") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="操作" ItemStyle-Width="120px">
                                <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton_edit" runat="server" CssClass="btn btn-sm btn-danger"
                                            CommandName="hisRemove" CommandArgument='<%# Eval("hisID") %>' >
                                            <i class="fa-solid fa-trash"></i> 刪除</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            無資料。
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>

        </asp:View>
    </asp:MultiView>
    <script type="text/javascript">
        function clearControl(controlId) {
            var textbox = document.getElementById(controlId);
            textbox.value = '';
            textbox.focus(); // 清除後自動聚焦
        }
    </script>
</asp:Content>
