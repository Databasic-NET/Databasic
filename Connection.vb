Imports Desharp
Imports System.Configuration
Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient
Imports System.Data.Common
Imports System.Reflection
Imports System.Threading

Public Class Connection
    Protected Shared supportedProviders As New List(Of String) From {
        "System.Data.SqlClient",
        "MySql.Data.MySqlClient"
    }
    Protected Shared config As New Dictionary(Of Int16, String())
    Protected Shared namesAndIndexes As New Dictionary(Of String, Int32)
    Protected Shared connections As New Dictionary(Of String, Dictionary(Of Int16, DbConnection))
    Protected Shared threadLock As New Object()
    ''' <summary>
    ''' Load config and set up connection strings.
    ''' </summary>
    Shared Sub New()
        If (Connection.config.Count = 0) Then
            Dim config As ConnectionStringsSection = DirectCast(
                ConfigurationSettings.GetConfig("connectionStrings"),
                ConnectionStringsSection
            )
            Dim i As Int16 = 0
            For Each cfgItem As ConnectionStringSettings In config.ConnectionStrings
                If Not Connection.supportedProviders.Contains(cfgItem.ProviderName) Then
                    Throw New Exception($"This DB engine is not supperted: '{cfgItem.ProviderName}'.")
                End If
                Connection.config.Add(
                    i, New String() {cfgItem.ProviderName, cfgItem.ConnectionString}
                )
                Connection.namesAndIndexes.Add(cfgItem.Name, i)
                i += 1
            Next
        End If
    End Sub
    ''' <summary>
    ''' Get and open connection by config index.
    ''' </summary>
    ''' <param name="connectionIndex">Config connection index.</param>
    ''' <returns></returns>
    Public Shared Function [Get](connectionIndex As Int32) As DbConnection
        If Not Connection.config.ContainsKey(connectionIndex) Then
            Throw New Exception($"Connection settings under index doesn't exist: {connectionIndex}.")
        End If
        Dim conn As DbConnection
        Dim processAndThreadKey As String = Connection.getProcessAndThreadKey()
        If (
            Connection.connections.ContainsKey(processAndThreadKey) AndAlso
            Connection.connections(processAndThreadKey).ContainsKey(connectionIndex)
        ) Then
            conn = Connection.connections(processAndThreadKey).Item(connectionIndex)
        Else
            Dim typeAndDsn As String() = Connection.config(connectionIndex)
            If typeAndDsn(0) = "System.Data.SqlClient" Then
                conn = New SqlConnection(typeAndDsn(1))
            ElseIf typeAndDsn(0) = "MySql.Data.MySqlClient" Then
                conn = New MySqlConnection(typeAndDsn(1))
            End If
            Connection.register(processAndThreadKey, connectionIndex, conn)
            Try
                conn.Open()
                Connection.addErrorHandler(conn)
            Catch ex As Exception
                Debug.Log(ex)
            End Try
        End If
        Return conn
    End Function
    ''' <summary>
    ''' Get and open connection by config name.
    ''' </summary>
    ''' <param name="connectionName">Config connection name.</param>
    ''' <returns></returns>
    Public Shared Function [Get](connectionName As String) As DbConnection
        Dim connIndex As Int32
        If Connection.namesAndIndexes.ContainsKey(connectionName) Then
            connIndex = Connection.namesAndIndexes(connectionName)
        Else
            Throw New Exception($"Connection settings under name doesn't exist: {connectionName}.")
        End If
        Return Connection.Get(connIndex)
    End Function
    ''' <summary>
    ''' Close and drop all connections for current process and thread.
    ''' Call this method always at thread end.
    ''' </summary>
    Public Shared Sub Close()
        Dim processAndThreadKey = Connection.getProcessAndThreadKey()
        Dim conn As DbConnection
        If Connection.connections.ContainsKey(processAndThreadKey) Then
            For Each item In Connection.connections(processAndThreadKey)
                conn = item.Value
                If conn.State = ConnectionState.Open Then
                    conn.Close()
                    conn.Dispose()
                End If
            Next
            SyncLock Connection.threadLock
                Connection.connections.Remove(processAndThreadKey)
            End SyncLock
        End If
    End Sub
    ''' <summary>
    ''' Close and drop connection by config index for current process and thread.
    ''' Call this method always after you have loaded all from any secondary database.
    ''' </summary>
    ''' <param name="connectionIndex">Config connection index.</param>
    Public Shared Sub Close(connectionIndex As Int32)
        Connection.Close(connectionIndex, Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId)
    End Sub
    ''' <summary>
    ''' Close and drop connection by config name for current process And thread.
    ''' Call this method always after you have loaded all from any secondary database.
    ''' </summary>
    ''' <param name="connectionName">Config connection name.</param>
    Public Shared Sub Close(connectionName As String)
        If Connection.namesAndIndexes.ContainsKey(connectionName) Then
            Connection.Close(Connection.namesAndIndexes(connectionName), Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId)
        End If
    End Sub
    ''' <summary>
    ''' Close and drop connection by config index for specificly called process and thread.
    ''' Call this method only if you know what you are doing:-)
    ''' </summary>
    ''' <param name="connectionIndex">Config connection index.</param>
    ''' <param name="processId">Specific process id.</param>
    ''' <param name="threadId">Specific thread id.</param>
    Public Shared Sub Close(connectionIndex As Int32, processId As Int32, threadId As Int32)
        Dim processAndThreadKey = $"{processId}_{threadId}"
        Dim conns As Dictionary(Of Int16, DbConnection)
        Dim conn As DbConnection
        If Connection.connections.ContainsKey(processAndThreadKey) Then
            conns = Connection.connections(processAndThreadKey)
            conn = conns(connectionIndex)
            If conn.State = ConnectionState.Open Then
                conn.Close()
                conn.Dispose()
            End If
            SyncLock Connection.threadLock
                conns.Remove(connectionIndex)
                If conns.Count = 0 Then
                    conns.Remove(processAndThreadKey)
                End If
            End SyncLock
        End If
    End Sub
    ''' <summary>
    ''' Close and drop connections for specificly called process.
    ''' </summary>
    Public Shared Sub CloseAllInProcess(processId As Int32)
        Dim keyBegin As String = processId.ToString() + "_"
        Dim key As String
        Dim conns As Dictionary(Of Int16, DbConnection)
        SyncLock Connection.threadLock
            For i As Int32 = Connection.connections.Keys.Count - 1 To 0 Step -1
                If Connection.connections.Keys.Contains(i) Then
                    key = Connection.connections.Keys(i)
                Else
                    Continue For
                End If
                If key.IndexOf(keyBegin) = 0 Then
                    conns = Connection.connections(key)
                    For Each item In conns
                        If item.Value.State = ConnectionState.Open Then
                            item.Value.Close()
                            item.Value.Dispose()
                        End If
                    Next
                    Connection.connections.Remove(key)
                End If
            Next
        End SyncLock
    End Sub
    ''' <summary>
    ''' Create and begin transaction on first config connection.
    ''' </summary>
    ''' <param name="transactionName">Transaction name.</param>
    ''' <param name="isolationLevel">Transaction isolation level.</param>
    ''' <returns>New transaction.</returns>
    Public Shared Function BeginTransaction(Optional transactionName As String = "", Optional isolationLevel As IsolationLevel = IsolationLevel.Unspecified) As DbTransaction
        Dim defaultConnectionIndex As Int16 = 0
        Return Connection.BeginTransaction(defaultConnectionIndex, transactionName, isolationLevel)
    End Function
    ''' <summary>
    ''' Create and begin transaction on specified connection config index.
    ''' </summary>
    ''' <param name="connectionIndex">Config connection index.</param>
    ''' <param name="transactionName">Transaction name.</param>
    ''' <param name="isolationLevel">Transaction isolation level.</param>
    ''' <returns>New transaction.</returns>
    Public Shared Function BeginTransaction(connectionIndex As Int16, Optional transactionName As String = "", Optional isolationLevel As IsolationLevel = IsolationLevel.Unspecified) As DbTransaction
        Dim conn As DbConnection = Connection.Get(connectionIndex)
        Dim connType As Type = conn.GetType()
        If connType.IsAssignableFrom(GetType(SqlConnection)) Then
            Dim msConn As SqlConnection = DirectCast(conn, SqlConnection)
            Return msConn.BeginTransaction(isolationLevel, transactionName)
        ElseIf connType.IsAssignableFrom(GetType(MySqlConnection)) Then
            Dim myConn As MySqlConnection = DirectCast(conn, MySqlConnection)
            Return myConn.BeginTransaction(isolationLevel)
        End If
        Return Nothing
    End Function
    ''' <summary>
    ''' Create and begin transaction on specified connection config name.
    ''' </summary>
    ''' <param name="connectionName">Config connection name.</param>
    ''' <param name="transactionName">Transaction name.</param>
    ''' <param name="isolationLevel">Transaction isolation level.</param>
    ''' <returns>New transaction.</returns>
    Public Shared Function BeginTransaction(connectionName As String, Optional transactionName As String = "", Optional isolationLevel As IsolationLevel = IsolationLevel.Unspecified) As DbTransaction
        Dim connectionIndex As Int16
        If Connection.namesAndIndexes.ContainsKey(connectionName) Then
            connectionIndex = Connection.namesAndIndexes(connectionName)
        Else
            Throw New Exception($"Connection settings under name doesn't exist: {connectionName}.")
        End If
        Return Connection.BeginTransaction(connectionIndex, transactionName, isolationLevel)
    End Function
    ''' <summary>
    ''' Add InfoMessage handler for opened connection to to log SQL errors.
    ''' </summary>
    ''' <param name="conn"></param>
    Protected Shared Sub addErrorHandler(conn As DbConnection)
        Dim connType As Type = conn.GetType()
        If connType.IsAssignableFrom(GetType(SqlConnection)) Then
            Dim msConn As SqlConnection = DirectCast(conn, SqlConnection)
            AddHandler msConn.InfoMessage, AddressOf Connection.errorHandler
        ElseIf connType.IsAssignableFrom(GetType(MySqlConnection)) Then
            Dim myConn As MySqlConnection = DirectCast(conn, MySqlConnection)
            AddHandler myConn.InfoMessage, AddressOf Connection.errorHandler
        End If
    End Sub
    Protected Shared Sub errorHandler(sender As Object, sqlErrorArgs As EventArgs)
        Dim fi As FieldInfo = sqlErrorArgs.GetType().GetField("exception", BindingFlags.NonPublic)
        Dim exception = fi.GetValue(sqlErrorArgs)
        If Debug.Enabled() Then
            Debug.Dump(exception)
        Else
            Debug.Log(exception)
        End If
    End Sub

    Protected Shared Sub register(processAndThreadKey As String, connectionIndex As Int32, conn As DbConnection)
        Dim conns As Dictionary(Of Int16, DbConnection)
        SyncLock Connection.threadLock
            If Connection.connections.ContainsKey(processAndThreadKey) Then
                conns = Connection.connections(processAndThreadKey)
                conns(connectionIndex) = conn
            Else
                Connection.connections.Add(processAndThreadKey, New Dictionary(Of Short, DbConnection) From {
                    {connectionIndex, conn}
                })
            End If
        End SyncLock
    End Sub

    Protected Shared Function getProcessAndThreadKey() As String
        Return $"{Process.GetCurrentProcess().Id}_{Thread.CurrentThread.ManagedThreadId}"
    End Function

End Class
