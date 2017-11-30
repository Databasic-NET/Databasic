Partial Public MustInherit Class Statement

	''' <summary>
	''' Create new instance by generic type and set up all called reader columns with one row at minimal into 
	''' new instance properties or fields. If TValue is primitive type, reader has to return single row and 
	''' single column select result and that result is converted and returned as to primitive value only.
	''' If reader has no rows, Nothing is returned.
	''' </summary>
	''' <typeparam name="TValue">New result class instance type or any primitive type for single row and single column select result.</typeparam>
	''' <returns>New instance by generic type with values by generic argument.</returns>
	Public Function ToInstance(Of TValue)(
		Optional instanceCompleter As InstanceCompleter(
			Of TValue
		) = Nothing
	) As TValue
		Dim result As Object = Nothing
		Try
			If (instanceCompleter = Nothing) Then
				result = ActiveRecord.Entity.ToInstance(Of TValue)(Me.Reader)
			Else
				result = ActiveRecord.Entity.ToInstance(Of TValue)(Me.Reader, instanceCompleter)
			End If
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToInstance(Of TValue)(
		instanceCompleter As InstanceCompleterWithColumns(
			Of TValue
		)
	) As TValue
		Dim result As Object = Nothing
		Try
			result = ActiveRecord.Entity.ToInstance(Of TValue)(
				Me.Reader,
				instanceCompleter
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToInstance(Of TValue)(
		instanceCompleter As InstanceCompleterWithAllInfo(
			Of TValue
		),
		Optional propertiesOnly As Boolean = True
	) As TValue
		Dim result As Object = Nothing
		Try
			result = ActiveRecord.Entity.ToInstance(Of TValue)(
				Me.Reader, instanceCompleter, propertiesOnly
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

End Class