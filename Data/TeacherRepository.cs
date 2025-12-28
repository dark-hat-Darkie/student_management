using Dapper;
using StudentManagement.Models;

namespace StudentManagement.Data;

/// <summary>
/// Repository for Teacher CRUD operations.
/// </summary>
public class TeacherRepository : IRepository<Teacher>
{
    private readonly DbConnectionFactory _connectionFactory;

    public TeacherRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Teacher>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, first_name AS FirstName, last_name AS LastName,
                   email, date_of_birth AS DateOfBirth, subject, department, created_at AS CreatedAt
            FROM teachers
            ORDER BY last_name, first_name";

        return await connection.QueryAsync<Teacher>(sql);
    }

    public async Task<Teacher?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, first_name AS FirstName, last_name AS LastName,
                   email, date_of_birth AS DateOfBirth, subject, department, created_at AS CreatedAt
            FROM teachers
            WHERE id = @Id";

        return await connection.QuerySingleOrDefaultAsync<Teacher>(sql, new { Id = id });
    }

    public async Task<Teacher?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, first_name AS FirstName, last_name AS LastName,
                   email, date_of_birth AS DateOfBirth, subject, department, created_at AS CreatedAt
            FROM teachers
            WHERE email = @Email";

        return await connection.QuerySingleOrDefaultAsync<Teacher>(sql, new { Email = email });
    }

    public async Task<int> CreateAsync(Teacher entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO teachers (first_name, last_name, email, date_of_birth, subject, department)
            VALUES (@FirstName, @LastName, @Email, @DateOfBirth, @Subject, @Department)
            RETURNING id";

        return await connection.QuerySingleAsync<int>(sql, new
        {
            entity.FirstName,
            entity.LastName,
            entity.Email,
            entity.DateOfBirth,
            entity.Subject,
            entity.Department
        });
    }

    public async Task<bool> UpdateAsync(Teacher entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE teachers
            SET first_name = @FirstName,
                last_name = @LastName,
                email = @Email,
                date_of_birth = @DateOfBirth,
                subject = @Subject,
                department = @Department
            WHERE id = @Id";

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.Email,
            entity.DateOfBirth,
            entity.Subject,
            entity.Department
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "DELETE FROM teachers WHERE id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}
