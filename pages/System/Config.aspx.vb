Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient

Public Class Config
    Inherits taifCattle.Base

    Private Const DEFAULT_MIN_LENGTH As Integer = 8
    Private Const DEFAULT_MAX_AGE As Integer = 90
    Private Const DEFAULT_HISTORY_COUNT As Integer = 3
    Private Const DEFAULT_MIN_AGE As Integer = 1
    Private Const DEFAULT_MAX_FAIL_ATTEMPTS As Integer = 5
    Private Const DEFAULT_LOCKOUT_DURATION As Integer = 15

    Private Structure SystemConfigValues
        Public PasswordMinLength As Integer
        Public RequireUppercase As Boolean
        Public RequireLowercase As Boolean
        Public RequireNumbers As Boolean
        Public RequireSymbols As Boolean
        Public PasswordMaxAge As Integer
        Public PasswordHistoryCount As Integer
        Public PasswordMinAge As Integer
        Public MaxFailAttempts As Integer
        Public LockoutDuration As Integer
    End Structure

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SetupInputAttributes()
        Button_Cancel.OnClientClick = "return confirm('尚未儲存的變更將會遺失，是否確認取消？');"
        Button_Save.OnClientClick = "return confirm('儲存後將立即生效，是否繼續儲存設定？');"

        If Not IsPostBack Then
            LoadConfig()
        End If
    End Sub

    Private Sub SetupInputAttributes()
        TextBox_MinLength.Attributes("min") = "8"
        TextBox_MinLength.Attributes("max") = "20"
        TextBox_MinLength.Attributes("step") = "1"

        TextBox_MaxAge.Attributes("min") = "30"
        TextBox_MaxAge.Attributes("max") = "90"
        TextBox_MaxAge.Attributes("step") = "1"

        TextBox_HistoryCount.Attributes("min") = "3"
        TextBox_HistoryCount.Attributes("max") = "10"
        TextBox_HistoryCount.Attributes("step") = "1"

        TextBox_MinAge.Attributes("min") = "1"
        TextBox_MinAge.Attributes("max") = "7"
        TextBox_MinAge.Attributes("step") = "1"

        TextBox_MaxFailAttempts.Attributes("min") = "3"
        TextBox_MaxFailAttempts.Attributes("max") = "5"
        TextBox_MaxFailAttempts.Attributes("step") = "1"

        TextBox_LockoutDuration.Attributes("min") = "15"
        TextBox_LockoutDuration.Attributes("max") = "60"
        TextBox_LockoutDuration.Attributes("step") = "1"
    End Sub

    Private Sub LoadConfig()
        Dim sql As String = "SELECT TOP 1 passwordMinLength, requireUppercase, requireLowercase, requireNumbers, requireSymbols, " &
                            "passwordMaxAge, passwordHistoryCount, passwordMinAge, maxFailAttempts, lockoutDuration FROM System_Config ORDER BY configID"

        Using da As New DataAccess.MS_SQL
            Dim dt As DataTable = da.GetDataTable(sql)

            If dt.Rows.Count = 0 Then
                ApplyDefaultValues()
            Else
                Dim row As Data.DataRow = dt.Rows(0)
                TextBox_MinLength.Text = row("passwordMinLength").ToString()
                CheckBox_RequireUppercase.Checked = Convert.ToBoolean(row("requireUppercase"))
                CheckBox_RequireLowercase.Checked = Convert.ToBoolean(row("requireLowercase"))
                CheckBox_RequireNumbers.Checked = Convert.ToBoolean(row("requireNumbers"))
                CheckBox_RequireSymbols.Checked = Convert.ToBoolean(row("requireSymbols"))
                TextBox_MaxAge.Text = row("passwordMaxAge").ToString()
                TextBox_HistoryCount.Text = row("passwordHistoryCount").ToString()
                TextBox_MinAge.Text = row("passwordMinAge").ToString()
                TextBox_MaxFailAttempts.Text = row("maxFailAttempts").ToString()
                TextBox_LockoutDuration.Text = row("lockoutDuration").ToString()
            End If
        End Using

        ShowMessage(String.Empty, String.Empty)
    End Sub

    Private Sub ApplyDefaultValues()
        TextBox_MinLength.Text = DEFAULT_MIN_LENGTH.ToString()
        CheckBox_RequireUppercase.Checked = True
        CheckBox_RequireLowercase.Checked = True
        CheckBox_RequireNumbers.Checked = True
        CheckBox_RequireSymbols.Checked = True
        TextBox_MaxAge.Text = DEFAULT_MAX_AGE.ToString()
        TextBox_HistoryCount.Text = DEFAULT_HISTORY_COUNT.ToString()
        TextBox_MinAge.Text = DEFAULT_MIN_AGE.ToString()
        TextBox_MaxFailAttempts.Text = DEFAULT_MAX_FAIL_ATTEMPTS.ToString()
        TextBox_LockoutDuration.Text = DEFAULT_LOCKOUT_DURATION.ToString()
    End Sub

    Private Sub ShowMessage(message As String, cssClass As String)
        Label_Message.Text = message

        Dim baseClass As String = "fw-bold d-block mb-3"
        If String.IsNullOrEmpty(cssClass) Then
            Label_Message.CssClass = baseClass
        Else
            Label_Message.CssClass = baseClass & " " & cssClass
        End If
    End Sub

    Protected Sub Button_Save_Click(sender As Object, e As EventArgs) Handles Button_Save.Click
        Dim values As SystemConfigValues
        Dim errorMessage As String = String.Empty

        If Not ValidateInput(values, errorMessage) Then
            ShowMessage(errorMessage, "text-danger")
            Return
        End If

        Dim updateUserId As Integer = 0
        If Session("userInfo") IsNot Nothing Then
            Dim userInfo As taifCattle.Base.stru_LoginUserInfo = CType(Session("userInfo"), taifCattle.Base.stru_LoginUserInfo)
            updateUserId = userInfo.accountID
        End If

        SaveConfig(values, updateUserId)

        If updateUserId > 0 Then
            Insert_UserLog(updateUserId, enum_UserLogItem.系統參數設定, enum_UserLogType.修改)
        End If

        LoadConfig()
        ShowMessage("設定已儲存，系統參數將立即生效。", "text-success")
    End Sub

    Private Sub SaveConfig(values As SystemConfigValues, updateUserId As Integer)
        Dim sql As String = "IF EXISTS (SELECT 1 FROM System_Config) " &
                            "BEGIN " &
                            "    UPDATE System_Config SET " &
                            "        passwordMinLength = @passwordMinLength, " &
                            "        requireUppercase = @requireUppercase, " &
                            "        requireLowercase = @requireLowercase, " &
                            "        requireNumbers = @requireNumbers, " &
                            "        requireSymbols = @requireSymbols, " &
                            "        passwordMaxAge = @passwordMaxAge, " &
                            "        passwordHistoryCount = @passwordHistoryCount, " &
                            "        passwordMinAge = @passwordMinAge, " &
                            "        maxFailAttempts = @maxFailAttempts, " &
                            "        lockoutDuration = @lockoutDuration, " &
                            "        updateDateTime = GETDATE(), " &
                            "        updateUserID = @updateUserID; " &
                            "END " &
                            "ELSE " &
                            "BEGIN " &
                            "    INSERT INTO System_Config (passwordMinLength, requireUppercase, requireLowercase, requireNumbers, requireSymbols, passwordMaxAge, passwordHistoryCount, passwordMinAge, maxFailAttempts, lockoutDuration, updateDateTime, updateUserID) " &
                            "    VALUES (@passwordMinLength, @requireUppercase, @requireLowercase, @requireNumbers, @requireSymbols, @passwordMaxAge, @passwordHistoryCount, @passwordMinAge, @maxFailAttempts, @lockoutDuration, GETDATE(), @updateUserID); " &
                            "END"

        Dim parameters As SqlParameter() = {
            New SqlParameter("@passwordMinLength", SqlDbType.Int) With {.Value = values.PasswordMinLength},
            New SqlParameter("@requireUppercase", SqlDbType.Bit) With {.Value = values.RequireUppercase},
            New SqlParameter("@requireLowercase", SqlDbType.Bit) With {.Value = values.RequireLowercase},
            New SqlParameter("@requireNumbers", SqlDbType.Bit) With {.Value = values.RequireNumbers},
            New SqlParameter("@requireSymbols", SqlDbType.Bit) With {.Value = values.RequireSymbols},
            New SqlParameter("@passwordMaxAge", SqlDbType.Int) With {.Value = values.PasswordMaxAge},
            New SqlParameter("@passwordHistoryCount", SqlDbType.Int) With {.Value = values.PasswordHistoryCount},
            New SqlParameter("@passwordMinAge", SqlDbType.Int) With {.Value = values.PasswordMinAge},
            New SqlParameter("@maxFailAttempts", SqlDbType.Int) With {.Value = values.MaxFailAttempts},
            New SqlParameter("@lockoutDuration", SqlDbType.Int) With {.Value = values.LockoutDuration},
            New SqlParameter("@updateUserID", SqlDbType.Int) With {.Value = updateUserId}
        }

        Using da As New DataAccess.MS_SQL
            da.ExecNonQuery(sql, parameters)
        End Using
    End Sub

    Private Function ValidateInput(ByRef values As SystemConfigValues, ByRef errorMessage As String) As Boolean
        Dim errors As New List(Of String)

        values.RequireUppercase = CheckBox_RequireUppercase.Checked
        values.RequireLowercase = CheckBox_RequireLowercase.Checked
        values.RequireNumbers = CheckBox_RequireNumbers.Checked
        values.RequireSymbols = CheckBox_RequireSymbols.Checked

        If Not Integer.TryParse(TextBox_MinLength.Text, values.PasswordMinLength) Then
            errors.Add("密碼最小長度需為數字。")
        ElseIf values.PasswordMinLength < 8 OrElse values.PasswordMinLength > 20 Then
            errors.Add("密碼最小長度需介於 8 至 20 之間。")
        End If

        If Not Integer.TryParse(TextBox_MaxAge.Text, values.PasswordMaxAge) Then
            errors.Add("密碼變更週期需為數字。")
        ElseIf values.PasswordMaxAge < 30 OrElse values.PasswordMaxAge > 90 Then
            errors.Add("密碼變更週期需介於 30 至 90 天之間。")
        End If

        If Not Integer.TryParse(TextBox_HistoryCount.Text, values.PasswordHistoryCount) Then
            errors.Add("密碼歷程紀錄需為數字。")
        ElseIf values.PasswordHistoryCount < 3 OrElse values.PasswordHistoryCount > 10 Then
            errors.Add("密碼歷程紀錄需介於 3 至 10 之間。")
        End If

        If Not Integer.TryParse(TextBox_MinAge.Text, values.PasswordMinAge) Then
            errors.Add("密碼最短使用期限需為數字。")
        ElseIf values.PasswordMinAge < 1 OrElse values.PasswordMinAge > 7 Then
            errors.Add("密碼最短使用期限需介於 1 至 7 天之間。")
        End If

        If Not Integer.TryParse(TextBox_MaxFailAttempts.Text, values.MaxFailAttempts) Then
            errors.Add("密碼錯誤次數上限需為數字。")
        ElseIf values.MaxFailAttempts < 3 OrElse values.MaxFailAttempts > 5 Then
            errors.Add("密碼錯誤次數上限需介於 3 至 5 次之間。")
        End If

        If Not Integer.TryParse(TextBox_LockoutDuration.Text, values.LockoutDuration) Then
            errors.Add("帳號鎖定時間需為數字。")
        ElseIf values.LockoutDuration < 15 OrElse values.LockoutDuration > 60 Then
            errors.Add("帳號鎖定時間需介於 15 至 60 分鐘之間。")
        End If

        If values.PasswordMinAge > values.PasswordMaxAge Then
            errors.Add("密碼最短使用期限不可大於密碼變更週期。")
        End If

        If Not (values.RequireUppercase OrElse values.RequireLowercase OrElse values.RequireNumbers OrElse values.RequireSymbols) Then
            errors.Add("請至少勾選一項密碼複雜性要求。")
        End If

        If errors.Count > 0 Then
            errorMessage = String.Join("<br/>", errors)
            Return False
        End If

        Return True
    End Function

    Protected Sub Button_Cancel_Click(sender As Object, e As EventArgs) Handles Button_Cancel.Click
        LoadConfig()
        ShowMessage("已取消變更，畫面已還原為目前儲存的設定。", "text-warning")
    End Sub
End Class
