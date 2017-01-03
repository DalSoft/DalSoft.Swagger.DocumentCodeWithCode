using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DalSoft.Swagger.DocumentCodeWithCode
{
    public abstract class BaseDocumentOperation
    {
        // ReSharper disable UnusedParameter.Local
        protected BaseDocumentOperation(Operation operation, ISchemaRegistry schemaRegistry)
        {
            operation.Parameters = operation.Parameters ?? new List<IParameter>();
        }
    }
}