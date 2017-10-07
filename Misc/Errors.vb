Imports System
Imports System.Reflection
Imports System.Diagnostics
Imports System.Runtime.CompilerServices

Public Class SqlException
    Inherits Exception

    Public Overloads ReadOnly Property Data As SqlErrorsCollection
        Get
            Return Me._data
        End Get
    End Property

    Public Overrides ReadOnly Property Message As String
        Get
            Return Me._message
        End Get
    End Property

    Private _data As SqlErrorsCollection
    Private _message As String

    Public Sub New(ByVal sqlErrors As SqlErrorsCollection)
        Me._data = sqlErrors
        Me._message = If((sqlErrors.Count > 0), sqlErrors.Item(0).Message, "SQL Error(s).")
    End Sub

End Class

<Serializable, DefaultMember("Item")>
Public Class SqlErrorsCollection
    Inherits ArrayList
    Public Overloads Function Add(ByVal sqlError As SqlError) As Integer
        Return MyBase.Add(sqlError)
    End Function


    Default Public Overloads Property Item(ByVal index As Integer) As SqlError
        Get
            Return DirectCast(MyBase.Item(index), SqlError)
        End Get
        Set(ByVal value As SqlError)
            MyBase.Item(index) = value
        End Set
    End Property

End Class

Public Class SqlError

    Public Property Code As Integer
        Get
            Return Me._code
        End Get
        Set(value As Integer)
            Me._code = value
        End Set
    End Property

    Public Property Message As String
        Get
            Return Me._message
        End Get
        Set(ByVal AutoPropertyValue As String)
            Me._message = AutoPropertyValue
        End Set
    End Property

    <CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)>
    Private _code As Integer
    <CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)>
    Private _message As String

    Public Sub New()
        Me.Code = 0
        Me.Message = ""
    End Sub
End Class
