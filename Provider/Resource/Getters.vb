Namespace Provider
    Partial Public Class Resource
        Inherits ActiveRecord.Resource

        Public Overridable Function GetTableColumns(connection As Databasic.Connection, table As String) As List(Of String)
            Return New List(Of String)
        End Function



        ''' <summary>
        ''' Get Databasic command instance to load single instance by unique identifier column, all table columns will be loaded by * SQL operator.
        ''' </summary>
        ''' <param name="connection">Database connection instance.</param>
        ''' <param name="table">Database table name.</param>
        ''' <param name="uniqueColumnName">Database table unique identifier column name.</param>
        ''' <param name="uniqueColumnValue">Identifier value.</param>
        ''' <returns></returns>
        Public Overridable Function GetById(
            connection As Databasic.Connection,
            table As String,
            Optional columns As String = "*",
            Optional uniqueColumnName As String = Database.DEFAUT_UNIQUE_COLUMN_NAME,
            Optional uniqueColumnValue As Object = Nothing
        ) As Statement
            Return Databasic.Statement.Prepare(
                $"SELECT {columns} FROM {table} WHERE {uniqueColumnName} = @uniqueParamValue", connection
            ).FetchAll(New With {.uniqueParamValue = uniqueColumnValue})
        End Function


        Public Overridable Function GetLastInsertedId(transaction As Databasic.Transaction) As Object
            Dim resource As Provider.Resource = Activator.CreateInstance(transaction.ConnectionWrapper.ResourceType)
            Return resource.GetLastInsertedId(transaction)
        End Function





        Public Overridable Function GetCount(connection As Databasic.Connection, table As String, Optional countColumn As String = "*") As Int64
            Return Databasic.Statement.Prepare($"SELECT COUNT({countColumn}) FROM  {table}", connection).FetchAll().ToValue(Of Int64)()
        End Function




        Public Overridable Function GetAll(
            connection As Databasic.Connection,
            table As String,
            columns As String,
            Optional offset As Int64? = Nothing,
            Optional limit As Int64? = Nothing,
            Optional orderByStatement As String = Database.DEFAUT_UNIQUE_COLUMN_NAME
        ) As Databasic.Statement
            Return Nothing
        End Function

    End Class
End Namespace