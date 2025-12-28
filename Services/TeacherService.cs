using StudentManagement.Data;
using StudentManagement.Models;

namespace StudentManagement.Services;

/// <summary>
/// Service layer for Teacher business logic.
/// </summary>
public class TeacherService
{
    private readonly TeacherRepository _teacherRepository;

    public TeacherService(TeacherRepository teacherRepository)
    {
        _teacherRepository = teacherRepository;
    }

    public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
    {
        return await _teacherRepository.GetAllAsync();
    }

    public async Task<Teacher?> GetTeacherByIdAsync(int id)
    {
        return await _teacherRepository.GetByIdAsync(id);
    }

    public async Task<Teacher?> GetTeacherByEmailAsync(string email)
    {
        return await _teacherRepository.GetByEmailAsync(email);
    }

    public async Task<int> CreateTeacherAsync(string firstName, string lastName, string email,
        string subject, string? department = null, DateOnly? dateOfBirth = null)
    {
        // Check if email already exists
        var existing = await _teacherRepository.GetByEmailAsync(email);
        if (existing != null)
        {
            throw new InvalidOperationException($"A teacher with email '{email}' already exists.");
        }

        var teacher = new Teacher
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Subject = subject,
            Department = department,
            DateOfBirth = dateOfBirth
        };

        return await _teacherRepository.CreateAsync(teacher);
    }

    public async Task<bool> UpdateTeacherAsync(int id, string firstName, string lastName, string email,
        string subject, string? department, DateOnly? dateOfBirth)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        if (teacher == null)
        {
            throw new InvalidOperationException($"Teacher with ID {id} not found.");
        }

        // Check if new email conflicts with another teacher
        if (teacher.Email != email)
        {
            var existing = await _teacherRepository.GetByEmailAsync(email);
            if (existing != null && existing.Id != id)
            {
                throw new InvalidOperationException($"A teacher with email '{email}' already exists.");
            }
        }

        teacher.FirstName = firstName;
        teacher.LastName = lastName;
        teacher.Email = email;
        teacher.Subject = subject;
        teacher.Department = department;
        teacher.DateOfBirth = dateOfBirth;

        return await _teacherRepository.UpdateAsync(teacher);
    }

    public async Task<bool> DeleteTeacherAsync(int id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        if (teacher == null)
        {
            throw new InvalidOperationException($"Teacher with ID {id} not found.");
        }

        return await _teacherRepository.DeleteAsync(id);
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
