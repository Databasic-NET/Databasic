Imports System.ComponentModel
Imports System.Dynamic

Namespace ActiveRecord
	Partial Public MustInherit Class Entity
		Inherits DynamicObject

        Public Overridable Function Delete() As Int32
            Dim connection As Connection = Connection.Get(
                Tools.GetConnectionIndexByClassAttr(Me.GetType(), True)
            )
            Return connection.GetProviderResource().Delete(Me, connection)
        End Function
        ''' <summary>
        ''' Delete ActiveRecord instance by instance Id property (Id database column).
        ''' </summary>
        ''' <param name="connectionIndex">Config connection index to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
        ''' <returns></returns>
        Public Overridable Function Delete(connectionIndex As Int32) As Int32
            Dim connection As Connection = Connection.Get(connectionIndex)
            Return connection.GetProviderResource().Delete(Me, connection)
        End Function
        ''' <summary>
        ''' Delete ActiveRecord instance by instance Id property (Id database column)
        ''' </summary>
        ''' <param name="connectionName">Config connection name to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
        ''' <returns></returns>
        Public Overridable Function Delete(connectionName As String) As Int32
            Dim connection As Connection = Connection.Get(connectionName)
            Return connection.GetProviderResource().Delete(Me, connection)
        End Function
        Public Overridable Function Delete(connection As Connection) As Int32
            Return connection.GetProviderResource().Delete(Me, connection)
        End Function
        Public Overridable Function Delete(transaction As Transaction) As Int32
            Return transaction.ConnectionWrapper.GetProviderResource().Delete(
                Me, transaction
            )
        End Function

    End Class
End Namespace