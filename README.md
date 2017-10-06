# Meridix API Client Command Line Client (CLI)

Precompiled binary can be downloaded from https://meridix.co/2xYUPan
 
 ## Usage
 All command are executed through the command line using a verb e.g. MeridixApi.exe VERB-HERE --parameters-here

 Use the --help command to get descriptions about the available options.

 ## Report API CLI Usage
 
 The Report API loads settings from a parameter JSON file (format and options decribed here http://bit.ly/meridix-report-api).
 The file can be scafollded by the API client with the following command. (If no file path is specified the value default report-parameters.json is used)

 ```
 MeridixApi.exe scaffold --parameter-file [--parameter-file-path]
 ```

 Edit the report-parameters.json file with a text editor. Then execute the follwing command to create a CSV file called [result.csv]

 ```
 MeridixApi.exe report --base-url https://reports.mydomain.com --token TOKEN_HERE -secret SECRET_HERE --output-file result.csv -format csv --languge en-GB
 ```

 Use the switches --from-date and --to-date to override the from and to dates specified in the report-parameters.json