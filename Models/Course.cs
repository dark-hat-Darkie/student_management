namespace StudentManagement.Models;

/// <summary>
/// Course entity representing a course in the system.
/// </summary>
public class Course
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string? Description { get; set; }
    public int? TeacherId { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Navigation property for the assigned teacher.
    /// </summary>
    public Teacher? Teacher { get; set; }

    /// <summary>
    /// Navigation property for enrollments.
    /// </summary>
    public List<Enrollment>? Enrollments { get; set; }

    /// <summary>
    /// Display course information.
    /// </summary>
    public void DisplayInfo()
    {
        Console.WriteLine($"  [{Code}] {Name}");
        Console.WriteLine($"    ID: {Id}");
        Console.WriteLine($"    Credits: {Credits}");
        if (!string.IsNullOrEmpty(Description))
            Console.WriteLine($"    Description: {Description}");
        if (Teacher != null)
            Console.WriteLine($"    Instructor: {Teacher.GetFullName()}");
        else if (TeacherId.HasValue)
            Console.WriteLine($"    Instructor ID: {TeacherId}");
    }
}
