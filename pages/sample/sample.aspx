<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="sample.aspx.vb" Inherits="taifCattle.sample" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    測試頁面
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
   <div class="container">
        <!-- 區塊 1：Excel 匯入測試 -->
         <div class="mb-5">
             <h4 class="mb-3">📘 區塊 1：Excel 匯入測試</h4>
             <div class="row g-3 align-items-center mb-3">
                 <div class="col-md-4">
                     <asp:FileUpload ID="fuExcel" runat="server" CssClass="form-control" />
                 </div>
                 <div class="col-md-2">
             
                     <asp:TextBox ID="txtColumnCount" runat="server" CssClass="form-control" placeholder="欄位數" />
                 </div>
                 <div class="col-md-2">
                     <asp:Button ID="btnUpload" runat="server" Text="上傳並顯示" CssClass="btn btn-primary w-100" />
                 </div>
             </div>

             <asp:Label ID="lblMessage" runat="server" CssClass="text-danger fw-bold"></asp:Label>

             <div class="table-responsive mt-3">
                 <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="True"
                     CssClass="gv"
                     HeaderStyle-CssClass="table-primary"
                     EmptyDataText="尚無資料">
                 </asp:GridView>
             </div>
         </div>

         <hr class="my-4" />

         <!-- 區塊 2：資料一致性檢查測試 -->
         <div>
             <h4 class="mb-3">🧩 區塊 2：資料一致性檢查測試</h4>
             <div class="row g-3 align-items-center mb-3">
                 <div class="col-md-3">
                     <asp:TextBox ID="txtEditId" runat="server" CssClass="form-control" placeholder="資料 ID" />
                 </div>
                 <div class="col-md-2">
                     <asp:Button ID="btnCheck" runat="server" Text="模擬儲存" CssClass="btn btn-primary w-100" />
                 </div>
             </div>
             <asp:HiddenField ID="hfLastUpdateTime" runat="server" />
             <asp:Label ID="lblCheckResult" runat="server" CssClass="fw-bold text-danger"></asp:Label>
         </div>
    </div>
</asp:Content>
