Public Class PrimaryKeyAttribute
	Inherits Attribute
	Public KeyName As String = Databasic.Defaults.KEY_NAME
	Public Sub New(Optional primaryKeyName As String = Databasic.Defaults.KEY_NAME)
		If Not String.IsNullOrEmpty(primaryKeyName) Then Me.KeyName = primaryKeyName
	End Sub
End Class
