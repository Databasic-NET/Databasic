Imports System.Data.Common
Imports Databasic.ActiveRecord

Public MustInherit Class Statement

	''' <summary>
	''' Currently prepared and executed SQL command.
	''' </summary>
	Public Overridable Property Command As DbCommand
	''' <summary>
	''' Currently executed data reader from SQL command.
	''' </summary>
	Public Overridable Property Reader As DbDataReader




	''' <summary>
	''' Empty SQL statement constructor.
	''' </summary>
	''' <param name="sql">SQL statement code.</param>
	''' <param name="connection">Connection instance.</param>
	Public Sub New(sql As String, connection As DbConnection)
	End Sub
	''' <summary>
	''' Empty SQL statement constructor.
	''' </summary>
	''' <param name="sql">SQL statement code.</param>
	''' <param name="transaction">SQL transaction instance with connection instance inside.</param>
	Public Sub New(sql As String, transaction As DbTransaction)
	End Sub





	''' <summary>
	''' Create proper type of SQL statement by connection type.
	''' </summary>
	''' <param name="sql">SQL statement code.</param>
	''' <param name="connection">Connection instance.</param>
	''' <returns>New specificly typed SQL statement.</returns>
	Friend Shared Function PrepareLocal(sql As String, connection As Connection) As Statement
		Return Activator.CreateInstance(
		 connection.Statement,
		 New Object() {sql, If(
		  TypeOf connection.OpenedTransaction Is Transaction,
		  DirectCast(connection.OpenedTransaction, Object),
		  DirectCast(connection.Provider, Object)
		 )}
		)
	End Function
	''' <summary>
	''' Create proper type of SQL statement by connection type.
	''' </summary>
	''' <param name="sql">SQL statement code.</param>
	''' <param name="transaction">SQL transaction instance with connection instance inside.</param>
	''' <returns>New specificly typed SQL statement.</returns>
	Friend Shared Function PrepareLocal(sql As String, transaction As Transaction) As Statement
		Return Activator.CreateInstance(
			transaction.ConnectionWrapper.Statement,
			New Object() {sql, transaction.Instance}
		)
	End Function





	''' <summary>
	''' Create and prepare database SQL statement. Put '@' char before all param names in your SQL code.
	''' </summary>
	''' <param name="sql">SQL code for statement. Put '@' char before all param names in your SQL code.</param>
	''' <param name="connectionIndex">
	''' Database connection index from App|Web.config to use specific database connection, 
	''' default value is 0 to use first connection settings subnode from &lt;connectionStrings&gt; config node.
	''' </param>
	''' <returns>New specificly typed SQL statement by connection.</returns>
	Public Shared Function Prepare(sql As String, Optional connectionIndex As Int32? = Nothing) As Statement
		If Not connectionIndex.HasValue Then connectionIndex = Tools.GetConnectionIndexByClassAttr(Tools.GetEntryClassType(), False)
		Return Statement.PrepareLocal(sql, Connection.Get(connectionIndex.Value))
	End Function
	''' <summary>
	''' Create and prepare database SQL statement. Put '@' char before all param names in your SQL code.
	''' </summary>
	''' <param name="sql">SQL code for statement. Put '@' char before all param names in your SQL code.</param>
	''' Database connection name from App|Web.config to use specific database connection, 
	''' default value is 'DefaultConnection' to use default connection settings subnode from &lt;connectionStrings&gt; config node.
	''' <returns>New specificly typed SQL statement by connection.</returns>
	Public Shared Function Prepare(sql As String, connectionName As String) As Statement
		Return Statement.PrepareLocal(sql, Connection.Get(connectionName))
	End Function
	''' <summary>
	''' Create and prepare database SQL statement. Put '@' char before all param names in your SQL code.
	''' </summary>
	''' <param name="sql">SQL code for statement. Put '@' char before all param names in your SQL code.</param>
	''' <param name="connection">Your specific database connection instance to execute this SQL statement inside.</param>
	''' <returns>New specificly typed SQL statement by connection.</returns>
	Public Shared Function Prepare(sql As String, connection As Connection) As Statement
		Return Statement.PrepareLocal(sql, connection)
	End Function
	''' <summary>
	''' Create and prepare database SQL statement. Put '@' char before all param names in your SQL code.
	''' Created database statement will be executed in passed transaction.
	''' </summary>
	''' <param name="sql">SQL code for statement. Put '@' char before all param names in your SQL code.</param>
	''' <param name="transaction">Database transaction from current connection to execute this SQL statement inside.</param>
	''' <returns>New specificly typed SQL statement by connection.</returns>
	Public Shared Function Prepare(sql As String, transaction As Transaction) As Statement
		Return Statement.PrepareLocal(sql, transaction)
	End Function





	''' <summary>
	''' Execute SQL statement and open data reader to get only first single row from select statement result.
	''' </summary>
	''' <returns>SQL statement instance with opened data reader.</returns>
	Public Function FetchOne() As Statement
		Try
			Me.Reader = Me.Command.ExecuteReader(CommandBehavior.SingleRow)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return Me
	End Function
	''' <summary>
	''' Execute SQL statement and open data reader to get only first single row from select statement result.
	''' </summary>
	''' <param name="sqlParams">Anonymous object with named keys as SQL statement params without any '@' chars in object keys.</param>
	''' <returns>SQL statement instance with opened data reader.</returns>
	Public Function FetchOne(sqlParams As Object) As Statement
		Me.addParamsWithValue(sqlParams)
		Try
			Me.Reader = Me.Command.ExecuteReader(CommandBehavior.SingleRow)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return Me
	End Function
	''' <summary>
	''' Execute SQL statement and open data reader to get only first single row from select statement result.
	''' </summary>
	''' <param name="sqlParams">Dictionary with named keys as SQL statement params without any '@' chars in dictionary keys.</param>
	''' <returns>SQL statement instance with opened data reader.</returns>
	Public Function FetchOne(sqlParams As Dictionary(Of String, Object)) As Statement
		Me.addParamsWithValue(sqlParams)
		Try
			Me.Reader = Me.Command.ExecuteReader(CommandBehavior.SingleRow)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return Me
	End Function






	''' <summary>
	''' Execute SQL statement and open data reader to get all rows from select statement result.
	''' </summary>
	''' <returns>SQL statement instance with opened data reader.</returns>
	Public Function FetchAll() As Statement
		Return Me.FetchAll(CommandBehavior.Default)
	End Function
	''' <summary>
	''' Execute SQL statement and open data reader to get all rows from select statement result.
	''' </summary>
	''' <param name="commandBehavior">SQL data reader command behaviour, optional.</param>
	''' <returns>SQL statement instance with opened data reader.</returns>
	Public Function FetchAll(Optional ByVal commandBehavior As CommandBehavior = CommandBehavior.Default) As Statement
		Try
			Me.Reader = Me.Command.ExecuteReader(commandBehavior)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return Me
	End Function
	''' <summary>
	''' Execute SQL statement and open data reader to get all rows from select statement result.
	''' </summary>
	''' <param name="sqlParams">Anonymous object with named keys as SQL statement params without any '@' chars in object keys.</param>
	''' <param name="commandBehavior">SQL data reader command behaviour, optional.</param>
	''' <returns>SQL statement instance with opened data reader.</returns>
	Public Function FetchAll(sqlParams As Object, Optional commandBehavior As CommandBehavior = CommandBehavior.Default) As Statement
		Me.addParamsWithValue(sqlParams)
		Try
			Me.Reader = Me.Command.ExecuteReader(commandBehavior)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return Me
	End Function
	''' <summary>
	''' Execute SQL statement and open data reader to get all rows from select statement result.
	''' </summary>
	''' <param name="sqlParams">Dictionary with named keys as SQL statement params without any '@' chars in dictionary keys.</param>
	''' <param name="commandBehavior">SQL data reader command behaviour, optional.</param>
	''' <returns>SQL statement instance with opened data reader.</returns>
	Public Function FetchAll(sqlParams As Dictionary(Of String, Object), Optional commandBehavior As CommandBehavior = CommandBehavior.Default) As Statement
		Me.addParamsWithValue(sqlParams)
		Try
			Me.Reader = Me.Command.ExecuteReader(commandBehavior)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return Me
	End Function





	''' <summary>
	''' Execute any non select SQL statement and return affected rows count.
	''' </summary>
	''' <returns>Affected rows count.</returns>
	Public Function Exec() As Int32
		Dim r As Int32 = 0
		Try
			r = Me.Command.ExecuteNonQuery()
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return r
	End Function
	''' <summary>
	''' Execute any non select SQL statement and return affected rows count.
	''' </summary>
	''' <param name="sqlParams">Anonymous object with named keys as SQL statement params without any '@' chars in object keys.</param>
	''' <returns>Affected rows count.</returns>
	Public Function Exec(sqlParams As Object) As Int32
		Me.addParamsWithValue(sqlParams)
		Return Me.Exec()
	End Function
	''' <summary>
	''' Execute any non select SQL statement and return affected rows count.
	''' </summary>
	''' <param name="sqlParams">Dictionary with named keys as SQL statement params without any '@' chars in dictionary keys.</param>
	''' <returns>Affected rows count.</returns>
	Public Function Exec(sqlParams As Dictionary(Of String, Object)) As Int32
		Me.addParamsWithValue(sqlParams)
		Dim r As Int32 = 0
		Try
			r = Me.Command.ExecuteNonQuery()
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return r
	End Function







	''' <summary>
	''' Create new instance by generic type and set up all called reader columns with one row at minimal into 
	''' new instance properties or fields. If TResult is primitive type, reader has to return single row and 
	''' single column select result and that result is converted and returned as to primitive value only.
	''' If reader has no rows, Nothing is returned.
	''' </summary>
	''' <typeparam name="TResult">New result class instance type or any primitive type for single row and single column select result.</typeparam>
	''' <returns>New instance by generic type with values by generic argument.</returns>
	Public Function ToInstance(Of TResult)() As TResult
		Return ActiveRecord.Entity.ToInstance(Of TResult)(Me.Reader)
	End Function
	''' <summary>
	''' Create List of desired class instance types or List of variables from singlerow 
	''' or multirow and single column or multi column select result.
	''' Specify result class instance type or result variable type by generic argument.
	''' If reader has no rows, empty list is returned.
	''' </summary>
	''' <typeparam name="TActiveRecord">Result List item generic type.</typeparam>
	''' <returns>List of new instances/variables by generic type with values by generic argument.</returns>
	Public Function ToList(Of TActiveRecord)() As List(Of TActiveRecord)
		Return ActiveRecord.Entity.ToList(Of TActiveRecord)(Me.Reader)
	End Function
	''' <summary>
	''' Create Dictionary of values by desired class instance types or Dictionary of values by variables 
	''' from singlerow or multirow and single column or multi column select result. 
	''' Specify result Dictionary key type by first generic argument.
	''' Specify result Dictionary value class instance type or result Dictionary value variable type by second generic argument.
	''' Specify which column from select result to use to complete dictionary keys by first string param.
	''' If reader has no rows, empty Dictionary is returned.
	''' </summary>
	''' <typeparam name="TKey">Result Dictionary generic type to complete Dictionary keys.</typeparam>
	''' <typeparam name="TActiveRecord">Result Dictionary generic type to complete Dictionary values.</typeparam>
	''' <param name="databaseKeyColumnName">Reader column name to use to complete result dictionary keys.</param>
	''' <param name="throwExceptionInDuplicateKey">True to thrown Exception if any previous key will be founded by completing the result Dictionary, False to overwrite any previous value in Dictionary, True by default.</param>
	''' <returns>Dictionary of new instances/variables by generic type with values by generic argument.</returns>
	Public Function ToDictionary(Of TKey, TActiveRecord)(Optional databaseKeyColumnName As String = "", Optional throwExceptionInDuplicateKey As Boolean = True) As Dictionary(Of TKey, TActiveRecord)
		Return ActiveRecord.Entity.ToDictionary(Of TKey, TActiveRecord)(Me.Reader, databaseKeyColumnName, throwExceptionInDuplicateKey)
	End Function
	''' <summary>
	''' Create new Dictionary with keys by first generic type and instances (values) by second generic type 
	''' and set up all called reader columns into new instances properties or fields. By first param as anonymous function,
	''' specify which field/property from active record instance to use to complete dictionary key for each item.
	''' If reader has no rows, empty dictionary is returned.
	''' </summary>
	''' <typeparam name="TKey">Result dictionary generic type to complete dictionary keys.</typeparam>
	''' <typeparam name="TActiveRecord">Result dictionary generic type to complete dictionary values.</typeparam>
	''' <param name="keySelector">Anonymous function accepting first argument as TActiveRecord instance and returning it's specific field/property value to complete Dictionary key.</param>
	''' <param name="throwExceptionInDuplicateKey">True to thrown Exception if any previous key will be founded by filling the result, false to overwrite any previous value.</param>
	''' <returns>Dictionary with keys completed by second anonymous function, values completed by reader columns with the same names as TActiveRecord type fields/properties.</returns>
	Public Function ToDictionary(Of TKey, TActiveRecord)(keySelector As Func(Of TActiveRecord, TKey), Optional throwExceptionInDuplicateKey As Boolean = True) As Dictionary(Of TKey, TActiveRecord)
		Return ActiveRecord.Entity.ToDictionary(Of TKey, TActiveRecord)(Me.Reader, keySelector, throwExceptionInDuplicateKey)
	End Function





	''' <summary>
	''' Set up all sql params into internal Command instance.
	''' </summary>
	''' <param name="sqlParams">Anonymous object with named keys as SQL statement params without any '@' chars in object keys.</param>
	Protected MustOverride Sub addParamsWithValue(sqlParams As Object)
	''' <summary>
	''' Set up all sql params into internal Command instance.
	''' </summary>
	''' <param name="sqlParams">Dictionary with named keys as SQL statement params without any '@' chars in dictionary keys.</param>
	Protected MustOverride Sub addParamsWithValue(sqlParams As Dictionary(Of String, Object))





End Class