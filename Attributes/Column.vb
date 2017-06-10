Public Class ColumnAttribute
    Inherits Attribute
    Friend ColumnName As String = ""
    Friend KeyType As KeyType = Nothing
    Friend KeyName As String = ""
    Public Sub New(columnName As String, Optional keyType As KeyType = Nothing, Optional keyName As String = "")
        If Not String.IsNullOrEmpty(columnName) Then Me.ColumnName = columnName
        If keyType <> Nothing Then Me.KeyType = keyType
        If Not String.IsNullOrEmpty(keyName) Then Me.KeyName = keyName
    End Sub
End Class
