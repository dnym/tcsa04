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
- What if the user inserts overlapping sessions?
  - >If a user inserts overlapping sessions, the application should detect and handle this as an error, providing a clear message to the user. Overlapping sessions could lead to incorrect time calculations, so they should not be allowed.

### More Questions & Decisions
- Should all three of `StartTime`, `EndTime`, and `Duration` be stored in the database?
  - No, `Duration` is secondary information to be calculated from `StartTime` and `EndTime`.
- What if there is no configuration file?
  - Use default values.
- Which date input format should be used?
  - For date, `YYYY-MM-DD`. For time, `HH:mm:ss`.
- "Allow for changing the order of records between ascending and descending", but sorting on which key?
  - Doesn't matter since overlapping sessions are not allowed.

## MVP Functionality

### Necessary Functionality
- Create SQLite database with tables
- Store coding session logs in and retrieve coding session logs from DB using ADO.NET
  - Map into `List<CodingSession>`
- Handle a configuration file
- Menu system for navigating the application
  - Main menu
  - Log new coding session
  - Coding session logs management screen
    - Pagination

### Possible Later Functionality
- Live tracking using a stopwatch
- Filter logs by period
- Toggle log listing order between ascending or descending
- Report generation by period
- User goal setting
  - Goal progress indicator

## User Interface
Main menu:
```text
Coding Tracker
==============

1. Log Coding Session
2. Manage Coding Session Logs
0. Quit

------------
Press a number to select.
```

Logging:
```text
Log Coding Session
==================

At what date did you start coding? 2023-10-09
At what time did you start coding? 23:45:00

At what date did you stop coding? 2023-10-10
At what time did you stop coding? _

------------------
Input time in the format HH:mm or HH:mm:ss,
or press [Enter] to use the current time: 11:31:12
Press [Esc] to cancel insertion.
```

Log management:
```text
Coding Sessions (page 2/3)
==========================

 1. 2h54m @ 2020-08-05, from 14:36:42 to 17:30:15
 2. 2h34m @ 2020-12-12, from 05:41:18 to 08:15:30
 3. 4h31m @ 2021-10-03, from 08:14:37 to 12:45:22
 4. 1h55m @ 2021-11-27, from 16:45:29 to 18:40:09
 5. 1h57m @ 2022-04-15, from 09:23:51 to 11:20:37
 6. 0h16m @ 2022-06-19, from 20:55:03 to 21:10:55
 7. 2h47m @ 2022-09-08, from 22:19:01 to 01:05:50 on 2022-09-09
 8. 2h13m @ 2023-01-10, from 11:07:14 to 13:20:37
 9. 0h56m @ 2023-03-28, from 17:29:56 to 18:25:40
10. 3h00m @ 2023-05-30, from 13:50:27 to 16:50:12

Select a session to manage: _

--------------------------
Press [PgUp] to go to the previous page,
[PgDown] to go to the next page,
or [Esc] to go back to the main menu.
```

Single log management:
```text
Viewing Coding Session
======================

Start: 2020-12-12 05:41:18
End: 2020-12-12 08:15:30
Duration: 2h 36m 12s

----------------------
Press [M] to modify the session,
[D] to delete,
or [Esc] to go back to the main menu.
```
