Imports System.IO

Partial Public MustInherit Class Statement

	' Function - FetchOne|FetchAll ToInstance|ToList - Type
	Public Sub ToJson(
		stream As Stream,
		Optional instanceCompleter As ItemCompleter = Nothing
	)
		Dim writer = Me.getJsonStreamWriter(stream),
			singleRow As Boolean = Me.commandBehavior.HasFlag(CommandBehavior.SingleRow),
			instanceCompleterUsed As Boolean = instanceCompleter = Nothing
		Try
			If (singleRow) Then
				If (instanceCompleterUsed) Then
					Databasic.ActiveRecord.Entity.ToJsonInstance(Me.Reader, writer, True, instanceCompleter)
				Else
					Databasic.ActiveRecord.Entity.ToJsonInstance(Me.Reader, writer, True)
				End If
			Else
				If (instanceCompleterUsed) Then
					Databasic.ActiveRecord.Entity.ToJsonList(Me.Reader, writer, True, instanceCompleter)
				Else
					Databasic.ActiveRecord.Entity.ToJsonList(Me.Reader, writer, True)
				End If
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Function - FetchOne|FetchAll ToInstance|ToList - Type
	Public Sub ToJson(
		stream As Stream,
		itemType As Type,
		Optional instanceCompleter As ItemCompleter = Nothing
	)
		Dim writer = Me.getJsonStreamWriter(stream),
			singleRow As Boolean = Me.commandBehavior.HasFlag(CommandBehavior.SingleRow),
			instanceCompleterUsed As Boolean = instanceCompleter = Nothing
		Try
			If (singleRow) Then
				If (instanceCompleterUsed) Then
					Databasic.ActiveRecord.Entity.ToJsonInstance(Me.Reader, writer, True, itemType, instanceCompleter)
				Else
					Databasic.ActiveRecord.Entity.ToJsonInstance(Me.Reader, writer, True, itemType)
				End If
			Else
				If (instanceCompleterUsed) Then
					Databasic.ActiveRecord.Entity.ToJsonList(Me.Reader, writer, True, itemType, instanceCompleter)
				Else
					Databasic.ActiveRecord.Entity.ToJsonList(Me.Reader, writer, True, itemType)
				End If
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Function - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithColumns
	Public Sub ToJson(
		stream As Stream,
		InstanceCompleter As ItemCompleterWithColumns
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			If (Me.commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
				Databasic.ActiveRecord.Entity.ToJsonInstance(
					Me.Reader, writer, True, InstanceCompleter
				)
			Else
				Databasic.ActiveRecord.Entity.ToJsonList(
					Me.Reader, writer, True, InstanceCompleter
				)
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Function - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithAllInfo
	Public Sub ToJson(
		stream As Stream,
		InstanceCompleter As ItemCompleterWithAllInfo,
		itemType As Type,
		Optional propertiesOnly As Boolean = True
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			If (Me.commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
				Databasic.ActiveRecord.Entity.ToJsonInstance(
					Me.Reader, writer, True, InstanceCompleter, itemType, propertiesOnly
				)
			Else
				Databasic.ActiveRecord.Entity.ToJsonList(
					Me.Reader, writer, True, InstanceCompleter, itemType, propertiesOnly
				)
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Function - FetchOne|FetchAll ToDictionary - Type, String
	Public Sub ToJson(
		stream As Stream,
		itemType As Type,
		jsonObjectKeyColumnName As String,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, True, itemType, jsonObjectKeyColumnName, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Function - FetchOne|FetchAll ToDictionary - Type, Func(Of Object, Object)
	Public Sub ToJson(
		stream As Stream,
		itemType As Type,
		jsonObjectKeySelector As Func(Of Object, Object),
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, True, itemType, jsonObjectKeySelector, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleter, String
	Public Sub ToJson(
		stream As Stream,
		ItemCompleter As ItemCompleter,
		jsonObjectKeyColumnName As String,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, True, ItemCompleter, jsonObjectKeyColumnName, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleter, Func(Of Object, Object)
	Public Sub ToJson(
		stream As Stream,
		ItemCompleter As ItemCompleter,
		jsonObjectKeySelector As Func(Of Object, Object),
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, True, ItemCompleter, jsonObjectKeySelector, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns, String
	Public Sub ToJson(
		stream As Stream,
		ItemCompleter As ItemCompleterWithColumns,
		jsonObjectKeyColumnName As String,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, True, ItemCompleter, jsonObjectKeyColumnName, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns, Func(Of Object, Object)
	Public Sub ToJson(
		stream As Stream,
		ItemCompleter As ItemCompleterWithColumns,
		jsonObjectKeySelector As Func(Of Object, Object),
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, True, ItemCompleter, jsonObjectKeySelector, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo, Type, String
	Public Sub ToJson(
		stream As Stream,
		ItemCompleter As ItemCompleterWithAllInfo,
		itemType As Type,
		jsonObjectKeyColumnName As String,
		Optional propertiesOnly As Boolean = True,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, True, ItemCompleter, itemType, jsonObjectKeyColumnName, propertiesOnly, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo, Type, Func(Of Object, Object)
	Public Sub ToJson(
		stream As Stream,
		ItemCompleter As ItemCompleterWithAllInfo,
		itemType As Type,
		jsonObjectKeySelector As Func(Of Object, Object),
		Optional propertiesOnly As Boolean = True,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	)
		Dim writer = Me.getJsonStreamWriter(stream)
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, True, ItemCompleter, itemType, jsonObjectKeySelector, propertiesOnly, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
	End Sub

End Class