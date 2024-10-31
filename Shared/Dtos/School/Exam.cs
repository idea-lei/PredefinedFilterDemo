using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PredefinedFilterDemo.Dtos.School;

public class Exam
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(SemesterCourse))]
    public int SemesterCourseId { get; set; }
    public SemesterCourse SemesterCourse { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan Duration { get; set; }
    public ICollection<ExamResult>? ExamResults { get; set; }
}
