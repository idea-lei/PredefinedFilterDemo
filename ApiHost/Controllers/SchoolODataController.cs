using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PredefinedFilterDemo.Controllers.FilterCollections;
using PredefinedFilterDemo.Data;
using PredefinedFilterDemo.Dtos;
using PredefinedFilterDemo.Dtos.School;

namespace PredefinedFilterDemo.Controllers;

[Route("odata/school")]
public class SchoolODataController(AppDbContext db) : ODataController
{
    [HttpGet("students-filters")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<FilterDescriptorDto>))]
    public IActionResult GetStudentsFilters()
    {
        return Ok(StudentFilterCollection.FilterDescriptors);
    }

    [HttpGet("students")]
    [EnableQuery]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Student>))]
    public IActionResult GetStudents([FromQuery] string[]? filters)
    {
        if(filters == null || filters.Length == 0)
            return Ok(db.Students);

        var filterCollection = StudentFilterCollection.Parse(filters);
        return Ok(filterCollection.Filter(db.Students));
    }
}
