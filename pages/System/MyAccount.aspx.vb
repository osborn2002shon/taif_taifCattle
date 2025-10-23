Imports System

Public Class MyAccount
    Inherits taifCattle.Base

    Private ReadOnly taifCattle_account As New taifCattle.Account

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not InfoSession.isExist Then
            Response.Redirect("~/Login.aspx")
            Return
        End If

        If Not IsPostBack Then
            HiddenField_activeTab.Value = "#tabBasic"
            ResetMessages()
            BindUserInfo()
        End If
    End Sub

    Private Sub ResetMessages()
        ShowBasicMessage(String.Empty, False)
        ShowPasswordMessage(String.Empty, False)
    End Sub

    Private Sub BindUserInfo()
        Dim currentUser = InfoSession
        Dim accountRow = taifCattle_account.GetSystemAccount(currentUser.accountID)

        If accountRow Is Nothing Then
            ShowBasicMessage("找不到帳號資料，請聯絡系統管理員。", True)
            Return
        End If

        TextBox_account.Text = accountRow("account").ToString()
        TextBox_name.Text = accountRow("name").ToString()
        TextBox_role.Text = accountRow("auTypeName").ToString()
        TextBox_email.Text = If(Convert.IsDBNull(accountRow("email")), String.Empty, accountRow("email").ToString())
        TextBox_unit.Text = If(Convert.IsDBNull(accountRow("unit")), String.Empty, accountRow("unit").ToString())
        TextBox_mobile.Text = If(Convert.IsDBNull(accountRow("mobile")), String.Empty, accountRow("mobile").ToString())
        TextBox_lastLogin.Text = FormatDateTimeValue(currentUser.lastLoginDateTime)
        TextBox_passwordChanged.Text = FormatDateTimeValue(currentUser.pwUpdateDateTime)
    End Sub

    Private Function FormatDateTimeValue(value As Object) As String
        If value Is Nothing OrElse Convert.IsDBNull(value) Then
            Return String.Empty
        End If

        Dim parsedDate As DateTime
        If DateTime.TryParse(value.ToString(), parsedDate) Then
            If parsedDate = DateTime.MinValue Then
                Return String.Empty
            End If
            Return parsedDate.ToString("yyyy-MM-dd HH:mm")
        End If

        Return value.ToString()
    End Function

    Private Sub ShowBasicMessage(message As String, isError As Boolean)
        If String.IsNullOrWhiteSpace(message) Then
            Label_basicMessage.Text = String.Empty
            Label_basicMessage.CssClass = "d-block fw-bold mb-3"
        Else
            Label_basicMessage.Text = message
            Dim cssClass = If(isError, "text-danger", "text-success")
            Label_basicMessage.CssClass = "d-block fw-bold mb-3 " & cssClass
        End If
    End Sub

    Private Sub ShowPasswordMessage(message As String, isError As Boolean)
        If String.IsNullOrWhiteSpace(message) Then
            Label_passwordMessage.Text = String.Empty
            Label_passwordMessage.CssClass = "d-block fw-bold mb-3"
        Else
            Label_passwordMessage.Text = message
            Dim cssClass = If(isError, "text-danger", "text-success")
            Label_passwordMessage.CssClass = "d-block fw-bold mb-3 " & cssClass
        End If
    End Sub

    Protected Sub Button_saveBasic_Click(sender As Object, e As EventArgs) Handles Button_saveBasic.Click
        HiddenField_activeTab.Value = "#tabBasic"

        Dim currentUser = InfoSession
        If Not currentUser.isExist Then
            Response.Redirect("~/Login.aspx")
            Return
        End If

        ShowPasswordMessage(String.Empty, False)

        Dim unit = TextBox_unit.Text.Trim()
        Dim mobile = TextBox_mobile.Text.Trim()

        Try
            Dim accountRow = taifCattle_account.GetSystemAccount(currentUser.accountID)
            If accountRow Is Nothing Then
                ShowBasicMessage("找不到帳號資料，請聯絡系統管理員。", True)
                Return
            End If

            Dim auTypeID As Integer = Convert.ToInt32(accountRow("auTypeID"))
            Dim account = accountRow("account").ToString()
            Dim name = accountRow("name").ToString()
            Dim email = If(Convert.IsDBNull(accountRow("email")), String.Empty, accountRow("email").ToString())
            Dim memo = If(Convert.IsDBNull(accountRow("memo")), String.Empty, accountRow("memo").ToString())
            Dim isActive = Convert.ToBoolean(accountRow("isActive"))
            Dim govCityID As Integer? = Nothing
            If Not Convert.IsDBNull(accountRow("govID")) Then
                govCityID = Convert.ToInt32(accountRow("govID"))
            End If

            taifCattle_account.UpdateSystemAccount(accountID:=currentUser.accountID,
                                                  auTypeID:=auTypeID,
                                                  account:=account,
                                                  name:=name,
                                                  email:=email,
                                                  unit:=unit,
                                                  mobile:=mobile,
                                                  memo:=memo,
                                                  isActive:=isActive,
                                                  updateAccountID:=currentUser.accountID,
                                                  govCityID:=govCityID)

            Insert_UserLog(currentUser.accountID,
                           taifCattle.Base.enum_UserLogItem.我的帳號管理,
                           taifCattle.Base.enum_UserLogType.修改,
                           "update basic info")

            currentUser.unit = unit
            currentUser.tel = mobile
            Session("UserInfo") = currentUser

            ShowBasicMessage("基本資料已儲存。", False)
            BindUserInfo()
        Catch ex As Exception
            ShowBasicMessage("儲存基本資料時發生錯誤，請稍後再試或聯絡系統管理員。", True)
        End Try
    End Sub

    Protected Sub Button_changePassword_Click(sender As Object, e As EventArgs) Handles Button_changePassword.Click
        HiddenField_activeTab.Value = "#tabPassword"

        Dim currentUser = InfoSession
        If Not currentUser.isExist Then
            Response.Redirect("~/Login.aspx")
            Return
        End If

        ShowBasicMessage(String.Empty, False)

        Dim newPassword = TextBox_newPassword.Text.Trim()
        Dim confirmPassword = TextBox_confirmPassword.Text.Trim()

        Dim checkResult = taifCattle_account.Check_PasswordReg(newPassword, confirmPassword)
        If Not checkResult.isPass Then
            ShowPasswordMessage(checkResult.msg, True)
            Return
        End If

        Try
            Dim success = taifCattle_account.ChangeUserPassword(accountID:=currentUser.accountID,
                                                                updateAccountID:=currentUser.accountID,
                                                                pw:=newPassword,
                                                                logItem:=taifCattle.Base.enum_UserLogItem.我的帳號管理)

            If success Then
                TextBox_newPassword.Text = String.Empty
                TextBox_confirmPassword.Text = String.Empty

                currentUser.pwUpdateDateTime = DateTime.Now
                Session("UserInfo") = currentUser

                ShowPasswordMessage("密碼變更成功。", False)
                BindUserInfo()
            Else
                ShowPasswordMessage("密碼變更失敗，請稍後再試或聯絡系統管理員。", True)
            End If
        Catch ex As Exception
            ShowPasswordMessage("密碼變更時發生錯誤，請稍後再試或聯絡系統管理員。", True)
        End Try
    End Sub

End Class

