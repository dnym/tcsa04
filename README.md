# Coding Tracker
An implementation of [the fourth project of The C# Academy](https://www.thecsharpacademy.com/project/13).

## Project Description
The project is a console based coding tracker: a programmer can record their daily time spent programming.

### Requirements
- Reused requirements from the [Habit Tracker](https://github.com/dnym/tcsa03):
  - Store and retrieve data from a SQLite database.
  - Create database upon application start, if none exists.
  - Create required tables in database upon database creation.
  - Show the user a menu of options.
  - Allow recording time durations: insertion, deletion, updating, viewing.
  - Handle all possible errors to guard against crashing.
  - Allow for exiting the application only inputting `0`.
  - Only interact with the database using ADO.NET and raw SQL (i.e. no mappers).
  - Keep a README file.
- Present time logs using [ConsoleTableExt](https://github.com/minhhungit/ConsoleTableExt).
- Organize classes in different files.
- Specify for the user the required input date format and disallow any other formats.
- Keep a configuration file for database path and connection strings.
- Represent time logs using a `CodingSession` class with properties `Id`, `StartTime`, `EndTime`, `Duration`.
- Calculate duration using start time and end time (i.e. don't take duration as input).
- Allow manual input of start time and end time.
- Properly map database read results into `List<CodingSession>`.
 
### Bonus Features
- [ ] Allow for live tracking by using a stopwatch.
- [ ] Allow for filtering records per period (days, weeks, years).
- [ ] Allow for changing the order of records between ascending and descending.
- [ ] Generate reports for total and average time and number of coding sessions per period (days, weeks, years).
- [ ] Allow for setting goals.
  - [ ] Show user time until reaching goal including hours per day necessary.
