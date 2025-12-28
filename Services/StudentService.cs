using StudentManagement.Data;
using StudentManagement.Models;

namespace StudentManagement.Services;

/// <summary>
/// Service layer for Student business logic.
/// </summary>
public class StudentService
{
    private readonly StudentRepository _studentRepository;
    private readonly EnrollmentRepository _enrollmentRepository;

    public StudentService(StudentRepository studentRepository, EnrollmentRepository enrollmentRepository)
    {
        _studentRepository = studentRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        return await _studentRepository.GetAllAsync();
    }

    public async Task<Student?> GetStudentByIdAsync(int id)
    {
        return await _studentRepository.GetByIdAsync(id);
    }

    public async Task<Student?> GetStudentByEmailAsync(string email)
    {
        return await _studentRepository.GetByEmailAsync(email);
    }

    public async Task<int> CreateStudentAsync(string firstName, string lastName, string email, DateTime? dateOfBirth = null)
    {
        // Check if email already exists
        var existing = await _studentRepository.GetByEmailAsync(email);
        if (existing != null)
        {
            throw new InvalidOperationException($"A student with email '{email}' already exists.");
        }

        var student = new Student
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DateOfBirth = dateOfBirth
        };

        return await _studentRepository.CreateAsync(student);
    }

    public async Task<bool> UpdateStudentAsync(int id, string firstName, string lastName, string email, DateTime? dateOfBirth)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null)
        {
            throw new InvalidOperationException($"Student with ID {id} not found.");
        }

        // Check if new email conflicts with another student
        if (student.Email != email)
        {
            var existing = await _studentRepository.GetByEmailAsync(email);
            if (existing != null && existing.Id != id)
            {
                throw new InvalidOperationException($"A student with email '{email}' already exists.");
            }
        }

        student.FirstName = firstName;
        student.LastName = lastName;
        student.Email = email;
        student.DateOfBirth = dateOfBirth;

        return await _studentRepository.UpdateAsync(student);
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null)
        {
            throw new InvalidOperationException($"Student with ID {id} not found.");
        }

        return await _studentRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Calculate and update GPA for a student based on their enrollments.
    /// </summary>
    public async Task<decimal> CalculateAndUpdateGpaAsync(int studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null)
        {
            throw new InvalidOperationException($"Student with ID {studentId} not found.");
        }

        var enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId);
        student.CalculateGpaFromEnrollments(enrollments);

        await _studentRepository.UpdateGpaAsync(studentId, student.GPA);

        return student.GPA;
    }

    /// <summary>
    /// Demonstrate polymorphism by displaying info for any Person.
    /// </summary>
    public void DisplayPersonInfo(Person person)
    {
        // This demonstrates polymorphism - the correct DisplayInfo() is called
        // based on the actual type (Student or Teacher)
        Console.WriteLine();
        person.DisplayInfo();
        Console.WriteLine();
    }
}
