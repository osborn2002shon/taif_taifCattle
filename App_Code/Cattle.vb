Imports Microsoft.VisualBasic

Namespace taifCattle

    ''' <summary>
    ''' Class Cattle
    ''' </summary>
    Partial Public Class Cattle

        Dim taifCattle_Base As New taifCattle.Base

#Region "Structure / Enum"

        ''' <summary>
        ''' stru stru_cattleInfo
        ''' </summary>
        Public Structure stru_cattleInfo
            Public cattleID As Integer
            Public cattleTypeID As Integer
            Public tagNo As String
            Public tagMemo As String
            Public birthYear As Object
            Public cattleMemo As String
            Public insertType As taifCattle.Base.enum_InsertType
            Public insertDateTime As Date
            Public insertAccountID As Integer
            Public updateDateTime As Date
            Public updateAccountID As Integer
            Public removeDateTime As Object
            Public removeAccountID As Object
        End Structure

        ''' <summary>
        ''' stru stru_cattleHistory
        ''' </summary>
        Public Structure stru_cattleHistory
            Public hisID As Integer
            Public cattleID As Integer
            Public hisTypeID As Integer
            Public dataDate As Date
            Public farmID As Object
            Public plantID As Object
            Public slauID As Object
            Public memo As Object
            Public insertType As taifCattle.Base.enum_InsertType
            Public insertDateTime As Date
            Public insertAccountID As String
            Public removeDateTime As Object
            Public removeAccountID As Object
        End Structure

        ''' <summary>
        ''' stru View_CattleList
        ''' </summary>
        Public Structure stru_cattleInfo_view
            Property cattleID As Integer
            Property tagNo As String
            Property birthYear As Object
            Property groupOrder As Object
            Property groupName As String
            Property cattleTypeID As Integer
            Property typeName As String
            Property cattleStatus As String
            Property cattleAge As Object
            Property cattleMemo As String
            Property tagMemo As String
            Property insertType As taifCattle.Base.enum_InsertType
            Property insertDateTime As Date
            Property milkProduction As Decimal
        End Structure

        ''' <summary>
        ''' enum HisType
        ''' </summary>
        Enum enum_hisType
            allHis
            defHis
            endHis
        End Enum

        ''' <summary>
        ''' stru View_CattleHistory
        ''' </summary>
        Public Structure stru_cattleHistory_view
            Property hisID As Integer
            Property cattleID As Integer
            Property dataDate As Date
            Property hisTypeID As Integer
            Property groupName As String
            Property typeName As String
            Property placeType As String
            Property placeTwID As Integer
            Property city As String
            Property area As String
            Property placeID As Integer
            Property placeName As String
            Property placeCode As String
            Property placeOwner As String
            Property memo As String
            Property insertType As taifCattle.Base.enum_InsertType
            Property insertDateTime As Date
        End Structure


        Public Structure stru_cattleInsClaStatus
            Property tagNo As String
            Property isInsurance As Boolean
            Property insDate_beg As Date
            Property insDate_end As Date
            Property isClaim As Boolean
            Property deadDate As Object
            Property saleDate As Object
        End Structure

#End Region

        ''' <summary>
        ''' 取得牛籍清單(標準)
        ''' </summary>
        ''' <param name="groupOrder"></param>
        ''' <param name="cattleTypeID"></param>
        ''' <param name="cattleStatus"></param>
        ''' <param name="tagNo"></param>
        ''' <param name="birthYear"></param>
        ''' <param name="cattleAge"></param>
        ''' <returns></returns>
        Function Get_CattleList(groupOrder As String, cattleTypeID As String, cattleStatus As String, tagNo As String, birthYear As String, cattleAge As String) As Data.DataTable
            Dim sqlString As String =
                <xml sql="
                    select * from View_CattleList 
                    where 
                        groupOrder like @groupOrder and 
                        cattleTypeID like @cattleTypeID and 
                        cattleStatus like @cattleStatus and 
                        tagNo like @tagNo and
                        cattleAge like @cattleAge and 
                        birthYear like @birthYear
                    order by cattleID desc
                "></xml>.FirstAttribute.Value
            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("groupOrder", groupOrder))
            para.Add(New Data.SqlClient.SqlParameter("cattleTypeID", cattleTypeID))
            para.Add(New Data.SqlClient.SqlParameter("cattleStatus", cattleStatus))
            para.Add(New Data.SqlClient.SqlParameter("tagNo", tagNo))
            para.Add(New Data.SqlClient.SqlParameter("cattleAge", cattleAge))
            para.Add(New Data.SqlClient.SqlParameter("birthYear", birthYear))
            Using da As New DataAccess.MS_SQL
                Return da.GetDataTable(sqlString, para.ToArray())
            End Using
        End Function

        ''' <summary>
        ''' 檢查牛籍編號是否已存在
        ''' </summary>
        ''' <param name="tagNo"></param>
        ''' <returns></returns>
        Function Check_IsCattleExist(tagNo As String) As taifCattle.Base.stru_checkResult
            Dim sqlString As String = "select * from Cattle_List where tagNo = @tagNo and removeDateTime is null "
            Dim para As New Data.SqlClient.SqlParameter("tagNo", tagNo)
            Dim result As New taifCattle.Base.stru_checkResult
            Using da As New DataAccess.MS_SQL
                Dim dt As Data.DataTable = da.GetDataTable(sqlString, para)
                If dt.Rows.Count > 0 Then
                    result.isPass = True
                    result.msg = dt.Rows(0)("cattleID")
                Else
                    result.isPass = False
                    result.msg = "-1"
                End If
            End Using
            Return result
        End Function

        ''' <summary>
        ''' 新增牛籍基本資料
        ''' </summary>
        ''' <param name="cattleInfo"></param>
        Function Insert_Cattle(cattleInfo As stru_cattleInfo) As Integer
            Dim sqlString As String =
            <xml sql="
                insert into Cattle_List
                    (cattleTypeID, tagNo, tagMemo, birthYear, cattleMemo, 
                     insertType, insertDateTime, insertAccountID, updateDateTime, updateAccountID, removeDateTime, removeAccountID)
                values
                    (@cattleTypeID, @tagNo, @tagMemo, @birthYear, @cattleMemo, 
                     @insertType, @insertDateTime, @insertAccountID, @updateDateTime, @updateAccountID, @removeDateTime, @removeAccountID)
                select scope_identity()
            "></xml>.FirstAttribute.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("cattleTypeID", cattleInfo.cattleTypeID))
            para.Add(New Data.SqlClient.SqlParameter("tagNo", cattleInfo.tagNo))
            para.Add(New Data.SqlClient.SqlParameter("tagMemo", taifCattle_Base.Convert_EmptyToObject(cattleInfo.tagMemo, DBNull.Value)))
            para.Add(New Data.SqlClient.SqlParameter("birthYear", taifCattle_Base.Convert_EmptyToObject(cattleInfo.birthYear, DBNull.Value)))
            para.Add(New Data.SqlClient.SqlParameter("cattleMemo", taifCattle_Base.Convert_EmptyToObject(cattleInfo.cattleMemo, DBNull.Value)))
            para.Add(New Data.SqlClient.SqlParameter("insertType", cattleInfo.insertType.ToString))
            para.Add(New Data.SqlClient.SqlParameter("insertDateTime", cattleInfo.insertDateTime))
            para.Add(New Data.SqlClient.SqlParameter("insertAccountID", cattleInfo.insertAccountID))
            para.Add(New Data.SqlClient.SqlParameter("updateDateTime", cattleInfo.updateDateTime))
            para.Add(New Data.SqlClient.SqlParameter("updateAccountID", cattleInfo.updateAccountID))
            para.Add(New Data.SqlClient.SqlParameter("removeDateTime", DBNull.Value))
            para.Add(New Data.SqlClient.SqlParameter("removeAccountID", DBNull.Value))

            Using da As New DataAccess.MS_SQL
                Return da.ExecuteScalar(sqlString, para.ToArray())
            End Using
        End Function

        ''' <summary>
        ''' 更新牛籍基本資料
        ''' </summary>
        ''' <param name="cattleInfo"></param>
        Sub Update_Cattle(cattleInfo As stru_cattleInfo)
            Dim sqlString As String =
                <xml sql="
                    update Cattle_List set
                        cattleTypeID   = @cattleTypeID, tagMemo = @tagMemo, birthYear = @birthYear, cattleMemo = @cattleMemo,
                        updateDateTime = @updateDateTime, updateAccountID= @updateAccountID
                    where cattleID = @cattleID
                "></xml>.FirstAttribute.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("cattleTypeID", cattleInfo.cattleTypeID))
            para.Add(New Data.SqlClient.SqlParameter("tagMemo", taifCattle_Base.Convert_EmptyToObject(cattleInfo.tagMemo, DBNull.Value)))
            para.Add(New Data.SqlClient.SqlParameter("birthYear", taifCattle_Base.Convert_EmptyToObject(cattleInfo.birthYear, DBNull.Value)))
            para.Add(New Data.SqlClient.SqlParameter("cattleMemo", taifCattle_Base.Convert_EmptyToObject(cattleInfo.cattleMemo, DBNull.Value)))
            para.Add(New Data.SqlClient.SqlParameter("updateDateTime", cattleInfo.updateDateTime))
            para.Add(New Data.SqlClient.SqlParameter("updateAccountID", cattleInfo.updateAccountID))
            para.Add(New Data.SqlClient.SqlParameter("cattleID", cattleInfo.cattleID))
            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sqlString, para.ToArray())
            End Using
        End Sub

        ''' <summary>
        ''' 取得牛籍基本資料
        ''' </summary>
        ''' <param name="cattleID"></param>
        ''' <returns></returns>
        Function Get_CattleInfo(cattleID As Integer) As stru_cattleInfo_view
            Dim sqlString As String =
                <xml sql="select * from View_CattleList where cattleID = @cattleID"></xml>.FirstAttribute.Value
            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("cattleID", cattleID))
            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString, para.ToArray())
            End Using
            Dim cattleInfo As New stru_cattleInfo_view
            If dt.Rows.Count > 0 Then
                Dim row As Data.DataRow = dt.Rows(0)
                cattleInfo.cattleID = cattleID
                cattleInfo.tagNo = row("tagNo")
                cattleInfo.birthYear = CInt(row("birthYear"))
                cattleInfo.groupOrder = row("groupOrder")
                cattleInfo.groupName = row("groupName")
                cattleInfo.cattleTypeID = row("cattleTypeID")
                cattleInfo.typeName = row("typeName")
                cattleInfo.cattleStatus = row("cattleStatus")
                cattleInfo.cattleAge = row("cattleAge")
                cattleInfo.cattleMemo = "" & row("cattleMemo")
                cattleInfo.tagMemo = "" & row("tagMemo")
                cattleInfo.insertType = taifCattle_Base.Convert_ValueToEnum(GetType(taifCattle.Base.enum_InsertType), row("insertType"))
                cattleInfo.insertDateTime = CDate(row("insertDateTime"))
                cattleInfo.milkProduction = row("milkProduction")
            End If
            Return cattleInfo
        End Function

        ''' <summary>
        ''' 取得旅程紀錄清單(旅程/除籍)
        ''' </summary>
        ''' <param name="cattleID"></param>
        ''' <param name="queryType"></param>
        ''' <returns></returns>
        Function Get_CattleHistoryList(cattleID As Integer, queryType As enum_hisType) As List(Of stru_cattleHistory_view)
            '排序取消使用insertDateTime，因為有可能補資料，直接用狀態的參考排序去排
            Dim groupName As String = ""
            Select Case queryType
                Case enum_hisType.allHis
                    groupName = "%"
                Case enum_hisType.defHis
                    groupName = "旅程"
                Case enum_hisType.endHis
                    groupName = "除籍"
            End Select
            Dim list As New List(Of stru_cattleHistory_view)
            Dim sqlString As String =
                <xml sql="
                    select * from View_CattleHistory
                    where cattleID = @cattleID and groupName like @groupName
                    order by dataDate, orderBy
                "></xml>.FirstAttribute.Value
            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("cattleID", cattleID))
            para.Add(New Data.SqlClient.SqlParameter("groupName", groupName))

            Using da As New DataAccess.MS_SQL
                Dim dt As Data.DataTable = da.GetDataTable(sqlString, para.ToArray())

                For Each row As Data.DataRow In dt.Rows
                    Dim item As New stru_cattleHistory_view
                    item.hisID = row("hisID")
                    item.cattleID = row("cattleID")
                    item.dataDate = row("dataDate")
                    item.hisTypeID = row("hisTypeID")
                    item.groupName = row("groupName")
                    item.typeName = row("typeName")
                    item.placeType = taifCattle_Base.Convert_DBNullToString(row("placeType"), "-")
                    item.placeTwID = taifCattle_Base.Convert_DBNullToString(row("placeTwID"), -1)
                    item.city = taifCattle_Base.Convert_DBNullToString(row("city"), "-")
                    item.area = taifCattle_Base.Convert_DBNullToString(row("area"), "-")
                    item.placeID = taifCattle_Base.Convert_DBNullToString(row("placeID"), -1)
                    item.placeName = taifCattle_Base.Convert_DBNullToString(row("placeName"), "-")
                    item.placeCode = taifCattle_Base.Convert_DBNullToString(row("placeCode"), "-")
                    item.placeOwner = taifCattle_Base.Convert_DBNullToString(row("placeOwner"), "-")
                    item.memo = "" & row("memo")
                    item.insertType = taifCattle_Base.Convert_ValueToEnum(GetType(taifCattle.Base.enum_InsertType), row("insertType"))
                    item.insertDateTime = row("insertDateTime")
                    list.Add(item)
                Next
            End Using
            Return list
        End Function

        ''' <summary>
        ''' 新增牛籍旅程紀錄(旅程/除籍)
        ''' </summary>
        ''' <param name="hisInfo"></param>
        Function Insert_CattleHistory(hisInfo As stru_cattleHistory) As Integer
            Dim sqlString As String =
                <xml sql="
                    INSERT INTO Cattle_History
                        (cattleID, hisTypeID, dataDate, farmID, plantID, slauID, memo, insertType, insertDateTime, insertAccountID)
                    VALUES
                        (@cattleID, @hisTypeID, @dataDate, @farmID, @plantID, @slauID, @memo, @insertType, @insertDateTime, @insertAccountID);
                    SELECT SCOPE_IDENTITY();
                "></xml>.FirstAttribute.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("cattleID", hisInfo.cattleID))
            para.Add(New Data.SqlClient.SqlParameter("hisTypeID", hisInfo.hisTypeID))
            para.Add(New Data.SqlClient.SqlParameter("dataDate", hisInfo.dataDate))
            para.Add(New Data.SqlClient.SqlParameter("farmID", taifCattle_Base.Convert_EmptyToObject(hisInfo.farmID, DBNull.Value)))
            para.Add(New Data.SqlClient.SqlParameter("plantID", taifCattle_Base.Convert_EmptyToObject(hisInfo.plantID, DBNull.Value)))
            para.Add(New Data.SqlClient.SqlParameter("slauID", taifCattle_Base.Convert_EmptyToObject(hisInfo.slauID, DBNull.Value)))
            para.Add(New Data.SqlClient.SqlParameter("memo", taifCattle_Base.Convert_EmptyToObject(hisInfo.memo, DBNull.Value)))
            para.Add(New Data.SqlClient.SqlParameter("insertType", hisInfo.insertType.ToString()))
            para.Add(New Data.SqlClient.SqlParameter("insertDateTime", hisInfo.insertDateTime))
            para.Add(New Data.SqlClient.SqlParameter("insertAccountID", hisInfo.insertAccountID))

            Using da As New DataAccess.MS_SQL
                Return da.ExecuteScalar(sqlString, para.ToArray())
            End Using
        End Function

        ''' <summary>
        ''' 刪除牛籍旅程紀錄(旅程/除籍)
        ''' </summary>
        ''' <param name="hisID"></param>
        ''' <param name="removeDateTime"></param>
        ''' <param name="removeAccountID"></param>
        Sub Remove_CattleHistroy(hisID As Integer, removeDateTime As Date, removeAccountID As Integer)
            Dim sqlString As String =
                <xml sql="
                    update Cattle_History set removeDateTime = @removeDateTime, removeAccountID = @removeAccountID where hisID = @hisID
                "></xml>.FirstAttribute.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("hisID", hisID))
            para.Add(New Data.SqlClient.SqlParameter("removeDateTime", removeDateTime))
            para.Add(New Data.SqlClient.SqlParameter("removeAccountID", removeAccountID))
            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sqlString, para.ToArray())
            End Using
        End Sub

        ''' <summary>
        ''' 檢查牛籍旅程是否重複
        ''' </summary>
        ''' <param name="cattleID"></param>
        ''' <param name="dataDate"></param>
        ''' <param name="placeID">farmID,slauID,plantID</param>
        ''' <returns></returns>
        Function Check_IsHistoryExist(cattleID As Integer, dataDate As Date, placeID As Integer) As Boolean
            Dim sqlString As String =
                <xml sql="
                    select * from Cattle_History
                    where cattleID = @cattleID and dataDate = @dataDate and COALESCE(farmID, slauID, plantID) = @placeID and removeDateTime is null
                "></xml>.FirstAttribute.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("cattleID", cattleID))
            para.Add(New Data.SqlClient.SqlParameter("dataDate", dataDate))
            para.Add(New Data.SqlClient.SqlParameter("placeID", placeID))

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
        ''' 取得保險與理賠最新資訊（資料來源：TAIF家畜保險系統）
        ''' </summary>
        ''' <param name="tagNo"></param>
        ''' <returns></returns>
        Function Get_InsClaimStatus(tagNo As String) As List(Of stru_cattleInsClaStatus)
            Dim list As New List(Of stru_cattleInsClaStatus)

            Dim sqlString As String =
                <xml sql="select * from View_CattleInsClaStatus where tagNo like @tagNo"></xml>.FirstAttribute.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("tagNo", tagNo))

            Using da As New DataAccess.MS_SQL
                Dim dt As Data.DataTable = da.GetDataTable(sqlString, para.ToArray())
                For Each row As Data.DataRow In dt.Rows
                    Dim item As New stru_cattleInsClaStatus
                    item.tagNo = row("tagNo")
                    item.isInsurance = row("isInsurance")
                    item.insDate_beg = row("insDate_beg")
                    item.insDate_end = row("insDate_end")
                    item.isClaim = row("isClaim")
                    item.deadDate = row("deadDate")
                    item.saleDate = row("saleDate")
                    list.Add(item)
                Next
            End Using
            Return list
        End Function


    End Class

End Namespace
