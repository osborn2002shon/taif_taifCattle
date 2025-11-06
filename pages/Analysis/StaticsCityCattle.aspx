<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="StaticsCityCattle.aspx.vb" Inherits="taifCattle.StaticsCityCattle" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
     <i class="fa-solid fa-table"></i>縣市別牛隻編號詳報
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
                    <label>指定日期(起)</label>
                    <input type="date" id="dateBeg" name="dateBeg" class="form-control" value="<%=Property_Query_dateBeg.ToString("yyyy-MM-dd") %>">
                </div>
                <div class="col">
                    <label>指定日期(訖)</label>
                    <input type="date" id="dateEnd" name="dateEnd" class="form-control" value="<%=Property_Query_dateEnd.ToString("yyyy-MM-dd") %>">
                </div>
                <div class="col">
                    <label>畜牧場縣市</label>
                    <asp:DropDownList ID="DropDownList_farmCity" runat="server" class="form-select" ></asp:DropDownList>
                </div>
                   
            </div>
            <div class="row">
                <div class="col text-center">
                     <asp:LinkButton ID="LinkButton_excel" runat="server" CssClass="btn btn-success"><i class="fa-solid fa-file-arrow-down me-1"></i>報表下載</asp:LinkButton>
                </div>
                <%--<div class="col-12 text-center">
                    <asp:Label ID="Label_msg" runat="server" Text="" CssClass="text-danger"></asp:Label>
                </div>--%>
            </div>
      
        </div>
        <div class="queryBox-footer"></div>
    </div>
    <!-- 新增範例圖片區 -->
    <div class="text-center mt-4">
        <p class="text-muted mt-2">報表範例：縣市別牛隻編號詳報</p>
        <img src="../../_img/縣市別牛隻編號詳報_sample.png" 
            alt="範例圖片：縣市別牛隻編號詳報"
            class="img-fluid border rounded shadow-sm"
            style="max-width: 80%; height: auto;">
    
    </div>
     <div style="width:100%;overflow-x:auto;display:none">
         <asp:Panel ID="Panel_data" runat="server" Width="100%"></asp:Panel>
     </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
    系統訊息
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
    <asp:Label ID="Label_message" runat="server" CssClass="msg"></asp:Label>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>

