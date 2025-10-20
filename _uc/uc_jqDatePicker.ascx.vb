Imports System.ComponentModel

Public Class uc_jqDatePicker
    Inherits System.Web.UI.UserControl
#Region "自訂屬性"
    <[Category]("自訂屬性")>
    Public Property AutoPostBack As Boolean = False

    Public Property CssClass As String
        Get
            Return If(ViewState("CssClass"), String.Empty)
        End Get
        Set(value As String)
            ViewState("CssClass") = value
        End Set
    End Property
#End Region
#Region "Function // Sub"

    ''' <summary>
    ''' 取得控制項日期
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDate() As Date
        If IsDate(TextBox_date.Text.Trim) Then
            Return CDate(TextBox_date.Text.Trim)
        Else
            Return New Date(1900, 1, 1)
        End If
    End Function

    ''' <summary>
    ''' 設定時間
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Value() As Date?
        Get
            If IsDate(TextBox_date.Text.Trim) Then
                Return CDate(TextBox_date.Text.Trim).ToString("yyyy/MM/dd")
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal inDate As Date?)
            If inDate.HasValue Then
                TextBox_date.Text = CDate(inDate).ToString("yyyy/MM/dd")
            Else
                TextBox_date.Text = ""
            End If
        End Set
    End Property

    ''' <summary>
    ''' 設定Enable狀態
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Enable() As Boolean
        Get
            Return TextBox_date.Enabled
        End Get
        Set(value As Boolean)
            TextBox_date.Enabled = value
        End Set
    End Property

#End Region

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack = False Then
            'TextBox_date.Text = Today.ToString("yyyy/MM/dd")
        End If
        TextBox_date.Attributes.Add("readonly", "")
    End Sub

    Public Event DateChanged(ByVal NewDate As Date)

    Protected Sub Button_ValueChanged_Click(sender As Object, e As EventArgs) Handles Button_ValueChanged.Click
        RaiseEvent DateChanged(Me.Value)
    End Sub

    Protected Overrides Sub OnPreRender(e As EventArgs)
        MyBase.OnPreRender(e)

        If Not String.IsNullOrEmpty(Me.CssClass) Then
            TextBox_date.CssClass = $"{TextBox_date.CssClass} {Me.CssClass}".Trim()
        End If
    End Sub
End Class