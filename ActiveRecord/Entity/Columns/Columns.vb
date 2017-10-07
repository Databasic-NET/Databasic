Imports System.Dynamic

Namespace ActiveRecord
	Partial Public MustInherit Class Entity
		Inherits DynamicObject

		Public Shared Function Columns(
			tableIndex As Integer,
			Optional separator As String = ","
		) As String
			Return String.Join(separator, ProviderResource.ColumnsArray(
				Tools.GetEntryClassType(), tableIndex
			))
		End Function

		Public Shared Function Columns(Of TActiveRecord)(
			tableIndex As Integer,
			Optional separator As String = ","
		) As String
			Return String.Join(
				separator,
				ProviderResource.ColumnsArray(GetType(TActiveRecord),
				tableIndex
			))
		End Function

		Public Shared Function Columns(
			Optional separator As String = ",",
			Optional tableIndex As Integer = 0
		) As String
			Return String.Join(separator, ProviderResource.ColumnsArray(
				Tools.GetEntryClassType(), tableIndex
			))
		End Function

		Public Shared Function Columns(Of TActiveRecord)(
			Optional separator As String = ",",
			Optional tableIndex As Integer = 0
		) As String
			Return String.Join(
				separator,
				ProviderResource.ColumnsArray(GetType(TActiveRecord),
				tableIndex
			))
		End Function

	End Class
End Namespace
