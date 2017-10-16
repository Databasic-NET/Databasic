<Flags>
Public Enum CommandBehavior
	CloseConnection = &H20
	[Default] = 0
	KeyInfo = 4
	SequentialAccess = &H10
	SchemaOnly = 2
	SingleResult = 1
	SingleRow = 8
End Enum

Public Enum IsolationLevel
	Chaos = &H10
	ReadCommitted = &H1000
	ReadUncommitted = &H100
	RepeatableRead = &H10000
	Serializable = &H100000
	Snapshot = &H1000000
	Unspecified = -1
End Enum

Public Enum KeyType
	None
	Primary
	Unique
End Enum

Public Enum DuplicateKeyBehaviour
	ThrownException
	KeepFirstValue
	OverwriteByNewValue
End Enum

Public Enum MemberInfoType
	None
	Prop
	Field
End Enum

Public Enum ProviderName
	''' <summary>
	''' Microsoft SQL server - System.Data.SqlClient
	''' </summary>
	MsSql
	''' <summary>
	''' MariaDB/MySQL server - MySql.Data.MySqlClient
	''' </summary>
	MySql
	''' <summary>
	''' ODBC server - System.Data.Odbc
	''' </summary>
	OdbcSql
	''' <summary>
	''' ODBC server - System.Data.Odbc
	''' </summary>
	OleSql
	''' <summary>
	''' OLE database or server - Oracle.DataAccess.Client
	''' </summary>
	OracleSql
	''' <summary>
	''' PostreSql server - Npgsql
	''' </summary>
	PostgreSQL
	''' <summary>
	''' SQLite database - System.Data.SQLite
	''' </summary>
	SQLite
End Enum
