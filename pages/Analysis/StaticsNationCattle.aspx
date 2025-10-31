<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="StaticsNationCattle.aspx.vb" Inherits="taifCattle.StaticsNationCattle" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-table"></i>全國牛隻在養總表
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
                    <label>畜牧場縣市</label>
                    <asp:DropDownList ID="DropDownList_farmCity" runat="server" class="form-select" ></asp:DropDownList>
                </div>
                
            </div>
            <div class="row">
                <div class="col text-center">
            <%--        <asp:LinkButton ID="LinkButton_query" runat="server" CssClass="btn btn-primary">
                        <i class="fas fa-search"></i>查詢
                    </asp:LinkButton>--%>
                    <asp:LinkButton ID="LinkButton_excel" runat="server" CssClass="btn btn-outline-success">下載列表</asp:LinkButton>
                </div>
                <div class="col-12 text-center">
                    <asp:Label ID="Label_msg" runat="server" Text="" CssClass="text-danger"></asp:Label>
                </div>
            </div>
   
        </div>
        <div class="queryBox-footer"></div>
    </div>
    <!-- 新增範例圖片區 -->
    <div class="text-center mt-4">
        <p class="text-muted mt-2">範例圖片：全國牛隻在養總表</p>
        <img src="../../_img/全國牛隻在養總表_sample.png" 
             alt="範例圖片：全國牛隻在養總表"
             class="img-fluid border rounded shadow-sm"
             style="max-width: 80%; height: auto;">
      
    </div>
</asp:Content>
