Public Class ConnectionAttribute
    Inherits Attribute
    Friend ConnectionIndex As Int16 = 0
    Public Sub New(configConnectionName As String)
        If Not String.IsNullOrEmpty(configConnectionName) Then
            Me.ConnectionIndex = Connection.GetIndexByName(configConnectionName)
        End If
    End Sub
    Public Sub New(Optional configConnectionIndex As Int16 = 0)
        If configConnectionIndex > -1 AndAlso configConnectionIndex < Connection.Config.Count Then
            Me.ConnectionIndex = configConnectionIndex
        End If
    End Sub
End Class
