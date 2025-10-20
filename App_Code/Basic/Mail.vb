Imports Microsoft.VisualBasic

Namespace taifCattle

    ''' <summary>
    ''' Class 信件寄送相關函式庫
    ''' </summary>
    Public Class Mail

        Structure stru_mailAddress
            Property name As String
            Property address As String
        End Structure

        Structure stru_mailBody
            Property subject As String
            Property content As String
        End Structure

        Structure stru_mailInfo
            Property mailTo As List(Of stru_mailAddress)
            Property mailBcc As List(Of stru_mailAddress)
            Property subject As String
            Property content As String
        End Structure

        Enum enum_mailType
            忘記密碼
            其他
        End Enum

        ''' <summary>
        ''' 取得系統信件預設內容
        ''' </summary>
        ''' <param name="mailType"></param>
        ''' <param name="toName"></param>
        ''' <param name="toAddress"></param>
        ''' <param name="sysContent"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetMailContent(mailType As enum_mailType, toName As String, toAddress As String,
                                Optional sysContent As List(Of String) = Nothing) As stru_mailBody
            Dim mailBody As New stru_mailBody
            Select Case mailType
                Case enum_mailType.忘記密碼
                    mailBody.subject = "【" & System.Configuration.ConfigurationManager.AppSettings("SiteName") & "】忘記密碼系統訊息"
                    mailBody.content =
                        "親愛的" & toName & "《" & toAddress & "》您好，系統已收到您提交重設個人密碼申請。<br />" &
                        "請點選以下連結進入網站進行密碼變更：<a href='" & sysContent(0) & "'>" & sysContent(0) & "</a><br />" &
                        "請盡快更改個人密碼並緊記時刻妥善保管你的個人密碼，以保障你的個人資料及私隱。<br /><br />" &
                        "此通知由系統自動發出，請勿回覆此電郵，若有任何問題請撥打電話(02)2301-5569至中央畜產會進行詢問。<br />" &
                        Now.ToString("yyyy/MM/dd HH:mm")

                Case enum_mailType.其他
                    mailBody.subject = "【" & System.Configuration.ConfigurationManager.AppSettings("SiteName") & "】其他通知"
                    mailBody.content = "" & Now.ToString("yyyy/MM/dd HH:mm")


            End Select

            Return mailBody
        End Function

        ''' <summary>
        ''' 寄出系統信件
        ''' </summary>
        ''' <param name="mailInfo"></param>
        ''' <remarks></remarks>
        Sub SendMail(mailInfo As stru_mailInfo, Optional isNeedBCCNaif As Boolean = False)
            '建立連線資訊
            Dim mail_server As New Net.Mail.SmtpClient()
            With mail_server
                .Host = System.Configuration.ConfigurationManager.AppSettings("MailSMTP")
                .Port = System.Configuration.ConfigurationManager.AppSettings("MailPort")
                .Credentials = New Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings("MailAccount"),
                                                         System.Configuration.ConfigurationManager.AppSettings("MailPassword"))
                .EnableSsl = True
                .DeliveryMethod = Net.Mail.SmtpDeliveryMethod.Network
            End With

            '信件內容
            Dim mail_msg As New Net.Mail.MailMessage
            mail_msg.To.Clear()
            With mail_msg
                .Subject = mailInfo.subject
                .Body = mailInfo.content
                .From = New Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings("MailAccount"),
                                                 System.Configuration.ConfigurationManager.AppSettings("SiteName"))
                .IsBodyHtml = True
            End With

            For i = 0 To mailInfo.mailTo.Count - 1
                mail_msg.To.Add(New Net.Mail.MailAddress(mailInfo.mailTo(i).address, mailInfo.mailTo(i).name))
            Next

            If mailInfo.mailBcc IsNot Nothing AndAlso mailInfo.mailBcc.Count > 0 Then
                For i = 0 To mailInfo.mailBcc.Count - 1
                    mail_msg.Bcc.Add(New Net.Mail.MailAddress(mailInfo.mailBcc(i).address, mailInfo.mailBcc(i).name))
                Next
            End If

            If isNeedBCCNaif = True Then
                '2023/07 擴充新增加上信件BCC寄送至beef@ms.naif.org.tw
                mail_msg.Bcc.Add(New Net.Mail.MailAddress("beef@ms.naif.org.tw", "中央畜產會國產牛肉溯源管理者"))
                mail_msg.Bcc.Add(New Net.Mail.MailAddress("naifcattle@gmail.com", "國產牛肉溯源系統"))
                mail_msg.Bcc.Add(New Net.Mail.MailAddress("olga@i-forcetech.com", "IFT_Olga"))
            End If

            mail_server.Send(mail_msg)
        End Sub

    End Class

End Namespace
