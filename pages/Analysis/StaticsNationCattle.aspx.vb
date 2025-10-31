Imports System.IO
Imports NPOI.OpenXmlFormats.Dml
Imports NPOI.SS.UserModel
Imports NPOI.SS.Util
Imports NPOI.XSSF.UserModel
Imports Org.BouncyCastle.Math.EC
Imports taifCattle.StaticsCityCattle

Public Class StaticsNationCattle
    Inherits taifCattle.Base
    Dim taifCattle_con As New taifCattle.Control
    Dim taifCattle_report As New taifCattle.Report
    Dim taifCattle_farm As New taifCattle.Farm

#Region "Property"

    ''' <summary>
    ''' 搜尋條件：縣市
    ''' </summary>
    Public Property Property_Query_city As String
        Get
            If IsNothing(ViewState("Property_FarmQuery_city")) Then
                Return "%"
            Else
                Return ViewState("Property_FarmQuery_city").ToString()
            End If
        End Get
        Set(value As String)
            ViewState("Property_FarmQuery_city") = value
        End Set
    End Property
#End Region

#Region "Fun/Sub"
    ''' <summary>
    ''' 取得各畜牧場現存牛隻統計資料
    ''' </summary>
    ''' <param name="cityID">縣市 ID</param>
    ''' <returns>牛隻統計 DataTable</returns>
    Function Get_CattleCurrentSummary(cityID As String) As Data.DataTable

        Dim sqlString As String =
    <sql>
        SELECT 
            
            twFarm.city AS farmCity,
            List_Farm.farmCode AS farmCode,
            List_Farm.farmName AS farmName,
            List_Farm.owner AS farmOwner,

           
            SUM(COUNT(*)) OVER (PARTITION BY List_Farm.farmID) AS totalCattleCount,
            ISNULL(Cattle_List.birthYear, -1) AS birthYear,

           
            COUNT(*) AS yearCattleCount,
            SUM(CASE WHEN Cattle_List.cattleTypeID = 2 THEN 1 ELSE 0 END) AS femaleDairyCount,
            SUM(CASE WHEN Cattle_List.cattleTypeID = 1 THEN 1 ELSE 0 END) AS maleDairyCount,
            SUM(CASE WHEN Cattle_List.cattleTypeID IN (3,4,5,6) THEN 1 ELSE 0 END) AS beefCount,
            SUM(CASE WHEN Cattle_List.cattleTypeID = -1 THEN 1 ELSE 0 END) AS otherCount,

            
            SUM(CASE WHEN Cattle_List.cattleTypeID = 2 AND ISNULL(vIns.isInsurance,0) = 1 THEN 1 ELSE 0 END) AS insuredFemaleDairyCount

        FROM Cattle_List
        LEFT JOIN View_CattleInsClaStatus vIns ON Cattle_List.tagNo = vIns.tagNo

        OUTER APPLY (
            SELECT TOP 1 
                histNonRemoval.hisTypeID,
                histNonRemoval.dataDate,
                histNonRemoval.farmID,
                histNonRemoval.memo
            FROM Cattle_History AS histNonRemoval
            INNER JOIN Cattle_TypeHistory AS typeRefNonRemoval
                ON histNonRemoval.hisTypeID = typeRefNonRemoval.hisTypeID
            WHERE histNonRemoval.cattleID = Cattle_List.cattleID
              AND typeRefNonRemoval.groupName &lt;&gt; N'除籍'
              AND histNonRemoval.removeDateTime IS NULL
            ORDER BY histNonRemoval.dataDate DESC, typeRefNonRemoval.orderby DESC
        ) AS latestNonRemoval

        LEFT JOIN List_Farm ON latestNonRemoval.farmID = List_Farm.farmID
        LEFT JOIN System_Taiwan AS twFarm  ON List_Farm.twID = twFarm.twID

        WHERE 
            Cattle_List.removeDateTime IS NULL
            AND (@cityID IS NULL OR twFarm.cityID = @cityID)
            AND NOT EXISTS (
                SELECT 1 FROM Cattle_History AS h
                INNER JOIN Cattle_TypeHistory AS th ON h.hisTypeID = th.hisTypeID
                WHERE h.cattleID = Cattle_List.cattleID
                  AND th.groupName = N'除籍'
                  AND h.removeDateTime IS NULL
            )

        GROUP BY 
            twFarm.city,
            List_Farm.farmID,
            List_Farm.farmCode,
            List_Farm.farmName,
            List_Farm.owner,
            ISNULL(Cattle_List.birthYear, -1)

        ORDER BY 
            twFarm.city,
            List_Farm.farmCode,
            ISNULL(Cattle_List.birthYear, -1);
    </sql>.Value

        ' === 組成參數 ===
        Dim para As New List(Of SqlClient.SqlParameter)
        para.Add(New SqlClient.SqlParameter("cityID", If(cityID Is Nothing OrElse cityID.ToString().Trim() = "%" OrElse cityID.ToString().Trim() = "", DBNull.Value, cityID)))

        ' === 執行查詢 ===
        Dim dt As New Data.DataTable
        Using da As New DataAccess.MS_SQL()
            dt = da.GetDataTable(sqlString, para.ToArray())
        End Using

        Return dt
    End Function
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then

            '縣市
            taifCattle_con.BindDropDownList_city(DropDownList_farmCity, False)
        End If
    End Sub

    Private Sub LinkButton_excel_Click(sender As Object, e As EventArgs) Handles LinkButton_excel.Click
        Property_Query_city = DropDownList_farmCity.SelectedValue

        ' === 取得資料 ===
        Dim dt As DataTable = Get_CattleCurrentSummary(Property_Query_city)

        ' === 建立 Workbook & Sheet ===
        Dim wb As New XSSFWorkbook()
        Dim ws As ISheet = wb.CreateSheet("牧場牛隻統計")

        ' === 樣式設定 ===
        Dim headerStyle As ICellStyle = wb.CreateCellStyle()
        headerStyle.Alignment = HorizontalAlignment.Center
        headerStyle.VerticalAlignment = VerticalAlignment.Center
        headerStyle.WrapText = True
        headerStyle.BorderTop = BorderStyle.Thin
        headerStyle.BorderBottom = BorderStyle.Thin
        headerStyle.BorderLeft = BorderStyle.Thin
        headerStyle.BorderRight = BorderStyle.Thin
        Dim fontHeader = wb.CreateFont()
        fontHeader.IsBold = True
        headerStyle.SetFont(fontHeader)

        Dim cellStyleCenter As ICellStyle = wb.CreateCellStyle()
        cellStyleCenter.Alignment = HorizontalAlignment.Center
        cellStyleCenter.VerticalAlignment = VerticalAlignment.Center
        cellStyleCenter.WrapText = True
        cellStyleCenter.BorderTop = BorderStyle.Thin
        cellStyleCenter.BorderBottom = BorderStyle.Thin
        cellStyleCenter.BorderLeft = BorderStyle.Thin
        cellStyleCenter.BorderRight = BorderStyle.Thin

        Dim cellStyleText As ICellStyle = wb.CreateCellStyle()
        cellStyleText.Alignment = HorizontalAlignment.Center
        cellStyleText.VerticalAlignment = VerticalAlignment.Center
        cellStyleText.WrapText = True
        cellStyleText.BorderTop = BorderStyle.Thin
        cellStyleText.BorderBottom = BorderStyle.Thin
        cellStyleText.BorderLeft = BorderStyle.Thin
        cellStyleText.BorderRight = BorderStyle.Thin
        cellStyleText.DataFormat = wb.CreateDataFormat().GetFormat("@")

        Dim cellStyleInteger As ICellStyle = wb.CreateCellStyle()
        cellStyleInteger.Alignment = HorizontalAlignment.Center
        cellStyleInteger.VerticalAlignment = VerticalAlignment.Center
        cellStyleInteger.WrapText = True
        cellStyleInteger.BorderTop = BorderStyle.Thin
        cellStyleInteger.BorderBottom = BorderStyle.Thin
        cellStyleInteger.BorderLeft = BorderStyle.Thin
        cellStyleInteger.BorderRight = BorderStyle.Thin
        cellStyleInteger.DataFormat = wb.CreateDataFormat().GetFormat("#,##0")

        Dim cellStyleLeft As ICellStyle = wb.CreateCellStyle()
        cellStyleLeft.Alignment = HorizontalAlignment.Left
        cellStyleLeft.VerticalAlignment = VerticalAlignment.Center
        cellStyleLeft.WrapText = True
        cellStyleLeft.BorderTop = BorderStyle.Thin
        cellStyleLeft.BorderBottom = BorderStyle.Thin
        cellStyleLeft.BorderLeft = BorderStyle.Thin
        cellStyleLeft.BorderRight = BorderStyle.Thin

        ' === 標題列 ===
        Dim headers() As String = {
            "畜牧場縣市", "畜牧場證號", "畜牧場名稱", "畜牧場負責人", "總頭數",
            "出生年度", "A" & vbLf & "頭數", "B" & vbLf & "乳母牛",
            "C" & vbLf & "乳公牛", "D" & vbLf & "肉牛", "E" & vbLf & "其他", "乳母牛已投保"
        }

        Dim rowHeader = ws.CreateRow(0)
        rowHeader.HeightInPoints = 30  ' 預留多行空間
        For i As Integer = 0 To headers.Length - 1
            Dim cell = rowHeader.CreateCell(i)
            cell.SetCellValue(headers(i))
            cell.CellStyle = headerStyle
        Next

        ' === 寫入資料 ===
        Dim rowIndex As Integer = 1
        Dim displayDash As String = "-"

        ' 按牧場分組
        Dim farmGroups = dt.AsEnumerable().GroupBy(Function(r) r("farmCode").ToString())

        ' 全域加總
        Dim totalSum As New Dictionary(Of String, Integer) From {
            {"yearCattleCount", 0},
            {"femaleDairyCount", 0},
            {"maleDairyCount", 0},
            {"beefCount", 0},
            {"otherCount", 0},
            {"insuredFemaleDairyCount", 0}
        }

        Dim mergeList As New List(Of CellRangeAddress)()
        For Each farmGroup In farmGroups
            Dim startRow As Integer = rowIndex
            Dim farm = farmGroup.First()

            ' 列出該牧場所有年度
            For Each yr In farmGroup
                Dim row = ws.CreateRow(rowIndex)

                ' 出生年度：特別處理 -1 顯示為「不明」
                row.CreateCell(5).SetCellValue(If(yr("birthYear") = -1, "不明", $"{yr("birthYear")}年"))
                row.GetCell(5).CellStyle = cellStyleCenter

                ' 數值欄位（整數樣式）
                row.CreateCell(6).SetCellValue(Convert.ToInt32(yr("yearCattleCount")))
                row.GetCell(6).CellStyle = cellStyleInteger

                row.CreateCell(7).SetCellValue(Convert.ToInt32(yr("femaleDairyCount")))
                row.GetCell(7).CellStyle = cellStyleInteger

                row.CreateCell(8).SetCellValue(Convert.ToInt32(yr("maleDairyCount")))
                row.GetCell(8).CellStyle = cellStyleInteger

                row.CreateCell(9).SetCellValue(Convert.ToInt32(yr("beefCount")))
                row.GetCell(9).CellStyle = cellStyleInteger

                row.CreateCell(10).SetCellValue(Convert.ToInt32(yr("otherCount")))
                row.GetCell(10).CellStyle = cellStyleInteger

                row.CreateCell(11).SetCellValue(Convert.ToInt32(yr("insuredFemaleDairyCount")))
                row.GetCell(11).CellStyle = cellStyleInteger

                ' 更新全域加總
                totalSum("yearCattleCount") += CInt(yr("yearCattleCount"))
                totalSum("femaleDairyCount") += CInt(yr("femaleDairyCount"))
                totalSum("maleDairyCount") += CInt(yr("maleDairyCount"))
                totalSum("beefCount") += CInt(yr("beefCount"))
                totalSum("otherCount") += CInt(yr("otherCount"))
                totalSum("insuredFemaleDairyCount") += CInt(yr("insuredFemaleDairyCount"))

                rowIndex += 1
            Next

            ' 合併欄位範圍樣式
            Dim endRow As Integer = rowIndex - 1
            taifCattle_report.ApplyCellStyle(ws, startRow, endRow, 0, 0, cellStyleCenter)
            taifCattle_report.ApplyCellStyle(ws, startRow, endRow, 1, 1, cellStyleText)
            taifCattle_report.ApplyCellStyle(ws, startRow, endRow, 2, 3, cellStyleCenter)
            taifCattle_report.ApplyCellStyle(ws, startRow, endRow, 4, 4, cellStyleInteger)

            If endRow > startRow Then
                mergeList.Add(New CellRangeAddress(startRow, endRow, 0, 0))
                mergeList.Add(New CellRangeAddress(startRow, endRow, 1, 1))
                mergeList.Add(New CellRangeAddress(startRow, endRow, 2, 2))
                mergeList.Add(New CellRangeAddress(startRow, endRow, 3, 3))
                mergeList.Add(New CellRangeAddress(startRow, endRow, 4, 4))
            End If

            ' 寫牧場基本資料
            Dim baseRow = ws.GetRow(startRow)
            baseRow.GetCell(0).SetCellValue(farm("farmCity").ToString())
            baseRow.GetCell(1).SetCellValue(taifCattle_farm.MaskFarmCode(farm("farmCode")).ToString())
            baseRow.GetCell(2).SetCellValue(farm("farmName").ToString())
            baseRow.GetCell(3).SetCellValue(farm("farmOwner").ToString())
            baseRow.GetCell(4).SetCellValue(Convert.ToInt32(farm("totalCattleCount")))
        Next

        ' === 加總列 ===
        Dim totalRow = ws.CreateRow(rowIndex)
        totalRow.CreateCell(0).SetCellValue("合計")

        ' 合併
        taifCattle_report.ApplyCellStyle(ws, rowIndex, rowIndex, 0, 5, headerStyle)
        mergeList.Add(New CellRangeAddress(rowIndex, rowIndex, 0, 5))

        ' 寫入合計數字
        totalRow.CreateCell(6).SetCellValue(totalSum("yearCattleCount"))
        totalRow.CreateCell(7).SetCellValue(totalSum("femaleDairyCount"))
        totalRow.CreateCell(8).SetCellValue(totalSum("maleDairyCount"))
        totalRow.CreateCell(9).SetCellValue(totalSum("beefCount"))
        totalRow.CreateCell(10).SetCellValue(totalSum("otherCount"))
        totalRow.CreateCell(11).SetCellValue(totalSum("insuredFemaleDairyCount"))

        For c As Integer = 6 To 11
            totalRow.GetCell(c).CellStyle = cellStyleInteger
        Next

        ' === 最後再一次合併 ===
        For Each r In mergeList
            ws.AddMergedRegionUnsafe(r)
        Next

        ' === 設定欄寬 ===
        ws.SetColumnWidth(0, 12 * 256)  ' 縣市
        ws.SetColumnWidth(1, 14 * 256)  ' 證號
        ws.SetColumnWidth(2, 20 * 256)  ' 名稱
        ws.SetColumnWidth(3, 12 * 256)  ' 負責人
        ws.SetColumnWidth(4, 8 * 256)   ' 總頭數
        ws.SetColumnWidth(5, 10 * 256)  ' 出生年度
        For i As Integer = 6 To 10
            ws.SetColumnWidth(i, 8 * 256)
        Next
        ws.SetColumnWidth(11, 9 * 256)

        ' === 輸出 ===
        Dim fileName As String = $"全國牛隻在養總表_{Now:yyyyMMddHHmmss}.xlsx"
        Using ms As New MemoryStream()
            wb.Write(ms)
            Response.Clear()
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Response.AddHeader("Content-Disposition", $"attachment; filename={HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8)}")
            Response.BinaryWrite(ms.ToArray())
            Response.End()
        End Using
    End Sub
End Class