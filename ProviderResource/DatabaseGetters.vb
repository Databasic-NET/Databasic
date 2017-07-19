Partial Public MustInherit Class ProviderResource





	Public Overridable Function GetById(id As Int64, ByRef connectionOrTransaction As Object, ByRef classMetaDescription As MetaDescription) As Statement
		Dim columns As String = String.Join(",", Databasic.ProviderResource.ColumnsArray(classMetaDescription.ClassType, 0))
		Return Databasic.Statement.Prepare(
			$"SELECT {columns} 
			FROM {ActiveRecord.Resource.Table(classMetaDescription)} 
			WHERE {classMetaDescription.AutoIncrementColumn.Value.DatabaseColumnName} = @idColumn",
			connectionOrTransaction
		).FetchOne(New With {.idColumn = id})
	End Function






	Public Overridable Function GetByKey(
		keyData As Dictionary(Of String, Object),
		orderSqlStatement As String,
		connectionOrTransaction As Object,
		ByRef classMetaDescription As MetaDescription
	) As Statement
		Dim columns As String = String.Join(",", Databasic.ProviderResource.ColumnsArray(classMetaDescription.ClassType, 0))
		Dim params As New Dictionary(Of String, Object)
		Dim conditionsSql As String = ""
		Dim paramsCounter As Int16 = 0
		Dim separator As String = ""
		For Each keyColumnName As String In keyData.Keys
			conditionsSql += separator + keyColumnName + " = @param" + paramsCounter.ToString()
			params.Add("param" + paramsCounter.ToString(), keyData(keyColumnName))
			paramsCounter += 1
			separator = ", "
		Next
		Dim sql As String = $"
			SELECT {columns} 
			FROM {ActiveRecord.Resource.Table(classMetaDescription)} 
			WHERE {conditionsSql}
		"
		If orderSqlStatement.Length > 0 Then
			Return Databasic.Statement.Prepare(sql + " " + orderSqlStatement, connectionOrTransaction).FetchAll(params)
		Else
			Return Databasic.Statement.Prepare(sql, connectionOrTransaction).FetchOne(params)
		End If
	End Function







	Public MustOverride Function GetLastInsertedId(ByRef transaction As Databasic.Transaction, Optional ByRef classMetaDescription As MetaDescription = Nothing) As Object




	Public Overridable Function GetCount(
		conditionSqlStatement As String,
		connectionOrTransaction As Object,
		ByRef classMetaDescription As MetaDescription
	) As Int64
		Dim countColumn As String = "*"
		Dim table As String = ActiveRecord.Resource.Table(classMetaDescription)
		If classMetaDescription.AutoIncrementColumn.HasValue Then
			countColumn = classMetaDescription.AutoIncrementColumn.Value.DatabaseColumnName
			Dim connectionIndex As Int32 = 0
			If TypeOf connectionOrTransaction Is Connection Then
				connectionIndex = DirectCast(connectionOrTransaction, Connection).ConnectionIndex
			Else
				connectionIndex = DirectCast(connectionOrTransaction, Transaction).ConnectionWrapper.ConnectionIndex
			End If
			If (ProviderResource.IsColumnNullable(connectionIndex, table, countColumn)) Then
				countColumn = "*"
			End If
		End If
		Dim sql As String = $"SELECT COUNT({countColumn}) FROM {table}"
		If Not String.IsNullOrEmpty(conditionSqlStatement) Then sql += " WHERE " + conditionSqlStatement
		Return Databasic.Statement.Prepare(sql, connectionOrTransaction).FetchOne().ToInstance(Of Int64)()
	End Function




	Public Overridable Function GetList(
		conditionSqlStatement As String,
		conditionParams As Object,
		orderBySqlStatement As String,
		offset As Int64?,
		limit As Int64?,
		connectionOrTransaction As Object,
		ByRef classMetaDescription As MetaDescription
	) As Databasic.Statement
		Dim columns As String = String.Join(",", Databasic.ProviderResource.ColumnsArray(classMetaDescription.ClassType, 0))
		Dim sql As String = $"SELECT {columns} FROM {ActiveRecord.Resource.Table(classMetaDescription)}"
		Dim params As Object = Nothing
		If Not String.IsNullOrEmpty(conditionSqlStatement) Then
			sql += " WHERE " + conditionSqlStatement
			params = conditionParams
		End If
		If Not String.IsNullOrEmpty(orderBySqlStatement) Then
			sql += " ORDER BY " + orderBySqlStatement
		End If
		If offset.HasValue Then sql += $" OFFSET {offset} ROWS"
		If limit.HasValue Then sql += $" FETCH NEXT {limit} ROWS ONLY"
		Return Databasic.Statement.Prepare(sql, connectionOrTransaction).FetchAll(params)
	End Function

End Class