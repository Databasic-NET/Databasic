Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Databasic

Public Class MetaDescriptor

    Private Shared _lock As ReaderWriterLockSlim = New ReaderWriterLockSlim()
    Private Shared _register As New Dictionary(Of String, MetaDescription)

    Friend Shared Function GetIndexerPropertyName(Type As Type) As String
        Dim defaultAttr As Attribute = Attribute.GetCustomAttribute(Type, GetType(DefaultMemberAttribute))
        Return If(TypeOf defaultAttr Is DefaultMemberAttribute, DirectCast(defaultAttr, DefaultMemberAttribute).MemberName, "")
    End Function

    Friend Shared Function IsCompilerGeneratedOrSystem(propOrFieldInfo As System.Reflection.MemberInfo) As Boolean
        If propOrFieldInfo.Name = "Resource" Then Return True
        Dim compilerGeneratedAttr As Attribute = Attribute.GetCustomAttribute(propOrFieldInfo, GetType(CompilerGeneratedAttribute))
        Return TypeOf compilerGeneratedAttr Is CompilerGeneratedAttribute
    End Function

    Friend Shared Function GetClassDescription(type As Type) As MetaDescription
        Dim result As MetaDescription = Nothing
        Dim key As String = type.Assembly.GetName().Name + ":" + type.FullName
        MetaDescriptor._lock.EnterUpgradeableReadLock()
        If MetaDescriptor._register.ContainsKey(key) Then
            result = MetaDescriptor._register(key)
            MetaDescriptor._lock.ExitUpgradeableReadLock()
        Else
            MetaDescriptor._lock.EnterWriteLock()
            MetaDescriptor._lock.ExitUpgradeableReadLock()
            result = MetaDescriptor._completeMetaDescription(type)
            MetaDescriptor._register.Add(key, result)
            MetaDescriptor._lock.ExitWriteLock()
        End If
        Return result
    End Function

    Friend Shared Function GetColumnByCodeName(type As Type, codeColumnName As String) As Databasic.MemberInfo
        Dim result As Databasic.MemberInfo = Nothing
        Dim members As Dictionary(Of String, Databasic.MemberInfo)
        Dim meta As MetaDescription
        Dim key As String = type.Assembly.GetName().Name + ":" + type.FullName
        MetaDescriptor._lock.EnterUpgradeableReadLock()
        If MetaDescriptor._register.ContainsKey(key) Then
            members = MetaDescriptor._register(key).ColumnsByCodeNames
            If members.ContainsKey(codeColumnName) Then
                result = members(codeColumnName)
            End If
            MetaDescriptor._lock.ExitUpgradeableReadLock()
        Else
            MetaDescriptor._lock.EnterWriteLock()
            MetaDescriptor._lock.ExitUpgradeableReadLock()
            meta = MetaDescriptor._completeMetaDescription(type)
            MetaDescriptor._register.Add(key, meta)
            members = meta.ColumnsByCodeNames
            If members.ContainsKey(codeColumnName) Then
                result = members(codeColumnName)
            End If
            MetaDescriptor._lock.ExitWriteLock()
        End If
        Return result
    End Function

    Friend Shared Function GetColumnsByDbNames(type As Type) As Dictionary(Of String, Databasic.MemberInfo)
        Dim result As Dictionary(Of String, Databasic.MemberInfo)
        Dim meta As MetaDescription
        Dim key As String = type.Assembly.GetName().Name + ":" + type.FullName
        MetaDescriptor._lock.EnterUpgradeableReadLock()
        If MetaDescriptor._register.ContainsKey(key) Then
            result = MetaDescriptor._register(key).ColumnsByDatabaseNames
            MetaDescriptor._lock.ExitUpgradeableReadLock()
        Else
            MetaDescriptor._lock.EnterWriteLock()
            MetaDescriptor._lock.ExitUpgradeableReadLock()
            meta = MetaDescriptor._completeMetaDescription(type)
            MetaDescriptor._register.Add(key, meta)
            result = meta.ColumnsByDatabaseNames
            MetaDescriptor._lock.ExitWriteLock()
        End If
        Return result
    End Function

    Friend Shared Function GetColumnsByCodeNames(type As Type) As Dictionary(Of String, Databasic.MemberInfo)
        Dim result As Dictionary(Of String, Databasic.MemberInfo)
        Dim meta As MetaDescription
        Dim key As String = type.Assembly.GetName().Name + ":" + type.FullName
        MetaDescriptor._lock.EnterUpgradeableReadLock()
        If MetaDescriptor._register.ContainsKey(key) Then
            result = MetaDescriptor._register(key).ColumnsByCodeNames
            MetaDescriptor._lock.ExitUpgradeableReadLock()
        Else
            MetaDescriptor._lock.EnterWriteLock()
            MetaDescriptor._lock.ExitUpgradeableReadLock()
            meta = MetaDescriptor._completeMetaDescription(type)
            MetaDescriptor._register.Add(key, meta)
            result = meta.ColumnsByCodeNames
            MetaDescriptor._lock.ExitWriteLock()
        End If
        Return result
    End Function

    Friend Shared Function GetAllKeyColumns(type As Type) As AllKeyColumns
        Dim result As AllKeyColumns = New AllKeyColumns
        Dim meta As MetaDescription
        Dim key As String = type.Assembly.GetName().Name + ":" + type.FullName
        MetaDescriptor._lock.EnterUpgradeableReadLock()
        If MetaDescriptor._register.ContainsKey(key) Then
            meta = MetaDescriptor._register(key)
            result.PrimaryColumns = meta.PrimaryColumns
            result.UniqueColumns = meta.UniqueColumns
            result.AutoIncrementColumn = meta.AutoIncrementColumn
            MetaDescriptor._lock.ExitUpgradeableReadLock()
        Else
            MetaDescriptor._lock.EnterWriteLock()
            MetaDescriptor._lock.ExitUpgradeableReadLock()
            meta = MetaDescriptor._completeMetaDescription(type)
            MetaDescriptor._register.Add(key, meta)
            result.PrimaryColumns = meta.PrimaryColumns
            result.UniqueColumns = meta.UniqueColumns
            result.AutoIncrementColumn = meta.AutoIncrementColumn
            MetaDescriptor._lock.ExitWriteLock()
        End If
        Return result
    End Function

    Friend Shared Function GetAllKeyColumns(type As Type, keyType As KeyType) As AllKeyColumns
        Dim result As AllKeyColumns = New AllKeyColumns
        Dim meta As MetaDescription
        Dim key As String = type.Assembly.GetName().Name + ":" + type.FullName
        MetaDescriptor._lock.EnterUpgradeableReadLock()
        If MetaDescriptor._register.ContainsKey(key) Then
            meta = MetaDescriptor._register(key)
            If keyType = KeyType.Primary Then
                result.PrimaryColumns = meta.PrimaryColumns
            Else
                result.UniqueColumns = meta.UniqueColumns
            End If
            result.AutoIncrementColumn = meta.AutoIncrementColumn
            MetaDescriptor._lock.ExitUpgradeableReadLock()
        Else
            MetaDescriptor._lock.EnterWriteLock()
            MetaDescriptor._lock.ExitUpgradeableReadLock()
            meta = MetaDescriptor._completeMetaDescription(type)
            MetaDescriptor._register.Add(key, meta)
            If keyType = KeyType.Primary Then
                result.PrimaryColumns = meta.PrimaryColumns
            Else
                result.UniqueColumns = meta.UniqueColumns
            End If
            result.AutoIncrementColumn = meta.AutoIncrementColumn
            MetaDescriptor._lock.ExitWriteLock()
        End If
        Return result
    End Function

    Private Shared Function _completeMetaDescription(type As Type) As MetaDescription
        Dim connectionAttr As ConnectionAttribute = DirectCast(Attribute.GetCustomAttribute(type, Constants.ConnectionAttrType), ConnectionAttribute)
        Dim tableAttr As TableAttribute = DirectCast(Attribute.GetCustomAttribute(type, Constants.TableAttrType), TableAttribute)
        Dim result As New MetaDescription With {
            .ClassType = type,
            .ConnectionIndex = If(TypeOf connectionAttr Is ConnectionAttribute, connectionAttr.ConnectionIndex, Databasic.Defaults.CONNECTION_INDEX),
            .Tables = If(TypeOf tableAttr Is TableAttribute, tableAttr.Tables, New String() {})
        }
        MetaDescriptor._completeColumnsCollections(type, result)
        Return result
    End Function

    Private Shared Function _completeInstanceDataMembers(type As Type) As Dictionary(Of String, Databasic.MemberInfo)
        Dim result As New Dictionary(Of String, Databasic.MemberInfo)
        Dim indexerPropertyName As String = MetaDescriptor.GetIndexerPropertyName(type)
        For Each prop As PropertyInfo In type.GetProperties(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
            If (
                (Not result.ContainsKey(prop.Name)) AndAlso
                prop.Name <> indexerPropertyName AndAlso
                (Not MetaDescriptor.IsCompilerGeneratedOrSystem(prop))
            ) Then result.Add(prop.Name, New Databasic.MemberInfo With {
                .MemberInfo = prop,
                .MemberInfoType = MemberInfoType.Prop,
                .Type = If(
                    Nullable.GetUnderlyingType(prop.PropertyType) <> Nothing,
                    Nullable.GetUnderlyingType(prop.PropertyType),
                    prop.PropertyType
                )
            })
        Next
        For Each field As FieldInfo In type.GetFields(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
            If (
                Not result.ContainsKey(field.Name) AndAlso
                Not MetaDescriptor.IsCompilerGeneratedOrSystem(field)
            ) Then result.Add(field.Name, New Databasic.MemberInfo With {
                .MemberInfo = field,
                .MemberInfoType = MemberInfoType.Field,
                .Type = If(
                    Nullable.GetUnderlyingType(field.FieldType) <> Nothing,
                    Nullable.GetUnderlyingType(field.FieldType),
                    field.FieldType
                )
            })
        Next
        Return result
    End Function

    Private Shared Sub _completeColumnsCollections(type As Type, ByRef result As MetaDescription)
		Dim codeColumnName As String, dbColumnName As String,
			columnAttr As ColumnAttribute,
			formatAttr As FormatAttribute, formatProvider As IFormatProvider,
			trimAttr As TrimAttribute, trimChars As Char(), keyName As String,
			idColumnAttr As IdColumnAttribute,
			autoIncrementAttr As AutoIncrementAttribute,
			primaryKeyAttr As PrimaryKeyAttribute,
			uniqueKeyAttr As UniqueKeyAttribute,
			useEnumUnderlyingValueAttr As UseEnumUnderlyingValue,
		reflMemberInfo As Reflection.MemberInfo
		result.ColumnsByCodeNames = New Dictionary(Of String, Databasic.MemberInfo)
		result.ColumnsByDatabaseNames = New Dictionary(Of String, Databasic.MemberInfo)
		result.PrimaryColumns = New Dictionary(Of String, Dictionary(Of String, String))
		result.UniqueColumns = New Dictionary(Of String, Dictionary(Of String, String))
		result.AutoIncrementColumn = Nothing
		Dim members As Dictionary(Of String, Databasic.MemberInfo) = MetaDescriptor._completeInstanceDataMembers(type)
		For Each item In members
			reflMemberInfo = item.Value.MemberInfo
			columnAttr = DirectCast(Attribute.GetCustomAttribute(reflMemberInfo, Constants.ColumnAttrType), ColumnAttribute)
			formatAttr = DirectCast(Attribute.GetCustomAttribute(reflMemberInfo, Constants.FormatAttrType), FormatAttribute)
			trimAttr = DirectCast(Attribute.GetCustomAttribute(reflMemberInfo, Constants.TrimAttrType), TrimAttribute)
			idColumnAttr = DirectCast(Attribute.GetCustomAttribute(reflMemberInfo, Constants.IdColumnAttrType), IdColumnAttribute)
			autoIncrementAttr = DirectCast(Attribute.GetCustomAttribute(reflMemberInfo, Constants.AutoIncrementAttrType), AutoIncrementAttribute)
			primaryKeyAttr = DirectCast(Attribute.GetCustomAttribute(reflMemberInfo, Constants.PrimaryKeyAttrType), PrimaryKeyAttribute)
			uniqueKeyAttr = DirectCast(Attribute.GetCustomAttribute(reflMemberInfo, Constants.UniqueKeyAttrType), UniqueKeyAttribute)
			useEnumUnderlyingValueAttr = DirectCast(Attribute.GetCustomAttribute(item.Value.Type, Constants.UseEnumUnderlyingValuesAttrType), UseEnumUnderlyingValue)
			codeColumnName = reflMemberInfo.Name
			dbColumnName = codeColumnName
			keyName = ""
			If TypeOf columnAttr Is ColumnAttribute Then
				dbColumnName = If(String.IsNullOrEmpty(columnAttr.ColumnName), codeColumnName, columnAttr.ColumnName)
			End If
			formatProvider = If(TypeOf formatAttr Is FormatAttribute, formatAttr.FormatProvider, Nothing)
			trimChars = If(TypeOf trimAttr Is TrimAttribute, trimAttr.Chars, New Char() {})
			' complete code column names and db column names collections
			result.ColumnsByCodeNames.Add(codeColumnName, New Databasic.MemberInfo With {
				.Name = dbColumnName,
				.MemberInfo = reflMemberInfo,
				.Type = item.Value.Type,
				.FormatProvider = formatProvider,
				.TrimChars = trimChars,
				.MemberInfoType = item.Value.MemberInfoType,
				.Value = Nothing,
				.UseEnumUnderlyingValue = TypeOf useEnumUnderlyingValueAttr Is UseEnumUnderlyingValue
			})
			result.ColumnsByDatabaseNames.Add(dbColumnName, New Databasic.MemberInfo With {
				.Name = codeColumnName,
				.MemberInfo = reflMemberInfo,
				.Type = item.Value.Type,
				.FormatProvider = formatProvider,
				.TrimChars = trimChars,
				.MemberInfoType = item.Value.MemberInfoType,
				.Value = Nothing,
				.UseEnumUnderlyingValue = TypeOf useEnumUnderlyingValueAttr Is UseEnumUnderlyingValue
			})
			' if there is any key info at class element, add it into keys info collections
			If (
				TypeOf primaryKeyAttr Is PrimaryKeyAttribute OrElse
				TypeOf idColumnAttr Is IdColumnAttribute
			) Then
				If Not String.IsNullOrEmpty(primaryKeyAttr.KeyName) Then keyName = primaryKeyAttr.KeyName
				If result.PrimaryColumns.ContainsKey(keyName) Then
					result.PrimaryColumns(keyName).Add(codeColumnName, dbColumnName)
				Else
					result.PrimaryColumns.Add(keyName, New Dictionary(Of String, String)() From {
						{codeColumnName, dbColumnName}
					})
				End If
			End If
			If TypeOf uniqueKeyAttr Is UniqueKeyAttribute Then
				If Not String.IsNullOrEmpty(uniqueKeyAttr.KeyName) Then keyName = uniqueKeyAttr.KeyName
				If result.UniqueColumns.ContainsKey(keyName) Then
					result.UniqueColumns(keyName).Add(codeColumnName, dbColumnName)
				Else
					result.UniqueColumns.Add(keyName, New Dictionary(Of String, String)() From {
						{codeColumnName, dbColumnName}
					})
				End If
			End If
			If (
				TypeOf autoIncrementAttr Is AutoIncrementAttribute OrElse
				TypeOf idColumnAttr Is IdColumnAttribute
			) Then
				If Not result.AutoIncrementColumn.HasValue Then
					result.AutoIncrementColumn = New AutoIncrementColumn With {
						.CodeColumnName = codeColumnName,
						.DatabaseColumnName = dbColumnName
					}
				Else
					Throw New Exception(String.Format(
						"Class '{0}' has defined multiple autoincrement members. " +
						"Plase define only one field or property with attribute '{1}'.",
						type.FullName, "AutoIncrement"
					))
				End If
			End If
		Next
    End Sub

End Class