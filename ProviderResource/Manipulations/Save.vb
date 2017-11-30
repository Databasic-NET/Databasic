Partial Public MustInherit Class ProviderResource

    Public Overridable Function Save(
        insertNew As Boolean,
        ByRef instance As ActiveRecord.Entity,
        ByRef connection As Connection
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
                    trans = Connection.BeginTransaction(
                        connection, "Dtbsc.Prvdr.Rsrc.Save()", IsolationLevel.Unspecified
                    )
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
				Events.RaiseError(
					  $"Inserting of new {instance.GetType().FullName} failed.", ex
				)
			End Try
            Return result
        End If
    End Function

    Public Overridable Function Save(
        insertNew As Boolean,
        ByRef instance As ActiveRecord.Entity,
        ByRef transaction As Transaction
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

End Class