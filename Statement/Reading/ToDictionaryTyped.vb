Partial Public MustInherit Class Statement
	''' <summary>
	''' Create Dictionary of values by desired class instance types or Dictionary of values by variables 
	''' from singlerow or multirow and single column or multi column select result. 
	''' Specify result Dictionary key type by first generic argument.
	''' Specify result Dictionary value class instance type or result Dictionary value variable type by second generic argument.
	''' Specify which column from select result to use to complete dictionary keys by first string param.
	''' If reader has no rows, empty Dictionary is returned.
	''' </summary>
	''' <typeparam name="TKey">Result Dictionary generic type to complete Dictionary keys.</typeparam>
	''' <typeparam name="TValue">Result Dictionary generic type to complete Dictionary values.</typeparam>
	''' <param name="keyColumnName">Reader column name to use to complete result dictionary keys.</param>
	''' <param name="duplicateKeyBehaviour">True to thrown Exception if any previous key will be founded by completing the result Dictionary, False to overwrite any previous value in Dictionary, True by default.</param>
	''' <returns>Dictionary of new instances/variables by generic type with values by generic argument.</returns>
	Public Function ToDictionary(Of TKey, TValue)(
		Optional keyColumnName As String = "",
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of TKey, TValue)
		Dim result As New Dictionary(Of TKey, TValue)
		Try
			result = ActiveRecord.Entity.ToDictionary(Of TKey, TValue)(
				Me.Reader, keyColumnName, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function
	''' <summary>
	''' Create new Dictionary with keys by first generic type and instances (values) by second generic type 
	''' and set up all called reader columns into new instances properties or fields. By first param as anonymous function,
	''' specify which field/property from active record instance to use to complete dictionary key for each item.
	''' If reader has no rows, empty dictionary is returned.
	''' </summary>
	''' <typeparam name="TKey">Result dictionary generic type to complete dictionary keys.</typeparam>
	''' <typeparam name="TValue">Result dictionary generic type to complete dictionary values.</typeparam>
	''' <param name="keySelector">Anonymous function accepting first argument as TActiveRecord instance and returning it's specific field/property value to complete Dictionary key.</param>
	''' <param name="duplicateKeyBehaviour">True to thrown Exception if any previous key will be founded by filling the result, false to overwrite any previous value.</param>
	''' <returns>Dictionary with keys completed by second anonymous function, values completed by reader columns with the same names as TActiveRecord type fields/properties.</returns>
	Public Function ToDictionary(Of TKey, TValue)(
		Optional keySelector As Func(Of TValue, TKey) = Nothing,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of TKey, TValue)
		Dim result As New Dictionary(Of TKey, TValue)
		Try
			result = ActiveRecord.Entity.ToDictionary(Of TKey, TValue)(
				Me.Reader, keySelector, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToDictionary(Of TKey, TValue)(
		itemCompleter As ItemCompleter(Of TValue),
		Optional keyColumnName As String = "",
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of TKey, TValue)
		Dim result As New Dictionary(Of TKey, TValue)
		Try
			result = ActiveRecord.Entity.ToDictionary(Of TKey, TValue)(
				Me.Reader, itemCompleter, keyColumnName, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToDictionary(Of TKey, TValue)(
		itemCompleter As ItemCompleter(Of TValue),
		Optional keySelector As Func(Of TValue, TKey) = Nothing,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of TKey, TValue)
		Dim result As New Dictionary(Of TKey, TValue)
		Try
			result = ActiveRecord.Entity.ToDictionary(Of TKey, TValue)(
				Me.Reader, itemCompleter, keySelector, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToDictionary(Of TKey, TValue)(
		itemCompleter As ItemCompleterWithColumns(Of TValue),
		Optional keyColumnName As String = "",
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of TKey, TValue)
		Dim result As New Dictionary(Of TKey, TValue)
		Try
			result = ActiveRecord.Entity.ToDictionary(Of TKey, TValue)(
				Me.Reader, itemCompleter, keyColumnName, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToDictionary(Of TKey, TValue)(
		itemCompleter As ItemCompleterWithColumns(Of TValue),
		Optional keySelector As Func(Of TValue, TKey) = Nothing,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of TKey, TValue)
		Dim result As New Dictionary(Of TKey, TValue)
		Try
			result = ActiveRecord.Entity.ToDictionary(Of TKey, TValue)(
				Me.Reader, itemCompleter, keySelector, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToDictionary(Of TKey, TValue)(
		itemCompleter As ItemCompleterWithAllInfo(Of TValue),
		Optional keyColumnName As String = "",
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of TKey, TValue)
		Dim result As New Dictionary(Of TKey, TValue)
		Try
			result = ActiveRecord.Entity.ToDictionary(Of TKey, TValue)(
				Me.Reader, itemCompleter, keyColumnName, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

	Public Function ToDictionary(Of TKey, TValue)(
		itemCompleter As ItemCompleterWithAllInfo(Of TValue),
		Optional keySelector As Func(Of TValue, TKey) = Nothing,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As Dictionary(Of TKey, TValue)
		Dim result As New Dictionary(Of TKey, TValue)
		Try
			result = ActiveRecord.Entity.ToDictionary(Of TKey, TValue)(
				Me.Reader, itemCompleter, keySelector, duplicateKeyBehaviour
			)
		Catch e As Exception
			Events.RaiseError(e)
		End Try
		Return result
	End Function

End Class