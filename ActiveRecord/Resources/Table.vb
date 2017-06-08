Imports System.Data.Common
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Databasic.Connections

Namespace ActiveRecord
    Partial Public Class Resource
        ''' <summary>
        ''' Resource class database table name(s), first table is primary table.
        ''' </summary>
        <CompilerGenerated>
        Protected tableNames As String() = New String() {}



        ''' <summary>
        ''' Get declared table name by 'resourceType' and by optional called index argument.
        ''' </summary>
        ''' <param name="resourceType">Class type, inherited from Resource class with declared protected static field 'tables' as array of strings.</param>
        ''' <param name="tableIndex">Array index to get proper table name string from declared protected override property 'tables' as array of strings.</param>
        ''' <returns>Declared database table name from resource class.</returns>
        Public Shared Function Table(resourceType As Type, Optional tableIndex As Int16 = 0) As String
            Dim result As String = ""
            Dim instance As Object = Activator.CreateInstance(resourceType)
            Dim fieldInfo As FieldInfo = resourceType.GetField("tableNames", BindingFlags.Instance Or BindingFlags.Static Or BindingFlags.NonPublic)
            If Not TypeOf fieldInfo Is FieldInfo Then
                Throw New Exception($"Class '{resourceType.FullName}' has no field 'tableNames'. Please define this field as array of strings.")
            End If
            Dim tables As String() = DirectCast(fieldInfo.GetValue(instance), String())
            If tableIndex < 0 Or tableIndex + 1 > tables.Length Then
                Throw New Exception($"Class '{resourceType.FullName}' has no table name contained at index {tableIndex} in field 'tableNames'.")
            End If
            Return tables(tableIndex)
        End Function
        ''' <summary>
        ''' Get declared table name from generic type 'TResource' and by optional called index argument.
        ''' </summary>
        ''' <typeparam name="TResource">Class name, inherited from Resource class with declared protected static field 'tables' as array of strings.</typeparam>
        ''' <param name="tableIndex">Array index to get proper table name string from declared protected override property 'tables' as array of strings.</param>
        ''' <returns>Declared database table name from resource class.</returns>
        Public Shared Function Table(Of TResource)(Optional tableIndex As Int16 = 0) As String
            Return Resource.Table(GetType(TResource), tableIndex)
        End Function
        Public Function Table(Optional tableIndex As Int16 = 0) As String
            Return Resource.Table(Me.GetType(), tableIndex)
        End Function
    End Class
End Namespace