Imports taifCattle.taifCattle.Cattle

Public Class CattleManage_Detail
    Inherits taifCattle.Base

    Dim taifCattle_cattle As New taifCattle.Cattle



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Dim mode As enum_EditMode
            If IsNothing(Session("CattleManage")) Then
                mode = enum_EditMode.預設
            Else
                mode = CType(Session("CattleManage"), enum_EditMode)
            End If
            Select Case mode
                Case enum_EditMode.預設
                    Response.Redirect("CattleManage.aspx")
                Case enum_EditMode.新增
                    MultiView_main.SetActiveView(View_new)
                Case enum_EditMode.編輯
                    MultiView_main.SetActiveView(View_edit)
                Case Else
                    Response.Redirect("CattleManage.aspx")
            End Select

            taifCattle_cattle.Bind_DropDownList_cattleType(DropDownList_typeName, False, "%", True)
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

        If String.IsNullOrEmpty(cattleInfo.tagNo) Then
            Label_addMsg.Text = "新增失敗，請確認已輸入牛籍編號！"
            Exit Sub
        End If

        If taifCattle_cattle.Check_IsCattleExist(cattleInfo.tagNo) Then
            Label_addMsg.Text = "新增失敗，牛籍編號已存在！"
            Exit Sub
        End If

        taifCattle_cattle.Insert_Cattle(cattleInfo)


    End Sub
End Class