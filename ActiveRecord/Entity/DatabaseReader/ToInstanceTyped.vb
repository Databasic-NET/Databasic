Imports System.Data.Common
Imports System.Dynamic
Imports Databasic.ActiveRecord

Namespace ActiveRecord

	Partial Public MustInherit Class Entity

		''' <summary>
		''' Create new instance by generic type and set up all called dictionary keys into new instance properties or fields.
		''' </summary>
		''' <typeparam name="TValue">New instance type.</typeparam>
		''' <param name="data">Data with values for new instance properties and fields.</param>
		''' <returns>New instance by generic type with values by second param.</returns>
		Friend Shared Function ToInstance(Of TValue As Entity)(
			data As Dictionary(Of String, Object)
		) As TValue
			Dim instance As TValue = Activator.CreateInstance(Of TValue)()
			TryCast(instance, Entity).SetUp(data, True)
			Return instance
		End Function
		''' <summary>
		''' Create new instance by generic type and set up all called reader columns with one row at minimal into 
		''' new instance properties or fields. If TValue is primitive type, reader has to return single row and 
		''' single column select result and that result is converted and returned as to primitive value only.
		''' If reader has no rows, Nothing is returned.
		''' </summary>
		''' <typeparam name="TValue">New result class instance type or any primitive type for single row and single column select result.</typeparam>
		''' <param name="reader">Reader with values for new instance properties and fields.</param>
		''' <param name="columnsByDbNames">Optional class members meta info, indexed by database column names.</param>
		''' <returns>New instance as primitive type or as class instance, completed by reader columns with the same names as TActiveRecord type fields/properties.</returns>
		Friend Shared Function ToInstance(Of TValue)(
			reader As DbDataReader,
			Optional ByRef columnsByDbNames As Dictionary(
				Of String, Databasic.MemberInfo
			) = Nothing
		) As TValue
			Dim result As Object = Nothing
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String)
			Dim instanceType = GetType(TValue)
			If reader.HasRows() Then
				reader.Read()
				If Tools.IsDescriptableType(instanceType) Then
					If (
						Not TypeOf columnsByDbNames Is Dictionary(Of String, Databasic.MemberInfo)
					) Then
						columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(instanceType)
					End If
					result = Activator.CreateInstance(Of TValue)()
					columns = If(
						columns.Count = 0,
						ActiveRecord.Entity._getReaderRowColumns(reader),
						columns
					)
					ActiveRecord.Entity._readerRowToTypedInstance(
						reader, columns, columnsByDbNames, result,
						Databasic.Constants.EntityType.IsAssignableFrom(instanceType)
					)
				ElseIf Tools.IsPrimitiveType(instanceType) Then
					result = Convert.ChangeType(reader.Item(0), instanceType)
				Else
					result = New Databasic.Object()
					columns = If(
						columns.Count = 0,
						ActiveRecord.Entity._getReaderRowColumns(reader),
						columns
					)
					ActiveRecord.Entity._readerRowToAnonymousInstance(
						reader, columns, result
					)
				End If
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToInstance(Of TValue)(
			reader As DbDataReader,
			instanceCompleter As InstanceCompleter(Of TValue)
		) As TValue
			Dim result As Object = Nothing
			If Not TypeOf reader Is DbDataReader Then Return result
			If reader.HasRows() Then
				reader.Read()
				result = instanceCompleter.Invoke(reader)
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToInstance(Of TValue)(
			reader As DbDataReader,
			instanceCompleter As InstanceCompleterWithColumns(Of TValue)
		) As TValue
			Dim result As Object = Nothing
			If Not TypeOf reader Is DbDataReader Then Return result
			If reader.HasRows() Then
				reader.Read()
				result = instanceCompleter.Invoke(
					reader,
					ActiveRecord.Entity._getReaderRowColumns(reader)
				)
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToInstance(Of TValue)(
			reader As DbDataReader,
			instanceCompleter As InstanceCompleterWithAllInfo(Of TValue),
			Optional propertiesOnly As Boolean = True
		) As TValue
			Dim result As Object = Nothing
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim instanceType = GetType(TValue),
				columnsByDbNames As New Dictionary(Of String, Databasic.MemberInfo)
			If reader.HasRows() Then
				reader.Read()
				If Tools.IsDescriptableType(instanceType) Then
					If (propertiesOnly) Then
						columnsByDbNames = (
							From item In MetaDescriptor.GetColumnsByDbNames(instanceType)
							Where item.Value.MemberInfoType = MemberInfoType.Prop
							Select item
						).ToDictionary(Of String, Databasic.MemberInfo)(
							Function(item) item.Key,
							Function(item) item.Value
						)
					Else
						columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(instanceType)
					End If
				End If
				result = instanceCompleter.Invoke(
					reader,
					ActiveRecord.Entity._getReaderRowColumns(reader),
					columnsByDbNames
				)
			End If
			reader.Close()
			Return result
		End Function

	End Class
End Namespace