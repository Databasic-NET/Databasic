Public Class TrimAttribute
	Inherits Attribute

	Public Chars As Char() = New Char() {" ", "\t", "\n", "\v", "\r"}

	Public Sub New(ParamArray chars As Char())
		If chars.Length > 0 Then Me.Chars = chars
	End Sub

End Class
