Namespace ActiveRecord
	Partial Public Class Resource

		''' <summary>
		''' Get declared table name by 'resourceType' and by optional called index argument.
		''' </summary>
		''' <param name="resourceType">Class type, inherited from Resource class with declared protected static field 'tables' as array of strings.</param>
		''' <param name="tableIndex">Array index to get proper table name string from declared protected override property 'tables' as array of strings.</param>
		''' <returns>Declared database table name from resource class.</returns>
		Public Shared Function Table(resourceType As Type, Optional tableIndex As Int32 = Database.DEFAUT_CONNECTION_INDEX) As String
			Dim tableAttr As TableAttribute = DirectCast(Attribute.GetCustomAttribute(resourceType, MetaDescriptor.TableAttrType), TableAttribute)
			If Not TypeOf tableAttr Is TableAttribute Then
				Throw New Exception($"Class '{resourceType.FullName}' has no Table attribute.")
			End If
			Dim tables As String() = tableAttr.Tables
			If tableIndex < 0 Or tableIndex + 1 > tables.Length Then
				Throw New Exception($"Class '{resourceType.FullName}' has not defined table in Table attribute under index '{tableIndex}'.")
			End If
			Return tables(tableIndex)
		End Function

		''' <summary>
		''' Get declared table name from generic type 'TResource' and by optional called index argument.
		''' </summary>
		''' <typeparam name="TResource">Class name, inherited from Resource class with declared protected static field 'tables' as array of strings.</typeparam>
		''' <param name="tableIndex">Array index to get proper table name string from declared protected override property 'tables' as array of strings.</param>
		''' <returns>Declared database table name from resource class.</returns>
		Public Shared Function Table(Of TResource)(Optional tableIndex As Int32 = Database.DEFAUT_CONNECTION_INDEX) As String
			Return Resource.Table(GetType(TResource), tableIndex)
		End Function

		Public Shared Function Table(Optional tableIndex As Int32 = Database.DEFAUT_CONNECTION_INDEX) As String
			Return Resource.Table(Tools.GetEntryClassType(), tableIndex)
		End Function

	End Class
End Namespace