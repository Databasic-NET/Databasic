Imports System.Configuration
Imports System.Data.Common
Imports System.IO
Imports System.Reflection
Imports System.Threading
Imports Databasic

Partial Public MustInherit Class Connection

	Private Const _DATABASIC_ASM_NAME As String = "Databasic"
	Private Const _DATABASIC_ASM_NAME_BEGIN As String = "Databasic."
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
			Events.StaticInit()
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
		Dim asms As New List(Of String)
		Dim asmName As String
		Dim connectionType As Type
		Dim conn As Connection
		Dim referencedAsms As Reflection.Assembly() = AppDomain.CurrentDomain.GetAssemblies()
		For Each asm As Reflection.Assembly In referencedAsms
			asmName = asm.GetName().Name
			asms.Add(asmName)
			If Connection._staticInitIsDatabasicSubAssembly(asm) Then
				connectionType = asm.GetType(asmName + ".Connection")
				If Not TypeOf connectionType Is Type Then Continue For
				conn = Activator.CreateInstance(connectionType)
				Connection._supportedProviders.Add(conn.ClientName, connectionType)
				Connection._providersResources.Add(
					conn.ClientName, Activator.CreateInstance(conn.ProviderResource)
				)
			End If
		Next
		Try
			Dim appDirAsms As IEnumerable(Of Assembly) = (
				From file In Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory)
				Where Path.GetExtension(file).ToLower() = ".dll"
				Select Assembly.LoadFrom(file)
			)
			For Each asm As System.Reflection.Assembly In appDirAsms
				asmName = asm.GetName().Name
				If (
					Not asms.Contains(asmName) And
					Connection._staticInitIsDatabasicSubAssembly(asm)
				) Then
					connectionType = asm.GetType(asmName + ".Connection")
					If Not TypeOf connectionType Is Type Then Continue For
					conn = Activator.CreateInstance(connectionType)
					Connection._supportedProviders.Add(conn.ClientName, connectionType)
					Connection._providersResources.Add(
						conn.ClientName, Activator.CreateInstance(conn.ProviderResource)
					)
				End If
			Next
		Catch ex As Exception
		End Try
	End Sub

	Private Shared Function _staticInitIsDatabasicSubAssembly(asm As Reflection.Assembly) As Boolean
		Dim asmName As String = asm.GetName().Name
		If (
			asmName.IndexOf(Connection._DATABASIC_ASM_NAME_BEGIN) = 0 And
			asmName <> Connection._DATABASIC_ASM_NAME
		) Then Return True
		Return False
	End Function

End Class
