Imports System.Threading

Partial Public MustInherit Class Connection

	''' <summary>
	''' Create and open new connection and set it into Provider property, register InfoMessage handler.
	''' This procedure is not necessary to call manualy, it's automaticly called internaly by first <c>Connection.Get()</c> call.
	''' </summary>
	''' <param name="dsn">Database dsn connection string, usually with server address, username and password.</param>
	Public MustOverride Sub Open(dsn As String)

    ''' <summary>
    ''' Get (and open if necessary) registered connection instance by config index, internaly by process id and thread id.
    ''' </summary>
    ''' <param name="connectionIndex">Config connection index.</param>
    ''' <returns>New opened or existing connection instance with registered InfoMessage handler, registered in internal store.</returns>
    Public Shared Function [Get](Optional connectionIndex As Int32 = Databasic.Defaults.CONNECTION_INDEX) As Connection
        If Not Databasic.Connection.Config.ContainsKey(connectionIndex) Then
            Events.RaiseError(New Exception($"Connection settings under index doesn't exist: {connectionIndex}."))
        End If
		Dim connection As Databasic.Connection = Nothing
		Dim processAndThreadKey As String = Databasic.Connection._getProcessAndThreadKey()
        Dim processAndThreadLock As ReaderWriterLockSlim
        Dim processAndThreadConnections As Dictionary(Of Int32, Connection)
        ' A. read check begin - if register contains any connection records under process and thread key
        Databasic.Connection._registerLock.EnterUpgradeableReadLock()
        ' check if register contains any connection records under process and thread key
        If Databasic.Connection._connectionsRegister.ContainsKey(processAndThreadKey) Then
            ' register contains any connection records under process and thread key - fill local variables to use them later
            processAndThreadLock = Databasic.Connection._registerLocks(processAndThreadKey)
            processAndThreadConnections = Databasic.Connection._connectionsRegister(processAndThreadKey)

            ' C. read check begin - if process and thread record contains connection under called index
            processAndThreadLock.EnterUpgradeableReadLock()
            ' check if process and thread record contains connection under called index
            If processAndThreadConnections.ContainsKey(connectionIndex) Then
                ' process and thread record contains connection under called index - let's read connection record
                connection = processAndThreadConnections(connectionIndex)
                ' C. read check end - if process and thread record contains connection under called index
                processAndThreadLock.ExitUpgradeableReadLock()
            Else
                ' process and thread record doesn't contain connection under called index - let's create new connection record
                ' D. write lock begin - to change process and thread record under called index
                processAndThreadLock.EnterWriteLock()
                ' C. read check end - if process and thread record contains connection under called index
                processAndThreadLock.ExitUpgradeableReadLock()
                ' create new connection record
                connection = Databasic.Connection._createAndOpen(connectionIndex)
                ' D. write lock end - to change process and thread record under called index
                processAndThreadLock.ExitWriteLock()
            End If

            ' A. read check end - if register contains any connection records under process and thread key
            Databasic.Connection._registerLock.ExitUpgradeableReadLock()
        Else
            ' register doesn't contain any connection records under process and thread key
            ' B. write lock begin - to change register records under process and thread key
            Databasic.Connection._registerLock.EnterWriteLock()
			' A. read check end - if register contains any connection records under process and thread key
			Databasic.Connection._registerLock.ExitUpgradeableReadLock()
			processAndThreadLock = New ReaderWriterLockSlim()

			' lets change the register - add connection records under thread and process key
			processAndThreadConnections = New Dictionary(Of Int32, Connection)()
			Databasic.Connection._registerLocks.Add(processAndThreadKey, processAndThreadLock)
			Databasic.Connection._connectionsRegister.Add(processAndThreadKey, processAndThreadConnections)

			' B. write lock end - to change register records under process and thread key
			Databasic.Connection._registerLock.ExitWriteLock()

			' C. read check begin - if process and thread record contains connection under called index
			processAndThreadLock.EnterUpgradeableReadLock()
			' check if process and thread record contains connection under called index
			If processAndThreadConnections.ContainsKey(connectionIndex) Then
				' process and thread record contains connection under called index - let's read connection record
				connection = processAndThreadConnections(connectionIndex)
				' C. read check end - if process and thread record contains connection under called index
				processAndThreadLock.ExitUpgradeableReadLock()
			Else
				' process and thread record doesn't contain connection under called index - let's create new connection record
				' D. write lock begin - to change process and thread record under called index
				processAndThreadLock.EnterWriteLock()
				' C. read check end - if process and thread record contains connection under called index
				processAndThreadLock.ExitUpgradeableReadLock()
				' create new connection record
				connection = Databasic.Connection._createAndOpen(connectionIndex)
				' D. write lock end - to change process and thread record under called index
				processAndThreadLock.ExitWriteLock()
			End If
		End If
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
			Databasic.Events.RaiseError(ex, New EventArgs())
		End Try
		Return result
    End Function

End Class
