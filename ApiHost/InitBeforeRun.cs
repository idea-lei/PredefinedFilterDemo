using PredefinedFilterDemo.Data;
using PredefinedFilterDemo.Dtos.School;

namespace PredefinedFilterDemo;

static class InitBeforeRun
{
    public static void SeedDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (context.Semesters.Any())
            return;

        Random rand = new Random();

        // Create Semesters
        var semesters = new List<Semester>();
        DateTime semesterStartDate = new DateTime(2020, 9, 1);

        for (int i = 0; i < 10; i++)
        {
            var fromDate = semesterStartDate.AddMonths(i * 6);
            var toDate = fromDate.AddMonths(6).AddDays(-1);

            semesters.Add(new Semester
            {
                From = fromDate,
                To = toDate
            });
        }
        context.Semesters.AddRange(semesters);
        context.SaveChanges();

        // Create Teachers
        var teachers = new List<Teacher>();

        for (int i = 0; i < 5; i++)
        {
            teachers.Add(new Teacher
            {
                Name = $"Teacher {i + 1}",
                EmploymentFrom = new DateTime(2019, 1, 1).AddMonths(i * 6)
            });
        }
        context.Teachers.AddRange(teachers);
        context.SaveChanges();

        // Create Courses
        var courses = new List<Course>();

        for (int i = 0; i < 10; i++)
        {
            courses.Add(new Course
            {
                Name = $"Course {i + 1}",
                Description = $"Description for Course {i + 1}"
            });
        }
        context.Courses.AddRange(courses);
        context.SaveChanges();

        // Assign Courses to Teachers
        var courseTeacherMap = new Dictionary<int, int>(); // CourseId -> TeacherId

        for (int i = 0; i < courses.Count; i++)
        {
            var teacherId = teachers[i / 2].Id; // Each teacher teaches 2 courses
            courseTeacherMap.Add(courses[i].Id, teacherId);
        }

        // Create SemesterCourses and Exams
        var semesterCourses = new List<SemesterCourse>();
        var exams = new List<Exam>();

        foreach (var semester in semesters)
        {
            foreach (var course in courses)
            {
                var teacherId = courseTeacherMap[course.Id];
                var teacher = teachers.First(t => t.Id == teacherId);

                var semesterCourse = new SemesterCourse
                {
                    Course = course,
                    Semester = semester,
                    Teacher = teacher
                };

                // Create Exam for this SemesterCourse
                var examDate = semester.To.AddDays(-7);

                var exam = new Exam
                {
                    SemesterCourse = semesterCourse,
                    StartTime = examDate.AddHours(9),
                    Duration = TimeSpan.FromHours(3)
                };

                semesterCourse.Exam = exam;
                exams.Add(exam);
                semesterCourses.Add(semesterCourse);
            }
        }
        context.SemesterCourses.AddRange(semesterCourses);
        context.Exams.AddRange(exams);
        context.SaveChanges();

        // Create Students
        var students = new List<Student>();

        for (int i = 0; i < 1000; i++)
        {
            var birthday = new DateTime(1995, 1, 1).AddDays(rand.Next(365 * 10));

            students.Add(new Student
            {
                Name = $"Student {i + 1}",
                Birthday = birthday
            });
        }
        context.Students.AddRange(students);
        context.SaveChanges();

        // Assign Students to SemesterCourses and Create ExamResults
        var examResults = new List<ExamResult>();

        foreach (var student in students)
        {
            // Assign 5 random SemesterCourses to each student
            var randomSemesterCourses = semesterCourses.OrderBy(x => rand.Next()).Take(5).ToList();

            foreach (var semesterCourse in randomSemesterCourses)
            {
                // Add student to SemesterCourse
                if (semesterCourse.Students == null)
                    semesterCourse.Students = new List<Student>();
                semesterCourse.Students.Add(student);

                // Create ExamResult
                var examResult = new ExamResult
                {
                    Student = student,
                    Exam = semesterCourse.Exam,
                    Score = rand.NextDouble() * 100,
                    Comment = "Generated Score"
                };
                examResults.Add(examResult);

                // Add ExamResult to Exam
                if (semesterCourse.Exam.ExamResults == null)
                    semesterCourse.Exam.ExamResults = new List<ExamResult>();
                semesterCourse.Exam.ExamResults.Add(examResult);
            }
        }
        context.ExamResults.AddRange(examResults);
        context.SaveChanges();
    }
}
