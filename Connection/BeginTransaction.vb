Imports System.Data.Common

Partial Public MustInherit Class Connection

 ''' <summary>
 ''' Create and begin transaction on first config connection.
 ''' </summary>
 ''' <param name="transactionName">Transaction name.</param>
 ''' <param name="isolationLevel">Transaction isolation level.</param>
 ''' <returns>New transaction.</returns>
 Public Function BeginTransaction(transactionName As String, isolationLevel As IsolationLevel) As Transaction
  Return Me.createAndBeginTransaction(
   transactionName, isolationLevel
  )
 End Function

 ''' <summary>
 ''' Create and begin transaction on specified connection config index.
 ''' </summary>
 ''' <param name="connectionIndex">Config connection index.</param>
 ''' <param name="transactionName">Transaction name.</param>
 ''' <param name="isolationLevel">Transaction isolation level.</param>
 ''' <returns>New transaction.</returns>
 Public Shared Function BeginTransaction(connectionIndex As Int32, Optional transactionName As String = "", Optional isolationLevel As IsolationLevel = IsolationLevel.Unspecified) As Transaction
  Return Databasic.Connection.Get(connectionIndex).createAndBeginTransaction(
   transactionName, isolationLevel
  )
 End Function

 ''' <summary>
 ''' Create and begin transaction on specified connection config name.
 ''' </summary>
 ''' <param name="connectionName">Config connection name.</param>
 ''' <param name="transactionName">Transaction name.</param>
 ''' <param name="isolationLevel">Transaction isolation level.</param>
 ''' <returns>New transaction.</returns>
 Public Shared Function BeginTransaction(connectionName As String, Optional transactionName As String = "", Optional isolationLevel As IsolationLevel = IsolationLevel.Unspecified) As Transaction
  Return Databasic.Connection.Get(connectionName).createAndBeginTransaction(
   transactionName, isolationLevel
  )
 End Function

 Protected Overridable Function createAndBeginTransaction(Optional transactionName As String = "", Optional isolationLevel As IsolationLevel = -1) As Transaction
		Return Nothing
	End Function

End Class
