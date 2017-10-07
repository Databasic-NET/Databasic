Imports System.ComponentModel
Imports System.Dynamic

Namespace ActiveRecord
    Partial Public MustInherit Class Entity
        Inherits DynamicObject

        ''' <summary>
        ''' Get active record entity instance by autoincrement column, there will be loaded all table columns.
        ''' </summary>
        ''' <typeparam name="TValue">Model class type, inherited from ActiveRecord.</typeparam>
        ''' <param name="connectionIndex">Config connection index to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
        ''' <returns></returns>
        Public Shared Function GetById(Of TValue)(id As Int64, Optional connectionIndex As Int32 = Databasic.Defaults.CONNECTION_INDEX) As TValue
            Return Entity.GetById(Of TValue)(
                id, Connection.Get(connectionIndex)
            )
        End Function

        ''' <summary>
        ''' Get active record entity instance by autoincrement column, there will be loaded all table columns.
        ''' </summary>
        ''' <typeparam name="TValue">Model class type, inherited from ActiveRecord.</typeparam>
        ''' <param name="connectionName">Config connection name to use different database, default by Databasic.Defaults.CONNECTION_INDEX to use first connection in &lt;connectionStrings&gt; list.</param>
        ''' <returns></returns>
        Public Shared Function GetById(Of TValue)(id As Int64, connectionName As String) As TValue
            Return Entity.GetById(Of TValue)(
                id, Connection.Get(If(
                    String.IsNullOrEmpty(connectionName),
                    Databasic.Defaults.CONNECTION_INDEX,
                    connectionName
                ))
            )
        End Function

        ''' <summary>
        ''' Get active record entity instance by autoincrement column, there will be loaded all table columns.
        ''' </summary>
        ''' <typeparam name="TValue">Model class type, inherited from ActiveRecord.</typeparam>
        ''' <param name="connection">Connection intance.</param>
        ''' <exception cref="Exception">TODO</exception>
        ''' <returns></returns>
        Public Shared Function GetById(Of TValue)(id As Int64, connection As Connection) As TValue
            Dim instanceType As Type = GetType(TValue)
            Dim classMetaDescription As MetaDescription = MetaDescriptor.GetClassDescription(instanceType)
            If Not classMetaDescription.AutoIncrementColumn.HasValue Then
                Events.RaiseError(New Exception(String.Format(
                    "Class '{0}' has no whole number member with 'AutoIncrement' attribute.",
                    classMetaDescription.ClassType
                )))
            End If
            Dim statement As Statement = connection.GetProviderResource().GetById(
                id, connection, classMetaDescription
            )
            Return ActiveRecord.Entity.ToInstance(Of TValue)(
                statement.Reader, classMetaDescription.ColumnsByDatabaseNames
            )
        End Function

        Public Shared Function GetById(Of TValue)(id As Int64, transaction As Transaction) As TValue
            Dim instanceType As Type = GetType(TValue)
            Dim classMetaDescription As MetaDescription = MetaDescriptor.GetClassDescription(instanceType)
            If Not classMetaDescription.AutoIncrementColumn.HasValue Then
                Events.RaiseError(New Exception(String.Format(
                    "Class '{0}' has no whole number member with 'AutoIncrement' attribute.",
                    classMetaDescription.ClassType
                )))
            End If
            Dim statement As Statement = transaction.ConnectionWrapper.GetProviderResource().GetById(
                id, transaction, classMetaDescription
            )
            Return ActiveRecord.Entity.ToInstance(Of TValue)(
                statement.Reader, classMetaDescription.ColumnsByDatabaseNames
            )
        End Function

    End Class
End Namespace