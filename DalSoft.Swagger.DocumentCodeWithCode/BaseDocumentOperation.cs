using System.Collections.Generic;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;

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