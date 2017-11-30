Imports System.Data.Common
Imports System.Dynamic
Imports Databasic.ActiveRecord

Namespace ActiveRecord

	Partial Public MustInherit Class Entity

		Friend Shared Function ToInstance(
			instanceType As Type,
			data As Dictionary(Of String, Object)
		) As Entity
			Dim instance As Object = Activator.CreateInstance(instanceType)
			TryCast(instance, Entity).SetUp(data, True)
			Return instance
		End Function

		Friend Shared Function ToInstance(
			reader As DbDataReader,
			instanceType As Type,
			Optional ByRef columnsByDbNames As Dictionary(
				Of String, Databasic.MemberInfo
			) = Nothing
		) As Object
			Dim result As Object = Nothing
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columns As New List(Of String)
			If reader.HasRows() Then
				reader.Read()
				If Tools.IsDescriptableType(instanceType) Then
					If (
						Not TypeOf columnsByDbNames Is Dictionary(Of String, Databasic.MemberInfo)
					) Then
						columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(instanceType)
					End If
					result = Activator.CreateInstance(instanceType)
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

		Friend Shared Function ToInstance(
			reader As DbDataReader,
			instanceCompleter As InstanceCompleter
		) As Object
			Dim result As Object = Nothing
			If Not TypeOf reader Is DbDataReader Then Return result
			If reader.HasRows() Then
				reader.Read()
				result = instanceCompleter.Invoke(reader)
			End If
			reader.Close()
			Return result
		End Function

		Friend Shared Function ToInstance(
			reader As DbDataReader,
			instanceCompleter As InstanceCompleterWithColumns
		) As Object
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

		Friend Shared Function ToInstance(
			reader As DbDataReader,
			instanceCompleter As InstanceCompleterWithAllInfo,
			instanceType As Type,
			Optional propertiesOnly As Boolean = True
		) As Object
			Dim result As Object = Nothing
			If Not TypeOf reader Is DbDataReader Then Return result
			Dim columnsByDbNames As New Dictionary(Of String, Databasic.MemberInfo)
			If reader.HasRows() Then
				reader.Read()
				If Tools.IsDescriptableType(instanceType) Then
					columnsByDbNames = MetaDescriptor.GetColumnsByDbNames(instanceType)
					If (propertiesOnly) Then
						columnsByDbNames = (
							From item In columnsByDbNames
							Where item.Value.MemberInfoType = MemberInfoType.Prop
							Select item
						).ToDictionary(Of String, Databasic.MemberInfo)(
							Function(item) item.Key,
							Function(item) item.Value
						)
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