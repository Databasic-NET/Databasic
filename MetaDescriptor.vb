Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Databasic

Public Class MetaDescriptor

    Private Shared _lock As ReaderWriterLockSlim = New ReaderWriterLockSlim()
    Private Shared _register As New Dictionary(Of String, MetaDescription)

    Friend Shared ConnectionAttrType As Type = GetType(ConnectionAttribute)
    Friend Shared TableAttrType As Type = GetType(TableAttribute)
    Friend Shared ColumnAttrType As Type = GetType(ColumnAttribute)
    Friend Shared FormatAttrType As Type = GetType(FormatAttribute)
    Friend Shared TrimAttrType As Type = GetType(TrimAttribute)
    Friend Shared PrimaryKeyAttrType As Type = GetType(PrimaryKeyAttribute)
    Friend Shared UniqueKeyAttrType As Type = GetType(UniqueKeyAttribute)

    Friend Shared Function GetIndexerPropertyName(Type As Type) As String
        Dim defaultAttr As Attribute = Attribute.GetCustomAttribute(Type, GetType(DefaultMemberAttribute))
        Return If(TypeOf defaultAttr Is DefaultMemberAttribute, DirectCast(defaultAttr, DefaultMemberAttribute).MemberName, "")
    End Function

    Friend Shared Function IsCompilerGenerated(propOrFieldInfo As System.Reflection.MemberInfo) As Boolean
        Dim compilerGeneratedAttr As Attribute = Attribute.GetCustomAttribute(propOrFieldInfo, GetType(CompilerGeneratedAttribute))
        Return TypeOf compilerGeneratedAttr Is CompilerGeneratedAttribute
    End Function

	Friend Shared Function GetColumnByCodeName(type As Type, codeColumnName As String) As Databasic.MemberInfo?
		Dim result As Databasic.MemberInfo? = Nothing
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
			result = MetaDescriptor._register(key).ColumnsByDbNames
			MetaDescriptor._lock.ExitUpgradeableReadLock()
		Else
			MetaDescriptor._lock.EnterWriteLock()
			MetaDescriptor._lock.ExitUpgradeableReadLock()
			meta = MetaDescriptor._completeMetaDescription(type)
			MetaDescriptor._register.Add(key, meta)
			result = meta.ColumnsByDbNames
			MetaDescriptor._lock.ExitWriteLock()
		End If
		Return result
	End Function

	Private Shared Function _completeMetaDescription(type As Type) As MetaDescription
        Dim connectionAttr As ConnectionAttribute = DirectCast(Attribute.GetCustomAttribute(type, MetaDescriptor.ConnectionAttrType), ConnectionAttribute)
        Dim tableAttr As TableAttribute = DirectCast(Attribute.GetCustomAttribute(type, MetaDescriptor.TableAttrType), TableAttribute)
        Dim result As New MetaDescription With {
            .ClassType = type,
            .ConnectionIndex = connectionAttr.ConnectionIndex,
            .Tables = tableAttr.Tables
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
                (Not MetaDescriptor.IsCompilerGenerated(prop))
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
                Not MetaDescriptor.IsCompilerGenerated(field)
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
        Dim codeColumnName As String
        Dim dbColumnName As String
        Dim columnAttr As ColumnAttribute
        Dim formatAttr As FormatAttribute
        Dim formatProvider As IFormatProvider
        Dim trimAttr As TrimAttribute
        Dim trimChars As Char()
        Dim keyType As KeyType
        Dim keyName As String
        Dim primaryKeyAttr As PrimaryKeyAttribute
        Dim uniqueKeyAttr As UniqueKeyAttribute
		Dim reflMemberInfo As Reflection.MemberInfo
		result.ColumnsByCodeNames = New Dictionary(Of String, Databasic.MemberInfo)
        result.ColumnsByDbNames = New Dictionary(Of String, Databasic.MemberInfo)
        result.PrimaryColumnsByCodeNames = New Dictionary(Of String, List(Of String))
        result.PrimaryColumnsByDbNames = New Dictionary(Of String, List(Of String))
        result.UniqueColumnsByCodeNames = New Dictionary(Of String, List(Of String))
        result.UniqueColumnsByDbNames = New Dictionary(Of String, List(Of String))
        Dim members As Dictionary(Of String, Databasic.MemberInfo) = MetaDescriptor._completeInstanceDataMembers(type)
        For Each item In members
            reflMemberInfo = item.Value.MemberInfo
            columnAttr = DirectCast(Attribute.GetCustomAttribute(reflMemberInfo, MetaDescriptor.ColumnAttrType), ColumnAttribute)
            formatAttr = DirectCast(Attribute.GetCustomAttribute(reflMemberInfo, MetaDescriptor.FormatAttrType), FormatAttribute)
            trimAttr = DirectCast(Attribute.GetCustomAttribute(reflMemberInfo, MetaDescriptor.TrimAttrType), TrimAttribute)
            primaryKeyAttr = DirectCast(Attribute.GetCustomAttribute(reflMemberInfo, MetaDescriptor.PrimaryKeyAttrType), PrimaryKeyAttribute)
            uniqueKeyAttr = DirectCast(Attribute.GetCustomAttribute(reflMemberInfo, MetaDescriptor.UniqueKeyAttrType), UniqueKeyAttribute)
            codeColumnName = reflMemberInfo.Name
            dbColumnName = codeColumnName
            keyType = KeyType.None
            keyName = ""
            If TypeOf columnAttr Is ColumnAttribute Then
                dbColumnName = If(String.IsNullOrEmpty(columnAttr.ColumnName), codeColumnName, columnAttr.ColumnName)
                If columnAttr.KeyType <> Nothing Then keyType = columnAttr.KeyType
            End If
            If keyType = Nothing AndAlso TypeOf primaryKeyAttr Is PrimaryKeyAttribute Then
                keyType = KeyType.Primary
                If Not String.IsNullOrEmpty(primaryKeyAttr.KeyName) Then keyName = primaryKeyAttr.KeyName
            End If
            If keyType = Nothing AndAlso TypeOf uniqueKeyAttr Is UniqueKeyAttribute Then
                keyType = KeyType.Unique
                If Not String.IsNullOrEmpty(uniqueKeyAttr.KeyName) Then keyName = uniqueKeyAttr.KeyName
            End If
            formatProvider = If(TypeOf formatAttr Is FormatAttribute, formatAttr.FormatProvider, Nothing)
            trimChars = If(TypeOf trimAttr Is TrimAttribute, trimAttr.Chars, New Char() {})
            result.ColumnsByCodeNames.Add(codeColumnName, New Databasic.MemberInfo With {
                .Name = dbColumnName,
                .MemberInfo = reflMemberInfo,
                .Type = item.Value.Type,
                .FormatProvider = formatProvider,
                .TrimChars = trimChars
            })
            result.ColumnsByDbNames.Add(dbColumnName, New Databasic.MemberInfo With {
                .Name = codeColumnName,
                .MemberInfo = reflMemberInfo,
                .FormatProvider = formatProvider,
                .Type = item.Value.Type,
                .TrimChars = trimChars
            })
            If keyType = KeyType.Primary Then
                If result.PrimaryColumnsByCodeNames.ContainsKey(keyName) Then
                    result.PrimaryColumnsByCodeNames(keyName).Add(codeColumnName)
                Else
                    result.PrimaryColumnsByCodeNames.Add(keyName, New List(Of String)() From {codeColumnName})
                End If
                If result.PrimaryColumnsByDbNames.ContainsKey(keyName) Then
                    result.PrimaryColumnsByDbNames(keyName).Add(dbColumnName)
                Else
                    result.PrimaryColumnsByDbNames.Add(keyName, New List(Of String)() From {dbColumnName})
                End If
            End If
            If keyType = KeyType.Unique Then
                If result.UniqueColumnsByCodeNames.ContainsKey(keyName) Then
                    result.UniqueColumnsByCodeNames(keyName).Add(codeColumnName)
                Else
                    result.UniqueColumnsByCodeNames.Add(keyName, New List(Of String)() From {codeColumnName})
                End If
                If result.UniqueColumnsByDbNames.ContainsKey(keyName) Then
                    result.UniqueColumnsByDbNames(keyName).Add(dbColumnName)
                Else
                    result.UniqueColumnsByDbNames.Add(keyName, New List(Of String)() From {dbColumnName})
                End If
            End If
        Next
    End Sub

End Class

