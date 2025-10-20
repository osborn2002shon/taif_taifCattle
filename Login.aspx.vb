Public Class Login
    Inherits taifCattle.Base

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub Button_login_Click(sender As Object, e As EventArgs) Handles Button_login.Click
        Dim userInfo As New stru_LoginUserInfo
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
                Select Case userInfo.isActive
                    Case True
                        Insert_UserLog(userInfo.accountID, enum_UserLogItem.登入, enum_UserLogType.其他)
                        Select Case userInfo.auTypeID
                            Case 1 '最高管理者(畜產會)
                                Response.Redirect("~/pages/sample/sample.aspx")

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