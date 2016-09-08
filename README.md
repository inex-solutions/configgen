_The full ConfigGen wiki can be found [here] (https://github.com/inex-solutions/configgen/wiki)._

**NOTE: ConfigGen v2 is still in beta.**

This version of ConfigGen is a re-write of the original [ConfigGen](https://configgen.codeplex.com/).

For the support status of ConfigGen v1 features, please see [Supported v1 features](https://github.com/inex-solutions/configgen/wiki/configgen-v1-compatibility).

##ConfigGen 

Welcome to ConfigGen - primarily a configuration file generator, but also a general templating engine.

##Overview (or "why do I need ConfigGen?")

ConfigGen's main aim is to relieve you from the tyranny of managing configuration files for all your environments and machines. 

Rather than having to manually maintain configuration files for each workstation, server, or environment, ConfigGen reduces the problem to a single template (the core of your configuration file) together with a configuration spreadsheet that contains the individual settings for each machine or environment.

### A simple example

Here is a simple example of an xml template file (mytemplatefile.xml):

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <appSettings>
        <add key="Environment" value="[%Environment%]" />
        <add key="LogLevel" value="[%LogLevel%]" />
      </appSettings>
    </configuration>

and here is a simple example of a settings spreadsheet file (mysettingsfile.xls):

| MachineName   | Environment | LogLevel |
| ---           | ---         | ---      |
| DevServer     | DEV         | DEBUG    |
| UatServer     | UAT         | INFO     |
| ProdServer    | PROD        | INFO     |
| | |
| MyWorkstation | DEV         | DEBUG    |
| | |
| Default       | DEV         | DEBUG    |

These can be run through ConfigGen with the following command line:

    cfg.exe --template-file mytemplatefile.xml --settings-file mysettingsfile.xls

which results in 5 files being generated (one for each environment specified in the spreadsheet), each in its own sub-directory within a top-level directory named `Configs`:

    Configs
      +- DevServer.xml
      +- UatServer.xml
      +- ProdServer.xml
      +- MyWorkstation.xml
      +- Default.xml

Each file will have had its tokens (`[%Environment%]` and `[%LogLevel%]` in the example above) replaced with the relevant value from the settings spreadsheet.

For example, the ProdServer.xml file will contain:

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <appSettings>
        <add key="Environment" value="ProdServer" />
        <add key="LogLevel" value="INFO" />
      </appSettings>
    </configuration>

So why do you need it? Here are a few reasons:

* You can see all the different configuration settings for all your environments and machines tabulated in a single spreadsheet
* When your configuration file is changed, it only needs to be changed in one place.
* The tabulated view makes it easy to spot missing configuration data for certain machines: no more deploying to UAT, only to find your app fails due to a config change that was applied to your dev boxes only.
ConfigGen warns if any inconsistencies are found between the template file and the settings spreadsheet.

## Further reading
Template files, such as the example above, can be written as either xml, or razor. 
More advanced examples of these support changes to the structure of your configuration files depending on the values in the configuration settings spreadsheet.
More information can be found [here](https://github.com/inex-solutions/configgen/wiki/template-files).

Settings files, such as the example above, are typically a spreadsheet or csv file, although a more sophisticated hierarchical xml format is also supported. 
For more information on settings file formats, see [here](https://github.com/inex-solutions/configgen/wiki/settings-files).

There are also [filtering options] (https://github.com/inex-solutions/configgen/wiki/filtering-options) to control which configurations are generated from a spreadsheet, [output options] (https://github.com/inex-solutions/configgen/wiki/file-output-options) to control how files are written out, and a variety of other options too.

