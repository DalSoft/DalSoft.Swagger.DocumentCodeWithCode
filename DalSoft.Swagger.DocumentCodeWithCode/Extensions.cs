using System.Collections.Generic;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;

namespace DalSoft.Swagger.DocumentCodeWithCode
{
    public static class Extensions
    {
        public static void AddOrUpdate(this IDictionary<string, Response> responseDictionary, ISchemaRegistry schemaRegistry, string operationId, string statusCode, Response response, object example = null, bool allowMultipleExamples = false, JsonSerializerSettings customJsonSerializerSettings = null)
        {
            if (responseDictionary.ContainsKey(statusCode))
                responseDictionary[statusCode] = response;
            else
                responseDictionary.Add(statusCode, response);

            if (example!=null)
                responseDictionary[statusCode].Schema = schemaRegistry.AddExampleToSchemaDefinitions(operationId, example, statusCode, allowMultipleExamples, customJsonSerializerSettings);
        }

        public static void AddExampleToParameter(this BodyParameter parameter, ISchemaRegistry schemaRegistry, string operationId, object example, bool allowMultipleExamples = false, JsonSerializerSettings customJsonSerializerSettings = null)
        {
            if (example != null)
                parameter.Schema = schemaRegistry.AddExampleToSchemaDefinitions(operationId, example, allowMultipleExamples:allowMultipleExamples, customJsonSerializerSettings:customJsonSerializerSettings);
        }

        //TODO: https://github.com/domaindrivendev/Ahoy/issues/234
        //public static void AddDefaultToParameter(this BodyParameter parameter, ISchemaRegistry schemaRegistry, string operationId, object @default)
        //{
        //    if (@default != null)
        //        parameter.Schema = schemaRegistry.AddDefaultToSchemaDefinitions(operationId, @default);
        //}
        
        private static Schema AddExampleToSchemaDefinitions(this ISchemaRegistry schemaRegistry, string operationId, object example, string statusCode=null, bool allowMultipleExamples=false, JsonSerializerSettings customJsonSerializerSettings = null)
        {
            var type = example.GetType();
            schemaRegistry.GetOrRegister(type);

            var actualTypeName = type.Name.Replace("[]", string.Empty);
            var schema = schemaRegistry.Definitions[actualTypeName];
            if (!allowMultipleExamples)
            {
                schema.Example = example.UseCustomJsonSerializerSettings(customJsonSerializerSettings);
                return schema;
            }

            string exampleFakeTypeName;
                
            if (statusCode==null)
               exampleFakeTypeName = "examples<=" + actualTypeName + "<=" + operationId;
            else
               exampleFakeTypeName = "examples=>" + operationId + "=>" + statusCode + "=>" + actualTypeName;

            //Why? https://github.com/domaindrivendev/Ahoy/issues/228 and https://github.com/domaindrivendev/Swashbuckle/issues/397
            schemaRegistry.Definitions.Add(exampleFakeTypeName, new Schema
            {
                Ref = "#/definitions/" + exampleFakeTypeName,
                Example = example.UseCustomJsonSerializerSettings(customJsonSerializerSettings),
                Properties = schema?.Properties
            });

            return schemaRegistry.Definitions[exampleFakeTypeName];
        }

        private static object UseCustomJsonSerializerSettings(this object @object, JsonSerializerSettings customJsonSerializerSettings)
        {
            return customJsonSerializerSettings == null ? @object : JsonConvert.DeserializeObject(JsonConvert.SerializeObject(@object, customJsonSerializerSettings));
        }
    }
}
