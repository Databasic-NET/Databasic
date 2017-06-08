Imports System.Configuration
Imports System.Data.Common
Imports System.Reflection
Imports System.Threading
Imports Databasic
Imports Databasic.Connections

Partial Public MustInherit Class Connection







    ''' <summary>
    ''' Threads semahore to read/write into managed connections store.
    ''' </summary>
    Private Shared _closingLock As New Object()



    ''' <summary>
    ''' Close and drop all connections for current process and thread.
    ''' Call this method always at thread end.
    ''' </summary>
    Public Shared Sub Close()
        Dim processAndThreadKey = Databasic.Connection._getProcessAndThreadKey()
        Dim client As DbConnection
        If Databasic.Connection._connections.ContainsKey(processAndThreadKey) Then
            For Each item In Databasic.Connection._connections(processAndThreadKey)
                client = item.Value.Provider
                If client.State = ConnectionState.Open Then
                    client.Close()
                    client.Dispose()
                End If
            Next
            SyncLock Databasic.Connection._closingLock
                Databasic.Connection._connections.Remove(processAndThreadKey)
            End SyncLock
        End If
    End Sub
    ''' <summary>
    ''' Close and drop connection by config index for current process and thread.
    ''' Call this method always after you have loaded all from any secondary database.
    ''' </summary>
    ''' <param name="connectionIndex">Config connection index.</param>
    Public Shared Sub Close(connectionIndex As Int32)
        Databasic.Connection.Close(connectionIndex, Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId)
    End Sub
    ''' <summary>
    ''' Close and drop connection by config name for current process And thread.
    ''' Call this method always after you have loaded all from any secondary database.
    ''' </summary>
    ''' <param name="connectionName">Config connection name.</param>
    Public Shared Sub Close(connectionName As String)
        If Databasic.Connection.NamesAndIndexes.ContainsKey(connectionName) Then
            Databasic.Connection.Close(Databasic.Connection.NamesAndIndexes(connectionName), Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId)
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
        Dim conns As Dictionary(Of Int16, Connection)
        Dim provider As DbConnection
        If Databasic.Connection._connections.ContainsKey(processAndThreadKey) Then
            conns = Databasic.Connection._connections(processAndThreadKey)
            provider = conns(connectionIndex).Provider
            If provider.State = ConnectionState.Open Then
                provider.Close()
                provider.Dispose()
            End If
            SyncLock Databasic.Connection._closingLock
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
        Dim conns As Dictionary(Of Int16, Connection)
        Dim provider As DbConnection
        SyncLock Databasic.Connection._closingLock
            For i As Int32 = Databasic.Connection._connections.Keys.Count - 1 To 0 Step -1
                If Databasic.Connection._connections.Keys.Contains(i) Then
                    key = Databasic.Connection._connections.Keys(i)
                Else
                    Continue For
                End If
                If key.IndexOf(keyBegin) = 0 Then
                    conns = Databasic.Connection._connections(key)
                    For Each item In conns
                        provider = item.Value.Provider
                        If provider.State = ConnectionState.Open Then
                            provider.Close()
                            provider.Dispose()
                        End If
                    Next
                    Databasic.Connection._connections.Remove(key)
                End If
            Next
        End SyncLock
    End Sub


End Class
