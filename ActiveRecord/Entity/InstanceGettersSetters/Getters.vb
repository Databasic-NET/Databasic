Imports System.Dynamic
Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace ActiveRecord
	Partial Public MustInherit Class Entity
		Inherits DynamicObject

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
			Optional classMembersInfo As Dictionary(Of String, Databasic.MemberInfo) = Nothing
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
			Optional classMembersInfo As Dictionary(Of String, Databasic.MemberInfo) = Nothing
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