Public Class TableAttribute
    Inherits Attribute
    Friend Tables As String() = New String() {}
    Public Sub New(table As String)
        Me.Tables = New String() {table}
    End Sub
    Public Sub New(ParamArray tables As String())
        Me.Tables = tables
    End Sub
End Class