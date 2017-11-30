Imports System.Data.Common
Imports System.Dynamic
Imports Databasic.ActiveRecord

Namespace ActiveRecord
	Partial Public MustInherit Class Entity

		Friend Shared Function ToList(
			reader As DbDataReader,
			itemType As Type,
			Optional ByRef columnsByDbNames As Dictionary(
				Of String, Databasic.MemberInfo
			) = Nothing
		) As List(Of Object)
			Dim result As New List(Of Object)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String)
			Dim instance As Object
			Dim isEntity As Boolean = Databasic.Constants.EntityType.IsAssignableFrom(itemType)
			Dim descriptableType As Boolean = Tools.IsDescriptableType(itemType)
			If reader.HasRows() Then
				If descriptableType Then
					If (Not TypeOf columnsByDbNames Is Dictionary(Of String, Databasic.MemberInfo)) Then
						columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(itemType)
					End If
					While reader.Read()
						columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
						instance = Activator.CreateInstance(itemType)
						ActiveRecord.Entity._readerRowToTypedInstance(
							reader, columns, columnsByDbNames, instance, isEntity
						)
						result.Add(instance)
					End While
				ElseIf Tools.IsPrimitiveType(itemType) Then
					While reader.Read()
						result.Add(Convert.ChangeType(reader(0), itemType))
					End While
				Else
					While reader.Read()
						columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
						instance = Convert.ChangeType(reader(0), itemType)
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

		Friend Shared Function ToList(
			reader As DbDataReader,
			itemCompleter As ItemCompleter
		) As List(Of Object)
			Dim result As New List(Of Object)
			If Not TypeOf reader Is DbDataReader Then Return result
			If reader.HasRows() Then
				While reader.Read()
					result.Add(itemCompleter.Invoke(reader))
				End While
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToList(
			reader As DbDataReader,
			itemCompleter As ItemCompleterWithColumns
		) As List(Of Object)
			Dim result As New List(Of Object)
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

		Friend Shared Function ToList(
			reader As DbDataReader,
			itemCompleter As ItemCompleterWithAllInfo,
			instanceType As Type,
			Optional propertiesOnly As Boolean = True
		) As List(Of Object)
			Dim result As New List(Of Object)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String),
				columnsByDbNames As New Dictionary(Of String, Databasic.MemberInfo)
			If reader.HasRows() Then
				If Tools.IsDescriptableType(instanceType) Then
					columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(instanceType)
					If (propertiesOnly) Then
						columnsByDbNames = (
							From item In columnsByDbNames
							Where item.Value.MemberInfoType = MemberInfoType.Prop
							Select item
						).ToDictionary(Of String, Databasic.MemberInfo)(
							Function(item) item.Key,
							Function(item) item.Value
						)
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