Imports System.Data.Common
Imports System.Reflection
Imports System.Threading

Partial Public MustInherit Class Connection

    Public Overridable ReadOnly Property ProviderVersion As Version
        Get
            Return New Version(Me.Provider.ServerVersion)
        End Get
    End Property

    Public Overridable Property Provider As DbConnection

    Public Overridable Property StatementType As System.Type

    Public Overridable Property ResourceType As System.Type

    Public Overridable Property Resource As ActiveRecord.Resource

    ''' <summary>
    ''' Parsed subnode values from node &lt;connectionStrings&gt; in (App|Web).config file.
    ''' </summary>
    Friend Shared Config As New Dictionary(Of Int32, String())
    ''' <summary>
    ''' (App|Web).config connection names with their indexes.
    ''' </summary>
    Friend Shared NamesAndIndexes As New Dictionary(Of String, Int32)
    ''' <summary>
    ''' Client assembly name used by specific connection implementation.
    ''' </summary>
    Public Shared ClientName As String
    ''' <summary>
    ''' Threads semahore to read/write into managed connections store.
    ''' </summary>
    Private Shared _staticInitDoneLock As New ReaderWriterLockSlim()
    ''' <summary>
    ''' True if static initialization completed, nothing else.
    ''' </summary>
    Private Shared _staticInitDone As Boolean = False

    ''' <summary>
    ''' Supported database providers.
    ''' </summary>
    Private Shared _supportedProviders As New Dictionary(Of String, Type)
    ''' <summary>
    ''' All Databasic connections managed store for all processes and for all threads.
    ''' </summary>
    Private Shared _connectionsRegister As New Dictionary(Of String, Dictionary(Of Int32, Connection))



    Friend Shared Function GetIndexByName(connectionName As String) As Int32
        If Databasic.Connection.NamesAndIndexes.ContainsKey(connectionName) Then
            Return Databasic.Connection.NamesAndIndexes(connectionName)
        Else
            Events.RaiseError(New Exception($"Connection settings under name doesn't exist: {connectionName}."))
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
    ''' Fire all added error handlers.
    ''' </summary>
    ''' <param name="sender">Exception instance.</param>
    ''' <param name="e">EventArgs instance.</param>
    Protected Shared Sub errorHandler(sender As Object, e As EventArgs)
        Dim fi As FieldInfo = e.GetType().GetField("exception", BindingFlags.NonPublic)
        If TypeOf fi Is FieldInfo Then
            Dim exception = fi.GetValue(e)
            Events.RaiseError(exception, e)
        Else
            Events.RaiseError(sender, e)
        End If
    End Sub

End Class
