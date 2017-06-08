Imports System.Data.Common
Imports Databasic.ActiveRecord

Namespace Core
    Public Class Reader
        ''' <summary>
        ''' Convert single row and single column select result into desired type specified by generic argument.
        ''' </summary>
        ''' <typeparam name="TValue">Result variable type.</typeparam>
        ''' <returns>Retyped single row and single column select result.</returns>
        Public Shared Function ToValue(Of TValue)(reader As DbDataReader, Optional closeReaderAfterSetUp As Boolean = True) As TValue
            Dim result As TValue
            If reader.HasRows() Then
                reader.Read()
                result = Convert.ChangeType(reader.Item(0), GetType(TValue))
            Else
                result = Nothing
            End If
            If closeReaderAfterSetUp Then
                reader.Close()
            End If
            Return result
        End Function
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
        ''' Create new instance by generic type and set up all called reader columns with one row at minimal into new instance properties or fields.
        ''' If reader has no rows, Nothing is returned.
        ''' </summary>
        ''' <typeparam name="TActiveRecord">New instance type.</typeparam>
        ''' <param name="reader">Reader with values for new instance properties and fields</param>
        ''' <param name="closeReaderAfterSetUp">Automaticly close reader after all.</param>
        ''' <returns></returns>
        Public Shared Function ToInstance(Of TActiveRecord)(reader As DbDataReader, Optional closeReaderAfterSetUp As Boolean = True) As TActiveRecord
            Dim instance As TActiveRecord = Nothing
            Dim columns As New List(Of String)
            If reader.HasRows() Then
                instance = DirectCast(Activator.CreateInstance(GetType(TActiveRecord)), TActiveRecord)
                While reader.Read()
                    columns = If(columns.Count = 0, Core.Reader._getReaderRowColumns(reader), columns)
                    Core.Reader._readerRowToInstance(reader, columns, TryCast(instance, Entity))
                End While
            End If
            If closeReaderAfterSetUp Then
                reader.Close()
            End If
            Return instance
        End Function
        ''' <summary>
        ''' Create new Dictionary with instances by generic type and set up all called reader columns into new instances properties or fields.
        ''' If reader has no rows, empty list is returned.
        ''' </summary>
        ''' <typeparam name="TActiveRecord">Result list item generic type.</typeparam>
        ''' <param name="reader">Reader with values for new instance properties and fields</param>
        ''' <param name="closeReaderAfterSetUp">Automaticly close reader after all.</param>
        ''' <returns></returns>
        Public Shared Function ToList(Of TActiveRecord)(reader As DbDataReader, Optional closeReaderAfterSetUp As Boolean = True) As List(Of TActiveRecord)
            Dim result As New List(Of TActiveRecord)
            Dim columns As New List(Of String)
            Dim instance As TActiveRecord
            Dim instanceType As Type = GetType(TActiveRecord)
            Dim primitiveType As Boolean = instanceType.IsPrimitive Or instanceType.FullName = "System.String"
            If reader.HasRows() Then
                While reader.Read()
                    If primitiveType Then
                        instance = DirectCast(reader(0), TActiveRecord)
                    Else
                        columns = If(columns.Count = 0, Core.Reader._getReaderRowColumns(reader), columns)
                        instance = DirectCast(Activator.CreateInstance(instanceType), TActiveRecord)
                        Core.Reader._readerRowToInstance(reader, columns, TryCast(instance, Entity))
                    End If
                    result.Add(instance)
                End While
            End If
            If closeReaderAfterSetUp Then
                reader.Close()
            End If
            Return result
        End Function
        ''' <summary>
        ''' Create new Dictionary with keys by first generic type and instances (values) by second generic type 
        ''' and set up all called reader columns into new instances properties or fields. By first param as string,
        ''' specify which column from reader to use to complete dictionary keys.
        ''' If reader has no rows, empty dictionary is returned.
        ''' </summary>
        ''' <typeparam name="TKey">Result dictionary generic type to complete dictionary keys.</typeparam>
        ''' <typeparam name="TActiveRecord">Result dictionary generic type to complete dictionary values.</typeparam>
        ''' <param name="reader">Reader with values for new instance properties and fields</param>
        ''' <param name="keyColumnName">Reader column name to use to complete result dictionary keys.</param>
        ''' <param name="throwExceptionInDuplicateKey">True to thrown Exception if any previous key will be founded by filling the result, false to overwrite any previous value.</param>
        ''' <param name="closeReaderAfterSetUp">Automaticly close reader after all.</param>
        ''' <returns>Dictionary with keys completed by second param for reader column name, values completed by reader columns with the same names as TActiveRecord type fields/properties.</returns>
        Public Shared Function ToDictionary(Of TKey, TActiveRecord)(reader As DbDataReader, Optional keyColumnName As String = "", Optional throwExceptionInDuplicateKey As Boolean = True, ByVal Optional closeReaderAfterSetUp As Boolean = True) As Dictionary(Of TKey, TActiveRecord)
            Dim result As New Dictionary(Of TKey, TActiveRecord)
            Dim columns As New List(Of String)
            Dim key As TKey
            Dim instance As TActiveRecord
            Dim instanceType As Type = GetType(TActiveRecord)
            Dim primitiveType As Boolean = instanceType.IsPrimitive Or instanceType.FullName = "System.String"
            If reader.HasRows() Then
                If String.IsNullOrEmpty(keyColumnName) Then keyColumnName = Resource.UniqueColumn(GetType(TActiveRecord))
                While reader.Read()
                    If primitiveType Then
                        instance = DirectCast(reader(0), TActiveRecord)
                    Else
                        columns = If(columns.Count = 0, Core.Reader._getReaderRowColumns(reader), columns)
                        instance = DirectCast(Activator.CreateInstance(GetType(TActiveRecord)), TActiveRecord)
                        Core.Reader._readerRowToInstance(reader, columns, TryCast(instance, Entity))
                    End If
                    key = DirectCast(reader.Item(keyColumnName), TKey)
                    If result.ContainsKey(key) Then
                        If (throwExceptionInDuplicateKey) Then
                            Throw New Exception(String.Format(
                                "[Model] Duplicate key: '{0}' for result Dictionary<{1}, {2}>.",
                                key,
                                GetType(TKey).ToString(),
                                GetType(TActiveRecord).ToString()
                            ))
                        Else
                            result(key) = instance
                        End If
                    Else
                        result.Add(key, instance)
                    End If
                End While
            End If
            If closeReaderAfterSetUp Then
                reader.Close()
            End If
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
        ''' <param name="throwExceptionInDuplicateKey">True to thrown Exception if any previous key will be founded by filling the result, false to overwrite any previous value.</param>
        ''' <param name="closeReaderAfterSetUp">Automaticly close reader after all.</param>
        ''' <returns>Dictionary with keys completed by second anonymous function, values completed by reader columns with the same names as TActiveRecord type fields/properties.</returns>
        Public Shared Function ToDictionary(Of TKey, TActiveRecord)(reader As DbDataReader, keySelector As Func(Of TActiveRecord, TKey), Optional throwExceptionInDuplicateKey As Boolean = True, Optional closeReaderAfterSetUp As Boolean = True) As Dictionary(Of TKey, TActiveRecord)
            Dim result As New Dictionary(Of TKey, TActiveRecord)
            Dim columns As New List(Of String)
            Dim key As TKey
            Dim instance As TActiveRecord
            Dim instanceType As Type = GetType(TActiveRecord)
            Dim primitiveType As Boolean = instanceType.IsPrimitive Or instanceType.FullName = "System.String"
            If reader.HasRows() Then
                While reader.Read()
                    If primitiveType Then
                        instance = DirectCast(reader(0), TActiveRecord)
                    Else
                        columns = If(columns.Count = 0, Core.Reader._getReaderRowColumns(reader), columns)
                        instance = DirectCast(Activator.CreateInstance(GetType(TActiveRecord)), TActiveRecord)
                        Core.Reader._readerRowToInstance(reader, columns, TryCast(instance, Entity))
                    End If
                    key = keySelector(instance)
                    If result.ContainsKey(key) Then
                        If (throwExceptionInDuplicateKey) Then
                            Throw New Exception(String.Format(
                                "[Model] Duplicate key: '{0}' for result Dictionary<{1}, {2}>.",
                                key,
                                GetType(TKey).ToString(),
                                GetType(TActiveRecord).ToString()
                            ))
                        Else
                            result(key) = instance
                        End If
                    Else
                        result.Add(key, instance)
                    End If
                End While
            End If
            If closeReaderAfterSetUp Then
                reader.Close()
            End If
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
        ''' <param name="columns">Columns in reader in proper order.</param>
        ''' <param name="instance">Instance to fill.</param>
        Private Shared Sub _readerRowToInstance(reader As DbDataReader, columns As List(Of String), ByRef instance As Entity)
            instance.InitialSetUp = True
            Dim str As String
            For Each str In columns
                instance.[set](str, reader(str))
            Next
            instance.InitialSetUp = False
        End Sub
    End Class
End Namespace