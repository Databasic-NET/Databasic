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
		''' Default member to get/set any value through indexer from/to current instance.
		''' </summary>
		''' <param name="memberName">Property, field or reserve store value name.</param>
		''' <returns></returns>
		Default Public Property Item(memberName As String, Optional asInitialData As Boolean = False) As Object
			Get
				Return Me.[Get](memberName, asInitialData)
			End Get
			Set(ByVal value As Object)
				Me.[Set](memberName, value, asInitialData)
			End Set
		End Property
		''' <summary>
		''' Get any property, field or reserve store record from current instance.
		''' </summary>
		''' <param name="memberName">Property, field or reserve store value name.</param>
		''' <param name="asInitialData">Get value from initial store.</param>
		''' <param name="memberInfo">Databasic class member info, optional.</param>
		''' <returns></returns>
		Public Function [Get](ByVal memberName As String, Optional asInitialData As Boolean = False, Optional ByRef memberInfo As Databasic.MemberInfo? = Nothing) As Object
			If asInitialData Then
				Return If(Me._initialData.ContainsKey(memberName), Me._initialData(memberName), Nothing)
			End If
			If Not memberInfo.HasValue Then memberInfo = MetaDescriptor.GetColumnByCodeName(Me.GetType(), memberName)
			If memberInfo.HasValue Then
				If memberInfo.Value.MemberInfoType = MemberInfoType.Prop Then
					Dim pi As Reflection.PropertyInfo = DirectCast(memberInfo.Value.MemberInfo, Reflection.PropertyInfo)
					If pi.CanRead Then Return pi.GetValue(Me, Nothing)
				Else
					Dim fi As Reflection.FieldInfo = DirectCast(memberInfo.Value.MemberInfo, Reflection.FieldInfo)
					Return fi.GetValue(Me)
				End If
			End If
			If Me._reserveStore.ContainsKey(memberName) Then Return Me._reserveStore(memberName)
			Return Nothing
		End Function
		''' <summary>
		''' Set any property, field or reserve store value into current instance.
		''' </summary>
		''' <param name="memberName">Property, field or reserve store key.</param>
		''' <param name="value">Property, field or reserve store value.</param>
		''' <param name="asInitialData">Set value into initial store.</param>
		''' <param name="memberInfo">Databasic class member info, optional.</param>
		Public Sub [Set](ByVal memberName As String, ByVal value As Object, Optional asInitialData As Boolean = False, Optional ByRef memberInfo As Databasic.MemberInfo? = Nothing)
			If Not memberInfo.HasValue Then memberInfo = MetaDescriptor.GetColumnByCodeName(Me.GetType(), memberName)
			Dim assigned As Boolean = False
			Dim currentValueType = If(value = Nothing, Nothing, value.GetType())
			Dim targetValue As Object = Nothing
			If memberInfo.HasValue Then
				If (memberInfo.Value.Type.Equals(GetType(String)) AndAlso memberInfo.Value.TrimChars.Length > 0) Then
					value = value.ToString().Trim(memberInfo.Value.TrimChars)
				End If
				If memberInfo.Value.MemberInfoType = MemberInfoType.Prop Then
					Dim pi As Reflection.PropertyInfo = DirectCast(memberInfo.Value.MemberInfo, Reflection.PropertyInfo)
					If pi.CanWrite Then
						If currentValueType <> Nothing AndAlso Not currentValueType.Equals(memberInfo.Value.Type) Then
							targetValue = Convert.ChangeType(value, memberInfo.Value.Type, memberInfo.Value.FormatProvider)
						Else
							targetValue = value
						End If
						pi.SetValue(Me, targetValue, Nothing)
						assigned = True
					End If
				Else
					Dim fi As Reflection.FieldInfo = DirectCast(memberInfo.Value.MemberInfo, Reflection.FieldInfo)
					If currentValueType <> Nothing AndAlso Not currentValueType.Equals(memberInfo.Value.Type) Then
						targetValue = Convert.ChangeType(value, memberInfo.Value.Type, memberInfo.Value.FormatProvider)
					Else
						targetValue = value
					End If
					fi.SetValue(Me, targetValue)
					assigned = True
				End If
			End If
			If Not assigned Then
				targetValue = If(targetValue, value)
				Me._reserveStore.Item(memberName) = value
			End If
			If asInitialData Then
				If Me._initialData.ContainsKey(targetValue) Then
					Me._initialData(memberName) = targetValue
				Else
					Me._initialData.Add(memberName, targetValue)
				End If
			End If
		End Sub
		''' <summary>
		''' DynamicObject overloading method implementing reading nondeclared values from Me._reserveStore.
		''' </summary>
		''' <param name="binder"></param>
		''' <param name="result"></param>
		''' <returns></returns>
		Public Overrides Function TryGetMember(ByVal binder As GetMemberBinder, ByRef result As Object) As Boolean
            Me._reserveStore.TryGetValue(binder.Name, result)
            Return True
        End Function
		''' <summary>
		''' DynamicObject overloading method implementing writing nondeclared values into Me._reserveStore.
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
		''' Get any properties, fields or reserve store record values from current instance.
		''' </summary>
		''' <param name="memberNames">Properties, fields or reserve store value names.</param>
		''' <param name="keysByCode">If false (by default), get fields/properties values in dictionary with keys by code names, if true, get touched fields in dictionary with keys by database names.</param>
		''' <param name="asInitialData">Get values from initial store.</param>
		''' <param name="classMembersInfo">Optional, Databasic meta descriptor info about class members, usualy metaDescription.ColumnsByCodeNames.</param>
		''' <returns></returns>
		Public Function GetValues(
			Optional memberNames As List(Of String) = Nothing,
			Optional keysByCode As Boolean = False,
			Optional asInitialData As Boolean = False,
			Optional ByRef classMembersInfo As Dictionary(Of String, Databasic.MemberInfo) = Nothing
		) As Dictionary(Of String, Object)
			Dim result As New Dictionary(Of String, Object)
			If asInitialData Then
				If Not TypeOf memberNames Is List(Of String) Then
					If Not TypeOf classMembersInfo Is Dictionary(Of String, Databasic.MemberInfo) Then
						classMembersInfo = MetaDescriptor.GetColumnsByCodeNames(Me.GetType())
					End If
					memberNames = classMembersInfo.Keys.ToList()
				End If
				For Each memberName As String In memberNames
					result.Add(
						memberName,
						If(Me._initialData.ContainsKey(memberName), Me._initialData(memberName), Nothing)
					)
				Next
			Else
				If Not TypeOf classMembersInfo Is Dictionary(Of String, Databasic.MemberInfo) Then
					classMembersInfo = MetaDescriptor.GetColumnsByCodeNames(Me.GetType())
				End If
				If Not TypeOf memberNames Is List(Of String) Then memberNames = classMembersInfo.Keys.ToList()
				Dim member As Databasic.MemberInfo
				Dim memberValue As Object
				Dim asserted As Boolean
				For Each memberName As String In memberNames
					memberValue = Nothing
					asserted = False
					If classMembersInfo.ContainsKey(memberName) Then
						member = classMembersInfo(memberName)
						If member.MemberInfoType = MemberInfoType.Prop Then
							Dim pi As Reflection.PropertyInfo = DirectCast(member.MemberInfo, Reflection.PropertyInfo)
							If pi.CanRead Then
								memberValue = pi.GetValue(Me, Nothing)
								asserted = True
							End If
						Else
							Dim fi As Reflection.FieldInfo = DirectCast(member.MemberInfo, Reflection.FieldInfo)
							memberValue = fi.GetValue(Me)
							asserted = True
						End If
					End If
					If Not asserted AndAlso Me._reserveStore.ContainsKey(memberName) Then
						memberValue = Me._reserveStore(memberName)
					End If
					result.Add(memberName, memberValue)
				Next
			End If
			Return result
		End Function





		''' <summary>
		''' Set up all data from dictionary into current instance properties or fields with or without touching (with touching by default).
		''' </summary>
		''' <param name="data">Dictionary with any values, named as instance fields and properties.</param>
		''' <param name="asInitialData">True to fill data into initial Dictionary to compare them later by GetTouched() function.</param>
		Public Sub SetUp(data As Dictionary(Of String, Object), Optional asInitialData As Boolean = False)
			For Each pair As KeyValuePair(Of String, Object) In data
				Me.[Set](pair.Key, pair.Value, asInitialData)
			Next
		End Sub





		''' <summary>
		''' Get touched properties and fields in Dictionary. Get everything, what is different 
		''' to Me._initialData dictionary, filled in instance initial set up.
		''' </summary>
		''' <param name="keysByCode">If false (by default), get touched fields in dictionary with keys by code names, if true, get touched fields in dictionary with keys by database names.</param>
		''' <param name="classMembersInfo">Databasic meta descriptor info about class members, optional.</param>
		''' <returns>Dictionary with values, which are different from initial set up.</returns>
		Public Function GetTouched(
			Optional keysByCode As Boolean = False,
			Optional ByRef classMembersInfo As Dictionary(Of String, Databasic.MemberInfo) = Nothing
		) As Dictionary(Of String, Object)
			Dim touched As New Dictionary(Of String, Object)
			Dim initialValue As Object
			Dim currentValue As Object
			Dim initialType As Type
			Dim currentType As Type
			Dim instanceType As Type = Me.GetType()
			If Not TypeOf classMembersInfo Is Dictionary(Of String, Databasic.MemberInfo) Then
				classMembersInfo = MetaDescriptor.GetColumnsByCodeNames(instanceType)
			End If
			For Each current As KeyValuePair(Of String, Databasic.MemberInfo) In classMembersInfo
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
					touched.Add(If(keysByCode, current.Key, current.Value.Name), currentValue)
				End If
			Next
			Return touched
		End Function
	End Class
End Namespace