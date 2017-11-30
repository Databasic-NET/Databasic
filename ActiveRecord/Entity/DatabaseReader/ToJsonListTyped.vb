Imports System.Data.Common
Imports System.IO
Imports System.Text
Imports Databasic.ActiveRecord

Namespace ActiveRecord
	Partial Public MustInherit Class Entity

		Friend Shared Sub ToJsonList(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean
		)

		End Sub

		Friend Shared Sub ToJsonList(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			instanceCompleter As ItemCompleter(Of TValue)
		)

		End Sub

		Friend Shared Sub ToJsonList(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			instanceCompleter As ItemCompleterWithColumns(Of TValue)
		)

		End Sub

		Friend Shared Sub ToJsonList(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			instanceCompleter As ItemCompleterWithAllInfo(Of TValue),
			Optional propertiesOnly As Boolean = True
		)

		End Sub

	End Class
End Namespace