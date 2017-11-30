Imports System.Data.Common
Imports System.IO
Imports System.Text
Imports Databasic.ActiveRecord

Namespace ActiveRecord
	Partial Public MustInherit Class Entity

		Friend Shared Sub ToJsonDictionary(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemType As Type,
			jsonObjectKeyColumnName As String,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemType As Type,
			jsonObjectKeySelector As Func(Of Object, Object),
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			jsonObjectKeyColumnName As String,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			jsonObjectKeySelector As Func(Of Object, Object),
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemCompleter As ItemCompleter,
			jsonObjectKeyColumnName As String,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemCompleter As ItemCompleter,
			jsonObjectKeySelector As Func(Of Object, Object),
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemCompleter As ItemCompleterWithColumns,
			jsonObjectKeyColumnName As String,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemCompleter As ItemCompleterWithColumns,
			jsonObjectKeySelector As Func(Of Object, Object),
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemCompleter As ItemCompleterWithAllInfo,
			itemType As Type,
			jsonObjectKeyColumnName As String,
			Optional propertiesOnly As Boolean = True,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemCompleter As ItemCompleterWithAllInfo,
			itemType As Type,
			jsonObjectKeySelector As Func(Of Object, Object),
			Optional propertiesOnly As Boolean = True,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

	End Class
End Namespace