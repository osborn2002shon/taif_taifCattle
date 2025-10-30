﻿Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Linq
Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel

Public Class HisManage_Batch
    Inherits taifCattle.Base

    Private ReadOnly taifReport As New taifCattle.Report()
    Private ReadOnly taifCattle_cattle As New taifCattle.Cattle()
    Private ReadOnly taifCattle_farm As New taifCattle.Farm()

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

    Private Property MissingFarmSerial As String
        Get
            Return Convert.ToString(ViewState("MissingFarmSerial"))
        End Get
        Set(value As String)
            ViewState("MissingFarmSerial") = value
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

        Dim uploadFolder As String = Server.MapPath("~/_doc/temp")
        If Not Directory.Exists(uploadFolder) Then
            Directory.CreateDirectory(uploadFolder)
        End If

        Dim tempFile As String = Path.Combine(uploadFolder, $"history_batch_{Guid.NewGuid():N}{ext}")
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

        Dim headers As String() = {"牛籍編號", "日期", "類型", "畜牧場證號", "旅程備註", "失敗原因"}
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
            Response.AddHeader("Content-Disposition", "attachment; filename=旅程匯入失敗資料.xlsx")
            Response.BinaryWrite(ms.ToArray())
            Response.Flush()
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        End Using
    End Sub

    Private Sub ProcessImport(excelData As DataTable)
        Dim successTable As DataTable = CreateResultTable(False)
        Dim failedTable As DataTable = CreateResultTable(True)

        Dim historyTypeLookup As Dictionary(Of String, Integer) = GetHistoryTypeLookup()
        Dim farmLookup As Dictionary(Of String, Integer) = GetFarmLookup()
        Dim missingFarmCodes As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        Dim userInfo As taifCattle.Base.stru_LoginUserInfo = Session("userInfo")

        Dim insertUserId As Integer = userInfo.accountID
        Dim insertDate As DateTime = DateTime.Now

        For Each row As DataRow In excelData.Rows
            Dim reasons As New List(Of String)()

            Dim tagNo As String = Convert.ToString(row("牛籍編號")).Trim()
            Dim dataDateText As String = Convert.ToString(row("日期")).Trim()
            Dim hisTypeName As String = Convert.ToString(row("類型")).Trim()
            Dim farmCode As String = Convert.ToString(row("畜牧場證號")).Trim()
            Dim memo As String = Convert.ToString(row("旅程備註")).Trim()

            Dim cattleId As Integer = -1
            If String.IsNullOrEmpty(tagNo) Then
                reasons.Add("找不到牛籍編號")
            Else
                Dim checkResult As taifCattle.Base.stru_checkResult = taifCattle_cattle.Check_IsCattleExist(tagNo)
                If checkResult.isPass Then
                    cattleId = Convert.ToInt32(checkResult.msg)
                Else
                    reasons.Add("找不到牛籍編號")
                End If
            End If

            Dim hisTypeId As Integer = -1
            If String.IsNullOrEmpty(hisTypeName) OrElse Not historyTypeLookup.TryGetValue(hisTypeName, hisTypeId) Then
                reasons.Add("類型錯誤")
            End If

            Dim dataDateValue As DateTime = DateTime.MinValue
            If String.IsNullOrEmpty(dataDateText) Then
                reasons.Add("日期錯誤")
            Else
                Dim parsedDate As DateTime
                If DateTime.TryParse(dataDateText, parsedDate) Then
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
            Dim shouldRecordMissingFarm As Boolean = False
            If String.IsNullOrEmpty(farmCode) Then
                reasons.Add("畜牧場錯誤")
            ElseIf farmLookup.TryGetValue(farmCode, farmId) = False Then
                reasons.Add("畜牧場錯誤")
                shouldRecordMissingFarm = True
            End If

            If shouldRecordMissingFarm Then
                missingFarmCodes.Add(farmCode)
            End If

            If reasons.Count > 0 Then
                Dim failedRow As DataRow = failedTable.NewRow()
                FillCommonRowValues(failedRow, tagNo, dataDateText, hisTypeName, farmCode, memo)
                failedRow("失敗原因") = String.Join("、", reasons.Distinct())
                failedTable.Rows.Add(failedRow)
                Continue For
            End If

            Try
                Dim hisInfo As New taifCattle.Cattle.stru_cattleHistory With {
                    .cattleID = cattleId,
                    .hisTypeID = hisTypeId,
                    .dataDate = dataDateValue,
                    .farmID = farmId,
                    .plantID = Nothing,
                    .slauID = Nothing,
                    .memo = If(String.IsNullOrEmpty(memo), Nothing, memo),
                    .insertType = taifCattle.Base.enum_InsertType.旅程批次建檔,
                    .insertDateTime = insertDate,
                    .insertAccountID = insertUserId
                }

                Dim hisId As Integer = Convert.ToInt32(taifCattle_cattle.Insert_CattleHistory(hisInfo))

                Dim successRow As DataRow = successTable.NewRow()
                FillCommonRowValues(successRow, tagNo, dataDateValue.ToString("yyyy/MM/dd"), hisTypeName, farmCode, memo)
                successRow("匯入結果") = "成功"
                successTable.Rows.Add(successRow)

                Insert_UserLog(insertUserId, enum_UserLogItem.旅程批次新增功能, enum_UserLogType.新增, $"hisID:{hisId}")
            Catch ex As Exception
                Dim failedRow As DataRow = failedTable.NewRow()
                FillCommonRowValues(failedRow, tagNo, dataDateText, hisTypeName, farmCode, memo)
                failedRow("失敗原因") = "寫入資料庫失敗：" & ex.Message
                failedTable.Rows.Add(failedRow)
            End Try
        Next

        If missingFarmCodes.Count > 0 Then
            Try
                Dim serial As String = taifCattle_farm.SaveMissingFarmCodes("HisManage_Batch", missingFarmCodes)
                If String.IsNullOrEmpty(serial) Then
                    MissingFarmSerial = Nothing
                Else
                    MissingFarmSerial = serial
                End If
            Catch ex As Exception
                MissingFarmSerial = Nothing
                System.Diagnostics.Trace.WriteLine($"[HisManage_Batch] SaveMissingFarmCodes failed: {ex}")
            End Try
        End If

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

        UpdateMissingFarmBatchLink()
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
        MissingFarmSerial = Nothing
        UpdateMissingFarmBatchLink()
    End Sub

    Private Sub UpdateMissingFarmBatchLink()
        Dim serial As String = MissingFarmSerial
        If String.IsNullOrEmpty(serial) Then
            HyperLink_missingFarmBatch.Visible = False
            HyperLink_missingFarmBatch.NavigateUrl = String.Empty
        Else
            HyperLink_missingFarmBatch.Visible = True
            HyperLink_missingFarmBatch.NavigateUrl = $"~/pages/Data/FarmManage_Batch.aspx?serial={Server.UrlEncode(serial)}"
        End If
    End Sub

    Private Function CreateImportSchema() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("牛籍編號", GetType(String))
        dt.Columns.Add("日期", GetType(String))
        dt.Columns.Add("類型", GetType(String))
        dt.Columns.Add("畜牧場證號", GetType(String))
        dt.Columns.Add("旅程備註", GetType(String))
        Return dt
    End Function

    Private Function CreateResultTable(includeFailureReason As Boolean) As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("牛籍編號", GetType(String))
        dt.Columns.Add("日期", GetType(String))
        dt.Columns.Add("類型", GetType(String))
        dt.Columns.Add("畜牧場證號", GetType(String))
        dt.Columns.Add("旅程備註", GetType(String))
        If includeFailureReason Then
            dt.Columns.Add("失敗原因", GetType(String))
        Else
            dt.Columns.Add("匯入結果", GetType(String))
        End If
        Return dt
    End Function

    Private Sub FillCommonRowValues(targetRow As DataRow, tagNo As String, dataDate As String, hisType As String, farmCode As String, memo As String)
        targetRow("牛籍編號") = tagNo
        targetRow("日期") = dataDate
        targetRow("類型") = hisType
        targetRow("畜牧場證號") = farmCode
        targetRow("旅程備註") = memo
    End Sub

    Private Function GetHistoryTypeLookup() As Dictionary(Of String, Integer)
        Dim dict As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Using da As New DataAccess.MS_SQL()
            Dim sql As String = "select hisTypeID, typeName from Cattle_TypeHistory where groupName = @groupName"
            Dim para As New SqlParameter("groupName", "旅程")
            Dim dt As DataTable = da.GetDataTable(sql, para)
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

End Class
