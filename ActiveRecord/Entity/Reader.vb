Imports System.Data.Common
Imports Databasic.ActiveRecord

Namespace ActiveRecord
    Partial Public MustInherit Class Entity
        ''' <summary>
        ''' Create new instance by generic type and set up all called dictionary keys into new instance properties or fields.
        ''' </summary>
        ''' <typeparam name="TActiveRecord">New instance type.</typeparam>
        ''' <param name="data">Data with values for new instance properties and fields.</param>
        ''' <returns>New instance by generic type with values by second param.</returns>
        Public Shared Function ToInstance(Of TActiveRecord)(data As Dictionary(Of String, Object)) As TActiveRecord
            Dim instance As TActiveRecord = DirectCast(Activator.CreateInstance(GetType(TActiveRecord)), TActiveRecord)
            TryCast(instance, Entity).SetUp(data, True)
            Return instance
        End Function
        ''' <summary>
        ''' Create new instance by generic type and set up all called reader columns with one row at minimal into 
        ''' new instance properties or fields. If TResult is primitive type, reader has to return single row and 
        ''' single column select result and that result is converted and returned as to primitive value only.
        ''' If reader has no rows, Nothing is returned.
        ''' </summary>
        ''' <typeparam name="TResult">New result class instance type or any primitive type for single row and single column select result.</typeparam>
        ''' <param name="reader">Reader with values for new instance properties and fields</param>
        ''' <returns></returns>
        Public Shared Function ToInstance(Of TResult)(reader As DbDataReader) As TResult
            Dim instance As TResult = Nothing
            Dim columns As New List(Of String)
            Dim instanceType = GetType(TResult)
            Dim isEntity As Boolean = GetType(Entity).IsAssignableFrom(instanceType)
            Dim primitiveType As Boolean = instanceType.IsPrimitive Or instanceType.FullName = "System.String"
            Dim columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo) = If(primitiveType, Nothing, MetaDescriptor.GetColumnsByDbNames(instanceType))
            If reader.HasRows() Then
                reader.Read()
                If primitiveType Then
                    instance = Convert.ChangeType(reader.Item(0), instanceType)
                Else
                    instance = DirectCast(Activator.CreateInstance(instanceType), TResult)
                    columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
                    ActiveRecord.Entity._readerRowToInstance(
                        reader, columns, columnsByDbNames, instance, isEntity
                    )
                End If
            End If
            reader.Close()
            Return instance
        End Function
        ''' <summary>
        ''' Create new Dictionary with instances by generic type and set up all called reader columns into new instances properties or fields.
        ''' If reader has no rows, empty list is returned.
        ''' </summary>
        ''' <typeparam name="TActiveRecord">Result list item generic type.</typeparam>
        ''' <param name="reader">Reader with values for new instance properties and fields</param>
        ''' <returns></returns>
        Public Shared Function ToList(Of TActiveRecord)(reader As DbDataReader) As List(Of TActiveRecord)
            Dim result As New List(Of TActiveRecord)
            Dim columns As New List(Of String)
            Dim instance As TActiveRecord
            Dim instanceType As Type = GetType(TActiveRecord)
            Dim isEntity As Boolean = GetType(Entity).IsAssignableFrom(instanceType)
            Dim primitiveType As Boolean = instanceType.IsPrimitive Or instanceType.FullName = "System.String"
            Dim columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo) = If(primitiveType, Nothing, MetaDescriptor.GetColumnsByDbNames(instanceType))
            If reader.HasRows() Then
                While reader.Read()
                    If primitiveType Then
                        instance = DirectCast(reader(0), TActiveRecord)
                    Else
                        columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
                        instance = DirectCast(Activator.CreateInstance(instanceType), TActiveRecord)
                        ActiveRecord.Entity._readerRowToInstance(
                            reader, columns, columnsByDbNames, instance, isEntity
                        )
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
        ''' <returns>Dictionary with keys completed by second param for reader column name, values completed by reader columns with the same names as TActiveRecord type fields/properties.</returns>
        Public Shared Function ToDictionary(Of TKey, TValue)(reader As DbDataReader, Optional databaseKeyColumnName As String = "", Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException) As Dictionary(Of TKey, TValue)
            Dim result As New Dictionary(Of TKey, TValue)
            Dim columns As New List(Of String)
            Dim key As TKey
            Dim instance As TValue
            Dim keyType As Type = GetType(TKey)
            Dim instanceType As Type = GetType(TValue)
            Dim isEntity As Boolean = GetType(Entity).IsAssignableFrom(instanceType)
            Dim primitiveType As Boolean = instanceType.IsPrimitive Or instanceType.FullName = "System.String"
            Dim columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo) = If(primitiveType, Nothing, MetaDescriptor.GetColumnsByDbNames(instanceType))
            If reader.HasRows() Then
                If String.IsNullOrEmpty(databaseKeyColumnName) Then databaseKeyColumnName = Resource.UniqueColumn(instanceType)
                While reader.Read()
                    If primitiveType Then
                        instance = DirectCast(reader(0), TValue)
                    Else
                        columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
                        instance = DirectCast(Activator.CreateInstance(instanceType), TValue)
                        ActiveRecord.Entity._readerRowToInstance(
                            reader, columns, columnsByDbNames, instance, isEntity
                        )
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
        ''' <typeparam name="TActiveRecord">Result dictionary generic type to complete dictionary values.</typeparam>
        ''' <param name="reader">Reader with values for new instance properties and fields</param>
        ''' <param name="keySelector">Anonymous function accepting first argument as TActiveRecord instance and returning it's specific field/property value to complete Dictionary key.</param>
        ''' <param name="duplicateKeyBehaviour">Thrown an Exception if any previous key is already in result set by default, or keep in result set first completed record value or overwrite duplicate key every time with newly completed value.</param>
        ''' <returns>Dictionary with keys completed by second anonymous function, values completed by reader columns with the same names as TActiveRecord type fields/properties.</returns>
        Public Shared Function ToDictionary(Of TKey, TActiveRecord)(reader As DbDataReader, keySelector As Func(Of TActiveRecord, TKey), Optional duplicateKeyBehaviour As DuplicateKeyBehaviour = DuplicateKeyBehaviour.ThrownException) As Dictionary(Of TKey, TActiveRecord)
            Dim result As New Dictionary(Of TKey, TActiveRecord)
            Dim columns As New List(Of String)
            Dim key As TKey
            Dim instance As TActiveRecord
            Dim keyType = GetType(TKey)
            Dim instanceType As Type = GetType(TActiveRecord)
            Dim isEntity As Boolean = GetType(Entity).IsAssignableFrom(instanceType)
            Dim primitiveType As Boolean = instanceType.IsPrimitive Or instanceType.FullName = "System.String"
            Dim columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo) = If(primitiveType, Nothing, MetaDescriptor.GetColumnsByDbNames(instanceType))
            If reader.HasRows() Then
                While reader.Read()

                    If primitiveType Then
                        instance = DirectCast(reader(0), TActiveRecord)
                    Else
                        columns = If(columns.Count = 0, ActiveRecord.Entity._getReaderRowColumns(reader), columns)
                        instance = DirectCast(Activator.CreateInstance(instanceType), TActiveRecord)
                        ActiveRecord.Entity._readerRowToInstance(
                            reader, columns, columnsByDbNames, instance, isEntity
                        )
                    End If

                    key = keySelector(instance)
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
        ''' Set up current reader row columns into instance properties and fields.
        ''' </summary>
        ''' <param name="reader">DbDataReader with current row moved, where current row will be used to fill instance properties and fields.</param>
        ''' <param name="readerColumnNames">Columns in reader in proper order.</param>
        ''' <param name="instance">Instance to fill.</param>
        Private Shared Sub _readerRowToInstance(ByRef reader As DbDataReader, ByRef readerColumnNames As List(Of String), ByRef columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo), ByRef instance As Object, isEntity As Boolean)
            Dim mi As Databasic.MemberInfo
            Dim stringType As Type = GetType(String)
            Dim assigned As Boolean
            Dim propInfo As Reflection.PropertyInfo
            Dim targetName As String
            Dim formatProvider As IFormatProvider
            Dim rawValue As Object
            Dim targetValue As Object
            For Each readerColumnName As String In readerColumnNames
                assigned = False
                targetName = readerColumnName
                rawValue = reader(readerColumnName)
                targetValue = Nothing
                If columnsByDbNames.ContainsKey(readerColumnName) Then
                    mi = columnsByDbNames(readerColumnName)
                    targetName = mi.Name
                    If (mi.Type Is stringType AndAlso mi.TrimChars.Length > 0) Then
                        rawValue = rawValue.ToString().Trim(mi.TrimChars)
                    End If
                    formatProvider = If(mi.FormatProvider, System.Globalization.CultureInfo.CurrentCulture)
                    targetValue = If(TypeOf rawValue Is DBNull, Nothing, Convert.ChangeType(rawValue, mi.Type, formatProvider))
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
                End If
                If isEntity Then
                    targetValue = If(targetValue, rawValue)
                    If Not assigned Then
                        instance._reserveStore.Item(targetName) = targetValue
                    End If
                    If instance._initialData.ContainsKey(targetName) Then
                        instance._initialData(targetName) = targetValue
                    Else
                        instance._initialData.Add(targetName, targetValue)
                    End If
                End If
            Next
        End Sub
    End Class
End Namespace