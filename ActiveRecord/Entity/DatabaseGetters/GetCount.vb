Imports System.ComponentModel
Imports System.Dynamic

Namespace ActiveRecord
	Partial Public MustInherit Class Entity
		Inherits DynamicObject

		Public Shared Function GetCount(Of TValue)(
			conditionSqlStatement As String,
			sqlParams As Dictionary(Of String, Object),
			connection As Connection
		) As Long
			Return connection.GetProviderResource().GetCount(
				conditionSqlStatement, sqlParams, connection,
				MetaDescriptor.GetClassDescription(GetType(TValue))
			)
		End Function

		Public Shared Function GetCount(Of TValue)(
			conditionSqlStatement As String,
			sqlParams As Dictionary(Of String, Object),
			transaction As Transaction
		) As Long
			Return transaction.ConnectionWrapper.GetProviderResource().GetCount(
				conditionSqlStatement, sqlParams, transaction,
				MetaDescriptor.GetClassDescription(GetType(TValue))
			)
		End Function

		Public Shared Function GetCount(Of TValue)(
			conditionSqlStatement As String,
			sqlParams As Dictionary(Of String, Object),
			Optional connectionIndex As Integer? = Nothing
		) As Long
			Return Entity.GetCount(Of TValue)(
				conditionSqlStatement, sqlParams, Connection.Get(If(
					connectionIndex.HasValue,
					connectionIndex.Value,
					Tools.GetConnectionIndexByClassAttr(GetType(TValue), True)
				))
			)
		End Function

		Public Shared Function GetCount(Of TValue)(
			conditionSqlStatement As String,
			sqlParams As Object,
			connection As Connection
		) As Long
			Return connection.GetProviderResource().GetCount(
				conditionSqlStatement, sqlParams, connection,
				MetaDescriptor.GetClassDescription(GetType(TValue))
			)
		End Function

		Public Shared Function GetCount(Of TValue)(
			conditionSqlStatement As String,
			sqlParams As Object,
			transaction As Transaction
		) As Long
			Return transaction.ConnectionWrapper.GetProviderResource().GetCount(
				conditionSqlStatement, sqlParams, transaction,
				MetaDescriptor.GetClassDescription(GetType(TValue))
			)
		End Function

		Public Shared Function GetCount(Of TValue)(
			Optional conditionSqlStatement As String = "",
			Optional sqlParams As Object = Nothing,
			Optional connectionIndex As Integer? = Nothing
		) As Long
			Return Entity.GetCount(Of TValue)(
				conditionSqlStatement, sqlParams,
				Connection.Get(If(
					connectionIndex.HasValue,
					connectionIndex.Value,
					Tools.GetConnectionIndexByClassAttr(GetType(TValue), True)
				))
			)
		End Function

	End Class
End Namespace