using StudentManagement.Data;
using StudentManagement.Models;

namespace StudentManagement.Services;

/// <summary>
/// Service layer for Course business logic.
/// </summary>
public class CourseService
{
    private readonly CourseRepository _courseRepository;
    private readonly TeacherRepository _teacherRepository;

    public CourseService(CourseRepository courseRepository, TeacherRepository teacherRepository)
    {
        _courseRepository = courseRepository;
        _teacherRepository = teacherRepository;
    }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync()
    {
        return await _courseRepository.GetAllAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        return await _courseRepository.GetByIdAsync(id);
    }

    public async Task<Course?> GetCourseByCodeAsync(string code)
    {
        return await _courseRepository.GetByCodeAsync(code);
    }

    public async Task<int> CreateCourseAsync(string code, string name, int credits,
        string? description = null, int? teacherId = null)
    {
        // Check if code already exists
        var existing = await _courseRepository.GetByCodeAsync(code);
        if (existing != null)
        {
            throw new InvalidOperationException($"A course with code '{code}' already exists.");
        }

        // Validate teacher if provided
        if (teacherId.HasValue)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId.Value);
            if (teacher == null)
            {
                throw new InvalidOperationException($"Teacher with ID {teacherId} not found.");
            }
        }

        // Validate credits
        if (credits <= 0)
        {
            throw new ArgumentException("Credits must be greater than 0.");
        }

        var course = new Course
        {
            Code = code.ToUpper(),
            Name = name,
            Credits = credits,
            Description = description,
            TeacherId = teacherId
        };

        return await _courseRepository.CreateAsync(course);
    }

    public async Task<bool> UpdateCourseAsync(int id, string code, string name, int credits,
        string? description, int? teacherId)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
        {
            throw new InvalidOperationException($"Course with ID {id} not found.");
        }

        // Check if new code conflicts with another course
        if (course.Code != code.ToUpper())
        {
            var existing = await _courseRepository.GetByCodeAsync(code);
            if (existing != null && existing.Id != id)
            {
                throw new InvalidOperationException($"A course with code '{code}' already exists.");
            }
        }

        // Validate teacher if provided
        if (teacherId.HasValue)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId.Value);
            if (teacher == null)
            {
                throw new InvalidOperationException($"Teacher with ID {teacherId} not found.");
            }
        }

        // Validate credits
        if (credits <= 0)
        {
            throw new ArgumentException("Credits must be greater than 0.");
        }

        course.Code = code.ToUpper();
        course.Name = name;
        course.Credits = credits;
        course.Description = description;
        course.TeacherId = teacherId;

        return await _courseRepository.UpdateAsync(course);
    }

    public async Task<bool> DeleteCourseAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
        {
            throw new InvalidOperationException($"Course with ID {id} not found.");
        }

        return await _courseRepository.DeleteAsync(id);
    }

    public async Task<bool> AssignTeacherAsync(int courseId, int teacherId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new InvalidOperationException($"Course with ID {courseId} not found.");
        }

        var teacher = await _teacherRepository.GetByIdAsync(teacherId);
        if (teacher == null)
        {
            throw new InvalidOperationException($"Teacher with ID {teacherId} not found.");
        }

        course.TeacherId = teacherId;
        return await _courseRepository.UpdateAsync(course);
    }
}
