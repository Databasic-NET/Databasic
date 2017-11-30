Imports System.IO

Partial Public MustInherit Class Statement

	' Sub - FetchOne|FetchAll ToInstance|ToList - ItemCompleter(Of TValue)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		Optional instanceCompleter As ItemCompleter(
			Of TValue
		) = Nothing
	)
	End Sub

	' Sub - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithColumns(Of TValue)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		instanceCompleter As ItemCompleterWithColumns(
			Of TValue
		)
	)
	End Sub

	' Sub - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithAllInfo(Of TValue)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		instanceCompleter As ItemCompleterWithAllInfo(
			Of TValue
		)
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - Stream, String
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		jsonObjectKeyColumnName As String
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - Stream, Func(Of TValue, Object)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		jsonObjectKeySelector As Func(Of TValue, Object)
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - Stream, ItemCompleter(Of TValue), String
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		itemCompleter As ItemCompleter(Of TValue),
		jsonObjectKeyColumnName As String
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - Stream, ItemCompleter(Of TValue), Func(Of TValue, Object)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		itemCompleter As ItemCompleter(Of TValue),
		jsonObjectKeySelector As Func(Of TValue, Object)
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - Stream, ItemCompleterWithColumns(Of TValue), String
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		itemCompleter As ItemCompleterWithColumns(Of TValue),
		jsonObjectKeyColumnName As String
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - Stream, ItemCompleterWithColumns(Of TValue), Func(Of TValue, Object)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		itemCompleter As ItemCompleterWithColumns(Of TValue),
		jsonObjectKeySelector As Func(Of TValue, Object)
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - Stream, ItemCompleterWithAllInfo(Of TValue), String
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		itemCompleter As ItemCompleterWithAllInfo(Of TValue),
		jsonObjectKeyColumnName As String
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - Stream, ItemCompleterWithAllInfo(Of TValue), Func(Of TValue, Object)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		itemCompleter As ItemCompleterWithAllInfo(Of TValue),
		jsonObjectKeySelector As Func(Of TValue, Object)
	)
	End Sub

End Class