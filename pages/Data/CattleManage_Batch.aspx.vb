Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web
Imports System.Linq
Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel

Public Class CattleManage_Batch
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

        Dim tempFile As String = Path.Combine(uploadFolder, $"cattle_batch_{Guid.NewGuid():N}{ext}")
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

        Dim headers As String() = {"牛籍編號", "牛籍編號備註", "品項", "出生年度", "牛籍備註", "類型", "日期", "畜牧場證號", "失敗原因"}
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
            Response.AddHeader("Content-Disposition", "attachment; filename=匯入失敗資料.xlsx")
            Response.BinaryWrite(ms.ToArray())
            Response.Flush()
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        End Using
    End Sub

    Private Sub ProcessImport(excelData As DataTable)
        Dim successTable As DataTable = CreateResultTable(False)
        Dim failedTable As DataTable = CreateResultTable(True)

        Dim cattleTypeLookup As Dictionary(Of String, Integer) = GetCattleTypeLookup()
        Dim historyTypeLookup As Dictionary(Of String, Integer) = GetHistoryTypeLookup()
        Dim farmLookup As Dictionary(Of String, Integer) = GetFarmLookup()

        Dim userInfo As taifCattle.Base.stru_LoginUserInfo = Session("userInfo")
        Dim insertUserId As Integer = userInfo.accountID

        For Each row As DataRow In excelData.Rows
            Dim reasons As New List(Of String)()

            Dim tagNo As String = Convert.ToString(row("牛籍編號")).Trim()
            Dim tagMemo As String = Convert.ToString(row("牛籍編號備註")).Trim()
            Dim itemName As String = Convert.ToString(row("品項")).Trim()
            Dim birthYearText As String = Convert.ToString(row("出生年度")).Trim()
            Dim cattleMemo As String = Convert.ToString(row("牛籍備註")).Trim()
            Dim hisTypeName As String = Convert.ToString(row("類型")).Trim()
            Dim dataDateText As String = Convert.ToString(row("日期")).Trim()
            Dim farmCode As String = Convert.ToString(row("畜牧場證號")).Trim()

            If String.IsNullOrEmpty(tagNo) Then
                reasons.Add("牛籍編號未提供")
            ElseIf taifCattle_cattle.Check_IsCattleExist(tagNo).isPass Then
                reasons.Add("已有重複牛籍編號")
            End If

            Dim cattleTypeId As Integer = -1
            If String.IsNullOrEmpty(itemName) Then
                reasons.Add("品項錯誤")
            ElseIf cattleTypeLookup.ContainsKey(itemName) Then
                cattleTypeId = cattleTypeLookup(itemName)
            Else
                reasons.Add("品項錯誤")
            End If

            Dim birthYearValue As Object = DBNull.Value
            Dim isDairyCattle As Boolean = String.Equals(itemName, "乳公牛", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(itemName, "乳母牛", StringComparison.OrdinalIgnoreCase)
            If Not String.IsNullOrEmpty(birthYearText) Then
                Dim yearValue As Integer
                If Integer.TryParse(birthYearText, yearValue) Then
                    If yearValue < 1911 Then
                        yearValue += 1911
                    End If
                    If yearValue < 1911 OrElse yearValue > DateTime.Now.Year Then
                        reasons.Add("出生年度錯誤")
                    Else
                        birthYearValue = yearValue
                    End If
                Else
                    reasons.Add("出生年度錯誤")
                End If
            ElseIf isDairyCattle AndAlso Not String.IsNullOrEmpty(tagNo) Then
                Dim derivedYear As Integer
                If TryGetDerivedBirthYear(tagNo, derivedYear) Then
                    birthYearValue = derivedYear
                Else
                    reasons.Add("出生年度錯誤")
                End If
            End If

            Dim needHistory As Boolean = Not String.IsNullOrEmpty(hisTypeName) OrElse Not String.IsNullOrEmpty(dataDateText) OrElse Not String.IsNullOrEmpty(farmCode)
            Dim hisTypeId As Integer = -1
            Dim dataDateValue As DateTime = DateTime.MinValue
            Dim farmId As Integer = -1

            If needHistory Then
                If String.IsNullOrEmpty(hisTypeName) OrElse Not historyTypeLookup.ContainsKey(hisTypeName) Then
                    reasons.Add("類型錯誤")
                Else
                    hisTypeId = historyTypeLookup(hisTypeName)
                End If

                If String.IsNullOrEmpty(dataDateText) Then
                    reasons.Add("日期錯誤")
                Else
                    Dim dateValue As DateTime
                    If DateTime.TryParse(dataDateText, dateValue) Then
                        If dateValue.Date > DateTime.Today Then
                            reasons.Add("日期錯誤")
                        Else
                            dataDateValue = dateValue.Date
                        End If
                    Else
                        reasons.Add("日期錯誤")
                    End If
                End If

                If String.IsNullOrEmpty(farmCode) OrElse Not farmLookup.ContainsKey(farmCode) Then
                    reasons.Add("畜牧場錯誤")
                Else
                    farmId = farmLookup(farmCode)
                End If
            End If

            If reasons.Count > 0 Then
                Dim failedRow As DataRow = failedTable.NewRow()
                FillCommonRowValues(failedRow, tagNo, tagMemo, itemName, birthYearText, cattleMemo, hisTypeName, dataDateText, farmCode)
                failedRow("失敗原因") = String.Join("、", reasons.Distinct())
                failedTable.Rows.Add(failedRow)
                Continue For
            End If

            Dim insertDate As DateTime = DateTime.Now
            Dim cattleId As Integer = -1

            Try
                Dim cattleInfo As New taifCattle.Cattle.stru_cattleInfo With {
                    .cattleTypeID = cattleTypeId,
                    .tagNo = tagNo,
                    .tagMemo = tagMemo,
                    .birthYear = birthYearValue,
                    .cattleMemo = cattleMemo,
                    .insertType = taifCattle.Base.enum_InsertType.人工批次建檔,
                    .insertDateTime = insertDate,
                    .updateDateTime = insertDate,
                    .insertAccountID = insertUserId,
                    .updateAccountID = insertUserId
                }

                cattleId = Convert.ToInt32(taifCattle_cattle.Insert_Cattle(cattleInfo))
                UpdateInsertType("Cattle_List", "cattleID", cattleId)

                If needHistory Then
                    Dim hisInfo As New taifCattle.Cattle.stru_cattleHistory With {
                        .cattleID = cattleId,
                        .hisTypeID = hisTypeId,
                        .dataDate = dataDateValue,
                        .farmID = farmId,
                        .plantID = Nothing,
                        .slauID = Nothing,
                        .memo = Nothing,
                        .insertType = taifCattle.Base.enum_InsertType.人工批次建檔,
                        .insertDateTime = insertDate,
                        .insertAccountID = insertUserId
                    }
                    Dim hisId As Integer = Convert.ToInt32(taifCattle_cattle.Insert_CattleHistory(hisInfo))
                    UpdateInsertType("Cattle_History", "hisID", hisId)
                End If

                Dim successRow As DataRow = successTable.NewRow()
                Dim birthYearDisplay As String = If(birthYearValue Is DBNull.Value, "", birthYearValue.ToString())
                Dim dateDisplay As String = If(needHistory, dataDateValue.ToString("yyyy/MM/dd"), "")
                FillCommonRowValues(successRow, tagNo, tagMemo, itemName, birthYearDisplay, cattleMemo, If(needHistory, hisTypeName, ""), dateDisplay, If(needHistory, farmCode, ""))
                successRow("匯入結果") = "成功"
                successTable.Rows.Add(successRow)

                Insert_UserLog(insertUserId, enum_UserLogItem.牛籍批次新增功能, enum_UserLogType.新增, $"cattleID:{cattleId}")
            Catch ex As Exception
                If cattleId > 0 Then
                    CleanUpInsertedData(cattleId)
                End If

                Dim failedRow As DataRow = failedTable.NewRow()
                FillCommonRowValues(failedRow, tagNo, tagMemo, itemName, birthYearText, cattleMemo, hisTypeName, dataDateText, farmCode)
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
        dt.Columns.Add("牛籍編號備註", GetType(String))
        dt.Columns.Add("品項", GetType(String))
        dt.Columns.Add("出生年度", GetType(String))
        dt.Columns.Add("牛籍備註", GetType(String))
        dt.Columns.Add("類型", GetType(String))
        dt.Columns.Add("日期", GetType(String))
        dt.Columns.Add("畜牧場證號", GetType(String))
        Return dt
    End Function

    Private Function CreateResultTable(includeFailureReason As Boolean) As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("牛籍編號", GetType(String))
        dt.Columns.Add("牛籍編號備註", GetType(String))
        dt.Columns.Add("品項", GetType(String))
        dt.Columns.Add("出生年度", GetType(String))
        dt.Columns.Add("牛籍備註", GetType(String))
        dt.Columns.Add("類型", GetType(String))
        dt.Columns.Add("日期", GetType(String))
        dt.Columns.Add("畜牧場證號", GetType(String))
        If includeFailureReason Then
            dt.Columns.Add("失敗原因", GetType(String))
        Else
            dt.Columns.Add("匯入結果", GetType(String))
        End If
        Return dt
    End Function

    Private Sub FillCommonRowValues(targetRow As DataRow, tagNo As String, tagMemo As String, itemName As String, birthYear As String, cattleMemo As String, hisType As String, dataDate As String, farmCode As String)
        targetRow("牛籍編號") = tagNo
        targetRow("牛籍編號備註") = tagMemo
        targetRow("品項") = itemName
        targetRow("出生年度") = birthYear
        targetRow("牛籍備註") = cattleMemo
        targetRow("類型") = hisType
        targetRow("日期") = dataDate
        targetRow("畜牧場證號") = farmCode
    End Sub

    Private Function GetCattleTypeLookup() As Dictionary(Of String, Integer)
        Dim dict As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Using da As New DataAccess.MS_SQL()
            Dim sql As String = "select cattleTypeID, typeName from Cattle_TypeCattle where isActive = 1"
            Dim dt As DataTable = da.GetDataTable(sql)
            For Each row As DataRow In dt.Rows
                Dim name As String = Convert.ToString(row("typeName")).Trim()
                If Not dict.ContainsKey(name) Then
                    dict.Add(name, Convert.ToInt32(row("cattleTypeID")))
                End If
            Next
        End Using
        Return dict
    End Function

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

    Private Function TryGetDerivedBirthYear(tagNo As String, ByRef derivedYear As Integer) As Boolean
        If String.IsNullOrEmpty(tagNo) Then
            Return False
        End If

        Dim length As Integer = tagNo.Length
        If length <> 7 AndAlso length <> 8 Then
            Return False
        End If

        Dim prefix As String = tagNo.Substring(0, 2)
        Dim prefixValue As Integer
        If Not Integer.TryParse(prefix, prefixValue) Then
            Return False
        End If

        Dim candidate As Integer = prefixValue + 100 + 1911
        If candidate < 1911 OrElse candidate > DateTime.Now.Year Then
            Return False
        End If

        derivedYear = candidate
        Return True
    End Function

    Private Sub UpdateInsertType(tableName As String, keyColumn As String, keyValue As Integer)
        Dim sql As String = $"update {tableName} set insertType = @insertType where {keyColumn} = @keyValue"
        Using da As New DataAccess.MS_SQL()
            da.ExecNonQuery(sql,
                            New SqlParameter("insertType", enum_UserLogItem.牛籍批次新增功能.ToString()),
                            New SqlParameter("keyValue", keyValue))
        End Using
    End Sub

    Private Sub CleanUpInsertedData(cattleId As Integer)
        Using da As New DataAccess.MS_SQL()
            Dim sqlDeleteHistory As String = "delete from Cattle_History where cattleID = @cattleID"
            da.ExecNonQuery(sqlDeleteHistory, New SqlParameter("cattleID", cattleId))
            Dim sqlDeleteCattle As String = "delete from Cattle_List where cattleID = @cattleID"
            da.ExecNonQuery(sqlDeleteCattle, New SqlParameter("cattleID", cattleId))
        End Using
    End Sub
End Class
