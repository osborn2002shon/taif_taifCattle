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

        ''' <summary>
        ''' 產生符合規則的隨機密碼
        ''' </summary>
        Public Function GenerateCompliantPassword(Optional length As Integer = 12) As String
            Dim result As taifCattle.Base.stru_checkResult
            Dim password As String = String.Empty
            Do
                password = taifCattle_base.Make_RandomString(length, True)
                result = Check_PasswordReg(password, password)
            Loop Until result.isPass
            Return password
        End Function

        ''' <summary>
        ''' 取得系統帳號清單
        ''' </summary>
        Public Function GetSystemAccounts(status As String, roleID As Integer?, keyword As String) As Data.DataTable
            Return taifCattle_dao.GetSystemAccounts(status, roleID, keyword)
        End Function

        ''' <summary>
        ''' 依帳號ID取得單筆資料
        ''' </summary>
        Public Function GetSystemAccount(accountID As Integer) As Data.DataRow
            Dim dt = taifCattle_dao.GetSystemAccount(accountID)
            If dt.Rows.Count = 0 Then
                Return Nothing
            End If
            Return dt.Rows(0)
        End Function

        ''' <summary>
        ''' 依帳號取得單筆資料
        ''' </summary>
        Public Function GetSystemAccountByAccount(account As String) As Data.DataRow
            Dim dt = taifCattle_dao.GetSystemAccountByAccount(account)
            If dt.Rows.Count = 0 Then
                Return Nothing
            End If
            Return dt.Rows(0)
        End Function

        ''' <summary>
        ''' 新增系統帳號
        ''' </summary>
        Public Function CreateSystemAccount(auTypeID As Integer, account As String, password As String, name As String,
                                            email As String, unit As String, mobile As String, memo As String,
                                            insertAccountID As Integer) As Integer

            Dim md5pw As String
            Using md5Hash As System.Security.Cryptography.MD5 = System.Security.Cryptography.MD5.Create()
                md5pw = taifCattle_base.Convert_MD5(md5Hash, password)
            End Using

            Return taifCattle_dao.InsertSystemAccount(auTypeID, account, md5pw, name, email, unit, mobile, memo, insertAccountID)
        End Function

        ''' <summary>
        ''' 更新系統帳號
        ''' </summary>
        Public Sub UpdateSystemAccount(accountID As Integer, auTypeID As Integer, account As String, name As String,
                                       email As String, unit As String, mobile As String, memo As String,
                                       isActive As Boolean, updateAccountID As Integer)
            taifCattle_dao.UpdateSystemAccount(accountID, auTypeID, account, name, email, unit, mobile, memo, isActive, updateAccountID)
        End Sub

        ''' <summary>
        ''' 刪除尚未驗證的帳號
        ''' </summary>
        Public Sub DeletePendingAccount(accountID As Integer, removeAccountID As Integer)
            taifCattle_dao.DeleteSystemAccount(accountID, removeAccountID)
        End Sub

        ''' <summary>
        ''' 更新帳號啟用狀態
        ''' </summary>
        Public Sub UpdateAccountActiveStatus(accountID As Integer, isActive As Boolean, updateAccountID As Integer)
            taifCattle_dao.UpdateAccountActiveStatus(accountID, isActive, updateAccountID)
        End Sub

        ''' <summary>
        ''' 更新帳號登入資訊與驗證狀態
        ''' </summary>
        Public Sub UpdateAccountLoginInfo(accountID As Integer, loginDateTime As DateTime, isEmailVerified As Boolean)
            taifCattle_dao.UpdateAccountLoginInfo(accountID, loginDateTime, isEmailVerified)
        End Sub

        ''' <summary>
        ''' 停用超過指定日期未登入的帳號
        ''' </summary>
        Public Sub DeactivateDormantAccounts(referenceDate As DateTime, updateAccountID As Integer)
            taifCattle_dao.DeactivateDormantAccounts(referenceDate, updateAccountID)
        End Sub
    End Class
End Namespace

Namespace taifCattle.DAO
    Class Account
        Private ReadOnly Property NowTime As DateTime
            Get
                Return Date.Now
            End Get
        End Property

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

        Function GetSystemAccounts(status As String, roleID As Integer?, keyword As String) As Data.DataTable
            Dim sql =
                <sql>
                    select ua.accountID, ua.account, ua.name, ua.email, ua.unit, ua.mobile, ua.memo,
                           ua.isActive, ua.isEmailVerified, ua.emailVerifiedDateTime, ua.insertDateTime,
                           ua.updateDateTime, ua.updateAccountID, ua.lastLoginDateTime, ua.auTypeID,
                           aut.auTypeName,
                           datediff(day, isnull(ua.lastLoginDateTime, ua.insertDateTime), getdate()) as daysSinceLastLogin
                    from System_UserAccount ua
                    left join System_UserAuType aut on ua.auTypeID = aut.auTypeID
                    where ua.removeDateTime is null
                        and (@roleID is null or @roleID = 0 or ua.auTypeID = @roleID)
                        and (
                                @keyword = '' or
                                ua.account like '%' + @keyword + '%' or
                                ua.name like '%' + @keyword + '%' or
                                ua.email like '%' + @keyword + '%'
                            )
                </sql>.Value

            Select Case status
                Case "active"
                    sql &= " and ua.isEmailVerified = 1 and ua.isActive = 1"
                Case "inactive"
                    sql &= " and ua.isEmailVerified = 1 and ua.isActive = 0"
                Case "pending"
                    sql &= " and ua.isEmailVerified = 0"
            End Select

            sql &= " order by ua.insertDateTime desc"

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("roleID", If(roleID.HasValue, roleID.Value, CType(DBNull.Value, Object))))
            para.Add(New Data.SqlClient.SqlParameter("keyword", If(keyword Is Nothing, "", keyword)))

            Using da As New DataAccess.MS_SQL
                Return da.GetDataTable(sql, para.ToArray)
            End Using
        End Function

        Function GetSystemAccount(accountID As Integer) As Data.DataTable
            Dim sql =
                <sql>
                    select ua.accountID, ua.account, ua.name, ua.email, ua.unit, ua.mobile, ua.memo,
                           ua.isActive, ua.isEmailVerified, ua.emailVerifiedDateTime, ua.insertDateTime,
                           ua.updateDateTime, ua.updateAccountID, ua.lastLoginDateTime, ua.auTypeID,
                           aut.auTypeName
                    from System_UserAccount ua
                    left join System_UserAuType aut on ua.auTypeID = aut.auTypeID
                    where ua.removeDateTime is null and ua.accountID = @accountID
                </sql>.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter) From {
                New Data.SqlClient.SqlParameter("accountID", accountID)
            }

            Using da As New DataAccess.MS_SQL
                Return da.GetDataTable(sql, para.ToArray)
            End Using
        End Function

        Function GetSystemAccountByAccount(account As String) As Data.DataTable
            Dim sql =
                <sql>
                    select *
                    from System_UserAccount
                    where removeDateTime is null and account = @account
                </sql>.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter) From {
                New Data.SqlClient.SqlParameter("account", account)
            }

            Using da As New DataAccess.MS_SQL
                Return da.GetDataTable(sql, para.ToArray)
            End Using
        End Function

        Function InsertSystemAccount(auTypeID As Integer, account As String, password As String, name As String,
                                     email As String, unit As String, mobile As String, memo As String,
                                     insertAccountID As Integer) As Integer

            Dim sql =
                <sql>
                    insert into System_UserAccount
                        (auTypeID, account, password, name, email, unit, mobile, memo,
                         isActive, isEmailVerified, insertDateTime, insertAccountID)
                    values
                        (@auTypeID, @account, @password, @name, @email, @unit, @mobile, @memo,
                         @isActive, @isEmailVerified, @insertDateTime, @insertAccountID);
                    select scope_identity();
                </sql>.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter) From {
                New Data.SqlClient.SqlParameter("auTypeID", auTypeID),
                New Data.SqlClient.SqlParameter("account", account),
                New Data.SqlClient.SqlParameter("password", password),
                New Data.SqlClient.SqlParameter("name", name),
                New Data.SqlClient.SqlParameter("email", If(String.IsNullOrWhiteSpace(email), CType(DBNull.Value, Object), email)),
                New Data.SqlClient.SqlParameter("unit", If(String.IsNullOrWhiteSpace(unit), CType(DBNull.Value, Object), unit)),
                New Data.SqlClient.SqlParameter("mobile", If(String.IsNullOrWhiteSpace(mobile), CType(DBNull.Value, Object), mobile)),
                New Data.SqlClient.SqlParameter("memo", If(String.IsNullOrWhiteSpace(memo), CType(DBNull.Value, Object), memo)),
                New Data.SqlClient.SqlParameter("isActive", True),
                New Data.SqlClient.SqlParameter("isEmailVerified", False),
                New Data.SqlClient.SqlParameter("insertDateTime", NowTime),
                New Data.SqlClient.SqlParameter("insertAccountID", insertAccountID)
            }

            Using da As New DataAccess.MS_SQL
                Dim result = da.ExecuteScalar(sql, para.ToArray)
                Return Convert.ToInt32(result)
            End Using
        End Function

        Sub UpdateSystemAccount(accountID As Integer, auTypeID As Integer, account As String, name As String,
                                email As String, unit As String, mobile As String, memo As String,
                                isActive As Boolean, updateAccountID As Integer)

            Dim sql =
                <sql>
                    update System_UserAccount set
                        auTypeID=@auTypeID,
                        account=@account,
                        name=@name,
                        email=@email,
                        unit=@unit,
                        mobile=@mobile,
                        memo=@memo,
                        isActive=@isActive,
                        updateDateTime=@updateDateTime,
                        updateAccountID=@updateAccountID
                    where accountID=@accountID
                </sql>.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter) From {
                New Data.SqlClient.SqlParameter("auTypeID", auTypeID),
                New Data.SqlClient.SqlParameter("account", account),
                New Data.SqlClient.SqlParameter("name", name),
                New Data.SqlClient.SqlParameter("email", If(String.IsNullOrWhiteSpace(email), CType(DBNull.Value, Object), email)),
                New Data.SqlClient.SqlParameter("unit", If(String.IsNullOrWhiteSpace(unit), CType(DBNull.Value, Object), unit)),
                New Data.SqlClient.SqlParameter("mobile", If(String.IsNullOrWhiteSpace(mobile), CType(DBNull.Value, Object), mobile)),
                New Data.SqlClient.SqlParameter("memo", If(String.IsNullOrWhiteSpace(memo), CType(DBNull.Value, Object), memo)),
                New Data.SqlClient.SqlParameter("isActive", isActive),
                New Data.SqlClient.SqlParameter("updateDateTime", NowTime),
                New Data.SqlClient.SqlParameter("updateAccountID", updateAccountID),
                New Data.SqlClient.SqlParameter("accountID", accountID)
            }

            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sql, para.ToArray)
            End Using
        End Sub

        Sub DeleteSystemAccount(accountID As Integer, removeAccountID As Integer)
            Dim sql =
                <sql>
                    update System_UserAccount set
                        removeDateTime = @removeDateTime,
                        removeAccountID = @removeAccountID
                    where accountID = @accountID and isEmailVerified = 0
                </sql>.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter) From {
                New Data.SqlClient.SqlParameter("removeDateTime", NowTime),
                New Data.SqlClient.SqlParameter("removeAccountID", removeAccountID),
                New Data.SqlClient.SqlParameter("accountID", accountID)
            }

            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sql, para.ToArray)
            End Using
        End Sub

        Sub UpdateAccountActiveStatus(accountID As Integer, isActive As Boolean, updateAccountID As Integer)
            Dim sql =
                <sql>
                    update System_UserAccount set
                        isActive = @isActive,
                        updateDateTime = @updateDateTime,
                        updateAccountID = @updateAccountID
                    where accountID = @accountID
                </sql>.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter) From {
                New Data.SqlClient.SqlParameter("isActive", isActive),
                New Data.SqlClient.SqlParameter("updateDateTime", NowTime),
                New Data.SqlClient.SqlParameter("updateAccountID", updateAccountID),
                New Data.SqlClient.SqlParameter("accountID", accountID)
            }

            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sql, para.ToArray)
            End Using
        End Sub

        Sub UpdateAccountLoginInfo(accountID As Integer, loginDateTime As DateTime, isEmailVerified As Boolean)
            Dim sql =
                <sql>
                    update System_UserAccount set
                        lastLoginDateTime = @lastLoginDateTime,
                        emailVerifiedDateTime = case when @isEmailVerified = 1 and isEmailVerified = 0 then @lastLoginDateTime else emailVerifiedDateTime end,
                        isEmailVerified = case when @isEmailVerified = 1 then 1 else isEmailVerified end
                    where accountID = @accountID
                </sql>.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter) From {
                New Data.SqlClient.SqlParameter("lastLoginDateTime", loginDateTime),
                New Data.SqlClient.SqlParameter("isEmailVerified", If(isEmailVerified, 1, 0)),
                New Data.SqlClient.SqlParameter("accountID", accountID)
            }

            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sql, para.ToArray)
            End Using
        End Sub

        Sub DeactivateDormantAccounts(referenceDate As DateTime, updateAccountID As Integer)
            Dim sql =
                <xml sql="
                    update System_UserAccount set
                        isActive = 0,
                        updateDateTime = @updateDateTime,
                        updateAccountID = @updateAccountID
                    where removeDateTime is null
                        and isActive = 1
                        and isEmailVerified = 1
                        and isnull(lastLoginDateTime, insertDateTime) &lt; @referenceDate
                ">
                </xml>.FirstAttribute.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter) From {
                New Data.SqlClient.SqlParameter("updateDateTime", NowTime),
                New Data.SqlClient.SqlParameter("updateAccountID", updateAccountID),
                New Data.SqlClient.SqlParameter("referenceDate", referenceDate)
            }

            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sql, para.ToArray)
            End Using
        End Sub

    End Class
End Namespace


