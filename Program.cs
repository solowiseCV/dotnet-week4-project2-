using System;
using System.Collections.Generic;

namespace StudentManagementSystem
{
    class Program
    {
       
        static readonly List<int>    studentIDs      = [];
        static readonly List<string> studentNames    = [];
        static readonly List<double> studentGrades   = [];
        static readonly List<bool>   enrolledStatus  =[];
        static int          studentCount    = 0;

      
        static void Main(string[] args)
        {
           
           
        }

               static void AddNewStudent()
        {
            PrintHeader("Add New Student");

            // Name
            Console.Write("  Enter student name: ");
            string name = (Console.ReadLine() ?? "").Trim();
            if (name.Length == 0)
            {
                ShowError("Name cannot be empty.");
                return;
            }

            // ID
            Console.Write("  Enter student ID (positive integer): ");
            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                ShowError("ID must be a positive integer.");
                return;
            }
            if (studentIDs.Contains(id))
            {
                ShowError($"A student with ID {id} already exists.");
                return;
            }

            // Grade
            Console.Write("  Enter initial grade (0 – 100): ");
            if (!double.TryParse(Console.ReadLine(), out double grade) || grade < 0 || grade > 100)
            {
                ShowError("Grade must be a number between 0 and 100.");
                return;
            }

            // Store
            studentIDs.Add(id);
            studentNames.Add(name);
            studentGrades.Add(grade);
            enrolledStatus.Add(true);   
            studentCount++;             

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  ✓ Student '{name}' (ID: {id}) added successfully!");
            Console.ResetColor();
        }

               static void ViewAllStudents()
        {
            PrintHeader("All Students");

            if (studentCount == 0)
            {
                ShowError("No students in the system.");
                return;
            }

            Console.WriteLine($"  {"ID",-8} {"Name",-22} {"Grade",8}    {"Status",-10} {"Enrolled",-8}");
            Console.WriteLine("  " + new string('─', 62));

            for (int i = 0; i < studentCount; i++)
            {
                // Relational operator to determine pass/fail
                string status   = studentGrades[i] >= 60 ? "Pass" : "Fail";
                string enrolled = enrolledStatus[i] ? "Yes" : "No";

                ConsoleColor color = status == "Pass" ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write($"  {studentIDs[i],-8} {studentNames[i],-22} {studentGrades[i],8:F1}    ");
                Console.ForegroundColor = color;
                Console.Write($"{status,-10}");
                Console.ResetColor();
                Console.WriteLine($" {enrolled,-8}");
            }

            Console.WriteLine("  " + new string('─', 62));
            Console.WriteLine($"  Total students: {studentCount}");
        }


    }
}