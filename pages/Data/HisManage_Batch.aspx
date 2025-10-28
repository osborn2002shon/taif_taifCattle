<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="HisManage_Batch.aspx.vb" Inherits="taifCattle.HisManage_Batch" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-route"></i> 旅程批次新增
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="queryBox">
        <div class="queryBox-header">
            匯入設定
        </div>
        <div class="queryBox-body">
            <div class="row g-3 align-items-end">
                <div class="col-md-4">
                    <label class="form-label">選擇匯入檔案</label>
                    <asp:FileUpload ID="FileUpload_excel" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-2">
                    <asp:LinkButton ID="LinkButton_import" runat="server" CssClass="btn btn-success w-100">
                        <i class="fa-solid fa-upload"></i> 匯入
                    </asp:LinkButton>
                </div>
                <div class="col-md-3">
                    <label class="form-label">範本下載</label>
                    <asp:HyperLink ID="HyperLink_template" runat="server" CssClass="btn btn-outline-primary w-100" NavigateUrl="~/_doc/batch/hisManage_Batch.xlsx" Target="_blank">
                        <i class="fa-solid fa-download"></i> 下載範本
                    </asp:HyperLink>
                </div>
                <div class="col-md-3 text-end">
                    <asp:LinkButton ID="LinkButton_downloadFailed" runat="server" CssClass="btn btn-outline-danger" Visible="false">
                        <i class="fa-solid fa-file-excel"></i> 下載匯入失敗資料
                    </asp:LinkButton>
                </div>
            </div>
            <asp:Label ID="Label_message" runat="server" CssClass="text-danger fw-bold d-block mt-3"></asp:Label>
            <div class="alert alert-info mt-4" role="alert">
                <h5 class="alert-heading mb-3"><i class="fa-solid fa-circle-info me-2"></i>匯入欄位填寫提示</h5>
                <ul class="mb-0">
                    <li class="mb-2"><strong>牛籍編號 <span class="badge bg-danger ms-1">必填</span></strong>：需為系統既有牛籍，若找不到則會顯示「找不到牛籍編號」。</li>
                    <li class="mb-2"><strong>日期 <span class="badge bg-danger ms-1">必填</span></strong>：須為正確日期格式且不可晚於今日，否則會顯示「日期錯誤」。</li>
                    <li class="mb-2"><strong>類型 <span class="badge bg-danger ms-1">必填</span></strong>：請輸入旅程類型（出生、轉入、勸售等），需為系統中設定且隸屬「旅程」群組的類型，否則會顯示「類型錯誤」。</li>
                    <li class="mb-2"><strong>畜牧場證號 <span class="badge bg-danger ms-1">必填</span></strong>：需為既有牧場證號，若不存在則會顯示「畜牧場錯誤」。</li>
                    <li><strong>旅程備註 <span class="badge bg-secondary ms-1">選填</span></strong>：可留空，未填寫時系統會以空值儲存。</li>
                </ul>
                <h5 class="alert-heading mb-3"><i class="fa-solid fa-circle-info me-2"></i>匯入填寫範例</h5>
                <div style="height:200px; overflow:hidden;">
                    <img src="../../_img/history_batch_sample.png" style="width:100%; " class="img-fluid border" />
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="Panel_success" runat="server" Visible="false" CssClass="queryBox mt-4">
        <div class="queryBox-header">
            匯入成功 <span class="badge bg-success ms-2"><asp:Label ID="Label_successCount" runat="server" Text="0"></asp:Label> 筆</span>
        </div>
        <div class="queryBox-body">
            <div class="table-responsive">
                <asp:GridView ID="GridView_success" runat="server" AutoGenerateColumns="false" CssClass="gv" ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:BoundField DataField="牛籍編號" HeaderText="牛籍編號" />
                        <asp:BoundField DataField="日期" HeaderText="日期" />
                        <asp:BoundField DataField="類型" HeaderText="類型" />
                        <asp:BoundField DataField="畜牧場證號" HeaderText="畜牧場證號" />
                        <asp:BoundField DataField="旅程備註" HeaderText="旅程備註" />
                        <asp:BoundField DataField="匯入結果" HeaderText="匯入結果" />
                    </Columns>
                    <EmptyDataTemplate>
                        尚無成功資料。
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="Panel_failed" runat="server" Visible="false" CssClass="queryBox mt-4">
        <div class="queryBox-header">
            匯入失敗 <span class="badge bg-danger ms-2"><asp:Label ID="Label_failedCount" runat="server" Text="0"></asp:Label> 筆</span>
        </div>
        <div class="queryBox-body">
            <div class="table-responsive">
                <asp:GridView ID="GridView_failed" runat="server" AutoGenerateColumns="false" CssClass="gv" ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:BoundField DataField="牛籍編號" HeaderText="牛籍編號" />
                        <asp:BoundField DataField="日期" HeaderText="日期" />
                        <asp:BoundField DataField="類型" HeaderText="類型" />
                        <asp:BoundField DataField="畜牧場證號" HeaderText="畜牧場證號" />
                        <asp:BoundField DataField="旅程備註" HeaderText="旅程備註" />
                        <asp:BoundField DataField="失敗原因" HeaderText="失敗原因" />
                    </Columns>
                    <EmptyDataTemplate>
                        尚無失敗資料。
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
