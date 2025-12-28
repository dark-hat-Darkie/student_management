namespace StudentManagement.Models;

/// <summary>
/// Student class demonstrating INHERITANCE (extends Person) and ENCAPSULATION (private _gpa field).
/// </summary>
public class Student : Person
{
    private decimal _gpa;

    /// <summary>
    /// GPA property with ENCAPSULATION - private setter with validation.
    /// GPA can only be modified through SetGpa method which enforces business rules.
    /// </summary>
    public decimal GPA
    {
        get => _gpa;
        private set
        {
            if (value < 0 || value > 4.0m)
                throw new ArgumentException("GPA must be between 0.0 and 4.0");
            _gpa = Math.Round(value, 2);
        }
    }

    /// <summary>
    /// Navigation property for enrollments (not loaded from DB by default).
    /// </summary>
    public List<Enrollment>? Enrollments { get; set; }

    /// <summary>
    /// Public method to set GPA - demonstrates ENCAPSULATION.
    /// The internal validation logic is hidden from external code.
    /// </summary>
    public void SetGpa(decimal gpa)
    {
        GPA = gpa;
    }

    /// <summary>
    /// Calculate GPA from enrollments - demonstrates ENCAPSULATION.
    /// Business logic for GPA calculation is encapsulated within the Student class.
    /// </summary>
    public void CalculateGpaFromEnrollments(IEnumerable<Enrollment> enrollments)
    {
        var gradedEnrollments = enrollments.Where(e => e.Grade.HasValue).ToList();

        if (!gradedEnrollments.Any())
        {
            GPA = 0;
            return;
        }

        // Simple average - in a real system you'd weight by credits
        var totalGrade = gradedEnrollments.Sum(e => e.Grade!.Value);
        GPA = totalGrade / gradedEnrollments.Count;
    }

    /// <summary>
    /// POLYMORPHISM - Override of abstract method from Person.
    /// Student displays info differently than Teacher.
    /// </summary>
    public override void DisplayInfo()
    {
        Console.WriteLine($"  [STUDENT] {GetFullName()}");
        Console.WriteLine($"    ID: {Id}");
        Console.WriteLine($"    Email: {Email}");
        Console.WriteLine($"    GPA: {GPA:F2}");
        if (DateOfBirth.HasValue)
            Console.WriteLine($"    Age: {GetAge()} years old");
    }
}
