Imports System.Data.Common
Imports System.Threading
Imports Databasic

Partial Public MustInherit Class Connection
	''' <summary>
	''' Threads semahore to read/write first level records from/into managed connections store.
	''' </summary>
	Private Shared _registerLock As Object = New Object

	''' <summary>
	''' Close and drop all connections for current process and thread.
	''' Call this method always at thread end.
	''' </summary>
	Public Shared Sub Close()
		Dim processAndThreadKey = Databasic.Connection._getProcessAndThreadKey()
		Dim processAndThreadConnections As Dictionary(Of Int32, Connection)
		Dim processAndThreadConnectionsKeys As Int32()
		Dim provider As DbConnection

		SyncLock Connection._registerLock
			If Connection._connectionsRegister.ContainsKey(processAndThreadKey) Then
				Try
					' store all connections for current process id and thread in local variable to close them
					processAndThreadConnections = Connection._connectionsRegister(processAndThreadKey)
					' lets change the register - remove records under thread and process key
					Connection._connectionsRegister.Remove(processAndThreadKey)
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
				Catch ex As Exception
					Databasic.Events.RaiseError(ex)
				End Try
			End If
		End SyncLock

	End Sub

	''' <summary>
	''' Close and drop connection by config index for current process and thread.
	''' Call this method always after you have loaded all from any secondary database.
	''' </summary>
	''' <param name="connectionIndex">Config connection index.</param>
	Public Shared Sub Close(connectionIndex As Int32)
		Databasic.Connection.Close(
			connectionIndex, Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId
		)
	End Sub

	''' <summary>
	''' Close and drop connection by config name for current process And thread.
	''' Call this method always after you have loaded all from any secondary database.
	''' </summary>
	''' <param name="connectionName">Config connection name.</param>
	Public Shared Sub Close(connectionName As String)
		If Databasic.Connection.NamesAndIndexes.ContainsKey(connectionName) Then
			Databasic.Connection.Close(
				Databasic.Connection.NamesAndIndexes(connectionName),
				Process.GetCurrentProcess().Id,
				Thread.CurrentThread.ManagedThreadId)
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
		Dim provider As DbConnection

		SyncLock Connection._registerLock
			If Connection._connectionsRegister.ContainsKey(processAndThreadKey) Then
				processAndThreadConnections = Connection._connectionsRegister(processAndThreadKey)
				If processAndThreadConnections.ContainsKey(connectionIndex) Then
					' connection record under process and thread key contains connection under called index
					If processAndThreadConnections.Count = 1 Then
						' connection is only one - remove whole process and thread record from registry
						' remove and close connection under called index
						provider = processAndThreadConnections(connectionIndex).Provider
						processAndThreadConnections.Remove(connectionIndex)
						If provider.State = ConnectionState.Open Then
							provider.Close()
							provider.Dispose()
						End If
						' remove connection records with one connection under thread and process key
						Connection._connectionsRegister.Remove(processAndThreadKey)
					Else
						' there are still more connections - remove only one connection from process and thread id record
						' B. read check end - if connection record under process and thread key contains connection under called index
						' remove and close connection under called index
						provider = processAndThreadConnections(connectionIndex).Provider
						processAndThreadConnections.Remove(connectionIndex)
						If provider.State = ConnectionState.Open Then
							provider.Close()
							provider.Dispose()
						End If
					End If
				End If
			End If
		End SyncLock
	End Sub

	''' <summary>
	''' Close and drop connections for specificly called process.
	''' </summary>
	Public Shared Sub CloseAllInProcess(processId As Int32)
		Dim keyBegin As String = processId.ToString() + "_"
		Dim key As String
		Dim conns As Dictionary(Of Int32, Connection)
		Dim provider As DbConnection

		SyncLock Connection._registerLock
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
				End If
			Next
		End SyncLock
	End Sub

End Class
