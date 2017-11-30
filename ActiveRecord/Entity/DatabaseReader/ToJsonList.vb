Imports System.Data.Common
Imports System.IO
Imports System.Text
Imports Databasic.ActiveRecord

Namespace ActiveRecord
	Partial Public MustInherit Class Entity

		Friend Shared Sub ToJsonList(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemType As Type
		)

		End Sub

		Friend Shared Sub ToJsonList(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemType As Type,
			instanceCompleter As ItemCompleter
		)

		End Sub

		Friend Shared Sub ToJsonList(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean
		)

		End Sub

		Friend Shared Sub ToJsonList(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			instanceCompleter As ItemCompleter
		)

		End Sub

		Friend Shared Sub ToJsonList(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			instanceCompleter As ItemCompleterWithColumns
		)

		End Sub

		Friend Shared Sub ToJsonList(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			instanceCompleter As ItemCompleterWithAllInfo,
			itemType As Type,
			Optional propertiesOnly As Boolean = True
		)

		End Sub

	End Class
End Namespace