Imports System.Dynamic
Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace ActiveRecord
    ''' <summary>
    ''' Active record base class for database models.
    ''' All properties and fields in class extended from this class should be 
    ''' named in the same case sensitive way as columns are named in database.
    ''' Choose fields and properties types to fit into database types.
    ''' </summary>
    <DefaultMember("Item")>
    Partial Public MustInherit Class Entity
        Inherits DynamicObject





		'Public Property Resource As Resource





		''' <summary>
		''' Internal switch for setuping values by methods .ToXxxxx() to not fill touched dictionary.
		''' </summary>
		''' TODO: toto pujde doprdele, protože get set metody budou jinak!
		<CompilerGenerated>
        Private _initialSetUp As Boolean = False
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
		Friend Sub InitResource()
			Dim meType As Type = Me.GetType()
			Dim resourceMember As Reflection.MemberInfo = (
				From member As Reflection.MemberInfo In meType.GetMembers(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
				Where member.Name = "Resource" AndAlso (TypeOf member Is Reflection.PropertyInfo Or TypeOf member Is Reflection.FieldInfo)
				Select member
			).FirstOrDefault()
			If resourceMember = Nothing Then Return
			If TypeOf resourceMember Is Reflection.PropertyInfo Then
				Dim pi As Reflection.PropertyInfo = DirectCast(resourceMember, Reflection.PropertyInfo)
				If pi.GetValue(Me, Nothing) = Nothing AndAlso pi.CanWrite Then pi.SetValue(Me, Resource.GetInstance(pi.PropertyType), Nothing)
			ElseIf TypeOf resourceMember Is Reflection.FieldInfo Then
				Dim fi As Reflection.FieldInfo = DirectCast(resourceMember, Reflection.FieldInfo)
				If fi.GetValue(Me) = Nothing Then fi.SetValue(Me, Resource.GetInstance(fi.FieldType))
			End If
		End Sub





		''' <summary>
		''' Default member to get any model value by indexer.
		''' </summary>
		''' <param name="key"></param>
		''' <returns></returns>
		Default Public Property Item(key As String, Optional asInitialData As Boolean = False) As Object
			Get
				Return Me.[Get](key, asInitialData)
			End Get
			Set(ByVal value As Object)
				Me.[Set](key, value, asInitialData)
			End Set
		End Property
		''' <summary>
		''' Get any property, field or reserve store record.
		''' </summary>
		''' <param name="key">Property, field or reserve store value name.</param>
		''' <returns></returns>
		Public Function [Get](ByVal key As String, Optional asInitialData As Boolean = False) As Object
			If asInitialData Then
				Return If(Me._initialData.ContainsKey(key), Me._initialData(key), Nothing)
			End If
			Dim member As Databasic.MemberInfo? = MetaDescriptor.GetColumnByCodeName(Me.GetType(), key)
			If member.HasValue Then
				If member.Value.MemberInfoType = MemberInfoType.Prop Then
					Dim pi As Reflection.PropertyInfo = DirectCast(member.Value.MemberInfo, Reflection.PropertyInfo)
					If pi.CanRead Then Return pi.GetValue(Me, Nothing)
				Else
					Dim fi As Reflection.FieldInfo = DirectCast(member.Value.MemberInfo, Reflection.FieldInfo)
					Return fi.GetValue(Me)
				End If
			End If
			If Me._reserveStore.ContainsKey(key) Then Return Me._reserveStore(key)
			Return Nothing
		End Function
		''' <summary>
		''' Set any property, field or reserve store value.
		''' </summary>
		''' <param name="key">Property, field or reserve store key.</param>
		''' <param name="value">Property, field or reserve store value.</param>
		Public Sub [Set](ByVal key As String, ByVal value As Object, Optional asInitialData As Boolean = False)
			Dim member As Databasic.MemberInfo? = MetaDescriptor.GetColumnByCodeName(Me.GetType(), key)
			Dim assigned As Boolean = False
			Dim currentValueType = If(value = Nothing, Nothing, value.GetType())
			Dim targetValue As Object = Nothing
			If member.HasValue Then
				If (member.Value.Type.Equals(GetType(String)) AndAlso member.Value.TrimChars.Length > 0) Then
					value = value.ToString().Trim(member.Value.TrimChars)
				End If
				If member.Value.MemberInfoType = MemberInfoType.Prop Then
					Dim pi As Reflection.PropertyInfo = DirectCast(member.Value.MemberInfo, Reflection.PropertyInfo)
					If pi.CanWrite Then
						If currentValueType <> Nothing AndAlso Not currentValueType.Equals(member.Value.Type) Then
							targetValue = Convert.ChangeType(value, member.Value.Type, member.Value.FormatProvider)
						Else
							targetValue = value
						End If
						pi.SetValue(Me, targetValue, Nothing)
						assigned = True
					End If
				Else
					Dim fi As Reflection.FieldInfo = DirectCast(member.Value.MemberInfo, Reflection.FieldInfo)
					If currentValueType <> Nothing AndAlso Not currentValueType.Equals(member.Value.Type) Then
						targetValue = Convert.ChangeType(value, member.Value.Type, member.Value.FormatProvider)
					Else
						targetValue = value
					End If
					fi.SetValue(Me, targetValue)
					assigned = True
				End If
			End If
			If Not assigned Then
				targetValue = If(targetValue, value)
				Me._reserveStore.Item(key) = value
			End If
			If asInitialData Then
				If Me._initialData.ContainsKey(targetValue) Then
					Me._initialData(key) = targetValue
				Else
					Me._initialData.Add(key, targetValue)
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
		''' Set up all data from dictionary into instance properties or fields without touching.
		''' </summary>
		''' <param name="data">Dictionary with any values, named as instance fields and properties.</param>
		''' <param name="asInitialData">True to fill data into initial Dictionary to compare them later by GetTouched() function.</param>
		Public Sub SetUp(data As Dictionary(Of String, Object), Optional asInitialData As Boolean = False)
			For Each pair As KeyValuePair(Of String, Object) In data
				Me.[Set](pair.Key, pair.Value, asInitialData)
			Next
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
			Dim definedAndCurrent As Dictionary(Of String, Databasic.MemberInfo) = MetaDescriptor.GetColumnsByDbNames(instanceType)
			For Each current As KeyValuePair(Of String, Databasic.MemberInfo) In definedAndCurrent
				initialValue = Nothing
				If current.Value.MemberInfoType = MemberInfoType.Prop Then
					currentValue = DirectCast(current.Value.MemberInfo, Reflection.PropertyInfo).GetValue(Me, Nothing)
				Else
					currentValue = DirectCast(current.Value.MemberInfo, Reflection.FieldInfo).GetValue(Me)
				End If
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





        ''' <summary>
        ''' Insert/Update ActiveRecord instance by non-existing/existing instance Id property (Id database column).
        ''' </summary>
        ''' <param name="connectionIndex">Config connection index to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
        ''' <returns></returns>
        Public Function Save(Optional connectionIndex As Int32 = Database.DEFAUT_CONNECTION_INDEX) As Int32
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
		Public Function Save() As Int32
			Return Me.Save(Connection.Get(Tools.GetConnectionIndexByClassAttr(Me.GetType())))
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
		Public Function Delete(connectionIndex As Int32) As Int32
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
		Public Function Delete() As Int32
			Return Me.Delete(Connection.Get(Tools.GetConnectionIndexByClassAttr(Me.GetType())))
		End Function
		Public Function Delete(connection As Connection) As Int32
            Dim resource As Provider.Resource = Activator.CreateInstance(connection.ResourceType)
            Return resource.Delete(connection, Me)
        End Function





		Public Shared Function Columns(Of TActiveRecord)(tableIndex As Int32, Optional separator As String = ",") As String
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(
					Resource.Columns(GetType(TActiveRecord), tableIndex)
				)
			)
		End Function
        Public Shared Function Columns(Of TActiveRecord)(Optional separator As String = ",", Optional tableIndex As Int32 = 0) As String
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(
					Resource.Columns(GetType(TActiveRecord), tableIndex)
				)
			)
		End Function
		Public Shared Function Columns(tableIndex As Int32, Optional separator As String = ",") As String
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(
					Resource.Columns(Tools.GetEntryClassType(), tableIndex)
				)
			)
		End Function
		Public Shared Function Columns(Optional separator As String = ",", Optional tableIndex As Int32 = 0) As String
			Return String.Join(
				separator,
				Enumerable.ToArray(Of String)(
					Resource.Columns(Tools.GetEntryClassType(), tableIndex)
				)
			)
		End Function



		''' <summary>
		''' Get declared table name by 'activeRecordType' and by optional called index argument.
		''' </summary>
		''' <param name="activeRecordType">Class type, inherited from ActiveRecord class with declared protected static field 'tables' as array of strings.</param>
		''' <param name="tableIndex">Array index to get proper table name string from declared protected static field 'tables' as array of strings.</param>
		''' <returns>Declared database table name from active record class.</returns>
		Public Shared Function Table(activeRecordType As Type, Optional tableIndex As Int32 = 0) As String
			Return Resource.Table(activeRecordType, tableIndex)
		End Function
		''' <summary>
		''' Get declared table name from generic type 'TActiveRecord' and by optional called index argument.
		''' </summary>
		''' <typeparam name="TActiveRecord">Class name, inherited from ActiveRecord class with declared protected static field 'tables' as array of strings.</typeparam>
		''' <param name="tableIndex">Array index to get proper table name string from declared protected static field 'tables' as array of strings.</param>
		''' <returns>Declared database table name from active record class.</returns>
		Public Shared Function Table(Of TActiveRecord)(Optional tableIndex As Int32 = 0) As String
			Return Resource.Table(GetType(TActiveRecord), tableIndex)
		End Function
		Public Shared Function Table(Optional tableIndex As Int32 = 0) As String
			Return Resource.Table(Tools.GetEntryClassType(), tableIndex)
		End Function





		'''' <summary>
		'''' Get declared identifier table column name by 'activeRecordType' argument.
		'''' </summary>
		'''' <param name="activeRecordType">Class type, inherited from ActiveRecord class with declared protected static field 'idColumn' as string.</param>
		'''' <returns>Declared database table id column name from active record class.</returns>
		'Public Shared Function UniqueColumn(activeRecordType As Type) As String
		'    Return Resource.UniqueColumn(activeRecordType)
		'End Function
		'''' <summary>
		'''' Get declared identifier table column name from generic type 'TActiveRecord'.
		'''' </summary>
		'''' <typeparam name="TActiveRecord">Class name, inherited from ActiveRecord class with declared protected static field 'idColumn' as string.</typeparam>
		'''' <returns>Declared database table id column name from active record class.</returns>
		'Public Shared Function UniqueColumn(Of TActiveRecord)() As String
		'    Return Resource.UniqueColumn(GetType(TActiveRecord))
		'End Function
		'Public Function UniqueColumn() As String
		'    Return Resource.UniqueColumn(Me.GetType())
		'End Function
	End Class
End Namespace