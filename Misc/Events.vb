Imports System.Reflection

Public Class Events

	''' <summary>
	''' Custom error handler to print or log any database error.
	''' </summary>
	Public Shared Event [Error] As ErrorHandler

	''' <summary>
	''' Typed handler for catched Databasic error.
	''' </summary>
	''' <param name="e">Catched Exception.</param>
	''' <param name="args">Empty event args object.</param>
	Public Delegate Sub ErrorHandler(e As Exception, args As EventArgs)

	Public Shared Sub RaiseError(ex As Exception, Optional args As EventArgs = Nothing)
		If Not TypeOf args Is EventArgs Then args = New EventArgs
		' check if Error event has any handlers, if not, thrown an exception
		Dim fi As FieldInfo = GetType(Events).GetField("ErrorEvent", BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static)
		Dim del As Object = fi.GetValue(Nothing)
		If del <> Nothing Then
			RaiseEvent [Error](ex, args)
		Else
			Throw ex
		End If
	End Sub

End Class
