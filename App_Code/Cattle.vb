Imports Microsoft.VisualBasic

Namespace taifCattle

    ''' <summary>
    ''' Class Cattle_Control
    ''' </summary>
    Public Class Cattle_Control
        Sub Bind_DropDownList_cattleGroup(ddl As DropDownList, isNeedAll As Boolean)
            Dim sqlString As String = "select distinct groupOrder, groupName from Cattle_TypeCattle order by groupOrder "
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

        Sub Bind_DropDownList_cattleType(ddl As DropDownList, isNeedAll As Boolean)
            Dim sqlString As String = "select cattleTypeID, groupName, typeName from Cattle_TypeCattle order by groupOrder, cattleTypeID "
            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString)
            End Using

            ddl.Items.Clear()

            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*牛籍規格", "%"))
            End If
            For i = 0 To dt.Rows.Count - 1
                ddl.Items.Add(New ListItem(dt.Rows(i)("groupName") & "-" & dt.Rows(i)("typeName"), dt.Rows(i)("cattleTypeID")))
            Next
        End Sub
    End Class

    ''' <summary>
    ''' Class Cattle
    ''' </summary>
    Public Class Cattle
        Function Get_CattleList(groupOrder As Integer) As Data.DataTable
            Dim sqlString As String = "select * from View_CattleList order by cattleID desc"
            Using da As New DataAccess.MS_SQL
                Return da.GetDataTable(sqlString)
            End Using
        End Function
    End Class

End Namespace
