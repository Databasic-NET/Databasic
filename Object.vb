Imports System.Globalization
Imports System.Reflection

Public Class [Object]
	Inherits Databasic.ActiveRecord.Entity

	''' <summary>
	''' Properties with values and fields with values touched by indexer.
	''' </summary>
	Private _reserveStore As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

	''' <summary>
	''' Returns properties with values and fields with values touched by indexer.
	''' </summary>
	Protected Overrides Function getReserveStore() As Dictionary(Of String, Object)
		Return Me._reserveStore
	End Function

End Class