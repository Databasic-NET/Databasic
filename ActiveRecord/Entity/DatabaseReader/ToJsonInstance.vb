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

		Friend Shared Sub ToJsonInstance(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemType As Type
		)

		End Sub

		Friend Shared Sub ToJsonInstance(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemType As Type,
			InstanceCompleter As ItemCompleter
		)

		End Sub

		Friend Shared Sub ToJsonInstance(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean
		)

		End Sub

		Friend Shared Sub ToJsonInstance(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			InstanceCompleter As ItemCompleter
		)

		End Sub

		Friend Shared Sub ToJsonInstance(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			instanceCompleter As ItemCompleterWithColumns
		)

		End Sub

		Friend Shared Sub ToJsonInstance(
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