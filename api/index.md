# Pandv.DataAccess

Author: Victor.X.Qu

Email: fs7744@hotmail.com

Pandv.DataAccess is just config sql in xml files, base on dapper

DataAccess is for net core 

## db supports
DataAccess base on ado.net, so you can use blow db :

* [Pandv.DataAccess](https://www.nuget.org/packages/Pandv.DataAccess)

### use MSSql example 

#### Use config file

##### dependencies

``` xml
    <PackageReference Include="Pandv.DataAccess" Version="0.0.1" />
```

You can config sql in xml file for DataAcces, like:

``` xml
<?xml version="1.0" encoding="utf-8"?>
<DbConfig xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <ConnectionStrings>
    <DataConnection Name="Test" ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestDataAccess;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" />
  </ConnectionStrings>
  <SqlConfigs>
    <DbSql CommandName="SelectByName" Type="Text" ConnectionName="Test">
      <Text>
        <![CDATA[
SELECT top 1
    Id
    ,Age
    ,Name
    ,JoinDate
    ,[Money]
FROM [dbo].[Students] WITH(NOLOCK)
WHERE @Name = Name
      ]]>
      </Text>
    </DbSql>
    <DbSql CommandName="SelectAll" Type="Text" ConnectionName="Test">
      <Text>
        <![CDATA[
SELECT
    Id
    ,Age
    ,Name
    ,JoinDate
    ,[Money]
FROM [dbo].[Students] WITH(NOLOCK)
      ]]>
      </Text>
    </DbSql>
    <DbSql CommandName="SelectAllAge" Type="Text" ConnectionName="Test">
      <Text>
        <![CDATA[
SELECT
    sum(Age) as Age
FROM [dbo].[Students] WITH(NOLOCK)
      ]]>
      </Text>
    </DbSql>
    <DbSql CommandName="Clear" Type="Text" ConnectionName="Test">
      <Text>
        <![CDATA[
delete from [dbo].[Students]
      ]]>
      </Text>
    </DbSql>
    <DbSql CommandName="BulkCopy" Type="Text" ConnectionName="Test">
      <Text>
        <![CDATA[
[dbo].[Students]
      ]]>
      </Text>
    </DbSql>
  </SqlConfigs>
</DbConfig>
```

Code for use :

``` csharp
var provider = new ServiceCollection()
                     .UseSqlServer()
                     .UseDataAccessConfig(Directory.GetCurrentDirectory(), false, null, "db.xml")
                     .BuildServiceProvider();

List<Student> students = GenerateStudents(count);

var db = provider.GetService<IDbProvider>();        

db.ExecuteBulkCopy("BulkCopy",students);

var student = db.QueryFirstOrDefault<Student>("SelectByName", new { Name = new DbString() { Value = "3", IsAnsi = true } });
Assert.Equal(3, student.Age);

var students = db.Query<Student>("SelectAll").ToList();
Assert.Equal(500, students.Count);

```
