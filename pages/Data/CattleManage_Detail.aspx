<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="CattleManage_Detail.aspx.vb" Inherits="taifCattle.CattleManage_Detail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-database"></i> 牛籍資料管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <asp:MultiView ID="MultiView_main" runat="server">
        <asp:View ID="View_new" runat="server">
            <div class="card">
                <div class="card-header">
                    新增牛籍基本資料
                </div>
                <div class="card-body"></div>
                <div class="card-footer d-flex justify-content-end gap-2">
                    <asp:Button ID="Button_save" runat="server" CssClass="btn btn-primary" Text="儲存" />
                    <asp:Button ID="Button_cancel" runat="server" CssClass="btn btn-outline-secondary" Text="取消" />
                </div>
            </div>
        </asp:View>

        <asp:View ID="View_edit" runat="server">

        </asp:View>
    </asp:MultiView>
</asp:Content>
