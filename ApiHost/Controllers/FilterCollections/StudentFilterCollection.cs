﻿using PredefinedFilterDemo.Dtos;
using PredefinedFilterDemo.Dtos.School;
using PredefinedFilterDemo.Filters;
using PredefinedFilterDemo.Filters.Infrastructure;

namespace PredefinedFilterDemo.Controllers.FilterCollections;

public enum StudentFilterType
{
    [ParameterPattern(Pattern = "(DateTime)from|(DateTime)to")]
    Birthday,
    [ParameterPattern(Pattern = "(int)min|(int)max", Example = "80")]
    BestScore
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
                case StudentFilterType.BestScore:
                    collection._filters.Add(NumberFilter<Student>.FromTo(filterStr, s => s.Exams.Max(e=>e.Score)));
                    break;
            }
        }

        return collection;
    }
}