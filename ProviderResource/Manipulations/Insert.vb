Partial Public MustInherit Class ProviderResource

    Public Overridable Function Insert(
        ByRef instance As ActiveRecord.Entity,
        ByRef connection As Connection,
        ByRef classMetaDescription As MetaDescription
    ) As Int32
        Return Me.insertNew(instance, connection, classMetaDescription)
    End Function

    Public Overridable Function Insert(
        ByRef instance As ActiveRecord.Entity,
        ByRef transaction As Transaction,
        ByRef classMetaDescription As MetaDescription
    ) As Int32
        Return Me.insertNew(instance, transaction, classMetaDescription)
    End Function

    Protected Overridable Function insertNew(
        ByRef instance As ActiveRecord.Entity,
        ByRef transactionOrConnection As Object,
        ByRef classMetaDescription As MetaDescription
    ) As Int32
        Dim columnsSql As String = "",
            paramsSql As String = "",
            params As New Dictionary(Of String, Object),
            KeyColumns As KeyColumns = ActiveRecord.Resource.KeyColumns(classMetaDescription),
            touched As Dictionary(Of String, Object) = instance.GetTouched(
                False, classMetaDescription.ColumnsByCodeNames
            ), ' keys by db names
            separator As String = "",
            paramsCounter As Int16 = 0,
            aiColumnName As String = If(
                classMetaDescription.AutoIncrementColumn.HasValue,
                classMetaDescription.AutoIncrementColumn.Value.DatabaseColumnName,
                ""
            ),
            touchedItemValue As Object
        For Each item As KeyValuePair(Of String, MemberInfo) In classMetaDescription.ColumnsByDatabaseNames
            If (item.Key = aiColumnName) Then Continue For
            If (item.Value.MemberInfoType = MemberInfoType.Field) Then Continue For
            If (Not touched.ContainsKey(item.Key)) Then Continue For
            columnsSql += separator + item.Key
            paramsSql += separator + "@param" + paramsCounter.ToString()
            touchedItemValue = touched(item.Key)
            If (item.Value.Type.IsEnum) Then touchedItemValue = touchedItemValue.ToString()
            params.Add("param" + paramsCounter.ToString(), touchedItemValue)
            paramsCounter += 1
            separator = ", "
        Next
        Return Databasic.Statement.Prepare(
            $"INSERT INTO {ActiveRecord.Resource.Table(classMetaDescription)} ({columnsSql}) 
			VALUES ({paramsSql})",
            transactionOrConnection
        ).Exec(params)
    End Function

End Class