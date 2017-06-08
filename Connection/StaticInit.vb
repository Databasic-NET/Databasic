Imports System.Configuration
Imports System.Data.Common
Imports System.IO
Imports System.Reflection
Imports System.Threading
Imports Databasic

Partial Public MustInherit Class Connection


    ''' <summary>
    ''' Load config and set up connection strings.
    ''' </summary>
    Shared Sub New()
        Connection.StaticInit()
    End Sub

    Friend Shared Sub StaticInit()
        SyncLock Connection._staticInitLock
            If Not Connection._initialized Then
                Connection._initialized = True
                Connection._staticInitCompleteConfig()
                Connection._staticInitCompleteProviders()
                ActiveRecord.Resource.StaticInit(Databasic.Connection._config.Count)
            End If
        End SyncLock
    End Sub

    Private Shared Sub _staticInitCompleteConfig()
        If (Databasic.Connection._config.Count = 0) Then
            Dim config As ConnectionStringsSection = DirectCast(
                    ConfigurationManager.GetSection("connectionStrings"),
                    ConnectionStringsSection
                )
            Dim i As Int16 = 0
            For Each cfgItem As ConnectionStringSettings In config.ConnectionStrings
                Databasic.Connection._config.Add(
                    i, New String() {cfgItem.ProviderName, cfgItem.ConnectionString}
                )
                Databasic.Connection.NamesAndIndexes.Add(cfgItem.Name, i)
                i += 1
            Next
        End If
    End Sub

    Private Shared Sub _staticInitCompleteProviders()
        Dim result As New Dictionary(Of String, Type)
        Try
            Dim assemblyFolder As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            Dim assemblyNeighbours As IEnumerable(Of Assembly) = (
                From file In Directory.GetFiles(assemblyFolder)
                Where Path.GetExtension(file).ToLower() = ".dll"
                Select Assembly.LoadFrom(file)
            )
            Dim assemblyName As String
            Dim connectionType As Type
            Dim clientName As String
            For Each assembly As Reflection.Assembly In assemblyNeighbours
                assemblyName = assembly.GetName().Name
                If assemblyName.IndexOf("Databasic.") = 0 AndAlso assemblyName <> "Databasic" Then
                    connectionType = assembly.GetType(assemblyName + ".Connection")
                    clientName = connectionType.GetField("ClientName", BindingFlags.Public Or BindingFlags.Static).GetValue(Nothing)
                    Connection._supportedProviders.Add(clientName, connectionType)
                End If
            Next
        Catch ex As Exception
        End Try
    End Sub

End Class
