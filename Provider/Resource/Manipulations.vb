Namespace Provider
	Partial Public Class Resource
		Inherits ActiveRecord.Resource




		Public Overridable Function Update(connection As Connection, instanceType As Type, data As Dictionary(Of String, Object), uniqueColumnName As String, uniqueColumnValue As Object) As Int32
            Dim columns As String() = data.Keys.ToArray()
            Dim sql As String = $"UPDATE {Table(instanceType)} SET "
            For Each column As String In columns
                sql += $"{column} = @{column},"
            Next
            sql = sql.TrimEnd(New Char() {","c}) + $" WHERE {uniqueColumnName} = @{uniqueColumnName}"
            data.Add(uniqueColumnName, uniqueColumnValue)
			Return Databasic.Statement.Prepare(sql, connection).Exec(data)
		End Function


        Public Overridable Function Insert(transaction As Transaction, instanceType As Type, data As Dictionary(Of String, Object), uniqueColumnName As String) As Int32
            Dim columns As String()
            If data.ContainsKey(uniqueColumnName) Then
                data.Remove(uniqueColumnName)
                Dim columnsList As List(Of String) = data.Keys.ToList()
                columnsList.Remove(uniqueColumnName)
                columns = columnsList.ToArray()
            Else
                columns = data.Keys.ToArray()
            End If
			Return Databasic.Statement.Prepare(
				$"INSERT {Table(instanceType)} ({String.Join(",", columns)}) VALUES (@{String.Join(",@", columns)})",
				transaction
			).Exec(data)
		End Function



		Public Overridable Function Save(connection As Connection, ByRef instance As Object) As Int32
			Dim instanceType As Type = instance.GetType()
			Dim result As Int32 = 0
			Dim resource As Provider.Resource = Activator.CreateInstance(connection.ResourceType)
			Dim uniqueColumnName As String = instance.UniqueColumn()

			' TODO!!!!!!!!!!!!!

			Dim uniqueColumnValue As Object = instance.[get](uniqueColumnName)
			If uniqueColumnValue IsNot Nothing Then
				Return resource.Update(connection, instanceType, instance.GetTouched(), uniqueColumnName, uniqueColumnValue)
			Else
				Columns(instanceType)
				Dim trans As Transaction = connection.CreateAndBeginTransaction("DbShrp.ActivRcord.Rsourc.Save()")
				Try
					result = resource.Insert(trans, instanceType, instance.GetTouched(), uniqueColumnName)
					Dim newId As Object = resource.GetLastInsertedId(trans)
					instance.[set](uniqueColumnName, newId)
					trans.Commit()
				Catch ex As Exception
					trans.Rollback()
					result = 0
					Events.RaiseError(New Exception(
						  $"Inserting of new {instance.GetType().FullName} failed.", ex
					))
				End Try
				Return result
			End If
		End Function


		Public Overridable Function Delete(connection As Connection, instance As Object) As Int32
			Dim table As String = ActiveRecord.Resource.Table(instance.GetType())
			Dim resource As ActiveRecord.Resource = Activator.CreateInstance(connection.ResourceType)
			Dim uniqueColumnName As String = instance.UniqueColumn()

			' TODO!!!!!!!!!

			Dim uniqueColumnValue As Object = instance.[Get](uniqueColumnName)
			If uniqueColumnValue Is Nothing Then
				Events.RaiseError(New Exception($"Entity '{instance.GetType().FullName}' has no unique column specified."))
			End If
			Return Nothing
			Return Databasic.Statement.Prepare(
				$"DELETE {table} WHERE {uniqueColumnName} = @{uniqueColumnName}"
			).Exec(New Dictionary(Of String, Object) From {
				{uniqueColumnName, uniqueColumnValue}
			})
		End Function



	End Class
End Namespace