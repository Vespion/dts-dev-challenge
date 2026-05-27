using DtsDevChallenge.Common;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;

namespace DtsDevChallenge.Backend.Utils;

internal static class FilterExtensions
{
    public static TBuilder Accepts<TBuilder>(this TBuilder builder, string contentType,
        string errorTitle = "Invalid content type", string? errorDetail = null,
        int statusCode = StatusCodes.Status415UnsupportedMediaType)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder
            .AddEndpointFilter(new ContentTypeFilter(contentType, errorTitle, errorDetail, statusCode))
            .ProducesProblem(StatusCodes.Status415UnsupportedMediaType);
    }

    internal static Dictionary<string, string[]> ValidatePatchDocument(this JsonPatchDocument<TaskItem> patchDoc)
    {
        const string invalidOperationTypeKey = "invalid-operation-type";
        const string invalidOperationValueKey = "invalid-operation-value";
        const string invalidPathKey = "invalid-path";

        var errors = new Dictionary<string, List<string>>
        {
            { invalidOperationTypeKey, [] },
            { invalidOperationValueKey, [] },
            { invalidPathKey, [] }
        };

        string[] validDeletionPaths =
        [
            "/Description"
        ];

        string[] validEditingPaths =
        [
            ..validDeletionPaths,
            "/Title",
            "/DueBy",
            "/ItemStatus"
        ];

        string[] validPaths =
        [
            "/Id",
            ..validEditingPaths
        ];

        foreach (var operation in patchDoc.Operations)
        {
            if (operation.path == null || !validPaths.Contains(operation.path))
            {
                errors[invalidPathKey].Add($"The path '{operation.path}' is not valid for this resource");
            }

            switch (operation.OperationType)
            {
                case OperationType.Add:
                case OperationType.Remove:
                    if (!validDeletionPaths.Contains(operation.path))
                    {
                        //Attempted to delete/add a property that must always be present
                        errors[invalidOperationTypeKey]
                            .Add($"The '{operation.OperationType}' operation is not supported at '{operation.path}'");
                    }

                    break;
                case OperationType.Replace:
                    if (!validEditingPaths.Contains(operation.path))
                    {
                        //Attempted to update a read-only property
                        errors[invalidOperationTypeKey]
                            .Add($"The '{operation.OperationType}' operation is not supported at '{operation.path}'");
                    }
                    else
                    {
                        if (!validDeletionPaths.Contains(operation.path))
                        {
                            //This is not a deletion path so the operation value MUST exist
                            if (operation.value == null)
                            {
                                errors[invalidOperationValueKey]
                                    .Add($"The '{operation.OperationType}' operation is not supported at '{operation.path}' with value '{operation.value}'");
                            }
                        }
                    }
                    break;
                case OperationType.Test:
                    //Test operations are read-only
                    break;
                case OperationType.Move:
                case OperationType.Copy:
                case OperationType.Invalid:
                default:
                    errors[invalidOperationTypeKey]
                        .Add($"The '{operation.OperationType}' operation is not supported at '{operation.path}'");
                    break;
            }
        }

        return errors
            .Where(kvp => kvp.Value.Count > 0)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
    }
}