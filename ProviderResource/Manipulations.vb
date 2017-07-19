Partial Public MustInherit Class ProviderResource





	Public Overridable Function Update(
		ByRef instance As ActiveRecord.Entity, ByRef connection As Connection, ByRef classMetaDescription As MetaDescription
	) As Int32
		Return Me._update(instance, connection, classMetaDescription)
	End Function
	Public Overridable Function Update(
		ByRef instance As ActiveRecord.Entity, ByRef transaction As Transaction, ByRef classMetaDescription As MetaDescription
	) As Int32
		Return Me._update(instance, transaction, classMetaDescription)
	End Function
	Private Function _update(
		ByRef instance As ActiveRecord.Entity, ByRef transactionOrConnection As Object, ByRef classMetaDescription As MetaDescription
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
		For Each item As KeyValuePair(Of String, Object) In touched
			If (item.Key = aiColumnName) Then Continue For
			sql += separator + item.Key + " = @param" + paramsCounter.ToString()
			params.Add("param" + paramsCounter.ToString(), item.Value)
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





	Public Overridable Function Insert(
		ByRef instance As ActiveRecord.Entity, ByRef connection As Connection, ByRef classMetaDescription As MetaDescription
	) As Int32
		Return Me._insert(instance, connection, classMetaDescription)
	End Function
	Public Overridable Function Insert(
		ByRef instance As ActiveRecord.Entity, ByRef transaction As Transaction, ByRef classMetaDescription As MetaDescription
	) As Int32
		Return Me._insert(instance, transaction, classMetaDescription)
	End Function
	Private Function _insert(
		ByRef instance As ActiveRecord.Entity, ByRef transactionOrConnection As Object, ByRef classMetaDescription As MetaDescription
	) As Int32
		Dim columnsSql As String = ""
		Dim paramsSql As String = ""
		Dim params As New Dictionary(Of String, Object)
		Dim keyColumns As KeyColumns = ActiveRecord.Resource.KeyColumns(classMetaDescription)
		Dim touched As Dictionary(Of String, Object) = instance.GetTouched(
			False, classMetaDescription.ColumnsByCodeNames
		) ' keys by db names
		Dim separator As String = ""
		Dim paramsCounter As Int16 = 0
		Dim aiColumnName As String = If(
			classMetaDescription.AutoIncrementColumn.HasValue,
			classMetaDescription.AutoIncrementColumn.Value.DatabaseColumnName, ""
		)
		For Each item As KeyValuePair(Of String, Object) In touched
			If (item.Key = aiColumnName) Then Continue For
			columnsSql += separator + item.Key
			paramsSql += separator + "@param" + paramsCounter.ToString()
			params.Add("param" + paramsCounter.ToString(), item.Value)
			paramsCounter += 1
			separator = ", "
		Next
		Return Databasic.Statement.Prepare(
			$"INSERT INTO {ActiveRecord.Resource.Table(classMetaDescription)} ({columnsSql}) VALUES ({paramsSql})",
			transactionOrConnection
		).Exec(params)
	End Function





	Public Overridable Function Save(
		insertNew As Boolean, ByRef instance As ActiveRecord.Entity, ByRef connection As Connection
	) As Int32
		Dim instanceType As Type = instance.GetType()
		Dim classMetaDescription As MetaDescription = MetaDescriptor.GetClassDescription(instanceType)
		If Not insertNew Then
			Return Me.Update(instance, connection, classMetaDescription)
		Else
			Dim result As Int32 = 0
			Dim lastInsertedId As Object = Nothing
			Dim autoIncrementColName As String
			Dim trans As Transaction = Nothing
			Try
				If classMetaDescription.AutoIncrementColumn.HasValue Then
					trans = connection.CreateAndBeginTransaction("Dtbsc.Prvdr.Rsrc.Save()")
					result = Me.Insert(instance, trans, classMetaDescription)
					lastInsertedId = Me.GetLastInsertedId(trans, classMetaDescription)
					trans.Commit()
					autoIncrementColName = classMetaDescription.AutoIncrementColumn.Value.CodeColumnName
					instance.[Set](
						autoIncrementColName, lastInsertedId, True,
						classMetaDescription.ColumnsByCodeNames(autoIncrementColName)
					)
				Else
					result = Me.Insert(instance, connection, classMetaDescription)
				End If
			Catch ex As Exception
				If TypeOf trans Is Transaction Then trans.Rollback()
				Events.RaiseError(New Exception(
					  $"Inserting of new {instance.GetType().FullName} failed.", ex
				))
			End Try
			Return result
		End If
	End Function
	Public Overridable Function Save(
		insertNew As Boolean, ByRef instance As ActiveRecord.Entity, ByRef transaction As Transaction
	) As Int32
		Dim instanceType As Type = instance.GetType()
		Dim classMetaDescription As MetaDescription = MetaDescriptor.GetClassDescription(instanceType)
		If Not insertNew Then
			Return Me.Update(instance, transaction, classMetaDescription)
		Else
			Dim result As Int32 = 0
			Dim lastInsertedId As Object = Nothing
			Dim autoIncrementColName As String

			If classMetaDescription.AutoIncrementColumn.HasValue Then
				result = Me.Insert(instance, transaction, classMetaDescription)
				lastInsertedId = Me.GetLastInsertedId(transaction, classMetaDescription)
				autoIncrementColName = classMetaDescription.AutoIncrementColumn.Value.CodeColumnName
				instance.[Set](
					autoIncrementColName, lastInsertedId, True,
					classMetaDescription.ColumnsByCodeNames(autoIncrementColName)
				)
			Else
				result = Me.Insert(instance, transaction, classMetaDescription)
			End If
			Return result
		End If
	End Function






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