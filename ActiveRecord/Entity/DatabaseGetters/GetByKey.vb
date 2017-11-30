Imports System.ComponentModel
Imports System.Dynamic

Namespace ActiveRecord
	Partial Public MustInherit Class Entity
		Inherits DynamicObject

		Public Shared Function GetByKey(Of TValue)(
			keyValues As Dictionary(Of String, Object),
			keyName As String,
			connection As Connection
		) As TValue
			Return Entity._getByKey(Of TValue)(
				keyValues, keyName, connection, True
			)
		End Function

		Public Shared Function GetByKey(Of TValue)(
			keyValues As Dictionary(Of String, Object),
			keyName As String,
			transaction As Transaction
		) As TValue
			Return Entity._getByKey(Of TValue)(
				keyValues, keyName, transaction, False
			)
		End Function

		Public Shared Function GetByKey(Of TValue)(
			keyValues As Dictionary(Of String, Object),
			keyName As String,
			Optional connectionIndex As Integer? = Nothing
		) As TValue
			If Not connectionIndex.HasValue Then
				connectionIndex = Tools.GetConnectionIndexByClassAttr(
					GetType(TValue), True
				)
			End If
			Return Entity._getByKey(Of TValue)(
				keyValues, keyName, Connection.Get(connectionIndex.Value), True
			)
		End Function

		Public Shared Function GetByKey(Of TValue)(
			keyValues As Dictionary(Of String, Object),
			keyName As String,
			connectionName As String
		) As TValue
			Return Entity._getByKey(Of TValue)(
				keyValues, keyName, Connection.Get(connectionName), True
			)
		End Function

		Public Shared Function GetByKey(Of TValue)(
			keyValues As Object,
			keyName As String,
			connection As Connection
		) As TValue
			Return Entity._getByKey(Of TValue)(
				Entity._getByKeyValuesDct(keyValues), keyName, connection, True
			)
		End Function

		Public Shared Function GetByKey(Of TValue)(
			keyValues As Object,
			keyName As String,
			transaction As Transaction
		) As TValue
			Return Entity._getByKey(Of TValue)(
				Entity._getByKeyValuesDct(keyValues), keyName, transaction, False
			)
		End Function

		Public Shared Function GetByKey(Of TValue)(
			keyValues As Object,
			Optional keyName As String = "default",
			Optional connectionIndex As Integer? = Nothing
		) As TValue
			If Not connectionIndex.HasValue Then
				connectionIndex = Tools.GetConnectionIndexByClassAttr(
					GetType(TValue), True
				)
			End If
			Return Entity._getByKey(Of TValue)(
				Entity._getByKeyValuesDct(keyValues), keyName,
				Connection.Get(connectionIndex.Value), True
			)
		End Function

		Public Shared Function GetByKey(Of TValue)(
			keyValues As Object,
			keyName As String,
			connectionName As String
		) As TValue
			Return Entity._getByKey(Of TValue)(
				Entity._getByKeyValuesDct(keyValues), keyName,
				Connection.Get(connectionName), True
			)
		End Function

		Private Shared Function _getByKeyValuesDct(
			keyValues As Object
		) As Dictionary(Of String, Object)
			Dim result As New Dictionary(Of String, Object)
			For Each prop As PropertyDescriptor In TypeDescriptor.GetProperties(keyValues)
				result.Add(prop.Name, prop.GetValue(keyValues))
			Next
			Return result
		End Function

		Private Shared Function _getByKey(Of TValue)(
			keyValues As Dictionary(Of String, Object),
			keyName As String,
			connectionOrTransaction As Object,
			connectionTypeGiven As Boolean
		) As TValue
			Dim type As Type = GetType(TValue)
			Dim classDescription As MetaDescription = MetaDescriptor.GetClassDescription(type)
			Dim dictionary As New Dictionary(Of String, String)
			If classDescription.PrimaryColumns.ContainsKey(keyName) Then
				dictionary = classDescription.PrimaryColumns.Item(keyName)
			ElseIf classDescription.UniqueColumns.ContainsKey(keyName) Then
				dictionary = classDescription.UniqueColumns.Item(keyName)
			Else
				Events.RaiseError(If(
					keyName.Length > 0,
					$"Class '{classDescription.ClassType}' has no member(s) with 'UniqueKey(""{keyName}"")' or 'PrimaryKey(""{keyName}"")' attribute(s).",
					$"Class '{classDescription.ClassType}' has no member(s) with 'UniqueKey' or 'PrimaryKey' attribute(s)."
				))
			End If
			Dim keyParams As New Dictionary(Of String, Object)
			Dim pair As KeyValuePair(Of String, String)
			For Each pair In dictionary
				If keyValues.ContainsKey(pair.Key) Then
					keyParams.Add(pair.Value, keyValues.Item(pair.Key))
				Else
					Events.RaiseError(
						$"Key value '{pair.Key}' to load '{type.FullName}' by key '{keyName}' is missing, please put any record '{pair.Key}' into first param 'keyValues' to load data properly."
					)
				End If
			Next
			If connectionTypeGiven Then
				Return Entity._getByKeyResult(Of TValue)(
					keyParams,
					DirectCast(connectionOrTransaction, Connection),
					classDescription
				)
			End If
			Return Entity._getByKeyResult(Of TValue)(
				keyParams,
				DirectCast(connectionOrTransaction, Transaction),
				classDescription
			)
		End Function

		Private Shared Function _getByKeyResult(Of TValue)(ByVal keyParams As Dictionary(Of String, Object), ByVal connection As Connection, ByRef classMetaDescription As MetaDescription) As TValue
			Return Entity.ToInstance(Of TValue)(
				connection.GetProviderResource().GetByKey(
					keyParams, "", connection, classMetaDescription
				).Reader,
				classMetaDescription.ColumnsByDatabaseNames
			)
		End Function

		Private Shared Function _getByKeyResult(Of TValue)(
			keyParams As Dictionary(Of String, Object),
			transaction As Transaction,
			ByRef classMetaDescription As MetaDescription
		) As TValue
			Return Entity.ToInstance(Of TValue)(
				transaction.ConnectionWrapper.GetProviderResource().GetByKey(
					keyParams, "", transaction, classMetaDescription
				).Reader,
				classMetaDescription.ColumnsByDatabaseNames
			)
		End Function

	End Class
End Namespace