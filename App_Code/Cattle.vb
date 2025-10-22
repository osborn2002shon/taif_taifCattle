Imports Microsoft.VisualBasic

Namespace taifCattle

    ''' <summary>
    ''' Class Cattle
    ''' </summary>
    Public Class Cattle

        Enum enum_cattleEditMode
            list
            add
            edit
        End Enum

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

        Sub Bind_DropDownList_cattleType(ddl As DropDownList, isNeedAll As Boolean, groupOrder As String)
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
                ddl.Items.Add(New ListItem(dt.Rows(i)("typeName"), dt.Rows(i)("cattleTypeID")))
            Next
        End Sub

        Sub Bind_DropDownList_cattleStatus(ddl As DropDownList, isNeedAll As Boolean)
            ddl.Items.Clear()
            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*牛籍狀態不拘", "%"))
            End If
            ddl.Items.Add(New ListItem("正常", "正常"))
            ddl.Items.Add(New ListItem("除籍", "除籍"))
        End Sub

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
    End Class

End Namespace
