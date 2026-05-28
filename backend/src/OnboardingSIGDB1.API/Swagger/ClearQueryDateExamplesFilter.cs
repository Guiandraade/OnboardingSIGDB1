using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OnboardingSIGDB1.API.Swagger;

/// <summary>
/// Removes auto-generated date-time examples from query string parameters so that
/// filter fields appear empty in Swagger UI instead of pre-filled with a placeholder date.
/// </summary>
public class ClearQueryDateExamplesFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters is null)
            return;

        foreach (var parameter in operation.Parameters)
        {
            if (parameter.In == ParameterLocation.Query &&
                parameter.Schema?.Format == "date-time")
            {
                parameter.Schema.Example = null;
            }
        }
    }
}
