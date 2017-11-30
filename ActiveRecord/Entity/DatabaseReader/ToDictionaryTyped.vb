Imports System.Data.Common

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
		''' <param name="keyColumnName">Reader column name to use to complete result dictionary keys.</param>
		''' <param name="duplicateKeyBehaviour">Thrown an Exception if any previous key is already in result set by default, or keep in result set first completed record value or overwrite duplicate key every time with newly completed value.</param>
		''' <param name="columnsByDbNames">Optional class members meta info, indexed by database column names.</param>
		''' <returns>Dictionary with keys completed by second param for reader column name, values completed by reader columns with the same names as TActiveRecord type fields/properties.</returns>
		Friend Shared Function ToDictionary(Of TKey, TValue)(
			reader As DbDataReader,
			Optional keyColumnName As String = "",
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException,
			Optional ByRef columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo) = Nothing
		) As Dictionary(Of TKey, TValue)
			Dim result As New Dictionary(Of TKey, TValue)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String),
				instance As Object,
				itemType As Type = GetType(TValue),
				descriptableType As Boolean = Tools.IsDescriptableType(itemType)
			If reader.HasRows() Then
				If (String.IsNullOrEmpty(keyColumnName)) Then
					keyColumnName = Entity._autoDiscoverCodeKeyColumnNameIfNecessary(itemType)
				End If
				If descriptableType Then
					Dim isEntity As Boolean = Databasic.Constants.EntityType.IsAssignableFrom(itemType)
					If (Not TypeOf columnsByDbNames Is Dictionary(Of String, Databasic.MemberInfo)) Then
						columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(itemType)
					End If
					While reader.Read()
						columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
						instance = Activator.CreateInstance(Of TValue)()
						ActiveRecord.Entity._readerRowToTypedInstance(
							reader, columns, columnsByDbNames, instance, isEntity
						)
						Entity._addInstaceToDictionaryByKeyColumnName(Of TKey, TValue)(
							reader, result, keyColumnName, instance, duplicateKeyBehaviour
						)
					End While
				ElseIf Tools.IsPrimitiveType(itemType) Then
					While reader.Read()
						instance = DirectCast(reader(0), TValue)
						Entity._addInstaceToDictionaryByKeyColumnName(Of TKey, TValue)(
							reader, result, keyColumnName, instance, duplicateKeyBehaviour
						)
					End While
				Else
					While reader.Read()
						columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
						instance = New Databasic.Object()
						ActiveRecord.Entity._readerRowToAnonymousInstance(
							reader, columns, instance
						)
						Entity._addInstaceToDictionaryByKeyColumnName(Of TKey, TValue)(
							reader, result, keyColumnName, instance, duplicateKeyBehaviour
						)
					End While
				End If
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
			Dim columns As New List(Of String),
				instance As Object,
				itemType As Type = GetType(TValue),
				descriptableType As Boolean = Tools.IsDescriptableType(itemType),
				codeKeyColumnName As String = "",
				isEntity As Boolean? = Nothing
			If reader.HasRows() Then
				If descriptableType Then
					isEntity = Databasic.Constants.EntityType.IsAssignableFrom(itemType)
					If (Not TypeOf columnsByDbNames Is Dictionary(Of String, Databasic.MemberInfo)) Then
						columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(itemType)
					End If
					While reader.Read()
						columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
						instance = Activator.CreateInstance(Of TValue)()
						ActiveRecord.Entity._readerRowToTypedInstance(
							reader, columns, columnsByDbNames, instance, isEntity.Value
						)
						Entity._addInstaceToDictionaryByKeySelector(Of TKey, TValue)(
							result, keySelector, instance, duplicateKeyBehaviour, itemType, codeKeyColumnName, isEntity
						)
					End While
				ElseIf Tools.IsPrimitiveType(itemType) Then
					While reader.Read()
						instance = DirectCast(reader(0), TValue)
						Entity._addInstaceToDictionaryByKeySelector(Of TKey, TValue)(
							result, keySelector, instance, duplicateKeyBehaviour, itemType, codeKeyColumnName, isEntity
						)
					End While
				Else
					While reader.Read()
						columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
						instance = New Databasic.Object()
						ActiveRecord.Entity._readerRowToAnonymousInstance(
							reader, columns, instance
						)
						Entity._addInstaceToDictionaryByKeySelector(Of TKey, TValue)(
							result, keySelector, instance, duplicateKeyBehaviour, itemType, codeKeyColumnName, isEntity
						)
					End While
				End If
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToDictionary(Of TKey, TValue)(
			reader As DbDataReader,
			itemCompleter As ItemCompleter(Of TValue),
			Optional keyColumnName As String = "",
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		) As Dictionary(Of TKey, TValue)
			Dim result As New Dictionary(Of TKey, TValue)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim instance As TValue,
				itemType As Type = GetType(TValue)
			If reader.HasRows() Then
				If (String.IsNullOrEmpty(keyColumnName)) Then
					keyColumnName = Entity._autoDiscoverCodeKeyColumnNameIfNecessary(itemType)
				End If
				While reader.Read()
					instance = itemCompleter.Invoke(reader)
					Entity._addInstaceToDictionaryByKeyColumnName(Of TKey, TValue)(
						reader, result, keyColumnName, instance, duplicateKeyBehaviour
					)
				End While
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToDictionary(Of TKey, TValue)(
			reader As DbDataReader,
			itemCompleter As ItemCompleter(Of TValue),
			Optional keySelector As Func(Of TValue, TKey) = Nothing,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		) As Dictionary(Of TKey, TValue)
			Dim result As New Dictionary(Of TKey, TValue)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim instance As TValue,
				itemType As Type = GetType(TValue),
				codeKeyColumnName As String = "",
				isEntity As Boolean? = Nothing
			If reader.HasRows() Then
				While reader.Read()
					instance = itemCompleter.Invoke(reader)
					Entity._addInstaceToDictionaryByKeySelector(Of TKey, TValue)(
						result, keySelector, instance, duplicateKeyBehaviour, itemType, codeKeyColumnName, isEntity
					)
				End While
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToDictionary(Of TKey, TValue)(
			reader As DbDataReader,
			itemCompleter As ItemCompleterWithColumns(Of TValue),
			Optional keyColumnName As String = "",
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		) As Dictionary(Of TKey, TValue)
			Dim result As New Dictionary(Of TKey, TValue)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String),
				instance As TValue,
				itemType As Type = GetType(TValue)
			If reader.HasRows() Then
				If (String.IsNullOrEmpty(keyColumnName)) Then
					keyColumnName = Entity._autoDiscoverCodeKeyColumnNameIfNecessary(itemType)
				End If
				While reader.Read()
					columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
					instance = itemCompleter.Invoke(reader, columns)
					Entity._addInstaceToDictionaryByKeyColumnName(Of TKey, TValue)(
						reader, result, keyColumnName, instance, duplicateKeyBehaviour
					)
				End While
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToDictionary(Of TKey, TValue)(
			reader As DbDataReader,
			itemCompleter As ItemCompleterWithColumns(Of TValue),
			Optional keySelector As Func(Of TValue, TKey) = Nothing,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		) As Dictionary(Of TKey, TValue)
			Dim result As New Dictionary(Of TKey, TValue)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String),
				instance As TValue,
				itemType As Type = GetType(TValue),
				codeKeyColumnName As String = "",
				isEntity As Boolean? = Nothing
			If reader.HasRows() Then
				While reader.Read()
					columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
					instance = itemCompleter.Invoke(reader, columns)
					Entity._addInstaceToDictionaryByKeySelector(Of TKey, TValue)(
						result, keySelector, instance, duplicateKeyBehaviour, itemType, codeKeyColumnName, isEntity
					)
				End While
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToDictionary(Of TKey, TValue)(
			reader As DbDataReader,
			itemCompleter As ItemCompleterWithAllInfo(Of TValue),
			Optional keyColumnName As String = "",
			Optional propertiesOnly As Boolean = True,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		) As Dictionary(Of TKey, TValue)
			Dim result As New Dictionary(Of TKey, TValue)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String),
				instance As TValue,
				itemType As Type = GetType(TValue),
				descriptableType As Boolean = Tools.IsDescriptableType(itemType),
				columnsByDbNames As New Dictionary(Of String, Databasic.MemberInfo)
			If reader.HasRows() Then
				If (String.IsNullOrEmpty(keyColumnName)) Then
					keyColumnName = Entity._autoDiscoverCodeKeyColumnNameIfNecessary(itemType)
				End If
				If (descriptableType) Then
					columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(itemType)
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
					instance = itemCompleter.Invoke(reader, columns, columnsByDbNames)
					Entity._addInstaceToDictionaryByKeyColumnName(Of TKey, TValue)(
						reader, result, keyColumnName, instance, duplicateKeyBehaviour
					)
				End While
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToDictionary(Of TKey, TValue)(
			reader As DbDataReader,
			itemCompleter As ItemCompleterWithAllInfo(Of TValue),
			Optional keySelector As Func(Of TValue, TKey) = Nothing,
			Optional propertiesOnly As Boolean = True,
			Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException
		) As Dictionary(Of TKey, TValue)
			Dim result As New Dictionary(Of TKey, TValue)
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String),
				instance As TValue,
				itemType As Type = GetType(TValue),
				columnsByDbNames As New Dictionary(Of String, MemberInfo),
				codeKeyColumnName As String = "",
				isEntity As Boolean? = Nothing
			If reader.HasRows() Then
				columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(itemType)
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
				While reader.Read()
					columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
					instance = itemCompleter.Invoke(reader, columns, columnsByDbNames)
					Entity._addInstaceToDictionaryByKeySelector(Of TKey, TValue)(
						result, keySelector, instance, duplicateKeyBehaviour, itemType, codeKeyColumnName, isEntity
					)
				End While
			End If
			reader.Close()
			Return result
		End Function

	End Class
End Namespace