using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PredefinedFilterDemo.Dtos.School;

public class ExamResult
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Student Student { get; set; }

    public Exam Exam { get; set; }

    public double Score { get; set; }

    public string? Comment { get; set; }
}
