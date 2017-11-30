Imports System.IO

Partial Public MustInherit Class Statement

	' Function - FetchOne|FetchAll ToInstance|ToList - Type
	Public Function ToJson(
		itemType As Type
	)
	End Function

	' Function - FetchOne|FetchAll ToInstance|ToList - ItemCompleter
	Public Function ToJson(
		instanceCompleter As ItemCompleter
	)
	End Function

	' Function - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithColumns
	Public Function ToJson(
		instanceCompleter As ItemCompleterWithColumns
	)
	End Function

	' Function - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithAllInfo
	Public Function ToJson(
		instanceCompleter As ItemCompleterWithAllInfo,
		itemType As Type
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - Type, String
	Public Function ToJson(
		itemType As Type,
		jsonObjectKeyColumnName As String
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - Type, Func(Of Object, Object)
	Public Function ToJson(
		itemType As Type,
		jsonObjectKeySelector As Func(Of Object, Object)
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleter, String
	Public Function ToJson(
		itemCompleter As ItemCompleter,
		jsonObjectKeyColumnName As String
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleter, Func(Of Object, Object)
	Public Function ToJson(
		itemCompleter As ItemCompleter,
		jsonObjectKeySelector As Func(Of Object, Object)
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns, String
	Public Function ToJson(
		itemCompleter As ItemCompleterWithColumns,
		jsonObjectKeyColumnName As String
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns, Func(Of Object, Object)
	Public Function ToJson(
		itemCompleter As ItemCompleterWithColumns,
		jsonObjectKeySelector As Func(Of Object, Object)
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo, Type, String
	Public Function ToJson(
		itemCompleter As ItemCompleterWithAllInfo,
		itemType As Type,
		jsonObjectKeyColumnName As String
	)
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo, Type, Func(Of Object, Object)
	Public Function ToJson(
		itemCompleter As ItemCompleterWithAllInfo,
		itemType As Type,
		jsonObjectKeySelector As Func(Of Object, Object)
	)
	End Function

End Class