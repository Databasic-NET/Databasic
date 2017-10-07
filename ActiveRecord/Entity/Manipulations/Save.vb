Imports System.ComponentModel
Imports System.Dynamic

Namespace ActiveRecord
    Partial Public MustInherit Class Entity
        Inherits DynamicObject

        Public Function Save(insertNew As Boolean) As Integer
            Dim connection As Connection = Connection.Get(
                Tools.GetConnectionIndexByClassAttr(Me.GetType(), True)
            )
            Return connection.GetProviderResource().Save(insertNew, Me, connection)
        End Function

        Public Function Save(insertNew As Boolean, connection As Connection) As Integer
            Return connection.GetProviderResource().Save(insertNew, Me, connection)
        End Function

        Public Function Save(insertNew As Boolean, transaction As Transaction) As Integer
            Return transaction.ConnectionWrapper.GetProviderResource().Save(
                insertNew, Me, transaction
            )
        End Function

        Public Function Save(insertNew As Boolean, connectionIndex As Integer) As Integer
            Dim connection As Connection = Connection.Get(connectionIndex)
            Return connection.GetProviderResource().Save(
                insertNew, Me, connection
            )
        End Function

        Public Function Save(insertNew As Boolean, connectionName As String) As Integer
            Dim connection As Connection = Connection.Get(connectionName)
            Return connection.GetProviderResource().Save(
                insertNew, Me, connection
            )
        End Function

    End Class
End Namespace