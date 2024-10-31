using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PredefinedFilterDemo.Dtos.School;

public class Semester
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }

    public ICollection<SemesterCourse>? Course { get; set; }
}
