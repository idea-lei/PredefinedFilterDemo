using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PredefinedFilterDemo.Dtos.School;

public class Teacher
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime EmploymentFrom { get; set; }
    public ICollection<SemesterCourse>? Courses { get; set; }
}
