Imports System.IO

Partial Public MustInherit Class Statement

	' Function - FetchOne|FetchAll ToInstance|ToList - ItemCompleter(Of TValue)
	Public Function ToJson(Of TValue)(
		Optional instanceCompleter As ItemCompleter(
			Of TValue
		) = Nothing
	) As String
		Dim writer = Me.getJsonStreamWriter(),
			singleRow As Boolean = Me.commandBehavior.HasFlag(CommandBehavior.SingleRow),
			instanceCompleterUsed As Boolean = instanceCompleter = Nothing
		Try
			If (singleRow) Then
				If (instanceCompleterUsed) Then
					Databasic.ActiveRecord.Entity.ToJsonInstance(Of TValue)(Me.Reader, writer, False, instanceCompleter)
				Else
					Databasic.ActiveRecord.Entity.ToJsonInstance(Of TValue)(Me.Reader, writer, False)
				End If
			Else
				If (instanceCompleterUsed) Then
					Databasic.ActiveRecord.Entity.ToJsonList(Of TValue)(Me.Reader, writer, False, instanceCompleter)
				Else
					Databasic.ActiveRecord.Entity.ToJsonList(Of TValue)(Me.Reader, writer, False)
				End If
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithColumns(Of TValue)
	Public Function ToJson(Of TValue)(
		instanceCompleter As ItemCompleterWithColumns(
			Of TValue
		)
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			If (Me.commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
				Databasic.ActiveRecord.Entity.ToJsonInstance(Of TValue)(
					Me.Reader, writer, False, instanceCompleter
				)
			Else
				Databasic.ActiveRecord.Entity.ToJsonList(Of TValue)(
					Me.Reader, writer, False, instanceCompleter
				)
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToInstance|ToList - ItemCompleterWithAllInfo(Of TValue)
	Public Function ToJson(Of TValue)(
		instanceCompleter As ItemCompleterWithAllInfo(
			Of TValue
		),
		Optional propertiesOnly As Boolean = True
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			If (Me.commandBehavior.HasFlag(CommandBehavior.SingleRow)) Then
				Databasic.ActiveRecord.Entity.ToJsonInstance(Of TValue)(
					Me.Reader, writer, False, instanceCompleter, propertiesOnly
				)
			Else
				Databasic.ActiveRecord.Entity.ToJsonList(Of TValue)(
					Me.Reader, writer, False, instanceCompleter, propertiesOnly
				)
			End If
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - String
	Public Function ToJson(Of TValue)(
		jsonObjectKeyColumnName As String,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, False, jsonObjectKeyColumnName, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - Func(Of TValue, Object)
	Public Function ToJson(Of TValue)(
		jsonObjectKeySelector As Func(Of TValue, Object),
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, False, jsonObjectKeySelector, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleter(Of TValue), String
	Public Function ToJson(Of TValue)(
		itemCompleter As ItemCompleter(Of TValue),
		jsonObjectKeyColumnName As String,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, False, itemCompleter, jsonObjectKeyColumnName, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleter(Of TValue), Func(Of TValue, Object)
	Public Function ToJson(Of TValue)(
		itemCompleter As ItemCompleter(Of TValue),
		jsonObjectKeySelector As Func(Of TValue, Object),
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, False, itemCompleter, jsonObjectKeySelector, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns(Of TValue), String
	Public Function ToJson(Of TValue)(
		itemCompleter As ItemCompleterWithColumns(Of TValue),
		jsonObjectKeyColumnName As String,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, False, itemCompleter, jsonObjectKeyColumnName, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithColumns(Of TValue), Func(Of TValue, Object)
	Public Function ToJson(Of TValue)(
		itemCompleter As ItemCompleterWithColumns(Of TValue),
		jsonObjectKeySelector As Func(Of TValue, Object),
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, False, itemCompleter, jsonObjectKeySelector, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo(Of TValue), String
	Public Function ToJson(Of TValue)(
		itemCompleter As ItemCompleterWithAllInfo(Of TValue),
		jsonObjectKeyColumnName As String,
		Optional propertiesOnly As Boolean = True,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, False, itemCompleter, jsonObjectKeyColumnName, propertiesOnly, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

	' Function - FetchOne|FetchAll ToDictionary - ItemCompleterWithAllInfo(Of TValue), Func(Of TValue, Object)
	Public Function ToJson(Of TValue)(
		itemCompleter As ItemCompleterWithAllInfo(Of TValue),
		jsonObjectKeySelector As Func(Of TValue, Object),
		Optional propertiesOnly As Boolean = True,
		Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
	) As String
		Dim writer = Me.getJsonStreamWriter()
		Try
			Databasic.ActiveRecord.Entity.ToJsonDictionary(Of TValue)(
				Me.Reader, writer, False, itemCompleter, jsonObjectKeySelector, propertiesOnly, duplicateKeyBehaviour
			)
		Catch ex As Exception
			Events.RaiseError(ex)
		End Try
		Return writer.ToString()
	End Function

End Class