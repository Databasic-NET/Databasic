Imports System.ComponentModel
Imports System.Dynamic

Namespace ActiveRecord
	Partial Public MustInherit Class Entity
		Inherits DynamicObject

		Public Shared Function GetDictionary(Of TKey, TValue)(
			Optional databaseKeyColumnName As String = "",
			Optional conditionSqlStatement As String = "",
			Optional conditionParams As Object = Nothing,
			Optional orderBySqlStatement As String = "",
			Optional offset As Int64? = Nothing,
			Optional limit As Int64? = Nothing,
			Optional connectionIndex As Int32? = Nothing
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				databaseKeyColumnName, conditionSqlStatement,
				conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(If(
					connectionIndex.HasValue,
					connectionIndex.Value,
					Tools.GetConnectionIndexByClassAttr(GetType(TValue), True)
				))
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			Optional databaseKeyColumnName As String = "",
			Optional conditionSqlStatement As String = "",
			Optional conditionParams As Dictionary(Of String, Object) = Nothing,
			Optional orderBySqlStatement As String = "",
			Optional offset As Int64? = Nothing,
			Optional limit As Int64? = Nothing,
			Optional connectionIndex As Int32? = Nothing
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				databaseKeyColumnName, conditionSqlStatement,
				conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(If(
					connectionIndex.HasValue,
					connectionIndex.Value,
					Tools.GetConnectionIndexByClassAttr(GetType(TValue), True)
				))
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			databaseKeyColumnName As String,
			conditionSqlStatement As String,
			conditionParams As Object,
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connectionName As String
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				databaseKeyColumnName, conditionSqlStatement,
				conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(If(
					String.IsNullOrEmpty(connectionName),
					Tools.GetConnectionIndexByClassAttr(GetType(TValue), True),
					connectionName
				))
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			databaseKeyColumnName As String,
			conditionSqlStatement As String,
			conditionParams As Dictionary(Of String, Object),
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connectionName As String
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				databaseKeyColumnName, conditionSqlStatement,
				conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(If(
					String.IsNullOrEmpty(connectionName),
					Tools.GetConnectionIndexByClassAttr(GetType(TValue), True),
					connectionName
				))
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			databaseKeyColumnName As String,
			conditionSqlStatement As String,
			conditionParams As Object,
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connection As Connection
		) As Dictionary(Of TKey, TValue)
			Return connection.GetProviderResource().GetList(
				conditionSqlStatement, conditionParams,
				orderBySqlStatement, offset, limit, connection,
				MetaDescriptor.GetClassDescription(GetType(TValue))
			).ToDictionary(Of TKey, TValue)(databaseKeyColumnName)
		End Function

		Public Shared Function GetDictionary(Of TKey, TValue)(
			keySelector As Func(Of TValue, TKey),
			conditionSqlStatement As String,
			conditionParams As Object,
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			Optional connectionIndex As Int32? = Nothing
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				 keySelector, conditionSqlStatement,
				 conditionParams, orderBySqlStatement, offset, limit,
				 Databasic.Connection.Get(If(
					connectionIndex.HasValue,
					connectionIndex.Value,
					Tools.GetConnectionIndexByClassAttr(GetType(TValue), True)
				))
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			keySelector As Func(Of TValue, TKey),
			conditionSqlStatement As String,
			conditionParams As Dictionary(Of String, Object),
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			Optional connectionIndex As Int32? = Nothing
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				keySelector, conditionSqlStatement, conditionParams,
				orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(If(
					connectionIndex.HasValue,
					connectionIndex.Value,
					Tools.GetConnectionIndexByClassAttr(GetType(TValue), True)
				))
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			keySelector As Func(Of TValue, TKey),
			conditionSqlStatement As String,
			conditionParams As Object,
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connectionName As String
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				keySelector, conditionSqlStatement, conditionParams,
				orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(If(
					String.IsNullOrEmpty(connectionName),
					Tools.GetConnectionIndexByClassAttr(GetType(TValue), True),
					connectionName
				))
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			keySelector As Func(Of TValue, TKey),
			conditionSqlStatement As String,
			conditionParams As Dictionary(Of String, Object),
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connectionName As String
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				keySelector, conditionSqlStatement, conditionParams,
				orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(If(
					String.IsNullOrEmpty(connectionName),
					Tools.GetConnectionIndexByClassAttr(GetType(TValue), True),
					connectionName
				))
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			keySelector As Func(Of TValue, TKey),
			conditionSqlStatement As String,
			conditionParams As Object,
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connection As Connection
		) As Dictionary(Of TKey, TValue)
			Return connection.GetProviderResource().GetList(
				conditionSqlStatement, conditionParams,
				orderBySqlStatement, offset, limit, connection,
				MetaDescriptor.GetClassDescription(GetType(TValue))
			).ToDictionary(Of TKey, TValue)(keySelector)
		End Function

	End Class
End Namespace