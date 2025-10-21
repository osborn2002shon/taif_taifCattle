Imports NPOI.SS.UserModel
Imports NPOI.SS.Util
Imports NPOI.XSSF.UserModel
Imports System.IO

Public Class FarmManage
    Inherits taifCattle.Base
    Dim taifCattle_con As New taifCattle.Control
    Dim taifCattle_farm As New taifCattle.Farm

#Region "Property"

    ''' <summary>
    ''' 搜尋條件：縣市
    ''' </summary>
    Public Property Property_Query_city As String
        Get
            If IsNothing(ViewState("Property_Query_city")) Then
                Return "%"
            Else
                Return ViewState("Property_Query_city").ToString()
            End If
        End Get
        Set(value As String)
            ViewState("Property_Query_city") = value
        End Set
    End Property

    ''' <summary>
    ''' 搜尋條件：操作類型
    ''' </summary>
    Public Property Property_Query_town As String
        Get
            If IsNothing(ViewState("Property_Query_town")) Then
                Return "%"
            Else
                Return ViewState("Property_Query_town").ToString()
            End If
        End Get
        Set(value As String)
            ViewState("Property_Query_town") = value
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
#End Region
#Region "Fun/Sub"
    Private Sub SwitchView(mode As enum_EditMode)
        Select Case mode
            Case enum_EditMode.預設
                MultiView_main.SetActiveView(View_list)
            Case enum_EditMode.編輯
                MultiView_main.SetActiveView(View_edit)
            Case enum_EditMode.新增
                MultiView_main.SetActiveView(View_edit)

        End Select
    End Sub
    ''' <summary>
    ''' 儲存搜尋條件
    ''' </summary>
    Private Sub SaveQueryCondition()

        '操作項目
        Property_Query_city = DropDownList_city.SelectedValue

        '操作類型
        Property_Query_town = DropDownList_town.SelectedValue

        '關鍵字
        Property_Query_keyWord = TextBox_keyWord.Text.Trim()
    End Sub

    Sub BindGridView()
        Dim liFarm As List(Of taifCattle.Farm.stru_farmInfo) = taifCattle_farm.GetFarmList(Property_Query_city, Property_Query_town, Property_Query_keyWord)
        GridView_farmList.DataSource = liFarm
        GridView_farmList.DataBind()

        Label_recordCount.Text = liFarm.Count.ToString("N0")
    End Sub

    Function MaskFarmCode(value As Object) As String
        If value Is Nothing OrElse IsDBNull(value) Then
            Return ""
        End If

        Dim code As String = value.ToString().Trim()
        If code.Length = 10 Then
            ' 例：A123456789 → A12***6789
            Return code.Substring(0, 3) & "***" & code.Substring(6)
        End If

        ' 不是 10 碼就不處理
        Return code
    End Function

    ''' <summary>
    ''' 載入指定畜牧場資料至表單
    ''' </summary>
    ''' <param name="farmID"></param>
    Private Sub LoadFarmData(farmID As Integer)
        ClearFarmForm()

        Dim info As taifCattle.Farm.stru_farmInfo = taifCattle_farm.GetFarmByID(farmID)

        If info.farmID = 0 Then
            ' 查無資料處理
            Label_editTitle.Text = "查無資料"
            Exit Sub
        End If

        ' 標題設定
        Label_editTitle.Text = "編輯畜牧場"

        ' 填入資料
        TextBox_farmName.Text = info.farmName

        ' ---- 證號處理 ----
        Label_farmCode_display.Text = info.farmCode
        Label_farmCode_display.Visible = True
        TextBox_farmCode.Visible = False

        TextBox_owner.Text = info.owner
        TextBox_ownerID.Text = info.ownerID
        TextBox_ownerTel.Text = info.ownerTel
        TextBox_address.Text = info.address
        TextBox_animalCount.Text = info.animalCount
        TextBox_memo.Text = info.memo

        ' 下拉選單（縣市、鄉鎮）
        taifCattle_con.BindDropDownList_city(DropDownList_editCity, False)
        If DropDownList_editCity.Items.FindByValue(info.cityID.ToString()) IsNot Nothing Then
            DropDownList_editCity.SelectedValue = info.cityID.ToString()
        End If

        taifCattle_con.BindDropDownList_area(DropDownList_editTown, DropDownList_editCity.SelectedValue, False)
        If DropDownList_editTown.Items.FindByValue(info.twID.ToString()) IsNot Nothing Then
            DropDownList_editTown.SelectedValue = info.twID.ToString()
        End If


    End Sub

    ''' <summary>
    ''' 清空表單欄位內容
    ''' </summary>
    Private Sub ClearFarmForm()
        TextBox_farmName.Text = ""
        TextBox_farmCode.Text = ""
        Label_farmCode_display.Text = "'"
        TextBox_owner.Text = ""
        TextBox_ownerID.Text = ""
        TextBox_ownerTel.Text = ""
        TextBox_address.Text = ""
        TextBox_animalCount.Text = ""
        TextBox_memo.Text = ""
        Label_msg.Text = ""

        DropDownList_editCity.Items.Clear()
        DropDownList_editCity.SelectedIndex = -1
        DropDownList_editTown.Items.Clear()
        DropDownList_editTown.SelectedIndex = -1
    End Sub

    ''' <summary>
    ''' 檢查必填欄位（依據新增 / 編輯模式）
    ''' </summary>
    ''' <param name="isAddMode"></param>
    ''' <returns></returns>
    Private Function ValidateFarmForm(isAddMode As Boolean) As Boolean
        Dim msg As New List(Of String)

        ' === 新增模式才需要檢查 farmCode ===
        If isAddMode Then
            If String.IsNullOrWhiteSpace(TextBox_farmCode.Text) Then
                msg.Add("請輸入畜牧場證號或負責人證號。")
            Else
                If CheckFarmID(TextBox_farmCode.Text.Trim()) = False Then
                    msg.Add("請輸入正確的畜牧場證號或負責人證號。")
                Else
                    If taifCattle_farm.CheckFarmCodeExists(TextBox_farmCode.Text.Trim()) Then
                        msg.Add("畜牧場證號或負責人證號重複。")
                    End If

                End If
            End If
        End If

        ' === 共用必填欄位 ===
        If String.IsNullOrWhiteSpace(TextBox_farmName.Text) Then
            msg.Add("請輸入畜牧場名稱。")
        End If

        If String.IsNullOrWhiteSpace(TextBox_owner.Text) Then
            msg.Add("請輸入負責人姓名。")
        End If

        If DropDownList_editCity.SelectedIndex < 0 Then
            msg.Add("請選擇縣市。")
        End If

        If DropDownList_editTown.SelectedIndex < 0 Then
            msg.Add("請選擇鄉鎮。")
        End If

        If String.IsNullOrWhiteSpace(TextBox_address.Text) Then
            msg.Add("請輸入地址。")
        End If

        ' === 顯示錯誤訊息 ===
        If msg.Count > 0 Then
            Label_msg.Text = "以下欄位未填寫：<br/>" & String.Join("<br/>", msg)
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' 從表單收集牧場資料
    ''' </summary>
    Private Function CollectFarmData() As taifCattle.Farm.stru_farmInfo
        Dim info As New taifCattle.Farm.stru_farmInfo

        ' === 基本資料 ===
        info.farmID = Property_farmID
        info.farmName = TextBox_farmName.Text.Trim()
        info.farmCode = TextBox_farmCode.Text.Trim()
        info.owner = TextBox_owner.Text.Trim()
        info.ownerID = TextBox_ownerID.Text.Trim()
        info.ownerTel = TextBox_ownerTel.Text.Trim()

        ' === 行政區 ===
        If DropDownList_editTown.SelectedValue <> "" Then
            info.twID = CInt(DropDownList_editTown.SelectedValue)
        Else
            info.twID = 0
        End If

        ' === 地址 ===
        info.address = TextBox_address.Text.Trim()

        ' === 其他欄位 ===
        info.animalCount = TextBox_animalCount.Text.Trim()
        info.memo = TextBox_memo.Text.Trim()
        info.insertType = "人工網頁建檔"

        ' === 操作者資訊 ===
        Dim userInfo As taifCattle.Base.stru_LoginUserInfo = Session("userInfo")
        Dim currentUserID As Integer = userInfo.accountID
        info.insertAccountID = currentUserID
        info.updateAccountID = currentUserID

        Return info
    End Function
#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            '預設畫面
            SwitchView(enum_EditMode.預設)

            '縣市
            taifCattle_con.BindDropDownList_city(DropDownList_city, True)

            '鄉鎮
            taifCattle_con.BindDropDownList_area(DropDownList_town, DropDownList_city.SelectedValue, True)

            '搜尋紀錄
            SaveQueryCondition()

            '取得資料
            BindGridView()
        End If
    End Sub

    Private Sub LinkButton_query_Click(sender As Object, e As EventArgs) Handles LinkButton_query.Click
        '搜尋紀錄
        SaveQueryCondition()

        '取得資料
        BindGridView()
    End Sub

    Private Sub GridView_farmList_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView_farmList.RowCommand
        Select Case e.CommandName
            Case "myEdit"
                Dim farmID As Integer = Convert.ToInt32(e.CommandArgument)
                Property_farmID = farmID
                LoadFarmData(farmID)
                SwitchView(enum_EditMode.編輯)
        End Select
    End Sub

    Private Sub LinkButton_excel_Click(sender As Object, e As EventArgs) Handles LinkButton_excel.Click
        ' 取得資料
        Dim liFarm As List(Of taifCattle.Farm.stru_farmInfo) = taifCattle_farm.GetFarmList(Property_Query_city, Property_Query_town, Property_Query_keyWord)

        ' 建立 Workbook & Sheet
        Dim wb As New XSSFWorkbook()
        Dim ws As ISheet = wb.CreateSheet("牧場資料")

        ' === 建立 DataFormat 物件（用於設定文字格式）===
        Dim dataFormat = wb.CreateDataFormat()

        ' === 標題樣式 ===
        Dim headerStyle As ICellStyle = wb.CreateCellStyle()
        headerStyle.Alignment = HorizontalAlignment.Center
        headerStyle.VerticalAlignment = VerticalAlignment.Center
        headerStyle.WrapText = True ' 標題換行
        Dim fontHeader = wb.CreateFont()
        fontHeader.IsBold = True
        headerStyle.SetFont(fontHeader)

        ' === 內容樣式（一般欄位置中 + 換行）===
        Dim cellStyleCenter As ICellStyle = wb.CreateCellStyle()
        cellStyleCenter.Alignment = HorizontalAlignment.Center
        cellStyleCenter.VerticalAlignment = VerticalAlignment.Center
        cellStyleCenter.WrapText = True

        ' === 證號欄專用樣式（文字格式、置中）===
        Dim cellStyleText As ICellStyle = wb.CreateCellStyle()
        cellStyleText.Alignment = HorizontalAlignment.Center
        cellStyleText.VerticalAlignment = VerticalAlignment.Center
        cellStyleText.WrapText = True
        cellStyleText.DataFormat = dataFormat.GetFormat("@")

        ' === 地址樣式（靠左 + 換行）===
        Dim cellStyleLeft As ICellStyle = wb.CreateCellStyle()
        cellStyleLeft.Alignment = HorizontalAlignment.Left
        cellStyleLeft.VerticalAlignment = VerticalAlignment.Center
        cellStyleLeft.WrapText = True

        ' === 標題列===
        Dim headers() As String = {
            "縣市",
            "鄉鎮",
            "畜牧場名稱",
            "畜牧場證號 (畜牧場證號/畜禽飼養登記證/負責人證號)",
            "負責人",
            "畜牧場地址"
        }

        Dim rowHeader = ws.CreateRow(0)
        For i As Integer = 0 To headers.Length - 1
            Dim cell = rowHeader.CreateCell(i)
            cell.SetCellValue(headers(i))
            cell.CellStyle = headerStyle
        Next

        ' === 資料列 ===
        For r As Integer = 0 To liFarm.Count - 1
            Dim f = liFarm(r)
            Dim dataRow = ws.CreateRow(r + 1)

            Dim c0 = dataRow.CreateCell(0)
            c0.SetCellValue(f.city)
            c0.CellStyle = cellStyleCenter

            Dim c1 = dataRow.CreateCell(1)
            c1.SetCellValue(f.town)
            c1.CellStyle = cellStyleCenter

            Dim c2 = dataRow.CreateCell(2)
            c2.SetCellValue(f.farmName)
            c2.CellStyle = cellStyleCenter

            Dim c3 = dataRow.CreateCell(3)
            c3.SetCellValue(MaskFarmCode(f.farmCode))
            c3.CellStyle = cellStyleText

            Dim c4 = dataRow.CreateCell(4)
            c4.SetCellValue(f.owner)
            c4.CellStyle = cellStyleCenter

            Dim c5 = dataRow.CreateCell(5)
            c5.SetCellValue(f.address)
            c5.CellStyle = cellStyleLeft
        Next

        ' === 固定欄寬設定 ===
        ' NPOI 欄寬單位 = 1/256 字元寬度
        ws.SetColumnWidth(0, 10 * 256)   ' 縣市 
        ws.SetColumnWidth(1, 10 * 256)   ' 鄉鎮
        ws.SetColumnWidth(2, 20 * 256)  ' 畜牧場名稱
        ws.SetColumnWidth(3, 30 * 256)  ' 畜牧場證號
        ws.SetColumnWidth(4, 15 * 256)   ' 負責人
        ws.SetColumnWidth(5, 50 * 256)  ' 畜牧場地址

        ' === 檔案名稱 ===
        Dim fileName As String = $"牧場資料_{Now:yyyyMMddHHmmss}.xlsx"

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

    Private Sub GridView_farmList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_farmList.PageIndexChanging
        GridView_farmList.PageIndex = e.NewPageIndex
        BindGridView()
    End Sub

    Private Sub Button_cancel_Click(sender As Object, e As EventArgs) Handles Button_cancel.Click
        SwitchView(enum_EditMode.預設)
    End Sub

    Private Sub LinkButton_addFarm_Click(sender As Object, e As EventArgs) Handles LinkButton_addFarm.Click
        ClearFarmForm()

        ' 顯示可輸入的欄位
        TextBox_farmCode.Visible = True
        Label_farmCode_display.Visible = False

        taifCattle_con.BindDropDownList_city(DropDownList_editCity, False)
        taifCattle_con.BindDropDownList_area(DropDownList_editTown, DropDownList_editCity.SelectedValue, False)

        Label_editTitle.Text = "新增畜牧場"
        Property_farmID = -1
        SwitchView(enum_EditMode.編輯)
    End Sub
    Private Sub DropDownList_city_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_city.SelectedIndexChanged
        '鄉鎮
        taifCattle_con.BindDropDownList_area(DropDownList_town, DropDownList_city.SelectedValue, True)
    End Sub
    Private Sub DropDownList_editCity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_editCity.SelectedIndexChanged
        '鄉鎮
        taifCattle_con.BindDropDownList_area(DropDownList_editTown, DropDownList_editCity.SelectedValue, False)
    End Sub

    Private Sub Button_save_Click(sender As Object, e As EventArgs) Handles Button_save.Click
        Dim isAddMode As Boolean = (Property_farmID = -1)

        '驗證必填欄位
        If Not ValidateFarmForm(isAddMode) Then
            Exit Sub
        End If

        Dim info As taifCattle.Farm.stru_farmInfo = CollectFarmData()

        If isAddMode Then
            taifCattle_farm.InsertFarm(info)
            Label_msg.Text = "牧場資料已新增成功！"
        Else
            taifCattle_farm.UpdateFarm(info)
            Label_msg.Text = "牧場資料已更新成功！"
        End If

    End Sub
End Class