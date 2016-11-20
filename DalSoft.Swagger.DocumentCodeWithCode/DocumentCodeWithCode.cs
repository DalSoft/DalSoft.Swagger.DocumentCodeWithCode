using System;
using System.Linq;
using System.Reflection;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;

namespace DalSoft.Swagger.DocumentCodeWithCode
{
    public class DocumentCodeWithCode : IOperationFilter
    {
        private readonly Assembly _assemblyThatContainsDocs;

        public DocumentCodeWithCode(Assembly assemblyThatContainsDocs)
        {
            _assemblyThatContainsDocs = assemblyThatContainsDocs;
        }
        
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var type = _assemblyThatContainsDocs.GetTypes().FirstOrDefault(_ => _.Name == operation.OperationId);

            if (type==null)
                return;

            Activator.CreateInstance(type, operation, context.SchemaRegistry);
        }
    }
}
