using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Jobs;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.TaskManagement.Core;
using VirtoCommerce.TaskManagement.Core.Events;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Data.Handlers;
using VirtoCommerce.TaskManagement.Data.Jobs;
using Xunit;

namespace VirtoCommerce.TaskManagement.Tests;

// Any other test class that enqueues through the static BackgroundJob facade must join this collection:
// the facade has no reset API (Initialize rejects null), so Dispose leaves a DISPOSED provider behind in
// the static, and a class racing this one would see ObjectDisposedException from it.
[Collection(nameof(SendNotificationsWorkTaskChangedEventHandlerTests))]
[Trait("Category", "Unit")]
public class SendNotificationsWorkTaskChangedEventHandlerTests
{
    // Mirrors JobJsonSettings.Default of the BackgroundJobs module: no TypeNameHandling, which is precisely what
    // the payload has to survive.
    private static readonly JsonSerializerSettings JobSerializerSettings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        NullValueHandling = NullValueHandling.Include,
    };

    [Fact]
    public async Task Handle_NotificationsEnabled_EnqueuesAssignedTasksOnly()
    {
        //Arrange
        using var capture = new EnqueueCapture();
        var handler = CreateHandler(enabled: true);

        var message = new WorkTaskChangedEvent(
        [
            // Added with a responsible person - notified
            new GenericChangedEntry<WorkTask>(new WorkTask { Id = "task1", ResponsibleId = "member1" }, EntryState.Added),
            // Added without a responsible person - skipped
            new GenericChangedEntry<WorkTask>(new WorkTask { Id = "task2" }, EntryState.Added),
            // Reassigned - notified
            new GenericChangedEntry<WorkTask>(
                new WorkTask { Id = "task3", ResponsibleId = "member2" },
                new WorkTask { Id = "task3", ResponsibleId = "member1" },
                EntryState.Modified),
            // Modified, same responsible person - skipped
            new GenericChangedEntry<WorkTask>(
                new WorkTask { Id = "task4", ResponsibleId = "member1" },
                new WorkTask { Id = "task4", ResponsibleId = "member1" },
                EntryState.Modified),
        ]);

        //Act
        await handler.Handle(message);

        //Assert
        Assert.Equal(1, capture.EnqueueCount);
        var payload = Assert.IsType<SendWorkTaskNotificationsJobPayload>(capture.Payload);
        Assert.Collection(payload.WorkTasks,
            x => Assert.Equal("task1", x.Id),
            x => Assert.Equal("task3", x.Id));
    }

    [Fact]
    public async Task Handle_NotificationsDisabled_EnqueuesNothing()
    {
        //Arrange
        using var capture = new EnqueueCapture();
        var handler = CreateHandler(enabled: false);

        var message = new WorkTaskChangedEvent(
        [
            new GenericChangedEntry<WorkTask>(new WorkTask { Id = "task1", ResponsibleId = "member1" }, EntryState.Added),
        ]);

        //Act
        await handler.Handle(message);

        //Assert
        Assert.Equal(0, capture.EnqueueCount);
    }

    [Fact]
    public async Task Handle_NothingAssigned_EnqueuesNothing()
    {
        //Arrange
        using var capture = new EnqueueCapture();
        var handler = CreateHandler(enabled: true);

        var message = new WorkTaskChangedEvent(
        [
            new GenericChangedEntry<WorkTask>(new WorkTask { Id = "task1" }, EntryState.Added),
        ]);

        //Act
        await handler.Handle(message);

        //Assert
        Assert.Equal(0, capture.EnqueueCount);
    }

    [Fact]
    public void Payload_SurvivesJobSerialization()
    {
        //Arrange
        var payload = new SendWorkTaskNotificationsJobPayload
        {
            WorkTasks =
            [
                new WorkTask
                {
                    Id = "task1",
                    Name = "Task 1",
                    ResponsibleId = "member1",
                    Parameters = JObject.Parse("""{ "key": "value" }"""),
                    Attachments = [new WorkTaskAttachment { Id = "attachment1", Name = "file.txt" }],
                },
            ],
        };

        //Act
        var json = JsonConvert.SerializeObject(payload, JobSerializerSettings);
        var restored = JsonConvert.DeserializeObject<SendWorkTaskNotificationsJobPayload>(json, JobSerializerSettings);

        //Assert
        var task = Assert.Single(restored.WorkTasks);
        Assert.Equal("task1", task.Id);
        Assert.Equal("Task 1", task.Name);
        Assert.Equal("member1", task.ResponsibleId);
        Assert.Equal("value", task.Parameters["key"]?.ToString());
        Assert.Equal("attachment1", Assert.Single(task.Attachments).Id);
    }

    [Fact]
    public async Task JobHandler_DelegatesToTheEventHandler()
    {
        //Arrange
        var eventHandlerMock = CreateEventHandlerMock();
        var tasks = new List<WorkTask> { new() { Id = "task1", ResponsibleId = "member1" } };

        eventHandlerMock
            .Setup(x => x.SendNotificationsAsync(tasks))
            .Returns(Task.CompletedTask);

        var handler = new SendWorkTaskNotificationsJobHandler(eventHandlerMock.Object);

        //Act
        await handler.Execute(new SendWorkTaskNotificationsJobPayload { WorkTasks = tasks }, context: null,
            TestContext.Current.CancellationToken);

        //Assert
        eventHandlerMock.Verify(x => x.SendNotificationsAsync(tasks), Times.Once);
    }

    [Fact]
    public async Task LegacyMethod_ForwardsToTheNewPath()
    {
        //Arrange
        // Background jobs enqueued through Hangfire by an earlier version still reference the legacy method by name.
        var eventHandlerMock = CreateEventHandlerMock();
        var tasks = new List<WorkTask> { new() { Id = "task1", ResponsibleId = "member1" } };

        eventHandlerMock
            .Setup(x => x.SendNotificationsAsync(tasks))
            .Returns(Task.CompletedTask);

        //Act
#pragma warning disable VC0015 // Exercising the legacy entry point on purpose
        await eventHandlerMock.Object.TryToSendOrderNotificationsAsync(tasks);
#pragma warning restore VC0015

        //Assert
        eventHandlerMock.Verify(x => x.SendNotificationsAsync(tasks), Times.Once);
    }

    private static SendNotificationsWorkTaskChangedEventHandler CreateHandler(bool enabled)
    {
        return new SendNotificationsWorkTaskChangedEventHandler(
            Mock.Of<IMemberService>(),
            Mock.Of<INotificationSender>(),
            CreateSettingsManager(enabled),
            Mock.Of<INotificationSearchService>());
    }

    private static Mock<SendNotificationsWorkTaskChangedEventHandler> CreateEventHandlerMock()
    {
        return new Mock<SendNotificationsWorkTaskChangedEventHandler>(
            Mock.Of<IMemberService>(),
            Mock.Of<INotificationSender>(),
            CreateSettingsManager(enabled: true),
            Mock.Of<INotificationSearchService>())
        {
            CallBase = true,
        };
    }

    private static ISettingsManager CreateSettingsManager(bool enabled)
    {
        var settingsManager = new Mock<ISettingsManager>();
        settingsManager
            .Setup(x => x.GetObjectSettingAsync(ModuleConstants.Settings.General.TaskNotificationsEnabled.Name, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new ObjectSettingEntry { Value = enabled });

        return settingsManager.Object;
    }

    // Captures what the handler enqueued through the static BackgroundJob facade. IBackgroundJob is registered
    // Scoped here exactly as the engine module registers it, so this also proves the facade's per-call scope
    // resolves it - the event handler itself is root-resolved and must never hold it.
    private sealed class EnqueueCapture : IDisposable
    {
        private readonly ServiceProvider _provider;

        public EnqueueCapture()
        {
            BackgroundJobMock
                .Setup(x => x.Enqueue<SendWorkTaskNotificationsJobHandler>(It.IsAny<object>(), It.IsAny<EnqueueOptions>(), It.IsAny<CancellationToken>()))
                .Callback<object, EnqueueOptions, CancellationToken>((payload, _, _) =>
                {
                    Payload = payload;
                    EnqueueCount++;
                })
                .ReturnsAsync("job-id");

            var services = new ServiceCollection();
            services.AddScoped(_ => BackgroundJobMock.Object);
            _provider = services.BuildServiceProvider(validateScopes: true);

            BackgroundJob.Initialize(_provider);
        }

        public Mock<IBackgroundJob> BackgroundJobMock { get; } = new();

        public object Payload { get; private set; }

        public int EnqueueCount { get; private set; }

        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}
