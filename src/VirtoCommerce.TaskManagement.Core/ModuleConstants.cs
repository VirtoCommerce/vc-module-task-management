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
                public const string Access = "TaskManagement:access";
                public const string Create = "TaskManagement:create";
                public const string Read = "TaskManagement:read";
                public const string Update = "TaskManagement:update";
                public const string Delete = "TaskManagement:delete";

                public static string[] AllPermissions { get; } =
                {
                    Access,
                    Create,
                    Read,
                    Update,
                    Delete,
                };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor TaskManagementEnabled { get; } = new SettingDescriptor
                {
                    Name = "TaskManagement.TaskManagementEnabled",
                    GroupName = "TaskManagement|General",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false,
                };

                public static SettingDescriptor TaskManagementPassword { get; } = new SettingDescriptor
                {
                    Name = "TaskManagement.TaskManagementPassword",
                    GroupName = "TaskManagement|Advanced",
                    ValueType = SettingValueType.SecureString,
                    DefaultValue = "qwerty",
                };

                public static IEnumerable<SettingDescriptor> AllGeneralSettings
                {
                    get
                    {
                        yield return TaskManagementEnabled;
                        yield return TaskManagementPassword;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings
            {
                get
                {
                    return General.AllGeneralSettings;
                }
            }
        }
    }
}
