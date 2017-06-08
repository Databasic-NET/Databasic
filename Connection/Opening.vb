Imports System.Configuration
Imports System.Data.Common
Imports System.Reflection
Imports System.Threading
Imports Databasic
Imports Databasic.Connections

Partial Public MustInherit Class Connection








    ''' <summary>
    ''' Parsed subnode values from node &lt;connectionStrings&gt; in (App|Web).config file.
    ''' </summary>
    Private Shared _config As New Dictionary(Of Int16, String())



    ''' <summary>
    ''' Get and open connection by config index.
    ''' </summary>
    ''' <param name="connectionIndex">Config connection index.</param>
    ''' <returns></returns>
    Public Shared Function [Get](Optional connectionIndex As Int16 = Database.DEFAUT_CONNECTION_INDEX) As Connection
        If Not Databasic.Connection._config.ContainsKey(connectionIndex) Then
            Events.RaiseError(New Exception($"Connection settings under index doesn't exist: {connectionIndex}."))
        End If
        Dim connection As Connection = Nothing
        Dim typeAndDsn As String()
        Dim provider As String
        Dim connectionType As Type
        Dim processAndThreadKey As String = Databasic.Connection._getProcessAndThreadKey()
        If (
            Databasic.Connection._connections.ContainsKey(processAndThreadKey) AndAlso
            Databasic.Connection._connections(processAndThreadKey).ContainsKey(connectionIndex)
        ) Then
            connection = Databasic.Connection._connections(processAndThreadKey).Item(connectionIndex)
        Else
            typeAndDsn = Databasic.Connection._config(connectionIndex)
            provider = typeAndDsn(0).Trim()
            If Not Databasic.Connection._supportedProviders.ContainsKey(provider) Then
                Events.RaiseError(New Exception($"Connection provider not installed: '{provider}'."))
            End If
            connectionType = Databasic.Connection._supportedProviders(provider)
            Try
                connection = Activator.CreateInstance(connectionType)
                connection.Open(typeAndDsn(1).Trim())
            Catch ex As Exception
                Events.RaiseError(ex, New EventArgs())
            End Try
            Databasic.Connection._register(processAndThreadKey, connectionIndex, connection)
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

    Private Shared Sub _register(processAndThreadKey As String, connectionIndex As Int32, conn As Connection)
        Dim conns As Dictionary(Of Int16, Connection)
        SyncLock Databasic.Connection._closingLock
            If Databasic.Connection._connections.ContainsKey(processAndThreadKey) Then
                conns = Databasic.Connection._connections(processAndThreadKey)
                conns(connectionIndex) = conn
            Else
                Databasic.Connection._connections.Add(processAndThreadKey, New Dictionary(Of Short, Connection) From {
                    {connectionIndex, conn}
                })
            End If
        End SyncLock
    End Sub

End Class
