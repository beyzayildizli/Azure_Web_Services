namespace RestApi;

public static class ImageTags
{
    public static void RegisterImageTagsEndpoints(this IEndpointRouteBuilder routes)
    {
        var ImageTagsRoute = routes.MapGroup("/api");
        
        // ImageTagsRoute.MapGet("/GetImageTags/{imageFileLink}",  () => TypedResults.Ok());
    }
}
