Namespace taifCattle
    Public Class Account
        Dim taifCattle_dao As New DAO.Account
        Dim taifCattle_base As New taifCattle.Base

        ''' <summary>
        ''' stru 使用者帳密與基本資料
        ''' </summary>
        ''' <remarks></remarks>
        Structure stru_UserInfo
            Property accountID As Integer
            Property auTypeID As Integer
            Property account As String
            Property password As String
            Property name As String
            Property email As Object    '非必填，可NULL
            Property unit As Object     '非必填，可NULL
            Property mobile As Object   '非必填，可NULL
            Property memo As Object     '非必填，可NULL
            Property lastUpdatePWDateTime As Object

            Property slauID As Object
            Property govID As Object
            Property govCityID As Object
            Property govName As Object
            Property farmID As Object
            Property farmCode As Object
            Property farmName As Object

        End Structure

        ''' <summary>
        ''' 變更使用者密碼
        ''' </summary>
        ''' <param name="accountID">被變更者</param>
        ''' <param name="pw">新密碼</param>
        ''' <param name="updateAccountID">執行人帳號ID</param>
        Public Function ChangeUserPassword(accountID As Integer, updateAccountID As Integer, pw As String, logItem As taifCattle.Base.enum_UserLogItem) As Boolean

            Try
                '密碼加密
                Dim md5pw As String = ""
                Using md5Hash As System.Security.Cryptography.MD5 = System.Security.Cryptography.MD5.Create()
                    md5pw = taifCattle_base.Convert_MD5(md5Hash, pw)
                End Using

                ' 更新密碼
                taifCattle_dao.Update_UserPassword(accountID, updateAccountID, md5pw)

                ' 寫入操作紀錄
                taifCattle_base.Insert_UserLog(updateAccountID, logItem, taifCattle.Base.enum_UserLogType.修改, $"accountID:{accountID}")

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' <summary>
        ''' 檢查密碼是否符合規則or正確
        ''' </summary>
        ''' <param name="pw1"></param>
        ''' <param name="pw2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Check_PasswordReg(pw1 As String, pw2 As String) As taifCattle.Base.stru_checkResult
            Dim pwr As New taifCattle.Base.stru_checkResult
            Dim NumandEG As New Regex("(?=.*[0-9])(?=.*[a-zA-Z])")
            If pw1 <> pw2 Then
                pwr.isPass = False
                pwr.msg = "密碼確認不符！"
                Return pwr
            ElseIf pw1.Length <= 6 Then
                pwr.isPass = False
                pwr.msg = "密碼至少7個字元，並由英文及數字組成！"
                Return pwr
            ElseIf NumandEG.IsMatch(pw1) = False Then
                pwr.isPass = False
                pwr.msg = "密碼至少7個字元，並由英文及數字組成！"
                Return pwr
            Else
                pwr.isPass = True
                pwr.msg = ""
                Return pwr
            End If
        End Function
    End Class
End Namespace

Namespace taifCattle.DAO
    Class Account

        ''' <summary>
        ''' 更新使用者密碼
        ''' </summary>
        ''' <param name="accountID"></param>
        ''' <param name="password"></param>
        ''' <param name="updateAccountID"></param>
        ''' <remarks></remarks>
        Sub Update_UserPassword(accountID As Integer, updateAccountID As Integer, password As String)
            Dim sqlXml =
                <xml sql="
                    update System_UserAccount set 
                    password=@password, lastUpdatePWDateTime=@lastUpdatePWDateTime, 
                    updateDateTime=@updateDateTime, updateAccountID=@updateAccountID
                    where accountID=@accountID
                "></xml>
            Dim sql = sqlXml.Attribute("sql").Value
            Dim dateTime As DateTime = Now
            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("password", password))
            para.Add(New Data.SqlClient.SqlParameter("lastUpdatePWDateTime", dateTime))
            para.Add(New Data.SqlClient.SqlParameter("updateDateTime", dateTime))
            para.Add(New Data.SqlClient.SqlParameter("updateAccountID", updateAccountID))
            para.Add(New Data.SqlClient.SqlParameter("accountID", accountID))
            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sql, para.ToArray)
            End Using
        End Sub

    End Class
End Namespace


