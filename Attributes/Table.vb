Public Class TableAttribute
    Inherits Attribute
	Public Tables As String() = New String() {}
	Public Sub New(table As String)
        If Not String.IsNullOrEmpty(table) Then Me.Tables = New String() {table}
    End Sub
    Public Sub New(ParamArray tables As String())
        Me.Tables = tables
    End Sub
End Class