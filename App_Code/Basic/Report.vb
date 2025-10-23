Imports NPOI.HSSF.UserModel
Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel
Imports System.IO

Namespace taifCattle
    Public Class Report
        ''' <summary>
        ''' 使用 NPOI 將 Excel 內容讀入 DataTable（支援 .xls / .xlsx）
        ''' </summary>
        ''' <param name="serverExcelFilePath">Excel 檔案路徑</param>
        ''' <param name="dtSchema">DataTable 欄位結構 (外部傳入 Schema)</param>
        ''' <returns>包含 Excel 資料的 DataTable</returns>
        Public Function Convert_ExcelToDataTable(serverExcelFilePath As String, dtSchema As Data.DataTable) As Data.DataTable
            Dim ext As String = Path.GetExtension(serverExcelFilePath).ToLower()

            Try
                Using fs As New FileStream(serverExcelFilePath, FileMode.Open, FileAccess.Read)
                    Dim workbook As IWorkbook = Nothing

                    Select Case ext
                        Case ".xlsx"
                            workbook = New XSSFWorkbook(fs) ' Excel 2007+
                        Case ".xls"
                            workbook = New HSSFWorkbook(fs) ' Excel 2003
                        Case Else
                            Throw New Exception("不支援的檔案格式，僅支援 .xls 或 .xlsx")
                    End Select

                    Dim sheet As ISheet = workbook.GetSheetAt(0)
                    If sheet Is Nothing Then Throw New Exception("Excel 檔案中找不到任何工作表。")

                    ' 從第 2 列開始讀取（略過標題列）
                    For rowIndex As Integer = 1 To sheet.LastRowNum
                        Dim row As IRow = sheet.GetRow(rowIndex)
                        If row Is Nothing Then Exit For

                        ' 判斷該列是否完全空白（所有欄位都空）
                        Dim isEmptyRow As Boolean = True
                        For i As Integer = 0 To dtSchema.Columns.Count - 1
                            Dim cell As ICell = row.GetCell(i)
                            If cell IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(GetCellStringValue(cell)) Then
                                isEmptyRow = False
                                Exit For
                            End If
                        Next
                        If isEmptyRow Then Exit For ' 這列完全空白，略過

                        Dim newRow As Data.DataRow = dtSchema.NewRow()

                        For i As Integer = 0 To dtSchema.Columns.Count - 1
                            Dim cell As ICell = row.GetCell(i)
                            newRow(i) = GetCellStringValue(cell)
                        Next

                        dtSchema.Rows.Add(newRow)
                    Next
                End Using

                Return dtSchema

            Catch ex As Exception
                Throw New Exception("讀取 Excel 發生錯誤：" & ex.Message, ex)
            End Try
        End Function


        ''' <summary>
        ''' 取得 NPOI 儲存格內容
        ''' </summary>
        Private Function GetCellStringValue(cell As ICell) As String
            If cell Is Nothing Then Return String.Empty

            Select Case cell.CellType
                Case CellType.String
                    Return cell.StringCellValue.Trim()
                Case CellType.Numeric
                    If DateUtil.IsCellDateFormatted(cell) Then
                        Return cell.DateCellValue.ToString("yyyy/MM/dd")
                    Else
                        Return cell.NumericCellValue.ToString()
                    End If
                Case CellType.Boolean
                    Return cell.BooleanCellValue.ToString()
                Case CellType.Formula
                    Return cell.ToString()
                Case Else
                    Return String.Empty
            End Select
        End Function

    End Class
End Namespace
