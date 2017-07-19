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
        Connection._staticInitDoneLock.EnterUpgradeableReadLock()
        If Not Connection._staticInitDone Then
            Connection._staticInitDoneLock.EnterWriteLock()
            Connection._staticInitDoneLock.ExitUpgradeableReadLock()
			Connection._staticInitDone = True
			Connection._staticInitCompleteProviders()
			Connection._staticInitCompleteConfig()
			Databasic.ProviderResource.StaticInit(Databasic.Connection.Config.Count)
			Connection._staticInitDoneLock.ExitWriteLock()
        Else
            Connection._staticInitDoneLock.ExitUpgradeableReadLock()
        End If
    End Sub

    Private Shared Sub _staticInitCompleteConfig()
        If (Databasic.Connection.Config.Count = 0) Then
            Dim config As ConnectionStringsSection = DirectCast(
                ConfigurationManager.GetSection("connectionStrings"),
                ConnectionStringsSection
            )
            Dim i As Int32 = 0
            For Each cfgItem As ConnectionStringSettings In config.ConnectionStrings
                Databasic.Connection.Config.Add(
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
			Dim connection As Connection
			For Each assembly As Reflection.Assembly In assemblyNeighbours
				assemblyName = assembly.GetName().Name
				If assemblyName.IndexOf("Databasic.") = 0 AndAlso assemblyName <> "Databasic" Then
					connectionType = assembly.GetType(assemblyName + ".Connection")
					If Not TypeOf connectionType Is Type Then Continue For
					connection = Activator.CreateInstance(connectionType)
					Connection._supportedProviders.Add(connection.ClientName, connectionType)
					Connection._providersResources.Add(connection.ClientName, Activator.CreateInstance(connection.ProviderResource))
				End If
			Next
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
    End Sub

End Class
