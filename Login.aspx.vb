Public Class Login
    Inherits taifCattle.Base
    Dim taifCattle_account As New taifCattle.Account

    Private Sub Button_login_Click(sender As Object, e As EventArgs) Handles Button_login.Click
        Dim userInfo As New stru_LoginUserInfo
        Dim accountService As New taifCattle.Account
        Dim ac As String = TextBox_ac.Text.Trim
        Dim pw As String = TextBox_pw.Text.Trim

        Dim maxFailAttempts As Integer = 0
        Dim lockoutDuration As Integer = 0
        Dim accountID As Integer = -1
        Dim lastSuccess As Nullable(Of DateTime) = Nothing

        '=====機器人驗證
        If IsNothing(Session("CAPTCHA")) Then
            '圖形驗證碼的Session會過期，會出現錯誤
            Label_msg.Text = "驗證碼過期，請重新輸入。"
            Exit Sub
        Else
            'Sean加的 方便驗證程式進入作測試
            If Session("CAPTCHA").ToString <> TextBox_Captcha.Text.Trim Then
                Label_msg.Text = "請輸入驗證碼或驗證碼輸入錯誤。"
                TextBox_Captcha.Text = ""
                Exit Sub
            End If
        End If

        Session("CAPTCHA") = Nothing '避免POST重送封包Try帳密

        Dim cfg As taifCattle.Account.System_Config = accountService.Get_SystemConfig()
        If cfg IsNot Nothing Then
            maxFailAttempts = cfg.maxFailAttempts
            lockoutDuration = cfg.lockoutDuration
        End If

        Dim accountRow As System.Data.DataRow = accountService.GetSystemAccountByAccount(ac)
        If accountRow IsNot Nothing Then
            accountID = Convert.ToInt32(accountRow("accountID"))
            If Not IsDBNull(accountRow("lastLoginDateTime")) Then
                lastSuccess = CType(accountRow("lastLoginDateTime"), DateTime)
            End If
        End If

        If accountID > 0 AndAlso maxFailAttempts > 0 AndAlso lockoutDuration > 0 Then
            Dim failStatus = accountService.GetLoginFailStatus(accountID, maxFailAttempts, lockoutDuration, lastSuccess)
            If failStatus IsNot Nothing AndAlso failStatus.IsLocked Then
                Dim unlockTimeText As String = failStatus.UnlockTime.Value.ToString("yyyy/MM/dd HH:mm")
                Dim message As String = $"帳號已鎖定，請於 {unlockTimeText} 後再試。"
                If failStatus.FailCount > 0 Then
                    message &= $" (目前已失敗 {failStatus.FailCount} 次)"
                End If
                Insert_UserLog(accountID, enum_UserLogItem.登入, enum_UserLogType.其他, $"登入失敗：帳號鎖定至 {unlockTimeText}")
                Label_msg.Text = message
                Exit Sub
            End If
        End If

        '=====使用者帳密驗證
        Using md5Hash As System.Security.Cryptography.MD5 = System.Security.Cryptography.MD5.Create()
            Dim md5PW As String = Convert_MD5(md5Hash, pw)
            userInfo = Login(ac, md5PW)
        End Using

        Select Case userInfo.isExist
            Case True
                Dim lastActivity As DateTime = userInfo.insertDateTime
                If userInfo.lastLoginDateTime IsNot Nothing AndAlso Not IsDBNull(userInfo.lastLoginDateTime) Then
                    lastActivity = CType(userInfo.lastLoginDateTime, DateTime)
                End If

                If lastActivity <> Date.MinValue Then
                    If DateDiff(DateInterval.Day, lastActivity, Now) > 180 Then
                        accountService.UpdateAccountActiveStatus(userInfo.accountID, False, userInfo.accountID)
                        Insert_UserLog(userInfo.accountID, enum_UserLogItem.登入, enum_UserLogType.其他, "超過180天未登入自動停用")
                        Label_msg.Text = "您的帳號因超過180天未登入已被停用，請洽系統管理員。"
                        Exit Sub
                    End If
                End If

                Select Case userInfo.isActive
                    Case True

                        '初次變更檢查
                        If Not userInfo.isEmailVerified OrElse userInfo.pwUpdateDateTime Is Nothing OrElse
                           IsDBNull(userInfo.pwUpdateDateTime) Then
                            Session("CPW_accountID") = userInfo.accountID
                            Session.Remove("UserInfo")
                            Insert_UserLog(userInfo.accountID, enum_UserLogItem.登入, enum_UserLogType.其他, "初次登入，導向密碼變更")
                            Response.Redirect("CPW.aspx?mode=init")
                            Exit Sub
                        End If

                        '定期變更檢查
                        If taifCattle_account.Check_PasswordMaxAge(userInfo.accountID).isPass = False Then
                            Session("CPW_accountID") = userInfo.accountID
                            Session.Remove("UserInfo")
                            Insert_UserLog(userInfo.accountID, enum_UserLogItem.登入, enum_UserLogType.其他, "密碼到期，導向密碼變更")
                            Response.Redirect("CPW.aspx?mode=expired")
                            Exit Sub
                        End If

                        Insert_UserLog(userInfo.accountID, enum_UserLogItem.登入, enum_UserLogType.其他)
                        accountService.UpdateAccountLoginInfo(userInfo.accountID, Now, True)
                        userInfo.lastLoginDateTime = Now
                        userInfo.isEmailVerified = True
                        Session("UserInfo") = userInfo

                        If userInfo.liMenu IsNot Nothing AndAlso userInfo.liMenu.Count > 0 Then

                            ' === 篩選出 isShow = 1 的選單 ===
                            Dim visibleMenus = userInfo.liMenu.Where(Function(m) m.isShow).ToList()

                            If visibleMenus IsNot Nothing AndAlso visibleMenus.Count > 0 Then
                                ' 取得第一筆 isShow = 1 的目錄
                                Dim firstMenuPath As String = visibleMenus(0).menuURL.Trim()

                                ' 若是相對路徑 "../"，改成實際網站路徑 "~/pages/"
                                If firstMenuPath.StartsWith("../") Then
                                    firstMenuPath = "~/pages/" & firstMenuPath.Replace("../", "")
                                End If

                                ' 若沒有 ~ 或 / 開頭，補上 ~/pages/
                                If Not firstMenuPath.StartsWith("~") AndAlso Not firstMenuPath.StartsWith("/") Then
                                    firstMenuPath = "~/pages/" & firstMenuPath
                                End If

                                ' 轉換為實際 URL
                                Dim targetUrl As String = ResolveUrl(firstMenuPath)

                                ' 導向
                                Response.Redirect(targetUrl)
                            Else
                                ' 找不到任何 isShow = 1 的項目
                                Label_msg.Text = "目前您的帳號尚未設定任何可用功能，<br />如需使用系統，請聯絡系統管理員協助。"
                            End If

                        Else
                            Label_msg.Text = "目前您的帳號尚未設定任何可用功能，<br />如需使用系統，請聯絡系統管理員協助。"
                        End If

                    Case False
                        Insert_UserLog(userInfo.accountID, enum_UserLogItem.登入, enum_UserLogType.其他, "停用中")
                        Label_msg.Text = "您的帳號已被停用，<br />若有任何問題請洽詢系統管理員。"
                End Select

            Case False
                Dim message As String = userInfo.msg

                If accountID > 0 Then
                    Dim failureReason As String = If(String.IsNullOrWhiteSpace(userInfo.msg), "登入失敗", userInfo.msg)
                    Insert_UserLog(accountID, enum_UserLogItem.登入, enum_UserLogType.其他, $"登入失敗：{failureReason}")

                    If maxFailAttempts > 0 AndAlso lockoutDuration > 0 Then
                        Dim failStatus = accountService.GetLoginFailStatus(accountID, maxFailAttempts, lockoutDuration, lastSuccess)

                        If failStatus.FailCount > 0 Then
                            message &= $" (目前已失敗 {failStatus.FailCount} 次)"
                        End If

                        If failStatus.IsLocked AndAlso failStatus.UnlockTime.HasValue Then
                            message &= $"，請於 {failStatus.UnlockTime.Value:yyyy/MM/dd HH:mm} 後再試。"
                        End If
                    End If
                End If

                Label_msg.Text = message
        End Select
    End Sub
End Class
