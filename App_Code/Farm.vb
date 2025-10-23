Imports System.Data

Namespace taifCattle
    Public Class Farm
        Dim taifCattle_dao As New DAO.Farm
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

        ''' <summary>
        ''' 取得畜牧場清單
        ''' </summary>
        Public Function GetFarmList(Optional cityID As String = "%", Optional twID As String = "%", Optional keyWord As String = "") As List(Of stru_farmInfo)

            Dim dt As Data.DataTable = taifCattle_dao.Get_FarmList(cityID, twID, keyWord)
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
                    .insertType = taifCattle_base.Convert_DBToEnum(Of taifCattle.Base.enum_InsertType)(dr("insertType")),
                    .insertAccountID = If(IsDBNull(dr("insertAccountID")), 0, Convert.ToInt32(dr("insertAccountID")))
                }

                farmList.Add(f)
            Next

            Return farmList
        End Function

        ''' <summary>
        ''' 取得畜牧場(單筆)
        ''' </summary>
        ''' <param name="farmID"></param>
        ''' <returns></returns>
        Public Function GetFarmByID(farmID As Integer) As stru_farmInfo
            Dim dt As DataTable = taifCattle_dao.Get_FarmByID(farmID)
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
            f.insertType = taifCattle_base.Convert_DBToEnum(Of taifCattle.Base.enum_InsertType)(dr("insertType"))
            f.insertAccountID = If(IsDBNull(dr("insertAccountID")), 0, Convert.ToInt32(dr("insertAccountID")))

            Return f
        End Function


        ''' <summary>
        ''' 檢查畜牧場證號
        ''' </summary>
        ''' <param name="farmCode"></param>
        ''' <param name="excludeFarmID"></param>
        ''' <returns></returns>
        Public Function CheckFarmCodeExists(farmCode As String, Optional excludeFarmID As Integer = 0) As Boolean
            Return taifCattle_dao.Check_FarmCodeExists(farmCode, excludeFarmID)
        End Function
        Public Sub InsertFarm(info As taifCattle.Farm.stru_farmInfo)
            taifCattle_dao.Insert_Farm(info)

            ' 寫入操作紀錄
            taifCattle_base.Insert_UserLog(info.insertAccountID, taifCattle.Base.enum_UserLogItem.牧場資料管理, taifCattle.Base.enum_UserLogType.新增, $"farmCode:{info.farmCode}")
        End Sub

        Public Sub UpdateFarm(info As taifCattle.Farm.stru_farmInfo)
            taifCattle_dao.Update_Farm(info)

            ' 寫入操作紀錄
            taifCattle_base.Insert_UserLog(info.updateAccountID, taifCattle.Base.enum_UserLogItem.牧場資料管理, taifCattle.Base.enum_UserLogType.修改, $"farmID:{info.farmID}")
        End Sub
    End Class
End Namespace

Namespace taifCattle.DAO
    Class Farm
        Dim taifCattle_base As New taifCattle.Base
        ''' <summary>
        ''' 取得畜牧場清單
        ''' </summary>
        ''' <returns></returns>
        Function Get_FarmList(Optional cityID As String = "%", Optional twID As String = "%", Optional keyWord As String = "") As Data.DataTable
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
            Using da As New DataAccess.MS_SQL
                Return da.GetDataTable(sqlString, para.ToArray)
            End Using
        End Function

        ''' <summary>
        ''' 取得單筆畜牧場資料
        ''' </summary>
        Function Get_FarmByID(farmID As Integer) As DataTable
            Dim sqlString As String =
                <xml string="
            select
                farmID, farmName, farmCode, owner, ownerID, ownerTel, animalCount, 
                List_Farm.twID, cityID, city, area, address, memo, insertType,insertAccountID
            from List_Farm
            left join System_Taiwan on List_Farm.twID = System_Taiwan.twID
            where farmID = @farmID
        "></xml>.FirstAttribute.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("farmID", farmID))

            Using da As New DataAccess.MS_SQL
                Return da.GetDataTable(sqlString, para.ToArray)
            End Using
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
    End Class
End Namespace
