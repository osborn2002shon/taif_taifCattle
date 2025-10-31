Namespace taifCattle

    Partial Public Class Cattle

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
            ddl.Items.Add(New ListItem("未除籍", "未除籍"))
            ddl.Items.Add(New ListItem("已除籍", "已除籍"))
        End Sub

        ''' <summary>
        ''' DDL：資料類型
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <param name="groupName"></param>
        Sub Bind_DropDownList_hisType(ddl As DropDownList, groupName As String)
            Dim sqlString As String =
                "select * from Cattle_TypeHistory where groupName like @groupName order by hisTypeID"
            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("groupName", groupName))
            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString, para.ToArray())
            End Using
            ddl.Items.Clear()
            For i = 0 To dt.Rows.Count - 1
                ddl.Items.Add(New ListItem(dt.Rows(i)("typeName"), dt.Rows(i)("hisTypeID")))
            Next
        End Sub

        ''' <summary>
        ''' DDL：屠宰場清單
        ''' </summary>
        ''' <param name="ddl"></param>
        Sub Bind_DropDownList_slau(ddl As DropDownList, isNeedAll As Boolean)
            Dim sqlString As String =
                "select * from List_Slaughterhouse where isActive = 1 order by slauID"
            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString)
            End Using
            ddl.Items.Clear()
            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*屠宰場不拘", "%"))
            End If
            For i = 0 To dt.Rows.Count - 1
                ddl.Items.Add(New ListItem(dt.Rows(i)("slauName"), dt.Rows(i)("slauID")))
            Next
        End Sub

        ''' <summary>
        ''' DDL：化製廠清單
        ''' </summary>
        ''' <param name="ddl"></param>
        Sub Bind_DropDownList_plant(ddl As DropDownList, isNeedAll As Boolean)
            Dim sqlString As String =
                "select * from List_RenderingPlant where isActive = 1 order by plantID"
            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString)
            End Using
            ddl.Items.Clear()
            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*化製場不拘", "%"))
            End If
            For i = 0 To dt.Rows.Count - 1
                ddl.Items.Add(New ListItem(dt.Rows(i)("plantName"), dt.Rows(i)("plantID")))
            Next

        End Sub

        ''' <summary>
        ''' CheckBoxList：資料類型
        ''' </summary>
        ''' <param name="cbl"></param>
        ''' <param name="groupName"></param>
        Sub Bind_CheckBoxList_hisType(cbl As CheckBoxList, groupName As String, Optional selectAll As Boolean = False)
            Dim sqlString As String =
                "SELECT * FROM Cattle_TypeHistory WHERE groupName LIKE @groupName ORDER BY hisTypeID"

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("groupName", groupName))

            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString, para.ToArray())
            End Using

            cbl.Items.Clear()
            For i = 0 To dt.Rows.Count - 1
                Dim li As New ListItem(dt.Rows(i)("typeName").ToString(), dt.Rows(i)("hisTypeID").ToString())
                li.Selected = selectAll
                cbl.Items.Add(li)
            Next
        End Sub


    End Class

End Namespace

