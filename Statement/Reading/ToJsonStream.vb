Imports System.IO

Partial Public MustInherit Class Statement

	' Sub - FetchOne|FetchAll ToInstance|ToList - Type
	Public Sub ToJson(
		stream As Stream,
		itemType As Type
	)
	End Sub

	' Sub - FetchOne|FetchAll ToInstance|ToList - ItemCompleter
	Public Sub ToJson(
		stream As Stream,
		instanceCompleter As ItemCompleter
	)
	End Sub

	' Sub - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithColumns
	Public Sub ToJson(
		stream As Stream,
		instanceCompleter As ItemCompleterWithColumns
	)
	End Sub

	' Sub - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithAllInfo
	Public Sub ToJson(
		stream As Stream,
		instanceCompleter As ItemCompleterWithAllInfo,
		itemType As Type
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - Type, String
	Public Sub ToJson(
		stream As Stream,
		itemType As Type,
		jsonObjectKeyColumnName As String
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - Type, Func(Of Object, Object)
	Public Sub ToJson(
		stream As Stream,
		itemType As Type,
		jsonObjectKeySelector As Func(Of Object, Object)
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - ItemCompleter, String
	Public Sub ToJson(
		stream As Stream,
		itemCompleter As ItemCompleter,
		jsonObjectKeyColumnName As String
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - ItemCompleter, Func(Of Object, Object)
	Public Sub ToJson(
		stream As Stream,
		itemCompleter As ItemCompleter,
		jsonObjectKeySelector As Func(Of Object, Object)
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns, String
	Public Sub ToJson(
		stream As Stream,
		itemCompleter As ItemCompleterWithColumns,
		jsonObjectKeyColumnName As String
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns, Func(Of Object, Object)
	Public Sub ToJson(
		stream As Stream,
		itemCompleter As ItemCompleterWithColumns,
		jsonObjectKeySelector As Func(Of Object, Object)
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo, Type, String
	Public Sub ToJson(
		stream As Stream,
		itemCompleter As ItemCompleterWithAllInfo,
		itemType As Type,
		jsonObjectKeyColumnName As String
	)
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo, Type, Func(Of Object, Object)
	Public Sub ToJson(
		stream As Stream,
		itemCompleter As ItemCompleterWithAllInfo,
		itemType As Type,
		jsonObjectKeySelector As Func(Of Object, Object)
	)
	End Sub

End Class