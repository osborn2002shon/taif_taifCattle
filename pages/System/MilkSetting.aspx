<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="MilkSetting.aspx.vb" Inherits="taifCattle.MilkSetting"%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-sliders"></i> 平均產乳量設定
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    平均產乳量設定
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <%--2025/11：畜產試驗所北區分所畜產輔導科提供胎次與泌乳量，以胎次+1歲進行計算參考。--%>

    <div class="card formCard">
        <div class="card-header">
            平均產乳量設定
        </div>
        <div class="card-body">
            <asp:GridView ID="GridView_milkSetting" runat="server" AutoGenerateColumns="False" DataKeyNames="age" CssClass="tb" ShowHeaderWhenEmpty="true">
                <Columns>
                    <asp:BoundField DataField="age" HeaderText="年齡（歲）" ReadOnly="True" ItemStyle-Width="25%"/>
                    <asp:TemplateField HeaderText="產乳量（公升/日）" ItemStyle-Width="25%">
                        <ItemTemplate>
                            <asp:TextBox ID="TextBox_milkProduction" runat="server" CssClass="form-control"
                                Text='<%# Bind("milkProduction", "{0:0.00}") %>' TextMode="Number" Min="0" Max="999.99" Step="0.01" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="備註" ItemStyle-Width="50%">
                        <ItemTemplate>
                            <asp:TextBox ID="TextBox_remark" runat="server" CssClass="form-control" Text='<%# Bind("remark") %>'/>
                        </ItemTemplate>
                    </asp:TemplateField>
                          
                </Columns>
            </asp:GridView>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="LinkButton_save" runat="server" CssClass="btn btn-primary"><i class="fas fa-save me-1"></i>儲存設定</asp:LinkButton>            
        </div>
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