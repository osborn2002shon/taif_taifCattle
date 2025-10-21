<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="CattleManage.aspx.vb" Inherits="taifCattle.CattleManage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-database"></i> 牛籍資料管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="queryBox">
        <div class="queryBox-header">
            牛籍查詢與列表
        </div>
        <div class="queryBox-body">
            <div class="row">
                <div class="col">
                    <label>牛籍類型</label>
                    <asp:DropDownList ID="DropDownList_groupName" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="col">
                    <label>牛籍規格</label>
                    <asp:DropDownList ID="DropDownList_typeName" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="col">
                    <label>牛籍狀態</label>
                    <asp:DropDownList ID="DropDownList_cattleStatus" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="col">
                    <label>牛籍編號</label>
                    <asp:TextBox ID="TextBox_tagNo" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <asp:LinkButton ID="LinkButton_query" runat="server" CssClass="btn btn-primary">
                        <i class="fas fa-search me-1"></i>查詢
                    </asp:LinkButton>
                </div>
            </div>
        </div>
        <div class="queryBox-footer"></div>
    </div>
    <div class="row m-3">
        <div class="col p-0">
            <asp:LinkButton ID="LinkButton_addNew" runat="server" CssClass="btn btn-primary">新增牛籍</asp:LinkButton>
        </div>
        <div class="col p-0 text-end">
            共 1000 筆
        </div>
    </div>
    <div class="table-responsive gv-tb">
        <asp:GridView ID="GridView_data" runat="server" PageSize="25" AllowPaging="true" CssClass="gv" AutoGenerateColumns="false">
            <Columns>
                <asp:TemplateField HeaderText="牛籍類型">
                    <ItemTemplate>
                        <%# Eval("groupName") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="牛種規格">
                    <ItemTemplate>
                        <%# Eval("typeName") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="牛籍編號">
                    <ItemTemplate>
                        <%# Eval("tagNo") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="牛籍狀態">
                    <ItemTemplate>
                        <%# Eval("cattleStatus") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="出生年度">
                    <ItemTemplate>
                        <%# IIf(Eval("birthYear") = -1, "", Eval("birthYear")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="牛籍歲齡">
                    <ItemTemplate>
                        <%# IIf(Eval("cattleAge") = -1, "", Eval("cattleAge")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-CssClass="text-end">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton_edit" runat="server" CssClass="btn btn-primary"
                            CommandName="edit" CommandArgument='<%# Eval("cattleID") %>' >編輯</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>    
</asp:Content>
