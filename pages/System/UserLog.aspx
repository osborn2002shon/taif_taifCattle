<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="UserLog.aspx.vb" Inherits="taifCattle.UserLog" %>

<%@ Register Src="~/_uc/uc_jqDatePicker.ascx" TagPrefix="uc1" TagName="uc_jqDatePicker" %>




<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    系統管理 > 使用者操作紀錄
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    使用者操作紀錄
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="data-table">
        <div class="table-header">
            <div class="table-title-group">
                <h3 class="table-title">操作紀錄查詢</h3>
                <p class="table-subtitle">系統使用者操作歷程記錄</p>
            </div>
            <div class="table-actions">
                <%--<button class="btn btn-add-cattle" onclick="addNewCattle()">
                        <i class="fas fa-plus me-1"></i>新增牛籍
                    </button>--%>
            </div>

        </div>

        <div class="table-body">
            <!-- 篩選器區域 -->
            <div class="p-4 border-bottom">
                <div class="row g-3">
                     <div class="col-md-3">
                         <label class="form-label">操作時間(起)</label>
                         <uc1:uc_jqDatePicker runat="server" ID="uc_jqDatePicker_dateBeg" CssClass="form-control"/>
                     </div>
                     <div class="col-md-3">
                         <label class="form-label">操作日期(訖)</label>
                         <uc1:uc_jqDatePicker runat="server" ID="uc_jqDatePicker_dateEnd" CssClass="form-control"/>
                     </div>
                    <div class="col-md-3">
                        <label class="form-label">操作項目</label>
                        <asp:DropDownList ID="DropDownList_userLogItem" runat="server" class="form-select"></asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">操作類型</label>
                        <asp:DropDownList ID="DropDownList_userLogType" runat="server" class="form-select"></asp:DropDownList>
                        
                    </div>
                     <div class="col-md-3">
                        <label class="form-label">關鍵字</label>
                         <asp:TextBox ID="TextBox_keyWord" runat="server" CssClass="form-control" placeHolder="請輸入姓名、帳號、IP"></asp:TextBox>
                      </div>
                </div>
                <div class="row mt-3">
                    <div class="col-12">
                       <asp:LinkButton ID="LinkButton_query" runat="server" CssClass="btn btn-primary me-2">
                            <i class="fas fa-search me-1"></i>搜尋
                        </asp:LinkButton>
                       <%-- <button class="btn btn-outline-secondary me-2">
                            <i class="fas fa-redo me-1"></i>重置
                        </button>--%>
                     <asp:LinkButton ID="LinkButton_excel" runat="server" CssClass="btn btn-success">
                        <i class="fas fa-download me-1"></i>匯出Excel
                    </asp:LinkButton>
                    </div>
                </div>
            </div>

            <!-- 資料表格 -->
            <div class="table-responsive">
               <div class="text-muted text-end p-2">
                    共 <asp:Label ID="Label_recordCount" runat="server" Text="0"></asp:Label> 筆
                </div>
                <asp:GridView ID="GridView_userLog" runat="server" CssClass="table" AutoGenerateColumns="false" 
                    AllowPaging="true" PageSize="10" ShowHeaderWhenEmpty="true" HeaderStyle-CssClass="text-center">
                    <Columns>
                        <asp:BoundField DataField="logDateTime" HeaderText="操作時間" 
                            DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-Width="15%" ItemStyle-CssClass="text-center"/>

                        <asp:BoundField DataField="IP" HeaderText="IP位址" ItemStyle-Width="10%" ItemStyle-CssClass="text-center"/>

                        <asp:BoundField DataField="name" HeaderText="使用者姓名" ItemStyle-Width="10%" ItemStyle-CssClass="text-center"/>

                        <asp:BoundField DataField="account" HeaderText="帳號名稱" ItemStyle-Width="15%" ItemStyle-CssClass="text-center" />

                        <asp:BoundField DataField="logType" HeaderText="操作類型" ItemStyle-Width="10%" ItemStyle-CssClass="text-center"/>

                        <asp:BoundField DataField="logItem" HeaderText="操作項目" ItemStyle-Width="15%" ItemStyle-CssClass="text-center"/>

                        <asp:BoundField DataField="memo" HeaderText="操作內容"
                            HtmlEncode="False" ItemStyle-Width="25%" ItemStyle-Wrap="True" />
                      
                    </Columns>
                     <EmptyDataTemplate>
                       <div class="text-danger text-center py-2 fw-bold">
                            目前沒有任何操作紀錄。
                        </div>
                    </EmptyDataTemplate>
                    <PagerStyle HorizontalAlign="Center"/>
                </asp:GridView>
            </div>

            <%--<!-- 分頁 -->
            <div class="d-flex justify-content-between align-items-center p-3">
                <div class="text-muted">
                    顯示第 1 到 8 筆，共 156 筆資料
                </div>
                <nav>
                    <ul class="pagination mb-0">
                        <li class="page-item disabled">
                            <span class="page-link">上一頁</span>
                        </li>
                        <li class="page-item active">
                            <span class="page-link">1</span>
                        </li>
                        <li class="page-item">
                            <a class="page-link" href="#">2</a>
                        </li>
                        <li class="page-item">
                            <a class="page-link" href="#">3</a>
                        </li>
                        <li class="page-item">
                            <a class="page-link" href="#">下一頁</a>
                        </li>
                    </ul>
                </nav>
            </div>--%>
        </div>
    </div>
</asp:Content>
