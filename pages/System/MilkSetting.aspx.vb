Imports System.Data.SqlClient

Public Class MilkSetting
    Inherits taifCattle.Base
    Public js As New StringBuilder

#Region "Fun/Sub"
    Function Get_MailSetting() As Data.DataTable

        Dim sqlString As String =
        <sql>
            select age,milkProduction,remark,updateDateTime,updateUserID 
            from System_MilkSetting
            order by age
        </sql>.Value

        Using da As New DataAccess.MS_SQL
            Return da.GetDataTable(sqlString)
        End Using
    End Function

    Private Sub BindGridView()
        Dim dtSetting As Data.DataTable = Get_MailSetting()
        GridView_milkSetting.DataSource = dtSetting
        GridView_milkSetting.DataBind()
    End Sub
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            '讀取設定
            BindGridView()
        End If
    End Sub

    Private Sub LinkButton_save_Click(sender As Object, e As EventArgs) Handles LinkButton_save.Click
        Dim valuesList As New List(Of String)
        Dim errorCount As Integer = 0
        ' 逐列檢查輸入
        For Each row As GridViewRow In GridView_milkSetting.Rows
            Dim age As Integer = Convert.ToInt32(GridView_milkSetting.DataKeys(row.RowIndex).Value)

            Dim txtMilk As TextBox = CType(row.FindControl("TextBox_milkProduction"), TextBox)
            Dim inputMilk As String = If(txtMilk IsNot Nothing, txtMilk.Text.Trim(), "")
            Dim milkValue As Decimal = 0D

            ' 乳量：空字串 → 0.00；必須 0~999.99
            If String.IsNullOrEmpty(inputMilk) Then
                milkValue = 0D
                If txtMilk IsNot Nothing Then txtMilk.BackColor = Drawing.Color.White
            ElseIf Not Decimal.TryParse(inputMilk, milkValue) OrElse milkValue < 0D OrElse milkValue > 999.99D Then
                errorCount += 1
                If txtMilk IsNot Nothing Then txtMilk.BackColor = Drawing.Color.MistyRose
                Continue For
            Else
                If txtMilk IsNot Nothing Then txtMilk.BackColor = Drawing.Color.White
            End If

            ' 備註：允許空字串；跳脫單引號
            Dim txtRemark As TextBox = CType(row.FindControl("TextBox_remark"), TextBox)
            Dim remark As String = If(txtRemark IsNot Nothing, txtRemark.Text.Trim(), "")

            Dim remarkSqlLiteral As String = "N'" & remark.Replace("'", "''") & "'"

            ' 組 VALUES (Age, MilkProduction, Remark)
            valuesList.Add($"({age}, {milkValue.ToString("0.00")}, {remarkSqlLiteral})")
        Next

        ' 若有錯誤
        If errorCount > 0 Then
            Label_message.Text = $"有 {errorCount} 筆輸入錯誤，請修正後再儲存。"
            js.AppendLine("showModal();")
            Exit Sub
        End If

        ' 確保有資料
        If valuesList.Count = 0 Then
            Label_message.Text = "沒有可更新的資料。"
            js.AppendLine("showModal();")
            Exit Sub
        End If

        ' 組合 MERGE SQL
        Dim sql As String = $"
            MERGE INTO System_MilkSetting AS Target
            USING (VALUES {String.Join(",", valuesList)}) AS Source (age, milkProduction, remark)
                ON Target.age = Source.age
            WHEN MATCHED THEN
                UPDATE SET 
                    Target.milkProduction = Source.milkProduction,
                    Target.remark         = Source.remark,
                    Target.updateUserID     = @userID,
                    Target.updateDateTime     = GETDATE();
        "
        Dim userInfo As taifCattle.Base.stru_LoginUserInfo = Session("userInfo")

        ' 執行批次更新
        Using da As New DataAccess.MS_SQL
            da.ExecNonQuery(sql, New SqlParameter("userID", userInfo.accountID))
        End Using

        ' 更新成功訊息
        Label_message.Text = "儲存成功！"
        js.AppendLine("showModal();")

        'log
        Insert_UserLog(userInfo.accountID, taifCattle.Base.enum_UserLogItem.平均產乳量設定, taifCattle.Base.enum_UserLogType.修改)

        ' 讀取資料
        BindGridView()
    End Sub

    Private Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Page.ClientScript.RegisterStartupScript(Me.Page.GetType(), "page_js", js.ToString(), True)
    End Sub

End Class