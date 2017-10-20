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