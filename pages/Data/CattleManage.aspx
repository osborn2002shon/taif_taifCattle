<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="CattleManage.aspx.vb" Inherits="taifCattle.CattleManage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-cow"></i> 牛籍資料管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">    
    <div class="queryBox">
        <div class="queryBox-header">
            牛籍查詢與列表
        </div>
        <div class="queryBox-body">
            <asp:UpdatePanel runat="server" class="row">
                <ContentTemplate>
                    <div class="col">
                        <label>牛籍類型</label>
                        <asp:DropDownList ID="DropDownList_groupName" runat="server" CssClass="form-select" AutoPostBack="true"></asp:DropDownList>
                    </div>
                    <div class="col">
                        <label>牛籍規格</label>
                        <asp:DropDownList ID="DropDownList_typeName" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="col">
                        <label>牛籍狀態</label>
                        <asp:DropDownList ID="DropDownList_cattleStatus" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="row">
                <div class="col">
                    <label>牛籍編號</label>
                    <div class="input-group">
                        <asp:TextBox ID="TextBox_tagNo" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                        <span class="input-group-text" onclick="clearControl('<%= TextBox_tagNo.ClientID %>')" style="cursor: pointer;"><i class="fa-solid fa-xmark"></i></span>
                    </div>
                </div>
                <div class="col">
                    <label>出生年度</label>
                    <div class="input-group">
                        <asp:TextBox ID="TextBox_birthYear" runat="server" CssClass="form-control" TextMode="Number" MaxLength="4" min="2000"></asp:TextBox>
                            <span class="input-group-text" onclick="clearControl('<%= TextBox_birthYear.ClientID %>')" style="cursor: pointer;"><i class="fa-solid fa-xmark"></i></span>
                    </div>
                </div>
                <div class="col">
                    <label>牛籍歲齡</label>
                    <div class="input-group">
                    <asp:TextBox ID="TextBox_cattleAge" runat="server" CssClass="form-control" TextMode="Number" MaxLength="2" min="0"></asp:TextBox>
                        <span class="input-group-text" onclick="clearControl('<%= TextBox_cattleAge.ClientID %>')" style="cursor: pointer;"><i class="fa-solid fa-xmark"></i></span>
                    </div>
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
        <div class="col p-0">
            <asp:LinkButton ID="LinkButton_addNew" runat="server" CssClass="btn btn-success" CommandName="cattleAdd"><i class="fa-solid fa-plus me-1"></i>新增牛籍</asp:LinkButton>
            <%--<asp:LinkButton ID="LinkButton_excel" runat="server" CssClass="btn btn-outline-success">下載列表</asp:LinkButton>--%>
        </div>
        <div class="col p-0 text-end">
            共 <asp:Label ID="Label_datCount" runat="server"></asp:Label> 筆
        </div>
    </div>
    <div class="container-fluid gv-tb">
        <div class="table-responsive">
            <asp:GridView ID="GridView_data" runat="server" PageSize="25" AllowPaging="true" CssClass="gv" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true">
                <Columns>
                    <asp:TemplateField HeaderText="牛籍類型">
                        <ItemTemplate>
                            <%# IIf(Eval("groupName") = "肉牛", "<span class='tag_red'>肉牛</span>", IIf(Eval("groupName") = "乳牛", "<span class='tag_blue'>乳牛</span>", "<span class='tag_orange'>其他</span>")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="牛籍規格">
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
                            <%# IIf(Eval("cattleStatus") = "未除籍", "<span style='color:green'>未除籍</span>", "<span style='color:red'>已除籍</span>") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="出生年度">
                        <ItemTemplate>
                            <%# IIf(Eval("birthYear") = -1, "-", Eval("birthYear")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="牛籍歲齡">
                        <ItemTemplate>
                            <%# IIf(Eval("cattleAge") = -1, "-", Eval("cattleAge")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="操作">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton_edit" runat="server" CssClass="btn btn-sm btn-warning"
                                CommandName="cattleEdit" CommandArgument='<%# Eval("cattleID") %>' ><i class="fa-solid fa-pen-to-square me-1"></i>編輯</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    查無資料。
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</asp:Content>