Imports System.Globalization
Imports System.Runtime.CompilerServices

Public Class FormatAttribute
	Inherits Attribute

	Public Property FormatProvider As IFormatProvider
		Get
			Return Me._formatProvider
		End Get
		Set(value As IFormatProvider)
			Me._formatProvider = value
		End Set
	End Property
	<CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)>
	Private _formatProvider As IFormatProvider

	Public Sub New(ByVal cultureName As String)
		Me.FormatProvider = New CultureInfo(cultureName)
	End Sub
End Class
