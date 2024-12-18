﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PredefinedFilterDemo.Dtos.School;

public class Student
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Birthday { get; set; }

    public DateTime EntranceTime { get; set; }

    public DateTime? GraduationTime { get; set; }

    public bool Graduated => GraduationTime != null;

    public ICollection<SemesterCourse> Courses { get; set; }

    public ICollection<ExamResult>? Exams { get; set; }

}
