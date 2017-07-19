Imports System.Diagnostics
Friend Class Tools
	Friend Shared Function GetEntryClassType() As Type
		Return (New StackTrace(True)).GetFrames(2).GetMethod().DeclaringType
		'Dim stackTrace As System.Diagnostics.StackTrace = New System.Diagnostics.StackTrace(True)
		'Dim caller = stackTrace.GetFrames(2)
		'Return caller.GetMethod().DeclaringType
	End Function

	Friend Shared Function GetConnectionIndexByClassAttr(type As Type, Optional throwException As Boolean = True) As Int32
		Dim connAttr As ConnectionAttribute = DirectCast(Attribute.GetCustomAttribute(type, Constants.ConnectionAttrType), ConnectionAttribute)
		If Not TypeOf connAttr Is ConnectionAttribute Then
			If throwException Then
				Throw New Exception(
					$"Class '{type.FullName}' has no 'Connection' attribute. " +
					"Add 'Connection' class attribute or specify connection instance, " +
					"connection config index or connection config name."
				)
			Else
				Return Databasic.Defaults.CONNECTION_INDEX
			End If
		End If
		Return connAttr.ConnectionIndex
	End Function
End Class
