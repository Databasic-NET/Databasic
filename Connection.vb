Imports System.Data.Common
Imports System.Reflection
Imports System.Threading

Partial Public MustInherit Class Connection
	Public Property OpenedTransaction As Transaction
		Get
			Return Me._openedTransaction
		End Get
		Set(ByVal AutoPropertyValue As Transaction)
			Me._openedTransaction = AutoPropertyValue
		End Set
	End Property
	Private _openedTransaction As Transaction

	''' <summary>
	''' ADO.NET provider asembly version.
	''' </summary>
	Public Overridable ReadOnly Property ProviderVersion As Version
		Get
			Return New Version(Me.Provider.ServerVersion)
		End Get
	End Property
	''' <summary>
	''' Config connection index.
	''' </summary>
	Public Overridable ReadOnly Property ConnectionIndex As Int32
		Get
			Return Me._connectionIndex
		End Get
	End Property
	Private _connectionIndex As Int32 = 0
	''' <summary>
	''' ADO.NET connection instance.
	''' </summary>
	Public Overridable ReadOnly Property Provider As DbConnection
	''' <summary>
	''' Provider resource type to create specific resource class for common database operations.
	''' </summary>
	Public Overridable ReadOnly Property ProviderResource As System.Type
	''' <summary>
	''' Client assembly name used by specific connection implementation.
	''' </summary>
	Public Overridable ReadOnly Property ClientName As String
	''' <summary>
	''' ADO.NET statement type.
	''' </summary>
	Public Overridable ReadOnly Property Statement As System.Type

	''' <summary>
	''' Parsed subnode values from node &lt;connectionStrings&gt; in (App|Web).config file.
	''' </summary>
	Friend Shared Config As New Dictionary(Of Int32, String())
	''' <summary>
	''' (App|Web).config connection names with their indexes.
	''' </summary>
	Friend Shared NamesAndIndexes As New Dictionary(Of String, Int32)

	''' <summary>
	''' Threads semahore to read/write into managed connections store.
	''' </summary>
	Private Shared _staticInitDoneLock As New ReaderWriterLockSlim
	''' <summary>
	''' True if static initialization completed, nothing else.
	''' </summary>
	Private Shared _staticInitDone As Boolean = False
	''' <summary>
	''' Supported database providers.
	''' </summary>
	Private Shared _supportedProviders As New Dictionary(Of String, Type)
	''' <summary>
	''' Supported database providers resource instances.
	''' </summary>
	Private Shared _providersResources As New Dictionary(Of String, ProviderResource)
	''' <summary>
	''' All Databasic connections managed store for all processes and for all threads.
	''' </summary>
	Private Shared _connectionsRegister As New Dictionary(Of String, Dictionary(Of Int32, Connection))

	''' <summary>
	''' Get config connection index (sequence index) by connection name.
	''' </summary>
	''' <param name="connectionName">Connection name dictionary key.</param>
	''' <returns>Config connection index (sequence index).</returns>
	Friend Shared Function GetIndexByName(connectionName As String, Optional throwException As Boolean = True) As Int32?
		If Databasic.Connection.NamesAndIndexes.ContainsKey(connectionName) Then
			Return Databasic.Connection.NamesAndIndexes(connectionName)
		ElseIf throwException Then
			Events.RaiseError(New Exception(
				$"Connection settings under name doesn't exist: {connectionName}."
			))
		End If
		Return Nothing
	End Function

	''' <summary>
	''' Return string as combination of current process id, underscore char and thread id.
	''' </summary>
	''' <returns>String as combination of current process id, underscore char and thread id.</returns>
	Private Shared Function _getProcessAndThreadKey() As String
		Return $"{Process.GetCurrentProcess().Id}_{Thread.CurrentThread.ManagedThreadId}"
	End Function

	''' <summary>
	''' Get specific database implementation ProviderResource class instance by connection client name.
	''' </summary>
	''' <returns>Specific database inmplementation ProviderResource instance.</returns>
	Friend Function GetProviderResource() As ProviderResource
		Return Connection._providersResources(Me.ClientName)
	End Function

End Class
