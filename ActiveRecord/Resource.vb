Imports System.Dynamic
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace ActiveRecord
    <DefaultMember("Item")>
    Partial Public Class Resource

        Public Sub New()
        End Sub

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
        <CompilerGenerated>
        Private Shared _lock As ReaderWriterLockSlim = New ReaderWriterLockSlim()
        <CompilerGenerated>
        Private Shared _register As Dictionary(Of String, Resource) = New Dictionary(Of String, Resource)

        Private Shared Function _getConnectionIndex(instance As Object) As Int16
            Dim resourceType As Type = instance.GetType()
            Dim fieldInfo As FieldInfo = resourceType.GetField("connectionIndex", BindingFlags.Instance Or BindingFlags.NonPublic)
            If TypeOf fieldInfo Is FieldInfo Then
                Return DirectCast(fieldInfo.GetValue(instance), Int16)
            Else
                fieldInfo = resourceType.GetField("connectionName", BindingFlags.Instance Or BindingFlags.Static Or BindingFlags.NonPublic)
                If TypeOf fieldInfo Is FieldInfo Then
                    Dim connectionName As String = fieldInfo.GetValue(instance).ToString()
                    Return Connection.NamesAndIndexes(Connection.GetIndexByName(connectionName))
                Else
                    Events.RaiseError(New Exception($"Class '{resourceType.FullName}' doesn't contain any field named 'connectionIndex' or 'connectionName'."))
                    Return Nothing
                End If
            End If
        End Function

    End Class
End Namespace