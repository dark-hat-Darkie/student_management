namespace StudentManagement.Models;

/// <summary>
/// Enrollment entity representing a student's enrollment in a course.
/// This is a junction table for the many-to-many relationship.
/// </summary>
public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public decimal? Grade { get; set; }
    public DateTime EnrolledAt { get; set; }

    /// <summary>
    /// Navigation property for the student.
    /// </summary>
    public Student? Student { get; set; }

    /// <summary>
    /// Navigation property for the course.
    /// </summary>
    public Course? Course { get; set; }

    /// <summary>
    /// Get letter grade from numeric grade.
    /// </summary>
    public string GetLetterGrade()
    {
        if (!Grade.HasValue) return "N/A";

        return Grade.Value switch
        {
            >= 3.7m => "A",
            >= 3.3m => "A-",
            >= 3.0m => "B+",
            >= 2.7m => "B",
            >= 2.3m => "B-",
            >= 2.0m => "C+",
            >= 1.7m => "C",
            >= 1.3m => "C-",
            >= 1.0m => "D",
            _ => "F"
        };
    }

    /// <summary>
    /// Display enrollment information.
    /// </summary>
    public void DisplayInfo()
    {
        var studentName = Student?.GetFullName() ?? $"Student #{StudentId}";
        var courseName = Course != null ? $"{Course.Code} - {Course.Name}" : $"Course #{CourseId}";
        var gradeInfo = Grade.HasValue ? $"{Grade:F2} ({GetLetterGrade()})" : "Not graded";

        Console.WriteLine($"  {studentName} -> {courseName}");
        Console.WriteLine($"    Grade: {gradeInfo}");
        Console.WriteLine($"    Enrolled: {EnrolledAt:yyyy-MM-dd}");
    }
}
