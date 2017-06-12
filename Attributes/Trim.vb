Public Class TrimAttribute
    Inherits Attribute
	Public Chars As Char() = New Char() {" "c, ChrW(13), ChrW(10), ChrW(9), ChrW(11)}
	Public Sub New(ParamArray chars As Char())
        If chars.Length > 0 Then Me.Chars = chars
    End Sub
End Class
