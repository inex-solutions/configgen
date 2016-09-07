**NOTE: ConfigGen v2 is still in beta.**

This version of ConfigGen is a re-write of the original [ConfigGen](https://configgen.codeplex.com/).

For the support status of ConfigGen v1 features, please see [Supported v1 features](https://github.com/inex-solutions/configgen/wiki/configgen-v1-compatibility).

##ConfigGen 

Welcome to ConfigGen - primarily a configuration file generator, but also a general templating engine.

##Overview (or "why do I need ConfigGen?")

ConfigGen's main aim is to relieve you from the tyranny of managing configurations files for all your environments and machines. 

Rather than having to manually maintain copies of files for each workstation, server, or environment, ConfigGen reduces the problem to a single template (the core of your configuration file) together with a configuration spreadsheet that contains the individual settings for each machine or environment.

At it simplest, the template file is your existing configuration file where the values that vary between machines are replaced with tokens, and the spreadsheet is a tabulated view of the value of these tokens for each machine. ConfigGen also allows the structure of the configuration file to vary between environments. 

So why do you need it? Here are a few reasons:

* You can see all the different configuration settings for all your environments and machines tabulated in a single spreadsheet
* When your configuration file is changed, it only needs to be changed in one place.
* The tabulated view makes it easy to spot missing configuration data for certain machines: no more deploying to UAT, only to find your app fails due to a config change that was applied to your dev boxes only.
ConfigGen warns if any inconsistencies are found between the template file and the settings spreadsheet.

## How it works
To generate a configuration file, you simply execute ConfigGen, supplying both a template file and a configuration settings file. ConfigGen then generates configuration files for all the specified environments or machines.

### Template file
The template file forms the basis of the files to be generated.

Template files can be written as either xml, or razor. More information can be found [here](https://github.com/inex-solutions/configgen/wiki/template-files).

Below is a simple example of an xml template file:

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <appSettings>
        <add key="Environment" value="[%Environment%]" />
        <add key="LogLevel" value="[%LogLevel%]" />
      </appSettings>
    </configuration>


### Settings spreadsheet 
The configuration settings file contains the values for any tokens specified in the template. This is typically a spreadsheet or csv file, although a more sophisticated hierarchical xml format is also supported. 

For more information on settings file formats, see [here](https://github.com/inex-solutions/configgen/wiki/settings-files).

Below is a simple example of a settings spreadsheet file:

| MachineName   | Environment | LogLevel |
| ---           | ---         | ---      |
| DevServer     | DEV         | DEBUG    |
| UatServer     | UAT         | INFO     |
| ProdServer    | PROD        | INFO     |
| | |
| MyWorkstation | DEV         | DEBUG    |
| | |
| Default       | DEV         | DEBUG    |

### Running ConfigGen
By saving the sample template file as an xml file, and configuration settings file above as an excel spreadsheet, you can invoke ConfigGen with

    cfg.exe --template-file mytemplatefile.xml --settings-file mysettingsfile.xls

This will generate 5 configuration files (one for each environment specified in the spreadsheet), each in their own sub-directory within a top-level directory named `Configs`.

For example, the ProdServer configuration file will contain:

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <appSettings>
        <add key="Environment" value="ProdServer" />
        <add key="LogLevel" value="INFO" />
      </appSettings>
    </configuration>

## More Information
ConfigGen allows templates to be authored in [several different file formats] (https://github.com/inex-solutions/configgen/wiki/template-files). More advanced examples of these support changes to the structure of your configuration files depending on the values in the configuration settings spreadsheet.

Configuration settings files can also be supplied [in several different file formats] (inex-solutions/configgen/wiki/configuration-settings-files).

There are also [filtering options] (https://github.com/inex-solutions/configgen/wiki/filtering-options) to control which configurations are generated from a spreadsheet, [output options] (https://github.com/inex-solutions/configgen/wiki/file-output-options) to control how files are written out, and a variety of other options too.

