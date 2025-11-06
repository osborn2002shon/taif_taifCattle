<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="WebServiceLog.aspx.vb" Inherits="taifCattle.WebServiceLog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-clock-rotate-left"></i> 資料介接紀錄
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="queryBox">
        <div class="queryBox-header">
            介接紀錄查詢與列表
        </div>
        <div class="queryBox-body">
            <div class="row">
                <div class="col">
                    <label>紀錄日期(起)</label>
                    <asp:TextBox ID="TextBox_logDate_beg" runat="server" CssClass="form-control" TextMode="date"></asp:TextBox>
                </div>
                <div class="col">
                    <label>紀錄日期(迄)</label>
                    <asp:TextBox ID="TextBox_logDate_end" runat="server" CssClass="form-control" TextMode="date"></asp:TextBox>
                </div>
                <div class="col">
                    <label>介接類型</label>
                    <asp:DropDownList ID="DropDownList_actionType" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
                <div class="col">
                    <label>服務名稱</label>
                    <asp:DropDownList ID="DropDownList_apiName" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
            <div class="row">
                <div class="col text-center">
                    <asp:LinkButton ID="LinkButton_query" runat="server" CssClass="btn btn-primary">
                        <i class="fas fa-search me-1"></i>查詢
                    </asp:LinkButton>
                </div>
            </div>
        </div>
        <div class="queryBox-footer"></div>
    </div>
    <div class="row m-0 mt-3 mb-3 align-items-center">
        <div class="col p-0"></div>
        <div class="col p-0 text-end">
            共 <asp:Label ID="Label_datCount" runat="server"></asp:Label> 筆
        </div>
    </div>
    <div class="container-fluid gv-tb">
        <div class="table-responsive">
            <asp:GridView ID="GridView_data" runat="server" PageSize="25" AllowPaging="true" CssClass="gv" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true">
                <Columns>
                    <asp:TemplateField HeaderText="流水號" ItemStyle-Width="80">
                        <ItemTemplate>
                            <%# Eval("logID") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="記錄時間" ItemStyle-Width="210">
                        <ItemTemplate>
                            <%# CDate(Eval("logDateTime")).ToString("yyyy/MM/dd HH:mm") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="介接類型" ItemStyle-Width="100">
                        <ItemTemplate>
                            <%# If(Eval("actionType") = "dataIn", "接入", "接出") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="觸發類型" ItemStyle-Width="100">
                        <ItemTemplate>
                            <%# Eval("triggerType") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:TemplateField HeaderText="介接對象">
                        <ItemTemplate>
                            <%# Eval("dataSourceName") %>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:TemplateField HeaderText="服務名稱" ItemStyle-Width="250">
                        <ItemTemplate>
                            <%# Eval("apiName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="資料筆數" ItemStyle-Width="130">
                        <ItemTemplate>
                            <%#CInt(Eval("dataCount")).ToString("N0") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="IP／備註">
                        <ItemTemplate>
                            <div class="text_info">
                                <div class="text_info"><%# Eval("ip") %></div>
                                <%# Eval("queryMemo") %>
                                <asp:Label ID="Label_isError" runat="server" Visible='<%# Eval("isError") %>' ToolTip="請至資料庫查看錯誤訊息"><i class="fa-solid fa-triangle-exclamation"></i></asp:Label>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:TemplateField HeaderText="是否錯誤">
                        <ItemTemplate>
                            <%# If(Eval("isError") = True, "V", "") %>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                </Columns>
                <EmptyDataTemplate>
                    查無資料。
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
