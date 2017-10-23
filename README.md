# Databasic - Core Package

[![Latest Stable Version](https://img.shields.io/badge/Stable-v1.2.5-brightgreen.svg?style=plastic)](https://github.com/databasic-net/databasic-core/releases)
[![License](https://img.shields.io/badge/Licence-BSD3-brightgreen.svg?style=plastic)](https://raw.githubusercontent.com/databasic-net/databasic-core/master/LICENCE.md)
![.NET Version](https://img.shields.io/badge/.NET->=4.0-brightgreen.svg?style=plastic)

 Databasic - C#/VB.NET database utility
 - focusing on queries primarily based on pure SQL commands, no linq transations
- arranging data into primitive types, typed active record classes, collections and more
- allowing to run any nonselect queries

## Features
- choosing connection by config index or name as another param of Databasic.Statement.Prepare()
- passing transactions as second param of Databasic.Statement.Prepare()
- any non-select SQL statements
- Microsoft SQL Server/MySQL/MariaDB/PostgreSQL/Oracle/ODBC/OLEDB support
- selection single value (for example counts etc...)
- getting only touched (changed) values
- saving and deleting active record instances (insert/update/delete by primary key and autoincrement column)
- much more...

## Instalation
**This is core package only**. Install this package only by installing specific database package like: Databasic.&#60;DatabaseType&#62;. Then this package will be installed automaticly with the specific database package.
```nuget
PM> Install-Package Databasic.DatabaseType
```
### Databasic packages:
- `Databasic.MsSql`
- `Databasic.MySql`
- `Databasic.PostgreSql`
- `Databasic.OracleSql`
- `Databasic.SQLite`
- `Databasic.OdbcSql`
- `Databasic.OleSql`

## Basic Examples
```vb
Imports Databasic

' create active record class extending Databasic.ActiveRecord.Entity:
<Connection("ConfigConnectionName")>
Public Class Person
    <PrimaryKey, AutoIncrement>
    Public Id As Int32?
    Public Property Firstname As String
    Public Property Secondname As String
End Class

' load person with id 5 and create it's instance:
Dim instance As Person = Statement.Prepare(
    "SELECT * FROM Persons WHERE Id = @id"
).FetchOne(New With {
    .id = 5
}).ToInstance(Of Person)()

' load all persons with id higher than 5 into list
Dim list As List(Of Person) = Statement.Prepare(
    "SELECT * FROM Persons WHERE Id > @id"
).FetchAll(New With {
    .id = 5
}).ToList(Of Person)()

' load all persons with id higher than 5 into dictionary
' and complete dictionary keys by Id column
Dim dct As Dictionary(Of Int32, Person) = Statement.Prepare(
    "SELECT * FROM Persons WHERE Id > @id"
).FetchAll(New With {
    .id = 5
}).ToDictionary(Of Int32, Person)("Id")
```

```cs
using Databasic;

// create active record class extending Databasic.ActiveRecord.Entity:
[Connection("ConfigConnectionName")]
public class Person {
    [PrimaryKey, AutoIncrement]
    public int? Id;
    public string Firstname { get; set; }
    public string Secondname { get; set; }
}

// load person with id 5 and create it's instance:
Person instance = Statement.Prepare(
    "SELECT * FROM Persons WHERE Id = @id"
).FetchOne(new {
    id = 5
}).ToInstance<Person>()

// load all persons with id higher than 5 into list
List<Person> list = Statement.Prepare(
    "SELECT * FROM Persons WHERE Id > @id"
).FetchAll(new {
    id = 5
}).ToList<Person>()

// load all persons with id higher than 5 into dictionary
// and complete dictionary keys by Id column
Dictionary<Int32, Person> dct = Statement.Prepare(
    "SELECT * FROM Persons WHERE Id > @id"
).FetchAll(new {
    id = 5
}).ToDictionary<Int32, Person>("Id")
```

## Advanced Examples
```cs
using Databasic;
using System;
using System.Collections.Generic;

[Connection("DefaultConnection"),Table("Dealers")]
class Dealer: Person {
	[PrimaryKey, AutoIncrement]
	public int? Id { get; set; }
	[Column("Firstname"), Trim]
	public string FirstName { get; set; }
	[Column("Secondname")]
	public string SecondName { get; set; }
	public double? TurnOver { get; set; }

	public static Dealer GetById (int id) {
		return Statement.Prepare(
			$"SELECT {Columns()} FROM {Table()} WHERE Id = @idParam"
		).FetchOne(new {
			idParam = id
		}).ToInstance<Dealer>();
	}
	public static int GetCount () {
		return Statement.Prepare(
			$"SELECT COUNT(Id) FROM {Table()}"
		).FetchOne().ToInstance<Int32>();
	}
	public static Dictionary<int, Dealer> GetDictionary (Func<Dealer, int> keySelector = null) {
		return Statement.Prepare(
			$"SELECT {Columns()} FROM {Table()}"
		).FetchAll().ToDictionary<int, Dealer>(
			keySelector ?? (d => d.Id.Value)
		);
	}
	public static List<Dealer> GetList () {
		return Statement.Prepare(
			$"SELECT {Columns()} FROM {Table()}"
		).FetchAll().ToList<Dealer>();
	}
}
...
Dictionary<int, Dealer> dct = Dealer.GetDictionary();

List<Dealer> list = Dealer.GetList();

Dealer dealer1 = Dealer.GetById(3);

int cnt = Dealer.GetCount();
```
