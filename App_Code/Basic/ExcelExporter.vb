Imports System.Data
Imports System.IO
Imports System.Web
Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel

Namespace taifCattle
    Public NotInheritable Class ExcelExporter
        Private Sub New()
        End Sub

        Public Class ColumnDefinition
            Public Property Header As String
            Public Property FieldName As String
            Public Property ValueSelector As Func(Of DataRow, String)
        End Class

        Public Shared Sub ExportDataTable(response As HttpResponse, fileName As String, sheetName As String, data As DataTable, columns As IList(Of ColumnDefinition))
            If response Is Nothing Then
                Throw New ArgumentNullException(NameOf(response))
            End If
            If data Is Nothing Then
                Throw New ArgumentNullException(NameOf(data))
            End If
            If columns Is Nothing OrElse columns.Count = 0 Then
                Throw New ArgumentException("必須提供欄位定義。", NameOf(columns))
            End If

            Dim workbook As IWorkbook = New XSSFWorkbook()
            Dim worksheet As ISheet = workbook.CreateSheet(sheetName)

            Dim headerRow As IRow = worksheet.CreateRow(0)
            For i As Integer = 0 To columns.Count - 1
                headerRow.CreateCell(i).SetCellValue(columns(i).Header)
            Next

            For rowIndex As Integer = 0 To data.Rows.Count - 1
                Dim dataRow As DataRow = data.Rows(rowIndex)
                Dim sheetRow As IRow = worksheet.CreateRow(rowIndex + 1)

                For colIndex As Integer = 0 To columns.Count - 1
                    Dim column = columns(colIndex)
                    Dim cellValue As String = String.Empty

                    If column.ValueSelector IsNot Nothing Then
                        cellValue = Convert.ToString(column.ValueSelector(dataRow))
                    ElseIf data.Columns.Contains(column.FieldName) Then
                        cellValue = Convert.ToString(dataRow(column.FieldName))
                    End If

                    sheetRow.CreateCell(colIndex).SetCellValue(cellValue)
                Next
            Next

            For colIndex As Integer = 0 To columns.Count - 1
                worksheet.AutoSizeColumn(colIndex)
            Next

            Using ms As New MemoryStream()
                workbook.Write(ms)
                response.Clear()
                response.Buffer = True
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"

                Dim encodedFileName As String = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8)
                Dim disposition As String = $"attachment; filename={encodedFileName}; filename*=UTF-8''{Uri.EscapeDataString(fileName)}"
                response.AddHeader("Content-Disposition", disposition)

                response.BinaryWrite(ms.ToArray())
                response.Flush()
                HttpContext.Current.ApplicationInstance.CompleteRequest()
            End Using
        End Sub
    End Class
End Namespace
