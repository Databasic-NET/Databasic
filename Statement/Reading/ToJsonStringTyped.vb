Imports System.IO

Partial Public MustInherit Class Statement

	' Function - FetchOne|FetchAll ToInstance|ToList - ItemCompleter(Of TValue)
	Public Function ToJson(Of TValue)(
		Optional instanceCompleter As ItemCompleter(
			Of TValue
		) = Nothing
	)
	End Function

	' Function - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithColumns(Of TValue)
	Public Function ToJson(Of TValue)(
		instanceCompleter As ItemCompleterWithColumns(
			Of TValue
		)
	)
	End Function

	' Function - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithAllInfo(Of TValue)
	Public Function ToJson(Of TValue)(
		instanceCompleter As ItemCompleterWithAllInfo(
			Of TValue
		)
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - String
	Public Function ToJson(Of TValue)(
		jsonObjectKeyColumnName As String
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - Func(Of TValue, Object)
	Public Function ToJson(Of TValue)(
		jsonObjectKeySelector As Func(Of TValue, Object)
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleter(Of TValue), String
	Public Function ToJson(Of TValue)(
		itemCompleter As ItemCompleter(Of TValue),
		jsonObjectKeyColumnName As String
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleter(Of TValue), Func(Of TValue, Object)
	Public Function ToJson(Of TValue)(
		itemCompleter As ItemCompleter(Of TValue),
		jsonObjectKeySelector As Func(Of TValue, Object)
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns(Of TValue), String
	Public Function ToJson(Of TValue)(
		itemCompleter As ItemCompleterWithColumns(Of TValue),
		jsonObjectKeyColumnName As String
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns(Of TValue), Func(Of TValue, Object)
	Public Function ToJson(Of TValue)(
		itemCompleter As ItemCompleterWithColumns(Of TValue),
		jsonObjectKeySelector As Func(Of TValue, Object)
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo(Of TValue), String
	Public Function ToJson(Of TValue)(
		itemCompleter As ItemCompleterWithAllInfo(Of TValue),
		jsonObjectKeyColumnName As String
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo(Of TValue), Func(Of TValue, Object)
	Public Function ToJson(Of TValue)(
		itemCompleter As ItemCompleterWithAllInfo(Of TValue),
		jsonObjectKeySelector As Func(Of TValue, Object)
	)
	End Function

End Class