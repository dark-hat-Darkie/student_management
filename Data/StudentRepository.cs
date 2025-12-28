using Dapper;
using StudentManagement.Models;

namespace StudentManagement.Data;

/// <summary>
/// Repository for Student CRUD operations.
/// </summary>
public class StudentRepository : IRepository<Student>
{
    private readonly DbConnectionFactory _connectionFactory;

    public StudentRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, first_name AS FirstName, last_name AS LastName,
                   email, date_of_birth AS DateOfBirth, gpa, created_at AS CreatedAt
            FROM students
            ORDER BY last_name, first_name";

        var students = await connection.QueryAsync<Student>(sql);

        // Set GPA through the public method to maintain encapsulation
        foreach (var student in students)
        {
            var gpa = await connection.QuerySingleOrDefaultAsync<decimal>(
                "SELECT gpa FROM students WHERE id = @Id", new { student.Id });
            student.SetGpa(gpa);
        }

        return students;
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, first_name AS FirstName, last_name AS LastName,
                   email, date_of_birth AS DateOfBirth, gpa, created_at AS CreatedAt
            FROM students
            WHERE id = @Id";

        var student = await connection.QuerySingleOrDefaultAsync<Student>(sql, new { Id = id });

        if (student != null)
        {
            student.SetGpa(await connection.QuerySingleOrDefaultAsync<decimal>(
                "SELECT gpa FROM students WHERE id = @Id", new { Id = id }));
        }

        return student;
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, first_name AS FirstName, last_name AS LastName,
                   email, date_of_birth AS DateOfBirth, gpa, created_at AS CreatedAt
            FROM students
            WHERE email = @Email";

        var student = await connection.QuerySingleOrDefaultAsync<Student>(sql, new { Email = email });

        if (student != null)
        {
            student.SetGpa(await connection.QuerySingleOrDefaultAsync<decimal>(
                "SELECT gpa FROM students WHERE id = @Id", new { student.Id }));
        }

        return student;
    }

    public async Task<int> CreateAsync(Student entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO students (first_name, last_name, email, date_of_birth, gpa)
            VALUES (@FirstName, @LastName, @Email, @DateOfBirth, @GPA)
            RETURNING id";

        return await connection.QuerySingleAsync<int>(sql, new
        {
            entity.FirstName,
            entity.LastName,
            entity.Email,
            entity.DateOfBirth,
            entity.GPA
        });
    }

    public async Task<bool> UpdateAsync(Student entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE students
            SET first_name = @FirstName,
                last_name = @LastName,
                email = @Email,
                date_of_birth = @DateOfBirth,
                gpa = @GPA
            WHERE id = @Id";

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.Email,
            entity.DateOfBirth,
            entity.GPA
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "DELETE FROM students WHERE id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateGpaAsync(int studentId, decimal gpa)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE students SET gpa = @Gpa WHERE id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = studentId, Gpa = gpa });
        return rowsAffected > 0;
    }
}
