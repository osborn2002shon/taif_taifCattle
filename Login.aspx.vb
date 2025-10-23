Public Class Login
    Inherits taifCattle.Base

    Dim taifCattle_account As New taifCattle.Account
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub Button_login_Click(sender As Object, e As EventArgs) Handles Button_login.Click
        Dim userInfo As New stru_LoginUserInfo
        Dim accountService As New taifCattle.Account
        Dim ac As String = TextBox_ac.Text.Trim
        Dim pw As String = TextBox_pw.Text.Trim

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
                        If Not userInfo.isEmailVerified Then
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
                        Select Case userInfo.auTypeID
                            Case 1 '系統管理員(農業保險基金)
                                Response.Redirect("~/pages/System/AccountManage.aspx")
                            Case 2 '一般管理員(畜牧司草食產業科)
                                Response.Redirect("~/pages/System/AccountManage.aspx")
                            Case 3 '一般使用者(縣市政府農業局處)
                                Response.Redirect("~/pages/System/AccountManage.aspx")
                            Case 4 '查詢使用者(其他 (ex防檢署/畜產會))
                                Response.Redirect("~/pages/System/AccountManage.aspx")

                        End Select
                    Case False
                        Insert_UserLog(userInfo.accountID, enum_UserLogItem.登入, enum_UserLogType.其他, "停用中")
                        Label_msg.Text = "您的帳號已被停用，<br />若有任何問題請洽詢系統管理員。"
                End Select

            Case False
                Label_msg.Text = userInfo.msg
        End Select
    End Sub
End Class