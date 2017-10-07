Imports System.ComponentModel
Imports System.Dynamic

Namespace ActiveRecord
    Partial Public MustInherit Class Entity
        Inherits DynamicObject

        Public Shared Function GetList(Of TValue)(
            Optional conditionSqlStatement As String = "",
            Optional conditionParams As Object = Nothing,
            Optional orderBySqlStatement As String = "",
            Optional offset As Int64? = Nothing,
            Optional limit As Int64? = Nothing,
            Optional connectionIndex As Int32 = Databasic.Defaults.CONNECTION_INDEX
        ) As List(Of TValue)
            Return Entity.GetList(Of TValue)(
                conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
                Databasic.Connection.Get(connectionIndex)
            )
        End Function
        Public Shared Function GetList(Of TValue)(
            Optional conditionSqlStatement As String = "",
            Optional conditionParams As Dictionary(Of String, Object) = Nothing,
            Optional orderBySqlStatement As String = "",
            Optional offset As Int64? = Nothing,
            Optional limit As Int64? = Nothing,
            Optional connectionIndex As Int32 = Databasic.Defaults.CONNECTION_INDEX
        ) As List(Of TValue)
            Return Entity.GetList(Of TValue)(
                conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
                Databasic.Connection.Get(connectionIndex)
            )
        End Function
        Public Shared Function GetList(Of TValue)(
            conditionSqlStatement As String,
            conditionParams As Object,
            orderBySqlStatement As String,
            offset As Int64?,
            limit As Int64?,
            connectionName As String
        ) As List(Of TValue)
            Return Entity.GetList(Of TValue)(
                conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
                Databasic.Connection.Get(If(String.IsNullOrEmpty(connectionName), Databasic.Defaults.CONNECTION_INDEX, connectionName))
            )
        End Function
        Public Shared Function GetList(Of TValue)(
            conditionSqlStatement As String,
            conditionParams As Dictionary(Of String, Object),
            orderBySqlStatement As String,
            offset As Int64?,
            limit As Int64?,
            connectionName As String
        ) As List(Of TValue)
            Return Entity.GetList(Of TValue)(
                conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
                Databasic.Connection.Get(If(String.IsNullOrEmpty(connectionName), Databasic.Defaults.CONNECTION_INDEX, connectionName))
            )
        End Function
        Public Shared Function GetList(Of TValue)(
            conditionSqlStatement As String,
            conditionParams As Object,
            orderBySqlStatement As String,
            offset As Int64?,
            limit As Int64?,
            connection As Connection
        ) As List(Of TValue)
            Return connection.GetProviderResource().GetList(
                conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit, connection,
                MetaDescriptor.GetClassDescription(GetType(TValue))
            ).ToList(Of TValue)
        End Function

    End Class
End Namespace