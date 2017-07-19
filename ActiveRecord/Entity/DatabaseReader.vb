Imports System.Data.Common
Imports Databasic.ActiveRecord

Namespace ActiveRecord
	Partial Public MustInherit Class Entity
		''' <summary>
		''' Create new instance by generic type and set up all called dictionary keys into new instance properties or fields.
		''' </summary>
		''' <typeparam name="TValue">New instance type.</typeparam>
		''' <param name="data">Data with values for new instance properties and fields.</param>
		''' <returns>New instance by generic type with values by second param.</returns>
		Friend Shared Function ToInstance(Of TValue)(data As Dictionary(Of String, Object)) As TValue
			Dim instance As TValue = Activator.CreateInstance(Of TValue)()
			TryCast(instance, Entity).SetUp(data, True)
			Return instance
		End Function
		''' <summary>
		''' Create new instance by generic type and set up all called reader columns with one row at minimal into 
		''' new instance properties or fields. If TResult is primitive type, reader has to return single row and 
		''' single column select result and that result is converted and returned as to primitive value only.
		''' If reader has no rows, Nothing is returned.
		''' </summary>
		''' <typeparam name="TValue">New result class instance type or any primitive type for single row and single column select result.</typeparam>
		''' <param name="reader">Reader with values for new instance properties and fields.</param>
		''' <param name="columnsByDbNames">Optional class members meta info, indexed by database column names.</param>
		''' <returns>New instance as primitive type or as class instance, completed by reader columns with the same names as TActiveRecord type fields/properties.</returns>
		Friend Shared Function ToInstance(Of TValue)(reader As DbDataReader, Optional ByRef columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo) = Nothing) As TValue
			Dim result As Object = Nothing
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String)
			Dim instanceType = GetType(TValue)
			Dim isEntity As Boolean = GetType(Entity).IsAssignableFrom(instanceType)
			Dim descriptableType As Boolean = Entity._isDescriptableType(instanceType)
			If (descriptableType AndAlso Not TypeOf columnsByDbNames Is Dictionary(Of String, Databasic.MemberInfo)) Then
				columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(instanceType)
			End If
			If reader.HasRows() Then
				reader.Read()
				If descriptableType Then
					result = Activator.CreateInstance(Of TValue)()
					'If isEntity Then DirectCast(result, Databasic.ActiveRecord.Entity).InitResource()
					columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
					ActiveRecord.Entity._readerRowToInstance(
						reader, columns, columnsByDbNames, result, isEntity
					)
				Else
					result = Convert.ChangeType(reader.Item(0), instanceType)
				End If
			End If
			reader.Close()
			Return result
		End Function
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
		''' <summary>
		''' Get column names from reader as list of strings.
		''' </summary>
		''' <param name="reader"></param>
		''' <returns></returns>
		Private Shared Function _getReaderRowColumns(reader As DbDataReader) As List(Of String)
			Return Enumerable.ToList(Of String)(
				Enumerable.Select(Of Integer, String)(
					Enumerable.Range(0, reader.FieldCount),
					New Func(Of Integer, String)(AddressOf reader.GetName)
				)
			)
		End Function
		''' <summary>
		''' Return True if type is descriptable type by custom attributes, not primitive, not an object.
		''' </summary>
		''' <param name="instanceType">Instance type to check out.</param>
		''' <returns>True if descriptable.</returns>
		Private Shared Function _isDescriptableType(ByRef instanceType As Type) As Boolean
			Return Not instanceType.IsPrimitive AndAlso instanceType.FullName <> "System.String" AndAlso instanceType.FullName <> "System.Object"
		End Function
		''' <summary>
		''' Set up current reader row columns into instance properties and fields.
		''' </summary>
		''' <param name="reader">DbDataReader with current row moved, where current row will be used to fill instance properties and fields.</param>
		''' <param name="readerColumnNames">Columns in reader in proper order.</param>
		''' <param name="instance">Instance to fill.</param>
		Private Shared Sub _readerRowToInstance(ByRef reader As DbDataReader, ByRef readerColumnNames As List(Of String), ByRef columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo), ByRef instance As Object, isEntity As Boolean)
			Dim mi As Databasic.MemberInfo
			Dim rawValueTypeCode As TypeCode
			Dim assigned As Boolean
			Dim propInfo As Reflection.PropertyInfo
			Dim targetName As String
			Dim formatProvider As IFormatProvider
			Dim rawValue As Object
			Dim targetValue As Object
			Dim entity As Databasic.ActiveRecord.Entity
			For Each readerColumnName As String In readerColumnNames
				assigned = False
				rawValue = reader(readerColumnName)
				targetName = readerColumnName
				targetValue = Nothing
				If columnsByDbNames.ContainsKey(readerColumnName) Then
					mi = columnsByDbNames(readerColumnName)
					rawValueTypeCode = Type.GetTypeCode(rawValue.GetType())
					If (rawValueTypeCode = Constants.StringTypeCode AndAlso mi.TrimChars.Length > 0) Then
						rawValue = rawValue.ToString().Trim(mi.TrimChars)
					ElseIf rawValueTypeCode = Type.GetTypeCode(mi.Type) Then
						targetValue = rawValue
					Else
						formatProvider = If(mi.FormatProvider, System.Globalization.CultureInfo.CurrentCulture)
						targetValue = If(TypeOf rawValue Is DBNull, Nothing, Convert.ChangeType(rawValue, mi.Type, formatProvider))
					End If
					If (mi.MemberInfoType = MemberInfoType.Prop) Then
						propInfo = DirectCast(mi.MemberInfo, Reflection.PropertyInfo)
						If propInfo.CanWrite Then
							propInfo.SetValue(instance, targetValue, Nothing)
							assigned = True
						Else
							targetName = mi.Name
						End If
					Else
						DirectCast(mi.MemberInfo, Reflection.FieldInfo).SetValue(instance, targetValue)
						assigned = True
					End If
				End If
				If isEntity Then
					targetValue = If(targetValue, rawValue)
					entity = DirectCast(instance, Databasic.ActiveRecord.Entity)
					If Not assigned Then
						entity._reserveStore.Item(targetName) = targetValue
					End If
					If entity._initialData.ContainsKey(targetName) Then
						entity._initialData(targetName) = targetValue
					Else
						entity._initialData.Add(targetName, targetValue)
					End If
				End If
			Next
		End Sub
	End Class
End Namespace