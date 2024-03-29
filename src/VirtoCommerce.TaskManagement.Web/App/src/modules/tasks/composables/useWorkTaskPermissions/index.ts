import { usePermissions, useUser } from "@vc-shell/framework";

interface IUseWorkTaskTypes {
  checkWorkTaskPermission(permissions: string | string[]): boolean;
}

export default (): IUseWorkTaskTypes => {
  const { checkPermission } = usePermissions();
  const { user } = useUser();

  function checkWorkTaskPermission(permissions: string | string[]): boolean {
    return checkPermission(permissions) || user.value.isAdministrator;
  }

  return {
    checkWorkTaskPermission,
  };
};
