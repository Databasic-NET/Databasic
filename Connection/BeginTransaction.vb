Imports System.Data.Common

Partial Public MustInherit Class Connection

    ''' <summary>
    ''' Create and begin transaction on first config connection.
    ''' </summary>
    ''' <param name="transactionName">Transaction name.</param>
    ''' <param name="isolationLevel">Transaction isolation level.</param>
    ''' <returns>New transaction.</returns>
    Public Shared Function BeginTransaction(connection As Connection, transactionName As String, Optional isolationLevel As IsolationLevel = IsolationLevel.Unspecified) As Transaction
        Return connection.createAndBeginTransaction(
            transactionName, isolationLevel
        )
    End Function

    ''' <summary>
    ''' Create and begin transaction on specified connection config on entry class.
    ''' </summary>
    ''' <param name="transactionName">Transaction name.</param>
    ''' <param name="isolationLevel">Transaction isolation level.</param>
    ''' <returns>New transaction.</returns>
    Public Shared Function BeginTransaction(
        Optional transactionName As String = "",
        Optional isolationLevel As IsolationLevel = IsolationLevel.Unspecified
    ) As Transaction
        Dim connectionIndex = Tools.GetConnectionIndexByClassAttr(
            Tools.GetEntryClassType(), True
        )
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
    Public Shared Function BeginTransaction(
        connectionName As String,
        transactionName As String,
        Optional isolationLevel As IsolationLevel = IsolationLevel.Unspecified
    ) As Transaction
        Return Databasic.Connection.Get(connectionName).createAndBeginTransaction(
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

    Protected Overridable Function createAndBeginTransaction(Optional transactionName As String = "", Optional isolationLevel As IsolationLevel = -1) As Transaction
        Return Nothing
    End Function

End Class
