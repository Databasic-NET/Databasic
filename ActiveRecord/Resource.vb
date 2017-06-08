Imports System.Data.Common
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Databasic.Connections

Namespace ActiveRecord
    Partial Public Class Resource

        ''' <summary>
        ''' 
        ''' </summary>
        <CompilerGenerated>
        Protected connectionIndex As Int16 = Database.DEFAUT_CONNECTION_INDEX
        ''' <summary>
        ''' 
        ''' </summary>
        <CompilerGenerated>
        Protected connectionName As String = Database.DEFAUT_CONNECTION_NAME

        ''' <summary>
        ''' Empty constructor to create Resource instances by Activator.CreateInstance(typeof(TResource))
        ''' </summary>
        Public Sub New()
        End Sub
        ''' <summary>
        ''' Static constructor to call static init in Connection class to load all configured connection 
        ''' settings from (App|Web).config to be prepared for possible static function call Resource.Columns()
        ''' </summary>
        Shared Sub New()
            Connection.StaticInit()
        End Sub
        Friend Shared Sub StaticInit(configuredConnectionsCount As Int32)
            For index = 0 To configuredConnectionsCount - 1
                Resource._columnsLocks.Add(index, New Object())
            Next
        End Sub

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