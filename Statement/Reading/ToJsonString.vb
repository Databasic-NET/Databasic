Imports System.IO

Partial Public MustInherit Class Statement

	' Function - FetchOne|FetchAll ToInstance|ToList - Type
	Public Function ToJson(
		Optional instanceCompleter As ItemCompleter = Nothing
	) As String
		Dim writer = Me.getJsonStreamWriter(),
			singleRow As Boolean = Me.commandBehavior.HasFlag(CommandBehavior.SingleRow),
			instanceCompleterUsed As Boolean = instanceCompleter = Nothing
		Try
			If (singleRow) Then
				If (instanceCompleterUsed) Then
					Databasic.ActiveRecord.Entity.ToJsonInstance(Me.Reader, writer, False, instanceCompleter)
				Else
					Databasic.ActiveRecord.Entity.ToJsonInstance(Me.Reader, writer, False)
				End If
			Else
				If (instanceCompleterUsed) Then
					Databasic.ActiveRecord.Entity.ToJsonList(Me.Reader, writer, False, instanceCompleter)
				Else
					Databasic.ActiveRecord.Entity.ToJsonList(Me.Reader, writer, False)
				End If
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToInstance|ToList - Type
	Public Function ToJson(
		itemType As Type,
		Optional instanceCompleter As ItemCompleter = Nothing
	) As String
		Dim writer = Me.getJsonStreamWriter(),
			singleRow As Boolean = Me.commandBehavior.HasFlag(CommandBehavior.SingleRow),
			instanceCompleterUsed As Boolean = instanceCompleter = Nothing
		Try
			If (singleRow) Then
				If (instanceCompleterUsed) Then
					Databasic.ActiveRecord.Entity.ToJsonInstance(Me.Reader, writer, False, itemType, instanceCompleter)
				Else
					Databasic.ActiveRecord.Entity.ToJsonInstance(Me.Reader, writer, False, itemType)
				End If
			Else
				If (instanceCompleterUsed) Then
					Databasic.ActiveRecord.Entity.ToJsonList(Me.Reader, writer, False, itemType, instanceCompleter)
				Else
					Databasic.ActiveRecord.Entity.ToJsonList(Me.Reader, writer, False, itemType)
				End If
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithColumns
	Public Function ToJson(
		instanceCompleter As ItemCompleterWithColumns
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			If (Me.commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
				Databasic.ActiveRecord.Entity.ToJsonInstance(
					Me.Reader, writer, False, instanceCompleter
				)
			Else
				Databasic.ActiveRecord.Entity.ToJsonList(
					Me.Reader, writer, False, instanceCompleter
				)
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithAllInfo
	Public Function ToJson(
		instanceCompleter As ItemCompleterWithAllInfo,
		itemType As Type,
		Optional propertiesOnly As Boolean = True
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			If (Me.commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
				Databasic.ActiveRecord.Entity.ToJsonInstance(
					Me.Reader, writer, False, instanceCompleter, itemType, propertiesOnly
				)
			Else
				Databasic.ActiveRecord.Entity.ToJsonList(
					Me.Reader, writer, False, instanceCompleter, itemType, propertiesOnly
				)
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - Type, String
	Public Function ToJson(
		itemType As Type,
		jsonObjectKeyColumnName As String,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, False, itemType, jsonObjectKeyColumnName, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - Type, Func(Of Object, Object)
	Public Function ToJson(
		itemType As Type,
		jsonObjectKeySelector As Func(Of Object, Object),
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, False, itemType, jsonObjectKeySelector, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleter, String
	Public Function ToJson(
		itemCompleter As ItemCompleter,
		jsonObjectKeyColumnName As String,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, False, itemCompleter, jsonObjectKeyColumnName, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleter, Func(Of Object, Object)
	Public Function ToJson(
		itemCompleter As ItemCompleter,
		jsonObjectKeySelector As Func(Of Object, Object),
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, False, itemCompleter, jsonObjectKeySelector, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns, String
	Public Function ToJson(
		itemCompleter As ItemCompleterWithColumns,
		jsonObjectKeyColumnName As String,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, False, itemCompleter, jsonObjectKeyColumnName, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns, Func(Of Object, Object)
	Public Function ToJson(
		itemCompleter As ItemCompleterWithColumns,
		jsonObjectKeySelector As Func(Of Object, Object),
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, False, itemCompleter, jsonObjectKeySelector, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo, Type, String
	Public Function ToJson(
		itemCompleter As ItemCompleterWithAllInfo,
		itemType As Type,
		jsonObjectKeyColumnName As String,
		Optional propertiesOnly As Boolean = True,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, False, itemCompleter, itemType, jsonObjectKeyColumnName, propertiesOnly, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo, Type, Func(Of Object, Object)
	Public Function ToJson(
		itemCompleter As ItemCompleterWithAllInfo,
		itemType As Type,
		jsonObjectKeySelector As Func(Of Object, Object),
		Optional propertiesOnly As Boolean = True,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(
				Me.Reader, writer, False, itemCompleter, itemType, jsonObjectKeySelector, propertiesOnly, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

End Class