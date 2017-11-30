Imports System.Data.Common
Imports System.IO
Imports System.Text
Imports Databasic.ActiveRecord

Namespace ActiveRecord
	Partial Public MustInherit Class Entity

		Friend Shared Sub ToJsonDictionary(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			jsonObjectKeyColumnName As String,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			jsonObjectKeySelector As Func(Of TValue, Object),
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemCompleter As ItemCompleter(Of TValue),
			jsonObjectKeyColumnName As String,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemCompleter As ItemCompleter(Of TValue),
			jsonObjectKeySelector As Func(Of TValue, Object),
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemCompleter As ItemCompleterWithColumns(Of TValue),
			jsonObjectKeyColumnName As String,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemCompleter As ItemCompleterWithColumns(Of TValue),
			jsonObjectKeySelector As Func(Of TValue, Object),
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemCompleter As ItemCompleterWithAllInfo(Of TValue),
			jsonObjectKeyColumnName As String,
			Optional propertiesOnly As Boolean = True,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

		Friend Shared Sub ToJsonDictionary(Of TValue)(
			reader As DbDataReader,
			writer As StreamWriter,
			flushContinuously As Boolean,
			itemCompleter As ItemCompleterWithAllInfo(Of TValue),
			jsonObjectKeySelector As Func(Of TValue, Object),
			Optional propertiesOnly As Boolean = True,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		)

		End Sub

	End Class
End Namespace