namespace StudentManagement.Models;

/// <summary>
/// Teacher class demonstrating INHERITANCE (extends Person) and POLYMORPHISM.
/// </summary>
public class Teacher : Person
{
    public string Subject { get; set; } = string.Empty;
    public string? Department { get; set; }

    /// <summary>
    /// Navigation property for courses taught (not loaded from DB by default).
    /// </summary>
    public List<Course>? Courses { get; set; }

    /// <summary>
    /// POLYMORPHISM - Override of abstract method from Person.
    /// Teacher displays info differently than Student.
    /// </summary>
    public override void DisplayInfo()
    {
        Console.WriteLine($"  [TEACHER] {GetFullName()}");
        Console.WriteLine($"    ID: {Id}");
        Console.WriteLine($"    Email: {Email}");
        Console.WriteLine($"    Subject: {Subject}");
        if (!string.IsNullOrEmpty(Department))
            Console.WriteLine($"    Department: {Department}");
        if (DateOfBirth.HasValue)
            Console.WriteLine($"    Age: {GetAge()} years old");
    }

    /// <summary>
    /// Teacher-specific method - demonstrates that derived classes can have unique behaviors.
    /// </summary>
    public string GetTeachingInfo()
    {
        var deptInfo = string.IsNullOrEmpty(Department) ? "" : $" ({Department})";
        return $"{Subject}{deptInfo}";
    }
}
