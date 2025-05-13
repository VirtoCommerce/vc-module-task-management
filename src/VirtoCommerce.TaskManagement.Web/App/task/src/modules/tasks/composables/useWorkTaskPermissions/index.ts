import { usePermissions, useUser } from "@vc-shell/framework";

interface IUseWorkTaskTypes {
  checkWorkTaskPermission(permissions: string | string[]): boolean;
}

export default (): IUseWorkTaskTypes => {
  const { hasAccess } = usePermissions();
  const { user } = useUser();

  function checkWorkTaskPermission(permissions: string | string[]): boolean {
    return hasAccess(permissions) || user.value?.isAdministrator || false;
  }

  return {
    checkWorkTaskPermission,
  };
};
