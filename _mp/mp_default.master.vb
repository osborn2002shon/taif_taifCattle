Imports taifCattle.taifCattle.Base

Public Class mp_default
    Inherits System.Web.UI.MasterPage
    Public taifCattle_base As New taifCattle.Base

    Public Property menuStr_def As String
        Get
            Return ViewState("mp_menuStr_def")
        End Get
        Set(value As String)
            ViewState("mp_menuStr_def") = value
        End Set
    End Property

    ''' <summary>
    ''' 讓子頁面可以指定其父選單頁面（用於非直接顯示在選單上的頁面）
    ''' </summary>
    Public Property ParentMenuPage As String

    ''' <summary>
    ''' 群組名稱與 Icon 對應表
    ''' </summary>
    Private ReadOnly Property GroupIcons As Dictionary(Of String, String)
        Get
            Return New Dictionary(Of String, String) From {
            {"系統管理", "fa-solid fa-cogs"},
            {"資料管理", "fa-solid fa-database"},
            {"統計報表", "fa-solid fa-magnifying-glass-chart"}
        }
        End Get
    End Property

    Public modalStatic As String = ""
    Public Property isNeedStaticModal As Boolean
        Get
            Return ViewState("isNeedStaticModal")
        End Get
        Set(value As Boolean)
            ViewState("isNeedStaticModal") = value
        End Set
    End Property

#Region "Fun/Sub"
    ''' <summary>
    ''' 將路徑統一為以 "/" 開頭的相對路徑（支援虛擬目錄）
    ''' </summary>
    Private Function NormalizePath(path As String) As String
        If String.IsNullOrEmpty(path) Then Return ""

        ' 換斜線 + 去空白
        path = path.Replace("\", "/").Trim()

        ' 移除常見相對符號
        path = path.Replace("~/", "/").Replace("../", "/").Replace("./", "/")

        ' 移除虛擬目錄前綴
        Dim appPath As String = Request.ApplicationPath
        If Not String.IsNullOrEmpty(appPath) AndAlso appPath <> "/" Then
            If path.StartsWith(appPath, StringComparison.OrdinalIgnoreCase) Then
                path = path.Substring(appPath.Length)
            End If
        End If

        ' 移除固定層 /pages/
        If path.StartsWith("/Pages/", StringComparison.OrdinalIgnoreCase) Then
            path = path.Substring("/Pages".Length)
        End If

        ' 確保開頭為 "/"
        If Not path.StartsWith("/") Then
            path = "/" & path
        End If

        Return path.ToLower()
    End Function


    ''' <summary>
    ''' 清單動態組出選單 HTML
    ''' </summary>
    ''' <param name="menuList">使用者可用選單清單</param>
    ''' <returns>HTML 字串</returns>
    Private Function BuildMenuHTML_bak(menuList As List(Of stru_MenuItem)) As String
        Dim sb As New StringBuilder()

        ' 依群組分組
        Dim groups = menuList.Where(Function(x) x.isShow AndAlso x.canRead).GroupBy(Function(m) m.groupName).
                          OrderBy(Function(g) g.First().orderBy_group)

        For Each grp In groups
            Dim groupName = grp.Key
            Dim menuID = groupName.Replace(" ", "") & "Menu"

            ' 從字典取出群組 icon（沒有的話給預設）
            Dim groupIcon As String = If(GroupIcons.ContainsKey(groupName),
                                     GroupIcons(groupName),
                                     "fas fa-folder")

            sb.AppendLine("    <div class='nav-item'>")
            sb.AppendLine($"      <a class='nav-link' href='#' data-bs-toggle='collapse' data-bs-target='#{menuID}'>")
            sb.AppendLine($"        <i class='{groupIcon}'></i>{groupName}")
            sb.AppendLine("        <i class='fas fa-chevron-down float-end mt-1'></i>")
            sb.AppendLine("      </a>")
            sb.AppendLine($"      <div class='collapse show nav-submenu' id='{menuID}'>")

            ' 組子選單
            For Each item In grp.OrderBy(Function(x) x.orderBy_menu)
                Dim menuName = item.menuName
                Dim menuURL = item.menuURL
                Dim iconClass = If(String.IsNullOrEmpty(item.iconClass),
                               "fa-solid fa-angle-right",
                               item.iconClass)

                sb.AppendLine($"        <a class='nav-link' href='{menuURL}'>")
                sb.AppendLine($"          <i class='{iconClass}'></i>{menuName}")
                sb.AppendLine("        </a>")
            Next

            sb.AppendLine("      </div>")
            sb.AppendLine("    </div>")
        Next

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 清單動態組出選單 HTML
    ''' </summary>
    ''' <param name="menuList">使用者可用選單清單</param>
    ''' <param name="currentPagePath">當前頁面路徑</param>
    ''' <returns>HTML 字串</returns>
    Private Function BuildMenuHTML(menuList As List(Of stru_MenuItem), currentPagePath As String) As String
        Dim sb As New StringBuilder()

        ' 依群組分組
        Dim groups = menuList.Where(Function(x) x.isShow AndAlso x.canRead).GroupBy(Function(m) m.groupName).
                      OrderBy(Function(g) g.First().orderBy_group)

        For Each grp In groups
            Dim groupName = grp.Key
            Dim menuID = groupName.Replace(" ", "") & "Menu"

            ' 檢查群組內是否有 active 項目
            Dim hasActiveItem = grp.Any(Function(x) String.Equals(NormalizePath(x.menuURL), currentPagePath, StringComparison.OrdinalIgnoreCase))

            ' 從字典取出群組 icon（沒有的話給預設）
            Dim groupIcon As String = If(GroupIcons.ContainsKey(groupName),
                                 GroupIcons(groupName),
                                 "fas fa-folder")

            sb.AppendLine("    <div class='nav-item'>")
            sb.AppendLine($"      <a class='nav-link' href='#' data-bs-toggle='collapse' data-bs-target='#{menuID}'>")
            sb.AppendLine($"        <i class='{groupIcon}'></i>{groupName}")
            sb.AppendLine("        <i class='fas fa-chevron-down float-end mt-1'></i>")
            sb.AppendLine("      </a>")

            ' 如果群組內有 active 項目，預設展開
            Dim collapseClass = If(hasActiveItem, "collapse show nav-submenu", "collapse nav-submenu")
            sb.AppendLine($"      <div class='{collapseClass}' id='{menuID}'>")

            ' 組子選單
            For Each item In grp.OrderBy(Function(x) x.orderBy_menu)
                Dim menuName = item.menuName
                Dim menuURL = item.menuURL
                Dim iconClass = If(String.IsNullOrEmpty(item.iconClass),
                           "fa-solid fa-angle-right",
                           item.iconClass)

                ' 判斷是否為當前頁面
                Dim isActive = String.Equals(NormalizePath(menuURL), currentPagePath, StringComparison.OrdinalIgnoreCase)
                Dim activeClass = If(isActive, " active", "")

                sb.AppendLine($"        <a class='nav-link{activeClass}' href='{menuURL}'>")
                sb.AppendLine($"          <i class='{iconClass}'></i>{menuName}")
                sb.AppendLine("        </a>")
            Next

            sb.AppendLine("      </div>")
            sb.AppendLine("    </div>")
        Next

        Return sb.ToString()
    End Function

#End Region

    Private Sub mp_default_Init(sender As Object, e As EventArgs) Handles Me.Init
        '若檢查沒有正確的Session，直接返回登入畫面
        Dim userInfo As taifCattle.Base.stru_LoginUserInfo = Session("userInfo")
        If userInfo.isExist = False Then
            Response.Redirect("~/Login.aspx")
        End If

        '權限檢查
        Dim currentPage As String = NormalizePath(Request.Path)
        Dim ignorePages As String() = {}
        If Not ignorePages.Contains(currentPage, StringComparer.OrdinalIgnoreCase) Then

            Dim hasPermission As Boolean =
            userInfo.liMenu.Any(Function(m) _
                m.canRead AndAlso String.Equals(NormalizePath(m.menuURL), currentPage, StringComparison.OrdinalIgnoreCase))

            If Not hasPermission Then
                'Response.Redirect("~/Login.aspx")
                'Exit Sub
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim userInfo As taifCattle.Base.stru_LoginUserInfo = Session("userInfo")
        If IsPostBack = False Then
            LinkButton_userName.Text = userInfo.name

            '組成目錄
            Dim currentPage As String = NormalizePath(Request.Path)
            If String.IsNullOrEmpty(menuStr_def) Then
                '決定要HL哪個選單
                Dim activeMenuPage As String = If(String.IsNullOrEmpty(ParentMenuPage), currentPage, NormalizePath(ParentMenuPage))
                menuStr_def = BuildMenuHTML(userInfo.liMenu, activeMenuPage)
            End If

            '處理燈箱
            Select Case isNeedStaticModal
                Case True
                    modalStatic = " data-bs-backdrop='static' data-bs-keyboard='false' "
                    Panel_closeModal.Visible = False
                    Panel_closeModal_img.Visible = False
                Case False
                    modalStatic = ""
                    Panel_closeModal.Visible = True
                    Panel_closeModal_img.Visible = True
            End Select
        End If
    End Sub

    Private Sub LinkButton_logout_Click(sender As Object, e As EventArgs) Handles LinkButton_logout.Click
        Dim userInfo As taifCattle.Base.stru_LoginUserInfo = Session("userInfo")
        taifCattle_base.Insert_UserLog(userInfo.accountID, taifCattle.Base.enum_UserLogItem.登出, taifCattle.Base.enum_UserLogType.其他)
        taifCattle_base.Logout()
        Response.Redirect("~/Login.aspx")
    End Sub


End Class