Partial Public MustInherit Class Statement

	Public Function ToInstance(
		instanceType As Type
	) As Object
		Dim result As Object = Nothing
		Try
			result = ActiveRecord.Entity.ToInstance(
				Me.Reader, instanceType
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToInstance(
		instanceCompleter As InstanceCompleter
	) As Object
		Dim result As Object = Nothing
		Try
			result = ActiveRecord.Entity.ToInstance(
				Me.Reader, instanceCompleter
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToInstance(
		instanceCompleter As InstanceCompleterWithColumns
	) As Object
		Dim result As Object = Nothing
		Try
			result = ActiveRecord.Entity.ToInstance(
				Me.Reader,
				instanceCompleter
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToInstance(
		instanceCompleter As InstanceCompleterWithAllInfo,
		instanceType As Type,
		Optional propertiesOnly As Boolean = True
	) As Object
		Dim result As Object = Nothing
		Try
			result = ActiveRecord.Entity.ToInstance(
				Me.Reader, instanceCompleter, instanceType, propertiesOnly
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

End Class