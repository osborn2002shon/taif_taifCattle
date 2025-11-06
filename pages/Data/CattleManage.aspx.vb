Imports System.Data.SqlTypes
Imports taifCattle.taifCattle.Cattle

Public Class CattleManage
    Inherits taifCattle.Base

    Dim taifCattle_cattle As New taifCattle.Cattle

    Sub Bind_GridView()
        Dim groupOrder As String = DropDownList_groupName.SelectedValue
        Dim typeName As String = DropDownList_typeName.SelectedValue
        Dim cattleStatus As String = DropDownList_cattleStatus.SelectedValue
        Dim tagNo As String = Convert_EmptyToObject(TextBox_tagNo.Text.Trim, "%")
        Dim birthYear As String = Convert_EmptyToObject(TextBox_birthYear.Text.Trim, "%")
        Dim cattleAge As String = Convert_EmptyToObject(TextBox_cattleAge.Text.Trim, "%")
        Dim dt As Data.DataTable = taifCattle_cattle.Get_CattleList(groupOrder, typeName, cattleStatus, tagNo, birthYear, cattleAge)
        Label_datCount.Text = dt.Rows.Count.ToString("N0")
        GridView_data.DataSource = dt
        GridView_data.DataBind()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Session("CattleManage") = enum_EditMode.預設
            taifCattle_cattle.Bind_DropDownList_cattleGroup(DropDownList_groupName, True)
            taifCattle_cattle.Bind_DropDownList_cattleType(DropDownList_typeName, True, DropDownList_groupName.SelectedValue)
            taifCattle_cattle.Bind_DropDownList_cattleStatus(DropDownList_cattleStatus, True)
            'DropDownList_cattleStatus.SelectedValue = "正常" '全撈資料量會很大，以正常牛優先撈 > 查詢會忘記這個設定、反而找不到牛
            Bind_GridView()
        End If
    End Sub

    Private Sub DropDownList_groupName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_groupName.SelectedIndexChanged
        taifCattle_cattle.Bind_DropDownList_cattleType(DropDownList_typeName, True, DropDownList_groupName.SelectedValue)
    End Sub

    Private Sub LinkButton_query_Click(sender As Object, e As EventArgs) Handles LinkButton_query.Click
        Bind_GridView()
    End Sub

    Private Sub GridView_data_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView_data.RowCommand
        Select Case e.CommandName
            Case "cattleEdit"
                Session("CattleManage") = enum_EditMode.編輯
                Session("CattleManage_cid") = e.CommandArgument
                Response.Redirect("CattleManage_Detail.aspx")
            Case Else
                'nothing
        End Select
    End Sub

    Private Sub LinkButton_addNew_Click(sender As Object, e As EventArgs) Handles LinkButton_addNew.Click
        Session("CattleManage") = enum_EditMode.新增
        Session("CattleManage_cid") = -1
        Response.Redirect("CattleManage_Detail.aspx")
    End Sub

    Private Sub GridView_data_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_data.PageIndexChanging
        GridView_data.PageIndex = e.NewPageIndex
        Bind_GridView()
    End Sub
End Class