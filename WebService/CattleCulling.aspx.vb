Imports Newtonsoft.Json

Public Class CattleCulling
    Inherits System.Web.UI.Page
    Dim taifCattle_WSBase As New taifCattle.WS_Base
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Dim taifCattle_WS As New taifCattle.WS_CattleCulling
            Dim token As String = Request.QueryString("token")
            Dim type As String = Request.QueryString("type")
            Dim dateBegStr As String = Request.QueryString("dateBeg")
            Dim dateEndStr As String = Request.QueryString("dateEnd")

            Dim dateBeg As Date
            Dim dateEnd As Date

            Try
                '=== 日期必傳檢查
                If String.IsNullOrEmpty(dateBegStr) OrElse String.IsNullOrEmpty(dateEndStr) Then
                    Throw New Exception("起訖日期不得為空，請傳入 dateBeg 與 dateEnd 參數")
                End If

                '=== 日期格式檢查
                If Not Date.TryParse(dateBegStr, dateBeg) Then
                    Throw New Exception("起始日期格式不正確")
                End If
                If Not Date.TryParse(dateEndStr, dateEnd) Then
                    Throw New Exception("結束日期格式不正確")
                End If

                '=== 若日期顛倒，則自動交換
                If dateEnd < dateBeg Then
                    Dim tmp As Date = dateBeg
                    dateBeg = dateEnd
                    dateEnd = tmp
                End If

                '=== 取得資料
                Dim result As taifCattle.WS_CattleCulling.stru_resultWithList = taifCattle_WS.Get_CullingList_ByToken(token, dateBeg, dateEnd)

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
                    taifCattle.WS_Base.enum_apiName.撲殺補償牛籍編號清單查詢服務,
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

End Class