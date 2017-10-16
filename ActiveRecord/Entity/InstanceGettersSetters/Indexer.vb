Imports System.Dynamic
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization

Namespace ActiveRecord
	''' <summary>
	''' Active record base class for database models.
	''' All properties and fields in class extended from this class should be 
	''' named in the same case sensitive way as columns are named in database.
	''' Choose fields and properties types to fit into database types.
	''' </summary>
	<DefaultMember("Item"), DataContract, Serializable>
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

	End Class
End Namespace