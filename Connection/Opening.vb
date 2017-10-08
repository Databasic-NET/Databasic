Imports System.Threading

Partial Public MustInherit Class Connection

	''' <summary>
	''' Create and open new connection and set it into Provider property, register InfoMessage handler.
	''' This procedure is not necessary to call manualy, it's automaticly called internaly by first <c>Connection.Get()</c> call.
	''' </summary>
	''' <param name="dsn">Database dsn connection string, usually with server address, username and password.</param>
	Public MustOverride Sub Open(dsn As String)

	Protected Sub New()
		Me.OpenedTransaction = Nothing
		Me._connectionIndex = 0
	End Sub

	''' <summary>
	''' Get (and open if necessary) registered connection instance by config index, internaly by process id and thread id.
	''' </summary>
	''' <param name="connectionIndex">Config connection index.</param>
	''' <returns>New opened or existing connection instance with registered InfoMessage handler, registered in internal store.</returns>
	Public Shared Function [Get](Optional connectionIndex As Int32? = Nothing) As Connection
		If Not connectionIndex.HasValue Then
			connectionIndex = Tools.GetConnectionIndexByClassAttr(Tools.GetEntryClassType, True)
		End If
		If Not Databasic.Connection.Config.ContainsKey(connectionIndex.Value) Then
			Events.RaiseError(New Exception($"Connection settings under index doesn't exist: {connectionIndex.Value}."))
		End If
		Dim connection As Databasic.Connection = Nothing
		Dim processAndThreadKey As String = Databasic.Connection._getProcessAndThreadKey()
		Dim processAndThreadConnections As Dictionary(Of Int32, Connection)

		SyncLock Databasic.Connection._registerLock
			If Databasic.Connection._connectionsRegister.ContainsKey(processAndThreadKey) Then
				processAndThreadConnections = Databasic.Connection._connectionsRegister(processAndThreadKey)
			Else
				processAndThreadConnections = New Dictionary(Of Int32, Connection)()
				Databasic.Connection._connectionsRegister.Add(processAndThreadKey, processAndThreadConnections)
			End If
			If processAndThreadConnections.ContainsKey(connectionIndex.Value) Then
				connection = processAndThreadConnections(connectionIndex.Value)
			Else
				connection = Databasic.Connection._createAndOpen(connectionIndex.Value)
				processAndThreadConnections.Add(connectionIndex.Value, connection)
			End If
		End SyncLock

		Return connection
	End Function

	''' <summary>
	''' Get and open connection by config name.
	''' </summary>
	''' <param name="connectionName">Config connection name.</param>
	''' <returns></returns>
	Public Shared Function [Get](connectionName As String) As Connection
		Return Databasic.Connection.Get(Connection.GetIndexByName(connectionName))
	End Function

	''' <summary>
	''' Create new connection instance by connection index and returns it
	''' </summary>
	''' <param name="connectionIndex">Config connection index.</param>
	''' <returns>New opened connection instance with registered InfoMessage handler.</returns>
	Private Shared Function _createAndOpen(connectionIndex As Int32) As Connection
		Dim result As Connection
		Dim typeAndDsn As String() = Databasic.Connection.Config(connectionIndex)
		Dim provider As String = typeAndDsn(0).Trim()
		If Not Databasic.Connection._supportedProviders.ContainsKey(provider) Then
			Events.RaiseError(New Exception($"Connection provider not installed: '{provider}'."))
		End If
		Dim connectionType As Type = Databasic.Connection._supportedProviders(provider)
		result = Activator.CreateInstance(connectionType)
		Try
			result.Open(typeAndDsn(1).Trim())
			result._connectionIndex = connectionIndex
		Catch ex As Exception
			Databasic.Events.RaiseError(ex)
		End Try
		Return result
	End Function

End Class
