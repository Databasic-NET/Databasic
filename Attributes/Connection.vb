Public Class ConnectionAttribute
    Inherits Attribute
	Public ConnectionIndex As Int32 = Database.DEFAUT_CONNECTION_INDEX
	Public Sub New(configConnectionName As String)
        If Not String.IsNullOrEmpty(configConnectionName) Then
            Me.ConnectionIndex = Connection.GetIndexByName(configConnectionName)
        End If
    End Sub
    Public Sub New(Optional configConnectionIndex As Int32 = Database.DEFAUT_CONNECTION_INDEX)
        If configConnectionIndex > -1 AndAlso configConnectionIndex < Connection.Config.Count Then
            Me.ConnectionIndex = configConnectionIndex
        End If
    End Sub
End Class
