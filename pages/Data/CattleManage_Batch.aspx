<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="CattleManage_Batch.aspx.vb" Inherits="taifCattle.CattleManage_Batch" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-cow"></i> 牛籍批次新增
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="queryBox">
        <div class="queryBox-header">
            牛籍批次新增
        </div>
        <div class="queryBox-body">
            <div class="row align-items-end">
                <div class="col-3">
                    <label>範本下載</label>
                    <asp:HyperLink ID="HyperLink_template" runat="server" CssClass="btn btn-outline-primary w-100" NavigateUrl="~/_doc/batch/cattleManage_Batch.xlsx" Target="_blank">
                        <i class="fa-solid fa-download me-1"></i>下載範本
                    </asp:HyperLink>
                </div>
                <div class="col-6">
                    <label>選擇匯入檔案並上傳</label>
                    <asp:FileUpload ID="FileUpload_excel" runat="server" CssClass="form-control" />
                </div>
                <div class="col-3">
                    <asp:LinkButton ID="LinkButton_import" runat="server" CssClass="btn btn-success w-100">
                        <i class="fa-solid fa-upload me-1"></i>上傳檔案
                    </asp:LinkButton>
                </div>
            </div>
            
        </div>
        <div class="queryBox-footer"></div>
    </div>

    <div class="alert alert-info mt-4" role="alert">
        <h5 class="alert-heading mb-3"><i class="fa-solid fa-circle-info me-2"></i>匯入欄位填寫提示</h5>
        <p class="mb-2">請依下列規則準備 Excel 資料，避免常見的匯入失敗情況：</p>
        <ul class="mb-0">
            <li class="mb-2">
                <strong><span class="badge bg-danger me-1">必填</span>牛籍編號</strong>：每頭牛必須提供唯一的牛籍編號，如與系統既有編號重複，該筆資料會匯入失敗並顯示「已有重複牛籍編號」。
            </li>
            <li class="mb-2">
                <strong><span class="badge bg-secondary me-1">選填</span>牛籍編號備註</strong>：可留空，未填寫時系統會以空值儲存。
            </li>
            <li class="mb-2">
                <strong><span class="badge bg-danger me-1">必填</span>品項</strong>：請輸入系統既有的品項名稱<code class="text-nowrap">（乳公牛、乳母牛、黃雜牛、水牛、安格斯牛、其他、無資料）</code> ，若品項名稱不存在，將判定為「品項錯誤」。
            </li>
            <li class="mb-2">
                <strong><span class="badge bg-secondary me-1">選填</span>出生年度</strong>：可輸入西元或民國年份。若填寫民國年（如 109），系統會自動加上 1911 轉換為 2020。有效範圍為 1911 年至當年度，超出範圍或格式錯誤會顯示「出生年度錯誤」。
            </li>
            <li class="mb-2">
                <strong><span class="badge bg-secondary me-1">選填</span>牛籍備註</strong>：可留空，未填寫時系統會以空值儲存。
            </li>
            <li class="mb-2">
                <strong><span class="badge bg-warning me-1">次要</span>類型</strong>：當需要記錄旅程資訊時，請輸入旅程的類型名稱 <code class="text-nowrap">（出生、停留、勸售）</code> ，才能新增對應的牛籍旅程紀錄。輸入不存在的類型將顯示「類型錯誤」。
            </li>
            <li class="mb-2">
                <strong><span class="badge bg-warning me-1">次要</span>日期</strong>：需為有效日期格式且不可晚於今日，否則匯入會失敗並顯示「日期錯誤」。未填寫時不會新增旅程紀錄。
            </li>
            <li>
                <strong><span class="badge bg-warning me-1">次要</span>畜牧場證號</strong>：請輸入有效的畜牧場證號。若與現有證號不符，將顯示「畜牧場錯誤」。未填寫時不會新增旅程紀錄。
            </li>
        </ul>
        <div class="small text-muted mt-3">
            ※ 需同時填寫「類型」、「日期」、「畜牧場證號」才會建立旅程紀錄，任一欄位未填寫時將不會新增旅程資料。
        </div>
        <h5 class="alert-heading mt-5"><i class="fa-solid fa-circle-info me-2"></i>匯入填寫範例</h5>
        <div style="height:250px; overflow:hidden;">
            <img src="../../_img/cattle_batch_sample.png" style="width:100%;" />
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
                        <asp:BoundField DataField="牛籍編號備註" HeaderText="牛籍編號備註" />
                        <asp:BoundField DataField="品項" HeaderText="品項" />
                        <asp:BoundField DataField="出生年度" HeaderText="出生年度" />
                        <asp:BoundField DataField="牛籍備註" HeaderText="牛籍備註" />
                        <asp:BoundField DataField="類型" HeaderText="類型" />
                        <asp:BoundField DataField="日期" HeaderText="日期" />
                        <asp:BoundField DataField="畜牧場證號" HeaderText="畜牧場證號" />
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
                        <asp:BoundField DataField="牛籍編號備註" HeaderText="牛籍編號備註" />
                        <asp:BoundField DataField="品項" HeaderText="品項" />
                        <asp:BoundField DataField="出生年度" HeaderText="出生年度" />
                        <asp:BoundField DataField="牛籍備註" HeaderText="牛籍備註" />
                        <asp:BoundField DataField="類型" HeaderText="類型" />
                        <asp:BoundField DataField="日期" HeaderText="日期" />
                        <asp:BoundField DataField="畜牧場證號" HeaderText="畜牧場證號" />
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
         
        <div class="col text-end d-flex justify-content-end">
            <asp:LinkButton ID="LinkButton_downloadFailed" runat="server" CssClass="btn btn-outline-danger" Visible="false">
                <i class="fa-solid fa-file-excel"></i> 下載匯入失敗資料
            </asp:LinkButton>
            <asp:HyperLink ID="HyperLink_missingFarmBatch" runat="server" CssClass="btn btn-warning" Visible="false">
                <i class="fa-solid fa-tractor"></i> 牧場批次新增
            </asp:HyperLink>
        </div>
    </div>

    <script type="text/javascript">
        (function () {
            function initTooltips() {
                var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
                tooltipTriggerList.forEach(function (tooltipTriggerEl) {
                    if (!bootstrap.Tooltip.getInstance(tooltipTriggerEl)) {
                        new bootstrap.Tooltip(tooltipTriggerEl);
                    }
                });
            }

            if (window.Sys && Sys.Application) {
                Sys.Application.add_load(initTooltips);
            } else {
                document.addEventListener('DOMContentLoaded', initTooltips);
            }
        })();
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
    系統訊息
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
     <asp:Label ID="Label_message" runat="server" CssClass="msg"></asp:Label>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>