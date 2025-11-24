Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Web.UI.WebControls

Public Class Permission
    Inherits taifCattle.Base
    Public js As New StringBuilder
    Private Class RoleModel
        Public Property AuTypeID As Integer
        Public Property AuTypeName As String
    End Class

    Private Class RolePermissionModel
        Public Property AuTypeID As Integer
        Public Property AuTypeName As String
        Public Property IsEnabled As Boolean
    End Class

    Private Class MenuPermissionModel
        Public Property MenuID As Integer
        Public Property GroupName As String
        Public Property MenuName As String
        Public Property MenuURL As String
        Public Property OrderByGroup As Integer
        Public Property OrderByMenu As Integer
        Public Property Roles As List(Of RolePermissionModel)
    End Class

    Private Class MenuGroupModel
        Public Property GroupName As String
        Public Property OrderByGroup As Integer
        Public Property Menus As List(Of MenuPermissionModel)
    End Class

    Private Class MenuRoleSelectionModel
        Public Property MenuID As Integer
        Public Property AuTypeID As Integer
        Public Property IsEnabled As Boolean
    End Class

    Private Class MenuInfo
        Public Property MenuID As Integer
        Public Property MenuName As String
    End Class

    Private _roleCache As List(Of RoleModel)

    Private Sub Permission_Init(sender As Object, e As EventArgs) Handles Me.Init
        '指定父選單頁面
        Dim masterPage As mp_default = TryCast(Me.Master, mp_default)
        If masterPage IsNot Nothing Then
            masterPage.ParentMenuPage = "/System/AccountManage.aspx"
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Label_result.Text =""
            BindPermissionGroups()
        End If
    End Sub

    Private Sub ShowMessage(message As String)
        If String.IsNullOrWhiteSpace(message) Then
            Label_result.Text = ""

        Else
            Label_result.Text = message
        End If
        js.AppendLine("showModal();")
    End Sub

    Private Sub BindPermissionGroups()
        Dim roleList = GetRoleList()

        If roleList.Count = 0 Then
            Panel_permission.Visible = False
            ShowMessage("查無系統權限角色，請先建立角色。")
            Return
        End If

        Dim groups = LoadMenuGroups(roleList)

        If groups.Count = 0 Then
            Panel_permission.Visible = False
            ShowMessage("查無可設定的功能選項。")
            Return
        End If

        Panel_permission.Visible = True
        Repeater_groups.DataSource = groups.OrderBy(Function(g) g.OrderByGroup)
        Repeater_groups.DataBind()

        Dim totalMenus = groups.Sum(Function(g) g.Menus.Count)
        Label_status.Text = $"共有 {totalMenus} 項功能，{roleList.Count} 種角色可設定是否使用。"
    End Sub

    Private Function LoadMenuGroups(roleList As List(Of RoleModel)) As List(Of MenuGroupModel)
        Dim menuGroups As New List(Of MenuGroupModel)

        Dim sqlMenus As String =
            "SELECT menuID, groupName, menuName, menuURL, orderBy_group, orderBy_menu " &
            "FROM System_Menu WHERE isActive = 1 AND isShow = 1 ORDER BY orderBy_group, orderBy_menu"

        Dim sqlMenuAu As String = "SELECT auTypeID, menuID FROM System_MenuAu"

        Using da As New DataAccess.MS_SQL
            Dim dtMenus As DataTable = da.GetDataTable(sqlMenus)
            Dim dtMenuAu As DataTable = da.GetDataTable(sqlMenuAu)

            Dim assignedSet As New HashSet(Of String)(StringComparer.Ordinal)

            For Each row As DataRow In dtMenuAu.Rows
                Dim menuID = row.Field(Of Integer)("menuID")
                Dim auTypeID = row.Field(Of Integer)("auTypeID")
                assignedSet.Add($"{menuID}_{auTypeID}")
            Next

            Dim menuItems = (From row In dtMenus.AsEnumerable()
                             Let menuID = row.Field(Of Integer)("menuID")
                             Select New MenuPermissionModel With {
                                 .MenuID = menuID,
                                 .GroupName = row.Field(Of String)("groupName"),
                                 .MenuName = row.Field(Of String)("menuName"),
                                 .MenuURL = row.Field(Of String)("menuURL"),
                                 .OrderByGroup = row.Field(Of Integer)("orderBy_group"),
                                 .OrderByMenu = row.Field(Of Integer)("orderBy_menu"),
                                 .Roles = roleList.Select(Function(role) New RolePermissionModel With {
                                     .AuTypeID = role.AuTypeID,
                                     .AuTypeName = role.AuTypeName,
                                     .IsEnabled = assignedSet.Contains($"{menuID}_{role.AuTypeID}")
                                 }).ToList()
                             }).ToList()

            menuGroups = (From item In menuItems
                           Group item By item.GroupName, item.OrderByGroup Into Items = Group
                           Select New MenuGroupModel With {
                               .GroupName = GroupName,
                               .OrderByGroup = OrderByGroup,
                               .Menus = Items.OrderBy(Function(x) x.OrderByMenu).ToList()
                           }).OrderBy(Function(g) g.OrderByGroup).ToList()
        End Using

        Return menuGroups
    End Function

    Private Function GetRoleList() As List(Of RoleModel)
        If _roleCache IsNot Nothing Then
            Return _roleCache
        End If

        Dim roles As New List(Of RoleModel)
        Dim sqlString As String = "SELECT auTypeID, auTypeName FROM System_UserAuType ORDER BY auTypeID"

        Using da As New DataAccess.MS_SQL
            Dim dt As DataTable = da.GetDataTable(sqlString)
            roles = (From row In dt.AsEnumerable()
                     Select New RoleModel With {
                         .AuTypeID = row.Field(Of Integer)("auTypeID"),
                         .AuTypeName = row.Field(Of String)("auTypeName")
                     }).ToList()
        End Using

        _roleCache = roles
        Return roles
    End Function

    Private Function LoadAllMenuInfos() As List(Of MenuInfo)
        Dim menus As New List(Of MenuInfo)
        Dim sqlString As String = "SELECT menuID, menuName FROM System_Menu WHERE isActive = 1"

        Using da As New DataAccess.MS_SQL
            Dim dt As DataTable = da.GetDataTable(sqlString)
            menus = (From row In dt.AsEnumerable()
                     Select New MenuInfo With {
                         .MenuID = row.Field(Of Integer)("menuID"),
                         .MenuName = row.Field(Of String)("menuName")
                     }).ToList()
        End Using

        Return menus
    End Function

    Protected Sub Repeater_groups_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        If e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem Then
            Return
        End If

        Dim headerRepeater = TryCast(e.Item.FindControl("Repeater_roleHeader"), Repeater)
        If headerRepeater IsNot Nothing Then
            headerRepeater.DataSource = GetRoleList()
            headerRepeater.DataBind()
        End If

        Dim group = TryCast(e.Item.DataItem, MenuGroupModel)
        Dim repeaterMenus = TryCast(e.Item.FindControl("Repeater_menus"), Repeater)
        If group IsNot Nothing AndAlso repeaterMenus IsNot Nothing Then
            repeaterMenus.DataSource = group.Menus
            repeaterMenus.DataBind()
        End If
    End Sub

    Protected Sub Repeater_menus_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        If e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem Then
            Return
        End If

        Dim menu = TryCast(e.Item.DataItem, MenuPermissionModel)
        Dim roleRepeater = TryCast(e.Item.FindControl("Repeater_rolePermissions"), Repeater)
        If menu IsNot Nothing AndAlso roleRepeater IsNot Nothing Then
            roleRepeater.DataSource = menu.Roles
            roleRepeater.DataBind()
        End If
    End Sub

    Protected Sub Button_reload_Click(sender As Object, e As EventArgs) Handles Button_reload.Click
        _roleCache = Nothing
        BindPermissionGroups()
        ShowMessage("已重新載入最新的權限設定。")
    End Sub

    Protected Sub Button_save_Click(sender As Object, e As EventArgs) Handles Button_save.Click
        Dim selections As List(Of MenuRoleSelectionModel) = CollectPermissionsFromUI()
        Dim allMenus = LoadAllMenuInfos()
        Dim menuLookup As Dictionary(Of Integer, String) = allMenus.ToDictionary(Function(m) m.MenuID, Function(m) m.MenuName)

        Using da As New DataAccess.MS_SQL
            da.StartTransaction()
            Try
                Dim preservedKeys As New HashSet(Of String)(StringComparer.Ordinal)

                Dim sqlHiddenMenuAu As String =
                    "SELECT mau.auTypeID, mau.menuID FROM System_MenuAu AS mau " &
                    "INNER JOIN System_Menu AS m ON m.menuID = mau.menuID " &
                    "WHERE ISNULL(m.isShow, 0) = 0"

                Dim dtHiddenMenuAu As DataTable = da.GetDataTable(sqlHiddenMenuAu)
                For Each row As DataRow In dtHiddenMenuAu.Rows
                    Dim menuID = row.Field(Of Integer)("menuID")
                    Dim auTypeID = row.Field(Of Integer)("auTypeID")
                    preservedKeys.Add($"{menuID}_{auTypeID}")
                Next

                da.ExecNonQuery("DELETE FROM System_MenuAu WHERE menuID IN (SELECT menuID FROM System_Menu WHERE ISNULL(isShow, 0) = 1)")

                Dim insertedKeys As New HashSet(Of String)(preservedKeys, StringComparer.Ordinal)

                For Each selection In selections
                    If Not selection.IsEnabled Then
                        Continue For
                    End If

                    Dim menuName As String = Nothing
                    If Not menuLookup.TryGetValue(selection.MenuID, menuName) Then
                        Continue For
                    End If

                    If String.IsNullOrWhiteSpace(menuName) Then
                        Continue For
                    End If

                    Dim menuPrefix As String = menuName.Trim()
                    If String.IsNullOrEmpty(menuPrefix) Then
                        Continue For
                    End If

                    Dim relatedMenuIDs = allMenus.
                        Where(Function(m) Not String.IsNullOrEmpty(m.MenuName) AndAlso m.MenuName.Trim().StartsWith(menuPrefix, StringComparison.Ordinal)).
                        Select(Function(m) m.MenuID).
                        ToList()

                    For Each relatedMenuID In relatedMenuIDs
                        Dim key = $"{relatedMenuID}_{selection.AuTypeID}"
                        If Not insertedKeys.Add(key) Then
                            Continue For
                        End If

                        Dim parameters As Data.SqlClient.SqlParameter() = {
                            New Data.SqlClient.SqlParameter("@auTypeID", selection.AuTypeID),
                            New Data.SqlClient.SqlParameter("@menuID", relatedMenuID)
                        }

                        da.ExecNonQuery("INSERT INTO System_MenuAu (auTypeID, menuID) VALUES (@auTypeID, @menuID)", parameters)
                    Next
                Next

                da.Commit()
                ShowMessage("權限設定已成功儲存。")
            Catch ex As Exception
                da.RollBack()
                ShowMessage("儲存失敗：" & ex.Message)
            End Try
        End Using
        BindPermissionGroups()
    End Sub

    Private Function CollectPermissionsFromUI() As List(Of MenuRoleSelectionModel)
        Dim selections As New List(Of MenuRoleSelectionModel)

        For Each groupItem As RepeaterItem In Repeater_groups.Items
            Dim repeaterMenus = TryCast(groupItem.FindControl("Repeater_menus"), Repeater)
            If repeaterMenus Is Nothing Then
                Continue For
            End If

            For Each menuItem As RepeaterItem In repeaterMenus.Items
                Dim hiddenMenuID = TryCast(menuItem.FindControl("HiddenField_menuID"), HiddenField)
                Dim roleRepeater = TryCast(menuItem.FindControl("Repeater_rolePermissions"), Repeater)

                If hiddenMenuID Is Nothing OrElse String.IsNullOrEmpty(hiddenMenuID.Value) Then
                    Continue For
                End If

                Dim menuID As Integer
                If Not Integer.TryParse(hiddenMenuID.Value, menuID) Then
                    Continue For
                End If

                If roleRepeater Is Nothing Then
                    Continue For
                End If

                For Each roleItem As RepeaterItem In roleRepeater.Items
                    Dim hiddenRoleID = TryCast(roleItem.FindControl("HiddenField_roleID"), HiddenField)
                    Dim checkEnabled = TryCast(roleItem.FindControl("CheckBox_enabled"), CheckBox)

                    If hiddenRoleID Is Nothing OrElse String.IsNullOrEmpty(hiddenRoleID.Value) Then
                        Continue For
                    End If

                    Dim auTypeID As Integer
                    If Not Integer.TryParse(hiddenRoleID.Value, auTypeID) Then
                        Continue For
                    End If

                    selections.Add(New MenuRoleSelectionModel With {
                        .MenuID = menuID,
                        .AuTypeID = auTypeID,
                        .IsEnabled = (checkEnabled IsNot Nothing AndAlso checkEnabled.Checked)
                    })
                Next
            Next
        Next

        Return selections
    End Function

    Private Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Page.ClientScript.RegisterStartupScript(Me.Page.GetType(), "page_js", js.ToString(), True)
    End Sub

End Class
