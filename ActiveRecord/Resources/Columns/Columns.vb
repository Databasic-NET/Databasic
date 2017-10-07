Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace ActiveRecord
	Partial Public Class Resource

		Public Shared Function Columns(Of TResource)(
			separator As String,
			Optional tableIndex As Int16 = 0
		) As String
			Return String.Join(
				separator,
				Databasic.ProviderResource.ColumnsArray(GetType(TResource), tableIndex)
			)
		End Function
		Public Shared Function Columns(Of TResource)(
			Optional tableIndex As Int16 = 0,
			Optional separator As String = ","
		) As String
			Return String.Join(
				separator,
				Databasic.ProviderResource.ColumnsArray(GetType(TResource), tableIndex)
			)
		End Function
		Public Shared Function Columns(
			separator As String,
			Optional tableIndex As Int16 = 0
		) As String
			Return String.Join(
				separator,
				Databasic.ProviderResource.ColumnsArray(
					Tools.GetEntryClassType(), tableIndex
				)
			)
		End Function
		Public Shared Function Columns(
			Optional tableIndex As Int16 = 0,
			Optional separator As String = ","
		) As String
			Return String.Join(
				separator,
				Databasic.ProviderResource.ColumnsArray(
					Tools.GetEntryClassType(), tableIndex
				)
			)
		End Function

	End Class
End Namespace