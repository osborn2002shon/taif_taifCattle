<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="FarmManage.aspx.vb" Inherits="taifCattle.FarmManage"  MaintainScrollPositionOnPostback="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-house"></i> 牧場資料管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <asp:MultiView ID="MultiView_main" runat="server">
        <asp:View ID="View_list" runat="server">
            <div class="queryBox">
                <div class="queryBox-header">
                    畜牧場查詢與列表
                </div>
                <div class="queryBox-body">
                    <div class="row">
                        <div class="col">
                            <label>縣市</label>
                            <asp:DropDownList ID="DropDownList_city" runat="server" class="form-select" AutoPostBack="true"></asp:DropDownList>
                        </div>
                        <div class="col">
                            <label>鄉鎮</label>
                            <asp:DropDownList ID="DropDownList_town" runat="server" class="form-select"></asp:DropDownList>
                        </div>
                        <div class="col">
                            <label>關鍵字查詢</label>
                             <div class="input-group">
                                  <asp:TextBox ID="TextBox_keyWord" runat="server" CssClass="form-control" placeHolder="請輸入牧場證號、牧場名稱或負責人姓名"></asp:TextBox>
                                 <span class="input-group-text" onclick="clearControl('<%= TextBox_keyWord.ClientID %>')" style="cursor: pointer;"><i class="fa-solid fa-xmark"></i></span>
                             </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col text-center">
                            <asp:LinkButton ID="LinkButton_query" runat="server" CssClass="btn btn-primary">
                                <i class="fas fa-search me-1"></i>查詢
                            </asp:LinkButton>
                           <%-- <asp:LinkButton ID="LinkButton_excel" runat="server" CssClass="btn btn-primary">
                                <i class="fas fa-download me-1"></i>匯出Excel
                            </asp:LinkButton>--%>
                        </div>
                    </div>
                </div>
                <div class="queryBox-footer"></div>
            </div>
            <div class="row m-0 mt-3 mb-3 align-items-center">
                <div class="col p-0">
                    <asp:LinkButton ID="LinkButton_addFarm" runat="server" CssClass="btn btn-success"><i class="fa-solid fa-plus me-1"></i>新增畜牧場</asp:LinkButton>
                    <%-- 統一暫不提供下載+注意身份證字號遮蔽 --%>
                    <asp:LinkButton ID="LinkButton_excel" runat="server" CssClass="btn btn-outline-success" Visible="false"><i class="fa-solid fa-file-arrow-down me-1"></i>列表下載</asp:LinkButton>
                </div>
                <div class="col p-0 text-end">
                    共 <asp:Label ID="Label_recordCount" runat="server"  Text="0"></asp:Label> 筆
                </div>
            </div>
            <div class="table-responsive  gv-tb">
                <asp:GridView ID="GridView_farmList" runat="server" CssClass="gv" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" 
                    ShowHeaderWhenEmpty="true">
                    <Columns>
                        <%-- <asp:TemplateField HeaderText="系統<br>流水號" ItemStyle-Width="5%">
                            <ItemTemplate>
                                <%# Eval("farmID") %>
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                        <asp:BoundField DataField="city" HeaderText="縣市" ItemStyle-Width="5%" />
                        <asp:BoundField DataField="town" HeaderText="鄉鎮" ItemStyle-Width="5%"/>
                        <asp:BoundField DataField="farmName" HeaderText="畜牧場名稱" ItemStyle-Width="10%"/>
                        <asp:TemplateField HeaderText="畜牧場證號<br/><div style='font-size:0.8rem;'>(畜牧場證號/畜禽飼養登記證<br/>/負責人證號)</div>" ItemStyle-Width="15%">
                            <ItemTemplate>
                                <%# MaskFarmCode(Eval("farmCode")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="owner" HeaderText="負責人" ItemStyle-Width="5%"/>
                    <%-- <asp:BoundField DataField="ownerID" HeaderText="負責人證號" ItemStyle-Width="8%" />--%>
                        <asp:BoundField DataField="address" HeaderText="畜牧場地址" ItemStyle-Width="25%" ItemStyle-CssClass="txt_left"/>
                    <%--    <asp:BoundField DataField="insertType" HeaderText="新增來源" ItemStyle-Width="8%"/>--%>
                        <asp:TemplateField ItemStyle-Width="5%">
                            <ItemTemplate>
                                 <asp:LinkButton ID="LinkButton_edit" runat="server" CssClass="btn btn-sm btn-warning"
                                    CommandName="myEdit" CommandArgument='<%# Eval("farmID") %>' ><i class="fa-solid fa-pen-to-square me-1"></i>編輯</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
   
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="star text-center py-2 fw-bold">
                            目前沒有牧場資料。
                        </div>
                    </EmptyDataTemplate>
                    <PagerStyle HorizontalAlign="Center"/>
                </asp:GridView>
            </div>
        </asp:View>
        <asp:View ID="View_edit" runat="server">
            <div class="card formCard">
                <div class="card-header">
                    <asp:Label ID="Label_editTitle" runat="server" Text="新增畜牧場"></asp:Label>
                </div>
                <div class="card-body">
                    <div class="row">
                         <!-- 畜牧場證號／登記證／負責人證號 -->
                         <div class="col">
                             <label>
                                 畜牧場證號/畜禽飼養登記證(負責人證號)<span class="star">*</span>
                             </label>

                             <!-- 新增 Label 顯示：僅在編輯模式時出現 -->
                             <asp:Label ID="Label_farmCode_display" runat="server" CssClass="form-control bg-light text-secondary" Visible="false"></asp:Label>

                             <!-- TextBox：僅在新增模式可編輯 -->
                             <asp:TextBox ID="TextBox_farmCode" runat="server" CssClass="form-control" placeholder="請輸入牧場證號或登記證號"></asp:TextBox>

                             <!-- 備註說明 -->
                             <small class="text_info">
                                 ※若無畜牧場證號或登記證，請填寫負責人證號
                             </small>
                         </div>

                        <!-- 畜牧場名稱（必填） -->
                        <div class="col">
                            <label>畜牧場名稱<span class="star">*</span></label>
                            <asp:TextBox ID="TextBox_farmName" runat="server" CssClass="form-control" placeholder="請輸入畜牧場名稱"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <!-- 負責人（必填） -->
                        <div class="col">
                            <label>負責人<span class="star">*</span></label>
                            <asp:TextBox ID="TextBox_owner" runat="server" CssClass="form-control" placeholder="請輸入負責人姓名"></asp:TextBox>
                        </div>

                        <!-- 負責人證號（暫時隱藏） -->
                        <div class="col" style="display:none;">
                            <label>負責人證號</label>
                            <asp:TextBox ID="TextBox_ownerID" runat="server" CssClass="form-control" placeholder="請輸入負責人身分證號"></asp:TextBox>
                        </div>

                        <!-- 連絡電話 -->
                        <div class="col">
                            <label>連絡電話</label>
                            <asp:TextBox ID="TextBox_ownerTel" runat="server" CssClass="form-control" placeholder="請輸入電話"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <!-- 地址（必填） -->
                        <div class="col-12">
                            <label>畜牧場地址<span class="star">*</span></label>
                            <div class="row">
                                <!-- 縣市 -->
                                <div class="col-3">
                                    <asp:DropDownList ID="DropDownList_editCity" runat="server"
                                        CssClass="form-select" AutoPostBack="true"></asp:DropDownList>
                                </div>

                                <!-- 鄉鎮 -->
                                <div class="col-3">
                                    <asp:DropDownList ID="DropDownList_editTown" runat="server"
                                        CssClass="form-select"></asp:DropDownList>
                                </div>

                                <!-- 地址文字 -->
                                <div class="col-6">
                                    <asp:TextBox ID="TextBox_address" runat="server"
                                        CssClass="form-control" placeholder="請輸入詳細地址（路名、門牌號）"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <!-- 飼養規模 -->
                        <div class="col-6">
                            <label>飼養規模</label>
                            <asp:TextBox ID="TextBox_animalCount" runat="server" CssClass="form-control" placeholder="請輸入飼養規模"></asp:TextBox>
                        </div>

                        <!-- 備註 -->
                        <div class="col-6">
                            <label>備註</label>
                            <asp:TextBox ID="TextBox_memo" runat="server" CssClass="form-control" placeholder="可輸入備註說明"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton ID="LinkButton_save" runat="server" CssClass="btn btn-primary">儲存</asp:LinkButton>
                    <asp:LinkButton ID="LinkButton_cancel" runat="server" CssClass="btn btn-outline-secondary">取消</asp:LinkButton>
                </div>
            </div>
        </asp:View>
    </asp:MultiView>   
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder_msg_title" runat="server">
    系統訊息
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolder_msg_content" runat="server">
    <asp:Label ID="Label_msg" runat="server" CssClass="msg"></asp:Label>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentPlaceHolder_msg_btn" runat="server">
</asp:Content>
