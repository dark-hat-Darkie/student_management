using StudentManagement.Data;
using StudentManagement.Services;
using StudentManagement.UI;

/*
 * Student Management System
 * =========================
 * This application demonstrates the four pillars of OOP:
 *
 * 1. ABSTRACTION - Person is an abstract class that defines common properties
 *    and an abstract DisplayInfo() method that must be implemented by subclasses.
 *
 * 2. INHERITANCE - Student and Teacher inherit from Person, gaining all its
 *    properties and methods while adding their own specialized features.
 *
 * 3. ENCAPSULATION - The Student class has a private _gpa field that can only
 *    be modified through controlled methods (SetGpa, CalculateGpaFromEnrollments),
 *    ensuring data integrity with validation.
 *
 * 4. POLYMORPHISM - When we call DisplayInfo() on a Person reference, the
 *    correct implementation (Student or Teacher) is called based on the
 *    actual runtime type of the object.
 *
 * Database: PostgreSQL with Npgsql driver and Dapper micro-ORM
 */

// Connection string for PostgreSQL
const string connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres";

Console.WriteLine("═══════════════════════════════════════════════════════════");
Console.WriteLine("          STUDENT MANAGEMENT SYSTEM");
Console.WriteLine("          Demonstrating OOP Principles in C#");
Console.WriteLine("═══════════════════════════════════════════════════════════");
Console.WriteLine();

// Initialize database connection factory
var dbFactory = new DbConnectionFactory(connectionString);

// Test database connection
Console.Write("Testing database connection... ");
if (!dbFactory.TestConnection())
{
    Console.WriteLine("FAILED!");
    Console.WriteLine("\nCould not connect to the database.");
    Console.WriteLine("Please ensure PostgreSQL is running and the connection string is correct.");
    Console.WriteLine($"\nConnection string: {connectionString}");
    return;
}
Console.WriteLine("OK!");

// Ask if user wants to initialize/reset database schema
Console.Write("\nInitialize database tables? (y/n): ");
var initDb = Console.ReadLine()?.Trim().ToLower();

if (initDb == "y")
{
    var schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "schema.sql");

    // Try to find schema.sql in different locations
    if (!File.Exists(schemaPath))
    {
        schemaPath = "schema.sql";
    }

    if (!File.Exists(schemaPath))
    {
        schemaPath = Path.Combine(Directory.GetCurrentDirectory(), "schema.sql");
    }

    if (File.Exists(schemaPath))
    {
        Console.WriteLine($"Using schema file: {schemaPath}");
        await dbFactory.InitializeDatabaseAsync(schemaPath);
    }
    else
    {
        Console.WriteLine("schema.sql not found. Please run the SQL manually.");
        Console.WriteLine("The schema file should be in the project root directory.");
    }
}

Console.WriteLine("\nInitializing services...");

// Create repositories (Data Layer)
var studentRepo = new StudentRepository(dbFactory);
var teacherRepo = new TeacherRepository(dbFactory);
var courseRepo = new CourseRepository(dbFactory);
var enrollmentRepo = new EnrollmentRepository(dbFactory);

// Create services (Business Logic Layer)
var studentService = new StudentService(studentRepo, enrollmentRepo);
var teacherService = new TeacherService(teacherRepo);
var courseService = new CourseService(courseRepo, teacherRepo);
var enrollmentService = new EnrollmentService(enrollmentRepo, studentRepo, courseRepo);

// Create console menu (UI Layer)
var menu = new ConsoleMenu(studentService, teacherService, courseService, enrollmentService);

Console.WriteLine("Ready!\n");
Console.WriteLine("Press any key to start...");
Console.ReadKey(true);

// Run the application
await menu.RunAsync();
