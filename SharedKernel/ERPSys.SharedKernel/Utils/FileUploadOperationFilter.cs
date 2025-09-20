using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ERPSys.SharedKernel.Utils
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParam = context.MethodInfo
                .GetParameters()
                .FirstOrDefault(p => p.ParameterType == typeof(IFormFile));

            if (fileParam == null)
                return;

            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties =
                        {
                            [fileParam.Name] = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary",
                                Description = "Excel file to upload"
                            }
                        },
                        Required = new HashSet<string> { fileParam.Name }
                    }
                }
            }
            };
        }
    }
}
