using Dapper;
using StudentManagement.Models;

namespace StudentManagement.Data;

/// <summary>
/// Repository for Course CRUD operations.
/// </summary>
public class CourseRepository : IRepository<Course>
{
    private readonly DbConnectionFactory _connectionFactory;

    public CourseRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT c.id, c.code, c.name, c.credits, c.description,
                   c.teacher_id AS TeacherId, c.created_at AS CreatedAt,
                   t.id, t.first_name AS FirstName, t.last_name AS LastName,
                   t.email, t.subject, t.department
            FROM courses c
            LEFT JOIN teachers t ON c.teacher_id = t.id
            ORDER BY c.code";

        var courses = await connection.QueryAsync<Course, Teacher, Course>(
            sql,
            (course, teacher) =>
            {
                course.Teacher = teacher;
                return course;
            },
            splitOn: "id");

        return courses;
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT c.id, c.code, c.name, c.credits, c.description,
                   c.teacher_id AS TeacherId, c.created_at AS CreatedAt,
                   t.id, t.first_name AS FirstName, t.last_name AS LastName,
                   t.email, t.subject, t.department
            FROM courses c
            LEFT JOIN teachers t ON c.teacher_id = t.id
            WHERE c.id = @Id";

        var courses = await connection.QueryAsync<Course, Teacher, Course>(
            sql,
            (course, teacher) =>
            {
                course.Teacher = teacher;
                return course;
            },
            new { Id = id },
            splitOn: "id");

        return courses.FirstOrDefault();
    }

    public async Task<Course?> GetByCodeAsync(string code)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT c.id, c.code, c.name, c.credits, c.description,
                   c.teacher_id AS TeacherId, c.created_at AS CreatedAt,
                   t.id, t.first_name AS FirstName, t.last_name AS LastName,
                   t.email, t.subject, t.department
            FROM courses c
            LEFT JOIN teachers t ON c.teacher_id = t.id
            WHERE c.code = @Code";

        var courses = await connection.QueryAsync<Course, Teacher, Course>(
            sql,
            (course, teacher) =>
            {
                course.Teacher = teacher;
                return course;
            },
            new { Code = code },
            splitOn: "id");

        return courses.FirstOrDefault();
    }

    public async Task<int> CreateAsync(Course entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO courses (code, name, credits, description, teacher_id)
            VALUES (@Code, @Name, @Credits, @Description, @TeacherId)
            RETURNING id";

        return await connection.QuerySingleAsync<int>(sql, new
        {
            entity.Code,
            entity.Name,
            entity.Credits,
            entity.Description,
            entity.TeacherId
        });
    }

    public async Task<bool> UpdateAsync(Course entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE courses
            SET code = @Code,
                name = @Name,
                credits = @Credits,
                description = @Description,
                teacher_id = @TeacherId
            WHERE id = @Id";

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            entity.Id,
            entity.Code,
            entity.Name,
            entity.Credits,
            entity.Description,
            entity.TeacherId
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "DELETE FROM courses WHERE id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}
