using System;
using System.Collections.Generic;

namespace StudentManagementSystem
{
    // ============================================================
    // Student Management System
    // Demonstrates: variables, loops, conditionals, operators,
    // input validation, arrays/lists, and helper methods.
    // ============================================================
    class Program
    {
        // ── Parallel lists store each student's data by index ────────────────────
        // We use separate lists (instead of a class) to practice array/list
        // operations with primitive types as required by the assignment.
        static readonly List<int>    studentIDs     = new List<int>();
        static readonly List<string> studentNames   = new List<string>();
        static readonly List<double> studentGrades  = new List<double>();
        static readonly List<bool>   enrolledStatus = new List<bool>();

        // studentCount tracks the number of students; updated with ++ and --
        // rather than always calling .Count, to practice arithmetic operators.
        static int studentCount = 0;

        // ── Program entry point ──────────────────────────────────────────────────
        static void Main(string[] args)
        {
            // Pre-load 5 sample students so the system is usable immediately
            SeedData();

            // 'running' controls the main menu loop.
            // We use a bool flag (not break/goto) for clear, readable flow.
            bool running = true;

            // while loop: keeps the menu alive until the user chooses Exit (10)
            while (running)
            {
                Console.Clear();
                PrintBanner();

                Console.Write("Enter your choice: ");
                string raw = Console.ReadLine() ?? "";

                // TryParse safely converts the string to int without throwing
                // an exception if the user types letters or symbols
                if (!int.TryParse(raw, out int choice))
                {
                    ShowError("Please enter a number between 1 and 10.");
                    Pause();
                    continue; // skip the switch and re-show the menu
                }

                Console.Clear();

                // switch statement routes each valid menu option to its method.
                // 'default' catches any integer outside 1-10.
                switch (choice)
                {
                    case 1:  AddNewStudent();      break;
                    case 2:  ViewAllStudents();    break;
                    case 3:  CalculateAverage();   break;
                    case 4:  FindStudentByID();    break;
                    case 5:  UpdateStudentGrade(); break;
                    case 6:  DeleteStudent();      break;
                    case 7:  DisplayStatistics();  break;
                    case 8:  SearchByName();       break;
                    case 9:  SortStudents();       break;
                    case 10: running = false;      break;
                    default:
                        ShowError("Choice must be between 1 and 10.");
                        break;
                }

                if (running) Pause();
            }

            Console.WriteLine("\n  Goodbye!\n");
        }

        // ════════════════════════════════════════════════════════════════════════
        // 1. ADD NEW STUDENT
        // Collects name, ID, and grade with full validation before storing.
        // ════════════════════════════════════════════════════════════════════════
        static void AddNewStudent()
        {
            PrintHeader("Add New Student");

            // ── Name validation ──────────────────────────────────────────────
            Console.Write("  Enter student name     : ");
            string name = (Console.ReadLine() ?? "").Trim();

            // .Length == 0 is a relational check; we reject blank names early
            if (name.Length == 0)
            {
                ShowError("Name cannot be empty.");
                return;
            }

            // ── ID validation ────────────────────────────────────────────────
            Console.Write("  Enter student ID       : ");

            // TryParse + compound condition (&&) checks both parse success
            // and the positive-integer business rule in one expression
            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                ShowError("ID must be a positive integer.");
                return;
            }

            // List.Contains performs a linear search to detect duplicate IDs
            if (studentIDs.Contains(id))
            {
                ShowError($"A student with ID {id} already exists.");
                return;
            }

            // ── Grade validation ─────────────────────────────────────────────
            Console.Write("  Enter initial grade (0-100): ");

            // Double.TryParse handles decimals; the && chains the range check
            if (!double.TryParse(Console.ReadLine(), out double grade) || grade < 0 || grade > 100)
            {
                ShowError("Grade must be a number between 0 and 100.");
                return;
            }

            // ── Persist the new student across all parallel lists ────────────
            studentIDs.Add(id);
            studentNames.Add(name);
            studentGrades.Add(grade);
            enrolledStatus.Add(true);   // new students are active by default
            studentCount++;             // arithmetic increment operator

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  [OK] Student '{name}' (ID: {id}) added successfully!");
            Console.WriteLine($"       Letter Grade: {GetLetterGrade(grade)}");
            Console.ResetColor();
        }

        // ════════════════════════════════════════════════════════════════════════
        // 2. VIEW ALL STUDENTS
        // Prints a formatted table with ID, name, grade, letter grade,
        // pass/fail status, and enrollment for every student.
        // ════════════════════════════════════════════════════════════════════════
        static void ViewAllStudents()
        {
            PrintHeader("All Students");

            if (studentCount == 0)
            {
                ShowError("No students in the system.");
                return;
            }

            // Header row — format specifiers align columns:
            //   ,-8  = left-aligned in 8-char field
            //   ,8   = right-aligned in 8-char field
            Console.WriteLine($"  {"ID",-8} {"Name",-22} {"Grade",7}  {"Letter",-7} {"Status",-7} {"Enrolled"}");
            Console.WriteLine("  " + new string('-', 64));

            // for loop chosen over foreach because we need the index to
            // access the same position across multiple parallel lists
            for (int i = 0; i < studentCount; i++)
            {
                // Ternary operator (relational >=) determines pass/fail inline
                string status   = studentGrades[i] >= 60 ? "Pass" : "Fail";
                string enrolled = enrolledStatus[i] ? "Yes" : "No";

                // GetLetterGrade encapsulates the A/B/C/D/F logic (Bonus 1)
                string letter = GetLetterGrade(studentGrades[i]);

                // Use green for passing, red for failing — visual feedback
                ConsoleColor rowColor = status == "Pass" ? ConsoleColor.Green : ConsoleColor.Red;

                Console.Write($"  {studentIDs[i],-8} {studentNames[i],-22} {studentGrades[i],7:F1}  ");
                Console.ForegroundColor = rowColor;
                Console.Write($"{letter,-7} {status,-7}");
                Console.ResetColor();
                Console.WriteLine($" {enrolled}");
            }

            Console.WriteLine("  " + new string('-', 64));
            Console.WriteLine($"  Total students: {studentCount}");
        }

        // ════════════════════════════════════════════════════════════════════════
        // 3. CALCULATE AVERAGE GRADE
        // Iterates all grades with a loop, accumulates with +=, then divides.
        // ════════════════════════════════════════════════════════════════════════
        static void CalculateAverage()
        {
            PrintHeader("Average Grade");

            // Guard: dividing by zero would crash; return early with a message
            if (studentCount == 0)
            {
                ShowError("No students to calculate average for.");
                return;
            }

            double sum = 0;

            // for loop accumulates grades; += is the compound assignment operator
            for (int i = 0; i < studentCount; i++)
            {
                sum += studentGrades[i]; // equivalent to: sum = sum + studentGrades[i]
            }

            // Division operator produces the mean; studentCount > 0 guaranteed above
            double average = sum / studentCount;

            Console.WriteLine($"  Total students : {studentCount}");
            Console.WriteLine($"  Sum of grades  : {sum:F2}");

            // Highlight the result in cyan for visual emphasis
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  Average grade  : {average:F2}  ({GetLetterGrade(average)})");
            Console.ResetColor();
        }

        // ════════════════════════════════════════════════════════════════════════
        // 4. FIND STUDENT BY ID
        // Searches the ID list linearly via FindIndex() and displays the match.
        // ════════════════════════════════════════════════════════════════════════
        static void FindStudentByID()
        {
            PrintHeader("Find Student by ID");

            Console.Write("  Enter student ID to search: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Please enter a valid integer ID.");
                return;
            }

            // FindIndex encapsulates the search loop; returns -1 when not found
            int index = FindIndex(id);

            if (index == -1)
            {
                ShowError($"Student with ID {id} not found.");
                return;
            }

            PrintStudentCard(index);
        }

        // ════════════════════════════════════════════════════════════════════════
        // 5. UPDATE STUDENT GRADE
        // Locates a student by ID, validates the new grade, and updates it.
        // ════════════════════════════════════════════════════════════════════════
        static void UpdateStudentGrade()
        {
            PrintHeader("Update Student Grade");

            Console.Write("  Enter student ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Please enter a valid integer ID.");
                return;
            }

            int index = FindIndex(id);
            if (index == -1)
            {
                ShowError($"Student with ID {id} not found.");
                return;
            }

            double oldGrade = studentGrades[index];
            Console.WriteLine($"  Current grade for {studentNames[index]}: {oldGrade:F2}  ({GetLetterGrade(oldGrade)})");

            Console.Write("  Enter new grade (0-100): ");
            if (!double.TryParse(Console.ReadLine(), out double newGrade) || newGrade < 0 || newGrade > 100)
            {
                ShowError("Grade must be between 0 and 100.");
                return;
            }

            // Assignment operator = updates the grade in place
            studentGrades[index] = newGrade;

            // Subtraction operator calculates the change for user feedback
            double delta = newGrade - oldGrade;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  [OK] Grade updated for {studentNames[index]}:");
            Console.ResetColor();
            Console.WriteLine($"       {oldGrade:F2} ({GetLetterGrade(oldGrade)})  ->  " +
                              $"{newGrade:F2} ({GetLetterGrade(newGrade)})  " +
                              $"({(delta >= 0 ? "+" : "")}{delta:F2})");
        }

        // ════════════════════════════════════════════════════════════════════════
        // 6. DELETE STUDENT
        // Confirms with the user before removing a student from all lists.
        // ════════════════════════════════════════════════════════════════════════
        static void DeleteStudent()
        {
            PrintHeader("Delete Student");

            Console.Write("  Enter student ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Please enter a valid integer ID.");
                return;
            }

            int index = FindIndex(id);
            if (index == -1)
            {
                ShowError($"Student with ID {id} not found.");
                return;
            }

            string name = studentNames[index];

            // Confirmation step prevents accidental deletion
            Console.Write($"  Delete '{name}' (ID: {id})? (y/n): ");
            string confirm = (Console.ReadLine() ?? "").Trim().ToLower();

            // Logical NOT (!) inverts the equality check: cancel unless "y"
            if (!(confirm == "y"))
            {
                Console.WriteLine("  Deletion cancelled.");
                return;
            }

            // Remove from every parallel list at the same index to stay in sync
            studentIDs.RemoveAt(index);
            studentNames.RemoveAt(index);
            studentGrades.RemoveAt(index);
            enrolledStatus.RemoveAt(index);
            studentCount--; // decrement operator

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  [OK] Student '{name}' (ID: {id}) deleted successfully.");
            Console.ResetColor();
        }

        // ════════════════════════════════════════════════════════════════════════
        // 7. DISPLAY STATISTICS
        // Single pass through all students to compute highest, lowest,
        // pass/fail counts, and pass rate using relational & logical operators.
        // ════════════════════════════════════════════════════════════════════════
        static void DisplayStatistics()
        {
            PrintHeader("Statistics");

            if (studentCount == 0)
            {
                ShowError("No students to show statistics for.");
                return;
            }

            // Seed highest/lowest with the first student's grade so every
            // subsequent comparison is valid from the first iteration
            double highest  = studentGrades[0];
            double lowest   = studentGrades[0];
            string highName = studentNames[0];
            string lowName  = studentNames[0];
            int pass = 0, fail = 0;

            for (int i = 0; i < studentCount; i++)
            {
                double g = studentGrades[i];

                // Relational > and < update the running max/min
                if (g > highest) { highest = g; highName = studentNames[i]; }
                if (g < lowest)  { lowest  = g; lowName  = studentNames[i]; }

                // Logical &&: only count as "passing" if BOTH grade >= 60 AND enrolled
                if (g >= 60 && enrolledStatus[i])
                    pass++;
                // Logical ||: failing if grade < 60 OR not enrolled
                else if (g < 60 || !enrolledStatus[i])
                    fail++;
            }

            // Cast pass to double before dividing to avoid integer division truncation
            double passRate = (double)pass / studentCount * 100;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  Highest grade    : {highest:F2}  {GetLetterGrade(highest)}  ({highName})");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  Lowest grade     : {lowest:F2}  {GetLetterGrade(lowest)}  ({lowName})");
            Console.ResetColor();
            Console.WriteLine($"  Passing students : {pass}");
            Console.WriteLine($"  Failing students : {fail}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  Pass rate        : {passRate:F1}%");
            Console.ResetColor();
        }

        // ════════════════════════════════════════════════════════════════════════
        // BONUS 2 — SEARCH BY NAME
        // Finds students whose name contains the search term (case-insensitive).
        // Supports partial matches: "ali" matches "Alice Johnson".
        // ════════════════════════════════════════════════════════════════════════
        static void SearchByName()
        {
            PrintHeader("Search by Name");

            Console.Write("  Enter name (or partial name): ");
            string term = (Console.ReadLine() ?? "").Trim().ToLower();

            if (term.Length == 0)
            {
                ShowError("Search term cannot be empty.");
                return;
            }

            // Collect matching indexes into a results list first
            List<int> matches = new List<int>();

            for (int i = 0; i < studentCount; i++)
            {
                // String.Contains with ToLower() = case-insensitive partial match
                // "ali" matches "Alice", "alice johnson", etc.
                if (studentNames[i].ToLower().Contains(term))
                    matches.Add(i);
            }

            if (matches.Count == 0)
            {
                ShowError($"No students found matching \"{term}\".");
                return;
            }

            Console.WriteLine($"\n  Found {matches.Count} match(es) for \"{term}\":\n");

            // Print a student card for every match
            foreach (int idx in matches)
                PrintStudentCard(idx);
        }

        // ════════════════════════════════════════════════════════════════════════
        // BONUS 3 — SORT STUDENTS
        // Lets the user pick a sort key; uses selection sort on an index array
        // so the original lists are never reordered.
        // ════════════════════════════════════════════════════════════════════════
        static void SortStudents()
        {
            PrintHeader("Sort Students");

            if (studentCount == 0) { ShowError("No students to sort."); return; }

            Console.WriteLine("  Sort by:");
            Console.WriteLine("    1. Grade  (highest to lowest)");
            Console.WriteLine("    2. Name   (A to Z)");
            Console.WriteLine("    3. ID     (ascending)");
            Console.Write("\n  Your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int sortChoice) ||
                sortChoice < 1 || sortChoice > 3)
            {
                ShowError("Please enter 1, 2, or 3.");
                return;
            }

            // Build an index array [0, 1, 2, ...] then sort indexes, not data.
            // This avoids touching the parallel lists and is easier to follow.
            int[] order = new int[studentCount];
            for (int i = 0; i < studentCount; i++) order[i] = i;

            // Selection sort: O(n^2) but simple and transparent for learning.
            // Each outer pass finds the best remaining element and moves it forward.
            for (int i = 0; i < studentCount - 1; i++)
            {
                int best = i; // assume current position is best so far

                for (int j = i + 1; j < studentCount; j++)
                {
                    bool shouldSwap = false;

                    // Relational operators select the sort direction per key
                    if (sortChoice == 1)
                        shouldSwap = studentGrades[order[j]] > studentGrades[order[best]]; // desc
                    else if (sortChoice == 2)
                        shouldSwap = string.Compare(studentNames[order[j]],
                                                    studentNames[order[best]],
                                                    StringComparison.OrdinalIgnoreCase) < 0; // asc
                    else
                        shouldSwap = studentIDs[order[j]] < studentIDs[order[best]]; // asc

                    if (shouldSwap) best = j;
                }

                // Swap only the index pointers; student data is untouched
                int tmp = order[i];
                order[i] = order[best];
                order[best] = tmp;
            }

            string[] labels = { "", "Grade (High to Low)", "Name (A to Z)", "ID (Ascending)" };
            Console.WriteLine($"\n  Sorted by: {labels[sortChoice]}\n");
            Console.WriteLine($"  {"ID",-8} {"Name",-22} {"Grade",7}  {"Letter",-7} {"Status"}");
            Console.WriteLine("  " + new string('-', 58));

            // Iterate the sorted index array to print rows in the correct order
            foreach (int idx in order)
            {
                string status = studentGrades[idx] >= 60 ? "Pass" : "Fail";
                string letter = GetLetterGrade(studentGrades[idx]);

                Console.Write($"  {studentIDs[idx],-8} {studentNames[idx],-22} {studentGrades[idx],7:F1}  ");
                Console.ForegroundColor = status == "Pass" ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write($"{letter,-7} {status}");
                Console.ResetColor();
                Console.WriteLine();
            }

            Console.WriteLine("  " + new string('-', 58));
        }

        // ════════════════════════════════════════════════════════════════════════
        // BONUS 1 — LETTER GRADE HELPER
        // Converts a numeric grade to a letter using an if/else if chain.
        // Centralised here so every method stays consistent.
        //
        //   A  >= 90   excellent
        //   B  >= 80   above average
        //   C  >= 70   average
        //   D  >= 60   below average but passing
        //   F  <  60   failing
        // ════════════════════════════════════════════════════════════════════════
        static string GetLetterGrade(double grade)
        {
            // Cascading if/else if: evaluated top-down; first match wins
            if      (grade >= 90) return "A";
            else if (grade >= 80) return "B";
            else if (grade >= 70) return "C";
            else if (grade >= 60) return "D";
            else                  return "F"; // logical fallthrough for grade < 60
        }

        // ════════════════════════════════════════════════════════════════════════
        // HELPER — FindIndex
        // Linear search returns the list index of an ID, or -1 if not found.
        // Extracted to avoid duplicating the loop in every calling method.
        // ════════════════════════════════════════════════════════════════════════
        static int FindIndex(int id)
        {
            // for loop used (not foreach) because we need to return the
            // numeric index, not the element value itself
            for (int i = 0; i < studentCount; i++)
            {
                if (studentIDs[i] == id) // relational == equality check
                    return i;
            }
            return -1; // sentinel value: caller checks for -1 to detect "not found"
        }

        // ════════════════════════════════════════════════════════════════════════
        // HELPER — PrintStudentCard
        // Displays a formatted info block for one student by index.
        // Reused by FindStudentByID and SearchByName to keep output consistent.
        // ════════════════════════════════════════════════════════════════════════
        static void PrintStudentCard(int index)
        {
            string status = studentGrades[index] >= 60 ? "Pass" : "Fail";
            string letter = GetLetterGrade(studentGrades[index]);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  -------------------------------------");
            Console.ResetColor();
            Console.WriteLine($"  ID       : {studentIDs[index]}");
            Console.WriteLine($"  Name     : {studentNames[index]}");
            Console.Write($"  Grade    : {studentGrades[index]:F2}  (");
            Console.ForegroundColor = status == "Pass" ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write(letter);
            Console.ResetColor();
            Console.WriteLine(")");
            Console.Write("  Status   : ");
            Console.ForegroundColor = status == "Pass" ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(status);
            Console.ResetColor();
            Console.WriteLine($"  Enrolled : {(enrolledStatus[index] ? "Yes" : "No")}");
        }

        // ════════════════════════════════════════════════════════════════════════
        // HELPER — SeedData
        // Pre-loads 5 sample students for immediate demonstration.
        // ════════════════════════════════════════════════════════════════════════
        static void SeedData()
        {
            int[]    ids    = { 101, 102, 103, 104, 105 };
            string[] names  = { "Alice Johnson", "Bob Smith", "Carol White",
                                 "David Brown",  "Eve Davis" };
            double[] grades = { 85.5, 72.0, 45.5, 91.0, 68.5 };

            for (int i = 0; i < ids.Length; i++)
            {
                studentIDs.Add(ids[i]);
                studentNames.Add(names[i]);
                studentGrades.Add(grades[i]);
                enrolledStatus.Add(true);
                studentCount++;
            }
        }

        // ── UI helpers ───────────────────────────────────────────────────────────

        static void PrintBanner()
        {
            Console.WriteLine("=== Student Management System ===");
            Console.WriteLine(" 1. Add New Student");
            Console.WriteLine(" 2. View All Students");
            Console.WriteLine(" 3. Calculate Average Grade");
            Console.WriteLine(" 4. Find Student by ID");
            Console.WriteLine(" 5. Update Student Grade");
            Console.WriteLine(" 6. Delete Student");
            Console.WriteLine(" 7. Display Statistics");
            Console.WriteLine(" 8. Search by Name        [Bonus]");
            Console.WriteLine(" 9. Sort Students         [Bonus]");
            Console.WriteLine("10. Exit");
            Console.WriteLine(new string('-', 35));
        }

        static void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"=== {title} ===\n");
            Console.ResetColor();
        }

        static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n  [ERR] {message}");
            Console.ResetColor();
        }

        static void Pause()
        {
            Console.Write("\nPress any key to continue...");
            Console.ReadKey(true);
        }
    }
}