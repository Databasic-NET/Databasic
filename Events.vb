Imports System.Reflection

Public Class Events

    ''' <summary>
    ''' Custom error handler to print or log any database error.
    ''' </summary>
    Public Shared Event [Error] As EventHandler

    Public Shared Sub RaiseError(exception As Exception, Optional e As EventArgs = Nothing)
        If Not TypeOf e Is EventArgs Then e = New EventArgs
        RaiseEvent [Error](exception, e)
#If DEBUG Then
        Throw exception
#End If
    End Sub

End Class
