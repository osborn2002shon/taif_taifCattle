Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Linq
Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel

Public Class HisEndManage_Batch
    Inherits taifCattle.Base

    Private ReadOnly taifReport As New taifCattle.Report()
    Private ReadOnly taifCattle_cattle As New taifCattle.Cattle()

    Private Property SuccessTable As DataTable
        Get
            Return TryCast(ViewState("SuccessTable"), DataTable)
        End Get
        Set(value As DataTable)
            ViewState("SuccessTable") = value
        End Set
    End Property

    Private Property FailureTable As DataTable
        Get
            Return TryCast(ViewState("FailureTable"), DataTable)
        End Get
        Set(value As DataTable)
            ViewState("FailureTable") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Label_message.Text = ""
        End If
    End Sub

    Private Sub LinkButton_import_Click(sender As Object, e As EventArgs) Handles LinkButton_import.Click
        ResetResultView()

        If Not FileUpload_excel.HasFile Then
            Label_message.Text = "請選擇要匯入的 Excel 檔案。"
            Return
        End If

        Dim ext As String = Path.GetExtension(FileUpload_excel.FileName).ToLower()
        If ext <> ".xls" AndAlso ext <> ".xlsx" Then
            Label_message.Text = "僅支援 .xls 或 .xlsx 檔案。"
            Return
        End If

        Dim uploadFolder As String = Server.MapPath("~/App_Data/Uploads")
        If Not Directory.Exists(uploadFolder) Then
            Directory.CreateDirectory(uploadFolder)
        End If

        Dim tempFile As String = Path.Combine(uploadFolder, $"hisend_batch_{Guid.NewGuid():N}{ext}")
        FileUpload_excel.SaveAs(tempFile)

        Try
            Dim schema As DataTable = CreateImportSchema()
            Dim excelData As DataTable = taifReport.Convert_ExcelToDataTable(tempFile, schema)

            If excelData.Rows.Count = 0 Then
                Label_message.Text = "匯入的 Excel 無資料。"
                Return
            End If

            ProcessImport(excelData)
        Catch ex As Exception
            Label_message.Text = "匯入發生錯誤：" & ex.Message
        Finally
            If File.Exists(tempFile) Then
                File.Delete(tempFile)
            End If
        End Try
    End Sub

    Private Sub LinkButton_downloadFailed_Click(sender As Object, e As EventArgs) Handles LinkButton_downloadFailed.Click
        Dim dtFailed As DataTable = FailureTable
        If dtFailed Is Nothing OrElse dtFailed.Rows.Count = 0 Then
            Label_message.Text = "目前沒有可下載的失敗資料。"
            Return
        End If

        Dim wb As IWorkbook = New XSSFWorkbook()
        Dim ws As ISheet = wb.CreateSheet("匯入失敗")

        Dim headers As String() = {
            "牛籍編號",
            "除籍日期",
            "類型（其他／未使用）",
            "畜牧場證號（牧場登記證、飼養登記證、身分證）",
            "除籍備註",
            "失敗原因"
        }

        Dim headerRow As IRow = ws.CreateRow(0)
        For i As Integer = 0 To headers.Length - 1
            headerRow.CreateCell(i).SetCellValue(headers(i))
        Next

        For r As Integer = 0 To dtFailed.Rows.Count - 1
            Dim dataRow As IRow = ws.CreateRow(r + 1)
            For c As Integer = 0 To headers.Length - 1
                dataRow.CreateCell(c).SetCellValue(Convert.ToString(dtFailed.Rows(r)(headers(c))))
            Next
        Next

        Using ms As New MemoryStream()
            wb.Write(ms)
            Response.Clear()
            Response.Buffer = True
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Response.AddHeader("Content-Disposition", "attachment; filename=除籍匯入失敗資料.xlsx")
            Response.BinaryWrite(ms.ToArray())
            Response.Flush()
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        End Using
    End Sub

    Private Sub ProcessImport(excelData As DataTable)
        Dim successTable As DataTable = CreateResultTable(False)
        Dim failedTable As DataTable = CreateResultTable(True)

        Dim historyTypeLookup As Dictionary(Of String, Integer) = GetHistoryTypeLookup()
        Dim allowedTypeNames As New HashSet(Of String)(New String() {"其他", "未使用"}, StringComparer.OrdinalIgnoreCase)
        Dim farmLookup As Dictionary(Of String, Integer) = GetFarmLookup()

        Dim userInfo As taifCattle.Base.stru_LoginUserInfo = CType(Session("userInfo"), taifCattle.Base.stru_LoginUserInfo)
        Dim insertUserId As Integer = userInfo.accountID

        For Each row As DataRow In excelData.Rows
            Dim reasons As New List(Of String)()

            Dim tagNo As String = Convert.ToString(row("牛籍編號")).Trim()
            Dim removeDateText As String = Convert.ToString(row("除籍日期")).Trim()
            Dim hisTypeName As String = Convert.ToString(row("類型（其他／未使用）")).Trim()
            Dim farmCode As String = Convert.ToString(row("畜牧場證號（牧場登記證、飼養登記證、身分證）")).Trim()
            Dim memo As String = Convert.ToString(row("除籍備註")).Trim()

            Dim cattleId As Integer = -1
            Dim needCreateCattle As Boolean = False

            If String.IsNullOrEmpty(tagNo) Then
                reasons.Add("牛籍編號未填寫")
            Else
                Dim checkResult As taifCattle.Base.stru_checkResult = taifCattle_cattle.Check_IsCattleExist(tagNo)
                If checkResult.isPass Then
                    cattleId = Convert.ToInt32(checkResult.msg)
                Else
                    needCreateCattle = True
                End If
            End If

            Dim hisTypeId As Integer = -1
            Dim normalizedTypeName As String = hisTypeName
            If String.IsNullOrEmpty(normalizedTypeName) Then
                reasons.Add("類型錯誤")
            Else
                Dim tempId As Integer = -1
                If historyTypeLookup.TryGetValue(normalizedTypeName, tempId) AndAlso allowedTypeNames.Contains(normalizedTypeName) Then
                    hisTypeId = tempId
                Else
                    reasons.Add("類型錯誤")
                End If
            End If

            Dim dataDateValue As DateTime = DateTime.MinValue
            If String.IsNullOrEmpty(removeDateText) Then
                reasons.Add("日期錯誤")
            Else
                Dim parsedDate As DateTime
                If DateTime.TryParse(removeDateText, parsedDate) Then
                    If parsedDate.Date > DateTime.Today Then
                        reasons.Add("日期錯誤")
                    Else
                        dataDateValue = parsedDate.Date
                    End If
                Else
                    reasons.Add("日期錯誤")
                End If
            End If

            Dim farmId As Integer = -1
            Dim normalizedFarmCode As String = farmCode
            If String.IsNullOrEmpty(normalizedFarmCode) Then
                reasons.Add("畜牧場錯誤")
            ElseIf Not farmLookup.TryGetValue(normalizedFarmCode, farmId) Then
                reasons.Add("畜牧場錯誤")
            End If

            If reasons.Count > 0 Then
                Dim failedRow As DataRow = failedTable.NewRow()
                FillCommonRowValues(failedRow, tagNo, removeDateText, hisTypeName, farmCode, memo)
                failedRow("失敗原因") = String.Join("、", reasons.Distinct())
                failedTable.Rows.Add(failedRow)
                Continue For
            End If

            Dim insertDate As DateTime = DateTime.Now
            Dim createdCattleId As Integer = -1
            Dim preHistoryId As Integer = -1
            Dim finalHistoryId As Integer = -1

            Try
                If needCreateCattle Then
                    createdCattleId = CreateCattleShell(tagNo, insertUserId, insertDate)
                    cattleId = createdCattleId
                End If

                preHistoryId = InsertHistoryRecord(New taifCattle.Cattle.stru_cattleHistory With {
                    .cattleID = cattleId,
                    .hisTypeID = 2,
                    .dataDate = dataDateValue.AddDays(-1),
                    .farmID = farmId,
                    .plantID = Nothing,
                    .slauID = Nothing,
                    .memo = Nothing,
                    .insertType = taifCattle.Base.enum_InsertType.除籍批次建檔,
                    .insertDateTime = insertDate,
                    .insertAccountID = insertUserId
                })

                finalHistoryId = InsertHistoryRecord(New taifCattle.Cattle.stru_cattleHistory With {
                    .cattleID = cattleId,
                    .hisTypeID = hisTypeId,
                    .dataDate = dataDateValue,
                    .farmID = farmId,
                    .plantID = Nothing,
                    .slauID = Nothing,
                    .memo = If(String.IsNullOrEmpty(memo), Nothing, memo),
                    .insertType = taifCattle.Base.enum_InsertType.除籍批次建檔,
                    .insertDateTime = insertDate,
                    .insertAccountID = insertUserId
                })

                Dim successRow As DataRow = successTable.NewRow()
                FillCommonRowValues(successRow, tagNo, dataDateValue.ToString("yyyy/MM/dd"), hisTypeName, farmCode, memo)
                successRow("匯入結果") = "成功"
                successTable.Rows.Add(successRow)

                Insert_UserLog(insertUserId, enum_UserLogItem.除籍批次設定功能, enum_UserLogType.新增, $"hisID:{finalHistoryId}")
            Catch ex As Exception
                If finalHistoryId > 0 Then
                    DeleteHistoryById(finalHistoryId)
                End If

                If preHistoryId > 0 Then
                    DeleteHistoryById(preHistoryId)
                End If

                If createdCattleId > 0 Then
                    DeleteCattleById(createdCattleId)
                End If

                Dim failedRow As DataRow = failedTable.NewRow()
                FillCommonRowValues(failedRow, tagNo, removeDateText, hisTypeName, farmCode, memo)
                failedRow("失敗原因") = "寫入資料庫失敗：" & ex.Message
                failedTable.Rows.Add(failedRow)
            End Try
        Next

        SuccessTable = successTable
        FailureTable = failedTable

        BindResult(successTable, failedTable)
    End Sub

    Private Sub BindResult(successTable As DataTable, failedTable As DataTable)
        Panel_success.Visible = successTable.Rows.Count > 0
        Panel_failed.Visible = failedTable.Rows.Count > 0

        GridView_success.DataSource = successTable
        GridView_success.DataBind()
        GridView_failed.DataSource = failedTable
        GridView_failed.DataBind()

        Label_successCount.Text = successTable.Rows.Count.ToString()
        Label_failedCount.Text = failedTable.Rows.Count.ToString()

        LinkButton_downloadFailed.Visible = failedTable.Rows.Count > 0

        Label_message.CssClass = If(failedTable.Rows.Count > 0, "text-danger fw-bold d-block mt-3", "text-success fw-bold d-block mt-3")
        Label_message.Text = $"匯入完成，成功 {successTable.Rows.Count} 筆，失敗 {failedTable.Rows.Count} 筆。"
    End Sub

    Private Sub ResetResultView()
        Panel_success.Visible = False
        Panel_failed.Visible = False
        GridView_success.DataSource = Nothing
        GridView_success.DataBind()
        GridView_failed.DataSource = Nothing
        GridView_failed.DataBind()
        Label_successCount.Text = "0"
        Label_failedCount.Text = "0"
        LinkButton_downloadFailed.Visible = False
        SuccessTable = Nothing
        FailureTable = Nothing
        Label_message.Text = ""
        Label_message.CssClass = "text-danger fw-bold d-block mt-3"
    End Sub

    Private Function CreateImportSchema() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("牛籍編號", GetType(String))
        dt.Columns.Add("除籍日期", GetType(String))
        dt.Columns.Add("類型（其他／未使用）", GetType(String))
        dt.Columns.Add("畜牧場證號（牧場登記證、飼養登記證、身分證）", GetType(String))
        dt.Columns.Add("除籍備註", GetType(String))
        Return dt
    End Function

    Private Function CreateResultTable(includeFailureReason As Boolean) As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("牛籍編號", GetType(String))
        dt.Columns.Add("除籍日期", GetType(String))
        dt.Columns.Add("類型（其他／未使用）", GetType(String))
        dt.Columns.Add("畜牧場證號（牧場登記證、飼養登記證、身分證）", GetType(String))
        dt.Columns.Add("除籍備註", GetType(String))
        If includeFailureReason Then
            dt.Columns.Add("失敗原因", GetType(String))
        Else
            dt.Columns.Add("匯入結果", GetType(String))
        End If
        Return dt
    End Function

    Private Sub FillCommonRowValues(targetRow As DataRow, tagNo As String, dataDate As String, hisType As String, farmCode As String, memo As String)
        targetRow("牛籍編號") = tagNo
        targetRow("除籍日期") = dataDate
        targetRow("類型（其他／未使用）") = hisType
        targetRow("畜牧場證號（牧場登記證、飼養登記證、身分證）") = farmCode
        targetRow("除籍備註") = memo
    End Sub

    Private Function GetHistoryTypeLookup() As Dictionary(Of String, Integer)
        Dim dict As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Using da As New DataAccess.MS_SQL()
            Dim sql As String = "select hisTypeID, typeName from Cattle_TypeHistory where typeName in (N'其他', N'未使用')"
            Dim dt As DataTable = da.GetDataTable(sql)
            For Each row As DataRow In dt.Rows
                Dim name As String = Convert.ToString(row("typeName")).Trim()
                If Not dict.ContainsKey(name) Then
                    dict.Add(name, Convert.ToInt32(row("hisTypeID")))
                End If
            Next
        End Using
        Return dict
    End Function

    Private Function GetFarmLookup() As Dictionary(Of String, Integer)
        Dim dict As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Using da As New DataAccess.MS_SQL()
            Dim sql As String = "select farmID, farmCode from List_Farm where removeDateTime is null"
            Dim dt As DataTable = da.GetDataTable(sql)
            For Each row As DataRow In dt.Rows
                Dim code As String = Convert.ToString(row("farmCode")).Trim()
                If Not String.IsNullOrEmpty(code) AndAlso Not dict.ContainsKey(code) Then
                    dict.Add(code, Convert.ToInt32(row("farmID")))
                End If
            Next
        End Using
        Return dict
    End Function

    Private Function CreateCattleShell(tagNo As String, insertUserId As Integer, insertDate As DateTime) As Integer
        Using da As New DataAccess.MS_SQL()
            Dim sql As String = "insert into Cattle_List (cattleTypeID, tagNo, insertType, insertDateTime, insertAccountID) values (@cattleTypeID, @tagNo, @insertType, @insertDateTime, @insertAccountID); select scope_identity();"
            Dim para As SqlParameter() = {
                New SqlParameter("cattleTypeID", 6),
                New SqlParameter("tagNo", tagNo),
                New SqlParameter("insertType", taifCattle.Base.enum_InsertType.除籍批次建檔.ToString()),
                New SqlParameter("insertDateTime", insertDate),
                New SqlParameter("insertAccountID", insertUserId)
            }
            Dim result As Object = da.ExecuteScalar(sql, para)
            Return Convert.ToInt32(Convert.ToDecimal(result))
        End Using
    End Function

    Private Function InsertHistoryRecord(hisInfo As taifCattle.Cattle.stru_cattleHistory) As Integer
        Dim result As Object = taifCattle_cattle.Insert_CattleHistory(hisInfo)
        Return Convert.ToInt32(result)
    End Function

    Private Sub DeleteHistoryById(hisId As Integer)
        Using da As New DataAccess.MS_SQL()
            Dim sql As String = "delete from Cattle_History where hisID = @hisID"
            da.ExecNonQuery(sql, New SqlParameter("hisID", hisId))
        End Using
    End Sub

    Private Sub DeleteCattleById(cattleId As Integer)
        Using da As New DataAccess.MS_SQL()
            Dim sqlDeleteHistory As String = "delete from Cattle_History where cattleID = @cattleID"
            da.ExecNonQuery(sqlDeleteHistory, New SqlParameter("cattleID", cattleId))
            Dim sqlDeleteCattle As String = "delete from Cattle_List where cattleID = @cattleID"
            da.ExecNonQuery(sqlDeleteCattle, New SqlParameter("cattleID", cattleId))
        End Using
    End Sub

End Class
