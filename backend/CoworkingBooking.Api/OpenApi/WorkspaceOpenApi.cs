using System.Text.Json;
using System.Text.Json.Nodes;
using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Core.Workspace.Enums;
using CoworkingBooking.Shared.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Slugify;

internal sealed class WorkspaceOpenApiSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type == typeof(CreateWorkspaceRequestDTO))
        {
            schema.Description = "Data required to create Workspace.";

            if(schema.Properties != null)
            {
                if (schema.Properties.TryGetValue("name", out var nameSchema) && nameSchema is OpenApiSchema name)
                {
                    name.Description = "Name of the workspace.";
                    name.Example = "Swan Generation";
                }

                if (schema.Properties.TryGetValue("description", out var descriptionSchema) && descriptionSchema is OpenApiSchema description)
                {
                    description.Description = "Description of the Workspace.";
                    description.Example = "O Swan Generation é um hub de conexões perfeito para o seu networking.";
                }

                if (schema.Properties.TryGetValue("type", out var typeSchema) && typeSchema is OpenApiSchema type)
                {
                    type.Description = "Type to Workspace.";
                    type.Example = WorkspaceType.PrivateOffice.ToString();
                }

                if (schema.Properties.TryGetValue("coordinates", out var coordinatesSchema) && coordinatesSchema is OpenApiSchema coordinates)
                {
                    coordinates.Description = "Coordinates to Workspace.";
                    coordinates.Example = JsonSerializer.SerializeToNode(
                        new Coordinates
                        {
                            lat = 32.098452,
                            lgn = 2.684427
                        }
                    );
                   
                }

                if (schema.Properties.TryGetValue("pricePerHour", out var pricePerHourSchema) && pricePerHourSchema is OpenApiSchema pricePerHour)
                {
                    pricePerHour.Description = "Price per Hour to Workspace.";
                    pricePerHour.Example = 56.76;
                }

                if(schema.Properties.TryGetValue("resources", out var resourcesSchema) && resourcesSchema is OpenApiSchema resources)
                {
                    resources.Description = "Resources to Workspace.";
                    resources.Example = new JsonArray
                    {
                        "TV",
                        "Wifi"
                    };
                }
            }
        }
        
        if (context.JsonTypeInfo.Type == typeof(CreateWorkspaceResponseDTO))
        {
            if(schema.Properties != null)
            {
                if (schema.Properties.TryGetValue("slug", out var slugSchema) && slugSchema is OpenApiSchema slug)
                {
                    slug.Example = new SlugHelper().GenerateSlug("Swan Generation");
                }

                if (schema.Properties.TryGetValue("name", out var nameSchema) && nameSchema is OpenApiSchema name)
                {
                    name.Example = "Swan Generation";
                }

                if (schema.Properties.TryGetValue("description", out var descriptionSchema) && descriptionSchema is OpenApiSchema description)
                {
                    description.Example = "O Swan Generation é um hub de conexões perfeito para o seu networking.";
                }

                if (schema.Properties.TryGetValue("type", out var typeSchema) && typeSchema is OpenApiSchema type)
                {
                    type.Example = WorkspaceType.PrivateOffice.ToString();
                }

                if (schema.Properties.TryGetValue("status", out var statusSchema) && statusSchema is OpenApiSchema status)
                {
                    status.Example = WorkspaceStatus.Draft.ToString();
                }

                if (schema.Properties.TryGetValue("coordinates", out var coordinatesSchema) && coordinatesSchema is OpenApiSchema coordinates)
                {
                    coordinates.Example = JsonSerializer.SerializeToNode(
                        new Coordinates
                        {
                            lat = 32.098452,
                            lgn = 2.684427
                        }
                    );
                }

                if(schema.Properties.TryGetValue("resources", out var resourcesSchema) && resourcesSchema is OpenApiSchema resources)
                {
                    resources.Description = "Resources to Workspace.";
                    resources.Example = new JsonArray
                    {
                        "TV",
                        "Wifi"
                    };
                }   
            }
        }

        return Task.CompletedTask;
    }
}

internal sealed class WorkspaceOpenApiOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        if (operation.Responses != null)
        {
            if (operation.Responses.TryGetValue("400", out var response))
            {
                response.Content.Remove("text/plain");
                
                response.Content["application/json"] = new OpenApiMediaType
                {
                    Example = "teste"
                };
            }

        }
        


        return Task.CompletedTask;
    }
}