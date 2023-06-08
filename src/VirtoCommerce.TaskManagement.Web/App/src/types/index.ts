import { ComputedRef } from "vue";
import { PushNotification, VcButton } from "@vc-shell/framework";

enum TaskPermissions {
  Access = "task:access",
  Create = "task:create",
  Read = "task:read",
  Update = "task:update",
  Delete = "task:delete",
  Finish = "task:finish",
  AttachmentManagement = "task:attachment:management",
}

interface IShippingInfo {
  label: string;
  name?: string;
  address?: string;
  phone?: string;
  email?: string;
}

interface IShippingInfo {
  label: string;
  name?: string;
  address?: string;
  phone?: string;
  email?: string;
}

interface INotificationActions {
  name: string | ComputedRef<string>;
  clickHandler(): void;
  outline: boolean;
  variant?: InstanceType<typeof VcButton>["$props"]["variant"];
  isVisible?: boolean | ComputedRef<boolean>;
  disabled?: boolean | ComputedRef<boolean>;
}

interface IProductPushNotification extends PushNotification {
  profileName?: string;
  newStatus?: string;
  productId?: string;
  productName?: string;
}

interface INewOrderPushNotification extends PushNotification {
  orderId?: string;
}

export type { IShippingInfo, INotificationActions, IProductPushNotification, INewOrderPushNotification };

export { TaskPermissions };
