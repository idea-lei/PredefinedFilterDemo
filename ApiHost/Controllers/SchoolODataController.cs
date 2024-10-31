using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PredefinedFilterDemo.Controllers.FilterCollections;
using PredefinedFilterDemo.Data;
using PredefinedFilterDemo.Dtos.School;

namespace PredefinedFilterDemo.Controllers;

[Route("odata/school")]
public class SchoolODataController(AppDbContext db) : ODataController
{
    [HttpGet("students")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Student>))]
    public IActionResult GetStudents([FromQuery] string[]? filters)
    {
        if(filters == null || filters.Length == 0)
            return NoContent();

        var filterCollection = StudentFilterCollection.Parse(filters);
        return Ok(filterCollection.Filter(db.Students));
    }
}
