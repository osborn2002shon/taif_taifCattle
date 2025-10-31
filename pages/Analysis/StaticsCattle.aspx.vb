Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel
Imports System.IO
Imports taifCattle.taifCattle

Public Class StaticsCattle
    Inherits taifCattle.Base

    Dim taifCattle_cattle As New taifCattle.Cattle
    Dim taifCattle_farm As New taifCattle.Farm
#Region "Property"
    ''' <summary>
    ''' 搜尋條件：牛籍流水號
    ''' </summary>
    Public Property Property_Query_cattleID As Integer
        Get
            If IsNothing(ViewState("Property_Query_cattleID")) Then
                Return -1
            Else
                Return ViewState("Property_Query_cattleID").ToString()
            End If
        End Get
        Set(value As Integer)
            ViewState("Property_Query_cattleID") = value
        End Set
    End Property
#End Region
#Region "Fun/Sub"
    Public Function MaskFarmCode(value As Object) As String
        Return taifCattle_farm.MaskFarmCode(value)
    End Function
    Sub LoadInfo()
        Dim cattleInfo As taifCattle.Cattle.stru_cattleInfo_view = taifCattle_cattle.Get_CattleInfo(Property_Query_cattleID)
        TextBox_info_tagNo.Text = cattleInfo.tagNo
        TextBox_info_tagMemo.Text = cattleInfo.tagMemo
        TextBox_info_memo.Text = cattleInfo.cattleMemo
        TextBox_info_birthYear.Text = IIf(cattleInfo.birthYear = -1, "", cattleInfo.birthYear)
        TextBox_info_cattleAge.Text = IIf(cattleInfo.cattleAge = -1, "-", cattleInfo.cattleAge)
        If cattleInfo.cattleTypeID = 2 Then
            '乳母牛才有乳量、並且要有出生年度才能算
            TextBox_info_milkProduction.Text = IIf(cattleInfo.milkProduction = -1, "-", cattleInfo.milkProduction)
        Else
            TextBox_info_milkProduction.Text = "-"
        End If
        TextBox_info_typeName.Text = cattleInfo.groupName + "：" + cattleInfo.typeName

    End Sub

    Sub Bind_Gridview()
        Dim liHistory As List(Of taifCattle.Cattle.stru_cattleHistory_view) = taifCattle_cattle.Get_CattleHistoryList(Property_Query_cattleID, taifCattle.Cattle.enum_hisType.allHis)

        GridView_tagNoHistory.DataSource = liHistory
        GridView_tagNoHistory.DataBind()

        Label_recordCount.Text = liHistory.Count.ToString("N0")

    End Sub
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub LinkButton_query_Click(sender As Object, e As EventArgs) Handles LinkButton_query.Click
        Dim tagNo As String = TextBox_tagNo.Text.Trim()
        If String.IsNullOrEmpty(tagNo) Then
            Label_msg.Text = "請輸入牛籍編號。"
            Panel_result.Visible = False
            Exit Sub
        End If

        '檢查牛籍邊號
        Dim result As stru_checkResult = taifCattle_cattle.Check_IsCattleExist(tagNo)
        If result.msg = "-1" Then
            Label_msg.Text = "查無此牛籍編號。"
            Panel_result.Visible = False
            Exit Sub
        End If

        '紀錄牛籍編號
        Property_Query_cattleID = CInt(result.msg)

        '載入基本資料
        LoadInfo()

        '取得旅程
        Bind_Gridview()

        Panel_result.Visible = True
        Label_msg.Text = ""
    End Sub

    Private Sub LinkButton_excel_Click(sender As Object, e As EventArgs) Handles LinkButton_excel.Click
        Dim liHistory As List(Of taifCattle.Cattle.stru_cattleHistory_view) =
        taifCattle_cattle.Get_CattleHistoryList(Property_Query_cattleID, taifCattle.Cattle.enum_hisType.allHis)

        ' === 建立 Workbook & Sheet ===
        Dim wb As New XSSFWorkbook()
        Dim ws As ISheet = wb.CreateSheet("牛籍歷程")

        ' === DataFormat ===
        Dim dataFormat = wb.CreateDataFormat()

        ' === 標題樣式 ===
        Dim headerStyle As ICellStyle = wb.CreateCellStyle()
        headerStyle.Alignment = HorizontalAlignment.Center
        headerStyle.VerticalAlignment = VerticalAlignment.Center
        headerStyle.WrapText = True
        Dim fontHeader = wb.CreateFont()
        fontHeader.IsBold = True
        headerStyle.SetFont(fontHeader)

        ' === 一般置中樣式 ===
        Dim cellStyleCenter As ICellStyle = wb.CreateCellStyle()
        cellStyleCenter.Alignment = HorizontalAlignment.Center
        cellStyleCenter.VerticalAlignment = VerticalAlignment.Center
        cellStyleCenter.WrapText = True

        ' === 文字樣式（證號）===
        Dim cellStyleText As ICellStyle = wb.CreateCellStyle()
        cellStyleText.Alignment = HorizontalAlignment.Center
        cellStyleText.VerticalAlignment = VerticalAlignment.Center
        cellStyleText.WrapText = True
        cellStyleText.DataFormat = dataFormat.GetFormat("@")

        ' === 左對齊樣式（備註）===
        Dim cellStyleLeft As ICellStyle = wb.CreateCellStyle()
        cellStyleLeft.Alignment = HorizontalAlignment.Left
        cellStyleLeft.VerticalAlignment = VerticalAlignment.Center
        cellStyleLeft.WrapText = True

        ' === 標題列 ===
        Dim headers() As String = {
            "日期",
            "類型",
            "縣市",
            "鄉鎮",
            "證號",
            "名稱",
            "負責人",
            "資料來源",
            "資料建立時間"
        }

        Dim rowHeader = ws.CreateRow(0)
        For i As Integer = 0 To headers.Length - 1
            Dim cell = rowHeader.CreateCell(i)
            cell.SetCellValue(headers(i))
            cell.CellStyle = headerStyle
        Next

        ' === 資料列 ===
        For r As Integer = 0 To liHistory.Count - 1
            Dim f = liHistory(r)
            Dim row = ws.CreateRow(r + 1)

            ' 日期
            Dim c0 = row.CreateCell(0)
            c0.SetCellValue(f.dataDate.ToString("yyyy/MM/dd"))
            c0.CellStyle = cellStyleCenter

            ' 類型
            Dim c1 = row.CreateCell(1)
            c1.SetCellValue(f.typeName)
            c1.CellStyle = cellStyleCenter

            ' 縣市
            Dim c2 = row.CreateCell(2)
            c2.SetCellValue(f.city)
            c2.CellStyle = cellStyleCenter

            ' 鄉鎮
            Dim c3 = row.CreateCell(3)
            c3.SetCellValue(f.area)
            c3.CellStyle = cellStyleCenter

            ' 證號
            Dim c4 = row.CreateCell(4)
            If f.placeType IsNot Nothing AndAlso f.placeType.ToString() = "屠宰場" Then
                c4.SetCellValue("-")
            Else
                c4.SetCellValue(MaskFarmCode(f.placeCode))
            End If
            c4.CellStyle = cellStyleText

            ' 名稱
            Dim c5 = row.CreateCell(5)
            c5.SetCellValue(f.placeName)
            c5.CellStyle = cellStyleCenter

            ' 負責人
            Dim c6 = row.CreateCell(6)
            c6.SetCellValue(f.placeOwner)
            c6.CellStyle = cellStyleCenter

            ' 資料來源
            Dim c7 = row.CreateCell(7)
            c7.SetCellValue(f.insertType.ToString())
            c7.CellStyle = cellStyleCenter

            ' 資料建立時間
            Dim c8 = row.CreateCell(8)
            c8.SetCellValue(f.insertDateTime.ToString("yyyy/MM/dd HH:mm"))
            c8.CellStyle = cellStyleCenter

            '' 備註
            'Dim c9 = row.CreateCell(9)
            'c9.SetCellValue(f.memo)
            'c9.CellStyle = cellStyleLeft
        Next

        ' === 欄寬設定 ===
        ws.SetColumnWidth(0, 14 * 256)   ' 日期
        ws.SetColumnWidth(1, 10 * 256)   ' 類型
        ws.SetColumnWidth(2, 10 * 256)   ' 縣市
        ws.SetColumnWidth(3, 10 * 256)   ' 鄉鎮
        ws.SetColumnWidth(4, 20 * 256)   ' 證號
        ws.SetColumnWidth(5, 25 * 256)   ' 名稱
        ws.SetColumnWidth(6, 15 * 256)   ' 負責人
        ws.SetColumnWidth(7, 18 * 256)   ' 資料來源
        ws.SetColumnWidth(8, 20 * 256)   ' 資料建立時間
        'ws.SetColumnWidth(9, 40 * 256)   ' 備註

        ' === 檔案名稱 ===
        Dim fileName As String = $"牛籍歷程_{Now:yyyyMMddHHmmss}.xlsx"

        ' === 輸出下載 ===
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