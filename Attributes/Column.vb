Public Class ColumnAttribute
    Inherits Attribute
	Public ColumnName As String = ""
	Public Sub New(columnName As String)
		If Not String.IsNullOrEmpty(columnName) Then Me.ColumnName = columnName
	End Sub
End Class
