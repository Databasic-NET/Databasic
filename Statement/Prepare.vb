Imports System.Data.Common
Imports Databasic.ActiveRecord

Partial Public MustInherit Class Statement
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
	''' Create and prepare database SQL statement. Put '@' char before all param names in your SQL code.
	''' Created database statement will be executed in passed transaction.
	''' </summary>
	''' <param name="sql">SQL code for statement. Put '@' char before all param names in your SQL code.</param>
	''' <param name="connectionOrTransaction">Database connection or database transaction from current connection to execute this SQL statement inside.</param>
	''' <returns>New specificly typed SQL statement by connection.</returns>
	Public Shared Function Prepare(sql As String, connectionOrTransaction As Object) As Statement
		If TypeOf connectionOrTransaction Is Databasic.Connection Then
			Return Statement.PrepareLocal(sql, DirectCast(connectionOrTransaction, Databasic.Connection))
		Else
			Return Statement.PrepareLocal(sql, DirectCast(connectionOrTransaction, Databasic.Transaction))
		End If
	End Function

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
End Class