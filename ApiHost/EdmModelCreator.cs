using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using PredefinedFilterDemo.Dtos.School;

namespace PredefinedFilterDemo
{
    public static class EdmModelCreator
    {
        public static IEdmModel Create()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Student>("student");

            return builder.GetEdmModel();
        }
    }
}
