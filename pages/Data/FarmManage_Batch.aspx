<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="FarmManage_Batch.aspx.vb" Inherits="taifCattle.FarmManage_Batch" MaintainScrollPositionOnPostBack="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <style>
        .farm-details {
            transition: height .2s ease;
        }

        .farm-details.farm-details-collapsed {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fa-solid fa-database"></i> 牧場批次新增
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="queryBox">
        <div class="queryBox-header">
            待新增畜牧場清單
        </div>
        <div class="queryBox-body">
            <asp:Label ID="Label_message" runat="server" CssClass="text-danger fw-bold d-block mb-3"></asp:Label>

            <asp:Panel ID="Panel_serialInfo" runat="server" CssClass="alert alert-info d-flex align-items-start" Visible="false">
                <i class="fa-solid fa-circle-info me-2 mt-1"></i>
                <div>
                    <div>匯入流水號：<strong><asp:Literal ID="Literal_serial" runat="server" /></strong></div>
                    <div>資料來源：<asp:Literal ID="Literal_source" runat="server" /></div>
                </div>
            </asp:Panel>

            <asp:Panel ID="Panel_noData" runat="server" CssClass="alert alert-secondary" Visible="false">
                <i class="fa-solid fa-check me-2"></i> 本次匯入沒有待新增的畜牧場資料。
            </asp:Panel>

            <asp:Repeater ID="Repeater_missingFarms" runat="server">
                <ItemTemplate>
                    <div class="card mb-3">
                        <div class="card-header d-flex align-items-center gap-2">
                            <asp:CheckBox ID="CheckBox_select" runat="server"/>
                            <span class="fw-bold">畜牧場證號：<%# Eval("farmCode") %></span>
                            <asp:Label ID="Label_existing" runat="server" CssClass="badge bg-success ms-auto" Text="已新增" Visible="false"></asp:Label>
                        </div>
                        <asp:Panel ID="Panel_inputs" runat="server" CssClass="card-body farm-details">
                            <asp:HiddenField ID="HiddenField_farmCode" runat="server" Value='<%# Eval("farmCode") %>' />
                            <asp:HiddenField ID="HiddenField_dataSource" runat="server" Value='<%# Eval("dataSource") %>' />
                            <div class="row g-3">
                                <div class="col-md-6">
                                    <label class="form-label">畜牧場名稱 <span class="text-danger">*</span></label>
                                    <asp:TextBox ID="TextBox_farmName" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">負責人 <span class="text-danger">*</span></label>
                                    <asp:TextBox ID="TextBox_owner" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">連絡電話</label>
                                    <asp:TextBox ID="TextBox_ownerTel" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                </div>
                                <div class="col-12">
                                    <label class="form-label">畜牧場地址 <span class="text-danger">*</span></label>
                                    <div class="row g-2">
                                        <div class="col-md-4">
                                            <asp:DropDownList ID="DropDownList_city" runat="server" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="DropDownList_city_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                        <div class="col-md-4">
                                            <asp:DropDownList ID="DropDownList_area" runat="server" CssClass="form-select"></asp:DropDownList>
                                        </div>
                                        <div class="col-md-4">
                                            <asp:TextBox ID="TextBox_address" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">飼養規模</label>
                                    <asp:TextBox ID="TextBox_animalCount" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">備註</label>
                                    <asp:TextBox ID="TextBox_memo" runat="server" CssClass="form-control" MaxLength="300"></asp:TextBox>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="Panel_actions" runat="server" CssClass="d-flex justify-content-between mt-3" Visible="false">
                <asp:HyperLink ID="HyperLink_back" runat="server" CssClass="btn btn-outline-secondary" NavigateUrl="~/pages/Data/FarmManage.aspx">
                    <i class="fa-solid fa-arrow-left"></i> 返回牧場管理
                </asp:HyperLink>
                <asp:LinkButton ID="LinkButton_submit" runat="server" CssClass="btn btn-success">
                    <i class="fa-solid fa-plus"></i> 新增選取畜牧場
                </asp:LinkButton>
            </asp:Panel>
        </div>
    </div>

    <script type="text/javascript">
        (function () {
            function updateCardDetails(checkbox) {
                if (!checkbox) {
                    return;
                }

                var card = checkbox.closest('.card');
                if (!card) {
                    return;
                }

                var details = card.querySelector('.farm-details');
                if (!details) {
                    return;
                }

                if (checkbox.checked) {
                    details.classList.remove('farm-details-collapsed');
                } else {
                    details.classList.add('farm-details-collapsed');
                }
            }

            function onCheckboxChange(event) {
                updateCardDetails(event.target);
            }

            function initFarmCardToggles() {
                var checkboxes = document.querySelectorAll('.farm-card-select');
                checkboxes.forEach(function (checkbox) {
                    checkbox.removeEventListener('change', onCheckboxChange);
                    checkbox.addEventListener('change', onCheckboxChange);
                    updateCardDetails(checkbox);
                });
            }

            if (window.Sys && Sys.Application && typeof Sys.Application.add_load === 'function') {
                Sys.Application.add_load(initFarmCardToggles);
            } else {
                document.addEventListener('DOMContentLoaded', initFarmCardToggles);
                window.addEventListener('load', initFarmCardToggles);
            }
        })();
    </script>
</asp:Content>
