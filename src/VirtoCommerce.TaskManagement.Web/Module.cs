using System;
using System.IO;
using System.Linq;
using AutoMapper;
using GraphQL.Server;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.TemplateLoader.FileSystem;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.TaskManagement.Core;
using VirtoCommerce.TaskManagement.Core.Events;
using VirtoCommerce.TaskManagement.Core.Notifications;
using VirtoCommerce.TaskManagement.Core.Services;
using VirtoCommerce.TaskManagement.Data.Authorization;
using VirtoCommerce.TaskManagement.Data.Handlers;
using VirtoCommerce.TaskManagement.Data.Repositories;
using VirtoCommerce.TaskManagement.Data.Services;
using VirtoCommerce.TaskManagement.ExperienceApi;
using VirtoCommerce.TaskManagement.ExperienceApi.Authorization;
using VirtoCommerce.TaskManagement.Web.Authorization;

namespace VirtoCommerce.TaskManagement.Web
{
    public class Module : IModule, IHasConfiguration
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public IConfiguration Configuration { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            // Initialize database
            var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ??
                                   Configuration.GetConnectionString("VirtoCommerce");

            serviceCollection.AddDbContext<TaskManagementDbContext>(options => options.UseSqlServer(connectionString));

            // Override models
            //AbstractTypeFactory<OriginalModel>.OverrideType<OriginalModel, ExtendedModel>().MapToType<ExtendedEntity>();
            //AbstractTypeFactory<OriginalEntity>.OverrideType<OriginalEntity, ExtendedEntity>();

            // Register services
            serviceCollection.AddTransient<IWorkTaskRepository, WorkTaskRepository>();
            serviceCollection.AddTransient<Func<IWorkTaskRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IWorkTaskRepository>());
            serviceCollection.AddTransient<IWorkTaskService, WorkTaskService>();
            serviceCollection.AddTransient<IWorkTaskSearchService, WorkTaskSearchService>();
            serviceCollection.AddTransient<Func<IWorkTaskSearchService>>(provider => provider.GetRequiredService<IWorkTaskSearchService>);

            serviceCollection.AddTransient<SendNotificationsWorkTaskChangedEventHandler>();
            serviceCollection.AddTransient<IAuthorizationHandler, TaskAssignAuthorizationHandler>();

            // GraphQL
            var assemblyMarker = typeof(AssemblyMarker);
            var graphQlBuilder = new CustomGraphQLBuilder(serviceCollection);
            graphQlBuilder.AddGraphTypes(assemblyMarker);
            serviceCollection.AddMediatR(assemblyMarker);
            serviceCollection.AddAutoMapper(assemblyMarker);
            serviceCollection.AddSchemaBuilders(assemblyMarker);
            serviceCollection.AddSingleton<IAuthorizationHandler, WorkTaskAuthorizationHandler>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var serviceProvider = appBuilder.ApplicationServices;

            // Register settings
            var settingsRegistrar = serviceProvider.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.General.AllSettings, ModuleInfo.Id);

            // Register permissions
            var permissionsRegistrar = serviceProvider.GetRequiredService<IPermissionsRegistrar>();
            permissionsRegistrar.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions
                .Select(x => new Permission { ModuleId = ModuleInfo.Id, GroupName = "TaskManagement", Name = x })
                .ToArray());

            AbstractTypeFactory<PermissionScope>.RegisterType<TaskAssignToMyOrganizationScope>();
            AbstractTypeFactory<PermissionScope>.RegisterType<TaskAssignToAllScope>();

            permissionsRegistrar.WithAvailabeScopesForPermissions(new[] {
                ModuleConstants.Security.Permissions.Create,
                ModuleConstants.Security.Permissions.Update,
            }, new TaskAssignToMyOrganizationScope(), new TaskAssignToAllScope());

            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<WorkTaskChangedEvent>((message, token) => appBuilder.ApplicationServices.GetService<SendNotificationsWorkTaskChangedEventHandler>().Handle(message));

            // Apply migrations
            using var serviceScope = serviceProvider.CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
            dbContext.Database.Migrate();

            var notificationRegistrar = appBuilder.ApplicationServices.GetService<INotificationRegistrar>();
            var defaultTemplatesDirectory = Path.Combine(ModuleInfo.FullPhysicalPath, "NotificationTemplates");
            notificationRegistrar.RegisterNotification<WorkTaskAssignedEmailNotification>().WithTemplatesFromPath(defaultTemplatesDirectory);
        }

        public void Uninstall()
        {
        }
    }
}
