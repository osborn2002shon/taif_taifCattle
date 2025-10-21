Imports System.Security.Cryptography
Imports taifCattle.taifCattle.Base

Public Class CPW
    Inherits System.Web.UI.Page
    Dim taifCattle_account As New taifCattle.Account
    Dim js As New StringBuilder
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            ' 確認 Session 是否存在
            If Session("CPW_accountID") Is Nothing Then
                ' 安全考量：Session 遺失就返回登入頁
                Response.Redirect("Login.aspx")
                Exit Sub
            End If

            '保存到viewState
            ViewState("accountID") = Session("CPW_accountID")

            '清除session
            Session.Remove("CPW_accountID")

            ' 依功能顯示不同提示
            Dim mode As String = Request.QueryString("mode")
            Select Case mode
                Case "init"
                    Label_title.Text = "初次登入，請先變更密碼。"
                Case "expired"
                    Label_title.Text = "您的密碼已超過使用期限，請立即變更。"
                Case Else
                    ' 安全保護：不允許手動輸入網址
                    Response.Redirect("Login.aspx")
                    Exit Sub
            End Select
        End If
    End Sub

    Private Sub CPW_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Page.ClientScript.RegisterStartupScript(Me.Page.GetType(), "page_js", js.ToString(), True)
    End Sub

    Private Sub Button_change_Click(sender As Object, e As EventArgs) Handles Button_change.Click
        ' 檢查 ViewState 的帳號資訊
        If ViewState("accountID") Is Nothing Then
            Label_resetMsg.Text = "驗證資訊已失效，請重新登入。"
            Exit Sub
        End If

        Dim accountID As Integer = Convert.ToInt32(ViewState("accountID"))
        Dim pw1 As String = TextBox_pw.Text.Trim()
        Dim pw2 As String = TextBox_pwConfirm.Text.Trim()

        If String.IsNullOrEmpty(pw1) OrElse String.IsNullOrEmpty(pw2) Then
            Label_resetMsg.Text = "請輸入新密碼與確認密碼。"
            Exit Sub
        End If

        If pw1 <> pw2 Then
            Label_resetMsg.Text = "兩次輸入的密碼不一致，請重新確認。"
            Exit Sub
        End If

        Dim mode As String = Request.QueryString("mode")
        Dim logItem As enum_UserLogItem
        Select Case mode
            Case "init"
                logItem = enum_UserLogItem.預設密碼變更
            Case "expired"
                logItem = enum_UserLogItem.定期密碼變更
            Case Else
                Label_resetMsg.Text = "發生錯誤，請重新登入系統。"
                Exit Sub
        End Select

        Try
            Dim success As Boolean = taifCattle_account.ChangeUserPassword(accountID, pw1, accountID, logItem)

            If success Then
                taifCattle_account.UpdateAccountLoginInfo(accountID, Now, True)
                Label_resetMsg.Text = "密碼變更成功，請重新登入系統。"
                Button_change.Enabled = False
                js.AppendLine("setTimeout(function(){ window.location.href='Login.aspx'; }, 2000);")
            Else
                Label_resetMsg.Text = "密碼變更失敗，請稍後再試或聯絡系統管理員。"
            End If

        Catch ex As Exception
            Label_resetMsg.Text = "發生問題，請聯絡系統管理員。"
        End Try
    End Sub


End Class