Imports System
Imports System.Collections.Generic
Imports System.Web.Services.Description

Public Class MyAccount
    Inherits taifCattle.Base
    Public js As New StringBuilder
    Private ReadOnly taifCattle_account As New taifCattle.Account

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not InfoSession.isExist Then
            Response.Redirect("~/Login.aspx")
            Return
        End If

        If Not IsPostBack Then
            Label_Message.Text = ""
            BindUserInfo()
        End If
    End Sub

    Sub ShowMessage(msg As String)
        Label_Message.Text = msg
        js.AppendLine("showModal();")
    End Sub

    Private Sub BindUserInfo()
        Dim currentUser = InfoSession
        Dim accountRow = taifCattle_account.GetSystemAccount(currentUser.accountID)

        Panel_govCity.Visible = False
        Label_govCity.Text = String.Empty

        If accountRow Is Nothing Then
            ShowMessage("找不到帳號資料，請聯絡系統管理員。")
            Return
        End If

        Label_account.Text = accountRow("account").ToString()
        Label_name.Text = accountRow("name").ToString()
        Label_role.Text = accountRow("auTypeName").ToString()
        TextBox_unit.Text = If(Convert.IsDBNull(accountRow("unit")), String.Empty, accountRow("unit").ToString())
        TextBox_mobile.Text = If(Convert.IsDBNull(accountRow("mobile")), String.Empty, accountRow("mobile").ToString())
        Label_lastLogin.Text = FormatDateTimeValue(currentUser.lastLoginDateTime)
        Label_passwordChanged.Text = FormatDateTimeValue(currentUser.pwUpdateDateTime)

        Dim auTypeID As Integer = Convert.ToInt32(accountRow("auTypeID"))
        If auTypeID = 3 Then
            Dim govCityName = GetGovCityName(accountRow("govID"))
            Panel_govCity.Visible = Not String.IsNullOrWhiteSpace(govCityName)
            Label_govCity.Text = govCityName
        End If
    End Sub

    Private Function GetGovCityName(govIdValue As Object) As String
        If govIdValue Is Nothing OrElse Convert.IsDBNull(govIdValue) Then
            Return String.Empty
        End If

        Dim govId As Integer
        If Not Integer.TryParse(govIdValue.ToString(), govId) Then
            Return String.Empty
        End If

        Dim sql As String = "select top 1 city from System_Taiwan where cityID = @cityID"
        Dim para As New List(Of Data.SqlClient.SqlParameter) From {
            New Data.SqlClient.SqlParameter("cityID", govId)
        }

        Using da As New DataAccess.MS_SQL()
            Dim dt = da.GetDataTable(sql, para.ToArray())
            If dt.Rows.Count > 0 Then
                Return dt.Rows(0)("city").ToString()
            End If
        End Using

        Return String.Empty
    End Function

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

    Private Sub LinkButton_saveBasic_Click(sender As Object, e As EventArgs) Handles LinkButton_saveBasic.Click
        Dim currentUser = InfoSession
        If Not currentUser.isExist Then
            Response.Redirect("~/Login.aspx")
            Return
        End If

        Dim unit = TextBox_unit.Text.Trim()
        Dim mobile = TextBox_mobile.Text.Trim()

        Try
            Dim accountRow = taifCattle_account.GetSystemAccount(currentUser.accountID)
            If accountRow Is Nothing Then
                ShowMessage("找不到帳號資料，請聯絡系統管理員。")
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

            ShowMessage("基本資料已儲存。")
            BindUserInfo()
        Catch ex As Exception
            ShowMessage("儲存基本資料時發生錯誤，請稍後再試或聯絡系統管理員。")
        End Try
    End Sub

    Protected Sub LinkButton_changePassword_Click(sender As Object, e As EventArgs) Handles LinkButton_changePassword.Click

        Dim currentUser = InfoSession
        If Not currentUser.isExist Then
            Response.Redirect("~/Login.aspx")
            Return
        End If

        Dim pw1 = TextBox_newPassword.Text.Trim()
        Dim pw2 = TextBox_confirmPassword.Text.Trim()


        If String.IsNullOrEmpty(pw1) OrElse String.IsNullOrEmpty(pw2) Then
            ShowMessage("請輸入新密碼與確認密碼。")
            Exit Sub
        End If

        ' 檢查密碼最短使用期限
        Dim minAge As stru_checkResult = taifCattle_account.Check_PasswordMinAge(currentUser.accountID)
        If minAge.isPass = False Then
            ShowMessage(minAge.msg)
            Exit Sub
        End If

        ' 檢查密碼及複雜度
        Dim pwRegResult As stru_checkResult = taifCattle_account.Check_PasswordRegFromDB(pw1, pw2)
        If pwRegResult.isPass = False Then
            ShowMessage(pwRegResult.msg)
            Exit Sub
        End If

        '檢查密碼是否重複
        Dim pwHistory As stru_checkResult = taifCattle_account.Check_PasswordHistory(currentUser.accountID, pw1)
        If pwHistory.isPass = False Then
            ShowMessage(pwHistory.msg)
            Exit Sub
        End If

        Try
            Dim success = taifCattle_account.ChangeUserPassword(accountID:=currentUser.accountID,
                                                                updateAccountID:=currentUser.accountID,
                                                                pw:=pw1,
                                                                logItem:=taifCattle.Base.enum_UserLogItem.我的帳號管理)

            If success Then
                TextBox_newPassword.Text = String.Empty
                TextBox_confirmPassword.Text = String.Empty

                currentUser.pwUpdateDateTime = DateTime.Now
                Session("UserInfo") = currentUser

                ShowMessage("密碼變更成功。")
                BindUserInfo()
            Else
                ShowMessage("密碼變更失敗，請稍後再試或聯絡系統管理員。")
            End If
        Catch ex As Exception
            ShowMessage("密碼變更時發生錯誤，請稍後再試或聯絡系統管理員。")
        End Try
    End Sub

    Private Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Page.ClientScript.RegisterStartupScript(Me.Page.GetType(), "page_js", js.ToString(), True)
    End Sub
End Class

