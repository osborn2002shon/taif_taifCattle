Imports NPOI.XSSF.UserModel
Imports NPOI.SS.UserModel
Imports NPOI.SS.Util
Imports System.IO
Public Class UserLog
    Inherits System.Web.UI.Page
    Dim taifCattle_con As New taifCattle.Control

#Region "Property"

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
    ''' 搜尋條件：操作項目
    ''' </summary>
    Public Property Property_Query_userLogItem As String
        Get
            If IsNothing(ViewState("Property_Query_userLogItem")) Then
                Return "%"
            Else
                Return ViewState("Property_Query_userLogItem").ToString()
            End If
        End Get
        Set(value As String)
            ViewState("Property_Query_userLogItem") = value
        End Set
    End Property

    ''' <summary>
    ''' 搜尋條件：操作類型
    ''' </summary>
    Public Property Property_Query_userLogType As String
        Get
            If IsNothing(ViewState("Property_Query_userLogType")) Then
                Return "%"
            Else
                Return ViewState("Property_Query_userLogType").ToString()
            End If
        End Get
        Set(value As String)
            ViewState("Property_Query_userLogType") = value
        End Set
    End Property

    ''' <summary>
    ''' 搜尋條件：關鍵字
    ''' </summary>
    Public Property Property_Query_keyWord As String
        Get
            If IsNothing(ViewState("Property_Query_keyWord")) Then
                Return ""
            Else
                Return ViewState("Property_Query_keyWord").ToString()
            End If
        End Get
        Set(value As String)
            ViewState("Property_Query_keyWord") = value
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

        '操作項目
        Property_Query_userLogItem = DropDownList_userLogItem.SelectedValue

        '操作類型
        Property_Query_userLogType = DropDownList_userLogType.SelectedValue

        '關鍵字
        Property_Query_keyWord = TextBox_keyWord.Text.Trim()
    End Sub

    ''' <summary>
    ''' 取得使用者操作紀錄
    ''' </summary>
    ''' <param name="dateBeg">起始日期</param>
    ''' <param name="dateEnd">結束日期</param>
    ''' <param name="userLogItem">操作項目</param>
    ''' <param name="userLogType">操作類型</param>
    ''' <param name="keyWord">關鍵字（姓名、帳號、IP）</param>
    ''' <returns></returns>
    Function Get_UserLog(dateBeg As Date, dateEnd As Date, userLogItem As String, userLogType As String, keyWord As String) As Data.DataTable

        Dim sqlString As String =
        <sql>
            select su.logID, su.accountID, su.logDateTime, su.IP,
                   su.logItem, su.logType, su.memo,
                   ua.account, ua.name, ua.unit
            from System_UserLog su
            left join System_UserAccount ua on su.accountID = ua.accountID
            where su.logDateTime between @dateBeg and @dateEnd
                and su.logItem like @logItem
                and su.logType like @logType
                and (
                    ua.name like '%' +  @keyWord + '%'
                    or ua.account like '%' +  @keyWord + '%'
                    or su.IP like '%' +  @keyWord + '%'
                    )
            order by su.logDateTime desc
        </sql>.Value

        Dim dateBegDT As DateTime = dateBeg.Date
        Dim dateEndDT As DateTime = dateEnd.Date.AddDays(1).AddMilliseconds(-1)

        ' === 參數 ===
        Dim para As New List(Of Data.SqlClient.SqlParameter)
        para.Add(New Data.SqlClient.SqlParameter("dateBeg", dateBegDT))
        para.Add(New Data.SqlClient.SqlParameter("dateEnd", dateEndDT))
        para.Add(New Data.SqlClient.SqlParameter("logItem", userLogItem))
        para.Add(New Data.SqlClient.SqlParameter("logType", userLogType))
        para.Add(New Data.SqlClient.SqlParameter("keyWord", keyWord))

        Using da As New DataAccess.MS_SQL
            Return da.GetDataTable(sqlString, para.ToArray)
        End Using
    End Function

    Private Sub BindGridView()
        Dim dtLog As Data.DataTable = Get_UserLog(Property_Query_dateBeg, Property_Query_dateEnd, Property_Query_userLogItem, Property_Query_userLogType, Property_Query_keyWord)
        GridView_userLog.DataSource = dtLog
        GridView_userLog.DataBind()

        Label_recordCount.Text = dtLog.Rows.Count.ToString("N0")
    End Sub
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then

            '操作項目
            taifCattle_con.BindDropDownList_UserLogItem(DropDownList_userLogItem, True)
            '操作類型
            taifCattle_con.BindDropDownList_UserLogType(DropDownList_userLogType, True)

            '儲存紀錄
            SaveQueryCondition()

            '取得log
            BindGridView()

        End If
    End Sub

    Private Sub LinkButton_query_Click(sender As Object, e As EventArgs) Handles LinkButton_query.Click
        SaveQueryCondition()

        Dim dtLog As Data.DataTable = Get_UserLog(Property_Query_dateBeg, Property_Query_dateEnd, Property_Query_userLogItem, Property_Query_userLogType, Property_Query_keyWord)
        GridView_userLog.DataSource = dtLog
        GridView_userLog.DataBind()
    End Sub

    Private Sub LinkButton_excel_Click(sender As Object, e As EventArgs) Handles LinkButton_excel.Click
        '取得資料
        Dim dt As Data.DataTable = Get_UserLog(Property_Query_dateBeg, Property_Query_dateEnd, Property_Query_userLogItem, Property_Query_userLogType, Property_Query_keyWord)

        '建立 Workbook & Sheet
        Dim wb As New XSSFWorkbook()
        Dim ws As ISheet = wb.CreateSheet("使用者操作紀錄")

        '樣式設定
        Dim headerStyle As ICellStyle = wb.CreateCellStyle()
        headerStyle.Alignment = HorizontalAlignment.Center
        headerStyle.VerticalAlignment = VerticalAlignment.Center
        Dim fontHeader = wb.CreateFont()
        fontHeader.IsBold = True
        headerStyle.SetFont(fontHeader)

        '標題
        Dim headers() As String = {"操作時間", "IP位址", "使用者姓名", "帳號名稱", "操作類型", "操作項目", "操作內容"}
        Dim rowHeader = ws.CreateRow(0)
        For i As Integer = 0 To headers.Length - 1
            Dim cell = rowHeader.CreateCell(i)
            cell.SetCellValue(headers(i))
            cell.CellStyle = headerStyle
        Next

        '資料
        For r As Integer = 0 To dt.Rows.Count - 1
            Dim dataRow = ws.CreateRow(r + 1)
            dataRow.CreateCell(0).SetCellValue(Format(dt.Rows(r)("logDateTime"), "yyyy-MM-dd HH:mm:ss"))
            dataRow.CreateCell(1).SetCellValue(dt.Rows(r)("IP").ToString())
            dataRow.CreateCell(2).SetCellValue(dt.Rows(r)("name").ToString())
            dataRow.CreateCell(3).SetCellValue(dt.Rows(r)("account").ToString())
            dataRow.CreateCell(4).SetCellValue(dt.Rows(r)("logType").ToString())
            dataRow.CreateCell(5).SetCellValue(dt.Rows(r)("logItem").ToString())
            dataRow.CreateCell(6).SetCellValue(dt.Rows(r)("memo").ToString())

        Next

        '調整欄寬
        ws.SetColumnWidth(0, 20 * 256) ' 操作時間
        ws.SetColumnWidth(1, 15 * 256) ' IP位址
        ws.SetColumnWidth(2, 12 * 256) ' 使用者姓名
        ws.SetColumnWidth(3, 20 * 256) ' 帳號名稱
        ws.SetColumnWidth(4, 10 * 256) ' 操作類型
        ws.SetColumnWidth(5, 20 * 256) ' 操作項目
        ws.SetColumnWidth(6, 50 * 256) ' 操作內容


        '檔案名稱
        Dim fileName As String = $"使用者操作紀錄_{Property_Query_dateBeg:yyyyMMdd}_{Property_Query_dateEnd:yyyyMMdd}.xlsx"

        Using ms As New MemoryStream()
            wb.Write(ms)
            Response.Clear()
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Response.AddHeader("Content-Disposition", $"attachment; filename={HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8)}")
            Response.BinaryWrite(ms.ToArray())
            Response.End()
        End Using
    End Sub

    Private Sub GridView_userLog_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_userLog.PageIndexChanging
        GridView_userLog.PageIndex = e.NewPageIndex
        BindGridView()
    End Sub
End Class