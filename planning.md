# Planning

## Clarifications
*Quoted answers come from [ChatGPT](https://chat.openai.com/) who plays the role of external stakeholder (i.e. customer).*

- What's the goal and target group of this project?
  - >The goal of the Coding Tracker project is to create a console-based tool for programmers to record their daily coding sessions. The target group is programmers who want to track and manage their coding time efficiently.
- Any technological requirements other than it being C# and a console application?
  - >No.
- Should the configuration file be XML?
  - >The project doesn't specify the format of the configuration file. You can choose XML, JSON, or any other suitable format for the configuration file based on your preference and ease of implementation.
- What should happen if the database cannot be created? If the tables cannot be created? If the database cannot be read from or written to? There's a requirement to "Handle all possible errors to guard against crashing", but perhaps this doesn't apply to database errors, since there's not much to do if the database connection cannot function?
  - >If the database cannot be created or the tables cannot be created, the application should provide an error message to the user and gracefully exit. For read/write errors, the application should also display an error message and terminate, ensuring a user-friendly experience.
- I envision the main menu as "1. Log a Coding Session; 2. Manage Coding Sessions; 0. Quit". OK?
  - >That menu structure works well. It provides clear options for logging coding sessions, managing them, and quitting the application, making it user-friendly and aligned with the project's objectives.
- ConsoleTableExt is to be used for presenting logged coding sessions. A CodingSession object is to have the properties `Id`, `StartTime`, `EndTime`, and `Duration`; I assume `Id` is the internal database identifier, so should it be hidden from the user?
  - >Yes, `Id` should be hidden from the user when presenting coding sessions with ConsoleTableExt. It's an internal database identifier and not relevant for the user's view, focusing on the more user-friendly attributes like `StartTime`, `EndTime`, and `Duration`.
- As for "Generate reports for total and average time and number of coding sessions per period (days, weeks, years)", should these reports be exported to a specific file format, or just shown in the application?
  - >The reports should be shown within the application to provide immediate feedback to the user. There's no specific requirement to export them to a file format unless it's specified as an additional feature.
- As for "Allow for setting goals" and "Show user time until reaching goal including hours per day necessary", where should this be shown? In the main menu? Everywhere?
  - >The "setting goals" feature should ideally be accessible from the main menu, and the progress toward the goal, including hours per day necessary, should be displayed within the application, possibly in a dedicated section or menu related to goals.

### More Questions & Decisions
- Should all three of `StartTime`, `EndTime`, and `Duration` be stored in the database?
  - No, `Duration` is secondary information to be calculated from `StartTime` and `EndTime`.
- What if there is no configuration file?
  - Use default values.
- Which date input format should be used?
  - For date, `YYYY-MM-DD`. For time, `HH:mm:ss`.
