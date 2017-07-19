Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Threading

Partial Public MustInherit Class ProviderResource





	<CompilerGenerated>
	Private Shared _columns As New Dictionary(Of Int32, Dictionary(Of String, Dictionary(Of String, Boolean)))
	<CompilerGenerated>
	Private Shared _columnsLocks As New Dictionary(Of Int32, ReaderWriterLockSlim)





	Friend Shared Sub StaticInit(connectionsConfigLength As Int32)
		For index As Int32 = 0 To connectionsConfigLength - 1
			ProviderResource._columnsLocks.Add(index, New ReaderWriterLockSlim())
		Next
	End Sub





	Public Shared Function IsColumnNullable(connectionIndex As Int32, table As String, column As String) As Boolean
		Dim result As Boolean?
		Dim tablesAndColumns As Dictionary(Of String, Dictionary(Of String, Boolean))
		Dim tableColumns As Dictionary(Of String, Boolean)
		ProviderResource._columnsLocks(connectionIndex).EnterUpgradeableReadLock()
		If Databasic.ProviderResource._columns.ContainsKey(connectionIndex) Then
			tablesAndColumns = Databasic.ProviderResource._columns(connectionIndex)
			If tablesAndColumns.ContainsKey(table) Then
				tableColumns = tablesAndColumns(table)
				If tableColumns.ContainsKey(column) Then result = tableColumns(column)
				ProviderResource._columnsLocks(connectionIndex).ExitUpgradeableReadLock()
			Else
				ProviderResource._columnsLocks(connectionIndex).EnterWriteLock()
				ProviderResource._columnsLocks(connectionIndex).ExitUpgradeableReadLock()
				ProviderResource._columnsArray(connectionIndex, table)
				tablesAndColumns = Databasic.ProviderResource._columns(connectionIndex)
				tableColumns = tablesAndColumns(table)
				If tableColumns.ContainsKey(column) Then result = tableColumns(column)
				ProviderResource._columnsLocks(connectionIndex).ExitWriteLock()
			End If
		Else
			ProviderResource._columnsLocks(connectionIndex).EnterWriteLock()
			ProviderResource._columnsLocks(connectionIndex).ExitUpgradeableReadLock()
			ProviderResource._columnsArray(connectionIndex, table)
			tablesAndColumns = Databasic.ProviderResource._columns(connectionIndex)
			tableColumns = tablesAndColumns(table)
			If tableColumns.ContainsKey(column) Then result = tableColumns(column)
			ProviderResource._columnsLocks(connectionIndex).ExitWriteLock()
		End If
		If Not result.HasValue Then
			Events.RaiseError(New Exception(String.Format(
				"Table '{0}' has no column '{1}' under connection index {2}.",
				table, column, connectionIndex.ToString()
			)))
		End If
		Return result
	End Function
	Public Shared Function ColumnsArray(resourceType As Type, Optional tableIndex As Int32 = 0) As String()
		Dim result As String() = Nothing
		Dim table As String = ActiveRecord.Resource.Table(resourceType, tableIndex)
		Dim connectionIndex As Int32 = Tools.GetConnectionIndexByClassAttr(resourceType)
		Dim tablesAndColumns As Dictionary(Of String, Dictionary(Of String, Boolean))
		ProviderResource._columnsLocks(connectionIndex).EnterUpgradeableReadLock()
		If Databasic.ProviderResource._columns.ContainsKey(connectionIndex) Then
			tablesAndColumns = Databasic.ProviderResource._columns(connectionIndex)
			If tablesAndColumns.ContainsKey(table) Then
				result = tablesAndColumns(table).Keys.ToArray()
				ProviderResource._columnsLocks(connectionIndex).ExitUpgradeableReadLock()
			Else
				ProviderResource._columnsLocks(connectionIndex).EnterWriteLock()
				ProviderResource._columnsLocks(connectionIndex).ExitUpgradeableReadLock()
				result = ProviderResource._columnsArray(connectionIndex, table)
				ProviderResource._columnsLocks(connectionIndex).ExitWriteLock()
			End If
		Else
			ProviderResource._columnsLocks(connectionIndex).EnterWriteLock()
			ProviderResource._columnsLocks(connectionIndex).ExitUpgradeableReadLock()
			result = ProviderResource._columnsArray(connectionIndex, table)
			ProviderResource._columnsLocks(connectionIndex).ExitWriteLock()
		End If
		Return result
	End Function
	Public Shared Function ColumnsList(resourceType As Type, Optional tableIndex As Int32 = 0) As List(Of String)
		Dim result As List(Of String) = Nothing
		Dim table As String = ActiveRecord.Resource.Table(resourceType, tableIndex)
		Dim connectionIndex As Int32 = Tools.GetConnectionIndexByClassAttr(resourceType)
		Dim tablesAndColumns As Dictionary(Of String, Dictionary(Of String, Boolean))
		ProviderResource._columnsLocks(connectionIndex).EnterUpgradeableReadLock()
		If Databasic.ProviderResource._columns.ContainsKey(connectionIndex) Then
			tablesAndColumns = Databasic.ProviderResource._columns(connectionIndex)
			If tablesAndColumns.ContainsKey(table) Then
				result = tablesAndColumns(table).Keys.ToList()
				ProviderResource._columnsLocks(connectionIndex).ExitUpgradeableReadLock()
			Else
				ProviderResource._columnsLocks(connectionIndex).EnterWriteLock()
				ProviderResource._columnsLocks(connectionIndex).ExitUpgradeableReadLock()
				result = ProviderResource._columnsArray(connectionIndex, table).ToList()
				ProviderResource._columnsLocks(connectionIndex).ExitWriteLock()
			End If
		Else
			ProviderResource._columnsLocks(connectionIndex).EnterWriteLock()
			ProviderResource._columnsLocks(connectionIndex).ExitUpgradeableReadLock()
			result = ProviderResource._columnsArray(connectionIndex, table).ToList()
			ProviderResource._columnsLocks(connectionIndex).ExitWriteLock()
		End If
		Return result
	End Function
	Private Shared Function _columnsArray(connectionIndex As Int32, table As String) As String()
		Dim result As String()
		Dim connection As Connection = Connection.Get(connectionIndex)
		Dim colsAndNulls As Dictionary(Of String, Boolean) = connection.GetProviderResource().GetTableColumns(table, connection)
		If TypeOf colsAndNulls Is Dictionary(Of String, Boolean) AndAlso colsAndNulls.Count > 0 Then
			result = colsAndNulls.Keys.ToArray()
			If Not Databasic.ProviderResource._columns.ContainsKey(connectionIndex) Then
				Databasic.ProviderResource._columns.Add(connectionIndex, New Dictionary(Of String, Dictionary(Of String, Boolean)))
			End If
			tablesAndColumns = Databasic.ProviderResource._columns(connectionIndex)
			If Not tablesAndColumns.ContainsKey(table) Then
				tablesAndColumns.Add(table, colsAndNulls)
			End If
		Else
			Events.RaiseError(New Exception(
				$"No columns found for table: '{table}'. Is the table name correct?"
			))
			result = New String() {"*"}
		End If
		Return result
	End Function





	Public Overridable Function GetTableColumns(table As String, connection As Databasic.Connection) As Dictionary(Of String, Boolean)
		Return New Dictionary(Of String, Boolean)
	End Function





End Class