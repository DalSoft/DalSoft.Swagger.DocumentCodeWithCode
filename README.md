# Swagger Document Code With Code

This works with @domaindrivendev [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

## Why?
I created this so that you can embellish your Web API Swagger documentation using code via an OperationFilter. I prefer doing this for a couple of reasons:
* I don't pollute the API code with XML comments
* If required I can separate the documentation from the code 
* I can provide detailed and contextual request or response examples
* I can workaround providing multiple request or response examples (https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/228
and https://github.com/domaindrivendev/Swashbuckle/issues/397)

Update 23 Jan 2017: using Response.Examples as outlined in [228](https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/228) now works as expected in Swagger UI, however you are still constrained by only one example per status code / per MIME type. I'll update the code to reflect this and avoid polluting swagger.json where possible. 

## Alternatives
If you don't mind adding attributes to you controllers actions and don't need multiple response examples for the same status code - then I'd recommend Matt Frear's [Swashbuckle.AspNetCore.Examples](https://github.com/mattfrear/Swashbuckle.AspNetCore.Examples).

## Getting Started 
Install the [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) package and configure as normal.

Install the DalSoft.Swagger.DocumentCodeWithCode package using NuGet.
```dos
PM> Install-Package DalSoft.Swagger.DocumentCodeWithCode
```
Modify startup.cs to include the OperationFilter.
```cs
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddSwaggerGen(options =>
    {
        ...
        options.OperationFilter<DocumentCodeWithCode>(typeof(Startup).GetTypeInfo().Assembly);
    });
}
```

## Providing Documenation using Code

The first thing you need to do is find the the generated OperationId for the API method you want to document, this easily found using Swagger UI. Click on the method you want document and use the OperationId which is the last part of the URL.

![OperationId](http://www.dalsoft.co.uk/blog/wp-content/uploads/2017/01/swaggerOperationId.png)

Now for this example create a class called addPet.cs anywhere in your solution, I usually create a folder called SwaggerDocs.

Inherit the BaseDocumentOperation and it's abstract ctor. Now you can you document by setting the properties in the ctor.
```cs
public class addPet : BaseDocumentOperation
{
    public addPet(Operation operation, ISchemaRegistry schemaRegistry) : base(operation, schemaRegistry)
    {
        operation.Summary = "Add a new pet to the store";
        operation.Description = "Pet object that needs to be added to the store";
    }
}
```

## Providing Examples using Code
Use the the extension AddExampleToParameter and AddOrUpdate to provide request and response examples.

```cs
public class addPet : BaseDocumentOperation
{
    public addPet(Operation operation, ISchemaRegistry schemaRegistry) : base(operation, schemaRegistry)
    {
        var exampleRequest = (BodyParameter) operation.Parameters.Single(_ => _.Name == "body");
        
        exampleRequest.AddExampleToParameter(schemaRegistry, operation.OperationId, 
        new Pet { Id = 1, Name = "doggie", Status = "available" });
        
        var exampleResponse = new Pet[] { new Pet { Id = 1, Name = "doggie", Status = "available" } };
        
        operation.Responses.AddOrUpdate(schemaRegistry, operation.OperationId, "200", new Response
        {
            Description = "success"
        }, example:exampleResponse);
    }
}
```

## Providing Multiple Example using Code
Use the allowMultipleExamples parameter for AddExampleToParameter and AddOrUpdate to provide multiple examples. Multiple examples are useful for shared resources such as errors.
```cs
public class addPet : BaseDocumentOperation
{
    public addPet(Operation operation, ISchemaRegistry schemaRegistry) : base(operation, schemaRegistry)
    {
        var loginFailed = new Error { Id="loginFailed"  Description = "Login has Failed" };
            
        operation.Responses.AddOrUpdate(schemaRegistry, operation.OperationId, "401", 
        new Response { Description = "Login Failed" }, example:loginFailed, allowMultipleExamples:true);
        
        var validationFailed = new Error { Id="validationFailed"  Description = "Validation Failed" };
        
        operation.Responses.AddOrUpdate(schemaRegistry, operation.OperationId, "400",
        new Response { Description = "Validation Failed"}, example:validationFailed, allowMultipleExamples:true);
    }
}
```

## Custom Serialization for your examples
If you use custom serialization you can ensure that your examples use same the serialization by using the customJsonSerializerSettings parameter.
```cs
public class addPet : BaseDocumentOperation
{
    public addPet(Operation operation, ISchemaRegistry schemaRegistry) : base(operation, schemaRegistry)
    {
        var exampleResponse = new Pet[] { new Pet { Id = 1, Name = "doggie", Status = "available" } };
        
        operation.Responses.AddOrUpdate(schemaRegistry, operation.OperationId, "200", new Response
        {
            Description = "success"
        }, example:exampleResponse, 
        customJsonSerializerSettings:new JsonSerializerSettings 
        { 
            ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() } 
        });
    }
}
```

## Store your documentation in a separate project
Just move your Swagger documentation classes to the project you want to use and pass the project assembly to the OperationFilter.
```cs
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddSwaggerGen(options =>
    {
        ...
        options.OperationFilter<DocumentCodeWithCode>(typeof(AnyClassContainingTheDocumentation).GetTypeInfo().Assembly);
    });
}
```
