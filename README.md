# DbUp Project Generator

[Yeoman](http://yeoman.io) generator that scaffolds out a [DbUp](https://github.com/DbUp/DbUp) C# console project.

What's in the Template?
The template produces a C# console project which runs a DbUp deployment to a database. 

The console is built so it deploys journaled scripts reside in a folder called "MigrationScripts", and non-journaled scripts reside in the "NonMigrationScripts". Scripts in both folders are assumed to be Embedded Resources in the project, otherwise it will not work (this is an oppotutunity for future work to make this as an option in the generators options).

The console will also create the database if it does not exist through the EnsureDatabase functionality in DbUp.

## How to use it

If you have yo installed, run the following command in the console in the desired folder:
```
yo dbup --databasename YourDatabaseName --projectname TheDesiredNameForConsolePorject
```

If you don't supply these two options the generator will prompt you for them.

## Using The Generated DbUp Project

Once you have generated the project and built it, you can run the migrations by executing exe resulted from the compilation. The exe takes three parameters:
- "connectionstring" alias (c): It takes the name of the connectionstring setting in the config, or the connection string itself.
- "whatif" alias (w): set this parameter to true if you want to see the scripts that are about to be run without running them.
- "logtoconsole" alias (l): the default value is `true`, logs to the console while running the application.