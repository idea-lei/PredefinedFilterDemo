using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PredefinedFilterDemo.Dtos.School;

public class SemesterCourse
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Course Course { get; set; }

    public Semester Semester { get; set; }

    public Teacher Teacher { get; set; }

    /// <summary>
    /// Course may not have exam
    /// </summary>
    public Exam? Exam { get; set; }

    public ICollection<Student>? Students { get; set; }
}
