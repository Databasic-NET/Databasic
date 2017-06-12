Imports System.Data.Common
Imports System.Threading
Imports Databasic

Partial Public MustInherit Class Connection

    ''' <summary>
    ''' Threads semahore to read/write first level records from/into managed connections store.
    ''' </summary>
    Private Shared _registerLock As New ReaderWriterLockSlim()

    ''' <summary>
    ''' Threads semahores to read/write second level records from/into managed connections store.
    ''' </summary>
    Private Shared _registerLocks As New Dictionary(Of String, ReaderWriterLockSlim)

    ''' <summary>
    ''' Close and drop all connections for current process and thread.
    ''' Call this method always at thread end.
    ''' </summary>
    Public Shared Sub Close()
        Dim processAndThreadKey = Databasic.Connection._getProcessAndThreadKey()
        Dim processAndThreadConnections As Dictionary(Of Int32, Connection)
        Dim processAndThreadConnectionsKeys As Int32()
        Dim provider As DbConnection
        ' A. read check begin - if register contains any connection records under process and thread key
        Connection._registerLock.EnterUpgradeableReadLock()
        ' check if register contains any connection records under process and thread key
        If Connection._connectionsRegister.ContainsKey(processAndThreadKey) Then
            ' B. write lock begin - to change register records under process and thread key
            Connection._registerLock.EnterWriteLock()
            ' A. read check end - if register contains any connection records under process and thread key
            Connection._registerLock.ExitUpgradeableReadLock()
            ' store all connections for current process id and thread in local variable to close them
            processAndThreadConnections = Connection._connectionsRegister(processAndThreadKey)
            ' lets change the register - remove records under thread and process key
            Connection._connectionsRegister.Remove(processAndThreadKey)
            Connection._registerLocks.Remove(processAndThreadKey)
            ' close all connections for current process na thread only through local variable
            processAndThreadConnectionsKeys = DirectCast(processAndThreadConnections.Keys.ToArray().Clone(), Int32())
            For Each key As Int32 In processAndThreadConnectionsKeys
                provider = processAndThreadConnections(key).Provider
                processAndThreadConnections.Remove(key)
                If provider.State = ConnectionState.Open Then
                    provider.Close()
                    provider.Dispose()
                End If
            Next
            ' B. write lock end - to change register records under process and thread key
            Connection._registerLock.ExitWriteLock()
        Else
            ' A. read check end - if register contains any connection records under process and thread key
            Connection._registerLock.ExitUpgradeableReadLock()
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
        Dim processAndThreadConnections As Dictionary(Of Int32, Connection)
        Dim processAndThreadLock As ReaderWriterLockSlim
        Dim provider As DbConnection
        ' A. read check begin - if register contains any connection records under process and thread key
        Connection._registerLock.EnterUpgradeableReadLock()
        If Connection._connectionsRegister.ContainsKey(processAndThreadKey) Then
            ' store connections and lock for process and thread in local variables
            processAndThreadConnections = Connection._connectionsRegister(processAndThreadKey)
            processAndThreadLock = Connection._registerLocks(processAndThreadKey)
            ' B. read check begin - if connection record under process and thread key contains connection under called index
            processAndThreadLock.EnterUpgradeableReadLock()
            ' check if connection record under process and thread key contains connection under called index
            If processAndThreadConnections.ContainsKey(connectionIndex) Then
                ' connection record under process and thread key contains connection under called index
                If processAndThreadConnections.Count = 1 Then
                    ' connection is only one - remove whole process and thread record from registry
                    ' D. write lock begin - remove connection records with one connection under thread and process key
                    Connection._registerLock.EnterWriteLock()
                    ' B. read check end - if connection record under process and thread key contains connection under called index
                    processAndThreadLock.ExitUpgradeableReadLock()
                    ' A. read check end - if register contains any connection records under process and thread key
                    Connection._registerLock.ExitUpgradeableReadLock()
                    ' remove and close connection under called index
                    provider = processAndThreadConnections(connectionIndex).Provider
                    processAndThreadConnections.Remove(connectionIndex)
                    If provider.State = ConnectionState.Open Then
                        provider.Close()
                        provider.Dispose()
                    End If
                    ' remove connection records with one connection under thread and process key
                    Connection._connectionsRegister.Remove(processAndThreadKey)
                    Connection._registerLocks.Remove(processAndThreadKey)
                    ' D. write lock end - remove connection records with one connection under thread and process key
                    Connection._registerLock.ExitWriteLock()
                Else
                    ' there are still more connections - remove only one connection from process and thread id record
                    ' C. write lock begin - remove connection under called index in connections record under thread and process key
                    processAndThreadLock.EnterWriteLock()
                    ' B. read check end - if connection record under process and thread key contains connection under called index
                    processAndThreadLock.ExitUpgradeableReadLock()
                    ' remove and close connection under called index
                    provider = processAndThreadConnections(connectionIndex).Provider
                    processAndThreadConnections.Remove(connectionIndex)
                    If provider.State = ConnectionState.Open Then
                        provider.Close()
                        provider.Dispose()
                    End If
                    ' C. write lock begin - remove connection under called index in connections record under thread and process key
                    processAndThreadLock.ExitWriteLock()
                End If
            Else
                ' process and thread record doesn't contain any connection under called index - no change
                ' B. read check end - if connection record under process and thread key contains connection under called index
                processAndThreadLock.ExitUpgradeableReadLock()
                ' A. read check end - if register contains any connection records under process and thread key
                Connection._registerLock.ExitUpgradeableReadLock()
            End If
        Else
            ' A. read check end - if register contains any connection records under process and thread key
            Connection._registerLock.ExitUpgradeableReadLock()
        End If
    End Sub

    ''' <summary>
    ''' Close and drop connections for specificly called process.
    ''' </summary>
    Public Shared Sub CloseAllInProcess(processId As Int32)
        Dim keyBegin As String = processId.ToString() + "_"
        Dim key As String
        Dim conns As Dictionary(Of Int32, Connection)
		Dim provider As DbConnection
		Connection._registerLock.EnterWriteLock()
		For i As Int32 = Connection._connectionsRegister.Keys.Count - 1 To 0 Step -1
			If Connection._connectionsRegister.Keys.Contains(i) Then
				key = Connection._connectionsRegister.Keys(i)
			Else
				Continue For
			End If
			If key.IndexOf(keyBegin) = 0 Then
				conns = Connection._connectionsRegister(key)
				For Each item In conns
					provider = item.Value.Provider
					If provider.State = ConnectionState.Open Then
						provider.Close()
						provider.Dispose()
					End If
				Next
				Connection._connectionsRegister.Remove(key)
				Connection._registerLocks.Remove(key)
			End If
		Next
		Connection._registerLock.ExitWriteLock()
	End Sub

End Class
