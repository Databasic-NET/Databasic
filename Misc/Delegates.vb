Imports System.Data.Common
''' <summary>
''' Typed handler for catched Databasic utility exception or sql server error(s).
''' </summary>
''' <param name="ex">Catched Exception In .NET environment.</param>
''' <param name="sqlErrors">Collection of SQL errors sended from SQL server.</param>
Public Delegate Sub ErrorHandler(ex As Exception, sqlErrors As SqlErrorsCollection)

Public Delegate Function ItemCompleter(
	reader As DbDataReader
) As Object

Public Delegate Function ItemCompleter(
	Of TValue
)(
	reader As DbDataReader
) As TValue

Public Delegate Function ItemCompleterWithColumns(
	Of TValue
)(
	reader As DbDataReader,
	readerColumns As List(Of String)
) As TValue

Public Delegate Function ItemCompleterWithColumns(
	reader As DbDataReader,
	readerColumns As List(Of String)
) As Object

Public Delegate Function ItemCompleterWithAllInfo(
	Of TValue
)(
	reader As DbDataReader,
	readerColumns As List(Of String),
	columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo)
) As TValue

Public Delegate Function ItemCompleterWithAllInfo(
	reader As DbDataReader,
	readerColumns As List(Of String),
	columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo)
) As Object

Public Delegate Function InstanceCompleter(
	reader As DbDataReader
) As Object

Public Delegate Function InstanceCompleter(
	Of TValue
)(
	reader As DbDataReader
) As TValue

Public Delegate Function InstanceCompleterWithColumns(
	Of TValue
)(
	reader As DbDataReader,
	readerColumns As List(Of String)
) As TValue

Public Delegate Function InstanceCompleterWithColumns(
	reader As DbDataReader,
	readerColumns As List(Of String)
) As Object

Public Delegate Function InstanceCompleterWithAllInfo(
	Of TValue
)(
	reader As DbDataReader,
	readerColumns As List(Of String),
	columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo)
) As TValue

Public Delegate Function InstanceCompleterWithAllInfo(
	reader As DbDataReader,
	readerColumns As List(Of String),
	columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo)
) As Object