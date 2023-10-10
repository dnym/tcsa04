# Planning

## Clarifications
*Quoted answers come from [ChatGPT](https://chat.openai.com/) who plays the role of external stakeholder (i.e. customer).*

- What's the goal and target group of this project?
- Any technological requirements other than it being C# and a console application?
- Should the configuration file be XML?
- What should happen if the database cannot be created? If the tables cannot be created? If the database cannot be read from or written to? There's a requirement to "Handle all possible errors to guard against crashing", but perhaps this doesn't apply to database errors, since there's not much to do if the database connection cannot function?
- I envision the main menu as "1. Log a Coding Session; 2. Manage Coding Sessions; 0. Quit". OK?
- ConsoleTableExt is to be used for presenting logged coding sessions. A CodingSession object is to have the properties `Id`, `StartTime`, `EndTime`, and `Duration`; I assume `Id` is the internal database identifier, so should it be hidden from the user?
- As for "Generate reports for total and average time and number of coding sessions per period (days, weeks, years)", should these reports be exported to a specific file format, or just shown in the application?
- As for "Allow for setting goals" and "Show user time until reaching goal including hours per day necessary", where should this be shown? In the main menu? Everywhere?
