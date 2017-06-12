Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace ActiveRecord
	Partial Public Class Resource



		<CompilerGenerated>
		Private Shared _columns As New Dictionary(Of Int32, Dictionary(Of String, List(Of String)))
		<CompilerGenerated>
		Private Shared _columnsLocks As New Dictionary(Of Int32, ReaderWriterLockSlim)




		Friend Shared Sub StaticInit(connectionsConfigLength As Int32)
			For index As Int32 = 0 To connectionsConfigLength - 1
				Databasic.ActiveRecord.Resource._columnsLocks.Add(index, New ReaderWriterLockSlim())
			Next
		End Sub





		Public Shared Function Columns(resourceType As Type, Optional tableIndex As Int16 = 0) As List(Of String)
			Dim result As List(Of String) = Nothing
			Dim table As String = Resource.Table(resourceType, tableIndex)
			Dim connectionIndex As Int16 = Tools.GetConnectionIndexByClassAttr(resourceType)
			Dim tablesAndColumns As Dictionary(Of String, List(Of String))
			Dim loadingColumnsFromDb As Boolean = True
			Resource._columnsLocks(connectionIndex).EnterUpgradeableReadLock()

			If ActiveRecord.Resource._columns.ContainsKey(connectionIndex) Then
				tablesAndColumns = ActiveRecord.Resource._columns(connectionIndex)
				If tablesAndColumns.ContainsKey(table) Then
					result = tablesAndColumns(table)
					loadingColumnsFromDb = False
				End If
				Resource._columnsLocks(connectionIndex).ExitUpgradeableReadLock()
			Else
				Resource._columnsLocks(connectionIndex).EnterWriteLock()
				Resource._columnsLocks(connectionIndex).ExitUpgradeableReadLock()
				ActiveRecord.Resource._columns.Add(connectionIndex, New Dictionary(Of String, List(Of String)))
				Resource._columnsLocks(connectionIndex).ExitWriteLock()
			End If
			If loadingColumnsFromDb Then
				Dim connection As Connection = Connection.Get(connectionIndex)
				Dim resource As Provider.Resource = Activator.CreateInstance(connection.ResourceType)
				result = resource.GetTableColumns(connection, table)
				If result.Count = 0 Then
					Events.RaiseError(New Exception(
						$"No columns found for table: '{table}'. Is the table name correct?"
					))
				End If
				If TypeOf result Is List(Of String) Then
					Databasic.ActiveRecord.Resource._columnsLocks(connectionIndex).EnterWriteLock()
					tablesAndColumns = ActiveRecord.Resource._columns(connectionIndex)
					If Not tablesAndColumns.ContainsKey(table) Then
						tablesAndColumns.Add(table, result)
					End If
					Databasic.ActiveRecord.Resource._columnsLocks(connectionIndex).ExitWriteLock()
				End If
			End If
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

		Public Shared Function Columns(separator As String, Optional tableIndex As Int16 = 0) As String
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(
					Resource.Columns(Tools.GetEntryClassType(), tableIndex)
				)
			)
		End Function

		Public Shared Function Columns(Optional tableIndex As Int16 = 0, Optional separator As String = ",") As String
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(
					Resource.Columns(Tools.GetEntryClassType(), tableIndex)
				)
			)
		End Function




		Public Shared Function ColumnsExcept(Of TResource)(exceptColumns As String(), separator As String, Optional tableIndex As Int16 = Database.DEFAUT_CONNECTION_INDEX) As String
			Dim result As List(Of String) = Resource.Columns(GetType(TResource), tableIndex)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function
		Public Shared Function ColumnsExcept(Of TResource)(exceptColumns As String(), Optional tableIndex As Int16 = Database.DEFAUT_CONNECTION_INDEX, Optional separator As String = ",") As String
			Dim result As List(Of String) = Resource.Columns(GetType(TResource), tableIndex)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function

		Public Shared Function ColumnsExcept(exceptColumns As String(), separator As String, Optional tableIndex As Int16 = Database.DEFAUT_CONNECTION_INDEX) As String
			Dim result As List(Of String) = Resource.Columns(Tools.GetEntryClassType(), tableIndex)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function

		Public Shared Function ColumnsExcept(exceptColumns As String(), Optional tableIndex As Int16 = Database.DEFAUT_CONNECTION_INDEX, Optional separator As String = ",") As String
			Dim result As List(Of String) = Resource.Columns(Tools.GetEntryClassType(), tableIndex)
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