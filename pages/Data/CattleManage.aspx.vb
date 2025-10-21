Public Class CattleManage
    Inherits taifCattle.Base

    'Dim taifCattle_con As New taifCattle.

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            'GridView_data.DataSource = taif_cattle.Get_CattleList()
            'GridView_data.DataBind()
        End If
    End Sub

End Class