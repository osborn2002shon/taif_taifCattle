Imports Newtonsoft.Json
Public Class CattleUninsured
    Inherits System.Web.UI.Page

    Dim taifCattle_WSBase As New taifCattle.WS_Base
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Dim taifCattle_WS As New taifCattle.WS_CattleUninsured
            Dim token As String = Request.QueryString("token")
            Dim type As String = Request.QueryString("type")

            Try
                '=== 取得資料
                Dim result As taifCattle.WS_CattleUninsured.stru_resultWithList = taifCattle_WS.Get_UninsuredCattleList_ByToken(token)

                '=== 若驗證或撈取失敗，直接回傳錯誤訊息
                If result.isPass = False Then
                    Response.Write(JsonConvert.SerializeObject(result.msg))
                Else
                    '=== 驗證通過，回傳清單資料
                    Response.Write(JsonConvert.SerializeObject(result.dataList))
                End If

                '=== 轉換，對應不到則預設為「其他」
                Dim triggerType As taifCattle.WS_Base.enum_triggerType = taifCattle.WS_Base.enum_triggerType.其他
                If Not String.IsNullOrEmpty(type) Then
                    [Enum].TryParse(type, True, triggerType)
                    If Not [Enum].IsDefined(GetType(taifCattle.WS_Base.enum_triggerType), triggerType) Then
                        triggerType = taifCattle.WS_Base.enum_triggerType.其他
                    End If
                End If

                '=== 寫入LOG
                taifCattle_WSBase.Insert_DataExchangeLog(
                    taifCattle.WS_Base.enum_dataSourceName.外部單位,
                    taifCattle.WS_Base.enum_apiName.滿一歲未投保之乳牛清單,
                    token,
                    result.dataList.Count,
                    triggerType,
                    Not result.isPass,
                    If(Not result.isPass, result.msg, ""),
                    taifCattle.WS_Base.enum_actionType.dataOut
                )

            Catch ex As Exception
                '=== 系統例外情況，回傳錯誤訊息
                Response.Write(JsonConvert.SerializeObject("系統發生錯誤：" & ex.Message))
            Finally
                Response.End()
            End Try
        End If
    End Sub

    Private Sub CattleUninsured_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Response.ContentType = "application/json"
    End Sub
End Class