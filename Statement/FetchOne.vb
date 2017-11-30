Imports System.Data.Common
Imports Databasic.ActiveRecord

Partial Public MustInherit Class Statement
	''' <summary>
	''' Execute SQL statement and open data reader to get only first single row from select statement result.
	''' </summary>
	''' <returns>SQL statement instance with opened data reader.</returns>
	Public Function FetchOne(Optional commandBehavior As CommandBehavior = CommandBehavior.SingleRow) As Statement
		Me.commandBehavior = CommandBehavior.SingleRow
		If (Not commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
			commandBehavior = commandBehavior Or CommandBehavior.SingleRow
		End If
		Try
			Me.Reader = Me.Command.ExecuteReader(commandBehavior)
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
	Public Function FetchOne(sqlParams As Object, Optional commandBehavior As CommandBehavior = CommandBehavior.SingleRow) As Statement
		Me.addParamsWithValue(sqlParams)
		Me.commandBehavior = CommandBehavior.SingleRow
		If (Not commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
			commandBehavior = commandBehavior Or CommandBehavior.SingleRow
		End If
		Try
			Me.Reader = Me.Command.ExecuteReader(commandBehavior)
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
	Public Function FetchOne(sqlParams As Dictionary(Of String, Object), Optional commandBehavior As CommandBehavior = CommandBehavior.SingleRow) As Statement
		Me.addParamsWithValue(sqlParams)
		Me.commandBehavior = CommandBehavior.SingleRow
		If (Not commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
			commandBehavior = commandBehavior Or CommandBehavior.SingleRow
		End If
		Try
			Me.Reader = Me.Command.ExecuteReader(commandBehavior)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return Me
	End Function
End Class