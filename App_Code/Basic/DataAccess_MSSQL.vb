Imports System.Data

Namespace DataAccess
    ''' <summary>
    ''' DB 處理
    ''' </summary>
    ''' <remarks>最後版本：2013-06-14</remarks>
    Public Class MS_SQL
        Implements IDisposable
        Private connStrName As String '連線字串的名稱    
        Private _conn As System.Data.SqlClient.SqlConnection
        Private _cmd As New System.Data.SqlClient.SqlCommand
        Dim intResult As Integer
        Dim myTransaction As Data.SqlClient.SqlTransaction
        Private _useTrans As Boolean = False
        Private _DefaultConnStrName As String = "SQL_Connection"
        ''' <summary>
        ''' 傳回開啟中的 Connection
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property connection() As Data.SqlClient.SqlConnection
            Get
                Me.OpenConnection()
                Return Me._conn
            End Get
        End Property

        Sub New()
            Dim ConnectionString As String = System.Configuration.ConfigurationManager.ConnectionStrings(Me._DefaultConnStrName).ConnectionString
            _conn = New System.Data.SqlClient.SqlConnection(ConnectionString)
            _cmd.Connection = Me._conn
        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ConnectStringName">連線名稱</param>
        ''' <remarks></remarks>
        Sub New(ByVal ConnectStringName As String)
            Dim ConnectionString As String = System.Configuration.ConfigurationManager.ConnectionStrings(ConnectStringName).ConnectionString
            _conn = New System.Data.SqlClient.SqlConnection(ConnectionString)
            _cmd.Connection = Me._conn
        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ConnectStringName">連線名稱</param>
        ''' <param name="CustomConnectString">自訂連線字串</param>
        ''' <remarks></remarks>
        Sub New(ByVal ConnectStringName As String, ByVal CustomConnectString As String)
            _conn = New System.Data.SqlClient.SqlConnection(CustomConnectString)
            _cmd.Connection = Me._conn
        End Sub
        ''' <summary>
        ''' 啟用 Transaction
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub StartTransaction()
            If (Me.myTransaction Is Nothing) Then
                Me.OpenConnection()
                Me.myTransaction = Me._conn.BeginTransaction
                Me._useTrans = True
                Me._cmd.Transaction = Me.myTransaction

            End If
        End Sub

        Public Sub Commit()
            Me.myTransaction.Commit()
        End Sub

        Public Sub RollBack()
            Me.myTransaction.Rollback()
        End Sub
        ''' <summary>
        ''' 取得 Connection
        ''' </summary>
        ''' <remarks></remarks>
        Private Function GetConnection() As SqlClient.SqlConnection
            Return Me._conn
        End Function

        ''' <summary>
        ''' 開啟 Connection
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub OpenConnection()
            If (Me._conn.State = ConnectionState.Closed) Then Me._conn.Open()
        End Sub

        ''' <summary>
        ''' 關閉 Connection
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CloseConnection()
            Me._conn.Close()
        End Sub

        ''' <summary>
        ''' 執行 Sql Command
        ''' </summary>
        ''' <param name="sqlCommand"></param>
        ''' <param name="parament"></param>
        ''' <remarks></remarks>
        Public Function ExecNonQuery(ByVal sqlCommand As String, ByVal ParamArray parament() As SqlClient.SqlParameter) As Integer
            Dim cmd As System.Data.SqlClient.SqlCommand = Me.GetSqlCommand(sqlCommand, parament)
            Return cmd.ExecuteNonQuery()
        End Function

        ''' <summary>
        ''' 取得第一行第一列資料
        ''' </summary>
        ''' <param name="sqlCommand"></param>
        ''' <param name="parament"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteScalar(ByVal sqlCommand As String, ByVal ParamArray parament() As SqlClient.SqlParameter) As Object
            Dim cmd As System.Data.SqlClient.SqlCommand = Me.GetSqlCommand(sqlCommand, parament)
            Return cmd.ExecuteScalar()
        End Function

        ''' <summary>
        ''' 取得 Sql Command
        ''' </summary>
        ''' <param name="sqlCommand"></param>
        ''' <param name="parament"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetSqlCommand(ByVal sqlCommand As String, ByVal ParamArray parament() As SqlClient.SqlParameter) As SqlClient.SqlCommand
            Me.OpenConnection()
            Me._cmd.CommandText = sqlCommand
            Me._cmd.Parameters.Clear()
            If (parament.Length > 0) Then
                Me._cmd.Parameters.AddRange(parament)
            End If
            Return Me._cmd
        End Function

        ''' <summary>
        ''' 取得 DataTable
        ''' </summary>
        ''' <param name="sqlCommand"></param>
        ''' <param name="parament"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDataTable(ByVal sqlCommand As String, ByVal ParamArray parament() As SqlClient.SqlParameter) As Data.DataTable
            Dim cmd As SqlClient.SqlCommand = Me.GetSqlCommand(sqlCommand, parament)
            Dim da As New SqlClient.SqlDataAdapter(cmd)
            Dim dt As New Data.DataTable
            da.Fill(dt)
            Return dt
        End Function

        ''' <summary>
        ''' 取得 DataView
        ''' </summary>
        ''' <param name="sqlCommand"></param>
        ''' <param name="parament"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDataView(ByVal sqlCommand As String, ByVal ParamArray parament() As SqlClient.SqlParameter) As Data.DataView
            Dim dt As Data.DataTable = Me.GetDataTable(sqlCommand, parament)
            Return New Data.DataView(dt)
        End Function

        ''' <summary>
        ''' 大量工作
        ''' </summary>
        ''' <param name="dt"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BulkCopy(ByVal tableName As String, ByVal dt As Data.DataTable) As Boolean
            Me.OpenConnection()
            Using bulk = New System.Data.SqlClient.SqlBulkCopy(Me._conn)
                With bulk
                    .BatchSize = 1000
                    .DestinationTableName = tableName
                    For i As Integer = 0 To dt.Columns.Count - 1
                        .ColumnMappings.Add(dt.Columns(i).ColumnName, dt.Columns(i).ColumnName)
                    Next
                    .WriteToServer(dt)
                End With
            End Using
        End Function

        ''' <summary>
        ''' 釋放物件
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Dispose() Implements IDisposable.Dispose
            Me._cmd.Dispose()
            Me._conn.Close()
            Me._conn.Dispose()
        End Sub
    End Class
End Namespace
