This application has two views one for real time and one for historic

-----------------
Real time View
----------------
When running the application the real time view will be launched, this view uses the symbols, yahoo finance url and timer interval driven from the config 
It makes use of a dispatcher timer which keeps ticking based on the interval value specified in the config
- the symbols are specified in the app.config with key CsvSymbols
- the timer interval is specified in the config with key TimerIntervalInSeconds
- the coma separated symbol is specfied in the config with key CsvSymbols
- the field values that needs to be fetched from the yahoo finance are specified as a comma seprated list in config - FieldsToFetch 

To Test This:
 Launch the application and click on the Start Real Time feed, the data will be fetched and shown on GUI, 
 This approach is a Pull based approach where the GUI polls the service using a timer at specific intervals, the approach can be changed to a Publish subscribe 
 model wherein the service can continously publish data and the GUI can subscribe and process the same in real time


-----------------
Historic View
----------------
This view can be launched by clicking on the historic button. The data for 5 years is available for 0200.HK and <2 years for the others

To Test This:
 Click on historic button, select the symbol, start and end dates and click on Get History
 The data will be loaded asynchronously and displayed to the user.

 
 ***************
 To Do:
 ***************
1) Disable Get Hisotry button when the historic data is loading and enable the same after load completed
2) Change the design from UI pulling the data to UI subscribing for data
3) Add a Data Access Layer
4) Add logic to calculate returns (based on historical data analysis)
5) Code refactoring and increase unit test coverage
