using Microsoft.EntityFrameworkCore;
using PredefinedFilterDemo.Dtos.School;

namespace PredefinedFilterDemo.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<ExamResult> ExamResults { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Semester> Semesters { get; set; }
    public DbSet<SemesterCourse> SemesterCourses { get; set; }
}
