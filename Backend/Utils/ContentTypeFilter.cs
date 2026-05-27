namespace DtsDevChallenge.Backend.Utils;

/// <summary>
/// Filters an endpoint for the specific content type
/// </summary>
internal class ContentTypeFilter(string contentType, string errorTitle, string? errorDetail, int statusCode) : IEndpointFilter
{
    /// <inheritdoc />
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.HttpContext.Request.ContentType?.ToLowerInvariant() != contentType)
        {
            return ValueTask.FromResult<object?>(TypedResults.Problem(
                title: errorTitle,
                detail: errorDetail,
                statusCode: statusCode
            ));
        }

        return next(context);
    }
}