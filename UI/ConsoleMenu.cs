using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement.UI;

/// <summary>
/// Console-based UI for the Student Management System.
/// </summary>
public class ConsoleMenu
{
    private readonly StudentService _studentService;
    private readonly TeacherService _teacherService;
    private readonly CourseService _courseService;
    private readonly EnrollmentService _enrollmentService;

    public ConsoleMenu(StudentService studentService, TeacherService teacherService,
        CourseService courseService, EnrollmentService enrollmentService)
    {
        _studentService = studentService;
        _teacherService = teacherService;
        _courseService = courseService;
        _enrollmentService = enrollmentService;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            DisplayMainMenu();
            var choice = ReadChoice(1, 5);

            switch (choice)
            {
                case 1:
                    await StudentMenuAsync();
                    break;
                case 2:
                    await TeacherMenuAsync();
                    break;
                case 3:
                    await CourseMenuAsync();
                    break;
                case 4:
                    await EnrollmentMenuAsync();
                    break;
                case 5:
                    Console.WriteLine("\nGoodbye!");
                    return;
            }
        }
    }

    #region Main Menu

    private void DisplayMainMenu()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║     STUDENT MANAGEMENT SYSTEM          ║");
        Console.WriteLine("║     Demonstrating OOP Principles       ║");
        Console.WriteLine("╠════════════════════════════════════════╣");
        Console.WriteLine("║  1. Student Management                 ║");
        Console.WriteLine("║  2. Teacher Management                 ║");
        Console.WriteLine("║  3. Course Management                  ║");
        Console.WriteLine("║  4. Enrollment Management              ║");
        Console.WriteLine("║  5. Exit                               ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.Write("\nEnter your choice: ");
    }

    #endregion

    #region Student Menu

    private async Task StudentMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("         STUDENT MANAGEMENT");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("  1. Add New Student");
            Console.WriteLine("  2. View All Students");
            Console.WriteLine("  3. Search Student by ID");
            Console.WriteLine("  4. Update Student");
            Console.WriteLine("  5. Delete Student");
            Console.WriteLine("  6. Back to Main Menu");
            Console.WriteLine("═══════════════════════════════════════");
            Console.Write("\nEnter your choice: ");

            var choice = ReadChoice(1, 6);

            switch (choice)
            {
                case 1:
                    await AddStudentAsync();
                    break;
                case 2:
                    await ViewAllStudentsAsync();
                    break;
                case 3:
                    await SearchStudentByIdAsync();
                    break;
                case 4:
                    await UpdateStudentAsync();
                    break;
                case 5:
                    await DeleteStudentAsync();
                    break;
                case 6:
                    return;
            }
        }
    }

    private async Task AddStudentAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ ADD NEW STUDENT ═══\n");

        Console.Write("First Name: ");
        var firstName = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Last Name: ");
        var lastName = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Email: ");
        var email = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Date of Birth (yyyy-MM-dd, or leave empty): ");
        var dobInput = Console.ReadLine()?.Trim();
        DateOnly? dob = string.IsNullOrEmpty(dobInput) ? null : DateOnly.Parse(dobInput);

        try
        {
            var id = await _studentService.CreateStudentAsync(firstName, lastName, email, dob);
            Console.WriteLine($"\n✓ Student created successfully with ID: {id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }

        WaitForKey();
    }

    private async Task ViewAllStudentsAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ ALL STUDENTS ═══\n");

        var students = await _studentService.GetAllStudentsAsync();
        var studentList = students.ToList();

        if (!studentList.Any())
        {
            Console.WriteLine("No students found.");
        }
        else
        {
            Console.WriteLine($"Found {studentList.Count} student(s):\n");
            foreach (var student in studentList)
            {
                // Demonstrating POLYMORPHISM - calling overridden DisplayInfo()
                student.DisplayInfo();
                Console.WriteLine();
            }
        }

        WaitForKey();
    }

    private async Task SearchStudentByIdAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ SEARCH STUDENT ═══\n");

        Console.Write("Enter Student ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("\n✗ Invalid ID.");
            WaitForKey();
            return;
        }

        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
        {
            Console.WriteLine($"\n✗ Student with ID {id} not found.");
        }
        else
        {
            Console.WriteLine("\nStudent found:");
            student.DisplayInfo();
        }

        WaitForKey();
    }

    private async Task UpdateStudentAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ UPDATE STUDENT ═══\n");

        Console.Write("Enter Student ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("\n✗ Invalid ID.");
            WaitForKey();
            return;
        }

        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
        {
            Console.WriteLine($"\n✗ Student with ID {id} not found.");
            WaitForKey();
            return;
        }

        Console.WriteLine("\nCurrent details:");
        student.DisplayInfo();

        Console.WriteLine("\nEnter new values (leave empty to keep current):\n");

        Console.Write($"First Name [{student.FirstName}]: ");
        var firstName = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(firstName)) firstName = student.FirstName;

        Console.Write($"Last Name [{student.LastName}]: ");
        var lastName = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(lastName)) lastName = student.LastName;

        Console.Write($"Email [{student.Email}]: ");
        var email = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(email)) email = student.Email;

        var currentDob = student.DateOfBirth?.ToString("yyyy-MM-dd") ?? "not set";
        Console.Write($"Date of Birth [{currentDob}]: ");
        var dobInput = Console.ReadLine()?.Trim();
        DateOnly? dob = string.IsNullOrEmpty(dobInput) ? student.DateOfBirth : DateOnly.Parse(dobInput);

        try
        {
            await _studentService.UpdateStudentAsync(id, firstName, lastName, email, dob);
            Console.WriteLine("\n✓ Student updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }

        WaitForKey();
    }

    private async Task DeleteStudentAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ DELETE STUDENT ═══\n");

        Console.Write("Enter Student ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("\n✗ Invalid ID.");
            WaitForKey();
            return;
        }

        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
        {
            Console.WriteLine($"\n✗ Student with ID {id} not found.");
            WaitForKey();
            return;
        }

        Console.WriteLine("\nStudent to delete:");
        student.DisplayInfo();

        Console.Write("\nAre you sure? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();

        if (confirm == "y")
        {
            try
            {
                await _studentService.DeleteStudentAsync(id);
                Console.WriteLine("\n✓ Student deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("\nDeletion cancelled.");
        }

        WaitForKey();
    }

    #endregion

    #region Teacher Menu

    private async Task TeacherMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("         TEACHER MANAGEMENT");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("  1. Add New Teacher");
            Console.WriteLine("  2. View All Teachers");
            Console.WriteLine("  3. Search Teacher by ID");
            Console.WriteLine("  4. Update Teacher");
            Console.WriteLine("  5. Delete Teacher");
            Console.WriteLine("  6. Back to Main Menu");
            Console.WriteLine("═══════════════════════════════════════");
            Console.Write("\nEnter your choice: ");

            var choice = ReadChoice(1, 6);

            switch (choice)
            {
                case 1:
                    await AddTeacherAsync();
                    break;
                case 2:
                    await ViewAllTeachersAsync();
                    break;
                case 3:
                    await SearchTeacherByIdAsync();
                    break;
                case 4:
                    await UpdateTeacherAsync();
                    break;
                case 5:
                    await DeleteTeacherAsync();
                    break;
                case 6:
                    return;
            }
        }
    }

    private async Task AddTeacherAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ ADD NEW TEACHER ═══\n");

        Console.Write("First Name: ");
        var firstName = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Last Name: ");
        var lastName = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Email: ");
        var email = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Subject: ");
        var subject = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Department (optional): ");
        var department = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(department)) department = null;

        Console.Write("Date of Birth (yyyy-MM-dd, or leave empty): ");
        var dobInput = Console.ReadLine()?.Trim();
        DateOnly? dob = string.IsNullOrEmpty(dobInput) ? null : DateOnly.Parse(dobInput);

        try
        {
            var id = await _teacherService.CreateTeacherAsync(firstName, lastName, email, subject, department, dob);
            Console.WriteLine($"\n✓ Teacher created successfully with ID: {id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }

        WaitForKey();
    }

    private async Task ViewAllTeachersAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ ALL TEACHERS ═══\n");

        var teachers = await _teacherService.GetAllTeachersAsync();
        var teacherList = teachers.ToList();

        if (!teacherList.Any())
        {
            Console.WriteLine("No teachers found.");
        }
        else
        {
            Console.WriteLine($"Found {teacherList.Count} teacher(s):\n");
            foreach (var teacher in teacherList)
            {
                // Demonstrating POLYMORPHISM - calling overridden DisplayInfo()
                teacher.DisplayInfo();
                Console.WriteLine();
            }
        }

        WaitForKey();
    }

    private async Task SearchTeacherByIdAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ SEARCH TEACHER ═══\n");

        Console.Write("Enter Teacher ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("\n✗ Invalid ID.");
            WaitForKey();
            return;
        }

        var teacher = await _teacherService.GetTeacherByIdAsync(id);
        if (teacher == null)
        {
            Console.WriteLine($"\n✗ Teacher with ID {id} not found.");
        }
        else
        {
            Console.WriteLine("\nTeacher found:");
            teacher.DisplayInfo();
        }

        WaitForKey();
    }

    private async Task UpdateTeacherAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ UPDATE TEACHER ═══\n");

        Console.Write("Enter Teacher ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("\n✗ Invalid ID.");
            WaitForKey();
            return;
        }

        var teacher = await _teacherService.GetTeacherByIdAsync(id);
        if (teacher == null)
        {
            Console.WriteLine($"\n✗ Teacher with ID {id} not found.");
            WaitForKey();
            return;
        }

        Console.WriteLine("\nCurrent details:");
        teacher.DisplayInfo();

        Console.WriteLine("\nEnter new values (leave empty to keep current):\n");

        Console.Write($"First Name [{teacher.FirstName}]: ");
        var firstName = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(firstName)) firstName = teacher.FirstName;

        Console.Write($"Last Name [{teacher.LastName}]: ");
        var lastName = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(lastName)) lastName = teacher.LastName;

        Console.Write($"Email [{teacher.Email}]: ");
        var email = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(email)) email = teacher.Email;

        Console.Write($"Subject [{teacher.Subject}]: ");
        var subject = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(subject)) subject = teacher.Subject;

        Console.Write($"Department [{teacher.Department ?? "not set"}]: ");
        var department = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(department)) department = teacher.Department;

        var currentDob = teacher.DateOfBirth?.ToString("yyyy-MM-dd") ?? "not set";
        Console.Write($"Date of Birth [{currentDob}]: ");
        var dobInput = Console.ReadLine()?.Trim();
        DateOnly? dob = string.IsNullOrEmpty(dobInput) ? teacher.DateOfBirth : DateOnly.Parse(dobInput);

        try
        {
            await _teacherService.UpdateTeacherAsync(id, firstName, lastName, email, subject, department, dob);
            Console.WriteLine("\n✓ Teacher updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }

        WaitForKey();
    }

    private async Task DeleteTeacherAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ DELETE TEACHER ═══\n");

        Console.Write("Enter Teacher ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("\n✗ Invalid ID.");
            WaitForKey();
            return;
        }

        var teacher = await _teacherService.GetTeacherByIdAsync(id);
        if (teacher == null)
        {
            Console.WriteLine($"\n✗ Teacher with ID {id} not found.");
            WaitForKey();
            return;
        }

        Console.WriteLine("\nTeacher to delete:");
        teacher.DisplayInfo();

        Console.Write("\nAre you sure? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();

        if (confirm == "y")
        {
            try
            {
                await _teacherService.DeleteTeacherAsync(id);
                Console.WriteLine("\n✓ Teacher deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("\nDeletion cancelled.");
        }

        WaitForKey();
    }

    #endregion

    #region Course Menu

    private async Task CourseMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("         COURSE MANAGEMENT");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("  1. Add New Course");
            Console.WriteLine("  2. View All Courses");
            Console.WriteLine("  3. Search Course by Code");
            Console.WriteLine("  4. Update Course");
            Console.WriteLine("  5. Delete Course");
            Console.WriteLine("  6. Assign Teacher to Course");
            Console.WriteLine("  7. Back to Main Menu");
            Console.WriteLine("═══════════════════════════════════════");
            Console.Write("\nEnter your choice: ");

            var choice = ReadChoice(1, 7);

            switch (choice)
            {
                case 1:
                    await AddCourseAsync();
                    break;
                case 2:
                    await ViewAllCoursesAsync();
                    break;
                case 3:
                    await SearchCourseByCodeAsync();
                    break;
                case 4:
                    await UpdateCourseAsync();
                    break;
                case 5:
                    await DeleteCourseAsync();
                    break;
                case 6:
                    await AssignTeacherToCourseAsync();
                    break;
                case 7:
                    return;
            }
        }
    }

    private async Task AddCourseAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ ADD NEW COURSE ═══\n");

        Console.Write("Course Code (e.g., CS101): ");
        var code = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Course Name: ");
        var name = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Credits: ");
        if (!int.TryParse(Console.ReadLine(), out var credits))
        {
            Console.WriteLine("\n✗ Invalid credits value.");
            WaitForKey();
            return;
        }

        Console.Write("Description (optional): ");
        var description = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(description)) description = null;

        Console.Write("Teacher ID (optional, leave empty for none): ");
        var teacherInput = Console.ReadLine()?.Trim();
        int? teacherId = string.IsNullOrEmpty(teacherInput) ? null : int.Parse(teacherInput);

        try
        {
            var id = await _courseService.CreateCourseAsync(code, name, credits, description, teacherId);
            Console.WriteLine($"\n✓ Course created successfully with ID: {id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }

        WaitForKey();
    }

    private async Task ViewAllCoursesAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ ALL COURSES ═══\n");

        var courses = await _courseService.GetAllCoursesAsync();
        var courseList = courses.ToList();

        if (!courseList.Any())
        {
            Console.WriteLine("No courses found.");
        }
        else
        {
            Console.WriteLine($"Found {courseList.Count} course(s):\n");
            foreach (var course in courseList)
            {
                course.DisplayInfo();
                Console.WriteLine();
            }
        }

        WaitForKey();
    }

    private async Task SearchCourseByCodeAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ SEARCH COURSE ═══\n");

        Console.Write("Enter Course Code: ");
        var code = Console.ReadLine()?.Trim() ?? "";

        var course = await _courseService.GetCourseByCodeAsync(code);
        if (course == null)
        {
            Console.WriteLine($"\n✗ Course with code '{code}' not found.");
        }
        else
        {
            Console.WriteLine("\nCourse found:");
            course.DisplayInfo();
        }

        WaitForKey();
    }

    private async Task UpdateCourseAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ UPDATE COURSE ═══\n");

        Console.Write("Enter Course ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("\n✗ Invalid ID.");
            WaitForKey();
            return;
        }

        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
        {
            Console.WriteLine($"\n✗ Course with ID {id} not found.");
            WaitForKey();
            return;
        }

        Console.WriteLine("\nCurrent details:");
        course.DisplayInfo();

        Console.WriteLine("\nEnter new values (leave empty to keep current):\n");

        Console.Write($"Code [{course.Code}]: ");
        var code = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(code)) code = course.Code;

        Console.Write($"Name [{course.Name}]: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(name)) name = course.Name;

        Console.Write($"Credits [{course.Credits}]: ");
        var creditsInput = Console.ReadLine()?.Trim();
        var credits = string.IsNullOrEmpty(creditsInput) ? course.Credits : int.Parse(creditsInput);

        Console.Write($"Description [{course.Description ?? "not set"}]: ");
        var description = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(description)) description = course.Description;

        Console.Write($"Teacher ID [{course.TeacherId?.ToString() ?? "not set"}]: ");
        var teacherInput = Console.ReadLine()?.Trim();
        int? teacherId = string.IsNullOrEmpty(teacherInput) ? course.TeacherId : int.Parse(teacherInput);

        try
        {
            await _courseService.UpdateCourseAsync(id, code, name, credits, description, teacherId);
            Console.WriteLine("\n✓ Course updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }

        WaitForKey();
    }

    private async Task DeleteCourseAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ DELETE COURSE ═══\n");

        Console.Write("Enter Course ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("\n✗ Invalid ID.");
            WaitForKey();
            return;
        }

        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
        {
            Console.WriteLine($"\n✗ Course with ID {id} not found.");
            WaitForKey();
            return;
        }

        Console.WriteLine("\nCourse to delete:");
        course.DisplayInfo();

        Console.Write("\nAre you sure? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();

        if (confirm == "y")
        {
            try
            {
                await _courseService.DeleteCourseAsync(id);
                Console.WriteLine("\n✓ Course deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("\nDeletion cancelled.");
        }

        WaitForKey();
    }

    private async Task AssignTeacherToCourseAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ ASSIGN TEACHER TO COURSE ═══\n");

        Console.Write("Enter Course ID: ");
        if (!int.TryParse(Console.ReadLine(), out var courseId))
        {
            Console.WriteLine("\n✗ Invalid Course ID.");
            WaitForKey();
            return;
        }

        Console.Write("Enter Teacher ID: ");
        if (!int.TryParse(Console.ReadLine(), out var teacherId))
        {
            Console.WriteLine("\n✗ Invalid Teacher ID.");
            WaitForKey();
            return;
        }

        try
        {
            await _courseService.AssignTeacherAsync(courseId, teacherId);
            Console.WriteLine("\n✓ Teacher assigned to course successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }

        WaitForKey();
    }

    #endregion

    #region Enrollment Menu

    private async Task EnrollmentMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("       ENROLLMENT MANAGEMENT");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("  1. Enroll Student in Course");
            Console.WriteLine("  2. View Student's Enrollments");
            Console.WriteLine("  3. View Course Roster");
            Console.WriteLine("  4. Assign/Update Grade");
            Console.WriteLine("  5. Unenroll Student");
            Console.WriteLine("  6. Calculate & Update Student GPA");
            Console.WriteLine("  7. Demonstrate Polymorphism");
            Console.WriteLine("  8. Back to Main Menu");
            Console.WriteLine("═══════════════════════════════════════");
            Console.Write("\nEnter your choice: ");

            var choice = ReadChoice(1, 8);

            switch (choice)
            {
                case 1:
                    await EnrollStudentAsync();
                    break;
                case 2:
                    await ViewStudentEnrollmentsAsync();
                    break;
                case 3:
                    await ViewCourseRosterAsync();
                    break;
                case 4:
                    await AssignGradeAsync();
                    break;
                case 5:
                    await UnenrollStudentAsync();
                    break;
                case 6:
                    await CalculateGpaAsync();
                    break;
                case 7:
                    await DemonstratePolymorphismAsync();
                    break;
                case 8:
                    return;
            }
        }
    }

    private async Task EnrollStudentAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ ENROLL STUDENT ═══\n");

        Console.Write("Enter Student ID: ");
        if (!int.TryParse(Console.ReadLine(), out var studentId))
        {
            Console.WriteLine("\n✗ Invalid Student ID.");
            WaitForKey();
            return;
        }

        Console.Write("Enter Course ID: ");
        if (!int.TryParse(Console.ReadLine(), out var courseId))
        {
            Console.WriteLine("\n✗ Invalid Course ID.");
            WaitForKey();
            return;
        }

        try
        {
            var enrollmentId = await _enrollmentService.EnrollStudentAsync(studentId, courseId);
            Console.WriteLine($"\n✓ Student enrolled successfully. Enrollment ID: {enrollmentId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }

        WaitForKey();
    }

    private async Task ViewStudentEnrollmentsAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ STUDENT ENROLLMENTS ═══\n");

        Console.Write("Enter Student ID: ");
        if (!int.TryParse(Console.ReadLine(), out var studentId))
        {
            Console.WriteLine("\n✗ Invalid Student ID.");
            WaitForKey();
            return;
        }

        try
        {
            var student = await _studentService.GetStudentByIdAsync(studentId);
            if (student != null)
            {
                Console.WriteLine($"\nEnrollments for {student.GetFullName()}:\n");
            }

            var enrollments = await _enrollmentService.GetEnrollmentsByStudentIdAsync(studentId);
            var enrollmentList = enrollments.ToList();

            if (!enrollmentList.Any())
            {
                Console.WriteLine("No enrollments found.");
            }
            else
            {
                foreach (var enrollment in enrollmentList)
                {
                    var courseName = enrollment.Course != null
                        ? $"{enrollment.Course.Code} - {enrollment.Course.Name}"
                        : $"Course #{enrollment.CourseId}";
                    var grade = enrollment.Grade.HasValue
                        ? $"{enrollment.Grade:F2} ({enrollment.GetLetterGrade()})"
                        : "Not graded";

                    Console.WriteLine($"  • {courseName}");
                    Console.WriteLine($"    Grade: {grade}");
                    Console.WriteLine($"    Enrolled: {enrollment.EnrolledAt:yyyy-MM-dd}");
                    Console.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }

        WaitForKey();
    }

    private async Task ViewCourseRosterAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ COURSE ROSTER ═══\n");

        Console.Write("Enter Course ID: ");
        if (!int.TryParse(Console.ReadLine(), out var courseId))
        {
            Console.WriteLine("\n✗ Invalid Course ID.");
            WaitForKey();
            return;
        }

        try
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course != null)
            {
                Console.WriteLine($"\nRoster for {course.Code} - {course.Name}:\n");
            }

            var enrollments = await _enrollmentService.GetEnrollmentsByCourseIdAsync(courseId);
            var enrollmentList = enrollments.ToList();

            if (!enrollmentList.Any())
            {
                Console.WriteLine("No students enrolled.");
            }
            else
            {
                Console.WriteLine($"{"Student",-30} {"Grade",-15} {"Enrolled"}");
                Console.WriteLine(new string('-', 60));

                foreach (var enrollment in enrollmentList)
                {
                    var studentName = enrollment.Student?.GetFullName() ?? $"Student #{enrollment.StudentId}";
                    var grade = enrollment.Grade.HasValue
                        ? $"{enrollment.Grade:F2} ({enrollment.GetLetterGrade()})"
                        : "Not graded";

                    Console.WriteLine($"{studentName,-30} {grade,-15} {enrollment.EnrolledAt:yyyy-MM-dd}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }

        WaitForKey();
    }

    private async Task AssignGradeAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ ASSIGN GRADE ═══\n");

        Console.Write("Enter Student ID: ");
        if (!int.TryParse(Console.ReadLine(), out var studentId))
        {
            Console.WriteLine("\n✗ Invalid Student ID.");
            WaitForKey();
            return;
        }

        Console.Write("Enter Course ID: ");
        if (!int.TryParse(Console.ReadLine(), out var courseId))
        {
            Console.WriteLine("\n✗ Invalid Course ID.");
            WaitForKey();
            return;
        }

        Console.Write("Enter Grade (0.0 - 4.0): ");
        if (!decimal.TryParse(Console.ReadLine(), out var grade))
        {
            Console.WriteLine("\n✗ Invalid grade value.");
            WaitForKey();
            return;
        }

        try
        {
            await _enrollmentService.AssignGradeAsync(studentId, courseId, grade);
            Console.WriteLine("\n✓ Grade assigned successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }

        WaitForKey();
    }

    private async Task UnenrollStudentAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ UNENROLL STUDENT ═══\n");

        Console.Write("Enter Student ID: ");
        if (!int.TryParse(Console.ReadLine(), out var studentId))
        {
            Console.WriteLine("\n✗ Invalid Student ID.");
            WaitForKey();
            return;
        }

        Console.Write("Enter Course ID: ");
        if (!int.TryParse(Console.ReadLine(), out var courseId))
        {
            Console.WriteLine("\n✗ Invalid Course ID.");
            WaitForKey();
            return;
        }

        Console.Write("Are you sure? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();

        if (confirm == "y")
        {
            try
            {
                await _enrollmentService.UnenrollStudentAsync(studentId, courseId);
                Console.WriteLine("\n✓ Student unenrolled successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("\nUnenrollment cancelled.");
        }

        WaitForKey();
    }

    private async Task CalculateGpaAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ CALCULATE GPA ═══\n");

        Console.Write("Enter Student ID: ");
        if (!int.TryParse(Console.ReadLine(), out var studentId))
        {
            Console.WriteLine("\n✗ Invalid Student ID.");
            WaitForKey();
            return;
        }

        try
        {
            var gpa = await _enrollmentService.RecalculateStudentGpaAsync(studentId);
            Console.WriteLine($"\n✓ GPA calculated and updated: {gpa:F2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }

        WaitForKey();
    }

    /// <summary>
    /// Demonstrates POLYMORPHISM by treating Students and Teachers as Person objects.
    /// </summary>
    private async Task DemonstratePolymorphismAsync()
    {
        Console.Clear();
        Console.WriteLine("═══ POLYMORPHISM DEMONSTRATION ═══\n");
        Console.WriteLine("This demonstrates how the same DisplayInfo() method");
        Console.WriteLine("behaves differently for Students and Teachers.\n");

        // Collect some people (both students and teachers)
        var people = new List<Person>();

        var students = await _studentService.GetAllStudentsAsync();
        people.AddRange(students.Take(2));

        var teachers = await _teacherService.GetAllTeachersAsync();
        people.AddRange(teachers.Take(2));

        if (!people.Any())
        {
            Console.WriteLine("No students or teachers found in the system.");
            Console.WriteLine("Add some first to see the polymorphism demo.");
            WaitForKey();
            return;
        }

        Console.WriteLine("Calling DisplayInfo() on a List<Person>:\n");
        Console.WriteLine("─────────────────────────────────────────");

        foreach (var person in people)
        {
            // This is POLYMORPHISM in action!
            // The correct DisplayInfo() is called based on the actual type
            person.DisplayInfo();
            Console.WriteLine();
        }

        Console.WriteLine("─────────────────────────────────────────");
        Console.WriteLine("\nNotice how each type displays its info differently,");
        Console.WriteLine("even though we're iterating over a List<Person>!");

        WaitForKey();
    }

    #endregion

    #region Helper Methods

    private static int ReadChoice(int min, int max)
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (int.TryParse(input, out var choice) && choice >= min && choice <= max)
            {
                return choice;
            }

            Console.Write($"Please enter a number between {min} and {max}: ");
        }
    }

    private static void WaitForKey()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }

    #endregion
}
