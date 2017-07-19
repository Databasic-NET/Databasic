Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace ActiveRecord
	Partial Public Class Resource





		Public Shared Function Columns(Of TResource)(separator As String, Optional tableIndex As Int16 = 0) As String
			Return String.Join(
				separator, Databasic.ProviderResource.ColumnsArray(GetType(TResource), tableIndex)
			)
		End Function
		Public Shared Function Columns(Of TResource)(Optional tableIndex As Int16 = 0, Optional separator As String = ",") As String
			Return String.Join(
				separator, Databasic.ProviderResource.ColumnsArray(GetType(TResource), tableIndex)
			)
		End Function
		Public Shared Function Columns(separator As String, Optional tableIndex As Int16 = 0) As String
			Return String.Join(
				separator, Databasic.ProviderResource.ColumnsArray(Tools.GetEntryClassType(), tableIndex)
			)
		End Function
		Public Shared Function Columns(Optional tableIndex As Int16 = 0, Optional separator As String = ",") As String
			Return String.Join(
				separator, Databasic.ProviderResource.ColumnsArray(Tools.GetEntryClassType(), tableIndex)
			)
		End Function





		Public Shared Function ColumnsExcept(Of TResource)(exceptColumns As String(), Optional tableIndex As Int16 = 0, Optional separator As String = ",") As String
			Dim result As List(Of String) = Databasic.ProviderResource.ColumnsList(GetType(TResource), tableIndex)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function
		Public Shared Function ColumnsExcept(Of TResource)(exceptColumns As String(), separator As String, Optional tableIndex As Int16 = 0) As String
			Dim result As List(Of String) = Databasic.ProviderResource.ColumnsList(GetType(TResource), tableIndex)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function
		Public Shared Function ColumnsExcept(exceptColumns As String(), Optional tableIndex As Int16 = 0, Optional separator As String = ",") As String
			Dim result As List(Of String) = Databasic.ProviderResource.ColumnsList(Tools.GetEntryClassType(), tableIndex)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function
		Public Shared Function ColumnsExcept(exceptColumns As String(), separator As String, Optional tableIndex As Int16 = 0) As String
			Dim result As List(Of String) = Databasic.ProviderResource.ColumnsList(Tools.GetEntryClassType(), tableIndex)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function





		Public Shared Function KeyColumns(ByRef resourceType As Type, Optional keyName As String = Databasic.Defaults.KEY_NAME) As KeyColumns
			Dim result As KeyColumns = New KeyColumns
			Dim allKeyCols As AllKeyColumns = MetaDescriptor.GetAllKeyColumns(resourceType)
			If allKeyCols.PrimaryColumns.ContainsKey(keyName) Then
				result.Type = KeyType.Primary
				result.Columns = allKeyCols.PrimaryColumns(keyName)
			ElseIf allKeyCols.UniqueColumns.ContainsKey(keyName) Then
				result.Type = KeyType.Unique
				result.Columns = allKeyCols.UniqueColumns(keyName)
			Else
				Throw New Exception(String.Format(
				"Class '{0}' has no properties or fields with 'PrimaryKey' or 'UniqueKey' attributes defined. " +
				"Please set up this class properly by database structure to process this operation again.",
				resourceType.FullName
			))
			End If
			result.AutoIncrementColumn = allKeyCols.AutoIncrementColumn
			Return result
		End Function
		Public Shared Function KeyColumns(ByRef classMetaDescription As MetaDescription, Optional keyName As String = Databasic.Defaults.KEY_NAME) As KeyColumns
			Dim result As KeyColumns = New KeyColumns
			If classMetaDescription.PrimaryColumns.ContainsKey(keyName) Then
				result.Type = KeyType.Primary
				result.Columns = classMetaDescription.PrimaryColumns(keyName)
			ElseIf classMetaDescription.UniqueColumns.ContainsKey(keyName) Then
				result.Type = KeyType.Unique
				result.Columns = classMetaDescription.UniqueColumns(keyName)
			Else
				Throw New Exception(String.Format(
				"Class '{0}' has no properties or fields with 'PrimaryKey' or 'UniqueKey' attributes defined. " +
				"Please set up this class properly by database structure to process this operation again.",
				classMetaDescription.ClassType.FullName
			))
			End If
			result.AutoIncrementColumn = classMetaDescription.AutoIncrementColumn
			Return result
		End Function
		Public Shared Function KeyColumns(resourceType As Type, keyType As KeyType, Optional keyName As String = Databasic.Defaults.KEY_NAME) As KeyColumns
			Dim result As KeyColumns = New KeyColumns With {
				.Type = keyType
			}
			Dim allKeyCols As AllKeyColumns = MetaDescriptor.GetAllKeyColumns(resourceType, keyType)
			If keyType = KeyType.Primary AndAlso allKeyCols.PrimaryColumns.ContainsKey(keyName) Then
				result.Columns = allKeyCols.PrimaryColumns(keyName)
			ElseIf keyType = KeyType.Unique AndAlso allKeyCols.UniqueColumns.ContainsKey(keyName) Then
				result.Columns = allKeyCols.UniqueColumns(keyName)
			Else
				Throw New Exception(String.Format(
				"Class '{0}' has no properties or fields with '{1}' attribute defined. " +
				"Please set up this class properly by database structure to process this operation again.",
				resourceType.FullName,
				If(keyType = KeyType.Primary, "PrimaryKey", "UniqueKey")
			))
			End If
			result.AutoIncrementColumn = allKeyCols.AutoIncrementColumn
			Return result
		End Function
		Public Shared Function KeyColumns(Of TResource)(Optional keyType As KeyType = KeyType.Primary, Optional keyName As String = Databasic.Defaults.KEY_NAME) As KeyColumns
			Return Resource.KeyColumns(GetType(TResource), keyType, keyName)
		End Function
		Public Function KeyColumns(Optional keyType As KeyType = KeyType.Primary, Optional keyName As String = Databasic.Defaults.KEY_NAME) As KeyColumns
			Return Resource.KeyColumns(Me.GetType(), keyType, keyName)
		End Function
		Public Shared Function KeyColumns(Of TResource)(keyName As String, Optional keyType As KeyType = KeyType.Primary) As KeyColumns
			Return Resource.KeyColumns(GetType(TResource), keyType, keyName)
		End Function
		Public Function KeyColumns(keyName As String, Optional keyType As KeyType = KeyType.Primary) As KeyColumns
			Return Resource.KeyColumns(Me.GetType(), keyType, keyName)
		End Function





	End Class
End Namespace