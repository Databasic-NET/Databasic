Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading

'Dim sync = New ReaderWriterLockSlim()
'Using sync.Read()
'   ' do reading here
'End Using
'Using sync.Write()
'   ' do writing here
'End Using

<Extension>
Public Module ReaderWriterLockSlimExtensions

    Private NotInheritable Class ReadLockToken
        Implements IDisposable
        Private _sync As ReaderWriterLockSlim
        Public Sub New(sync As ReaderWriterLockSlim)
            Me._sync = sync
            sync.EnterReadLock()
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            If Me._sync IsNot Nothing Then
                Me._sync.ExitReadLock()
                Me._sync = Nothing
            End If
        End Sub
    End Class

    Private NotInheritable Class WriteLockToken
        Implements IDisposable
        Private _sync As ReaderWriterLockSlim
        Public Sub New(sync As ReaderWriterLockSlim)
            Me._sync = sync
            sync.EnterWriteLock()
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            If Me._sync IsNot Nothing Then
                Me._sync.ExitWriteLock()
                Me._sync = Nothing
            End If
        End Sub
    End Class

    <Extension>
    Public Function ReadLock(obj As ReaderWriterLockSlim) As IDisposable
        Return New ReadLockToken(obj)
    End Function

    <Extension>
    Public Function WriteLock(obj As ReaderWriterLockSlim) As IDisposable
        Return New WriteLockToken(obj)
    End Function

End Module