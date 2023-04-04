using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.TaskManagement.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Access = "task:access";
                public const string Create = "task:create";
                public const string Read = "task:read";
                public const string Update = "task:update";
                public const string Delete = "task:delete";
                public const string Approve = "task:approve";
                public const string Decline = "task:decline";

                public static string[] AllPermissions { get; } =
                {
                    Access,
                    Create,
                    Read,
                    Update,
                    Delete,
                    Approve,
                    Decline,
                };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor TaskNotificationsEnabled { get; } = new SettingDescriptor
                {
                    Name = "TaskManagement.TaskNotificationsEnabled",
                    GroupName = "TaskManagement|Notification",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false,
                    RestartRequired = true
                };

                public static SettingDescriptor TaskNotificationNoReplyEmail { get; } = new SettingDescriptor
                {
                    Name = "TaskManagement.TaskNotificationNoReplyEmail",
                    GroupName = "TaskManagement|Notification",
                    ValueType = SettingValueType.ShortText,
                };

                public static SettingDescriptor TaskAppBaseUrl { get; } = new SettingDescriptor
                {
                    Name = "TaskManagement.TaskAppUrl",
                    GroupName = "TaskManagement|Notification",
                    ValueType = SettingValueType.ShortText,
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return TaskNotificationsEnabled;
                        yield return TaskNotificationNoReplyEmail;
                        yield return TaskAppBaseUrl;
                    }
                }
            }
        }
    }
}
