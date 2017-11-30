Imports System.IO

Partial Public MustInherit Class Statement

	' Sub - FetchOne|FetchAll ToInstance|ToList - ItemCompleter(Of TValue)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		Optional instanceCompleter As ItemCompleter(
			Of TValue
		) = Nothing
	)
		Dim writer = Me.getJsonStreamWriter(stream),
			singleRow As Boolean = Me.commandBehavior.HasFlag(CommandBehavior.SingleRow),
			instanceCompleterUsed As Boolean = instanceCompleter = Nothing
		Try
			If (singleRow) Then
				If (instanceCompleterUsed) Then
					Databasic.ActiveRecord.Entity.ToJsonInstance(Of TValue)(Me.Reader, writer, True, instanceCompleter)
				Else
					Databasic.ActiveRecord.Entity.ToJsonInstance(Of TValue)(Me.Reader, writer, True)
				End If
			Else
				If (instanceCompleterUsed) Then
					Databasic.ActiveRecord.Entity.ToJsonList(Of TValue)(Me.Reader, writer, True, instanceCompleter)
				Else
					Databasic.ActiveRecord.Entity.ToJsonList(Of TValue)(Me.Reader, writer, True)
				End If
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Sub - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithColumns(Of TValue)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		instanceCompleter As ItemCompleterWithColumns(
			Of TValue
		)
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			If (Me.commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
				Databasic.ActiveRecord.Entity.ToJsonInstance(Of TValue)(
					Me.Reader, writer, True, instanceCompleter
				)
			Else
				Databasic.ActiveRecord.Entity.ToJsonList(Of TValue)(
					Me.Reader, writer, True, instanceCompleter
				)
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Sub - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithAllInfo(Of TValue)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		instanceCompleter As ItemCompleterWithAllInfo(
			Of TValue
		),
		Optional propertiesOnly As Boolean = True
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			If (Me.commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
				Databasic.ActiveRecord.Entity.ToJsonInstance(Of TValue)(
					Me.Reader, writer, True, instanceCompleter, propertiesOnly
				)
			Else
				Databasic.ActiveRecord.Entity.ToJsonList(Of TValue)(
					Me.Reader, writer, True, instanceCompleter, propertiesOnly
				)
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - String
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		jsonObjectKeyColumnName As String,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, True, jsonObjectKeyColumnName, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - Func(Of TValue, Object)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		jsonObjectKeySelector As Func(Of TValue, Object),
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, True, jsonObjectKeySelector, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - ItemCompleter(Of TValue), String
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		itemCompleter As ItemCompleter(Of TValue),
		jsonObjectKeyColumnName As String,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, True, itemCompleter, jsonObjectKeyColumnName, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - ItemCompleter(Of TValue), Func(Of TValue, Object)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		itemCompleter As ItemCompleter(Of TValue),
		jsonObjectKeySelector As Func(Of TValue, Object),
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, True, itemCompleter, jsonObjectKeySelector, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns(Of TValue), String
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		itemCompleter As ItemCompleterWithColumns(Of TValue),
		jsonObjectKeyColumnName As String,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, True, itemCompleter, jsonObjectKeyColumnName, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns(Of TValue), Func(Of TValue, Object)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		itemCompleter As ItemCompleterWithColumns(Of TValue),
		jsonObjectKeySelector As Func(Of TValue, Object),
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, True, itemCompleter, jsonObjectKeySelector, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo(Of TValue), String
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		itemCompleter As ItemCompleterWithAllInfo(Of TValue),
		jsonObjectKeyColumnName As String,
		Optional propertiesOnly As Boolean = True,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, True, itemCompleter, jsonObjectKeyColumnName, propertiesOnly, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Sub - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo(Of TValue), Func(Of TValue, Object)
	Public Sub ToJson(Of TValue)(
		stream As Stream,
		itemCompleter As ItemCompleterWithAllInfo(Of TValue),
		jsonObjectKeySelector As Func(Of TValue, Object),
		Optional propertiesOnly As Boolean = True,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, True, itemCompleter, jsonObjectKeySelector, propertiesOnly, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

End Class