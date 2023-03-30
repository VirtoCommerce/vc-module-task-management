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
            public static class PredefinedTaskTypes
            {
                public const string RegistrationReview = "Registration Review";
                public const string OrderReview = "Order Review";
                public const string OrderProcessing = "Order Processing";
                public const string ProductCatalogManagement = "Product Catalog Management";
                public const string PricingAndPromotions = "Pricing and Promotions";
                public const string ContentManagement = "Content Management";
                public const string CustomerSupport = "Customer Support";
                public const string Other = "Other";
            }

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

                public static SettingDescriptor TaskTypes { get; } = new SettingDescriptor
                {
                    Name = "TaskManagement.TaskTypes",
                    GroupName = "TaskManagement|General",
                    ValueType = SettingValueType.ShortText,
                    IsDictionary = true,
                    DefaultValue = PredefinedTaskTypes.RegistrationReview,
                    AllowedValues = new[]
                    {
                        PredefinedTaskTypes.RegistrationReview,
                        PredefinedTaskTypes.OrderReview,
                        PredefinedTaskTypes.OrderProcessing,
                        PredefinedTaskTypes.ProductCatalogManagement,
                        PredefinedTaskTypes.PricingAndPromotions,
                        PredefinedTaskTypes.ContentManagement,
                        PredefinedTaskTypes.CustomerSupport,
                        PredefinedTaskTypes.Other
                    }
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return TaskNotificationsEnabled;
                        yield return TaskNotificationNoReplyEmail;
                        yield return TaskAppBaseUrl;
                        yield return TaskTypes;
                    }
                }
            }
        }
    }
}
