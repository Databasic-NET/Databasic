Imports System.Dynamic

Namespace ActiveRecord
	Partial Public MustInherit Class Entity
		Inherits DynamicObject





		Public Shared Function Columns(Of TActiveRecord)(tableIndex As Int32, Optional separator As String = ",") As String
			Return String.Join(
				separator, Databasic.ProviderResource.ColumnsArray(GetType(TActiveRecord), tableIndex)
			)
		End Function
		Public Shared Function Columns(Of TActiveRecord)(Optional separator As String = ",", Optional tableIndex As Int32 = 0) As String
			Return String.Join(
				separator, Databasic.ProviderResource.ColumnsArray(GetType(TActiveRecord), tableIndex)
			)
		End Function
		Public Shared Function Columns(tableIndex As Int32, Optional separator As String = ",") As String
			Return String.Join(
				separator, Databasic.ProviderResource.ColumnsArray(Tools.GetEntryClassType(), tableIndex)
			)
		End Function
		Public Shared Function Columns(Optional separator As String = ",", Optional tableIndex As Int32 = 0) As String
			Return String.Join(
				separator, Databasic.ProviderResource.ColumnsArray(Tools.GetEntryClassType(), tableIndex)
			)
		End Function





		Public Shared Function ColumnsExcept(Of TResource)(exceptColumns As String(), Optional tableIndex As Int16 = 0, Optional separator As String = ",") As String
			Dim result As List(Of String) = Databasic.ProviderResource.ColumnsList(GetType(TResource), tableIndex)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function
		Public Shared Function ColumnsExcept(Of TResource)(exceptColumns As String(), separator As String, Optional tableIndex As Int16 = 0) As String
			Dim result As List(Of String) = Databasic.ProviderResource.ColumnsList(GetType(TResource), tableIndex)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function
		Public Shared Function ColumnsExcept(exceptColumns As String(), Optional tableIndex As Int16 = 0, Optional separator As String = ",") As String
			Dim result As List(Of String) = Databasic.ProviderResource.ColumnsList(Tools.GetEntryClassType(), tableIndex)
			For Each exceptCol As String In exceptColumns
				If result.Contains(exceptCol) Then result.Remove(exceptCol)
			Next
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(result)
			)
		End Function
		Public Shared Function ColumnsExcept(exceptColumns As String(), separator As String, Optional tableIndex As Int16 = 0) As String
			Dim result As List(Of String) = Databasic.ProviderResource.ColumnsList(Tools.GetEntryClassType(), tableIndex)
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
