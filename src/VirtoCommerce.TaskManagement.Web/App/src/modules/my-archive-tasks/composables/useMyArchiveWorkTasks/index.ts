import {
  useWorkTasksList,
  IUseBaseWorkTasksList,
  UseBaseWorkTasksListOptions,
} from "../../../tasks/composables/useWorkTasks";

export interface UseMyArchiveWorkTasksOptions extends UseBaseWorkTasksListOptions {}

export function useMyArchiveWorkTasks(options?: UseMyArchiveWorkTasksOptions): IUseBaseWorkTasksList {
  return useWorkTasksList({
    pageSize: options?.pageSize || 20,
    sort: options?.sort || "modifiedDate:desc",
    defaultFilters: {
      isActive: false,
      onlyAssignedToMe: true,
    },
  });
}
