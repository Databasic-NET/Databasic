Public Structure KeyColumns
	Public Type As KeyType
	Public Columns As Dictionary(Of String, String)
	Public AutoIncrementColumn As AutoIncrementColumn?
End Structure

Friend Structure AllKeyColumns
	Friend PrimaryColumns As Dictionary(Of String, Dictionary(Of String, String))
	Friend UniqueColumns As Dictionary(Of String, Dictionary(Of String, String))
	Friend AutoIncrementColumn As AutoIncrementColumn?
End Structure

Public Structure AutoIncrementColumn
	Public DatabaseColumnName As String
	Public CodeColumnName As String
End Structure

Public Structure MetaDescription
	Public ClassType As Type
	Public ConnectionIndex As Int32
	Public Tables As String()
	Public ColumnsByDatabaseNames As Dictionary(Of String, Databasic.MemberInfo)
	Public ColumnsByCodeNames As Dictionary(Of String, Databasic.MemberInfo)
	Public PrimaryColumns As Dictionary(Of String, Dictionary(Of String, String))
	Public UniqueColumns As Dictionary(Of String, Dictionary(Of String, String))
	Public AutoIncrementColumn As AutoIncrementColumn?
End Structure

Public Structure MemberInfo
	Public Value As Object
	Public Type As Type
	Public Name As String
	Public MemberInfo As System.Reflection.MemberInfo
	Public MemberInfoType As MemberInfoType
	Public FormatProvider As IFormatProvider
	Public TrimChars As Char()
End Structure

Public Structure Defaults
	Public Const CONNECTION_INDEX As Int32 = 0
	Public Const CONNECTION_NAME As String = "DefaultConnection"
	Public Const KEY_NAME As String = "default"
End Structure

Friend Structure Constants
	''' <summary>
	''' String type code value, used frequently in Entity._readerRowToInstance.
	''' </summary>
	Friend Shared ReadOnly StringTypeCode As TypeCode = Type.GetTypeCode(GetType(String))

	Friend Shared ConnectionAttrType As Type = GetType(ConnectionAttribute)
	Friend Shared TableAttrType As Type = GetType(TableAttribute)
	Friend Shared ColumnAttrType As Type = GetType(ColumnAttribute)
	Friend Shared FormatAttrType As Type = GetType(FormatAttribute)
	Friend Shared TrimAttrType As Type = GetType(TrimAttribute)
	Friend Shared PrimaryKeyAttrType As Type = GetType(PrimaryKeyAttribute)
	Friend Shared UniqueKeyAttrType As Type = GetType(UniqueKeyAttribute)
	Friend Shared AutoIncrementAttrType As Type = GetType(AutoIncrementAttribute)
End Structure

Friend Structure ProviderNames
	Friend Shared Values As Dictionary(Of ProviderName, String) = New Dictionary(Of ProviderName, String) From {
		{ProviderName.MsSql, "System.Data.SqlClient"},
		{ProviderName.MySql, "MySql.Data.MySqlClient"},
		{ProviderName.OdbcSql, "System.Data.Odbc"},
		{ProviderName.OleSql, "System.Data.OleDb"},
		{ProviderName.OracleSql, "Oracle.DataAccess.Client"},
		{ProviderName.PostgreSQL, "Npgsql"},
		{ProviderName.SQLite, "System.Data.SQLite"}
	}
End Structure