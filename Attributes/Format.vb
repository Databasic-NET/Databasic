Imports System.Globalization

Public Class FormatAttribute
    Inherits Attribute
    Friend FormatProvider As IFormatProvider = CultureInfo.CurrentCulture
    Public Sub New(formatProvider As IFormatProvider)
        Me.FormatProvider = formatProvider
    End Sub
    Public Sub New(cultureInfoName As String)
        Try
            Me.FormatProvider = New CultureInfo(cultureInfoName)
        Finally
        End Try
    End Sub
End Class
