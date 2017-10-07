Imports System.Data.Common
Imports Databasic.ActiveRecord

Namespace ActiveRecord
	Partial Public MustInherit Class Entity

		''' <summary>
		''' Create new Dictionary with instances by generic type and set up all called reader columns into new instances properties or fields.
		''' If reader has no rows, empty list is returned.
		''' </summary>
		''' <typeparam name="TValue">Result list item generic type.</typeparam>
		''' <param name="reader">Reader with values for new instance properties and fields</param>
		''' <param name="columnsByDbNames">Optional class members meta info, indexed by database column names.</param>
		''' <returns>List with values completed by reader columns with the same names as TActiveRecord type fields/properties.</returns>
		Friend Shared Function ToList(Of TValue)(reader As DbDataReader, Optional ByRef columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo) = Nothing) As List(Of TValue)
			Dim result As New List(Of TValue)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String)
			Dim instance As Object
			Dim instanceType As Type = GetType(TValue)
			Dim isEntity As Boolean = GetType(Entity).IsAssignableFrom(instanceType)
			Dim descriptableType As Boolean = Entity._isDescriptableType(instanceType)
			If (descriptableType AndAlso Not TypeOf columnsByDbNames Is Dictionary(Of String, Databasic.MemberInfo)) Then
				columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(instanceType)
			End If
			If reader.HasRows() Then
				While reader.Read()
					If descriptableType Then
						columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
						instance = Activator.CreateInstance(Of TValue)()
						'If isEntity Then DirectCast(instance, Databasic.ActiveRecord.Entity).InitResource()
						ActiveRecord.Entity._readerRowToInstance(
							reader, columns, columnsByDbNames, instance, isEntity
						)
					Else
						instance = DirectCast(reader(0), TValue)
					End If
					result.Add(instance)
				End While
			End If
			reader.Close()
			Return result
		End Function

	End Class
End Namespace