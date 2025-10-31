<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="HisEndManage_Batch.aspx.vb" Inherits="taifCattle.HisEndManage_Batch" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-skull"></i> 除籍批次設定
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
                <div class="col-md-6">
                    <label class="form-label">範本下載</label>
                    <asp:HyperLink ID="HyperLink_template" runat="server" CssClass="btn btn-outline-primary w-100" NavigateUrl="~/_doc/batch/hisEndManage_Batch.xlsx" Target="_blank">
                        <i class="fa-solid fa-download"></i> 下載範本
                    </asp:HyperLink>
                </div>
                
            </div>
            <asp:Label ID="Label_message" runat="server" CssClass="text-danger fw-bold d-block mt-3"></asp:Label>
        </div>
    </div>

    <div class="alert alert-info mt-4" role="alert">
        <h5 class="alert-heading mb-3"><i class="fa-solid fa-circle-info me-2"></i>匯入欄位填寫提示</h5>
        <ul class="mb-0">
            <li class="mb-2"><strong><span class="badge bg-danger me-1">必填</span>牛籍編號</strong>：若系統無對應牛籍，會以預設值自動建立牛籍資料。</li>
            <li class="mb-2"><strong><span class="badge bg-danger me-1">必填</span>除籍日期</strong>：須為正確日期格式且不可晚於今日，否則會顯示「日期錯誤」。</li>
            <li class="mb-2"><strong><span class="badge bg-danger me-1">必填</span>類型（其他／未使用）</strong>：僅接受「其他」或「未使用」，若不符合則會顯示「類型錯誤」。</li>
            <li class="mb-2"><strong><span class="badge bg-danger me-1">必填</span>畜牧場證號（牧場登記證、飼養登記證、身分證）</strong>：需為既有牧場證號，若不存在則會顯示「畜牧場錯誤」。</li>
            <li><strong><span class="badge bg-secondary me-1">選填</span>除籍備註</strong>：可留空，未填寫時系統會以空值儲存。</li>
        </ul>
            <h5 class="alert-heading mb-3"><i class="fa-solid fa-circle-info mt-5"></i>匯入填寫範例</h5>
            <div style="height:200px; overflow:hidden;">
                <img src="../../_img/history_end_batch_sample.png" style="width:100%; " class="img-fluid border" />
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
                        <asp:BoundField DataField="除籍日期" HeaderText="除籍日期" />
                        <asp:BoundField DataField="類型" HeaderText="類型（其他／未使用）" />
                        <asp:BoundField DataField="畜牧場證號" HeaderText="畜牧場證號（牧場登記證、飼養登記證、身分證）" />
                        <asp:BoundField DataField="除籍備註" HeaderText="除籍備註" />
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
                        <asp:BoundField DataField="除籍日期" HeaderText="除籍日期" />
                        <asp:BoundField DataField="類型" HeaderText="類型（其他／未使用）" />
                        <asp:BoundField DataField="畜牧場證號" HeaderText="畜牧場證號（牧場登記證、飼養登記證、身分證）" />
                        <asp:BoundField DataField="除籍備註" HeaderText="除籍備註" />
                        <asp:BoundField DataField="失敗原因" HeaderText="失敗原因" />
                    </Columns>
                    <EmptyDataTemplate>
                        尚無失敗資料。
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </asp:Panel>
    <div class="row">
        <div class="col text-end d-flex justify-content-end gap-2">
        <asp:LinkButton ID="LinkButton_downloadFailed" runat="server" CssClass="btn btn-outline-danger" Visible="false">
            <i class="fa-solid fa-file-excel"></i> 下載匯入失敗資料
        </asp:LinkButton>
        <asp:HyperLink ID="HyperLink_missingFarmBatch" runat="server" CssClass="btn btn-warning" Visible="false">
            <i class="fa-solid fa-tractor"></i> 牧場批次新增
        </asp:HyperLink>
    </div>
    </div>
</asp:Content>
