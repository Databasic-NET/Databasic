Imports System.Data.Common
Imports System.IO
Imports System.Text
Imports Databasic.ActiveRecord

Namespace ActiveRecord
	Partial Public MustInherit Class Entity

		'Dim result As String = ""
		'Dim serializer As JavaScriptSerializer = New JavaScriptSerializer()
		'Dim resultStream As New StringBuilder()
		'serializer.Serialize(Nothing, )
		'Return result

		Friend Shared Sub ToJsonInstance(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean
		)

		End Sub

		Friend Shared Sub ToJsonInstance(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			instanceCompleter As ItemCompleter(Of TValue)
		)

		End Sub

		Friend Shared Sub ToJsonInstance(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			instanceCompleter As ItemCompleterWithColumns(Of TValue)
		)

		End Sub

		Friend Shared Sub ToJsonInstance(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			instanceCompleter As ItemCompleterWithAllInfo(Of TValue),
			Optional propertiesOnly As Boolean = True
		)

		End Sub

	End Class
End Namespace