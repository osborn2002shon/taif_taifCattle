'Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Runtime.CompilerServices


Namespace taifCattle

    ''' <summary>
    ''' Class 基底函式庫
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public Class Base
        Inherits System.Web.UI.Page

        '主板號.次版號.修訂號.編譯日期
        '主板號：大幅度改版、合約變更、新合約
        '次版號：合約外功能調動、因應需求功能調整、應外部客戶要求調動之內容
        '修訂號：BUG FIX、非外部需求技術調動
        Public Shared Property ver As String = "1.0.0.251014"

        '========== ENUM
        ''' <summary>
        ''' enum_編輯模式
        ''' </summary>
        ''' <remarks></remarks>
        Enum enum_EditMode
            預設
            新增
            編輯
            唯讀
        End Enum

        Enum enum_InsertType
            人工網頁建檔
            人工批次建檔
            耳標來源建檔
            屠宰來源建檔
            牧登來源建檔
            投保來源建檔
            化製來源建檔
            勸售來源建檔
        End Enum

        ''' <summary>
        ''' enum 操作紀錄：功能項目
        ''' </summary>
        ''' <remarks></remarks>
        Enum enum_UserLogItem
            登入
            登出
            忘記密碼
            預設密碼變更
            定期密碼變更
            我的帳號管理
            系統帳號管理
            系統參數設定
            平均產乳量設定
            牧場資料管理
            牛籍資料管理
            牛籍批次新增功能
            旅程批次新增功能
            除籍批次設定功能

        End Enum

        ''' <summary>
        ''' 操作紀錄：動作類型
        ''' </summary>
        ''' <remarks></remarks>
        Enum enum_UserLogType
            新增
            刪除
            修改
            查詢
            下載
            其他
        End Enum

        '========== STRUCTURE
        ''' <summary>
        ''' stru 檢查結果
        ''' </summary>
        ''' <remarks></remarks>
        Structure stru_checkResult
            Dim isPass As Boolean
            Dim msg As String
        End Structure

        ''' <summary>
        ''' stru_使用者基本資料
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure stru_LoginUserInfo
            Property accountID As Integer
            Property auTypeID As enum_auType
            Property account As String
            Property name As String
            Property unit As String
            Property tel As String
            Property mail As String
            Property isActive As Boolean
            Property isExist As Boolean
            Property msg As String
            Property pwUpdateDateTime As Object
            Property lastLoginDateTime As Object
            Property insertDateTime As DateTime
            Property isEmailVerified As Boolean

            Property slauID As Integer
            Property govID As Integer
            Property govCityID As Integer
            Property govTwID As Integer
            Property govName As String
            Property farmID As Integer
            Property farmCode As String
            Property farmName As String
            Property slauCode_eng As String
            Property slauName_short As String

            Property liMenu As List(Of stru_MenuItem)
        End Structure

        ''' <summary>
        ''' stru_選單項目
        ''' </summary>
        Public Structure stru_MenuItem
            Property menuID As Integer
            Property groupName As String
            Property menuName As String
            Property menuURL As String
            Property iconClass As String
            Property orderBy_group As Integer
            Property orderBy_menu As Integer
            Property isActive As Boolean
            Property isShow As Boolean
            Property canRead As Boolean
        End Structure

        ''' <summary>
        ''' 系統帳號權限類型
        ''' </summary>
        ''' <remarks></remarks>
        Enum enum_auType
            最高管理者 = 1
            屠宰場 = 2
            縣市政府 = 3
            鄉鎮區公所 = 4
            畜牧場人員 = 5
            非畜牧場人員 = 6
            耳標維護管理者 = 7
        End Enum

        ''' <summary>
        ''' stru_項目結構
        ''' </summary>
        Public Structure stru_itemvalue
            Property itemText As String
            Property itemValue As Object
        End Structure

        '========== FUNCTION // SUB

        ''' <summary>
        ''' 取得使用者IP
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Get_IP() As String
            Dim clientIP As String = System.Web.HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If String.IsNullOrEmpty(clientIP) Then
                clientIP = System.Web.HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
            End If
            If String.IsNullOrEmpty(clientIP) Then
                clientIP = System.Web.HttpContext.Current.Request.UserHostAddress
            End If
            Return clientIP
        End Function

        ''' <summary>
        ''' 取得指定長度隨機字串
        ''' </summary>
        ''' <param name="strLength">字串長度</param>
        ''' <param name="isNeedUpper">是否需要大寫</param>
        ''' <returns></returns>
        Function Make_RandomString(strLength As Integer, Optional isNeedUpper As Boolean = False) As String
            '排除小寫的L，避免l跟1搞混
            Dim allowedChars As String = ""
            Select Case isNeedUpper
                Case True
                    'allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@_"
                    allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@_"
                Case False
                    'allowedChars = "abcdefghijklmnopqrstuvwxyz0123456789@_"
                    allowedChars = "abcdefghijkmnopqrstuvwxyz0123456789@_"
            End Select
            Dim listStr(strLength) As String
            Dim rd As New Random
            For i = 0 To strLength - 1
                listStr(i) = allowedChars(rd.Next(0, allowedChars.Length))
            Next
            Return String.Join("", listStr)
        End Function

        '========== USER_LOGIN
        ''' <summary>
        ''' 使用者登入
        ''' </summary>
        ''' <param name="account"></param>
        ''' <param name="pw_md5"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Login(account As String, pw_md5 As String) As stru_LoginUserInfo

            Dim userInfo As New stru_LoginUserInfo
            Dim sqlString As String =
                <xml xmlStr="
                    select 
                        System_UserAccount.*, 
                        case when System_UserAccount.isActive = 1 then '啟用' else '停用' end 'isActive_display',
                        slauName, isnull(slauName_short,slauName)'slauName_short', System_Taiwan.city 'slauCity', 
                        auTypeName
                    from System_UserAccount
                    left join List_Slaughterhouse on System_UserAccount.slauID = List_Slaughterhouse.slauID
                    left join System_Taiwan on List_Slaughterhouse.twID = System_Taiwan.twID
                    left join System_UserAuType on System_UserAccount.auTypeID = System_UserAuType.auTypeID
                    where (account = @account and password = @password) and removeDateTime is null
                "></xml>.FirstAttribute.Value
            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("account", account))
            para.Add(New Data.SqlClient.SqlParameter("password", pw_md5))
            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString, para.ToArray)
            End Using
            If dt.Rows.Count > 0 Then
                userInfo.isExist = True
                userInfo.msg = "登入成功！"
                userInfo.accountID = dt.Rows(0)("accountID")
                userInfo.auTypeID = dt.Rows(0)("auTypeID")
                userInfo.account = dt.Rows(0)("account")
                userInfo.name = dt.Rows(0)("name")
                userInfo.unit = "" & dt.Rows(0)("unit")
                userInfo.mail = dt.Rows(0)("email") & ""
                userInfo.tel = dt.Rows(0)("mobile") & ""
                userInfo.isActive = dt.Rows(0)("isActive")  '帳號啟用／停用狀態
                userInfo.pwUpdateDateTime = dt.Rows(0)("lastUpdatePWDateTime")
                userInfo.lastLoginDateTime = dt.Rows(0)("lastLoginDateTime")
                userInfo.insertDateTime = dt.Rows(0)("insertDateTime")
                userInfo.isEmailVerified = dt.Rows(0)("isEmailVerified")

                userInfo.slauID = Convert_DBNullToString(dt.Rows(0)("slauID"), -1)
                userInfo.govID = Convert_DBNullToString(dt.Rows(0)("govID"), -1)
                'userInfo.govCityID = Convert_DBNullToString(dt.Rows(0)("govCityID"), -1)
                'userInfo.govTwID = Convert_DBNullToString(dt.Rows(0)("govTwID"), -1)
                'userInfo.govName = Convert_DBNullToString(dt.Rows(0)("govName"), "無資料")
                'userInfo.farmID = Convert_DBNullToString(dt.Rows(0)("farmID"), -1)
                'userInfo.farmCode = Convert_DBNullToString(dt.Rows(0)("farmCode"), "無資料")
                'userInfo.farmName = Convert_DBNullToString(dt.Rows(0)("farmName"), "無資料")
                'userInfo.slauCode_eng = Convert_DBNullToString(dt.Rows(0)("slauCode_eng"), "＊")
                userInfo.slauName_short = Convert_DBNullToString(dt.Rows(0)("slauName_short"), "非屠宰場")

                '目錄
                userInfo.liMenu = Get_MenuData(userInfo.auTypeID)
            Else
                userInfo.isExist = False
                userInfo.msg = "帳號不存在或密碼不正確！"
                userInfo.accountID = -1
                userInfo.slauID = -1
                userInfo.account = account
                userInfo.name = ""
                userInfo.unit = ""
                userInfo.isActive = False
                userInfo.pwUpdateDateTime = DBNull.Value
                userInfo.lastLoginDateTime = DBNull.Value
                userInfo.insertDateTime = Date.MinValue
                userInfo.isEmailVerified = False

                userInfo.slauID = -1
                userInfo.govID = -1
                userInfo.govCityID = -1
                userInfo.govTwID = -1
                userInfo.govName = ""
                userInfo.farmID = -1
                userInfo.farmCode = ""
                userInfo.farmName = ""
                userInfo.slauCode_eng = "＊"
                userInfo.slauName_short = "非屠宰場"
                userInfo.liMenu = New List(Of stru_MenuItem)
            End If
            System.Web.HttpContext.Current.Session("UserInfo") = userInfo
            Return userInfo
        End Function

        ''' <summary>
        ''' 系統登出，並清除使用者基本資料
        ''' </summary>
        ''' <remarks></remarks>
        Sub Logout()
            Dim userInfo As stru_LoginUserInfo = System.Web.HttpContext.Current.Session("UserInfo")
            userInfo.isExist = False
            userInfo.msg = "您已登出！"
            userInfo.accountID = -1
            userInfo.auTypeID = -1
            userInfo.slauID = -1
            userInfo.govID = -1
            userInfo.govCityID = -1
            userInfo.govTwID = -1
            userInfo.govName = ""
            userInfo.farmID = -1
            userInfo.farmCode = ""
            userInfo.farmName = ""
            userInfo.slauCode_eng = ""
            userInfo.slauName_short = ""
            userInfo.account = ""
            userInfo.name = ""
            userInfo.unit = ""
            userInfo.isActive = False
            userInfo.pwUpdateDateTime = DBNull.Value
            userInfo.lastLoginDateTime = DBNull.Value
            userInfo.insertDateTime = Date.MinValue
            userInfo.isEmailVerified = False
            userInfo.liMenu = New List(Of stru_MenuItem)
            System.Web.HttpContext.Current.Session("UserInfo") = userInfo
        End Sub

        ''' <summary>
        ''' UserInfo的Session屬性
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property InfoSession As stru_LoginUserInfo
            Get
                If System.Web.HttpContext.Current.Session("UserInfo") Is Nothing Then
                    Dim userInfo As New stru_LoginUserInfo
                    userInfo.isExist = False
                    userInfo.msg = "無Session！"
                    userInfo.accountID = -1
                    userInfo.auTypeID = -1
                    userInfo.slauID = -1
                    userInfo.govID = -1
                    userInfo.govCityID = -1
                    userInfo.govTwID = -1
                    userInfo.govName = ""
                    userInfo.farmID = -1
                    userInfo.farmCode = ""
                    userInfo.farmName = ""
                    userInfo.slauCode_eng = ""
                    userInfo.slauName_short = ""
                    userInfo.account = ""
                    userInfo.name = ""
                    userInfo.unit = ""
                    userInfo.isActive = False
                    userInfo.pwUpdateDateTime = DBNull.Value
                    userInfo.lastLoginDateTime = DBNull.Value
                    userInfo.insertDateTime = Date.MinValue
                    userInfo.isEmailVerified = False
                    userInfo.liMenu = New List(Of stru_MenuItem)
                    Return userInfo
                Else
                    Return System.Web.HttpContext.Current.Session("UserInfo")
                End If
            End Get
        End Property

        ''' <summary>
        ''' 取得使用者可用選單資料（直接轉成 stru_MenuItem 結構清單）
        ''' </summary>
        ''' <param name="auTypeID">使用者角色代碼</param>
        ''' <returns>List(Of stru_MenuItem)</returns>
        Public Function Get_MenuData(auTypeID As Integer) As List(Of stru_MenuItem)
            Dim sqlString As String =
                "SELECT DISTINCT System_Menu.menuID, System_Menu.groupName, System_Menu.menuName, System_Menu.menuURL, " &
                "       System_Menu.iconClass, System_Menu.orderBy_group, System_Menu.orderBy_menu, System_Menu.isActive, System_Menu.isShow " &
                "FROM System_MenuAu " &
                "INNER JOIN System_Menu ON System_MenuAu.menuID = System_Menu.menuID " &
                "WHERE System_Menu.isActive = 1 AND System_MenuAu.auTypeID = @auTypeID " &
                "ORDER BY System_Menu.orderBy_group, System_Menu.orderBy_menu"

            Dim para As New Data.SqlClient.SqlParameter("@auTypeID", auTypeID)

            Dim menuList As New List(Of stru_MenuItem)

            Using da As New DataAccess.MS_SQL
                Dim dt As DataTable = da.GetDataTable(sqlString, para)

                For Each row As DataRow In dt.Rows
                    Dim item As New stru_MenuItem With {
                        .menuID = row.Field(Of Integer)("menuID"),
                        .groupName = row.Field(Of String)("groupName"),
                        .menuName = row.Field(Of String)("menuName"),
                        .menuURL = row.Field(Of String)("menuURL"),
                        .iconClass = If(row.IsNull("iconClass"), "fa-solid fa-angle-right", row.Field(Of String)("iconClass")),
                        .orderBy_group = row.Field(Of Integer)("orderBy_group"),
                        .orderBy_menu = row.Field(Of Integer)("orderBy_menu"),
                        .isActive = row.Field(Of Boolean)("isActive"),
                        .isShow = row.Field(Of Boolean)("isShow"),
                        .canRead = True
                    }
                    menuList.Add(item)
                Next
            End Using

            Return menuList
        End Function

        '========== USER_LOG
        ''' <summary>
        ''' 新增使用者操作紀錄
        ''' </summary>
        ''' <param name="accountID"></param>
        ''' <param name="logItem"></param>
        ''' <param name="logType"></param>
        ''' <param name="memo"></param>
        ''' <remarks></remarks>
        Sub Insert_UserLog(accountID As Integer, logItem As enum_UserLogItem, logType As enum_UserLogType, Optional memo As String = "", Optional insertDateTime As String = "")
            Dim sqlString As String =
                "insert into System_UserLog (accountID, logDateTime, IP, logItem, logType, memo) values " &
                "(@accountID, @logDateTime, @IP, @logItem, @logType, @memo)"

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("accountID", accountID))
            para.Add(New Data.SqlClient.SqlParameter("IP", Get_IP))
            para.Add(New Data.SqlClient.SqlParameter("logItem", logItem.ToString))
            para.Add(New Data.SqlClient.SqlParameter("logType", logType.ToString))
            para.Add(New Data.SqlClient.SqlParameter("memo", memo))
            If insertDateTime = "" Then
                para.Add(New Data.SqlClient.SqlParameter("logDateTime", Now))
            Else
                para.Add(New Data.SqlClient.SqlParameter("logDateTime", CDate(insertDateTime)))
            End If
            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sqlString, para.ToArray)
            End Using
        End Sub
        '========== Reg
        ''' <summary>
        ''' 檢查EMAIL格式是否正確
        ''' </summary>
        ''' <param name="mailString"></param>
        ''' <returns></returns>
        Function Check_Reg_Mail(mailString As String) As Boolean
            'https://docs.microsoft.com/zh-tw/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
            Dim emailRegString_MSDN As String =
                "^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                "(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$"
            Return Regex.IsMatch(mailString, emailRegString_MSDN, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))
        End Function

        ''' <summary>
        ''' 審核身份證字號是否符合規則
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Check_Reg_ID(ID As String) As Boolean
            '僅驗證字元位置，無驗算
            Dim IDCheck As New Regex("^[a-zA-Z]{1}[1-2]{1}[0-9]{8}$")
            Return IDCheck.IsMatch(ID)
        End Function

        ''' <summary>
        ''' 審核統一編號是否符合規則
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Check_Reg_TaxID(ID As String) As Boolean
            '僅驗證字元位置，無驗算
            Dim IDCheck As New Regex("^[0-9]{8}$")
            Return IDCheck.IsMatch(ID)
        End Function

        ''' <summary>
        ''' 檢查畜牧場證號
        ''' </summary>
        ''' <param name="farmID"></param>
        ''' <returns></returns>
        Function CheckFarmID(ByVal farmID As String) As Boolean
            '5碼、8碼、加上10碼身分證
            Dim reg As New Regex("^[0-9]{5}$|^[0-9]{8}$|^[a-zA-z]{1}[0-9]{9}$")
            Return reg.IsMatch(farmID)
        End Function

    End Class

End Namespace
