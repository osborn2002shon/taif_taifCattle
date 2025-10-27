<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="CattleManage_Batch.aspx.vb" Inherits="taifCattle.CattleManage_Batch" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-file-import"></i> 牛籍批次匯入
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
                    <asp:HyperLink ID="HyperLink_template" runat="server" CssClass="btn btn-outline-primary w-100" NavigateUrl="~/_doc/batch/cattleManage_Batch.xlsx" Target="_blank">
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
</asp:Content>
