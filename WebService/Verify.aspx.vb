Imports Newtonsoft.Json

Public Class Verify
    Inherits System.Web.UI.Page

    Dim taifCattle_WSBase As New taifCattle.WS_Base

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Dim taifCattle_WS As New taifCattle.WS_Verify
            Dim ac As String = Request.QueryString("ac")
            Dim pw As String = Request.QueryString("pw")
            If String.IsNullOrEmpty(ac) Or String.IsNullOrEmpty(pw) Then
                Response.Write(JsonConvert.SerializeObject(taifCattle_WS))
            Else
                taifCattle_WS = taifCattle_WS.Verify_Identity(ac, pw)
                taifCattle_WSBase.Insert_DataExchangeLog(
                    taifCattle.WS_Base.enum_dataSourceName.外部單位,
                    taifCattle.WS_Base.enum_apiName.資料介接身分驗證服務,
                    taifCattle_WS.passCode, 1,
                    taifCattle.WS_Base.enum_triggerType.其他,
                    False, "",
                    taifCattle.WS_Base.enum_actionType.dataOut)
                Response.Write(JsonConvert.SerializeObject(taifCattle_WS))
            End If
        End If
    End Sub
    Private Sub Verify_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Response.ContentType = "application/json"
    End Sub
End Class