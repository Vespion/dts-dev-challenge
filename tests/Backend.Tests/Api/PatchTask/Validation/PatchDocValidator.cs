using System.Linq.Expressions;
using DtsDevChallenge.Backend.Utils;
using DtsDevChallenge.Common;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

namespace DtsDevChallenge.Backend.Tests.Api.PatchTask.Validation;

public class PatchDocValidator
    {
        internal static async Task AssertEditingIdResults(IDictionary<string, string[]> errors)
        {
            await Assert.That(errors).Count().IsEqualTo(1);
            await Assert.That(errors.Keys.Single()).IsEqualTo("invalid-operation-type");

            var errorList = errors["invalid-operation-type"];
            await Assert.That(errorList.Count).IsEqualTo(3);
            
            await Assert.That(errorList).Contains("The 'Add' operation is not supported at '/Id'");
            await Assert.That(errorList).Contains("The 'Replace' operation is not supported at '/Id'");
            await Assert.That(errorList).Contains("The 'Remove' operation is not supported at '/Id'");
        }
        
        internal static async Task AssertRejectDeletionResults(IDictionary<string, string[]> errors, string path)
        {
            await Assert.That(errors).Count().IsEqualTo(1);
            await Assert.That(errors.Keys.Single()).IsEqualTo("invalid-operation-type");

            var errorList = errors["invalid-operation-type"];
            await Assert.That(errorList.Count).IsEqualTo(2);
            
            await Assert.That(errorList).Contains($"The 'Add' operation is not supported at '/{path}'");
            await Assert.That(errorList).Contains($"The 'Remove' operation is not supported at '/{path}'");
        }
        
        [Test]
        public Task RejectsEditingId()
        {
            var doc = new JsonPatchDocument<TaskItem>();
            doc.Add(t => t.Id, 1);
            doc.Replace(t => t.Id, 2);
            doc.Remove(t => t.Id);
            
            var errors = doc.ValidatePatchDocument();
            
            return AssertEditingIdResults(errors);
        }

        [Test]
        public Task RejectsDeletingTitleAllowingEdits()
        {
            return RejectDeletion(
                "Title",
                t => t.Title,
                "New Title"
            );
        }
        
        [Test]
        public Task RejectsDeletingDueByAllowingEdits()
        {
            return RejectDeletion(
                "DueBy",
                t => t.DueBy,
                DateTimeOffset.Now
            );
        }
        
        [Test]
        public Task RejectsDeletingItemStatusAllowingEdits()
        {
            return RejectDeletion(
                "ItemStatus",
                t => t.ItemStatus,
                TaskItemStatus.Closed
            );
        }
        
        [Test]
        public async Task AllowsEditingDescription()
        {
            var doc = new JsonPatchDocument<TaskItem>();
            doc.Add(x => x.Description, "blah blah blah");
            doc.Replace(x => x.Description, "blah2 blah2 blah2");
            doc.Remove(x => x.Description);
            
            var errors = doc.ValidatePatchDocument();
            
            await Assert.That(errors).Count().IsZero();
        }
        
        private static Task RejectDeletion<TProp>(
            string path,
            Expression<Func<TaskItem, TProp>> prop,
            TProp value
        )
        {
            var doc = new JsonPatchDocument<TaskItem>();
            doc.Add(prop, value);
            doc.Remove(prop);
            doc.Replace(prop, value);
            
            var errors = doc.ValidatePatchDocument();

            return AssertRejectDeletionResults(errors, path);
        }
    }