Imports System.Data
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace taifCattle
    Public Class Farm
        Dim taifCattle_base As New taifCattle.Base

        ''' <summary>
        ''' stru 畜牧場基本資料
        ''' </summary>
        Structure stru_farmInfo
            Property farmID As Integer
            Property farmName As String
            Property farmCode As String
            Property owner As String
            Property ownerID As String
            Property ownerTel As String
            Property twID As Integer
            Property cityID As Integer
            Property city As String
            Property town As String
            Property address As String
            Property animalCount As String
            Property memo As String
            Property insertType As taifCattle.Base.enum_InsertType
            Property insertAccountID As Integer
            Property updateAccountID As Integer
        End Structure
        Function MaskFarmCode(value As Object) As String
            If value Is Nothing OrElse IsDBNull(value) Then
                Return ""
            End If

            Dim code As String = value.ToString().Trim()
            If code.Length = 10 Then
                ' 例：A123456789 → A123***789
                Return code.Substring(0, 4) & "***" & code.Substring(7)
            End If

            ' 不是 10 碼就不處理
            Return code
        End Function
        ''' <summary>
        ''' 檢查畜牧場證號是否重複
        ''' </summary>
        ''' <param name="farmCode">牧場證號</param>
        ''' <param name="excludeFarmID">要排除的既有ID（編輯時用）</param>
        ''' <returns>True = 重複、False = 不重複</returns>
        Public Function Check_FarmCodeExists(farmCode As String, Optional excludeFarmID As Integer = 0) As Boolean
            Dim sql As String =
            <xml>
                SELECT COUNT(*) 
                FROM List_Farm 
                WHERE farmCode = @farmCode 
                  AND removeDateTime IS NULL
            </xml>.Value

            If excludeFarmID > 0 Then
                sql &= " AND farmID <> @farmID"
            End If

            Dim para As New List(Of SqlClient.SqlParameter) From {
                    New SqlClient.SqlParameter("@farmCode", farmCode)
                }
            If excludeFarmID > 0 Then
                para.Add(New SqlClient.SqlParameter("@farmID", excludeFarmID))
            End If

            Using da As New DataAccess.MS_SQL
                Dim count As Integer = Convert.ToInt32(da.ExecuteScalar(sql, para.ToArray))
                Return count > 0
            End Using
        End Function

        ''' <summary>
        ''' 取得畜牧場清單
        ''' </summary>
        Public Function Get_FarmList(Optional cityID As String = "%", Optional twID As String = "%", Optional keyWord As String = "") As List(Of stru_farmInfo)

            Dim sqlString As String =
                <xml string="
                    select
                        farmID, farmName, farmCode, owner, ownerID, ownerTel, animalCount, 
                        List_Farm.twID, cityID, city, area, address, memo, insertType,insertAccountID
                    from List_Farm
                    left join System_Taiwan on List_Farm.twID = System_Taiwan.twID
                    where removeDateTime is null and cityID like @cityID and List_Farm.twID like @twID  and
                    (
	                    farmCode like '%' + @keyWord + '%' or 
	                    farmName like '%' + @keyWord + '%' or
	                    owner like '%' + @keyWord + '%'
                    )
                    order by List_Farm.twID, farmCode, farmName, owner, ownerID
                "></xml>.FirstAttribute.Value
            Dim para As New List(Of Data.SqlClient.SqlParameter)

            para.Add(New Data.SqlClient.SqlParameter("cityID", cityID))
            para.Add(New Data.SqlClient.SqlParameter("twID", twID))
            para.Add(New Data.SqlClient.SqlParameter("keyWord", keyWord))

            Dim dt As New Data.DataTable

            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString, para.ToArray)
            End Using

            Dim farmList As New List(Of stru_farmInfo)

            For Each dr As Data.DataRow In dt.Rows
                Dim f As New stru_farmInfo With {
                    .farmID = If(IsDBNull(dr("farmID")), 0, Convert.ToInt32(dr("farmID"))),
                    .farmName = taifCattle_base.Convert_DBNullToString(dr("farmName")),
                    .farmCode = taifCattle_base.Convert_DBNullToString(dr("farmCode")),
                    .owner = taifCattle_base.Convert_DBNullToString(dr("owner")),
                    .ownerID = taifCattle_base.Convert_DBNullToString(dr("ownerID")),
                    .ownerTel = taifCattle_base.Convert_DBNullToString(dr("ownerTel")),
                    .twID = If(IsDBNull(dr("twID")), 0, Convert.ToInt32(dr("twID"))),
                    .cityID = If(IsDBNull(dr("cityID")), 0, Convert.ToInt32(dr("cityID"))),
                    .city = taifCattle_base.Convert_DBNullToString(dr("city")),
                    .town = taifCattle_base.Convert_DBNullToString(dr("area")),
                    .address = taifCattle_base.Convert_DBNullToString(dr("address")),
                    .animalCount = taifCattle_base.Convert_DBNullToString(dr("animalCount")),
                    .memo = taifCattle_base.Convert_DBNullToString(dr("memo")),
                    .insertType = taifCattle_base.Convert_ValueToEnum(GetType(taifCattle.Base.enum_InsertType), dr("insertType")),
                    .insertAccountID = If(IsDBNull(dr("insertAccountID")), 0, Convert.ToInt32(dr("insertAccountID")))
                }

                farmList.Add(f)
            Next

            Return farmList
        End Function

        ''' <summary>
        ''' 取得畜牧場(單筆)(流水號查)
        ''' </summary>
        ''' <param name="farmID"></param>
        ''' <returns></returns>
        Public Function Get_FarmByID(farmID As Integer) As stru_farmInfo
            Dim sqlString As String =
                <xml string="
                    select
                        farmID, farmName, farmCode, owner, ownerID, ownerTel, animalCount, 
                        List_Farm.twID, cityID, city, area, address, memo, insertType, insertAccountID
                    from List_Farm
                    left join System_Taiwan on List_Farm.twID = System_Taiwan.twID
                    where farmID = @farmID
                "></xml>.FirstAttribute.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("farmID", farmID))
            Dim dt As New DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString, para.ToArray)
            End Using

            Dim f As New stru_farmInfo

            If dt.Rows.Count = 0 Then
                f.farmID = -1
            Else
                Dim dr As DataRow = dt.Rows(0)
                f.farmID = If(IsDBNull(dr("farmID")), 0, Convert.ToInt32(dr("farmID")))
                f.farmName = taifCattle_base.Convert_DBNullToString(dr("farmName"))
                f.farmCode = taifCattle_base.Convert_DBNullToString(dr("farmCode"))
                f.owner = taifCattle_base.Convert_DBNullToString(dr("owner"))
                f.ownerID = taifCattle_base.Convert_DBNullToString(dr("ownerID"))
                f.ownerTel = taifCattle_base.Convert_DBNullToString(dr("ownerTel"))
                f.twID = If(IsDBNull(dr("twID")), 0, Convert.ToInt32(dr("twID")))
                f.cityID = If(IsDBNull(dr("cityID")), 0, Convert.ToInt32(dr("cityID")))
                f.city = taifCattle_base.Convert_DBNullToString(dr("city"))
                f.town = taifCattle_base.Convert_DBNullToString(dr("area"))
                f.address = taifCattle_base.Convert_DBNullToString(dr("address"))
                f.animalCount = taifCattle_base.Convert_DBNullToString(dr("animalCount"))
                f.memo = taifCattle_base.Convert_DBNullToString(dr("memo"))
                f.insertType = taifCattle_base.Convert_ValueToEnum(GetType(taifCattle.Base.enum_InsertType), dr("insertType"))
                f.insertAccountID = If(IsDBNull(dr("insertAccountID")), 0, Convert.ToInt32(dr("insertAccountID")))
            End If
            Return f
        End Function

        ''' <summary>
        ''' 取得畜牧場(單筆)(畜牧場證號查)
        ''' </summary>
        ''' <param name="farmCode"></param>
        ''' <returns></returns>
        Public Function Get_FarmByCode(farmCode As String) As stru_farmInfo
            Dim sqlString As String =
                <xml string="
                    select
                        farmID, farmName, farmCode, owner, ownerID, ownerTel, animalCount, 
                        List_Farm.twID, cityID, city, area, address, memo, insertType, insertAccountID
                    from List_Farm
                    left join System_Taiwan on List_Farm.twID = System_Taiwan.twID
                    where farmCode = @farmCode
                "></xml>.FirstAttribute.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("farmCode", farmCode))
            Dim dt As New DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString, para.ToArray)
            End Using

            Dim f As New stru_farmInfo

            If dt.Rows.Count = 0 Then
                Return Nothing
            End If

            Dim dr As DataRow = dt.Rows(0)

            f.farmID = If(IsDBNull(dr("farmID")), 0, Convert.ToInt32(dr("farmID")))
            f.farmName = taifCattle_base.Convert_DBNullToString(dr("farmName"))
            f.farmCode = taifCattle_base.Convert_DBNullToString(dr("farmCode"))
            f.owner = taifCattle_base.Convert_DBNullToString(dr("owner"))
            f.ownerID = taifCattle_base.Convert_DBNullToString(dr("ownerID"))
            f.ownerTel = taifCattle_base.Convert_DBNullToString(dr("ownerTel"))
            f.twID = If(IsDBNull(dr("twID")), 0, Convert.ToInt32(dr("twID")))
            f.cityID = If(IsDBNull(dr("cityID")), 0, Convert.ToInt32(dr("cityID")))
            f.city = taifCattle_base.Convert_DBNullToString(dr("city"))
            f.town = taifCattle_base.Convert_DBNullToString(dr("area"))
            f.address = taifCattle_base.Convert_DBNullToString(dr("address"))
            f.animalCount = taifCattle_base.Convert_DBNullToString(dr("animalCount"))
            f.memo = taifCattle_base.Convert_DBNullToString(dr("memo"))
            f.insertType = taifCattle_base.Convert_ValueToEnum(GetType(taifCattle.Base.enum_InsertType), dr("insertType"))
            f.insertAccountID = If(IsDBNull(dr("insertAccountID")), 0, Convert.ToInt32(dr("insertAccountID")))

            Return f
        End Function

        ''' <summary>
        ''' 新增畜牧場
        ''' </summary>
        ''' <param name="info"></param>
        Public Sub Insert_Farm(info As taifCattle.Farm.stru_farmInfo)
            Dim sql As String =
                <xml>
                    INSERT INTO List_Farm
                    (farmName, farmCode, owner, ownerID, ownerTel, twID, address,
                     animalCount, memo, insertType, insertDateTime, insertAccountID,updateDateTime,updateAccountID)
                    VALUES
                    (@farmName, @farmCode, @owner, @ownerID, @ownerTel, @twID, @address,
                     @animalCount, @memo, @insertType,@insertDateTime, @insertAccountID,@insertDateTime,@insertAccountID)
                </xml>.Value

            Dim para As New List(Of SqlClient.SqlParameter)
            para.Add(New SqlClient.SqlParameter("farmName", info.farmName))
            para.Add(New SqlClient.SqlParameter("farmCode", info.farmCode))
            para.Add(New SqlClient.SqlParameter("owner", info.owner))
            para.Add(New SqlClient.SqlParameter("ownerID", taifCattle_base.Convert_EmptyToObject(info.ownerID, DBNull.Value)))
            para.Add(New SqlClient.SqlParameter("ownerTel", taifCattle_base.Convert_EmptyToObject(info.ownerTel, DBNull.Value)))
            para.Add(New SqlClient.SqlParameter("twID", info.twID))
            para.Add(New SqlClient.SqlParameter("address", info.address))
            para.Add(New SqlClient.SqlParameter("animalCount", taifCattle_base.Convert_EmptyToObject(info.animalCount, DBNull.Value)))
            para.Add(New SqlClient.SqlParameter("memo", taifCattle_base.Convert_EmptyToObject(info.memo, DBNull.Value)))
            para.Add(New SqlClient.SqlParameter("insertType", info.insertType.ToString))
            para.Add(New SqlClient.SqlParameter("insertDateTime", Now))
            para.Add(New SqlClient.SqlParameter("insertAccountID", info.insertAccountID))

            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sql, para.ToArray())
            End Using
        End Sub

        ''' <summary>
        ''' 更新畜牧場
        ''' </summary>
        ''' <param name="info"></param>
        Public Sub Update_Farm(info As taifCattle.Farm.stru_farmInfo)
            Dim sql As String =
                <xml>
                    UPDATE List_Farm
                    SET farmName = @farmName,
                        owner = @owner,
                        ownerID = @ownerID,
                        ownerTel = @ownerTel,
                        twID = @twID,
                        address = @address,
                        animalCount = @animalCount,
                        memo = @memo,
                        updateDateTime = GETDATE(),
                        updateAccountID = @updateAccountID
                    WHERE farmID = @farmID
                </xml>.Value

            Dim para As New List(Of SqlClient.SqlParameter)
            para.Add(New SqlClient.SqlParameter("farmID", info.farmID))
            para.Add(New SqlClient.SqlParameter("farmName", info.farmName))
            para.Add(New SqlClient.SqlParameter("owner", info.owner))
            para.Add(New SqlClient.SqlParameter("ownerID", taifCattle_base.Convert_EmptyToObject(info.ownerID, DBNull.Value)))
            para.Add(New SqlClient.SqlParameter("ownerTel", taifCattle_base.Convert_EmptyToObject(info.ownerTel, DBNull.Value)))
            para.Add(New SqlClient.SqlParameter("twID", info.twID))
            para.Add(New SqlClient.SqlParameter("address", info.address))
            para.Add(New SqlClient.SqlParameter("animalCount", taifCattle_base.Convert_EmptyToObject(info.animalCount, DBNull.Value)))
            para.Add(New SqlClient.SqlParameter("memo", taifCattle_base.Convert_EmptyToObject(info.memo, DBNull.Value)))
            para.Add(New SqlClient.SqlParameter("updateAccountID", info.updateAccountID))

            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sql, para.ToArray())
            End Using

        End Sub

        Public Structure stru_missingFarmRecord
            Property missingID As Integer
            Property dataSource As String
            Property serialNo As String
            Property farmCode As String
            Property insertDateTime As DateTime
        End Structure

        Public Function SaveMissingFarmCodes(dataSource As String, farmCodes As IEnumerable(Of String)) As String
            If farmCodes Is Nothing Then
                Return String.Empty
            End If

            Dim normalizedDataSource As String = If(dataSource, String.Empty).Trim()
            If String.IsNullOrEmpty(normalizedDataSource) Then
                normalizedDataSource = "Unknown"
            End If

            Dim distinctCodes As List(Of String) =
                farmCodes.Where(Function(code) Not String.IsNullOrWhiteSpace(code)).Select(Function(code) code.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList()

            If distinctCodes.Count = 0 Then
                Return String.Empty
            End If

            Dim serialSeed As String = $"{DateTime.Now:yyyyMMddHHmmssfff}{Guid.NewGuid():N}"
            Dim serial As String = serialSeed.Substring(0, Math.Min(32, serialSeed.Length))

            Dim sql As String = "INSERT INTO Data_FarmMissingImport (dataSource, serialNo, farmCode, insertDateTime) VALUES (@dataSource, @serialNo, @farmCode, GETDATE())"

            Using da As New DataAccess.MS_SQL
                For Each code As String In distinctCodes
                    Dim para As SqlClient.SqlParameter() = {
                        New SqlClient.SqlParameter("@dataSource", normalizedDataSource),
                        New SqlClient.SqlParameter("@serialNo", serial),
                        New SqlClient.SqlParameter("@farmCode", code)
                    }
                    da.ExecNonQuery(sql, para)
                Next
            End Using

            Return serial
        End Function

        Public Function GetMissingFarmRecords(serialNo As String) As List(Of stru_missingFarmRecord)
            Dim result As New List(Of stru_missingFarmRecord)

            If String.IsNullOrWhiteSpace(serialNo) Then
                Return result
            End If

            Dim sql As String = "SELECT missingID, dataSource, serialNo, farmCode, insertDateTime FROM Data_FarmMissingImport WHERE serialNo = @serialNo ORDER BY missingID"

            Dim para As SqlClient.SqlParameter() = {
                New SqlClient.SqlParameter("@serialNo", serialNo)
            }

            Using da As New DataAccess.MS_SQL
                Dim dt As DataTable = da.GetDataTable(sql, para)
                For Each row As DataRow In dt.Rows
                    Dim record As New stru_missingFarmRecord With {
                        .missingID = Convert.ToInt32(row("missingID")),
                        .dataSource = taifCattle_base.Convert_DBNullToString(row("dataSource")),
                        .serialNo = taifCattle_base.Convert_DBNullToString(row("serialNo")),
                        .farmCode = taifCattle_base.Convert_DBNullToString(row("farmCode")),
                        .insertDateTime = Convert.ToDateTime(row("insertDateTime"))
                    }
                    result.Add(record)
                Next
            End Using

            Return result
        End Function

        Public Function GetExistingFarmCodes(farmCodes As IEnumerable(Of String)) As HashSet(Of String)
            Dim result As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            If farmCodes Is Nothing Then
                Return result
            End If

            Dim codeList As List(Of String) =
                farmCodes.Where(Function(code) Not String.IsNullOrWhiteSpace(code)).Select(Function(code) code.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList()

            If codeList.Count = 0 Then
                Return result
            End If

            Dim sqlBuilder As New StringBuilder()
            sqlBuilder.Append("SELECT farmCode FROM List_Farm WHERE removeDateTime IS NULL AND farmCode IN (")

            Dim para As New List(Of SqlClient.SqlParameter)
            For i As Integer = 0 To codeList.Count - 1
                Dim paramName As String = $"@farmCode{i}"
                If i > 0 Then
                    sqlBuilder.Append(",")
                End If
                sqlBuilder.Append(paramName)
                para.Add(New SqlClient.SqlParameter(paramName, codeList(i)))
            Next

            sqlBuilder.Append(")")

            Using da As New DataAccess.MS_SQL
                Dim dt As DataTable = da.GetDataTable(sqlBuilder.ToString(), para.ToArray())
                For Each row As DataRow In dt.Rows
                    Dim code As String = taifCattle_base.Convert_DBNullToString(row("farmCode"))
                    If Not String.IsNullOrEmpty(code) Then
                        result.Add(code)
                    End If
                Next
            End Using

            Return result
        End Function

        Public Function GetInsertTypeByDataSource(dataSource As String) As taifCattle.Base.enum_InsertType
            Select Case dataSource
                Case "CattleManage_Batch"
                    Return taifCattle.Base.enum_InsertType.牛籍批次建檔
                Case "HisManage_Batch"
                    Return taifCattle.Base.enum_InsertType.旅程批次建檔
                Case "HisEndManage_Batch"
                    Return taifCattle.Base.enum_InsertType.除籍批次建檔
                Case Else
                    Return taifCattle.Base.enum_InsertType.人工網頁建檔
            End Select
        End Function

    End Class
End Namespace
