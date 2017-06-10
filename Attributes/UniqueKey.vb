Public Class UniqueKeyAttribute
    Inherits Attribute
    Friend KeyName As String = ""
    Public Sub New(Optional uniqueKeyName As String = "")
        If Not String.IsNullOrEmpty(uniqueKeyName) Then Me.KeyName = uniqueKeyName
    End Sub
End Class