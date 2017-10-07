Partial Public MustInherit Class ProviderResource

	Public Overridable Function Delete(ByRef instance As ActiveRecord.Entity, ByRef connection As Connection) As Int32
		Return Me._delete(instance, connection)
	End Function

	Public Overridable Function Delete(ByRef instance As ActiveRecord.Entity, ByRef transaction As Transaction) As Int32
		Return Me._delete(instance, transaction)
	End Function

	Private Function _delete(ByRef instance As ActiveRecord.Entity, ByRef transactionOrConnection As Object) As Int32
		Dim instanceType As Type = instance.GetType()
		Dim classMetaDescription As MetaDescription = MetaDescriptor.GetClassDescription(instanceType)
		Dim sql As String = $"DELETE FROM {ActiveRecord.Resource.Table(classMetaDescription)} WHERE "
		Dim keyColumns As KeyColumns = ActiveRecord.Resource.KeyColumns(classMetaDescription)
		Dim keyValues As Dictionary(Of String, Object) = instance.GetValues(
			keyColumns.Columns.Keys.ToList(), False, False, classMetaDescription.ColumnsByCodeNames
		) ' keys by db names
		Dim params As New Dictionary(Of String, Object)
		Dim separator As String = ""
		Dim paramsCounter As Int16 = 0
		For Each columnItem As KeyValuePair(Of String, String) In keyColumns.Columns
			sql += separator + columnItem.Value + " = @param" + paramsCounter.ToString()
			params.Add(
				"param" + paramsCounter.ToString(),
				keyValues(columnItem.Key)
			)
			paramsCounter += 1
			separator = ", "
		Next
		Return Databasic.Statement.Prepare(sql, transactionOrConnection).Exec(params)
	End Function

End Class