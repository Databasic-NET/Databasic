Partial Public MustInherit Class ProviderResource

	Public Overridable Function Update(
		ByRef instance As ActiveRecord.Entity,
		ByRef connection As Connection,
		ByRef classMetaDescription As MetaDescription
	) As Int32
        Return Me.updateExisting(instance, connection, classMetaDescription)
    End Function

	Public Overridable Function Update(
		ByRef instance As ActiveRecord.Entity,
		ByRef transaction As Transaction,
		ByRef classMetaDescription As MetaDescription
	) As Int32
        Return Me.updateExisting(instance, transaction, classMetaDescription)
    End Function

    Protected Overridable Function updateExisting(
        ByRef instance As ActiveRecord.Entity,
        ByRef transactionOrConnection As Object,
        ByRef classMetaDescription As MetaDescription
    ) As Int32
        Dim instanceType As Type = instance.GetType()
        Dim sql As String = $"UPDATE {ActiveRecord.Resource.Table(classMetaDescription)} SET "
        Dim params As New Dictionary(Of String, Object)
        Dim keyColumns As KeyColumns = ActiveRecord.Resource.KeyColumns(classMetaDescription)
        Dim touched As Dictionary(Of String, Object) = instance.GetTouched(
            False, classMetaDescription.ColumnsByCodeNames
        ) ' keys by db names
        Dim keyValues As Dictionary(Of String, Object) = instance.GetValues(
            keyColumns.Columns.Keys.ToList(), False, False, classMetaDescription.ColumnsByCodeNames
        ) ' keys by db names
        Dim paramsCounter As Int16 = 0
        Dim aiColumnName As String = If(
            classMetaDescription.AutoIncrementColumn.HasValue,
            classMetaDescription.AutoIncrementColumn.Value.DatabaseColumnName, ""
        )
        Dim separator As String = ""
        For Each item As KeyValuePair(Of String, MemberInfo) In classMetaDescription.ColumnsByDatabaseNames
            If (item.Key = aiColumnName) Then Continue For
            If (item.Value.MemberInfoType = MemberInfoType.Field) Then Continue For
            If (Not touched.ContainsKey(item.Key)) Then Continue For
            sql += separator + item.Key + " = @param" + paramsCounter.ToString()
            params.Add("param" + paramsCounter.ToString(), touched(item.Key))
            paramsCounter += 1
            separator = ", "
        Next
        sql += " WHERE "
        separator = ""
        For Each columnItem As KeyValuePair(Of String, String) In keyColumns.Columns
            sql += separator + columnItem.Value + " = @param" + paramsCounter.ToString()
            params.Add("param" + paramsCounter.ToString(), keyValues(columnItem.Key))
            paramsCounter += 1
            separator = ", "
        Next
        Return Databasic.Statement.Prepare(sql, transactionOrConnection).Exec(params)
    End Function

End Class