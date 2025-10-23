Imports Microsoft.VisualBasic

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

        Public Class System_Config
            Public Property passwordMinLength As Integer
            Public Property requireUppercase As Boolean
            Public Property requireLowercase As Boolean
            Public Property requireNumbers As Boolean
            Public Property requireSymbols As Boolean

            Public Property passwordMaxAge As Integer
            Public Property passwordHistoryCount As Integer
            Public Property passwordMinAge As Integer

            Public Property maxFailAttempts As Integer
            Public Property lockoutDuration As Integer

            Public Property updateDateTime As DateTime
            Public Property updateUserID As Integer
        End Class

        Public Class LoginFailStatus
            Public Property FailCount As Integer
            Public Property LastFailTime As Nullable(Of DateTime)
            Public Property UnlockTime As Nullable(Of DateTime)
            Public Property IsLocked As Boolean
        End Class

        ''' <summary>
        ''' 變更使用者密碼
        ''' </summary>
        ''' <param name="accountID">被變更者</param>
        ''' <param name="pw">新密碼</param>
        ''' <param name="updateAccountID">執行人帳號ID</param>
        Public Function ChangeUserPassword(accountID As Integer, updateAccountID As Integer, pw As String, logItem As taifCattle.Base.enum_UserLogItem) As Boolean

            Try
                '密碼加密
                Dim md5pw As String
                Using md5Hash As System.Security.Cryptography.MD5 = System.Security.Cryptography.MD5.Create()
                    md5pw = taifCattle_base.Convert_MD5(md5Hash, pw)
                End Using

                ' 更新密碼
                taifCattle_dao.Update_UserPassword(accountID, updateAccountID, md5pw)

                '寫入密碼變更紀錄
                taifCattle_dao.Insert_UserPasswordLog(accountID, updateAccountID, md5pw, logItem.ToString())

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
        ''' 檢查密碼是否符合系統設定規則，並驗證密碼確認是否一致
        ''' </summary>
        ''' <param name="pw1">密碼</param>
        ''' <param name="pw2">密碼確認</param>
        ''' <returns>stru_checkResult</returns>
        ''' <remarks></remarks>
        Function Check_PasswordRegFromDB(pw1 As String, pw2 As String) As taifCattle.Base.stru_checkResult
            Dim pwr As New taifCattle.Base.stru_checkResult
            Dim cfg As System_Config = Get_SystemConfig()

            ' === 若系統設定不存在 ===
            If cfg Is Nothing Then
                pwr.isPass = False
                pwr.msg = "系統密碼設定未載入，請聯絡系統管理員！"
                Return pwr
            End If

            ' === 檢查密碼一致性 ===
            If pw1 <> pw2 Then
                pwr.isPass = False
                pwr.msg = "密碼確認不符！"
                Return pwr
            End If

            ' === 長度檢查 ===
            If pw1.Length < cfg.passwordMinLength Then
                pwr.isPass = False
                pwr.msg = $"密碼長度不足，至少需 {cfg.passwordMinLength} 個字元！"
                Return pwr
            End If

            ' === 大寫字母 ===
            If cfg.requireUppercase AndAlso Not Regex.IsMatch(pw1, "[A-Z]") Then
                pwr.isPass = False
                pwr.msg = "密碼必須包含至少一個大寫英文字母！"
                Return pwr
            End If

            ' === 小寫字母 ===
            If cfg.requireLowercase AndAlso Not Regex.IsMatch(pw1, "[a-z]") Then
                pwr.isPass = False
                pwr.msg = "密碼必須包含至少一個小寫英文字母！"
                Return pwr
            End If

            ' === 數字 ===
            If cfg.requireNumbers AndAlso Not Regex.IsMatch(pw1, "[0-9]") Then
                pwr.isPass = False
                pwr.msg = "密碼必須包含至少一個數字！"
                Return pwr
            End If

            ' === 特殊符號 ===
            If cfg.requireSymbols AndAlso Not Regex.IsMatch(pw1, "[!@#$%^&*]") Then
                pwr.isPass = False
                pwr.msg = "密碼必須包含至少一個特殊符號！"
                Return pwr
            End If

            ' === 若全部通過 ===
            pwr.isPass = True
            pwr.msg = ""
            Return pwr
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
                                            insertAccountID As Integer, govCityID As Integer?) As Integer

            Dim md5pw As String
            Using md5Hash As System.Security.Cryptography.MD5 = System.Security.Cryptography.MD5.Create()
                md5pw = taifCattle_base.Convert_MD5(md5Hash, password)
            End Using

            Return taifCattle_dao.InsertSystemAccount(auTypeID, account, md5pw, name, email, unit, mobile, memo, insertAccountID, govCityID)
        End Function

        ''' <summary>
        ''' 更新系統帳號
        ''' </summary>
        Public Sub UpdateSystemAccount(accountID As Integer, auTypeID As Integer, account As String, name As String,
                                       email As String, unit As String, mobile As String, memo As String,
                                       isActive As Boolean, updateAccountID As Integer, govCityID As Integer?)
            taifCattle_dao.UpdateSystemAccount(accountID, auTypeID, account, name, email, unit, mobile, memo, isActive, updateAccountID, govCityID)
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

        ''' <summary>
        ''' 取得系統設定
        ''' </summary>
        ''' <returns>System_Config</returns>
        Public Function Get_SystemConfig() As System_Config
            Dim dt As Data.DataTable = taifCattle_dao.Get_SystemConfig()
            Dim cfg As New System_Config

            If dt.Rows.Count = 0 Then
                Return Nothing
            End If

            Dim dr As Data.DataRow = dt.Rows(0)

            ' === 密碼規則 ===
            cfg.passwordMinLength = Convert.ToInt32(taifCattle_base.Convert_EmptyToObject(dr("passwordMinLength").ToString, 0))
            cfg.requireUppercase = Convert.ToBoolean(taifCattle_base.Convert_EmptyToObject(dr("requireUppercase").ToString, False))
            cfg.requireLowercase = Convert.ToBoolean(taifCattle_base.Convert_EmptyToObject(dr("requireLowercase").ToString, False))
            cfg.requireNumbers = Convert.ToBoolean(taifCattle_base.Convert_EmptyToObject(dr("requireNumbers").ToString, False))
            cfg.requireSymbols = Convert.ToBoolean(taifCattle_base.Convert_EmptyToObject(dr("requireSymbols").ToString, False))

            ' === 密碼期限與歷史 ===
            cfg.passwordMaxAge = Convert.ToInt32(taifCattle_base.Convert_EmptyToObject(dr("passwordMaxAge").ToString, 0))
            cfg.passwordHistoryCount = Convert.ToInt32(taifCattle_base.Convert_EmptyToObject(dr("passwordHistoryCount").ToString, 0))
            cfg.passwordMinAge = Convert.ToInt32(taifCattle_base.Convert_EmptyToObject(dr("passwordMinAge").ToString, 0))

            ' === 登入與鎖定 ===
            cfg.maxFailAttempts = Convert.ToInt32(taifCattle_base.Convert_EmptyToObject(dr("maxFailAttempts").ToString, 0))
            cfg.lockoutDuration = Convert.ToInt32(taifCattle_base.Convert_EmptyToObject(dr("lockoutDuration").ToString, 0))

            ' === 系統維護資訊 ===
            cfg.updateDateTime = Convert.ToDateTime(taifCattle_base.Convert_EmptyToObject(dr("updateDateTime").ToString, Now))
            cfg.updateUserID = Convert.ToInt32(taifCattle_base.Convert_EmptyToObject(dr("updateUserID").ToString, 0))

            Return cfg
        End Function

        Public Function GetLoginFailStatus(accountID As Integer, maxFailAttempts As Integer, lockoutDurationMinutes As Integer, Optional lastSuccess As Nullable(Of DateTime) = Nothing) As LoginFailStatus
            Dim status As New LoginFailStatus With {
                .FailCount = 0,
                .IsLocked = False,
                .LastFailTime = Nothing,
                .UnlockTime = Nothing
            }

            If accountID <= 0 OrElse maxFailAttempts <= 0 OrElse lockoutDurationMinutes <= 0 Then
                Return status
            End If

            Dim sinceTime As DateTime = Date.Now.AddMinutes(-lockoutDurationMinutes)

            If lastSuccess.HasValue AndAlso lastSuccess.Value > sinceTime Then
                sinceTime = lastSuccess.Value
            End If

            Dim dtFail As Data.DataTable = taifCattle_dao.Get_LoginFailLogs(accountID, sinceTime, taifCattle.Base.enum_UserLogItem.登入.ToString())

            status.FailCount = dtFail.Rows.Count

            If status.FailCount > 0 Then
                Dim lastFailDate As DateTime = Convert.ToDateTime(dtFail.Rows(0)("logDateTime"))
                status.LastFailTime = lastFailDate

                Dim unlockTime As DateTime = lastFailDate.AddMinutes(lockoutDurationMinutes)
                status.UnlockTime = unlockTime

                If status.FailCount >= maxFailAttempts AndAlso unlockTime > Date.Now Then
                    status.IsLocked = True
                End If
            End If

            Return status
        End Function

        ''' <summary>
        ''' 檢查密碼是否重複使用（依歷史紀錄）
        ''' </summary>
        ''' <param name="accountID">使用者帳號 ID</param>
        ''' <param name="newPassword">密碼</param>
        ''' <remarks></remarks>
        Function Check_PasswordHistory(accountID As Integer, newPassword As String) As taifCattle.Base.stru_checkResult
            Dim pwr As New taifCattle.Base.stru_checkResult
            Dim cfg As System_Config = Get_SystemConfig()

            ' === 檢查設定是否存在 ===
            If cfg Is Nothing Then
                pwr.isPass = False
                pwr.msg = "系統密碼設定未載入，請聯絡系統管理員！"
                Return pwr
            End If

            ' === 沒有設定歷史檢查次數 ===
            If cfg.passwordHistoryCount <= 0 Then
                pwr.isPass = True
                Return pwr
            End If

            ' === 取得歷史紀錄（取最新 N 筆） ===
            Dim dt As Data.DataTable = taifCattle_dao.Get_UserPasswordHistory(accountID, cfg.passwordHistoryCount)

            If dt.Rows.Count = 0 Then
                pwr.isPass = True
                Return pwr
            End If

            ' === 將新密碼雜湊 ===
            Dim newHash As String
            Using md5Hash As System.Security.Cryptography.MD5 = System.Security.Cryptography.MD5.Create()
                newHash = taifCattle_base.Convert_MD5(md5Hash, newPassword)
            End Using

            ' === 比對歷史雜湊 ===
            For Each dr As Data.DataRow In dt.Rows
                Dim oldHash As String = dr("pwdHash").ToString()
                If String.Equals(newHash, oldHash, StringComparison.OrdinalIgnoreCase) Then
                    pwr.isPass = False
                    pwr.msg = $"密碼與過去 {cfg.passwordHistoryCount} 次密碼重複，請設定不同密碼。"
                    Return pwr
                End If
            Next

            ' === 若無重複 ===
            pwr.isPass = True
            Return pwr
        End Function

        ''' <summary>
        ''' 檢查密碼是否達最短使用期限
        ''' </summary>
        ''' <param name="accountID">使用者帳號 ID</param>
        ''' <returns>stru_checkResult</returns>
        Function Check_PasswordMinAge(accountID As Integer) As taifCattle.Base.stru_checkResult
            Dim pwr As New taifCattle.Base.stru_checkResult
            Dim cfg As System_Config = Get_SystemConfig()

            If cfg Is Nothing Then
                pwr.isPass = False
                pwr.msg = "系統設定未載入，請聯絡系統管理員！"
                Return pwr
            End If

            If cfg.passwordMinAge <= 0 Then
                pwr.isPass = True
                Return pwr
            End If

            Dim dt As Data.DataTable = taifCattle_dao.Get_UserPasswordHistory(accountID, 1) ' 只撈最新一筆

            If dt.Rows.Count = 0 Then
                pwr.isPass = True
                Return pwr
            End If

            Dim lastChangeTime As DateTime = Convert.ToDateTime(dt.Rows(0)("changeDateTime"))
            Dim nextAllowDate As DateTime = lastChangeTime.AddDays(cfg.passwordMinAge)

            If Now < nextAllowDate Then
                pwr.isPass = False
                pwr.msg = $"密碼變更過於頻繁，請於 {nextAllowDate:yyyy/MM/dd HH:mm} 後再試。"
                Return pwr
            End If

            pwr.isPass = True
            Return pwr
        End Function

        ''' <summary>
        ''' 檢查密碼是否已達最長使用期限
        ''' </summary>
        ''' <param name="accountID">使用者帳號 ID</param>
        ''' <returns>stru_checkResult</returns>
        Function Check_PasswordMaxAge(accountID As Integer) As taifCattle.Base.stru_checkResult
            Dim pwr As New taifCattle.Base.stru_checkResult
            Dim cfg As System_Config = Get_SystemConfig()

            ' === 沒有設定期限 ===
            If cfg Is Nothing OrElse cfg.passwordMaxAge <= 0 Then
                pwr.isPass = True
                Return pwr
            End If

            ' === 撈取最新一次密碼變更時間 ===
            Dim dt As Data.DataTable = taifCattle_dao.Get_UserPasswordHistory(accountID, 1)
            If dt.Rows.Count = 0 Then
                ' 沒有歷史紀錄 → 視為需變更（初始帳號）
                pwr.isPass = False
                pwr.msg = "系統尚未設定密碼，請立即變更密碼。"
                Return pwr
            End If

            Dim lastChangeTime As DateTime = Convert.ToDateTime(dt.Rows(0)("changeDateTime"))
            Dim expireDate As DateTime = lastChangeTime.AddDays(cfg.passwordMaxAge)

            If Now >= expireDate Then
                pwr.isPass = False
                pwr.msg = $"密碼已使用超過 {cfg.passwordMaxAge} 天，請立即變更密碼。"
                Return pwr
            Else
                pwr.isPass = True
                Return pwr
            End If
        End Function
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
                           ua.govID,
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
                           ua.govID,
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

        Function Get_LoginFailLogs(accountID As Integer, since As DateTime, logItem As String) As Data.DataTable
            Dim sql =
                <sql>
                    select logDateTime, memo
                    from System_UserLog
                    where accountID = @accountID
                      and logItem = @logItem
                      and memo like @memoPrefix
                      and logDateTime >= @since
                    order by logDateTime desc
                </sql>.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter) From {
                New Data.SqlClient.SqlParameter("accountID", accountID),
                New Data.SqlClient.SqlParameter("logItem", logItem),
                New Data.SqlClient.SqlParameter("memoPrefix", "登入失敗%"),
                New Data.SqlClient.SqlParameter("since", since)
            }

            Using da As New DataAccess.MS_SQL
                Return da.GetDataTable(sql, para.ToArray)
            End Using
        End Function

        Function InsertSystemAccount(auTypeID As Integer, account As String, password As String, name As String,
                                     email As String, unit As String, mobile As String, memo As String,
                                     insertAccountID As Integer, govCityID As Integer?) As Integer

            Dim sql =
                <sql>
                    insert into System_UserAccount
                        (auTypeID, govID, account, password, name, email, unit, mobile, memo,
                         isActive, isEmailVerified, insertDateTime, insertAccountID)
                    values
                        (@auTypeID, @govID, @account, @password, @name, @email, @unit, @mobile, @memo,
                         @isActive, @isEmailVerified, @insertDateTime, @insertAccountID);
                    select scope_identity();
                </sql>.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter) From {
                New Data.SqlClient.SqlParameter("auTypeID", auTypeID),
                New Data.SqlClient.SqlParameter("govID", If(govCityID.HasValue, CType(govCityID.Value, Object), CType(DBNull.Value, Object))),
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
                                isActive As Boolean, updateAccountID As Integer, govCityID As Integer?)

            Dim sql =
                <sql>
                    update System_UserAccount set
                        auTypeID=@auTypeID,
                        govID=@govID,
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
                New Data.SqlClient.SqlParameter("govID", If(govCityID.HasValue, CType(govCityID.Value, Object), CType(DBNull.Value, Object))),
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

        ''' <summary>
        ''' 取得系統設定
        ''' </summary>
        ''' <returns>DataTable</returns>
        Function Get_SystemConfig() As Data.DataTable
            Dim sqlString =
                <xml sql="
               SELECT TOP 1
                    passwordMinLength,
                    requireUppercase,
                    requireLowercase,
                    requireNumbers,
                    requireSymbols,
                    passwordMaxAge,
                    passwordHistoryCount,
                    passwordMinAge,
                    maxFailAttempts,
                    lockoutDuration,
                    updateDateTime,
                    updateUserID
                FROM dbo.System_Config
            "></xml>.FirstAttribute.Value

            Using da As New DataAccess.MS_SQL
                Return da.GetDataTable(sqlString)
            End Using
        End Function
        ''' <summary>
        ''' 取得使用者密碼歷史紀錄（最新 N 筆）
        ''' </summary>
        Public Function Get_UserPasswordHistory(accountID As Integer, topCount As Integer) As Data.DataTable
            Dim sqlString =
                <xml sql="
                SELECT TOP (@topCount)
                    pwdHash, changeDateTime
                FROM System_UserCPWLog
                WHERE accountID = @accountID
                ORDER BY changeDateTime DESC
            "></xml>.FirstAttribute.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("accountID", accountID))
            para.Add(New Data.SqlClient.SqlParameter("topCount", topCount))

            Using da As New DataAccess.MS_SQL
                Return da.GetDataTable(sqlString, para.ToArray)
            End Using
        End Function

        ''' <summary>
        ''' 新增使用者密碼變更紀錄
        ''' </summary>
        ''' <param name="accountID">被變更帳號 ID</param>
        ''' <param name="exAccountID">執行變更者帳號 ID（自己、管理員、系統）</param>
        ''' <param name="pwdHash">密碼雜湊值</param>
        ''' <param name="changeType">變更類型</param>
        ''' <remarks></remarks>
        Sub Insert_UserPasswordLog(accountID As Integer, exAccountID As Integer, pwdHash As String, changeType As String)
            Dim sqlString =
                <xml sql="
                    INSERT INTO System_UserCPWLog (
                        accountID,
                        exAccountID,
                        pwdHash,
                        changeType,
                        changeDateTime
                    )
                    VALUES (
                        @accountID,
                        @exAccountID,
                        @pwdHash,
                        @changeType,
                        @changeDateTime
                    )
                "></xml>.FirstAttribute.Value


            Dim para As New List(Of Data.SqlClient.SqlParameter)

            para.Add(New Data.SqlClient.SqlParameter("accountID", accountID))
            para.Add(New Data.SqlClient.SqlParameter("exAccountID", exAccountID))
            para.Add(New Data.SqlClient.SqlParameter("pwdHash", pwdHash))
            para.Add(New Data.SqlClient.SqlParameter("changeType", changeType))
            para.Add(New Data.SqlClient.SqlParameter("changeDateTime", Now))

            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sqlString, para.ToArray)
            End Using
        End Sub

    End Class
End Namespace


