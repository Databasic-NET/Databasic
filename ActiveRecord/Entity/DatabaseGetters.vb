Imports System.ComponentModel
Imports System.Dynamic
Imports System.Reflection
'Imports System.Runtime.CompilerServices

Namespace ActiveRecord
	Partial Public MustInherit Class Entity
		Inherits DynamicObject





		'<CompilerGenerated>
		'Public Property Resource As Resource





		''' <summary>
		''' Automaticly initializes 'Resource' class member (static or instance) with singleton Resource instance, 
		''' if there is no instance initialized manualy by programmer.
		''' </summary>
		Public Sub New()
			Me.initResource()
		End Sub





		''' <summary>
		''' Initializes 'Resource' class member (static or instance) with singleton Resource instance,
		''' if there is no instance yet. This method is always called by Entity constructor.
		''' </summary>
		Protected Sub initResource()
			Dim meType As Type = Me.GetType()
			Dim resourceMember As Reflection.MemberInfo = (
				From member As Reflection.MemberInfo In meType.GetMembers(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
				Where member.Name = "Resource" AndAlso (TypeOf member Is Reflection.PropertyInfo OrElse TypeOf member Is Reflection.FieldInfo)
				Select member
			).FirstOrDefault()
			If resourceMember = Nothing Then Return
			If TypeOf resourceMember Is Reflection.PropertyInfo Then
				Dim pi As Reflection.PropertyInfo = DirectCast(resourceMember, Reflection.PropertyInfo)
				If pi.GetValue(Me, Nothing) = Nothing AndAlso pi.CanWrite Then pi.SetValue(Me, Resource.GetInstance(pi.PropertyType), Nothing)
			ElseIf TypeOf resourceMember Is Reflection.FieldInfo Then
				Dim fi As Reflection.FieldInfo = DirectCast(resourceMember, Reflection.FieldInfo)
				If fi.GetValue(Me) = Nothing Then fi.SetValue(Me, Resource.GetInstance(fi.FieldType))
			End If
		End Sub





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
				id, Connection.Get(If(String.IsNullOrEmpty(connectionName), Databasic.Defaults.CONNECTION_INDEX, connectionName))
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
			Dim statement As Statement = connection.GetProviderResource().GetById(id, connection, classMetaDescription)
			Return ActiveRecord.Entity.ToInstance(Of TValue)(statement.Reader, classMetaDescription.ColumnsByDatabaseNames)
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
			Dim statement As Statement = transaction.ConnectionWrapper.GetProviderResource().GetById(id, transaction, classMetaDescription)
			Return ActiveRecord.Entity.ToInstance(Of TValue)(statement.Reader, classMetaDescription.ColumnsByDatabaseNames)
		End Function




		Public Shared Function GetByKey(Of TValue)(keyValues As Object, keyName As String, transaction As Transaction) As TValue
			Return Entity._getByKey(Of TValue)(Entity._getByKeyValuesDct(keyValues), keyName, transaction)
		End Function
		Public Shared Function GetByKey(Of TValue)(keyValues As Dictionary(Of String, Object), keyName As String, transaction As Transaction) As TValue
			Return Entity._getByKey(Of TValue)(keyValues, keyName, transaction)
		End Function
		Public Shared Function GetByKey(Of TValue)(keyValues As Object, keyName As String, connection As Connection) As TValue
			Return Entity._getByKey(Of TValue)(Entity._getByKeyValuesDct(keyValues), keyName, connection)
		End Function
		Public Shared Function GetByKey(Of TValue)(keyValues As Dictionary(Of String, Object), keyName As String, connection As Connection) As TValue
			Return Entity._getByKey(Of TValue)(keyValues, keyName, connection)
		End Function
		Private Shared Function _getByKeyValuesDct(keyValues As Object) As Dictionary(Of String, Object)
			Dim result As New Dictionary(Of String, Object)
			Dim keyValuesProps As PropertyDescriptorCollection = TypeDescriptor.GetProperties(keyValues)
			For Each prop As PropertyDescriptor In keyValuesProps
				result.Add(prop.Name, prop.GetValue(keyValues))
			Next
			Return result
		End Function
		Private Shared Function _getByKey(Of TValue)(keyValues As Dictionary(Of String, Object), keyName As String, connectionOrTransaction As Object) As TValue
			Dim instanceType As Type = GetType(TValue)
			Dim classMetaDescription As MetaDescription = MetaDescriptor.GetClassDescription(instanceType)
			Dim keyColumns As New Dictionary(Of String, String)
			If classMetaDescription.UniqueColumns.ContainsKey(keyName) Then
				keyColumns = classMetaDescription.PrimaryColumns(keyName)
			ElseIf classMetaDescription.PrimaryColumns.ContainsKey(keyName) Then
				keyColumns = classMetaDescription.UniqueColumns(keyName)
			Else
				Events.RaiseError(New Exception(If(keyName.Length > 0, String.Format(
					"Class '{0}' has no member(s) with 'UniqueKey(""{2}"")' or 'PrimaryKey(""{1}"")' attribute(s).",
					classMetaDescription.ClassType,
					keyName, keyName
				), String.Format(
					"Class '{0}' has no member(s) with 'UniqueKey' or 'PrimaryKey' attribute(s).",
					classMetaDescription.ClassType
				))))
			End If
			Dim keyParams As New Dictionary(Of String, Object)
			For Each item As KeyValuePair(Of String, String) In keyColumns
				If keyValues.ContainsKey(item.Key) Then
					keyParams.Add(item.Value, keyValues(item.Key))
				Else
					Events.RaiseError(New Exception(String.Format(
						"Key value '{0}' to load '{1}' by key '{2}' is missing, " +
						"please put any record '{3}' into first param 'keyValues' to load data properly.",
						item.Key, instanceType.FullName, keyName, item.Key
					)))
				End If
			Next
			Return Entity._getByKeyResult(Of TValue)(keyParams, connectionOrTransaction, classMetaDescription)
		End Function
		Private Shared Function _getByKeyResult(Of TValue)(
			keyParams As Dictionary(Of String, Object), connection As Connection, ByRef classMetaDescription As MetaDescription
		) As TValue
			Dim statement As Statement = connection.GetProviderResource().GetByKey(
				keyParams, "", connection, classMetaDescription
			)
			Return ActiveRecord.Entity.ToInstance(Of TValue)(
				statement.Reader, classMetaDescription.ColumnsByDatabaseNames
			)
		End Function
		Private Shared Function _getByKeyResult(Of TValue)(
			keyParams As Dictionary(Of String, Object), transaction As Transaction, ByRef classMetaDescription As MetaDescription
		) As TValue
			Dim statement As Statement = transaction.ConnectionWrapper.GetProviderResource().GetByKey(
				keyParams, "", transaction, classMetaDescription
			)
			Return ActiveRecord.Entity.ToInstance(Of TValue)(
				statement.Reader, classMetaDescription.ColumnsByDatabaseNames
			)
		End Function






		Public Shared Function GetList(Of TValue)(
			Optional conditionSqlStatement As String = "",
			Optional conditionParams As Object = Nothing,
			Optional orderBySqlStatement As String = "",
			Optional offset As Int64? = Nothing,
			Optional limit As Int64? = Nothing,
			Optional connectionIndex As Int32 = Databasic.Defaults.CONNECTION_INDEX
		) As List(Of TValue)
			Return Entity.GetList(Of TValue)(
				conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(connectionIndex)
			)
		End Function
		Public Shared Function GetList(Of TValue)(
			Optional conditionSqlStatement As String = "",
			Optional conditionParams As Dictionary(Of String, Object) = Nothing,
			Optional orderBySqlStatement As String = "",
			Optional offset As Int64? = Nothing,
			Optional limit As Int64? = Nothing,
			Optional connectionIndex As Int32 = Databasic.Defaults.CONNECTION_INDEX
		) As List(Of TValue)
			Return Entity.GetList(Of TValue)(
				conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(connectionIndex)
			)
		End Function
		Public Shared Function GetList(Of TValue)(
			conditionSqlStatement As String,
			conditionParams As Object,
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connectionName As String
		) As List(Of TValue)
			Return Entity.GetList(Of TValue)(
				conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(If(String.IsNullOrEmpty(connectionName), Databasic.Defaults.CONNECTION_INDEX, connectionName))
			)
		End Function
		Public Shared Function GetList(Of TValue)(
			conditionSqlStatement As String,
			conditionParams As Dictionary(Of String, Object),
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connectionName As String
		) As List(Of TValue)
			Return Entity.GetList(Of TValue)(
				conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(If(String.IsNullOrEmpty(connectionName), Databasic.Defaults.CONNECTION_INDEX, connectionName))
			)
		End Function
		Public Shared Function GetList(Of TValue)(
			conditionSqlStatement As String,
			conditionParams As Object,
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connection As Connection
		) As List(Of TValue)
			Return connection.GetProviderResource().GetList(
				conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit, connection,
				MetaDescriptor.GetClassDescription(GetType(TValue))
			).ToList(Of TValue)
		End Function






		Public Shared Function GetDictionary(Of TKey, TValue)(
			Optional databaseKeyColumnName As String = "",
			Optional conditionSqlStatement As String = "",
			Optional conditionParams As Object = Nothing,
			Optional orderBySqlStatement As String = "",
			Optional offset As Int64? = Nothing,
			Optional limit As Int64? = Nothing,
			Optional connectionIndex As Int32 = Databasic.Defaults.CONNECTION_INDEX
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				databaseKeyColumnName, conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(connectionIndex)
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			Optional databaseKeyColumnName As String = "",
			Optional conditionSqlStatement As String = "",
			Optional conditionParams As Dictionary(Of String, Object) = Nothing,
			Optional orderBySqlStatement As String = "",
			Optional offset As Int64? = Nothing,
			Optional limit As Int64? = Nothing,
			Optional connectionIndex As Int32 = Databasic.Defaults.CONNECTION_INDEX
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				databaseKeyColumnName, conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(connectionIndex)
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			databaseKeyColumnName As String,
			conditionSqlStatement As String,
			conditionParams As Object,
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connectionName As String
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				databaseKeyColumnName, conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(If(String.IsNullOrEmpty(connectionName), Databasic.Defaults.CONNECTION_INDEX, connectionName))
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			databaseKeyColumnName As String,
			conditionSqlStatement As String,
			conditionParams As Dictionary(Of String, Object),
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connectionName As String
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				databaseKeyColumnName, conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(If(String.IsNullOrEmpty(connectionName), Databasic.Defaults.CONNECTION_INDEX, connectionName))
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			databaseKeyColumnName As String,
			conditionSqlStatement As String,
			conditionParams As Object,
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connection As Connection
		) As Dictionary(Of TKey, TValue)
			Return connection.GetProviderResource().GetList(
				conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit, connection,
				MetaDescriptor.GetClassDescription(GetType(TValue))
			).ToDictionary(Of TKey, TValue)(databaseKeyColumnName)
		End Function





		Public Shared Function GetDictionary(Of TKey, TValue)(
			keySelector As Func(Of TValue, TKey),
			conditionSqlStatement As String,
			conditionParams As Object,
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			Optional connectionIndex As Int32 = Databasic.Defaults.CONNECTION_INDEX
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				keySelector, conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(connectionIndex)
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			keySelector As Func(Of TValue, TKey),
			conditionSqlStatement As String,
			conditionParams As Dictionary(Of String, Object),
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			Optional connectionIndex As Int32 = Databasic.Defaults.CONNECTION_INDEX
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				keySelector, conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(connectionIndex)
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			keySelector As Func(Of TValue, TKey),
			conditionSqlStatement As String,
			conditionParams As Object,
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connectionName As String
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				keySelector, conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(If(String.IsNullOrEmpty(connectionName), Databasic.Defaults.CONNECTION_INDEX, connectionName))
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			keySelector As Func(Of TValue, TKey),
			conditionSqlStatement As String,
			conditionParams As Dictionary(Of String, Object),
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connectionName As String
		) As Dictionary(Of TKey, TValue)
			Return Entity.GetDictionary(Of TKey, TValue)(
				keySelector, conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit,
				Databasic.Connection.Get(If(String.IsNullOrEmpty(connectionName), Databasic.Defaults.CONNECTION_INDEX, connectionName))
			)
		End Function
		Public Shared Function GetDictionary(Of TKey, TValue)(
			keySelector As Func(Of TValue, TKey),
			conditionSqlStatement As String,
			conditionParams As Object,
			orderBySqlStatement As String,
			offset As Int64?,
			limit As Int64?,
			connection As Connection
		) As Dictionary(Of TKey, TValue)
			Return connection.GetProviderResource().GetList(
				conditionSqlStatement, conditionParams, orderBySqlStatement, offset, limit, connection,
				MetaDescriptor.GetClassDescription(GetType(TValue))
			).ToDictionary(Of TKey, TValue)(keySelector)
		End Function
		'






		Public Shared Function GetCount(Of TValue)(Optional conditionSqlStatement As String = "", Optional connectionIndex As Int32 = Databasic.Defaults.CONNECTION_INDEX) As Int64
			Return Entity.GetCount(Of TValue)(
				conditionSqlStatement, Databasic.Connection.Get(connectionIndex)
			)
		End Function
		Public Shared Function GetCount(Of TValue)(conditionSqlStatement As String, connectionName As String) As Int64
			Return Entity.GetCount(Of TValue)(
				conditionSqlStatement,
				Databasic.Connection.Get(If(String.IsNullOrEmpty(connectionName), Databasic.Defaults.CONNECTION_INDEX, connectionName))
			)
		End Function
		Public Shared Function GetCount(Of TValue)(conditionSqlStatement As String, connection As Connection) As Int64
			Return connection.GetProviderResource().GetCount(
				conditionSqlStatement, connection, MetaDescriptor.GetClassDescription(GetType(TValue))
			)
		End Function
		Public Shared Function GetCount(Of TValue)(conditionSqlStatement As String, transaction As Transaction) As Int64
			Return transaction.ConnectionWrapper.GetProviderResource().GetCount(
				conditionSqlStatement, transaction, MetaDescriptor.GetClassDescription(GetType(TValue))
			)
		End Function





	End Class
End Namespace