Public Class ColumnAttribute
    Inherits Attribute
	Public ColumnName As String = ""
	Public KeyType As KeyType = KeyType.None
	Public KeyName As String = ""
	Public Sub New(columnName As String, Optional keyType As KeyType = KeyType.None, Optional keyName As String = "")
        If Not String.IsNullOrEmpty(columnName) Then Me.ColumnName = columnName
        If keyType <> KeyType.None Then Me.KeyType = keyType
        If Not String.IsNullOrEmpty(keyName) Then Me.KeyName = keyName
    End Sub
End Class
