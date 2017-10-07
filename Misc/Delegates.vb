''' <summary>
''' Typed handler for catched Databasic utility exception or sql server error(s).
''' </summary>
''' <param name="ex">Catched Exception In .NET environment.</param>
''' <param name="sqlErrors">Collection of SQL errors sended from SQL server.</param>
Public Delegate Sub ErrorHandler(ex As Exception, sqlErrors As SqlErrorsCollection)