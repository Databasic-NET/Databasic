Imports System.Data.Common
Imports Databasic.ActiveRecord

Partial Public MustInherit Class Statement
	''' <summary>
	''' Execute SQL statement and open data reader to get all rows from select statement result.
	''' </summary>
	''' <param name="commandBehavior">SQL data reader command behaviour, optional.</param>
	''' <returns>SQL statement instance with opened data reader.</returns>
	Public Function FetchAll(Optional commandBehavior As CommandBehavior = CommandBehavior.Default) As Statement
		Me.commandBehavior = CommandBehavior.Default
		If (commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
			commandBehavior = commandBehavior And Not CommandBehavior.SingleRow
		End If
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
		Me.commandBehavior = CommandBehavior.Default
		If (commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
			commandBehavior = commandBehavior And Not CommandBehavior.SingleRow
		End If
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
		Me.commandBehavior = CommandBehavior.Default
		If (commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
			commandBehavior = commandBehavior And Not CommandBehavior.SingleRow
		End If
		Try
			Me.Reader = Me.Command.ExecuteReader(commandBehavior)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return Me
	End Function
End Class