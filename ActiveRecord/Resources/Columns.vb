Imports System.Data.Common
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Databasic.Connections

Namespace ActiveRecord
    Partial Public Class Resource

        ''' <summary>
        ''' Resource class primary database table unique column name, usually "Id".
        ''' </summary>
        <CompilerGenerated>
        Protected uniqueColumnName As String = Database.DEFAUT_UNIQUE_COLUMN_NAME



        <CompilerGenerated>
        Private Shared _columns As New Dictionary(Of Int16, Dictionary(Of String, List(Of String)))
        <CompilerGenerated>
        Private Shared _columnsLocks As New Dictionary(Of Int16, Object)




        Public Shared Function Columns(resourceType As Type, Optional tableIndex As Int16 = 0) As List(Of String)
            Dim result As List(Of String) = Nothing
            Dim table As String = Resource.Table(resourceType, tableIndex)
            Dim instance As Object = Activator.CreateInstance(resourceType)
            Dim connectionIndex As Int16 = Resource._getConnectionIndex(instance)
            Dim tablesAndColumns As Dictionary(Of String, List(Of String))
            SyncLock Resource._columnsLocks(connectionIndex)
                If Not ActiveRecord.Resource._columns.ContainsKey(connectionIndex) Then
                    ActiveRecord.Resource._columns.Add(connectionIndex, New Dictionary(Of String, List(Of String)))
                End If
                tablesAndColumns = ActiveRecord.Resource._columns(connectionIndex)
                If tablesAndColumns.ContainsKey(table) Then
                    result = tablesAndColumns(table)
                End If
            End SyncLock
            If Not TypeOf result Is List(Of String) Then
                Dim connection As Connection = Connection.Get(connectionIndex)
                Dim resource As Provider.Resource = Activator.CreateInstance(connection.ResourceType)
                result = resource.GetTableColumns(connection, table)
                If result.Count = 0 Then
                    Events.RaiseError(New Exception(
                        $"No columns found for table: '{table}'. Is the table name correct?"
                    ))
                End If
            End If
            SyncLock Resource._columnsLocks(connectionIndex)
                If TypeOf result Is List(Of String) Then
                    If Not tablesAndColumns.ContainsKey(table) Then
                        tablesAndColumns.Add(table, result)
                    End If
                End If
            End SyncLock
            Return result
        End Function
        Public Shared Function Columns(Of TResource)(separator As String, Optional tableIndex As Int16 = 0) As String
            Return String.Join(
                separator,
                Enumerable.ToArray(Of String)(
                    Resource.Columns(GetType(TResource), tableIndex)
                )
            )
        End Function
        Public Shared Function Columns(Of TResource)(Optional tableIndex As Int16 = 0, Optional separator As String = ",") As String
            Return String.Join(
                separator,
                Enumerable.ToArray(Of String)(
                    Resource.Columns(GetType(TResource), tableIndex)
                )
            )
        End Function

        Public Function Columns(separator As String, Optional tableIndex As Int16 = 0) As String
            Return String.Join(
                separator,
                Enumerable.ToArray(Of String)(
                    Resource.Columns(Me.GetType(), tableIndex)
                )
            )
        End Function

        Public Function Columns(Optional tableIndex As Int16 = 0, Optional separator As String = ",") As String
            Return String.Join(
                separator,
                Enumerable.ToArray(Of String)(
                    Resource.Columns(Me.GetType(), tableIndex)
                )
            )
        End Function




        Public Shared Function ColumnsExcept(Of TResource)(exceptColumns As String(), separator As String, Optional tableIndex As Int16 = 0) As String
            Dim result As List(Of String) = Resource.Columns(GetType(TResource), tableIndex)
            For Each exceptCol As String In exceptColumns
                If result.Contains(exceptCol) Then result.Remove(exceptCol)
            Next
            Return String.Join(
                separator,
                Enumerable.ToArray(Of String)(result)
            )
        End Function
        Public Shared Function ColumnsExcept(Of TResource)(exceptColumns As String(), Optional tableIndex As Int16 = 0, Optional separator As String = ",") As String
            Dim result As List(Of String) = Resource.Columns(GetType(TResource), tableIndex)
            For Each exceptCol As String In exceptColumns
                If result.Contains(exceptCol) Then result.Remove(exceptCol)
            Next
            Return String.Join(
                separator,
                Enumerable.ToArray(Of String)(result)
            )
        End Function

        Public Function ColumnsExcept(exceptColumns As String(), separator As String, Optional tableIndex As Int16 = 0) As String
            Dim result As List(Of String) = Resource.Columns(Me.GetType(), tableIndex)
            For Each exceptCol As String In exceptColumns
                If result.Contains(exceptCol) Then result.Remove(exceptCol)
            Next
            Return String.Join(
                separator,
                Enumerable.ToArray(Of String)(result)
            )
        End Function




        ''' <summary>
        ''' Get declared identifier table column name by 'resourceType' argument.
        ''' </summary>
        ''' <param name="resourceType">Class type, inherited from Resource class with declared protected static field 'idColumn' as string.</param>
        ''' <returns>Declared database table id column name from resource class.</returns>
        Public Shared Function UniqueColumn(resourceType As Type) As String
            Dim result As String = ""
            Dim instance As Object = Activator.CreateInstance(resourceType)
            Dim fieldInfo As FieldInfo = resourceType.GetField("uniqueColumnName", BindingFlags.Instance Or BindingFlags.Static Or BindingFlags.NonPublic)
            If Not TypeOf fieldInfo Is FieldInfo Then
                Throw New Exception($"Class '{resourceType.FullName}' has no field 'uniqueColumnName'. Please define this field as string.")
            End If
            result = DirectCast(fieldInfo.GetValue(instance), String)
            Return If(String.IsNullOrEmpty(result), Database.DEFAUT_UNIQUE_COLUMN_NAME, result)
        End Function
        ''' <summary>
        ''' Get declared identifier table column name from generic type 'TResource'.
        ''' </summary>
        ''' <typeparam name="TResource">Class name, inherited from Resource class with declared protected static field 'idColumn' as string.</typeparam>
        ''' <returns>Declared database table id column name from resource class.</returns>
        Public Shared Function UniqueColumn(Of TResource)() As String
            Return Resource.UniqueColumn(GetType(TResource))
        End Function
        Public Function UniqueColumn() As String
            Return Resource.UniqueColumn(Me.GetType())
        End Function



    End Class
End Namespace