Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace ActiveRecord
	Partial Public Class Resource

		Public Shared Function ColumnsExcept(Of TResource)(
			exceptColumns As String(),
			Optional tableIndex As Int16 = 0,
			Optional separator As String = ","
		) As String
			Dim result As List(Of String) = Databasic.ProviderResource.ColumnsList(
				GetType(TResource), tableIndex
			)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function
		Public Shared Function ColumnsExcept(Of TResource)(
			exceptColumns As String(),
			separator As String,
			Optional tableIndex As Int16 = 0
		) As String
			Dim result As List(Of String) = Databasic.ProviderResource.ColumnsList(
				GetType(TResource), tableIndex
			)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function
		Public Shared Function ColumnsExcept(
			exceptColumns As String(),
			Optional tableIndex As Int16 = 0,
			Optional separator As String = ","
		) As String
			Dim result As List(Of String) = Databasic.ProviderResource.ColumnsList(
				Tools.GetEntryClassType(), tableIndex
			)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function

		Public Shared Function ColumnsExcept(
			exceptColumns As String(),
			separator As String,
			Optional tableIndex As Int16 = 0
		) As String
			Dim result As List(Of String) = Databasic.ProviderResource.ColumnsList(
				Tools.GetEntryClassType(), tableIndex
			)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function

	End Class
End Namespace