<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="MilkSetting.aspx.vb" Inherits="taifCattle.MilkSetting" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
     <style type="text/css">
        /* 設定卡片樣式 */
        .settings-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            overflow: hidden;
            margin-bottom: 30px;
        }

        .card-header {
            background: linear-gradient(135deg, #0f2350, #1a3b6b);
            color: white;
            padding: 20px 30px;
            border-radius: 12px 12px 0 0;
        }

        .card-title {
            font-size: 1.3rem;
            font-weight: 600;
            margin: 0;
        }

        .card-subtitle {
            font-size: 0.9rem;
            opacity: 0.9;
            margin: 5px 0 0 0;
        }

        .card-body {
            padding: 30px;
        }

        /* 產乳量表格樣式 */
        .production-table {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            overflow: hidden;
            margin-bottom: 30px;
        }

            .production-table .table {
                margin: 0;
            }

                .production-table .table th {
                    background: #f8f9fa;
                    color: #0f2350;
                    font-weight: 600;
                    border: none;
                    padding: 15px;
                    font-size: 1rem;
                    text-align: center;
                    position: sticky;
                    top: 0;
                    z-index: 10;
                }

                .production-table .table td {
                    padding: 12px 15px;
                    border-top: 1px solid #dee2e6;
                    vertical-align: middle;
                    text-align: center;
                }

                .production-table .table tbody tr:hover {
                    background-color: #f8f9fa;
                }

        /* 年齡欄位樣式 */
        .age-cell {
            font-weight: 600;
            color: #0f2350;
            background: #f8f9fa !important;
            font-size: 1rem;
        }

        /* 產乳量輸入框樣式 */
        .milk-input {
            border: 2px solid #e9ecef;
            border-radius: 6px;
            padding: 8px 12px;
            text-align: center;
            font-size: 0.95rem;
            font-weight: 500;
            transition: all 0.3s ease;
            width: 100%;
            max-width: 120px;
            margin: 0 auto;
        }

            .milk-input:focus {
                border-color: #0f2350;
                box-shadow: 0 0 0 0.2rem rgba(15,35,80,0.1);
                outline: none;
            }

            .milk-input:invalid {
                border-color: #dc3545;
                background-color: #fff5f5;
            }

        /* 按鈕樣式 */
        .btn-group-custom {
            display: flex;
            gap: 15px;
            justify-content: center;
            margin-top: 30px;
            padding-top: 30px;
            border-top: 1px solid #dee2e6;
        }

        .btn-save {
            background: linear-gradient(135deg, #28a745, #20c997);
            border: none;
            color: white;
            padding: 12px 40px;
            border-radius: 8px;
            font-size: 1rem;
            font-weight: 600;
            transition: all 0.3s ease;
            box-shadow: 0 2px 8px rgba(40,167,69,0.3);
        }

            .btn-save:hover {
                background: linear-gradient(135deg, #20c997, #17a2b8);
                transform: translateY(-1px);
                box-shadow: 0 4px 12px rgba(40,167,69,0.4);
                color: white;
            }

        .btn-reset {
            background: linear-gradient(135deg, #6c757d, #5a6268);
            border: none;
            color: white;
            padding: 12px 40px;
            border-radius: 8px;
            font-size: 1rem;
            font-weight: 600;
            transition: all 0.3s ease;
        }

            .btn-reset:hover {
                background: linear-gradient(135deg, #5a6268, #495057);
                transform: translateY(-1px);
                color: white;
            }

        .btn-cancel {
            background: transparent;
            border: 2px solid #6c757d;
            color: #6c757d;
            padding: 10px 40px;
            border-radius: 8px;
            font-size: 1rem;
            font-weight: 600;
            transition: all 0.3s ease;
        }

            .btn-cancel:hover {
                background: #6c757d;
                color: white;
                transform: translateY(-1px);
            }

        /* 警告訊息樣式 */
        .alert-info {
            background: linear-gradient(135deg, #d1ecf1, #bee5eb);
            border: none;
            border-left: 4px solid #17a2b8;
            border-radius: 8px;
            color: #0c5460;
            margin-bottom: 25px;
        }

            .alert-info .alert-heading {
                color: #0c5460;
                font-weight: 600;
            }

        /* 統計資訊樣式 */
        .stats-row {
            background: #f8f9fa;
            border-top: 2px solid #0f2350;
            font-weight: 600;
            color: #0f2350;
        }

            .stats-row td {
                background: #f8f9fa !important;
                font-size: 1rem;
            }
    </style>

    <script>
        window.scrollTo = function (x, y) {
            window.scroll({
                top: y,
                left: x,
                behavior: "instant"
            });
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    系統管理 > 平均產乳量設定
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    平均產乳量設定
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
     <div class="alert alert-info">
        <h6 class="alert-heading"><i class="fas fa-info-circle me-2"></i>設定說明</h6>
        依據台灣乳牛產乳量統計資料，設定各年齡乳牛的標準日產乳量（公升/日）。此設定將用於牛籍管理及保險計算基準。<br>
        <%--<strong>參考標準：</strong>台灣乳牛平均日產量約23公斤，高產牛可達30-40公斤，低產牛約20公斤。--%>
    </div>

    <!-- 產乳量設定卡片 -->
    <div class="settings-card">
        <div class="card-header">
            <h3 class="card-title">
                <i class="fas fa-tint me-2"></i>年齡別平均產乳量設定
            </h3>
            <p class="card-subtitle">設定1-20歲乳牛的標準日產乳量（公升/日）</p>
        </div>

        <div class="card-body">
            <div class="production-table">
                <div class="table-responsive">
                    <asp:GridView ID="GridView_milkSetting" runat="server" AutoGenerateColumns="False" DataKeyNames="age" CssClass="table" ShowHeaderWhenEmpty="true">
                        <Columns>
                            <asp:BoundField DataField="age" HeaderText="年齡（歲）" ReadOnly="True" ItemStyle-CssClass="age-cell" ItemStyle-Width="25%"/>
                            <asp:TemplateField HeaderText="日產乳量（公升/日）" ItemStyle-Width="25%">
                                <ItemTemplate>
                                    <asp:TextBox ID="TextBox_milkProduction" runat="server" CssClass="milk-input"
                                        Text='<%# Bind("milkProduction", "{0:0.0}") %>' 
                                        TextMode="Number" Min="0" Max="999.9" Step="0.1" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="備註" ItemStyle-Width="50%">
                                <ItemTemplate>
                                    <asp:TextBox ID="TextBox_remark" runat="server" CssClass="form-control"
                                        Text='<%# Bind("remark") %>'/>
                                </ItemTemplate>
                            </asp:TemplateField>
                          
                        </Columns>
                        <EmptyDataTemplate>
                           <div class="text-danger text-center py-2 fw-bold">
                                目前沒有設定值。
                            </div>
                        </EmptyDataTemplate>
                        <PagerStyle HorizontalAlign="Center"/>
                    </asp:GridView>
                </div>
            </div>
            
            <!-- 按鈕群組 -->
            <div class="btn-group-custom">
              
                <%--<button type="button" class="btn btn-cancel" onclick="cancelSettings()">
                    <i class="fas fa-times me-2"></i>取消
                </button>
                <button type="button" class="btn btn-reset" onclick="resetToDefault()">
                    <i class="fas fa-undo me-2"></i>重設為預設值
                </button>--%>
                <asp:LinkButton ID="LinkButton_save" runat="server" CssClass="btn btn-save">
                    <i class="fas fa-save me-2"></i>儲存設定
                </asp:LinkButton>
            </div>
              <div class="text-center">
                    <asp:Label ID="Label_message" runat="server" CssClass="text-danger"></asp:Label>
              </div>
        </div>
    </div>
</asp:Content>
