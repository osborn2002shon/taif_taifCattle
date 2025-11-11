Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.UI.WebControls
Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel

Public Class AccountManage
    Inherits taifCattle.Base
    Public js As New StringBuilder

    Private ReadOnly taifCattle_con As New taifCattle.Control
    Private ReadOnly taifCattle_account As New taifCattle.Account
    Private ReadOnly taifCattle_mail As New taifCattle.Mail

    Private Sub ShowEditorView()
        Panel_query.Visible = False
        Panel_editor.Visible = True
    End Sub

    Private Sub ShowQueryView()
        Panel_query.Visible = True
        Panel_editor.Visible = False
    End Sub

    Private Sub UpdateCitySelectorVisibility()
        Dim isGovUser As Boolean = (DropDownList_editRole.SelectedValue = "3")
        Panel_citySelector.Visible = isGovUser
        If Not isGovUser Then
            If DropDownList_editCity.Items.Count > 0 Then
                DropDownList_editCity.SelectedIndex = 0
            End If
        End If
    End Sub

    Private Property Property_Query_Status As String
        Get
            If ViewState("Property_Query_Status") Is Nothing Then
                Return String.Empty
            End If
            Return ViewState("Property_Query_Status").ToString()
        End Get
        Set(value As String)
            ViewState("Property_Query_Status") = value
        End Set
    End Property

    Private Property Property_Query_RoleID As String
        Get
            If ViewState("Property_Query_RoleID") Is Nothing Then
                Return String.Empty
            End If
            Return ViewState("Property_Query_RoleID").ToString()
        End Get
        Set(value As String)
            ViewState("Property_Query_RoleID") = value
        End Set
    End Property

    Private Property Property_Query_Keyword As String
        Get
            If ViewState("Property_Query_Keyword") Is Nothing Then
                Return String.Empty
            End If
            Return ViewState("Property_Query_Keyword").ToString()
        End Get
        Set(value As String)
            ViewState("Property_Query_Keyword") = value
        End Set
    End Property

    Private Property Property_EditMode As taifCattle.Base.enum_EditMode
        Get
            If ViewState("Property_EditMode") Is Nothing Then
                Return taifCattle.Base.enum_EditMode.預設
            End If
            Return CType(ViewState("Property_EditMode"), taifCattle.Base.enum_EditMode)
        End Get
        Set(value As taifCattle.Base.enum_EditMode)
            ViewState("Property_EditMode") = value
        End Set
    End Property

    Private Property Property_EditAccountID As Integer
        Get
            If ViewState("Property_EditAccountID") Is Nothing Then
                Return -1
            End If
            Return Convert.ToInt32(ViewState("Property_EditAccountID"))
        End Get
        Set(value As Integer)
            ViewState("Property_EditAccountID") = value
        End Set
    End Property

    Private Property Property_EditIsVerified As Boolean
        Get
            If ViewState("Property_EditIsVerified") Is Nothing Then
                Return False
            End If
            Return Convert.ToBoolean(ViewState("Property_EditIsVerified"))
        End Get
        Set(value As Boolean)
            ViewState("Property_EditIsVerified") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindStatusDropdown()
            taifCattle_con.BindDropDownList_userRole(DropDownList_role, True)
            taifCattle_con.BindDropDownList_userRole(DropDownList_editRole, False, True)
            DropDownList_editRole.Items.Insert(0, New ListItem("請選擇系統權限", String.Empty))
            taifCattle_con.BindDropDownList_city(DropDownList_editCity, False)
            DropDownList_editCity.Items.Insert(0, New ListItem("請選擇縣市", String.Empty))

            Property_Query_Status = String.Empty
            Property_Query_RoleID = String.Empty
            Property_Query_Keyword = String.Empty
            ApplyQueryToControls()
            BindGridView()
        End If
    End Sub

    Private Sub BindStatusDropdown()
        DropDownList_status.Items.Clear()
        DropDownList_status.Items.Add(New ListItem("全部狀態", String.Empty))
        DropDownList_status.Items.Add(New ListItem("啟用", "active"))
        DropDownList_status.Items.Add(New ListItem("停用", "inactive"))
        DropDownList_status.Items.Add(New ListItem("待驗證", "pending"))
    End Sub

    Private Sub ApplyQueryToControls()
        If DropDownList_status.Items.FindByValue(Property_Query_Status) IsNot Nothing Then
            DropDownList_status.SelectedValue = Property_Query_Status
        End If
        If DropDownList_role.Items.FindByValue(Property_Query_RoleID) IsNot Nothing Then
            DropDownList_role.SelectedValue = Property_Query_RoleID
        End If
        TextBox_keyword.Text = Property_Query_Keyword
    End Sub

    Private Sub SaveQueryCondition()
        Property_Query_Status = DropDownList_status.SelectedValue
        Property_Query_RoleID = DropDownList_role.SelectedValue
        Property_Query_Keyword = TextBox_keyword.Text.Trim()
    End Sub

    Private Sub BindGridView()
        ApplyQueryToControls()
        Dim currentUser = InfoSession
        If currentUser.isExist Then
            taifCattle_account.DeactivateDormantAccounts(Now.AddDays(-180), currentUser.accountID)
        End If

        Dim roleIDNullable As Integer? = Nothing
        If Not String.IsNullOrEmpty(Property_Query_RoleID) Then
            Dim roleValue As Integer
            If Integer.TryParse(Property_Query_RoleID, roleValue) Then
                roleIDNullable = roleValue
            End If
        End If

        Dim dtAccounts As DataTable = taifCattle_account.GetSystemAccounts(Property_Query_Status, roleIDNullable, Property_Query_Keyword)
        GridView_accounts.DataSource = dtAccounts
        GridView_accounts.DataBind()
        Label_recordCount.Text = dtAccounts.Rows.Count.ToString("N0")
    End Sub

    Private Sub ShowMessage(message As String)
        If String.IsNullOrWhiteSpace(message) Then
            Label_message.Text = String.Empty
        Else
            Label_message.Text = message
        End If
    End Sub

    'Private Sub ShowFormMessage(message As String, isError As Boolean)
    '    If String.IsNullOrWhiteSpace(message) Then
    '        Label_formMessage.Text = String.Empty
    '        Label_formMessage.CssClass = "d-block fw-bold mb-3"
    '    Else
    '        Label_formMessage.Text = message
    '        Dim css As String = If(isError, "text-danger", "text-success")
    '        Label_formMessage.CssClass = "d-block fw-bold mb-3 " & css
    '    End If
    'End Sub

    Private Sub ResetEditor(Optional ByVal cleanMessage As Boolean = False)
        TextBox_account.Text = String.Empty
        TextBox_account.Enabled = True
        TextBox_name.Text = String.Empty
        DropDownList_editRole.SelectedIndex = 0
        TextBox_mobile.Text = String.Empty
        TextBox_unit.Text = String.Empty
        TextBox_memo.Text = String.Empty
        CheckBox_isActive.Checked = True
        CheckBox_isActive.Visible = False
        Label_isActive.Visible = False
        If cleanMessage Then
            ShowMessage(String.Empty)
        End If

        HiddenField_editAccountID.Value = String.Empty
        If DropDownList_editCity.Items.Count > 0 Then
            DropDownList_editCity.SelectedIndex = 0
        End If
        Panel_citySelector.Visible = False
    End Sub

    Private Sub OpenAddEditor()
        Property_EditMode = taifCattle.Base.enum_EditMode.新增
        Property_EditAccountID = -1
        Property_EditIsVerified = False

        ResetEditor()
        ShowEditorView()
        Label_message.Text = ""
        UpdateCitySelectorVisibility()
    End Sub

    Private Sub OpenEditEditor(accountID As Integer)
        Dim row As DataRow = taifCattle_account.GetSystemAccount(accountID)
        If row Is Nothing Then
            ShowMessage("找不到指定的帳號資料。")
            Exit Sub
        End If

        Property_EditMode = taifCattle.Base.enum_EditMode.編輯
        Property_EditAccountID = accountID
        Property_EditIsVerified = Convert.ToBoolean(row("isEmailVerified"))

        ResetEditor(True)
        ShowEditorView()

        HiddenField_editAccountID.Value = accountID.ToString()
        TextBox_account.Text = row("account").ToString()
        TextBox_account.Enabled = Not Property_EditIsVerified
        TextBox_name.Text = row("name").ToString()
        Dim roleID As String = row("auTypeID").ToString()
        If DropDownList_editRole.Items.FindByValue(roleID) IsNot Nothing Then
            DropDownList_editRole.SelectedValue = roleID
        End If

        Dim cityValue As String = String.Empty
        If row.Table.Columns.Contains("govID") AndAlso row("govID") IsNot DBNull.Value Then
            cityValue = row("govID").ToString()
        End If
        If Not String.IsNullOrEmpty(cityValue) AndAlso DropDownList_editCity.Items.FindByValue(cityValue) IsNot Nothing Then
            DropDownList_editCity.SelectedValue = cityValue
        End If

        TextBox_mobile.Text = If(row("mobile") Is DBNull.Value, String.Empty, row("mobile").ToString())
        TextBox_unit.Text = If(row("unit") Is DBNull.Value, String.Empty, row("unit").ToString())
        TextBox_memo.Text = If(row("memo") Is DBNull.Value, String.Empty, row("memo").ToString())

        If Property_EditIsVerified Then
            CheckBox_isActive.Visible = True
            Label_isActive.Visible = True
            CheckBox_isActive.Checked = Convert.ToBoolean(row("isActive"))
        Else
            CheckBox_isActive.Visible = False
            Label_isActive.Visible = False
        End If

        UpdateCitySelectorVisibility()
    End Sub

    Private Function ValidateAccountUniqueness(account As String, excludeAccountID As Integer) As Boolean
        Dim existRow = taifCattle_account.GetSystemAccountByAccount(account)
        If existRow Is Nothing Then
            Return True
        End If
        Dim existID As Integer = Convert.ToInt32(existRow("accountID"))
        Return existID = excludeAccountID
    End Function

    Private Function IsValidEmailFormat(email As String) As Boolean
        If String.IsNullOrWhiteSpace(email) Then
            Return False
        End If

        Dim pattern As String = "^[^@\s]+@[^@\s]+\.[^@\s]+$"
        Return Regex.IsMatch(email, pattern)
    End Function

    Private Function GenerateLoginUrl() As String
        Dim baseUrl = Request.Url.GetLeftPart(UriPartial.Authority)
        Dim relative = ResolveUrl("~/Login.aspx")
        Return baseUrl & relative
    End Function

    Private Function SendAccountNotification(userName As String, accountEmail As String, extraEmail As String, password As String, isNew As Boolean) As Boolean
        Dim recipients As New List(Of taifCattle.Mail.stru_mailAddress)
        Dim displayName As String = If(String.IsNullOrWhiteSpace(userName), accountEmail, userName)
        recipients.Add(New taifCattle.Mail.stru_mailAddress With {.name = displayName, .address = accountEmail})

        If Not String.IsNullOrWhiteSpace(extraEmail) AndAlso Not String.Equals(extraEmail, accountEmail, StringComparison.OrdinalIgnoreCase) Then
            recipients.Add(New taifCattle.Mail.stru_mailAddress With {.name = displayName, .address = extraEmail})
        End If

        Dim mailInfo As New taifCattle.Mail.stru_mailInfo With {
            .mailTo = recipients,
            .mailBcc = Nothing
        }

        Dim siteName As String = ConfigurationManager.AppSettings("SiteName")
        Dim loginUrl As String = GenerateLoginUrl()

        If isNew Then
            mailInfo.subject = $"【{siteName}】系統帳號建立通知"
            mailInfo.content = $"親愛的{displayName}您好：<br/>系統管理者已為您建立帳號，請使用以下資訊登入系統：<br/>" &
                               $"登入帳號：{accountEmail}<br/>預設密碼：{password}<br/>登入網址：<a href='{loginUrl}'>{loginUrl}</a><br/>" &
                               "登入後請立即變更密碼並妥善保管您的帳號資訊。<br/><br/>此信件由系統自動發出，請勿直接回覆。"
        Else
            mailInfo.subject = $"【{siteName}】系統密碼重設通知"
            mailInfo.content = $"親愛的{displayName}您好：<br/>系統管理者已為您重設登入密碼，請使用以下資訊登入系統：<br/>" &
                               $"登入帳號：{accountEmail}<br/>新密碼：{password}<br/>登入網址：<a href='{loginUrl}'>{loginUrl}</a><br/>" &
                               "登入後請立即變更密碼以確保帳號安全。<br/><br/>此信件由系統自動發出，請勿直接回覆。"
        End If

        Try
            taifCattle_mail.SendMail(mailInfo)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub HandleSave()
        Page.Validate("AccountForm")
        If Not Page.IsValid Then
            ShowEditorView()
            Exit Sub
        End If

        Dim accountText As String = TextBox_account.Text.Trim()
        Dim nameText As String = TextBox_name.Text.Trim()
        Dim roleValue As String = DropDownList_editRole.SelectedValue
        Dim mobileText As String = TextBox_mobile.Text.Trim()
        Dim unitText As String = TextBox_unit.Text.Trim()
        Dim emailText As String = accountText
        Dim memoText As String = TextBox_memo.Text.Trim()
        Dim cityValue As String = DropDownList_editCity.SelectedValue
        Dim selectedCityID As Integer? = Nothing

        If String.IsNullOrWhiteSpace(accountText) Then
            ShowMessage("請輸入登入帳號。")
            ShowEditorView()
            Exit Sub
        End If

        If Not IsValidEmailFormat(accountText) Then
            ShowMessage("請輸入正確的登入帳號電子信箱格式。")
            ShowEditorView()
            Exit Sub
        End If

        If String.IsNullOrWhiteSpace(nameText) Then
            ShowMessage("請輸入使用者姓名。")
            ShowEditorView()
            Exit Sub
        End If

        If String.IsNullOrEmpty(roleValue) Then
            ShowMessage("請選擇系統權限。")
            ShowEditorView()
            Exit Sub
        End If

        Dim isGovUser As Boolean = (roleValue = "3")
        Dim cityIDValue As Integer
        If isGovUser Then
            If String.IsNullOrEmpty(cityValue) OrElse Not Integer.TryParse(cityValue, cityIDValue) Then
                ShowMessage("請選擇縣市。")
                ShowEditorView()
                Exit Sub
            End If
            selectedCityID = cityIDValue
        ElseIf Not String.IsNullOrEmpty(cityValue) AndAlso Integer.TryParse(cityValue, cityIDValue) Then
            selectedCityID = cityIDValue
        End If

        Dim currentUser = InfoSession
        If currentUser.isExist = False Then
            Response.Redirect("~/Login.aspx")
            Exit Sub
        End If

        Try
            If Property_EditMode = taifCattle.Base.enum_EditMode.新增 Then
                If Not ValidateAccountUniqueness(accountText, -1) Then
                    ShowMessage("此登入帳號已存在，請重新輸入。")
                    ShowEditorView()
                    Exit Sub
                End If

                Dim tempPassword As String = taifCattle_account.GenerateCompliantPassword()
                Dim newID As Integer = taifCattle_account.CreateSystemAccount(Convert.ToInt32(roleValue), accountText, tempPassword, nameText, emailText, unitText, mobileText, memoText, currentUser.accountID, selectedCityID)
                Insert_UserLog(currentUser.accountID, taifCattle.Base.enum_UserLogItem.系統帳號管理, taifCattle.Base.enum_UserLogType.新增, $"accountID:{newID},account:{accountText}")

                Dim mailSent = SendAccountNotification(nameText, accountText, emailText, tempPassword, True)
                If mailSent Then
                    ShowMessage($"已建立帳號並寄送預設密碼至「{accountText}」。")
                Else
                    ShowMessage($"帳號已建立，但寄送通知信件至「{accountText}」時發生錯誤。")
                End If
            ElseIf Property_EditMode = taifCattle.Base.enum_EditMode.編輯 Then
                Dim targetID As Integer = Property_EditAccountID
                Dim originalRow As DataRow = taifCattle_account.GetSystemAccount(targetID)
                If originalRow Is Nothing Then
                    ShowMessage("找不到指定的帳號資料。")
                    ShowEditorView()
                    Exit Sub
                End If

                Dim isVerified As Boolean = Property_EditIsVerified
                If isVerified Then
                    accountText = originalRow("account").ToString()
                Else
                    If Not ValidateAccountUniqueness(accountText, targetID) Then
                        ShowMessage("此登入帳號已存在，請重新輸入。")
                        ShowEditorView()
                        Exit Sub
                    End If
                End If

                Dim isActive As Boolean = If(isVerified, CheckBox_isActive.Checked, True)
                taifCattle_account.UpdateSystemAccount(targetID, Convert.ToInt32(roleValue), accountText, nameText, emailText, unitText, mobileText, memoText, isActive, currentUser.accountID, selectedCityID)
                Insert_UserLog(currentUser.accountID, taifCattle.Base.enum_UserLogItem.系統帳號管理, taifCattle.Base.enum_UserLogType.修改, $"accountID:{targetID}")
                ShowMessage("帳號資料已更新完成。")
            End If

            ResetEditor()
            ShowQueryView()
            Property_EditMode = taifCattle.Base.enum_EditMode.預設
            BindGridView()
        Catch ex As Exception
            ShowMessage("儲存帳號資料時發生錯誤，請稍後再試。")
            ShowEditorView()
        End Try
    End Sub

    Private Sub HandleResetPassword(accountID As Integer)
        Dim targetRow As DataRow = taifCattle_account.GetSystemAccount(accountID)
        If targetRow Is Nothing Then
            ShowMessage("找不到指定的帳號資料，無法重設密碼。")
            Exit Sub
        End If

        Dim currentUser = InfoSession
        If currentUser.isExist = False Then
            Response.Redirect("~/Login.aspx")
            Exit Sub
        End If

        Dim tempPassword As String = taifCattle_account.GenerateCompliantPassword()
        Dim result = taifCattle_account.ChangeUserPassword(accountID, currentUser.accountID, tempPassword, taifCattle.Base.enum_UserLogItem.系統帳號管理)
        If result = False Then
            ShowMessage("重設密碼時發生錯誤，請稍後再試。")
            Exit Sub
        End If

        Dim nameText As String = targetRow("name").ToString()
        Dim accountText As String = targetRow("account").ToString()
        Dim emailText As String = If(targetRow("email") Is DBNull.Value, accountText, targetRow("email").ToString())
        Dim mailSent = SendAccountNotification(nameText, accountText, emailText, tempPassword, False)
        If mailSent Then
            ShowMessage($"已重設帳號「{accountText}」的密碼並寄送通知信件。")
        Else
            ShowMessage($"已重設帳號「{accountText}」的密碼，但寄送通知信件時發生錯誤。")
        End If
    End Sub

    Private Sub HandleToggleActive(accountID As Integer)
        Dim targetRow As DataRow = taifCattle_account.GetSystemAccount(accountID)
        If targetRow Is Nothing Then
            ShowMessage("找不到指定的帳號資料，無法變更狀態。")
            Exit Sub
        End If

        Dim isVerified As Boolean = Convert.ToBoolean(targetRow("isEmailVerified"))
        If Not isVerified Then
            ShowMessage("此帳號尚未完成驗證，請先請使用者登入後再進行啟用或停用設定。")
            Exit Sub
        End If

        Dim currentUser = InfoSession
        If currentUser.isExist = False Then
            Response.Redirect("~/Login.aspx")
            Exit Sub
        End If

        Dim isActive As Boolean = Convert.ToBoolean(targetRow("isActive"))
        Dim newStatus As Boolean = Not isActive
        taifCattle_account.UpdateAccountActiveStatus(accountID, newStatus, currentUser.accountID)
        Dim actionText As String = If(newStatus, "啟用", "停用")
        Insert_UserLog(currentUser.accountID, taifCattle.Base.enum_UserLogItem.系統帳號管理, taifCattle.Base.enum_UserLogType.修改, $"accountID:{accountID},action:{actionText}")
        ShowMessage($"已{actionText}帳號「{targetRow("account")}」。")
        BindGridView()
    End Sub

    Private Sub HandleDeleteAccount(accountID As Integer)
        Dim targetRow As DataRow = taifCattle_account.GetSystemAccount(accountID)
        If targetRow Is Nothing Then
            ShowMessage("找不到指定的帳號資料，無法刪除。")
            Exit Sub
        End If

        Dim isVerified As Boolean = Convert.ToBoolean(targetRow("isEmailVerified"))
        If isVerified Then
            ShowMessage("此帳號已完成驗證，無法刪除。")
            Exit Sub
        End If

        Dim currentUser = InfoSession
        If currentUser.isExist = False Then
            Response.Redirect("~/Login.aspx")
            Exit Sub
        End If

        taifCattle_account.DeletePendingAccount(accountID, currentUser.accountID)
        Insert_UserLog(currentUser.accountID, taifCattle.Base.enum_UserLogItem.系統帳號管理, taifCattle.Base.enum_UserLogType.刪除, $"accountID:{accountID}")
        ResetEditor()
        ShowQueryView()
        Property_EditMode = taifCattle.Base.enum_EditMode.預設
        ShowMessage($"已刪除尚未驗證的帳號「{targetRow("account")}」。")
        BindGridView()
    End Sub

    Private Function GetStatusBadge(isVerified As Boolean, isActive As Boolean) As String
        If Not isVerified Then
            Return "<span class='status-badge status-pending'>待驗證</span>"
        End If
        If isActive Then
            Return "<span class='status-badge status-active'>啟用</span>"
        End If
        Return "<span class='status-badge status-inactive'>停用</span>"
    End Function

    Private Sub ExportAccounts()
        Dim roleIDNullable As Integer? = Nothing
        If Not String.IsNullOrEmpty(Property_Query_RoleID) Then
            Dim roleValue As Integer
            If Integer.TryParse(Property_Query_RoleID, roleValue) Then
                roleIDNullable = roleValue
            End If
        End If

        Dim dtAccounts As DataTable = taifCattle_account.GetSystemAccounts(Property_Query_Status, roleIDNullable, Property_Query_Keyword)

        Dim workbook As New XSSFWorkbook()
        Dim sheet As ISheet = workbook.CreateSheet("系統帳號列表")

        Dim headerStyle As ICellStyle = workbook.CreateCellStyle()
        headerStyle.Alignment = HorizontalAlignment.Center
        headerStyle.VerticalAlignment = VerticalAlignment.Center
        Dim headerFont = workbook.CreateFont()
        headerFont.IsBold = True
        headerStyle.SetFont(headerFont)

        Dim headers As String() = {"登入帳號", "使用者姓名", "權限角色", "帳號狀態", "建立日期", "最後登入"}
        Dim headerRow = sheet.CreateRow(0)
        For i As Integer = 0 To headers.Length - 1
            Dim cell = headerRow.CreateCell(i)
            cell.SetCellValue(headers(i))
            cell.CellStyle = headerStyle
        Next

        For r As Integer = 0 To dtAccounts.Rows.Count - 1
            Dim dataRow = sheet.CreateRow(r + 1)
            Dim row = dtAccounts.Rows(r)
            Dim isVerified As Boolean = Convert.ToBoolean(row("isEmailVerified"))
            Dim isActive As Boolean = Convert.ToBoolean(row("isActive"))
            Dim insertDate As String = String.Empty
            If Not row("insertDateTime") Is DBNull.Value Then
                insertDate = Convert.ToDateTime(row("insertDateTime")).ToString("yyyy-MM-dd")
            End If

            Dim lastLoginText As String = "從未登入"
            If Not row("lastLoginDateTime") Is DBNull.Value Then
                lastLoginText = Convert.ToDateTime(row("lastLoginDateTime")).ToString("yyyy-MM-dd HH:mm")
            End If

            dataRow.CreateCell(0).SetCellValue(row("account").ToString())
            dataRow.CreateCell(1).SetCellValue(row("name").ToString())
            dataRow.CreateCell(2).SetCellValue(row("auTypeName").ToString())
            dataRow.CreateCell(3).SetCellValue(If(Not isVerified, "待驗證", If(isActive, "啟用", "停用")))
            dataRow.CreateCell(4).SetCellValue(insertDate)
            dataRow.CreateCell(5).SetCellValue(lastLoginText)
        Next

        sheet.SetColumnWidth(0, 26 * 256)
        sheet.SetColumnWidth(1, 16 * 256)
        sheet.SetColumnWidth(2, 18 * 256)
        sheet.SetColumnWidth(3, 12 * 256)
        sheet.SetColumnWidth(4, 14 * 256)
        sheet.SetColumnWidth(5, 20 * 256)

        Dim fileName As String = $"系統帳號列表_{Now:yyyyMMddHHmmss}.xlsx"

        Using ms As New MemoryStream()
            workbook.Write(ms)
            Response.Clear()
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Response.AddHeader("Content-Disposition", $"attachment; filename={HttpUtility.UrlEncode(fileName, Encoding.UTF8)}")
            Response.BinaryWrite(ms.ToArray())
            Response.End()
        End Using
    End Sub

    Protected Sub LinkButton_addAccount_Click(sender As Object, e As EventArgs) Handles LinkButton_addAccount.Click
        ShowMessage(String.Empty)
        OpenAddEditor()
    End Sub

    Protected Sub LinkButton_search_Click(sender As Object, e As EventArgs) Handles LinkButton_search.Click
        SaveQueryCondition()
        BindGridView()
        ResetEditor(True)
    End Sub


    Protected Sub LinkButton_export_Click(sender As Object, e As EventArgs) Handles LinkButton_export.Click
        SaveQueryCondition()
        ExportAccounts()
    End Sub

    Protected Sub Button_save_Click(sender As Object, e As EventArgs) Handles Button_save.Click
        HandleSave()
    End Sub

    Protected Sub Button_cancel_Click(sender As Object, e As EventArgs) Handles Button_cancel.Click
        ResetEditor(True)
        ShowQueryView()
        Property_EditMode = taifCattle.Base.enum_EditMode.預設
        ShowMessage(String.Empty)
    End Sub

    Protected Sub GridView_accounts_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_accounts.PageIndexChanging
        GridView_accounts.PageIndex = e.NewPageIndex
        BindGridView()
        ResetEditor(True)
    End Sub

    Protected Sub GridView_accounts_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView_accounts.RowCommand
        If e.CommandName = "EditAccount" Then
            Dim accountID As Integer = Convert.ToInt32(e.CommandArgument)
            OpenEditEditor(accountID)
        ElseIf e.CommandName = "ResetPassword" Then
            Dim accountID As Integer = Convert.ToInt32(e.CommandArgument)
            HandleResetPassword(accountID)
        ElseIf e.CommandName = "ToggleActive" Then
            Dim accountID As Integer = Convert.ToInt32(e.CommandArgument)
            HandleToggleActive(accountID)
        ElseIf e.CommandName = "DeleteAccount" Then
            Dim accountID As Integer = Convert.ToInt32(e.CommandArgument)
            HandleDeleteAccount(accountID)
        End If
    End Sub

    Protected Sub GridView_accounts_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView_accounts.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
            Dim isActive As Boolean = Convert.ToBoolean(data("isActive"))
            Dim isVerified As Boolean = Convert.ToBoolean(data("isEmailVerified"))

            Dim accountLiteral As Literal = CType(e.Row.FindControl("Literal_accountInfo"), Literal)
            If accountLiteral IsNot Nothing Then
                Dim accountValue As String = Convert.ToString(data("account"))
                Dim emailValue As String = If(data("email") Is DBNull.Value, String.Empty, Convert.ToString(data("email")))

                Dim accountDisplay As New StringBuilder()
                accountDisplay.AppendFormat("<span class='account-id'>{0}</span>", HttpUtility.HtmlEncode(accountValue))

                If Not String.IsNullOrWhiteSpace(emailValue) AndAlso Not String.Equals(accountValue, emailValue, StringComparison.OrdinalIgnoreCase) Then
                    'accountDisplay.AppendFormat("<div class='text-muted small'>{0}</div>", HttpUtility.HtmlEncode(emailValue))
                End If

                accountLiteral.Text = accountDisplay.ToString()
            End If

            Dim statusLabel As Label = CType(e.Row.FindControl("Label_status"), Label)
            If statusLabel IsNot Nothing Then
                statusLabel.Text = GetStatusBadge(isVerified, isActive)
            End If

            Dim lastLoginLabel As Label = CType(e.Row.FindControl("Label_lastLogin"), Label)
            If lastLoginLabel IsNot Nothing Then
                If data("lastLoginDateTime") Is DBNull.Value Then
                    lastLoginLabel.Text = "從未登入"
                Else
                    lastLoginLabel.Text = Convert.ToDateTime(data("lastLoginDateTime")).ToString("yyyy-MM-dd HH:mm")
                End If
                lastLoginLabel.CssClass = "last-login"
            End If

            Dim toggleButton As LinkButton = CType(e.Row.FindControl("LinkButton_toggleActive"), LinkButton)
            Dim deleteButton As LinkButton = CType(e.Row.FindControl("LinkButton_delete"), LinkButton)

            If toggleButton IsNot Nothing Then
                toggleButton.Visible = isVerified
                If isVerified Then
                    If isActive Then
                        toggleButton.CssClass = "btn btn-danger btn-sm me-1"
                        toggleButton.Text = "<i class='fas fa-toggle-off me-1'></i>停用"
                    Else
                        toggleButton.CssClass = "btn btn-success btn-sm me-1"
                        toggleButton.Text = "<i class='fas fa-toggle-on me-1'></i>啟用"
                    End If
                End If
            End If

            If deleteButton IsNot Nothing Then
                deleteButton.Visible = Not isVerified
            End If
        End If
    End Sub

    Protected Sub DropDownList_editRole_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_editRole.SelectedIndexChanged
        UpdateCitySelectorVisibility()
        ShowEditorView()
    End Sub

    Private Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Label_message.Text <> String.Empty Then
            js.AppendLine("showModal();")
        End If
        Page.ClientScript.RegisterStartupScript(Me.Page.GetType(), "page_js", js.ToString(), True)
    End Sub

End Class
