Imports Microsoft.VisualBasic.CompilerServices
Imports System.Reflection
Imports System.Web
Imports System.Web.Hosting

Public Class Events
	''' <summary>
	''' Custom error handler to print or log any database error.
	''' </summary>
	Public Shared Event [Error] As ErrorHandler

	Public Shared Function HasErrorHandler() As Boolean
		Return Operators.ConditionalCompareObjectNotEqual(
			GetType(Events).GetField(
				"ErrorEvent",
				BindingFlags.NonPublic Or BindingFlags.Public Or BindingFlags.Static
			).GetValue(Nothing),
			Nothing,
			False
		)
	End Function

	Public Shared Sub RaiseError(ByVal sqlErrors As SqlErrorsCollection)
		If Not Events.HasErrorHandler() Then
			Throw New SqlException(sqlErrors)
		End If
		Dim errorEvent As ErrorHandler = Events.ErrorEvent
		If (Not errorEvent Is Nothing) Then
			errorEvent.Invoke(Nothing, sqlErrors)
		End If
	End Sub

	Public Shared Sub RaiseError(ByRef ex As Exception)
		If Not Events.HasErrorHandler() Then
			Throw ex
		End If
		Dim errorEvent As ErrorHandler = Events.ErrorEvent
		If (Not errorEvent Is Nothing) Then errorEvent.Invoke(ex, Nothing)
	End Sub

	Public Shared Sub RaiseError(errorMessage As String)
		If Not Events.HasErrorHandler() Then
			Throw New Exception(errorMessage)
		Else
			Try
				Throw New Exception(errorMessage)
			Catch ex As Exception
				Dim errorEvent As ErrorHandler = Events.ErrorEvent
				If (Not errorEvent Is Nothing) Then errorEvent.Invoke(ex, Nothing)
			End Try
		End If
	End Sub

	Friend Shared Sub StaticInit()
		If (
			Operators.CompareString(HttpRuntime.AppDomainAppId, Nothing, False) > 0 And
			HostingEnvironment.IsHosted
		) Then
			AddHandler HttpContext.Current.ApplicationInstance.Disposed, New EventHandler(
				AddressOf Events._webRequestDisposed
			)
		End If
	End Sub

	Private Shared Sub _webRequestDisposed(ByVal sender As Object, ByVal e As EventArgs)
		Connection.Close()
	End Sub

End Class
