Imports System.Data.Common

Namespace ActiveRecord
    Partial Public MustInherit Class Entity

		Private Shared Function _autoDiscoverCodeKeyColumnNameIfNecessary(itemType As Type) As String
			Dim keyColumns As KeyColumns = Resource.KeyColumns(itemType)
			If keyColumns.Columns.Count <> 1 Then
				Throw New Exception(String.Format(
					"There was not possible to discover key column from class '{0}' to complete result dictionary keys. " +
					"Class has no properties or fields or multiple properties or fields with 'AutoIncrementPrimaryKey', 'PrimaryKey' or 'UniqueKey' attributes defined. " +
					"Please define optional param 'keySelector' in method 'ToDictionary()' to complete result " +
					"dictionary keys.", itemType.FullName
				))
			End If
			Return keyColumns.Columns.FirstOrDefault().Key
		End Function

		Private Shared Sub _addInstaceToDictionaryByKeyColumnName(Of TKey, TValue)(
			ByRef reader As DbDataReader,
			ByRef result As Dictionary(Of TKey, TValue),
			ByRef keyColumnName As String,
			ByRef instance As Object,
			ByRef duplicateKeyBehaviour As DuplicateKeyBehaviour
		)
			Dim resultDictionaryKey As TKey = DirectCast(reader.Item(keyColumnName), TKey)
			If result.ContainsKey(resultDictionaryKey) Then
				If duplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException Then
					Throw New Exception(String.Format(
						"[Databasic.ActiveRecord.Entity] Duplicate key: '{0}' for result Dictionary<{1}, {2}>.",
						resultDictionaryKey,
						GetType(TKey).ToString(),
						instance.GetType().ToString()
					))
				ElseIf duplicateKeyBehaviour = DuplicateKeyBehaviour.OverwriteByNewValue Then
					result(resultDictionaryKey) = instance
				End If
			Else
				result.Add(resultDictionaryKey, instance)
			End If
		End Sub

		Private Shared Sub _addInstaceToDictionaryByKeyColumnName(
			ByRef reader As DbDataReader,
			ByRef result As Dictionary(Of Object, Object),
			ByRef keyColumnName As String,
			ByRef instance As Object,
			ByRef duplicateKeyBehaviour As DuplicateKeyBehaviour
		)
			If (String.IsNullOrEmpty(keyColumnName)) Then
				keyColumnName = Entity._autoDiscoverCodeKeyColumnNameIfNecessary(instance.GetType())
			End If
			Dim resultDictionaryKey As Object = reader.Item(keyColumnName)
			If result.ContainsKey(resultDictionaryKey) Then
				If duplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException Then
					Throw New Exception(String.Format(
						"[Databasic.ActiveRecord.Entity] Duplicate key: '{0}' for result Dictionary<object, object>.",
						resultDictionaryKey
					))
				ElseIf duplicateKeyBehaviour = DuplicateKeyBehaviour.OverwriteByNewValue Then
					result(resultDictionaryKey) = instance
				End If
			Else
				result.Add(resultDictionaryKey, instance)
			End If
		End Sub

		Private Shared Sub _addInstaceToDictionaryByKeySelector(Of TKey, TValue)(
			ByRef result As Dictionary(Of TKey, TValue),
			ByRef keySelector As Func(Of TValue, TKey),
			ByRef instance As Object,
			ByRef duplicateKeyBehaviour As DuplicateKeyBehaviour,
			ByRef itemType As Type,
			ByRef codeKeyColumnName As String,
			ByRef isEntity As Boolean?
		)
			Dim resultDctKey As TKey
			If (keySelector <> Nothing) Then
				resultDctKey = keySelector.Invoke(instance)
			Else
				If (String.IsNullOrEmpty(codeKeyColumnName)) Then
					If (itemType = Nothing) Then itemType = instance.GetType()
					codeKeyColumnName = Entity._autoDiscoverCodeKeyColumnNameIfNecessary(itemType)
					If (Not isEntity.HasValue) Then
						isEntity = Databasic.Constants.EntityType.IsAssignableFrom(itemType)
					End If
				End If
				If (isEntity) Then
					resultDctKey = instance(codeKeyColumnName)
				Else
					If (itemType = Nothing) Then itemType = instance.GetType()
					Throw New Exception(String.Format(
						"There was not possible to discover key column from class '{0}' to complete result dictionary keys. " +
						"Class is not extended from 'Databasic.ActiveRecord.Entity' type. " +
						"Please define optional param 'keySelector' in method 'ToDictionary()' to complete result " +
						"dictionary keys.", itemType.FullName
					))
				End If
			End If
			If result.ContainsKey(resultDctKey) Then
				If duplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException Then
					Throw New Exception(String.Format(
						"[Databasic.ActiveRecord.Entity] Duplicate key: '{0}' for result Dictionary<{1}, {2}>.",
						resultDctKey,
						GetType(TKey).ToString(),
						itemType.ToString()
					))
				ElseIf duplicateKeyBehaviour = DuplicateKeyBehaviour.OverwriteByNewValue Then
					result(resultDctKey) = instance
				End If
			Else
				result.Add(resultDctKey, instance)
			End If
		End Sub

		Private Shared Sub _addInstaceToDictionaryByKeySelector(
			ByRef result As Dictionary(Of Object, Object),
			ByRef keySelector As Func(Of Object, Object),
			ByRef instance As Object,
			ByRef duplicateKeyBehaviour As DuplicateKeyBehaviour,
			ByRef codeKeyColumnName As String,
			ByRef isEntity As Boolean?
		)
			Dim resultDctKey As Object
			If (keySelector <> Nothing) Then
				resultDctKey = keySelector.Invoke(instance)
			Else
				If (String.IsNullOrEmpty(codeKeyColumnName)) Then
					Dim itemType As Type = instance.GetType()
					codeKeyColumnName = Entity._autoDiscoverCodeKeyColumnNameIfNecessary(itemType)
					If (Not isEntity.HasValue) Then
						isEntity = Databasic.Constants.EntityType.IsAssignableFrom(itemType)
					End If
				End If
				If (isEntity) Then
					resultDctKey = instance(codeKeyColumnName)
				Else
					Throw New Exception(String.Format(
						"There was not possible to discover key column from class '{0}' to complete result dictionary keys. " +
						"Class is not extended from 'Databasic.ActiveRecord.Entity' type. " +
						"Please define optional param 'keySelector' in method 'ToDictionary()' to complete result " +
						"dictionary keys.", instance.GetType().FullName
					))
				End If
			End If
			If result.ContainsKey(resultDctKey) Then
				If duplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException Then
					Throw New Exception(String.Format(
						"[Databasic.ActiveRecord.Entity] Duplicate key: '{0}' for result Dictionary<object, object>.",
						resultDctKey
					))
				ElseIf duplicateKeyBehaviour = DuplicateKeyBehaviour.OverwriteByNewValue Then
					result(resultDctKey) = instance
				End If
			Else
				result.Add(resultDctKey, instance)
			End If
		End Sub

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
        ''' Set up current reader row columns into instance properties and fields.
        ''' </summary>
        ''' <param name="reader">DbDataReader with current row moved, where current row will be used to fill instance properties and fields.</param>
        ''' <param name="readerColumnNames">Columns in reader in proper order.</param>
        ''' <param name="instance">Instance to fill.</param>
        Private Shared Sub _readerRowToTypedInstance(
            ByRef reader As DbDataReader,
            ByRef readerColumnNames As List(Of String),
            ByRef columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo),
            ByRef instance As Object,
            isEntity As Boolean
        )
            Dim mi As Databasic.MemberInfo
            Dim rawValueTypeCode As TypeCode
            Dim assigned As Boolean
            Dim propInfo As Reflection.PropertyInfo
            Dim targetName As String
            Dim formatProvider As IFormatProvider
            Dim rawValue As Object
            Dim targetValue As Object
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
                    ElseIf mi.Type.IsEnum Then
                        targetValue = Entity._parseEnumMemberValue(mi, rawValue)
                    Else
                        formatProvider = If(mi.FormatProvider, System.Globalization.CultureInfo.CurrentCulture)
                        targetValue = If(TypeOf rawValue Is DBNull, Nothing, Convert.ChangeType(rawValue, mi.Type, formatProvider))
                    End If
                    If (mi.MemberInfoType = MemberInfoType.Prop) Then
                        propInfo = DirectCast(mi.MemberInfo, Reflection.PropertyInfo)
                        If propInfo.CanWrite Then
                            propInfo.SetValue(instance, targetValue, Nothing)
                            assigned = True
                        End If
                    Else
                        DirectCast(mi.MemberInfo, Reflection.FieldInfo).SetValue(instance, targetValue)
                        assigned = True
                    End If
                    targetName = mi.Name
                End If
                If isEntity Then
					Entity._setUpEntityValueToReserveStoreAndInitialValues(
						DirectCast(instance, Databasic.ActiveRecord.Entity),
						targetName, rawValue, targetValue, assigned
					)
				End If
			Next
		End Sub

		Private Shared Sub _readerRowToAnonymousInstance(
			ByRef reader As DbDataReader,
			ByRef readerColumnNames As List(Of String),
			ByRef instance As Databasic.Object
		)
			For Each readerColumnName As String In readerColumnNames
				Entity._setUpEntityValueToReserveStoreAndInitialValues(
					instance, readerColumnName, Nothing, reader(readerColumnName), False
				)
			Next
		End Sub

		Private Shared Function _parseEnumMemberValue(mi As MemberInfo, rawValue As Object) As Object
			If mi.UseEnumUnderlyingValue Then
				Dim underType As Type = System.Enum.GetUnderlyingType(mi.Type),
					underValue = Convert.ChangeType(rawValue, underType),
					values = System.[Enum].GetValues(mi.Type)
				For i = 0 To values.Length - 1
					If (underValue = values(i)) Then
						Return values(i)
					End If
				Next
				Return Nothing
			Else
				Return System.[Enum].Parse(mi.Type, rawValue.ToString())
			End If
		End Function

		Private Shared Sub _setUpEntityValueToReserveStoreAndInitialValues(
			entity As Databasic.ActiveRecord.Entity,
			targetName As String,
			rawValue As Object,
			targetValue As Object,
			assigned As Boolean
		)
			targetValue = If(targetValue, rawValue)
			If Not assigned Then
				entity.getReserveStore().Item(targetName) = targetValue
			End If
			If entity.getInitialData().ContainsKey(targetName) Then
				entity.getInitialData()(targetName) = targetValue
			Else
				entity.getInitialData().Add(targetName, targetValue)
			End If
		End Sub

	End Class
End Namespace