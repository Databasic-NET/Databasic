Imports System.Data.Common
Imports Databasic.ActiveRecord

Partial Public MustInherit Class Statement
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
End Class