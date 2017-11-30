Partial Public MustInherit Class Statement

	Public Function ToList(
		itemType As Type
	) As List(Of Object)
		Dim result As New List(Of Object)
		Try
			result = ActiveRecord.Entity.ToList(
				Me.Reader, itemType
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToList(
		itemCompleter As ItemCompleter
	) As List(Of Object)
		Dim result As New List(Of Object)
		Try
			result = ActiveRecord.Entity.ToList(
				Me.Reader, itemCompleter
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToList(
		itemCompleter As ItemCompleterWithColumns
	) As List(Of Object)
		Dim result As New List(Of Object)
		Try
			result = ActiveRecord.Entity.ToList(
				Me.Reader, itemCompleter
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToList(
		itemCompleter As ItemCompleterWithAllInfo,
		instanceType As Type,
		Optional propertiesOnly As Boolean = True
	) As List(Of Object)
		Dim result As New List(Of Object)
		Try
			result = ActiveRecord.Entity.ToList(
				Me.Reader, itemCompleter, instanceType, propertiesOnly
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

End Class