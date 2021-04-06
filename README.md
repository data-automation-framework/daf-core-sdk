# Data Automation Framework - Core SDK (Daf Core SDK)
**Note: This project is currently in an alpha state and should be considered unstable. Breaking changes to the public API will occur.**

Daf is a plugin-based data and integration automation framework primarily designed to facilitate data warehouse and ETL processes. Developers use this framework to programatically generate data integration objects using the Daf templating language.

This SDK library provides an API for developing Daf plugins. It provides the following
* Attributes to specify root node, limitations and behaviors.
* A parser that creates object structures from an Ion file.
* A plugin interface that the Daf Core library uses to identify the assembly as a plugin.

## Installation
Add a reference to the Daf Core SDK nuget package in your plugin development project.

## Usage
A Daf plugins consist of two conceptual parts:
* A POCO object structure (called the _plugin structrue_) representing the classes that the user will use in the Daf template file.
* The logic which uses the POCO objects to genererate output (e.g. database table definitions).

The parser provided with the SDK takes an Ion file as input and searches it for a root node corresponding to the root node of the plugin structure. It then maps the textual nodes to the plugin structure.
The logic part of the application needs to have a class that implements the IPlugin interface and implements its Execute method. This method should use the IonReader object provided by the parser to get a plugin structure populated with data from the Ion file.

Example:
Ion file for SQL Server specifying one database with a single database schema:
![Ion db sample](https://user-images.githubusercontent.com/1073539/112764075-54fd7680-9007-11eb-8b0c-2d4875e31a83.png)

Plugin structure:
```c#
[IsRootNode]
public class Sql
{
    public List<SqlProject> SqlProjects { get; set; }
}

public class SqlProject
{
    public List<Database> Databases { get; set; }

    public string Name { get; set; }
}

public class Database
{
    public List<Schema> Schemas { get; set; }

    public string Name { get; set; }

    [DefaultValue(TargetSqlServerPlatformEnum.SqlServer)]
    public TargetSqlServerPlatformEnum TargetSqlServerPlatform { get; set; }

    [DefaultValue(TargetSqlServerVersionEnum.SqlServer2019)]
    public TargetSqlServerVersionEnum TargetSqlServerVersion { get; set; }

    [DefaultValue(false)]
    public bool CreateProject { get; set; }
}

public class Schema
{
    public string Name { get; set; }
}
```

IPlugin Execute method implementation: 
```c#
public class SqlPlugin : IPlugin
{
    public int Execute()
    {
        //Instantiate IonReader from the Daf SDK
        IonReader<Sql> sqlProjectParser = new(pathToIonFile, typeof(Sql).Assembly);

        //Verify that the provided Ion file has a matching root node
        if (sqlProjectParser.RootNodeExistInFile())
        {
            //Get the populate plugin structure
            Sql sqlRootNode = sqlProjectParser.Parse();

            //Execute program logic
            foreach (SqlProject sqlProject in sqlRootNode.SqlProjects)
            {
                SqlGenerator.DefineSQL(sqlProject);
            }
        }
        
        return 0;
    }
}
```

See the standard Daf plugins for SQL Server, SSIS and ADF in the links section for examples.

## Links
[Daf organization](https://github.com/data-automation-framework)
[Documentation](https://data-automation-framework.com)

Standard plugins:
[SQL Server](https://github.com/data-automation-framework/daf-core-sql)
[SSIS](https://github.com/data-automation-framework/daf-core-ssis)
[Azure Data Factory](https://github.com/data-automation-framework/daf-core-adf)
