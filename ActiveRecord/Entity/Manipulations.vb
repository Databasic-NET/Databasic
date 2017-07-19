Imports System.Dynamic
Imports System.Reflection
'Imports System.Runtime.CompilerServices

Namespace ActiveRecord
	Partial Public MustInherit Class Entity
		Inherits DynamicObject





		Public Function Save(insertNew As Boolean) As Int32
			Dim connection As Connection = Connection.Get(Tools.GetConnectionIndexByClassAttr(Me.GetType()))
			Return connection.GetProviderResource().Save(insertNew, Me, connection)
		End Function
		''' <summary>
		''' Insert/Update ActiveRecord instance by non-existing/existing instance Id property (Id database column).
		''' </summary>
		''' <param name="connectionIndex">Config connection index to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
		''' <returns></returns>
		Public Function Save(insertNew As Boolean, connectionIndex As Int32) As Int32
			Dim connection As Connection = Connection.Get(connectionIndex)
			Return connection.GetProviderResource().Save(insertNew, Me, connection)
		End Function
		''' <summary>
		''' Insert/Update ActiveRecord instance by non-existing/existing instance Id property (Id database column).
		''' </summary>
		''' <param name="connectionName">Config connection name to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
		''' <returns></returns>
		Public Function Save(insertNew As Boolean, connectionName As String) As Int32
			Dim connection As Connection = Connection.Get(connectionName)
			Return connection.GetProviderResource().Save(insertNew, Me, connection)
		End Function
		Public Function Save(insertNew As Boolean, connection As Connection) As Int32
			Return connection.GetProviderResource().Save(insertNew, Me, connection)
		End Function
		Public Function Save(insertNew As Boolean, transaction As Transaction) As Int32
			Return transaction.ConnectionWrapper.GetProviderResource().Save(insertNew, Me, transaction)
		End Function





		Public Function Delete() As Int32
			Dim connection As Connection = Connection.Get(Tools.GetConnectionIndexByClassAttr(Me.GetType()))
			Return connection.GetProviderResource().Delete(Me, connection)
		End Function
		''' <summary>
		''' Delete ActiveRecord instance by instance Id property (Id database column).
		''' </summary>
		''' <param name="connectionIndex">Config connection index to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
		''' <returns></returns>
		Public Function Delete(connectionIndex As Int32) As Int32
			Dim connection As Connection = Connection.Get(connectionIndex)
			Return connection.GetProviderResource().Delete(Me, connection)
		End Function
		''' <summary>
		''' Delete ActiveRecord instance by instance Id property (Id database column)
		''' </summary>
		''' <param name="connectionName">Config connection name to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
		''' <returns></returns>
		Public Function Delete(connectionName As String) As Int32
			Dim connection As Connection = Connection.Get(connectionName)
			Return connection.GetProviderResource().Delete(Me, connection)
		End Function
		Public Function Delete(connection As Connection) As Int32
			Return connection.GetProviderResource().Delete(Me, connection)
		End Function
		Public Function Delete(transaction As Transaction) As Int32
			Return transaction.ConnectionWrapper.GetProviderResource().Delete(Me, transaction)
		End Function





	End Class
End Namespace