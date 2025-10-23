Imports Microsoft.VisualBasic

Namespace taifCattle

    ''' <summary>
    ''' Class Cattle
    ''' </summary>
    Public Class Cattle

        Dim taifBase As New taifCattle.Base

        ''' <summary>
        ''' 結構：對應 Cattle_List 資料表
        ''' </summary>
        Public Structure stru_cattleInfo
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
        ''' DDL：牛籍類型
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <param name="isNeedAll"></param>
        Sub Bind_DropDownList_cattleGroup(ddl As DropDownList, isNeedAll As Boolean)
            Dim sqlString As String =
                "select distinct groupOrder, groupName from Cattle_TypeCattle order by groupOrder "
            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString)
            End Using

            ddl.Items.Clear()

            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*牛籍類型不拘", "%"))
            End If

            For i = 0 To dt.Rows.Count - 1
                ddl.Items.Add(New ListItem(dt.Rows(i)("groupName"), dt.Rows(i)("groupOrder")))
            Next
        End Sub

        ''' <summary>
        ''' DDL：牛籍規格
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <param name="isNeedAll"></param>
        ''' <param name="groupOrder"></param>
        Sub Bind_DropDownList_cattleType(ddl As DropDownList, isNeedAll As Boolean, groupOrder As String, Optional isNeedGroupName As Boolean = False)
            Dim sqlString As String =
                "select cattleTypeID, groupName, typeName from Cattle_TypeCattle where groupOrder like @groupOrder order by groupOrder, cattleTypeID "
            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("groupOrder", groupOrder))
            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString, para.ToArray())
            End Using
            ddl.Items.Clear()
            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*牛籍規格不拘", "%"))
            End If
            For i = 0 To dt.Rows.Count - 1
                If isNeedGroupName Then
                    ddl.Items.Add(New ListItem(dt.Rows(i)("GroupName") & "：" & dt.Rows(i)("typeName"), dt.Rows(i)("cattleTypeID")))
                Else
                    ddl.Items.Add(New ListItem(dt.Rows(i)("typeName"), dt.Rows(i)("cattleTypeID")))
                End If

            Next
        End Sub

        ''' <summary>
        ''' DDL：牛籍狀態
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <param name="isNeedAll"></param>
        Sub Bind_DropDownList_cattleStatus(ddl As DropDownList, isNeedAll As Boolean)
            ddl.Items.Clear()
            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*牛籍狀態不拘", "%"))
            End If
            ddl.Items.Add(New ListItem("正常", "正常"))
            ddl.Items.Add(New ListItem("除籍", "除籍"))
        End Sub

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
        Function Check_IsCattleExist(tagNo As String) As Boolean
            Dim sqlString As String = "select * from Cattle_List where tagNo = @tagNo and removeDateTime is null "
            Dim para As New Data.SqlClient.SqlParameter("tagNo", tagNo)
            Using da As New DataAccess.MS_SQL
                Dim dt As Data.DataTable = da.GetDataTable(sqlString, para)
                If dt.Rows.Count > 0 Then
                    Return True
                Else
                    Return False
                End If
            End Using
        End Function

        ''' <summary>
        ''' 新增牛籍資料
        ''' </summary>
        ''' <param name="cattleInfo">Cattle_Record 結構</param>
        Public Sub Insert_Cattle(cattleInfo As stru_cattleInfo)
            Dim sqlString As String =
            <xml sql="
                insert into Cattle_List
                    (cattleTypeID, tagNo, tagMemo, birthYear, cattleMemo, 
                     insertType, insertDateTime, insertAccountID, updateDateTime, updateAccountID, removeDateTime, removeAccountID)
                values
                    (@cattleTypeID, @tagNo, @tagMemo, @birthYear, @cattleMemo, 
                     @insertType, @insertDateTime, @insertAccountID, @updateDateTime, @updateAccountID, @removeDateTime, @removeAccountID)
            "></xml>.FirstAttribute.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("cattleTypeID", cattleInfo.cattleTypeID))
            para.Add(New Data.SqlClient.SqlParameter("tagNo", cattleInfo.tagNo))
            para.Add(New Data.SqlClient.SqlParameter("tagMemo", taifBase.Convert_EmptyToObject(cattleInfo.tagMemo, DBNull.Value)))
            para.Add(New Data.SqlClient.SqlParameter("birthYear", taifBase.Convert_EmptyToObject(cattleInfo.birthYear, DBNull.Value)))
            para.Add(New Data.SqlClient.SqlParameter("cattleMemo", taifBase.Convert_EmptyToObject(cattleInfo.cattleMemo, DBNull.Value)))
            para.Add(New Data.SqlClient.SqlParameter("insertType", cattleInfo.insertType.ToString))
            para.Add(New Data.SqlClient.SqlParameter("insertDateTime", cattleInfo.insertDateTime))
            para.Add(New Data.SqlClient.SqlParameter("insertAccountID", cattleInfo.insertAccountID))
            para.Add(New Data.SqlClient.SqlParameter("updateDateTime", cattleInfo.updateDateTime))
            para.Add(New Data.SqlClient.SqlParameter("updateAccountID", cattleInfo.updateAccountID))
            para.Add(New Data.SqlClient.SqlParameter("removeDateTime", DBNull.Value))
            para.Add(New Data.SqlClient.SqlParameter("removeAccountID", DBNull.Value))

            Using da As New DataAccess.MS_SQL
                da.ExecNonQuery(sqlString, para.ToArray())
            End Using
        End Sub


    End Class

End Namespace
