Imports System.Data.Common
Imports System.Threading

Partial Public MustInherit Class Connection
	''' <summary>
	''' Register any connection string for databasic models and queries which doesn't exist in application config file.
	''' </summary>
	''' <param name="name">Connection string unique name, the same name used in your model classes in Databasic.Connection("name") class attribute.</param>
	''' <param name="connectionString">DSN connection string value.</param>
	''' <param name="providerName">Database provider supported by databasic library.</param>
	''' <param name="overwriteExisting">Overwrite already existing connection string under given name in databasic config values internal register.</param>
	Public Shared Sub RegisterConfigRecord(
		name As String,
		connectionString As String,
		providerName As String,
		Optional overwriteExisting As Boolean = False
	)
		Connection._registerConfigRecord(name, connectionString, providerName, overwriteExisting)
	End Sub
	''' <summary>
	''' Register any connection string for databasic models and queries which doesn't exist in application config file.
	''' </summary>
	''' <param name="name">Connection string unique name, the same name used in your model classes in Databasic.Connection("name") class attribute.</param>
	''' <param name="connectionString">DSN connection string value.</param>
	''' <param name="providerName">Database provider supported by databasic library.</param>
	''' <param name="overwriteExisting">Overwrite already existing connection string under given name in databasic config values internal register.</param>
	Public Shared Sub RegisterConfigRecord(
		name As String,
		connectionString As String,
		providerName As ProviderName,
		Optional overwriteExisting As Boolean = False
	)
		Dim providerNameStr = If(
			ProviderNames.Values.ContainsKey(providerName),
			ProviderNames.Values(providerName),
			""
		)
		Connection._registerConfigRecord(
			name,
			connectionString,
			providerNameStr,
			overwriteExisting
		)
	End Sub
	Private Shared Sub _registerConfigRecord(
		name As String,
		connectionString As String,
		providerName As String,
		Optional overwriteExisting As Boolean = False
	)
		Dim throwException As Boolean = False
		Dim connectionIndex As Int32
		Connection._staticInitDoneLock.EnterWriteLock()
		Dim nameExists As Boolean = Databasic.Connection.NamesAndIndexes.ContainsKey(name)
		If (
			Not nameExists Or (nameExists And overwriteExisting)
		) Then
			If nameExists Then
				connectionIndex = Databasic.Connection.NamesAndIndexes(name)
			Else
				If Not ProviderNames.Values.ContainsValue(providerName) Then
					Throw New Exception(
						$"Provider name not supported by Databasic: '{providerName}'. " +
						"Check if you are using any supported provider from this list: '" +
						String.Join("', '", ProviderNames.Values.Values.ToArray()) + "'."
					)
				End If
				connectionIndex = Databasic.Connection.NamesAndIndexes.Count
				Databasic.Connection.Config.Add(
					connectionIndex, New String() {
						providerName, connectionString
					}
				)
				Databasic.Connection.NamesAndIndexes.Add(name, connectionIndex)
			End If
		ElseIf nameExists And Not overwriteExisting Then
			throwException = True
		End If
		Connection._staticInitDoneLock.ExitWriteLock()
		If throwException Then
			Throw New Exception($"Connection name: '{name}' allready exists.")
		End If
	End Sub
End Class
