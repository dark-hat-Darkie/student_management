namespace StudentManagement.Models;

/// <summary>
/// Abstract base class demonstrating ABSTRACTION in OOP.
/// Defines common properties and behaviors for all person types.
/// </summary>
public abstract class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Abstract method that must be implemented by derived classes (POLYMORPHISM).
    /// Each derived class will display info differently.
    /// </summary>
    public abstract void DisplayInfo();

    /// <summary>
    /// Concrete method available to all derived classes.
    /// </summary>
    public string GetFullName() => $"{FirstName} {LastName}";

    /// <summary>
    /// Calculate age from date of birth.
    /// </summary>
    public int? GetAge()
    {
        if (!DateOfBirth.HasValue) return null;

        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - DateOfBirth.Value.Year;
        if (DateOfBirth.Value > today.AddYears(-age)) age--;
        return age;
    }
}
