Public Class PrimaryKeyAttribute
    Inherits Attribute
	Public KeyName As String = ""
	Public Sub New(Optional primaryKeyName As String = "")
        If Not String.IsNullOrEmpty(primaryKeyName) Then Me.KeyName = primaryKeyName
    End Sub
End Class
