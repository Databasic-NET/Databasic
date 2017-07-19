Namespace ActiveRecord
	Partial Public Class Resource

		''' <summary>
		''' Get declared table name by 'resourceType' and by optional called index argument.
		''' </summary>
		''' <param name="resourceType">Class type, inherited from Resource class with declared protected static field 'tables' as array of strings.</param>
		''' <param name="tableIndex">Array index to get proper table name string from declared protected override property 'tables' as array of strings.</param>
		''' <returns>Declared database table name from resource class.</returns>
		Public Shared Function Table(ByRef resourceType As Type, Optional tableIndex As Int32 = 0) As String
			Dim tableAttr As TableAttribute = DirectCast(Attribute.GetCustomAttribute(resourceType, Constants.TableAttrType), TableAttribute)
			If Not TypeOf tableAttr Is TableAttribute Then
				Throw New Exception($"Class '{resourceType.FullName}' has no Table attribute.")
			End If
			Dim tables As String() = tableAttr.Tables
			If tableIndex < 0 OrElse tableIndex + 1 > tables.Length Then
				Throw New Exception($"Class '{resourceType.FullName}' has not defined table in Table attribute under index '{tableIndex}'.")
			End If
			Return tables(tableIndex)
		End Function

		Public Shared Function Table(ByRef classMetaDescription As MetaDescription, Optional tableIndex As Int32 = 0) As String
			Dim tables As String() = classMetaDescription.Tables
			If tableIndex < 0 OrElse tableIndex + 1 > tables.Length Then
				Throw New Exception($"Class '{classMetaDescription.ClassType.FullName}' has not defined table in Table attribute under index '{tableIndex}'.")
			End If
			Return tables(tableIndex)
		End Function
		''' <summary>
		''' Get declared table name from generic type 'TResource' and by optional called index argument.
		''' </summary>
		''' <typeparam name="TResource">Class name, inherited from Resource class with declared protected static field 'tables' as array of strings.</typeparam>
		''' <param name="tableIndex">Array index to get proper table name string from declared protected override property 'tables' as array of strings.</param>
		''' <returns>Declared database table name from resource class.</returns>
		Public Shared Function Table(Of TResource)(Optional tableIndex As Int32 = 0) As String
			Return Resource.Table(GetType(TResource), tableIndex)
		End Function
		''' <summary>
		''' Get declared table name from called class attribute and by optional called index argument.
		''' </summary>
		''' <param name="tableIndex">Array index to get proper table name string from declared protected override property 'tables' as array of strings.</param>
		''' <returns>Declared database table name from resource class.</returns>
		Public Shared Function Table(Optional tableIndex As Int32 = 0) As String
			Return Resource.Table(Tools.GetEntryClassType(), tableIndex)
		End Function





	End Class
End Namespace