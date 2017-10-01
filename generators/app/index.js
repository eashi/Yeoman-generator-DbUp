var Generator = require('yeoman-generator');
var mkdirp = require('mkdirp');

module.exports = class extends Generator {

    constructor(args, opts) {
        super(args, opts);

        this.option("projectname", {type: String, default: "", description: "The name of the project, include namespace."});
        this.option("databasename", {type: String, default: "", description: "The name of the database in the SQL server, i.e. 'initial catalog' in connectionstring."});
        
        this.projectname = this.options.projectname;
        this.localdatabasename = this.options.databasename;


    }

    prompting() {

        var promptingprojectname = this.projectname;
        var promptingdatabasename = this.localdatabasename;

        var logger = this.log;

        return this.prompt([{
            type: 'input',
            name: "projectname",
            message: "Project name?",
            when: function(answers) { 
                return promptingprojectname == null || promptingprojectname == "";
            },
            validate: function (value) {
                if(!value || value == "") {
                    return "This Projec'ts projectname parameter is required, please enter a valid value";
                }
                return true;
            }
        },{
            type: 'input',
            name: "databasename",
            message: "Database name?",
            when: answers => promptingdatabasename == null || promptingdatabasename == "",
            validate: function (value) {
                if(!value || value == "") {
                    return "The Database Name parameter is required, please enter a valid value";
                }
                return true;
            }
        }]).then((answers) => {
            if( answers.projectname ) {
                this.projectname = answers.projectname;
            }
            if( answers.databasename ) {
                this.localdatabasename = answers.databasename;
            }
        });
    }

    writing(){
        this.fs.copyTpl(
            this.templatePath('Project.csproj'),
            this.destinationPath(this.projectname + ".csproj"),
            { projectname: this.projectname}
        );
        this.fs.copyTpl(
            this.templatePath("Program.cs"),
            this.destinationPath("Program.cs"),
            { projectname: this.projectname}
        );
        this.fs.copyTpl(
            this.templatePath("App.config"),
            this.destinationPath("App.config"),
            { databasename: this.localdatabasename}
        );
        this.fs.copyTpl(
            this.templatePath("Options.cs"),
            this.destinationPath("Options.cs"),
            { projectname: this.projectname}
        );
        this.fs.copy(
            this.templatePath("packages.config"),
            this.destinationPath("packages.config")
        );
        mkdirp(this.destinationPath("MigrationScripts"));
        mkdirp(this.destinationPath("NonMigrationScripts"));
    }
};