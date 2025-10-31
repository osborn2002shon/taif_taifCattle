Imports NPOI.SS.UserModel

Public Class WebServiceLog
    Inherits System.Web.UI.Page

    Sub Bind_DropDownList_api()
        DropDownList_apiName.Items.Clear()
        DropDownList_apiName.Items.Add(New ListItem("*介接服務不拘", "%"))
        For Each item As Object In [Enum].GetValues(GetType(taifCattle.WS_Base.enum_apiName))
            DropDownList_apiName.Items.Add(item.ToString())
        Next
    End Sub

    Sub Bind_DropDownList_actionType()
        DropDownList_actionType.Items.Clear()
        DropDownList_actionType.Items.Add(New ListItem("接入", "dataIn"))
        DropDownList_actionType.Items.Add(New ListItem("接出", "dataOut"))
    End Sub

    Sub Bind_GridView_data()
        Dim queryDate_beg As Date = TextBox_logDate_beg.Text.Trim
        Dim queryDate_end As Date = TextBox_logDate_end.Text.Trim
        Dim apiName As String = DropDownList_apiName.SelectedValue
        Dim actionType As String = DropDownList_actionType.SelectedValue
        Dim dt As Data.DataTable = Get_Data(queryDate_beg, queryDate_end, apiName, actionType)
        Label_datCount.Text = dt.Rows.Count
        GridView_data.DataSource = dt
        GridView_data.DataBind()
    End Sub

    Function Get_Data(queryDate_beg As Date, queryDate_end As Date, apiName As String, actionType As String) As Data.DataTable
        Dim sqlString As String = "
            select * from Data_DataExchangeLog
            where 
                cast(logDateTime as date) between @queryDate_beg and @queryDate_end and
                apiName like @apiName and actionType like @actionType
            order by logDateTime desc"
        Dim para As New List(Of Data.SqlClient.SqlParameter)
        para.Add(New Data.SqlClient.SqlParameter("queryDate_beg", queryDate_beg))
        para.Add(New Data.SqlClient.SqlParameter("queryDate_end", queryDate_end))
        para.Add(New Data.SqlClient.SqlParameter("apiName", apiName))
        para.Add(New Data.SqlClient.SqlParameter("actionType", actionType))
        Using da As New DataAccess.MS_SQL
            Return da.GetDataTable(sqlString, para.ToArray())
        End Using

    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            '預設近6天
            TextBox_logDate_beg.Text = DateAdd(DateInterval.Day, -6, Today).ToString("yyyy-MM-dd")
            TextBox_logDate_end.Text = Today.ToString("yyyy-MM-dd")
            Bind_DropDownList_api()
            Bind_DropDownList_actionType()
            Bind_GridView_data()
        End If
    End Sub

    Private Sub LinkButton_query_Click(sender As Object, e As EventArgs) Handles LinkButton_query.Click
        Bind_GridView_data()
    End Sub

    Private Sub GridView_data_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_data.PageIndexChanging
        GridView_data.PageIndex = e.NewPageIndex
        Bind_GridView_data()
    End Sub
End Class