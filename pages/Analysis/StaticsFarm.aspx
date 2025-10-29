<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="StaticsFarm.aspx.vb" Inherits="taifCattle.StaticsFarm" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <style>
        .selected-row {
            background-color: #f0f0f0; /* 淡灰底色 */
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
     <i class="fa-solid fa-magnifying-glass"></i>指定牧場牛籍查詢
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="queryBox">
        <div class="queryBox-header">
            牧場查詢條件
        </div>
        <div class="queryBox-body">
            <div class="row">
                <div class="col">
                    <label>縣市</label>
                    <asp:DropDownList ID="DropDownList_farmCity" runat="server" class="form-select" AutoPostBack="true"></asp:DropDownList>
                </div>
                <div class="col">
                    <label>鄉鎮</label>
                    <asp:DropDownList ID="DropDownList_farmTown" runat="server" class="form-select"></asp:DropDownList>
                </div>
                <div class="col">
                    <label>關鍵字查詢</label>
                    <div class="input-group">
                        <asp:TextBox ID="TextBox_farmKeyWord" runat="server" CssClass="form-control" placeHolder="請輸入牧場證號、牧場名稱或負責人姓名"></asp:TextBox>
                        <span class="input-group-text" onclick="clearControl('<%= TextBox_farmKeyWord.ClientID %>')" style="cursor: pointer;"><i class="fa-solid fa-xmark"></i></span>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col text-center">
                    <asp:LinkButton ID="LinkButton_farmQuery" runat="server" CssClass="btn btn-primary">
                        <i class="fas fa-search"></i>查詢
                    </asp:LinkButton>
                </div>
                <div class="col-12 text-center">
                    <asp:Label ID="Label_farmMsg" runat="server" Text="" CssClass="text-danger"></asp:Label>
                </div>
            </div>
         
        </div>
        <div class="queryBox-footer"></div>
    </div>
    <asp:Panel ID="Panel_farm" runat="server" Visible="false">
        <div class="card formCard h-100">
            <div class="card-header">
                <i class="fas fa-list"></i> 畜牧場清單
            </div>
            <div class="card-body">
                 <div class="table-responsive gv-tb">
                     <asp:GridView ID="GridView_farmList" runat="server" CssClass="gv" AutoGenerateColumns="false" AllowPaging="true" PageSize="10"  ShowHeaderWhenEmpty="true">
                         <Columns>
                           <asp:TemplateField HeaderText="選擇" ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:RadioButton ID="RadioButton_select" runat="server"
                                        GroupName="farmSelectGroup"
                                        onclick="recordSelectedFarm(this)"
                                        data-farmid='<%# Eval("farmID") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="city" HeaderText="縣市" ItemStyle-Width="5%" />
                            <asp:BoundField DataField="town" HeaderText="鄉鎮" ItemStyle-Width="5%"/>
                            <asp:TemplateField HeaderText="畜牧場證號" ItemStyle-Width="10%">
                                <ItemTemplate>
                                    <%#  MaskFarmCode(Eval("farmCode")) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="farmName" HeaderText="畜牧場名稱"  ItemStyle-Width="20%"/>
                            <asp:BoundField DataField="owner" HeaderText="負責人"  ItemStyle-Width="10%"/>
                            <asp:BoundField DataField="address" HeaderText="地址" />
                         </Columns>
                         <EmptyDataTemplate>
                         <div class="text-danger text-center py-2 fw-bold">
                                 目前沒有牧場資料。
                             </div>
                         </EmptyDataTemplate>
                         <PagerStyle HorizontalAlign="Center"/>
                     </asp:GridView>
                     <asp:HiddenField ID="HiddenField_selectedFarm" runat="server" />
                 </div>
            </div>
         <%--   <div class="card-footer text-center">
               
            </div>--%>
        </div>
        <!-- 牛籍查詢條件區 -->
        <div class="queryBox mt-3">
            <div class="queryBox-header">牛籍查詢條件</div>
            <div class="queryBox-body">
                <div class="row">
                    <div class="col">
                        <label>最新狀態</label>
                        <asp:CheckBoxList ID="CheckBoxList_status" runat="server"
                            RepeatDirection="Horizontal" >
                        </asp:CheckBoxList>
                    </div>
                    <div class="col">
                        <label>是否僅顯示現存牛籍</label><br />
                        <asp:CheckBox ID="CheckBox_currentOnly" runat="server" Text="僅顯示未除籍" />
                    </div>
                </div>
                <div class="row">
                    <div class="col text-center">
                        <asp:LinkButton ID="LinkButton_cattleQuery" runat="server" CssClass="btn btn-primary"><i class="fa-solid fa-floppy-disk"></i>確認選擇並查詢牛籍</asp:LinkButton>
                    </div>
                    <div class="col-12 text-center">
                        <asp:Label ID="Label_cattleMsg" runat="server" Text="" CssClass="text-danger"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="queryBox-footer"></div>
        </div>
    </asp:Panel>
    <asp:Panel ID="Panel_result" runat="server" Visible="false">
        <div class="card formCard h-100">
            <div class="card-header">
                <i class="fa-regular fa-id-card"></i> 牧場基本資訊
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col">
                        <label>畜牧場證號</label>
                        <asp:TextBox ID="TextBox_farm_farmCode" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                    </div>
                    <div class="col">
                        <label>畜牧場名稱</label>
                        <asp:TextBox ID="TextBox_farmInfo_farmName" runat="server" CssClass="form-control"  Enabled="false"></asp:TextBox>
                    </div>
                    <div class="col">
                        <label>畜牧場負責人</label>
                        <asp:TextBox ID="TextBox_farmInfo_owner" runat="server" CssClass="form-control"  Enabled="false"></asp:TextBox>
                    </div>
                    
                </div>
                <div class="row">
                     <div class="col">
                         <label>縣市</label>
                         <asp:TextBox ID="TextBox_farmInfo_city" runat="server" CssClass="form-control"  Enabled="false"></asp:TextBox>
                     </div>
                                         <div class="col">
                         <label>鄉鎮</label>
                         <asp:TextBox ID="TextBox_farmInfo_town" runat="server" CssClass="form-control"  Enabled="false"></asp:TextBox>
                     </div>
                    <div class="col">
                        <label>牧場地址</label>
                        <asp:TextBox ID="TextBox_farmInfo_address" runat="server" CssClass="form-control"  Enabled="false"></asp:TextBox>
                    </div>
                </div>
            </div>
           <%-- <div class="card-footer text-center">
            </div>--%>
        </div>
        <div class="row m-0 mt-3 mb-3 align-items-center">
            <div class="col p-0">

                <asp:LinkButton ID="LinkButton_excel" runat="server" CssClass="btn btn-outline-success">下載列表</asp:LinkButton>
            </div>
            <div class="col p-0 text-end">
                共 <asp:Label ID="Label_recordCount" runat="server" Text="0"></asp:Label> 筆
            </div>
        </div>
        <div class="table-responsive gv-tb">
            <asp:GridView ID="GridView_cattleList" runat="server" CssClass="gv" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true">
                <Columns>
                    <asp:TemplateField HeaderText="牛籍編號" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton_tagNo" runat="server" Text='<%# Eval("tagNo") %>'
                                CommandName="_cattle" CommandArgument='<%# Eval("cattleID") %>' >
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="編號備註" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <%# Eval("tagMemo") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="狀態" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <%# Eval("latestTypeName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="除籍" ItemStyle-Width="80px">
                        <ItemTemplate>
                            <%# If(IsDBNull(Eval("removeDate")) OrElse Eval("removeDate") Is Nothing, "未除籍", "已除籍") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="類型規格" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <%# Eval("groupName") & "：" & Eval("typeName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="出生年度" ItemStyle-Width="80px">
                        <ItemTemplate>
                            <%# If(Convert.ToInt32(Eval("birthYear")) = -1, "-", Eval("birthYear")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="牛籍歲齡" ItemStyle-Width="80px">
                        <ItemTemplate>
                            <%# If(Convert.ToInt32(Eval("cattleAge")) = -1, "-", Eval("cattleAge")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="平均乳產量" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <%# If(Convert.ToInt32(Eval("cattleTypeID")) = 2,
                                                                                    If(Convert.ToDecimal(Eval("milkProduction")) = -1, "-", Eval("milkProduction")), "-") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="牛籍備註" ItemStyle-Width="250px">
                        <ItemTemplate>
                            <%# Eval("cattleMemo") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
                <EmptyDataTemplate>
                <div class="text-danger text-center py-2 fw-bold">
                        目前沒有牛籍資料。
                    </div>
                </EmptyDataTemplate>
                <PagerStyle HorizontalAlign="Center"/>
            </asp:GridView>
        </div>

    </asp:Panel>

    <script type="text/javascript">
        function clearControl(controlId) {
            var textbox = document.getElementById(controlId);
            textbox.value = '';
            textbox.focus(); // 清除後自動聚焦
        }

        $(function () {
            // 點選 radio 時
            window.recordSelectedFarm = function (elem) {
                var $radio = $(elem);
                var $row = $radio.closest("tr");
                var farmID = $row.find("span[data-farmid]").data("farmid");

                // 取消其他所有 radio 的選取
                $("input[type=radio][onclick*='recordSelectedFarm']").not($radio).prop("checked", false);

                // 記錄目前選取
                $("#<%= HiddenField_selectedFarm.ClientID %>").val(farmID);

               // 高亮顯示目前選取的列（可選）
               $row.addClass("selected-row").siblings().removeClass("selected-row");
            };

           // 點選整列觸發 radio
           $("#<%= GridView_farmList.ClientID %> tr").on("click", function (e) {
                // 排除點擊到 input 或 span 本身（避免重複觸發）
                if (!$(e.target).is("input, span, i")) {
                    var $radio = $(this).find("input[type=radio]");
                    if ($radio.length > 0) {
                        $radio.prop("checked", true).trigger("click");
                    }
                }
            });

            // 換頁後還原選取
            restoreFarmSelection();

            function restoreFarmSelection() {
                var selectedFarm = $("#<%= HiddenField_selectedFarm.ClientID %>").val();
                if (selectedFarm) {
                    $("span[data-farmid]").each(function () {
                        if ($(this).data("farmid") == selectedFarm) {
                            $(this).find("input[type=radio]").prop("checked", true);
                            $(this).closest("tr").addClass("selected-row");
                        }
                    });
                }
            }
       });


    </script>
</asp:Content>
