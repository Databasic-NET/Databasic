Imports System.Data.Common
Imports System.Dynamic
Imports System.Reflection

''' <summary>
''' Active record base class for database models.
''' All properties and fields in class extended from this class should be 
''' named in the same case sensitive way as columns are named in database.
''' Choose fields and properties types to fit into database types.
''' </summary>
<DefaultMember("Item")>
Public MustInherit Class Model
    Inherits DynamicObject







    ''' <summary>
    ''' Properties with values and fields with values touched by indexer.
    ''' </summary>
    Protected touched As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
    ''' <summary>
    ''' Reserve store to store anything what has been not specified by any property or field.
    ''' </summary>
    Private _store As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
    ''' <summary>
    ''' Internal switch for setuping values by methods .ToXxxxx() to not fill touched dictionary.
    ''' </summary>
    Private _internalChange As Boolean = False
    Public Sub New()
    End Sub




    ''' <summary>
    ''' Default member to get any model value by indexer.
    ''' </summary>
    ''' <param name="key"></param>
    ''' <returns></returns>
    Default Public Property Item(key As String) As Object
        Get
            Return Me.[get](key)
        End Get
        Set(ByVal value As Object)
            Me.[set](key, value)
        End Set
    End Property
    ''' <summary>
    ''' Get any property, field or reserve store record.
    ''' </summary>
    ''' <param name="key">Property, field or reserve store value name.</param>
    ''' <returns></returns>
    Protected Function [get](ByVal key As String) As Object
        Dim result As Object = Nothing
        Dim type As Type = MyBase.GetType()
        Dim runtimeProperty As PropertyInfo = type.GetProperty(key)
        Dim runtimeField As FieldInfo = type.GetField(key)
        If ((runtimeProperty Is Nothing) OrElse (runtimeField Is Nothing)) Then
            If (runtimeProperty Is Nothing) Then
                Throw New Exception(String.Format("Property '{0}' is not defined in '{1}'. Define property as: public type PropertyName &#123; get; set; &#125;", key.ToString, type.ToString))
            End If
            If (runtimeField Is Nothing) Then
                Throw New Exception(String.Format("Field '{0}' is not defined in '{1}'. Define field as: public type FieldName;", key.ToString, type.ToString))
            End If
        End If
        If ((Not runtimeProperty Is Nothing) AndAlso runtimeProperty.CanRead) Then
            Return runtimeProperty.GetValue(Me, Nothing)
        End If
        If (Not runtimeField Is Nothing) Then
            Return runtimeField.GetValue(Me)
        End If
        If Not Me._store.TryGetValue(key, result) Then
            result = Nothing
        End If
        Return result
    End Function
    ''' <summary>
    ''' Set any property, field or reserve store value.
    ''' </summary>
    ''' <param name="key">Property, field or reserve store key.</param>
    ''' <param name="value">Property, field or reserve store value.</param>
    Protected Sub [set](ByVal key As String, ByVal value As Object)
        Dim type As Type = MyBase.GetType()
        Dim runtimeProperty As PropertyInfo = type.GetProperty(key)
        Dim runtimeField As FieldInfo = type.GetField(key)
        If ((Not runtimeProperty Is Nothing) AndAlso runtimeProperty.CanWrite) Then
            Me.setProperty(key, value, runtimeProperty)
        ElseIf (Not runtimeField Is Nothing) Then
            Me.setField(key, value, runtimeField)
        Else
            Me._store.Item(key) = value
        End If
        If Not Me._internalChange Then
            If Me.touched.ContainsKey(key) Then
                Me.touched.Item(key) = value
            Else
                Me.touched.Add(key, value)
            End If
        End If
    End Sub
    ''' <summary>
    ''' Internal overloading method for DynamicObject.
    ''' </summary>
    ''' <param name="binder"></param>
    ''' <param name="result"></param>
    ''' <returns></returns>
    Public Overrides Function TryGetMember(ByVal binder As GetMemberBinder, ByRef result As Object) As Boolean
        result = Me._store.Item(binder.Name)
        Return True
    End Function
    ''' <summary>
    ''' Internal overloading method for DynamicObject.
    ''' </summary>
    ''' <param name="binder"></param>
    ''' <param name="value"></param>
    ''' <returns></returns>
    Public Overrides Function TrySetMember(ByVal binder As SetMemberBinder, ByVal value As Object) As Boolean
        Me._store.Item(binder.Name) = value
        Return True
    End Function
    ''' <summary>
    ''' Set field setter method - called from indexer set block.
    ''' </summary>
    ''' <param name="key"></param>
    ''' <param name="rawValue"></param>
    ''' <param name="fieldInfo"></param>
    Protected Sub setField(key As String, rawValue As Object, fieldInfo As FieldInfo)
        If (fieldInfo.FieldType Is GetType(String)) Then
            rawValue = rawValue.ToString.Trim(New Char() {" "c, ChrW(13), ChrW(10), ChrW(9), ChrW(11)})
        End If
        Dim finalType As Type = If(
            Nullable.GetUnderlyingType(fieldInfo.FieldType) <> Nothing,
            Nullable.GetUnderlyingType(fieldInfo.FieldType),
            fieldInfo.FieldType
        )
        Dim finalValue As Object = If(
            TypeOf rawValue Is DBNull,
            Nothing,
            Convert.ChangeType(rawValue, finalType)
        )
        fieldInfo.SetValue(Me, finalValue)
    End Sub
    ''' <summary>
    ''' Set property setter method - called from indexer set block.
    ''' </summary>
    ''' <param name="key"></param>
    ''' <param name="rawValue"></param>
    ''' <param name="propertyInfo"></param>
    Protected Sub setProperty(key As String, rawValue As Object, propertyInfo As PropertyInfo)
        If (propertyInfo.PropertyType Is GetType(String)) Then
            rawValue = rawValue.ToString.Trim(New Char() {" "c, ChrW(13), ChrW(10), ChrW(9), ChrW(11)})
        End If
        Dim finalType As Type = If(
            Nullable.GetUnderlyingType(propertyInfo.PropertyType) <> Nothing,
            Nullable.GetUnderlyingType(propertyInfo.PropertyType),
            propertyInfo.PropertyType
        )
        Dim finalValue As Object = If(
            TypeOf rawValue Is DBNull,
            Nothing,
            Convert.ChangeType(rawValue, finalType)
        )
        propertyInfo.SetValue(Me, finalValue, Nothing)
    End Sub










    ''' <summary>
    ''' Create new instance by generic type and set up all called dictionary keys into new instance properties or fields.
    ''' </summary>
    ''' <typeparam name="ResultType">New instance type.</typeparam>
    ''' <param name="data">Data with values for new instance properties and fields.</param>
    ''' <returns>New instance by generic type with values by second param.</returns>
    Public Shared Function ToInstance(Of ResultType)(data As Dictionary(Of String, Object)) As ResultType
        Dim instance As ResultType = DirectCast(Activator.CreateInstance(GetType(ResultType)), ResultType)
        TryCast(instance, Model).SetUp(data)
        Return instance
    End Function
    ''' <summary>
    ''' Create new instance by generic type and set up all called reader columns with one row at minimal into new instance properties or fields.
    ''' If reader has no rows, Nothing is returned.
    ''' </summary>
    ''' <typeparam name="ResultType">New instance type.</typeparam>
    ''' <param name="reader">Reader with values for new instance properties and fields</param>
    ''' <param name="closeReaderAfterSetUp">Automaticly close reader after all.</param>
    ''' <returns></returns>
    Public Shared Function ToInstance(Of ResultType)(reader As DbDataReader, Optional closeReaderAfterSetUp As Boolean = True) As ResultType
        Dim instance As ResultType = Nothing
        Dim columns As New List(Of String)
        If reader.HasRows() Then
            instance = DirectCast(Activator.CreateInstance(GetType(ResultType)), ResultType)
            While reader.Read()
                columns = If(columns.Count = 0, Model._getReaderRowColumns(reader), columns)
                Model._readerRowToInstance(reader, columns, TryCast(instance, Model))
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
    ''' <typeparam name="ResultItemType">Result list item generic type.</typeparam>
    ''' <param name="reader">Reader with values for new instance properties and fields</param>
    ''' <param name="closeReaderAfterSetUp">Automaticly close reader after all.</param>
    ''' <returns></returns>
    Public Shared Function ToList(Of ResultItemType)(reader As DbDataReader, Optional closeReaderAfterSetUp As Boolean = True) As List(Of ResultItemType)
        Dim result As New List(Of ResultItemType)
        Dim columns As New List(Of String)
        Dim instance As ResultItemType
        If reader.HasRows() Then
            While reader.Read()
                columns = If(columns.Count = 0, Model._getReaderRowColumns(reader), columns)
                instance = DirectCast(Activator.CreateInstance(GetType(ResultItemType)), ResultItemType)
                Model._readerRowToInstance(reader, columns, TryCast(instance, Model))
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
    ''' <typeparam name="ResultItemKeyType">Result dictionary generic type to complete dictionary keys.</typeparam>
    ''' <typeparam name="ResultItemValueType">Result dictionary generic type to complete dictionary values.</typeparam>
    ''' <param name="reader">Reader with values for new instance properties and fields</param>
    ''' <param name="keyColumnName">Reader column name to use to complete result dictionary keys.</param>
    ''' <param name="throwExceptionInDuplicateKey">True to thrown Exception if any previous key will be founded by filling the result, false to overwrite any previous value.</param>
    ''' <param name="closeReaderAfterSetUp">Automaticly close reader after all.</param>
    ''' <returns></returns>
    Public Shared Function ToDictionary(Of ResultItemKeyType, ResultItemValueType)(ByVal reader As DbDataReader, ByVal Optional keyColumnName As String = "Id", ByVal Optional throwExceptionInDuplicateKey As Boolean = True, ByVal Optional closeReaderAfterSetUp As Boolean = True) As Dictionary(Of ResultItemKeyType, ResultItemValueType)
        Dim result As New Dictionary(Of ResultItemKeyType, ResultItemValueType)
        Dim columns As New List(Of String)
        Dim key As ResultItemKeyType
        Dim instance As ResultItemValueType
        If reader.HasRows() Then
            While reader.Read()
                columns = If(columns.Count = 0, Model._getReaderRowColumns(reader), columns)
                instance = DirectCast(Activator.CreateInstance(GetType(ResultItemValueType)), ResultItemValueType)
                Model._readerRowToInstance(reader, columns, TryCast(instance, Model))
                key = DirectCast(reader.Item(keyColumnName), ResultItemKeyType)
                If result.ContainsKey(key) Then
                    If (throwExceptionInDuplicateKey) Then
                        Throw New Exception(String.Format(
                            "[Model] Duplicate key: '{0}' for result Dictionary<{1}, {2}>.",
                            key,
                            GetType(ResultItemKeyType).ToString(),
                            GetType(ResultItemValueType).ToString()
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
    ''' Set up all data from dictionary into instance properties or fields without touching .
    ''' </summary>
    ''' <param name="data">Dictionary with any values, named as instance fields and properties.</param>
    ''' <param name="doNotTouch">True to not fill anything in touched dictionary.</param>
    Public Sub SetUp(data As Dictionary(Of String, Object), Optional doNotTouch As Boolean = True)
        If doNotTouch Then
            Me._internalChange = True
        End If
        Dim pair As KeyValuePair(Of String, Object)
        For Each pair In data
            Me.[set](pair.Key, pair.Value)
        Next
        If doNotTouch Then
            Me._internalChange = False
        End If
    End Sub
    ''' <summary>
    ''' Set up current reader row columns into instance properties and fields.
    ''' </summary>
    ''' <param name="reader">DbDataReader with current row moved, where current row will be used to fill instance properties and fields.</param>
    ''' <param name="columns">Columns in reader in proper order.</param>
    ''' <param name="instance">Instance to fill.</param>
    Private Shared Sub _readerRowToInstance(reader As DbDataReader, columns As List(Of String), ByRef instance As Model)
        instance._internalChange = True
        Dim str As String
        For Each str In columns
            instance.[set](str, reader.Item(str))
        Next
        instance._internalChange = False
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
End Class
