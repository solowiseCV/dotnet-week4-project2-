# Student Management System

A C# console application that manages student records with full CRUD functionality. Built as a demonstration of core C# programming concepts including variables, loops, conditionals, operators, and input validation.

---

## Features

### 1. Add New Student
Prompts for a student ID, name, and grade. Validates that:
- The ID is a positive integer and not already taken
- The name is not empty
- The grade is a number between 0 and 100

### 2. View All Students
Displays all students in a formatted table showing ID, name, grade, pass/fail status, and enrollment status.

### 3. Calculate Average Grade
Sums all grades using a loop and divides by the number of students. Guards against division by zero when the list is empty.

### 4. Find Student by ID
Searches the student list by ID and displays a full info card for the matching student, or reports "not found."

### 5. Update Student Grade
Locates a student by ID, shows their current grade, accepts a new validated grade, updates it, and displays the before/after change including the delta (+/-).

### 6. Delete Student
Finds a student by ID, asks for confirmation (y/n), then removes them from the system.

### 7. Display Statistics
Shows:
- Highest and lowest grade (with student name)
- Number of passing students (grade >= 60)
- Number of failing students (grade < 60)
- Overall pass rate as a percentage

### 8. Exit
Exits the application cleanly.

---

## How to Run

### Requirements
- [.NET SDK](https://dotnet.microsoft.com/download) **or** [Mono](https://www.mono-project.com/)

### With .NET SDK
```bash
dotnet-script StudentManagementSystem.cs
```

### With Mono
```bash
mcs StudentManagementSystem.cs -out:sms.exe
mono sms.exe
```

---

## Example Usage

```
=== Student Management System ===
1. Add New Student
2. View All Students
3. Calculate Average Grade
4. Find Student by ID
5. Update Student Grade
6. Delete Student
7. Display Statistics
8. Exit
---------------------------------
Enter your choice: 2

=== All Students ===

  ID       Name                     Grade   Status Enrolled
  --------------------------------------------------------
  101      Alice Johnson             85.5   Pass   Yes
  102      Bob Smith                 72.0   Pass   Yes
  103      Carol White               45.5   Fail   Yes
  104      David Brown               91.0   Pass   Yes
  105      Eve Davis                 68.5   Pass   Yes
  --------------------------------------------------------
  Total: 5 student(s)

Press any key to continue...
```

```
Enter your choice: 7

=== Statistics ===

  Highest grade    : 91.00  (David Brown)
  Lowest grade     : 45.50  (Carol White)
  Passing students : 4
  Failing students : 1
  Pass rate        : 80.0%
```

```
Enter your choice: 5

=== Update Student Grade ===

  Enter ID: 103
  Current grade for Carol White: 45.50
  New grade (0-100): 65
  [OK] Grade updated for Carol White:  45.50  ->  65.00  (+19.50)
```

---

## Concepts Demonstrated

| Concept | Where Used |
|---|---|
| **Variables** | `id`, `name`, `grade`, `sum`, `highest`, `lowest`, `pass`, `fail`, `running` |
| **Data types** | `int` (IDs, counts), `double` (grades, average), `string` (names), `bool` (enrolled, running) |
| **List / data structure** | `List<Student>` stores all student records |
| **Class & object** | `Student` class with `Id`, `Name`, `Grade`, `IsEnrolled` fields and a `Status` computed property |
| **while loop** | Main menu keeps running until user chooses Exit |
| **foreach loop** | Iterates students in View, Average, Statistics |
| **for loop** | Index-based iteration in Delete |
| **switch statement** | Routes menu choices 1–8 to the correct method |
| **if / else** | Input validation, pass/fail checks, highest/lowest comparisons |
| **Relational operators** | `>=`, `<`, `>`, `==` used throughout for comparisons |
| **Logical operators** | `&&` (pass AND enrolled), `\|\|` (fail OR not enrolled), `!` (not enrolled) |
| **Arithmetic operators** | `+`, `/`, `-` for sum, average, grade delta |
| **Assignment operators** | `=` (update grade), `+=` (accumulate sum) |
| **Type conversion** | `(double)pass` cast for accurate pass rate percentage |
| **Input validation** | `int.TryParse`, `double.TryParse`, range checks, empty string checks |
| **Ternary operator** | `Grade >= 60 ? "Pass" : "Fail"` in `Status` property |
| **String interpolation** | `$"..."` used throughout for formatted output |
| **Methods / functions** | `AddStudent`, `ViewStudents`, `FindById`, `Pause`, `Error`, `Success`, etc. |
| **Return values** | `FindById()` returns a `Student` object or `null` |

---

## Challenges Faced

**Input validation without crashing** — Using `int.Parse()` directly throws an exception if the user types letters. Switching to `int.TryParse()` and `double.TryParse()` across every input made the program resilient without needing try-catch blocks everywhere.

**Preventing duplicate IDs** — The first version didn't check for existing IDs, meaning you could add two students with ID 101. Adding a `FindById()` check before inserting fixed this cleanly.

**Avoiding repeated logic** — The pass/fail check (`Grade >= 60`) was originally copy-pasted into every method. Moving it into a `Status` computed property on the `Student` class means it only lives in one place and is always consistent.

**Delete by index vs. by object** — Removing from a `List<T>` by index while iterating is error-prone. Using `students.Remove(s)` after finding the object with `FindById()` is cleaner and safer.

**Division by zero on average** — If no students exist and the user asks for the average, dividing by zero would crash the program. A simple `students.Count == 0` guard returns early with a message before any calculation happens.

---



## Author

Created as part of a C# fundamentals assignment demonstrating variables, data structures, loops, conditionals, and input validation.