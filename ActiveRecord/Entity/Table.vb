Imports System.Dynamic

Namespace ActiveRecord
	Partial Public MustInherit Class Entity
		Inherits DynamicObject





		''' <summary>
		''' Get declared table name by 'activeRecordType' and by optional called index argument.
		''' </summary>
		''' <param name="activeRecordType">Class type, inherited from ActiveRecord class with declared protected static field 'tables' as array of strings.</param>
		''' <param name="tableIndex">Array index to get proper table name string from declared protected static field 'tables' as array of strings.</param>
		''' <returns>Declared database table name from active record class.</returns>
		Public Shared Function Table(activeRecordType As Type, Optional tableIndex As Int32 = 0) As String
			Return Resource.Table(activeRecordType, tableIndex)
		End Function

		Public Shared Function Table(ByRef classMetaDescription As MetaDescription, Optional tableIndex As Int32 = 0) As String
			Dim tables As String() = classMetaDescription.Tables
			If tableIndex < 0 OrElse tableIndex + 1 > tables.Length Then
                Throw New Exception(
                    $"Class '{classMetaDescription.ClassType.FullName}' 
                    has not defined table in Table attribute under index 
                    '{tableIndex}'."
                )
            End If
			Return tables(tableIndex)
		End Function
		''' <summary>
		''' Get declared table name from generic type 'TActiveRecord' and by optional called index argument.
		''' </summary>
		''' <typeparam name="TActiveRecord">Class name, inherited from ActiveRecord class with declared protected static field 'tables' as array of strings.</typeparam>
		''' <param name="tableIndex">Array index to get proper table name string from declared protected static field 'tables' as array of strings.</param>
		''' <returns>Declared database table name from active record class.</returns>
		Public Shared Function Table(Of TActiveRecord)(Optional tableIndex As Int32 = 0) As String
			Return Resource.Table(GetType(TActiveRecord), tableIndex)
		End Function
		Public Shared Function Table(Optional tableIndex As Int32 = 0) As String
			Return Resource.Table(Tools.GetEntryClassType(), tableIndex)
		End Function





	End Class
End Namespace
