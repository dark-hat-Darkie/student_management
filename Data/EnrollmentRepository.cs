using Dapper;
using StudentManagement.Models;

namespace StudentManagement.Data;

/// <summary>
/// Repository for Enrollment CRUD operations.
/// </summary>
public class EnrollmentRepository : IRepository<Enrollment>
{
    private readonly DbConnectionFactory _connectionFactory;

    public EnrollmentRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Enrollment>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT e.id, e.student_id AS StudentId, e.course_id AS CourseId,
                   e.grade, e.enrolled_at AS EnrolledAt
            FROM enrollments e
            ORDER BY e.enrolled_at DESC";

        return await connection.QueryAsync<Enrollment>(sql);
    }

    public async Task<Enrollment?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT e.id, e.student_id AS StudentId, e.course_id AS CourseId,
                   e.grade, e.enrolled_at AS EnrolledAt
            FROM enrollments e
            WHERE e.id = @Id";

        return await connection.QuerySingleOrDefaultAsync<Enrollment>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT e.id, e.student_id AS StudentId, e.course_id AS CourseId,
                   e.grade, e.enrolled_at AS EnrolledAt,
                   c.id, c.code, c.name, c.credits, c.description
            FROM enrollments e
            INNER JOIN courses c ON e.course_id = c.id
            WHERE e.student_id = @StudentId
            ORDER BY e.enrolled_at DESC";

        var enrollments = await connection.QueryAsync<Enrollment, Course, Enrollment>(
            sql,
            (enrollment, course) =>
            {
                enrollment.Course = course;
                return enrollment;
            },
            new { StudentId = studentId },
            splitOn: "id");

        return enrollments;
    }

    public async Task<IEnumerable<Enrollment>> GetByCourseIdAsync(int courseId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT e.id, e.student_id AS StudentId, e.course_id AS CourseId,
                   e.grade, e.enrolled_at AS EnrolledAt,
                   s.id, s.first_name AS FirstName, s.last_name AS LastName,
                   s.email, s.gpa
            FROM enrollments e
            INNER JOIN students s ON e.student_id = s.id
            WHERE e.course_id = @CourseId
            ORDER BY s.last_name, s.first_name";

        var enrollments = await connection.QueryAsync<Enrollment, Student, Enrollment>(
            sql,
            (enrollment, student) =>
            {
                enrollment.Student = student;
                return enrollment;
            },
            new { CourseId = courseId },
            splitOn: "id");

        return enrollments;
    }

    public async Task<Enrollment?> GetByStudentAndCourseAsync(int studentId, int courseId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT id, student_id AS StudentId, course_id AS CourseId,
                   grade, enrolled_at AS EnrolledAt
            FROM enrollments
            WHERE student_id = @StudentId AND course_id = @CourseId";

        return await connection.QuerySingleOrDefaultAsync<Enrollment>(sql,
            new { StudentId = studentId, CourseId = courseId });
    }

    public async Task<int> CreateAsync(Enrollment entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO enrollments (student_id, course_id, grade)
            VALUES (@StudentId, @CourseId, @Grade)
            RETURNING id";

        return await connection.QuerySingleAsync<int>(sql, new
        {
            entity.StudentId,
            entity.CourseId,
            entity.Grade
        });
    }

    public async Task<bool> UpdateAsync(Enrollment entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE enrollments
            SET grade = @Grade
            WHERE id = @Id";

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            entity.Id,
            entity.Grade
        });

        return rowsAffected > 0;
    }

    public async Task<bool> UpdateGradeAsync(int enrollmentId, decimal grade)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE enrollments SET grade = @Grade WHERE id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = enrollmentId, Grade = grade });
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "DELETE FROM enrollments WHERE id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteByStudentAndCourseAsync(int studentId, int courseId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "DELETE FROM enrollments WHERE student_id = @StudentId AND course_id = @CourseId";
        var rowsAffected = await connection.ExecuteAsync(sql,
            new { StudentId = studentId, CourseId = courseId });
        return rowsAffected > 0;
    }
}
