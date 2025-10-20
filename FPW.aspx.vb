
Imports System.Security.Cryptography

Public Class FPW
    Inherits taifCattle.Base
    Dim taifCattle_mail As New taifCattle.Mail
    Dim taifCattle_account As New taifCattle.Account

    Dim js As New StringBuilder

    ' 忘記密碼寄信間隔（秒）
    Public Const FPW_EMAIL_INTERVAL As Integer = 60
    Private Enum EnumViewMode
        Verification = 0
        Account = 1
        ResetPassword = 2
        Invalid = 3
    End Enum
    Public Structure stru_checkAccount
        Public accountID As Integer     ' 帳號ID
        Public account As String        ' 帳號
        Public isActive As Boolean      ' 是否啟用
    End Structure

    Public Structure stru_tokenVerifyResult
        Public isValid As Boolean     ' 是否通過驗證
        Public accountID As Integer      ' 對應使用者 ID
        Public email As String        ' 對應信箱
        Public reason As String       ' 結果
    End Structure
#Region "Fun/Sub"

    Private Sub SwitchView(mode As EnumViewMode)
        Select Case mode
            Case EnumViewMode.Verification
                MultiView_main.SetActiveView(View_verification)
            Case EnumViewMode.Account
                MultiView_main.SetActiveView(View_account)
            Case EnumViewMode.ResetPassword
                MultiView_main.SetActiveView(View_reset)
            Case EnumViewMode.Invalid
                MultiView_main.SetActiveView(View_err)
        End Select
    End Sub

    ''' <summary>
    ''' 檢查使用者帳號與信箱資料
    ''' </summary>
    ''' <param name="mail">電子信箱</param>
    ''' <param name="acc">可選的帳號參數（若為空，僅依信箱查詢）</param>
    ''' <returns>帳號資料清單（可為多筆或一筆）</returns>
    ''' <remarks>
    ''' 若只輸入 mail，則查出該信箱所有帳號；
    ''' 若同時輸入 mail 與 acc，則僅回傳符合的帳號。
    ''' </remarks>
    Function Check_Data(mail As String, Optional acc As String = "") As List(Of stru_checkAccount)
        Dim sqlString As New StringBuilder()
        sqlString.AppendLine("SELECT accountID, account, isActive")
        sqlString.AppendLine("FROM System_UserAccount")
        sqlString.AppendLine("WHERE removeDateTime IS NULL AND ISNULL(email,'') = @mail")

        Dim para As New List(Of Data.SqlClient.SqlParameter) From {
        New Data.SqlClient.SqlParameter("@mail", mail)
    }

        ' 若指定帳號，加入條件
        If Not String.IsNullOrEmpty(acc) Then
            sqlString.AppendLine("AND account = @acc")
            para.Add(New Data.SqlClient.SqlParameter("@acc", acc))
        End If

        Using da As New DataAccess.MS_SQL
            Dim dt As Data.DataTable = da.GetDataTable(sqlString.ToString(), para.ToArray())
            Dim results As New List(Of stru_checkAccount)

            For Each row As Data.DataRow In dt.Rows
                results.Add(New stru_checkAccount With {
                .accountID = Convert.ToInt32(row("accountID")),
                .account = row("account").ToString(),
                .isActive = Convert.ToBoolean(row("isActive"))
            })
            Next

            Return results
        End Using
    End Function

    ''' <summary>
    ''' 產生安全的密碼重設 Token（SHA256）
    ''' </summary>
    ''' <param name="account">帳號</param>
    ''' <returns>64字元安全 Token</returns>
    Public Function GenerateResetToken(account As String) As String
        Dim input As String = account & Now.ToString("yyyy/MM/ddHH:mm:ss")
        Using sha256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256.Create()
            Dim bytes As Byte() = System.Text.Encoding.UTF8.GetBytes(input)
            Dim hashBytes As Byte() = sha256.ComputeHash(bytes)
            Return BitConverter.ToString(hashBytes).Replace("-", "").ToLower()
        End Using
    End Function

    ''' <summary>
    ''' 寫入忘記密碼申請記錄
    ''' </summary>
    ''' <param name="accountID">帳號ID</param>
    ''' <param name="email">信箱</param>
    ''' <param name="token">重設 Token</param>
    Public Sub InsertFPWLog(accountID As Integer, email As String, token As String)
        Dim sql As String =
        "INSERT INTO System_UserFPWLog (accountID, email, resetToken, requestTime, isUsed) " &
        "VALUES (@accountID, @email, @token, GETDATE(), 0)"
        Using da As New DataAccess.MS_SQL
            da.ExecNonQuery(sql,
                New Data.SqlClient.SqlParameter("@accountID", accountID),
                New Data.SqlClient.SqlParameter("@email", email),
                New Data.SqlClient.SqlParameter("@token", token))
        End Using
    End Sub

    ''' <summary>
    ''' 寄送密碼重設信件
    ''' </summary>
    ''' <param name="accountID">帳號ID</param>
    ''' <param name="email">信箱</param>
    ''' <param name="account">帳號名稱</param>
    Public Sub SendResetPasswordMail(accountID As Integer, email As String, account As String)
        ' 產生 Token
        Dim token As String = GenerateResetToken(account)

        ' 寫入記錄
        InsertFPWLog(accountID, email, token)

        ' 組信件內容
        Dim dnsName As String = ConfigurationManager.AppSettings("DNS_Name")
        Dim resetLinks As New List(Of String)
        resetLinks.Add($"{dnsName}/FPW.aspx?resetToken={HttpUtility.UrlEncode(token)}")

        '寄信
        Dim guestToAddress As New taifCattle.Mail.stru_mailAddress
        guestToAddress.name = account
        guestToAddress.address = email
        Dim listGuest As New List(Of taifCattle.Mail.stru_mailAddress)
        listGuest.Add(guestToAddress)
        Dim mailBody As taifCattle.Mail.stru_mailBody =
                    taifCattle_mail.GetMailContent(taifCattle.Mail.enum_mailType.忘記密碼, account, email, resetLinks)
        Dim mailInfo As New taifCattle.Mail.stru_mailInfo
        mailInfo.mailTo = listGuest
        mailInfo.subject = mailBody.subject
        mailInfo.content = mailBody.content
        taifCattle_mail.SendMail(mailInfo)
    End Sub

    ''' <summary>
    ''' 驗證重設密碼 Token 是否有效（存在、未使用、五分鐘內）
    ''' </summary>
    ''' <param name="token">Token 字串</param>
    ''' <returns>驗證結果物件</returns>
    Public Function VerifyResetToken(token As String) As stru_tokenVerifyResult
        Dim sql As String =
        "SELECT TOP 1 accountID, email, requestTime, isUsed " &
        "FROM System_UserFPWLog " &
        "WHERE resetToken = @token " &
        "ORDER BY requestTime DESC"

        Dim result As New stru_tokenVerifyResult With {
            .isValid = False,
            .reason = "查無此驗證資料"
        }

        Using da As New DataAccess.MS_SQL
            Dim dt As Data.DataTable = da.GetDataTable(sql,
            New Data.SqlClient.SqlParameter("@token", token))

            If dt.Rows.Count = 0 Then
                result.reason = "查無此驗證碼"
                Return result
            End If

            Dim row = dt.Rows(0)
            Dim isUsed As Boolean = Convert.ToBoolean(row("isUsed"))
            Dim requestTime As DateTime = Convert.ToDateTime(row("requestTime"))
            Dim accountID As Integer = Convert.ToInt32(row("accountID"))
            Dim email As String = row("email").ToString()

            ' 已使用
            If isUsed Then
                result.reason = "此驗證碼已使用過，請重新申請。"
                Return result
            End If

            ' 超過五分鐘
            If (DateTime.Now - requestTime).TotalMinutes > 5 Then
                result.reason = "驗證碼已逾時，請重新申請。"
                Return result
            End If

            ' 通過驗證
            result.isValid = True
            result.accountID = accountID
            result.email = email
            result.reason = "驗證成功"
        End Using

        Return result
    End Function

    ''' <summary>
    ''' 檢查指定信箱在設定時間內是否已有寄信紀錄
    ''' </summary>
    ''' <param name="email">電子信箱</param>
    ''' <param name="secondsLimit">限制秒數</param>
    ''' <returns>True 表示應阻擋（太頻繁），False 表示可寄信</returns>
    Public Function IsRecentFPWRequest(email As String, secondsLimit As Integer) As Boolean
        Dim sql As String = "
        SELECT TOP 1 requestTime
        FROM System_UserFPWLog
        WHERE email = @email
        ORDER BY requestTime DESC"

        Using da As New DataAccess.MS_SQL
            Dim dt As DataTable = da.GetDataTable(sql, New SqlClient.SqlParameter("@email", email))
            If dt.Rows.Count = 0 Then
                Return False ' 沒有紀錄，允許寄信
            End If

            Dim lastRequest As DateTime = CType(dt.Rows(0)("requestTime"), DateTime)
            Dim diffSeconds As Double = (DateTime.Now - lastRequest).TotalSeconds
            Return diffSeconds < secondsLimit
        End Using
    End Function

    ''' <summary>
    ''' 將 token 標記為已使用
    ''' </summary>
    Public Sub MarkTokenAsUsed(token As String)
        Dim sql As String = "UPDATE System_UserFPWLog SET isUsed = 1, changeTime = GETDATE() WHERE resetToken = @token"
        Using da As New DataAccess.MS_SQL
            da.ExecNonQuery(sql, New SqlClient.SqlParameter("@token", token))
        End Using
    End Sub
#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Dim queryString As String = Request.QueryString("resetToken")
            If String.IsNullOrEmpty(queryString) = False Then
                If VerifyResetToken(queryString).isValid Then
                    SwitchView(EnumViewMode.ResetPassword)
                Else
                    SwitchView(EnumViewMode.Invalid)
                End If
            Else
                SwitchView(EnumViewMode.Verification)
            End If
        End If
    End Sub

    Private Sub FPW_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Page.ClientScript.RegisterStartupScript(Me.Page.GetType(), "page_js", js.ToString(), True)
    End Sub

    Private Sub Button_email_Click(sender As Object, e As EventArgs) Handles Button_email.Click
        Dim mail As String = TextBox_email.Text.Trim()

        If String.IsNullOrEmpty(mail) Then
            Label_msg.Text = "請輸入電子信箱。"
            Exit Sub
        End If

        Dim results As List(Of stru_checkAccount) = Check_Data(mail)

        If results.Count = 0 Then

            ' 查無信箱
            Label_msg.Text = "查無信箱資料！</br>" &
                    "可能是信箱不正確，或是您當初未填寫信箱資料，請與管理者聯繫，感謝您。"
            Exit Sub
        ElseIf results.Count = 1 Then

            ' 檢查是否停用
            Dim r = results(0)
            If Not r.isActive Then
                Label_msg.Text = "您的帳號已被停用，請與管理員聯繫。"
                Exit Sub
            End If

            If IsRecentFPWRequest(mail, FPW_EMAIL_INTERVAL) Then
                Label_msg.Text = "您剛剛已寄送重設密碼信，請稍候再試。"
                Exit Sub
            End If

            SendResetPasswordMail(results(0).accountID, mail, results(0).account)
            Label_msg.Text = "重設密碼連結已寄出，請至信箱查收。"

        Else
            ' 多筆帳號 → 切到輸入帳號頁面
            ViewState("FPW_email") = mail
            SwitchView(EnumViewMode.Account)
            Label_msg_account.Text = "此信箱綁定多個帳號，請輸入欲重設密碼的帳號。"
        End If
    End Sub
    Private Sub Button_account_Click(sender As Object, e As EventArgs) Handles Button_account.Click
        Dim mail As String = ViewState("FPW_email")?.ToString()
        Dim acc As String = TextBox_account.Text.Trim()

        If String.IsNullOrEmpty(acc) Then
            Label_msg_account.Text = "請輸入帳號。"
            Exit Sub
        End If

        Dim results = Check_Data(mail, acc)

        If results.Count = 0 Then
            Label_msg_account.Text = "帳號與信箱不符，請確認後再試。"
        Else
            Dim r = results(0)
            If Not r.isActive Then
                Label_msg_account.Text = "帳號已被停用，無法重設密碼。"
            Else

                If IsRecentFPWRequest(mail, FPW_EMAIL_INTERVAL) Then
                    Label_msg_account.Text = "您剛剛已寄送重設密碼信，請稍候再試。"
                    Exit Sub
                End If

                SendResetPasswordMail(results(0).accountID, mail, results(0).account)
                Label_msg_account.Text = "重設密碼信已寄出，請至信箱查收。"
            End If
        End If

    End Sub
    Private Sub Button_change_Click(sender As Object, e As EventArgs) Handles Button_change.Click
        Dim pw1 As String = TextBox_pw.Text.Trim()
        Dim pw2 As String = TextBox_pwConfirm.Text.Trim()
        Dim token As String = Request.QueryString("resetToken")

        '檢查token
        If String.IsNullOrEmpty(token) Then
            Label_resetMsg.Text = "驗證碼遺失，請重新申請重設密碼。"
            Exit Sub
        End If

        Dim result As stru_tokenVerifyResult = VerifyResetToken(token)
        If Not result.isValid Then
            Label_resetMsg.Text = result.reason
            Exit Sub
        End If

        '檢查密碼
        If String.IsNullOrEmpty(pw1) OrElse String.IsNullOrEmpty(pw2) Then
            Label_resetMsg.Text = "請輸入新密碼與確認密碼。"
            Exit Sub
        End If

        If pw1 <> pw2 Then
            Label_resetMsg.Text = "兩次輸入的密碼不一致，請重新確認。"
            Exit Sub
        End If

        Try

            Dim success As Boolean = taifCattle_account.ChangeUserPassword(result.accountID, result.accountID, pw1, enum_UserLogItem.忘記密碼)

            If success Then
                MarkTokenAsUsed(token)
                Label_resetMsg.Text = "密碼變更成功，請重新登入系統。"
                Button_change.Enabled = False
                js.AppendLine("setTimeout(function(){ window.location.href='Login.aspx'; }, 2000);")
            Else
                Label_resetMsg.Text = "密碼變更失敗，請稍後再試。"
            End If
        Catch ex As Exception
            Label_resetMsg.Text = "發生問題，請聯繫管理員。"
        End Try

    End Sub


End Class