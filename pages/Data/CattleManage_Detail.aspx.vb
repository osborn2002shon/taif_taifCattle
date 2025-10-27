Imports taifCattle.taifCattle
Imports taifCattle.taifCattle.Cattle

Public Class CattleManage_Detail
    Inherits taifCattle.Base
    Dim taifCattle_cattle As New taifCattle.Cattle
    Dim taifCattle_farm As New taifCattle.Farm

    Sub Bind_EditInfo(cattleID As Integer)
        '===視覺圖
        MultiView_main.SetActiveView(View_edit)
        HiddenField_cattleID.Value = cattleID

        '===基本資料
        Dim cattleInfo As taifCattle.Cattle.stru_cattleInfo_view = taifCattle_cattle.Get_CattleInfo(cattleID)
        TextBox_edit_tagNo.Text = cattleInfo.tagNo
        TextBox_edit_tagMemo.Text = cattleInfo.tagMemo
        TextBox_edit_memo.Text = cattleInfo.cattleMemo
        TextBox_edit_birthYear.Text = IIf(cattleInfo.birthYear = -1, "", cattleInfo.birthYear)
        TextBox_edit_cattleAge.Text = IIf(cattleInfo.cattleAge = -1, "-", cattleInfo.cattleAge)
        If cattleInfo.cattleTypeID = 2 Then
            '乳母牛才有乳量、並且要有出生年度才能算
            TextBox_edit_milkProduction.Text = IIf(cattleInfo.milkProduction = -1, "-", cattleInfo.milkProduction)
        Else
            TextBox_edit_milkProduction.Text = "-"
        End If
        DropDownList_edit_typeName.SelectedValue = cattleInfo.cattleTypeID

        '===旅程紀錄
        Dim cattleHis_def As List(Of taifCattle.Cattle.stru_cattleHistory_view) = taifCattle_cattle.Get_CattleHistoryList(cattleID, enum_hisType.defHis)
        GridView_his_def.DataSource = cattleHis_def
        GridView_his_def.DataBind()

        '===除籍紀錄
        Dim cattleHis_end As List(Of taifCattle.Cattle.stru_cattleHistory_view) = taifCattle_cattle.Get_CattleHistoryList(cattleID, enum_hisType.endHis)
        GridView_his_end.DataSource = cattleHis_end
        GridView_his_end.DataBind()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Dim s_cattleManage As Object = Session("CattleManage")
            Dim s_cattleID As Object = Session("CattleManage_cid")

            taifCattle_cattle.Bind_DropDownList_cattleType(DropDownList_typeName, False, "%", True)
            taifCattle_cattle.Bind_DropDownList_cattleType(DropDownList_edit_typeName, False, "%", True)
            taifCattle_cattle.Bind_DropDownList_hisType(DropDownList_edit_hisDef_hisType, "旅程")
            taifCattle_cattle.Bind_DropDownList_hisType(DropDownList_edit_hisEnd_hisType, "除籍")
            Select Case DropDownList_edit_hisEnd_hisType.SelectedValue
                Case 5
                    taifCattle_cattle.Bind_DropDownList_slau(DropDownList_edit_hisEnd_place, False)
                    DropDownList_edit_hisEnd_place.Enabled = True
                Case 6
                    taifCattle_cattle.Bind_DropDownList_plant(DropDownList_edit_hisEnd_place, False)
                    DropDownList_edit_hisEnd_place.Enabled = True
                Case Else
                    DropDownList_edit_hisEnd_place.Items.Clear()
                    DropDownList_edit_hisEnd_place.Items.Add(New ListItem("*無", -1))
                    DropDownList_edit_hisEnd_place.Enabled = False
            End Select

            TextBox_edit_hisDef_date.Text = Today.ToString("yyyy-MM-dd")
            TextBox_edit_hisEnd_date.Text = Today.ToString("yyyy-MM-dd")

            Dim mode As enum_EditMode
            If IsNothing(s_cattleManage) Then
                mode = enum_EditMode.預設
            Else
                mode = CType(s_cattleManage, enum_EditMode)
            End If

            Dim cattleID As Integer = -1
            If IsNumeric(s_cattleID) = True Then
                cattleID = CInt(s_cattleID)
            End If

            Select Case mode
                Case enum_EditMode.新增
                    MultiView_main.SetActiveView(View_new)

                Case enum_EditMode.編輯
                    Bind_EditInfo(cattleID)

                Case Else
                    Response.Redirect("CattleManage.aspx")

            End Select

        End If
    End Sub

    Private Sub LinkButton_save_Click(sender As Object, e As EventArgs) Handles LinkButton_save.Click
        Dim userInfo As taifCattle.Base.stru_LoginUserInfo = Session("userInfo")
        Dim insertDateTime As Date = Now
        Dim cattleInfo As New taifCattle.Cattle.stru_cattleInfo
        cattleInfo.tagNo = TextBox_tagNo.Text.Trim
        cattleInfo.tagMemo = TextBox_tagMemo.Text.Trim
        cattleInfo.cattleTypeID = DropDownList_typeName.SelectedValue
        cattleInfo.birthYear = TextBox_birthYear.Text.Trim
        cattleInfo.cattleMemo = TextBox_memo.Text.Trim
        cattleInfo.insertDateTime = insertDateTime
        cattleInfo.updateDateTime = insertDateTime
        cattleInfo.insertAccountID = userInfo.accountID
        cattleInfo.updateAccountID = userInfo.accountID

        '===驗證
        If String.IsNullOrEmpty(cattleInfo.tagNo) Then
            Label_addMsg.Text = "新增失敗，請確認已輸入牛籍編號！"
            Exit Sub
        End If
        If taifCattle_cattle.Check_IsCattleExist(cattleInfo.tagNo).isPass Then
            Label_addMsg.Text = "新增失敗，牛籍編號已存在！"
            Exit Sub
        End If

        '===新增
        cattleInfo.cattleID = taifCattle_cattle.Insert_Cattle(cattleInfo)
        Insert_UserLog(userInfo.accountID, enum_UserLogItem.牛籍資料管理, enum_UserLogType.新增, cattleInfo.cattleID, insertDateTime)

        '===跳轉編輯
        Bind_EditInfo(cattleInfo.cattleID)

    End Sub

    Private Sub DropDownList_edit_hisEnd_hisType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_edit_hisEnd_hisType.SelectedIndexChanged
        Select Case DropDownList_edit_hisEnd_hisType.SelectedValue
            Case 5
                taifCattle_cattle.Bind_DropDownList_slau(DropDownList_edit_hisEnd_place, False)
                DropDownList_edit_hisEnd_place.Enabled = True
            Case 6
                taifCattle_cattle.Bind_DropDownList_plant(DropDownList_edit_hisEnd_place, False)
                DropDownList_edit_hisEnd_place.Enabled = True
            Case Else
                DropDownList_edit_hisEnd_place.Items.Clear()
                DropDownList_edit_hisEnd_place.Items.Add(New ListItem("*無", -1))
                DropDownList_edit_hisEnd_place.Enabled = False
        End Select
    End Sub

    Private Sub LinkButton_edit_update_Click(sender As Object, e As EventArgs) Handles LinkButton_edit_update.Click
        Dim userInfo As taifCattle.Base.stru_LoginUserInfo = Session("userInfo")
        Dim updateDateTime As Date = Now
        Dim cattleInfo As New taifCattle.Cattle.stru_cattleInfo
        cattleInfo.cattleID = HiddenField_cattleID.Value
        cattleInfo.tagMemo = TextBox_edit_tagMemo.Text.Trim
        cattleInfo.cattleTypeID = DropDownList_edit_typeName.SelectedValue
        cattleInfo.birthYear = TextBox_edit_birthYear.Text.Trim
        cattleInfo.cattleMemo = TextBox_edit_memo.Text.Trim
        cattleInfo.updateDateTime = updateDateTime
        cattleInfo.updateAccountID = userInfo.accountID
        taifCattle_cattle.Update_Cattle(cattleInfo)
        Insert_UserLog(userInfo.accountID, enum_UserLogItem.牛籍資料管理, enum_UserLogType.修改, cattleInfo.cattleID, updateDateTime)
        Label_edit_msg.Text = "儲存成功！"
        Bind_EditInfo(cattleInfo.cattleID)
    End Sub

    Private Sub LinkButton_edit_insert_hisDef_Click(sender As Object, e As EventArgs) Handles LinkButton_edit_insert_hisDef.Click

        Dim farmCode As String = TextBox_edit_hisDef_farmCode.Text.Trim
        If String.IsNullOrEmpty(farmCode) Then
            Label_edit_msg_hisDef.Text = "新增失敗，請輸入證號！"
            Exit Sub
        End If

        Dim farmInfo As taifCattle.Farm.stru_farmInfo = taifCattle_farm.Get_FarmByCode(farmCode)
        If farmInfo.farmID = -1 Then
            Label_edit_msg_hisDef.Text = "新增失敗，查無此畜牧場！"
            Exit Sub
        End If

        Dim userInfo As taifCattle.Base.stru_LoginUserInfo = Session("userInfo")
        Dim insertDateTime As Date = Now
        Dim cattleHisInfo As New taifCattle.Cattle.stru_cattleHistory
        cattleHisInfo.cattleID = HiddenField_cattleID.Value
        cattleHisInfo.dataDate = TextBox_edit_hisDef_date.Text.Trim
        cattleHisInfo.hisTypeID = DropDownList_edit_hisDef_hisType.SelectedValue
        cattleHisInfo.memo = TextBox_edit_hisDef_memo.Text.Trim
        cattleHisInfo.farmID = farmInfo.farmID
        cattleHisInfo.plantID = ""
        cattleHisInfo.slauID = ""
        cattleHisInfo.insertType = enum_InsertType.人工網頁建檔
        cattleHisInfo.insertAccountID = userInfo.accountID
        cattleHisInfo.insertDateTime = insertDateTime

        If taifCattle_cattle.Check_IsHistoryExist(cattleHisInfo.cattleID, cattleHisInfo.dataDate, farmInfo.farmID) Then
            Label_edit_msg_hisDef.Text = "新增失敗，同一天同一場已有重複旅程！"
            Exit Sub
        Else
            Dim hisID As Integer = taifCattle_cattle.Insert_CattleHistory(cattleHisInfo)
            Insert_UserLog(userInfo.accountID, enum_UserLogItem.牛籍資料管理, enum_UserLogType.修改, cattleHisInfo.cattleID & "_" & hisID, insertDateTime)
            Response.Redirect("CattleManage_Detail.aspx")
            Label_edit_msg_hisDef.Text = "新增成功！"
            Bind_EditInfo(cattleHisInfo.cattleID)
        End If

    End Sub

    Private Sub LinkButton_edit_insert_hisEnd_Click(sender As Object, e As EventArgs) Handles LinkButton_edit_insert_hisEnd.Click

        Dim slauID As Object = ""
        Dim plantID As Object = ""
        Dim placeID As Integer = -1
        Select Case DropDownList_edit_hisEnd_hisType.SelectedValue
            Case 5 '屠宰
                slauID = CInt(DropDownList_edit_hisEnd_place.SelectedValue)
                placeID = slauID
            Case 6 '化製
                plantID = CInt(DropDownList_edit_hisEnd_place.SelectedValue)
                placeID = plantID
        End Select
        Dim userInfo As taifCattle.Base.stru_LoginUserInfo = Session("userInfo")
        Dim insertDateTime As Date = Now
        Dim cattleHisInfo As New taifCattle.Cattle.stru_cattleHistory
        cattleHisInfo.cattleID = HiddenField_cattleID.Value
        cattleHisInfo.dataDate = TextBox_edit_hisEnd_date.Text.Trim
        cattleHisInfo.hisTypeID = DropDownList_edit_hisEnd_hisType.SelectedValue
        cattleHisInfo.memo = TextBox_edit_hisEnd_memo.Text.Trim
        cattleHisInfo.farmID = ""
        cattleHisInfo.plantID = plantID
        cattleHisInfo.slauID = slauID
        cattleHisInfo.insertType = enum_InsertType.人工網頁建檔
        cattleHisInfo.insertAccountID = userInfo.accountID
        cattleHisInfo.insertDateTime = insertDateTime

        If taifCattle_cattle.Check_IsHistoryExist(cattleHisInfo.cattleID, cattleHisInfo.dataDate, placeID) Then
            Label_edit_msg_hisEnd.Text = "新增失敗，同一天同一場已有重複旅程！"
            Exit Sub
        Else
            Dim hisID As Integer = taifCattle_cattle.Insert_CattleHistory(cattleHisInfo)
            Insert_UserLog(userInfo.accountID, enum_UserLogItem.牛籍資料管理, enum_UserLogType.修改, cattleHisInfo.cattleID & "_" & hisID, insertDateTime)
            Label_edit_msg_hisEnd.Text = "新增成功！"
            Bind_EditInfo(cattleHisInfo.cattleID)
        End If

    End Sub

    Private Sub GridView_his_def_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView_his_def.RowCommand
        If e.CommandName = "hisRemove" Then
            Dim hisID As Integer = e.CommandArgument
            Dim userInfo As taifCattle.Base.stru_LoginUserInfo = Session("userInfo")
            Dim removeDateTime As Date = Now
            Dim cattleID As Integer = HiddenField_cattleID.Value
            taifCattle_cattle.Remove_CattleHistroy(hisID, removeDateTime, userInfo.accountID)
            Insert_UserLog(userInfo.accountID, enum_UserLogItem.牛籍資料管理, enum_UserLogType.修改, cattleID & "_" & hisID, removeDateTime)
            Label_edit_msg_hisDef.Text = "刪除旅程紀錄成功！"
            Bind_EditInfo(cattleID)
        End If
    End Sub

    Private Sub GridView_his_end_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView_his_end.RowCommand
        If e.CommandName = "hisRemove" Then
            Dim hisID As Integer = e.CommandArgument
            Dim userInfo As taifCattle.Base.stru_LoginUserInfo = Session("userInfo")
            Dim removeDateTime As Date = Now
            Dim cattleID As Integer = HiddenField_cattleID.Value
            taifCattle_cattle.Remove_CattleHistroy(hisID, removeDateTime, userInfo.accountID)
            Insert_UserLog(userInfo.accountID, enum_UserLogItem.牛籍資料管理, enum_UserLogType.修改, cattleID & "_" & hisID, removeDateTime)
            Label_edit_msg_hisEnd.Text = "刪除除籍紀錄成功！"
            Bind_EditInfo(cattleID)
        End If
    End Sub

    Private Sub LinkButton_backList_Click(sender As Object, e As EventArgs) Handles LinkButton_backList.Click
        Session("CattleManage") = enum_EditMode.預設
        Session("CattleManage_cid") = -1
        Response.Redirect("CattleManage.aspx")
    End Sub
End Class