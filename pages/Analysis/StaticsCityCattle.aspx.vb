Imports System.IO
Imports System.Reflection.Emit
Imports NPOI.SS.UserModel
Imports NPOI.SS.Util
Imports NPOI.XSSF.UserModel

Public Class StaticsCityCattle
    Inherits taifCattle.Base
    Dim taifCattle_con As New taifCattle.Control
    Dim taifCattle_farm As New taifCattle.Farm
    Dim taifCattle_report As New taifCattle.Report
#Region "Property"
    ''' <summary>
    ''' 牛籍主要資料
    ''' </summary>
    Public Class CattleMain
        ' === 主體欄位 ===
        Public Property latestCity As String           ' 畜牧場縣市
        Public Property latestPlaceName As String      ' 畜牧場名稱
        Public Property latestPlaceCode As String      ' 畜牧場證號
        Public Property removeTypeName As String       ' 除籍原因
        Public Property tagNo As String                ' 牛籍編號
        Public Property cattleTypeID As Integer        ' 品項流水號
        Public Property groupName As String            ' 牛種
        Public Property typeName As String             ' 品項
        Public Property milkProduction As Decimal     ' 平均乳產量
        Public Property isInsurance As Boolean         ' 投保狀態
        Public Property isClaim As Boolean             ' 理賠狀態
        Public Property birthYear As Integer           ' 出生年
        Public Property cattleAge As Integer           ' 牛籍歲齡

        ' === 歷程清單 ===
        Public Property historyList As List(Of CattleHistory)

        Public Sub New()
            historyList = New List(Of CattleHistory)
        End Sub
    End Class
    ''' <summary>
    ''' 牛籍歷程資料
    ''' </summary>
    Public Class CattleHistory
        Public Property dataDate As Date               ' 日期
        Public Property hisTypeName As String          ' 類型
        Public Property city As String                 ' 畜牧場縣市
        Public Property placeName As String            ' 畜牧場名稱
        Public Property placeCode As String            ' 畜牧場證號
        Public Property placeOwner As String           ' 畜牧場負責人
        Public Property memo As String                 ' 旅程備註
    End Class
    ''' <summary>
    ''' 搜尋條件：開始日期
    ''' </summary>
    ''' <returns></returns>
    Property Property_Query_dateBeg As Date
        Get
            If IsNothing(ViewState("Property_Query_dateBeg")) Then
                Return Today.AddDays(-6)
            Else
                Return ViewState("Property_Query_dateBeg")
            End If
        End Get
        Set(value As Date)
            ViewState("Property_Query_dateBeg") = value
        End Set
    End Property

    ''' <summary>
    ''' 搜尋條件：結束日期
    ''' </summary>
    ''' <returns></returns>
    Property Property_Query_dateEnd As Date
        Get
            If IsNothing(ViewState("Property_Query_dateEnd")) Then
                Return Today
            Else
                Return ViewState("Property_Query_dateEnd")
            End If
        End Get
        Set(value As Date)
            ViewState("Property_Query_dateEnd") = value
        End Set
    End Property

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
    ''' 儲存搜尋條件
    ''' </summary>
    Private Sub SaveQueryCondition()

        ' --- 操作時間(起) ---
        Dim strBeg As String = Request.Form("dateBeg")
        Dim begDate As DateTime = Property_Query_dateBeg   ' 預設用現有屬性值

        If Not String.IsNullOrEmpty(strBeg) AndAlso DateTime.TryParse(strBeg, begDate) Then
            Property_Query_dateBeg = begDate.ToString("yyyy-MM-dd")
        End If

        ' --- 操作時間(訖) ---
        Dim strEnd As String = Request.Form("dateEnd")
        Dim endDate As DateTime = Property_Query_dateEnd   ' 預設用現有屬性值

        If Not String.IsNullOrEmpty(strEnd) AndAlso DateTime.TryParse(strEnd, endDate) Then
            Property_Query_dateEnd = endDate.ToString("yyyy-MM-dd")
        End If

        ' --- 若起訖相反則自動交換 ---
        If begDate > endDate Then
            Dim tmp = begDate
            begDate = endDate
            endDate = tmp
            Property_Query_dateBeg = begDate.ToString("yyyy-MM-dd")
            Property_Query_dateEnd = endDate.ToString("yyyy-MM-dd")
        End If

        ' --- 縣市 ---
        Property_Query_city = DropDownList_farmCity.SelectedValue

    End Sub

    ''' <summary>
    ''' 撈取指定期間與縣市之牛隻歷程資料
    ''' </summary>
    ''' <param name="dateBeg">起始日期</param>
    ''' <param name="dateEnd">結束日期</param>
    ''' <param name="cityID">縣市 ID</param>
    ''' <returns>牛隻歷程 DataTable</returns>
    Function Get_CattleHistoryList(dateBeg As Date, dateEnd As Date, cityID As Object) As List(Of CattleMain)


        Dim sqlString As String =
        <sql>

        ;WITH CattleInRange AS (
            SELECT DISTINCT cattleHistory.cattleID
            FROM Cattle_History AS cattleHistory
            WHERE cattleHistory.removeDateTime IS NULL
              AND cattleHistory.dataDate BETWEEN @dateBeg AND @dateEnd
        )
        SELECT 
            CattleInRange.cattleID,

            /*最新非除籍旅程（畜牧場資訊）*/
            latestFarm.farmCode                        AS latestPlaceCode,    
            latestFarm.farmName                        AS latestPlaceName,    
            taiwanFarm.city                            AS latestCity,         

            /* 最新除籍旅程（屠宰場或化製廠）*/
            removalType.typeName                       AS removeTypeName,   

            /* 牛籍現況 */
            cattleView.tagNo,
            cattleView.groupName,
            cattleView.typeName,
            cattleView.cattleTypeID,
            cattleView.milkProduction,
            ISNULL(insClaimView.isInsurance, 0)        AS isInsurance,
            ISNULL(insClaimView.isClaim, 0)            AS isClaim,
            cattleView.birthYear,
	        cattleView.cattleAge,

            /* 歷程資訊（迄止日期之前的全部記錄）*/
            cattleHistory.dataDate,
            historyType.typeName                       AS hisTypeName,
            taiwanPlace.city                           AS historyCity,
            COALESCE(farm.farmName, slaughterhouse.slauName, renderingPlant.plantName) AS placeName,
            COALESCE(farm.farmCode, slaughterhouse.baphiqCode, renderingPlant.plantCode) AS placeCode,
            COALESCE(farm.owner, slaughterhouse.ownerName, renderingPlant.owner) AS placeOwner,
            cattleHistory.memo

        FROM View_CattleList AS cattleView
        INNER JOIN CattleInRange ON cattleView.cattleID = CattleInRange.cattleID
        LEFT JOIN View_CattleInsClaStatus AS insClaimView ON cattleView.tagNo = insClaimView.tagNo

        /* 取得最新「非除籍」旅程（限定期間內）*/
        OUTER APPLY (
            SELECT TOP 1 
                cattleHistory.hisTypeID,
                cattleHistory.dataDate,
                cattleHistory.farmID
            FROM Cattle_History AS cattleHistory
            INNER JOIN Cattle_TypeHistory AS typeNonRemoval 
                ON cattleHistory.hisTypeID = typeNonRemoval.hisTypeID
            WHERE cattleHistory.cattleID = CattleInRange.cattleID
              AND cattleHistory.removeDateTime IS NULL
              AND typeNonRemoval.groupName &lt;&gt; N'除籍'
              AND cattleHistory.dataDate BETWEEN @dateBeg AND @dateEnd
            ORDER BY cattleHistory.dataDate DESC, typeNonRemoval.orderBy DESC
        ) AS latestNonRemoval
        LEFT JOIN List_Farm AS latestFarm ON latestNonRemoval.farmID = latestFarm.farmID
        LEFT JOIN System_Taiwan AS taiwanFarm ON latestFarm.twID = taiwanFarm.twID

        /* 取得最新「除籍」旅程（限定期間內）*/
        OUTER APPLY (
            SELECT TOP 1 
                cattleHistory.hisTypeID,
                cattleHistory.dataDate
            FROM Cattle_History AS cattleHistory
            INNER JOIN Cattle_TypeHistory AS typeRemovalRef 
                ON cattleHistory.hisTypeID = typeRemovalRef.hisTypeID
            WHERE cattleHistory.cattleID = CattleInRange.cattleID
              AND cattleHistory.removeDateTime IS NULL
              AND typeRemovalRef.groupName = N'除籍'
              AND cattleHistory.dataDate BETWEEN @dateBeg AND @dateEnd
            ORDER BY cattleHistory.dataDate DESC, typeRemovalRef.orderBy DESC
        ) AS latestRemoval
        LEFT JOIN Cattle_TypeHistory AS removalType ON latestRemoval.hisTypeID = removalType.hisTypeID

        /* 歷程 JOIN：拿到截至結束日期的所有歷程 */
        INNER JOIN Cattle_History AS cattleHistory ON cattleView.cattleID = cattleHistory.cattleID
        LEFT JOIN Cattle_TypeHistory AS historyType ON cattleHistory.hisTypeID = historyType.hisTypeID
        LEFT JOIN List_Farm AS farm ON cattleHistory.farmID = farm.farmID
        LEFT JOIN List_Slaughterhouse AS slaughterhouse ON cattleHistory.slauID = slaughterhouse.slauID
        LEFT JOIN List_RenderingPlant AS renderingPlant ON cattleHistory.plantID = renderingPlant.plantID
        LEFT JOIN System_Taiwan AS taiwanPlace ON ISNULL(ISNULL(farm.twID, slaughterhouse.twID), renderingPlant.twID) = taiwanPlace.twID

        WHERE cattleHistory.removeDateTime IS NULL 
	        AND cattleHistory.dataDate &lt;= @dateEnd
            AND (@cityID IS NULL OR taiwanFarm.cityID = @cityID)
        ORDER BY 
            latestFarm.twID,
            latestFarm.farmID,
            cattleView.tagNo,
            CattleHistory.dataDate,
            historyType.orderBy


        </sql>.Value

        ' === 組成參數 ===
        Dim para As New List(Of SqlClient.SqlParameter)
        para.Add(New SqlClient.SqlParameter("dateBeg", dateBeg))
        para.Add(New SqlClient.SqlParameter("dateEnd", dateEnd))
        para.Add(New SqlClient.SqlParameter("cityID", If(cityID Is Nothing OrElse cityID.ToString().Trim() = "%" OrElse cityID.ToString().Trim() = "", DBNull.Value, cityID)))


        ' === 執行查詢 ===
        Dim dt As New Data.DataTable
        Using da As New DataAccess.MS_SQL()
            dt = da.GetDataTable(sqlString, para.ToArray())
        End Using

        ' === 轉換結果===
        Dim result As New List(Of CattleMain)

        If dt.Rows.Count > 0 Then
            Dim groups = dt.AsEnumerable().GroupBy(Function(r) New With {
            Key .placeCode = r.Field(Of String)("latestPlaceCode"),
            Key .tagNo = r.Field(Of String)("tagNo")
            })

            For Each grp In groups
                Dim first = grp.First()

                ' 建立主要資料
                Dim main As New CattleMain With {
                    .latestCity = first.Field(Of String)("latestCity"),
                    .latestPlaceName = first.Field(Of String)("latestPlaceName"),
                    .latestPlaceCode = first.Field(Of String)("latestPlaceCode"),
                    .removeTypeName = If(first.IsNull("removeTypeName"), "", first.Field(Of String)("removeTypeName")),
                    .tagNo = first.Field(Of String)("tagNo"),
                    .groupName = first.Field(Of String)("groupName"),
                    .typeName = first.Field(Of String)("typeName"),
                    .cattleTypeID = first.Field(Of Integer)("cattleTypeID"),
                    .milkProduction = If(first.IsNull("milkProduction"), Nothing, first.Field(Of Decimal)("milkProduction")),
                    .isInsurance = Convert.ToBoolean(first.Field(Of Integer)("isInsurance")),
                    .isClaim = Convert.ToBoolean(first.Field(Of Integer)("isClaim")),
                    .birthYear = first.Field(Of Integer)("birthYear"),
                    .cattleAge = first.Field(Of Integer)("cattleAge"),
                    .historyList = New List(Of CattleHistory)
                }

                ' 加入歷程
                For Each row In grp
                    Dim his As New CattleHistory With {
                    .dataDate = row.Field(Of Date)("dataDate"),
                    .hisTypeName = row.Field(Of String)("hisTypeName"),
                    .city = row.Field(Of String)("historyCity"),
                    .placeName = row.Field(Of String)("placeName"),
                    .placeCode = row.Field(Of String)("placeCode"),
                    .placeOwner = row.Field(Of String)("placeOwner"),
                    .memo = If(row.IsNull("memo"), "", row.Field(Of String)("memo"))
                }
                    main.historyList.Add(his)
                Next

                result.Add(main)
            Next
        End If

        Return result
    End Function

    Sub AddCell(row As HtmlTableRow, text As String, style As String, Optional rowSpan As Integer = 1, Optional colSpan As Integer = 1, Optional tag As String = "td")
        Dim cell As New HtmlTableCell(tag)
        If rowSpan > 1 Then cell.RowSpan = rowSpan
        If colSpan > 1 Then cell.ColSpan = colSpan
        cell.InnerText = text
        cell.Attributes("style") = style
        row.Cells.Add(cell)
    End Sub

    Private Function Get_HtmlTable() As HtmlTable
        Dim cattleList As List(Of CattleMain) = Get_CattleHistoryList(Property_Query_dateBeg, Property_Query_dateEnd, Property_Query_city)
        Dim displayDash As String = "-"

        ' === 建立 HtmlTable ===
        Dim tbl As New HtmlTable()
        tbl.Attributes("class") = "gv"
        tbl.Border = 1

        ' === 樣式 ===
        Dim tdStyleCenter As String = "text-align:center;border:1px solid #999;padding:4px;mso-number-format:'\@';"
        Dim tdStyleLeft As String = "text-align:left;border:1px solid #999;padding:4px;mso-number-format:'\@';"

        ' === 第一層大標題 ===
        Dim trTop As New HtmlTableRow()
        AddCell(trTop, "指定期間之最新狀態資訊", tdStyleCenter, 1, 4, "th")
        AddCell(trTop, "牛籍現況", tdStyleCenter, 1, 8, "th")
        AddCell(trTop, "轉移歷程", tdStyleCenter, 1, 7, "th")
        tbl.Rows.Add(trTop)

        ' === 第二層標題列 ===
        Dim headers() As String = {
        "畜牧場縣市", "畜牧場名稱", "畜牧場證號", "除籍原因", "牛籍編號",
        "牛種", "品項", "平均產乳量", "投保狀態", "理賠狀態", "出生年", "牛籍歲齡",
        "日期", "類型", "畜牧場縣市", "畜牧場名稱", "畜牧場證號", "畜牧場負責人", "旅程備註"
    }
        Dim trHeader As New HtmlTableRow()
        For Each h In headers
            AddCell(trHeader, h, tdStyleCenter, 1, 1, "th")
        Next
        tbl.Rows.Add(trHeader)

        ' === 資料列 ===
        For Each main In cattleList
            Dim historyCount As Integer = If(main.historyList IsNot Nothing AndAlso main.historyList.Count > 0, main.historyList.Count, 1)

            ' 預先轉換前段資料
            Dim birthYearVal As String = If(main.birthYear = -1, displayDash, main.birthYear.ToString())
            Dim cattleAgeVal As String = If(main.cattleAge = -1, displayDash, main.cattleAge.ToString())
            Dim milkProductionVal As String = displayDash
            If main.cattleTypeID = 2 Then
                Dim mp As Decimal
                If Decimal.TryParse(main.milkProduction.ToString(), mp) AndAlso mp <> -1D Then
                    milkProductionVal = mp.ToString()
                End If
            End If

            ' === 第一筆列（含RowSpan）===
            Dim tr0 As New HtmlTableRow()
            AddCell(tr0, Convert_EmptyToObject(main.latestCity, displayDash), tdStyleCenter, historyCount)
            AddCell(tr0, Convert_EmptyToObject(main.latestPlaceName, displayDash), tdStyleLeft, historyCount)
            AddCell(tr0, Convert_EmptyToObject(main.latestPlaceCode, displayDash), tdStyleCenter, historyCount)
            AddCell(tr0, Convert_EmptyToObject(main.removeTypeName, displayDash), tdStyleCenter, historyCount)
            AddCell(tr0, Convert_EmptyToObject(main.tagNo, displayDash), tdStyleCenter, historyCount)
            AddCell(tr0, Convert_EmptyToObject(main.groupName, displayDash), tdStyleCenter, historyCount)
            AddCell(tr0, Convert_EmptyToObject(main.typeName, displayDash), tdStyleCenter, historyCount)
            AddCell(tr0, milkProductionVal, tdStyleCenter, historyCount)
            AddCell(tr0, If(main.isInsurance, "已投保", "未投保"), tdStyleCenter, historyCount)
            AddCell(tr0, If(main.isClaim, "已理賠", "未理賠"), tdStyleCenter, historyCount)
            AddCell(tr0, birthYearVal, tdStyleCenter, historyCount)
            AddCell(tr0, cattleAgeVal, tdStyleCenter, historyCount)

            ' 第一筆歷程
            If main.historyList IsNot Nothing AndAlso main.historyList.Count > 0 Then
                Dim h0 = main.historyList(0)
                AddCell(tr0, If(h0.dataDate = Date.MinValue, displayDash, h0.dataDate.ToString("yyyy-MM-dd")), tdStyleCenter)
                AddCell(tr0, Convert_EmptyToObject(h0.hisTypeName, displayDash), tdStyleCenter)
                AddCell(tr0, Convert_EmptyToObject(h0.city, displayDash), tdStyleCenter)
                AddCell(tr0, Convert_EmptyToObject(h0.placeName, displayDash), tdStyleLeft)
                AddCell(tr0, Convert_EmptyToObject(taifCattle_farm.MaskFarmCode(h0.placeCode), displayDash), tdStyleCenter)
                AddCell(tr0, Convert_EmptyToObject(h0.placeOwner, displayDash), tdStyleCenter)
                AddCell(tr0, Convert_EmptyToObject(h0.memo, displayDash), tdStyleLeft)
            Else
                ' 沒有歷程就補空列
                For i As Integer = 1 To 7
                    AddCell(tr0, displayDash, tdStyleCenter)
                Next
            End If
            tbl.Rows.Add(tr0)

            ' === 其餘歷程 ===
            If main.historyList IsNot Nothing AndAlso main.historyList.Count > 1 Then
                For i = 1 To main.historyList.Count - 1
                    Dim hi = main.historyList(i)
                    Dim tr As New HtmlTableRow()
                    AddCell(tr, If(hi.dataDate = Date.MinValue, displayDash, hi.dataDate.ToString("yyyy-MM-dd")), tdStyleCenter)
                    AddCell(tr, Convert_EmptyToObject(hi.hisTypeName, displayDash), tdStyleCenter)
                    AddCell(tr, Convert_EmptyToObject(hi.city, displayDash), tdStyleCenter)
                    AddCell(tr, Convert_EmptyToObject(hi.placeName, displayDash), tdStyleLeft)
                    AddCell(tr, Convert_EmptyToObject(taifCattle_farm.MaskFarmCode(hi.placeCode), displayDash), tdStyleCenter)
                    AddCell(tr, Convert_EmptyToObject(hi.placeOwner, displayDash), tdStyleCenter)
                    AddCell(tr, Convert_EmptyToObject(hi.memo, displayDash), tdStyleLeft)
                    tbl.Rows.Add(tr)
                Next
            End If
        Next

        Return tbl
    End Function
#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then

            '縣市
            taifCattle_con.BindDropDownList_city(DropDownList_farmCity, False)

        End If
    End Sub

    'Private Sub LinkButton_excel_Click(sender As Object, e As EventArgs) Handles LinkButton_excel.Click
    '    '儲存搜尋條件
    '    SaveQueryCondition()

    '    ' === 1. 取得資料，轉成 HtmlTable ===
    '    Dim tb_report As HtmlTable = Get_HtmlTable()

    '    ' === 2. 用 StringWriter Render HTML===
    '    Dim sw As New StringWriter()
    '    Dim hw As New HtmlTextWriter(sw)
    '    Panel_data.Controls.Add(tb_report)
    '    Panel_data.RenderControl(hw)
    '    Dim htmlContent As String = sw.ToString()

    '    ' === 3. 匯出成假 Excel ===
    '    Dim fileName As String = $"縣市別牛隻編號詳報_{Now:yyyyMMddHHmmss}.xls"

    '    ' 清空整個 Response，避免母版被輸出
    '    Response.Clear()
    '    Response.BufferOutput = True
    '    Response.Charset = "utf-8"
    '    Response.ContentEncoding = Encoding.UTF8
    '    Response.ContentType = "application/vnd.ms-excel"
    '    Response.AddHeader("Content-Disposition", "attachment;filename=" & Server.UrlEncode(fileName))

    '    Response.Write("<meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>")
    '    Response.Write(htmlContent)
    '    Response.End()
    'End Sub

    Private Sub LinkButton_excel_Click(sender As Object, e As EventArgs) Handles LinkButton_excel.Click

        '儲存搜尋條件
        SaveQueryCondition()

        ' === 驗證日期區間不能超過 365 天 ===
        If Property_Query_dateEnd.Subtract(Property_Query_dateBeg).TotalDays > 365 Then
            Label_msg.Text = "查詢區間不可超過 365 天，請重新選擇！"
            Exit Sub
        End If

        ' === 取得資料 ===
        Dim cattleList As List(Of CattleMain) = Get_CattleHistoryList(Property_Query_dateBeg, Property_Query_dateEnd, Property_Query_city)

        ' === 建立 Workbook & Sheet ===
        Dim wb As New XSSFWorkbook()
        Dim ws As ISheet = wb.CreateSheet("縣市別牛隻編號詳報")

        ' === 建立 DataFormat 物件（用於設定文字格式）===
        Dim dataFormat = wb.CreateDataFormat()

        ' === 標題樣式 ===
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

        ' === 一般儲存格樣式（置中 + 框線）===
        Dim cellStyleCenter As ICellStyle = wb.CreateCellStyle()
        cellStyleCenter.Alignment = HorizontalAlignment.Center
        cellStyleCenter.VerticalAlignment = VerticalAlignment.Center
        cellStyleCenter.WrapText = True
        cellStyleCenter.BorderTop = BorderStyle.Thin
        cellStyleCenter.BorderBottom = BorderStyle.Thin
        cellStyleCenter.BorderLeft = BorderStyle.Thin
        cellStyleCenter.BorderRight = BorderStyle.Thin

        ' === 左對齊樣式（含框線）===
        Dim cellStyleLeft As ICellStyle = wb.CreateCellStyle()
        cellStyleLeft.Alignment = HorizontalAlignment.Left
        cellStyleLeft.VerticalAlignment = VerticalAlignment.Center
        cellStyleLeft.WrapText = True
        cellStyleLeft.BorderTop = BorderStyle.Thin
        cellStyleLeft.BorderBottom = BorderStyle.Thin
        cellStyleLeft.BorderLeft = BorderStyle.Thin
        cellStyleLeft.BorderRight = BorderStyle.Thin

        ' === 紀錄合併儲存格 ===
        Dim mergeList As New List(Of CellRangeAddress)()

        ' === 第一層大標題 ===
        Dim rowTop = ws.CreateRow(0)
        rowTop.HeightInPoints = 25

        ' === 指定期間之最新狀態資訊 ===
        taifCattle_report.ApplyCellStyle(ws, 0, 0, 0, 3, headerStyle)
        mergeList.Add(New CellRangeAddress(0, 0, 0, 3))
        rowTop.GetCell(0).SetCellValue("指定期間之最新狀態資訊")

        ' === 牛籍現況 ===
        taifCattle_report.ApplyCellStyle(ws, 0, 0, 4, 11, headerStyle)
        mergeList.Add(New CellRangeAddress(0, 0, 4, 11))
        rowTop.GetCell(4).SetCellValue("牛籍現況")

        ' === 轉移歷程 ===
        taifCattle_report.ApplyCellStyle(ws, 0, 0, 12, 18, headerStyle)
        mergeList.Add(New CellRangeAddress(0, 0, 12, 18))
        rowTop.GetCell(12).SetCellValue("轉移歷程")

        ' === 第二層標題 ===
        Dim headers() As String = {
        "畜牧場" & vbLf & "縣市", "畜牧場" & vbLf & "名稱", "畜牧場" & vbLf & "證號", "除籍" & vbLf & "原因", "牛籍編號",
        "牛種", "品項", "平均" & vbLf & "產乳量", "投保狀態", "理賠狀態", "出生年", "牛籍" & vbLf & "歲齡",
        "日期", "類型", "畜牧場" & vbLf & "縣市", "畜牧場" & vbLf & "名稱", "畜牧場" & vbLf & "證號", "畜牧場" & vbLf & "負責人", "旅程備註"
        }

        Dim rowHeader = ws.CreateRow(1)
        rowHeader.HeightInPoints = 30
        For i As Integer = 0 To headers.Length - 1
            Dim cell = rowHeader.CreateCell(i)
            cell.SetCellValue(headers(i))
            cell.CellStyle = headerStyle
        Next

        ' === 寫入資料 ===
        Dim displayDash As String = "-"
        Dim rowIndex As Integer = 2

        For Each main In cattleList
            Dim startRow = rowIndex
            Dim historyCount = main.historyList.Count
            If historyCount = 0 Then historyCount = 1

            For Each his In main.historyList
                Dim row = ws.CreateRow(rowIndex)

                ' === 最新狀態欄位===
                If rowIndex = startRow Then
                    ' === 處理牛籍數值欄位 ===
                    Dim birthYearVal As String = If(main.birthYear = -1, displayDash, main.birthYear.ToString())
                    Dim cattleAgeVal As String = If(main.cattleAge = -1, displayDash, main.cattleAge.ToString())

                    ' 平均乳產量：乳母牛才有
                    Dim milkProductionVal As String = displayDash
                    If main.cattleTypeID = 2 Then
                        Dim mp As Decimal
                        If Decimal.TryParse(main.milkProduction.ToString(), mp) AndAlso mp <> -1D Then
                            milkProductionVal = mp.ToString()
                        End If
                    End If
                    ' === 寫入最新狀態欄位 ===
                    row.CreateCell(0).SetCellValue(Convert_EmptyToObject(main.latestCity, displayDash).ToString())
                    row.CreateCell(1).SetCellValue(Convert_EmptyToObject(main.latestPlaceName, displayDash).ToString())
                    row.CreateCell(2).SetCellValue(Convert_EmptyToObject(main.latestPlaceCode, displayDash).ToString())
                    row.CreateCell(3).SetCellValue(Convert_EmptyToObject(main.removeTypeName, displayDash).ToString())
                    row.CreateCell(4).SetCellValue(Convert_EmptyToObject(main.tagNo, displayDash).ToString())
                    row.CreateCell(5).SetCellValue(Convert_EmptyToObject(main.groupName, displayDash).ToString())
                    row.CreateCell(6).SetCellValue(Convert_EmptyToObject(main.typeName, displayDash).ToString())
                    row.CreateCell(7).SetCellValue(milkProductionVal)
                    row.CreateCell(8).SetCellValue(If(main.isInsurance, "已投保", "未投保"))
                    row.CreateCell(9).SetCellValue(If(main.isClaim, "已理賠", "未理賠"))
                    row.CreateCell(10).SetCellValue(birthYearVal)
                    row.CreateCell(11).SetCellValue(cattleAgeVal)
                End If

                ' === 歷程欄位 ===
                row.CreateCell(12).SetCellValue(If(his.dataDate = Date.MinValue, displayDash, his.dataDate.ToString("yyyy-MM-dd")))
                row.CreateCell(13).SetCellValue(Convert_EmptyToObject(his.hisTypeName, displayDash).ToString())
                row.CreateCell(14).SetCellValue(Convert_EmptyToObject(his.city, displayDash).ToString())
                row.CreateCell(15).SetCellValue(Convert_EmptyToObject(his.placeName, displayDash).ToString())
                row.CreateCell(16).SetCellValue(Convert_EmptyToObject(taifCattle_farm.MaskFarmCode(his.placeCode), displayDash).ToString())
                row.CreateCell(17).SetCellValue(Convert_EmptyToObject(his.placeOwner, displayDash).ToString())
                row.CreateCell(18).SetCellValue(Convert_EmptyToObject(his.memo, displayDash).ToString())

                ' === 套用樣式 ===
                For c As Integer = 0 To 18
                    Dim style = If(c = 18, cellStyleLeft, cellStyleCenter)
                    If row.GetCell(c) Is Nothing Then row.CreateCell(c)
                    row.GetCell(c).CellStyle = style
                Next

                rowIndex += 1
            Next

            ' === 合併主要欄位 ===
            If main.historyList.Count > 1 Then
                For c As Integer = 0 To 11
                    mergeList.Add(New CellRangeAddress(startRow, rowIndex - 1, c, c))
                Next
            End If
        Next

        ' == 批次執行合併 ==
        For Each m In mergeList
            ws.AddMergedRegionUnsafe(m)
        Next

        ' === 固定欄寬設定（單位：1/256 字元寬度）===
        ws.SetColumnWidth(0, 8 * 256)   ' 畜牧場縣市
        ws.SetColumnWidth(1, 20 * 256)   ' 畜牧場名稱
        ws.SetColumnWidth(2, 10 * 256)   ' 畜牧場證號
        ws.SetColumnWidth(3, 8 * 256)   ' 除籍原因
        ws.SetColumnWidth(4, 10 * 256)   ' 牛籍編號
        ws.SetColumnWidth(5, 8 * 256)   ' 牛種
        ws.SetColumnWidth(6, 8 * 256)   ' 品項
        ws.SetColumnWidth(7, 10 * 256)   ' 平均產乳量
        ws.SetColumnWidth(8, 10 * 256)   ' 投保狀態
        ws.SetColumnWidth(9, 10 * 256)   ' 理賠狀態
        ws.SetColumnWidth(10, 10 * 256)  ' 出生年
        ws.SetColumnWidth(11, 8 * 256)  ' 牛籍歲齡
        ws.SetColumnWidth(12, 13 * 256)  ' 日期
        ws.SetColumnWidth(13, 10 * 256)  ' 類型
        ws.SetColumnWidth(14, 8 * 256)  ' 畜牧場縣市
        ws.SetColumnWidth(15, 20 * 256)  ' 畜牧場名稱
        ws.SetColumnWidth(16, 10 * 256)  ' 畜牧場證號
        ws.SetColumnWidth(17, 10 * 256)  ' 畜牧場負責人
        ws.SetColumnWidth(18, 40 * 256)  ' 旅程備註

        ' === 輸出 ===
        Dim fileName As String = $"縣市別牛隻編號詳報_{Now:yyyyMMddHHmmss}.xlsx"
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