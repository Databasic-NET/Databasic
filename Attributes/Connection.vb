Public Class ConnectionAttribute
	Inherits Attribute
	Public ConnectionIndex As Int32
	Public Sub New(ByVal Optional configConnectionIndex As Integer = 0)
		Me.ConnectionIndex = 0
		If ((configConnectionIndex > -1) AndAlso (configConnectionIndex < Connection.Config.Count)) Then
			Me.ConnectionIndex = configConnectionIndex
		End If
	End Sub
	Public Sub New(ByVal configConnectionName As String)
		Me.ConnectionIndex = 0
		If Not String.IsNullOrEmpty(configConnectionName) Then
			Me.ConnectionIndex = Connection.GetIndexByName(configConnectionName)
		End If
	End Sub
End Class
