Imports System.Data.Common

Public MustInherit Class Transaction
	Inherits DbTransaction

	Public Overrides ReadOnly Property IsolationLevel As Data.IsolationLevel
		Get
			Return Me.Instance.IsolationLevel
		End Get
	End Property

	Public Shadows ReadOnly Property Connection As DbConnection
		Get
			Return Me.ConnectionWrapper.Provider
		End Get
	End Property

	Protected Overrides ReadOnly Property DbConnection As DbConnection
		Get
			Return Me.ConnectionWrapper.Provider
		End Get
	End Property

	Public Overridable Property Instance As DbTransaction
	Public Overridable Property ConnectionWrapper As Connection

	Public Overrides Sub Commit()
		Me.Instance.Commit()
	End Sub

	Public Shadows Function CreateObjRef(requestedType As Type)
		Return Me.Instance.CreateObjRef(requestedType)
	End Function

	Public Shadows Sub Dispose()
		Me.Instance.Dispose()
	End Sub

	Public Shadows Function GetLifetimeService()
		Return Me.Instance.GetLifetimeService()
	End Function

	Public Overrides Function InitializeLifetimeService()
		Return Me.Instance.InitializeLifetimeService()
	End Function

	Public Overrides Sub Rollback()
		Me.Instance.Rollback()
	End Sub

End Class
