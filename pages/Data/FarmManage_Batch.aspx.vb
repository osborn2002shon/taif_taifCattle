Imports System.Collections.Generic
Imports System.Data
Imports System.Data.DataTableExtensions
Imports System.Linq
Imports System.Web.UI
Imports System.Web.UI.WebControls

Public Class FarmManage_Batch
    Inherits taifCattle.Base

    Public js As New StringBuilder

    Private ReadOnly taifCattle_farm As New taifCattle.Farm()
    Private ReadOnly taifCattle_con As New taifCattle.Control()

    Private Const FarmDetailsBaseCssClass As String = "card-body farm-details"
    Private Const FarmDetailsCollapsedCssSuffix As String = " farm-details-collapsed"

    Private Property CurrentSerial As String
        Get
            Return Convert.ToString(ViewState("CurrentSerial"))
        End Get
        Set(value As String)
            ViewState("CurrentSerial") = value
        End Set
    End Property

    Private Sub FarmManage_Batch_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim masterPage As mp_default = TryCast(Me.Master, mp_default)
        If masterPage Is Nothing Then
            Return
        End If

        Dim serialQuery As String = Request.QueryString("serial")
        Dim parentMenuPage As String = ResolveParentMenuPageBySerial(serialQuery)

        If Not String.IsNullOrEmpty(parentMenuPage) Then
            masterPage.ParentMenuPage = parentMenuPage
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim serialQuery As String = Request.QueryString("serial")
        If String.IsNullOrWhiteSpace(serialQuery) Then
            Response.Redirect("~/pages/Data/FarmManage.aspx")
            Return
        End If

        If Not IsPostBack Then
            CurrentSerial = serialQuery.Trim()
            Label_message.Text = ""
            Label_message.CssClass = "text-danger fw-bold d-block mb-3"
            LoadMissingFarmRecords()
        End If
    End Sub

    Protected Overrides Sub OnPreRender(e As EventArgs)
        MyBase.OnPreRender(e)
        UpdateFarmCardDisplayStates()
    End Sub

    Private Sub LoadMissingFarmRecords()
        Dim serial As String = CurrentSerial
        Dim records As List(Of taifCattle.Farm.stru_missingFarmRecord) = taifCattle_farm.GetMissingFarmRecords(serial)

        Panel_serialInfo.Visible = True
        Literal_serial.Text = Server.HtmlEncode(serial)

        Dim dataSources As List(Of String) = records _
            .Select(Function(r) r.dataSource) _
            .Where(Function(ds) Not String.IsNullOrWhiteSpace(ds)) _
            .Distinct(StringComparer.OrdinalIgnoreCase) _
            .ToList()

        Dim displaySource As String
        If dataSources.Count = 0 Then
            displaySource = "未指定"
        Else
            displaySource = String.Join("、", dataSources.Select(Function(ds) GetDataSourceDisplayName(ds)))
        End If
        Literal_source.Text = Server.HtmlEncode(displaySource)

        Dim recordMap As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        For Each record In records
            If String.IsNullOrWhiteSpace(record.farmCode) Then
                Continue For
            End If
            Dim code As String = record.farmCode.Trim()
            If Not recordMap.ContainsKey(code) Then
                recordMap.Add(code, record.dataSource)
            End If
        Next

        If recordMap.Count = 0 Then
            Repeater_missingFarms.DataSource = Nothing
            Repeater_missingFarms.DataBind()
            Panel_noData.Visible = True
            Panel_actions.Visible = True
            LinkButton_submit.Visible = False
            LinkButton_submit.Enabled = False
            Return
        End If

        Panel_noData.Visible = False

        Dim existingCodes As HashSet(Of String) = taifCattle_farm.GetExistingFarmCodes(recordMap.Keys)

        Dim table As New DataTable()
        table.Columns.Add("farmCode", GetType(String))
        table.Columns.Add("dataSource", GetType(String))
        table.Columns.Add("isExisting", GetType(Boolean))

        For Each kvp In recordMap.OrderBy(Function(p) p.Key, StringComparer.OrdinalIgnoreCase)
            Dim row As DataRow = table.NewRow()
            row("farmCode") = kvp.Key
            row("dataSource") = kvp.Value
            row("isExisting") = existingCodes.Contains(kvp.Key)
            table.Rows.Add(row)
        Next

        Repeater_missingFarms.DataSource = table
        Repeater_missingFarms.DataBind()

        Dim canAdd As Boolean = table.AsEnumerable().Any(Function(r) r.Field(Of Boolean)("isExisting") = False)
        Panel_actions.Visible = True
        LinkButton_submit.Visible = canAdd
        LinkButton_submit.Enabled = canAdd
    End Sub

    Private Function ResolveParentMenuPageBySerial(serialNo As String) As String
        Dim dataSource As String = GetMissingImportDataSource(serialNo)
        If String.IsNullOrEmpty(dataSource) Then
            Return String.Empty
        End If

        Select Case dataSource.Trim()
            Case "CattleManage_Batch"
                Return "../Data/CattleManage_Batch.aspx"
            Case "HisManage_Batch"
                Return "../Data/hisManage_Batch.aspx"
            Case "HisEndManage_Batch"
                Return "../Data/hisEndManage_Batch.aspx"
            Case Else
                Return String.Empty
        End Select
    End Function

    Private Function GetMissingImportDataSource(serialNo As String) As String
        If String.IsNullOrWhiteSpace(serialNo) Then
            Return String.Empty
        End If

        Const sql As String = "SELECT TOP 1 dataSource FROM Data_FarmMissingImport WHERE serialNo = @serialNo ORDER BY missingID"
        Dim para As SqlClient.SqlParameter() = {
            New SqlClient.SqlParameter("@serialNo", serialNo.Trim())
        }

        Using da As New DataAccess.MS_SQL
            Dim result As Object = da.ExecuteScalar(sql, para)
            If result Is Nothing OrElse Convert.IsDBNull(result) Then
                Return String.Empty
            End If

            Return Convert.ToString(result)
        End Using
    End Function

    Protected Sub Repeater_missingFarms_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles Repeater_missingFarms.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem Then
            Return
        End If

        Dim ddlCity As DropDownList = TryCast(e.Item.FindControl("DropDownList_city"), DropDownList)
        Dim ddlArea As DropDownList = TryCast(e.Item.FindControl("DropDownList_area"), DropDownList)
        PopulateCityDropDown(ddlCity)
        PopulateAreaDropDown(ddlArea, String.Empty)

        Dim chkSelect As CheckBox = TryCast(e.Item.FindControl("CheckBox_select"), CheckBox)
        Dim lblExisting As Label = TryCast(e.Item.FindControl("Label_existing"), Label)
        Dim panelInputs As Panel = TryCast(e.Item.FindControl("Panel_inputs"), Panel)

        Dim dataRow As DataRowView = TryCast(e.Item.DataItem, DataRowView)
        Dim isExisting As Boolean = False
        If dataRow IsNot Nothing Then
            isExisting = Convert.ToBoolean(dataRow("isExisting"))
        End If

        If chkSelect IsNot Nothing Then
            chkSelect.Visible = Not isExisting
            chkSelect.Checked = Not isExisting
        End If

        If lblExisting IsNot Nothing Then
            lblExisting.Visible = isExisting
        End If

        If panelInputs IsNot Nothing Then
            panelInputs.Enabled = Not isExisting
        End If
    End Sub

    Protected Sub DropDownList_city_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim ddlCity As DropDownList = TryCast(sender, DropDownList)
        If ddlCity Is Nothing Then
            Return
        End If

        Dim item As RepeaterItem = TryCast(ddlCity.NamingContainer, RepeaterItem)
        If item Is Nothing Then
            Return
        End If

        Dim ddlArea As DropDownList = TryCast(item.FindControl("DropDownList_area"), DropDownList)
        If ddlArea Is Nothing Then
            Return
        End If

        PopulateAreaDropDown(ddlArea, ddlCity.SelectedValue)
    End Sub

    Protected Sub LinkButton_submit_Click(sender As Object, e As EventArgs) Handles LinkButton_submit.Click
        Dim userInfo As taifCattle.Base.stru_LoginUserInfo = CType(Session("userInfo"), taifCattle.Base.stru_LoginUserInfo)
        Dim operatorId As Integer = userInfo.accountID

        Dim errors As New List(Of String)()
        Dim successCount As Integer = 0
        Dim selectedCount As Integer = 0

        For Each item As RepeaterItem In Repeater_missingFarms.Items
            Dim chkSelect As CheckBox = TryCast(item.FindControl("CheckBox_select"), CheckBox)
            If chkSelect Is Nothing OrElse Not chkSelect.Visible OrElse Not chkSelect.Checked Then
                Continue For
            End If

            selectedCount += 1

            Dim farmCodeField = GetControlValue(Of HiddenField)(item, "HiddenField_farmCode")
            Dim farmCode As String = If(farmCodeField?.Value, String.Empty).Trim()
            If String.IsNullOrEmpty(farmCode) Then
                errors.Add("資料異常：缺少畜牧場證號。")
                Continue For
            End If

            Dim farmName As String = If(GetControlValue(Of TextBox)(item, "TextBox_farmName")?.Text, String.Empty).Trim()
            Dim owner As String = If(GetControlValue(Of TextBox)(item, "TextBox_owner")?.Text, String.Empty).Trim()
            Dim ownerTel As String = If(GetControlValue(Of TextBox)(item, "TextBox_ownerTel")?.Text, String.Empty).Trim()
            Dim address As String = If(GetControlValue(Of TextBox)(item, "TextBox_address")?.Text, String.Empty).Trim()
            Dim animalCount As String = If(GetControlValue(Of TextBox)(item, "TextBox_animalCount")?.Text, String.Empty).Trim()
            Dim memo As String = If(GetControlValue(Of TextBox)(item, "TextBox_memo")?.Text, String.Empty).Trim()
            Dim ddlCity As DropDownList = GetControlValue(Of DropDownList)(item, "DropDownList_city")
            Dim ddlArea As DropDownList = GetControlValue(Of DropDownList)(item, "DropDownList_area")
            Dim dataSource As String = If(GetControlValue(Of HiddenField)(item, "HiddenField_dataSource")?.Value, String.Empty)

            If String.IsNullOrWhiteSpace(farmName) Then
                errors.Add($"{farmCode}：請輸入畜牧場名稱。")
                Continue For
            End If

            If String.IsNullOrWhiteSpace(owner) Then
                errors.Add($"{farmCode}：請輸入負責人。")
                Continue For
            End If

            If ddlCity Is Nothing OrElse String.IsNullOrEmpty(ddlCity.SelectedValue) Then
                errors.Add($"{farmCode}：請選擇縣市。")
                Continue For
            End If

            If ddlArea Is Nothing OrElse String.IsNullOrEmpty(ddlArea.SelectedValue) Then
                errors.Add($"{farmCode}：請選擇鄉鎮。")
                Continue For
            End If

            If String.IsNullOrWhiteSpace(address) Then
                errors.Add($"{farmCode}：請輸入地址。")
                Continue For
            End If

            If taifCattle_farm.Check_FarmCodeExists(farmCode) Then
                errors.Add($"{farmCode}：畜牧場已存在。")
                Continue For
            End If

            Dim info As New taifCattle.Farm.stru_farmInfo With {
                .farmName = farmName,
                .farmCode = farmCode,
                .owner = owner,
                .ownerID = String.Empty,
                .ownerTel = ownerTel,
                .twID = Convert.ToInt32(ddlArea.SelectedValue),
                .address = address,
                .animalCount = animalCount,
                .memo = memo,
                .insertType = taifCattle_farm.GetInsertTypeByDataSource(dataSource),
                .insertAccountID = operatorId,
                .updateAccountID = operatorId
            }

            Try
                taifCattle_farm.Insert_Farm(info)
                Insert_UserLog(operatorId, taifCattle.Base.enum_UserLogItem.牧場資料管理, taifCattle.Base.enum_UserLogType.新增, $"farmCode:{farmCode}")
                successCount += 1
            Catch ex As Exception
                errors.Add($"{farmCode}：寫入資料庫失敗 - {ex.Message}")
            End Try
        Next

        If selectedCount = 0 Then
            Label_message.CssClass = "text-danger fw-bold d-block mb-3"
            Label_message.Text = "請勾選要新增的畜牧場。"
            Return
        End If

        If errors.Count > 0 Then
            Dim encodedErrors = errors.Select(Function(err) Server.HtmlEncode(err))
            If successCount > 0 Then
                Label_message.Text = $"成功新增 {successCount} 筆畜牧場，但仍有資料未完成：<br/>{String.Join("<br/>", encodedErrors)}"
            Else
                Label_message.Text = String.Join("<br/>", encodedErrors)
            End If
            Label_message.CssClass = "text-danger fw-bold d-block mb-3"
        Else
            Label_message.Text = $"成功新增 {successCount} 筆畜牧場。"
            Label_message.CssClass = "text-success fw-bold d-block mb-3"
        End If

        If successCount > 0 Then
            LoadMissingFarmRecords()
        End If
    End Sub

    Private Function GetDataSourceDisplayName(dataSource As String) As String
        Select Case dataSource
            Case "CattleManage_Batch"
                Return "牛籍批次新增"
            Case "HisManage_Batch"
                Return "旅程批次新增"
            Case "HisEndManage_Batch"
                Return "除籍批次新增"
            Case Else
                Return dataSource
        End Select
    End Function

    Private Sub PopulateCityDropDown(ddlCity As DropDownList)
        If ddlCity Is Nothing Then
            Return
        End If

        taifCattle_con.BindDropDownList_city(ddlCity, False)
        ddlCity.Items.Insert(0, New ListItem("--請選擇--", ""))
    End Sub

    Private Sub PopulateAreaDropDown(ddlArea As DropDownList, cityID As String)
        If ddlArea Is Nothing Then
            Return
        End If

        If String.IsNullOrEmpty(cityID) Then
            ddlArea.Items.Clear()
            ddlArea.Items.Add(New ListItem("--請選擇--", ""))
        Else
            taifCattle_con.BindDropDownList_area(ddlArea, cityID, False)
            ddlArea.Items.Insert(0, New ListItem("--請選擇--", ""))
        End If
    End Sub

    Private Function GetControlValue(Of T As Control)(container As Control, controlId As String) As T
        Return TryCast(container.FindControl(controlId), T)
    End Function

    Private Sub UpdateFarmCardDisplayStates()
        For Each item As RepeaterItem In Repeater_missingFarms.Items
            Dim panelInputs As Panel = TryCast(item.FindControl("Panel_inputs"), Panel)
            If panelInputs Is Nothing Then
                Continue For
            End If

            Dim chkSelect As CheckBox = TryCast(item.FindControl("CheckBox_select"), CheckBox)
            Dim shouldShow As Boolean = True
            If chkSelect IsNot Nothing AndAlso chkSelect.Visible Then
                shouldShow = chkSelect.Checked
            End If

            Dim cssClass As String = FarmDetailsBaseCssClass
            If Not shouldShow Then
                cssClass &= FarmDetailsCollapsedCssSuffix
            End If

            panelInputs.CssClass = cssClass
        Next
    End Sub
    Private Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Label_message.Text <> String.Empty Then
            js.AppendLine("showModal();")
        End If
        Page.ClientScript.RegisterStartupScript(Me.Page.GetType(), "page_js", js.ToString(), True)
    End Sub
End Class
