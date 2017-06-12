Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace ActiveRecord
	<DefaultMember("Item")>
	Partial Public Class Resource

		<CompilerGenerated>
		Private Shared _lock As ReaderWriterLockSlim = New ReaderWriterLockSlim()

		<CompilerGenerated>
		Private Shared _register As Dictionary(Of String, Resource) = New Dictionary(Of String, Resource)

		Public Shared Function GetInstance(type As Type) As Resource
			Dim result As Resource
			Dim key As String = type.Assembly.GetName().Name + ":" + type.FullName
			Resource._lock.EnterUpgradeableReadLock()
			If Resource._register.ContainsKey(key) Then
				result = Resource._register(key)
				Resource._lock.ExitUpgradeableReadLock()
			Else
				Resource._lock.EnterWriteLock()
				Resource._lock.ExitUpgradeableReadLock()
				result = Activator.CreateInstance(type)
				Resource._register.Add(key, result)
				Resource._lock.ExitWriteLock()
			End If
			Return result
		End Function

	End Class
End Namespace