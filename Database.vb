Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Reflection
Imports Databasic.ActiveRecord
Imports Databasic

Public Class Database
    Public Const DEFAUT_CONNECTION_INDEX As Int32 = 0
    Public Const DEFAUT_CONNECTION_NAME As String = "DefaultConnection"
    Public Const DEFAUT_UNIQUE_COLUMN_NAME As String = "Id"





	''' <summary>
	''' Get active record entity instance by unique column, all table columns will be loaded by * SQL operator if no columns defined.
	''' </summary>
	''' <typeparam name="TValue">Model class type, inherited from ActiveRecord.</typeparam>
	''' <param name="uniqueColumnValue">Id column value.</param>
	''' <param name="connectionIndex">Config connection index to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
	''' <returns></returns>
	Public Shared Function GetById(Of TValue)(uniqueColumnValue As Object, Optional connectionIndex As Int32 = Database.DEFAUT_CONNECTION_INDEX) As TValue
        Return Database.GetById(Of TValue)(
            uniqueColumnValue, Connection.Get(connectionIndex)
        )
    End Function
    ''' <summary>
    ''' Get active record entity instance by unique column, all table columns will be loaded by * SQL operator if no columns defined.
    ''' </summary>
    ''' <typeparam name="TValue">Model class type, inherited from ActiveRecord.</typeparam>
    ''' <param name="uniqueColumnValue">Id column value.</param>
    ''' <param name="connectionName">Config connection name to use different database, default by Database.DEFAUT_CONNECTION_INDEX to use first connection in &lt;connectionStrings&gt; list.</param>
    ''' <returns></returns>
    Public Shared Function GetById(Of TValue)(uniqueColumnValue As Object, connectionName As String) As TValue
        Return Database.GetById(Of TValue)(
            uniqueColumnValue, Connection.Get(If(String.IsNullOrEmpty(connectionName), Database.DEFAUT_CONNECTION_INDEX, connectionName))
        )
    End Function
    ''' <summary>
    ''' Get active record entity instance by unique column, all table columns will be loaded by * SQL operator if no columns defined.
    ''' </summary>
    ''' <typeparam name="TValue">Model class type, inherited from ActiveRecord.</typeparam>
    ''' <param name="uniqueColumnValue">Id column value.</param>
    ''' <param name="connection">Connection intance.</param>
    ''' <returns></returns>
    Public Shared Function GetById(Of TValue)(uniqueColumnValue As Object, connection As Connection) As TValue
        Dim resultType As Type = GetType(TValue)
        Return DirectCast(Activator.CreateInstance(connection.ResourceType), Provider.Resource).GetById(
            connection,
            Resource.Table(resultType),
            String.Join(",", Resource.Columns(resultType)),
            Resource.UniqueColumn(resultType),
            uniqueColumnValue
        ).ToInstance(Of TValue)()
    End Function










    Public Shared Function GetAll(Of TValue)(
        Optional offset As Int64? = Nothing, Optional limit As Int64? = Nothing, Optional orderByStatement As String = Database.DEFAUT_UNIQUE_COLUMN_NAME, Optional connectionIndex As Int32 = Database.DEFAUT_CONNECTION_INDEX
    ) As List(Of TValue)
        Return Database.GetAll(Of TValue)(
            offset, limit, orderByStatement, Databasic.Connection.Get(connectionIndex)
        )
    End Function
    Public Shared Function GetAll(Of TValue)(
         offset As Int64?, limit As Int64?, orderByStatement As String, connectionName As String
    ) As List(Of TValue)
        Return Database.GetAll(Of TValue)(
            offset, limit, orderByStatement,
            Connection.Get(If(String.IsNullOrEmpty(connectionName), Database.DEFAUT_CONNECTION_INDEX, connectionName))
        )
    End Function
    Public Shared Function GetAll(Of TValue)(
         offset As Int64?, limit As Int64?, orderByStatement As String, connection As Connection
    ) As List(Of TValue)
        Dim resultType As Type = GetType(TValue)
        Return DirectCast(Activator.CreateInstance(connection.ResourceType), Provider.Resource).GetAll(
            connection,
            ActiveRecord.Resource.Table(resultType),
            String.Join(",", ActiveRecord.Resource.Columns(resultType)),
            offset,
            limit,
            orderByStatement
        ).ToList(Of TValue)()
    End Function

    Public Shared Function GetAll(Of TKey, TValue)(
        Optional offset As Int64? = Nothing,
        Optional limit As Int64? = Nothing,
        Optional keyColumnName As String = Database.DEFAUT_UNIQUE_COLUMN_NAME,
        Optional orderByStatement As String = Database.DEFAUT_UNIQUE_COLUMN_NAME,
        Optional connectionIndex As Int32 = Database.DEFAUT_CONNECTION_INDEX
    ) As Dictionary(Of TKey, TValue)
        Dim connection As Connection = Databasic.Connection.Get(connectionIndex)
        Return Database.GetAll(Of TKey, TValue)(
            offset, limit, keyColumnName, orderByStatement, connection
        )
    End Function
    Public Shared Function GetAll(Of TKey, TValue)(
        offset As Int64?,
        limit As Int64?,
        keyColumnName As String,
        orderByStatement As String,
        connectionName As String
    ) As Dictionary(Of TKey, TValue)
        Dim connection As Connection = Databasic.Connection.Get(If(String.IsNullOrEmpty(connectionName), Database.DEFAUT_CONNECTION_INDEX, connectionName))
        Return Database.GetAll(Of TKey, TValue)(
            offset, limit, keyColumnName, orderByStatement, connection
        )
    End Function
    Public Shared Function GetAll(Of TKey, TValue)(
        offset As Int64?,
        limit As Int64?,
        keyColumnName As String,
        orderByStatement As String,
        connection As Connection
    ) As Dictionary(Of TKey, TValue)
        Dim resultItemType As Type = GetType(TValue)
        Return DirectCast(Activator.CreateInstance(connection.ResourceType), Provider.Resource).GetAll(
            connection,
            Resource.Table(resultItemType),
            String.Join(",", Resource.Columns(resultItemType)),
            offset,
            limit,
            orderByStatement
        ).ToDictionary(Of TKey, TValue)(keyColumnName)
    End Function

    Public Shared Function GetCount(Of TValue)(Optional connectionIndex As Int32 = Database.DEFAUT_CONNECTION_INDEX) As Int64
        Return Database.GetCount(Of TValue)(Databasic.Connection.Get(connectionIndex))
    End Function
    Public Shared Function GetCount(Of TValue)(connectionName As String) As Int64
        Return Database.GetCount(Of TValue)(Databasic.Connection.Get(If(String.IsNullOrEmpty(connectionName), Database.DEFAUT_CONNECTION_INDEX, connectionName)))
    End Function
    Public Shared Function GetCount(Of TValue)(connection As Connection) As Int64
        Dim resultType As Type = GetType(TValue)
        Dim resource As Provider.Resource = Activator.CreateInstance(connection.ResourceType)
        Return resource.GetCount(
            connection,
            ActiveRecord.Resource.Table(resultType),
            ActiveRecord.Resource.UniqueColumn(resultType)
        )
    End Function

End Class
