Imports System.Data.Common
Imports System.Dynamic
Imports Databasic.ActiveRecord

Namespace ActiveRecord
	Partial Public MustInherit Class Entity

		''' <summary>
		''' Create new Dictionary with instances by generic type and set up all called reader columns into new instances properties or fields.
		''' If reader has no rows, empty list is returned.
		''' </summary>
		''' <typeparam name="TItem">Result list item generic type.</typeparam>
		''' <param name="reader">Reader with values for new instance properties and fields</param>
		''' <param name="columnsByDbNames">Optional class members meta info, indexed by database column names.</param>
		''' <returns>List with values completed by reader columns with the same names as TActiveRecord type fields/properties.</returns>
		Friend Shared Function ToList(Of TItem)(
			reader As DbDataReader,
			Optional ByRef columnsByDbNames As Dictionary(
				Of String, Databasic.MemberInfo
			) = Nothing
		) As List(Of TItem)
			Dim result As New List(Of TItem)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String)
			Dim instance As Object
			Dim itemType As Type = GetType(TItem)
			Dim isEntity As Boolean = Databasic.Constants.EntityType.IsAssignableFrom(itemType)
			Dim descriptableType As Boolean = Tools.IsDescriptableType(itemType)
			If reader.HasRows() Then
				If descriptableType Then
					If (Not TypeOf columnsByDbNames Is Dictionary(Of String, Databasic.MemberInfo)) Then
						columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(itemType)
					End If
					While reader.Read()
						columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
						instance = Activator.CreateInstance(Of TItem)()
						'If isEntity Then DirectCast(instance, Databasic.ActiveRecord.Entity).InitResource()
						ActiveRecord.Entity._readerRowToTypedInstance(
							reader, columns, columnsByDbNames, instance, isEntity
						)
						result.Add(instance)
					End While
				ElseIf Tools.IsPrimitiveType(itemType) Then
					While reader.Read()
						result.Add(DirectCast(reader(0), TItem))
					End While
				Else
					While reader.Read()
						columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
						instance = DirectCast(reader(0), TItem)
						ActiveRecord.Entity._readerRowToAnonymousInstance(
							reader, columns, instance
						)
						result.Add(instance)
					End While
				End If
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToList(Of TItem)(
			reader As DbDataReader,
			itemCompleter As ItemCompleter(Of TItem)
		) As List(Of TItem)
			Dim result As New List(Of TItem)
			If Not TypeOf reader Is DbDataReader Then Return result
			If reader.HasRows() Then
				While reader.Read()
					result.Add(itemCompleter.Invoke(reader))
				End While
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToList(Of TItem)(
			reader As DbDataReader,
			itemCompleter As ItemCompleterWithColumns(Of TItem)
		) As List(Of TItem)
			Dim result As New List(Of TItem)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String)
			If reader.HasRows() Then
				While reader.Read()
					columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
					result.Add(itemCompleter.Invoke(
						reader, columns
					))
				End While
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToList(Of TItem)(
			reader As DbDataReader,
			itemCompleter As ItemCompleterWithAllInfo(Of TItem),
			Optional propertiesOnly As Boolean = True
		) As List(Of TItem)
			Dim result As New List(Of TItem)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String),
				itemType = GetType(TItem),
				columnsByDbNames As New Dictionary(Of String, Databasic.MemberInfo)
			If reader.HasRows() Then
				If Tools.IsDescriptableType(itemType) Then
					If (propertiesOnly) Then
						columnsByDbNames = (
							From item In MetaDescriptor.GetColumnsByDbNames(itemType)
							Where item.Value.MemberInfoType = MemberInfoType.Prop
							Select item
						).ToDictionary(Of String, Databasic.MemberInfo)(
							Function(item) item.Key,
							Function(item) item.Value
						)
					Else
						columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(itemType)
					End If
				End If
				While reader.Read()
					columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
					result.Add(itemCompleter.Invoke(
						reader,
						columns,
						columnsByDbNames
					))
				End While
			End If
			reader.Close()
			Return result
		End Function

	End Class
End Namespace