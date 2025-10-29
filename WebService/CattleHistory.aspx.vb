Imports Newtonsoft.Json

Public Class CattleHistory
    Inherits System.Web.UI.Page

    Dim taifCattle_WSBase As New taifCattle.WS_Base

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Dim taifCattle_WS As New taifCattle.WS_CattleHistory
            Dim token As String = Request.QueryString("token")
            Dim tagNo As String = Request.QueryString("tagNo")

            If String.IsNullOrEmpty(token) Or String.IsNullOrEmpty(tagNo) Then
                Response.Write(JsonConvert.SerializeObject(taifCattle_WS))
            Else
                taifCattle_WS = taifCattle_WS.Get_CattleHistory(token, tagNo)
                taifCattle_WSBase.Insert_DataExchangeLog(
                    taifCattle.WS_Base.enum_dataSourceName.外部單位,
                    taifCattle.WS_Base.enum_apiName.指定牛籍編號旅程查詢服務,
                    token & "," & tagNo, 1,
                    taifCattle.WS_Base.enum_triggerType.其他,
                    False, "",
                    taifCattle.WS_Base.enum_actionType.dataOut)
                Response.Write(JsonConvert.SerializeObject(taifCattle_WS))
            End If
        End If
    End Sub

    Private Sub CattleHistory_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Response.ContentType = "application/json"
    End Sub
End Class