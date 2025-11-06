<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="StaticsCattle.aspx.vb" Inherits="taifCattle.StaticsCattle" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-magnifying-glass"></i> 指定牛籍歷程查詢
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="queryBox">
        <div class="queryBox-header">
             查詢條件
        </div>
        <div class="queryBox-body">
            <div class="row">
                <div class="col">
                    <label>牛籍編號</label>
                    <div class="input-group">
                        <asp:TextBox ID="TextBox_tagNo" runat="server" CssClass="form-control" placeHolder="請輸入牛籍編號（例如：13K0001 或 13-12345）"></asp:TextBox>
                        <span class="input-group-text" onclick="clearControl('<%= TextBox_tagNo.ClientID %>')" style="cursor: pointer;"><i class="fa-solid fa-xmark"></i></span>
                    </div>
                </div>
             
            </div>
            <div class="row">
                <div class="col text-center">
                    <asp:LinkButton ID="LinkButton_query" runat="server" CssClass="btn btn-primary">
                        <i class="fas fa-search me-1"></i>查詢
                    </asp:LinkButton>
                </div>
              <%--  <div class="col-12 text-center">
                    <asp:Label ID="Label_msg" runat="server" Text="" CssClass="text-danger"></asp:Label>
                </div>--%>
            </div>
            
        </div>
        <div class="queryBox-footer"></div>
    </div>
    <asp:Panel ID="Panel_result" runat="server" Visible="false">
        <div class="card formCard h-100">
            <div class="card-header green">
                牛籍基本資料
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col">
                        <label>牛籍編號</label>
                        <asp:TextBox ID="TextBox_info_tagNo" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                    </div>
                    <div class="col">
                        <label>編號備註</label>
                        <asp:TextBox ID="TextBox_info_tagMemo" runat="server" CssClass="form-control"  Enabled="false"></asp:TextBox>
                    </div>
                    <div class="col">
                        <label>類型規格</label>
                        <asp:TextBox ID="TextBox_info_typeName" runat="server" CssClass="form-control"  Enabled="false"></asp:TextBox>
                    </div>
                </div>
                <div class="row"> 
                    <div class="col">
                        <label>出生年度（西元）</label>
                        <asp:TextBox ID="TextBox_info_birthYear" runat="server" CssClass="form-control"  Enabled="false"></asp:TextBox>
                    </div>
                    <div class="col">
                        <label>牛籍歲齡</label>
                        <asp:TextBox ID="TextBox_info_cattleAge" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                    </div>
                    <div class="col">
                        <label>平均產乳量</label>
                        <asp:TextBox ID="TextBox_info_milkProduction" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        <label>牛籍備註</label>
                        <asp:TextBox ID="TextBox_info_memo" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                    </div>
                </div>
            </div>
           <%-- <div class="card-footer text-center">
            </div>--%>
        </div>
        <div class="row m-0 mt-3 mb-3 align-items-center">
            <div class="col p-0">
    
                <asp:LinkButton ID="LinkButton_excel" runat="server" CssClass="btn btn-success"><i class="fa-solid fa-file-arrow-down me-1"></i>列表下載</asp:LinkButton>
            </div>
            <div class="col p-0 text-end">
                共 <asp:Label ID="Label_recordCount" runat="server" Text="0"></asp:Label> 筆
            </div>
        </div>
        <div class="table-responsive gv-tb">
            <asp:GridView ID="GridView_tagNoHistory" runat="server" CssClass="gv" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true">
                <Columns>
                     <asp:TemplateField HeaderText="日期" ItemStyle-Width="120px">
                        <ItemTemplate>
                            <%#CDate(Eval("dataDate")).ToString("yyyy/MM/dd") %>
                        </ItemTemplate>
                     </asp:TemplateField>
                    <asp:TemplateField HeaderText="類型" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <%# Eval("typeName") %>
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
                            <%# 
                                If(Eval("placeType") IsNot Nothing AndAlso Eval("placeType").ToString() = "屠宰場", "-", MaskFarmCode(Eval("placeCode")))
                            %>
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
                              <asp:Label ID="Label_memo" runat="server" Visible='<%# If(Eval("memo") = "", False, True) %>' ToolTip='<%# Eval("memo") %>'><i class="fa-solid fa-comment-dots"></i></asp:Label>
                         </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="資料建立時間" ItemStyle-Width="120px">
                         <ItemTemplate>
                             <%# CDate(Eval("insertDateTime")).ToString("yyyy/MM/dd HH:mm") %>
                         </ItemTemplate>
                     </asp:TemplateField>
                    
                </Columns>
                <EmptyDataTemplate>
                <div class="text-danger text-center py-2 fw-bold">
                        目前沒有歷程記錄。
                    </div>
                </EmptyDataTemplate>
                <PagerStyle HorizontalAlign="Center"/>
            </asp:GridView>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
    系統訊息
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
    <asp:Label ID="Label_message" runat="server" CssClass="msg"></asp:Label>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
