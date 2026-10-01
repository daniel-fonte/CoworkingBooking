namespace CoworkingBooking.Api.DependencyInjection
{
    public static class DocumentationService
    {
        public static IServiceCollection AddDocumentationService(
            this IServiceCollection services
        )
        {
            services.AddOpenApi(options => {
                options.AddSchemaTransformer(new WorkspaceOpenApiSchemaTransformer());
                options.AddOperationTransformer(new WorkspaceOpenApiOperationTransformer());
            });

            return services;
        }
    }
}