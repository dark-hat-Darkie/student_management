using StudentManagement.Data;
using StudentManagement.Models;

namespace StudentManagement.Services;

/// <summary>
/// Service layer for Enrollment business logic.
/// </summary>
public class EnrollmentService
{
    private readonly EnrollmentRepository _enrollmentRepository;
    private readonly StudentRepository _studentRepository;
    private readonly CourseRepository _courseRepository;

    public EnrollmentService(EnrollmentRepository enrollmentRepository,
        StudentRepository studentRepository, CourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync()
    {
        return await _enrollmentRepository.GetAllAsync();
    }

    public async Task<Enrollment?> GetEnrollmentByIdAsync(int id)
    {
        return await _enrollmentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(int studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null)
        {
            throw new InvalidOperationException($"Student with ID {studentId} not found.");
        }

        return await _enrollmentRepository.GetByStudentIdAsync(studentId);
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(int courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new InvalidOperationException($"Course with ID {courseId} not found.");
        }

        return await _enrollmentRepository.GetByCourseIdAsync(courseId);
    }

    public async Task<int> EnrollStudentAsync(int studentId, int courseId)
    {
        // Validate student exists
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null)
        {
            throw new InvalidOperationException($"Student with ID {studentId} not found.");
        }

        // Validate course exists
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new InvalidOperationException($"Course with ID {courseId} not found.");
        }

        // Check if already enrolled
        var existing = await _enrollmentRepository.GetByStudentAndCourseAsync(studentId, courseId);
        if (existing != null)
        {
            throw new InvalidOperationException(
                $"Student '{student.GetFullName()}' is already enrolled in '{course.Code}'.");
        }

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId
        };

        return await _enrollmentRepository.CreateAsync(enrollment);
    }

    public async Task<bool> AssignGradeAsync(int studentId, int courseId, decimal grade)
    {
        // Validate grade
        if (grade < 0 || grade > 4.0m)
        {
            throw new ArgumentException("Grade must be between 0.0 and 4.0.");
        }

        // Find enrollment
        var enrollment = await _enrollmentRepository.GetByStudentAndCourseAsync(studentId, courseId);
        if (enrollment == null)
        {
            throw new InvalidOperationException(
                $"No enrollment found for student {studentId} in course {courseId}.");
        }

        enrollment.Grade = grade;
        return await _enrollmentRepository.UpdateAsync(enrollment);
    }

    public async Task<bool> UpdateGradeByEnrollmentIdAsync(int enrollmentId, decimal grade)
    {
        // Validate grade
        if (grade < 0 || grade > 4.0m)
        {
            throw new ArgumentException("Grade must be between 0.0 and 4.0.");
        }

        var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
        if (enrollment == null)
        {
            throw new InvalidOperationException($"Enrollment with ID {enrollmentId} not found.");
        }

        return await _enrollmentRepository.UpdateGradeAsync(enrollmentId, grade);
    }

    public async Task<bool> UnenrollStudentAsync(int studentId, int courseId)
    {
        // Validate student exists
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null)
        {
            throw new InvalidOperationException($"Student with ID {studentId} not found.");
        }

        // Validate course exists
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new InvalidOperationException($"Course with ID {courseId} not found.");
        }

        // Check if enrolled
        var enrollment = await _enrollmentRepository.GetByStudentAndCourseAsync(studentId, courseId);
        if (enrollment == null)
        {
            throw new InvalidOperationException(
                $"Student '{student.GetFullName()}' is not enrolled in '{course.Code}'.");
        }

        return await _enrollmentRepository.DeleteByStudentAndCourseAsync(studentId, courseId);
    }

    /// <summary>
    /// Calculate and update GPA for a student based on their enrollments.
    /// </summary>
    public async Task<decimal> RecalculateStudentGpaAsync(int studentId)
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
}
