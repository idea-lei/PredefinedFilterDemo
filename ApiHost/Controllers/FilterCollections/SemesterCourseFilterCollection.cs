using PredefinedFilterDemo.Dtos.School;
using PredefinedFilterDemo.Dtos;
using PredefinedFilterDemo.Filters;
using PredefinedFilterDemo.Filters.Infrastructure;
using System.ComponentModel;

namespace PredefinedFilterDemo.Controllers.FilterCollections;

public enum SemesterCourseFilterType
{
    [ParameterPattern(Pattern = "")]
    HasExam
}

public class SemesterCourseFilterCollection : IFilterCollection<SemesterCourse>
{
    private SemesterCourseFilterCollection() { }

    public static IEnumerable<FilterDescriptorDto> FilterDescriptors => FilterDescriptorHelper.GetDescriptorDtos<SemesterCourseFilterType>();

    private List<BaseFilter<SemesterCourse>> _filters = [];
    public IEnumerable<BaseFilter<SemesterCourse>> FilterCollection => _filters;

    public static IFilterCollection<SemesterCourse> Parse(string[]? filters, params object?[]? args)
    {
        var collection = new SemesterCourseFilterCollection();
        if (filters == null)
            return collection;

        foreach (var filterStr in filters)
        {
            var parts = filterStr.Split('|');
            if (!Enum.TryParse<SemesterCourseFilterType>(parts[0], true, out var type))
                continue;

            switch (type)
            {
                case SemesterCourseFilterType.HasExam:
                    collection._filters.Add(NullFilter<SemesterCourse>.NotNull(s => s.Exam));
                    //collection._filters.Add(DateTimeFilter<Student>.FromTo(filterStr, s => s.GraduationTime!.Value));
                    break;
            }
        }

        return collection;
    }
}
