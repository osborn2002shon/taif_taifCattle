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
            <div class="card formCard">
                <div class="card-header">
                    輸入牛籍基本資料
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col">
                            <label>牛籍編號<span class="star">*</span></label>
                            <asp:TextBox ID="TextBox_tagNo" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col">
                            <label>編號備註</label>
                            <asp:TextBox ID="TextBox_tagMemo" runat="server" CssClass="form-control"></asp:TextBox>
                            <span class="info">若此牛有其他編號可以文字備註方式進行補充</span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label>類型規格<span class="star">*</span></label>
                            <asp:DropDownList ID="DropDownList_typeName" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col">
                            <label>出生年度（西元）</label>
                            <div class="input-group">
                                <asp:TextBox ID="TextBox_birthYear" runat="server" CssClass="form-control" TextMode="Number" MaxLength="4" min="2000"></asp:TextBox>
                                    <span class="input-group-text" onclick="clearControl('<%= TextBox_birthYear.ClientID %>')" style="cursor: pointer;"><i class="fa-solid fa-xmark"></i></span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <label>牛籍備註</label>
                            <asp:TextBox ID="TextBox_memo" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="card-footer text-center">
                    <div class="msg">
                        <asp:Label ID="Label_addMsg" runat="server"></asp:Label>
                    </div>
                    <asp:LinkButton ID="LinkButton_save" runat="server" CssClass="btn btn-primary"><i class="fa-solid fa-floppy-disk"></i> 新增</asp:LinkButton>
                    <asp:LinkButton ID="LinkButton_cancel" runat="server" CssClass="btn btn-outline-secondary"><i class="fa-solid fa-xmark"></i> 取消</asp:LinkButton>
                </div>
            </div>
        </asp:View>

        <asp:View ID="View_edit" runat="server">

        </asp:View>
    </asp:MultiView>
    <script type="text/javascript">
        function clearControl(controlId) {
            var textbox = document.getElementById(controlId);
            textbox.value = '';
            textbox.focus(); // 清除後自動聚焦
        }
    </script>
</asp:Content>
