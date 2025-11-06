Imports System.Data.SqlTypes
Imports System.Web.Management

Namespace taifCattle

    Public Class WS_Base

        Dim taifCattle_base As New taifCattle.Base

        Enum enum_dataSourceName
            'in
            中央畜產會
            農業部
            農業保險基金

            'out
            外部單位
        End Enum

        Enum enum_apiName
            'in
            溯源屠宰資料
            耳標發放資料
            畜牧場資料
            投保資料
            化製資料
            勸售資料
            撲殺資料

            'out
            資料介接身分驗證服務
            指定牛籍編號旅程查詢服務
            滿一歲未投保乳母牛清單查詢服務
            撲殺補償牛籍編號清單查詢服務
        End Enum

        Enum enum_triggerType
            手動
            自動
            其他
        End Enum

        Enum enum_actionType
            dataIn
            dataOut
        End Enum

        ''' <summary>
        ''' 寫入 DataExchangeLog 紀錄
        ''' </summary>
        ''' <param name="dataSourceName">資料來源名稱</param>
        ''' <param name="apiName">API名稱</param>
        ''' <param name="queryMemo">查詢備註</param>
        ''' <param name="dataCount">處理的資料筆數</param>
        ''' <param name="triggerType">觸發類型</param>
        ''' <param name="isError">是否發生錯誤</param>
        ''' <param name="errorString">錯誤訊息</param>
        Public Sub Insert_DataExchangeLog(dataSourceName As enum_dataSourceName,
                                          apiName As enum_apiName,
                                          queryMemo As String,
                                          dataCount As Integer,
                                          triggerType As enum_triggerType,
                                          isError As Boolean,
                                          Optional errorString As String = Nothing,
                                          Optional actionType As enum_actionType = enum_actionType.dataIn)
            Dim sqlString As String =
                    "INSERT INTO Data_DataExchangeLog " &
                    "(logDateTime, dataSourceName, apiName, dataCount, triggerType, queryMemo, isError, errorString, ip, actionType) " &
                    "VALUES (@logDateTime, @dataSourceName, @apiName, @dataCount, @triggerType, @queryMemo, @isError, @errorString, @ip, @actionType)"

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("@logDateTime", Now))
            para.Add(New Data.SqlClient.SqlParameter("@dataSourceName", dataSourceName.ToString()))
            para.Add(New Data.SqlClient.SqlParameter("@apiName", apiName.ToString()))
            para.Add(New Data.SqlClient.SqlParameter("@dataCount", dataCount))
            para.Add(New Data.SqlClient.SqlParameter("@triggerType", triggerType.ToString()))
            para.Add(New Data.SqlClient.SqlParameter("@queryMemo", queryMemo.ToString()))
            para.Add(New Data.SqlClient.SqlParameter("@isError", isError))
            para.Add(New Data.SqlClient.SqlParameter("@ip", taifCattle_base.Get_IP()))
            para.Add(New Data.SqlClient.SqlParameter("@actionType", actionType.ToString()))
            If String.IsNullOrEmpty(errorString) Then
                para.Add(New Data.SqlClient.SqlParameter("@errorString", DBNull.Value))
            Else
                para.Add(New Data.SqlClient.SqlParameter("@errorString", errorString))
            End If
            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sqlString, para.ToArray)
            End Using
        End Sub

        ''' <summary>
        ''' 檢查token是否通過且存活(token字串正確+時間15分鐘內)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Check_IsTokenAlive(token As String) As Boolean
            Dim sqlString As String =
                "select * from Data_WS_Token " &
                "where token = @token and isPass = 1 and getDate() <= DATEADD(MINUTE,15,logDateTime) " &
                "order by logDateTime desc "

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("token", token))
            Using da As New DataAccess.MS_SQL
                If da.GetDataTable(sqlString, para.ToArray).Rows.Count > 0 Then
                    Return True
                Else
                    Return False
                End If
            End Using
        End Function

    End Class

    Public Class WS_Verify

        Dim taifCattle_base As New taifCattle.Base

        Property isPass As Boolean
        Property passCode As String

        Sub New()
            isPass = False
            passCode = "You shall not pass!!"
        End Sub

        ''' <summary>
        ''' 檢查帳號密碼是否確
        ''' </summary>
        ''' <returns></returns>
        Private Function Check_AcPw(ac As String, pw As String) As Boolean
            Dim sqlString As String = "select * from Data_WS_Identity where ac = @ac and pw = @pw"
            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("ac", ac))
            para.Add(New Data.SqlClient.SqlParameter("pw", pw))

            Using da As New DataAccess.MS_SQL
                Dim dt As Data.DataTable = da.GetDataTable(sqlString, para.ToArray())
                If dt.Rows.Count > 0 Then
                    Return True
                Else
                    Return False
                End If
            End Using

        End Function

        ''' <summary>
        ''' 寫入LOG並取得TOKEN
        ''' </summary>
        ''' <param name="account"></param>
        ''' <param name="pw"></param>
        ''' <param name="ispass"></param>
        ''' <param name="token"></param>
        ''' <param name="ip"></param>
        ''' <remarks></remarks>
        Private Sub Insert_Log_token(account As String, pw As String, ispass As Boolean, token As Object, ip As String)
            Dim sqlString As String =
                "insert into Data_WS_Token (account, pw, isPass, token, logDateTime, ip) values (@account, @pw, @isPass, @token, @logDateTime, @ip)"
            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("account", account))
            para.Add(New Data.SqlClient.SqlParameter("pw", pw))
            para.Add(New Data.SqlClient.SqlParameter("isPass", ispass))
            para.Add(New Data.SqlClient.SqlParameter("token", token))
            para.Add(New Data.SqlClient.SqlParameter("ip", ip))
            para.Add(New Data.SqlClient.SqlParameter("logDateTime", Now))
            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sqlString, para.ToArray)
            End Using
        End Sub

        Function Verify_Identity(ac As String, pw As String) As WS_Verify
            '不論驗證成功與否，寫入LOG
            Dim verifyResult As New WS_Verify
            Dim token As String = "iftWebService" & Now.ToString("yyyyMMddHHmmss") & "forTAIF"
            If Check_AcPw(ac, pw) Then
                Using md5Hash As System.Security.Cryptography.MD5 = System.Security.Cryptography.MD5.Create()
                    token = taifCattle_base.Convert_MD5(md5Hash, token)
                End Using
                verifyResult.isPass = True
                verifyResult.passCode = token '回傳Token
                Insert_Log_token(ac, pw, True, token, taifCattle_base.Get_IP)
            Else
                verifyResult.isPass = False
                verifyResult.passCode = "You shall not pass!!"
                Insert_Log_token(ac, pw, False, DBNull.Value, taifCattle_base.Get_IP)
            End If
            Return verifyResult
        End Function

    End Class

    Public Class WS_CattleHistory
        Dim taifCattle_cattle As New taifCattle.Cattle
        Dim taifCattle_wsBase As New taifCattle.WS_Base

        Structure stru_cattleHistory
            Property historyDate As Date
            Property historyType As String
            Property historyDetail As String
            Property placeType As String
            Property placeCounty As String
            Property placeDist As String
            Property placeCode As String
            Property placeName As String
            Property placeOwner As String
        End Structure

        Property tagNo As String
        Property birthYear As String
        Property cattleAge As String
        Property cattleType As String
        Property cattleFormat As String
        Property cattleStatus As String
        Property cattleLifeHistory As List(Of stru_cattleHistory)
        Property isCattleExist As Boolean
        Property isTokenPass As Boolean

        Sub New()
            tagNo = ""
            birthYear = ""
            cattleAge = ""
            cattleType = ""
            cattleFormat = ""
            cattleStatus = ""
            cattleLifeHistory = Nothing
            isCattleExist = False
            isTokenPass = False
        End Sub

        Function Get_CattleHistory(token As String, tagNo As String) As WS_CattleHistory
            Dim result As New WS_CattleHistory
            result.tagNo = tagNo

            '=== 先檢查TOKEN
            If taifCattle_wsBase.Check_IsTokenAlive(token) = False Then
                Return result
            End If

            '=== TOKEN通過才能取資料
            Dim chk_result As taifCattle.Base.stru_checkResult = taifCattle_cattle.Check_IsCattleExist(tagNo)

            If chk_result.isPass = True Then
                Dim cattleID As Integer = chk_result.msg
                Dim cattleInfo As taifCattle.Cattle.stru_cattleInfo_view = taifCattle_cattle.Get_CattleInfo(cattleID)
                Dim cattleHisInfo As List(Of taifCattle.Cattle.stru_cattleHistory_view) = taifCattle_cattle.Get_CattleHistoryList(cattleID, taifCattle.Cattle.enum_hisType.allHis)

                result.isCattleExist = True
                result.isTokenPass = True
                result.birthYear = cattleInfo.birthYear
                result.cattleAge = cattleInfo.cattleAge
                result.cattleType = cattleInfo.groupName
                result.cattleFormat = cattleInfo.typeName
                result.cattleStatus = cattleInfo.cattleStatus


                Dim list_his As New List(Of stru_cattleHistory)
                For Each item In cattleHisInfo
                    Dim cattleHis As New stru_cattleHistory
                    cattleHis.historyDate = item.dataDate
                    cattleHis.historyType = item.groupName
                    cattleHis.historyDetail = item.typeName
                    cattleHis.placeType = item.placeType
                    cattleHis.placeCounty = item.city
                    cattleHis.placeDist = item.area
                    cattleHis.placeCode = item.placeCode
                    cattleHis.placeName = item.placeName
                    cattleHis.placeOwner = item.placeOwner
                    list_his.Add(cattleHis)
                Next
                result.cattleLifeHistory = list_his

            End If

            Return result
        End Function

    End Class

    Public Class WS_CattleUninsured
        Dim taifCattle_wsBase As New taifCattle.WS_Base
        ''' <summary>
        ''' 滿一歲未投保乳牛資料結構
        ''' </summary>
        Public Structure stru_UninsuredCattle
            Public city As String
            Public area As String
            Public address As String
            Public farmCode As String
            Public farmName As String
            Public tagNo As String
            Public birthYear As Integer
            Public cattleAge As Integer
            Public milkProduction As Decimal
        End Structure

        ''' <summary>
        ''' 查詢結果    
        ''' </summary>
        Public Structure stru_resultWithList
            Public isPass As Boolean
            Public msg As String
            Public dataList As List(Of stru_UninsuredCattle)
        End Structure

        ''' <summary>
        ''' 取得滿一歲未投保乳牛清單
        ''' </summary>
        ''' <returns>List(Of stru_UninsuredCattle)</returns>
        Function Get_UninsuredCattleList() As List(Of stru_UninsuredCattle)
            Dim sqlString As String =
                <xml>
                    select twFarm.city,twFarm.area,List_Farm.address,List_Farm.farmCode,List_Farm.farmName,
                           vc.tagNo,vc.birthYear,vc.cattleAge,vc.milkProduction
                    from View_CattleList vc
                    OUTER APPLY (
                        SELECT TOP 1 
                            histNonRemoval.hisTypeID,
                            histNonRemoval.dataDate,
                            histNonRemoval.farmID,
                            histNonRemoval.memo
                        FROM Cattle_History AS histNonRemoval
                        INNER JOIN Cattle_TypeHistory AS typeRefNonRemoval
                            ON histNonRemoval.hisTypeID = typeRefNonRemoval.hisTypeID
                        WHERE histNonRemoval.cattleID = vc.cattleID
                          AND vc.groupName &lt;&gt; N'除籍'
                          AND histNonRemoval.removeDateTime IS NULL
                        ORDER BY histNonRemoval.dataDate DESC, typeRefNonRemoval.orderby DESC
                    ) AS latestNonRemoval
                    LEFT JOIN View_CattleInsClaStatus vIns ON vc.tagNo = vIns.tagNo
                    LEFT JOIN List_Farm ON latestNonRemoval.farmID = List_Farm.farmID
                    LEFT JOIN System_Taiwan AS twFarm  ON List_Farm.twID = twFarm.twID
                    where vc.cattleTypeID = 2 
                      and vc.cattleAge &gt;= 1 
                      and isnull(vIns.isInsurance,0) = 0 
                      and vc.cattleStatus = N'未除籍'
                    order by twFarm.twID,List_Farm.farmID,tagNo
        </xml>.Value

            Dim result As New List(Of stru_UninsuredCattle)
            Dim dt As New Data.DataTable

            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString)
            End Using

            For Each row As Data.DataRow In dt.Rows
                Dim info As New stru_UninsuredCattle
                info.city = "" & row("city")
                info.area = "" & row("area")
                info.address = "" & row("address")
                info.farmCode = "" & row("farmCode")
                info.farmName = "" & row("farmName")
                info.tagNo = "" & row("tagNo")
                info.birthYear = If(IsDBNull(row("birthYear")), 0, CInt(row("birthYear")))
                info.cattleAge = If(IsDBNull(row("cattleAge")), 0, CInt(row("cattleAge")))
                info.milkProduction = If(IsDBNull(row("milkProduction")), 0D, CDec(row("milkProduction")))
                result.Add(info)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 檢查 Token 後撈取乳牛清單
        ''' </summary>
        ''' <param name="token">驗證用 Token</param>
        ''' <returns>stru_resultWithList</returns>
        Function Get_UninsuredCattleList_ByToken(token As String) As stru_resultWithList
            Dim result As New stru_resultWithList With {
                .isPass = False,
                .msg = "驗證失敗",
                .dataList = Nothing
            }

            Try
                '=== 先檢查TOKEN
                If taifCattle_wsBase.Check_IsTokenAlive(token) = False Then
                    result.msg = "Token 已失效或不存在"
                    Return result
                End If

                '=== TOKEN通過才能取資料
                Dim list As List(Of stru_UninsuredCattle) = Get_UninsuredCattleList()

                If list IsNot Nothing AndAlso list.Count > 0 Then
                    result.isPass = True
                    result.msg = $"成功取得 {list.Count} 筆資料"
                    result.dataList = list
                Else
                    result.isPass = True
                    result.msg = "查無符合條件資料"
                    result.dataList = New List(Of stru_UninsuredCattle)
                End If

            Catch ex As Exception
                result.isPass = False
                result.msg = "系統發生錯誤：" & ex.Message
                result.dataList = Nothing
            End Try

            Return result
        End Function
    End Class


End Namespace
