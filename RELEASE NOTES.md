
### v.2.0.x
* New namespace for XmlTemplate (http://inex-solutions.com/Namespaces/ConfigGen/1/1/). Old template namespace now raises error.


### v2.0.13-beta (08/09/2016)
* RazorTemplate now uses [RazorEngine] (https://github.com/Antaris/RazorEngine) internally. This fixes [Issue #1 - RazorTemplate does not support writing attributes with Razor content] (https://github.com/inex-solutions/configgen/issues/1)  
* PrettyPrint preferences renamed to XmlPrettyPrint
* XmlPrettyPrint moved from XmlTemplate to the output pipeline. This fixes [Issue #2 - Razor templates should support pretty-printing] (https://github.com/inex-solutions/configgen/issues/2)
* Preferences can now be specified in Excel spreadsheet, and in xml and razor templates.


### v2.0.7-beta (20/08/2016)
* Improvements to ConsoleRunner.  
* OutputDirectory support added.  
* Minor fixes.  


### v2.0.5-beta  (19/08/2016)  
* Razor & Xml template support.  
* Xls, xls, xml, csv settings support.  
* Console runner.  


### pre-alpha
* Razor template support.  
* Basic XML templa support.  
* No settings file support.  
* No runner support.  