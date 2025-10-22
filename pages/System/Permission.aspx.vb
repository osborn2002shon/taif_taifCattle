Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Web.UI.WebControls

Public Class Permission
    Inherits taifCattle.Base

    Private ReadOnly taifCattleControl As New taifCattle.Control

    Private Class MenuPermissionModel
        Public Property MenuID As Integer
        Public Property GroupName As String
        Public Property MenuName As String
        Public Property MenuURL As String
        Public Property OrderByGroup As Integer
        Public Property OrderByMenu As Integer
        Public Property CanCreate As Boolean
        Public Property CanRead As Boolean
        Public Property CanUpdate As Boolean
        Public Property CanDelete As Boolean
    End Class

    Private Class MenuGroupModel
        Public Property GroupName As String
        Public Property OrderByGroup As Integer
        Public Property Menus As List(Of MenuPermissionModel)
    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            InitializeRoleDropdown()
            Panel_permission.Visible = False
            UpdateStatusMessage("請選擇要設定的系統權限角色。")
            ResetResultMessage()
        End If
    End Sub

    Private Sub InitializeRoleDropdown()
        taifCattleControl.BindDropDownList_userRole(DropDownList_role, False, True)
        DropDownList_role.Items.Insert(0, New ListItem("請選擇權限角色", String.Empty))
        DropDownList_role.SelectedIndex = 0
    End Sub

    Private Sub UpdateStatusMessage(message As String)
        Label_status.Text = message
    End Sub

    Private Sub ResetResultMessage()
        ShowResultMessage(String.Empty, False)
    End Sub

    Private Sub ShowResultMessage(message As String, isError As Boolean)
        Dim baseClass As String = "me-auto text-start d-inline-block"
        If String.IsNullOrWhiteSpace(message) Then
            Label_result.Text = String.Empty
            Label_result.CssClass = baseClass
        Else
            Dim cssStatus As String = If(isError, " text-danger", " text-success")
            Label_result.Text = message
            Label_result.CssClass = baseClass & " fw-semibold" & cssStatus
        End If
    End Sub

    Protected Sub DropDownList_role_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_role.SelectedIndexChanged
        ResetResultMessage()

        If String.IsNullOrEmpty(DropDownList_role.SelectedValue) Then
            Panel_permission.Visible = False
            UpdateStatusMessage("請選擇要設定的系統權限角色。")
            Return
        End If

        BindPermissionGroups()
    End Sub

    Private Sub BindPermissionGroups()
        Dim roleID As Integer
        If Not Integer.TryParse(DropDownList_role.SelectedValue, roleID) Then
            Panel_permission.Visible = False
            UpdateStatusMessage("權限代碼格式不正確。")
            Return
        End If

        Dim groups = LoadMenuGroups(roleID)

        If groups.Count = 0 Then
            Panel_permission.Visible = False
            UpdateStatusMessage("查無可設定的功能選項。")
            Return
        End If

        Panel_permission.Visible = True
        Literal_roleName.Text = DropDownList_role.SelectedItem.Text
        Repeater_groups.DataSource = groups.OrderBy(Function(g) g.OrderByGroup)
        Repeater_groups.DataBind()

        Dim totalMenus = groups.Sum(Function(g) g.Menus.Count)
        UpdateStatusMessage($"共有 {totalMenus} 項功能可進行權限設定。")
    End Sub

    Private Function LoadMenuGroups(auTypeID As Integer) As List(Of MenuGroupModel)
        Dim sqlString As String =
            "SELECT m.menuID, m.groupName, m.menuName, m.menuURL, m.orderBy_group, m.orderBy_menu, " &
            "       ISNULL(ma.canCreate, 0) AS canCreate, " &
            "       ISNULL(ma.canRead, 0) AS canRead, " &
            "       ISNULL(ma.canUpdate, 0) AS canUpdate, " &
            "       ISNULL(ma.canDelete, 0) AS canDelete " &
            "FROM System_Menu AS m " &
            "LEFT JOIN System_MenuAu AS ma ON ma.menuID = m.menuID AND ma.auTypeID = @auTypeID " &
            "WHERE m.isActive = 1 " &
            "ORDER BY m.orderBy_group, m.orderBy_menu"

        Dim para As New Data.SqlClient.SqlParameter("@auTypeID", auTypeID)

        Dim menuGroups As New List(Of MenuGroupModel)

        Using da As New DataAccess.MS_SQL
            Dim dt As DataTable = da.GetDataTable(sqlString, para)

            Dim menuItems = (From row In dt.AsEnumerable()
                             Select New MenuPermissionModel With {
                                 .MenuID = row.Field(Of Integer)("menuID"),
                                 .GroupName = row.Field(Of String)("groupName"),
                                 .MenuName = row.Field(Of String)("menuName"),
                                 .MenuURL = row.Field(Of String)("menuURL"),
                                 .OrderByGroup = row.Field(Of Integer)("orderBy_group"),
                                 .OrderByMenu = row.Field(Of Integer)("orderBy_menu"),
                                 .CanCreate = row.Field(Of Boolean)("canCreate"),
                                 .CanRead = row.Field(Of Boolean)("canRead"),
                                 .CanUpdate = row.Field(Of Boolean)("canUpdate"),
                                 .CanDelete = row.Field(Of Boolean)("canDelete")
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

    Protected Sub Repeater_groups_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        If e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem Then
            Return
        End If

        Dim group = TryCast(e.Item.DataItem, MenuGroupModel)
        Dim repeaterMenus = TryCast(e.Item.FindControl("Repeater_menus"), Repeater)
        If group IsNot Nothing AndAlso repeaterMenus IsNot Nothing Then
            repeaterMenus.DataSource = group.Menus
            repeaterMenus.DataBind()
        End If
    End Sub

    Protected Sub Button_reload_Click(sender As Object, e As EventArgs) Handles Button_reload.Click
        ResetResultMessage()

        If String.IsNullOrEmpty(DropDownList_role.SelectedValue) Then
            Panel_permission.Visible = False
            UpdateStatusMessage("請先選擇要重新載入的系統權限角色。")
            Return
        End If

        BindPermissionGroups()
        ShowResultMessage("已重新載入最新的權限設定。", False)
    End Sub

    Protected Sub Button_save_Click(sender As Object, e As EventArgs) Handles Button_save.Click
        ResetResultMessage()

        If String.IsNullOrEmpty(DropDownList_role.SelectedValue) Then
            Panel_permission.Visible = False
            UpdateStatusMessage("請先選擇要設定的系統權限角色。")
            ShowResultMessage("尚未選擇任何角色。", True)
            Return
        End If

        Dim roleID As Integer
        If Not Integer.TryParse(DropDownList_role.SelectedValue, roleID) Then
            ShowResultMessage("角色代碼格式不正確。", True)
            Return
        End If

        Dim selectedPermissions As List(Of MenuPermissionModel) = CollectPermissionsFromUI()

        Using da As New DataAccess.MS_SQL
            da.StartTransaction()
            Try
                Dim paramRole = New Data.SqlClient.SqlParameter("@auTypeID", roleID)
                da.ExecNonQuery("DELETE FROM System_MenuAu WHERE auTypeID = @auTypeID", paramRole)

                For Each permission In selectedPermissions
                    Dim effectiveRead As Boolean = permission.CanRead OrElse permission.CanCreate OrElse permission.CanUpdate OrElse permission.CanDelete
                    Dim effectiveCreate As Boolean = permission.CanCreate
                    Dim effectiveUpdate As Boolean = permission.CanUpdate
                    Dim effectiveDelete As Boolean = permission.CanDelete

                    If Not (effectiveRead OrElse effectiveCreate OrElse effectiveUpdate OrElse effectiveDelete) Then
                        Continue For
                    End If

                    Dim parameters As New List(Of Data.SqlClient.SqlParameter) From {
                        New Data.SqlClient.SqlParameter("@auTypeID", roleID),
                        New Data.SqlClient.SqlParameter("@menuID", permission.MenuID),
                        New Data.SqlClient.SqlParameter("@canCreate", If(effectiveCreate, 1, 0)),
                        New Data.SqlClient.SqlParameter("@canRead", If(effectiveRead, 1, 0)),
                        New Data.SqlClient.SqlParameter("@canUpdate", If(effectiveUpdate, 1, 0)),
                        New Data.SqlClient.SqlParameter("@canDelete", If(effectiveDelete, 1, 0))
                    }

                    da.ExecNonQuery("INSERT INTO System_MenuAu (auTypeID, menuID, canCreate, canRead, canUpdate, canDelete) VALUES (@auTypeID, @menuID, @canCreate, @canRead, @canUpdate, @canDelete)", parameters.ToArray())
                Next

                da.Commit()
                ShowResultMessage("權限設定已成功儲存。", False)
            Catch ex As Exception
                da.RollBack()
                ShowResultMessage("儲存失敗：" & ex.Message, True)
            End Try
        End Using

        BindPermissionGroups()
    End Sub

    Private Function CollectPermissionsFromUI() As List(Of MenuPermissionModel)
        Dim permissions As New List(Of MenuPermissionModel)

        For Each groupItem As RepeaterItem In Repeater_groups.Items
            Dim repeaterMenus = TryCast(groupItem.FindControl("Repeater_menus"), Repeater)
            If repeaterMenus Is Nothing Then
                Continue For
            End If

            For Each menuItem As RepeaterItem In repeaterMenus.Items
                Dim hiddenMenuID = TryCast(menuItem.FindControl("HiddenField_menuID"), HiddenField)
                Dim checkRead = TryCast(menuItem.FindControl("CheckBox_read"), CheckBox)
                Dim checkCreate = TryCast(menuItem.FindControl("CheckBox_create"), CheckBox)
                Dim checkUpdate = TryCast(menuItem.FindControl("CheckBox_update"), CheckBox)
                Dim checkDelete = TryCast(menuItem.FindControl("CheckBox_delete"), CheckBox)

                If hiddenMenuID Is Nothing OrElse String.IsNullOrEmpty(hiddenMenuID.Value) Then
                    Continue For
                End If

                Dim menuID As Integer
                If Not Integer.TryParse(hiddenMenuID.Value, menuID) Then
                    Continue For
                End If

                permissions.Add(New MenuPermissionModel With {
                    .MenuID = menuID,
                    .CanRead = If(checkRead IsNot Nothing AndAlso checkRead.Checked, True, False),
                    .CanCreate = If(checkCreate IsNot Nothing AndAlso checkCreate.Checked, True, False),
                    .CanUpdate = If(checkUpdate IsNot Nothing AndAlso checkUpdate.Checked, True, False),
                    .CanDelete = If(checkDelete IsNot Nothing AndAlso checkDelete.Checked, True, False)
                })
            Next
        Next

        Return permissions
    End Function

End Class
