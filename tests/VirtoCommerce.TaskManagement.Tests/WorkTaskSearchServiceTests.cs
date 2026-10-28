using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Data.Models;
using VirtoCommerce.TaskManagement.Data.Repositories;
using VirtoCommerce.TaskManagement.Data.Services;
using Xunit;

namespace VirtoCommerce.TaskManagement.Tests;

[Trait("Category", "Unit")]
public class WorkTaskSearchServiceTests
{
    private static readonly WorkTaskEntity[] _tasks =
    [
        new() { Id = "customer-1-normal", ObjectType = "Customer", ObjectId = "customer-1", Priority = nameof(TaskPriority.Normal) },
        new() { Id = "customer-2-low", ObjectType = "Customer", ObjectId = "customer-2", Priority = nameof(TaskPriority.Low) },
        new() { Id = "order-high", ObjectType = "Order", ObjectId = "customer-1", Priority = nameof(TaskPriority.High) },
        new() { Id = "unlinked-high", Priority = nameof(TaskPriority.High) },
    ];

    [Fact]
    public void LinkedObjectType_MatchesTheTasksLinkedToThatType()
    {
        var ids = Search(new WorkTaskSearchCriteria { LinkedObjectType = "Customer" });

        Assert.Equal(new[] { "customer-1-normal", "customer-2-low" }, ids);
    }

    [Theory]
    [InlineData(new[] { "customer-2" }, new[] { "customer-2-low" })]
    [InlineData(new[] { "customer-1", "customer-2" }, new[] { "customer-1-normal", "customer-2-low", "order-high" })]
    public void LinkedObjectIds_MatchTheTasksLinkedToThoseRecords(string[] linkedObjectIds, string[] expected)
    {
        var ids = Search(new WorkTaskSearchCriteria { LinkedObjectIds = linkedObjectIds });

        Assert.Equal(expected, ids);
    }

    // The same id under another type is another record: an order that happens to share a customer's id.
    [Fact]
    public void LinkedObjectTypeAndIds_MatchTogether()
    {
        var ids = Search(new WorkTaskSearchCriteria { LinkedObjectType = "Customer", LinkedObjectIds = ["customer-1"] });

        Assert.Equal(new[] { "customer-1-normal" }, ids);
    }

    // ObjectIds (from SearchCriteriaBase) are the tasks' own ids; a task id is not a linked record.
    [Fact]
    public void LinkedObjectIds_AreNotTaskIds()
    {
        var ids = Search(new WorkTaskSearchCriteria { LinkedObjectIds = ["customer-1-normal"] });

        Assert.Empty(ids);
    }

    [Theory]
    [InlineData(new[] { "High" }, new[] { "order-high", "unlinked-high" })]
    [InlineData(new[] { "Low", "Normal" }, new[] { "customer-1-normal", "customer-2-low" })]
    public void Priorities_MatchAnyOfThem(string[] priorities, string[] expected)
    {
        var ids = Search(new WorkTaskSearchCriteria { Priorities = priorities });

        Assert.Equal(expected, ids);
    }

    // Priority is a list of one, as Status is to Statuses in the order search.
    [Fact]
    public void Priority_IsAListOfOne()
    {
        var criteria = new WorkTaskSearchCriteria { Priority = nameof(TaskPriority.High) };

        Assert.Equal(new[] { "High" }, criteria.Priorities);
        Assert.Equal(new[] { "order-high", "unlinked-high" }, Search(criteria));
    }

    // A list set on its own wins over the single value, as ObjectTypes does over ObjectType.
    [Fact]
    public void Priorities_TakePrecedenceOverPriority()
    {
        var ids = Search(new WorkTaskSearchCriteria { Priority = nameof(TaskPriority.Low), Priorities = ["High"] });

        Assert.Equal(new[] { "order-high", "unlinked-high" }, ids);
    }

    [Fact]
    public void Priority_Empty_DoesNotFilter()
    {
        var ids = Search(new WorkTaskSearchCriteria { Priority = string.Empty });

        Assert.Equal(_tasks.Length, ids.Length);
    }

    [Fact]
    public void EmptyLists_DoNotFilter()
    {
        var ids = Search(new WorkTaskSearchCriteria { LinkedObjectIds = [], Priorities = [] });

        Assert.Equal(_tasks.Length, ids.Length);
    }

    private static string[] Search(WorkTaskSearchCriteria criteria)
    {
        return new TestWorkTaskSearchService()
            .Query(_tasks, criteria)
            .Select(x => x.Id)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();
    }

    // Exposes BuildQuery, the filtering under test, over tasks held in memory.
    private sealed class TestWorkTaskSearchService()
        : WorkTaskSearchService(repositoryFactory: null, platformMemoryCache: null, crudService: null, Options.Create(new CrudOptions()))
    {
        public IQueryable<WorkTaskEntity> Query(IEnumerable<WorkTaskEntity> tasks, WorkTaskSearchCriteria criteria)
        {
            return BuildQuery(new InMemoryWorkTaskRepository(tasks.AsQueryable()), criteria);
        }
    }

    private sealed class InMemoryWorkTaskRepository(IQueryable<WorkTaskEntity> workTasks) : IWorkTaskRepository
    {
        public IQueryable<WorkTaskEntity> WorkTasks => workTasks;

        public IQueryable<WorkTaskAttachmentEntity> WorkTaskAttachments => throw new NotSupportedException();

        public IUnitOfWork UnitOfWork => throw new NotSupportedException();

        public Task<IList<WorkTaskEntity>> GetWorkTaskByIds(IList<string> ids, string responseGroup = null) => throw new NotSupportedException();

        public void Attach<T>(T item) where T : class => throw new NotSupportedException();

        public void Add<T>(T item) where T : class => throw new NotSupportedException();

        public void Update<T>(T item) where T : class => throw new NotSupportedException();

        public void Remove<T>(T item) where T : class => throw new NotSupportedException();

        public void Dispose()
        {
            // Nothing to release: the tasks live in memory.
        }
    }
}
