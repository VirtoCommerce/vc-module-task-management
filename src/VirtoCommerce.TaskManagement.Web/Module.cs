using System;
using System.IO;
using System.Linq;
using GraphQL;
using GraphQL.MicrosoftDI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.TemplateLoader.FileSystem;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.TaskManagement.Core;
using VirtoCommerce.TaskManagement.Core.Events;
using VirtoCommerce.TaskManagement.Core.Notifications;
using VirtoCommerce.TaskManagement.Core.Services;
using VirtoCommerce.TaskManagement.Data.Authorization;
using VirtoCommerce.TaskManagement.Data.Handlers;
using VirtoCommerce.TaskManagement.Data.MySql;
using VirtoCommerce.TaskManagement.Data.PostgreSql;
using VirtoCommerce.TaskManagement.Data.Repositories;
using VirtoCommerce.TaskManagement.Data.Services;
using VirtoCommerce.TaskManagement.Data.SqlServer;
using VirtoCommerce.TaskManagement.ExperienceApi;
using VirtoCommerce.TaskManagement.ExperienceApi.Authorization;
using VirtoCommerce.TaskManagement.Web.Authorization;
using VirtoCommerce.Xapi.Core.Extensions;
using TaskPermissions = VirtoCommerce.TaskManagement.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.TaskManagement.Web
{
    public class Module : IModule, IHasConfiguration
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public IConfiguration Configuration { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            // Initialize database
            var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
            serviceCollection.AddDbContext<TaskManagementDbContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ?? Configuration.GetConnectionString("VirtoCommerce");

                switch (databaseProvider)
                {
                    case "MySql":
                        options.UseMySqlDatabase(connectionString);
                        break;
                    case "PostgreSql":
                        options.UsePostgreSqlDatabase(connectionString);
                        break;
                    default:
                        options.UseSqlServerDatabase(connectionString);
                        break;
                }
            });

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
            serviceCollection.AddTransient<IAuthorizationHandler, TaskAuthorizationHandler>();

            // GraphQL
            _ = new GraphQLBuilder(serviceCollection, builder =>
            {
                builder.AddSchema(serviceCollection, typeof(AssemblyMarker));
            });

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
            permissionsRegistrar.RegisterPermissions(TaskPermissions.AllPermissions
                .Select(x => new Permission { ModuleId = ModuleInfo.Id, GroupName = "TaskManagement", Name = x })
                .ToArray());

            AbstractTypeFactory<PermissionScope>.RegisterType<TaskToMyOrganizationScope>();
            AbstractTypeFactory<PermissionScope>.RegisterType<TaskToMeScope>();

            permissionsRegistrar.WithAvailabeScopesForPermissions(new[] {
                TaskPermissions.Read,
                TaskPermissions.Create,
                TaskPermissions.Update,
                TaskPermissions.Delete,
                TaskPermissions.Finish,
            }, new TaskToMyOrganizationScope(), new TaskToMeScope());

            appBuilder.RegisterEventHandler<WorkTaskChangedEvent, SendNotificationsWorkTaskChangedEventHandler>();

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
