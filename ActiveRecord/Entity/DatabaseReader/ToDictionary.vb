Imports System.Data.Common
Imports Databasic.ActiveRecord

Namespace ActiveRecord
	Partial Public MustInherit Class Entity

		''' <summary>
		''' Create new Dictionary with keys by first generic type and instances (values) by second generic type 
		''' and set up all called reader columns into new instances properties or fields. By first param as string,
		''' specify which column from reader to use to complete dictionary keys.
		''' If reader has no rows, empty dictionary is returned.
		''' </summary>
		''' <typeparam name="TKey">Result dictionary generic type to complete dictionary keys.</typeparam>
		''' <typeparam name="TValue">Result dictionary generic type to complete dictionary values.</typeparam>
		''' <param name="reader">Reader with values for new instance properties and fields</param>
		''' <param name="databaseKeyColumnName">Reader column name to use to complete result dictionary keys.</param>
		''' <param name="duplicateKeyBehaviour">Thrown an Exception if any previous key is already in result set by default, or keep in result set first completed record value or overwrite duplicate key every time with newly completed value.</param>
		''' <param name="columnsByDbNames">Optional class members meta info, indexed by database column names.</param>
		''' <returns>Dictionary with keys completed by second param for reader column name, values completed by reader columns with the same names as TActiveRecord type fields/properties.</returns>
		Friend Shared Function ToDictionary(Of TKey, TValue)(
			reader As DbDataReader,
			Optional databaseKeyColumnName As String = "",
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException,
			Optional ByRef columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo) = Nothing
		) As Dictionary(Of TKey, TValue)
			Dim result As New Dictionary(Of TKey, TValue)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String)
			Dim key As TKey
			Dim instance As Object
			Dim keyType As Type = GetType(TKey)
			Dim instanceType As Type = GetType(TValue)
			Dim isEntity As Boolean = GetType(Entity).IsAssignableFrom(instanceType)
			Dim descriptableType As Boolean = Entity._isDescriptableType(instanceType)
			If (descriptableType AndAlso Not TypeOf columnsByDbNames Is Dictionary(Of String, Databasic.MemberInfo)) Then
				columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(instanceType)
			End If
			If reader.HasRows() Then
				If String.IsNullOrEmpty(databaseKeyColumnName) Then
					Dim keyColumns As KeyColumns = Resource.KeyColumns(instanceType)
					If keyColumns.Columns.Count <> 1 Then
						Throw New Exception(String.Format(
						 "There was not possible to discover column name from class '{0}' to complete result dictionary keys. " +
						 "Class has no or multiple properties or fields with 'PrimaryKey' or 'UniqueKey' attributes defined. " +
						 "Please define optional param 'databaseKeyColumnName' in method 'ToDictionary()' to complete result " +
						 "dictionary keys.", instanceType.FullName
						))
					End If
					databaseKeyColumnName = keyColumns.Columns.FirstOrDefault().Value
				End If
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
					key = DirectCast(reader.Item(databaseKeyColumnName), TKey)
					If result.ContainsKey(key) Then
						If duplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException Then
							Throw New Exception(String.Format(
								"[Databasic.ActiveRecord.Entity] Duplicate key: '{0}' for result Dictionary<{1}, {2}>.",
								key,
								keyType.ToString(),
								instanceType.ToString()
							))
						ElseIf duplicateKeyBehaviour = DuplicateKeyBehaviour.OverwriteByNewValue Then
							result(key) = instance
						End If
					Else
						result.Add(key, instance)
					End If
				End While
			End If
			reader.Close()
			Return result
		End Function
		''' <summary>
		''' Create new Dictionary with keys by first generic type and instances (values) by second generic type 
		''' and set up all called reader columns into new instances properties or fields. By first param as anonymous function,
		''' specify which field/property from active record instance to use to complete dictionary key for each item.
		''' If reader has no rows, empty dictionary is returned.
		''' </summary>
		''' <typeparam name="TKey">Result dictionary generic type to complete dictionary keys.</typeparam>
		''' <typeparam name="TValue">Result dictionary generic type to complete dictionary values.</typeparam>
		''' <param name="reader">Reader with values for new instance properties and fields</param>
		''' <param name="keySelector">Anonymous function accepting first argument as TActiveRecord instance and returning it's specific field/property value to complete Dictionary key.</param>
		''' <param name="duplicateKeyBehaviour">Thrown an Exception if any previous key is already in result set by default, or keep in result set first completed record value or overwrite duplicate key every time with newly completed value.</param>
		''' <param name="columnsByDbNames">Optional class members meta info, indexed by database column names.</param>
		''' <returns>Dictionary with keys completed by second anonymous function, values completed by reader columns with the same names as TActiveRecord type fields/properties.</returns>
		Friend Shared Function ToDictionary(Of TKey, TValue)(
			reader As DbDataReader,
			Optional keySelector As Func(Of TValue, TKey) = Nothing,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException,
			Optional ByRef columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo) = Nothing
		) As Dictionary(Of TKey, TValue)
			Dim result As New Dictionary(Of TKey, TValue)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String)
			Dim key As TKey
			Dim instance As Object
			Dim keyType = GetType(TKey)
			Dim instanceType As Type = GetType(TValue)
			Dim isEntity As Boolean = GetType(Entity).IsAssignableFrom(instanceType)
			Dim descriptableType As Boolean = Entity._isDescriptableType(instanceType)
			If (descriptableType AndAlso Not TypeOf columnsByDbNames Is Dictionary(Of String, Databasic.MemberInfo)) Then
				columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(instanceType)
			End If
			Dim codeKeyColumnName As String = ""
			If reader.HasRows() Then
				If keySelector = Nothing Then
					Dim keyColumns As KeyColumns = Resource.KeyColumns(instanceType)
					If keyColumns.Columns.Count <> 1 Then
						Throw New Exception(String.Format(
							"There was not possible to discover column name from class '{0}' to complete result dictionary keys. " +
							"Class has no or multiple properties or fields with 'PrimaryKey' or 'UniqueKey' attributes defined. " +
							"Please define optional param 'keySelector' in method 'ToDictionary()' to complete result " +
							"dictionary keys.", instanceType.FullName
						))
					End If
					codeKeyColumnName = keyColumns.Columns.FirstOrDefault().Key
				End If
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
					key = If(keySelector <> Nothing, keySelector(instance), DirectCast(instance(codeKeyColumnName), TKey))
					If result.ContainsKey(key) Then
						If duplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException Then
							Throw New Exception(String.Format(
								"[Databasic.ActiveRecord.Entity] Duplicate key: '{0}' for result Dictionary<{1}, {2}>.",
								key,
								keyType.ToString(),
								instanceType.ToString()
							))
						ElseIf duplicateKeyBehaviour = DuplicateKeyBehaviour.OverwriteByNewValue Then
							result(key) = instance
						End If
					Else
						result.Add(key, instance)
					End If
				End While
			End If
			reader.Close()
			Return result
		End Function

	End Class
End Namespace