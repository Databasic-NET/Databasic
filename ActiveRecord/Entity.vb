Imports System.Dynamic
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Databasic

Namespace ActiveRecord

    ''' <summary>
    ''' Active record base class for database models.
    ''' All properties and fields in class extended from this class should be 
    ''' named in the same case sensitive way as columns are named in database.
    ''' Choose fields and properties types to fit into database types.
    ''' </summary>
    <DefaultMember("Item")>
    Public MustInherit Class Entity
        Inherits DynamicObject

        Private Structure TouchedInfo
            Public Value As Object
            Public Type As Type
        End Structure



        ''' <summary>
        ''' 
        ''' </summary>
        <CompilerGenerated>
        Protected connectionIndex As Int16 = Database.DEFAUT_CONNECTION_INDEX
        ''' <summary>
        ''' 
        ''' </summary>
        <CompilerGenerated>
        Protected connectionName As String = Database.DEFAUT_CONNECTION_NAME
        ''' <summary>
        ''' ActiveRecord class database table name(s), first table is primary table.
        ''' </summary>
        <CompilerGenerated>
        Protected tableNames As String() = New String() {}
        ''' <summary>
        ''' ActiveRecord class primary database table unique column name, usually "Id".
        ''' </summary>
        <CompilerGenerated>
        Protected uniqueColumnName As String = "Id"
        ''' <summary>
        ''' 
        ''' </summary>
        <CompilerGenerated>
        Protected Shared resource As Resource = New Resource()




        ''' <summary>
        ''' Active record model Id
        ''' </summary>
        <CompilerGenerated>
        Public Property Id As Object
        ''' <summary>
        ''' Internal switch for setuping values by methods .ToXxxxx() to not fill touched dictionary.
        ''' </summary>
        <CompilerGenerated>
        Friend InitialSetUp As Boolean = False
        ''' <summary>
        ''' Properties with values and fields with values touched by indexer.
        ''' </summary>
        <CompilerGenerated>
        Private _initialData As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
        ''' <summary>
        ''' Reserve store to store anything what has been not specified by any property or field.
        ''' </summary>
        <CompilerGenerated>
        Private _reserveStore As Dictionary(Of String, Object) = New Dictionary(Of String, Object)





        ''' <summary>
        ''' Empty constructor to create ActiveRecord instances by Activator.CreateInstance(typeof(TActiveRecord))
        ''' </summary>
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
        Public Function [get](ByVal key As String) As Object
            Dim type As Type = Me.GetType()
            Dim runtimeProperty As PropertyInfo = (
            From prop In type.GetProperties(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
            Where prop.Name = key
            Select prop
        ).FirstOrDefault()
            If TypeOf runtimeProperty Is PropertyInfo AndAlso runtimeProperty.CanRead Then
                Return runtimeProperty.GetValue(Me, Nothing)
            Else
                Dim runtimeField As FieldInfo = (
                From field In type.GetFields(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
                Where field.Name = key
                Select field
            ).FirstOrDefault()
                If TypeOf runtimeField Is FieldInfo Then
                    Return runtimeField.GetValue(Me)
                ElseIf Me._reserveStore.ContainsKey(key) Then
                    Return Me._reserveStore(key)
                End If
            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' Set any property, field or reserve store value.
        ''' </summary>
        ''' <param name="key">Property, field or reserve store key.</param>
        ''' <param name="value">Property, field or reserve store value.</param>
        Public Sub [set](ByVal key As String, ByVal value As Object)
            Dim type As Type = Me.GetType()
            Dim runtimeProperty As PropertyInfo = (
                From prop In type.GetProperties(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
                Where prop.Name = key
                Select prop
            ).FirstOrDefault()
            If TypeOf runtimeProperty Is PropertyInfo AndAlso runtimeProperty.CanWrite Then
                Me.setProperty(key, value, runtimeProperty)
            Else
                Dim runtimeField As FieldInfo = (
                    From field In type.GetFields(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
                    Where field.Name = key
                    Select field
                ).FirstOrDefault()
                If TypeOf runtimeField Is FieldInfo Then
                    Me.setField(key, value, runtimeField)
                Else
                    Me._reserveStore.Item(key) = value
                End If
            End If
            If Me.InitialSetUp Then
                If Me._initialData.ContainsKey(key) Then
                    Me._initialData(key) = value
                Else
                    Me._initialData.Add(key, value)
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
            Me._reserveStore.TryGetValue(binder.Name, result)
            Return True
        End Function
        ''' <summary>
        ''' Internal overloading method for DynamicObject.
        ''' </summary>
        ''' <param name="binder"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Overrides Function TrySetMember(ByVal binder As SetMemberBinder, ByVal value As Object) As Boolean
            If Me._reserveStore.ContainsKey(binder.Name) Then
                Me._reserveStore(binder.Name) = value
            Else
                Me._reserveStore.Add(binder.Name, value)
            End If
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
        ''' Set up all data from dictionary into instance properties or fields without touching.
        ''' </summary>
        ''' <param name="data">Dictionary with any values, named as instance fields and properties.</param>
        ''' <param name="asInitialData">True to fill data into initial Dictionary to compare them later by GetTouched() function.</param>
        Public Sub SetUp(data As Dictionary(Of String, Object), Optional asInitialData As Boolean = False)
            If asInitialData Then
                Me.InitialSetUp = True
            End If
            Dim pair As KeyValuePair(Of String, Object)
            For Each pair In data
                Me.[set](pair.Key, pair.Value)
            Next
            If asInitialData Then
                Me.InitialSetUp = False
            End If
        End Sub












        ''' <summary>
        ''' Get touched properties and fields in Dictionary. Everything what is different 
        ''' to Me._initialData filled in instance initial set up.
        ''' </summary>
        ''' <returns>Dictionary with values, which are different from initial set up.</returns>
        Public Function GetTouched() As Dictionary(Of String, Object)
            Dim touched As New Dictionary(Of String, Object)
            Dim initialValue As Object
            Dim currentValue As Object
            Dim initialType As Type
            Dim currentType As Type
            Dim instanceType As Type = Me.GetType()
            Dim indexerPropertyName = Entity._getIndexerPropertyName(instanceType)
            Dim protectedElements As New List(Of String) From {
                "InitialSetUp", "_initialData", "_reserveStore", "tableNames", "connectionIndex", "connectionName", "uniqueColumnName"
            }
            Dim definedAndCurrent As New Dictionary(Of String, TouchedInfo)
            For Each prop As PropertyInfo In instanceType.GetProperties(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
                If (
                    Not definedAndCurrent.ContainsKey(prop.Name) And
                    prop.Name <> indexerPropertyName And
                    Not protectedElements.Contains(prop.Name)
                ) Then
                    definedAndCurrent.Add(prop.Name, New TouchedInfo() With {
                        .Value = prop.GetValue(Me, Nothing),
                        .Type = prop.PropertyType
                    })
                End If
            Next
            For Each field As FieldInfo In instanceType.GetFields(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
                If (
                    Not definedAndCurrent.ContainsKey(field.Name) And
                    Not protectedElements.Contains(field.Name) And
                    Not Entity._isCompilerGenerated(field)
                ) Then
                    definedAndCurrent.Add(field.Name, New TouchedInfo() With {
                        .Value = field.GetValue(Me),
                        .Type = field.FieldType
                    })
                End If
            Next
            For Each current As KeyValuePair(Of String, TouchedInfo) In definedAndCurrent
                initialValue = Nothing
                currentValue = current.Value.Value
                If Me._initialData.ContainsKey(current.Key) Then
                    initialValue = Me._initialData(current.Key)
                End If
                initialType = If(initialValue IsNot Nothing, initialValue.GetType(), Nothing)
                currentType = If(current.Value.Value IsNot Nothing, current.Value.Type, Nothing)
                If (
                    initialType IsNot Nothing AndAlso
                    currentType IsNot Nothing AndAlso
                    Not initialType.Equals(currentType)
                ) Then
                    initialType = If(
                        Nullable.GetUnderlyingType(currentType) <> Nothing,
                        Nullable.GetUnderlyingType(currentType),
                        currentType
                    )
                    initialValue = Convert.ChangeType(initialValue, initialType)
                End If
                If (
                    (initialValue Is Nothing And currentValue IsNot Nothing) OrElse
                    (currentValue Is Nothing And initialValue IsNot Nothing) OrElse
                    (
                        initialValue IsNot Nothing AndAlso currentValue IsNot Nothing AndAlso
                        Not currentValue.Equals(initialValue)
                    )
                ) Then
                    touched.Add(current.Key, currentValue)
                End If
            Next
            Return touched
        End Function
        Private Shared Function _getIndexerPropertyName(Type As Type) As String
            Dim defaultAttr As Attribute = Attribute.GetCustomAttribute(Type, GetType(DefaultMemberAttribute))
            Return If(TypeOf defaultAttr Is DefaultMemberAttribute, DirectCast(defaultAttr, DefaultMemberAttribute).MemberName, "")
        End Function

        Private Shared Function _isCompilerGenerated(fieldInfo As MemberInfo) As Boolean
            Dim compilerGeneratedAttr As Attribute = Attribute.GetCustomAttribute(fieldInfo, GetType(CompilerGeneratedAttribute))
            Return TypeOf compilerGeneratedAttr Is CompilerGeneratedAttribute
        End Function





        ''' <summary>
        ''' Insert/Update ActiveRecord instance by non-existing/existing instance Id property (Id database column).
        ''' </summary>
        ''' <param name="connectionIndex">Config connection index to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
        ''' <returns></returns>
        Public Function Save(Optional connectionIndex As Int16 = Database.DEFAUT_CONNECTION_INDEX) As Int32
            Return Me.Save(Connection.Get(connectionIndex))
        End Function
        ''' <summary>
        ''' Insert/Update ActiveRecord instance by non-existing/existing instance Id property (Id database column).
        ''' </summary>
        ''' <param name="connectionName">Config connection name to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
        ''' <returns></returns>
        Public Function Save(connectionName As String) As Int32
            Return Me.Save(Connection.Get(connectionName))
        End Function

        Public Function Save(connection As Connection) As Int32
            Dim resource As Provider.Resource = Activator.CreateInstance(connection.ResourceType)
            Return resource.Save(connection, Me)
        End Function








        ''' <summary>
        ''' Delete ActiveRecord instance by instance Id property (Id database column).
        ''' </summary>
        ''' <param name="connectionIndex">Config connection index to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
        ''' <returns></returns>
        Public Function Delete(Optional connectionIndex As Int16 = Database.DEFAUT_CONNECTION_INDEX) As Int32
            Return Me.Delete(Connection.Get(connectionIndex))
        End Function
        ''' <summary>
        ''' Delete ActiveRecord instance by instance Id property (Id database column)
        ''' </summary>
        ''' <param name="connectionName">Config connection name to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
        ''' <returns></returns>
        Public Function Delete(connectionName As String) As Int32
            Return Me.Delete(Connection.Get(connectionName))
        End Function
        Public Function Delete(connection As Connection) As Int32
            Dim resource As Provider.Resource = Activator.CreateInstance(connection.ResourceType)
            Return resource.Delete(connection, Me)
        End Function





        Public Shared Function Columns(activeRecordType As Type, tableIndex As Int16, Optional separator As String = ",") As String
            Return String.Join(separator, Resource.Columns(activeRecordType, tableIndex).ToArray())
        End Function
        Public Shared Function Columns(activeRecordType As Type, Optional separator As String = ",", Optional tableIndex As Int16 = 0) As String
            Return String.Join(separator, Resource.Columns(activeRecordType, tableIndex))
        End Function
        Public Shared Function Columns(Of TActiveRecord)(tableIndex As Int16, Optional separator As String = ",") As String
            Return String.Join(separator, Resource.Columns(GetType(TActiveRecord), tableIndex))
        End Function
        Public Shared Function Columns(Of TActiveRecord)(Optional separator As String = ",", Optional tableIndex As Int16 = 0) As String
            Return String.Join(separator, Resource.Columns(GetType(TActiveRecord), tableIndex))
        End Function
        Public Function Columns(tableIndex As Int16, Optional separator As String = ",") As String
            Return String.Join(separator, Resource.Columns(Me.GetType(), tableIndex))
        End Function
        Public Function Columns(Optional separator As String = ",", Optional tableIndex As Int16 = 0) As String
            Return String.Join(separator, Resource.Columns(Me.GetType(), tableIndex))
        End Function



        ''' <summary>
        ''' Get declared table name by 'activeRecordType' and by optional called index argument.
        ''' </summary>
        ''' <param name="activeRecordType">Class type, inherited from ActiveRecord class with declared protected static field 'tables' as array of strings.</param>
        ''' <param name="tableIndex">Array index to get proper table name string from declared protected static field 'tables' as array of strings.</param>
        ''' <returns>Declared database table name from active record class.</returns>
        Public Shared Function TableName(activeRecordType As Type, Optional tableIndex As Int16 = 0) As String
            Return Resource.Table(activeRecordType, tableIndex)
        End Function
        ''' <summary>
        ''' Get declared table name from generic type 'TActiveRecord' and by optional called index argument.
        ''' </summary>
        ''' <typeparam name="TActiveRecord">Class name, inherited from ActiveRecord class with declared protected static field 'tables' as array of strings.</typeparam>
        ''' <param name="tableIndex">Array index to get proper table name string from declared protected static field 'tables' as array of strings.</param>
        ''' <returns>Declared database table name from active record class.</returns>
        Public Shared Function TableName(Of TActiveRecord)(Optional tableIndex As Int16 = 0) As String
            Return Resource.Table(GetType(TActiveRecord), tableIndex)
        End Function
        Public Function TableName(Optional tableIndex As Int16 = 0) As String
            Return Resource.Table(Me.GetType(), tableIndex)
        End Function





        ''' <summary>
        ''' Get declared identifier table column name by 'activeRecordType' argument.
        ''' </summary>
        ''' <param name="activeRecordType">Class type, inherited from ActiveRecord class with declared protected static field 'idColumn' as string.</param>
        ''' <returns>Declared database table id column name from active record class.</returns>
        Public Shared Function UniqueColumn(activeRecordType As Type) As String
            Return Resource.UniqueColumn(activeRecordType)
        End Function
        ''' <summary>
        ''' Get declared identifier table column name from generic type 'TActiveRecord'.
        ''' </summary>
        ''' <typeparam name="TActiveRecord">Class name, inherited from ActiveRecord class with declared protected static field 'idColumn' as string.</typeparam>
        ''' <returns>Declared database table id column name from active record class.</returns>
        Public Shared Function UniqueColumn(Of TActiveRecord)() As String
            Return Resource.UniqueColumn(GetType(TActiveRecord))
        End Function
        Public Function UniqueColumn() As String
            Return Resource.UniqueColumn(Me.GetType())
        End Function
    End Class
End Namespace