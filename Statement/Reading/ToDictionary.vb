Partial Public MustInherit Class Statement

	Public Function ToDictionary(
		itemType As Type,
		Optional keyColumnName As String = "",
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of Object, Object)
		Dim result As New Dictionary(Of Object, Object)
		Try
			result = ActiveRecord.Entity.ToDictionary(
				Me.Reader, itemType, keyColumnName, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToDictionary(
		itemType As Type,
		Optional keySelector As Func(Of Object, Object) = Nothing,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of Object, Object)
		Dim result As New Dictionary(Of Object, Object)
		Try
			result = ActiveRecord.Entity.ToDictionary(
				Me.Reader, itemType, keySelector, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToDictionary(
		itemCompleter As ItemCompleter,
		Optional keyColumnName As String = "",
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of Object, Object)
		Dim result As New Dictionary(Of Object, Object)
		Try
			result = ActiveRecord.Entity.ToDictionary(
				Me.Reader, itemCompleter, keyColumnName, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToDictionary(
		itemCompleter As ItemCompleter,
		Optional keySelector As Func(Of Object, Object) = Nothing,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of Object, Object)
		Dim result As New Dictionary(Of Object, Object)
		Try
			result = ActiveRecord.Entity.ToDictionary(
				Me.Reader, itemCompleter, keySelector, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToDictionary(
		itemCompleter As ItemCompleterWithColumns,
		Optional keyColumnName As String = "",
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of Object, Object)
		Dim result As New Dictionary(Of Object, Object)
		Try
			result = ActiveRecord.Entity.ToDictionary(
				Me.Reader, itemCompleter, keyColumnName, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToDictionary(
		itemCompleter As ItemCompleterWithColumns,
		Optional keySelector As Func(Of Object, Object) = Nothing,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of Object, Object)
		Dim result As New Dictionary(Of Object, Object)
		Try
			result = ActiveRecord.Entity.ToDictionary(
				Me.Reader, itemCompleter, keySelector, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToDictionary(
		itemCompleter As ItemCompleterWithAllInfo,
		itemType As Type,
		Optional keyColumnName As String = "",
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of Object, Object)
		Dim result As New Dictionary(Of Object, Object)
		Try
			result = ActiveRecord.Entity.ToDictionary(
				Me.Reader, itemCompleter, itemType, keyColumnName, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToDictionary(
		itemCompleter As ItemCompleterWithAllInfo,
		itemType As Type,
		Optional keySelector As Func(Of Object, Object) = Nothing,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of Object, Object)
		Dim result As New Dictionary(Of Object, Object)
		Try
			result = ActiveRecord.Entity.ToDictionary(
				Me.Reader, itemCompleter, itemType, keySelector, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

End Class