Imports System.ComponentModel
Imports System.Data.Common
Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient

Public Class Command







    ''' <summary>
    ''' Current command provider type.
    ''' </summary>
    Protected Enum CmdType
        MsSql
        MySql
        'Oracle
        'Postgre
        '...
    End Enum
    ''' <summary>
    ''' Currently prepared and executed Sql command.
    ''' </summary>
    Protected cmd As DbCommand
    ''' <summary>
    ''' Currently prepared and executed Sql command type by database provider.
    ''' </summary>
    Protected type As CmdType
    ''' <summary>
    ''' Currently executed reader from command.
    ''' </summary>
    Protected reader As DbDataReader








    Public Sub New(connection As DbConnection)
        Dim connType As Type = connection.GetType()
        If connType.IsAssignableFrom(GetType(SqlConnection)) Then
            Me.type = CmdType.MsSql
        ElseIf connType.IsAssignableFrom(GetType(MySqlConnection)) Then
            Me.type = CmdType.MySql
        End If
    End Sub
    Public Sub New(sql As String, connection As DbConnection)
        Me.New(connection)
        If Me.type = CmdType.MsSql Then
            Me.cmd = New SqlCommand(sql, connection)
        ElseIf Me.type = CmdType.MySql Then
            Me.cmd = New MySqlCommand(sql, connection)
        End If
        Me.cmd.Prepare()
    End Sub
    Public Sub New(sql As String, connection As DbConnection, transaction As DbTransaction)
        Me.New(connection)
        If Me.type = CmdType.MsSql Then
            Me.cmd = New SqlCommand(sql, connection, transaction)
        ElseIf Me.type = CmdType.MySql Then
            Me.cmd = New MySqlCommand(sql, connection, transaction)
        End If
        Me.cmd.Prepare()
    End Sub









    ''' <summary>
    ''' Prepare database SQL statement. All params name with '@' char at the beginning of param name.
    ''' </summary>
    ''' <param name="sql">SQL statement.</param>
    ''' <returns>Command instance.</returns>
    Public Shared Function Prepare(sql As String) As Command
        Return New Command(sql, Connection.Get(0))
    End Function
    ''' <summary>
    ''' Prepare database SQL statement. All params name with '@' char at the beginning of param name.
    ''' </summary>
    ''' <param name="sql">SQL statement.</param>
    ''' <param name="connectionIndex">Config connection index to use different database, default by 0 to use first connection in &lt;connectionStrings&gt; list.</param>
    ''' <returns>Command instance.</returns>
    Public Shared Function Prepare(sql As String, Optional connectionIndex As Int32 = 0) As Command
        Return New Command(sql, Connection.Get(connectionIndex))
    End Function
    ''' <summary>
    ''' Prepare database SQL statement. All params name with '@' char at the beginning of param name.
    ''' </summary>
    ''' <param name="sql">SQL statement.</param>
    ''' <param name="connectionName">Config connection index to use different database, default by 'DefaultConnection' to use default connection in &lt;connectionStrings&gt; list.</param>
    ''' <returns>Command instance.</returns>
    Public Shared Function Prepare(sql As String, Optional connectionName As String = "DefaultConnection") As Command
        Return New Command(sql, Connection.Get(connectionName))
    End Function

    Public Shared Function Prepare(sql As String, Optional connection As DbConnection = Nothing) As Command
        Return New Command(sql, connection)
    End Function
    ''' <summary>
    ''' Prepare database SQL statement. All params name with '@' char at the beginning of param name.
    ''' </summary>
    ''' <param name="sql">SQL statement.</param>
    ''' <param name="transaction">Database connection transaction to execute this SQL command in.</param>
    ''' <returns>Command instance.</returns>
    Public Shared Function Prepare(sql As String, Optional transaction As DbTransaction = Nothing) As Command
        Return New Command(sql, Connection.Get(0), transaction)
    End Function
    ''' <summary>
    ''' Prepare database SQL statement. All params name with '@' char at the beginning of param name.
    ''' </summary>
    ''' <param name="sql">SQL statement.</param>
    ''' <param name="transaction">Database connection transaction to execute this SQL command in.</param>
    ''' <param name="connectionIndex">Database connection config index.</param>
    ''' <returns>Command instance.</returns>
    Public Shared Function Prepare(sql As String, transaction As SqlTransaction, connectionIndex As Int32) As Command
        Return New Command(sql, Connection.Get(connectionIndex), transaction)
    End Function
    ''' <summary>
    ''' Prepare database SQL statement. All params name with '@' char at the beginning of param name.
    ''' </summary>
    ''' <param name="sql">SQL statement.</param>
    ''' <param name="transaction">Database connection transaction to execute this SQL command in.</param>
    ''' <param name="connectionName">Database connection config name.</param>
    ''' <returns>Command instance.</returns>
    Public Shared Function Prepare(sql As String, transaction As SqlTransaction, connectionName As String) As Command
        Return New Command(sql, Connection.Get(connectionName), transaction)
    End Function
    ''' <summary>
    ''' Prepare database SQL statement. All params name with '@' char at the beginning of param name.
    ''' </summary>
    ''' <param name="sql">SQL statement.</param>
    ''' <param name="transaction">Database connection transaction to execute this SQL command in.</param>
    ''' <param name="connection">Database connection to use..</param>
    ''' <returns>Command instance.</returns>
    Public Shared Function Prepare(sql As String, Optional transaction As DbTransaction = Nothing, Optional connection As DbConnection = Nothing) As Command
        Return New Command(sql, connection, transaction)
    End Function










    ''' <summary>
    ''' Get first row from select result.
    ''' </summary>
    ''' <returns>Opened database reader.</returns>
    Public Function FetchOne() As Command
        Return Me.FetchOne(CommandBehavior.SingleRow)
    End Function
    ''' <summary>
    ''' Get first row from select result.
    ''' </summary>
    ''' <param name="sqlParams">Anonymous object with named values as SQL statement params.</param>
    ''' <returns>Opened database reader.</returns>
    Public Function FetchOne(sqlParams As Object) As Command
        Command.addParamsWithValue(Me.type, Me.cmd, sqlParams)
        Me.reader = Me.cmd.ExecuteReader(CommandBehavior.SingleRow)
        Return Me
    End Function
    ''' <summary>
    ''' Get first row from select result.
    ''' </summary>
    ''' <param name="sqlParams">Dictionary with named values as SQL statement params, do not use any '@' chars for dictionary keys.</param>
    ''' <returns>Opened database reader.</returns>
    Public Function FetchOne(sqlParams As Dictionary(Of String, Object)) As Command
        Command.addParamsWithValue(Me.type, Me.cmd, sqlParams)
        Me.reader = cmd.ExecuteReader(CommandBehavior.SingleRow)
        Return Me
    End Function
    ''' <summary>
    ''' Get all rows from select result.
    ''' </summary>
    ''' <returns>Opened database reader.</returns>
    Public Function FetchAll() As Command
        Return Me.FetchAll(CommandBehavior.Default)
    End Function
    ''' <summary>
    ''' Get all rows from select result.
    ''' </summary>
    ''' <param name="commandBehavior">Database reader command behaviour.</param>
    ''' <returns>Opened database reader.</returns>
    Public Function FetchAll(Optional ByVal commandBehavior As CommandBehavior = CommandBehavior.Default) As Command
        Me.reader = Me.cmd.ExecuteReader(commandBehavior)
        Return Me
    End Function
    ''' <summary>
    ''' Get all rows from select result.
    ''' </summary>
    ''' <param name="sqlParams">Anonymous object with named values as SQL statement params.</param>
    ''' <param name="commandBehavior">Database reader command behaviour.</param>
    ''' <returns>Opened database reader.</returns>
    Public Function FetchAll(sqlParams As Object, Optional commandBehavior As CommandBehavior = CommandBehavior.Default) As Command
        Command.addParamsWithValue(Me.type, Me.cmd, sqlParams)
        Me.reader = Me.cmd.ExecuteReader(commandBehavior)
        Return Me
    End Function
    ''' <summary>
    ''' Get all rows from select result.
    ''' </summary>
    ''' <param name="sqlParams">Dictionary with named values as SQL statement params, do not use any '@' chars for dictionary keys.</param>
    ''' <param name="commandBehavior">Database reader command behaviour.</param>
    ''' <returns>Opened database reader.</returns>
    Public Function FetchAll(sqlParams As Dictionary(Of String, Object), Optional commandBehavior As CommandBehavior = CommandBehavior.Default) As Command
        Command.addParamsWithValue(Me.type, Me.cmd, sqlParams)
        Me.reader = cmd.ExecuteReader(commandBehavior)
        Return Me
    End Function










    ''' <summary>
    ''' Get currently used database reader.
    ''' </summary>
    ''' <returns>Currently used database reader.</returns>
    Public Function GetReader() As DbDataReader
        Return Me.reader
    End Function









    ''' <summary>
    ''' Convert single row and single column select result into desired type specified by generic argument.
    ''' </summary>
    ''' <typeparam name="ResultType">Result variable type.</typeparam>
    ''' <returns>Retyped single row and single column select result.</returns>
    Public Function ToValue(Of ResultType)() As ResultType
        Dim result As ResultType
        If reader.HasRows() Then
            reader.Read()
            result = Convert.ChangeType(reader.Item(0), GetType(ResultType))
        Else
            result = Nothing
        End If
        reader.Close()
        Return result
    End Function
    ''' <summary>
    ''' Create new instance by generic type and set up all called dictionary keys into new instance properties or fields.
    ''' </summary>
    ''' <typeparam name="InstanceResultType">New instance type.</typeparam>
    ''' <returns>New instance by generic type with values by generic param.</returns>
    Public Function ToInstance(Of InstanceResultType)() As InstanceResultType
        Return Model.ToInstance(Of InstanceResultType)(Me.reader)
    End Function
    ''' <summary>
    ''' Create new List with instances by generic type and set up all called reader columns into new instances properties or fields.
    ''' If reader has no rows, empty list is returned.
    ''' </summary>
    ''' <typeparam name="ItemInstanceResultType">Result list item generic type.</typeparam>
    ''' <returns></returns>
    Public Function ToList(Of ItemInstanceResultType)() As List(Of ItemInstanceResultType)
        Return Model.ToList(Of ItemInstanceResultType)(Me.reader)
    End Function
    ''' <summary>
    ''' Create new Dictionary with keys by first generic type and instances (values) by second generic type 
    ''' and set up all select result columns into new instances properties or fields. By first param as string,
    ''' specify which column from select result to use to complete dictionary keys.
    ''' If select result has no rows, empty dictionary is returned.
    ''' </summary>
    ''' <typeparam name="ItemKeyType">Result dictionary generic type to complete dictionary keys.</typeparam>
    ''' <typeparam name="ItemInstanceResultType">Result dictionary generic type to complete dictionary values.</typeparam>
    ''' <param name="keyColumnName">Reader column name to use to complete result dictionary keys.</param>
    ''' <param name="throwExceptionInDuplicateKey">True to thrown Exception if any previous key will be founded by filling the result, false to overwrite any previous value.</param>
    ''' <returns></returns>
    Public Function ToDictionary(Of ItemKeyType, ItemInstanceResultType)(Optional keyColumnName As String = "Id", Optional throwExceptionInDuplicateKey As Boolean = True) As Dictionary(Of ItemKeyType, ItemInstanceResultType)
        Return Model.ToDictionary(Of ItemKeyType, ItemInstanceResultType)(Me.reader, keyColumnName, throwExceptionInDuplicateKey)
    End Function









    ''' <summary>
    ''' Execute any non select SQL statement and return affected rows count.
    ''' </summary>
    ''' <returns>Affected rows count</returns>
    Public Function Exec() As Int32
        Return Me.cmd.ExecuteNonQuery()
    End Function
    ''' <summary>
    ''' Execute any non select SQL statement and return affected rows count.
    ''' </summary>
    ''' <param name="sqlParams">Anonymous object with named values as SQL statement params.</param>
    ''' <returns>Affected rows count</returns>
    Public Function Exec(sqlParams As Object) As Int32
        Command.addParamsWithValue(Me.type, Me.cmd, sqlParams)
        Return Me.Exec()
    End Function
    ''' <summary>
    ''' Execute any non select SQL statement and return affected rows count.
    ''' </summary>
    ''' <param name="sqlParams">Dictionary with named values as SQL statement params, do not use any '@' chars for dictionary keys.</param>
    ''' <returns>Affected rows count</returns>
    Public Function Exec(sqlParams As Dictionary(Of String, Object)) As Int32
        Command.addParamsWithValue(Me.type, Me.cmd, sqlParams)
        Return Me.cmd.ExecuteNonQuery()
    End Function









    Protected Shared Sub addParamsWithValue(type As CmdType, cmd As DbCommand, params As Object)
        If type = CmdType.MsSql Then
            Command.addParamsWithValue(DirectCast(cmd, SqlCommand), params)
        ElseIf type = CmdType.MySql Then
            Command.addParamsWithValue(DirectCast(cmd, MySqlCommand), params)
        End If
    End Sub
    Protected Shared Sub addParamsWithValue(type As CmdType, cmd As DbCommand, params As Dictionary(Of String, Object))
        If type = CmdType.MsSql Then
            Command.addParamsWithValue(DirectCast(cmd, SqlCommand), params)
        ElseIf type = CmdType.MySql Then
            Command.addParamsWithValue(DirectCast(cmd, MySqlCommand), params)
        End If
    End Sub

    Protected Shared Sub addParamsWithValue(cmd As SqlCommand, params As Object)
        If (Not params Is Nothing) Then
            Dim sqlParamValue As Object
            For Each prop As PropertyDescriptor In TypeDescriptor.GetProperties(params)
                sqlParamValue = prop.GetValue(params)
                cmd.Parameters.AddWithValue(
                    prop.Name,
                    If((sqlParamValue Is Nothing), DBNull.Value, sqlParamValue)
                )
            Next
        End If
    End Sub
    Protected Shared Sub addParamsWithValue(cmd As MySqlCommand, params As Object)
        If (Not params Is Nothing) Then
            Dim sqlParamValue As Object
            For Each prop As PropertyDescriptor In TypeDescriptor.GetProperties(params)
                sqlParamValue = prop.GetValue(params)
                cmd.Parameters.AddWithValue(
                    prop.Name,
                    If((sqlParamValue Is Nothing), DBNull.Value, sqlParamValue)
                )
            Next
        End If
    End Sub
    Protected Shared Sub addParamsWithValue(cmd As SqlCommand, params As Dictionary(Of String, Object))
        If (Not params Is Nothing) Then
            Dim pair As KeyValuePair(Of String, Object)
            For Each pair In params
                cmd.Parameters.AddWithValue(
                    pair.Key,
                    If((pair.Value Is Nothing), DBNull.Value, pair.Value)
                )
            Next
        End If
    End Sub
    Protected Shared Sub addParamsWithValue(cmd As MySqlCommand, params As Dictionary(Of String, Object))
        If (Not params Is Nothing) Then
            Dim pair As KeyValuePair(Of String, Object)
            For Each pair In params
                cmd.Parameters.AddWithValue(
                    pair.Key,
                    If((pair.Value Is Nothing), DBNull.Value, pair.Value)
                )
            Next
        End If
    End Sub





End Class

