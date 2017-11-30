Partial Public MustInherit Class Statement

	Public Function ToList(Of TItem)(
		Optional itemCompleter As ItemCompleter(
			Of TItem
		) = Nothing
	) As List(Of TItem)
		Dim result As New List(Of TItem)
		Try
			If (itemCompleter = Nothing) Then
				result = ActiveRecord.Entity.ToList(Of TItem)(Me.Reader)
			Else
				result = ActiveRecord.Entity.ToList(Of TItem)(Me.Reader, itemCompleter)
			End If
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToList(Of TItem)(
		itemCompleter As ItemCompleterWithColumns(
			Of TItem
		)
	) As List(Of TItem)
		Dim result As New List(Of TItem)
		Try
			result = ActiveRecord.Entity.ToList(Of TItem)(
				Me.Reader, itemCompleter
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToList(Of TItem)(
		itemCompleter As ItemCompleterWithAllInfo(
			Of TItem
		),
		Optional propertiesOnly As Boolean = True
	) As List(Of TItem)
		Dim result As New List(Of TItem)
		Try
			result = ActiveRecord.Entity.ToList(Of TItem)(
				Me.Reader, itemCompleter, propertiesOnly
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

End Class