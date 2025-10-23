Imports System.IO
Imports taifCattle.taifCattle.Base

Public Class sample
    Inherits System.Web.UI.Page

    Dim taifReport As New taifCattle.Report

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        lblMessage.Text = ""
        gvData.DataSource = Nothing
        gvData.DataBind()

        Try
            If Not fuExcel.HasFile Then
                lblMessage.Text = "請選擇 Excel 檔案。"
                Return
            End If

            ' 檔案儲存到暫存資料夾
            Dim savePath As String = Server.MapPath("~/_temp/")
            If Not Directory.Exists(savePath) Then Directory.CreateDirectory(savePath)
            Dim filePath As String = Path.Combine(savePath, Path.GetFileName(fuExcel.FileName))
            fuExcel.SaveAs(filePath)

            ' 取得欄位數
            Dim columnCount As Integer = 0
            If Not Integer.TryParse(txtColumnCount.Text, columnCount) OrElse columnCount <= 0 Then
                lblMessage.Text = "請輸入正確的欄位數。"
                Return
            End If

            ' 建立 DataTable Schema
            Dim dtSchema As New DataTable()
            For i As Integer = 1 To columnCount
                dtSchema.Columns.Add("欄位" & i.ToString())
            Next

            ' 解析 Excel
            Dim dt As DataTable = taifReport.Convert_ExcelToDataTable(filePath, dtSchema)
            gvData.DataSource = dt
            gvData.DataBind()

            lblMessage.ForeColor = Drawing.Color.Green
            lblMessage.Text = $"上傳完成，共 {dt.Rows.Count} 筆資料。"

        Catch ex As Exception
            lblMessage.Text = "讀取錯誤：" & ex.Message
        End Try
    End Sub

    Private Sub btnCheck_Click(sender As Object, e As EventArgs) Handles btnCheck.Click
        lblCheckResult.Text = ""
        Try
            ' 模擬載入時抓到的時間（假設先存一筆）
            If String.IsNullOrEmpty(hfLastUpdateTime.Value) Then
                hfLastUpdateTime.Value = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss.fff")
            End If

            Dim editId As Integer
            If Not Integer.TryParse(txtEditId.Text, editId) Then
                lblCheckResult.Text = "請輸入正確的 ID。"
                Return
            End If

            Dim userTime As DateTime = DateTime.Parse(hfLastUpdateTime.Value)

            ' 呼叫檢查函式
            If Not Check_DataNotChanged(enum_UserLogItem.牧場資料管理, editId, userTime) Then
                lblCheckResult.Text = "資料已於您編輯期間被其他使用者修改，請重新載入最新資料後再試。"
            Else
                lblCheckResult.ForeColor = Drawing.Color.Green
                lblCheckResult.Text = "資料未被修改，可以安全儲存。"
            End If

        Catch ex As Exception
            lblCheckResult.Text = "錯誤：" & ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' 根據操作項目 Enum 回傳資料表名稱與主鍵欄位名稱
    ''' </summary>
    Public Function GetTableInfoByLogItem(logItem As enum_UserLogItem) As (TableName As String, KeyName As String)
        Select Case logItem
            Case enum_UserLogItem.系統參數設定
                Return ("System_Config", "configID")
            Case enum_UserLogItem.牧場資料管理
                Return ("List_Farm", "farmID")
            Case enum_UserLogItem.牛籍資料管理
                Return ("Cattle_List", "cattleID")
            Case enum_UserLogItem.牛籍批次新增功能
                Return ("Cattle_Batch", "batchID")
            Case enum_UserLogItem.旅程批次新增功能
                Return ("Journey_Batch", "journeyBatchID")
            Case enum_UserLogItem.除籍批次設定功能
                Return ("Deregister_Batch", "batchID")
            Case Else
                Throw New ArgumentException($"無法對應的功能項目：{logItem}")
        End Select
    End Function

    ''' <summary>
    ''' 檢查資料是否未被他人修改
    ''' </summary>
    ''' <param name="logItem"></param>
    ''' <param name="editId"></param>
    ''' <param name="userTime"></param>
    ''' <returns></returns>
    Public Function Check_DataNotChanged(logItem As enum_UserLogItem, editId As Integer, userTime As DateTime) As Boolean
        Dim tableInfo = GetTableInfoByLogItem(logItem)
        Dim tableName As String = tableInfo.TableName
        Dim keyName As String = tableInfo.KeyName
        Dim sqlString As String =
        $"SELECT COUNT(1) FROM {tableName} WHERE {keyName} = @id AND updateDateTime = @userTime"

        Dim para As New List(Of Data.SqlClient.SqlParameter)
        para.Add(New Data.SqlClient.SqlParameter("id", editId))
        para.Add(New Data.SqlClient.SqlParameter("userTime", Data.SqlDbType.DateTime2) With {.Value = userTime})

        Using da As New DataAccess.MS_SQL
            Dim count As Integer = CInt(da.ExecuteScalar(sqlString, para.ToArray()))
            Return count > 0
        End Using
    End Function

End Class