
Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel
Imports System.IO

Public Class StaticsFarm
    Inherits taifCattle.Base
    Dim taifCattle_cattle As New taifCattle.Cattle
    Dim taifCattle_farm As New taifCattle.Farm
    Dim taifCattle_con As New taifCattle.Control
    Dim js As New StringBuilder
#Region "Property"

    ''' <summary>
    ''' 搜尋條件：縣市
    ''' </summary>
    Public Property Property_FarmQuery_city As String
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

    ''' <summary>
    ''' 搜尋條件：操作類型
    ''' </summary>
    Public Property Property_FarmQuery_town As String
        Get
            If IsNothing(ViewState("Property_FarmQuery_town")) Then
                Return "%"
            Else
                Return ViewState("Property_FarmQuery_town").ToString()
            End If
        End Get
        Set(value As String)
            ViewState("Property_FarmQuery_town") = value
        End Set
    End Property

    ''' <summary>
    ''' 搜尋條件：關鍵字
    ''' </summary>
    Public Property Property_FarmQuery_keyWord As String
        Get
            If IsNothing(ViewState("Property_FarmQuery_keyWord")) Then
                Return ""
            Else
                Return ViewState("Property_FarmQuery_keyWord").ToString()
            End If
        End Get
        Set(value As String)
            ViewState("Property_FarmQuery_keyWord") = value
        End Set
    End Property

    ''' <summary>
    ''' 選中的牧場流水號
    ''' </summary>
    Public Property Property_farmID As Integer
        Get
            If IsNothing(ViewState("Property_farmID")) Then
                Return -1
            Else
                Return CInt(ViewState("Property_farmID"))
            End If
        End Get
        Set(value As Integer)
            ViewState("Property_farmID") = value
        End Set
    End Property

    ''' <summary>
    ''' 使用者勾選的顯示狀態清單（CheckBoxList）
    ''' </summary>
    Public Property Property_selectedHisType As List(Of Integer)
        Get
            If ViewState("Property_selectedHisType") Is Nothing Then
                Return New List(Of Integer)
            Else
                Return CType(ViewState("Property_selectedHisType"), List(Of Integer))
            End If
        End Get
        Set(value As List(Of Integer))
            ViewState("Property_selectedHisType") = value
        End Set
    End Property

    ''' <summary>
    ''' 是否僅顯示未除籍
    ''' </summary>
    Public Property Property_showCurrentOnly As Boolean
        Get
            If ViewState("Property_showCurrentOnly") Is Nothing Then
                Return False
            Else
                Return CBool(ViewState("Property_showCurrentOnly"))
            End If
        End Get
        Set(value As Boolean)
            ViewState("Property_showCurrentOnly") = value
        End Set
    End Property
#End Region
#Region "Fun/Sub"
    Public Function MaskFarmCode(value As Object) As String
        Return taifCattle_farm.MaskFarmCode(value)
    End Function
    ''' <summary>
    ''' 儲存搜尋條件
    ''' </summary>
    Private Sub SaveFarmQueryCondition()

        '操作項目
        Property_FarmQuery_city = DropDownList_farmCity.SelectedValue

        '操作類型
        Property_FarmQuery_town = DropDownList_farmTown.SelectedValue

        '關鍵字
        Property_FarmQuery_keyWord = TextBox_farmKeyWord.Text.Trim()
    End Sub
    Sub BindFarmGridView()
        Dim liFarm As List(Of taifCattle.Farm.stru_farmInfo) = taifCattle_farm.Get_FarmList(Property_FarmQuery_city, Property_FarmQuery_town, Property_FarmQuery_keyWord)
        GridView_farmList.DataSource = liFarm
        GridView_farmList.DataBind()
    End Sub


    ''' <summary>
    ''' 驗證查詢條件
    ''' </summary>
    ''' <returns>是否通過驗證</returns>
    Private Function ValidateCattleQueryCondition() As Boolean
        ' 檢查是否有勾選任何顯示狀態
        Dim hasSelected As Boolean = CheckBoxList_status.Items.Cast(Of ListItem).Any(Function(li) li.Selected)

        If String.IsNullOrEmpty(HiddenField_selectedFarm.Value) Then
            Label_cattleMsg.Text = "請先選擇一個牧場。"
            Return False
        Else
            Dim farm As taifCattle.Farm.stru_farmInfo = taifCattle_farm.Get_FarmByID(HiddenField_selectedFarm.Value)
            If farm.farmID <= 0 Then
                Label_cattleMsg.Text = "查無該牧場資料。"
                Return False
            End If
        End If

        If Not hasSelected Then
            Label_cattleMsg.Text = "請至少勾選一項最新狀態再進行查詢。"
            Return False
        End If

        Label_cattleMsg.Text = "" ' 清除前次錯誤訊息
        Return True
    End Function
    Protected Sub SaveCattleQueryCondition()
        '儲存選中的牧場
        Dim selectedFarmID As Integer = -1
        If Not String.IsNullOrEmpty(HiddenField_selectedFarm.Value) Then
            Integer.TryParse(HiddenField_selectedFarm.Value, selectedFarmID)
        End If
        Property_farmID = selectedFarmID

        '儲存最新狀態
        Dim selectedTypes As New List(Of Integer)
        For Each li As ListItem In CheckBoxList_status.Items
            If li.Selected Then
                selectedTypes.Add(CInt(li.Value))
            End If
        Next
        Property_selectedHisType = selectedTypes

        '儲存是否僅顯示未除籍
        Property_showCurrentOnly = CheckBox_currentOnly.Checked
    End Sub

    ''' <summary>
    ''' 撈取指定牧場之牛籍資料（無勾選狀態則使用 IN (0)）
    ''' </summary>
    Function Get_CattleList(farmID As Integer?, selectedHisTypes As List(Of Integer), showCurrentOnly As Boolean) As Data.DataTable


        Dim sql As String =
        <sql>
            SELECT 
                lates.cattleID,
                lates.tagNo,
                lates.latestTypeName,
                lates.latestDataDate,
                lates.removeDate,
                vc.cattleTypeID,
                vc.groupName,
                vc.typeName,
                vc.tagMemo,
                vc.birthYear,
                vc.cattleAge,
                vc.milkProduction,
                vc.cattleMemo
            FROM fn_FarmCattleLatestStatus(@farmID) AS lates
            INNER JOIN View_CattleList AS vc ON lates.cattleID = vc.cattleID
            WHERE 1 = 1
                -- 狀態條件
                AND (lates.latestHisTypeID IN ({0}))
                -- 未除籍條件
                AND (@showCurrentOnly = 0 OR lates.removeDate IS NULL)
            ORDER BY lates.latestDataDate DESC
        </sql>.Value

        ' === 狀態條件 ===
        Dim hisTypeCondition As String
        If selectedHisTypes IsNot Nothing AndAlso selectedHisTypes.Count > 0 Then
            hisTypeCondition = String.Join(",", selectedHisTypes)
        Else
            hisTypeCondition = "0"
        End If

        Dim sqlString As String = String.Format(sql, hisTypeCondition)

        ' === 參數 ===
        Dim para As New List(Of SqlClient.SqlParameter) From {
            New SqlClient.SqlParameter("farmID", If(farmID.HasValue, CType(farmID, Object), DBNull.Value)),
            New SqlClient.SqlParameter("showCurrentOnly", If(showCurrentOnly, 1, 0))
        }

        ' === 執行查詢 ===
        Using da As New DataAccess.MS_SQL
            Return da.GetDataTable(sqlString, para.ToArray())
        End Using

    End Function

    ''' <summary>
    ''' 載入畜牧場基本資訊到畫面
    ''' </summary>
    ''' <param name="farmID">牧場流水號</param>
    Private Sub Load_FarmInfo(farmID As Integer)
        Dim farm As taifCattle.Farm.stru_farmInfo = taifCattle_farm.Get_FarmByID(farmID)

        If farm.farmID <= 0 Then

            ' 若查無資料，清空欄位
            TextBox_farm_farmCode.Text = ""
            TextBox_farmInfo_farmName.Text = ""
            TextBox_farmInfo_owner.Text = ""
            TextBox_farmInfo_city.Text = ""
            TextBox_farmInfo_town.Text = ""
            TextBox_farmInfo_address.Text = ""
            Label_cattleMsg.Text = "查無該牧場資料。"
            Exit Sub
        End If

        ' === 載入資料 ===
        TextBox_farm_farmCode.Text = MaskFarmCode(farm.farmCode)
        TextBox_farmInfo_farmName.Text = farm.farmName
        TextBox_farmInfo_owner.Text = farm.owner
        TextBox_farmInfo_city.Text = farm.city
        TextBox_farmInfo_town.Text = farm.town
        TextBox_farmInfo_address.Text = farm.address

        Label_cattleMsg.Text = ""
    End Sub

    Sub BindCattleGridView()
        Dim dtResult As Data.DataTable = Get_CattleList(Property_farmID, Property_selectedHisType, Property_showCurrentOnly)
        GridView_cattleList.DataSource = dtResult
        GridView_cattleList.DataBind()

        Label_recordCount.Text = dtResult.Rows.Count.ToString("N0")
    End Sub

#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then

            '縣市
            taifCattle_con.BindDropDownList_city(DropDownList_farmCity, True)

            '鄉鎮
            taifCattle_con.BindDropDownList_area(DropDownList_farmTown, DropDownList_farmCity.SelectedValue, True)

        End If
    End Sub

    Private Sub StaticsFarm_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Page.ClientScript.RegisterStartupScript(Me.Page.GetType(), "page_js", js.ToString(), True)
    End Sub

    Private Sub DropDownList_farmCity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_farmCity.SelectedIndexChanged

        '鄉鎮
        taifCattle_con.BindDropDownList_area(DropDownList_farmTown, DropDownList_farmCity.SelectedValue, True)
    End Sub

    Private Sub LinkButton_farmQuery_Click(sender As Object, e As EventArgs) Handles LinkButton_farmQuery.Click
        '隱藏區塊
        Panel_farm.Visible = False
        Panel_result.Visible = False

        '清除選取
        HiddenField_selectedFarm.Value = ""

        '儲存搜尋條件
        SaveFarmQueryCondition()

        '取得資料
        GridView_farmList.PageIndex = 0
        BindFarmGridView()

        '牛籍查詢條件
        taifCattle_cattle.Bind_CheckBoxList_hisType(CheckBoxList_status, "旅程")

        '顯示區塊
        Panel_farm.Visible = True

    End Sub

    Private Sub GridView_farmList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_farmList.PageIndexChanging
        GridView_farmList.PageIndex = e.NewPageIndex
        BindFarmGridView()
    End Sub


    Private Sub LinkButton_cattleQuery_Click(sender As Object, e As EventArgs) Handles LinkButton_cattleQuery.Click

        ' 先檢查條件
        If Not ValidateCattleQueryCondition() Then
            Exit Sub
        End If

        '儲存篩選條件
        SaveCattleQueryCondition()

        '載入牧場基本資料
        Load_FarmInfo(Property_farmID)

        '取得牛籍資料
        BindCattleGridView()

        '顯示結果
        Panel_result.Visible = True
    End Sub
    Private Sub GridView_cattleList_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView_cattleList.RowCommand
        If e.CommandName = "_cattle" Then
            Dim cattleID As Integer = Convert.ToInt32(e.CommandArgument)

            ' 儲存 Session 狀態
            Session("CattleManage") = enum_EditMode.編輯
            Session("CattleManage_cid") = e.CommandArgument

            '另外開新視窗
            Dim url As String = ResolveUrl("~/Pages/Data/CattleManage_Detail.aspx")
            js.AppendLine($"window.open('{url}', '_blank');")

            'Response.Redirect("../Data/CattleManage_Detail.aspx")
        End If
    End Sub
    Private Sub LinkButton_excel_Click(sender As Object, e As EventArgs) Handles LinkButton_excel.Click
        ' 取得牛籍資料
        Dim dtResult As Data.DataTable = Get_CattleList(Property_farmID, Property_selectedHisType, Property_showCurrentOnly)

        ' 建立 Workbook & Sheet
        Dim wb As New XSSFWorkbook()
        Dim ws As ISheet = wb.CreateSheet("牛籍清單")

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

        ' === 標題列 ===
        Dim headers() As String = {
        "牛籍編號",
        "編號備註",
        "狀態",
        "除籍",
        "類型規格",
        "出生年度",
        "牛籍歲齡",
        "平均乳產量",
        "牛籍備註"
    }

        Dim rowHeader = ws.CreateRow(0)
        For i As Integer = 0 To headers.Length - 1
            Dim cell = rowHeader.CreateCell(i)
            cell.SetCellValue(headers(i))
            cell.CellStyle = headerStyle
        Next

        ' === 資料列 ===
        For r As Integer = 0 To dtResult.Rows.Count - 1
            Dim dr = dtResult.Rows(r)
            Dim dataRow = ws.CreateRow(r + 1)

            ' 牛籍編號
            dataRow.CreateCell(0).SetCellValue(dr("tagNo").ToString())
            dataRow.GetCell(0).CellStyle = cellStyleCenter

            ' 編號備註
            dataRow.CreateCell(1).SetCellValue(dr("tagMemo").ToString())
            dataRow.GetCell(1).CellStyle = cellStyleCenter

            ' 狀態
            dataRow.CreateCell(2).SetCellValue(dr("latestTypeName").ToString())
            dataRow.GetCell(2).CellStyle = cellStyleCenter

            ' 除籍
            Dim removeText = If(IsDBNull(dr("removeDate")) OrElse dr("removeDate") Is Nothing, "未除籍", "已除籍")
            dataRow.CreateCell(3).SetCellValue(removeText)
            dataRow.GetCell(3).CellStyle = cellStyleCenter

            ' 類型規格
            dataRow.CreateCell(4).SetCellValue($"{dr("groupName")}：{dr("typeName")}")
            dataRow.GetCell(4).CellStyle = cellStyleCenter

            ' 出生年度
            Dim birthYear = If(Convert.ToInt32(dr("birthYear")) = -1, "-", dr("birthYear").ToString())
            dataRow.CreateCell(5).SetCellValue(birthYear)
            dataRow.GetCell(5).CellStyle = cellStyleCenter

            ' 牛籍歲齡
            Dim cattleAge = If(Convert.ToInt32(dr("cattleAge")) = -1, "-", dr("cattleAge").ToString())
            dataRow.CreateCell(6).SetCellValue(cattleAge)
            dataRow.GetCell(6).CellStyle = cellStyleCenter

            ' 平均乳產量
            Dim milkVal As String = "-"
            If Convert.ToInt32(dr("cattleTypeID")) = 2 Then
                If Convert.ToDecimal(dr("milkProduction")) <> -1 Then
                    milkVal = dr("milkProduction").ToString()
                End If
            End If
            dataRow.CreateCell(7).SetCellValue(milkVal)
            dataRow.GetCell(7).CellStyle = cellStyleCenter

            ' 牛籍備註
            dataRow.CreateCell(8).SetCellValue(dr("cattleMemo").ToString())
            dataRow.GetCell(8).CellStyle = cellStyleLeft
        Next

        ' === 固定欄寬 ===
        ws.SetColumnWidth(0, 15 * 256)
        ws.SetColumnWidth(1, 20 * 256)
        ws.SetColumnWidth(2, 10 * 256)
        ws.SetColumnWidth(3, 10 * 256)
        ws.SetColumnWidth(4, 20 * 256)
        ws.SetColumnWidth(5, 12 * 256)
        ws.SetColumnWidth(6, 12 * 256)
        ws.SetColumnWidth(7, 15 * 256)
        ws.SetColumnWidth(8, 40 * 256)

        ' === 檔案名稱 ===
        Dim fileName As String = $"牛籍清單_{Now:yyyyMMddHHmmss}.xlsx"

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