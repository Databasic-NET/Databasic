# Databasic - Core Package

 Databasic - C#/VB.NET database utility
 - focusing on queries primarily based on pure SQL commands, no linq transations
- arranging data into primitive types, typed active record classes, collections and more
- allowing to run any nonselect queries

**This is core package!**
Install this package only with specific database package like: 
- Databasic.&#60;DatabaseType&#62;

// TODO:
## Basic Examples
```vb
' create active record class extending DbSharp.Model:
Public Class Dealer
    Inherits DbSharp.Model
    Public Id As Int32?
    Public Property Firstname As String
    Public Property Secondname As String
End Class

' load dealer with id 5 and create it's instance:
Dim instance As Dealer = Command.Prepare(
    "SELECT * FROM Dealers WHERE Id = @id"
).FetchOne(New With {
    .id = 5
}).ToInstance(Of Dealer)()

' load all dealers with id higher than 5 into list
Dim list As List(Of Dealer) = Command.Prepare(
    "SELECT * FROM Dealers WHERE Id > @id"
).FetchAll(New With {
    .id = 5
}).ToList(Of Dealer)()

' load all dealers with id higher than 5 into dictionary
' and complete dictionary keys by Id column
Dim dct As Dictionary(Of Int32, Dealer) = Command.Prepare(
    "SELECT * FROM Dealers WHERE Id > @id"
).FetchAll(New With {
    .id = 5
}).ToDictionary(Of Int32, Dealer)("Id")
```

```cs
// create active record class extending DbSharp.Model:
public class Dealer: DbSharp.Model {
    public int? Id;
    public string Firstname { get; set; }
    public string Secondname { get; set; }
}

// load dealer with id 5 and create it's instance:
Dealer instance = Command.Prepare(
    "SELECT * FROM Dealers WHERE Id = @id"
).FetchOne(new {
    id = 5
}).ToInstance<Dealer>()

// load all dealers with id higher than 5 into list
List<Dealer> list = Command.Prepare(
    "SELECT * FROM Dealers WHERE Id > @id"
).FetchAll(new {
    id = 5
}).ToList<Dealer>()

// load all dealers with id higher than 5 into dictionary
// and complete dictionary keys by Id column
Dictionary<Int32, Dealer> dct = Command.Prepare(
    "SELECT * FROM Dealers WHERE Id > @id"
).FetchAll(new {
    id = 5
}).ToDictionary<Int32, Dealer>("Id")
```

## Features
- choosing connection by config index or name as another param of Command.Prepare()
- passing transactions as second param of Command.Prepare()
- any non-select SQL statements
- Microsoft SQL Server and MySQL server support
- selection single value (for example counts etc...)
- much more
