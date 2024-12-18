﻿using PredefinedFilterDemo.Dtos;
using PredefinedFilterDemo.Dtos.School;
using PredefinedFilterDemo.Filters;
using PredefinedFilterDemo.Filters.Infrastructure;
using System.ComponentModel;

namespace PredefinedFilterDemo.Controllers.FilterCollections;

public enum StudentFilterType
{
    [ParameterPattern(Pattern = "(DateTime)from|(DateTime)to", Example = "2000-01-01|2010-01-01")]
    Birthday,

    [ParameterPattern(Pattern = "(int)score", Example = "99")]
    [Description("The best score of all exams should greater than or equal to..")]
    MinBestScore,

    [ParameterPattern(Pattern = "(int)course amount", Example = "8")]
    [Description("The min courses taken")]
    MinCourses,

    [ParameterPattern(Pattern = "(DateTime)from|(DateTime)to", Example = "2000-01-01|2010-01-01")]
    [Description("Graduated in the time range")]
    GraduationDate,
}

public class StudentFilterCollection : IFilterCollection<Student>
{
    private StudentFilterCollection() { }

    public static IEnumerable<FilterDescriptorDto> FilterDescriptors => FilterDescriptorHelper.GetDescriptorDtos<StudentFilterType>();

    private List<BaseFilter<Student>> _filters = [];
    public IEnumerable<BaseFilter<Student>> FilterCollection => _filters;

    public static IFilterCollection<Student> Parse(string[]? filters, params object?[]? args)
    {
        var collection = new StudentFilterCollection();
        if(filters == null)
            return collection;

        foreach(var filterStr in filters)
        {
            var parts = filterStr.Split('|');
            if (!Enum.TryParse<StudentFilterType>(parts[0], true, out var type))
                continue;

            switch (type) { 
                case StudentFilterType.Birthday:
                    collection._filters.Add(DateTimeFilter<Student>.FromTo(filterStr, s => s.Birthday));
                    break;
                case StudentFilterType.MinBestScore:
                    collection._filters.Add(NumberFilter<Student>.GreaterThanOrEqual(filterStr, s => s.Exams!.Max(e=>e.Score)));
                    break;
                case StudentFilterType.MinCourses:
                    collection._filters.Add(NumberFilter<Student>.GreaterThanOrEqual(filterStr, s => s.Courses.Count()));
                    break;
                case StudentFilterType.GraduationDate:
                    collection._filters.Add(DateTimeFilter<Student>.FromTo(filterStr, s => s.GraduationTime!.Value));
                    break;
            }
        }

        return collection;
    }
}
