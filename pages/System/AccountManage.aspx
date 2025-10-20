<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="AccountManage.aspx.vb" Inherits="taifCattle.AccountManage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
    <style type="text/css">
        /* 狀態標籤 */
        .status-badge {
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 500;
        }

        .status-active {
            background: #d4edda;
            color: #155724;
        }

        .status-inactive {
            background: #f8d7da;
            color: #721c24;
        }

        .status-locked {
            background: #fff3cd;
            color: #856404;
        }

        /* 角色標籤 */
        .role-badge {
            padding: 4px 10px;
            border-radius: 15px;
            font-size: 0.8rem;
            font-weight: 500;
        }

        .role-admin {
            background: #e2e3e5;
            color: #41464b;
        }

        .role-manager {
            background: #cfe2ff;
            color: #084298;
        }

        .role-operator {
            background: #d1ecf1;
            color: #0c5460;
        }

        .role-viewer {
            background: #f8d7da;
            color: #842029;
        }

        /* 帳號ID樣式 */
        .account-id {
            font-family: 'Courier New', monospace;
            font-weight: 600;
            color: #0f2350;
        }

        /* 操作按鈕 */

        .btn-add-account {
            background: linear-gradient(135deg, #28a745, #20c997);
            border: none;
            color: white;
            padding: 10px 20px;
            border-radius: 8px;
            font-size: 0.95rem;
            font-weight: 500;
            transition: all 0.3s ease;
            box-shadow: 0 2px 8px rgba(40,167,69,0.3);
        }

            .btn-add-account:hover {
                background: linear-gradient(135deg, #20c997, #17a2b8);
                transform: translateY(-1px);
                box-shadow: 0 4px 12px rgba(40,167,69,0.4);
                color: white;
            }

        .btn-edit {
            background: linear-gradient(135deg, #0f2350, #1a3b6b);
            border: none;
            color: white;
            padding: 6px 12px;
            border-radius: 6px;
            font-size: 0.8rem;
            font-weight: 500;
            transition: all 0.3s ease;
            /*margin-right: 5px;*/
        }

            .btn-edit:hover {
                background: linear-gradient(135deg, #1a3b6b, #2c5282);
                transform: translateY(-1px);
                box-shadow: 0 3px 8px rgba(15,35,80,0.3);
                color: white;
            }

        .btn-password {
            background: linear-gradient(135deg, #ffc107, #fd7e14);
            border: none;
            color: white;
            padding: 6px 12px;
            border-radius: 6px;
            font-size: 0.8rem;
            font-weight: 500;
            transition: all 0.3s ease;
        }

            .btn-password:hover {
                background: linear-gradient(135deg, #fd7e14, #dc3545);
                transform: translateY(-1px);
                box-shadow: 0 3px 8px rgba(255,193,7,0.3);
                color: white;
            }


        .btn-approve {
            background: linear-gradient(135deg, #28a745, #20c997);
            border: none;
            color: white;
            padding: 6px 12px;
            border-radius: 6px;
            font-size: 0.8rem;
            font-weight: 500;
            transition: all 0.3s ease;
            /*margin-left: 5px;*/
        }

        .btn-approve:hover {
            background: linear-gradient(135deg, #20c997, #17a2b8);
            transform: translateY(-1px);
            box-shadow: 0 3px 8px rgba(40,167,69,0.3);
            color: white;
        }

        /* 最後登入時間 */
        .last-login {
            font-size: 0.85rem;
            color: #666;
        }


        .status-pending {
            background: #fff3cd;
            color: #856404;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    系統管理 > 系統帳號管理
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
    系統帳號管理
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="data-table">
    <div class="table-header">
        <div class="table-title-group">
            <h3 class="table-title">系統帳號查詢與列表</h3>
            <p class="table-subtitle">管理所有系統使用者帳號與權限設定</p>
        </div>
        <div class="table-actions">
            <button class="btn btn-add-account" onclick="addNewAccount()">
                <i class="fas fa-user-plus me-1"></i>新增帳號
            </button>
        </div>
    </div>

    <div class="table-body">
        <!-- 篩選器區域 -->
        <div class="p-4 border-bottom">
            <div class="row g-3">
                <div class="col-md-3">
                    <label class="form-label">帳號狀態</label>
                    <select class="form-select" id="statusFilter">
                        <option value="">全部狀態</option>
                        <option value="active">啟用</option>
                        <option value="inactive">停用</option>
                        <option value="pending">待驗證</option>
                    </select>
                </div>
                <div class="col-md-3">
                    <label class="form-label">系統權限</label>
                    <!-- 讀取System_UserAuType -->
                    <select class="form-select" id="roleFilter">
                        <option value="">全部權限</option>
                    </select>
                </div>
                <div class="col-md-6">
                    <label class="form-label">關鍵字查詢</label>
                    <input type="text" class="form-control" id="keywordFilter" placeholder="請輸入電子信箱或使用者姓名">
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-12">
                    <button class="btn btn-primary me-2" onclick="searchAccounts()">
                        <i class="fas fa-search me-1"></i>搜尋
                    </button>
                    <button class="btn btn-outline-secondary me-2" onclick="resetFilter()">
                        <i class="fas fa-redo me-1"></i>重置
                    </button>
                    <button class="btn btn-success" onclick="exportData()">
                        <i class="fas fa-download me-1"></i>匯出Excel
                    </button>
                </div>
            </div>
        </div>

        <!-- 資料表格 -->
        <div class="table-responsive">
            <table class="table">
                <thead>
                    <tr>
                        <th>登入帳號（電子信箱）</th>
                        <th>使用者姓名</th>
                        <th>使用者角色</th>
                        <th>帳號狀態</th>
                        <th>建立日期</th>
                        <th>最後登入</th>
                        <th>操作</th>
                    </tr>
                </thead>
                <tbody id="accountTableBody">
                    <tr>
                        <td><span class="account-id">admin@example.com</span></td>
                        <td><strong>系統管理員</strong></td>
                        <td><span class="role-badge role-admin">系統管理者</span></td>
                        <td><span class="status-badge status-active">啟用</span></td>
                        <td>2024-01-01</td>
                        <td class="last-login">2024-07-29 09:15:23</td>
                        <td>
                            <button class="btn btn-edit">
                                <i class="fas fa-edit me-1"></i>編輯
                            </button>
                            <button class="btn btn-password">
                                <i class="fas fa-key me-1"></i>重設密碼
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <td><span class="account-id">wang.daming@example.com</span></td>
                        <td><strong>王大明</strong></td>
                        <td><span class="role-badge role-manager">一般管理員</span></td>
                        <td><span class="status-badge status-active">啟用</span></td>
                        <td>2024-02-15</td>
                        <td class="last-login">2024-07-28 16:42:18</td>
                        <td>
                            <button class="btn btn-edit">
                                <i class="fas fa-edit me-1"></i>編輯
                            </button>
                            <button class="btn btn-password">
                                <i class="fas fa-key me-1"></i>重設密碼
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <td><span class="account-id">li.xiaohua@example.com</span></td>
                        <td><strong>李小華</strong></td>
                        <td><span class="role-badge role-operator">查詢使用者</span></td>
                        <td><span class="status-badge status-pending">待驗證</span></td>
                        <td>2024-03-10</td>
                        <td class="last-login">從未登入</td>
                        <td>
                            <button class="btn btn-approve" onclick="approveAccount('li.xiaohua@example.com')">
                                <i class="fas fa-envelope me-1"></i>重發驗證信
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <td><span class="account-id">zhang.zhiming@example.com</span></td>
                        <td><strong>張志明</strong></td>
                        <td><span class="role-badge role-viewer">一般使用者</span></td>
                        <td><span class="status-badge status-inactive">停用</span></td>
                        <td>2024-04-05</td>
                        <td class="last-login">2024-07-20 14:22:11</td>
                        <td>
                            <button class="btn btn-edit" onclick="editAccount('zhang.zhiming@example.com')">
                                <i class="fas fa-edit me-1"></i>編輯
                            </button>
                            <button class="btn btn-password" onclick="changePassword('zhang.zhiming@example.com')">
                                <i class="fas fa-key me-1"></i>重設密碼
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <td><span class="account-id">chen.meiling@example.com</span></td>
                        <td><strong>陳美玲</strong></td>
                        <td><span class="role-badge role-operator">一般使用者</span></td>
                        <td><span class="status-badge status-pending">待審核</span></td>
                        <td>2024-05-20</td>
                        <td class="last-login">從未登入</td>
                        <td>
                            <button class="btn btn-approve" onclick="approveAccount('li.xiaohua@example.com')">
                                <i class="fas fa-envelope me-1"></i>重發驗證信
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <td><span class="account-id">liu.jianguo@example.com</span></td>
                        <td><strong>劉建國</strong></td>
                        <td><span class="role-badge role-manager">一般管理員</span></td>
                        <td><span class="status-badge status-active">啟用</span></td>
                        <td>2024-06-12</td>
                        <td class="last-login">2024-07-29 07:45:12</td>
                        <td>
                            <button class="btn btn-edit" onclick="editAccount('liu.jianguo@example.com')">
                                <i class="fas fa-edit me-1"></i>編輯
                            </button>
                            <button class="btn btn-password" onclick="changePassword('liu.jianguo@example.com')">
                                <i class="fas fa-key me-1"></i>重設密碼
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <td><span class="account-id">huang.shujuan@example.com</span></td>
                        <td><strong>黃淑娟</strong></td>
                        <td><span class="role-badge role-viewer">一般使用者</span></td>
                        <td><span class="status-badge status-active">啟用</span></td>
                        <td>2024-07-01</td>
                        <td class="last-login">2024-07-28 15:30:28</td>
                        <td>
                            <button class="btn btn-edit" onclick="editAccount('huang.shujuan@example.com')">
                                <i class="fas fa-edit me-1"></i>編輯
                            </button>
                            <button class="btn btn-password" onclick="changePassword('huang.shujuan@example.com')">
                                <i class="fas fa-key me-1"></i>重設密碼
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <td><span class="account-id">lin.zhihao@example.com</span></td>
                        <td><strong>林志豪</strong></td>
                        <td><span class="role-badge role-operator">一般使用者</span></td>
                        <td><span class="status-badge status-active">啟用</span></td>
                        <td>2024-07-15</td>
                        <td class="last-login">從未登入</td>
                        <td>
                            <button class="btn btn-edit" onclick="editAccount('lin.zhihao@example.com')">
                                <i class="fas fa-edit me-1"></i>編輯
                            </button>
                            <button class="btn btn-password" onclick="changePassword('lin.zhihao@example.com')">
                                <i class="fas fa-key me-1"></i>重設密碼
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- 分頁 -->
        <div class="d-flex justify-content-between align-items-center p-3">
            <div class="text-muted">
                顯示第 1 到 8 筆，共 8 筆帳號
            </div>
            <nav>
                <ul class="pagination mb-0">
                    <li class="page-item disabled">
                        <span class="page-link">上一頁</span>
                    </li>
                    <li class="page-item active">
                        <span class="page-link">1</span>
                    </li>
                    <li class="page-item disabled">
                        <span class="page-link">下一頁</span>
                    </li>
                </ul>
            </nav>
        </div>
    </div>
</div>

</asp:Content>
