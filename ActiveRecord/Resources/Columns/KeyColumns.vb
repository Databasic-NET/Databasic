Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace ActiveRecord
	Partial Public Class Resource

		Public Shared Function KeyColumns(Of TResource)(
			Optional keyType As KeyType = KeyType.Primary,
			Optional keyName As String = Databasic.Defaults.KEY_NAME
		) As KeyColumns
			Return Resource.KeyColumns(GetType(TResource), keyType, keyName)
		End Function

		Public Function KeyColumns(
			Optional keyType As KeyType = KeyType.Primary,
			Optional keyName As String = Databasic.Defaults.KEY_NAME
		) As KeyColumns
			Return Resource.KeyColumns(Me.GetType(), keyType, keyName)
		End Function

		Public Shared Function KeyColumns(Of TResource)(
			keyName As String,
			Optional keyType As KeyType = KeyType.Primary
		) As KeyColumns
			Return Resource.KeyColumns(GetType(TResource), keyType, keyName)
		End Function

		Public Function KeyColumns(
			keyName As String,
			Optional keyType As KeyType = KeyType.Primary
		) As KeyColumns
			Return Resource.KeyColumns(Me.GetType(), keyType, keyName)
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
				Throw New Exception(
					$"Class '{resourceType.FullName}' has no properties or fields with " +
					$"'{If(keyType = KeyType.Primary, "PrimaryKey", "UniqueKey")}' " +
					"attribute defined. Please set up this class properly by database " +
					"structure to process this operation again."
				)
			End If
			result.AutoIncrementColumn = allKeyCols.AutoIncrementColumn
			Return result
		End Function

		Public Shared Function KeyColumns(
			ByRef resourceType As Type,
			Optional keyName As String = Databasic.Defaults.KEY_NAME
		) As KeyColumns
			Dim result As KeyColumns = New KeyColumns
			Dim allKeyCols As AllKeyColumns = MetaDescriptor.GetAllKeyColumns(
				resourceType
			)
			If allKeyCols.PrimaryColumns.ContainsKey(keyName) Then
				result.Type = KeyType.Primary
				result.Columns = allKeyCols.PrimaryColumns(keyName)
			ElseIf allKeyCols.UniqueColumns.ContainsKey(keyName) Then
				result.Type = KeyType.Unique
				result.Columns = allKeyCols.UniqueColumns(keyName)
			Else
				Throw New Exception(
					$"Class '{resourceType.FullName}' has no properties or fields with 'PrimaryKey' or 'UniqueKey' attributes defined. " +
					"Please set up this class properly by database structure to process this operation again."
				)
			End If
			result.AutoIncrementColumn = allKeyCols.AutoIncrementColumn
			Return result
		End Function

		Public Shared Function KeyColumns(
			ByRef classMetaDescription As MetaDescription,
			Optional keyName As String = Databasic.Defaults.KEY_NAME
		) As KeyColumns
			Dim result As KeyColumns = New KeyColumns
			If classMetaDescription.PrimaryColumns.ContainsKey(keyName) Then
				result.Type = KeyType.Primary
				result.Columns = classMetaDescription.PrimaryColumns(keyName)
			ElseIf classMetaDescription.UniqueColumns.ContainsKey(keyName) Then
				result.Type = KeyType.Unique
				result.Columns = classMetaDescription.UniqueColumns(keyName)
			Else
				Throw New Exception(
					$"Class '{classMetaDescription.ClassType.FullName}' has no " +
					"properties or fields with 'PrimaryKey' or 'UniqueKey' attributes " +
					"defined. Please set up this class properly by database structure " +
					"to process this operation again."
				)
			End If
			result.AutoIncrementColumn = classMetaDescription.AutoIncrementColumn
			Return result
		End Function

	End Class
End Namespace