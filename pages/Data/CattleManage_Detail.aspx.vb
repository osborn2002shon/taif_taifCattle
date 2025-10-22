Imports taifCattle.taifCattle.Cattle

Public Class CattleManage_Detail
    Inherits taifCattle.Base

    Dim taifCattle_cattle As New taifCattle.Cattle

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Dim mode As enum_cattleEditMode
            If IsNothing(Session("CattleManage")) Then
                mode = enum_cattleEditMode.list
            Else
                mode = CType(Session("CattleManage"), enum_cattleEditMode)
            End If
            Select Case mode
                Case taifCattle.Cattle.enum_cattleEditMode.list
                    Response.Redirect("CattleManage.aspx")
                Case taifCattle.Cattle.enum_cattleEditMode.add
                    MultiView_main.SetActiveView(View_new)
                Case taifCattle.Cattle.enum_cattleEditMode.edit
                    MultiView_main.SetActiveView(View_edit)
                Case Else
                    Response.Redirect("CattleManage.aspx")
            End Select
        End If
    End Sub

End Class