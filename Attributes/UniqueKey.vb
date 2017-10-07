Public Class UniqueKeyAttribute
 Inherits Attribute
	Public KeyName As String = Databasic.Defaults.KEY_NAME
	Public Sub New(Optional uniqueKeyName As String = Databasic.Defaults.KEY_NAME)
		If Not String.IsNullOrEmpty(uniqueKeyName) Then Me.KeyName = uniqueKeyName
	End Sub
End Class